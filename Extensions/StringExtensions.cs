/*
 * ii's Stupid Menu  Extensions/StringExtensions.cs
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

using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RandomUtilities;

namespace iiMenu.Extensions
{
    public static class StringExtensions
    {
        public static string ClearTags(this string input) =>
            NoRichtextTags(input);

        public static string ToTitleCase(this string input) =>
            Menu.Main.ToTitleCase(input);

        public static string Hash(this string input) =>
            GetSHA256(input);

        public static string EnforceLength(this string str, int maxLength) =>
            str.Length > maxLength ? str[..maxLength] : str;

        public static string Random(this string _, int length) =>
            RandomString(length);
    }
}
