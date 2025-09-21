/*
 * ii's Stupid Menu  Patches/Menu/OwnershipPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(RequestableOwnershipGuard), "OwnershipRequested")]
    public class OwnershipPatch
    {
        public static bool enabled;
        public static List<RequestableOwnershipGuard> blacklistedGuards = new List<RequestableOwnershipGuard>();

        public static bool Prefix(RequestableOwnershipGuard __instance, string nonce, PhotonMessageInfo info) =>
            !enabled || (__instance.photonView.IsMine && !blacklistedGuards.Contains(__instance));
    }
}
