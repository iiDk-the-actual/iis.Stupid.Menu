/*
 * ii's Stupid Menu  Patches/Menu/FailurePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes.Menu;
using iiMenu.Managers;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaComputer), "GeneralFailureMessage")]
    public class FailurePatch
    {
        public static void Prefix(string failMessage)
        {
            if (ServerData.ServerDataEnabled && failMessage.Contains("YOUR ACCOUNT"))
                CoroutineManager.instance.StartCoroutine(ServerData.ReportFailureMessage(failMessage));
        }
    }
}
