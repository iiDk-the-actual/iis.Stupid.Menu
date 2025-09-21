/*
 * ii's Stupid Menu  Patches/Menu/NetworkTriggerPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "OnBoxTriggered")]
    public class NetworkTriggerPatch
    {
        public static bool enabled;

        public static bool Prefix() =>
            !enabled;
    }
}
