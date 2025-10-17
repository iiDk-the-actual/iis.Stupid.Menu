/*
 * ii's Stupid Menu  Patches/Safety/TelemetryPatches.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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
using PlayFab.EventsModels;

namespace iiMenu.Patches.Safety
{
    // Gorilla Tag's one weakness -- tracking data to get players banned. This is how they did it over the years.
    [HarmonyPatch(typeof(GorillaTelemetry), "EnqueueTelemetryEvent")]
    public class TelemetryPatch1
    {
        private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null) =>
            false;
    }

    [HarmonyPatch(typeof(GorillaTelemetry), "EnqueueTelemetryEventPlayFab")]
    public class TelemetryPatch2
    {
        private static bool Prefix(EventContents eventContent) =>
            false;
    }
}
