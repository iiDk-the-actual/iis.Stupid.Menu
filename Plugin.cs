using BepInEx;
using BepInEx.Logging;
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches;
using iiMenu.Patches.Menu;
using System.IO;
using System.Linq;
using UnityEngine;
using Console = System.Console;

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

            LogManager.Log($@"

     ••╹   ┏┓     • ┓  ┳┳┓      
     ┓┓ ┏  ┗┓╋┓┏┏┓┓┏┫  ┃┃┃┏┓┏┓┓┏
     ┗┗ ┛  ┗┛┗┗┻┣┛┗┗┻  ┛ ┗┗ ┛┗┗┻
                ┛               
    ii's Stupid Menu  {(PluginInfo.BetaBuild ? "Beta " : "Build")} {PluginInfo.Version}
    Compiled {PluginInfo.BuildTimestamp}
");

            FirstLaunch = !Directory.Exists(PluginInfo.BaseDirectory);

            string[] ExistingDirectories = new string[]
            {
                "",
                "/Sounds",
                "/Plugins",
                "/Backups",
                "/TTS",
                "/PlayerInfo",
                "/CustomScripts"
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
                    TOSPatch.enabled = true;
            }

            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_DisableTelemetry.txt"))
                ServerData.DisableTelemetry = true;
            
            GorillaTagger.OnPlayerSpawned(LoadMenu);
        }

        private static void LoadMenu()
        {
            PatchHandler.PatchAll();

            GameObject Loader = new GameObject("iiMenu_Loader");
            Loader.AddComponent<UI>();
            Loader.AddComponent<Notifications.NotifiLib>();
            Loader.AddComponent<CoroutineManager>();

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
