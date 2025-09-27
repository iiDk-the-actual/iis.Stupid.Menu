/*
 * ii's Stupid Menu  Patches/Menu/ThrowPatch.cs
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

﻿using HarmonyLib;
using iiMenu.Menu;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GrowingSnowballThrowable), "PerformSnowballThrowAuthority")]
    public class ThrowPatch
    {
        public static bool enabled;
        public static int extraCount = 5;

        public static bool Prefix(GrowingSnowballThrowable __instance)
        {
            if (enabled)
            {
                enabled = false;

                Vector3 originalLinearVelocity = __instance.velocityEstimator.linearVelocity;
                for (int i = 0; i < extraCount; i++)
                {
                    __instance.velocityEstimator.linearVelocity = originalLinearVelocity + Main.RandomVector3(2f);
                    __instance.PerformSnowballThrowAuthority();
                }

                enabled = true;

                return false;
            }

            return true;
        }
    }
}
