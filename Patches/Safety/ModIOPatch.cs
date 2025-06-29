using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(ModIOManager), "OnJoinedRoom")]
    public class ModIOPatch
    {
        public static bool Prefix(VRRig __instance) =>
            false;
    }
}
