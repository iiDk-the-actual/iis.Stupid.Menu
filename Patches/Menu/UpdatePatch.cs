/*
 * ii's Stupid Menu  Patches/Menu/UpdatePatch.cs
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
using iiMenu.Extensions;
using iiMenu.Menu;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), nameof(GorillaPlayerScoreboardLine.UpdatePlayerText))]
    public class UpdatePatch
    {
        public static bool enabled;

        private static int GetPing(VRRig rig)
        {
            int ping = rig.GetPing();
            return ping <= 150 ? 5 : ping <= 300 ? 4 : ping <= 450 ? 3 : ping <= 600 ? 2 : 1;
        }

        public static void Postfix(GorillaPlayerScoreboardLine __instance)
        {
            if (enabled)
            {
                string targetName = Main.CleanPlayerName(__instance.linePlayer.NickName) + " ERR";
                try
                {
                    VRRig rig = __instance.linePlayer.VRRig();
                    targetName = $"{Main.CleanPlayerName(__instance.linePlayer.NickName)}<size=50> <sprite name=\"{rig.GetPlatform()}\"> <sprite name=\"Ping{GetPing(rig)}\">{rig.fps}</size>";
                } catch { }
                __instance.playerNameVisible = targetName;
            }
        }
    }
}
