using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController), "AttemptToJoinRankedPublicRoom")]
    public class RankedPatch
    {
        public static bool enabled;
        public static string targetPlatform;
        public static string targetTier;

        public static bool Prefix(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
        {
            if (enabled)
            {
                PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoomAsync(
                    triggeredTrigger,
                    targetTier == "" ? RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString() : targetTier,
                    targetPlatform == "" ? "PC" : targetPlatform,
                    roomJoinType
                );

                return false;
            }
            return true;
        }
    }
}
