using HarmonyLib;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaSpeakerLoudness))]
    [HarmonyPatch("InvokeUpdate", MethodType.Normal)]
    internal class MicPatch
    {
        public static bool returnAsNone = true;

        public static void Postfix(GorillaSpeakerLoudness __instance, bool ___isMicEnabled)
        {
            if (returnAsNone)
            {
                ___isMicEnabled = false;
            }
        }
    }
}
