/*
 * ii's Stupid Menu  Patches/Menu/EffectDataPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaLocomotion;
using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "SetHandEffectData")]
    public class EffectDataPatch
    {
        public static bool enabled;
        public static bool tapsEnabled = true;
        public static bool doOverride;
        public static float overrideVolume = 99999f;
        public static int tapMultiplier = 1;
        public static int material = -1;

        private static bool Prefix(VRRig __instance, HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
        {
            if (enabled)
            {
                if (__instance.isLocal)
                {
                    if (doOverride)
                    {
                        effectContext.soundFX = VRRig.LocalRig.GetHandSurfaceData(audioClipIndex).audio;
                        effectContext.speed = overrideVolume;
                        effectContext.soundVolume = overrideVolume;

                        if (PhotonNetwork.InRoom)
                        {
                            if (tapMultiplier > 1)
                            {
                                for (int i = 0; i < tapMultiplier; i++)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                                    {
                                        audioClipIndex,
                                        isLeftHand,
                                        handTapSpeed
                                    });
                                }
                                Main.RPCProtection();
                            }
                        }

                        return false;
                    }

                    if (!tapsEnabled)
                    {
                        effectContext.speed = 0f;
                        effectContext.soundVolume = 0f;

                        GorillaTagger.Instance.handTapVolume = 0f;
                        GorillaTagger.Instance.handTapSpeed = 0f;
                        GorillaTagger.Instance.audioClipIndex = -1;

                        return false;
                    }

                    if (material > 0)
                    {
                        GorillaTagger.Instance.audioClipIndex = material;
                        audioClipIndex = material;

                        if (isLeftHand)
                            GTPlayer.Instance.leftHandMaterialTouchIndex = material;
                        else
                            GTPlayer.Instance.rightHandMaterialTouchIndex = material;

                        effectContext.soundFX = VRRig.LocalRig.GetHandSurfaceData(material).audio;

                        return false;
                    }
                }
            }
            return true;
        }
    }
}
