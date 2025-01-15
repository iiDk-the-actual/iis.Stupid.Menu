using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using PlayFab.MultiplayerModels;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static Fusion.Sockets.NetBitBuffer;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Overpowered
    {
        public static void MasterCheck()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You are master client.</color>");
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
        }

        public static void SilentGuardian()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                    {
                        if (gorillaGuardianZoneManager.CurrentGuardian != NetworkSystem.Instance.LocalPlayer)
                        {
                            Traverse.Create(gorillaGuardianZoneManager).Field("guardianPlayer").SetValue(NetworkSystem.Instance.LocalPlayer);
                        }
                    }
                }
            }
            else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        public static void GuardianSelf()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                    {
                        gorillaGuardianZoneManager.SetGuardian(NetworkSystem.Instance.LocalPlayer);
                    }
                }
            }
            else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        public static void GuardianGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                            {
                                if (gorillaGuardianZoneManager.enabled)
                                {
                                    gorillaGuardianZoneManager.SetGuardian(GetPlayerFromVRRig(possibly));
                                }
                            }
                        }
                        else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
                        kgDebounce = Time.time + 0.1f;
                    }
                }
            }
        }

        public static void GuardianAll()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int i = 0;
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                    {
                        gorillaGuardianZoneManager.SetGuardian(PhotonNetwork.PlayerList[i]);
                        i++;
                    }
                }
            }
            else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        public static void UnguardianSelf()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                    {
                        if (gorillaGuardianZoneManager.CurrentGuardian == NetworkSystem.Instance.LocalPlayer)
                        {
                            gorillaGuardianZoneManager.SetGuardian(null);
                        }
                    }
                }
            }
            else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        public static void UnguardianGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                            {
                                if (gorillaGuardianZoneManager.enabled)
                                {
                                    if (gorillaGuardianZoneManager.CurrentGuardian == GetPlayerFromVRRig(possibly))
                                    {
                                        gorillaGuardianZoneManager.SetGuardian(null);
                                    }
                                }
                            }
                        }
                        else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
                        kgDebounce = Time.time + 0.1f;
                    }
                }
            }
        }

        public static void UnguardianAll()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                    {
                        gorillaGuardianZoneManager.SetGuardian(null);
                    }
                }
            }
            else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        private static float guardianspazdelay = 0f;
        private static bool lastGuardianThing = false;
        public static void GuardianSpaz()
        {
            if (Time.time > guardianspazdelay)
            {
                guardianspazdelay = Time.time + 0.1f;
                lastGuardianThing = !lastGuardianThing;
                if (lastGuardianThing)
                    GuardianAll();
                else
                    UnguardianAll();
            }
        }

        public static void AlwaysGuardian()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                if (!gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer)) // gzm.enabled && 
                {
                    foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                    {
                        if (gorillaGuardianZoneManager.enabled)
                        {
                            if (gorillaGuardianZoneManager.CurrentGuardian != NetworkSystem.Instance.LocalPlayer)
                            {
                                gorillaGuardianZoneManager.SetGuardian(NetworkSystem.Instance.LocalPlayer);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (TappableGuardianIdol tgi in GetGuardianIdols())
                {
                    if (!tgi.isChangingPositions)
                    {
                        GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                        if (!gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer)) // gzm.enabled && 
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = tgi.transform.position;

                            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = tgi.transform.position;
                            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = tgi.transform.position;

                            tgi.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, tgi.tappableId, UnityEngine.Random.Range(0.2f, 0.4f));
                            RPCProtection();
                        }
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = true;
                    }
                }
            }
        }

        public static void ActiveNetworkHandlerRPC(string rpc, Photon.Realtime.Player target, object[] args)
        {
            NetworkView netView = (NetworkView)Traverse.Create(typeof(GameMode)).Field("activeNetworkHandler").Field("netView").GetValue();
            netView.GetView.RPC(rpc, target, args);
            RPCProtection();
        }
        public static void ActiveNetworkHandlerRPC(string rpc, RpcTarget target, object[] args)
        {
            NetworkView netView = (NetworkView)Traverse.Create(typeof(GameMode)).Field("activeNetworkHandler").Field("netView").GetValue();
            netView.GetView.RPC(rpc, target, args);
            RPCProtection();
        }

        public static void BetaSetVelocityPlayer(NetPlayer victim, Vector3 velocity)
        {
            if (velocity.sqrMagnitude > 20f)
                velocity = Vector3.Normalize(velocity) * 20f;

            GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                ActiveNetworkHandlerRPC("GuardianLaunchPlayer", NetPlayerToPlayer(victim), new object[] { velocity });
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
            }
        }
        public static void BetaSetVelocityTargetGroup(RpcTarget victim, Vector3 velocity)
        {
            if (velocity.sqrMagnitude > 20f)
                velocity = Vector3.Normalize(velocity) * 20f;

            GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                ActiveNetworkHandlerRPC("GuardianLaunchPlayer", victim, new object[] { velocity });
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
            }
        }

        public static void GrabGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            RigManager.GetNetworkViewFromVRRig(possibly).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                            RPCProtection();
                        }
                        else
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                        }
                        kgDebounce = Time.time + 0.1f;
                    }
                }
            }
        }

        public static void GrabAll()
        {
            if (rightGrab && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.1f;
                GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                {
                    foreach (VRRig plr in GorillaParent.instance.vrrigs)
                    {
                        if (plr != GorillaTagger.Instance.offlineVRRig)
                        {
                            RigManager.GetNetworkViewFromVRRig(plr).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                            RPCProtection();
                        }
                    }
                }
                else
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                }
            }
        }

        public static void ReleaseGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            RigManager.GetNetworkViewFromVRRig(possibly).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 0f, 0f) });
                            RPCProtection();
                        }
                        else
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                        }
                        kgDebounce = Time.time + 0.1f;
                    }
                }
            }
        }

        public static void ReleaseAll()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.1f;
                GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                {
                    foreach (VRRig plr in GorillaParent.instance.vrrigs)
                    {
                        if (plr != GorillaTagger.Instance.offlineVRRig)
                        {
                            RigManager.GetNetworkViewFromVRRig(plr).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 0f, 0f) });
                            RPCProtection();
                        }
                    }
                }
                else
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                }
            }
        }

        public static void FlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(possibly), new Vector3(0f, 19.9f, 0f) );
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
            }
        }

        public static void FlingAll()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.1f;
                GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                {
                    BetaSetVelocityTargetGroup(RpcTarget.Others, new Vector3(0f, 19.9f, 0f));
                    RPCProtection();
                }
                else
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                }
            }
        }

        public static void SpazPlayerGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > kgDebounce)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(whoCopy), new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)));
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
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
                }
            }
        }

        public static void SpazAllPlayers()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.1f;
                BetaSetVelocityTargetGroup(RpcTarget.Others, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)));
                RPCProtection();
            }
        }

        public static void GuardianBlindGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > delaything)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        delaything = Time.time + 0.1f;
                        RigManager.GetNetworkViewFromVRRig(possibly).SendRPC("GrabbedByPlayer", GetPlayerFromVRRig(possibly), new object[] { true, false, false });
                        RigManager.GetNetworkViewFromVRRig(possibly).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(possibly), new object[] { new Vector3(0f, float.NaN, 0f) });
                    }
                }
            }
        }

        public static void GuardianBlindAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > delaything)
                {
                    delaything = Time.time + 0.1f;
                    foreach (VRRig player in GorillaParent.instance.vrrigs)
                    {
                        RigManager.GetNetworkViewFromVRRig(player).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        RigManager.GetNetworkViewFromVRRig(player).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, float.NaN, 0f) });
                    }
                }
            }
        }

        public static void GuardianBreakMovementGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > delaything)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        delaything = Time.time + 0.1f;
                        RigManager.GetNetworkViewFromVRRig(possibly).SendRPC("GrabbedByPlayer", GetPlayerFromVRRig(possibly), new object[] { true, false, false });
                        RigManager.GetNetworkViewFromVRRig(possibly).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(possibly), new object[] { new Vector3(0f, float.MinValue, 0f) });
                    }
                }
            }
        }

        public static void GuardianBreakMovementAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > delaything)
                {
                    delaything = Time.time + 0.1f;
                    foreach (VRRig player in GorillaParent.instance.vrrigs)
                    {
                        RigManager.GetNetworkViewFromVRRig(player).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        RigManager.GetNetworkViewFromVRRig(player).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, float.MinValue, 0f) });
                    }
                }
            }
        }

        private static float miniaturedelay = 0f;
        private static float lastBeforeClearTime = -1f;
        public static void AtticCrashAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > miniaturedelay)
                {
                    miniaturedelay = Time.time + 0.022f;
                    for (int i = 0; i < 4; i++)
                    {
                        BuilderTable.instance.RequestCreatePiece(691844031, new Vector3(-127.6248f, 16.99441f, -217.2094f), Quaternion.identity, 0);
                    }
                }

                if (Time.time > lastBeforeClearTime)
                {
                    RPCProtection();
                    foreach (BuilderPiece piece in GetPieces())
                    {
                        if (piece.pieceType == 691844031)
                            piece.gameObject.SetActive(false);
                    }
                    lastBeforeClearTime = Time.time + 1f;
                }
            }
        }

        private static Coroutine DisableCoroutine;
        private static IEnumerator DisableSnowball(bool rigDisabled)
        {
            yield return new WaitForSeconds(0.3f);

            if (rigDisabled)
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            GetProjectile("LMACF. RIGHT.").SetSnowballActiveLocal(false);
        }

        public static void BetaSpawnSnowball(Vector3 Pos, Vector3 Vel, float Scale, int Mode, Player Target = null, bool NetworkSize = true, int customNetworkedSize = -1)
        {
            try
            {
                try
                {
                    GetProjectile("LMACF. RIGHT.").SetSnowballActiveLocal(true);
                } catch { }

                Vel = Vel.ClampMagnitudeSafe(50f);

                bool isTooFar = Vector3.Distance(Pos, GorillaTagger.Instance.bodyCollider.transform.position) > 3.5f;
                if (isTooFar)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = Pos + new Vector3(0f, Vel.y > 0f ? -3f : 3f, 0f);
                }

                if (DisableCoroutine != null)
                    CoroutineManager.EndCoroutine(DisableCoroutine);

                DisableCoroutine = CoroutineManager.RunCoroutine(DisableSnowball(isTooFar));

                GrowingSnowballThrowable GrowingSnowball = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingSnowballRightAnchor(Clone)/LMACF. RIGHT.").GetComponent<GrowingSnowballThrowable>();
                PhotonEvent Event = (PhotonEvent)Traverse.Create(GrowingSnowball).Field("snowballThrowEvent").GetValue();

                if (NetworkSize)
                {
                    PhotonEvent Event2 = (PhotonEvent)Traverse.Create(GrowingSnowball).Field("changeSizeEvent").GetValue();
                    Event2.RaiseAll(customNetworkedSize > 0 ? customNetworkedSize : (int)Scale);
                }
                
                switch (Mode)
                {
                    case 0:
                        Event.RaiseAll(Pos, Vel, Scale);
                        break;
                    case 1:
                        Event.RaiseOthers(Pos, Vel, Scale);
                        break;
                    case 2:
                        PhotonNetwork.RaiseEvent(176, new object[]
                        {
                            (int)Traverse.Create(Event).Field("_eventId").GetValue(),
                            Pos,
                            Vel,
                            Scale
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { Target.ActorNumber }
                        }, new SendOptions
                        {
                            Reliability = false,
                            Encrypt = true
                        });
                        break;
                }
                RPCProtection();
            } catch { }
        }

        public static void BetaSnowballImpact(Player Target)
        {
            object[] playerEffectData = new object[6];
            playerEffectData[0] = Target.ActorNumber;
            playerEffectData[1] = 0;

            object[] sendEventData = new object[3];
            sendEventData[0] = PhotonNetwork.ServerTimestamp;
            sendEventData[1] = (byte)6;
            sendEventData[2] = playerEffectData;

            PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
            RPCProtection();
        }

        private static float snowballDelay = 0f;
        public static void SnowballAirstrikeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(NewPointer.transform.position + new Vector3(0f, 50f, 0f), Vector3.zero, 50f, 0);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void SnowballRain()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f)), Vector3.zero, 1f, 0);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void SnowballHail()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f)), new Vector3(0f, -50f, 0f), 3f, 0);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void SnowballOrbit()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30f), 2f, MathF.Sin((float)Time.frameCount / 30f)), new Vector3(0f, 50f, 0f), 5f, 0);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void MassiveSnowballGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(NewPointer.transform.position + new Vector3(0f, 1f, 0f), new Vector3(0f, 50f, 0f), 10f, 0);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void SnowballMinigun()
        {
            if (rightGrab && Time.time > snowballDelay)
            {
                BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength * 5f, 5f, 0);
                snowballDelay = Time.time + 0.12f;
            }
        }

        public static void SnowballParticleGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(NewPointer.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0f, 0f), 15f, 0);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void SnowballImpactEffectGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        snowballDelay = Time.time + 0.1f;
                        BetaSnowballImpact(NetPlayerToPlayer(GetPlayerFromVRRig(possibly)));
                    }
                }
            }
        }

        public static void SnowballPunchMod()
        {
            if (Time.time > snowballDelay)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (rig != GorillaTagger.Instance.offlineVRRig && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f))
                    {
                        Vector3 targetDirection = GorillaTagger.Instance.headCollider.transform.position - rig.headMesh.transform.position;
                        BetaSpawnSnowball(GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(rig)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
            }
        }

        public static void SnowballFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        BetaSpawnSnowball(whoCopy.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
                if (GetGunInput(true))
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
                }
            }
        }

        private static bool randomPlayerToggle = false;
        private static float playerWait = 0f;
        private static Player archiveRandomPlayer = null;
        public static Player SpedGetRandomPlayer()
        {
            if (Time.time > playerWait || archiveRandomPlayer == null)
            {
                randomPlayerToggle = !randomPlayerToggle;

                if (randomPlayerToggle || archiveRandomPlayer == null)
                    archiveRandomPlayer = GetRandomPlayer(false);

                playerWait = Time.time + 0.0777f;
            }
            
            return archiveRandomPlayer;
        }

        public static void SnowballFlingAll()
        {
            if (rightTrigger > 0.5f && Time.time > snowballDelay)
            {
                snowballDelay = Time.time + 0.0777f;
                Player plr = SpedGetRandomPlayer();
                BetaSpawnSnowball(GetVRRigFromPlayer(plr).transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, plr);
            }
        }

        public static void SnowballFlingVerticalGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        BetaSpawnSnowball(whoCopy.headMesh.transform.position + new Vector3(0f, -0.7f, 0f), new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
                if (GetGunInput(true))
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
                    isCopying = false;
            }
        }

        public static void SnowballFlingVerticalAll()
        {
            if (rightTrigger > 0.5f && Time.time > snowballDelay)
            {
                snowballDelay = Time.time + 0.0777f;
                Player plr = SpedGetRandomPlayer();
                BetaSpawnSnowball(GetVRRigFromPlayer(plr).transform.position + new Vector3(0f, -0.7f, 0f), new Vector3(0f, -500f, 0f), 5f, 2, plr);
            }
        }

        public static void SnowballFlingTowardsGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    snowballDelay = Time.time + 0.0777f;
                    Player plr = SpedGetRandomPlayer();
                    Vector3 targetDirection = (NewPointer.transform.position - GetVRRigFromPlayer(plr).headMesh.transform.position).normalized;
                    BetaSpawnSnowball(GetVRRigFromPlayer(plr).transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(-targetDirection.x, 0f, -targetDirection.z) / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, plr);
                }
            }
        }

        public static void SnowballFlingAwayGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(NewPointer.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -500f, 0f), 15f, 1);
                    snowballDelay = Time.time + 0.12f;
                }
            }
        }

        public static void SnowballFlingPlayerTowardsGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        Vector3 targetDirection = (whoCopy.headMesh.transform.position - GorillaTagger.Instance.headCollider.transform.position).normalized;
                        BetaSpawnSnowball(whoCopy.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z) * 1.5f, new Vector3(0f, -100f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
                if (GetGunInput(true))
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
                    isCopying = false;
            }
        }

        public static void SnowballFlingPlayerAwayGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        Vector3 targetDirection = (GorillaTagger.Instance.headCollider.transform.position - whoCopy.headMesh.transform.position).normalized;
                        BetaSpawnSnowball(whoCopy.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z) * 1.5f, new Vector3(0f, -100f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
                if (GetGunInput(true))
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
                    isCopying = false;
            }
        }

        public static void SnowballPushGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        Vector3 targetDirection = GorillaTagger.Instance.headCollider.transform.position - whoCopy.headMesh.transform.position;
                        BetaSpawnSnowball(GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
                if (GetGunInput(true))
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
                    isCopying = false;
            }
        }

        public static void SnowballStrongFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        BetaSpawnSnowball(new Vector3(GorillaTagger.Instance.headCollider.transform.position.x, 1000f, GorillaTagger.Instance.headCollider.transform.position.z), new Vector3(0f, -9999f, 0f), 9999f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)));
                        snowballDelay = Time.time + 0.12f;
                    }
                }
                if (GetGunInput(true))
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
                    isCopying = false;
            }
        }

        public static void SnowballStrongFlingAll()
        {
            if (rightTrigger > 0.5f && Time.time > snowballDelay)
            {
                snowballDelay = Time.time + 0.12f;
                BetaSpawnSnowball(new Vector3(GorillaTagger.Instance.headCollider.transform.position.x, 1000f, GorillaTagger.Instance.headCollider.transform.position.z), new Vector3(0f, -9999f, 0f), 9999f, 1);
            }
        }

        public static void AntiReportFling()
        {
            try
            {
                if (Time.time > snowballDelay)
                {
                    foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                    {
                        if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                        {
                            Transform report = line.reportButton.gameObject.transform;
                            if (GetIndex("Visualize Anti Report").enabled)
                            {
                                VisualizeAura(report.position, Safety.threshold, Color.red);
                            }
                            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                            {
                                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                                {
                                    float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                    float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                    if (D1 < Safety.threshold || D2 < Safety.threshold)
                                    {
                                        if (!Safety.smartarp || (Safety.smartarp && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                        {
                                            snowballDelay = Time.time + 0.12f;
                                            BetaSpawnSnowball(report.position, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(vrrig)));
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been flung.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static bool SpedRPC(PhotonView photonView, string method, Player player, object[] parameters)
        {
            if (photonView != null && parameters != null && !string.IsNullOrEmpty(method))
            {
                var rpcHash = new ExitGames.Client.Photon.Hashtable
                {
                    { 0, photonView.ViewID },
                    { 2, (int)(PhotonNetwork.ServerTimestamp + -int.MaxValue) },
                    { 3, method },
                    { 4, parameters }
                };

                if (photonView.Prefix > 0)
                {
                    rpcHash[1] = (short)photonView.Prefix;
                }
                if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(method))
                {
                    rpcHash[5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(method);
                }
                if (PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber == player.ActorNumber)
                {
                    typeof(PhotonNetwork).GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic).Invoke(typeof(PhotonNetwork), new object[]
                    {
                        (ExitGames.Client.Photon.Hashtable)rpcHash, (Player)PhotonNetwork.LocalPlayer
                    });
                }
                else
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash, new RaiseEventOptions
                    {
                        TargetActors = new int[]
                        {
                            player.ActorNumber,
                        }
                    }, new SendOptions
                    {
                        Reliability = true,
                        DeliveryMode = DeliveryMode.ReliableUnsequenced,
                        Encrypt = false
                    });
                }
            }
            return false;
        }

        // Hi skids :3
        // If you take this code you like giving sloppy wet kisses to cute boys >_<
        // I gotta stop
        private static float delaything = 0f;
        private static float CptiDelay = 0f;
        public static void AtticServersidedBlocks()
        {
            if (PhotonNetwork.IsMasterClient && Time.time > CptiDelay)
            {
                CptiDelay = Time.time + 5f;

                MethodInfo cpti = typeof(BuilderTableNetworking).GetMethod("CreatePlayerTableInit", BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    cpti.Invoke(BuilderTableNetworking.instance, new object[] { player });
                }
            }

            if (PhotonNetwork.InRoom)
            {
                if (!BuilderTable.instance.IsInBuilderZone() && PhotonNetwork.IsMasterClient)
                    BuilderTable.instance.SetInBuilderZone(true);
            } else
            {
                if (BuilderTable.instance.IsInBuilderZone())
                    BuilderTable.instance.SetInBuilderZone(false);
            }
        }

        /*public static void ChangeGamemode(string gamemode)
        {
            if (!PhotonNetwork.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
                {
                    { "gameMode", PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace(GorillaComputer.instance.currentGameMode.Value.ToUpper(), gamemode) }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);
                /*
                NetworkView netView = (NetworkView)Traverse.Create(typeof(GameMode)).Field("activeNetworkHandler").Field("netView").GetValue();
                NetworkSystem.Instance.NetDestroy(netView.gameObject);

                GorillaGameManager ggs = (GorillaGameManager)Traverse.Create(typeof(GameMode)).Field("activeGameMode").GetValue();
                Traverse.Create(typeof(GameMode)).Field("activeGameMode").SetValue(null);
                Traverse.Create(typeof(GameMode)).Field("activeNetworkHandler").SetValue(null);

                GameMode.LoadGameMode(gamemode);
            }
        }*/

        // I see you
        public static void ForceUnloadCustomMap()
        {
            delaything = Time.time + 0.1f;
            PhotonView goldentrophy = GameObject.Find("Environment Objects/LocalObjects_Prefab/VirtualStump_CustomMapLobby/ModIOMapsTerminal/NetworkObject").GetComponent<PhotonView>();

            goldentrophy.RPC("UnloadMapRPC", RpcTarget.All, new object[] { });
            RPCProtection();
        }

        public static void VirtualStumpTeleporterEffectSpammer()
        {
            if (rightTrigger > 0.5f)
            {
                PhotonView that = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/NetObject_VRTeleporter").GetComponent<Photon.Pun.PhotonView>();
                for (int i = 0; i < 8; i++)
                {
                    that.RPC("ActivateTeleportVFX", Photon.Pun.RpcTarget.All, new object[] { (short)i });
                    that.RPC("ActivateReturnVFX", Photon.Pun.RpcTarget.All, new object[] { (short)i });
                    RPCProtection();
                }
            }
        }

        // Don't steal this
        public static void VirtualStumpKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > delaything)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        delaything = Time.time + 0.1f;
                        PhotonView goldentrophy = GameObject.Find("Environment Objects/LocalObjects_Prefab/VirtualStump_CustomMapLobby/ModIOMapsTerminal/NetworkObject").GetComponent<PhotonView>();

                        goldentrophy.RPC("SetRoomMapRPC", NetPlayerToPlayer(GetPlayerFromVRRig(possibly)), new object[] { UnityEngine.Random.Range(-99999L, 99999L) });
                        goldentrophy.RPC("UnloadMapRPC", NetPlayerToPlayer(GetPlayerFromVRRig(possibly)), new object[] { });
                        RPCProtection();
                    }
                }
            }
        }

        // Or I will kill you
        public static void VirtualStumpKickAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > delaything)
                {
                    delaything = Time.time + 0.1f;

                    PhotonView goldentrophy = GameObject.Find("Environment Objects/LocalObjects_Prefab/VirtualStump_CustomMapLobby/ModIOMapsTerminal/NetworkObject").GetComponent<PhotonView>();

                    goldentrophy.RPC("SetRoomMapRPC", RpcTarget.Others, new object[] { UnityEngine.Random.Range(-99999L, 99999L) });
                    goldentrophy.RPC("UnloadMapRPC", RpcTarget.Others, new object[] { });
                    RPCProtection();
                }
            }
        }

        public static void PhysicalFreezeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > kgDebounce)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(whoCopy), Vector3.zero);
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
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
                }
            }
        }

        public static void PhysicalFreezeAll()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.1f;
                BetaSetVelocityTargetGroup(RpcTarget.Others, Vector3.zero);
                RPCProtection();
            }
        }

        public static void BringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > kgDebounce)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(whoCopy), (GorillaTagger.Instance.bodyCollider.transform.position - whoCopy.transform.position) * 2f);
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
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
                }
            }
        }

        public static void BringAll()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.2f;
                foreach (VRRig plr in GorillaParent.instance.vrrigs)
                {
                    if (plr != GorillaTagger.Instance.offlineVRRig)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), (GorillaTagger.Instance.bodyCollider.transform.position - plr.transform.position) * 3f);
                        RPCProtection();
                    }
                }
            }
        }

        private static float thingdeb = 0f;
        public static void GiveFlyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > thingdeb)
                    {
                        if (whoCopy.rightThumb.calcT > 0.5f)
                        {
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(whoCopy), whoCopy.headMesh.transform.forward * 15f);
                            RPCProtection();
                        }
                        thingdeb = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
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
                }
            }
        }

        public static void GiveFlyAll()
        {
            if (Time.time > thingdeb)
            {
                thingdeb = Time.time + 0.1f;
                foreach (VRRig plr in GorillaParent.instance.vrrigs)
                {
                    if (plr != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (plr.rightThumb.calcT > 0.5f)
                        {
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), plr.headMesh.transform.forward * 15f);
                            RPCProtection();
                        }
                    }
                }
            }
        }

        private static float anotdelay = 0f;
        public static void SafetyBubble()
        {
            if (Time.time > anotdelay)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position) < 3f)
                        {
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(vrrig), Vector3.Normalize(vrrig.transform.position - GorillaTagger.Instance.bodyCollider.transform.position) * 5f);
                            if (PhotonNetwork.InRoom)
                            {
                                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                    248,
                                    false,
                                    999999f
                                });
                            }
                            else
                            {
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(248, false, 999999f);
                            }
                            anotdelay = Time.time + 0.1f;
                        }
                    }
                }
            }
        }

        public static void BringAllGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (Time.time > kgDebounce)
                    {
                        foreach (VRRig plr in GorillaParent.instance.vrrigs)
                        {
                            if (plr != GorillaTagger.Instance.offlineVRRig)
                            {
                                BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), Vector3.Normalize(NewPointer.transform.position - plr.transform.position) * 50f);
                            }
                        }
                        RPCProtection();
                        kgDebounce = Time.time + 0.2f;
                    }
                }
            }
        }

        private static float slamDel = 0f;
        private static bool flip = false;
        public static void EffectSpamHands()
        {
            if (rightGrab)
            {
                if (Time.time > slamDel)
                {
                    GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                    if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    {
                        ActiveNetworkHandlerRPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", RpcTarget.All, new object[] { GorillaTagger.Instance.rightHandTransform.position, new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) });
                        RPCProtection();
                        flip = !flip;
                    }
                    else
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                    }
                    slamDel = Time.time + 0.05f;
                }
            }
            if (leftGrab)
            {
                if (Time.time > slamDel)
                {
                    GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                    if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    {
                        ActiveNetworkHandlerRPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", RpcTarget.All, new object[] { GorillaTagger.Instance.leftHandTransform.position, new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) });
                        RPCProtection();
                        flip = !flip;
                    }
                    else
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                    }
                    slamDel = Time.time + 0.05f;
                }
            }
        }

        public static void EffectSpamGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                    if (Time.time > slamDel)
                    {
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            ActiveNetworkHandlerRPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", RpcTarget.All, new object[] { NewPointer.transform.position, new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) });
                            RPCProtection();
                            flip = !flip;
                        }
                        else
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                        }
                        slamDel = Time.time + 0.05f;
                    }
                    
                }
            }
        }

        public static void SpawnSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
        }

        public static void AngerSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated)
            {
                secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            }
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemotePlayerSeen", RpcTarget.All, new object[] { });
        }

        public static void ThrowSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated)
            {
                secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            }
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemotePlayerCaught", RpcTarget.All, new object[] { });
        }

        public static float lasttimeaa = 0f;
        public static void SpazSecondLook()
        {
            if (Time.time > lasttimeaa)
            {
                lasttimeaa = Time.time + 0.75f;
                GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
                secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.PlayerThrown || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Reset)
                {
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
                }
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Activated || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Patrolling)
                {
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemotePlayerSeen", RpcTarget.All, new object[] { });
                }
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Chasing)
                {
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
                }
            }
        }

        // No, it's not skidded, read the debunk: https://pastebin.com/raw/FL5j8fcy
        private static bool lastfreezegarbage = false;
        private static float Garfield = 0f;
        public static void FreezeAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > Garfield)
                {
                    lastfreezegarbage = !lastfreezegarbage;
                    foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                    {
                        GorillaPlayerScoreboardLine.MutePlayer(line.linePlayer.UserId, line.linePlayer.NickName, lastfreezegarbage ? 1 : 0);
                    }
                    Garfield = Time.time + 0.1f;
                }
            }
        }

        public static void DestroyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        NetPlayer player = GetPlayerFromVRRig(possibly);
                        PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void DestroyAll()
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
            }
        }

        public static void TargetSpam()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (HitTargetNetworkState hitTargetNetworkState in Resources.FindObjectsOfTypeAll<HitTargetNetworkState>())
                {
                    Traverse.Create(hitTargetNetworkState).Field("hitCooldownTime").SetValue(0);
                    hitTargetNetworkState.TargetHit(Vector3.zero, Vector3.zero);
                }
            } else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        public static void InfectionToTag()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                gorillaTagManager.SetisCurrentlyTag(true);
                gorillaTagManager.ClearInfectionState();
                gorillaTagManager.ChangeCurrentIt(GameMode.ParticipatingPlayers[UnityEngine.Random.Range(0, GameMode.ParticipatingPlayers.Count)]);
            }
        }

        public static void TagToInfection()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                gorillaTagManager.SetisCurrentlyTag(false);
                gorillaTagManager.ClearInfectionState();
                NetPlayer victim = GameMode.ParticipatingPlayers[UnityEngine.Random.Range(0, GameMode.ParticipatingPlayers.Count)];
                gorillaTagManager.AddInfectedPlayer(victim);
                gorillaTagManager.lastInfectedPlayer = victim;
            }
        }

        public static void BetaSetStatus(int state, RaiseEventOptions balls)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                object[] statusSendData = new object[1];
                statusSendData[0] = state;
                object[] sendEventData = new object[3];
                sendEventData[0] = PhotonNetwork.ServerTimestamp;
                sendEventData[1] = (byte)2;
                sendEventData[2] = statusSendData;
                PhotonNetwork.RaiseEvent(3, sendEventData, balls, SendOptions.SendUnreliable);
            }
        }

        public static void SlowGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        NetPlayer player = GetPlayerFromVRRig(possibly);
                        BetaSetStatus(0, new RaiseEventOptions { TargetActors = new int[1] { player.ActorNumber } });
                        RPCProtection();
                        kgDebounce = Time.time + 1f;
                    }
                }
            }
        }

        public static void SlowAll()
        {
            if (Time.time > kgDebounce)
            {
                BetaSetStatus(0, new RaiseEventOptions { Receivers = ReceiverGroup.Others });
                RPCProtection();
                kgDebounce = Time.time + 1f;
            }
        }

        public static void VibrateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        NetPlayer owner = GetPlayerFromVRRig(possibly);
                        BetaSetStatus(1, new RaiseEventOptions { TargetActors = new int[1] { owner.ActorNumber } });
                        RPCProtection();
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void VibrateAll()
        {
            if (Time.time > kgDebounce)
            {
                BetaSetStatus(1, new RaiseEventOptions { Receivers = ReceiverGroup.Others });
                RPCProtection();
                kgDebounce = Time.time + 0.5f;
            }
        }
        
        public static void GliderBlindGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }

                if (isCopying)
                {
                    foreach (GliderHoldable glider in GetGliders())
                    {
                        if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            glider.gameObject.transform.position = whoCopy.headMesh.transform.position;
                            glider.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                        }
                        else
                        {
                            glider.OnHover(null, null);
                        }
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                }
            }
        }

        public static void GliderBlindAll()
        {
            GliderHoldable[] those = GetGliders();
            int index = 0;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    try
                    {
                        GliderHoldable glider = those[index];
                        if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            glider.gameObject.transform.position = vrrig.headMesh.transform.position;
                            glider.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                        }
                        else
                        {
                            glider.OnHover(null, null);
                        }
                    } catch { }
                    index++;
                }
            }
        }

        public static void BreakAudioGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", GetPlayerFromVRRig(whoCopy), new object[]{
                        111,
                        false,
                        999999f
                        //0.01f
                    });
                    RPCProtection();
                }
                if (GetGunInput(true))
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

        public static void BreakAudioAll()
        {
            if (rightTrigger > 0.5f)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]{
                    111,
                    false,
                    999999f
                });
            }
        }

        private static float RopeDelay = 0f;
        public static void JoystickRopeControl() // Thanks to ShibaGT for the fix
        {
            Vector2 joy = ControllerInputPoller.instance.rightControllerPrimary2DAxis;

            if ((Mathf.Abs(joy.x) > 0.05f || Mathf.Abs(joy.y) > 0.05f) && Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.25f;
                foreach (GorillaRopeSwing rope in GetRopes())
                {
                    RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { rope.ropeId, 1, new Vector3(joy.x * 50f, joy.y * 50f, 0f), true, null });
                    RPCProtection();
                }
            }
        }

        public static void SpazRopeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaRopeSwing possibly = Ray.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (possibly && Time.time > RopeDelay)
                    {
                        RopeDelay = Time.time + 0.25f;
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { possibly.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true, null });
                        RPCProtection();
                    }
                }
            }
        }

        public static void SpazAllRopes()
        {
            if (rightTrigger > 0.5f)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > RopeDelay)
                {
                    RopeDelay = Time.time + 0.25f;
                    foreach (GorillaRopeSwing rope in GetRopes())
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { rope.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true, null });
                        RPCProtection();
                    }
                }
            }
        }

        public static void SpazGrabbedRopes()
        {
            if (Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.1f;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    GorillaRopeSwing rope = (GorillaRopeSwing)Traverse.Create(vrrig).Field("currentRopeSwing").GetValue();
                    if (rope != null)
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { rope.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true, null });
                        RPCProtection();
                    }
                }
            }
        }

        public static void ConfusingRopes()
        {
            if (Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.1f;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    GorillaRopeSwing rope = (GorillaRopeSwing)Traverse.Create(vrrig).Field("currentRopeSwing").GetValue();
                    if (rope != null)
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)), new object[] { rope.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true, null });
                        RPCProtection();
                    }
                }
            }
        }

        public static void FlingRopeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaRopeSwing possibly = Ray.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (possibly)
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { possibly.ropeId, 1, (possibly.transform.position - GorillaTagger.Instance.headCollider.transform.position).normalized * 50f, true, null });
                        RPCProtection();
                    }
                }
            }
        }

        public static void FlingAllRopesGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > RopeDelay)
                {
                    RopeDelay = Time.time + 0.25f;
                    foreach (GorillaRopeSwing rope in GetRopes())
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { rope.ropeId, 1, (NewPointer.transform.position - rope.transform.position).normalized * 50f, true, null });
                        RPCProtection();
                    }
                }
            }
        }
    }
}
