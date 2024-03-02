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
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = (isCopying || (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)) ? buttonClickedA : buttonDefaultA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = isCopying ? whoCopy.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f);
                liner.endColor = GetBGColor(0.5f);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, isCopying ? whoCopy.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

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
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = (isCopying || (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)) ? buttonClickedA : buttonDefaultA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = isCopying ? whoCopy.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f);
                liner.endColor = GetBGColor(0.5f);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, isCopying ? whoCopy.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

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
