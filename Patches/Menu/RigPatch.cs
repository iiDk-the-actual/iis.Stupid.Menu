using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    public class RigPatch
    {
        public static bool Prefix(VRRig __instance) =>
            !__instance.isLocal;
    }

    // Thanks nugget for help with patch
    [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
    public static class RigPatch2
    {
        public static bool Prefix(VRRigJobManager __instance, VRRig rig) =>
            !rig.isLocal;
    }
}
