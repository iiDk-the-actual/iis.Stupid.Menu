using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
    public class GNPTJ1
    {
        private static bool Prefix() =>
            false;
    }
}
