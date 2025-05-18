using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Classes.RigManager;
using System.IO;
using HarmonyLib;
using iiMenu.Menu;

namespace iiMenu.Mods
{
    public class Experimental
    {
        public static void Hyperflush()
        {
            Traverse.Create(typeof(PhotonNetwork)).Field("serializeStreamOut").Method("ResetWriteStream").GetValue();
        }

        public static void FixDuplicateButtons()
        {
            int duplicateButtons = 0;
            List<string> previousNames = new List<string> { };
            foreach (ButtonInfo[] buttonn in Buttons.buttons)
            {
                foreach (ButtonInfo button in buttonn)
                {
                    if (previousNames.Contains(button.buttonText))
                    {
                        string buttonText = button.overlapText == null ? button.buttonText : button.overlapText;
                        button.overlapText = buttonText;
                        button.buttonText += "X";
                        duplicateButtons++;
                    }
                    previousNames.Add(button.buttonText);
                }
            }
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully fixed " + duplicateButtons.ToString() + " broken buttons.");
        }

        public static void EnableOverlapRPCs()
        {
            NoOverlapRPCs = false;
        }

        public static void DisableOverlapRPCs()
        {
            NoOverlapRPCs = true;
        }

        // Damn sure this does jack shit, idk who sent this to me
        public static void AntiRPCBan()
        {
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);

            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);

