/*
 * ii's Stupid Menu  LegacyInjectCompatibility.cs
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

using iiMenu;

#pragma warning disable IDE0130 // Namespace does not match folder structure [for legacy compatibility with default SMI settings]
// ReSharper disable once CheckNamespace
namespace Loading
#pragma warning restore IDE0130 // Namespace does not match folder structure [for legacy compatibility with default SMI settings]
{
    public static class Loader 
    {
        public static void Load() =>
            Plugin.InjectDontDestroy();
    }
}
