using HarmonyLib;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate")]
    public class TeleportPatch
    {
        public static bool doTeleport = false;
        public static Vector3 telePos;

        public static bool Prefix(GorillaLocomotion.Player __instance, ref Vector3 ___lastPosition, ref Vector3 ___lastHeadPosition, ref Vector3 ___lastLeftHandPosition, ref Vector3 ___lastRightHandPosition, ref Vector3[] ___velocityHistory, ref Vector3 ___currentVelocity)
        {
            if (doTeleport)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().transform.position = World2Player(telePos);
                ___lastPosition = telePos;

                ___lastHeadPosition = GorillaLocomotion.Player.Instance.headCollider.transform.position;
                ___lastLeftHandPosition = telePos;
                ___lastRightHandPosition = telePos;

                ___velocityHistory = new Vector3[GorillaLocomotion.Player.Instance.velocityHistorySize];
                ___currentVelocity = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity;

                doTeleport = false;
                return true;
            }
            return true;
        }
    }
}
