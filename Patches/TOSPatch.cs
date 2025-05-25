using HarmonyLib;
using iiMenu.Mods;
using System;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(LegalAgreements), "Update")]
    public class TOSPatch
    {
        public static bool enabled = false;
        private static bool Prefix(LegalAgreements __instance)
        {
            if (enabled)
            {
                __instance.controllerBehaviour.isDownStick = true;
                __instance.scrollSpeed = 10f;
                __instance._maxScrollSpeed = 10f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ModIOTermsOfUse), "PostUpdate")]
    public class TOSPatch2
    {
        private static bool Prefix(ModIOTermsOfUse __instance)
        {
            if (TOSPatch.enabled)
            {
                __instance.TurnPage(999);
                __instance.controllerBehaviour.isDownStick = true;
                __instance.holdTime = 0.1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AgeSlider), "PostUpdate")]
    public class TOSPatch3
    {
        public static bool enabled = false;
        private static bool Prefix(AgeSlider __instance)
        {
            if (enabled)
            {
                __instance.controllerBehaviour.buttonDown = true;
                __instance.holdTime = 0.1f;
                return false;
            }
            return true;
        }
    }
}
