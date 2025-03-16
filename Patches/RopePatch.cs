using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.XR;
using UnityEngine;
using Photon.Voice.Unity.UtilityScripts;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(GorillaRopeSwing), "AttachLocalPlayer")]
    public class RopePatch
    {
        public static bool enabled = false;
        public static float amplifier = 5f;

        public static void Prefix(XRNode xrNode, Transform grabbedBone, Vector3 offset, ref Vector3 velocity)
        {
            if (enabled)
                velocity *= amplifier;
        }
    }
}
