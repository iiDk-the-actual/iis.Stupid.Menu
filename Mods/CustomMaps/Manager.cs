/*
 * ii's Stupid Menu  Mods/CustomMaps/Manager.cs
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GorillaTagScripts.VirtualStumpCustomMaps;
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using iiMenu.Menu;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.CustomMaps
{
    public class Manager
    {
        private static Dictionary<long, string> mapScriptArchives = new Dictionary<long, string>();
        public static void UpdateCustomMapsTab(long? overwriteId = null)
        {
            int category = GetCategory("Custom Maps");
            List<ButtonInfo> buttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Custom Maps", method = () => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." } };

            if (overwriteId != -1 && CustomMapLoader.IsMapLoaded())
            {
                long mapID = overwriteId ?? CustomMapLoader.LoadedMapModId;
                if (!mapScriptArchives.ContainsKey(mapID))
                    mapScriptArchives.Add(mapID, CustomGameMode.LuaScript);

                CustomMap map = GetMapByID(mapID);
                if (map == null)
                    buttons.Add(new ButtonInfo { buttonText = "This map is not supported yet.", label = true });
                else
                    buttons.AddRange(map.Buttons);

                buttons.AddRange(new[]
                {
                    new ButtonInfo { buttonText = " ", label = true },
                    new ButtonInfo { buttonText = "Edit Custom Script", method =() => EditUserScript(), isTogglable = false, toolTip = "Opens your custom script for this map." },
                    new ButtonInfo { buttonText = "Delete Custom Script", method =() => DeleteUserScript(), isTogglable = false, toolTip = "Deletes your custom script for this map." },
                    new ButtonInfo { buttonText = "Run Custom Script", enableMethod =() => StartUserScript(), disableMethod =() => StopUserScript(), toolTip = "Runs your custom script for this map." }
                });
            }
            else
                buttons.Add(new ButtonInfo { buttonText = "You have not loaded a map.", label = true });

            Buttons.buttons[category] = buttons.ToArray();
        }

        public static void ModifyCustomScript(Dictionary<int, string> replacements)
        {
            string input = CustomGameMode.LuaScript;
            string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (KeyValuePair<int, string> kvp in replacements)
            {
                int lineIndex = kvp.Key;
                if (lineIndex >= 0 && lineIndex < lines.Length)
                {
                    LogManager.Log("Replacing " + lines[lineIndex] + " with " + kvp.Value);
                    lines[lineIndex] = kvp.Value;
                }

            }

            CustomGameMode.LuaScript = string.Join(Environment.NewLine, lines);

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();

            CustomMapManager.ReturnToVirtualStump();
        }

        public static void EditUserScript()
        {
            string DirectoryTarget = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau";
            if (!File.Exists(DirectoryTarget))
                File.WriteAllText(DirectoryTarget, mapScriptArchives[CustomMapManager.currentRoomMapModId]);
            Process.Start(DirectoryTarget);
        }

        public static void DeleteUserScript()
        {
            string DirectoryTarget = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau";
            if (File.Exists(DirectoryTarget))
                File.Delete(DirectoryTarget);
        }

        public static void StartUserScript()
        {
            string DirectoryTarget = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau";
            if (File.Exists(DirectoryTarget))
                CustomGameMode.LuaScript = File.ReadAllText(DirectoryTarget);

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();
            CustomMapManager.ReturnToVirtualStump();
        }

        public static void StopUserScript()
        {
            CustomGameMode.LuaScript = mapScriptArchives[CustomMapManager.currentRoomMapModId];

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();
            CustomMapManager.ReturnToVirtualStump();
        }

        public static void RevertCustomScript(int[] lines)
        {
            Dictionary<int, string> replacements = new Dictionary<int, string>();
            foreach (int line in lines)
                replacements.Add(line, mapScriptArchives[CustomMapManager.currentRoomMapModId].Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[line]);

            ModifyCustomScript(replacements);
        }

        public static void RevertCustomScript(int line) =>
            RevertCustomScript(new[] { line });

        public static Dictionary<long, CustomMap> mapCache = new Dictionary<long, CustomMap>();
        public static CustomMap GetMapByID(long id)
        {
            if (!mapCache.TryGetValue(id, out var instance))
            {
                var mapTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CustomMap)));

                foreach (var type in mapTypes)
                {
                    CustomMap mapInstance = (CustomMap)Activator.CreateInstance(type);
                    if (mapInstance.MapID == id)
                    {
                        instance = mapInstance;
                        break;
                    }
                }

                mapCache[id] = instance;
            }

            return instance;
        }
    }
}