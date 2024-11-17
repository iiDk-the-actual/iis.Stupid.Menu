using ExitGames.Client.Photon;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Notifications;
using OVR;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Valve.VR;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    public class Sound
    {
        public static bool LoopAudio = false;
        public static int BindMode = 0;
        public static void LoadSoundboard()
        {
            buttonsType = 18;
            pageNumber = 0;
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            if (!Directory.Exists("iisStupidMenu/Sounds"))
            {
                Directory.CreateDirectory("iisStupidMenu/Sounds");
            }
            List<string> enabledSounds = new List<string> { };
            foreach (ButtonInfo binfo in Buttons.buttons[18])
            {
                if (binfo.enabled)
                {
                    enabledSounds.Add(binfo.overlapText);
                }
            }
            List<ButtonInfo> soundbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Soundboard", method = () => Settings.EnableFun(), isTogglable = false, toolTip = "Returns you back to the fun mods." } };
            int index = 0;
            string[] files = Directory.GetFiles("iisStupidMenu/Sounds");
            foreach (string file in files)
            {
                index++;
                string FileName = file.Replace("\\", "/").Substring(21);
                if (BindMode > 0)
                {
                    string soundName = RemoveFileExtension(FileName).Replace("_", " ");
                    bool enabled = enabledSounds.Contains(soundName);
                    soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardSound" + index.ToString(), overlapText = soundName, method = () => PrepareBindAudio(file.Substring(14)), disableMethod = () => FixMicrophone(), enabled = enabled, toolTip = "Plays \"" + RemoveFileExtension(FileName).Replace("_", " ") + "\" through your microphone." });
                    
                } else
                {
                    if (LoopAudio)
                    {
                        string soundName = RemoveFileExtension(FileName).Replace("_", " ");
                        bool enabled = enabledSounds.Contains(soundName);
                        soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardSound" + index.ToString(), overlapText = soundName, enableMethod = () => PlayAudio(file.Substring(14)), disableMethod = () => FixMicrophone(), enabled = enabled, toolTip = "Plays \"" + RemoveFileExtension(FileName).Replace("_", " ") + "\" through your microphone." });
                    }
                    else
                    {
                        soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardSound" + index.ToString(), overlapText = RemoveFileExtension(FileName).Replace("_", " "), method = () => PlayAudio(file.Substring(14)), isTogglable = false, toolTip = "Plays \"" + RemoveFileExtension(FileName).Replace("_", " ") + "\" through your microphone." });
                    }
                }
            }
            soundbuttons.Add(new ButtonInfo { buttonText = "Stop All Sounds", method = () => FixMicrophone(), isTogglable = false, toolTip = "Stops all currently playing sounds." });
            soundbuttons.Add(new ButtonInfo { buttonText = "Open Sound Folder", method = () => OpenSoundFolder(), isTogglable = false, toolTip = "Opens a folder containing all of your sounds." });
            soundbuttons.Add(new ButtonInfo { buttonText = "Reload Sounds", method = () => LoadSoundboard(), isTogglable = false, toolTip = "Reloads all of your sounds." });
            soundbuttons.Add(new ButtonInfo { buttonText = "Get More Sounds", method = () => LoadSoundLibrary(), isTogglable = false, toolTip = "Opens a public audio library, where you can download your own sounds." });
            Buttons.buttons[18] = soundbuttons.ToArray();
        }

        public static void LoadSoundLibrary()
        {
            buttonsType = 26;
            pageNumber = 0;
            string library = GetHttp("https://github.com/iiDk-the-actual/ModInfo/raw/main/SoundLibrary.txt");
            string[] audios = AlphabetizeNoSkip(library.Split("\n"));
            List<ButtonInfo> soundbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Sound Library", method = () => LoadSoundboard(), isTogglable = false, toolTip = "Returns you back to the soundboard." } };
            int index = 0;
            foreach (string audio in audios)
            {
                if (audio.Length > 2)
                {
                    index++;
                    string[] Data = audio.Split(";");
                    soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardDownload" + index.ToString(), overlapText = Data[0], method = () => DownloadSound(Data[0], Data[1]), isTogglable = false, toolTip = "Downloads " + Data[0] + " to your sound library." });
                }
            }
            Buttons.buttons[26] = soundbuttons.ToArray();
        }

        public static void DownloadSound(string name, string url)
        {
            string filename = "Sounds/" + name + "." + GetFileExtension(url);
            if (File.Exists("iisStupidMenu/"+filename))
            {
                File.Delete("iisStupidMenu/" + filename);
            }
            if (audioFilePool.ContainsKey(name))
            {
                audioFilePool.Remove(name);
            }
            AudioClip soundDownloaded = LoadSoundFromURL(url, filename);
            if (soundDownloaded.length < 20f)
            {
                Play2DAudio(soundDownloaded, 1f);
            }
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to the soundboard.");
        }

        public static bool AudioIsPlaying = false;
        public static float RecoverTime = -1f;
        public static void PlayAudio(string file)
        {
            AudioClip sound = LoadSoundFromFile(file);
            GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
            GorillaTagger.Instance.myRecorder.AudioClip = sound;
            GorillaTagger.Instance.myRecorder.RestartRecording(true);
            GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
            if (!LoopAudio)
            {
                AudioIsPlaying = true;
                RecoverTime = Time.time + sound.length;
            }
        }

        public static void FixMicrophone()
        {
            GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
            GorillaTagger.Instance.myRecorder.AudioClip = null;
            GorillaTagger.Instance.myRecorder.RestartRecording(true);
            GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
            AudioIsPlaying = false;
            RecoverTime = -1f;
        }

        private static bool lastBindPressed = false;
        public static void PrepareBindAudio(string file)
        {
            bool[] bindings = new bool[]
            {
                rightPrimary,
                rightSecondary,
                leftPrimary,
                leftSecondary,
                leftGrab,
                rightGrab,
                leftTrigger > 0.5f,
                rightTrigger > 0.5f,
                false,
                false
            };
            bool bindPressed = bindings[BindMode - 1];
            if ((BindMode - 1) == 8) // If I don't do this it errors for some reason
                bindPressed = SteamVR_Actions.gorillaTag_LeftJoystickClick.state;
            if ((BindMode - 1) == 9)
                bindPressed = SteamVR_Actions.gorillaTag_RightJoystickClick.state;
            if (bindPressed && !lastBindPressed)
            {
                if (GorillaTagger.Instance.myRecorder.SourceType == Recorder.InputSourceType.AudioClip)
                {
                    FixMicrophone();
                }
                else
                {
                    PlayAudio(file);
                }
            }
            lastBindPressed = bindPressed;
        }

        public static void OpenSoundFolder()
        {
            string filePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, "iisStupidMenu/Sounds");
            filePath = filePath.Split("BepInEx\\")[0] + "iisStupidMenu/Sounds";
            Process.Start(filePath);
        }

        public static void EnableLoopSounds()
        {
            LoopAudio = true;
        }

        public static void DisableLoopSounds()
        {
            LoopAudio = false;
        }

        public static void SoundBindings()
        {
            string[] names = new string[]
            {
                "None",
                "A",
                "B",
                "X",
                "Y",
                "Left Grip",
                "Right Grip",
                "Left Trigger",
                "Right Trigger",
                "Left Joystick",
                "Right Joystick"
            };
            BindMode++;
            if (BindMode > names.Length - 1)
            {
                BindMode = 0;
            }
            GetIndex("sbds").overlapText = "Sound Bindings <color=grey>[</color><color=green>" + names[BindMode] + "</color><color=grey>]</color>";
        }

        public static void BetaPlayTag(int id, float volume)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                if (Time.time > soundDebounce)
                {
                    object[] soundSendData = new object[2];
                    soundSendData[0] = id;
                    soundSendData[1] = volume;

                    object[] sendEventData = new object[3];
                    sendEventData[0] = PhotonNetwork.ServerTimestamp;
                    sendEventData[1] = (byte)3;
                    sendEventData[2] = soundSendData;
                    try
                    {
                        PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
                    }
                    catch { }
                    RPCProtection();

                    soundDebounce = Time.time + 0.2f;
                }
            }
        }

        public static void RandomSoundSpam()
        {
            if (rightGrab)
            {
                int soundId = UnityEngine.Random.Range(0, 259);
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[] {
                        soundId,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundId, false, 999999f);
                }
            }
        }

        public static void BassSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        68,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, false, 999999f);
                }
            }
        }

        public static void MetalSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        18,
                        false,
                        999999f
                    });
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(18, false, 999999f);
                }
            }
        }

        public static void WolfSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        195,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(195, false, 999999f);
                }
            }
        }

        public static void CatSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        236,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(236, false, 999999f);
                }
            }
        }

        public static void TurkeySoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        83,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(83, false, 999999f);
                }
            }
        }

        public static void FrogSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        91,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(91, false, 999999f);
                }
            }
        }

        public static void BeeSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        191,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(191, false, 999999f);
                }
            }
        }

        public static void EarrapeSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        215,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(215, false, 999999f);
                }
            }
        }

        public static void DingSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        244,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(244, false, 999999f);
                }
            }
        }

        public static void CrystalSoundSpam()
        {
            if (rightGrab)
            {
                int[] sounds = new int[]
                {
                    UnityEngine.Random.Range(40,54),
                    UnityEngine.Random.Range(214,221)
                };
                int soundId = sounds[UnityEngine.Random.Range(0, 1)];
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        soundId,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundId, false, 999999f);
                }
                RPCProtection();
            }
        }

        public static void BigCrystalSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        213,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(213, false, 999999f);
                }
            }
        }

        public static void PanSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        248,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(248, false, 999999f);
                }
            }
        }

        public static void AK47SoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        203,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(203, false, 999999f);
                }
            }
        }

        public static void SqueakSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        75 + (Time.frameCount % 2),
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(75 + (Time.frameCount % 2), false, 999999f);
                }
            }
        }

        public static void SirenSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        48 + ((Time.frameCount / 15) % 2) * 2,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48 + ((Time.frameCount / 15) % 2) * 2, false, 999999f);
                }
            }
        }

        public static void DecreaseSoundID()
        {
            soundId = soundId - 1;
            if (soundId < 0)
            {
                soundId = 0;
            }
            GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId.ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseSoundID()
        {
            soundId++;
            GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId.ToString() + "</color><color=grey>]</color>";
        }

        public static void CustomSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        soundId,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundId, false, 999999f);
                }
            }
        }

        public static void CountSoundSpam()
        {
            if (rightGrab)
            {
                /*GorillaTagger.Instance.myVRRig.SendRPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    1,
                    999999f
                });
                RPCProtection();*/
                BetaPlayTag(1, 999999f);
            }
        }

        public static void BrawlCountSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(6, 999999f);
            }
        }

        public static void BrawlStartSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(7, 999999f);
            }
        }

        public static void TagSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(0, 999999f);
            }
        }

        public static void RoundEndSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(2, 999999f);
            }
        }

        public static void BonkSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(4, 999999f);
            }
        }
    }
}
