using System.ComponentModel;
using BepInEx;

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
	}
}
