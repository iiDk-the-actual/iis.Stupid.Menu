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
                if (!__instance.myRig.isOfflineVRRig)
                {
                    bool grounded = false;

                    HandLink handLink = ((releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
                   
                    HandLinkAuthorityStatus selfHandLinkAuthority = GTPlayer.Instance.GetSelfHandLinkAuthority();
                    HandLinkAuthorityStatus selfChainAuthority = handLink.GetChainAuthority(out int selfAuthority);

                    if (selfHandLinkAuthority.type >= HandLinkAuthorityType.ButtGrounded && selfChainAuthority.type < selfHandLinkAuthority.type)
                        grounded = true;
                    else if (handLink.myOtherHandLink.grabbedLink != null)
                    {
                        HandLinkAuthorityStatus otherChainAuthority = handLink.myOtherHandLink.GetChainAuthority(out int otherAuthority);
                        if (otherChainAuthority.type >= HandLinkAuthorityType.ButtGrounded && selfChainAuthority.type < otherChainAuthority.type)
                            grounded = true;
                    }

                    if (grounded)
                    {
                        Vector3 averageVelocity = (handLink.isLeftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false).normalized * 20f;
                        __instance.myRig.netView.SendRPC("DroppedByPlayer", __instance.myRig.OwningNetPlayer, new object[] { averageVelocity });
                        __instance.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
                    }

                    handLink.BreakLink();
                }
                return false;
            }
            
            return true;
        }
    }
}
