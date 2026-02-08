/*
 * ii's Stupid Menu  Mods/Sound.cs
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

using ExitGames.Client.Photon;
using GorillaLocomotion;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.FileUtilities;
using Random = UnityEngine.Random;

namespace iiMenu.Mods
{
    public static class Sound
    {
        public static bool LoopAudio = false;
        public static bool OverlapAudio = false;
        public static int BindMode;
        public static string Subdirectory = "";
        public static void LoadSoundboard(bool openCategory = true)
        {
            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory);
            
            List<string> enabledSounds = (from binfo in Buttons.buttons[Buttons.GetCategory("Soundboard")] where binfo.enabled select binfo.overlapText).ToList();
            List<ButtonInfo> soundButtons = new List<ButtonInfo>();
            if (Subdirectory != "")
                soundButtons.Add(new ButtonInfo { buttonText = "Exit Parent Directory", overlapText = "Exit " + Subdirectory.Split("/")[^1], method = ExitParentDirectory, isTogglable = false, toolTip = "Returns you back to the last folder." });

            soundButtons.Add(new ButtonInfo { buttonText = "Exit Soundboard", method = () => Buttons.CurrentCategoryName = "Sound Mods", isTogglable = false, toolTip = "Returns you back to the sound mods." });

            string[] folders = Directory.GetDirectories($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory);
            soundButtons.AddRange(from folder in folders
            let substringLength = ($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory + "/").Length
            let FolderName = folder.Replace("\\", "/")[substringLength..]
            select new ButtonInfo
            {
                buttonText = "SoundboardFolder" + FolderName.Hash(),
                overlapText = $"<sprite name=\"Folder\">  {FolderName}  ",
                method = () => OpenFolder(folder[21..]),
                isTogglable = false,
                toolTip = "Opens the " + FolderName + " folder."
            });

            string[] files = Directory.GetFiles($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory);
            if (!RecorderPatch.enabled || Buttons.GetIndex("Legacy Microphone").enabled)
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> You are using the legacy microphone system. Modern soundboard features will not be implemented.");
            foreach (string file in files)
            {
                string fileName = file.Replace("\\", "/")[(21 + Subdirectory.Length)..];
                string soundName = RemoveFileExtension(fileName).Replace("_", " ");

                if (RecorderPatch.enabled)
                {
                    var buttonInfo = new ButtonInfo
                    {
                        buttonText = "SoundboardSound" + soundName.Hash(),
                        overlapText = soundName,
                        toolTip = "Plays \"" + RemoveFileExtension(fileName).Replace("_", " ") + "\" through your microphone."
                    };
                    if (OverlapAudio)
                    {
                        buttonInfo.method = () => PlayAudio(file[14..]);
                        buttonInfo.isTogglable = false;
                    }
                    else
                    {
                        buttonInfo.method = () => PlaySoundboardSound(file[14..], buttonInfo, LoopAudio, BindMode > 0);
                        buttonInfo.disableMethod = () => StopSoundboardSound(buttonInfo);
                    }

                    soundButtons.Add(buttonInfo);
                } else
                {
                    if (BindMode > 0)
                    {
                        bool enabled = enabledSounds.Contains(soundName);
                        soundButtons.Add(new ButtonInfo { buttonText = "SoundboardSound" + soundName.Hash(), overlapText = soundName, method = () => PrepareBindAudio(file[14..]), disableMethod = StopAllSounds, enabled = enabled, toolTip = "Plays \"" + RemoveFileExtension(fileName).Replace("_", " ") + "\" through your microphone." });

                    }
                    else
                    {
                        if (LoopAudio)
                        {
                            bool enabled = enabledSounds.Contains(soundName);
                            soundButtons.Add(new ButtonInfo { buttonText = "SoundboardSound" + soundName.Hash(), overlapText = soundName, enableMethod = () => PlayAudio(file[14..]), disableMethod = StopAllSounds, enabled = enabled, toolTip = "Plays \"" + RemoveFileExtension(fileName).Replace("_", " ") + "\" through your microphone." });
                        }
                        else
                            soundButtons.Add(new ButtonInfo { buttonText = "SoundboardSound" + soundName.Hash(), overlapText = RemoveFileExtension(fileName).Replace("_", " "), method = () => PlayAudio(file[14..]), isTogglable = false, toolTip = "Plays \"" + RemoveFileExtension(fileName).Replace("_", " ") + "\" through your microphone." });
                    }
                }
                

                
            }
            soundButtons.Add(new ButtonInfo { buttonText = "Stop All Sounds", method = StopAllSounds, isTogglable = false, toolTip = "Stops all currently playing sounds." });
            soundButtons.Add(new ButtonInfo { buttonText = "Open Sound Folder", method = OpenSoundFolder, isTogglable = false, toolTip = "Opens a folder containing all of your sounds." });
            soundButtons.Add(new ButtonInfo { buttonText = "Reload Sounds", method = () => LoadSoundboard(), isTogglable = false, toolTip = "Reloads all of your sounds." });
            soundButtons.Add(new ButtonInfo { buttonText = "Get More Sounds", method = LoadSoundLibrary, isTogglable = false, toolTip = "Opens a public audio library, where you can download your own sounds." });
            Buttons.buttons[Buttons.GetCategory("Soundboard")] = soundButtons.ToArray();

            if (openCategory)
                Buttons.CurrentCategoryName = "Soundboard";
        }

        public static void ExitParentDirectory()
        {
            Subdirectory = RemoveLastDirectory(Subdirectory);
            LoadSoundboard();
        }

        public static void OpenFolder(string folder)
        {
            Subdirectory = "/" + folder;
            LoadSoundboard();
        }

        public static void LoadSoundLibrary()
        {
            string library = GetHttp($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/Soundboard/SoundLibrary.txt");
            string[] audios = AlphabetizeNoSkip(library.Split("\n"));
            List<ButtonInfo> soundbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Sound Library", method = () => LoadSoundboard(), isTogglable = false, toolTip = "Returns you back to the soundboard." } };
            int index = 0;
            foreach (string audio in audios)
            {
                if (audio.Length > 2)
                {
                    index++;
                    string[] Data = audio.Split(";");
                    soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardDownload" + index, overlapText = Data[0], method = () => DownloadSound(Data[0], $"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/Soundboard/Sounds/{Data[1]}"), isTogglable = false, toolTip = "Downloads " + Data[0] + " to your sound library." });
                }
            }
            Buttons.buttons[Buttons.GetCategory("Sound Library")] = soundbuttons.ToArray();
            Buttons.CurrentCategoryName = "Sound Library";
        }

        public static void DownloadSound(string name, string url)
        {
            if (name.Contains(".."))
                name = name.Replace("..", "");

            if (name.Contains(":"))
                return;

            string filename = $"Sounds{Subdirectory}/{name}.{GetFileExtension(url)}";
            if (File.Exists($"{PluginInfo.BaseDirectory}/{filename}"))
                File.Delete($"{PluginInfo.BaseDirectory}/{filename}");
            
            audioFilePool.Remove(name);
            
            AudioClip soundDownloaded = LoadSoundFromURL(url, filename);
            if (soundDownloaded.length < 20f)
                Play2DAudio(soundDownloaded);
            
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to the soundboard.");
        }

        public static bool AudioIsPlaying;
        public static float RecoverTime = -1f;

        private static GameObject soundboardAudioManager;

        public static void PlayAudio(AudioClip sound, bool disableMicrophone = false)
        {
            if (!PhotonNetwork.InRoom)
            {
                if (soundboardAudioManager == null)
                {
                    soundboardAudioManager = new GameObject("2DAudioMgr");
                    AudioSource temp = soundboardAudioManager.AddComponent<AudioSource>();
                    temp.spatialBlend = 0f;
                }

                AudioSource ausrc = soundboardAudioManager.GetComponent<AudioSource>();
                ausrc.volume = 1f;
                ausrc.clip = sound;
                ausrc.loop = false;
                ausrc.Play();

                AudioIsPlaying = true;
                RecoverTime = Time.time + sound.length;

                return;
            }


            if (RecorderPatch.enabled)
                VoiceManager.Get().AudioClip(sound, disableMicrophone);
            else
            {
                GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
                GorillaTagger.Instance.myRecorder.AudioClip = sound;
                GorillaTagger.Instance.myRecorder.RestartRecording(true);
            }

            GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
            if (!LoopAudio)
            {
                AudioIsPlaying = true;
                RecoverTime = Time.time + sound.length + 0.4f;
            }
        }

        private static readonly Dictionary<ButtonInfo, (Guid id, AudioClip clip)> activeSounds = new Dictionary<ButtonInfo, (Guid id, AudioClip clip)>();

        public static void PlaySoundboardSound(object file, ButtonInfo info, bool loopAudio, bool bind)
        {
            bool[] bindings = {
                rightPrimary,
                rightSecondary,
                leftPrimary,
                leftSecondary,
                leftGrab,
                rightGrab,
                leftTrigger > 0.5f,
                rightTrigger > 0.5f,
                leftJoystickClick,
                rightJoystickClick
            };

            AudioClip clip = null;
            if (file is string filePath)
                clip = LoadSoundFromFile(filePath);
            else if (file is AudioClip audioClip)
                clip = audioClip;

            if (clip == null)
                return;

            bool shouldPlay = true;
            if (bind && BindMode > 0)
            {
                bool bindPressed = bindings[BindMode - 1];
                shouldPlay = bindPressed && !lastBindPressed; 
                lastBindPressed = bindPressed;
            }

            // GorillaTagger.Instance.myRecorder.DebugEchoMode = true;

            if (shouldPlay && !activeSounds.ContainsKey(info))
            {
                if (RecorderPatch.enabled)
                {
                    Guid id = VoiceManager.Get().AudioClip(clip, false);
                    activeSounds[info] = (id, clip);
                }
            }

            var ids = VoiceManager.Get().AudioClips.Select(c => c.Id).ToHashSet();
            var finished = activeSounds.Where(kvp => !ids.Contains(kvp.Value.id)).ToList();

            foreach (var kvp in finished)
            {
                ButtonInfo finishedInfo = kvp.Key;
                AudioClip finishedClip = kvp.Value.clip;
                activeSounds.Remove(finishedInfo);

                if (loopAudio)
                {
                    Guid newId = VoiceManager.Get().AudioClip(finishedClip, false);
                    activeSounds[finishedInfo] = (newId, finishedClip);
                }
                else
                {
                    if (finishedInfo.enabled)
                        Toggle(finishedInfo);
                }
            }
        }

        public static void StopSoundboardSound(ButtonInfo info)
        {
            if (activeSounds != null)
            {
                if (activeSounds.ContainsKey(info))
                {
                    VoiceManager.Get().StopAudioClip(activeSounds[info].id);
                    activeSounds.Remove(info);
                }
            }
        }
        public static void PlayAudio(string file)
        {
            if (PhotonNetwork.InRoom)
            {
                AudioClip sound = LoadSoundFromFile(file);
                PlayAudio(sound);
            }
        }

        public static void StopAllSounds() // used to be FixMicrophone
        {
            if (soundboardAudioManager != null)
                soundboardAudioManager.GetComponent<AudioSource>().Stop();

            if (PhotonNetwork.InRoom)
            {
                if (RecorderPatch.enabled)
                {
                    if (activeSounds != null)
                        foreach (ButtonInfo info in activeSounds.Keys)
                            info.enabled = false;
                    activeSounds.Clear();
                    VoiceManager.Get().StopAudioClips();
                    GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
                }
                else
                {
                    GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
                    GorillaTagger.Instance.myRecorder.AudioClip = null;
                    GorillaTagger.Instance.myRecorder.RestartRecording(true);
                    GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
                }
            }

            AudioIsPlaying = false;
            RecoverTime = -1f;
        }

        public static void FixMicrophone()
        {
            GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
            GorillaTagger.Instance.myRecorder.AudioClip = null;
            GorillaTagger.Instance.myRecorder.RestartRecording(true);
            GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
        }

        private static bool lastBindPressed;
        public static void PrepareBindAudio(string file)
        {
            bool[] bindings = {
                rightPrimary,
                rightSecondary,
                leftPrimary,
                leftSecondary,
                leftGrab,
                rightGrab,
                leftTrigger > 0.5f,
                rightTrigger > 0.5f,
                leftJoystickClick,
                rightJoystickClick
            };

            bool bindPressed = bindings[BindMode - 1];
            if (bindPressed && !lastBindPressed)
            {
                if (GorillaTagger.Instance.myRecorder.SourceType == Recorder.InputSourceType.AudioClip)
                    FixMicrophone();
                else
                    PlayAudio(file);
            }
            lastBindPressed = bindPressed;
        }

        public static void OpenSoundFolder()
        {
            string filePath = GetGamePath() + $"/{PluginInfo.BaseDirectory}/Sounds";
            Process.Start(filePath);
        }

        public static void SoundBindings(bool positive = true)
        {
            string[] names = {
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

            if (positive)
                BindMode++;
            else
                BindMode--;

            BindMode %= names.Length;
            if (BindMode < 0)
                BindMode = names.Length - 1;

            Buttons.GetIndex("Sound Bindings").overlapText = "Sound Bindings <color=grey>[</color><color=green>" + names[BindMode] + "</color><color=grey>]</color>";
        }

        public static float sendEffectDelay;
        public static void BetaPlayTag(int id, float volume)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                if (Time.time > sendEffectDelay)
                {
                    object[] soundSendData = { id, volume, false };
                    object[] sendEventData = { PhotonNetwork.ServerTimestamp, (byte)3, soundSendData };

                    try
                    {
                        PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
                    }
                    catch { }
                    RPCProtection();

                    sendEffectDelay = Time.time + 0.2f;
                }
            }
        }

        private static float soundSpamDelay;
        public static void SoundSpam(int soundId, bool constant = false)
        {
            if (rightGrab || constant)
            {
                if (Time.time > soundSpamDelay)
                    soundSpamDelay = Time.time + 0.1f;
                else
                    return;

                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, soundId, false, 999999f);
                    RPCProtection();
                }
                else
                    VRRig.LocalRig.PlayHandTapLocal(soundId, false, 999999f);
            }
        }

        public static void JmancurlySoundSpam() =>
            SoundSpam(Random.Range(336, 338));

        public static void RandomSoundSpam() =>
            SoundSpam(Random.Range(0, GTPlayer.Instance.materialData.Count));

        public static void CrystalSoundSpam()
        {
            int[] sounds = {
                Random.Range(40,54),
                Random.Range(214,221)
            };
            SoundSpam(sounds[Random.Range(0, 1)]);
        }

        private static bool squeakToggle;
        public static void SqueakSoundSpam()
        {
            if (Time.time > soundSpamDelay)
                squeakToggle = !squeakToggle;
            
            SoundSpam(squeakToggle ? 75 : 76);
        }

        private static bool sirenToggle;
        public static void SirenSoundSpam()
        {
            if (Time.time > soundSpamDelay)
                sirenToggle = !sirenToggle;

            SoundSpam(sirenToggle ? 48 : 50);
        }

        public static int soundId;
        public static void DecreaseSoundID()
        {
            soundId--;
            if (soundId < 0)
                soundId = GTPlayer.Instance.materialData.Count - 1;

            Buttons.GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId + "</color><color=grey>]</color>";
        }

        public static void IncreaseSoundID()
        {
            soundId++;
            soundId %= GTPlayer.Instance.materialData.Count;

            Buttons.GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId + "</color><color=grey>]</color>";
        }

        public static void CustomSoundSpam() => SoundSpam(soundId);

        public static void BetaSoundSpam(int id)
        {
            if (rightGrab)
                BetaPlayTag(id, 999999f);
        }
    }
}
