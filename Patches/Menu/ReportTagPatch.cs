/*
 * ii's Stupid Menu  Patches/Menu/ReportTagPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaTagManager), "ReportTag")]
    public class ReportTagPatch
    {
        public static List<NetPlayer> blacklistedPlayers = new List<NetPlayer>();
        public static List<NetPlayer> invinciblePlayers = new List<NetPlayer>();

        public static bool Prefix(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
        {
            if (blacklistedPlayers.Contains(taggingPlayer) || invinciblePlayers.Contains(taggedPlayer))
                return false;

            return true;
        }
    }
}
