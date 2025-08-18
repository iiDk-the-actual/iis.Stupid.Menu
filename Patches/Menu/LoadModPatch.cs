using GorillaTagScripts.ModIO;
using HarmonyLib;
using iiMenu.Mods;
using ModIO;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(CustomMapManager), "LoadMod")]
    public class LoadModPatch
    {
        public static void Prefix(ModId modId) =>
            Fun.CustomMaps.UpdateCustomMapsTab(modId.id);
    }

    [HarmonyPatch(typeof(CustomMapManager), "UnloadMod")]
    public class UnloadModPatch
    {
        public static void Prefix(bool returnToSinglePlayerIfInPublic) =>
            Fun.CustomMaps.UpdateCustomMapsTab(-1);
    }
}
