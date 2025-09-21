using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "ShouldDisconnectFromRoom")]
    public class NoShouldDisconnectFromRoom
    {
        private static bool Prefix() =>
            false;
    }
}
