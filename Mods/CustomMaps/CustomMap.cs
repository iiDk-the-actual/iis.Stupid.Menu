/*
 * ii's Stupid Menu  Mods/CustomMaps/CustomMap.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using iiMenu.Classes.Menu;

namespace iiMenu.Mods.CustomMaps
{
    public abstract class CustomMap
    {
        public abstract long MapID { get; }
        public abstract ButtonInfo[] Buttons { get; }
    }
}
