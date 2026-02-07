/*
 * ii's Stupid Menu  Patches/Menu/TOSPatches.cs
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
    public class TOSPatches
    {
        public static bool enabled;

        [HarmonyPatch(typeof(LegalAgreements), nameof(LegalAgreements.Update))]
        public class Update
        {
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

        [HarmonyPatch(typeof(ModIOTermsOfUse_v1), nameof(ModIOTermsOfUse_v1.PostUpdate))]
        public class PostUpdateModIO
        {
            private static bool Prefix(ModIOTermsOfUse_v1 __instance)
            {
                if (enabled)
                {
                    __instance.TurnPage(999);
                    ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                    __instance.holdTime = 0.1f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(AgeSlider), nameof(AgeSlider.PostUpdate))]
        public class PostUpdateAgeSlider
        {
            private static bool Prefix(AgeSlider __instance)
            {
                if (enabled)
                {
                    __instance._currentAge = 21;
                    __instance.holdTime = 0.1f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PrivateUIRoom), nameof(PrivateUIRoom.StartOverlay))]
        public class StartOverlay
        {
            private static bool Prefix() =>
                !enabled;
        }

        [HarmonyPatch(typeof(KIDManager), nameof(KIDManager.UseKID))]
        public class UseKID
        {
            private static bool Prefix(ref Task<bool> __result)
            {
                if (!enabled)
                    return true;

                __result = Task.FromResult(false);
                return false;
            }
        }
    }
}
