/*
 * ii's Stupid Menu  Patches/Safety/PlayFabTelemetryPatches.cs
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
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using static iiMenu.Utilities.RandomUtilities;
using Random = UnityEngine.Random;

namespace iiMenu.Patches.Safety
{
    public class PlayFabTelemetryPatches
    {
        [HarmonyPatch(typeof(PlayFabDeviceUtil), nameof(PlayFabDeviceUtil.SendDeviceInfoToPlayFab))]
        public class PlayfabUtil01
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabClientInstanceAPI), nameof(PlayFabClientInstanceAPI.ReportDeviceInfo))]
        public class PlayfabUtil02
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabClientAPI), nameof(PlayFabClientAPI.ReportDeviceInfo))]
        public class PlayfabUtil03
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabDeviceUtil), nameof(PlayFabDeviceUtil.GetAdvertIdFromUnity))]
        public class PlayfabUtil04
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabClientAPI), nameof(PlayFabClientAPI.AttributeInstall))]
        public class PlayfabUtil05
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabHttp), nameof(PlayFabHttp.InitializeScreenTimeTracker))]
        public class PlayfabUtil06
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabClientAPI), nameof(PlayFabClientAPI.UpdateUserTitleDisplayName))] // Credits to Shiny for letting me use this
        public class DisplayNamePatch
        {
            public static void Prefix(ref UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null) =>
                request.DisplayName = RandomString(Random.Range(3, 12)); // Min and max is 3 and 12, do not modify this
        }
    }
}
