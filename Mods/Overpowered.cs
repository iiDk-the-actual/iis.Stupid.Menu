using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaTagScripts;
using iiMenu.Classes;
using iiMenu.Notifications;
using iiMenu.Patches;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
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
            GorillaGuardianManager guardianManager = (GorillaGuardianManager)GorillaGameManager.instance;
            if (guardianManager.IsPlayerGuardian(target))
                return;
            
            foreach (TappableGuardianIdol tgi in GetAllType<TappableGuardianIdol>())
            {
                if (tgi.manager && tgi.manager.photonView && !tgi.isChangingPositions)
                {
                    GorillaGuardianZoneManager zoneManager = tgi.zoneManager;
                    if (zoneManager.IsZoneValid() && tgi.manager && zoneManager.CurrentGuardian == null)
                    {
                        zoneManager.SetGuardian(target);
                        return;
                    }
                }
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
                    if (gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid())
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
                    if (gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid())
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
                                if (gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid())
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
                    if (gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid())
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

        public static float alwaysGuardianDelay;
        public static void AlwaysGuardian()
        {
            if (PhotonNetwork.InRoom)
            {
                if (NetworkSystem.Instance.IsMasterClient)
                {
                    if (!VRRig.LocalRig.enabled)
                        VRRig.LocalRig.enabled = true;
                    GorillaGuardianManager guardianManager = (GorillaGuardianManager)GorillaGameManager.instance;
                    if (!guardianManager.IsPlayerGuardian(PhotonNetwork.LocalPlayer))
                        SetGuardianTarget(PhotonNetwork.LocalPlayer);
                }
                else
                {
                    GorillaGuardianManager guardianManager = (GorillaGuardianManager)GorillaGameManager.instance;
                    foreach (TappableGuardianIdol tgi in GetAllType<TappableGuardianIdol>())
                    {
                        if (tgi.manager && tgi.manager.photonView && !tgi.isChangingPositions)
                        {
                            GorillaGuardianZoneManager zoneManager = tgi.zoneManager;
                            if (!guardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer) && zoneManager.IsZoneValid() && tgi.manager)
                            {
                                VRRig.LocalRig.enabled = false;
                                VRRig.LocalRig.transform.position = tgi.transform.position + RandomVector3(0.1f);
                                VRRig.LocalRig.leftHand.rigTarget.transform.position = tgi.transform.position;
                                VRRig.LocalRig.rightHand.rigTarget.transform.position = tgi.transform.position;

                                if (Time.time > alwaysGuardianDelay)
                                {
                                    alwaysGuardianDelay = Time.time + (zoneManager._currentActivationTime >= zoneManager.requiredActivationTime - 1f ? 0f : 0.2f);
                                    tgi.OnTap(UnityEngine.Random.Range(0f, 1f));
                                    RPCProtection();
                                }
                            }
                        }
                        else
                            VRRig.LocalRig.enabled = true;
                    }
                }
            }
        }

        public static float guardianProtectorDelay;
        public static void GuardianProtector()
        {
            if (PhotonNetwork.InRoom)
            {
                GorillaGuardianManager manager = (GorillaGuardianManager)GorillaGameManager.instance;

                if (manager.IsPlayerGuardian(PhotonNetwork.LocalPlayer))
                {
                    foreach (TappableGuardianIdol tgi in GetAllType<TappableGuardianIdol>())
                    {
                        if (tgi.manager && tgi.manager.photonView)
                        {
                            foreach (VRRig rig in GorillaParent.instance.vrrigs)
                            {
                                if (!rig.isLocal && Vector3.Distance(rig.transform.position, tgi.transform.position) < 2f && Time.time > guardianProtectorDelay)
                                {
                                    BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), (rig.transform.position - tgi.transform.position).normalized * 50f);
                                    guardianProtectorDelay = Time.time + 0.1f;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void KickPlayer(NetPlayer target)
        {
            if (Time.time > crashAllDelay)
            {
                crashAllDelay = Time.time + 0.1f;
                VRRig rig = GetVRRigFromPlayer(target);
                BetaSetVelocityPlayer(target, rig.transform.position.z < -28.5f ? (new Vector3(-47.82025f, 6.460508f, -29.04836f) - rig.transform.position).normalized * 50f : (rig.transform.position.z < -23f ? new Vector3(-50f, 0f, 50f) : Vector3.left * 50f));
                RPCProtection();
            }
        }

        private static float crashAllDelay;
        public static void GuardianKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > crashAllDelay)
                    {
                        crashAllDelay = Time.time + 0.1f;
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), lockTarget.transform.position.z < -28.5f ? (new Vector3(-47.82025f, 6.460508f, -29.04836f) - lockTarget.transform.position).normalized * 50f : (lockTarget.transform.position.z < -23f ? new Vector3(-50f, 0f, 50f) : Vector3.left * 50f));
                        RPCProtection();
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
        public static void GuardianKickAll()
        {
            if (rightTrigger > 0.5f && Time.time > crashAllDelay)
            {
                crashAllDelay = Time.time + 0.1f;
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!rig.isLocal)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), rig.transform.position.z < -28.5f ? (new Vector3(-47.82025f, 6.460508f, -29.04836f) - rig.transform.position).normalized * 50f : (rig.transform.position.z < -23f ? new Vector3(-50f, 0f, 50f) : Vector3.left * 50f));
                        RPCProtection();
                    }
                }
            }
        }

        public static void CrashPlayer(NetPlayer target)
        {
            VRRig rig = GetVRRigFromPlayer(target);
            if (Time.time > crashAllDelay && rig.transform.position.x < -5)
            {
                crashAllDelay = Time.time + 0.1f;
                
                BetaSetVelocityPlayer(target, (rig.transform.position.y > 55f ? Vector3.right : Vector3.up) * 50f);
                RPCProtection();
            }
        }

        public static void GuardianCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > crashAllDelay && lockTarget.transform.position.x < -5)
                    {
                        crashAllDelay = Time.time + 0.1f;
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), (lockTarget.transform.position.y > 55f ? Vector3.right : Vector3.up) * 50f);
                        RPCProtection();
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

        public static void GuardianCrashAll()
        {
            if (rightTrigger > 0.5f && Time.time > crashAllDelay)
            {
                crashAllDelay = Time.time + 0.1f;
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!rig.isLocal && rig.transform.position.x < -5)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), (rig.transform.position.y > 55f ? Vector3.right : Vector3.up) * 50f);
                        RPCProtection();
                    }
                }
            }
        }

        private static float reportDelay;
        public static void DelayBanGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !gunLocked)
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;

                        if (PlayerIsTagged(VRRig.LocalRig))
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be tagged.");
                            return;
                        }

                        if (!PlayerIsTagged(lockTarget))
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> The target must be tagged.");
                            return;
                        }

                        if (PhotonNetwork.IsMasterClient)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be master client.");
                            return;
                        }

                        if (Time.time > reportDelay)
                        {
                            reportDelay = Time.time + 0.5f;
                            GorillaPlayerScoreboardLine.ReportPlayer(GetPlayerFromVRRig(lockTarget).UserId, GorillaPlayerLineButton.ButtonType.Cheating, GetPlayerFromVRRig(lockTarget).NickName);
                        }
                        
                        SerializePatch.OverrideSerialization = () =>
                        {
                            NetPlayer target = GetPlayerFromVRRig(lockTarget);
                            MassSerialize(true, new PhotonView[] { GorillaTagger.Instance.myVRRig.GetView });

                            Vector3 positionArchive = VRRig.LocalRig.transform.position;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = AllActorNumbersExcept(new int[] { PhotonNetwork.MasterClient.ActorNumber, RigManager.GetPlayerFromVRRig(lockTarget).ActorNumber }) });

                            VRRig.LocalRig.transform.position = new Vector3(99999f, 99999f, 99999f);
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber } });

                            VRRig.LocalRig.transform.position = lockTarget.rightHandTransform.position;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = new int[] { GetPlayerFromVRRig(lockTarget).ActorNumber } });

                            RPCProtection();
                            VRRig.LocalRig.transform.position = positionArchive;

                            return false;
                        };
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    SerializePatch.OverrideSerialization = null;
                }
            }
        }

        public static void DelayBanAll()
        {
            SerializePatch.OverrideSerialization = () =>
            {
                if (PlayerIsTagged(VRRig.LocalRig))
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be tagged.");
                    return true;
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be master client.");
                    return true;
                }

                NetPlayer target = GetPlayerFromVRRig(lockTarget);
                MassSerialize(true, new PhotonView[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 positionArchive = VRRig.LocalRig.transform.position;
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = PhotonNetwork.PlayerList.Where(player => !player.IsMasterClient && PlayerIsTagged(GetVRRigFromPlayer(player))).Select(player => player.ActorNumber).ToArray() });

                VRRig.LocalRig.transform.position = new Vector3(99999f, 99999f, 99999f);
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber } });

                foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig rig = GetVRRigFromPlayer(player);
                    if (!player.IsMasterClient && PlayerIsTagged(rig))
                    {
                        VRRig.LocalRig.transform.position = rig.rightHandTransform.position;
                        SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = new int[] { player.ActorNumber } });
                    }
                }

                RPCProtection();
                VRRig.LocalRig.transform.position = positionArchive;

                return false;
            };
        }

        public static void ObliteratePlayer(NetPlayer target)
        {
            if (Time.time > crashAllDelay)
            {
                crashAllDelay = Time.time + 0.1f;
                VRRig rig = GetVRRigFromPlayer(target);
                BetaSetVelocityPlayer(target, (rig.transform.position.y > 55f ? Vector3.right : Vector3.up) * 50f);
                RPCProtection();
            }
        }

        public static void GuardianObliterateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > crashAllDelay) /*&& lockTarget.transform.position.x < 80)*/
                    {
                        crashAllDelay = Time.time + 0.1f;
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), (lockTarget.transform.position.y > 55f ? Vector3.right : Vector3.up) * 50f);
                        RPCProtection();
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

        public static void GuardianObliterateAll()
        {
            if (rightTrigger > 0.5f && Time.time > crashAllDelay)
            {
                crashAllDelay = Time.time + 0.1f;
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!rig.isLocal)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), (rig.transform.position.y > 55f ? Vector3.right : Vector3.up) * 50f);
                        RPCProtection();
                    }
                }
            }
        }

        private static float serializeDelay;
        public static void DirectionOnGrab(Vector3 direction)
        {
            VRRig.LocalRig.enabled = true;
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal) /*&& rig.transform.position.x < 80)*/
                {
                    if (rig.leftHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer || rig.rightHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        VRRig.LocalRig.enabled = false;

                        if (Time.time > serializeDelay)
                        {
                            serializeDelay = Time.time + 0.3f;

                            for (int i = 0; i < 100; i++)
                            {
                                VRRig.LocalRig.transform.position += direction.normalized;
                                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { GetPlayerFromVRRig(rig).ActorNumber } });
                            }

                            RPCProtection();
                        }

                        if (rig.LatestVelocity().y > 6f)
                        {
                            VRRig.LocalRig.transform.position += direction.normalized * 100f;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { GetPlayerFromVRRig(rig).ActorNumber } });

                            RPCProtection();
                        }
                    }
                }
            }
        }

        private static GameObject point;
        public static void TowardsPointOnGrab()
        {
            if (point == null)
            {
                point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(point.GetComponent<Collider>());
                point.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                point.transform.localScale = Vector3.one * 0.2f;
            }

            point.GetComponent<Renderer>().material.color = GetBDColor(0f);

            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                    point.transform.position = NewPointer.transform.position + (Vector3.up * 0.5f);
            }

            TowardsPositionOnGrab(point.transform.position);
        }

        public static void DisableTowardsPointOnGrab()
        {
            if (point != null)
            {
                UnityEngine.Object.Destroy(point);
                point = null;
            }
        }

        public static void TowardsPositionOnGrab(Vector3 position)
        {
            VRRig.LocalRig.enabled = true;
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal) /*&& rig.transform.position.x < 80)*/
                {
                    if (rig.leftHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer || rig.rightHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Vector3 direction = position - rig.transform.position;

                        VRRig.LocalRig.enabled = false;

                        if (Time.time > serializeDelay)
                        {
                            serializeDelay = Time.time + 0.3f;

                            for (int i = 0; i < 100; i++)
                            {
                                VRRig.LocalRig.transform.position += direction.normalized;
                                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { GetPlayerFromVRRig(rig).ActorNumber } });
                            }

                            RPCProtection();
                        }

                        if (rig.LatestVelocity().y > 6f)
                        {
                            VRRig.LocalRig.transform.position += direction.normalized * 100f;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { GetPlayerFromVRRig(rig).ActorNumber } });

                            RPCProtection();
                        }
                    }
                }
            }
        }

        public static void BlindOnGrab()
        {
            VRRig.LocalRig.enabled = true;
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal) /*&& rig.transform.position.x < 80)*/
                {
                    if (rig.leftHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer || rig.rightHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * UnityEngine.Random.Range(99999f, 99f);
                        VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position;
                    }
                }
            }
        }

        public static void FlingOnGrab()
        {
            VRRig.LocalRig.enabled = true;
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal) /*&& rig.transform.position.x < 80)*/
                {
                    if (rig.leftHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer || rig.rightHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
                        GetNetworkViewFromVRRig(VRRig.LocalRig).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(rig), new object[] { (Vector3.up + (GorillaTagger.Instance.bodyCollider.transform.forward * 1)).normalized * 20f }); 
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

            GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                GetNetworkViewFromVRRig(GetVRRigFromPlayer(victim)).SendRPC("GrabbedByPlayer", victim, new object[] { true, false, false });
                GetNetworkViewFromVRRig(GetVRRigFromPlayer(victim)).SendRPC("DroppedByPlayer", victim, new object[] { velocity });
            }
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
        }

        public static void BetaSetVelocityTargetGroup(RpcTarget victim, Vector3 velocity)
        {
            if (velocity.sqrMagnitude > 20f)
                velocity = Vector3.Normalize(velocity) * 20f;

            GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                switch (victim)
                {
                    case RpcTarget.All:
                        {
                            foreach (VRRig rig in GorillaParent.instance.vrrigs)
                            {
                                GetNetworkViewFromVRRig(rig).SendRPC("GrabbedByPlayer", GetPlayerFromVRRig(rig), new object[] { true, false, false });
                                GetNetworkViewFromVRRig(rig).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(rig), new object[] { velocity });
                            }
                            break;
                        }
                    case RpcTarget.Others:
                        {
                            foreach (VRRig rig in GorillaParent.instance.vrrigs)
                            {
                                if (!rig.isLocal)
                                {
                                    GetNetworkViewFromVRRig(rig).SendRPC("GrabbedByPlayer", GetPlayerFromVRRig(rig), new object[] { true, false, false });
                                    GetNetworkViewFromVRRig(rig).SendRPC("DroppedByPlayer", GetPlayerFromVRRig(rig), new object[] { velocity });
                                }
                            }
                            break;
                        }
                    case RpcTarget.MasterClient:
                        {
                            GetNetworkViewFromVRRig(GetVRRigFromPlayer(NetworkSystem.Instance.MasterClient)).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                            GetNetworkViewFromVRRig(GetVRRigFromPlayer(NetworkSystem.Instance.MasterClient)).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { velocity });
                            break;
                        }
                }
            }
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
                        GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GetNetworkViewFromVRRig(gunTarget).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                            RPCProtection();
                        }
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
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
                GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
                if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                {
                    foreach (VRRig plr in GorillaParent.instance.vrrigs)
                    {
                        if (!plr.isLocal)
                        {
                            GetNetworkViewFromVRRig(plr).SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                            RPCProtection();
                        }
                    }
                }
                else
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
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
                        GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
                        if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GetNetworkViewFromVRRig(gunTarget).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 0f, 0f) });
                            RPCProtection();
                        }
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                        
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
                GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
                if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                {
                    foreach (VRRig plr in GorillaParent.instance.vrrigs)
                    {
                        if (!plr.isLocal)
                        {
                            GetNetworkViewFromVRRig(plr).SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 0f, 0f) });
                            RPCProtection();
                        }
                    }
                }
                else
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
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

                BetaSetVelocityTargetGroup(RpcTarget.Others, new Vector3(0f, 19.9f, 0f));
                RPCProtection();
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
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), RandomVector3(50f));
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

        public static void SpazAllPlayers()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.1f;
                BetaSetVelocityTargetGroup(RpcTarget.Others, RandomVector3(50f));
                RPCProtection();
            }
        }

        public static void BlockCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                    Fun.RequestCreatePiece(1934114066, new Vector3(-127.6248f, 16.99441f, -217.2094f), Quaternion.identity, 0, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)));
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

        public static void BlockCrashAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                Fun.RequestCreatePiece(1934114066, new Vector3(-127.6248f, 16.99441f, -217.2094f), Quaternion.identity, 0, RpcTarget.Others);
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

        public static int snowballScale = 5;
        public static void ChangeSnowballScale(bool positive = true)
        {
            if (positive)
                snowballScale++;
            else
                snowballScale--;

            if (snowballScale > 5)
                snowballScale = 1;

            if (snowballScale < 1)
                snowballScale = 5;

            GetIndex("Change Snowball Scale").overlapText = "Change Snowball Scale <color=grey>[</color><color=green>" + (snowballScale).ToString() + "</color><color=grey>]</color>";
        }

        public static int lagIndex = 1;
        public static int lagAmount;
        public static float lagDelay;
        public static void ChangeLagPower(bool positive = true)
        {
            if (positive)
                lagIndex++;
            else
                lagIndex--;

            lagIndex %= 3;
            if (lagIndex < 0)
                lagIndex = 2;

            lagAmount = new int[] { 40, 113, 425 } [lagIndex];
            lagDelay = new float[] { 0.1f, 0.25f, 1f } [lagIndex];

            GetIndex("Change Lag Power").overlapText = "Change Lag Power <color=grey>[</color><color=green>" + new string[] { "Light", "Heavy", "Spike" } [lagIndex] + "</color><color=grey>]</color>";
        }

        public static float snowballSpawnDelay = 0.1f;
        public static bool SnowballHandIndex;
        public static void BetaSpawnSnowball(Vector3 Pos, Vector3 Vel, float Scale, int Mode, Player Target = null, bool NetworkSize = true, int customNetworkedSize = -1)
        {
            try
            {
                Scale = snowballScale;

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
                    if (!rig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f))
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
                    if (!rig.isLocal)
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

        public static void FlingPlayer(NetPlayer player)
        {
            if (Time.time > snowballDelay)
            {
                BetaSpawnSnowball(GetVRRigFromPlayer(player).transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 5f, 2, NetPlayerToPlayer(player));
                snowballDelay = Time.time + snowballSpawnDelay;
            }
        }

        public static void FlingPlayer(VRRig player) => FlingPlayer(GetPlayerFromVRRig(player));

        public static void SnowballFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                    FlingPlayer(lockTarget);

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

        public static void SnowballFlingAll()
        {
            if (rightTrigger > 0.5f)
                FlingPlayer(GetCurrentTargetRig(0.5f));
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

                Player plr = NetPlayerToPlayer(GetPlayerFromVRRig(GetCurrentTargetRig(0.5f)));
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
                    Player plr = NetPlayerToPlayer(GetPlayerFromVRRig(GetCurrentTargetRig(0.5f)));
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

        private static float antireportfling;
        public static void AntiReportFling()
        {
            try
            {
                if (Time.time > antireportfling)
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
                                if (!vrrig.isLocal)
                                {
                                    float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                    float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                    if (D1 < Safety.threshold || D2 < Safety.threshold)
                                    {
                                        if (!Safety.smartarp || (Safety.smartarp && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                        {
                                            antireportfling = Time.time + 0.1f;
                                            BetaSetVelocityPlayer(GetPlayerFromVRRig(vrrig), (vrrig.transform.position - report.position) * 50f);
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

        public static void AntiReportSnowballFling()
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
                                if (!vrrig.isLocal)
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
                    { 2, PhotonNetwork.ServerTimestamp },
                    { 3, method },
                    { 4, parameters }
                };

                if (photonView.Prefix > 0)
                    rpcData[1] = (short)photonView.Prefix;
                
                if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(method))
                    rpcData[5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(method);
                
                if (targetActors.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
                    PhotonNetwork.ExecuteRpc(rpcData, PhotonNetwork.LocalPlayer);
                
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

        public static bool SpecialTimeRPC(PhotonView photonView, int timeOffset, string method, int[] targetActors, object[] parameters)
        {
            if (photonView != null && parameters != null && !string.IsNullOrEmpty(method))
            {
                ExitGames.Client.Photon.Hashtable rpcData = new ExitGames.Client.Photon.Hashtable
                {
                    { 0, photonView.ViewID },
                    { 2, PhotonNetwork.ServerTimestamp + timeOffset },
                    { 3, method },
                    { 4, parameters }
                };

                if (photonView.Prefix > 0)
                    rpcData[1] = (short)photonView.Prefix;

                if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(method))
                    rpcData[5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(method);

                if (targetActors.Contains(PhotonNetwork.LocalPlayer.ActorNumber) && targetActors.Length == 1)
                    PhotonNetwork.ExecuteRpc(rpcData, PhotonNetwork.LocalPlayer);

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
                PhotonView that = GetObject("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>();
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
                PhotonView that = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/StumpVRHeadset/ModIOArcadeTeleporter (1)/NetObject_VRTeleporter").GetComponent<PhotonView>();
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
                    gunLocked = false;
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

        public static void BringPlayer(NetPlayer player)
        {
            if (Time.time > kgDebounce)
            {
                BetaSetVelocityPlayer(player, (GorillaTagger.Instance.bodyCollider.transform.position - GetVRRigFromPlayer(player).transform.position).normalized * 20f);
                RPCProtection();
                kgDebounce = Time.time + 0.1f;
            }
        }

        public static void BringPlayerGun(NetPlayer player)
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
                        BetaSetVelocityPlayer(player, Vector3.Normalize(NewPointer.transform.position - GetVRRigFromPlayer(player).transform.position) * 50f);
                        RPCProtection();
                        kgDebounce = Time.time + 0.2f;
                    }
                }
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
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), (GorillaTagger.Instance.bodyCollider.transform.position - lockTarget.transform.position).normalized * 20f);
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
                    if (!plr.isLocal)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), (GorillaTagger.Instance.bodyCollider.transform.position - plr.transform.position).normalized * 20f);
                        RPCProtection();
                    }
                }
            }
        }

        public static void BringAwayGun()
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
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), (lockTarget.transform.position - GorillaTagger.Instance.bodyCollider.transform.position).normalized * 20f);
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

        public static void BringAwayAll()
        {
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.2f;
                foreach (VRRig plr in GorillaParent.instance.vrrigs)
                {
                    if (!plr.isLocal)
                    {
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), (plr.transform.position - GorillaTagger.Instance.bodyCollider.transform.position).normalized * 20f);
                        RPCProtection();
                    }
                }
            }
        }

        public static void OrbitAll()
        {
            float scale = 5f;
            if (rightTrigger > 0.5f && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.2f;
                int index = 0;

                VRRig[] rigs = GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal).ToArray();
                foreach (VRRig rig in rigs)
                {
                    float offset = (360f / rigs.Length) * index;
                    Vector3 targetPosition = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + Time.time) * scale, 2, MathF.Sin(offset + Time.time) * scale);

                    BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), (targetPosition - rig.transform.position) * 1f);
                    RPCProtection();
                    index++;
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
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(lockTarget), lockTarget.headMesh.transform.forward * Movement._flySpeed);
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
                    gunLocked = false;
            }
        }

        public static void GiveFlyAll()
        {
            if (Time.time > thingdeb)
            {
                thingdeb = Time.time + 0.1f;
                foreach (VRRig plr in GorillaParent.instance.vrrigs)
                {
                    if (!plr.isLocal)
                    {
                        if (plr.rightThumb.calcT > 0.5f)
                        {
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), plr.headMesh.transform.forward * Movement._flySpeed);
                            RPCProtection();
                        }
                    }
                }
            }
        }

        public static void PunchMod()
        {
            if (Time.time > thingdeb)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    bool leftHand = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f;
                    bool rightHand = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f;

                    if (!rig.isLocal && (leftHand || rightHand))
                    {
                        Vector3 vel = rightHand ? GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0) : GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);

                        BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), vel);
                        thingdeb = Time.time + 0.1f;
                    }
                }
            }
        }

        private static Dictionary<VRRig, float> boxingDelay = new Dictionary<VRRig, float> { };
        private static float GetBoxingDelay(VRRig rig)
        {
            if (boxingDelay.ContainsKey(rig))
                return boxingDelay[rig];

            return -1;
        }

        private static void SetBoxingDelay(VRRig rig)
        {
            if (boxingDelay.ContainsKey(rig))
                boxingDelay.Remove(rig);

            boxingDelay.Add(rig, Time.time + 0.5f);
        }

        public static void Boxing()
        {
            foreach (VRRig rig1 in GorillaParent.instance.vrrigs)
            {
                if (Time.time < GetBoxingDelay(rig1))
                    continue;

                foreach (VRRig rig2 in GorillaParent.instance.vrrigs)
                {
                    if (rig2 == rig1) continue;
                    if (Vector3.Distance(rig2.leftHandTransform.position, rig1.headMesh.transform.position) < 0.25f || Vector3.Distance(rig2.rightHandTransform.position, rig1.headMesh.transform.position) < 0.25f)
                    {
                        Vector3 targetDirection = (rig1.headMesh.transform.position - rig2.headMesh.transform.position) * 20f;
                        BetaSetVelocityPlayer(GetPlayerFromVRRig(rig1), targetDirection);
                        SetBoxingDelay(rig1);
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
                            if (!plr.isLocal)
                                BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), Vector3.Normalize(NewPointer.transform.position - plr.transform.position) * 50f);
                        }
                        RPCProtection();
                        kgDebounce = Time.time + 0.2f;
                    }
                }
            }
        }

        public static void BringAwayAllGun()
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
                            if (!plr.isLocal)
                                BetaSetVelocityPlayer(GetPlayerFromVRRig(plr), Vector3.Normalize(plr.transform.position - NewPointer.transform.position) * 50f);
                        }
                        RPCProtection();
                        kgDebounce = Time.time + 0.2f;
                    }
                }
            }
        }

        public static void AntiStump()
        {
            if (Time.time > kgDebounce)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!rig.isLocal)
                    {
                        Vector3 stump = new Vector3(-66f, 12f, -79f);
                        if (Vector3.Distance(stump, rig.transform.position) < 3f)
                        {
                            BetaSetVelocityPlayer(GetPlayerFromVRRig(rig), (rig.transform.position - stump).normalized * 20f);
                            kgDebounce = Time.time + 0.2f;
                        }
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
                    GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
                    if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    {
                        ActiveNetworkHandlerRPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", RpcTarget.All, new object[] { GorillaTagger.Instance.rightHandTransform.position, new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) });
                        RPCProtection();
                        flip = !flip;
                    }
                    else
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                    
                    slamDel = Time.time + 0.05f;
                }
            }
            if (leftGrab)
            {
                if (Time.time > slamDel)
                {
                    GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
                    if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    {
                        ActiveNetworkHandlerRPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", RpcTarget.All, new object[] { GorillaTagger.Instance.leftHandTransform.position, new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) });
                        RPCProtection();
                        flip = !flip;
                    }
                    else
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be guardian.</color>");
                    
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
                    GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
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

        public static void LagPlayer(object general)
        {
            if (PhotonNetwork.InRoom && Time.time > lagDebounce)
            {
                if (general is NetPlayer player)
                {
                    for (int i = 0; i < lagAmount; i++)
                       FriendshipGroupDetection.Instance.photonView.RPC("NotifyPartyMerging", NetPlayerToPlayer(player), new object[] { null });
                }
                else if (general is RpcTarget target)
                {
                    for (int i = 0; i < lagAmount; i++)
                        FriendshipGroupDetection.Instance.photonView.RPC("NotifyPartyMerging", target, new object[] { null });
                }
                else if (general is int[] targets)
                {
                    for (int i = 0; i < lagAmount; i++)
                        SpecialTargetRPC(FriendshipGroupDetection.Instance.photonView, "NotifyPartyMerging", targets, new object[] { null });
                }

                RPCProtection();
                lagDebounce = Time.time + lagDelay;
            }
        }

        private static float lagDebounce;
        public static void LagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                    LagPlayer(GetPlayerFromVRRig(lockTarget));

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

        public static void LagAll() => LagPlayer(RpcTarget.Others);

        public static void LagAura()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                Vector3 they = vrrig.headMesh.transform.position;
                Vector3 notthem = VRRig.LocalRig.head.rigTarget.position;
                float distance = Vector3.Distance(they, notthem);

                if (distance < 3f && !PlayerIsLocal(vrrig))
                {
                    LagPlayer(GetPlayerFromVRRig(vrrig));
                }
            }
        }

        public static void AntiReportLag()
        {
            try
            {
                List<int> actors = new List<int> { };

                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, Safety.threshold, Color.red);

                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isLocal)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < Safety.threshold || D2 < Safety.threshold)
                                {
                                    if (!Safety.smartarp || (Safety.smartarp && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        actors.Add(GetPlayerFromVRRig(vrrig).ActorNumber);
                                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they are being lagged.");
                                    }
                                }
                            }
                        }
                    }
                }

                if (actors.Count > 0)
                    LagPlayer(actors.ToArray());
            }
            catch { } // Not connected
        }

        public static void SetRoomLock(bool status)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { { 254, status ? NetworkSystem.Instance.SessionIsPrivate : !NetworkSystem.Instance.SessionIsPrivate } };

            Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
            dictionary.Add(251, hash);
            dictionary.Add(250, false);
            dictionary.Add(231, null);

            PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOperation(
                252,
                dictionary,
                SendOptions.SendReliable
            );
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
                        DestroyPlayer(NetPlayerToPlayer(GetPlayerFromVRRig(gunTarget)));
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void DestroyAll()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
                DestroyPlayer(player);
        }

        public static void DestroyPlayer(NetPlayer player) =>
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);

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
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
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
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
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
        
        public static void SetPlayerStatus(int status, NetPlayer player) =>
            BetaSetStatus(status, new RaiseEventOptions { TargetActors = new int[1] { player.ActorNumber } });

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
                if (!vrrig.isLocal)
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
