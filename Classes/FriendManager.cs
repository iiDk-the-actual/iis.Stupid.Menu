using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaNetworking;
using iiMenu.Menu;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using Valve.Newtonsoft.Json;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
{
    public class FriendManager : MonoBehaviour
    {
        #region Friend Manager Code
        public static FriendManager instance;

        private float UpdateTime;
        private string FriendResponse;
        public const int FriendByte = 53;

        public FriendData Friends = new FriendData { friends = new Dictionary<string, FriendData.Friend> { }, incoming = new Dictionary<string, FriendData.PendingFriend> { }, outgoing = new Dictionary<string, FriendData.PendingFriend> { } };

        public void Awake()
        {
            instance = this;
            UpdateTime = Time.time + 5f;

            NetworkSystem.Instance.OnJoinedRoomEvent += CheckAllPlayersFriends;
            NetworkSystem.Instance.OnPlayerJoined += CheckPlayerFriends;

            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
        }

        private static Material starMaterial;
        private static Texture2D starTexture;

        private static float updateRigDelay;
        private static Dictionary<VRRig, GameObject> starPool = new Dictionary<VRRig, GameObject> { };

        public static bool RigNetworking = true;
        public static bool PlatformNetworking = true;

        public static bool PhysicalPlatforms;

        public void Update()
        {
            if (Time.time > UpdateTime)
            {
                UpdateTime = Time.time + 30f;
                CoroutineManager.RunCoroutine(UpdateFriendsList());
            }

            List<VRRig> toRemoveRigs = new List<VRRig>();

            foreach (KeyValuePair<VRRig, GameObject> star in starPool)
            {
                if (!GorillaParent.instance.vrrigs.Contains(star.Key) || !IsPlayerFriend(GetPlayerFromVRRig(star.Key)))
                {
                    toRemoveRigs.Add(star.Key);
                    Destroy(star.Value);
                }
            }

            foreach (VRRig rig in toRemoveRigs)
                starPool.Remove(rig);

            if (PhotonNetwork.InRoom)
            {
                NetPlayer[] AllFriends = GetAllFriendsInRoom();
                foreach (NetPlayer player in AllFriends)
                {
                    VRRig playerRig = GetVRRigFromPlayer(player);
                    if (playerRig != null)
                    {
                        if (!starPool.TryGetValue(playerRig, out GameObject playerStar))
                        {
                            playerStar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            Destroy(playerStar.GetComponent<Collider>());

                            if (starMaterial == null)
                            {
                                if (starTexture == null)
                                    starTexture = LoadTextureFromResource("iiMenu.Resources.star.png");

                                starMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
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

                                starMaterial.SetFloat("_Glossiness", 0f);
                                starMaterial.SetFloat("_Metallic", 0f);
                            }

                            playerStar.GetComponent<Renderer>().material = starMaterial;
                            starPool.Add(playerRig, playerStar);
                        }

                        playerStar.GetComponent<Renderer>().material.color = playerRig.playerColor;

                        playerStar.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * playerRig.scaleFactor;
                        playerStar.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * ((ServerData.Administrators.ContainsKey(player.UserId) ? 1.3f : 0.8f) * playerRig.scaleFactor);
                        playerStar.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    }
                }

                int[] NetworkedActors = GetAllNetworkActorNumbers();
                if (RigNetworking && !VRRig.LocalRig.enabled && Time.time > updateRigDelay && NetworkedActors.Length > 0)
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

                if (AllFriends.Length > 0)
                {
                    foreach (Dictionary<VRRig, GameObject> PlatformDictionary in new[] { leftPlatform, rightPlatform })
                    {
                        List<VRRig> toRemove = new List<VRRig>();

                        foreach (var Platform in PlatformDictionary)
                        {
                            if (!GorillaParent.instance.vrrigs.Contains(Platform.Key))
                            {
                                toRemove.Add(Platform.Key);
                                Destroy(Platform.Value);
                            }
                        }

                        foreach (VRRig rig in toRemove)
                            PlatformDictionary.Remove(rig);
                    }
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
                NotifiLib.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> Your friend " + Player.NickName + " is in your current room.", 5000);
        }

        public static NetPlayer[] GetAllFriendsInRoom()
        {
            if (!NetworkSystem.Instance.InRoom)
                return Array.Empty<NetPlayer>();

            return NetworkSystem.Instance.PlayerListOthers
                .Where(player => instance.Friends.friends.Values
                    .Any(friend => player.UserId == friend.currentUserID))
                .ToArray();
        }

        public static int[] GetAllNetworkActorNumbers()
        {
            List<int> actorNumbers = new List<int> { };

            if (!PhotonNetwork.InRoom)
                return actorNumbers.ToArray();

            foreach (NetPlayer Player in GetAllFriendsInRoom())
                actorNumbers.Add(Player.ActorNumber);

            foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
            {
                if (ServerData.Administrators.ContainsKey(Player.UserId))
                    actorNumbers.Add(Player.ActorNumber);
            }

            return actorNumbers.ToArray();
        }

        public static bool IsPlayerFriend(NetPlayer Player) =>
            instance.Friends.friends.Values.Any(friend => friend.currentUserID == Player.UserId);

        private static Dictionary<VRRig, float> ghostRigDelay = new Dictionary<VRRig, float> { };

        private static Dictionary<VRRig, GameObject> leftPlatform = new Dictionary<VRRig, GameObject> { };
        private static Dictionary<VRRig, GameObject> rightPlatform = new Dictionary<VRRig, GameObject> { };

        public static void PlatformSpawned(bool leftHand, Vector3 position, Quaternion rotation, Vector3 scale, PrimitiveType spawnType)
        {
            if (!PlatformNetworking || !PhotonNetwork.InRoom || GetAllNetworkActorNumbers().Length <= 0)
                return;

            ExecuteCommand("platformSpawn", GetAllNetworkActorNumbers(), leftHand, position, rotation, scale, (int)spawnType);
        }

        public static void PlatformDespawned(bool leftHand)
        {
            if (!PlatformNetworking || !PhotonNetwork.InRoom || GetAllNetworkActorNumbers().Length <= 0)
                return;

            ExecuteCommand("platformDespawn", GetAllNetworkActorNumbers(), leftHand);
        }

        public static void EventReceived(EventData data)
        {
            try
            {
                NetPlayer Sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
                if (data.Code == FriendByte && (IsPlayerFriend(Sender) || ServerData.Administrators.ContainsKey(Sender.UserId)))
                {
                    VRRig SenderRig = GetVRRigFromPlayer(Sender);
                    object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                    string command = args.Length > 0 ? (string)args[0] : "";

                    switch (command)
                    {
                        case "rig":
                            {
                                if (!RigNetworking)
                                    break;

                                if (ghostRigDelay.TryGetValue(SenderRig, out float delay))
                                {
                                    if (Time.time < delay)
                                        return;

                                    ghostRigDelay.Remove(SenderRig);
                                }

                                ghostRigDelay.Add(SenderRig, Time.time + 0.09f);

                                object[] HeadTransform = (object[])args[1] ?? null;
                                object[] LeftTransform = (object[])args[2] ?? null;
                                object[] RightTransform = (object[])args[3] ?? null;

                                GameObject Head = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                Destroy(Head.GetComponent<Collider>());
                                Destroy(Head, 0.15f);

                                Head.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                                Head.transform.position = (Vector3)HeadTransform[0];
                                Head.transform.rotation = (Quaternion)HeadTransform[1];
                                Head.GetComponent<Renderer>().material.color = SenderRig.playerColor;

                                GameObject LeftHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                Destroy(LeftHand.GetComponent<Collider>());
                                Destroy(LeftHand, 0.15f);

                                LeftHand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                                LeftHand.transform.position = (Vector3)LeftTransform[0];
                                LeftHand.transform.rotation = (Quaternion)LeftTransform[1];
                                LeftHand.GetComponent<Renderer>().material.color = SenderRig.playerColor;

                                GameObject RightHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                Destroy(RightHand.GetComponent<Collider>());
                                Destroy(RightHand, 0.15f);

                                RightHand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                                RightHand.transform.position = (Vector3)RightTransform[0];
                                RightHand.transform.rotation = (Quaternion)RightTransform[1];
                                RightHand.GetComponent<Renderer>().material.color = SenderRig.playerColor;

                                GameObject Nametag = new GameObject("iiMenu_Nametag");
                                Destroy(Nametag, 0.15f);
                                Nametag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                TextMesh textMesh = Nametag.AddComponent<TextMesh>();
                                textMesh.fontSize = 48;
                                textMesh.characterSize = 0.1f;
                                textMesh.anchor = TextAnchor.MiddleCenter;
                                textMesh.alignment = TextAlignment.Center;

                                textMesh.text = Sender.NickName;
                                textMesh.color = SenderRig.playerColor;
                                textMesh.fontStyle = activeFontStyle;

                                Nametag.transform.position = (Vector3)HeadTransform[0] + Vector3.up * 0.25f;
                                Nametag.transform.LookAt(Camera.main.transform.position);
                                Nametag.transform.Rotate(0f, 180f, 0f);
                                break;
                            }
                        case "platformSpawn":
                            {
                                if (!PlatformNetworking)
                                    break;

                                bool leftHand = (bool)args[1];
                                Vector3 position = (Vector3)args[2];
                                Quaternion rotation = (Quaternion)args[3];

                                Vector3 scale = ((Vector3)args[4]).ClampMagnitudeSafe(5f);
                                PrimitiveType spawnType = (PrimitiveType)(int)args[5];

                                if (!position.IsValid() || !scale.IsValid())
                                    break;

                                Dictionary<VRRig, GameObject> targetDictionary = leftHand ? leftPlatform : rightPlatform;
                                if (targetDictionary.TryGetValue(SenderRig, out GameObject Platform))
                                {
                                    Destroy(Platform);
                                    targetDictionary.Remove(SenderRig);
                                }

                                Platform = GameObject.CreatePrimitive(spawnType);
                                Platform.transform.position = position;
                                Platform.transform.rotation = rotation;
                                Platform.transform.localScale = scale;

                                Platform.GetComponent<Renderer>().material.color = SenderRig.playerColor;

                                if (!PhysicalPlatforms)
                                    Destroy(Platform.GetComponent<Collider>());

                                targetDictionary.Add(SenderRig, Platform);

                                break;
                            }
                        case "platformDespawn":
                            {
                                bool leftHand = (bool)args[1];

                                Dictionary<VRRig, GameObject> targetDictionary = leftHand ? leftPlatform : rightPlatform;
                                if (targetDictionary.TryGetValue(SenderRig, out GameObject Platform))
                                {
                                    Destroy(Platform);
                                    targetDictionary.Remove(SenderRig);
                                }
                                break;
                            }
                        case "sendProjectile":
                            {
                                RoomSystem.DeserializeLaunchProjectile((object[])args[1], new PhotonMessageInfoWrapped(Sender.ActorNumber, 0));
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

                                GrowingSnowballThrowable snowball = GetProjectile("GrowingSnowballLeftAnchor") as GrowingSnowballThrowable;

                                SlingshotProjectile projectile = snowball.SpawnGrowingSnowball(ref velocity, scale);
                                projectile.Launch(position, velocity, Sender, false, false, index, scale, true, new Color(r, g, b, 1f));

                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch { }
        }

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.RaiseEvent(FriendByte,
                (new object[] { command })
                    .Concat(parameters)
                    .ToArray(),
            options, SendOptions.SendReliable);
        }

        public static void ExecuteCommand(string command, int[] targets, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = targets }, parameters);

        public static void ExecuteCommand(string command, int target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = new int[] { target } }, parameters);

        public static void ExecuteCommand(string command, ReceiverGroup target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { Receivers = target }, parameters);

        public System.Collections.IEnumerator UpdateFriendsList()
        {
            using UnityWebRequest request = new UnityWebRequest("https://iidk.online/getfriends", "GET");
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
            CoroutineManager.instance.StartCoroutine(ExecuteAction(uid, "frienduser",
                () => NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully sent friend request.", 5000),
                (string error) => NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not send friend request: {error}", 5000)
            ));
        }

        public static void AcceptFriendRequest(string uid)
        {
            CoroutineManager.instance.StartCoroutine(ExecuteAction(uid, "frienduser",
                () => NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully accepted friend request.", 5000),
                (string error) => NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not accept friend request: {error}", 5000)
            ));
        }

        public static void RemoveFriend(string uid)
        {
            CoroutineManager.instance.StartCoroutine(ExecuteAction(uid, "unfrienduser",
                () => NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Removed friend from friends list.", 5000),
                (string error) => NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not remove friend from friends list: {error}", 5000)
            ));
        }

        public static void DenyFriendRequest(string uid)
        {
            CoroutineManager.instance.StartCoroutine(ExecuteAction(uid, "unfrienduser",
                () => NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Denied friend request.", 5000),
                (string error) => NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not deny friend request: {error}", 5000)
            ));
        }

        public static void CancelFriendRequest(string uid)
        {
            CoroutineManager.instance.StartCoroutine(ExecuteAction(uid, "unfrienduser",
                () => NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Cancelled friend request.", 5000),
                (string error) => NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not cancel friend request: {error}", 5000)
            ));
        }

        public static System.Collections.IEnumerator ExecuteAction(string uid, string action, Action success, Action<string> failure)
        {
            UnityWebRequest request = new UnityWebRequest($"https://iidk.online/{action}", "POST");

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

                    if (responseJson != null && responseJson.ContainsKey("error"))
                        reason = responseJson["error"].ToString();
                }
                catch { }

                failure.Invoke(reason);
            }
        }

        public class FriendData
        {
            public Dictionary<string, Friend> friends { get; set; }
            public Dictionary<string, PendingFriend> incoming { get; set; }
            public Dictionary<string, PendingFriend> outgoing { get; set; }

            public class Friend
            {
                public bool online { get; set; }
                public string currentRoom { get; set; }
                public string currentName { get; set; }
                public string currentUserID { get; set; }
            }

            public class PendingFriend
            {
                public string currentName { get; set; }
                public string currentUserID { get; set; }
            }
        }
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

            if (onlineFriends.Length > previousOnlineCount && onlineFriends.Length > 0)
                NotifiLib.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have {onlineFriends.Length - (previousOnlineCount + (previousOnlineCount < 0 ? 1 : 0))}{(previousOnlineCount < 0 ? " " : " new ")}friend{(onlineFriends.Length > 1 ? "s" : "")} online.", 5000);

            if (instance.Friends.incoming.Values.Count > previousIncomingCount && instance.Friends.incoming.Values.Count > 0)
                NotifiLib.SendNotification($"<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have {instance.Friends.incoming.Values.Count - (previousIncomingCount + (previousIncomingCount < 0 ? 1 : 0))}{(previousIncomingCount < 0 ? " " : " new ")}friend request{(instance.Friends.incoming.Values.Count > 1 ? "s" : "")}.", 5000);

            previousOnlineCount = onlineFriends.Length;
            previousIncomingCount = instance.Friends.incoming.Values.Count;

            if (onlineFriends.Length > 0)
                GetIndex("Friends").overlapText = $"Friends <color=grey>[</color><color=green>{onlineFriends.Length} Online</color><color=grey>]</color>";
            else
                GetIndex("Friends").overlapText = null;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Exit Friends",
                    method =() => currentCategoryName = "Main",
                    isTogglable = false,
                    toolTip = "Returns you back to the main page."
                }
            };

            for (int i = 0; i < organizedFriends.Length; i++)
            {
                FriendData.Friend friend = organizedFriends[i];
                buttons.Add(new ButtonInfo
                {
                    buttonText = $"FriendButton{i}",
                    overlapText = friend.currentName + (friend.online ? " <color=grey>[</color><color=green>Online</color><color=grey>]</color>" : " <color=grey>[</color><color=red>Offline</color><color=grey>]</color>"),
                    method = () => InspectFriend(instance.Friends.friends.FirstOrDefault(x => x.Value == friend).Key),
                    isTogglable = false,
                    toolTip = $"See information on your friend {friend.currentName}."
                });
            }

            buttons.Add(new ButtonInfo
            {
                buttonText = "Add Friends",
                method = () => AddFriendsUI(),
                isTogglable = false,
                toolTip = "Use this tab to add people as friends."
            });

            Buttons.buttons[34] = buttons.ToArray();
        }

        public static void AddFriendsUI()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Friends",
                    method =() => currentCategoryName = "Friends",
                    isTogglable = false,
                    toolTip = "Returns you back to the friends page."
                },

                new ButtonInfo {
                    buttonText = "Incoming Friend Requests",
                    overlapText = $"Incoming Friend Requests{(instance.Friends.incoming.Count > 0 ? $" <color=grey>[</color><color=green>{instance.Friends.incoming.Count}</color><color=grey>]</color>" : " ")}",
                    method =() => IncomingFriendRequests(),
                    isTogglable = false,
                    toolTip = "Shows your current incoming friend requests."
                },

                new ButtonInfo {
                    buttonText = "Outgoing Friend Requests",
                    overlapText = $"Outgoing Friend Requests{(instance.Friends.outgoing.Count > 0 ? $" <color=grey>[</color><color=green>{instance.Friends.outgoing.Count}</color><color=grey>]</color>" : " ")}",
                    method =() => OutgoingFriendRequests(),
                    isTogglable = false,
                    toolTip = "Shows your current outgoing friend requests."
                }
            };

            if (NetworkSystem.Instance.InRoom)
            {
                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    if (!IsPlayerFriend(Player))
                        buttons.Add(new ButtonInfo
                        {
                            buttonText = $"Friend <color=#{ColorToHex(GetVRRigFromPlayer(Player).playerColor)}>{Player.NickName}</color>",
                            method = () => SendFriendRequest(Player.UserId),
                            isTogglable = false,
                            toolTip = $"Sends a friend request to {Player.NickName}."
                        });
                }
            }
            else
                buttons.Add(new ButtonInfo
                {
                    buttonText = "Not in a Room",
                    label = true
                });

            Buttons.buttons[29] = buttons.ToArray();
        }

        public static void IncomingFriendRequests()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Add Friends",
                    method =() => AddFriendsUI(),
                    isTogglable = false,
                    toolTip = "Returns you back to the add friends page."
                }
            };

            FriendData.PendingFriend[] organizedFriends = instance.Friends.incoming.Values
               .OrderBy(friend => friend.currentName)
               .ToArray();

            for (int i = 0; i < organizedFriends.Length; i++)
            {
                FriendData.PendingFriend friend = organizedFriends[i];
                buttons.Add(new ButtonInfo
                {
                    buttonText = $"PendingFriend{i}",
                    overlapText = friend.currentName,
                    method = () => InspectPendingFriend(instance.Friends.incoming.FirstOrDefault(x => x.Value == friend).Key),
                    isTogglable = false,
                    toolTip = $"Inspect {friend.currentName}'s friend request."
                });
            }

            Buttons.buttons[29] = buttons.ToArray();
        }

        public static void OutgoingFriendRequests()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Add Friends",
                    method =() => AddFriendsUI(),
                    isTogglable = false,
                    toolTip = "Returns you back to the add friends page."
                }
            };

            FriendData.PendingFriend[] organizedFriends = instance.Friends.outgoing.Values
               .OrderBy(friend => friend.currentName)
               .ToArray();

            for (int i = 0; i < organizedFriends.Length; i++)
            {
                FriendData.PendingFriend friend = organizedFriends[i];
                buttons.Add(new ButtonInfo
                {
                    buttonText = $"CancelFriend{i}",
                    overlapText = friend.currentName,
                    method = () => CancelFriendRequest(instance.Friends.outgoing.FirstOrDefault(x => x.Value == friend).Key),
                    isTogglable = false,
                    toolTip = $"Cancels {friend.currentName}'s friend request."
                });
            }

            Buttons.buttons[29] = buttons.ToArray();
        }

        public static void InspectFriend(string friendTarget)
        {
            currentCategoryName = "Temporary Category";

            FriendData.Friend friend = instance.Friends.friends[friendTarget];
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Friends",
                    method =() => currentCategoryName = "Friends",
                    isTogglable = false,
                    toolTip = "Returns you back to the friends page."
                }
            };

            if (friend.online && friend.currentRoom != "")
            {
                buttons.Add(new ButtonInfo
                {
                    buttonText = $"JoinFriend{friendTarget}",
                    overlapText = "Join Friend",
                    method = () => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(instance.Friends.friends[friendTarget].currentRoom, GorillaNetworking.JoinType.Solo),
                    isTogglable = false,
                    toolTip = $"Joins the user {friend.currentName}'s current room."
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

            Buttons.buttons[29] = buttons.ToArray();
        }

        public static void InspectPendingFriend(string friendTarget)
        {
            currentCategoryName = "Temporary Category";

            FriendData.PendingFriend friend = instance.Friends.incoming[friendTarget];
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Return to Incoming Friend Requests",
                    method =() => IncomingFriendRequests(),
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

            Buttons.buttons[29] = buttons.ToArray();
        }
        #endregion
    }
}