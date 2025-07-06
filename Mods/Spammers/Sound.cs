using ExitGames.Client.Photon;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    public class Sound
    {
        public static bool LoopAudio = false;
        public static int BindMode = 0;
        public static string Subdirectory = "";
        public static void LoadSoundboard()
        {
            currentCategoryName = "Soundboard";

            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory);
            
            List<string> enabledSounds = new List<string> { };
            foreach (ButtonInfo binfo in Buttons.buttons[18])
            {
                if (binfo.enabled)
                    enabledSounds.Add(binfo.overlapText);
            }
            List<ButtonInfo> soundbuttons = new List<ButtonInfo> { };
            if (Subdirectory != "")
                soundbuttons.Add(new ButtonInfo { buttonText = "Exit Parent Directory", overlapText = "Exit " + Subdirectory.Split("/")[^1], method = () => ExitParentDirectory(), isTogglable = false, toolTip = "Returns you back to the last folder." });

            soundbuttons.Add(new ButtonInfo { buttonText = "Exit Soundboard", method = () => currentCategoryName = "Fun Mods", isTogglable = false, toolTip = "Returns you back to the fun mods." });
            int index = 0;

            string[] folders = Directory.GetDirectories($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory);
            foreach (string folder in folders)
            {
                index++;
                int substringLength = ($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory + "/").Length;
                string FolderName = folder.Replace("\\", "/")[substringLength..];
                soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardFolder" + index.ToString(), overlapText = "▶ " + FolderName, method = () => OpenFolder(folder[21..]), isTogglable = false, toolTip = "Opens the " + FolderName + " folder."});
            }

            index = 0;
            string[] files = Directory.GetFiles($"{PluginInfo.BaseDirectory}/Sounds" + Subdirectory);
            foreach (string file in files)
            {
                index++;
                string FileName = file.Replace("\\", "/")[(21 + Subdirectory.Length)..];
                if (BindMode > 0)
                {
                    string soundName = RemoveFileExtension(FileName).Replace("_", " ");
                    bool enabled = enabledSounds.Contains(soundName);
                    soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardSound" + index.ToString(), overlapText = soundName, method = () => PrepareBindAudio(file[14..]), disableMethod = () => FixMicrophone(), enabled = enabled, toolTip = "Plays \"" + RemoveFileExtension(FileName).Replace("_", " ") + "\" through your microphone." });
                    
                } else
                {
                    if (LoopAudio)
                    {
                        string soundName = RemoveFileExtension(FileName).Replace("_", " ");
                        bool enabled = enabledSounds.Contains(soundName);
                        soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardSound" + index.ToString(), overlapText = soundName, enableMethod = () => PlayAudio(file[14..]), disableMethod = () => FixMicrophone(), enabled = enabled, toolTip = "Plays \"" + RemoveFileExtension(FileName).Replace("_", " ") + "\" through your microphone." });
                    }
                    else
                    {
                        soundbuttons.Add(new ButtonInfo { buttonText = "SoundboardSound" + index.ToString(), overlapText = RemoveFileExtension(FileName).Replace("_", " "), method = () => PlayAudio(file[14..]), isTogglable = false, toolTip = "Plays \"" + RemoveFileExtension(FileName).Replace("_", " ") + "\" through your microphone." });
                    }
                }
            }
            soundbuttons.Add(new ButtonInfo { buttonText = "Stop All Sounds", method = () => FixMicrophone(), isTogglable = false, toolTip = "Stops all currently playing sounds." });
            soundbuttons.Add(new ButtonInfo { buttonText = "Open Sound Folder", method = () => OpenSoundFolder(), isTogglable = false, toolTip = "Opens a folder containing all of your sounds." });
            soundbuttons.Add(new ButtonInfo { buttonText = "Reload Sounds", method = () => LoadSoundboard(), isTogglable = false, toolTip = "Reloads all of your sounds." });
            soundbuttons.Add(new ButtonInfo { buttonText = "Get More Sounds", method = () => LoadSoundLibrary(), isTogglable = false, toolTip = "Opens a public audio library, where you can download your own sounds." });
            Buttons.buttons[18] = soundbuttons.ToArray();
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
            currentCategoryName = "Sound Library";

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
            if (name.Contains(".."))
                name = name.Replace("..", "");

            if (name.Contains(":"))
                return;

            string filename = $"Sounds{Subdirectory}/{name}.{GetFileExtension(url)}";
            if (File.Exists($"{PluginInfo.BaseDirectory}/{filename}"))
                File.Delete($"{PluginInfo.BaseDirectory}/{filename}");
            
            if (audioFilePool.ContainsKey(name))
                audioFilePool.Remove(name);
            
            AudioClip soundDownloaded = LoadSoundFromURL(url, filename);
            if (soundDownloaded.length < 20f)
                Play2DAudio(soundDownloaded, 1f);
            
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to the soundboard.");
        }

        public static bool AudioIsPlaying = false;
        public static float RecoverTime = -1f;
        public static void PlayAudio(string file)
        {
            if (PhotonNetwork.InRoom)
            {
                AudioClip sound = LoadSoundFromFile(file);
                GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
                GorillaTagger.Instance.myRecorder.AudioClip = sound;
                GorillaTagger.Instance.myRecorder.RestartRecording(true);
                GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
                if (!LoopAudio)
                {
                    AudioIsPlaying = true;
                    RecoverTime = Time.time + sound.length + 0.4f;
                }
            }
        }

        public static void FixMicrophone()
        {
            if (PhotonNetwork.InRoom)
            {
                GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
                GorillaTagger.Instance.myRecorder.AudioClip = null;
                GorillaTagger.Instance.myRecorder.RestartRecording(true);
                GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
            }
            
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
            string filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/Sounds");
            filePath = filePath.Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/Sounds";
            Process.Start(filePath);
        }

        public static void SoundBindings(bool positive = true)
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

            if (positive)
                BindMode++;
            else
                BindMode--;

            BindMode %= names.Length;
            if (BindMode < 0)
                BindMode = names.Length - 1;

            GetIndex("Sound Bindings").overlapText = "Sound Bindings <color=grey>[</color><color=green>" + names[BindMode] + "</color><color=grey>]</color>";
        }

        public static void BetaPlayTag(int id, float volume)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                if (Time.time > soundDebounce)
                {
                    object[] soundSendData = new object[2] { id, volume };
                    object[] sendEventData = new object[3] { PhotonNetwork.ServerTimestamp, (byte)3, soundSendData };

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

        private static float soundSpamDelay;
        public static void SoundSpam(int soundId)
        {
            if (rightGrab)
            {
                if (Time.time > soundSpamDelay)
                    soundSpamDelay = Time.time + 0.05f;
                else
                    return;

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
                    VRRig.LocalRig.PlayHandTapLocal(soundId, false, 999999f);
            }
        }

        public static void RandomSoundSpam() =>
            SoundSpam(Random.Range(0, GorillaLocomotion.GTPlayer.Instance.materialData.Count));

        public static void CrystalSoundSpam()
        {
            int[] sounds = new int[]
            {
                Random.Range(40,54),
                Random.Range(214,221)
            };
            SoundSpam(sounds[Random.Range(0, 1)]);
        }

        private static bool squeakToggle = false;
        public static void SqueakSoundSpam()
        {
            squeakToggle = !squeakToggle;
            if (Time.time > soundSpamDelay)
                SoundSpam(squeakToggle ? 75 : 76);
        }

        private static bool sirenToggle = false;
        public static void SirenSoundSpam()
        {
            sirenToggle = !sirenToggle;
            if (Time.time > soundSpamDelay)
                SoundSpam(sirenToggle ? 48 : 50);
        }


        public static void DecreaseSoundID()
        {
            soundId--;
            if (soundId < 0)
                soundId = GorillaLocomotion.GTPlayer.Instance.materialData.Count - 1;

            GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId.ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseSoundID()
        {
            soundId++;
            soundId %= GorillaLocomotion.GTPlayer.Instance.materialData.Count;

            GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId.ToString() + "</color><color=grey>]</color>";
        }

        public static void CustomSoundSpam() => SoundSpam(soundId);

        public static void BetaSoundSpam(int id)
        {
            if (rightGrab)
                BetaPlayTag(id, 999999f);
        }
    }
}
