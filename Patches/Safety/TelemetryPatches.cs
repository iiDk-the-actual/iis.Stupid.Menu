/*
 * ii's Stupid Menu  Patches/Safety/TelemetryPatches.cs
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
using JetBrains.Annotations;
using Liv.Lck.Telemetry;
using PlayFab.EventsModels;

namespace iiMenu.Patches.Safety
{
    // Gorilla Tag's one weakness -- tracking data to get players banned. This is how they did it over the years.
    public class TelemetryPatches
    {
        public static bool enabled = true;

        [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.EnqueueTelemetryEvent))]
        public class TelemetryPatch1
        {
            private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null) =>
                !enabled;
        }

        [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.EnqueueTelemetryEventPlayFab))]
        public class TelemetryPatch2
        {
            private static bool Prefix(EventContents eventContent) =>
                !enabled;
        }

        [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.FlushPlayFabTelemetry))]
        public class TelemetryPatch3
        {
            private static bool Prefix() =>
                !enabled;
        }

        [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.FlushMothershipTelemetry))]
        public class TelemetryPatch4
        {
            private static bool Prefix() =>
                !enabled;
        }

        [HarmonyPatch(typeof(LckTelemetryClient), nameof(LckTelemetryClient.SendTelemetry))]
        public class TelemetryPatch5
        {
            private static bool Prefix(LckTelemetryEvent lckTelemetryEvent) =>
                !enabled;
        }
    }
}
