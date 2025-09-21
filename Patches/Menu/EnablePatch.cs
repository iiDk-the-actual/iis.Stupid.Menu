/*
 * ii's Stupid Menu  Patches/Menu/EnablePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using iiMenu.Mods;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GrowingSnowballThrowable), "OnEnable")]
    public class EnablePatch
    {
        public static bool enabled;

        public static void Postfix(GrowingSnowballThrowable __instance)
        {
            if (enabled)
                __instance.IncreaseSize(Overpowered.snowballScale);
        }
    }
}
