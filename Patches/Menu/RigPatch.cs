using GorillaNetworking;
using HarmonyLib;
using System;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    public class RigPatch
    {
        public static bool Prefix(VRRig __instance) =>
            !__instance.isLocal;
    }

    [HarmonyPatch(typeof(VRRig), "Awake")]
    public class RigPatch2
    {
        public static bool Prefix(VRRig __instance) =>
            __instance.gameObject.name != "Local Gorilla Player(Clone)";
    }

    // Thanks nugget for help with patch
    [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
    public static class RigPatch3
    {
        public static bool Prefix(VRRigJobManager __instance, VRRig rig) =>
            !rig.isLocal;
    }
}
