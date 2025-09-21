using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaGameModes.GameMode), "ReportTag")]
    public class TagPatch
    {
        public static List<NetPlayer> taggedPlayers = new List<NetPlayer>();

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

                    string killsounds = PluginInfo.ResourceURL;
                    switch (tagCount)
                    {
                        case 1:
                            if (iiMenu.Menu.Main.InfectedList().Count <= 1)
                                iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/firstblood.wav", "firstblood.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            
                            break;
                        case 2:
                            iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/doublekill.wav", "doublekill.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 3:
                            iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/triplekill.wav", "triplekill.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 4:
                            iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/killingspree.wav", "killingspree.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 5:
                            iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/wickedsick.wav", "wickedsick.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 6:
                            iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/monsterkill.wav", "monsterkill.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 7:
                            iiMenu.Menu.Main.Play2DAudio(iiMenu.Menu.Main.LoadSoundFromURL($"{killsounds}/rampage.wav", "rampage.wav"), iiMenu.Menu.Main.buttonClickVolume / 10f);
                            break;
                    }
                }
            }
        }
    }
}
