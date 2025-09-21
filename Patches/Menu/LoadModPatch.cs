/*
 * ii's Stupid Menu  Patches/Menu/LoadModPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaTagScripts.VirtualStumpCustomMaps;
using HarmonyLib;
using Modio.Mods;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(CustomMapManager), "LoadMap")]
    public class LoadModPatch
    {
        public static void Prefix(ModId modId) =>
            Mods.CustomMaps.Manager.UpdateCustomMapsTab(modId);
    }

    [HarmonyPatch(typeof(CustomMapManager), "UnloadMap")]
    public class UnloadModPatch
    {
        public static void Prefix(bool returnToSinglePlayerIfInPublic) =>
            Mods.CustomMaps.Manager.UpdateCustomMapsTab(-1);
    }
}
