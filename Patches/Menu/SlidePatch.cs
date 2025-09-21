/*
 * ii's Stupid Menu  Patches/Menu/SlidePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaLocomotion;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GTPlayer), "GetSlidePercentage")]
    public class SlidePatch
    {
        public static bool everythingSlippery;
        public static bool everythingGrippy;

        public static void Postfix(GTPlayer __instance, ref float __result)
        {
            if (everythingSlippery)
                __result = 1;

            if (everythingGrippy)
                __result = 0;
        }
    }
}