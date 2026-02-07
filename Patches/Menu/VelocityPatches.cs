/*
 * ii's Stupid Menu  Patches/Menu/VelocityPatches.cs
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
    public class VelocityPatches
    {
        public static bool enabled;
        public static float multipleFactor;

        [HarmonyPatch(typeof(GorillaVelocityTracker), nameof(GorillaVelocityTracker.GetAverageVelocity))]
        public class VelocityPatch
        {
            public static void Postfix(GorillaVelocityTracker __instance, ref Vector3 __result, bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
            {
                if (enabled)
                    __result *= multipleFactor;
            }
        }

        [HarmonyPatch(typeof(GorillaVelocityTracker), nameof(GorillaVelocityTracker.GetLatestVelocity))]
        public class VelocityPatch2
        {
            public static void Postfix(GorillaVelocityTracker __instance, ref Vector3 __result, bool worldSpace = false)
            {
                if (enabled)
                    __result *= multipleFactor;
            }
        }

        [HarmonyPatch(typeof(GorillaVelocityEstimator), nameof(GorillaVelocityEstimator.TriggeredLateUpdate))]
        public class VelocityPatch3
        {
            public static void Postfix(GorillaVelocityEstimator __instance)
            {
                if (enabled)
                {
                    __instance.linearVelocity *= multipleFactor;
                    __instance.angularVelocity *= multipleFactor;
                    __instance.handPos *= multipleFactor;
                }
            }
        }
    }
}
