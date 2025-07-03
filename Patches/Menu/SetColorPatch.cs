using HarmonyLib;
using iiMenu.Classes;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "InitializeNoobMaterial")]
    public class InitializeNoobMaterial
    {
        public static bool Prefix(VRRig __instance, float red, float green, float blue, PhotonMessageInfoWrapped info)
        {
            NetPlayer player = RigManager.GetPlayerFromVRRig(__instance) ?? null;
            if (player != null && Menu.Main.ShouldBypassChecks(player))
            {
                if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(__instance.rigSerializer.gameObject))
                    __instance.InitializeNoobMaterialLocal(red, green, blue);

                return false;
            }

            return true;
        }
    }
}
