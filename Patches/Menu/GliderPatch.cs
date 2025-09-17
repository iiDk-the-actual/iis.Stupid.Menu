using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GliderHoldable), "Respawn")]
    public class GliderPatch
    {
        public static bool enabled;
        public static bool Prefix() =>
            !enabled;
    }
}
