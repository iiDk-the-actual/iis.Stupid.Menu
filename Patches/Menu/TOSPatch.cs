/*
 * ii's Stupid Menu  Patches/Menu/TOSPatch.cs
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
﻿using System.Threading.Tasks;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(LegalAgreements), "Update")]
    public class TOSPatch
    {
        public static bool enabled;
        private static bool Prefix(LegalAgreements __instance)
        {
            if (enabled)
            {
                ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                __instance.scrollSpeed = 10f;
                __instance._maxScrollSpeed = 10f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ModIOTermsOfUse_v1), "PostUpdate")]
    public class TOSPatch2
    {
        private static bool Prefix(ModIOTermsOfUse_v1 __instance)
        {
            if (TOSPatch.enabled)
            {
                __instance.TurnPage(999);
                ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                __instance.holdTime = 0.1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AgeSlider), "PostUpdate")]
    public class TOSPatch3
    {
        private static bool Prefix(AgeSlider __instance)
        {
            if (TOSPatch.enabled)
            {
                __instance._currentAge = 21;
                __instance.holdTime = 0.1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PrivateUIRoom), "StartOverlay")]
    public class TOSPatch4
    {
        private static bool Prefix() =>
            !TOSPatch.enabled;
    }

    [HarmonyPatch(typeof(KIDManager), "UseKID")]
    public class TOSPatch5
    {
        private static bool Prefix(ref Task<bool> __result)
        {
            if (!TOSPatch.enabled)
                return true;

            __result = Task.FromResult(false);
            return false;
        }
    }
}
