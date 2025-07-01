using GorillaTag.Audio;
using HarmonyLib;
using iiMenu.Mods;
using Photon.Voice;
using Photon.Voice.Unity;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(Speaker), "OnAudioFrame")]
    public class SpeakerPatch
    {
        public static bool enabled;
        public static Speaker targetSpeaker;

        static void Postfix(Speaker __instance, FrameOut<float> frame)
        {
            if (!enabled || targetSpeaker == null || __instance != targetSpeaker)
                return;

            Fun.ProcessFrameBuffer(frame.Buf);
        }
    }
}
