/*
 * ii's Stupid Menu  Patches/Menu/RPCFilter.cs
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
using iiMenu.Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(PhotonNetwork), nameof(PhotonNetwork.RPC), typeof(PhotonView), typeof(string), typeof(RpcTarget), typeof(Player), typeof(bool), typeof(object[]))]
    public class RPCFilter
    {
        /// <summary>
        /// Stores a mapping of RPC names to filter functions that determine whether each RPC should be sent.
        /// </summary>
        public static Dictionary<string, Func<bool>> FilteredRPCs = new Dictionary<string, Func<bool>>();

        public static bool Prefix(PhotonView view, string methodName, RpcTarget target, Player player, bool encrypt, params object[] parameters)
        {
            if (FilteredRPCs.Count <= 0)
                return true;

            try
            {
                if (FilteredRPCs.TryGetValue(methodName, out var function))
                    return function?.Invoke() ?? true;
            } catch (Exception e)
            {
                LogManager.LogError($"Error in RPCFilter.FilteredRPCs.{methodName}: {e}");
            }

            return true;
        }
    }
}
