using GorillaLocomotion;
using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GTPlayer), "GetSlidePercentage")]
    public class SlidePatch
    {
        private static void Postfix(GTPlayer __instance, ref float __result)
        {
            if (EverythingSlippery)
                __result = 1;

            if (EverythingGrippy == true)
                __result = 0;
        }
    }
}