/*
 * ii's Stupid Menu  Patches/Menu/RopePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaLocomotion.Gameplay;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaRopeSwing), "AttachLocalPlayer")]
    public class RopePatch
    {
        public static bool enabled;
        public static float amplifier = 5f;

        public static void Prefix(XRNode xrNode, Transform grabbedBone, Vector3 offset, ref Vector3 velocity)
        {
            if (enabled)
                velocity *= amplifier;
        }
    }
}
