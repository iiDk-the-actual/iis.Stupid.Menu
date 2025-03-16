using HarmonyLib;
using UnityEngine;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(GrowingSnowballThrowable), "OnEnable")]
    public class EnablePatch
    {
        public static bool enabled = false;

        public static void Postfix(GrowingSnowballThrowable __instance)
        {
            if (enabled)
                __instance.IncreaseSize(9999);
        }
    }
}
