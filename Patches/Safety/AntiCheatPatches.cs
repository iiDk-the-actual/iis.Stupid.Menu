/*
 * ii's Stupid Menu  Patches/Safety/AntiCheatPatches.cs
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

using GorillaNetworking;
using HarmonyLib;
using iiMenu.Managers;
using Photon.Pun;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace iiMenu.Patches.Safety
{
    public class AntiCheatPatches
    {
        [HarmonyPatch(typeof(GorillaNot), "SendReport")]
        public class SendReportPatch
        {
            public static bool AntiCheatSelf;
            public static bool AntiCheatAll;
            public static bool AntiCheatReasonHide;
            public static bool AntiACReport;

            private static bool Prefix(string susReason, string susId, string susNick)
            {
                if (susReason.ToLower() == "empty rig")
                    return false;

                if (AntiCheatSelf || AntiCheatAll)
                {
                    if (susId == PhotonNetwork.LocalPlayer.UserId)
                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>]</color> You have been reported for {(AntiCheatReasonHide ? "hidden reason" : susReason)}.");
                    else
                    {
                        if (AntiCheatAll)
                            NotificationManager.SendNotification($"<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>]</color> {susNick} was reported for {(AntiCheatReasonHide ? "hidden reason" : susReason)}.");
                    }
                }

                if (AntiACReport)
                {
                    Mods.Safety.AntiReportFRT(PhotonNetwork.LocalPlayer);
                    NotificationManager.ClearAllNotifications();
                    NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> The anti cheat attempted to report you, you have been disconnected.");
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]
        public class NoCloseInvalidRoom
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNot), "CheckReports")]
        public class NoCheckReports
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNot), "DispatchReport")]
        public class NoDispatchReport
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNot), "GetRPCCallTracker")]
        internal class NoGetRPCCallTracker
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
        public class NoLogErrorCount
        {
            private static bool Prefix(string logString, string stackTrace, LogType type) =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNot), "QuitDelay", MethodType.Enumerator)]
        public class NoQuitDelay
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
        public class NoQuitOnBan
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNot), "ShouldDisconnectFromRoom")]
        public class NoShouldDisconnectFromRoom                                                                         
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
        public class GracePeriodPatch1
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
        public class GracePeriodPatch2
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(PlayFabCloudScriptAPI), "ExecuteFunction")]
        public class NoBanCrash
        {
            private static bool Prefix(ExecuteFunctionRequest request, Action<ExecuteFunctionResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
            {
                Action<PlayFabError> overrideError = (error) =>
                {
                    if (error.ErrorMessage.Contains("ban") || error.ErrorMessage.Contains("banned") || error.ErrorMessage.Contains("suspended") || error.ErrorMessage.Contains("suspension"))
                    {
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ANTI-BAN</color><color=grey>]</color> Your account is currently banned.");
                        PlayFabError fakeError = new PlayFabError
                        {
                            Error = PlayFabErrorCode.UnknownError,
                            ErrorMessage = "An unknown error occurred.",
                            ErrorDetails = new Dictionary<string, List<string>>()
                        };
                        errorCallback?.Invoke(fakeError);
                        return;
                    }
                    errorCallback?.Invoke(error);
                };
                PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
                PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
                if (!playFabAuthenticationContext.IsEntityLoggedIn())
                    throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
                
                string localApiServer = PlayFabSettings.LocalApiServer;
                if (!string.IsNullOrEmpty(localApiServer))
                {
                    PlayFabHttp.MakeApiCallWithFullUri(new Uri(new Uri(localApiServer), "/CloudScript/ExecuteFunction".TrimStart('/')).AbsoluteUri, request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
                    return false;
                }
                PlayFabHttp.MakeApiCall("/CloudScript/ExecuteFunction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);

                return false;
            }
        }
    }
}
