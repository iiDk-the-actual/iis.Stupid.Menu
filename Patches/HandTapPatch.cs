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
    [HarmonyPatch(typeof(VRRig), "OnHandTap")]
    public class HandTapPatch
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
                    if (doOverride)
                    {
                        handSpeed = overrideVolume;
                        tapDir = new Vector3(overrideVolume, overrideVolume, overrideVolume);
                        GorillaTagger.Instance.handTapVolume = overrideVolume;
                        typeof(GorillaTagger).GetField("tempHitDir", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GorillaTagger.Instance, new Vector3(overrideVolume, overrideVolume, overrideVolume));
                        if (PhotonNetwork.InRoom)
                        {
                            GorillaTagger.Instance.myVRRig.SendRPC("OnHandTapRPC", PhotonNetwork.LocalPlayer, new object[]
                            {
                                soundIndex,
                                isLeftHand,
                                handSpeed,
                                Utils.PackVector3ToLong(tapDir)
                            });
                        } else
                        {
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundIndex, isLeftHand, handSpeed);
                        }
                        return false;
                    }
                    if (!tapsEnabled)
                    {
                        handSpeed = 0f;
                        GorillaTagger.Instance.handTapVolume = 0f;
                        typeof(GorillaTagger).GetField("tempInt", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GorillaTagger.Instance, (int)-1);
                        soundIndex = -1;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
