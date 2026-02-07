/*
 * ii's Stupid Menu  Patches/Menu/RopePatch.cs
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

﻿using GorillaLocomotion.Gameplay;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaRopeSwing), nameof(GorillaRopeSwing.AttachLocalPlayer))]
    public class RopePatch
    {
        public static bool enabled;
        public static readonly float amplifier = 5f;

        public static void Prefix(XRNode xrNode, Transform grabbedBone, Vector3 offset, ref Vector3 velocity)
        {
            if (enabled)
                velocity *= amplifier;
        }
    }
}
