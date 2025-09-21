/*
 * ii's Stupid Menu  Patches/Menu/BuildPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(BuilderPieceInteractor), "UpdateHandState")]
    public class BuildPatch
    {
        public static bool enabled;
        public static float previous;
        public static float previous2;

        private static void Prefix()
        {
            if (enabled)
            {
                previous = VRRig.LocalRig.NativeScale;
                previous2 = VRRig.LocalRig.ScaleMultiplier;
                VRRig.LocalRig.NativeScale = 1f;
                VRRig.LocalRig.ScaleMultiplier = 1f;
            }
        }

        private static void Postfix()
        {
            if (enabled)
            {
                VRRig.LocalRig.NativeScale = previous;
                VRRig.LocalRig.ScaleMultiplier = previous2;
            }
        }
    }
}
