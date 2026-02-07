/*
 * ii's Stupid Menu  Patches/Menu/SetColorPatch.cs
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
using iiMenu.Menu;
using iiMenu.Utilities;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.InitializeNoobMaterial))]
    public class InitializeNoobMaterial
    {
        public static bool Prefix(VRRig __instance, float red, float green, float blue, PhotonMessageInfoWrapped info)
        {
            NetPlayer player = RigUtilities.GetPlayerFromVRRig(__instance) ?? null;
            if (player != null && Main.ShouldBypassChecks(player))
            {
                if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(__instance.rigSerializer.gameObject))
                    __instance.InitializeNoobMaterialLocal(red, green, blue);

                return false;
            }

            return true;
        }
    }
}
