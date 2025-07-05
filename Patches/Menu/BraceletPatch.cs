using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "UpdateFriendshipBracelet")]
    public class BraceletPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance) =>
            !enabled;
    }
}
