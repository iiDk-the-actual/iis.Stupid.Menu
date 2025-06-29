using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "CheckReports", MethodType.Enumerator)]
    public class NoCheckReports
    {
        private static bool Prefix() =>
            false;
    }
}
