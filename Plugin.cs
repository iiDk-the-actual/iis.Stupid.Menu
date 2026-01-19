/*
 * ii's Stupid Menu  Plugin.cs
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

using BepInEx;
using BepInEx.Logging;
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches;
using iiMenu.Patches.Menu;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using Console = System.Console;

namespace iiMenu
{
    [Description(PluginInfo.Description)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static ManualLogSource PluginLogger => instance.Logger;
        public static bool FirstLaunch;

        private void Awake()
        {
            // Set console title
            Console.Title = $"ii's Stupid Menu // Build {PluginInfo.Version}";
            instance = this;

            string logoLines = PluginInfo.Logo.Split(@"
")
                .Aggregate("", (current, line) => current + (System.Environment.NewLine + "     " + line));

            LogManager.Log($@"
{logoLines}
    ii's Stupid Menu  {(PluginInfo.BetaBuild ? "Beta " : "Build")} {PluginInfo.Version}
    Compiled {PluginInfo.BuildTimestamp}
    
    This program comes with ABSOLUTELY NO WARRANTY;
    for details see `https://github.com/iiDk-the-actual/iis.Stupid.Menu/GPL/WARRANTY`
    
    This is free software, and you are welcome to redistribute it under certain conditions;
    see `https://github.com/iiDk-the-actual/iis.Stupid.Menu/GPL/REDISTRIBUTION` for details.
");

            FirstLaunch = !Directory.Exists(PluginInfo.BaseDirectory);

            string[] ExistingDirectories = {
                "",
                "/Sounds",
                "/Plugins",
                "/Backups",
                "/Macros",
                "/TTS",
                "/PlayerInfo",
                "/CustomScripts",
                "/Friends",
                "/Friends/Messages",
                "/Achievements"
            };

            foreach (string DirectoryString in ExistingDirectories)
            {
                string DirectoryTarget = $"{PluginInfo.BaseDirectory}{DirectoryString}";
                if (!Directory.Exists(DirectoryTarget))
                    Directory.CreateDirectory(DirectoryTarget);
            }

            // Ugily hard-coded but works so well
            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt"))
            {
                if (File.ReadAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt")[0].Split(";;").Contains("Accept TOS"))
                    TOSPatches.enabled = true;
            }

            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_DisableTelemetry.txt"))
                ServerData.DisableTelemetry = true;
            
            GorillaTagger.OnPlayerSpawned(LoadMenu);
        }

        private void OnDestroy() =>
            Main.UnloadMenu();

        private static void LoadMenu()
        {
            PatchHandler.PatchAll();

            GameObject Loader = new GameObject("iiMenu_Loader");
            Loader.AddComponent<CoroutineManager>();
            Loader.AddComponent<NotificationManager>();
            Loader.AddComponent<CustomBoardManager>();

            Loader.AddComponent<UI>();

            DontDestroyOnLoad(Loader);
        }

        // For SharpMonoInjector usage
        // Don't merge these methods, it just doesn't work
        public static void Inject()
        {
            GameObject iiMenu = new GameObject("iiMenu");
            iiMenu.AddComponent<Plugin>();
        }

        public static void InjectDontDestroy()
        {
            GameObject iiMenu = new GameObject("iiMenu");
            iiMenu.AddComponent<Plugin>();
            DontDestroyOnLoad(iiMenu);
        }
    }
}
