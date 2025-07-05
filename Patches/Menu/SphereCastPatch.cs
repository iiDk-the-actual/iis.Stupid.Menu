using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaTagger), "get_sphereCastRadius")]
    public class SphereCastPatch
    {
        public static bool enabled;
        public static float overrideRadius = 0.1f;

        public static void Postfix(GorillaTagger __instance, ref float __result)
        {
            if (enabled)
                __result = overrideRadius;
        }
    }
}
