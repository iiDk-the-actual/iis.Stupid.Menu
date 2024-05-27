using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;
using static iiMenu.Classes.RigManager;

namespace iiMenu.Mods
{
    internal class Battle
    {
        public static void BattleStartGame()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.StartBattle();
            }
        }

        public static void BattleEndGame()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.BattleEnd();
            }
        }

        public static void BattleRestartGame()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.BattleEnd();
                lol.StartBattle();
            }
        }

        public static void BattleBalloonSpamSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = UnityEngine.Random.Range(0, 4);
            }
        }

        public static void BattleBalloonSpam()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                foreach (Photon.Realtime.Player loln in PhotonNetwork.PlayerList)
                {
                    lol.playerLives[loln.ActorNumber] = UnityEngine.Random.Range(0, 4);
                }
            }
        }

        public static void BattleKillGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player owner = GetPlayerFromVRRig(possibly);
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            if (!GetIndex("Disable Auto Anti Ban").enabled)
                            {
                                Overpowered.FastMaster();
                            }
                        }
                        else
                        {
                            GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                            lol.playerLives[owner.ActorNumber] = 0;
                        }
                    }
                }
            }
        }

        public static void BattleKillSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 0;
            }
        }

        public static void BattleKillAll()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                foreach (Photon.Realtime.Player loln in PhotonNetwork.PlayerList)
                {
                    lol.playerLives[loln.ActorNumber] = 0;
                }
            }
        }

        public static void BattleReviveGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player owner = GetPlayerFromVRRig(possibly);
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            if (!GetIndex("Disable Auto Anti Ban").enabled)
                            {
                                Overpowered.FastMaster();
                            }
                        }
                        else
                        {
                            GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                            lol.playerLives[owner.ActorNumber] = 4;
                        }
                    }
                }
            }
        }

        public static void BattleReviveSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
            }
        }

        public static void BattleReviveAll()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                foreach (Photon.Realtime.Player loln in PhotonNetwork.PlayerList)
                {
                    lol.playerLives[loln.ActorNumber] = 4;
                }
            }
        }

        public static void BattleGodMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
                GorillaLocomotion.Player.Instance.disableMovement = false;
            }
        }
    }
}
