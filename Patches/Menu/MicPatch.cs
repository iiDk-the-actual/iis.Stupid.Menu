using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaSpeakerLoudness), "UpdateLoudness")]
    public class MicPatch
    {
        public static bool enabled;

        private static bool Prefix(GorillaSpeakerLoudness __instance, ref bool ___isMicEnabled, ref bool ___isSpeaking, ref float ___loudness)
        {
            if (enabled && __instance.gameObject.name == "Local Gorilla Player")
            {
                ___isMicEnabled = false;
                ___isSpeaking = false;
                ___loudness = 0f;
                return false;
            }
            return true;
        }
    }
}
