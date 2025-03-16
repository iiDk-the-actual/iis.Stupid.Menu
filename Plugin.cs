using BepInEx;
using System;
using UnityEngine;

namespace hykmMenu
{
    [System.ComponentModel.Description(PluginInfo.Description)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start() // To that one dude that uses SMI to inject my menu, it's this method
        {
            Console.Title = "Hayakom Menu // Build " + PluginInfo.Version;

            hykmMenu.Patches.Menu.ApplyHarmonyPatches();
            GameObject Loading = new GameObject("hykm");
            Loading.AddComponent<hykmMenu.UI.Main>();
            Loading.AddComponent<hykmMenu.Notifications.NotifiLib>();
            Loading.AddComponent<hykmMenu.Classes.CoroutineManager>();
            UnityEngine.Object.DontDestroyOnLoad(Loading);
        }
    }
}
