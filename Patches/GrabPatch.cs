using GorillaExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "GrabbedByPlayer")]
    public class GrabPatch
    {
        public static bool enabled = false;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
        {
            if (enabled && __instance == GorillaTagger.Instance.offlineVRRig)
            {
                return false;
            }
            return true;
        }
    }
}
