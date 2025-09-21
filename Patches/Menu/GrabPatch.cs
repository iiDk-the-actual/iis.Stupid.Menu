/*
 * ii's Stupid Menu  Patches/Menu/GrabPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "GrabbedByPlayer")]
    public class GrabPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand) =>
            !(enabled);
    }

    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class DropPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity) =>
            !GrabPatch.enabled;
    }

    [HarmonyPatch(typeof(GuardianRPCs), "GuardianLaunchPlayer")]
    public class LaunchPatch
    {
        public static bool Prefix(Vector3 velocity) =>
            !GrabPatch.enabled;
    }
}
