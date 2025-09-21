using HarmonyLib;
using iiMenu.Managers;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "IsPositionInRange")]
    public class DistancePatch
    {
        public static bool enabled;

        public static void Postfix(VRRig __instance, ref bool __result, Vector3 position, float range)
        {
            NetPlayer player = RigManager.GetPlayerFromVRRig(__instance) ?? null;
            if ((enabled && __instance.isLocal) || (player != null && iiMenu.Menu.Main.ShouldBypassChecks(player)))
                __result = true;
        }
    }
}
