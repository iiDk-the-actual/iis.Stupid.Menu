/*
 * ii's Stupid Menu  Patches/Menu/BanPatches.cs
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

using GorillaNetworking;
using HarmonyLib;
using iiMenu.Managers;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    public class BanPatches
    {
        public static bool enabled;

        [HarmonyPatch(typeof(GorillaServer), nameof(GorillaServer.CheckForBadName))]
        public class AutoBanPlayfabFunction
        {
            public static bool Prefix(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
            {
                if (enabled)
                {
                    successCallback?.Invoke(new ExecuteFunctionResult { FunctionResult = new PlayFab.Json.JsonObject { { "result", 0 } } });
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GorillaComputer), nameof(GorillaComputer.CheckAutoBanListForName))]
        public class CheckAutoBanListForName
        {
            public static bool Prefix(string nameToCheck, ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }

            public static bool CheckBanList(string nameToCheck)
            {
                nameToCheck = nameToCheck.ToLower();
                nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));

                foreach (string twoWeekNames in GorillaComputer.instance.anywhereTwoWeek)
                {
                    if (nameToCheck.IndexOf(twoWeekNames) >= 0)
                        return false;
                }

                foreach (string oneWeekNames in GorillaComputer.instance.anywhereOneWeek)
                {
                    if (nameToCheck.IndexOf(oneWeekNames) >= 0 && !nameToCheck.Contains("fagol"))
                        return false;
                }

                string[] exactNames = GorillaComputer.instance.exactOneWeek;
                for (int i = 0; i < exactNames.Length; i++)
                {
                    if (exactNames[i] == nameToCheck)
                        return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayFabUnityHttp), nameof(PlayFabUnityHttp.MakeApiCall))]
        public class AntiBanCrash1
        {
            public static bool enabled;

            private static bool Prefix(object reqContainerObj)
            {
                if (!enabled || reqContainerObj == null)
                    return true;

                CallRequestContainer callRequestContainer = (CallRequestContainer)reqContainerObj;
                
                if (callRequestContainer.ErrorCallback != null)
                {
                    Action<PlayFabError> errorCallback = callRequestContainer.ErrorCallback;
                    void overrideError(PlayFabError error)
                    {
                        if (error.ErrorMessage.ToLower().Contains("ban") || error.ErrorMessage.ToLower().Contains("banned") || error.ErrorMessage.ToLower().Contains("suspended") || error.ErrorMessage.ToLower().Contains("suspension"))
                        {
                            if (error.ErrorMessage.ToLower().Contains("this ip has been banned"))
                                NotificationManager.SendNotification("<color=grey>[</color><color=red>ANTI-BAN</color><color=grey>]</color> Your IP address is currently banned.");
                            else
                                NotificationManager.SendNotification("<color=grey>[</color><color=red>ANTI-BAN</color><color=grey>]</color> Your account is currently banned.");
                            PlayFabError fakeError = new PlayFabError
                            {
                                Error = PlayFabErrorCode.UnknownError,
                                ErrorMessage = "An unknown error occurred.", // thinking of setting it to the original. I think the Error is the one that the game uses to crash you. Not the ErrorMessage. idk - kingofnetflix
                                ErrorDetails = new Dictionary<string, List<string>>()
                            };
                            errorCallback?.Invoke(fakeError);
                            return;
                        }
                        errorCallback?.Invoke(error);
                    }

                    callRequestContainer.ErrorCallback = overrideError;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayFabWebRequest), nameof(PlayFabWebRequest.MakeApiCall))]
        public class AntiBanCrash2
        {
            private static bool Prefix(object reqContainerObj)
            {
                if (!AntiBanCrash1.enabled || reqContainerObj == null)
                    return true;

                CallRequestContainer callRequestContainer = (CallRequestContainer)reqContainerObj;

                if (callRequestContainer.ErrorCallback != null)
                {
                    Action<PlayFabError> errorCallback = callRequestContainer.ErrorCallback;
                    void overrideError(PlayFabError error)
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
                    }

                    callRequestContainer.ErrorCallback = overrideError;
                }

                return true;
            }
        }
    }
}
