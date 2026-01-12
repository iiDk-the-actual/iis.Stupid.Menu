/*
 * ii's Stupid Menu  Extensions/TextMeshProExtensions.cs
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

using iiMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Extensions
{
    public static class TextMeshProExtensions
    {
        public static void SafeSetText(this TextMeshPro tmp, string text)
        {
            if (tmp == null)
                return;

            if (tmp.text != text)
                tmp.text = text;
        }

        public static void SafeSetFont(this TextMeshPro tmp, TMP_FontAsset font)
        {
            if (tmp == null)
                return;
            if (tmp.font != font)
                tmp.font = font;
        }

        public static void SafeSetFontSize(this TextMeshPro tmp, float size)
        {
            if (tmp == null)
                return;
            if (Math.Abs(tmp.fontSize - size) > 0.01f)
                tmp.fontSize = size;
        }

        public static void SafeSetFontStyle(this TextMeshPro tmp, FontStyles style)
        {
            if (tmp == null)
                return;
            if (tmp.fontStyle != style)
                tmp.fontStyle = style;
        }
    }
}
