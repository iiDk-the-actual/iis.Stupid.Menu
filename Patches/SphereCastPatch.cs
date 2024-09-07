using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaTagger), "get_sphereCastRadius")]
    public class SphereCastPatch
    {
        public static bool patchEnabled = false;
        public static float overrideRadius = 0.1f;

        public static void Postfix(GorillaTagger __instance, ref float __result)
        {
            if (patchEnabled)
            {
                __result = overrideRadius;
            }
        }
    }
}
