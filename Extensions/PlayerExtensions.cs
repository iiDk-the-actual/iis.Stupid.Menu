﻿/*
 * ii's Stupid Menu  Extensions/PlayerExtensions.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

using ExitGames.Client.Photon;
using iiMenu.Managers;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace iiMenu.Extensions
{
    public static class PlayerExtensions
    {
        #region NetPlayer
        public static Player GetPlayer(this NetPlayer self) =>
            RigManager.NetPlayerToPlayer(self);

        public static VRRig VRRig(this NetPlayer self) =>
            RigManager.GetVRRigFromPlayer(self);

        public static bool InRoom(this NetPlayer self) =>
            NetworkSystem.Instance.AllNetPlayers.Contains(self);

        public static Hashtable GetCustomProperties(this NetPlayer self) =>
            self.GetPlayer().CustomProperties;
        #endregion

        #region Player
        public static VRRig VRRig(this Player self) =>
            RigManager.GetVRRigFromPlayer(self);

        public static bool InRoom(this Player self) =>
            PhotonNetwork.PlayerList.Contains(self);

        #endregion
    }
}
