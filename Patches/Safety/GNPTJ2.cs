using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
    public class GNPTJ2
    {
        private static bool Prefix() =>
            false;
    }
}
