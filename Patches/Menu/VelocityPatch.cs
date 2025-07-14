﻿using GorillaLocomotion.Climbing;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaVelocityTracker), "GetAverageVelocity")]
    public class VelocityPatch
    {
        public static bool enabled;
        public static float multipleFactor;

        public static void Postfix(GorillaVelocityTracker __instance, ref Vector3 __result, bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
        {
            if (enabled)
                __result *= multipleFactor;
        }
    }

    [HarmonyPatch(typeof(GorillaVelocityTracker), "GetLatestVelocity")]
    public class VelocityPatch2
    {
        public static void Postfix(GorillaVelocityTracker __instance, ref Vector3 __result, bool worldSpace = false)
        {
            if (VelocityPatch.enabled)
                __result *= VelocityPatch.multipleFactor;
        }
    }

    [HarmonyPatch(typeof(GorillaVelocityEstimator), "TriggeredLateUpdate")]
    public class VelocityPatch3
    {
        public static void Postfix(GorillaVelocityEstimator __instance)
        {
            if (VelocityPatch.enabled)
            {
                __instance.linearVelocity *= VelocityPatch.multipleFactor;
                __instance.angularVelocity *= VelocityPatch.multipleFactor;
                __instance.handPos *= VelocityPatch.multipleFactor;
            }
        }
    }
}
