using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion.Gameplay;
using iiMenu.Classes;
using iiMenu.Notifications;
using iiMenu.Patches;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Linq;
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
            if (NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You are master client.</color>");
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
        }

        public static void SetGuardianTarget(NetPlayer target)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
            foreach (GorillaGuardianZoneManager zoneManager in GorillaGuardianZoneManager.zoneManagers)
            {
                if (zoneManager.enabled)
                    zoneManager.SetGuardian(NetworkSystem.Instance.LocalPlayer);
            }
        }

        public static void GuardianSelf() =>
            SetGuardianTarget(PhotonNetwork.LocalPlayer);

        public static void GuardianGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        SetGuardianTarget(GetPlayerFromVRRig(gunTarget));
                        kgDebounce = Time.time + 0.1f;
                    }
                }
            }
        }

        public static void GuardianAll()
        {
            if (NetworkSystem.Instance.IsMasterClient)
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
            if (NetworkSystem.Instance.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                    {
                        if (gorillaGuardianZoneManager.CurrentGuardian == NetworkSystem.Instance.LocalPlayer)
                            gorillaGuardianZoneManager.SetGuardian(null);
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        if (NetworkSystem.Instance.IsMasterClient)
                        {
                            foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                            {
                                if (gorillaGuardianZoneManager.enabled)
                                {
                                    if (gorillaGuardianZoneManager.CurrentGuardian == GetPlayerFromVRRig(gunTarget))
                                        gorillaGuardianZoneManager.SetGuardian(null);
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
            if (NetworkSystem.Instance.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (gorillaGuardianZoneManager.enabled)
                        gorillaGuardianZoneManager.SetGuardian(null);
                }
            }
            else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        private static float guardianSpazDelay = 0f;
        private static bool guardianSpazToggle = false;
        public static void GuardianSpaz()
        {
            if (Time.time > guardianSpazDelay)
            {
                guardianSpazDelay = Time.time + 0.1f;
                guardianSpazToggle = !guardianSpazToggle;
                if (guardianSpazToggle)
                    GuardianAll();
                else
                    UnguardianAll();
            }
        }

        public static void AlwaysGuardian()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                if (!gman.IsPlayerGuardian(PhotonNetwork.LocalPlayer))
                    SetGuardianTarget(PhotonNetwork.LocalPlayer);
            }
            else
            {
                foreach (TappableGuardianIdol tgi in GetAllType<TappableGuardianIdol>())
                {
                    if (!tgi.isChangingPositions)
                    {
                        GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                        if (!gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer)) // gzm.enabled && 
                        {
                            VRRig.LocalRig.enabled = false;
                            VRRig.LocalRig.transform.position = tgi.transform.position;

                            VRRig.LocalRig.leftHand.rigTarget.transform.position = tgi.transform.position;
                            VRRig.LocalRig.rightHand.rigTarget.transform.position = tgi.transform.position;

                            tgi.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, tgi.tappableId, UnityEngine.Random.Range(0.2f, 0.4f));
                            RPCProtection();
                        }
                    }
                    else
                    {
                        VRRig.LocalRig.enabled = true;
                    }
                }
            }
        }

        private static float propHuntSpazDelay;
        private static bool propHuntSpazMode;
        public static void SpazPropHuntObjects()
        {
            if (Time.time > propHuntSpazDelay)
            {
                propHuntSpazDelay = Time.time + 0.1f;
                propHuntSpazMode = !propHuntSpazMode;

                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

                if (PhotonNetwork.InRoom && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
                {
                    GorillaPropHuntGameManager hauntManager = (GorillaPropHuntGameManager)GorillaGameManager.instance;
                    hauntManager._ph_timeRoundStartedMillis = propHuntSpazMode ? 1 : 2;
                    hauntManager._ph_randomSeed = UnityEngine.Random.Range(1, int.MaxValue);
                }
            }
        }

        public static void SpazPropHunt()
        {
            if (Time.time > propHuntSpazDelay)
            {
                propHuntSpazDelay = Time.time + 0.1f;
                propHuntSpazMode = !propHuntSpazMode;

                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

                if (PhotonNetwork.InRoom && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
                {
                    GorillaPropHuntGameManager hauntManager = (GorillaPropHuntGameManager)GorillaGameManager.instance;
                    hauntManager._ph_timeRoundStartedMillis = propHuntSpazMode ? 0 : 1;
                }
            }
        }

        public static void CreateItem(object target, int hash, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, long[] sendData = null)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            int netId = GhostReactorManager.instance.gameEntityManager.CreateNetId();

            if (target is NetPlayer)
                target = NetPlayerToPlayer((NetPlayer)target);

            sendData ??= new long[] { 0L };

            object[] createData = new object[]
            {
                new int[] { netId },
                new int[] { (int)GTZone.ghostReactor },
                new int[] { hash },
                new long[] { BitPackUtils.PackWorldPosForNetwork(position) },
                new int[] { BitPackUtils.PackQuaternionForNetwork(rotation) },
                sendData
            };

            if (target is RpcTarget)
                GhostReactorManager.instance.gameEntityManager.photonView.RPC("CreateItemRPC", (RpcTarget)target, createData);

            if (target is Player)
                GhostReactorManager.instance.gameEntityManager.photonView.RPC("CreateItemRPC", (Player)target, createData);

            if (velocity != Vector3.zero || angVelocity != Vector3.zero)
            {
                bool handTarget = GamePlayerLocal.instance.gamePlayer.GetGameEntityId(GamePlayer.GetHandIndex(false)) != null;
                velocity = velocity.ClampMagnitudeSafe(1600f);

                object[] grabData = new object[]
                {
                    netId,
                    handTarget,
                    BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity),
                    PhotonNetwork.LocalPlayer
                };

                if (target is RpcTarget)
                    GhostReactorManager.instance.gameEntityManager.photonView.RPC("GrabEntityRPC", (RpcTarget)target, grabData);

                if (target is Player)
                    GhostReactorManager.instance.gameEntityManager.photonView.RPC("GrabEntityRPC", (Player)target, grabData);
                
                object[] dropData = new object[]
                {
                    netId,
                    handTarget,
                    position,
                    rotation,
                    velocity,
                    angVelocity,
                    PhotonNetwork.LocalPlayer,
                    PhotonNetwork.Time
                };

                if (target is RpcTarget)
                    GhostReactorManager.instance.gameEntityManager.photonView.RPC("ThrowEntityRPC", (RpcTarget)target, dropData);

                if (target is Player)
                    GhostReactorManager.instance.gameEntityManager.photonView.RPC("ThrowEntityRPC", (Player)target, dropData);
            }

            RPCProtection();
        }

        public static void SpamObjectGun(int objectId)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                    CreateItem(RpcTarget.All, objectId, NewPointer.transform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)), Vector3.zero, Vector3.zero);
            }
        }

        private static float minigunDelay;
        public static void SpamObjectMinigun(int objectId)
        {
            if (rightGrab)
            {
                Safety.NoFinger();
                if (Time.time > minigunDelay)
                {
                    minigunDelay = Time.time + 0.02f;
                    CreateItem(RpcTarget.All, objectId, GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)), GorillaTagger.Instance.rightHandTransform.forward * ShootStrength * 5f, Vector3.zero);
                }
            }
        }

        public static void ToolSpamGun()
        {
            int[] objectIds = new int[]
            {
                225241881,
                1115277044,
                1165678479,
                1989693521,
                -175001459
            };

            SpamObjectGun(UnityEngine.Random.Range(0, objectIds.Length - 1));
        }

        private static float destroyDelay;
        public static void DestroyEntityGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (Time.time > destroyDelay)
                    {
                        destroyDelay = Time.time + 0.02f;
                        if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

                        GameEntity gameEntity = null;
                        float closestDist = float.MaxValue;

                        foreach (GameEntity entity in GhostReactorManager.instance.gameEntityManager.entities)
                        {
                            if (entity != null)
                            {
                                float distance = Vector3.Distance(NewPointer.transform.position, entity.transform.position);
                                if (distance < 0.75f && distance < closestDist)
                                {
                                    gameEntity = entity;
                                    closestDist = distance;
                                }
                            }
                        }

                        if (gameEntity != null)
                        {
                            GhostReactorManager.instance.gameEntityManager.photonView.RPC("DestroyItemRPC", RpcTarget.All, new object[] { new int[] { gameEntity.GetNetId() } });
                            RPCProtection();
                        }
                    }
                }
            }
        }

        public static void ActiveNetworkHandlerRPC(string rpc, Player target, object[] args)
        {
            GameMode.activeNetworkHandler.netView.GetView.RPC(rpc, target, args);
            RPCProtection();
        }
        public static void ActiveNetworkHandlerRPC(string rpc, RpcTarget target, object[] args)
        {
            GameMode.activeNetworkHandler.netView.GetView.RPC(rpc, target, args);
            RPCProtection();
        }

        public static void BetaSetVelocityPlayer(NetPlayer victim, Vector3 velocity)
        {
            if (velocity.sqrMagnitude > 20f)
                velocity = Vector3.Normalize(velocity) * 20f;

            GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                ActiveNetworkHandlerRPC("GuardianLaunchPlayer", NetPlayerToPlayer(victim), new object[] { velocity });
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
        }
        public static void BetaSetVelocityTargetGroup(RpcTarget victim, Vector3 velocity)
        {
            if (velocity.sqrMagnitude > 20f)
                velocity = Vector3.Normalize(velocity) * 20f;

            GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                ActiveNetworkHandlerRPC("GuardianLaunchPlayer", victim, new object[] { velocity });
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");

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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GetNetworkViewFromVRRig(gunTarget).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
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
                        if (plr != VRRig.LocalRig)
                        {
                            GetNetworkViewFromVRRig(plr).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        GorillaGuardianManager gman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager").GetComponent<GorillaGuardianManager>();
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GetNetworkViewFromVRRig(gunTarget).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 0f, 0f) });
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
                        if (plr != VRRig.LocalRig)
                        {
                            GetNetworkViewFromVRRig(plr).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 0f, 0f) });
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(gunTarget), new Vector3(0f, 19.9f, 0f) );
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

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > kgDebounce)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)));
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
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

        private static float delaything = 0f;
        public static void GuardianBlindGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > delaything)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        delaything = Time.time + 0.1f;
                        GetNetworkViewFromVRRig(gunTarget).SendRPC("GrabbedByPlayer", GetPlayerFromVRRig(gunTarget), new object[] { true, false, false });
                        GetNetworkViewFromVRRig(gunTarget).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(gunTarget), new object[] { new Vector3(0f, float.NaN, 0f) });
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
                        GetNetworkViewFromVRRig(player).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        GetNetworkViewFromVRRig(player).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, float.NaN, 0f) });
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        delaything = Time.time + 0.1f;
                        GetNetworkViewFromVRRig(gunTarget).SendRPC("GrabbedByPlayer", GetPlayerFromVRRig(gunTarget), new object[] { true, false, false });
                        GetNetworkViewFromVRRig(gunTarget).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(gunTarget), new object[] { new Vector3(0f, float.MinValue, 0f) });
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
                        GetNetworkViewFromVRRig(player).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        GetNetworkViewFromVRRig(player).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, float.MinValue, 0f) });
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
                        Fun.RequestCreatePiece(691844031, new Vector3(-127.6248f, 16.99441f, -217.2094f), Quaternion.identity, 0);
                    }
                }

                if (Time.time > lastBeforeClearTime)
                {
                    RPCProtection();
                    foreach (BuilderPiece piece in GetAllType<BuilderPiece>())
                    {
                        if (piece.pieceType == 691844031)
                            piece.gameObject.SetActive(false);
                    }
                    lastBeforeClearTime = Time.time + 1f;
                }
            }
        }

        public static Coroutine DisableCoroutine;
        public static IEnumerator DisableSnowball(bool rigDisabled)
        {
            yield return new WaitForSeconds(0.3f);

            if (rigDisabled)
                VRRig.LocalRig.enabled = true;
            DistancePatch.enabled = false;

            GetProjectile("GrowingSnowballLeftAnchor").SetSnowballActiveLocal(false);
            GetProjectile("GrowingSnowballRightAnchor").SetSnowballActiveLocal(false);
        }

        private static int archiveIncrement;
        public static int GetProjectileIncrement(Vector3 Position, Vector3 Velocity, float Scale)
        {
            try
            {
                GameObject SlingshotProjectileGameObject = new GameObject("SlingshotProjectileHolder");
                SlingshotProjectile SlingshotProjectile = SlingshotProjectileGameObject.AddComponent<SlingshotProjectile>();

                ProjectileTracker.ProjectileInfo LocalProjectileInfo = new ProjectileTracker.ProjectileInfo(PhotonNetwork.Time, Velocity, Position, Scale, SlingshotProjectile);

                int Data = ProjectileTracker.m_localProjectiles.AddAndIncrement(LocalProjectileInfo);
                archiveIncrement = Data;

                UnityEngine.Object.Destroy(SlingshotProjectileGameObject);
                return Data;
            } catch
            {
                LogManager.Log("Falling back to archiveIncrement");

                archiveIncrement++;
                return archiveIncrement;
            }
        }

        private static float timeSinceCallInvalidated;
        public static void DisableSnowballImpactEffect()
        {
            if (PhotonNetwork.InRoom && Time.time < snowballDelay + 0.1f && Time.time > timeSinceCallInvalidated)
            {
                timeSinceCallInvalidated = Time.time + 1f;
                
                for (int i = 0; i < 11; i++)
                {
                    object[] playerEffectData = new object[6];
                    playerEffectData[0] = -1;
                    playerEffectData[1] = -1;

                    object[] sendEventData = new object[3];
                    sendEventData[0] = NetworkSystem.Instance.ServerTimestamp - (11 - i);
                    sendEventData[1] = (byte)6;
                    sendEventData[2] = playerEffectData;

                    PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
                }
                RPCProtection();
            }
        }

        public static float snowballSpawnDelay = 0.15f;
        public static bool SnowballHandIndex;
        public static void BetaSpawnSnowball(Vector3 Pos, Vector3 Vel, float Scale, int Mode, Player Target = null, bool NetworkSize = true, int customNetworkedSize = -1)
        {
            try
            {
                SnowballHandIndex = !SnowballHandIndex;
                Vel = Vel.ClampMagnitudeSafe(50f);

                bool isTooFar = Vector3.Distance(Pos, GorillaTagger.Instance.bodyCollider.transform.position) > 3.9f;
                if (isTooFar)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = Pos + new Vector3(0f, Vel.y > 0f ? -3f : 3f, 0f);
                }

                DistancePatch.enabled = true;

                if (DisableCoroutine != null)
                    CoroutineManager.EndCoroutine(DisableCoroutine);

                DisableCoroutine = CoroutineManager.RunCoroutine(DisableSnowball(isTooFar));

                GrowingSnowballThrowable GrowingSnowball = GetProjectile($"GrowingSnowball{(SnowballHandIndex ? "Right" : "Left")}Anchor") as GrowingSnowballThrowable;
                GrowingSnowball.SetSnowballActiveLocal(true);

                switch (Mode)
                {
                    case 0:
                        GrowingSnowball.changeSizeEvent.RaiseAll(customNetworkedSize > 0 ? customNetworkedSize : (int)Scale);
                        GrowingSnowball.snowballThrowEvent.RaiseAll(Pos, Vel, GetProjectileIncrement(Pos, Vel, Scale));
                        break;
                    case 1:
                        GrowingSnowball.changeSizeEvent.RaiseOthers(customNetworkedSize > 0 ? customNetworkedSize : (int)Scale);
                        GrowingSnowball.snowballThrowEvent.RaiseOthers(Pos, Vel, GetProjectileIncrement(Pos, Vel, Scale));
                        break;
                    case 2:
                        PhotonNetwork.RaiseEvent(176, new object[]
                        {
                            GrowingSnowball.changeSizeEvent._eventId,
                            customNetworkedSize > 0 ? customNetworkedSize : (int)Scale
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { Target.ActorNumber }
                        }, new SendOptions
                        {
                            Reliability = false,
                            Encrypt = true
                        });

                        PhotonNetwork.RaiseEvent(176, new object[]
                        {
                            GrowingSnowball.snowballThrowEvent._eventId,
                            Pos,
                            Vel,
                            GetProjectileIncrement(Pos, Vel, Scale)
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
            } catch { }
            RPCProtection();
        }

        public static void BetaSnowballImpact(Player Target)
        {
            object[] playerEffectData = new object[6];
            playerEffectData[0] = Target.ActorNumber;
            playerEffectData[1] = 0;

            object[] sendEventData = new object[3];
            sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
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
                    snowballDelay = Time.time + snowballSpawnDelay;
                }
            }
        }

        public static void SnowballRain()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(VRRig.LocalRig.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f)), Vector3.zero, 1f, 0);
                    snowballDelay = Time.time + snowballSpawnDelay;
                }
            }
        }

        public static void SnowballHail()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(VRRig.LocalRig.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f)), new Vector3(0f, -50f, 0f), 3f, 0);
                    snowballDelay = Time.time + snowballSpawnDelay;
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
                    snowballDelay = Time.time + snowballSpawnDelay;
                }
            }
        }

        public static void SnowballGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > snowballDelay)
                {
                    BetaSpawnSnowball(NewPointer.transform.position + new Vector3(0f, 1f, 0f), new Vector3(0f, 30f, 0f), 10f, 0);
                    snowballDelay = Time.time + snowballSpawnDelay;
                }
            }
        }

        public static void SnowballMinigun()
        {
            if ((rightGrab || Mouse.current.leftButton.isPressed) && Time.time > snowballDelay)
            {
                Vector3 velocity = GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength * 5f;
                if (Mouse.current.leftButton.isPressed)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                    velocity = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                    velocity.Normalize();
                    velocity *= ShootStrength * 2f;
                }

                BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, velocity, 5f, 0);
                snowballDelay = Time.time + snowballSpawnDelay;
            }
        }

        public static void GiveSnowballMinigun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null && Time.time > snowballDelay)
                {
                    Vector3 velocity = lockTarget.rightHandTransform.transform.forward * ShootStrength * 5f;
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                        velocity = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        velocity.Normalize();
                        velocity *= ShootStrength * 2f;
                    }

                    BetaSpawnSnowball(lockTarget.rightHandTransform.transform.position, velocity, 5f, 0);
                    snowballDelay = Time.time + snowballSpawnDelay;
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
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
                    snowballDelay = Time.time + snowballSpawnDelay;
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        snowballDelay = Time.time + snowballSpawnDelay;
                        BetaSnowballImpact(NetPlayerToPlayer(GetPlayerFromVRRig(gunTarget)));
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
                    if (rig != VRRig.LocalRig && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f))
                    {
                        Vector3 targetDirection = GorillaTagger.Instance.headCollider.transform.position - rig.headMesh.transform.position;
                        BetaSpawnSnowball(GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(rig)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
            }
        }

        public static void SnowballSafetyBubble()
        {
            if (Time.time > snowballDelay)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (rig != VRRig.LocalRig)
                    {
                        if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, rig.transform.position) < 3f)
                        {
                            Vector3 targetDirection = GorillaTagger.Instance.headCollider.transform.position - rig.headMesh.transform.position;
                            BetaSpawnSnowball(GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(rig)));
                            snowballDelay = Time.time + snowballSpawnDelay;
                            if (PhotonNetwork.InRoom)
                            {
                                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                    248,
                                    false,
                                    999999f
                                });
                            }
                            else
                                VRRig.LocalRig.PlayHandTapLocal(248, false, 999999f);
                        }
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

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        BetaSpawnSnowball(lockTarget.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
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
                snowballDelay = Time.time + snowballSpawnDelay;
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

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        BetaSpawnSnowball(lockTarget.headMesh.transform.position + new Vector3(0f, -0.7f, 0f), new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void SnowballFlingVerticalAll()
        {
            if (rightTrigger > 0.5f && Time.time > snowballDelay)
            {
                snowballDelay = Time.time + snowballSpawnDelay;
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
                    snowballDelay = Time.time + snowballSpawnDelay;
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
                    snowballDelay = Time.time + snowballSpawnDelay;
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

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        Vector3 targetDirection = (lockTarget.headMesh.transform.position - GorillaTagger.Instance.headCollider.transform.position).normalized;
                        BetaSpawnSnowball(lockTarget.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z) * 1.5f, new Vector3(0f, -100f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void SnowballFlingPlayerAwayGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        Vector3 targetDirection = (GorillaTagger.Instance.headCollider.transform.position - lockTarget.headMesh.transform.position).normalized;
                        BetaSpawnSnowball(lockTarget.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z) * 1.5f, new Vector3(0f, -100f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void SnowballPushGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        Vector3 targetDirection = GorillaTagger.Instance.headCollider.transform.position - lockTarget.headMesh.transform.position;
                        BetaSpawnSnowball(GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(targetDirection.x, 0f, targetDirection.z).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void SnowballStrongFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > snowballDelay)
                    {
                        BetaSpawnSnowball(new Vector3(GorillaTagger.Instance.headCollider.transform.position.x, 1000f, GorillaTagger.Instance.headCollider.transform.position.z), new Vector3(0f, -9999f, 0f), 9999f, 2, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
                        snowballDelay = Time.time + snowballSpawnDelay;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void SnowballStrongFlingAll()
        {
            if (rightTrigger > 0.5f && Time.time > snowballDelay)
            {
                snowballDelay = Time.time + snowballSpawnDelay;
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
                                VisualizeAura(report.position, Safety.threshold, Color.red);
                            
                            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                            {
                                if (vrrig != VRRig.LocalRig)
                                {
                                    float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                    float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                    if (D1 < Safety.threshold || D2 < Safety.threshold)
                                    {
                                        if (!Safety.smartarp || (Safety.smartarp && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                        {
                                            snowballDelay = Time.time + snowballSpawnDelay;
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

        public static bool SpecialTargetRPC(PhotonView photonView, string method, int[] targetActors, object[] parameters)
        {
            if (photonView != null && parameters != null && !string.IsNullOrEmpty(method))
            {
                ExitGames.Client.Photon.Hashtable rpcData = new ExitGames.Client.Photon.Hashtable
                {
                    { 0, photonView.ViewID },
                    { 2, (int)(PhotonNetwork.ServerTimestamp + -int.MaxValue) },
                    { 3, method },
                    { 4, parameters }
                };

                if (photonView.Prefix > 0)
                {
                    rpcData[1] = (short)photonView.Prefix;
                }
                if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(method))
                {
                    rpcData[5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(method);
                }
                if (targetActors.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
                {
                    PhotonNetwork.ExecuteRpc(rpcData, PhotonNetwork.LocalPlayer);
                }
                else
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcData, new RaiseEventOptions
                    {
                        TargetActors = targetActors
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

        private static float delay;
        private static bool returnOrTeleport;
        public static void ArcadeTeleporterEffectSpammer()
        {
            if (rightTrigger > 0.5f && Time.time > delay)
            {
                PhotonView that = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/NetObject_VRTeleporter").GetComponent<Photon.Pun.PhotonView>();
                delay = Time.time + 0.05f;
                returnOrTeleport = !returnOrTeleport;
                that.RPC(returnOrTeleport ? "ActivateTeleportVFX" : "ActivateReturnVFX", RpcTarget.All, new object[] { (short)UnityEngine.Random.Range(0, 7) });
                RPCProtection();
            }
        }

        public static void StumpTeleporterEffectSpammer()
        {
            if (rightTrigger > 0.5f && Time.time > delay)
            {
                PhotonView that = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/StumpVRHeadset/ModIOArcadeTeleporter (1)/NetObject_VRTeleporter").GetComponent<Photon.Pun.PhotonView>();
                delay = Time.time + 0.05f;
                returnOrTeleport = !returnOrTeleport;
                that.RPC(returnOrTeleport ? "ActivateTeleportVFX" : "ActivateReturnVFX", RpcTarget.All, new object[] { (short)0 });
                RPCProtection();
            }
        }

        public static void PhysicalFreezeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > kgDebounce)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), Vector3.zero);
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
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

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > kgDebounce)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), (GorillaTagger.Instance.bodyCollider.transform.position - lockTarget.transform.position) * 2f);
                        RPCProtection();
                        kgDebounce = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void BringAll()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.2f;
                foreach (VRRig plr in GorillaParent.instance.vrrigs)
                {
                    if (plr != VRRig.LocalRig)
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

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > thingdeb)
                    {
                        if (lockTarget.rightThumb.calcT > 0.5f)
                        {
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), lockTarget.headMesh.transform.forward * 15f);
                            RPCProtection();
                        }
                        thingdeb = Time.time + 0.1f;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
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
                    if (plr != VRRig.LocalRig)
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
                            if (plr != VRRig.LocalRig)
                                BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), Vector3.Normalize(NewPointer.transform.position - plr.transform.position) * 50f);
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
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                        
                        slamDel = Time.time + 0.05f;
                    }
                    
                }
            }
        }

        public static void SpawnSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
        }

        public static void AngerSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated)
                secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemotePlayerSeen", RpcTarget.All, new object[] { });
        }

        public static void ThrowSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated)
                secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            
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
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
                
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Activated || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Patrolling)
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemotePlayerSeen", RpcTarget.All, new object[] { });
                
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Chasing)
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().GetView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            }
        }

        private static bool previousMuteValue;
        private static float freezeAllDelay;
        public static void FreezeAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > freezeAllDelay)
                {
                    previousMuteValue = !previousMuteValue;
                    foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        GorillaPlayerScoreboardLine.MutePlayer(line.linePlayer.UserId, line.linePlayer.NickName, previousMuteValue ? 1 : 0);

                    freezeAllDelay = Time.time + 0.1f;
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        NetPlayer player = GetPlayerFromVRRig(gunTarget);
                        PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void DestroyAll()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
        }

        public static void TargetSpam()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                foreach (HitTargetNetworkState hitTargetNetworkState in Resources.FindObjectsOfTypeAll<HitTargetNetworkState>())
                {
                    hitTargetNetworkState.hitCooldownTime = 0;
                    hitTargetNetworkState.TargetHit(Vector3.zero, Vector3.zero);
                }
            } else { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); }
        }

        public static void InfectionToTag()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
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
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
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

        public static void BetaSetStatus(int state, RaiseEventOptions reo)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                object[] statusSendData = new object[1];
                statusSendData[0] = state;
                object[] sendEventData = new object[3];
                sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
                sendEventData[1] = (byte)2;
                sendEventData[2] = statusSendData;
                PhotonNetwork.RaiseEvent(3, sendEventData, reo, SendOptions.SendUnreliable);
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        NetPlayer player = GetPlayerFromVRRig(gunTarget);
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        NetPlayer owner = GetPlayerFromVRRig(gunTarget);
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }

                if (gunLocked)
                {
                    foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
                    {
                        if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            glider.gameObject.transform.position = lockTarget.headMesh.transform.position;
                            glider.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                        }
                        else
                            glider.OnHover(null, null);
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void GliderBlindAll()
        {
            GliderHoldable[] those = GetAllType<GliderHoldable>();
            int index = 0;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != VRRig.LocalRig)
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
                            glider.OnHover(null, null);
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

                if (gunLocked && lockTarget != null)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", GetPlayerFromVRRig(lockTarget), new object[]{
                        111,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
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

        public static Coroutine RopeCoroutine;
        public static IEnumerator RopeEnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }

        public static void BetaSetRopeVelocity(int RopeId, Vector3 Velocity)
        {
            Velocity = Velocity.ClampMagnitudeSafe(100f);

            if (RopeSwingManager.instance.ropes.TryGetValue(RopeId, out GorillaRopeSwing Rope))
            {
                var ClosestNode = Rope.nodes
                    .Skip(1)
                    .Select((v, i) => new { index = i, 
                                            transform = v,
                                            distance = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, v.transform.position) 
                                          })
                    .OrderBy(x => x.distance)
                    .First();

                if (ClosestNode.distance > 5f)
                {
                    if (RopeCoroutine != null)
                        CoroutineManager.instance.StopCoroutine(RopeCoroutine);

                    RopeCoroutine = CoroutineManager.instance.StartCoroutine(RopeEnableRig());

                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = ClosestNode.transform.position;
                }

                if (Vector3.Distance(ServerPos, ClosestNode.transform.position) < 5f)
                    RopeSwingManager.instance.SendSetVelocity_RPC(RopeId, ClosestNode.index, Velocity.ClampMagnitudeSafe(100f), true);
                else
                    RopeDelay = 0f;

                RPCProtection();
            }
        }

        public static void BetaSetRopeVelocity(GorillaRopeSwing Rope, Vector3 Velocity) =>
            BetaSetRopeVelocity(RopeSwingManager.instance.ropes.FirstOrDefault(x => x.Value == Rope).Key, Velocity);

        private static float randomRopeDelay;
        private static GorillaRopeSwing randomRope;
        public static GorillaRopeSwing GetRandomRope()
        {
            if (Time.time > randomRopeDelay)
            {
                randomRopeDelay = Time.time + 0.5f;
                randomRope = RopeSwingManager.instance.ropes.Values.OrderBy(_ => UnityEngine.Random.value).FirstOrDefault();
            }
            return randomRope;
        }

        private static float RopeDelay = 0f;
        public static void JoystickRopeControl() // Thanks to ShibaGT for the fix
        {
            if ((Mathf.Abs(rightJoystick.x) > 0.05f || Mathf.Abs(rightJoystick.y) > 0.05f) && Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.125f;

                GorillaRopeSwing rope = GetRandomType<GorillaRopeSwing>(0.25f);
                BetaSetRopeVelocity(rope, new Vector3(rightJoystick.x * 100f, rightJoystick.y * 100f, 0f));
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
                    GorillaRopeSwing gunTarget = Ray.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (gunTarget && Time.time > RopeDelay)
                    {
                        RopeDelay = Time.time + 0.25f;
                        BetaSetRopeVelocity(gunTarget, RandomVector3(100f));
                    }
                }
            }
        }

        public static void SpazAllRopes()
        {
            if (rightTrigger > 0.5f && Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.125f;

                GorillaRopeSwing rope = GetRandomRope();
                BetaSetRopeVelocity(rope, RandomVector3(100f));
            }
        }

        public static void SpazGrabbedRopes()
        {
            if (Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.125f;
                VRRig randomRig = GorillaParent.instance.vrrigs
                    .Where(rig => rig.currentRopeSwing != null)
                    .OrderBy(_ => UnityEngine.Random.value)
                    .FirstOrDefault();

                if (randomRig != null)
                    BetaSetRopeVelocity(randomRig.currentRopeSwing, RandomVector3(100f));
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
                    GorillaRopeSwing gunTarget = Ray.collider.GetComponentInParent<GorillaRopeSwing>();

                    if (gunTarget && Time.time > RopeDelay)
                    {
                        RopeDelay = Time.time + 0.125f;
                        BetaSetRopeVelocity(gunTarget, RandomVector3(100f));
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
                    RopeDelay = Time.time + 0.125f;

                    GorillaRopeSwing rope = GetRandomRope();
                    BetaSetRopeVelocity(rope, (NewPointer.transform.position - rope.transform.position).normalized * 100f);
                }
            }
        }

        public static float notifdelay;
        public static void ShockwaveSpam()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (rightGrab)
                {
                    CrittersPawn[] critters = GetAllType<CrittersPawn>();
                    if (critters.Length > 0)
                    {
                        CrittersPawn critter = critters[0];
                        critter.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
                        int actorId = critter.actorId;
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                    }
                }
            }
            else
            {
                if (Time.time > notifdelay)
                {
                    notifdelay = Time.time + 0.1f;
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                }
            }
        }

        public static void StickySpam()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (rightGrab)
                {
                    CrittersPawn[] critters = GetAllType<CrittersPawn>();
                    if (critters.Length > 0)
                    {
                        CrittersPawn critter = critters[0];
                        critter.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
                        int actorId = critter.actorId;
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                    }
                }
            }
            else
            {
                if (Time.time > notifdelay)
                {
                    notifdelay = Time.time + 0.1f;
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                }
            }
        }

        public static void DustSpam()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (rightGrab)
                {
                    CrittersPawn[] critters = GetAllType<CrittersPawn>();
                    if (critters.Length > 0)
                    {
                        CrittersPawn critter = critters[0];
                        critter.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
                        int actorId = critter.actorId;
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                    }
                }
            }
            else
            {
                if (Time.time > notifdelay)
                {
                    notifdelay = Time.time + 0.1f;
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                }
            }
        }

        public static void NoiseSpam()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (rightGrab)
                {
                    CrittersPawn[] critters = GetAllType<CrittersPawn>();
                    if (critters.Length > 0)
                    {
                        CrittersPawn critter = critters[0];
                        critter.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
                        int actorId = critter.actorId;
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                    }
                }
            }
            else
            {
                if (Time.time > notifdelay)
                {
                    notifdelay = Time.time + 0.1f;
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                }
            }
        }

        public static void AllSpam()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (rightGrab)
                {
                    CrittersPawn[] critters = GetAllType<CrittersPawn>();
                    if (critters.Length > 0)
                    {
                        CrittersPawn critter = critters[0];
                        critter.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
                        int actorId = critter.actorId;
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                    }
                }
            }
            else
            {
                if (Time.time > notifdelay)
                {
                    notifdelay = Time.time + 0.1f;
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                }
            }
        }

        public static void ShockwaveGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        CrittersPawn[] critters = GetAllType<CrittersPawn>();
                        if (critters.Length > 0)
                        {
                            CrittersPawn critter = critters[0];
                            critter.transform.position = NewPointer.transform.position;
                            int actorId = critter.actorId;
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        }
                    }
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void StickyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        CrittersPawn[] critters = GetAllType<CrittersPawn>();
                        if (critters.Length > 0)
                        {
                            CrittersPawn critter = critters[0];
                            critter.transform.position = NewPointer.transform.position;
                            int actorId = critter.actorId;
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        }
                    }
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void DustGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        CrittersPawn[] critters = GetAllType<CrittersPawn>();
                        if (critters.Length > 0)
                        {
                            CrittersPawn critter = critters[0];
                            critter.transform.position = NewPointer.transform.position;
                            int actorId = critter.actorId;
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        }
                    }
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void NoiseGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        CrittersPawn[] critters = GetAllType<CrittersPawn>();
                        if (critters.Length > 0)
                        {
                            CrittersPawn critter = critters[0];
                            critter.transform.position = NewPointer.transform.position;
                            int actorId = critter.actorId;
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        }
                    }
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void AllSpamGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        CrittersPawn[] critters = GetAllType<CrittersPawn>();
                        if (critters.Length > 0)
                        {
                            CrittersPawn critter = critters[0];
                            critter.transform.position = NewPointer.transform.position;
                            int actorId = critter.actorId;
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                            CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
                        }
                    }
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void CritterSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    CrittersPawn Critter = CrittersManager.instance.SpawnCritter(UnityEngine.Random.Range(0, 3), GorillaTagger.Instance.rightHandTransform.transform.position, Quaternion.identity);
                    Critter.SetVelocity(GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength);
                }
                else
                {
                    if (Time.time > notifdelay)
                    {
                        notifdelay = Time.time + 0.1f;
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
        }

        public static void CritterGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        CrittersManager.instance.SpawnCritter(UnityEngine.Random.Range(0, 3), NewPointer.transform.position, Quaternion.identity);
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void ObjectSpam(CrittersActor.CrittersActorType type)
        {
            if (rightGrab)
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    CrittersActor Object = CrittersManager.instance.SpawnActor(type);
                    Object.MoveActor(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation);

                    if (Object.usesRB)
                        Object.SetImpulseVelocity(GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength, RandomVector3(100f));
                }
                else
                {
                    if (Time.time > notifdelay)
                    {
                        notifdelay = Time.time + 0.1f;
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
        }

        public static void ObjectGun(CrittersActor.CrittersActorType type)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        CrittersActor Object = CrittersManager.instance.SpawnActor(type);
                        Object.MoveActor(NewPointer.transform.position, RandomQuaternion());
                    }
                    else
                    {
                        if (Time.time > notifdelay)
                        {
                            notifdelay = Time.time + 0.1f;
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }
    }
}
