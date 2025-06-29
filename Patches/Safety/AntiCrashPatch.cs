using GorillaExtensions;
using HarmonyLib;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class AntiCrashPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
        {
            if (AntiCrashToggle && __instance == VRRig.LocalRig && !GTExt.IsValid(throwVelocity))
                return false;
            
            return true;
        }
    }
}
