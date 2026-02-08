/*
 * ii's Stupid Menu  Managers/PluginManager.cs
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

using iiMenu.Classes.Menu;
using iiMenu.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.FileUtilities;

namespace iiMenu.Managers
{
    public class PluginManager
    {
        public class Plugin
        {
            public string FileName;
            public bool Enabled;

            public string Name;
            public string Description;

            public Assembly Assembly;
        }

        public static readonly List<Plugin> Plugins = new List<Plugin>();
        public static void LoadPlugins()
        {
            Buttons.buttons[Buttons.GetCategory("Plugin Settings")] = new[] { new ButtonInfo { buttonText = "Exit Plugin Settings", method = () => Buttons.CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu." } };

            if (Plugins.Count > 0)
            {
                foreach (var Plugin in Plugins.Where(Plugin => Plugin.Enabled))
                    DisablePlugin(Plugin.Assembly);
            }

            cacheAssembly.Clear();

            cacheUpdate.Clear();
            cacheOnGUI.Clear();

            Plugins.Clear();

            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/Plugins"))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/Plugins");

            string[] disabledPlugins = { };
            if (!File.Exists($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt", "");
            else
            {
                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt");
                if (text.Length > 1)
                    disabledPlugins = text.Split("\n");
            }

            string[] Files = Directory.GetFiles($"{PluginInfo.BaseDirectory}/Plugins");
            foreach (string File in Files)
            {
                try
                {
                    if (GetFileExtension(File) != "dll") continue;
                    string pluginName = File.Replace($"{PluginInfo.BaseDirectory}/Plugins/", "");

                    Assembly assembly = GetAssembly(File);
                    string[] pluginData = GetPluginInfo(assembly);

                    Plugin plugin = new Plugin()
                    {
                        FileName = pluginName,
                        Name = pluginData[0],
                        Description = pluginData[1],
                        Assembly = GetAssembly(File),
                        Enabled = !disabledPlugins.Contains(pluginName)
                    };

                    if (plugin.Enabled)
                        EnablePlugin(plugin.Assembly);

                    Plugins.Add(plugin);
                }
                catch (Exception e) { LogManager.Log("Error with loading plugin " + File + ": " + e); }
            }

            foreach (Plugin Plugin in Plugins)
            {
                try
                {
                    Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo { buttonText = Plugin.FileName, overlapText = (Plugin.Enabled ? "<color=grey>[</color><color=green>ON</color><color=grey>]</color>" : "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>") + " " + Plugin.Name, method = () => TogglePlugin(Plugin), isTogglable = false, toolTip = Plugin.Description });
                }
                catch (Exception e) { LogManager.Log("Error with enabling plugin " + Plugin.Name + ": " + e); }
            }

            Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo { buttonText = "Open Plugins Folder", method = OpenPluginsFolder, isTogglable = false, toolTip = "Opens a folder containing all of your plugins." });
            Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo { buttonText = "Reload Plugins", method = ReloadPlugins, isTogglable = false, toolTip = "Reloads all of your plugins." });
            Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo { buttonText = "Get More Plugins", method = LoadPluginLibrary, isTogglable = false, toolTip = "Opens a public plugin library, where you can download your own plugins." });
        }

        public static void DownloadPlugin(string name, string url)
        {
            if (name.Contains(".."))
                name = name.Replace("..", "");

            string filename = url.Split("/")[^1];

            if (File.Exists($"{PluginInfo.BaseDirectory}/Plugins/" + filename))
                File.Delete($"{PluginInfo.BaseDirectory}/Plugins/" + filename);

            WebClient stream = new WebClient();
            stream.DownloadFile(url, $"{PluginInfo.BaseDirectory}/Plugins/" + filename);

            LoadPlugins();
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to your plugins.");
        }

        public static void TogglePlugin(Plugin plugin)
        {
            if (plugin.Enabled)
                DisablePlugin(plugin.Assembly);
            else
                EnablePlugin(plugin.Assembly);

            plugin.Enabled = !plugin.Enabled;

            string disabledPluginsString = Plugins.Where(plugin => !plugin.Enabled).Select(plugin => plugin.FileName).Aggregate("", (current, disabledPlugin) => current + (disabledPlugin + "\n"));

            File.WriteAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt", disabledPluginsString);

            Buttons.GetIndex(plugin.FileName).overlapText = (plugin.Enabled ? "<color=grey>[</color><color=green>ON</color><color=grey>]</color>" : "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>") + " " + plugin.Name;
        }

        public static void ExecuteUpdate()
        {
            foreach (Plugin plugin in Plugins.Where(plugin => plugin.Enabled))
            {
                try
                {
                    PluginUpdate(plugin.Assembly);
                }
                catch (Exception e) { LogManager.Log("Error with Update() with plugin " + plugin.Name + ": " + e); }
            }
        }

        public static void ExecuteOnGUI()
        {
            foreach (Plugin plugin in Plugins.Where(plugin => plugin.Enabled))
            {
                try
                {
                    PluginOnGUI(plugin.Assembly);
                }
                catch (Exception e) { LogManager.Log("Error with OnGUI() with plugin " + plugin.Name + ": " + e); }
            }
        }

        private static readonly Dictionary<string, Assembly> cacheAssembly = new Dictionary<string, Assembly>();
        private static Assembly GetAssembly(string dllName)
        {
            if (cacheAssembly.TryGetValue(dllName, out var assembly))
                return assembly;

            Assembly Assembly = Assembly.Load(File.ReadAllBytes(dllName.Replace("/", "\\")));
            cacheAssembly.Add(dllName, Assembly);
            return Assembly;
        }

        private static string[] GetPluginInfo(Assembly Assembly)
        {
            Type[] Types = Assembly.GetTypes();
            foreach (Type Type in Types)
            {
                FieldInfo Name = Type.GetField("Name", BindingFlags.Public | BindingFlags.Static);
                FieldInfo Description = Type.GetField("Description", BindingFlags.Public | BindingFlags.Static);
                if (Name != null && Description != null)
                    return new[] { (string)Name.GetValue(null), (string)Description.GetValue(null) };
            }

            return new[] { "null", "null" };
        }

        private static void EnablePlugin(Assembly Assembly)
        {
            Type[] Types = Assembly.GetTypes();
            foreach (Type Type in Types)
            {
                try
                {
                    MethodInfo Method = Type.GetMethod("OnEnable", BindingFlags.Public | BindingFlags.Static);
                    Method?.Invoke(null, null);
                }
                catch { }
            }
        }

        private static void DisablePlugin(Assembly Assembly)
        {
            Type[] Types = Assembly.GetTypes();
            foreach (Type Type in Types)
            {
                try
                {
                    MethodInfo Method = Type.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.Static);
                    Method?.Invoke(null, null);
                }
                catch { }
            }
        }

        private static readonly Dictionary<Assembly, MethodInfo[]> cacheOnGUI = new Dictionary<Assembly, MethodInfo[]>();
        private static void PluginOnGUI(Assembly Assembly)
        {
            if (cacheOnGUI.TryGetValue(Assembly, out var value))
            {
                foreach (MethodInfo Method in value)
                    Method.Invoke(null, null);
            }
            else
            {
                Type[] Types = Assembly.GetTypes();
                List<MethodInfo> Methods = Types.Select(Type => Type.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static)).Where(Method => Method != null).ToList();

                cacheOnGUI.Add(Assembly, Methods.ToArray());

                foreach (MethodInfo Method in Methods)
                    Method.Invoke(null, null);
            }
        }

        private static readonly Dictionary<Assembly, MethodInfo[]> cacheUpdate = new Dictionary<Assembly, MethodInfo[]>();
        private static void PluginUpdate(Assembly Assembly)
        {
            if (cacheUpdate.TryGetValue(Assembly, out var value))
            {
                foreach (MethodInfo Method in value)
                    Method.Invoke(null, null);
            }
            else
            {
                Type[] Types = Assembly.GetTypes();
                List<MethodInfo> Methods = Types.Select(Type => Type.GetMethod("Update", BindingFlags.Public | BindingFlags.Static)).Where(Method => Method != null).ToList();

                cacheUpdate.Add(Assembly, Methods.ToArray());

                foreach (MethodInfo Method in Methods)
                    Method.Invoke(null, null);
            }
        }

        #region Menu Integration
        public static void ReloadPlugins()
        {
            Mods.Settings.SavePreferences();
            LoadPlugins();
            Mods.Settings.LoadPreferences();

            if (isSearching)
                Mods.Settings.Search();

            Buttons.CurrentCategoryName = "Main";
        }

        public static void OpenPluginsFolder() =>
            Process.Start(GetGamePath() + $"/{PluginInfo.BaseDirectory}/Plugins");

        public static void LoadPluginLibrary()
        {
            string library = GetHttp($"{PluginInfo.ServerResourcePath}/Plugins/PluginLibrary.txt");
            string[] plugins = AlphabetizeNoSkip(library.Split("\n"));

            List<ButtonInfo> buttonInfos = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Plugin Library", method = () => Buttons.CurrentCategoryName = "Plugin Settings", isTogglable = false, toolTip = "Returns you back to the plugin settings." } };
            int index = 0;

            foreach (string plugin in plugins)
            {
                if (plugin.Length <= 2) continue;
                index++;
                string[] Data = plugin.Split(";");
                buttonInfos.Add(new ButtonInfo { buttonText = "PluginDownload" + index, overlapText = Data[0], method = () => DownloadPlugin(Data[0], Data[2]), isTogglable = false, toolTip = Data[1] });
            }
            
            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttonInfos.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        #endregion
    }
}
