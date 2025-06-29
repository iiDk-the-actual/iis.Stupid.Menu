using HarmonyLib;
using PlayFab;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
    public class PlayfabUtil05
    {
        private static bool Prefix() =>
            false;
    }
}
