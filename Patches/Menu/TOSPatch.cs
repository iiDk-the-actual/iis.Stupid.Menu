using System.Threading.Tasks;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(LegalAgreements), "Update")]
    public class TOSPatch
    {
        public static bool enabled;
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

    [HarmonyPatch(typeof(ModIOTermsOfUse_v1), "PostUpdate")]
    public class TOSPatch2
    {
        private static bool Prefix(ModIOTermsOfUse_v1 __instance)
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
        private static bool Prefix(AgeSlider __instance)
        {
            if (TOSPatch.enabled)
            {
                __instance.controllerBehaviour.buttonDown = true;
                __instance.holdTime = 0.1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PrivateUIRoom), "StartOverlay")]
    public class TOSPatch4
    {
        private static bool Prefix() =>
            !TOSPatch.enabled;
    }

    [HarmonyPatch(typeof(KIDManager), "UseKID")]
    public class TOSPatch5
    {
        private static bool Prefix(ref Task<bool> __result)
        {
            if (!TOSPatch.enabled)
                return true;

            __result = Task.FromResult(false);
            return false;
        }
    }
}
