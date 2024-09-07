using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    public class GhostPatch
    {
        public static bool Prefix(VRRig __instance)
        {
            return !(__instance == GorillaTagger.Instance.offlineVRRig);
        }
    }
}
