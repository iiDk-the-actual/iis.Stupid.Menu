using HarmonyLib;
using iiMenu.Classes;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "SetNameTagText")]
    public class NamePatch
    {
        private static void Postfix(VRRig __instance, string name)
        {
            NetPlayer player = RigManager.GetPlayerFromVRRig(__instance) ?? null;
            if (player != null && Menu.Main.ShouldBypassChecks(player))
            {
                player.SanitizedNickName = player.NickName;
                __instance.playerNameVisible = player.NickName;
                __instance.playerText1.text = player.NickName;
                __instance.playerText2.text = Menu.Main.NoRichtextTags(player.NickName);
            }
        }
    }
}