            try
            {
                GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
                //GorillaNot.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
                //GorillaNot.instance.OnLeftRoom();
                //GorillaNot.instance.OnPreLeavingRoom();
                if (GorillaNot.instance != null)
                {
                    FieldInfo report = typeof(GorillaNot).GetField("sendReport", BindingFlags.NonPublic);
                    if (report != null)
                    {
                        report.SetValue(GorillaNot.instance, false);
                    }
                    report = typeof(GorillaNot).GetField("_sendReport", BindingFlags.NonPublic);
                    if (report != null)
                    {
                        report.SetValue(GorillaNot.instance, false);
                    }
                }
            }
            catch { }
            RPCProtection();
            //GorillaNot.instance.OnLeftRoom();
        }

        private static Dictionary<Renderer, Material> oldMats = new Dictionary<Renderer, Material> { };
        public static void BetterFPSBoost()
        {
            foreach (Renderer v in Resources.FindObjectsOfTypeAll<Renderer>())
            {
                try
                {
                    if (v.material.shader.name == "GorillaTag/UberShader")
                    {
                        oldMats.Add(v, v.material);
                        Material replacement = new Material(Shader.Find("GorillaTag/UberShader"));
                        replacement.color = v.material.color;
                        v.material = replacement;
                    }
                } catch (System.Exception exception) { UnityEngine.Debug.LogError(string.Format("mat error {1} - {0}", exception.Message, exception.StackTrace)); }
            }
        }
        public static void DisableBetterFPSBoost()
        {
            foreach (KeyValuePair<Renderer, Material> v in oldMats)
            {
                v.Key.material = v.Value;
            }
        }

        private static float stupiddelayihate = 0f;
        public static void AdminKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "kick", RigManager.GetPlayerFromVRRig(possibly).UserId }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void AdminKickAll()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "kickall" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void FlipMenuGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "toggle", "Right Hand" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void AdminTeleportGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "tp", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                }
            }
        }

        public static void AdminFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", new Vector3(0f, 50f, 0f) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        private static float stdell = 0f;
        private static VRRig thestrangled = null;
        private static VRRig thestrangledleft = null;
        public static void AdminStrangle()
        {
            if (leftGrab)
            {
                if (thestrangledleft == null)
                {
                    foreach (VRRig lol in GorillaParent.instance.vrrigs)
                    {
                        if (lol != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (Vector3.Distance(lol.headMesh.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 0.2f)
                            {
                                thestrangledleft = lol;
                                if (PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                        89,
                                        true,
                                        999999f
                                    });
                                }
                                else
                                {
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, true, 999999f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Time.time > stdell)
                    {
                        stdell = Time.time + 0.05f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.leftHandTransform.position }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
            else
            {
                if (thestrangledleft != null)
                {
                    try {
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", GorillaLocomotion.GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber } }, SendOptions.SendReliable);
                    } catch { }
                thestrangledleft = null;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            true,
                            999999f
                        });
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, true, 999999f);
                    }
                }
            }

            if (rightGrab)
            {
                if (thestrangled == null)
                {
                    foreach (VRRig lol in GorillaParent.instance.vrrigs)
                    {
                        if (lol != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (Vector3.Distance(lol.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 0.2f)
                            {
                                thestrangled = lol;
                                if (PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                        89,
                                        false,
                                        999999f
                                    });
                                }
                                else
                                {
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, false, 999999f);
                                }
                            }
                        }
                    }
                } else
                {
                    if (Time.time > stupiddelayihate)
                    {
                        stupiddelayihate = Time.time + 0.05f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.rightHandTransform.position }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
            else
            {
                if (thestrangled != null)
                {
                    try
                    {
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", GorillaLocomotion.GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber } }, SendOptions.SendReliable);
                    } catch { }
                    thestrangled = null;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            false,
                            999999f
                        });
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, false, 999999f);
                    }
                }
            }
        }

        public static void AdminObjectGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "platf", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
        }

        private static float lastnetscale = 1f;
        private static float scalenetdel = 0f;
        private static int lastplayercount = 0;
        public static void AdminNetworkScale()
        {
            if (Time.time > scalenetdel && (lastnetscale != GorillaTagger.Instance.offlineVRRig.scaleFactor || PhotonNetwork.PlayerList.Length != lastplayercount))
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "scale", GorillaTagger.Instance.offlineVRRig.scaleFactor }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                scalenetdel = Time.time + 0.1f;
                lastnetscale = GorillaTagger.Instance.offlineVRRig.scaleFactor;
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }
        public static void UnAdminNetworkScale()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "scale", 1f }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void LightningGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "strike", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
        }

        public static void LightningAura()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "strike", GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 1f, MathF.Sin((float)Time.frameCount / 30)) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void LightningRain()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.1f;
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f), 10f, UnityEngine.Random.Range(-10f, 10f)), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
                VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "kick", RigManager.GetPlayerFromVRRig(possibly).UserId }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                } else
                {
                    PhotonNetwork.RaiseEvent(68, new object[] { "strike", Ray.point }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
        }

        private static Vector3 whereOriginalPlayerPos = Vector3.zero;
        private static Vector3 originalMePosition = Vector3.zero;
        public static void AdminFearGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    TeleportPlayer(whoCopy.transform.position + whoCopy.transform.forward);
                    if (Time.time > stupiddelayihate)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "muteall" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", new Vector3(0f, 21f, 0f) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        originalMePosition = GorillaTagger.Instance.bodyCollider.transform.position;
                        whereOriginalPlayerPos = possibly.transform.position;

                        int anum = RigManager.GetPlayerFromVRRig(possibly).ActorNumber;
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 16f, 0f), new Vector3(10f, 1f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 24f, 0f), new Vector3(10f, 1f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(4f, 20f, 0f), new Vector3(1f, 10f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(-4f, 20f, 0f), new Vector3(1f, 10f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);

                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 20f, 4f), new Vector3(10f, 10f, 1f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 20f, -4f), new Vector3(10f, 10f, 1f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);

                        GameObject lol = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(lol, 60f);
                        lol.GetComponent<Renderer>().material.color = Color.black;
                        lol.transform.position = new Vector3(0f, 20f, 0f);
                        lol.transform.localScale = new Vector3(10f, 1f, 10f);

                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    TeleportPlayer(originalMePosition);
                    PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", whereOriginalPlayerPos }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                    PhotonNetwork.RaiseEvent(68, new object[] { "unmuteall" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                }
            }
        }

        public static void AdminSoundMicGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "soundboard", "Alone at the Edge of a Universe", "https://github.com/iiDk-the-actual/ModInfo/raw/main/alone%20at%20the%20edge%20of%20a%20universe.mp3" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void AdminSoundLocalGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "soundcs", "Alone at the Edge of a Universe", "https://github.com/iiDk-the-actual/ModInfo/raw/main/alone%20at%20the%20edge%20of%20a%20universe.mp3" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void EnableNoAdminIndicator()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "nocone", true }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            lastplayercount = -1;
        }

        public static void NoAdminIndicator()
        {
            if (!PhotonNetwork.InRoom)
            {
                lastplayercount = -1;
            }
            if (PhotonNetwork.PlayerList.Length != lastplayercount && PhotonNetwork.InRoom)
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "nocone", true }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }

        public static void AdminIndicatorBack()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "nocone", false }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void EnableAdminMenuUserTags()
        {
            PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;
        }

        private static bool lastInRoom = false;
        private static int lastPlayerCount = -1;
        public static void AdminUserTagSys(EventData data)
        {
            try
            {
                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
                if (data.Code == 68 && sender != PhotonNetwork.LocalPlayer)
                {
                    object[] args = (object[])data.CustomData;
                    string command = (string)args[0];
                    switch (command)
                    {
                        case "confirmusing":
                            if (admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            {
                                VRRig vrrig = GetVRRigFromPlayer(sender);
                                if (!nametags.ContainsKey(vrrig))
                                {
                                    GameObject go = new GameObject("iiMenu_Nametag");
                                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                    TextMesh textMesh = go.AddComponent<TextMesh>();
                                    textMesh.fontSize = 48;
                                    textMesh.characterSize = 0.1f;
                                    textMesh.anchor = TextAnchor.MiddleCenter;
                                    textMesh.alignment = TextAlignment.Center;

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Classes.Console.GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);

                                    nametags.Add(vrrig, go);
                                } else
                                {
                                    TextMesh textMesh = nametags[vrrig].GetComponent<TextMesh>();

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Classes.Console.GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);
                                }
                            }
                            break;
                    }
                }
            }
            catch { }
        }

        private static Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject> { };
        public static void AdminMenuUserTags()
        {
            if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "isusing" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
            lastInRoom = PhotonNetwork.InRoom;
            lastPlayerCount = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount = -1;
            
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    nametags.Remove(nametag.Key);
                } else
                {
                    nametag.Value.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                    nametag.Value.transform.position = nametag.Key.headMesh.transform.position + nametag.Key.headMesh.transform.up * 0.6f;
                    nametag.Value.transform.LookAt(Camera.main.transform.position);
                    nametag.Value.transform.Rotate(0f, 180f, 0f);
                }
            }
        }

        public static void DisableAdminMenuUserTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                UnityEngine.Object.Destroy(nametag.Value);
            }
            nametags.Clear();
        }

        public static void JoinGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "join", searchText.ToUpper() }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void JoinAll()
        {
            if (rightTrigger > 0.5f && Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(68, new object[] { "join", searchText.ToUpper() }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void NotifyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "notify", searchText }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void NotifyAll()
        {
            if (rightTrigger > 0.5f && Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(68, new object[] { "notify", searchText }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void GetMenuUsers()
        {
            Classes.Console.indicatorDelay = Time.time + 2f;
            PhotonNetwork.RaiseEvent(68, new object[] { "isusing" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        private static bool lastLasering = false;
        public static void AdminLaser()
        {
            if (leftPrimary || rightPrimary)
            {
                Vector3 dir = rightPrimary ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.right : -GorillaTagger.Instance.offlineVRRig.leftHandTransform.right;
                Vector3 startPos = (rightPrimary ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.position : GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) + (dir * 0.1f);
                try
                {
                    Physics.Raycast(startPos + (dir / 3f), dir, out var Ray, 512f, NoInvisLayerMask());
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        PhotonNetwork.RaiseEvent(68, new object[] { "silkick", RigManager.GetPlayerFromVRRig(possibly).UserId }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    }
                } catch { }
                if (Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "laser", true, rightPrimary }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
            bool isLasering = leftPrimary || rightPrimary;
            if (lastLasering && !isLasering)
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "laser", false, false }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
            lastLasering = isLasering;
        }

        private static float beamDelay = 0f;
        public static void AdminBeam()
        {
            if (rightTrigger > 0.5f && Time.time > beamDelay)
            {
                beamDelay = Time.time + 0.05f;
                float h = (Time.frameCount / 180f) % 1f;
                Color color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                PhotonNetwork.RaiseEvent(68, new object[] { "lr", color.r, color.g, color.b, color.a, 0.5f, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f), GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30) * 100f, 0.5f, Mathf.Sin((float)Time.frameCount / 30) * 100f), 0.1f }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        private static float startTimeTrigger = 0f;
        private static bool lastTriggerLaserSpam = false;
        public static void AdminFractals()
        {
            if (rightTrigger > 0.5f && !lastTriggerLaserSpam)
                startTimeTrigger = Time.time;

            lastTriggerLaserSpam = rightTrigger > 0.5f;

            if (rightTrigger > 0.5f && Time.time > beamDelay)
            {
                beamDelay = Time.time + 0.5f;
                float h = (Time.frameCount / 180f) % 1f;
                Color color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                PhotonNetwork.RaiseEvent(68, new object[] { "lr", 0f, 1f, 1f, 0.3f, 0.25f, GorillaTagger.Instance.bodyCollider.transform.position, GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 1000f, 20f - (Time.time - startTimeTrigger) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void FlyAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "vel", new Vector3(0f, 10f, 0f) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void AdminBringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void BringAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void BringHandAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", TrueRightHand().position + TrueRightHand().forward }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void BringHeadAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void OrbitAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f)) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void ConfirmNotifyAllUsing()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "notify", admins[PhotonNetwork.LocalPlayer.UserId] == "goldentrophy" ? "Yes, I am the real goldentrophy. I made the menu." : "Yes, I am the real " + admins[PhotonNetwork.LocalPlayer.UserId] + ". I am an admin in the Discord server." }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void AdminFakeCosmetics()
        {
            foreach (string cosmetic in CosmeticsController.instance.currentWornSet.ToDisplayNameArray())
                PhotonNetwork.RaiseEvent(68, new object[] { "cosmetic", cosmetic }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray());
        }

        public static bool daaind = false;
        public static void DisableAllAdminIndicators()
        {
            daaind = true;
        }
        public static void EnableAllAdminIndicators()
        {
            daaind = false;
        }
    }
}
