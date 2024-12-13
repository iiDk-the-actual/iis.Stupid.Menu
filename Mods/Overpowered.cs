using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion.Gameplay;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
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

        private static float miniaturedelay = 0f;
        private static float lastBeforeClearTime = -1f;
        public static void AtticCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > miniaturedelay)
                    {
                        miniaturedelay = Time.time + 0.022f;
                        BuilderTable.instance.RequestCreatePiece(1700948013, whoCopy.headMesh.transform.position, Quaternion.identity, 0);
                        BuilderTable.instance.RequestCreatePiece(1700948013, whoCopy.rightHandTransform.position, Quaternion.identity, 0);
                        BuilderTable.instance.RequestCreatePiece(1700948013, whoCopy.leftHandTransform.position, Quaternion.identity, 0);
                        BuilderTable.instance.RequestCreatePiece(1700948013, whoCopy.transform.position, Quaternion.identity, 0);
                    }

                    if (Time.time > lastBeforeClearTime)
                    {
                        RPCProtection();
                        foreach (BuilderPiece piece in GetPieces())
                        {
                            if (piece.pieceType == 1700948013)
                                piece.gameObject.SetActive(false);
                        }
                        lastBeforeClearTime = Time.time + 1f;
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

        public static void AtticCrashAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > miniaturedelay)
                {
                    miniaturedelay = Time.time + 0.022f;
                    for (int i = 0; i < 4; i++)
                    {
                        BuilderTable.instance.RequestCreatePiece(1700948013, GorillaTagger.Instance.headCollider.transform.position, Quaternion.identity, 0);
                    }
                }

                if (Time.time > lastBeforeClearTime)
                {
                    RPCProtection();
                    foreach (BuilderPiece piece in GetPieces())
                    {
                        if (piece.pieceType == 1700948013)
                            piece.gameObject.SetActive(false);
                    }
                    lastBeforeClearTime = Time.time + 1f;
                }
            }
        }

        private static float delaything = 0f;

        // Hi skids :3
        // If you take this code you like giving sloppy wet kisses to cute boys >_<
        // I gotta stop

        public static void MuteGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > delaything)
                    {
                        delaything = Time.time + 0.027f;
                        PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            TargetActors = new int[]
                            {
                                whoCopy.OwningNetPlayer.ActorNumber
                            }
                        }, SendOptions.SendReliable);

                        NetworkView view = RigManager.GetNetworkViewFromVRRig(whoCopy);
                        view.GetView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

                        PhotonNetwork.Destroy(view.GetView); // Trust
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

        public static void MuteAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > delaything)
                {
                    delaything = Time.time + 0.05f;
                    PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions
                    {
                        CachingOption = EventCaching.DoNotCache,
                        Receivers = ReceiverGroup.Others
                    }, SendOptions.SendReliable);

                    foreach (VRRig player in GorillaParent.instance.vrrigs)
                    {
                        NetworkView view = RigManager.GetNetworkViewFromVRRig(player);
                        view.GetView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

                        PhotonNetwork.Destroy(view.GetView); // Trust
                    }
                }
            }
        }

        // Huge thanks to kingofnetflix
        private static float flushDelay = 0f;
        public static void LagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (Time.time > delaything)
                    {
                        delaything = Time.time + 0.049f;
                        for (int i = 0; i < 25; i++)
                        {
                            FriendshipGroupDetection.Instance.photonView.RPC("NotifyNoPartyToMerge", NetPlayerToPlayer(GetPlayerFromVRRig(whoCopy)), new object[] { null });
                        }
                    }
                    if (Time.time > flushDelay)
                    {
                        flushDelay = Time.time + 0.5f;
                        RPCProtection();
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        RPCProtection();
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    RPCProtection();
                    isCopying = false;
                }
            }
        }

        public static void LagAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > delaything)
                {
                    delaything = Time.time + 0.049f;
                    PhotonView photonView = GameObject.Find("WorldShareableCosmetic").GetComponent<WorldShareableItem>().guard.photonView;
                    for (int i = 0; i < 25; i++)
                    {
                        FriendshipGroupDetection.Instance.photonView.RPC("NotifyNoPartyToMerge", RpcTarget.Others, new object[] { null });
                    }
                }
                if (Time.time > flushDelay)
                {
                    flushDelay = Time.time + 0.5f;
                    RPCProtection();
                }
            }
        }

        public static IEnumerator KickRig(VRRig FUCKER)
        {
            Traverse.Create(GameObject.Find("PhotonMono").GetComponent<PhotonHandler>()).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * 9999f));
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 3950; i++)
            {
                // What is this
                PhotonView photonView = GetPhotonViewFromVRRig(FUCKER);
                ExitGames.Client.Photon.Hashtable rpcHash = new ExitGames.Client.Photon.Hashtable
                {
                    { 0, photonView.ViewID },
                    { 2, (int)(PhotonNetwork.ServerTimestamp + -int.MaxValue) },
                    { 3, "RPC_RequestMaterialColor" },
                    { 4, new object[] { NetPlayerToPlayer(GetPlayerFromVRRig(FUCKER)) } },
                    { 5, (byte)91 }
                };
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash, new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    InterestGroup = photonView.Group
                }, new SendOptions
                {
                    Reliability = true,
                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                    Encrypt = false
                });
            }
            RPCProtection();
        }

        private static Coroutine KVCoroutine = null;
        private static string ihavediahrrea = "";
        public static void KickGun()
        {
            if (!PhotonNetwork.InRoom)
            {
                SetTick(1000f);
            }
            if (GetGunInput(false) || isCopying)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (!PhotonNetwork.InRoom)
                    {
                        isCopying = false;
                        whoCopy = null;
                        SetTick(1000f);
                        if (!GetIndex("Disable Kick Gun Reconnect").enabled)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You have been kicked for sending too many RPCs, you will reconnect shortly.</color>");
                            rejRoom = ihavediahrrea;
                        }
                        try { CoroutineManager.EndCoroutine(KVCoroutine); } catch { }
                    }
                    if (GetPlayerFromVRRig(whoCopy) == null)
                    {
                        isCopying = false;
                        whoCopy = null;
                        SetTick(1000f);
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Player has been kicked!</color>");
                        try { CoroutineManager.EndCoroutine(KVCoroutine); } catch { }
                    }
                }
                if (GetGunInput(true) && !isCopying && PhotonVoiceNetwork.Instance.PrimaryRecorder.IsInitialized)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                        SetTick(9999f);
                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>KICK</color><color=grey>]</color> <color=white>Player is being kicked...</color>");
                        KVCoroutine = CoroutineManager.RunCoroutine(KickRig(whoCopy));
                    }
                }
            } else
            {
                if (isCopying)
                {
                    isCopying = false;
                    SetTick(1000f);
                    try { CoroutineManager.EndCoroutine(KVCoroutine); } catch { }
                }
            }
        }

        public static void DisableKickGun()
        {
            SetTick(1000f);
            if (isCopying)
            {
                isCopying = false;
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>Player kick has been cancelled.</color>");
            }
        }

        public static void SetTick(float tick)
        {
            Traverse.Create(GameObject.Find("PhotonMono").GetComponent<PhotonHandler>()).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * tick));
        }

        // I see you
        public static void ForceUnloadCustomMap()
        {
            delaything = Time.time + 0.1f;
            PhotonView goldentrophy = GameObject.Find("Environment Objects/LocalObjects_Prefab/VirtualStump_CustomMapLobby/ModIOMapsTerminal/NetworkObject").GetComponent<PhotonView>();

            goldentrophy.RPC("UnloadMapRPC", RpcTarget.All, new object[] { });
            RPCProtection();
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

        private static float stupiddelayihate = 0f;
        private static float stdell = 0f;
        private static VRRig thestrangled = null;
        private static VRRig thestrangledleft = null;
        public static void Strangle()
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
                        stdell = Time.time + 0.1f;
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(thestrangledleft), Vector3.Normalize(GorillaTagger.Instance.leftHandTransform.position - thestrangledleft.transform.position) * 3f);
                    }
                }
            }
            else
            {
                if (thestrangledleft != null)
                {
                    try
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(thestrangledleft), GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0));
                    }
                    catch { }
                    thestrangledleft = null;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[] {
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
                }
                else
                {
                    if (Time.time > stupiddelayihate)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(thestrangled), Vector3.Normalize(GorillaTagger.Instance.rightHandTransform.position - thestrangled.transform.position) * 2f);
                    }
                }
            }
            else
            {
                if (thestrangled != null)
                {
                    try
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(thestrangled), GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0));
                    }
                    catch { }
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
