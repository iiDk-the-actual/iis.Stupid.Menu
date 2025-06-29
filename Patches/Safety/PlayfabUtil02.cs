using HarmonyLib;
using PlayFab;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    public class PlayfabUtil02
    {
        private static bool Prefix() =>
            false;
    }
}
