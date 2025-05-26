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

            if (!Directory.Exists("iisStupidMenu"))
                Directory.CreateDirectory("iisStupidMenu");

            if (!Directory.Exists("iisStupidMenu/Sounds"))
                Directory.CreateDirectory("iisStupidMenu/Sounds");

            if (!Directory.Exists("iisStupidMenu/Plugins"))
                Directory.CreateDirectory("iisStupidMenu/Plugins");

            if (!Directory.Exists("iisStupidMenu/Backups"))
                Directory.CreateDirectory("iisStupidMenu/Backups");
        }

        private void Start() => LoadMenu();

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
