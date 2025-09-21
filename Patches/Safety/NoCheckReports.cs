/*
 * ii's Stupid Menu  Patches/Safety/NoCheckReports.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "CheckReports")]
    public class NoCheckReports
    {
        private static bool Prefix() =>
            false;
    }
}
