/*
 * ii's Stupid Menu  Patches/Safety/NoIncrementRPCCall.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using Photon.Pun;
using System;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCall", new Type[] { typeof(PhotonMessageInfo), typeof(string) })]
    public class NoIncrementRPCCall
    {
        private static bool Prefix(PhotonMessageInfo info, string callingMethod = "") =>
            false;
    }
}
