using BepInEx;
using BepInEx.Logging;
using System;
using System.IO;
using UnityEngine;

namespace iiMenu
{
    [System.ComponentModel.Description(PluginInfo.Description)]
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

            Classes.LogManager.Log($@"

     ••╹   ┏┓     • ┓  ┳┳┓      
     ┓┓ ┏  ┗┓╋┓┏┏┓┓┏┫  ┃┃┃┏┓┏┓┓┏
     ┗┗ ┛  ┗┛┗┗┻┣┛┗┗┻  ┛ ┗┗ ┛┗┗┻
                ┛               
    ii's Stupid Menu  {(PluginInfo.BetaBuild ? "Beta " : "Build")} {PluginInfo.Version}
    Compiled {PluginInfo.BuildTimestamp}
");

            FirstLaunch = !Directory.Exists("iisStupidMenu");

            string[] ExistingDirectories = new string[]
            {
                "iisStupidMenu",
                "iisStupidMenu/Sounds",
                "iisStupidMenu/Plugins",
                "iisStupidMenu/Backups",
                "iisStupidMenu/TTS",
                "iisStupidMenu/PlayerInfo"
            };

            foreach (string DirectoryString in ExistingDirectories)
            {
                if (!Directory.Exists(DirectoryString))
                    Directory.CreateDirectory(DirectoryString);
            }
        }

        private void Start() => LoadMenu();

        private static void LoadMenu()
        {
            Console.Title = $"ii's Stupid Menu // Build {PluginInfo.Version}";

            Patches.Menu.ApplyHarmonyPatches();

            GameObject Loader = new GameObject("iiMenu_Loader");
            Loader.AddComponent<UI.Main>();
            Loader.AddComponent<Notifications.NotifiLib>();
            Loader.AddComponent<Classes.CoroutineManager>();

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
