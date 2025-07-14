using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController), "JoinedRoomPatch")]
    internal class JoinedRoomPatch
    {
        public static bool enabled;

        private static void Prefix()
        {
            if (enabled)
                PhotonNetworkController.Instance.currentJoinType = JoinType.FollowingParty;
        }
    }
}
