using HarmonyLib;
using iiMenu.Classes;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    public class RigPatch
    {
        public static bool Prefix(VRRig __instance) =>
            __instance != RigManager.LocalRig;
    }

    // Thanks nugget for help with patch
    [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
    public static class RigPatch2
    {
        public static bool Prefix(VRRigJobManager __instance, VRRig rig) =>
            rig != RigManager.LocalRig;
    }
}
