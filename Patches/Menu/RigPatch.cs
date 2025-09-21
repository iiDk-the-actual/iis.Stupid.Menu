/*
 * ii's Stupid Menu  Patches/Menu/RigPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    public class RigPatch
    {
        public static bool Prefix(VRRig __instance) =>
            !__instance.isLocal;
    }

    [HarmonyPatch(typeof(VRRig), "Awake")]
    public class RigPatch2
    {
        public static bool Prefix(VRRig __instance) =>
            __instance.gameObject.name != "Local Gorilla Player(Clone)";
    }
}
