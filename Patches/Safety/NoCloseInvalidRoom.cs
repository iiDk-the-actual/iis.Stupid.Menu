using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]
    public class NoCloseInvalidRoom
    {
        private static bool Prefix() =>
            false;
    }
}
