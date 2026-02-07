/*
 * ii's Stupid Menu  Patches/Menu/TimerPatch.cs
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
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaTagger), nameof(GorillaTagger.LateUpdate))]
    public class TimerPatch
    {
        public static bool enabled;
        private static float oldDeltaTime;

        public static void Prefix(GorillaTagger __instance)
        {
            if (enabled)
            {
                oldDeltaTime = Time.fixedDeltaTime;
                __instance._framerateUpdated = true;
                Time.fixedDeltaTime = 1 / UnityEngine.XR.XRDevice.refreshRate;
            }
        }

        public static void Postfix(GorillaTagger __instance)
        {
            if (enabled)
                Time.fixedDeltaTime = oldDeltaTime;
        }
    }
}