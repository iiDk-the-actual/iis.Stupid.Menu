/*
 * ii's Stupid Menu  Patches/Menu/SubscriptionPatches.cs
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

using GorillaTagScripts;
using HarmonyLib;
using System;

namespace iiMenu.Patches.Menu
{
    public class SubscriptionPatches
    {
        public static bool enabled;

        [HarmonyPatch(typeof(SubscriptionManager), nameof(SubscriptionManager.IsLocalSubscribed))]
        public class IsLocalSubscribed
        {
            private static bool Prefix(ref bool __result)
            {
                if (!enabled)
                    return true;

                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(SubscriptionManager), nameof(SubscriptionManager.LocalSubscriptionStatus))]
        public class LocalSubscriptionStatus
        {
            private static bool Prefix(ref SubscriptionManager.SubscriptionStatus __result)
            {
                if (!enabled)
                    return true;

                __result = SubscriptionManager.SubscriptionStatus.Active;
                return false;
            }
        }

        [HarmonyPatch(typeof(SubscriptionManager), nameof(SubscriptionManager.LocalSubscriptionDetails))]
        public class LocalSubscriptionDetails
        {
            private static bool Prefix(ref SubscriptionManager.SubscriptionDetails __result)
            {
                if (!enabled)
                    return true;

                __result = new SubscriptionManager.SubscriptionDetails
                {
                    active = true,
                    daysAccrued = int.MaxValue,
                    subscriptionFeatureSettings = new[] {true, true},
                    tier = int.MaxValue,
                    subscriptionActiveUntilDate = DateTime.MaxValue,
                    autoRenew = true,
                    autoRenewMonths = int.MaxValue
                };
                return false;
            }
        }
    }
}
