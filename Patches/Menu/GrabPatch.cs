using HarmonyLib;
using iiMenu.Classes;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "GrabbedByPlayer")]
    public class GrabPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand) =>
            !(enabled && __instance == RigManager.LocalRig);
    }

    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class DropPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity) =>
            !(GrabPatch.enabled && __instance == RigManager.LocalRig);
    }

    [HarmonyPatch(typeof(GuardianRPCs), "GuardianLaunchPlayer")]
    public class LaunchPatch
    {
        public static bool Prefix(Vector3 velocity) =>
            !GrabPatch.enabled;
    }
}
