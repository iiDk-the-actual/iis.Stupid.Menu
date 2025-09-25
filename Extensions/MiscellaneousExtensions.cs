/*
 * ii's Stupid Menu  Extensions/MiscellaneousExtensions.cs
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

using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Extensions
{
    public static class MiscellaneousExtensions
    {
        public static float GetDelay(this CallLimiter self) =>
            GetCallLimiterDelay(self);

        public static void ToHex(this Color input) =>
            ColorToHex(input);

        public static string Key(this KeyCode key)
        {
            if (key >= KeyCode.A && key <= KeyCode.Z)
                return key.ToString(); // Returns "A", "B", etc.

            if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
                return ((char)('0' + (key - KeyCode.Alpha0))).ToString();

            if (key == KeyCode.Space)
                return " ";

            if (key == KeyCode.Return || key == KeyCode.KeypadEnter)
                return "\n";

            if (key == KeyCode.Tab)
                return "\t";

            switch (key)
            {
                case KeyCode.Comma: return ",";
                case KeyCode.Period: return ".";
                case KeyCode.Slash: return "/";
                case KeyCode.Backslash: return "\\";
                case KeyCode.Minus: return "-";
                case KeyCode.Equals: return "=";
                case KeyCode.Semicolon: return ";";
                case KeyCode.Quote: return "'";
                case KeyCode.LeftBracket: return "[";
                case KeyCode.RightBracket: return "]";
                default: return ""; // Unknown key
            }
        }
    }
}
