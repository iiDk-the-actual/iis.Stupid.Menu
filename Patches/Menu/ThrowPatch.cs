/*
 * ii's Stupid Menu  Patches/Menu/ThrowPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
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
