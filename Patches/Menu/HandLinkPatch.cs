using GorillaLocomotion;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GTPlayer), "HandleHandLink")]
    public class HandLinkPatch
    {
        public static bool enabled;

        public static bool Prefix(GTPlayer __instance) =>
            !enabled;
    }
}
