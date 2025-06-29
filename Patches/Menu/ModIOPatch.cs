using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(ModIOManager), "OnJoinedRoom")]
    public class ModIOPatch
    {
        public static bool Prefix(VRRig __instance) =>
            false;
    }
}
