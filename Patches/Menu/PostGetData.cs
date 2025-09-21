/*
 * ii's Stupid Menu  Patches/Menu/PostGetData.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(BundleManager), "CheckIfBundlesOwned")]
    public class PostGetData
    {
        public static bool CosmeticsInitialized;
        private static void Postfix()
        {
            CosmeticsInitialized = true;
            CosmeticsOwned = CosmeticsController.instance.concatStringCosmeticsAllowed;
        }
    }
}
