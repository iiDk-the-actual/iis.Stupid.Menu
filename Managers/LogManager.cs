/*
 * ii's Stupid Menu  Managers/LogManager.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿namespace iiMenu.Managers
{
    public class LogManager
    {
        public static void Log(object log) =>
            Plugin.PluginLogger.LogInfo(log);

        public static void LogError(object log) =>
            Plugin.PluginLogger.LogError(log);

        public static void LogWarning(object log) =>
            Plugin.PluginLogger.LogDebug(log);
    }
}
