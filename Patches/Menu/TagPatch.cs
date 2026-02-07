/*
 * ii's Stupid Menu  Patches/Menu/TagPatch.cs
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

using GorillaGameModes;
using HarmonyLib;
using iiMenu.Extensions;
using iiMenu.Menu;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.GameModeUtilities;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GameMode), nameof(GameMode.ReportTag))]
    public class TagPatch
    {
        public static readonly List<NetPlayer> taggedPlayers = new List<NetPlayer>();

        public static bool enabled;
        public static float tagDelay;
        public static int tagCount;

        private static void PlaySound(string name) =>
            LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/TagSounds/{name}.wav", $"{name}.wav").Play(Main.buttonClickVolume / 10f);

        public static void Postfix(NetPlayer player)
        {
            if (enabled && PhotonNetwork.InRoom)
            {
                if (Time.time > tagDelay)
                {
                    taggedPlayers.Clear();
                    tagCount = 0;
                }

                if (!taggedPlayers.Contains(player))
                {
                    taggedPlayers.Add(player);
                    tagCount = Math.Min(tagCount + 1, 7);
                    tagDelay = Time.time + 10f;

                    switch (tagCount)
                    {
                        case 1:
                            if (InfectedList().Count <= 1)
                                PlaySound("firstblood");

                            break;
                        case 2:
                            PlaySound("doublekill");
                            break;
                        case 3:
                            PlaySound("triplekill");
                            break;
                        case 4:
                            PlaySound("killingspree");
                            break;
                        case 5:
                            PlaySound("wickedsick");
                            break;
                        case 6:
                            PlaySound("monsterkill");
                            break;
                        case 7:
                            PlaySound("rampage");
                            break;
                    }
                }
            }
        }
    }
}
