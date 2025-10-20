﻿/*
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

using System;
using System.Collections.Generic;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Extensions
{
    public static class MiscellaneousExtensions
    {
        public static float GetDelay(this CallLimiter self) =>
            GetCallLimiterDelay(self);

        public static string ToHex(this Color input) =>
            ColorToHex(input);

        public static string ToRichRGBString(this Color input, int roundAmount = 255) =>
            $"<color=red>{Math.Round(input.r * roundAmount)}</color> <color=green>{Math.Round(input.g * roundAmount)}</color> <color=blue>{Math.Round(input.b * roundAmount)}</color>";

        public static string ToRGBString(this Color input, int roundAmount = 255) =>
            $"{Math.Round(input.r * roundAmount)} {Math.Round(input.g * roundAmount)} {Math.Round(input.b * roundAmount)}";

        private static readonly Dictionary<KeyCode, string> normalMap = new Dictionary<KeyCode, string>
        {
            { KeyCode.Space, " " },
            { KeyCode.Comma, "," },
            { KeyCode.Period, "." },
            { KeyCode.Slash, "/" },
            { KeyCode.Backslash, "\\" },
            { KeyCode.Minus, "-" },
            { KeyCode.Equals, "=" },
            { KeyCode.Semicolon, ";" },
            { KeyCode.Quote, "'" },
            { KeyCode.LeftBracket, "[" },
            { KeyCode.RightBracket, "]" }
        };

        private static readonly Dictionary<KeyCode, string> shiftMap = new Dictionary<KeyCode, string>
        {
            { KeyCode.Alpha1, "!" },
            { KeyCode.Alpha2, "@" },
            { KeyCode.Alpha3, "#" },
            { KeyCode.Alpha4, "$" },
            { KeyCode.Alpha5, "%" },
            { KeyCode.Alpha6, "^" },
            { KeyCode.Alpha7, "&" },
            { KeyCode.Alpha8, "*" },
            { KeyCode.Alpha9, "(" },
            { KeyCode.Alpha0, ")" },

            { KeyCode.Minus, "_" },
            { KeyCode.Equals, "+" },
            { KeyCode.LeftBracket, "{" },
            { KeyCode.RightBracket, "}" },
            { KeyCode.Backslash, "|" },
            { KeyCode.Semicolon, ":" },
            { KeyCode.Quote, "\"" },
            { KeyCode.Comma, "<" },
            { KeyCode.Period, ">" },
            { KeyCode.Slash, "?" }
        };

        public static string Key(this KeyCode key)
        {
            if (key >= KeyCode.A && key <= KeyCode.Z)
                return key.ToString().ToLower();

            if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
                return ((char)('0' + (key - KeyCode.Alpha0))).ToString();

            if (normalMap.TryGetValue(key, out string value))
                return value;

            switch (key)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter: return "\n";
                case KeyCode.Tab: return "\t";
                default: return "";
            }
        }

        public static string ShiftedKey(this KeyCode key)
        {
            if (key >= KeyCode.A && key <= KeyCode.Z)
                return key.ToString();

            if (shiftMap.TryGetValue(key, out string shifted))
                return shifted;

            return key.Key();
        }

        public static IEnumerable<GameObject> Children(this Transform t)
        {
            var list = new List<GameObject>();
            for (int i = 0; i < t.childCount; i++)
                list.Add(t.GetChild(i).gameObject);
            return list;
        }

        public static void Play(this AudioClip clip, float volume = 1f) =>
            Play2DAudio(clip, volume);

        public static void PlayAt(this AudioClip clip, Vector3 position, float volume = 1f) =>
            PlayPositionAudio(clip, position, volume);
    }
}
