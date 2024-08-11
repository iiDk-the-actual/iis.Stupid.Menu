using GorillaLocomotion.Climbing;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using Photon.Realtime;
using Valve.VR;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig))]
    [HarmonyPatch("OnHandTap", MethodType.Normal)]
    internal class HandTapPatch
    {
        public static bool doPatch = false;
        public static bool tapsEnabled = true;
        public static bool doOverride = false;
        public static float overrideVolume = 99999f;

        private static bool Prefix(VRRig __instance, int soundIndex, bool isLeftHand, float handSpeed, Vector3 tapDir)
        {
            if (doPatch)
            {
                if (__instance == GorillaTagger.Instance.offlineVRRig)
                {
                    if (!tapsEnabled)
                    {
                        handSpeed = 0f;
                        GorillaTagger.Instance.handTapVolume = 0f;
                        typeof(GorillaTagger).GetField("tempInt", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GorillaTagger.Instance, (int)-1);
                        soundIndex = -1;
                        return false;
                    }
                    if (doOverride)
                    {
                        handSpeed = overrideVolume;
                        GorillaTagger.Instance.handTapVolume = overrideVolume;
                    }
                }
            }
            return true;
        }
    }
}
