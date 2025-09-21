/*
 * ii's Stupid Menu  Patches/Menu/QuitBoxPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using iiMenu.Mods;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaQuitBox), "OnBoxTriggered")]
    public class QuitBoxPatch
    {
        public static bool enabled = true;
        public static bool teleportToStump;

        public static bool Prefix()
        {
            if (teleportToStump)
            {
                Movement.TeleportToMap(Movement.mapData[0][1], Movement.mapData[0][2]);
                return false;
            }

            return enabled;
        }
    }
}
