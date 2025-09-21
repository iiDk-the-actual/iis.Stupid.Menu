using HarmonyLib;
using PlayFab.Internal;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
    public class PlayfabUtil04
    {
        private static bool Prefix() =>
            false;
    }
}
