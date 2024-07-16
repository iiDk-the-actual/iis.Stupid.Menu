using BepInEx;
using System;
using System.ComponentModel;
using UnityEngine;

namespace iiMenu
{
    [Description(PluginInfo.Description)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            Console.Title = "ii's Stupid Menu // Build " + PluginInfo.Version;

            iiMenu.Patches.Menu.ApplyHarmonyPatches();
            GameObject Loading = new GameObject(); // To that one dude that uses SMI to inject my menu, it's this method
            Loading.AddComponent<iiMenu.UI.Main>();
            Loading.AddComponent<iiMenu.Notifications.NotifiLib>();
            UnityEngine.Object.DontDestroyOnLoad(Loading);
        }
    }
}
