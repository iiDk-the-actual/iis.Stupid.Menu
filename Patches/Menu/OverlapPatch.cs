/*
 * ii's Stupid Menu  Patches/Menu/OverlapPatch.cs
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

﻿using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(BuilderAttachGridPlane), nameof(BuilderAttachGridPlane.IsConnected))]
    public class OverlapPatch
    {
        public static bool enabled;

        public static void Postfix(BuilderAttachGridPlane __instance, ref bool __result, SnapBounds bounds)
        {
            if (enabled)
                __result = false;
        }
    }
}
