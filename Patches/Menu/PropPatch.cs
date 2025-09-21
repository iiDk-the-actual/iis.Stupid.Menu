/*
 * ii's Stupid Menu  Patches/Menu/PropPatch.cs
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
    [HarmonyPatch(typeof(PropHuntHandFollower), "GeoCollisionPoint")]
    public class PropPatch
    {
        public static bool enabled;

        public static void Postfix(ref Vector3 __result, Vector3 sourcePos, Vector3 targetPos)
        {
            if (enabled)
                __result = targetPos;
        }
    }
}
