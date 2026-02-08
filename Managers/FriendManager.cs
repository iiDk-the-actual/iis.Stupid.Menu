/*
 * ii's Stupid Menu  Managers/FriendManager.cs
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
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using iiMenu.Classes.Menu;
using iiMenu.Menu;
using iiMenu.Mods;
using iiMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.RigUtilities;
using JoinType = GorillaNetworking.JoinType;

namespace iiMenu.Managers
{
    public class FriendManager : MonoBehaviour
    {
        #region Friend Manager Code

        private struct GameObjectData
        {
            public GameObject AssociatedGameObject;
            public Vector3 TargetPosition;
            public Vector3 OldTargetPosition;
            public Quaternion TargetRotation;
            public Quaternion OldTargetRotation;

            public readonly void InterpolateBetween(float t)
            {
                AssociatedGameObject.transform.position = Vector3.Lerp(OldTargetPosition, TargetPosition, t);
                AssociatedGameObject.transform.rotation = Quaternion.Lerp(OldTargetRotation, TargetRotation, t);
            }
        }

        public static FriendManager instance;

        private readonly Dictionary<VRRig, (float, GameObjectData[], GameObject)> rigDatas = new Dictionary<VRRig, (float, GameObjectData[], GameObject)>();
        private readonly Dictionary<VRRig, float> rigUpdateDelays = new Dictionary<VRRig, float>();

        private float UpdateTime;
        private string FriendResponse;
        public const byte FriendByte = 53;
        private const float RigDespawnTime = 0.5f;

        public FriendData Friends = new FriendData { friends = new Dictionary<string, FriendData.Friend>(), incoming = new Dictionary<string, FriendData.PendingFriend>(), outgoing = new Dictionary<string, FriendData.PendingFriend>() };

        public void Awake()
        {
            instance = this;
            UpdateTime = Time.time + 5f;

            gameObject.AddComponent<FriendWebSocket>();

            NetworkSystem.Instance.OnJoinedRoomEvent += CheckAllPlayersFriends;
            NetworkSystem.Instance.OnPlayerJoined += CheckPlayerFriends;

            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
        }

        private static Material starMaterial;
        private static Texture2D starTexture;

        private static float updateRigDelay;
        private static bool pingingState;

        private static GameObject pingObject;
        private static readonly Dictionary<VRRig, GameObject> starPool = new Dictionary<VRRig, GameObject>();

        public static bool RigNetworking = true;
        public static bool PlatformNetworking = true;
        public static bool Pinging = true;

        public static bool InviteNotifications = true;
        public static bool PreferenceSharing = true;
        public static bool ThemeSharing = true;
        public static bool MacroSharing = true;

        public static bool SoundEffects = true;
        public static bool Messaging = true;

        public static bool PhysicalPlatforms;

        public void Update()
        {
            if (Time.time > UpdateTime)
            {
                UpdateTime = Time.time + 30f;
                instance.StartCoroutine(UpdateFriendsList());
            }

            List<VRRig> toRemoveRigs = new List<VRRig>();

            foreach (var star in starPool.Where(star => !GorillaParent.instance.vrrigs.Contains(star.Key) || !IsPlayerFriend(GetPlayerFromVRRig(star.Key))))
            {
                toRemoveRigs.Add(star.Key);
                Destroy(star.Value);
            }

            foreach (VRRig rig in toRemoveRigs)
                starPool.Remove(rig);

            Dictionary<VRRig, (float, GameObjectData[], GameObject)> toRemoveGhostRigs = new Dictionary<VRRig, (float, GameObjectData[], GameObject)>();

            foreach ((VRRig rig, (float lastRigUpdate, GameObjectData[] gameObjectDatas, GameObject nametag)) in rigDatas)
            {
                float timeSinceLastRigUpdate = Time.time - lastRigUpdate;

                if (timeSinceLastRigUpdate > RigDespawnTime)
                {
                    toRemoveGhostRigs.Add(rig, rigDatas[rig]);

                    continue;
                }

                float delay = rigUpdateDelays.GetValueOrDefault(rig, 0.1f);
                float t = timeSinceLastRigUpdate / delay;
                foreach (GameObjectData gameObjectData in gameObjectDatas)
                    gameObjectData.InterpolateBetween(t);

                nametag.transform.LookAt(Camera.main.transform.position);
                nametag.transform.Rotate(0f, 180f, 0f);
            }

            foreach (KeyValuePair<VRRig, (float, GameObjectData[], GameObject)> pair in toRemoveGhostRigs)
            {
                rigDatas.Remove(pair.Key);
                foreach (GameObjectData gameObjectData in pair.Value.Item2)
                    Destroy(gameObjectData.AssociatedGameObject);

                Destroy(pair.Value.Item3);
            }

            if (NetworkSystem.Instance.InRoom)
            {
                NetPlayer[] allFriends = GetAllFriendsInRoom();
                foreach (NetPlayer player in allFriends)
                {
                    VRRig playerRig = GetVRRigFromPlayer(player);
                    if (playerRig == null) continue;
                    if (!starPool.TryGetValue(playerRig, out GameObject playerStar))
                    {
                        playerStar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(playerStar.GetComponent<Collider>());

                        if (starMaterial == null)
                        {
                            if (starTexture == null)
                                starTexture = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.star.png");

                            starMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                            {
                                mainTexture = starTexture
                            };

                            starMaterial.SetFloat("_Surface", 1);
                            starMaterial.SetFloat("_Blend", 0);
                            starMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                            starMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                            starMaterial.SetFloat("_ZWrite", 0);
                            starMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                            starMaterial.renderQueue = (int)RenderQueue.Transparent;
                        }

                        playerStar.GetComponent<Renderer>().material = starMaterial;
                        starPool.Add(playerRig, playerStar);
                    }

                    playerStar.GetComponent<Renderer>().material.color = playerRig.playerColor;

                    playerStar.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * playerRig.scaleFactor;
                    playerStar.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * (Classes.Menu.Console.GetIndicatorDistance(playerRig) * playerRig.scaleFactor);
                    playerStar.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }

                int[] NetworkedActors = GetAllNetworkActorNumbers();
                if (NetworkedActors.Length > 0 && RigNetworking && !VRRig.LocalRig.enabled && Time.time > updateRigDelay)
                {
                    updateRigDelay = Time.time + 0.1f;

                    ExecuteCommand("rig", NetworkedActors,
                        new object[] {
                            GorillaTagger.Instance.headCollider.transform.position,
                            GorillaTagger.Instance.headCollider.transform.rotation
                        },
                        new object[] {
                            GorillaTagger.Instance.leftHandTransform.transform.position,
                            GorillaTagger.Instance.leftHandTransform.transform.rotation
                        },
                        new object[] {
                            GorillaTagger.Instance.rightHandTransform.transform.position,
                            GorillaTagger.Instance.rightHandTransform.transform.rotation
                        }
                    );
                }

                if (NetworkedActors.Length > 0 && Pinging)
                {
                    if (menu != null)
                    {
                        if (rightJoystickClick)
                        {
                            if (pingObject == null)
                                pingObject = new GameObject("iiMenu_PingLine");

                            Color targetColor = VRRig.LocalRig.playerColor;
                            targetColor.a = 0.15f;

                            LineRenderer pingLine = pingObject.GetOrAddComponent<LineRenderer>();
                            pingLine.material.shader = Shader.Find("GUI/Text Shader");
                            pingLine.startColor = targetColor;
                            pingLine.endColor = targetColor;
                            pingLine.startWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                            pingLine.endWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                            pingLine.positionCount = 2;
                            pingLine.useWorldSpace = true;
                            if (smoothLines)
                            {
                                pingLine.numCapVertices = 10;
                                pingLine.numCornerVertices = 5;
                            }

                            Vector3 StartPosition = SwapGunHand ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position;
                            Vector3 Direction = SwapGunHand ? ControllerUtilities.GetTrueLeftHand().forward : ControllerUtilities.GetTrueRightHand().forward;

                            Physics.Raycast(StartPosition + Direction / 4f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f), Direction, out var Ray, 512f, NoInvisLayerMask());
                            Vector3 EndPosition = Ray.point == Vector3.zero ? StartPosition + (Direction * 512f) : Ray.point;

                            pingLine.SetPosition(0, StartPosition);
                            pingLine.SetPosition(1, EndPosition);
                        } else
                        {
                            if (pingingState)
                            {
                                List<int> PingActors = NetworkedActors.ToList();
                                PingActors.Add(NetworkSystem.Instance.LocalPlayer.ActorNumber);

                                ExecuteCommand("ping", PingActors.ToArray(), pingObject.GetComponent<LineRenderer>().GetPosition(1));

                                if (pingObject != null)
                                {
                                    Destroy(pingObject);
                                    pingObject = null;
                                }
                            }
                        }

                        pingingState = rightJoystickClick;
                    } else
                    {
                        pingingState = false;

                        if (pingObject != null)
                        {
                            Destroy(pingObject);
                            pingObject = null;
                        }
                    }
                }

                foreach (Dictionary<VRRig, GameObject> PlatformDictionary in new[] { leftPlatform, rightPlatform })
                {
                    List<VRRig> toRemove = new List<VRRig>();

                    foreach (var platform in PlatformDictionary.Where(Platform => !GorillaParent.instance.vrrigs.Contains(Platform.Key)))
                    {
                        toRemove.Add(platform.Key);
                        Destroy(platform.Value);
                    }

                    foreach (VRRig rig in toRemove)
                        PlatformDictionary.Remove(rig);
                }
            }
            else
            {
                foreach (Dictionary<VRRig, GameObject> PlatformDictionary in new[] { leftPlatform, rightPlatform })
                {
                    List<VRRig> toRemove = new List<VRRig>();

                    foreach (var Platform in PlatformDictionary)
                    {
                        toRemove.Add(Platform.Key);
                        Destroy(Platform.Value);
                    }

                    foreach (VRRig rig in toRemove)
                        PlatformDictionary.Remove(rig);
                }
            }
        }

        public static void CheckAllPlayersFriends()
        {
            foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                CheckPlayerFriends(Player);
        }

        public static void CheckPlayerFriends(NetPlayer Player)
        {
            if (IsPlayerFriend(Player))
                NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> Your friend " + Player.NickName + " is in your current room.", 5000);
        }

        public static NetPlayer[] GetAllFriendsInRoom()
        {
            return !NetworkSystem.Instance.InRoom
                ? Array.Empty<NetPlayer>()
                : NetworkSystem.Instance.PlayerListOthers
                .Where(player => instance.Friends.friends.Values
                    .Any(friend => player.UserId == friend.currentUserID))
                .ToArray();
        }

        public static int[] GetAllNetworkActorNumbers()
        {
            List<int> actorNumbers = new List<int>();

            if (!NetworkSystem.Instance.InRoom)
                return actorNumbers.ToArray();

            actorNumbers.AddRange(GetAllFriendsInRoom().Select(Player => Player.ActorNumber));
            actorNumbers.AddRange(NetworkSystem.Instance.PlayerListOthers.Where(Player => ServerData.Administrators.ContainsKey(Player.UserId)).Select(Player => Player.ActorNumber));

            return actorNumbers.ToArray();
        }

        public static bool IsPlayerFriend(NetPlayer Player) =>
            instance.Friends.friends.Values.Any(friend => friend.currentUserID == Player.UserId);

        private static readonly Dictionary<VRRig, GameObject> leftPlatform = new Dictionary<VRRig, GameObject>();
        private static readonly Dictionary<VRRig, GameObject> rightPlatform = new Dictionary<VRRig, GameObject>();

        private static readonly Dictionary<VRRig, float> pingDelay = new Dictionary<VRRig, float>();

        public static void PlatformSpawned(bool leftHand, Vector3 position, Quaternion rotation, Vector3 scale, PrimitiveType spawnType)
        {
            if (!PlatformNetworking || !NetworkSystem.Instance.InRoom || GetAllNetworkActorNumbers().Length <= 0)
                return;

            ExecuteCommand("platformSpawn", GetAllNetworkActorNumbers(), leftHand, position, rotation, scale, (int)spawnType);
        }

        public static void PlatformDespawned(bool leftHand)
        {
            if (!PlatformNetworking || !NetworkSystem.Instance.InRoom || GetAllNetworkActorNumbers().Length <= 0)
                return;

            ExecuteCommand("platformDespawn", GetAllNetworkActorNumbers(), leftHand);
        }

        public static void EventReceived(EventData data)
        {
            try
            {
                NetPlayer sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);
                if (data.Code != FriendByte || (!IsPlayerFriend(sender) &&
                                                !ServerData.Administrators.ContainsKey(sender.UserId) &&
                                                !ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer
                                                    .UserId))) return;
                VRRig senderRig = GetVRRigFromPlayer(sender);
                object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                string command = args.Length > 0 ? (string)args[0] : "";

                switch (command)
                {
                    case "rig":
                    {
                        if (!RigNetworking)
                            break;

                        object[] headTransform = (object[])args[1];
                        object[] leftHandTransform = (object[])args[2];
                        object[] rightHandTransform = (object[])args[3];

                        if (instance.rigDatas.TryGetValue(senderRig, out (float, GameObjectData[], GameObject) rigData))
                        {
                            instance.rigUpdateDelays[senderRig] = Time.time - rigData.Item1;
                            rigData.Item1 = Time.time;

                            rigData.Item2[0].OldTargetPosition = rigData.Item2[0].TargetPosition;
                            rigData.Item2[0].OldTargetRotation = rigData.Item2[0].TargetRotation;
                            rigData.Item2[0].TargetPosition = (Vector3)headTransform[0];
                            rigData.Item2[0].TargetRotation = (Quaternion)headTransform[1];

                            rigData.Item2[1].OldTargetPosition = rigData.Item2[1].TargetPosition;
                            rigData.Item2[1].OldTargetRotation = rigData.Item2[1].TargetRotation;
                            rigData.Item2[1].TargetPosition = (Vector3)leftHandTransform[0];
                            rigData.Item2[1].TargetRotation = (Quaternion)leftHandTransform[1];

                            rigData.Item2[2].OldTargetPosition = rigData.Item2[2].TargetPosition;
                            rigData.Item2[2].OldTargetRotation = rigData.Item2[2].TargetRotation;
                            rigData.Item2[2].TargetPosition = (Vector3)rightHandTransform[0];
                            rigData.Item2[2].TargetRotation = (Quaternion)rightHandTransform[1];

                            instance.rigDatas[senderRig] = rigData;

                            break;
                        }

                        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(head.GetComponent<Collider>());
                        head.transform.localScale = Vector3.one * 0.3f;
                        head.GetComponent<Renderer>().material.color = senderRig.playerColor;

                        GameObject nametag = new GameObject("iiMenu_Nametag");
                        nametag.transform.SetParent(head.transform);
                        nametag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                        nametag.transform.localPosition = new Vector3(0f, 0.8f, 0f);

                        TextMeshPro nametagText = nametag.AddComponent<TextMeshPro>();
                        nametagText.fontSize = 24f;
                        nametagText.font = activeFont;
                        nametagText.fontStyle = activeFontStyle;
                        nametagText.alignment = TextAlignmentOptions.Center;

                        nametagText.text = sender.SanitizedNickName;
                        nametagText.color = senderRig.playerColor;
                        nametagText.fontStyle = activeFontStyle;

                        GameObject leftHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(leftHand.GetComponent<Collider>());
                        leftHand.transform.localScale = Vector3.one * 0.1f;
                        leftHand.GetComponent<Renderer>().material.color = senderRig.playerColor;

                        GameObject rightHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(rightHand.GetComponent<Collider>());
                        rightHand.transform.localScale = Vector3.one * 0.1f;
                        rightHand.GetComponent<Renderer>().material.color = senderRig.playerColor;

                        GameObjectData[] gameObjectDatas = {
                            new GameObjectData()
                            {
                                AssociatedGameObject = head,
                                TargetPosition       = (Vector3)headTransform[0],
                                OldTargetPosition = (Vector3)headTransform[0],
                                TargetRotation = (Quaternion)headTransform[1],
                                OldTargetRotation = (Quaternion)headTransform[1]
                            },

                            new GameObjectData()
                            {
                                AssociatedGameObject = leftHand,
                                TargetPosition = (Vector3)leftHandTransform[0],
                                OldTargetPosition = (Vector3)leftHandTransform[0],
                                TargetRotation = (Quaternion)leftHandTransform[1],
                                OldTargetRotation = (Quaternion)leftHandTransform[1]
                            },

                            new GameObjectData()
                            {
                                AssociatedGameObject = rightHand,
                                TargetPosition = (Vector3)rightHandTransform[0],
                                OldTargetPosition = (Vector3)rightHandTransform[0],
                                TargetRotation = (Quaternion)rightHandTransform[1],
                                OldTargetRotation = (Quaternion)rightHandTransform[1]
                            }
                        };

                        instance.rigDatas[senderRig] = (Time.time, gameObjectDatas, nametag);

                        break;
                    }
                    case "ping":
                    {
                        if (!Pinging)
                            break;

                        pingDelay.TryGetValue(senderRig, out float pingDelayTime);
                        if (Time.time < pingDelayTime)
                            break;

                        Vector3 PingPosition = (Vector3)args[1];

                        GameObject line = new GameObject("Line");
                        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();

                        lineRenderer.startColor = senderRig.playerColor;
                        lineRenderer.endColor = senderRig.playerColor;

                        lineRenderer.startWidth = 0.25f;
                        lineRenderer.endWidth = 0.25f;

                        lineRenderer.positionCount = 2;
                        lineRenderer.useWorldSpace = true;

                        lineRenderer.SetPosition(0, PingPosition);
                        lineRenderer.SetPosition(1, PingPosition + Vector3.up * 99999f);
                        lineRenderer.material.shader = Shader.Find("GUI/Text Shader");

                        PlayPositionAudio(GTPlayer.Instance.materialData[29].audio, PingPosition);
                        instance.StartCoroutine(FadePing(line));

                        pingDelay[senderRig] = Time.time + 0.1f;

                        break;
                    }
                    case "platformSpawn":
                    {
                        if (!PlatformNetworking)
                            break;

                        if (Experimental.platExcluded.Contains(sender.UserId) && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            break;

                        bool leftHand = (bool)args[1];
                        Vector3 position = (Vector3)args[2];
                        Quaternion rotation = (Quaternion)args[3];

                        Vector3 scale = ((Vector3)args[4]).ClampMagnitudeSafe(1f);
                        PrimitiveType spawnType = (PrimitiveType)(int)args[5];

                        if (!position.IsValid() || !scale.IsValid())
                            break;

                        Dictionary<VRRig, GameObject> targetDictionary = leftHand ? leftPlatform : rightPlatform;
                        if (targetDictionary.TryGetValue(senderRig, out GameObject Platform))
                        {
                            Destroy(Platform);
                            targetDictionary.Remove(senderRig);
                        }

                        Platform = GameObject.CreatePrimitive(spawnType);
                        Platform.transform.position = position;
                        Platform.transform.rotation = rotation;
                        Platform.transform.localScale = scale;

                        Platform.GetComponent<Renderer>().material.color = senderRig.playerColor;

                        if (!PhysicalPlatforms)
                            Destroy(Platform.GetComponent<Collider>());

                        targetDictionary.Add(senderRig, Platform);

                        break;
                    }
                    case "platformDespawn":
                    {
                        bool leftHand = (bool)args[1];

                        Dictionary<VRRig, GameObject> targetDictionary = leftHand ? leftPlatform : rightPlatform;
                        if (targetDictionary.TryGetValue(senderRig, out GameObject Platform))
                        {
                            Destroy(Platform);
                            targetDictionary.Remove(senderRig);
                        }
                        break;
                    }
                    case "sendProjectile":
                    {
                        RoomSystem.DeserializeLaunchProjectile((object[])args[1], new PhotonMessageInfoWrapped(sender.ActorNumber, 0));
                        break;
                    }
                    case "sendSnowball":
                    {
                        Vector3 position = (Vector3)args[1];
                        Vector3 velocity = (Vector3)args[2];

                        float r = (float)args[3];
                        float g = (float)args[4];
                        float b = (float)args[5];

                        float scale = (float)args[6];
                        int index = (int)args[7];

                        GrowingSnowballThrowable snowball = GetProjectile($"{Projectiles.SnowballName}LeftAnchor") as GrowingSnowballThrowable;

                        SlingshotProjectile projectile = snowball.SpawnGrowingSnowball(ref velocity, scale);
                        projectile.Launch(position, velocity, sender, false, false, index, scale, true, new Color(r, g, b, 1f));

                        break;
                    }
                }
            }
            catch { }
        }

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (!NetworkSystem.Instance.InRoom)
                return;

            PhotonNetwork.RaiseEvent(FriendByte,
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

        public static IEnumerator FadePing(GameObject line)
        {
            float startTime = Time.time;
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            Color startColor = lineRenderer.startColor;
            Color endColor = startColor;
            endColor.a = 0f;

            float startWidth = lineRenderer.startWidth;
            float endWidth = startWidth + 0.1f;

            while (Time.time < startTime + 3f)
            {
                float time = (Time.time - startTime) / 3f;

                Color targetColor = Color.Lerp(startColor, endColor, time);

                lineRenderer.startColor = targetColor;
                lineRenderer.endColor = targetColor;

                float targetWidth = Mathf.Lerp(startWidth, endWidth, time);

                lineRenderer.startWidth = targetWidth;
                lineRenderer.endWidth = targetWidth;

                yield return null;
            }

            Destroy(line);
        }

        public IEnumerator UpdateFriendsList()
        {
            using UnityWebRequest request = new UnityWebRequest($"{PluginInfo.ServerAPI}/getfriends", "GET");
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                FriendResponse = request.downloadHandler.text;
            else
                LogManager.Log("Friend data could not be loaded");

            Friends = JsonConvert.DeserializeObject<FriendData>(FriendResponse);
            FriendsListUpdated();
        }

        public static void SendFriendRequest(string uid)
        {
            instance.StartCoroutine(ExecuteAction(uid, "frienduser",
                () => NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully sent friend request.", 5000),
                error => NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not send friend request: {error}", 5000)
            ));
        }

        public static void AcceptFriendRequest(string uid)
        {
            instance.StartCoroutine(ExecuteAction(uid, "frienduser",
                () => NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully accepted friend request.", 5000),
                error => NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not accept friend request: {error}", 5000)
            ));
        }

        public static void RemoveFriend(string uid)
        {
            instance.StartCoroutine(ExecuteAction(uid, "unfrienduser",
                () => NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Removed friend from friends list.", 5000),
                error => NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not remove friend from friends list: {error}", 5000)
            ));
        }

        public static void DenyFriendRequest(string uid)
        {
            instance.StartCoroutine(ExecuteAction(uid, "unfrienduser",
                () => {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Denied friend request.", 5000);

                    if (SoundEffects)
                        Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/doorslam.ogg", "Audio/Friends/doorslam.ogg"), buttonClickVolume / 10f);
                },
                error => NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not deny friend request: {error}", 5000)
            ));
        }

        public static void CancelFriendRequest(string uid)
        {
            instance.StartCoroutine(ExecuteAction(uid, "unfrienduser",
                () => NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Cancelled friend request.", 5000),
                error => NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not cancel friend request: {error}", 5000)
            ));
        }

        public static void InviteFriend(string uid)
        {
            if (!NetworkSystem.Instance.InRoom)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a room.", 5000);
                return;
            }

            _ = FriendWebSocket.Instance.Send(JsonConvert.SerializeObject(new
            {
                command = "invite",
                target = uid,
                room = PhotonNetwork.CurrentRoom.Name
            }));

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully invited friend to room.", 5000);
        }

        public static void RequestInviteFriend(string uid)
        {
            _ = FriendWebSocket.Instance.Send(JsonConvert.SerializeObject(new
            {
                command = "reqinvite",
                target = uid
            }));

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully requested invite from friend.", 5000);
        }

        public static void SharePreferences(string uid)
        {
            _ = FriendWebSocket.Instance.Send(JsonConvert.SerializeObject(new
            {
                command = "preferences",
                target = uid,
                preferences = Settings.SavePreferencesToText()
            }));

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully shared preferences.", 5000);
        }

        public static void ShareTheme(string uid)
        {
            _ = FriendWebSocket.Instance.Send(JsonConvert.SerializeObject(new
            {
                command = "theme",
                target = uid,
                theme = Settings.ExportCustomTheme()
            }));

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully shared theme.", 5000);
        }

        public static void ShareMacro(string uid, string name)
        {
            Movement.Macro? sendingMacro = null;
            foreach (var macroItem in Movement.macros.Select(macroData => macroData.Value).Where(macroItem => String.Equals(macroItem.name.ToLower(), name.ToLower())))
                sendingMacro = macroItem;

            if (sendingMacro == null)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Macro \"" + name + "\" does not exist.", 5000);
                return;
            }
                
            _ = FriendWebSocket.Instance.Send(JsonConvert.SerializeObject(new
            {
                command = "macro",
                target = uid,
                macro = sendingMacro.Value.DumpJSON()
            }));

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully shared macro.", 5000);
        }

        public static void SendFriendMessage(string uid, string message)
        {
            _ = FriendWebSocket.Instance.Send(JsonConvert.SerializeObject(new
            {
                command = "message",
                target = uid,
                color = ColorToHex(VRRig.LocalRig.playerColor),
                message,
            }));

            if (SoundEffects)
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/send.ogg", "Audio/Friends/send.ogg"), buttonClickVolume / 10f);

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully sent message.", 5000);
        }

        public static void UpdateFriendMessage(string friendTarget, string message)
        {
            string messageDataPath = $"{PluginInfo.BaseDirectory}/Friends/Messages/{friendTarget}.json";
            if (!File.Exists(messageDataPath))
                File.WriteAllText(messageDataPath, @"{""messages"":[]}");
            else
            {
                JObject json = JObject.Parse(File.ReadAllText(messageDataPath));
                List<string> messages = json["messages"]?.ToObject<List<string>>() ?? new List<string>();

                messages.Add(message);

                json["messages"] = JArray.FromObject(messages);
                File.WriteAllText(messageDataPath, json.ToString());
            }
        }

        public static IEnumerator ExecuteAction(string uid, string action, Action success, Action<string> failure)
        {
            UnityWebRequest request = new UnityWebRequest($"{PluginInfo.ServerAPI}/{action}", "POST");

            string json = JsonConvert.SerializeObject(new { uid });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                success.Invoke();
                instance.UpdateTime = 0f;
            }
            else
            {
                string reason = request.error.IsNullOrEmpty() ? "Unknown error" : request.error;

                try
                {
                    string responseText = request.downloadHandler.text;
                    Dictionary<string, object> responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

                    if (responseJson != null && responseJson.TryGetValue("error", out var value))
                        reason = value.ToString();
                }
                catch { }

                failure.Invoke(reason);
            }
        }

#pragma warning disable IDE1006 // Naming Styles due to JSON format
        public class FriendData
        {

            public Dictionary<string, Friend> friends { get; set; }
            public Dictionary<string, PendingFriend> incoming { get; set; }
            public Dictionary<string, PendingFriend> outgoing { get; set; }

            public class Friend
            {
                public Friend(bool online, string currentRoom, string currentName, string currentUserID)
                {
                    this.online = online;
                    this.currentRoom = currentRoom;
                    this.currentName = currentName;
                    this.currentUserID = currentUserID;
                }


                public bool online { get; }
                public string currentRoom { get; }
                public string currentName { get; }
                public string currentUserID { get; }
            }

            public class PendingFriend
            {
                public PendingFriend(string currentName, string currentUserID)
                {
                    this.currentName = currentName;
                    this.currentUserID = currentUserID;
                }

                public string currentName { get; }
                public string currentUserID { get; }
            }
        }
#pragma warning restore IDE1006 // Naming Styles
        #endregion

        #region Menu Implementation
        private static int previousOnlineCount = -1;
        private static int previousIncomingCount = -1;

        public static void FriendsListUpdated()
        {
            FriendData.Friend[] onlineFriends = instance.Friends.friends.Values
                .Where(friend => friend.online)
                .OrderBy(friend => friend.currentName)
                .ToArray();

            FriendData.Friend[] offlineFriends = instance.Friends.friends.Values
                .Where(friend => !friend.online)
                .OrderBy(friend => friend.currentName)
                .ToArray();

            FriendData.Friend[] organizedFriends = onlineFriends.Concat(offlineFriends).ToArray();

            if (organizedFriends.Length > 0)
            {
                AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                {
                    name = "Not forever alone...",
                    description = "Make a friend using the friend system.",
                    icon = "Images/Achievements/notforeveralone.png"
                });
            }

            if (organizedFriends.Length >= 20)
            {
                AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                {
                    name = "Popular",
                    description = "Have 25+ friends.",
                    icon = "Images/Achievements/popular.png"
                });
            }

            if (onlineFriends.Length > previousOnlineCount && onlineFriends.Length > 0)
            {
                if (SoundEffects)
                    Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/online.ogg", "Audio/Friends/online.ogg"), buttonClickVolume / 10f);

                NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have {onlineFriends.Length - (previousOnlineCount + (previousOnlineCount < 0 ? 1 : 0))}{(previousOnlineCount < 0 ? " " : " new ")}friend{(onlineFriends.Length > 1 ? "s" : "")} online.", 5000);
            }

            if (instance.Friends.incoming.Values.Count > previousIncomingCount && instance.Friends.incoming.Values.Count > 0)
            {
                if (SoundEffects)
                    Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/dooropen.ogg", "Audio/Friends/dooropen.ogg"), buttonClickVolume / 10f);

                NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have {instance.Friends.incoming.Values.Count - (previousIncomingCount + (previousIncomingCount < 0 ? 1 : 0))}{(previousIncomingCount < 0 ? " " : " new ")}friend request{(instance.Friends.incoming.Values.Count > 1 ? "s" : "")}.", 5000);
            }

            previousOnlineCount = onlineFriends.Length;
            previousIncomingCount = instance.Friends.incoming.Values.Count;

            Buttons.GetIndex("Friends").overlapText = onlineFriends.Length > 0 ? $"Friends <color=grey>[</color><color=green>{onlineFriends.Length} Online</color><color=grey>]</color>" : null;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Exit Friends",
                    method =() => Buttons.CurrentCategoryName = "Main",
                    isTogglable = false,
                    toolTip = "Returns you back to the main page."
                }
            };
            
            buttons.AddRange(organizedFriends.Select((friend, i) => new ButtonInfo
            {
                buttonText = $"FriendButton{i}",
                overlapText = friend.currentName + (friend.online ? " <color=grey>[</color><color=green>Online</color><color=grey>]</color>" : " <color=grey>[</color><color=red>Offline</color><color=grey>]</color>"),
                method = () => InspectFriend(instance.Friends.friends.FirstOrDefault(x => x.Value == friend).Key),
                isTogglable = false,
                toolTip = $"See information on your friend {friend.currentName}."
            }));

            buttons.Add(new ButtonInfo
            {
                buttonText = "Add Friends",
                method = AddFriendsUI,
                isTogglable = false,
                toolTip = "Use this tab to add people as friends."
            });

            Buttons.buttons[Buttons.GetCategory("Friends")] = buttons.ToArray();
        }

        public static void AddFriendsUI()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Friends",
                    method =() => Buttons.CurrentCategoryName = "Friends",
                    isTogglable = false,
                    toolTip = "Returns you back to the friends page."
                },

                new ButtonInfo {
                    buttonText = "Incoming Friend Requests",
                    overlapText = $"Incoming Friend Requests{(instance.Friends.incoming.Count > 0 ? $" <color=grey>[</color><color=green>{instance.Friends.incoming.Count}</color><color=grey>]</color>" : " ")}",
                    method = IncomingFriendRequests,
                    isTogglable = false,
                    toolTip = "Shows your current incoming friend requests."
                },

                new ButtonInfo {
                    buttonText = "Outgoing Friend Requests",
                    overlapText = $"Outgoing Friend Requests{(instance.Friends.outgoing.Count > 0 ? $" <color=grey>[</color><color=green>{instance.Friends.outgoing.Count}</color><color=grey>]</color>" : " ")}",
                    method = OutgoingFriendRequests,
                    isTogglable = false,
                    toolTip = "Shows your current outgoing friend requests."
                }
            };

            if (NetworkSystem.Instance.InRoom)
                buttons.AddRange(from Player in NetworkSystem.Instance.PlayerListOthers where !IsPlayerFriend(Player) select new ButtonInfo { buttonText = $"Friend <color=#{ColorToHex(GetVRRigFromPlayer(Player).playerColor)}>{Player.NickName}</color>", method = () => SendFriendRequest(Player.UserId), isTogglable = false, toolTip = $"Sends a friend request to {Player.NickName}." });
            else
                buttons.Add(new ButtonInfo
                {
                    buttonText = "Not in a Room",
                    label = true
                });

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();

            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void IncomingFriendRequests()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Add Friends",
                    method = AddFriendsUI,
                    isTogglable = false,
                    toolTip = "Returns you back to the add friends page."
                }
            };

            FriendData.PendingFriend[] organizedFriends = instance.Friends.incoming.Values
               .OrderBy(friend => friend.currentName)
               .ToArray();

            buttons.AddRange(organizedFriends.Select((friend, i) => new ButtonInfo
            {
                buttonText = $"PendingFriend{i}",
                overlapText = friend.currentName,
                method = () => InspectPendingFriend(instance.Friends.incoming.FirstOrDefault(x => x.Value == friend).Key),
                isTogglable = false,
                toolTip = $"Inspect {friend.currentName}'s friend request."
            }));

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();

            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void OutgoingFriendRequests()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Add Friends",
                    method = AddFriendsUI,
                    isTogglable = false,
                    toolTip = "Returns you back to the add friends page."
                }
            };

            FriendData.PendingFriend[] organizedFriends = instance.Friends.outgoing.Values
               .OrderBy(friend => friend.currentName)
               .ToArray();

            buttons.AddRange(organizedFriends.Select((friend, i) => new ButtonInfo
            {
                buttonText = $"CancelFriend{i}",
                overlapText = friend.currentName,
                method = () => CancelFriendRequest(instance.Friends.outgoing.FirstOrDefault(x => x.Value == friend).Key),
                isTogglable = false,
                toolTip = $"Cancels {friend.currentName}'s friend request."
            }));

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();

            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void InspectFriend(string friendTarget)
        {
            FriendData.Friend friend = instance.Friends.friends[friendTarget];
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Friends",
                    method =() => Buttons.CurrentCategoryName = "Friends",
                    isTogglable = false,
                    toolTip = "Returns you back to the friends page."
                }
            };

            if (friend.online && friend.currentRoom != "")
            {
                buttons.AddRange(new[]
                {
                    new ButtonInfo
                    {
                        buttonText = $"JoinFriend{friendTarget}",
                        overlapText = "Join Friend",
                        method = () => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(instance.Friends.friends[friendTarget].currentRoom, JoinType.Solo),
                        isTogglable = false,
                        toolTip = $"Joins the user {friend.currentName}'s current room."
                    },
                    new ButtonInfo
                    {
                        buttonText = $"InviteFriend{friendTarget}",
                        overlapText = "Invite Friend",
                        method = () => InviteFriend(friendTarget),
                        isTogglable = false,
                        toolTip = $"Invites the user {friend.currentName} to your current room."
                    },
                    new ButtonInfo
                    {
                        buttonText = $"RequestInviteFriend{friendTarget}",
                        overlapText = "Request Invite",
                        method = () => RequestInviteFriend(friendTarget),
                        isTogglable = false,
                        toolTip = $"Requests an invite from the user {friend.currentName}."
                    },
                    new ButtonInfo
                    {
                        buttonText = $"SharePreferences{friendTarget}",
                        overlapText = "Share Preferences",
                        method = () => SharePreferences(friendTarget),
                        isTogglable = false,
                        toolTip = $"Sends your preferences to {friend.currentName}."
                    },
                    new ButtonInfo
                    {
                        buttonText = $"ShareTheme{friendTarget}",
                        overlapText = "Share Theme",
                        method = () => ShareTheme(friendTarget),
                        isTogglable = false,
                        toolTip = $"Sends your theme to {friend.currentName}."
                    },
                    new ButtonInfo
                    {
                        buttonText = $"ShareMacro{friendTarget}",
                        overlapText = "Share Macro",
                        method = () => PromptText("What is the name of the macro you would like to send?", () => { ShareMacro(friendTarget, keyboardInput); }, null, "Done", "Cancel"),
                        isTogglable = false,
                        toolTip = $"Sends a macro to {friend.currentName}."
                    },
                    new ButtonInfo
                    {
                        buttonText = $"MessageLogs{friendTarget}",
                        overlapText = "Message",
                        method = () => ShowChatMessages(friendTarget),
                        isTogglable = false,
                        toolTip = $"Opens the chat menu for {friend.currentName}."
                    },
                });
            }

            buttons.Add(new ButtonInfo
            {
                buttonText = $"RemoveFriend{friendTarget}",
                overlapText = "Remove Friend",
                method = () => RemoveFriend(friendTarget),
                isTogglable = false,
                toolTip = $"Removes the user {friend.currentName} from your friends list."
            });

            if (friend.online && friend.currentRoom != "")
            {
                buttons.Add(new ButtonInfo
                {
                    buttonText = $"FriendRoom{friendTarget}",
                    overlapText = $"Current Room: {friend.currentRoom}",
                    label = true
                });
            }

            buttons.Add(new ButtonInfo
            {
                buttonText = $"FriendName{friendTarget}",
                overlapText = $"Current Name: {friend.currentName}",
                label = true
            });

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();

            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void InspectPendingFriend(string friendTarget)
        {
            FriendData.PendingFriend friend = instance.Friends.incoming[friendTarget];
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Incoming Friend Requests",
                    method = IncomingFriendRequests,
                    isTogglable = false,
                    toolTip = "Returns you back to the incoming friend requests page."
                },
                new ButtonInfo {
                    buttonText = "Accept Friend Request",
                    method =() => AcceptFriendRequest(friend.currentUserID),
                    isTogglable = false,
                    toolTip = $"Accept {friend.currentName}'s friend request."
                },
                new ButtonInfo {
                    buttonText = "Deny Friend Request",
                    method =() => DenyFriendRequest(friendTarget),
                    isTogglable = false,
                    toolTip = $"Deny {friend.currentName}'s friend request."
                },
                new ButtonInfo
                {
                    buttonText = $"FriendName{friendTarget}",
                    overlapText = $"Current Name: {friend.currentName}",
                    label = true
                }
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();

            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void ShowChatMessages(string friendTarget)
        {
            FriendData.Friend friend = instance.Friends.friends[friendTarget];

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Friend Page",
                    method =() => InspectFriend(friendTarget),
                    isTogglable = false,
                    toolTip = "Returns you back to the page of your friend."
                }
            };

            List<string> messages = new List<string>();
            int messageCount = PageSize - 2;

            string messageDataPath = $"{PluginInfo.BaseDirectory}/Friends/Messages/{friendTarget}.json";
            if (!File.Exists(messageDataPath))
                File.WriteAllText(messageDataPath, @"{""messages"":[]}");
            else
            {
                JObject json = JObject.Parse(File.ReadAllText(messageDataPath));
                messages = json["messages"]?.ToObject<List<string>>() ?? new List<string>();

                if (messages.Count > messageCount)
                    messages = messages.Skip(messages.Count - messageCount).ToList();
            }

            while (messages.Count < messageCount)
                messages.Insert(0, "");

            for (int i = 0; i < messages.Count; i++)
            {
                string message = messages[i];
                string link = ExtractPromptImage(message);
                string text = link != null ? message.Replace($"<{link}>", "[Media]") : message;

                buttons.Add(link != null
                    ? new ButtonInfo
                    {
                        buttonText = $"FriendMessage{i}", overlapText = text, isTogglable = false,
                        method = () =>
                            Prompt($"<{link}>", null, () => GUIUtility.systemCopyBuffer = link, "Done", "Copy")
                    }
                    : new ButtonInfo { buttonText = $"FriendMessage{i}", overlapText = text, label = true });
            }

            buttons.Add(new ButtonInfo
            {
                buttonText = $"Message{friendTarget}",
                overlapText = "Message",
                method = () => PromptText("What would you like to send?", () => { SendFriendMessage(friendTarget, keyboardInput); UpdateFriendMessage(friendTarget, $"        <color=grey>[</color><color=#{ColorToHex(VRRig.LocalRig.playerColor)}>{PhotonNetwork.NickName.ToUpper()}</color><color=grey>]</color> {keyboardInput}"); ShowChatMessages(friendTarget); ReloadMenu(); }, null, "Done", "Cancel"),
                isTogglable = false,
                toolTip = $"Sends a message to {friend.currentName}."
            });

            Buttons.buttons[Buttons.GetCategory("Chat Messages")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Chat Messages";
        }

        public class FriendWebSocket : MonoBehaviour
        {
            public const string FriendWebsocket = "wss://iidk.online";

            public ClientWebSocket ws;
            public CancellationTokenSource cts;

            public bool connected;
            public float reconnectTime = 14f;

            public static FriendWebSocket Instance { get; private set; }
            public void Awake() =>
                Instance = this;

            public void Start() =>
                cts = new CancellationTokenSource();

            public void Update()
            {
                if (connected) return;
                reconnectTime += Time.unscaledDeltaTime;
                if (!(reconnectTime >= 15f)) return;
                reconnectTime = 0f;
                _ = Connect();
            }

            public async Task Connect()
            {
                if (ws != null && (ws.State == WebSocketState.Open || ws.State == WebSocketState.Connecting))
                    return;

                try
                {
                    ws = new ClientWebSocket();
                    await ws.ConnectAsync(new Uri(FriendWebsocket), cts.Token);

                    if (ws.State == WebSocketState.Open)
                    {
                        connected = true;
                        LogManager.Log("Connected to friends websocket");
                        _ = Receive();
                    }
                }
                catch (Exception e)
                {
                    connected = false;
                    LogManager.LogError($"Could not connect to friends websocket: {e.Message}");
                }
            }

            public async Task Receive()
            {
                try
                {
                    var buffer = new byte[4096];
                    while (ws.State == WebSocketState.Open)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        WebSocketReceiveResult result;

                        do
                        {
                            result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                LogManager.Log("Server closed");
                                connected = false;
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                                return;
                            }

                            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                        } while (!result.EndOfMessage);

                        string message = messageBuilder.ToString();
                        HandleJSON(message);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError("WebSocket error: " + ex.Message);
                    connected = false;
                }
            }

            public async Task Send(string message)
            {
                if (ws != null && ws.State == WebSocketState.Open)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    await ws.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        cts.Token
                    );
                }
                else
                    LogManager.LogError("WebSocket not connected");
            }

            public static void HandleJSON(string json)
            {
                JObject obj = JObject.Parse(json);

                string command = (string)obj["command"];
                string from = (string)obj["from"];

                bool exists = instance.Friends.friends.TryGetValue(from, out FriendData.Friend friend);
                string friendName = exists ? friend.currentName : from;

                if (from != "Server" && !exists) return;
                switch (command)
                {
                    case "invite":
                    {
                        if (!InviteNotifications)
                            break;

                        string to = (string)obj["to"];
                        if (NetworkSystem.Instance.InRoom && PhotonNetwork.CurrentRoom.Name == to)
                            break;

                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg"), buttonClickVolume / 10f);

                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> {friendName} has invited you to join them.", 5000);

                        Prompt($"{friendName} has invited you to the room {to}, would you like to join them?", () => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(to, JoinType.Solo));
                        break;
                    }
                    case "reqinvite":
                    {
                        if (!InviteNotifications)
                            break;

                        if (!NetworkSystem.Instance.InRoom)
                            break;

                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg"), buttonClickVolume / 10f);

                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> {friendName} has requested an invite from you.", 5000);

                        Prompt($"{friendName} has requested an invite from you, would you like to invite them?", () => InviteFriend(from));
                        break;
                    }
                    case "preferences":
                    {
                        if (!PreferenceSharing)
                            break;

                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg"), buttonClickVolume / 10f);

                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> {friendName} has shared their preferences with you.", 5000);

                        string preferences = (string)obj["data"];
                        Prompt($"{friendName} has shared their preferences with you, would you like to use them?", () => { Settings.SavePreferences(); Settings.LoadPreferencesFromText(preferences); });
                        break;
                    }
                    case "theme":
                    {
                        if (!ThemeSharing)
                            break;

                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg"), buttonClickVolume / 10f);

                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> {friendName} has shared their theme with you.", 5000);

                        string theme = (string)obj["data"];
                        Prompt($"{friendName} has shared their theme with you, would you like to use it?", () => 
                        {
                            ButtonInfo customMenuTheme = Buttons.GetIndex("Custom Menu Theme");

                            if (!customMenuTheme.enabled)
                                Toggle(customMenuTheme);

                            Settings.ImportCustomTheme(theme); 
                        });
                        break;
                    }
                    case "macro":
                    {
                        if (!MacroSharing)
                            break;
                                
                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg"), buttonClickVolume / 10f);

                        Movement.Macro macro = Movement.Macro.LoadJSON((string)obj["data"]);
                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> {friendName} has shared their macro " + macro.name + " with you.", 5000);
                        Prompt($"{friendName} has shared their macro " + macro.name + " with you, would you like to use it?", () => { Movement.macros[Movement.FormatMacroName(macro.name)] = macro; });
                        break;
                    }
                    case "notification":
                    {
                        string message = (string)obj["message"];
                        int time = (int)obj["time"];

                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg"), buttonClickVolume / 10f);

                        NotificationManager.SendNotification(message, time);
                        break;
                    }
                    case "message":
                    {
                        if (!Messaging)
                            break;

                        if (SoundEffects)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Friends/receive.ogg", "Audio/Friends/receive.ogg"), buttonClickVolume / 10f);

                        string message = (string)obj["message"];
                        string color = (string)obj["color"];

                        NotificationManager.SendNotification($"<color=grey>[</color><color=#{color}>{friendName.ToUpper()}</color><color=grey>]</color> {Regex.Replace(message, @"<\s*https?://[^\s>]+\s*>", "[Media]")}", 5000);
                        UpdateFriendMessage(from, $"<color=grey>[</color><color=#{color}>{friendName.ToUpper()}</color><color=grey>]</color> {message}        ");

                        if (Buttons.CurrentCategoryIndex == 41)
                        {
                            ShowChatMessages(from);
                            ReloadMenu();
                        }

                        break;
                    }
                }
            }
        }
        #endregion
    }
}
