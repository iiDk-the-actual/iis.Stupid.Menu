/*
 * ii's Stupid Menu  Patches/Menu/TagPatch.cs
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

using GorillaGameModes;
using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using static iiMenu.Utilities.AssetUtilities;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GameMode), "ReportTag")]
    public class TagPatch
    {
        public static readonly List<NetPlayer> taggedPlayers = new List<NetPlayer>();

        public static bool enabled;
        public static float tagDelay;
        public static int tagCount;

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

                    string killsounds = $"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/TagSounds";
                    switch (tagCount)
                    {
                        case 1:
                            if (Main.InfectedList().Count <= 1)
                                Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/firstblood.wav", "firstblood.wav"), Main.buttonClickVolume / 10f);
                            
                            break;
                        case 2:
                            Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/doublekill.wav", "doublekill.wav"), Main.buttonClickVolume / 10f);
                            break;
                        case 3:
                            Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/triplekill.wav", "triplekill.wav"), Main.buttonClickVolume / 10f);
                            break;
                        case 4:
                            Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/killingspree.wav", "killingspree.wav"), Main.buttonClickVolume / 10f);
                            break;
                        case 5:
                            Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/wickedsick.wav", "wickedsick.wav"), Main.buttonClickVolume / 10f);
                            break;
                        case 6:
                            Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/monsterkill.wav", "monsterkill.wav"), Main.buttonClickVolume / 10f);
                            break;
                        case 7:
                            Main.Play2DAudio(LoadSoundFromURL($"{killsounds}/rampage.wav", "rampage.wav"), Main.buttonClickVolume / 10f);
                            break;
                    }
                }
            }
        }
    }
}
