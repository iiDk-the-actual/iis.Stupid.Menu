using GorillaLocomotion;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GTPlayer), "GetSlidePercentage")]
    public class SlidePatch
    {
        public static bool everythingSlippery;
        public static bool everythingGrippy;

        public static void Postfix(GTPlayer __instance, ref float __result)
        {
            if (everythingSlippery)
                __result = 1;

            if (everythingGrippy)
                __result = 0;
        }
    }
}