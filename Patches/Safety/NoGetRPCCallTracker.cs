using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker
    {
        private static bool Prefix() =>
            false;
    }
}
