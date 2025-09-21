/*
 * ii's Stupid Menu  Patches/Menu/KnockbackPatch.cs
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
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "ApplyKnockback")]
    public class KnockbackPatch
    {
        public static bool enabled;

        public static bool Prefix(Vector3 direction, float speed) =>
            !enabled;
    }
}
