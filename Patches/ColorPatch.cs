using HarmonyLib;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig))]
    [HarmonyPatch("GetThrowableProjectileColor", MethodType.Normal)]
    internal class ColorPatch
    {
        private static void Postfix(bool isLeftHand, ref Color __result)
        {
            try
            {
                __result = currentProjectileColor;
            }
            catch {}
        }
    }
}
