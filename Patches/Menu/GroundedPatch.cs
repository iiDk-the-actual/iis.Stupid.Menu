using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(HandLink), "LocalUpdate")]
    public class GroundedPatch
    {
        public static bool enabled;

        public static void Postfix(HandLink __instance, bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool canBeGrabbed)
        {
            if (enabled)
                __instance.isGroundedHand = true;
        }
    }
}
