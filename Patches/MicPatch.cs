using HarmonyLib;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaSpeakerLoudness), "InvokeUpdate")]
    public class MicPatch
    {
        public static bool returnAsNone = false;

        private static bool Prefix(GorillaSpeakerLoudness __instance, bool ___isMicEnabled, bool ___isSpeaking, float ___loudness)
        {
            if (returnAsNone && __instance.gameObject.name == "Local Gorilla Player")
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
