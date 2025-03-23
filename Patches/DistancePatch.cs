using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "IsPositionInRange")]
    public class DistancePatch
    {
        public static bool enabled = false;

        public static void Postfix(ref bool __result, Vector3 position, float range)
        {
            if (enabled)
                __result = true;
        }
    }
}
