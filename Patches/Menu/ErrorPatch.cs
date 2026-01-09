/*
 * ii's Stupid Menu  Patches/Menu/ErrorPatch.cs
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
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using PlayFab;
using System;
using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaComputer), "OnErrorShared")]
    public class ErrorPatch
    {
        public static bool enabled;
        public static bool Prefix(PlayFabError error)
        {
            if (!enabled)
                return true;

            if (!error.ErrorMessage.Contains("is currently banned"))
                return true;

            using Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator();
            if (!enumerator.MoveNext())
                return false;

            KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;

            bool isIp = error.ErrorMessage.Contains("IP");
            bool isIndefinite = keyValuePair.Value[0] == "Indefinite";

            DateTime banEnd = DateTime.Parse(keyValuePair.Value[0]);
            TimeSpan remaining = banEnd - DateTime.UtcNow;

            string banMessage = @$"Your account {PlayFabAuthenticator.instance.GetPlayFabPlayerId()} has been banned.
Ban Reason: {keyValuePair.Key}
Time Left: {(isIndefinite ? "Indefinite" : FormatTimeLeft(remaining))}
Unban Date: {(isIndefinite ? "Never" : banEnd.ToString("MMMM dd, yyyy h:mm tt"))}";

            GorillaComputer.instance.GeneralFailureMessage(banMessage);
            return false;
        }

        private static string FormatTimeLeft(TimeSpan time)
        {
            if (time <= TimeSpan.Zero)
                return "Expired";

            var parts = new List<string>();

            int months = time.Days / 30;
            int weeks = (time.Days % 30) / 7;
            int days = (time.Days % 30) % 7;

            if (months > 0) parts.Add($"{months} months");
            if (weeks > 0) parts.Add($"{weeks} weeks");
            if (days > 0) parts.Add($"{days} days");
            if (time.Hours > 0) parts.Add($"{time.Hours} hours");
            if (time.Minutes > 0) parts.Add($"{time.Minutes} minutes");
            if (time.Seconds > 0) parts.Add($"{time.Seconds} seconds");

            return string.Join(" ", parts);
        }
    }
}
