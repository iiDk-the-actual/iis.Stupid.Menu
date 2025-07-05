using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "OnBoxTriggered")]
    public class NetworkTriggerPatch
    {
        public static bool enabled;

        public static bool Prefix() =>
            !enabled;
    }
}
