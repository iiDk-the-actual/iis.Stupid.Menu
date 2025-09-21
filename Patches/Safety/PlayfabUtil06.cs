using HarmonyLib;
using PlayFab.Internal;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
    public class PlayfabUtil06
    {
        private static bool Prefix() =>
            false;
    }
}
