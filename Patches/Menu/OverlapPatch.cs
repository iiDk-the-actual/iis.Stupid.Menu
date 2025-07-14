using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BuilderAttachGridPlane), "IsConnected")]
    public class OverlapPatch
    {
        public static bool enabled;

        public static void Postfix(BuilderAttachGridPlane __instance, ref bool __result, SnapBounds bounds)
        {
            if (enabled)
                __result = false;
        }
    }
}
