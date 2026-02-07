/*
 * ii's Stupid Menu  Patches/Menu/LoadModPatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

﻿using GorillaTagScripts.VirtualStumpCustomMaps;
using HarmonyLib;
using iiMenu.Mods.CustomMaps;
using Modio.Mods;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(CustomMapManager), nameof(CustomMapManager.LoadMap))]
    public class LoadModPatch
    {
        public static void Prefix(ModId modId) =>
            Manager.UpdateCustomMapsTab(modId);
    }

    [HarmonyPatch(typeof(CustomMapManager), nameof(CustomMapManager.UnloadMap))]
    public class UnloadModPatch
    {
        public static void Prefix(bool returnToSinglePlayerIfInPublic) =>
            Manager.UpdateCustomMapsTab();
    }
}
