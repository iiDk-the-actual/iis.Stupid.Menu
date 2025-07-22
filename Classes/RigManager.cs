using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

namespace iiMenu.Classes
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
    }
}