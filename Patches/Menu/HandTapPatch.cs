using HarmonyLib;
using System;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRigSerializer), "OnHandTapRPCShared")]
    public class HandTapPatch
    {
        public static Action<VRRig, Vector3> OnHandTap;
        public static bool enabled;

        public static bool Prefix(VRRigSerializer __instance, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
        {
            OnHandTap?.Invoke(__instance.vrrig, isLeftHand ? __instance.vrrig.leftHandTransform.position : __instance.vrrig.rightHandTransform.position);

            return !enabled;
        }
    }
}
