using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace iiMenu.Managers
{
    public class RigManager
    {
        public static VRRig GetVRRigFromPlayer(NetPlayer p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

        public static NetPlayer GetPlayerFromVRRig(VRRig p) =>
            p.Creator;

        public static NetPlayer GetPlayerFromID(string id) =>
            PhotonNetwork.PlayerList.FirstOrDefault(player => player.UserId == id);

        public static Player NetPlayerToPlayer(NetPlayer p) =>
            p.GetPlayerRef();

        public static Player GetRandomPlayer(bool includeSelf) =>
            includeSelf ?
            PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)] :
            PhotonNetwork.PlayerListOthers[Random.Range(0, PhotonNetwork.PlayerListOthers.Length)];

        public static VRRig GetRandomVRRig(bool includeSelf) =>
            GetVRRigFromPlayer(GetRandomPlayer(includeSelf));

        public static NetworkView GetNetworkViewFromVRRig(VRRig p) =>
            p.netView;

        public static PhotonView GetPhotonViewFromVRRig(VRRig p) =>
            GetNetworkViewFromVRRig(p).GetView;

        public static VRRig GetClosestVRRig() =>
            GorillaParent.instance.vrrigs
                .Where(rig => rig != null && !rig.isLocal)
                .OrderBy(rig => Vector3.Distance(rig.transform.position, GorillaTagger.Instance.bodyCollider.transform.position))
                .FirstOrDefault();

        public static Dictionary<string, float> waitingForCreationDate = new Dictionary<string, float>();
        public static Dictionary<string, string> creationDateCache = new Dictionary<string, string>();
        public static string GetCreationDate(string input, System.Action<string> onTranslated = null)
        {
            if (creationDateCache.TryGetValue(input, out string date))
                return date;
            else
            {
                if (!waitingForCreationDate.ContainsKey(input))
                {
                    waitingForCreationDate[input] = Time.time + 10f;
                    GetCreationCoroutine(input, onTranslated);
                }
                else
                {
                    if (Time.time > waitingForCreationDate[input])
                    {
                        waitingForCreationDate[input] = Time.time + 10f;
                        GetCreationCoroutine(input, onTranslated);
                    }
                }

                return "Loading...";
            }
        }

        public static void GetCreationCoroutine(string userId, System.Action<string> onTranslated = null)
        {
            if (creationDateCache.TryGetValue(userId, out string date))
            {
                onTranslated?.Invoke(date);
                return;
            }

            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = userId }, delegate (GetAccountInfoResult result) // Who designed this
            {
                string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy h:mm tt");
                creationDateCache[userId] = date;

                onTranslated?.Invoke(date);
            }, delegate { creationDateCache[userId] = "Error"; onTranslated?.Invoke(date); }, null, null);
        }
    }
}