/*
 * ii's Stupid Menu  Properties/AssemblyInfo.cs
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
using System.Reflection;

[assembly: AssemblyCompany("Goldentrophy Software")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyFileVersion(PluginInfo.Version + ".0")]
[assembly: AssemblyInformationalVersion(PluginInfo.Version)]
[assembly: AssemblyVersion(PluginInfo.Version + ".0")]

#if DEBUG
[assembly: AssemblyProduct(PluginInfo.Name + " [Debug]")]
[assembly: AssemblyTitle(PluginInfo.Name + " [Debug]")]
#else
[assembly: AssemblyProduct(PluginInfo.Name)]
[assembly: AssemblyTitle(PluginInfo.Name)]
#endif

[assembly: AssemblyDescription(PluginInfo.Description + " [Debug]")]
