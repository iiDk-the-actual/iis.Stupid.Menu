using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GliderHoldable), "Respawn")]
    public class AntiGliderRespawn
    {
        public static bool Prefix() =>
            !NoGliderRespawn;
    }
}
