using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "GrabbedByPlayer")]
    public class GrabPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand) =>
            !(enabled && __instance.isLocal);
    }

    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class DropPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity) =>
            !(GrabPatch.enabled && __instance.isLocal);
    }
}
