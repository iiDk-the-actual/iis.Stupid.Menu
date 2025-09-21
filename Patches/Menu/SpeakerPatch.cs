/*
 * ii's Stupid Menu  Patches/Menu/SpeakerPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using iiMenu.Mods;
using Photon.Voice;
using Photon.Voice.Unity;

namespace iiMenu.Patches.Menu
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
