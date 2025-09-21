/*
 * ii's Stupid Menu  Patches/Menu/GroundedPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(HandLink), "LocalUpdate")]
    public class GroundedPatch
    {
        public static bool enabled;

        public static void Postfix(HandLink __instance, bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool canBeGrabbed)
        {
            if (enabled)
                __instance.isGroundedHand = true;
        }
    }
}
