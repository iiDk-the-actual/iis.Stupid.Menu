/*
 * ii's Stupid Menu  Managers/LogManager.cs
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

﻿namespace iiMenu.Managers
{
    public class LogManager
    {
        public static void Log(object log) =>
            Plugin.PluginLogger.LogInfo(log);

        public static void Log(object log, object[] args) =>
            Plugin.PluginLogger.LogInfo(string.Format(log.ToString(), args));

        public static void LogError(object log) =>
            Plugin.PluginLogger.LogError(log);

        public static void LogError(object log, object[] args) =>
            Plugin.PluginLogger.LogError(string.Format(log.ToString(), args));

        public static void LogWarning(object log) =>
            Plugin.PluginLogger.LogDebug(log);

        public static void LogWarning(object log, object[] args) =>
            Plugin.PluginLogger.LogDebug(string.Format(log.ToString(), args));
    }
}
