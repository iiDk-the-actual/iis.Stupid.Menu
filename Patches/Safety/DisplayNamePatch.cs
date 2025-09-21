/*
 * ii's Stupid Menu  Patches/Safety/DisplayNamePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabClientAPI), "UpdateUserTitleDisplayName")] // Credits to Shiny for letting me use this
    public class DisplayNamePatch
    {
        public static void Prefix(ref UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null) =>
            request.DisplayName = GenerateRandomString(UnityEngine.Random.Range(3, 12));
    }
}
