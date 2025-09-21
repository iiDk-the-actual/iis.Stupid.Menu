/*
 * ii's Stupid Menu  Patches/Menu/ForcePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(ForceVolume), "OnTriggerEnter")]
    public class ForcePatch
    {
        public static bool enabled;

        public static bool Prefix() =>
            !enabled;
    }

    [HarmonyPatch(typeof(ForceVolume), "OnTriggerExit")]
    public class ForcePatch2
    {
        public static bool Prefix() =>
            !ForcePatch.enabled;
    }

    [HarmonyPatch(typeof(ForceVolume), "OnTriggerStay")]
    public class ForcePatch3
    {
        public static bool Prefix() =>
            !ForcePatch.enabled;
    }
}
