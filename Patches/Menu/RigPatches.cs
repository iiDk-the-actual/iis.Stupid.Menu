/*
 * ii's Stupid Menu  Patches/Menu/RigPatches.cs
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

namespace iiMenu.Patches.Menu
{
    public class RigPatches
    {
        [HarmonyPatch(typeof(VRRig), "OnDisable")]
        public class RigPatch
        {
            public static bool Prefix(VRRig __instance) =>
                !__instance.isLocal;
        }

        // Because of ghost view
        [HarmonyPatch(typeof(VRRig), "Awake")]
        public class RigPatch2
        {
            public static bool Prefix(VRRig __instance) =>
                __instance.gameObject.name != "Local Gorilla Player(Clone)";
        }

        // Ugly code. Fuck optimization
        [HarmonyPatch(typeof(VRRig), "PostTick")]
        public class RigPatch3
        {
            public static bool Prefix(VRRig __instance) =>
                !__instance.isLocal || __instance.enabled;
        }
    }
}
