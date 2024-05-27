using System;
using System.ComponentModel;
using BepInEx;
using UnityEngine;

namespace iiMenu.Patches
{
	[Description(PluginInfo.Description)]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class HarmonyPatches : BaseUnityPlugin
	{
		private void OnEnable()
		{
			Menu.ApplyHarmonyPatches();
        }

		private void OnDisable()
		{
			Menu.RemoveHarmonyPatches();
		}

		private void Start()
		{
			Console.Title = "ii's Stupid Menu // Build "+PluginInfo.Version;

            GameObject Loading = new GameObject();
            Loading.AddComponent<iiMenu.UI.Main>();
            Loading.AddComponent<iiMenu.Notifications.NotifiLib>();
            UnityEngine.Object.DontDestroyOnLoad(Loading);
        }
	}
}
