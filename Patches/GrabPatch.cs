using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "GrabbedByPlayer")]
    public class GrabPatch
    {
        public static bool enabled = false;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
        {
            if (enabled && __instance == GorillaTagger.Instance.offlineVRRig)
                return false;
            
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class DropPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
        {
            if (GrabPatch.enabled && __instance == GorillaTagger.Instance.offlineVRRig)
                return false;

            return true;
        }
    }
}
