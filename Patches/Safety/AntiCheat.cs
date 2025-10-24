/*
 * ii's Stupid Menu  Patches/Safety/AntiCheat.cs
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

﻿using HarmonyLib;
using iiMenu.Managers;
using Photon.Pun;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    public class AntiCheat
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
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>] </color><color=white>You have been reported for " + (AntiCheatReasonHide ? "hidden reason" : susReason) + ".</color>");
                    susNick.Remove(PhotonNetwork.LocalPlayer.NickName.Length);
                    susId.Remove(PhotonNetwork.LocalPlayer.UserId.Length);
                    RPCProtection();
                }
                else
                {
                    if (AntiCheatAll)
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>] </color><color=white>" + susNick + " was reported for " + (AntiCheatReasonHide ? "hidden reason" : susReason) + ".</color>");
                }
            }

            if (AntiACReport)
            {
                Mods.Safety.AntiReportFRT(null, false);
                NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>The anti cheat attempted to report you, you have been disconnected.</color>");
            }

            return false;
        }
    }
}
