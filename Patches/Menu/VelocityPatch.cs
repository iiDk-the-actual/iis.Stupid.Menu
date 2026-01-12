/*
 * ii's Stupid Menu  Patches/Menu/VelocityPatch.cs
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

﻿using GorillaLocomotion.Climbing;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaVelocityTracker), "GetAverageVelocity")]
    public class VelocityPatch
    {
        public static bool enabled;
        public static float multipleFactor;

        public static void Postfix(GorillaVelocityTracker __instance, ref Vector3 __result, bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
        {
            if (enabled)
                __result *= multipleFactor;
        }
    }

    [HarmonyPatch(typeof(GorillaVelocityTracker), "GetLatestVelocity")]
    public class VelocityPatch2
    {
        public static void Postfix(GorillaVelocityTracker __instance, ref Vector3 __result, bool worldSpace = false)
        {
            if (VelocityPatch.enabled)
                __result *= VelocityPatch.multipleFactor;
        }
    }

    [HarmonyPatch(typeof(GorillaVelocityEstimator), "TriggeredLateUpdate")]
    public class VelocityPatch3
    {
        public static void Postfix(GorillaVelocityEstimator __instance)
        {
            if (VelocityPatch.enabled)
            {
                __instance.linearVelocity *= VelocityPatch.multipleFactor;
                __instance.angularVelocity *= VelocityPatch.multipleFactor;
                __instance.handPos *= VelocityPatch.multipleFactor;
            }
        }
    }
}
