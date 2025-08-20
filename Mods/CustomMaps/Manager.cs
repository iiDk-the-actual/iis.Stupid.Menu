using GorillaTagScripts.ModIO;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods.CustomMaps.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.CustomMaps
{
    public class Manager
    {
        private static Dictionary<long, string> mapScriptArchives = new Dictionary<long, string> { };
        public static void UpdateCustomMapsTab(long? overwriteId = null)
        {
            int category = GetCategory("Custom Maps");
            List<ButtonInfo> buttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Custom Maps", method = () => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." } };

            if (overwriteId != -1 && CustomMapLoader.IsModLoaded())
            {
                long mapID = overwriteId ?? CustomMapLoader.LoadedMapModId;
                if (!mapScriptArchives.ContainsKey(mapID))
                    mapScriptArchives.Add(mapID, CustomGameMode.LuaScript);

                CustomMap map = GetMapByID(mapID);
                if (map == null)
                    buttons.Add(new ButtonInfo { buttonText = "This map is not supported yet.", label = true });
                else
                {
                    buttons.AddRange(map.Buttons);
                }

                buttons.Add(new ButtonInfo { buttonText = " ", label = true });
                buttons.Add(new ButtonInfo { buttonText = "Edit custom script", method =() => editUserScript(), isTogglable = false, toolTip = "Opens your custom script for this map." });
                buttons.Add(new ButtonInfo { buttonText = "Delete custom script", method =() => deleteUserScript(), isTogglable = false, toolTip = "Deletes your custom script for this map." });
                buttons.Add(new ButtonInfo { buttonText = "Run custom script", enableMethod =() => startUserScript(), disableMethod =() => stopUserScript(), toolTip = "Runs your custom script for this map." });
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

        public static void editUserScript()
        {
            string DirectoryTarget = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.lua").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.lua";
            if (!System.IO.File.Exists(DirectoryTarget))
                System.IO.File.WriteAllText(DirectoryTarget, mapScriptArchives[CustomMapManager.currentRoomMapModId]);
            System.Diagnostics.Process.Start(DirectoryTarget);
        }

        public static void deleteUserScript()
        {
            string DirectoryTarget = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.lua").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.lua";
            if (System.IO.File.Exists(DirectoryTarget))
                System.IO.File.Delete(DirectoryTarget);
        }

        public static void startUserScript()
        {
            string DirectoryTarget = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.lua").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.lua";
            if (System.IO.File.Exists(DirectoryTarget))
                CustomGameMode.LuaScript = System.IO.File.ReadAllText(DirectoryTarget);
                
            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();
            CustomMapManager.ReturnToVirtualStump();
        }

        public static void stopUserScript()
        {
            CustomGameMode.LuaScript = mapScriptArchives[CustomMapManager.currentRoomMapModId];

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();
            CustomMapManager.ReturnToVirtualStump();
        }

        public static void RevertCustomScript(int[] lines)
        {
            Dictionary<int, string> replacements = new Dictionary<int, string> { };
            foreach (int line in lines)
                replacements.Add(line, mapScriptArchives[CustomMapManager.currentRoomMapModId].Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[line]);

            ModifyCustomScript(replacements);
        }

        public static void RevertCustomScript(int line) =>
            RevertCustomScript(new int[] { line });

        public static CustomMap GetMapByID(long id)
        {
            var mapTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CustomMap)));

            foreach (var type in mapTypes)
            {
                var instance = (CustomMap)Activator.CreateInstance(type);
                if (instance.MapID == id)
                    return instance;
            }

            return null;
        }

    }
}
