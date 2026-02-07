/*
 * ii's Stupid Menu  Patches/Menu/PlacePatch.cs
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
    [HarmonyPatch(typeof(BuilderTable), nameof(BuilderTable.RequestPlacePiece))]
    public class PlacePatch
    {
        public static BuilderPiece _piece;

        public static sbyte _bumpOffsetX;
        public static sbyte _bumpOffsetZ;
        public static byte _twist;
        public static BuilderPiece _parentPiece;

        public static int _attachIndex;
        public static int _parentAttachIndex;

        private static void Postfix(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
        {
            _piece = piece;

            _bumpOffsetX = bumpOffsetX;
            _bumpOffsetZ = bumpOffsetZ;
            _twist = twist;
            _parentPiece = parentPiece;

            _attachIndex = attachIndex;
            _parentAttachIndex = parentAttachIndex;
        }
    }
}
