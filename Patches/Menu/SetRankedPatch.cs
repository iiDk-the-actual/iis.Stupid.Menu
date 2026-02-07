/*
 * ii's Stupid Menu  Patches/Menu/SetRankedPatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using HarmonyLib;
﻿using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(RankedProgressionManager), nameof(RankedProgressionManager.SetLocalProgressionData))]
    public class SetRankedPatch
    {
        public static bool enabled;

        public static bool Prefix(RankedProgressionManager __instance, GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData data)
        {
            if (enabled)
            {
                Dictionary<int, int[]> tierData = new Dictionary<int, int[]> 
                {
                    { 0, new[] { 0, 0 } },
                    { 1, new[] { 0, 1 } },

                    { 2, new[] { 1, 0 } },
                    { 3, new[] { 1, 1 } },
                    { 4, new[] { 1, 2 } },

                    { 5, new[] { 2, 0 } },
                    { 6, new[] { 2, 1 } },
                    { 7, new[] { 2, 2 } },
                };

                foreach (GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData platformData in data.platformData)
                {
                    platformData.elo = Mods.Safety.targetElo;
                    platformData.majorTier = tierData[Mods.Safety.targetBadge][0];
                    platformData.minorTier = tierData[Mods.Safety.targetBadge][1];
                }
                __instance.ProgressionData = data;
                return false;
            }

            return true;
        }
    }
}
