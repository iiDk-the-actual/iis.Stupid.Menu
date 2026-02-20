/*
 * ii's Stupid Menu  Patches/Safety/AntiCheatPatches.cs
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
using UnityEngine;

namespace iiMenu.Patches.Safety
{
    public class AntiCheatPatches
    {
        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.SendReport))]
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

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.CloseInvalidRoom))]
        public class NoCloseInvalidRoom
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.CheckReports))]
        public class NoCheckReports
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.DispatchReport))]
        public class NoDispatchReport
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.GetRPCCallTracker))]
        internal class NoGetRPCCallTracker
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.LogErrorCount))]
        public class NoLogErrorCount
        {
            private static bool Prefix(string logString, string stackTrace, LogType type) =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.QuitDelay), MethodType.Enumerator)]
        public class NoQuitDelay
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaGameManager), nameof(GorillaGameManager.ForceStopGame_DisconnectAndDestroy))]
        public class NoQuitOnBan
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.ShouldDisconnectFromRoom))]
        public class NoShouldDisconnectFromRoom                                                                         
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), nameof(GorillaNetworkPublicTestsJoin.GracePeriod))]
        public class GracePeriodPatch1
        {
            private static bool Prefix() =>
                false;
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), nameof(GorillaNetworkPublicTestJoin2.GracePeriod))]
        public class GracePeriodPatch2
        {
            private static bool Prefix() =>
                false;
        }
    }
}
