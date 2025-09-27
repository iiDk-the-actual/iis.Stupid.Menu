/*
 * ii's Stupid Menu  Patches/Menu/HandTapPatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

using HarmonyLib;
﻿using System;
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
