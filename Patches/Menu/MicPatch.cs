/*
 * ii's Stupid Menu  Patches/Menu/MicPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

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
