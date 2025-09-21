/*
 * ii's Stupid Menu  Properties/AssemblyInfo.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

using System.Reflection;

[assembly: AssemblyCompany("Goldentrophy Software")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyFileVersion(iiMenu.PluginInfo.Version + ".0")]
[assembly: AssemblyInformationalVersion(iiMenu.PluginInfo.Version)]
[assembly: AssemblyProduct(iiMenu.PluginInfo.Name)]
[assembly: AssemblyTitle(iiMenu.PluginInfo.Name)]
[assembly: AssemblyVersion(iiMenu.PluginInfo.Version + ".0")]
[assembly: AssemblyDescription(iiMenu.PluginInfo.Description)]
