using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using iiMenu.Classes;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System;
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
                        lmfao.RPC("UpdateCosmeticsWithTryon", RpcTarget.All, CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray());
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
                lmfao.RPC("UpdateCosmeticsWithTryon", RpcTarget.All, CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray());
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

        private static VRRig thestrangled = null;
        public static void AdminStrangle()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
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
                                    GorillaTagger.Instance.myVRRig.SendRPC("PlayHandTap", RpcTarget.All, new object[]{
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
                } else
                {
                    if (Time.time > stupiddelayihate)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.rightHandTransform.position }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            } else
            {
                if (thestrangled != null)
                {
                    thestrangled = null;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("PlayHandTap", RpcTarget.All, new object[]{
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

        public static void FlyAllUsing()
        {
            ChangeName("gtup");
        }

        public static void BecomeAllUsing()
        {
            ChangeName("gtarmy");
        }

        public static void BringAllUsing()
        {
            ChangeName("gtbring");
        }

        public static void BringHandAllUsing()
        {
            ChangeName("gtctrhand");
        }

        public static void BringHeadAllUsing()
        {
            ChangeName("gtctrhead");
        }

        public static void OrbitAllUsing()
        {
            ChangeName("gtorbit");
        }

        public static void CopyAllUsing()
        {
            ChangeName("gtcopy");
        }

        public static void TagAllUsing()
        {
            ChangeName("gttagall");
        }

        public static void SpamNotifsAllUsing()
        {
            ChangeName("gtnotifs");
        }

        public static void UpdateWarningAllUsing()
        {
            ChangeName("gtupdate");
        }

        public static void NoMenuAllUsing()
        {
            ChangeName("gtnomenu");
        }

        public static void NoModsAllUsing()
        {
            ChangeName("gtnomods");
        }
    }
}
