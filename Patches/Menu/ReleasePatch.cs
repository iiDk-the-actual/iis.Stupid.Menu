using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(HandLink), "OnRelease")]
    public class ReleasePatch
    {
        public static bool enabled;

        public static bool Prefix(HandLink __instance, bool __result, DropZone zoneReleased, GameObject releasingHand)
        {
            if (enabled)
            {
                if (!__instance.OnRelease(zoneReleased, releasingHand))
                {
                    __result = false;
                    return false;
                }

                if (!__instance.myRig.isOfflineVRRig)
                {
                    HandLink handLink = ((releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
                    bool targetNotGrounded = false;

                    if (GTPlayer.Instance.IsGroundedHand)
                    {
                        if (!__instance.isGroundedHand)
                            targetNotGrounded = true;
                    }
                    else if (GTPlayer.Instance.IsGroundedButt)
                    {
                        if (!__instance.isGroundedHand && !__instance.isGroundedButt)
                            targetNotGrounded = true;
                    }
                    else if (handLink.myOtherHandLink.grabbedLink != null)
                    {
                        handLink.GetChainAuthority(out int stepsToGroundSelf, out bool groundIsHandSelf, out float lastTouchedGroundTimestampSelf, out float lastHandTouchedGroundTimestampSelf);
                        handLink.myOtherHandLink.GetChainAuthority(out int stepsToGroundTarget, out bool groundIsHandTarget, out float lastTouchedGroundTimestampTarget, out float lastHandTouchedGroundTimestampTarget);

                        if (groundIsHandSelf && !groundIsHandTarget)
                            targetNotGrounded = true;
                        
                        else if (stepsToGroundSelf < 0 && stepsToGroundTarget >= 0)
                            targetNotGrounded = true;
                    }

                    if (targetNotGrounded)
                    {
                        Vector3 averageVelocity = (handLink.isLeftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false).normalized * 20f;
                        __instance.myRig.netView.SendRPC("DroppedByPlayer", __instance.myRig.OwningNetPlayer, new object[] { averageVelocity });
                        __instance.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
                    }

                    handLink.BreakLink();
                }
                __result = true;
                return false;
            }
            
            return true;
        }
    }
}
