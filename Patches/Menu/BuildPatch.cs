/*
 * ii's Stupid Menu  Patches/Menu/BuildPatch.cs
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

﻿using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(BuilderPieceInteractor), nameof(BuilderPieceInteractor.UpdateHandState))]
    public class BuildPatch
    {
        public static bool enabled;
        public static float previous;
        public static float previous2;

        private static void Prefix()
        {
            if (enabled)
            {
                previous = VRRig.LocalRig.NativeScale;
                previous2 = VRRig.LocalRig.ScaleMultiplier;
                VRRig.LocalRig.NativeScale = 1f;
                VRRig.LocalRig.ScaleMultiplier = 1f;
            }
        }

        private static void Postfix()
        {
            if (enabled)
            {
                VRRig.LocalRig.NativeScale = previous;
                VRRig.LocalRig.ScaleMultiplier = previous2;
            }
        }
    }
}
