using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "DispatchReport")]
    public class NoDispatchReport
    {
        private static bool Prefix() =>
            false;
    }
}
