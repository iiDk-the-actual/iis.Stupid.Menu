using GorillaTagScripts.VirtualStumpCustomMaps;
using HarmonyLib;
using ModIO;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(CustomMapManager), "LoadMap")]
    public class LoadModPatch
    {
        public static void Prefix(ModId modId) =>
            Mods.CustomMaps.Manager.UpdateCustomMapsTab(modId.id);
    }

    [HarmonyPatch(typeof(CustomMapManager), "UnloadMap")]
    public class UnloadModPatch
    {
        public static void Prefix(bool returnToSinglePlayerIfInPublic) =>
            Mods.CustomMaps.Manager.UpdateCustomMapsTab(-1);
    }
}
