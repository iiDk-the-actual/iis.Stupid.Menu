/*
 * ii's Stupid Menu  Patches/Menu/OverlapPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(BuilderAttachGridPlane), "IsConnected")]
    public class OverlapPatch
    {
        public static bool enabled;

        public static void Postfix(BuilderAttachGridPlane __instance, ref bool __result, SnapBounds bounds)
        {
            if (enabled)
                __result = false;
        }
    }
}
