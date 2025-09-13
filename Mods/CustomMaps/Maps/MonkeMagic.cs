using ExitGames.Client.Photon;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.CustomMaps.Maps
{
    public class MonkeMagic : CustomMap
    {
        public override long MapID => 5107228;
        public override ButtonInfo[] Buttons => new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Lightning Strike Self", method =() => LightningStrikeSelf(), toolTip = "Strikes yourself with lightning."},
            new ButtonInfo { buttonText = "Lightning Strike Gun", method =() => LightningStrikeGun(), toolTip = "Strikes whoever your hand desires with lightning."},
            new ButtonInfo { buttonText = "Lightning Strike All", method =() => LightningStrikeAll(), toolTip = "Strikes everyone in the room with lightning."},

            new ButtonInfo { buttonText = "Change Material Self", method =() => ChangeMaterialSelf(), toolTip = "Changes your material."},
            new ButtonInfo { buttonText = "Change Material Gun", method =() => ChangeMaterialGun(), toolTip = "Changes the material of whoever your hand desires."},
            new ButtonInfo { buttonText = "Change Material All", method =() => ChangeMaterialAll(), toolTip = "Changes the material of everyone in the room."},

            new ButtonInfo { buttonText = "Spawn Lucy Self", isTogglable = false, method =() => SpawnLucySelf(), toolTip = "Spawns lucy on yourself."},
            new ButtonInfo { buttonText = "Spawn Lucy Gun", method =() => SpawnLucyGun(), toolTip = "Spawns lucy on whoever your hand desires."},
            new ButtonInfo { buttonText = "Spawn Lucy All", isTogglable = false, method =() => SpawnLucyAll(), toolTip = "Spawns lucy on everyone in the room."},

            new ButtonInfo { buttonText = "Crash Gun", method =() => CrashGun(), toolTip = "Crashes whoever your hand desires in the custom map." },
            new ButtonInfo { buttonText = "Crash All", method =() => CrashAll(), isTogglable = false, toolTip = "Crashes everyone in the custom map." },
            new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>", method =() => AntiReportCrash(), toolTip = "Crashes everyone who tries to report you." },
            new ButtonInfo { buttonText = "Crash On Touch", method =() => CrashOnTouch(), toolTip = "Crashes whoever you touch in the custom map." }
        };

        private static float lightningDelay;
        public static void LightningStrikeSelf()
        {
            if (Time.time > lightningDelay)
            {
                lightningDelay = Time.time + 0.2f;
                PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)PhotonNetwork.LocalPlayer.ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        public static void LightningStrikeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null && Time.time > lightningDelay)
                {
                    lightningDelay = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)GetPlayerFromVRRig(lockTarget).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
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
                    gunLocked = false;
            }
        }

        public static void LightningStrikeAll()
        {
            if (Time.time > lightningDelay)
            {
                lightningDelay = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)GetRandomPlayer(false).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        private static float materialDelay;
        public static void ChangeMaterialSelf()
        {
            if (Time.time > materialDelay)
            {
                materialDelay = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(180, new object[] { "ChangingMaterial", (double)PhotonNetwork.LocalPlayer.ActorNumber, (double)UnityEngine.Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        public static void ChangeMaterialGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null && Time.time > materialDelay)
                {
                    materialDelay = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(180, new object[] { "ChangingMaterial", (double)PhotonNetwork.LocalPlayer.ActorNumber, (double)UnityEngine.Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
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
                    gunLocked = false;
            }
        }

        public static void ChangeMaterialAll()
        {
            if (Time.time > materialDelay)
            {
                materialDelay = Time.time + 0.2f;
                PhotonNetwork.RaiseEvent(180, new object[] { "ChangingMaterial", (double)GetPlayerFromVRRig(lockTarget).ActorNumber, (double)UnityEngine.Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        private static float lucyDelay;
        public static void SpawnLucySelf()
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)PhotonNetwork.LocalPlayer.ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            RPCProtection();
        }

        public static void SpawnLucyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && Time.time > lucyDelay)
                    {
                        lucyDelay = Time.time + 0.2f;
                        PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)GetPlayerFromVRRig(lockTarget).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                        RPCProtection();
                    }
                }
            }
        }

        public static void SpawnLucyAll()
        {
            foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
            {
                PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)player.ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        // I don't know who made this
        public static float crashDelay = 0f;
        public static void CrashPlayer(int ActorNumber)
        {
            Photon.Pun.PhotonNetwork.RaiseEvent(180, new object[] { "leaveGame", (double)ActorNumber, false, (double)ActorNumber }, new Photon.Realtime.RaiseEventOptions()
            {
                TargetActors = new int[]
                {
                    ActorNumber
                }
            }, ExitGames.Client.Photon.SendOptions.SendReliable);
            RPCProtection();
        }
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                UnityEngine.RaycastHit Ray = GunData.Ray;
                UnityEngine.GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null && UnityEngine.Time.time > crashDelay)
                {
                    NetPlayer Player = RigManager.GetPlayerFromVRRig(lockTarget);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = UnityEngine.Time.time + 0.2f;
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
        public static void CrashOnTouch()
        {
            if (UnityEngine.Time.time < crashDelay)
                return;
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal && (UnityEngine.Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || UnityEngine.Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f))
                {
                    NetPlayer Player = RigManager.GetPlayerFromVRRig(rig);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = UnityEngine.Time.time + 0.2f;
                }
            }
        }
        public static void CrashAll()
        {
            if (UnityEngine.Time.time > crashDelay)
            {
                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    CrashPlayer(Player.ActorNumber);
                }
                crashDelay = UnityEngine.Time.time + 0.1f;
            }
        }
        public static void AntiReportCrash()
        {
            Safety.AntiReport((vrrig, position) =>
            {

                if (UnityEngine.Time.time > crashDelay)
                {
                    NetPlayer Player = RigManager.GetPlayerFromVRRig(vrrig);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = UnityEngine.Time.time + 0.5f;
                    Notifications.NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigManager.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been crashed.");
                }
            });
        }
    }
}