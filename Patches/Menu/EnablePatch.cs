using HarmonyLib;
using iiMenu.Mods;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GrowingSnowballThrowable), "OnEnable")]
    public class EnablePatch
    {
        public static bool enabled;

        public static void Postfix(GrowingSnowballThrowable __instance)
        {
            if (enabled)
                __instance.IncreaseSize(Overpowered.snowballScale);
        }
    }
}
