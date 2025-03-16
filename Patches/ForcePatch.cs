using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(ForceVolume), "OnTriggerEnter")]
    public class ForcePatch
    {
        public static bool enabled = false;

        public static bool Prefix()
        {
            return !enabled;
        }
    }

    [HarmonyPatch(typeof(ForceVolume), "OnTriggerExit")]
    public class ForcePatch2
    {
        public static bool Prefix()
        {
            return !ForcePatch.enabled;
        }
    }

    [HarmonyPatch(typeof(ForceVolume), "OnTriggerStay")]
    public class ForcePatch3
    {
        public static bool Prefix()
        {
            return !ForcePatch.enabled;
        }
    }
}
