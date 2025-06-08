using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(SecondLookSkeleton), "GrabPlayer")]
    public class CaughtPatch
    {
        public static bool enabled = false;

        public static bool Prefix()
        {
            if (enabled)
                return false;
            
            return true;
        }
    }
    [HarmonyPatch(typeof(SecondLookSkeleton), "CanGrab")]
    public class CaughtPatch2
    {
        public static void Postfix(ref bool __result)
        {
            if (CaughtPatch.enabled)
                __result = false;
        }
    }
    [HarmonyPatch(typeof(SecondLookSkeleton), "RemotePlayerSeen")]
    public class CaughtPatch3
    {
        public static bool Prefix()
        {
            if (CaughtPatch.enabled)
                return false;

            return true;
        }
    }
}
