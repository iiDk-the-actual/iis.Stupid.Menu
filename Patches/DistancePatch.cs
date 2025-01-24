using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "CheckDistance")]
    public class DistancePatch
    {
        public static bool enabled = false;

        public static void Postfix(ref bool __result, Vector3 position, float max)
        {
            if (enabled)
                __result = true;
        }
    }
}
