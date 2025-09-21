/*
 * ii's Stupid Menu  Patches/Menu/PlayerSerializePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using System;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "SerializeReadShared")]
    public class PlayerSerializePatch
    {
        public static event Action<VRRig> OnPlayerSerialize;

        public static void Postfix(VRRig __instance, InputStruct data) =>
            OnPlayerSerialize?.Invoke(__instance);
    }
}
