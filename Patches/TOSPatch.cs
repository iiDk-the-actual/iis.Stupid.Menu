using HarmonyLib;
using hykmMenu.Mods;
using System;
using UnityEngine;
using static hykmMenu.Menu.Main;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(LegalAgreements), "PostUpdate")]
    public class TOSPatch
    {
        public static bool enabled = false;
        private static bool Prefix(LegalAgreements __instance)
        {
            if (enabled)
            {
                __instance.TurnPage(999);
                Traverse.Create(__instance).Field("controllerBehaviour").Field("buttonDown").SetValue(true);
                Traverse.Create(__instance).Field("holdTime").SetValue(0.1f);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ModIOTermsOfUse), "PostUpdate")]
    public class TOSPatch2
    {
        private static bool Prefix(LegalAgreements __instance)
        {
            if (TOSPatch.enabled)
            {
                __instance.TurnPage(999);
                Traverse.Create(__instance).Field("controllerBehaviour").Field("buttonDown").SetValue(true);
                Traverse.Create(__instance).Field("holdTime").SetValue(0.1f);
                return false;
            }
            return true;
        }
    }
}
