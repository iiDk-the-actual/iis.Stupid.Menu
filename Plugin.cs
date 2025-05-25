using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace iiMenu
{
    [System.ComponentModel.Description(PluginInfo.Description)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static ManualLogSource PluginLogger => instance.Logger;

        private void Awake()
        {
            // Set console title
            Console.Title = $"ii's Stupid Menu // Build {PluginInfo.Version}";
            instance = this;

            Classes.LogManager.Log($@"
 _ _ _       ____  _               _     _   __  __                  
(_|_| )___  / ___|| |_ _   _ _ __ (_) __| | |  \/  | ___ _ __  _   _ 
| | |// __| \___ \| __| | | | '_ \| |/ _` | | |\/| |/ _ \ '_ \| | | |
| | | \__ \  ___) | |_| |_| | |_) | | (_| | | |  | |  __/ | | | |_| |
|_|_| |___/ |____/ \__|\__,_| .__/|_|\__,_| |_|  |_|\___|_| |_|\__,_|
                            |_|                                      
    ii's Stupid Menu Build {PluginInfo.Version}
    Compiled {PluginInfo.BuildTimestamp}
");
        }

        private void Start()
        {
            LoadMenu();
        }

        // For SharpMonoInjector usage
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
    }
}
