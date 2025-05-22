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
            Console.Title = "ii's Stupid Menu // Build " + PluginInfo.Version;
            instance = this;
        }

        private void Start()
        {
            LoadMenu();
        }

        // For SharpMonoInjector usage
        private static void LoadMenu()
        {
            Console.Title = "ii's Stupid Menu // Build " + PluginInfo.Version;

            Patches.Menu.ApplyHarmonyPatches();

            GameObject Loader = new GameObject("iiMenu_Loader");
            Loader.AddComponent<UI.Main>();
            Loader.AddComponent<Notifications.NotifiLib>();
            Loader.AddComponent<Classes.CoroutineManager>();

            DontDestroyOnLoad(Loader);
        }
    }
}
