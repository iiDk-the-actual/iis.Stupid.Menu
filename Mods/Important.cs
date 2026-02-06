/*
 * ii's Stupid Menu  Mods/Important.cs
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

using BepInEx;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Managers.DiscordRPC;
using iiMenu.Patches.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.TextCore;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.Newtonsoft.Json;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.RandomUtilities;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;

namespace iiMenu.Mods
{
    public static class Important
    {
        public static string oldId = "";
        public async static void CheckNewAcc()
        {
            await Task.Delay(10000);

            if (PhotonNetwork.LocalPlayer.UserId != oldId)
                playTime = 0f;
        }

        public static Coroutine queueCoroutine;
        public static int reconnectDelay = 1;

        public static IEnumerator QueueRoomCoroutine(string roomName)
        {
            NetworkSystemPUN instance = (NetworkSystemPUN)NetworkSystem.Instance;

            instance.ReturnToSinglePlayer();
            yield return new WaitUntil(() => instance.netState == NetSystemState.Idle);
            yield return new WaitForSeconds(0.5f);

            instance.netState = NetSystemState.Connecting;

            while (!instance.InRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomName, JoinType.Solo);
                yield return new WaitForSeconds(reconnectDelay);
            }
        }

        public static void QueueRoom(string roomName)
        {
            if (queueCoroutine != null)
                CoroutineManager.instance.StopCoroutine(queueCoroutine);

            queueCoroutine = CoroutineManager.instance.StartCoroutine(QueueRoomCoroutine(roomName));
        }

        public static void Reconnect()
        {
            string roomName = NetworkSystem.Instance.RoomName;

            NetworkSystem.Instance.ReturnToSinglePlayer();
            QueueRoom(roomName);
        }

        public static void CancelReconnect()
        {
            if (queueCoroutine != null)
                CoroutineManager.instance.StopCoroutine(queueCoroutine);

            NetworkSystem.Instance.netState = NetworkSystem.Instance.InRoom ? NetSystemState.InGame : NetSystemState.Idle;

            partyLastCode = null;
            partyKickReconnecting = false;
        }

        public static void JoinRandom()
        {
            if (PhotonNetwork.InRoom)
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
                CoroutineManager.instance.StartCoroutine(JoinRandomDelay());
                return;
            }

            GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest");
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger);
        }

        public static IEnumerator JoinRandomDelay()
        {
            yield return new WaitForSeconds(1.5f);
            JoinRandom();
        }

        public static async Task ForceCreateRoom(string name, RoomConfig options)
        {
            if (NetworkSystem.Instance.InRoom)
                await NetworkSystem.Instance.ReturnToSinglePlayer();

            await (NetworkSystem.Instance as NetworkSystemPUN).TryCreateRoom(name, options);
        }

        public static bool instantCreate;
        public static void CreateRoom(string roomName, bool isPublic, JoinType roomJoinType = JoinType.Solo)
        {
            RoomConfig roomConfig = new RoomConfig
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = isPublic,
                MaxPlayers = RoomSystem.GetRoomSizeForCreate((PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest")).zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), !isPublic, SubscriptionManager.IsLocalSubscribed()),
                CustomProps = new Hashtable
                {
                    { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.GetFullDesiredGameModeString() },
                    { "platform", PhotonNetworkController.Instance.platformTag },
                    { "queueName", GorillaComputer.instance.currentQueue }
                }
            };


            PhotonNetworkController.Instance.currentJoinType = roomJoinType;

            if (roomJoinType == JoinType.JoinWithParty || roomJoinType == JoinType.ForceJoinWithParty)
                Task.Run(PhotonNetworkController.Instance.SendPartyFollowCommands);

            switch (roomJoinType)
            {
                case JoinType.JoinWithNearby:
                case JoinType.JoinWithElevator:
                    roomConfig.SetFriendIDs(PhotonNetworkController.Instance.FriendIDList);
                    break;
                case JoinType.JoinWithParty:
                case JoinType.ForceJoinWithParty:
                    roomConfig.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList());
                    break;
            }

            if (instantCreate)
            {
                (NetworkSystem.Instance as NetworkSystemPUN).internalState = NetworkSystemPUN.InternalState.Searching_Creating;
                _ = ForceCreateRoom(roomName, roomConfig);
            }
            else
                NetworkSystem.Instance.ConnectToRoom(roomName, roomConfig);
        }

        public static void BroadcastRoom(string roomName, bool create, string key, string shuffler)
        {
            string text = NetworkSystem.ShuffleRoomName(roomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler[..2], true);

            BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest
            {
                KeyToFollow = key,
                RoomToJoin = text,
                Set = create
            };

            GorillaServer.Instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate {}, delegate { });
        }

        // The code below is fully safe. I know, it seems suspicious.
        public static void RestartGame()
        {
            string logoLines = PluginInfo.Logo.Split(@"
")
                .Aggregate("", (current, line) => current + (Environment.NewLine + "echo      " + line));

            string restartScript = @"@echo off
title ii's Stupid Menu
color 0E

cls
echo." + logoLines + @"
echo.

echo Your game is restarting, please wait...
echo.

:WAIT_LOOP
tasklist /FI ""IMAGENAME eq Gorilla Tag.exe"" | find /I ""Gorilla Tag.exe"" >nul
if %ERRORLEVEL%==0 (
    timeout /t 1 >nul
    goto WAIT_LOOP
)

start steam://run/1533390
exit";
            
            string fileName = $"{PluginInfo.BaseDirectory}/RestartScript.bat";

            File.WriteAllText(fileName, restartScript);

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
            Process.Start(filePath);
            Application.Quit();
        }

        public static void OpenGorillaTagFolder()
        {
            string filePath = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0];
            Process.Start(filePath);
        }

        private static DiscordRpcClient discord;
        private static DateTime? startTime;
        private static DateTime? endTime;
        private static float updateTime;
        public static void DiscordRPC()
        {
            if (discord == null)
            {
                discord = new DiscordRpcClient("1436519874368114850")
                {
                    Logger = new Managers.DiscordRPC.Logging.DiscordLogManager()
                };

                discord.Initialize();
            }

            if (NetworkSystem.Instance.InRoom)
            {
                endTime = null;

                if (startTime == null)
                    startTime = DateTime.UtcNow;
            }
            else
            {
                startTime = null;

                if (endTime == null)
                    endTime = DateTime.UtcNow;
            }

            if (Time.time > updateTime)
            {
                updateTime = Time.time + 1f;
                bool inRoom = NetworkSystem.Instance.InRoom;
                string roomName = inRoom ? NetworkSystem.Instance.RoomName : "-";

                discord.SetPresence(new RichPresence
                {
                    Details = inRoom ? $"Playing {GorillaGameManager.instance.GameType().ToString().ToLower()}" : "Playing alone",
                    State = inRoom ? $"Room: {roomName} ({PhotonNetwork.PlayerList.Length}/{PhotonNetwork.CurrentRoom.MaxPlayers})" : "Not in a room",
                    Assets = new Managers.DiscordRPC.Assets
                    {
                        LargeImageKey = "cone",
                        LargeImageText = "ii's Stupid Menu",
                        SmallImageKey = inRoom ? "online" : "offline",
                        SmallImageText = inRoom ? "Online" : "Offline"
                    },
                    Timestamps = inRoom ? new Timestamps
                    {
                        Start = startTime ?? endTime ?? DateTime.UtcNow
                    } : null,
                    Buttons = new[]
                    {
                        new Button
                        {
                            Label = "Discord Server",
                            Url = serverLink
                        },
                        new Button
                        {
                            Label = "Download",
                            Url = "https://github.com/iiDk-the-actual/iis.Stupid.Menu/"
                        }
                    }
                });
            }
        }

        public static void DisableDiscordRPC()
        {
            if (discord != null)
            {
                discord.ClearPresence();
                discord.Dispose();
                discord = null;
            }
        }

        private static bool quickSongExists;
        public static void EnsureIntegrationProgram()
        {
            quickSongExists = File.Exists($"{PluginInfo.BaseDirectory}/QuickSong.exe");
            if (!quickSongExists)
            {
                Prompt("This mod requires the \"QuickSong\" library. Would you like to automatically download it? (16.3mb)", () =>
                {
                    using UnityWebRequest request = UnityWebRequest.Get("https://github.com/iiDk-the-actual/QuickSong/releases/latest/download/QuickSong.exe");
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                    while (!operation.isDone) { }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllBytes($"{PluginInfo.BaseDirectory}/QuickSong.exe", request.downloadHandler.data);
                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded QuickSong to {PluginInfo.BaseDirectory}/QuickSong.exe.");
                    }
                    else
                        NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not download QuickSong: {(request.error.IsNullOrEmpty() ? "Unknown error" : request.error)}");

                    quickSongExists = File.Exists($"{PluginInfo.BaseDirectory}/QuickSong.exe");
                }, () => Toggle("Media Integration"));
            }
        }

        public static string Title { get; private set; } = "Unknown";
        public static string Artist { get; private set; } = "Unknown";
        public static Texture2D Icon { get; private set; } = new Texture2D(2, 2);
        public static bool Paused { get; private set; } = true;

        public static float StartTime { get; private set; }
        public static float EndTime { get; private set; }
        public static float ElapsedTime { get; private set; }

        public static bool ValidData { get; private set; }

        public static async Task UpdateDataAsync()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = $"{FileUtilities.GetGamePath()}/{PluginInfo.BaseDirectory}/QuickSong.exe",
                Arguments = "-all",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using Process proc = new Process { StartInfo = psi };
            proc.Start();
            string output = await proc.StandardOutput.ReadToEndAsync();

            await Task.Run(() => proc.WaitForExit());

            Paused = true;
            Title = "Unknown";
            Artist = "Unknown";

            StartTime = 0f;
            EndTime = 0f;
            ElapsedTime = 0f;

            try
            {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
                Title = (string)data["Title"];
                Artist = (string)data["Artist"];

                StartTime = Convert.ToSingle(data["StartTime"]);
                EndTime = Convert.ToSingle(data["EndTime"]);
                ElapsedTime = Convert.ToSingle(data["ElapsedTime"]);

                Paused = (string)data["Status"] != "Playing";
                Icon.LoadImage(Convert.FromBase64String((string)data["ThumbnailBase64"]));

                ValidData = true;
            }
            catch { }
        }

        private static IEnumerator UpdateDataCoroutine(float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            _ = UpdateDataAsync();
            yield return null;
        }

        // Credits to The-Graze/MusicControls for control methods
        internal enum VirtualKeyCodes : uint
        {
            NEXT_TRACK = 0xB0,
            PREVIOUS_TRACK = 0xB1,
            PLAY_PAUSE = 0xB3,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);
        internal static void SendKey(VirtualKeyCodes virtualKeyCode) => keybd_event((uint)virtualKeyCode, 0, 0, 0);

        public static void PreviousTrack()
        {
            CoroutineManager.instance.StartCoroutine(UpdateDataCoroutine(0.1f));
            ElapsedTime = 0f;
            SendKey(VirtualKeyCodes.PREVIOUS_TRACK);
        }

        public static void PauseTrack()
        {
            Paused = !Paused;
            SendKey(VirtualKeyCodes.PLAY_PAUSE);
        }

        public static void SkipTrack()
        {
            CoroutineManager.instance.StartCoroutine(UpdateDataCoroutine(0.1f));
            ElapsedTime = 0f;
            SendKey(VirtualKeyCodes.NEXT_TRACK);
        }

        private static float updateDataDelay;
        private static float inputDelay;

        private static GameObject mediaIcon;
        private static Material mediaIconMaterial;

        private static TextMeshPro mediaText;

        private static Shader _tmpShader;
        private static Shader TmpShader
        {
            get
            {
                if (_tmpShader == null)
                    _tmpShader = LoadAsset<Shader>("TMP_SDF-Mobile Overlay");

                return _tmpShader;
            }
        }

        private static TMP_SpriteAsset _mediaSpriteSheet;
        public static TMP_SpriteAsset MediaSpriteSheet
        {
            get
            {
                if (_mediaSpriteSheet == null)
                {
                    _mediaSpriteSheet = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
                    _mediaSpriteSheet.name = "iiMenu_SpriteSheet";

                    var textureList = new List<Texture2D>();
                    var spriteDataList = new List<(string name, int index)>();

                    void AddSprite(string name, Texture2D tex)
                    {
                        spriteDataList.Add((name, textureList.Count));
                        textureList.Add(tex);
                    }

                    AddSprite("Pause", LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Important/pause.png", $"Images/Mods/Important/pause.png"));

                    int maxSize = 512;
                    Texture2D spriteSheet = new Texture2D(maxSize, maxSize);
                    Rect[] rects = spriteSheet.PackTextures(textureList.ToArray(), 2, maxSize);

                    _mediaSpriteSheet.spriteSheet = spriteSheet;
                    _mediaSpriteSheet.material = new Material(Shader.Find("TextMeshPro/Sprite"))
                    {
                        mainTexture = spriteSheet
                    };

                    _mediaSpriteSheet.spriteInfoList = new List<TMP_Sprite>();
                    Traverse.Create(_mediaSpriteSheet).Field("m_Version").SetValue("1.1.0"); // TextMeshPro kills itself unless this is set.

                    _mediaSpriteSheet.spriteGlyphTable.Clear();
                    for (int i = 0; i < spriteDataList.Count; i++)
                    {
                        var rect = rects[i];

                        var glyph = new TMP_SpriteGlyph
                        {
                            index = (uint)i,
                            metrics = new GlyphMetrics(
                                width: rect.width * spriteSheet.width,
                                height: rect.height * spriteSheet.height,
                                bearingX: -(rect.width * spriteSheet.width) / 2f,
                                bearingY: rect.height * spriteSheet.height * 0.8f,
                                advance: rect.width * spriteSheet.width
                            ),
                            glyphRect = new GlyphRect(
                                x: (int)(rect.x * spriteSheet.width),
                                y: (int)(rect.y * spriteSheet.height),
                                width: (int)(rect.width * spriteSheet.width),
                                height: (int)(rect.height * spriteSheet.height)
                            ),
                            scale = 1f,
                            atlasIndex = 0
                        };
                        _mediaSpriteSheet.spriteGlyphTable.Add(glyph);
                    }

                    _mediaSpriteSheet.spriteCharacterTable.Clear();
                    for (int i = 0; i < spriteDataList.Count; i++)
                    {
                        var (name, _) = spriteDataList[i];

                        var character = new TMP_SpriteCharacter(0xFFFE, _mediaSpriteSheet.spriteGlyphTable[i])
                        {
                            name = name,
                            scale = 1f,
                            glyphIndex = (uint)i
                        };
                        _mediaSpriteSheet.spriteCharacterTable.Add(character);
                    }

                    _mediaSpriteSheet.UpdateLookupTables();
                }
                return _mediaSpriteSheet;
            }
        }

        public static void MediaIntegration()
        {
            if (quickSongExists)
            {
                if (mediaIcon == null)
                {
                    mediaIcon = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(mediaIcon.GetComponent<Collider>());

                    if (mediaIconMaterial == null)
                        mediaIconMaterial = new Material(LoadAsset<Shader>("Chams"));

                    mediaIcon.GetComponent<Renderer>().material = mediaIconMaterial;
                }

                mediaIcon.transform.localScale = new Vector3(0.25f, 0.25f, 0.01f) * VRRig.LocalRig.scaleFactor;
                mediaIcon.transform.position = GorillaTagger.Instance.headCollider.transform.TransformPoint(new Vector3(-0.5f, 0.2f, 1f));
                mediaIcon.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);

                if (mediaText == null)
                {
                    GameObject textHolder = new GameObject("iiMenu_MediaText");

                    TextMeshPro text = textHolder.GetOrAddComponent<TextMeshPro>();
                    text.color = Color.white;
                    text.fontSize = 0.75f;
                    text.fontStyle = activeFontStyle;
                    text.font = activeFont;
                    text.alignment = TextAlignmentOptions.Left;
                    text.spriteAsset = MediaSpriteSheet;
                    text.margin = new Vector4(0.5f, 0, 0, 0);

                    if (text != null && text.fontMaterial != null)
                        text.fontMaterial.shader = TmpShader;

                    mediaText = text;
                }

                mediaText.transform.localScale = Vector3.one * VRRig.LocalRig.scaleFactor;
                mediaText.transform.position = GorillaTagger.Instance.headCollider.transform.TransformPoint(new Vector3(-0.35f, 0.2f, 1f));
                mediaText.transform.LookAt(Camera.main.transform.position);
                mediaText.transform.Rotate(0f, 180f, 0f);
                mediaText.transform.position += mediaText.transform.right * mediaText.bounds.size.x;

                FollowMenuSettings(mediaText);

                float clampedElapsed = Mathf.Clamp(ElapsedTime, StartTime, EndTime);
                mediaText.text =
                    $@"{Artist} - {Title}
{(Paused ? "  <sprite name=\"Pause\"> " : "")}{Mathf.Floor(clampedElapsed / 60)}:{Mathf.Floor(clampedElapsed % 60):00} - {Mathf.Floor(EndTime / 60)}:{Mathf.Floor(EndTime % 60):00}";

                if (Time.time > updateDataDelay)
                {
                    updateDataDelay = Time.time + 5f;
                    CoroutineManager.instance.StartCoroutine(UpdateDataCoroutine());
                }

                if (!Paused)
                    ElapsedTime += Time.deltaTime;

                if (Time.time > inputDelay)
                {
                    if (Mathf.Abs(leftJoystick.x) > 0.5f)
                    {
                        inputDelay = Time.time + 0.5f;
                        PlayButtonSound(null, true, true);

                        if (leftJoystick.x > 0f)
                            SkipTrack();
                        else
                            PreviousTrack();
                    }

                    if (leftJoystickClick)
                    {
                        inputDelay = Time.time + 0.5f;
                        PlayButtonSound(null, true, true);

                        PauseTrack();
                    }
                }

                Texture2D targetIcon = Icon == null || !ValidData ? null : Icon;
                Renderer icon = mediaIcon.GetComponent<Renderer>();

                if (icon.material.GetTexture("_MainTex") != targetIcon)
                    icon.material.SetTexture("_MainTex", targetIcon);
            }
        }

        public static void DisableMediaIntegration()
        {
            quickSongExists = false;

            if (mediaIcon != null)
                Object.Destroy(mediaIcon);

            if (mediaText != null)
                Object.Destroy(mediaText.gameObject);

            mediaIcon = null;
            mediaText = null;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private static bool wasenabled = true;

        public static void EnableFPC()
        {
            if (TPC != null)
                wasenabled = TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled;
        }

        public static float zoomFOV = 35f;
        public static void MoveFPC()
        {
            if (TPC != null)
            {
                if (menu != null && !XRSettings.isDeviceActive)
                    return;

                float FOV = 90f;
                if (Keyboard.current.cKey.isPressed)
                {
                    Vector2 scroll = Mouse.current.scroll.ReadValue();
                    zoomFOV += -scroll.y * 5f;
                    zoomFOV = Mathf.Clamp(zoomFOV, 10f, 90f);
                    TPC.fieldOfView = Mathf.Lerp(TPC.fieldOfView, zoomFOV, 0.1f);
                }
                else
                {
                    zoomFOV = 35f;
                    TPC.fieldOfView = Mathf.Lerp(TPC.fieldOfView, FOV, 0.1f);
                }
                TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = false;
                TPC.gameObject.transform.position = Keyboard.current.cKey.isPressed ? Vector3.Lerp(TPC.transform.position, GorillaTagger.Instance.headCollider.transform.position, 0.1f) : GorillaTagger.Instance.headCollider.transform.position;
                TPC.gameObject.transform.rotation = Quaternion.Lerp(TPC.transform.rotation, GorillaTagger.Instance.headCollider.transform.rotation, 0.075f);
            }
        }

        public static void DisableFPC()
        {
            if (TPC != null)
            {
                TPC.GetComponent<Camera>().fieldOfView = 60f;
                TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = wasenabled;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public static void ForceEnableHands()
        {
            if (!XRSettings.isDeviceActive)
                return;

            ConnectedControllerHandler.Instance.leftControllerValid = true;
            ConnectedControllerHandler.Instance.rightControllerValid = true;

            ConnectedControllerHandler.Instance.leftValid = true;
            ConnectedControllerHandler.Instance.rightValid = true;
        }

        private static bool reportMenuToggle;
        public static void OculusReportMenu()
        {
            if (leftPrimary && !reportMenuToggle)
            {
                GorillaMetaReport metaReporting = GetObject("Miscellaneous Scripts").transform.Find("MetaReporting").GetComponent<GorillaMetaReport>();
                metaReporting.gameObject.SetActive(true);
                metaReporting.enabled = true;

                metaReporting.StartOverlay();
            }
            reportMenuToggle = leftPrimary;
        }

        private static bool acceptedTOS;
        public static void AcceptTOS()
        {
            GameObject RoomObject = GetObject("Miscellaneous Scripts/PrivateUIRoom_HandRays");
            if (RoomObject == null)
                return;

            HandRayController HandRayController = RoomObject.GetComponent<HandRayController>();
            PrivateUIRoom PrivateUIRoom = RoomObject.GetComponent<PrivateUIRoom>();

            if (!acceptedTOS && PrivateUIRoom.inOverlay)
            {
                HandRayController.DisableHandRays();

                PrivateUIRoom.overlayForcedActive = false;
                PrivateUIRoom.StopOverlay();

                if (!TOSPatches.enabled)
                {
                    GorillaTagger.Instance.tapHapticStrength = 0.5f;
                    GorillaSnapTurn.LoadSettingsFromCache();
                    TOSPatches.enabled = true;
                }

                acceptedTOS = true;
            }

            if (RoomObject.activeSelf)
                RoomObject.SetActive(false);
        }
        
        public static IEnumerator RedeemShinyRocks()
        {
            Task<GetPlayerData_Data> newSessionDataTask = KIDManager.TryGetPlayerData(true); 

            while (!newSessionDataTask.IsCompleted)
                yield return null;
            if (newSessionDataTask.IsFaulted)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");

            GetPlayerData_Data newSessionData = newSessionDataTask.Result;
            if (newSessionData.responseType == GetSessionResponseType.NOT_FOUND)
            {
                Task optInTask = KIDManager.Server_OptIn();

                while (!optInTask.IsCompleted)
                    yield return null;
                if (optInTask.IsFaulted)
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");

                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully redeemed shiny rocks!");
                CosmeticsController.instance.GetCurrencyBalance();
            }
            else
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You have already redeemed the shiny rocks.");
            yield break;
        }

        public static void JoinDiscord() =>
            Process.Start(serverLink);

        public static void CopyPlayerPosition()
        {
            string text = "Body\n";
            Transform p = GorillaTagger.Instance.bodyCollider.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);\n\n";

            text += "Head\n";
            p = GorillaTagger.Instance.headCollider.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);\n\n";

            text += "Left Hand\n";
            p = VRRig.LocalRig.leftHand.rigTarget.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);\n\n";

            text += "Right Hand\n";
            p = VRRig.LocalRig.rightHand.rigTarget.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);";

            GUIUtility.systemCopyBuffer = text;
        }

        public static GameObject physicalQuitBox;
        public static void PhysicalQuitbox()
        {
            GameObject quitBox = GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox");
            physicalQuitBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            physicalQuitBox.transform.position = quitBox.transform.position;
            physicalQuitBox.transform.rotation = quitBox.transform.rotation;
            physicalQuitBox.transform.localScale = quitBox.transform.localScale;
            physicalQuitBox.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;

            quitBox.SetActive(false);
        }

        public static void DisablePhysicalQuitbox()
        {
            Object.Destroy(physicalQuitBox);
            GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true);
        }

        public static void BlockOnMute()
        {
            bool selfTagged = VRRig.LocalRig.IsTagged();
            foreach (VRRig rig in GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal() && rig.muted))
            {
                if (GameModeUtilities.InfectedList().Count <= 0 || (selfTagged ? !rig.IsTagged() : rig.IsTagged()))
                    rig.transform.position = rig.syncPos - (Vector3.up * 99999f);
            }
        }

        public static void DisablePitchScaling()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                vrrig.voicePitchForRelativeScale = new AnimationCurve(
                    new Keyframe(0f, 1f, 0f, 0f),
                    new Keyframe(1f, 1f, 0f, 0f)
                );
            }
        }

        public static void EnablePitchScaling()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
                vrrig.voicePitchForRelativeScale = VRRig.LocalRig.voicePitchForRelativeScale;
        }

        public static void DisableMouthMovement()
        {
            VRRig.LocalRig.shouldSendSpeakingLoudness = false;
            LoudnessPatch.enabled = true;
        }

        public static void EnableMouthMovement()
        {
            VRRig.LocalRig.shouldSendSpeakingLoudness = true;
            LoudnessPatch.enabled = false;
        }

        private static float lastTime;
        public static void CapFPS(int fps)
        {
            float targetDelta = 1f / fps;
            float elapsed = Time.realtimeSinceStartup - lastTime;

            if (elapsed < targetDelta)
            {
                int sleepMs = Mathf.FloorToInt((targetDelta - elapsed) * 1000);
                if (sleepMs > 0)
                    Thread.Sleep(sleepMs);
            }

            lastTime = Time.realtimeSinceStartup;
        }

        public static void UncapFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = int.MaxValue;
        }

        private static Vector3? oldLocalPosition;
        public static void PCButtonClick()
        {
            if (Mouse.current.leftButton.isPressed && GunPointer == null)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out var Ray, 512f, NoInvisLayerMask());

                oldLocalPosition ??= GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition;
                GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = false;
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = Ray.point;
            }
            else
            {
                if (oldLocalPosition != null)
                {
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPosition.Value;
                    oldLocalPosition = null;
                }
                GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = true;
            }
        }

        public static void DisablePCButtonClick()
        {
            if (oldLocalPosition != null)
            {
                GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPosition.Value;
                oldLocalPosition = null;
            }
        }

        public static void PCControllerEmulation()
        {
            ControllerInputPoller.instance.rightControllerPrimaryButton |= UnityInput.Current.GetKey(KeyCode.E);
            ControllerInputPoller.instance.rightControllerSecondaryButton |= UnityInput.Current.GetKey(KeyCode.R);

            ControllerInputPoller.instance.leftControllerPrimaryButton |= UnityInput.Current.GetKey(KeyCode.F);
            ControllerInputPoller.instance.leftControllerSecondaryButton |= UnityInput.Current.GetKey(KeyCode.G);

            ControllerInputPoller.instance.leftGrab |= UnityInput.Current.GetKey(KeyCode.LeftBracket);
            ControllerInputPoller.instance.leftControllerGripFloat += UnityInput.Current.GetKey(KeyCode.LeftBracket) ? 1f : 0f;

            ControllerInputPoller.instance.rightGrab |= UnityInput.Current.GetKey(KeyCode.RightBracket);
            ControllerInputPoller.instance.rightControllerGripFloat += UnityInput.Current.GetKey(KeyCode.RightBracket) ? 1f : 0f;

            ControllerInputPoller.instance.rightControllerTriggerButton |= UnityInput.Current.GetKey(KeyCode.Equals);
            ControllerInputPoller.instance.rightControllerIndexFloat += UnityInput.Current.GetKey(KeyCode.Equals) ? 1f : 0f;

            ControllerInputPoller.instance.leftControllerTriggerButton |= UnityInput.Current.GetKey(KeyCode.Minus);
            ControllerInputPoller.instance.leftControllerIndexFloat += UnityInput.Current.GetKey(KeyCode.Minus) ? 1f : 0f;

            ControllerInputPoller.instance.rightControllerTriggerButton |= UnityInput.Current.GetKey(KeyCode.Equals);
            ControllerInputPoller.instance.rightControllerIndexFloat += UnityInput.Current.GetKey(KeyCode.Equals) ? 1f : 0f;
        }

        public static void ConnectToRegion(string region)
        {
            string currentRegion = PhotonNetwork.CloudRegion;
            if (!string.IsNullOrEmpty(currentRegion))
                currentRegion = currentRegion.Replace("/*", "");

            if (currentRegion != region)
                PhotonNetwork.ConnectToRegion(region);

            NetworkSystem.Instance.currentRegionIndex = Array.IndexOf(NetworkSystem.Instance.regionNames, region);

            NetworkSystemPUN punNetwork = (NetworkSystemPUN)NetworkSystem.Instance;
            for (int i = 0; i < punNetwork.regionData.Length; i++)
            {
                NetworkRegionInfo regionInfo = punNetwork.regionData[i];
                regionInfo.pingToRegion = Array.IndexOf(NetworkSystem.Instance.regionNames, regionInfo) == i ? 0 : 9999;
            }
        }

        private static bool lastTagLag;
        public static void TagLagDetector()
        {
            if (PhotonNetwork.InRoom && !NetworkSystem.Instance.IsMasterClient)
            {
                VRRig masterRig = PhotonNetwork.MasterClient.VRRig();
                bool thereIsTagLag = masterRig.GetTruePing() > 1000;

                switch (thereIsTagLag)
                {
                    case true when !lastTagLag:
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>TAG LAG</color><color=grey>]</color> There is currently tag lag.");
                        break;
                    case false when lastTagLag:
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> There is no longer tag lag.");
                        break;
                }

                lastTagLag = thereIsTagLag;
            } else
            {
                if (lastTagLag)
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> There is no longer tag lag.");
                lastTagLag = false;
            }
        }

        private static bool lastSteam;
        public static void SteamDetector()
        {
            bool playerOnSteam = GorillaParent.instance.vrrigs.Any(vrrig => !vrrig.IsLocal() && vrrig.IsSteam());
            if (playerOnSteam && !lastSteam)
            {
                VRRig vrrig = GorillaParent.instance.vrrigs.First(vrrig => !vrrig.IsLocal() && vrrig.IsSteam());
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>STEAM</color><color=grey>]</color> {vrrig.GetName()} is on Steam.");

                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Safety/steam.ogg", "Audio/Mods/Safety/steam.ogg"), buttonClickVolume / 10f);
            }

            lastSteam = playerOnSteam;
        }

        public static string RandomRoomName()
        {
            while (true)
            {
                string text = RandomString();
                if (GorillaComputer.instance.CheckAutoBanListForName(text)) return text;
            }
        }
    }
}
