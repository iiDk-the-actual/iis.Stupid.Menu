using HarmonyLib;
using iiMenu.Classes;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(Slingshot), "GetLaunchVelocity")]
    public class GetLaunchPatch
    {
        public static bool enabled;

        public static void Postfix(Slingshot __instance, ref Vector3 __result)
        {
            if (enabled)
                __result = (RigManager.GetClosestVRRig().transform.position + (Vector3.up * 0.5f) - __instance.centerOrigin.transform.position).normalized * 50f;
        }
    }
}
