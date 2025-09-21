/*
 * ii's Stupid Menu  Patches/Menu/PhotonTimePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using Photon.Pun;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(PhotonNetwork), "get_ServerTimestamp")]
    public class PhotonTimePatch
    {
        public static bool enabled;
        public static int distTime = 0;

        public static void Postfix(ref int __result)
        {
            if (enabled)
                __result += distTime;
        }
    }
}
