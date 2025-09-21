using iiMenu.Managers;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Extensions
{
    public static class VRRigExtensions
    {
        public static bool IsLocal(this VRRig rig) =>
            PlayerIsLocal(rig);

        public static bool IsTagged(this VRRig rig) =>
            PlayerIsTagged(rig);

        public static Color GetColor(this VRRig rig) =>
            GetPlayerColor(rig);

        public static bool Active(this VRRig rig) =>
            rig != null && GorillaParent.instance.vrrigs.Contains(rig);

        public static float Distance(this VRRig rig, Vector3 position) =>
            Vector3.Distance(rig.transform.position, position);

        public static float Distance(this VRRig rig, VRRig otherRig) =>
            rig.Distance(otherRig.transform.position);
        
        public static float Distance(this VRRig rig) =>
            rig.Distance(GorillaTagger.Instance.bodyCollider.transform.position);

        public static VRRig GetClosestVRRig(this VRRig rig) =>
            GorillaParent.instance.vrrigs.Where(targetRig => targetRig != null && !targetRig.isLocal)
                                         .OrderBy(targetRig => rig.Distance(targetRig))
                                         .FirstOrDefault();

        public static string GetName(this VRRig rig) =>
            RigManager.GetPlayerFromVRRig(rig)?.NickName ?? "null";

        public static NetPlayer GetPlayer(this VRRig rig) =>
            RigManager.GetPlayerFromVRRig(rig);

        public static Slingshot GetSlingshot(this VRRig rig) =>
            rig.projectileWeapon as Slingshot;
    }
}
