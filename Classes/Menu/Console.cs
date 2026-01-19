/*
 * ii's Stupid Menu  Classes/Menu/Console.cs
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
using GorillaNetworking;
using GorillaTag.Rendering;
using HarmonyLib;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Mods;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;
using JoinType = GorillaNetworking.JoinType;
using Random = UnityEngine.Random;

namespace iiMenu.Classes.Menu
{
    public class Console : MonoBehaviour
    {
        #region Configuration
        public static readonly string MenuName = "stupid";
        public static readonly string MenuVersion = PluginInfo.Version;

        public static readonly string ConsoleResourceLocation = $"{PluginInfo.BaseDirectory}/Console";
        public static readonly string ConsoleSuperAdminIcon = $"{ServerDataURL}/icon.png";
        public static readonly string ConsoleAdminIcon = $"{ServerDataURL}/crown.png";

        public static bool DisableMenu // Variable used to disable menu from opening
        {
            get => Main.Lockdown;
            set =>
                Main.Lockdown = value;
        }

        public static void SendNotification(string text, int sendTime = 1000) => // Method used to spawn notifications
            NotificationManager.SendNotification(text, sendTime);

        public static void TeleportPlayer(Vector3 position) // Only modify this if you need any special logic
        {
            GTPlayer.Instance.TeleportTo(position, GTPlayer.Instance.transform.rotation);
            VRRig.LocalRig.transform.position = position;
            Movement.lastPosition = position;
            Main.closePosition = position;
        }

        public static void EnableMod(string mod, bool enable) // Method used to enable mods
        {
            if (mod == "Decline Prompt" || mod == "Accept Prompt") // Can be vulnerabized
                return;

            ButtonInfo Button = Buttons.GetIndex(mod);
            if (!Button.isTogglable)
                Button.method.Invoke();
            else
            {
                Button.enabled = !enable;
                ToggleMod(Button.buttonText);
            }
        }

        public static void ToggleMod(string mod)
        {
            if (mod == "Decline Prompt" || mod == "Accept Prompt") // Can be vulnerabized
                return;

            Main.Toggle(mod);
        }

        public static void ConfirmUsing(string id, string version, string menuName) => // Code ran on isusing call
            Visuals.ConsoleBeacon(id, version, menuName);

        public static void Log(string text) => // Method used to log info
            LogManager.Log(text);
        #endregion

        #region Events
        public static readonly string ConsoleVersion = "2.9.3";
        public static Console instance;

        public void Awake()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;

            NetworkSystem.Instance.OnReturnedToSinglePlayer += ClearConsoleAssets;
            NetworkSystem.Instance.OnPlayerJoined += SyncConsoleAssets;
            NetworkSystem.Instance.OnPlayerLeft += SyncConsoleUsers;

            string blockDir = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0] + "Console.txt";
            if (File.Exists(blockDir))
                isBlocked = long.Parse(File.ReadAllText(blockDir));
            NetworkSystem.Instance.OnJoinedRoomEvent += BlockedCheck;

            if (!Directory.Exists(ConsoleResourceLocation))
                Directory.CreateDirectory(ConsoleResourceLocation);

            instance.StartCoroutine(DownloadAdminTextures());
            instance.StartCoroutine(PreloadAssets());

            Log($@"

     ▄▄·        ▐ ▄ .▄▄ ·       ▄▄▌  ▄▄▄ .
    ▐█ ▌▪▪     •█▌▐█▐█ ▀. ▪     ██•  ▀▄.▀·
    ██ ▄▄ ▄█▀▄ ▐█▐▐▌▄▀▀▀█▄ ▄█▀▄ ██▪  ▐▀▀▪▄
    ▐███▌▐█▌.▐▌██▐█▌▐█▄▪▐█▐█▌.▐▌▐█▌▐▌▐█▄▄▌
    ·▀▀▀  ▀█▄▀▪▀▀ █▪ ▀▀▀▀  ▀█▄▀▪.▀▀▀  ▀▀▀       
           Console {MenuName} {ConsoleVersion}
     Developed by goldentrophy & Twigcore
");

            (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).supportsCameraOpaqueTexture = true;
            (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).supportsCameraDepthTexture = true;
        }

        public static void LoadConsole() =>
            GorillaTagger.OnPlayerSpawned(() => LoadConsoleImmediately());

        public const string LoadVersionEventKey = "%<CONSOLE>%LoadVersion"; // Do not change this, it's used to prevent multiple instances of Console from colliding with each other
        public static void NoOverlapEvents(string eventName, int id)
        {
            if (eventName == LoadVersionEventKey)
            {
                if (ServerData.VersionToNumber(ConsoleVersion) <= id)
                {
                    PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;
                    PlayerGameEvents.OnMiscEvent += ConsoleAssetCommunication;
                }
            }
        }

        public const string SyncAssetsEventKey = "%<CONSOLE>%SyncAssets";
        public static void ConsoleAssetCommunication(string eventName, int id)
        {
            if (eventName.StartsWith(SyncAssetsEventKey))
            {
                string[] data = eventName.Split("||");
                string command = data[0];
                switch (command)
                {
                    case "spawn":
                        string assetName = data[1];
                        string assetBundle = data[2];
                        string linkObjectName = data[3];

                        instance.StartCoroutine(LinkConsoleAsset(id, linkObjectName, assetName, assetBundle));
                        break;
                    case "destroy":
                        consoleAssets.Remove(id);
                        break;
                    case "confirmusing":
                        ConfirmUsing(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(id).UserId, data[1], data[2]);
                        break;
                }
            }
        }

        public static void CommunicateConsole(string command, int id, params object[] args)
        {
            string eventName = $"{SyncAssetsEventKey}||{command}";
            if (args.Length > 0)
                eventName += $"||{string.Join("||", args)}";

            PlayerGameEvents.MiscEvent(eventName, id);
        }

        public static IEnumerator LinkConsoleAsset(int id, string linkObjectName, string assetName, string assetBundle)
        {
            if (!PhotonNetwork.InRoom)
            {
                Log("Attempt to retrieve asset while not in room");
                yield break;
            }

            if (GameObject.Find(linkObjectName) == null)
            {
                float timeoutTime = Time.time + 10f;
                while (Time.time < timeoutTime && GameObject.Find(linkObjectName) == null)
                    yield return null;
            }

            GameObject finalLink = GameObject.Find(linkObjectName);
            if (finalLink == null)
            {
                Log("Failed to retrieve asset from link");
                yield break;
            }

            if (!PhotonNetwork.InRoom)
            {
                Log("Attempt to retrieve asset while not in room");
                yield break;
            }

            consoleAssets.Add(id, new ConsoleAsset(id, finalLink.transform.parent.gameObject, assetName, assetBundle));
        }

        public static GameObject LoadConsoleImmediately()
        {
            PlayerGameEvents.MiscEvent(LoadVersionEventKey, ServerData.VersionToNumber(ConsoleVersion));
            PlayerGameEvents.OnMiscEvent += NoOverlapEvents;

            string ConsoleGUID = "goldentrophy_Console";
            GameObject ConsoleObject = GameObject.Find(ConsoleGUID) ?? new GameObject(ConsoleGUID);
            ConsoleObject.AddComponent<Console>();

            if (ServerData.ServerDataEnabled)
                ConsoleObject.AddComponent<ServerData>();

            return ConsoleObject;
        }

        public void OnDisable() =>
            PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            string justName = Path.GetFileName(fileName);

            if (string.IsNullOrWhiteSpace(justName))
                return null;

            foreach (char c in Path.GetInvalidFileNameChars())
                justName = justName.Replace(c.ToString(), "");

            return justName;
        }

        private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public static IEnumerator GetTextureResource(string url, Action<Texture2D> onComplete = null)
        {
            if (!textures.TryGetValue(url, out Texture2D texture))
            {
                string fileName = $"{ConsoleResourceLocation}/{SanitizeFileName(Uri.UnescapeDataString(url.Split("/")[^1]))}";

                if (fileName == null)
                    yield break;

                if (File.Exists(fileName))
                    File.Delete(fileName);

                Log($"Downloading {fileName}");
                using HttpClient client = new HttpClient();
                Task<byte[]> downloadTask = client.GetByteArrayAsync(url);

                while (!downloadTask.IsCompleted)
                    yield return null;

                if (downloadTask.Exception != null)
                {
                    Log("Failed to download texture: " + downloadTask.Exception);
                    yield break;
                }

                byte[] downloadedData = downloadTask.Result;
                Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);

                while (!writeTask.IsCompleted)
                    yield return null;

                if (writeTask.Exception != null)
                {
                    Log("Failed to save texture: " + writeTask.Exception);
                    yield break;
                }

                Task<byte[]> readTask = File.ReadAllBytesAsync(fileName);
                while (!readTask.IsCompleted)
                    yield return null;

                if (readTask.Exception != null)
                {
                    Log("Failed to read texture file: " + readTask.Exception);
                    yield break;
                }

                byte[] bytes = readTask.Result;
                texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
            }

            textures[url] = texture;
            onComplete?.Invoke(texture);
        }

        private static readonly Dictionary<string, AudioClip> audios = new Dictionary<string, AudioClip>();
        public static IEnumerator GetSoundResource(string url, Action<AudioClip> onComplete = null)
        {
            if (!audios.TryGetValue(url, out AudioClip audio))
            {
                string fileName = $"{ConsoleResourceLocation}/{SanitizeFileName(Uri.UnescapeDataString(url.Split("/")[^1]))}";

                if (fileName == null)
                    yield break;

                if (File.Exists(fileName))
                    File.Delete(fileName);

                Log($"Downloading {fileName}");
                using HttpClient client = new HttpClient();
                Task<byte[]> downloadTask = client.GetByteArrayAsync(url);

                while (!downloadTask.IsCompleted)
                    yield return null;

                if (downloadTask.Exception != null)
                {
                    Log("Failed to download texture: " + downloadTask.Exception);
                    yield break;
                }

                byte[] downloadedData = downloadTask.Result;
                Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);

                while (!writeTask.IsCompleted)
                    yield return null;

                if (writeTask.Exception != null)
                {
                    Log("Failed to save texture: " + writeTask.Exception);
                    yield break;
                }

                string filePath = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0] + fileName;

                Log($"Loading audio from {filePath}");

                using UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(
                    $"file://{filePath}",
                    GetAudioType(GetFileExtension(fileName))
                );
                yield return audioRequest.SendWebRequest();

                if (audioRequest.result != UnityWebRequest.Result.Success)
                {
                    Log("Failed to load audio: " + audioRequest.error);
                    yield break;
                }

                audio = DownloadHandlerAudioClip.GetContent(audioRequest);
            }

            audios[url] = audio;
            onComplete?.Invoke(audio);
        }

        public static IEnumerator PlaySoundMicrophone(AudioClip sound)
        {
            GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
            GorillaTagger.Instance.myRecorder.AudioClip = sound;
            GorillaTagger.Instance.myRecorder.RestartRecording(true);
            GorillaTagger.Instance.myRecorder.DebugEchoMode = true;

            yield return new WaitForSeconds(sound.length + 0.4f);

            GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
            GorillaTagger.Instance.myRecorder.AudioClip = null;
            GorillaTagger.Instance.myRecorder.RestartRecording(true);
            GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
        }

        public static IEnumerator DownloadAdminTextures()
        {
            {
                string fileName = $"{ConsoleResourceLocation}/cone.png";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                Log($"Downloading {fileName}");
                using HttpClient client = new HttpClient();
                Task<byte[]> downloadTask = client.GetByteArrayAsync(ConsoleSuperAdminIcon);

                while (!downloadTask.IsCompleted)
                    yield return null;

                if (downloadTask.Exception != null)
                {
                    Log("Failed to download texture: " + downloadTask.Exception);
                    yield break;
                }

                byte[] downloadedData = downloadTask.Result;
                Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);

                while (!writeTask.IsCompleted)
                    yield return null;

                if (writeTask.Exception != null)
                {
                    Log("Failed to save texture: " + writeTask.Exception);
                    yield break;
                }

                Task<byte[]> readTask = File.ReadAllBytesAsync(fileName);
                while (!readTask.IsCompleted)
                    yield return null;

                if (readTask.Exception != null)
                {
                    Log("Failed to read texture file: " + readTask.Exception);
                    yield break;
                }

                byte[] bytes = readTask.Result;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);

                adminConeTexture = texture;
            }

            {
                string fileName = $"{ConsoleResourceLocation}/crown.png";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                Log($"Downloading {fileName}");
                using HttpClient client = new HttpClient();
                Task<byte[]> downloadTask = client.GetByteArrayAsync(ConsoleAdminIcon);

                while (!downloadTask.IsCompleted)
                    yield return null;

                if (downloadTask.Exception != null)
                {
                    Log("Failed to download texture: " + downloadTask.Exception);
                    yield break;
                }

                byte[] downloadedData = downloadTask.Result;
                Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);

                while (!writeTask.IsCompleted)
                    yield return null;

                if (writeTask.Exception != null)
                {
                    Log("Failed to save texture: " + writeTask.Exception);
                    yield break;
                }

                Task<byte[]> readTask = File.ReadAllBytesAsync(fileName);
                while (!readTask.IsCompleted)
                    yield return null;

                if (readTask.Exception != null)
                {
                    Log("Failed to read texture file: " + readTask.Exception);
                    yield break;
                }

                byte[] bytes = readTask.Result;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);

                adminCrownTexture = texture;
            }
        }

        public static string GetFileExtension(string fileName) =>
            fileName.ToLower().Split(".")[fileName.Split(".").Length - 1];

        public static AudioType GetAudioType(string extension)
        {
            return extension.ToLower() switch
            {
                "mp3" => AudioType.MPEG,
                "wav" => AudioType.WAV,
                "ogg" => AudioType.OGGVORBIS,
                "aiff" => AudioType.AIFF,
                _ => AudioType.WAV,
            };
        }

        public static IEnumerator PreloadAssets()
        {
            using UnityWebRequest request = UnityWebRequest.Get($"{ServerDataURL}/PreloadedAssets.txt");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string returnText = request.downloadHandler.text;

                foreach (string assetBundle in returnText.Split("\n"))
                {
                    if (assetBundle.Length > 0)
                        instance.StartCoroutine(PreloadAssetBundle(assetBundle));
                }
            }
        }

        public const byte ConsoleByte = 68; // Do not change this unless you want a local version of Console only your mod can be used by
        public const string ServerDataURL = "https://raw.githubusercontent.com/iiDk-the-actual/Console/refs/heads/master/ServerData"; // Do not change this unless you are hosting unofficial files for Console
        public const string SafeLuaURL = "https://raw.githubusercontent.com/iiDk-the-actual/Console/refs/heads/master/SafeLua"; // Do not change this unless you are hosting unofficial files for Console

        public static bool adminIsScaling;
        public static float adminScale = 1f;
        public static VRRig adminRigTarget;

        public static readonly List<Player> excludedCones = new List<Player>();
        public static readonly Dictionary<VRRig, GameObject> conePool = new Dictionary<VRRig, GameObject>();

        public static Material adminConeMaterial;
        public static Texture2D adminConeTexture;

        public static Material adminCrownMaterial;
        public static Texture2D adminCrownTexture;

        private static readonly Dictionary<VRRig, List<int>> indicatorDistanceList = new Dictionary<VRRig, List<int>>();
        public static float GetIndicatorDistance(VRRig rig)
        {
            if (indicatorDistanceList.ContainsKey(rig))
            {
                if (indicatorDistanceList[rig][0] == Time.frameCount)
                {
                    indicatorDistanceList[rig].Add(Time.frameCount);
                    return (0.3f + indicatorDistanceList[rig].Count * 0.5f);
                }

                indicatorDistanceList[rig].Clear();
                indicatorDistanceList[rig].Add(Time.frameCount);
                return (0.3f + indicatorDistanceList[rig].Count * 0.5f);
            }

            indicatorDistanceList.Add(rig, new List<int> { Time.frameCount });
            return 0.8f;
        }

        public void Update()
        {
            if (PhotonNetwork.InRoom)
            {
                try
                {
                    List<VRRig> toRemove = new List<VRRig>();

                    foreach (var nametag in from nametag in conePool
                                            let nametagPlayer = nametag.Key.Creator?.GetPlayerRef()
                                            where !GorillaParent.instance.vrrigs.Contains(nametag.Key) ||
                                 nametagPlayer == null ||
                                 !ServerData.Administrators.ContainsKey(nametagPlayer.UserId) ||
                                 excludedCones.Contains(nametagPlayer)
                                            select nametag)
                    {
                        Destroy(nametag.Value);
                        toRemove.Add(nametag.Key);
                    }

                    foreach (VRRig rig in toRemove)
                        conePool.Remove(rig);

                    bool localIsSuperAdmin =
                        ServerData.Administrators.TryGetValue(PhotonNetwork.LocalPlayer.UserId, out string localAdminName) &&
                        ServerData.SuperAdministrators.Contains(localAdminName);

                    // Admin indicators
                    foreach (Player player in PhotonNetwork.PlayerListOthers)
                    {
                        if (ServerData.Administrators.TryGetValue(player.UserId, out string adminName) && (localIsSuperAdmin || !excludedCones.Contains(player)))
                        {
                            VRRig playerRig = GetVRRigFromPlayer(player);
                            if (playerRig != null)
                            {
                                if (!conePool.TryGetValue(playerRig, out GameObject adminConeObject))
                                {
                                    adminConeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    Destroy(adminConeObject.GetComponent<Collider>());

                                    if (adminCrownMaterial == null)
                                    {
                                        adminCrownMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                                        {
                                            mainTexture = adminCrownTexture
                                        };

                                        adminCrownMaterial.SetFloat("_Surface", 1);
                                        adminCrownMaterial.SetFloat("_Blend", 0);
                                        adminCrownMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                        adminCrownMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                        adminCrownMaterial.SetFloat("_ZWrite", 0);
                                        adminCrownMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                        adminCrownMaterial.renderQueue = (int)RenderQueue.Transparent;
                                    }

                                    if (adminConeMaterial == null)
                                    {
                                        adminConeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                                        {
                                            mainTexture = adminConeTexture
                                        };

                                        adminConeMaterial.SetFloat("_Surface", 1);
                                        adminConeMaterial.SetFloat("_Blend", 0);
                                        adminConeMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                        adminConeMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                        adminConeMaterial.SetFloat("_ZWrite", 0);
                                        adminConeMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                        adminConeMaterial.renderQueue = (int)RenderQueue.Transparent;
                                    }

                                    adminConeObject.GetComponent<Renderer>().material = ServerData.SuperAdministrators.Contains(adminName) ? adminConeMaterial : adminCrownMaterial;
                                    conePool.Add(playerRig, adminConeObject);
                                }

                                adminConeObject.GetComponent<Renderer>().material.color = playerRig.playerColor;

                                adminConeObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * playerRig.scaleFactor;
                                adminConeObject.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * (GetIndicatorDistance(playerRig) * playerRig.scaleFactor);

                                adminConeObject.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);

                                Vector3 rot = adminConeObject.transform.rotation.eulerAngles;
                                rot += new Vector3(0f, 0f, Mathf.Sin(Time.time * 2f) * 10f);
                                adminConeObject.transform.rotation = Quaternion.Euler(rot);
                            }
                        }
                    }

                    // Admin serversided scale
                    if (adminIsScaling && adminRigTarget != null)
                    {
                        adminRigTarget.NativeScale = adminScale;
                        if (Mathf.Approximately(adminScale, 1f))
                            adminIsScaling = false;
                    }
                }
                catch { }
            }
            else
            {
                if (conePool.Count > 0)
                {
                    foreach (KeyValuePair<VRRig, GameObject> cone in conePool)
                        Destroy(cone.Value);

                    conePool.Clear();
                }
            }

            SanitizeConsoleAssets();
        }

        private static readonly Dictionary<string, Color> menuColors = new Dictionary<string, Color> {
            { "stupid", new Color32(255, 128, 0, 255) },
            { "symex", new Color32(138, 43, 226, 255) },
            { "colossal", new Color32(204, 0, 255, 255) },
            { "ccm", new Color32(204, 0, 255, 255) },
            { "untitled", new Color32(45, 115, 175, 255) },
            { "genesis", Color.blue },
            { "console", Color.gray },
            { "resurgence", new Color32(113, 10, 10, 255) },
            { "grate", new Color32(195, 145, 110, 255) },
            { "sodium", new Color32(220, 208, 255, 255) }
        };

        public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int Zone = LayerMask.NameToLayer("Zone");
        public static readonly int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static readonly int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static readonly int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
        public static readonly int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

        public static int NoInvisLayerMask() =>
            ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone | 1 << GorillaTrigger | 1 << GorillaBoundary | 1 << GorillaCosmetics | 1 << GorillaParticle);

        public static Vector3 World2Player(Vector3 world) =>
            world - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position;

        public static Color GetMenuTypeName(string type)
        {
            return menuColors.TryGetValue(type, out var typeName) ? typeName : Color.red;
        }

        public static VRRig GetVRRigFromPlayer(NetPlayer p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

        public static NetPlayer GetPlayerFromID(string id) =>
            PhotonNetwork.PlayerList.FirstOrDefault(player => player.UserId == id);

        public static Player GetMasterAdministrator()
        {
            return PhotonNetwork.PlayerList
                .Where(player => ServerData.Administrators.ContainsKey(player.UserId))
                .OrderBy(player => player.ActorNumber)
                .FirstOrDefault();
        }

        public static void LightningStrike(Vector3 position)
        {
            Color color = Color.cyan;

            GameObject line = new GameObject("LightningOuter");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = color; liner.endColor = color; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 5; liner.useWorldSpace = true;
            Vector3 victim = position;
            for (int i = 0; i < 5; i++)
            {
                VRRig.LocalRig.PlayHandTapLocal(68, false, 0.25f);
                VRRig.LocalRig.PlayHandTapLocal(68, true, 0.25f);

                liner.SetPosition(i, victim);
                victim += new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
            }
            liner.material.shader = Shader.Find("GUI/Text Shader");
            Destroy(line, 2f);

            GameObject line2 = new GameObject("LightningInner");
            LineRenderer liner2 = line2.AddComponent<LineRenderer>();
            liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.15f; liner2.endWidth = 0.15f; liner2.positionCount = 5; liner2.useWorldSpace = true;
            for (int i = 0; i < 5; i++)
                liner2.SetPosition(i, liner.GetPosition(i));

            liner2.material.shader = Shader.Find("GUI/Text Shader");
            liner2.material.renderQueue = liner.material.renderQueue + 1;
            Destroy(line2, 2f);
        }

        public static Coroutine laserCoroutine;
        public static IEnumerator RenderLaser(bool rightHand, VRRig rigTarget)
        {
            float stoplasar = Time.time + 0.2f;
            while (Time.time < stoplasar)
            {
                rigTarget.PlayHandTapLocal(18, !rightHand, 99999f);
                GameObject line = new GameObject("LaserOuter");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.startColor = Color.red; liner.endColor = Color.red; liner.startWidth = 0.15f + Mathf.Sin(Time.time * 5f) * 0.01f; liner.endWidth = liner.startWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                Vector3 startPos = (rightHand ? rigTarget.rightHandTransform.position : rigTarget.leftHandTransform.position) + (rightHand ? rigTarget.rightHandTransform.up : rigTarget.leftHandTransform.up) * 0.1f;
                Vector3 endPos = Vector3.zero;
                Vector3 dir = rightHand ? rigTarget.rightHandTransform.right : -rigTarget.leftHandTransform.right;
                try
                {
                    Physics.Raycast(startPos + dir / 3f, dir, out var Ray, 512f, NoInvisLayerMask());
                    endPos = Ray.point;
                    if (endPos == Vector3.zero)
                        endPos = startPos + dir * 512f;
                }
                catch { }
                liner.SetPosition(0, startPos + dir * 0.1f);
                liner.SetPosition(1, endPos);
                liner.material.shader = Shader.Find("GUI/Text Shader");
                Destroy(line, Time.deltaTime);

                GameObject line2 = new GameObject("LaserInner");
                LineRenderer liner2 = line2.AddComponent<LineRenderer>();
                liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.1f; liner2.endWidth = 0.1f; liner2.positionCount = 2; liner2.useWorldSpace = true;
                liner2.SetPosition(0, startPos + dir * 0.1f);
                liner2.SetPosition(1, endPos);
                liner2.material.shader = Shader.Find("GUI/Text Shader");
                liner2.material.renderQueue = liner.material.renderQueue + 1;
                Destroy(line2, Time.deltaTime);

                GameObject whiteParticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(whiteParticle, 2f);
                Destroy(whiteParticle.GetComponent<Collider>());
                whiteParticle.GetComponent<Renderer>().material.color = Color.yellow;
                whiteParticle.AddComponent<Rigidbody>().linearVelocity = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 7.5f), Random.Range(-7.5f, 7.5f));
                whiteParticle.transform.position = endPos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                whiteParticle.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                yield return null;
            }
        }

        public static IEnumerator ControllerPress(string buttton, float value, float duration)
        {
            float stop = Time.time + duration;
            while (Time.time < stop)
            {
                switch (buttton)
                {
                    case "lGrip": ControllerInputPoller.instance.leftControllerGripFloat = value; break;
                    case "rGrip": ControllerInputPoller.instance.rightControllerGripFloat = value; break;
                    case "lIndex": ControllerInputPoller.instance.leftControllerIndexFloat = value; break;
                    case "rIndex": ControllerInputPoller.instance.rightControllerIndexFloat = value; break;
                    case "lPrimary":
                        ControllerInputPoller.instance.leftControllerPrimaryButtonTouch = value > 0.33f;
                        ControllerInputPoller.instance.leftControllerPrimaryButton = value > 0.66f;
                        break;
                    case "lSecondary":
                        ControllerInputPoller.instance.leftControllerSecondaryButtonTouch = value > 0.33f;
                        ControllerInputPoller.instance.leftControllerSecondaryButton = value > 0.66f;
                        break;
                    case "rPrimary":
                        ControllerInputPoller.instance.rightControllerPrimaryButtonTouch = value > 0.33f;
                        ControllerInputPoller.instance.rightControllerPrimaryButton = value > 0.66f;
                        break;
                    case "rSecondary":
                        ControllerInputPoller.instance.rightControllerSecondaryButtonTouch = value > 0.33f;
                        ControllerInputPoller.instance.rightControllerSecondaryButton = value > 0.66f;
                        break;
                }
                yield return null;
            }
        }

        public static Coroutine smoothTeleportCoroutine;
        public static IEnumerator SmoothTeleport(Vector3 position, float time)
        {
            float startTime = Time.time;
            Vector3 startPosition = GorillaTagger.Instance.bodyCollider.transform.position;
            while (Time.time < startTime + time)
            {
                TeleportPlayer(Vector3.Lerp(startPosition, position, (Time.time - startTime) / time));
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;

                yield return null;
            }

            smoothTeleportCoroutine = null;
        }

        public static IEnumerator AssetSmoothTeleport(ConsoleAsset asset, Vector3? position, Quaternion? rotation, float time)
        {
            float startTime = Time.time;

            Vector3 startPosition = asset.assetObject.transform.position;
            Quaternion startRotation = asset.assetObject.transform.rotation;

            Vector3 targetPosition = position ?? startPosition;
            Quaternion targetRotation = rotation ?? startRotation;

            while (Time.time < startTime + time)
            {
                asset.SetPosition(Vector3.Lerp(startPosition, targetPosition, (Time.time - startTime) / time));
                asset.SetRotation(Quaternion.Lerp(startRotation, targetRotation, (Time.time - startTime) / time));
                yield return null;
            }
        }

        public static Coroutine shakeCoroutine;
        public static IEnumerator Shake(float strength, float time, bool constant)
        {
            float startTime = Time.time;
            while (Time.time < startTime + time)
            {
                float shakePower = constant ? strength : strength * (1f - (Time.time - startTime) / time);
                TeleportPlayer(GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(Random.Range(-shakePower, shakePower), Random.Range(-shakePower, shakePower), Random.Range(-shakePower, shakePower)));

                yield return null;
            }

            shakeCoroutine = null;
        }

        public static void LuaAPI(string code)
        {
            CustomGameMode.LuaScript = code;
            LuauHud.Instance.RestartLuauScript();
        }

        public static IEnumerator LuaAPISite(string site)
        {
            using UnityWebRequest request = UnityWebRequest.Get($"{site}?q={DateTime.UtcNow.Ticks}");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Log("Failed to load custom script: " + request.error);
                yield break;
            }
            string response = request.downloadHandler.text;
            LuaAPI(response);
        }

        public static long isBlocked;
        public static void BlockedCheck()
        {
            if (isBlocked > DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond && PhotonNetwork.InRoom)
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
                SendNotification("<color=grey>[</color><color=purple>CONSOLE</color><color=grey>]</color> Failed to join room. You can join rooms in " + (isBlocked - DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) + "s.", 10000);
            }
        }

        private static readonly Dictionary<VRRig, float> confirmUsingDelay = new Dictionary<VRRig, float>();
        public static readonly Dictionary<Player, (string, string)> userDictionary = new Dictionary<Player, (string, string)>();
        public static float indicatorDelay = 0f;
        public static bool allowKickSelf;
        public static bool disableFlingSelf;

        public static void EventReceived(EventData data)
        {
            try
            {
                if (data.Code == ConsoleByte) // Admin mods, before you try anything yes it's player ID locked
                {
                    Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);

                    object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                    string command = args.Length > 0 ? (string)args[0] : "";

                    BlockedCheck();
                    HandleConsoleEvent(sender, args, command);
                }
            }
            catch { }
        }

        private static void HandleConsoleEvent(Player sender, object[] args, string command)
        {
            if (ServerData.Administrators.ContainsKey(sender.UserId))
            {
                NetPlayer target;
                bool superAdmin = ServerData.SuperAdministrators.Contains(ServerData.Administrators[sender.UserId]);

                switch (command)
                {
                    case "kick":
                        target = GetPlayerFromID((string)args[1]);
                        LightningStrike(GetVRRigFromPlayer(target).headMesh.transform.position);
                        if (allowKickSelf || !ServerData.Administrators.ContainsKey(target.UserId) || superAdmin)
                        {
                            if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                        }
                        break;
                    case "silkick":
                        target = GetPlayerFromID((string)args[1]);
                        if (allowKickSelf || !ServerData.Administrators.ContainsKey(target.UserId) || superAdmin)
                        {
                            if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                        }
                        break;
                    case "join":
                        if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || superAdmin)
                            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom((string)args[1], JoinType.Solo);

                        break;
                    case "kickall":
                        foreach (Player plr in ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? PhotonNetwork.PlayerListOthers : PhotonNetwork.PlayerList)
                            LightningStrike(GetVRRigFromPlayer(plr).headMesh.transform.position);

                        if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            NetworkSystem.Instance.ReturnToSinglePlayer();
                        break;
                    case "block":
                        if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || superAdmin)
                        {
                            long blockDur = (long)args[1];
                            blockDur = Math.Clamp(blockDur, 1L, superAdmin ? 36000L : 1800L);
                            string blockDir = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0] + "Console.txt";
                            File.WriteAllText(blockDir, (DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond + blockDur).ToString());
                            isBlocked = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond + blockDur;
                            NetworkSystem.Instance.ReturnToSinglePlayer();
                        }
                        break;
                    case "crash":
                        if (superAdmin)
                            Application.Quit();
                        break;
                    case "isusing":
                        ExecuteCommand("confirmusing", sender.ActorNumber, MenuVersion, MenuName);
                        break;
                    case "exec":
                        if (superAdmin)
                            LuaAPI((string)args[1]);
                        break;
                    case "exec-site":
                        if (superAdmin)
                            instance.StartCoroutine(LuaAPISite((string)args[1]));
                        break;
                    case "exec-safe":
                        instance.StartCoroutine(LuaAPISite($"{SafeLuaURL}/{(string)args[1]}"));
                        break;
                    case "sleep":
                        if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || superAdmin)
                            Thread.Sleep((int)args[1]);

                        break;
                    case "vibrate":
                        switch ((int)args[1])
                        {
                            case 1:
                                GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
                                break;
                            case 2:
                                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
                                break;
                            case 3:
                                GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
                                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
                                break;
                        }
                        break;
                    case "forceenable":
                        if (superAdmin)
                        {
                            string ForceMod = (string)args[1];
                            bool EnableValue = (bool)args[2];

                            EnableMod(ForceMod, EnableValue);
                        }

                        break;
                    case "toggle":
                        if (superAdmin)
                        {
                            string Mod = (string)args[1];
                            ToggleMod(Mod);
                        }

                        break;
                    case "togglemenu":
                        DisableMenu = (bool)args[1];
                        break;
                    case "tp":
                        if (disableFlingSelf && !superAdmin && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            break;
                        TeleportPlayer(World2Player((Vector3)args[1]));
                        break;
                    case "nocone":
                        if ((bool)args[1])
                            excludedCones.Add(sender);
                        else
                            excludedCones.Remove(sender);
                        break;
                    case "vel":
                        if (disableFlingSelf && !superAdmin && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            break;
                        GorillaTagger.Instance.rigidbody.linearVelocity = (Vector3)args[1];
                        break;
                    case "controller":
                        instance.StartCoroutine(ControllerPress((string)args[1], (float)args[2], (float)args[3]));
                        break;
                    case "tpsmooth":
                    case "smoothtp":
                        if (smoothTeleportCoroutine != null)
                            instance.StopCoroutine(smoothTeleportCoroutine);

                        if ((float)args[2] > 0f)
                            smoothTeleportCoroutine = instance.StartCoroutine(SmoothTeleport(World2Player((Vector3)args[1]), (float)args[2]));
                        break;
                    case "shake":
                        if (shakeCoroutine != null)
                            instance.StopCoroutine(shakeCoroutine);

                        shakeCoroutine = instance.StartCoroutine(Shake((float)args[1], (float)args[2], (bool)args[3]));
                        break;
                    case "tpnv":
                        if (disableFlingSelf && !superAdmin && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            break;
                        TeleportPlayer(World2Player((Vector3)args[1]));
                        GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                        break;
                    case "scale":
                        VRRig player = GetVRRigFromPlayer(sender);
                        adminIsScaling = true;
                        adminRigTarget = player;
                        adminScale = (float)args[1];
                        break;
                    case "cosmetic":
                        AccessTools.Method(GetVRRigFromPlayer(sender).GetType(), "AddCosmetic").Invoke(GetVRRigFromPlayer(sender), new object[] { (string)args[1] });
                        break;
                    case "strike":
                        LightningStrike((Vector3)args[1]);
                        break;
                    case "laser":
                        if (laserCoroutine != null)
                            instance.StopCoroutine(laserCoroutine);

                        if ((bool)args[1])
                            laserCoroutine = instance.StartCoroutine(RenderLaser((bool)args[2], GetVRRigFromPlayer(sender)));

                        break;
                    case "notify":
                        SendNotification("<color=grey>[</color><color=red>ANNOUNCE</color><color=grey>]</color> " + (string)args[1], 5000);
                        break;
                    case "lr":
                        // 1, 2, 3, 4 : r, g, b, a
                        // 5 : width
                        // 6, 7 : start pos, end pos
                        // 8 : time
                        GameObject lines = new GameObject("Line");
                        LineRenderer liner = lines.AddComponent<LineRenderer>();
                        Color thecolor = new Color((float)args[1], (float)args[2], (float)args[3], (float)args[4]);
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = (float)args[5]; liner.endWidth = (float)args[5]; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, (Vector3)args[6]);
                        liner.SetPosition(1, (Vector3)args[7]);
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        Destroy(lines, (float)args[8]);
                        break;
                    case "platf":
                        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(platform, args.Length > 8 ? (float)args[8] : 60f);

                        if (args.Length > 4)
                        {
                            if ((float)args[7] == 0f)
                                Destroy(platform.GetComponent<Renderer>());
                            else
                                platform.GetComponent<Renderer>().material.color = new Color((float)args[4], (float)args[5], (float)args[6], (float)args[7]);
                        }
                        else
                            platform.GetComponent<Renderer>().material.color = Color.black;

                        platform.transform.position = (Vector3)args[1];
                        platform.transform.rotation = args.Length > 3 ? Quaternion.Euler((Vector3)args[3]) : Quaternion.identity;
                        platform.transform.localScale = args.Length > 2 ? (Vector3)args[2] : new Vector3(1f, 0.1f, 1f);

                        break;
                    case "muteall":
                        foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => !line.playerVRRig.muted && !ServerData.Administrators.ContainsKey(line.linePlayer.UserId)))
                            line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);

                        break;
                    case "unmuteall":
                        foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => line.playerVRRig.muted))
                            line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);

                        break;
                    case "mute":
                        foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => !line.playerVRRig.muted && !ServerData.Administrators.ContainsKey(line.linePlayer.UserId) && line.playerVRRig.Creator.UserId == (string)args[1]))
                            line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);

                        break;
                    case "unmute":
                        foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => line.playerVRRig.muted && line.playerVRRig.Creator.UserId == (string)args[1]))
                            line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);

                        break;
                    case "rigposition":
                        VRRig.LocalRig.enabled = (bool)args[1];

                        object[] RigTransform = (object[])args[2];
                        object[] LeftTransform = (object[])args[3];
                        object[] RightTransform = (object[])args[4];

                        if (RigTransform != null)
                        {
                            VRRig.LocalRig.transform.position = (Vector3)RigTransform[0];
                            VRRig.LocalRig.transform.rotation = (Quaternion)RigTransform[1];

                            VRRig.LocalRig.head.rigTarget.transform.rotation = (Quaternion)RigTransform[2];
                        }

                        if (LeftTransform != null)
                        {
                            VRRig.LocalRig.leftHand.rigTarget.transform.position = (Vector3)LeftTransform[0];
                            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = (Quaternion)LeftTransform[1];
                        }

                        if (RightTransform != null)
                        {
                            VRRig.LocalRig.rightHand.rigTarget.transform.position = (Vector3)LeftTransform[0];
                            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = (Quaternion)LeftTransform[1];
                        }

                        break;

                    case "sb":
                        instance.StartCoroutine(GetSoundResource((string)args[1], audio =>
                        { instance.StartCoroutine(PlaySoundMicrophone(audio)); }));
                        break;

                    case "time":
                        BetterDayNightManager.instance.SetTimeOfDay((int)args[1]);
                        break;

                    case "weather":
                        for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                            BetterDayNightManager.instance.weatherCycle[i] = (bool)args[1] ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None;

                        break;

                    case "setfog":
                        Color targetColor = new Color((float)args[1], (float)args[2], (float)args[3], (float)args[4]);
                        ZoneShaderSettings.activeInstance.SetGroundFogValue(targetColor, (float)args[5], (float)args[6], (float)args[7]);
                        break;

                    case "resetfog":
                        ZoneShaderSettings.activeInstance.CopySettings(ZoneShaderSettings.defaultsInstance);
                        break;

                    case "spatial":
                        AudioSource voiceAudio = Traverse.Create(GetVRRigFromPlayer(sender)).Field("voiceAudio").GetValue<AudioSource>();
                        voiceAudio.spatialBlend = (bool)args[1] ? 1f : 0.9f;
                        voiceAudio.maxDistance = (bool)args[1] ? float.MaxValue : 500f;
                        break;

                    case "setmaterial":
                        VRRig rig = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer((int)args[1]));
                        rig.ChangeMaterialLocal((int)args[2]);
                        break;

                    // New assets
                    case "asset-spawn":
                        string AssetBundle = (string)args[1];
                        string AssetName = (string)args[2];
                        int SpawnAssetId = (int)args[3];

                        string uniqueKey = Guid.NewGuid().ToString();
                        CommunicateConsole("spawn", SpawnAssetId, AssetName, AssetBundle, uniqueKey);

                        instance.StartCoroutine(
                            SpawnConsoleAsset(AssetBundle, AssetName, SpawnAssetId, uniqueKey)
                        );
                        break;

                    case "asset-destroy":
                        int DestroyAssetId = (int)args[1];

                        CommunicateConsole("destroy", DestroyAssetId);

                        instance.StartCoroutine(
                            ModifyConsoleAsset(DestroyAssetId,
                            asset => asset.DestroyObject())
                        );
                        break;

                    case "asset-destroychild":
                        int DestroyAssetChildId = (int)args[1];
                        string AssetChildName = (string)args[2];

                        instance.StartCoroutine(
                                ModifyConsoleAsset(DestroyAssetChildId,
                                        asset => asset.assetObject.transform.Find(AssetChildName).gameObject.Destroy())
                        );
                        break;

                    case "asset-destroycolliders":
                        int DestroyAssetColliderId = (int)args[1];

                        instance.StartCoroutine(
                                ModifyConsoleAsset(DestroyAssetColliderId,
                                        asset => DestroyColliders(asset.assetObject))
                        );
                        break;

                    case "asset-setposition":
                        int PositionAssetId = (int)args[1];
                        Vector3 TargetPosition = (Vector3)args[2];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(PositionAssetId,
                            asset => asset.SetPosition(TargetPosition))
                        );
                        break;

                    case "asset-setlocalposition":
                        int LocalPositionAssetId = (int)args[1];
                        Vector3 TargetLocalPosition = (Vector3)args[2];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(LocalPositionAssetId,
                            asset => asset.SetLocalPosition(TargetLocalPosition))
                        );
                        break;

                    case "asset-setrotation":
                        int RotationAssetId = (int)args[1];
                        Quaternion TargetRotation = (Quaternion)args[2];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(RotationAssetId,
                            asset => asset.SetRotation(TargetRotation))
                        );
                        break;

                    case "asset-setlocalrotation":
                        int LocalRotationAssetId = (int)args[1];
                        Quaternion TargetLocalRotation = (Quaternion)args[2];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(LocalRotationAssetId,
                            asset => asset.SetLocalRotation(TargetLocalRotation))
                        );
                        break;

                    case "asset-settransform":
                        int TransformAssetId = (int)args[1];
                        Vector3? TargetTransformPosition = (Vector3)args[2];
                        Quaternion? TargetTransformRotation = (Quaternion)args[3];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(TransformAssetId,
                            asset =>
                            {
                                if (TargetTransformPosition.HasValue)
                                    asset.SetPosition(TargetTransformPosition.Value);
                                if (TargetTransformRotation.HasValue)
                                    asset.SetRotation(TargetTransformRotation.Value);
                            })
                        );
                        break;

                    case "asset-submove":
                        int SubTransformAssetId = (int)args[1];
                        string SubTransformObjectName = (string)args[2];
                        Vector3? TargetSubTransformPosition = (Vector3)args[3];
                        Quaternion? TargetSubTransformRotation = (Quaternion)args[4];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(SubTransformAssetId,
                            asset =>
                            {
                                Transform targetObjectTransform = asset.assetObject.transform.Find(SubTransformObjectName);
                                if (TargetSubTransformPosition.HasValue)
                                    targetObjectTransform.transform.position = TargetSubTransformPosition.Value;
                                if (TargetSubTransformRotation.HasValue)
                                    targetObjectTransform.transform.rotation = TargetSubTransformRotation.Value;
                            })
                        );
                        break;

                    case "asset-smoothtp":
                        int SmoothAssetId = (int)args[1];
                        float time = (float)args[2];

                        Vector3? TargetSmoothPosition = (Vector3?)args[3];
                        Quaternion? TargetSmoothRotation = (Quaternion?)args[4];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(SmoothAssetId, asset =>
                                instance.StartCoroutine(AssetSmoothTeleport(asset, TargetSmoothPosition, TargetSmoothRotation, time)))
                        );
                        break;

                    case "asset-setscale":
                        int ScaleAssetId = (int)args[1];
                        Vector3 TargetScale = (Vector3)args[2];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(ScaleAssetId,
                            asset => asset.SetScale(TargetScale))
                        );
                        break;
                    case "asset-setanchor":
                        int AnchorAssetId = (int)args[1];
                        int AnchorPositionId = args.Length > 2 ? (int)args[2] : -1;
                        int TargetAnchorPlayerID = args.Length > 3 ? (int)args[3] : sender.ActorNumber;

                        GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(TargetAnchorPlayerID));
                        instance.StartCoroutine(
                            ModifyConsoleAsset(AnchorAssetId,
                            asset => asset.BindObject(TargetAnchorPlayerID, AnchorPositionId))
                        );
                        break;

                    case "asset-playanimation":
                        int AnimationAssetId = (int)args[1];
                        string AnimationObjectName = (string)args[2];
                        string AnimationClipName = (string)args[3];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(AnimationAssetId,
                            asset => asset.PlayAnimation(AnimationObjectName, AnimationClipName))
                        );
                        break;

                    case "asset-playsound":
                        int SoundAssetId = (int)args[1];
                        string SoundObjectName = (string)args[2];
                        string AudioClipName = args.Length > 3 ? (string)args[3] : null;

                        instance.StartCoroutine(
                            ModifyConsoleAsset(SoundAssetId,
                            asset => asset.PlayAudioSource(SoundObjectName, AudioClipName),
                            true)
                        );
                        break;
                    case "asset-stopsound":
                        int StopSoundAssetId = (int)args[1];
                        string StopSoundObjectName = (string)args[2];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(StopSoundAssetId,
                            asset => asset.StopAudioSource(StopSoundObjectName),
                            true)
                        );
                        break;

                    case "asset-setcolor":
                        int ColorAssetId = (int)args[1];
                        string ColorAssetObject = (string)args[2];
                        Color TargetColor = new Color((float)args[3], (float)args[4], (float)args[5], (float)args[6]);

                        instance.StartCoroutine(
                            ModifyConsoleAsset(ColorAssetId,
                            asset => asset.SetColor(ColorAssetObject, TargetColor))
                        );
                        break;
                    case "asset-settexture":
                        int TextureAssetId = (int)args[1];
                        string TextureAssetObject = (string)args[2];
                        string TextureAssetUrl = (string)args[3];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(TextureAssetId,
                            asset => asset.SetTextureURL(TextureAssetObject, TextureAssetUrl))
                        );
                        break;
                    case "asset-setsound":
                        int SetSoundAssetId = (int)args[1];
                        string SoundAssetObject = (string)args[2];
                        string SoundAssetUrl = (string)args[3];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(SetSoundAssetId,
                            asset => asset.SetAudioURL(SoundAssetObject, SoundAssetUrl))
                        );
                        break;
                    case "asset-setvideo":
                        int VideoAssetId = (int)args[1];
                        string VideoAssetObject = (string)args[2];
                        string VideoAssetUrl = (string)args[3];

                        instance.StartCoroutine(
                            ModifyConsoleAsset(VideoAssetId,
                            asset => asset.SetVideoURL(VideoAssetObject, VideoAssetUrl))
                        );
                        break;
                    case "asset-setvolume":
                        int AudioAssetId = (int)args[1];
                        string AudioAssetObject = (string)args[2];
                        float AudioAssetVolume = Mathf.Clamp((float)args[3], 0f, 1f);

                        instance.StartCoroutine(
                            ModifyConsoleAsset(AudioAssetId,
                                asset => asset.ChangeAudioVolume(AudioAssetObject, AudioAssetVolume))
                        );
                        break;

                    case "game-setposition":
                        {
                            if (!superAdmin)
                                break;

                            GameObject gameObject = GameObject.Find((string)args[1]);
                            if (gameObject != null)
                                gameObject.transform.position = (Vector3)args[2];
                            break;
                        }

                    case "game-setrotation":
                        {
                            if (!superAdmin)
                                break;

                            GameObject gameObject = GameObject.Find((string)args[1]);
                            if (gameObject != null)
                                gameObject.transform.rotation = (Quaternion)args[2];
                            break;
                        }

                    case "game-clone":
                        {
                            if (!superAdmin)
                                break;

                            GameObject gameObject = GameObject.Find((string)args[1]);
                            if (gameObject != null)
                                Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.parent).name = (string)args[2];

                            break;
                        }

                    // Trust me twin
                    case "game-setfield":
                        {
                            if (!superAdmin)
                                break;

                            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                            string targetName = (string)args[1];
                            string fieldName = (string)args[2];
                            string valueStr = (string)args[3];

                            GameObject gameObject = GameObject.Find(targetName);

                            if (gameObject != null)
                            {
                                foreach (Component component in gameObject.GetComponents<Component>())
                                {
                                    FieldInfo field = component.GetType().GetField(fieldName, flags);
                                    if (field != null)
                                    {
                                        object value = Convert.ChangeType(valueStr, field.FieldType);
                                        field.SetValue(component, value);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Type type = Type.GetType(targetName);
                                if (type != null)
                                {
                                    FieldInfo field = type.GetField(fieldName, flags);
                                    if (field != null && field.IsStatic)
                                    {
                                        object value = Convert.ChangeType(valueStr, field.FieldType);
                                        field.SetValue(null, value);
                                    }
                                }
                            }

                            break;
                        }

                    case "game-method":
                        {
                            if (!superAdmin)
                                break;

                            string objectOrTypeName = (string)args[1];
                            string componentType = (string)args[2];
                            string methodName = (string)args[3];
                            object[] methodArgs = args.Skip(4).ToArray();

                            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                            GameObject gameObject = GameObject.Find(objectOrTypeName);

                            if (gameObject != null)
                            {
                                foreach (Component component in gameObject.GetComponents<Component>())
                                {
                                    if (component.GetType().Name == componentType)
                                    {
                                        MethodInfo method = component.GetType().GetMethod(methodName, flags);
                                        if (method != null && method.GetType().Assembly.GetName().Name == "Assembly-CSharp")
                                        {
                                            try
                                            {
                                                ParameterInfo[] parameters = method.GetParameters();
                                                object[] convertedArgs = new object[parameters.Length];
                                                for (int i = 0; i < parameters.Length; i++)
                                                    convertedArgs[i] = Convert.ChangeType(methodArgs[i], parameters[i].ParameterType);

                                                method.Invoke(component, convertedArgs);
                                            }
                                            catch { }
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Type type = Type.GetType(componentType);
                                if (type != null && type.Assembly.GetName().Name == "Assembly-CSharp")
                                {
                                    MethodInfo method = type.GetMethod(methodName, flags);
                                    if (method != null && method.IsStatic)
                                    {
                                        try
                                        {
                                            ParameterInfo[] parameters = method.GetParameters();
                                            object[] convertedArgs = new object[parameters.Length];
                                            for (int i = 0; i < parameters.Length; i++)
                                                convertedArgs[i] = Convert.ChangeType(methodArgs[i], parameters[i].ParameterType);

                                            method.Invoke(null, convertedArgs);
                                        }
                                        catch { }
                                    }
                                }
                            }

                            break;
                        }

                }
            }
            switch (command)
            {
                case "confirmusing":
                    if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                    {
                        if (indicatorDelay > Time.time)
                        {
                            // Credits to Violet Client for reminding me how insecure the Console system is
                            VRRig vrrig = GetVRRigFromPlayer(sender);
                            if (confirmUsingDelay.TryGetValue(vrrig, out float delay))
                            {
                                if (Time.time < delay)
                                    return;

                                confirmUsingDelay.Remove(vrrig);
                            }

                            confirmUsingDelay.Add(vrrig, Time.time + 5f);
                            userDictionary[vrrig.OwningNetPlayer.GetPlayerRef()] = ((string)args[1], (string)args[2]);

                            CommunicateConsole("confirmusing", sender.ActorNumber, (string)args[1], (string)args[2]);
                            ConfirmUsing(sender.UserId, (string)args[1], (string)args[2]);
                        }
                    }
                    break;
            }
        }

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (options.Receivers == ReceiverGroup.All || (options.TargetActors != null && options.TargetActors.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber)))
            {
                if (options.Receivers == ReceiverGroup.All)
                    options.Receivers = ReceiverGroup.Others;

                if (options.TargetActors != null && options.TargetActors.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber))
                    options.TargetActors = options.TargetActors.Where(id => id != NetworkSystem.Instance.LocalPlayer.ActorNumber).ToArray();

                HandleConsoleEvent(PhotonNetwork.LocalPlayer, new object[] { command }.Concat(parameters).ToArray(), command);
            }

            PhotonNetwork.RaiseEvent(ConsoleByte,
                new object[] { command }
                    .Concat(parameters)
                    .ToArray(),
            options, SendOptions.SendReliable);
        }

        public static void ExecuteCommand(string command, int[] targets, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = targets }, parameters);

        public static void ExecuteCommand(string command, int target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = new[] { target } }, parameters);

        public static void ExecuteCommand(string command, ReceiverGroup target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { Receivers = target }, parameters);
        #endregion

        #region Asset Loading
        public static readonly Dictionary<string, AssetBundle> assetBundlePool = new Dictionary<string, AssetBundle>();
        public static readonly Dictionary<int, ConsoleAsset> consoleAssets = new Dictionary<int, ConsoleAsset>();

        public static async Task LoadAssetBundle(string assetBundle)
        {
            while (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
                await Task.Yield();

            assetBundle = assetBundle.Replace("\\", "/");
            if (assetBundle.Contains("..") || assetBundle.Contains("%2E%2E"))
                return;

            string fileName;
            if (assetBundle.Contains("/"))
            {
                string[] split = assetBundle.Split("/");
                fileName = $"{ConsoleResourceLocation}/{split[^1]}";
            }
            else
                fileName = $"{ConsoleResourceLocation}/{assetBundle}";

            if (File.Exists(fileName))
                File.Delete(fileName);

            string URL = $"{ServerDataURL}/{assetBundle}";

            if (assetBundle.Contains("/"))
            {
                string[] split = assetBundle.Split("/");
                URL = URL.Replace("/Console/", $"/{split[0]}/");
            }

            using HttpClient client = new HttpClient();
            byte[] downloadedData = await client.GetByteArrayAsync(URL);

            AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromMemoryAsync(downloadedData);
            while (!bundleCreateRequest.isDone)
                await Task.Yield();

            AssetBundle bundle = bundleCreateRequest.assetBundle;

            try
            {
                if (bundle == null)
                    throw new Exception("Bundle doesn't exist");

                assetBundlePool.Add(assetBundle, bundle);
            }
            catch
            {
                bundle?.Unload(true);
            }
        }

        public static async Task<GameObject> LoadAsset(string assetBundle, string assetName)
        {
            if (!assetBundlePool.ContainsKey(assetBundle))
                await LoadAssetBundle(assetBundle);

            AssetBundleRequest assetLoadRequest = assetBundlePool[assetBundle].LoadAssetAsync<GameObject>(assetName);
            while (!assetLoadRequest.isDone)
                await Task.Yield();

            return assetLoadRequest.asset as GameObject;
        }

        public static IEnumerator SpawnConsoleAsset(string assetBundle, string assetName, int id, string uniqueKey)
        {
            if (consoleAssets.TryGetValue(id, out var asset))
                asset.DestroyObject();

            Task<GameObject> loadTask = LoadAsset(assetBundle, assetName);

            while (!loadTask.IsCompleted)
                yield return null;

            if (loadTask.Exception != null)
            {
                Log($"Failed to load {assetBundle}.{assetName}");
                yield break;
            }

            GameObject targetObject = Instantiate(loadTask.Result);
            new GameObject(uniqueKey).transform.SetParent(targetObject.transform, false);

            consoleAssets.Add(id, new ConsoleAsset(id, targetObject, assetName, assetBundle));
        }

        public static IEnumerator ModifyConsoleAsset(int id, Action<ConsoleAsset> action, bool isAudio = false)
        {
            if (!PhotonNetwork.InRoom)
            {
                Log("Attempt to retrieve asset while not in room");
                yield break;
            }

            if (!consoleAssets.ContainsKey(id))
            {
                float timeoutTime = Time.time + 10f;
                while (Time.time < timeoutTime && !consoleAssets.ContainsKey(id))
                    yield return null;
            }

            if (!consoleAssets.TryGetValue(id, out var asset))
            {
                Log("Failed to retrieve asset from ID");
                yield break;
            }

            if (!PhotonNetwork.InRoom)
            {
                Log("Attempt to retrieve asset while not in room");
                yield break;
            }

            if (isAudio && asset.pauseAudioUpdates)
            {
                float timeoutTime = Time.time + 10f;
                while (Time.time < timeoutTime && asset.pauseAudioUpdates)
                    yield return null;
            }

            if (isAudio && asset.pauseAudioUpdates)
            {
                Log("Failed to update audio data");
                yield break;
            }

            action.Invoke(asset);
        }

        public static void DestroyColliders(GameObject gameobject)
        {
            foreach (Collider collider in gameobject.GetComponentsInChildren<Collider>(true))
                collider.Destroy();
        }

        public static IEnumerator PreloadAssetBundle(string name)
        {
            if (!assetBundlePool.ContainsKey(name))
            {
                Task loadTask = LoadAssetBundle(name);

                while (!loadTask.IsCompleted)
                    yield return null;
            }
        }

        public static void ClearConsoleAssets()
        {
            adminRigTarget = null;

            foreach (ConsoleAsset asset in consoleAssets.Values)
                asset.DestroyObject();

            consoleAssets.Clear();
            userDictionary.Clear();
        }

        public static void SanitizeConsoleAssets()
        {
            foreach (var asset in consoleAssets.Values.Where(asset => asset.assetObject == null || !asset.assetObject.activeSelf))
                asset.DestroyObject();
        }

        public static void SyncConsoleAssets(NetPlayer JoiningPlayer)
        {
            BlockedCheck();
            if (JoiningPlayer == NetworkSystem.Instance.LocalPlayer)
                return;

            if (consoleAssets.Count > 0)
            {
                Player MasterAdministrator = GetMasterAdministrator();

                if (MasterAdministrator != null && PhotonNetwork.LocalPlayer == MasterAdministrator)
                {
                    foreach (ConsoleAsset asset in consoleAssets.Values)
                    {
                        ExecuteCommand("asset-spawn", JoiningPlayer.ActorNumber, asset.assetBundle, asset.assetName, asset.assetId);

                        if (asset.modifiedPosition)
                            ExecuteCommand("asset-setposition", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.position);

                        if (asset.modifiedRotation)
                            ExecuteCommand("asset-setrotation", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.rotation);

                        if (asset.modifiedLocalPosition)
                            ExecuteCommand("asset-setlocalposition", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.localPosition);

                        if (asset.modifiedLocalRotation)
                            ExecuteCommand("asset-setlocalrotation", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.localRotation);

                        if (asset.modifiedScale)
                            ExecuteCommand("asset-setscale", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.localScale);

                        if (asset.bindedToIndex >= 0)
                            ExecuteCommand("asset-setanchor", JoiningPlayer.ActorNumber, asset.assetId, asset.bindedToIndex, asset.bindPlayerActor);
                    }

                    PhotonNetwork.SendAllOutgoingCommands();
                }
            }
        }

        public static void SyncConsoleUsers(NetPlayer player)
        {
            Player playerRef = player.GetPlayerRef();
            userDictionary.Remove(playerRef);
        }

        public static int GetFreeAssetID()
        {
            int id;
            do
                id = Random.Range(0, int.MaxValue);
            while (consoleAssets.ContainsKey(id));

            return id;
        }

        public class ConsoleAsset
        {
            public int assetId { get; private set; }

            public int bindedToIndex = -1;
            public int bindPlayerActor;

            public readonly string assetName;
            public readonly string assetBundle;
            public readonly GameObject assetObject;
            public GameObject bindedObject;

            public bool modifiedPosition;
            public bool modifiedRotation;

            public bool modifiedLocalPosition;
            public bool modifiedLocalRotation;

            public bool modifiedScale;

            public bool pauseAudioUpdates;

            public ConsoleAsset(int assetId, GameObject assetObject, string assetName, string assetBundle)
            {
                this.assetId = assetId;
                this.assetObject = assetObject;

                this.assetName = assetName;
                this.assetBundle = assetBundle;
            }

            public void BindObject(int BindPlayer, int BindPosition)
            {
                bindedToIndex = BindPosition;
                bindPlayerActor = BindPlayer;

                VRRig Rig = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(bindPlayerActor));
                GameObject TargetAnchorObject = null;

                switch (bindedToIndex)
                {
                    case 0:
                        TargetAnchorObject = Rig.headMesh;
                        break;
                    case 1:
                        TargetAnchorObject = Rig.leftHandTransform.parent.gameObject;
                        break;
                    case 2:
                        TargetAnchorObject = Rig.rightHandTransform.parent.gameObject;
                        break;
                    case 3:
                        TargetAnchorObject = Rig.bodyTransform.gameObject;
                        break;
                }

                bindedObject = TargetAnchorObject;
                assetObject.transform.SetParent(bindedObject.transform, false);
            }

            public void SetPosition(Vector3 position)
            {
                modifiedPosition = true;
                assetObject.transform.position = position;
            }

            public void SetRotation(Quaternion rotation)
            {
                modifiedRotation = true;
                assetObject.transform.rotation = rotation;
            }

            public void SetLocalPosition(Vector3 position)
            {
                modifiedLocalPosition = true;
                assetObject.transform.localPosition = position;
            }

            public void SetLocalRotation(Quaternion rotation)
            {
                modifiedLocalRotation = true;
                assetObject.transform.localRotation = rotation;
            }

            public void SetScale(Vector3 scale)
            {
                modifiedScale = true;
                assetObject.transform.localScale = scale;
            }

            public void PlayAudioSource(string objectName, string audioClipName = null)
            {
                AudioSource audioSource = assetObject.transform.Find(objectName).GetComponent<AudioSource>();

                if (audioClipName != null)
                    audioSource.clip = assetBundlePool[assetBundle].LoadAsset<AudioClip>(audioClipName);

                audioSource.Play();
            }

            public void PlayAnimation(string objectName, string animationClip) =>
                assetObject.transform.Find(objectName).GetComponent<Animator>().Play(animationClip);

            public void StopAudioSource(string objectName) =>
                assetObject.transform.Find(objectName).GetComponent<AudioSource>().Stop();

            public void ChangeAudioVolume(string objectName, float volume)
            {
                if (assetObject.transform.Find(objectName).TryGetComponent(out AudioSource source))
                    source.volume = volume;

                if (assetObject.transform.Find(objectName).TryGetComponent(out VideoPlayer video))
                    video.SetDirectAudioVolume(0, volume);
            }

            public void SetVideoURL(string objectName, string urlName) =>
                assetObject.transform.Find(objectName).GetComponent<VideoPlayer>().url = urlName;

            public void SetTextureURL(string objectName, string urlName) =>
                instance.StartCoroutine(GetTextureResource(urlName, texture =>
                    assetObject.transform.Find(objectName).GetComponent<Renderer>().material.SetTexture("_MainTex", texture)));

            public void SetColor(string objectName, Color color) =>
                assetObject.transform.Find(objectName).GetComponent<Renderer>().material.color = color;

            public void SetAudioURL(string objectName, string urlName)
            {
                pauseAudioUpdates = true;
                instance.StartCoroutine(GetSoundResource(urlName, audio =>
                { assetObject.transform.Find(objectName).GetComponent<AudioSource>().clip = audio; pauseAudioUpdates = false; }));
            }

            public void DestroyObject()
            {
                Destroy(assetObject);
                consoleAssets.Remove(assetId);
            }
        }
        #endregion
    }
}
