using System;
using System.ComponentModel;
using BepInEx;
using UnityEngine;

namespace iiMenu.Patches
{
	[Description(PluginInfo.Description)]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    //[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.11")]
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
        }
	}
}
