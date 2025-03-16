using HarmonyLib;
using UnityEngine;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "UpdateFriendshipBracelet")]
    public class BraceletPatch
    {
        public static bool enabled = false;

        public static bool Prefix(VRRig __instance)
        {
            return !enabled;
        }
    }
}
