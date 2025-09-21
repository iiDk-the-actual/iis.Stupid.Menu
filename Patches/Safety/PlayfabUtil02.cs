/*
 * ii's Stupid Menu  Patches/Safety/PlayfabUtil02.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using PlayFab;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    public class PlayfabUtil02
    {
        private static bool Prefix() =>
            false;
    }
}
