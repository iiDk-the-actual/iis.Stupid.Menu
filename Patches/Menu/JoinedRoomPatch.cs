/*
 * ii's Stupid Menu  Patches/Menu/JoinedRoomPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(PhotonNetworkController), "OnJoinedRoom")]
    public class JoinedRoomPatch
    {
        public static bool enabled;

        private static void Prefix()
        {
            if (enabled)
                PhotonNetworkController.Instance.currentJoinType = JoinType.FollowingParty;
        }
    }
}
