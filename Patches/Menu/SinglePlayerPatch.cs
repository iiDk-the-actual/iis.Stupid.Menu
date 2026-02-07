/*
 * ii's Stupid Menu  Patches/Menu/SinglePlayerPatch.cs
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
using Photon.Pun;
﻿using System.Threading.Tasks;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(NetworkSystemPUN), nameof(NetworkSystemPUN.InternalDisconnect))]
    public class SinglePlayerPatch
    {
        public static bool enabled;

        private static bool Prefix(NetworkSystemPUN __instance, ref Task __result)
        {
            if (!enabled)
                return true;

            __instance.internalState = NetworkSystemPUN.InternalState.Internal_Disconnecting;
            PhotonNetwork.Disconnect();

            Object.Destroy(__instance.VoiceNetworkObject);
            __instance.UpdatePlayers();
            __instance.SinglePlayerStarted();

            __result = InternalDisconnect(__instance);

            return false;
        }

        private static async Task InternalDisconnect(NetworkSystemPUN instance)
        {
            await instance.WaitForStateCheck(NetworkSystemPUN.InternalState.Internal_Disconnected);
            instance.internalState = NetworkSystemPUN.InternalState.Idle;
        }
    }
}
