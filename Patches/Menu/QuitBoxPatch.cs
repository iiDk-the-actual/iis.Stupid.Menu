using HarmonyLib;
using iiMenu.Mods;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaQuitBox), "OnBoxTriggered")]
    public class QuitBoxPatch
    {
        public static bool enabled = true;
        public static bool teleportToStump;

        public static bool Prefix()
        {
            if (teleportToStump)
            {
                Movement.TeleportToMap(Movement.mapData[0][1], Movement.mapData[0][2]);
                return false;
            }

            return enabled;
        }
    }
}
