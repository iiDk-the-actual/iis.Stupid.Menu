using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "QuitDelay", MethodType.Enumerator)]
    public class NoQuitDelay
    {
        private static bool Prefix() =>
            false;
    }
}
