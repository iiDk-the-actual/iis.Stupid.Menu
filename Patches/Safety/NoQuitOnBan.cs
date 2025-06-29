using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
    public class NoQuitOnBan
    {
        private static bool Prefix() =>
            false;
    }
}
