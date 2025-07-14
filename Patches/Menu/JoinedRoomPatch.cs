using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
