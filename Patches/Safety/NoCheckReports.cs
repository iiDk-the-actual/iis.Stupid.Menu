using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "CheckReports")]
    public class NoCheckReports
    {
        private static bool Prefix() =>
            false;
    }
}
