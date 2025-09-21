/*
 * ii's Stupid Menu  Mods/CustomMaps/Maps/MiningSimulator.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
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
