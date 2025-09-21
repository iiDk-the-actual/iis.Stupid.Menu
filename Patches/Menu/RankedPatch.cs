/*
 * ii's Stupid Menu  Patches/Menu/RankedPatch.cs
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
    [HarmonyPatch(typeof(PhotonNetworkController), "AttemptToJoinRankedPublicRoom")]
    public class RankedPatch
    {
        public static bool enabled;
        public static string targetPlatform;
        public static string targetTier;

        public static bool Prefix(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
        {
            if (enabled)
            {
                PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoomAsync(
                    triggeredTrigger,
                    targetTier ?? RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString(),
                    targetPlatform ?? "PC",
                    roomJoinType
                );

                return false;
            }
            return true;
        }
    }
}
