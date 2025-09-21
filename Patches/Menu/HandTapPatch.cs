/*
 * ii's Stupid Menu  Patches/Menu/HandTapPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using System;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRigSerializer), "OnHandTapRPCShared")]
    public class HandTapPatch
    {
        public static Action<VRRig, Vector3> OnHandTap;
        public static bool enabled;

        public static bool Prefix(VRRigSerializer __instance, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
        {
            OnHandTap?.Invoke(__instance.vrrig, isLeftHand ? __instance.vrrig.leftHandTransform.position : __instance.vrrig.rightHandTransform.position);

            return !enabled;
        }
    }
}
