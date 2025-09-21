/*
 * ii's Stupid Menu  Mods/CustomMaps/Maps/MiningSimulator.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

﻿using iiMenu.Classes.Menu;
using System.Collections.Generic;
using static iiMenu.Mods.CustomMaps.Manager;

namespace iiMenu.Mods.CustomMaps.Maps
{
    public class MiningSimulator : CustomMap
    {
        public override long MapID => 4977315;
        public override ButtonInfo[] Buttons => new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Instant Mine", enableMethod =() => InstantMine(), disableMethod =() => DisableInstantMine(), toolTip = "Instantly mines any blocks with your pickaxe."},
            new ButtonInfo { buttonText = "Mine Anything", enableMethod =() => MineAnything(), disableMethod =() => DisableMineAnything(), toolTip = "Lets you mine any block."},
            new ButtonInfo { buttonText = "Infinite Backpack", enableMethod =() => InfiniteBackpack(), disableMethod =() => DisableInfiniteBackpack(), toolTip = "Lets you mine more blocks even if your inventory is full."},
        };

        public static void InstantMine()
        {
            ModifyCustomScript(new Dictionary<int, string>
                    {
                        { 844, "lastMinedTime = 0" }
                    });
        }
        public static void DisableInstantMine() =>
            RevertCustomScript(844);

        public static void MineAnything()
        {
            ModifyCustomScript(new Dictionary<int, string>
                    {
                        { 831, "if true then" }
                    });
        }
        public static void DisableMineAnything() =>
            RevertCustomScript(831);

        public static void InfiniteBackpack()
        {
            ModifyCustomScript(new Dictionary<int, string>
                    {
                        { 810, "if false then" }
                    });
        }
        public static void DisableInfiniteBackpack() =>
            RevertCustomScript(810);
    }
}
