/*
 * ii's Stupid Menu  Patches/Menu/DreidelPatch.cs
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

using GorillaTag.Cosmetics;
using HarmonyLib;
using Photon.Pun;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(DreidelHoldable), "OnActivate")]
    public class DreidelPatch
    {
        public static bool enabled;
        public static double? time;

        public static void Prefix()
        {
            if (enabled)
            {
                time = PhotonNetwork.frametime;
                PhotonNetwork.frametime = double.MaxValue;
            }
        }

        public static void Postfix()
        {
            if (enabled)
                PhotonNetwork.frametime = time ?? PhotonNetwork.ServerTimestamp / 1000.0;
            
            time = null;
        }
    }
}
