using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "OnBoxTriggered")]
    public class NetworkTriggerPatch
    {
        public static bool enabled;

        public static bool Prefix() =>
            !enabled;
    }
}
