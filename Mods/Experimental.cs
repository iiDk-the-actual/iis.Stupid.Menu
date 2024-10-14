using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Experimental
    {
        public static void DelayBanGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    PhotonView lmfao = RigManager.GetPhotonViewFromVRRig(whoCopy);
                    GetOwnership(lmfao);
                    if (lmfao.AmOwner)
                    {
                        lmfao.RPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.All, CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray());
                    }
                    RPCProtection();
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
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
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void DelayBanAll()
        {
            if (rightTrigger > 0.5f)
            {
                PhotonView lmfao = RigManager.GetPhotonViewFromVRRig(RigManager.GetRandomVRRig(false));
                GetOwnership(lmfao);
                lmfao.RPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.All, CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray());
                RPCProtection();
            }
        }

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

        // Admin mods
        public static void FixName()
        {
            ChangeName(admins[PhotonNetwork.LocalPlayer.UserId]);
        }

        private static float stupiddelayihate = 0f;
        public static void AdminKickGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "tp", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                }
            }
        }

        public static void AdminFlingGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "vel", new Vector3(0f, 100f, 0f) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
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
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber } }, SendOptions.SendReliable);
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
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber } }, SendOptions.SendReliable);
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
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
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
        }

        public static void NoAdminIndicator()
        {
            if (PhotonNetwork.PlayerList.Length != lastplayercount)
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "nocone", true }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void AdminIndicatorBack()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "nocone", false }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void NotifyGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stupiddelayihate)
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
            Miscellaneous.indicatorDelay = Time.time + 2f;
            PhotonNetwork.RaiseEvent(68, new object[] { "isusing" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        private static bool lastLasering = false;
        public static Coroutine laserCoroutine = null;
        public static void AdminLaser()
        {
            if (leftPrimary || rightPrimary)
            {
                Vector3 startPos = (rightHand ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.position : GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) + ((rightHand ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.up : GorillaTagger.Instance.offlineVRRig.leftHandTransform.up) * 0.1f);
                Vector3 endPos = Vector3.zero;
                Vector3 dir = rightHand ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.right : -GorillaTagger.Instance.offlineVRRig.leftHandTransform.right;
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

        public static void FlyAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "vel", new Vector3(0f, 10f, 0f) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
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
    }
}
