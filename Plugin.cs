using BepInEx;
using System;
using UnityEngine;

namespace iiMenu
{
    [System.ComponentModel.Description(PluginInfo.Description)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start() // To that one dude that uses SMI to inject my menu, it's this method
        {
            Console.Title = "ii's Stupid Menu // Build " + PluginInfo.Version;

            iiMenu.Patches.Menu.ApplyHarmonyPatches();
            GameObject Loading = new GameObject();
            Loading.AddComponent<iiMenu.UI.Main>();
            Loading.AddComponent<iiMenu.Notifications.NotifiLib>();
            Loading.AddComponent<iiMenu.Classes.CoroutineManager>();
            UnityEngine.Object.DontDestroyOnLoad(Loading);
        }
    }
}
