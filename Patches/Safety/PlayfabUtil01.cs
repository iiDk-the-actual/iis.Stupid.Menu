using HarmonyLib;
using PlayFab.Internal;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    public class PlayfabUtil01
    {
        private static bool Prefix() =>
            false;
    }
}
