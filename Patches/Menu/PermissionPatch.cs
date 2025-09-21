/*
 * ii's Stupid Menu  Patches/Menu/PermissionPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(KIDManager), "HasPermissionToUseFeature")]
    public class PermissionPatch
    {
        public static bool enabled;

        public static void Postfix(ref bool __result)
        {
            if (enabled)
                __result = true;
        }
    }
}
