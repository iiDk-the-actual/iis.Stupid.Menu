using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController), "AttemptToJoinRankedPublicRoomAsync")]
    public class RankedPatch
    {
        public static bool enabled;
        public static string targetPlatform;
        public static string targetTier;

        public static void Prefix(GorillaNetworkJoinTrigger triggeredTrigger, ref string mmrTier, ref string platform, JoinType roomJoinType)
        {
            if (enabled)
            {
                if (targetPlatform != "")
                    platform = targetPlatform;

                if (targetTier != "")
                    mmrTier = targetTier;
            }
        }
    }
}
