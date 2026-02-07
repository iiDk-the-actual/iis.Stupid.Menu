/*
 * ii's Stupid Menu  Patches/Menu/EffectDataPatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

﻿using GorillaLocomotion;
using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.SetHandEffectData))]
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
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, audioClipIndex, isLeftHand, handTapSpeed);
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
                            GTPlayer.Instance.leftHand.materialTouchIndex = material;
                        else
                            GTPlayer.Instance.rightHand.materialTouchIndex = material;

                        effectContext.soundFX = VRRig.LocalRig.GetHandSurfaceData(material).audio;

                        return false;
                    }
                }
            }
            return true;
        }
    }
}
