using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "UpdateFriendshipBracelet")]
    public class BraceletPatch
    {
        public static bool enabled = false;

        public static bool Prefix(VRRig __instance) =>
            !enabled;
    }
}
