using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaTagger))]
    public class TagPatch
    {
        private static MethodBase TargetMethod() =>
            typeof(GorillaTagger).GetMethod("TryToTag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(VRRig), typeof(Vector3), typeof(bool), typeof(NetPlayer).MakeByRefType(), typeof(NetPlayer).MakeByRefType() }, null);

        public static List<VRRig> taggedRigs = new List<VRRig> { };

        public static bool enabled;
        public static float tagDelay;
        public static int tagCount;

        public static void Postfix(GorillaTagger __instance, VRRig rig, Vector3 hitObjectPos, bool isBodyTag, ref NetPlayer taggedPlayer, ref NetPlayer touchedPlayer)
        {
            if (enabled && PhotonNetwork.InRoom && __instance == GorillaTagger.Instance)
            {
                if (Time.time > tagDelay)
                {
                    taggedRigs.Clear();
                    tagCount = 0;
                }

                if (!taggedRigs.Contains(rig))
                {
                    taggedRigs.Add(rig);
                    tagCount = Math.Min(tagCount + 1, 7);
                    tagDelay = Time.time + 10f;

                    string killsounds = "https://github.com/iiDk-the-actual/ModInfo/raw/main/killsounds";
                    switch (tagCount)
                    {
                        case 1:
                            if (Menu.Main.InfectedList().Count <= 1)
                                Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/firstblood.wav", "firstblood.wav"), Menu.Main.buttonClickVolume / 10f);
                            
                            break;
                        case 2:
                            Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/doublekill.wav", "doublekill.wav"), Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 3:
                            Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/triplekill.wav", "triplekill.wav"), Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 4:
                            Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/killingspree.wav", "killingspree.wav"), Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 5:
                            Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/wickedsick.wav", "wickedsick.wav"), Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 6:
                            Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/monsterkill.wav", "monsterkill.wav"), Menu.Main.buttonClickVolume / 10f);
                            break;
                        case 7:
                            Menu.Main.Play2DAudio(Menu.Main.LoadSoundFromURL($"{killsounds}/rampage.wav", "rampage.wav"), Menu.Main.buttonClickVolume / 10f);
                            break;
                    }
                }
            }
        }
    }
}
