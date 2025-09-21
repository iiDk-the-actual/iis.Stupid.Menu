using HarmonyLib;
using PlayFab;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
    public class PlayfabUtil03
    {
        private static bool Prefix() =>
            false;
    }
}
