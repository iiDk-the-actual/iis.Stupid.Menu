using GorillaExtensions;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class AntiCrashPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
        {
            if (enabled && __instance.isLocal && !GTExt.IsValid(throwVelocity))
                return false;
            
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
    public class AntiCrashPatch2
    {
        private static List<float> callTimestamps = new List<float>();
        public static bool Prefix(VRRig __instance)
        {
            if (AntiCrashPatch.enabled && __instance.isLocal)
            {
                callTimestamps.Add(Time.time);
                callTimestamps.RemoveAll(t => (Time.time - t) > 1);

                return callTimestamps.Count < 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "RequestMaterialColor")]
    public class AntiCrashPatch3
    {
        private static List<float> callTimestamps = new List<float>();
        public static bool Prefix(VRRig __instance)
        {
            if (AntiCrashPatch.enabled && __instance.isLocal)
            {
                callTimestamps.Add(Time.time);
                callTimestamps.RemoveAll(t => (Time.time - t) > 1);

                return callTimestamps.Count < 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DeployedChild), "Deploy")]
    public class AntiCrashPatch4
    {
        public static void Postfix(DeployedChild __instance, DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
        {
            if (AntiCrashPatch.enabled)
                __instance._rigidbody.linearVelocity = __instance._rigidbody.linearVelocity.ClampMagnitudeSafe(100f);
        }
    }
}
