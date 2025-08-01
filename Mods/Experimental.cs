﻿using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Experimental
    {
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
                        string buttonText = button.overlapText ?? button.buttonText;
                        button.overlapText = buttonText;
                        button.buttonText += "X";
                        duplicateButtons++;
                    }
                    previousNames.Add(button.buttonText);
                }
            }
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully fixed " + duplicateButtons.ToString() + " broken buttons.");
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
                        Material replacement = new Material(Shader.Find("GorillaTag/UberShader"))
                        {
                            color = v.material.color
                        };
                        v.material = replacement;
                    }
                } catch (Exception exception) { LogManager.LogError(string.Format("mat error {1} - {0}", exception.Message, exception.StackTrace)); }
            }
        }
        public static void DisableBetterFPSBoost()
        {
            foreach (KeyValuePair<Renderer, Material> v in oldMats)
                v.Key.material = v.Value;
        }

        public static void DumpSoundData()
        {
            string text = "Handtap Sound Data\n(from GorillaLocomotion.GTPlayer.Instance.materialData)";
            int i = 0;
            foreach (GorillaLocomotion.GTPlayer.MaterialData oneshot in GorillaLocomotion.GTPlayer.Instance.materialData)
            {
                try
                {
                    text += "\n====================================\n";
                    text += i.ToString() + " ; " + oneshot.matName + " ; " + oneshot.slidePercent.ToString() + "% ; " + (oneshot.audio == null ? "none" : oneshot.audio.name);
                }
                catch { LogManager.Log("Failed to log sound"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/SoundData.txt";

            File.WriteAllText(fileName, text);

            string filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;

            Process.Start(filePath);
        }

        public static void DumpCosmeticData()
        {
            string text = "Cosmetic Data\n(from GorillaNetworking.CosmeticsController.allCosmeticsDict)";
            int i = 0;
            foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
            {
                try
                {
                    text += "\n====================================\n";
                    text += hat.itemName + " ; " + hat.displayName + " (override " + hat.overrideDisplayName + ") ; " + hat.cost.ToString() + "SR ; canTryOn = " + hat.canTryOn.ToString();
                }
                catch { LogManager.Log("Failed to log hat"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/CosmeticData.txt";

            File.WriteAllText(fileName, text);

            string filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;

            Process.Start(filePath);
        }

        public static void DecryptableCosmeticData()
        {
            string text = "";
            int i = 0;
            foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
            {
                try
                {
                    text += hat.itemName + ";;" + hat.overrideDisplayName + ";;" + hat.cost.ToString() + "\n";
                }
                catch { LogManager.Log("Failed to log hat"); }
                i++;
            }
            string fileName = $"{PluginInfo.BaseDirectory}/DecryptableCosmeticData.txt";

            File.WriteAllText(fileName, text);

            string filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;

            Process.Start(filePath);
        }

        public static void DumpRPCData()
        {
            string text = "RPC Data\n(from PhotonNetwork.PhotonServerSettings.RpcList)";
            int i = 0;
            foreach (string name in PhotonNetwork.PhotonServerSettings.RpcList)
            {
                try
                {
                    text += "\n====================================\n";
                    text += i.ToString() + " ; " + name;
                }
                catch { LogManager.Log("Failed to log RPC"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/RPCData.txt";

            File.WriteAllText(fileName, text);

            string filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;
            
            Process.Start(filePath);
        }

        public static void CopyCustomGamemodeScript() =>
            GUIUtility.systemCopyBuffer = CustomGameMode.LuaScript;

        private static float adminEventDelay;
        public static void AdminKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("kick", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                    }
                }
            }
        }

        public static void AdminKickAll() =>
            Classes.Console.ExecuteCommand("kickall", ReceiverGroup.All);

        public static void FlipMenuGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Right Hand");
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

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Classes.Console.ExecuteCommand("tp", ReceiverGroup.Others, NewPointer.transform.position);
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

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("vel", GetPlayerFromVRRig(gunTarget).ActorNumber, new Vector3(0f, 50f, 0f));
                    }
                }
            }
        }

        public static void AdminLockdownGun(bool enable)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("togglemenu", GetPlayerFromVRRig(gunTarget).ActorNumber, enable);
                    }
                }
            }
        }

        public static void AdminFullLockdownGun(bool enable)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        if (enable)
                        {
                            Classes.Console.ExecuteCommand("forceenable", GetPlayerFromVRRig(gunTarget).ActorNumber, "Disable Autosave");
                            Classes.Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Disable Autosave");
                            Classes.Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Load Preferences");
                            Classes.Console.ExecuteCommand("togglemenu", GetPlayerFromVRRig(gunTarget).ActorNumber, enable);
                        } else
                        {
                            Classes.Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Save Preferences");
                            Classes.Console.ExecuteCommand("forceenable", GetPlayerFromVRRig(gunTarget).ActorNumber, "Disable Autosave");
                            Classes.Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Panic");
                            Classes.Console.ExecuteCommand("togglemenu", GetPlayerFromVRRig(gunTarget).ActorNumber, enable);
                        }
                    }
                }
            }
        }

        private static bool lastInRoom2;
        private static int lastPlayerCount2 = -1;
        public static void AdminLockdownAll(bool enable)
        {
            if (PhotonNetwork.InRoom && (!lastInRoom2 || PhotonNetwork.PlayerList.Length != lastPlayerCount2))
                Classes.Console.ExecuteCommand("togglemenu", ReceiverGroup.Others, enable);

            lastInRoom2 = PhotonNetwork.InRoom;
            lastPlayerCount2 = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount2 = -1;
        }

        public static void AdminFullLockdownAll(bool enable)
        {
            if (PhotonNetwork.InRoom && (!lastInRoom2 || PhotonNetwork.PlayerList.Length != lastPlayerCount2))
            {
                if (enable)
                {
                    Classes.Console.ExecuteCommand("forceenable", ReceiverGroup.Others, "Disable Autosave");
                    Classes.Console.ExecuteCommand("toggle", ReceiverGroup.Others, "Disable Autosave");
                    Classes.Console.ExecuteCommand("toggle", ReceiverGroup.Others, "Load Preferences");
                    Classes.Console.ExecuteCommand("togglemenu", ReceiverGroup.Others, enable);
                }
                else
                {
                    Classes.Console.ExecuteCommand("toggle", ReceiverGroup.Others, "Save Preferences");
                    Classes.Console.ExecuteCommand("forceenable", ReceiverGroup.Others, "Disable Autosave");
                    Classes.Console.ExecuteCommand("toggle", ReceiverGroup.Others, "Panic");
                    Classes.Console.ExecuteCommand("togglemenu", ReceiverGroup.Others, enable);
                }
            }

            lastInRoom2 = PhotonNetwork.InRoom;
            lastPlayerCount2 = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount2 = -1;
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
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (!rig.isLocal)
                        {
                            if (Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 0.2f)
                            {
                                thestrangledleft = rig;
                                if (PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                        89,
                                        true,
                                        999999f
                                    });
                                }
                                else
                                    VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
                            }
                        }
                    }
                }
                else
                {
                    if (Time.time > stdell)
                    {
                        stdell = Time.time + 0.05f;
                        Classes.Console.ExecuteCommand("tpnv", GetPlayerFromVRRig(thestrangledleft).ActorNumber, GorillaTagger.Instance.leftHandTransform.position);
                    }
                }
            }
            else
            {
                if (thestrangledleft != null)
                {
                    try {
                        Classes.Console.ExecuteCommand("vel", GetPlayerFromVRRig(thestrangledleft).ActorNumber, GorillaLocomotion.GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0));
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
                        VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
                }
            }

            if (rightGrab)
            {
                if (thestrangled == null)
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (!rig.isLocal)
                        {
                            if (Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 0.2f)
                            {
                                thestrangled = rig;
                                if (PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                        89,
                                        false,
                                        999999f
                                    });
                                }
                                else
                                    VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
                            }
                        }
                    }
                } else
                {
                    if (Time.time > adminEventDelay)
                    {
                        adminEventDelay = Time.time + 0.05f;
                        Classes.Console.ExecuteCommand("tpnv", GetPlayerFromVRRig(thestrangled).ActorNumber, GorillaTagger.Instance.rightHandTransform.position);
                    }
                }
            }
            else
            {
                if (thestrangled != null)
                {
                    try
                    {
                        Classes.Console.ExecuteCommand("vel", GetPlayerFromVRRig(thestrangled).ActorNumber, GorillaLocomotion.GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0));
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
                        VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
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

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Classes.Console.ExecuteCommand("platf", ReceiverGroup.All, NewPointer.transform.position);
                }
            }
        }

        private static float lastnetscale = 1f;
        private static float scalenetdel = 0f;
        private static int lastplayercount = 0;
        public static void AdminNetworkScale()
        {
            if (Time.time > scalenetdel && (lastnetscale != VRRig.LocalRig.scaleFactor || PhotonNetwork.PlayerList.Length != lastplayercount))
            {
                Classes.Console.ExecuteCommand("scale", ReceiverGroup.All, VRRig.LocalRig.scaleFactor);
                scalenetdel = Time.time + 0.05f;
                lastnetscale = VRRig.LocalRig.scaleFactor;
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }

        public static void UnAdminNetworkScale() =>
            Classes.Console.ExecuteCommand("scale", ReceiverGroup.All, 1f);

        public static void LightningGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Classes.Console.ExecuteCommand("strike", ReceiverGroup.All, NewPointer.transform.position);
                }
            }
        }

        public static void LightningAura()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Classes.Console.ExecuteCommand("strike", ReceiverGroup.All, GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 1f, MathF.Sin((float)Time.frameCount / 30)));
            }
        }

        public static void LightningRain()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.1f;
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f), 10f, UnityEngine.Random.Range(-10f, 10f)), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
                VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                if (gunTarget && !PlayerIsLocal(gunTarget))
                {
                    adminEventDelay = Time.time + 0.1f;
                    Classes.Console.ExecuteCommand("kick", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                } else
                    Classes.Console.ExecuteCommand("strike", ReceiverGroup.All, Ray.point);
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

                if (gunLocked && lockTarget != null)
                {
                    TeleportPlayer(lockTarget.transform.position + lockTarget.transform.forward);
                    if (Time.time > adminEventDelay)
                        adminEventDelay = Time.time + 0.1f;
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        originalMePosition = GorillaTagger.Instance.bodyCollider.transform.position;
                        whereOriginalPlayerPos = gunTarget.transform.position;

                        int actorNumber = GetPlayerFromVRRig(gunTarget).ActorNumber;
                        Classes.Console.ExecuteCommand("platf", new int[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 16f, 0f), new Vector3(10f, 1f, 10f));
                        Classes.Console.ExecuteCommand("platf", new int[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 24f, 0f), new Vector3(10f, 1f, 10f));
                        
                        Classes.Console.ExecuteCommand("platf", new int[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(4f, 20f, 0f), new Vector3(1f, 10f, 10f));
                        Classes.Console.ExecuteCommand("platf", new int[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(-4f, 20f, 0f), new Vector3(1f, 10f, 10f));
                        
                        Classes.Console.ExecuteCommand("platf", new int[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 20f, 4f), new Vector3(10f, 10f, 1f));
                        Classes.Console.ExecuteCommand("platf", new int[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 20f, -4f), new Vector3(10f, 10f, 1f));

                        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(platform, 60f);
                        platform.GetComponent<Renderer>().material.color = Color.black;
                        platform.transform.position = new Vector3(0f, 20f, 0f);
                        platform.transform.localScale = new Vector3(10f, 1f, 10f);

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

                    TeleportPlayer(originalMePosition);
                    Classes.Console.ExecuteCommand("tpnv", GetPlayerFromVRRig(lockTarget).ActorNumber, whereOriginalPlayerPos);
                    Classes.Console.ExecuteCommand("unmuteall", GetPlayerFromVRRig(lockTarget).ActorNumber);
                }
            }
        }

        public static void EnableNoAdminIndicator()
        {
            Classes.Console.ExecuteCommand("nocone", ReceiverGroup.All, true);
            lastplayercount = -1;
        }

        public static void NoAdminIndicator()
        {
            if (!PhotonNetwork.InRoom)
                lastplayercount = -1;
            
            if (PhotonNetwork.PlayerList.Length != lastplayercount && PhotonNetwork.InRoom)
            {
                Classes.Console.ExecuteCommand("nocone", ReceiverGroup.All, true);
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }

        public static void AdminIndicatorBack() =>
            Classes.Console.ExecuteCommand("nocone", ReceiverGroup.All, false);

        public static void EnableAdminMenuUserTags() =>
            PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;

        private static bool lastInRoom = false;
        private static int lastPlayerCount = -1;
        public static void AdminUserTagSys(EventData data)
        {
            try
            {
                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
                if (data.Code == Classes.Console.ConsoleByte && sender != PhotonNetwork.LocalPlayer)
                {
                    object[] args = (object[])data.CustomData;
                    string command = (string)args[0];
                    switch (command)
                    {
                        case "confirmusing":
                            if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
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
                Classes.Console.ExecuteCommand("isusing", ReceiverGroup.All);
            
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

                    nametag.Value.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * nametag.Key.scaleFactor;

                    nametag.Value.transform.position = nametag.Key.headMesh.transform.position + nametag.Key.headMesh.transform.up * Visuals.GetTagDistance(nametag.Key);
                    nametag.Value.transform.LookAt(Camera.main.transform.position);
                    nametag.Value.transform.Rotate(0f, 180f, 0f);
                }
            }
        }

        public static void DisableAdminMenuUserTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
                UnityEngine.Object.Destroy(nametag.Value);

            nametags.Clear();
        }

        public static void JoinGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("join", GetPlayerFromVRRig(gunTarget).ActorNumber, searchText.ToUpper());
                    }
                }
            }
        }

        public static void JoinAll()
        {
            if (rightTrigger > 0.5f && Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.1f;
                Classes.Console.ExecuteCommand("join", ReceiverGroup.Others, searchText.ToUpper());
            }
        }

        public static void NotifyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("notify", GetPlayerFromVRRig(gunTarget).ActorNumber, searchText);
                    }
                }
            }
        }

        public static void NotifyAll()
        {
            if (rightTrigger > 0.5f && Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.1f;
                Classes.Console.ExecuteCommand("notify", ReceiverGroup.All, searchText);
            }
        }

        public static void GetMenuUsers()
        {
            Classes.Console.indicatorDelay = Time.time + 2f;
            Classes.Console.ExecuteCommand("isusing", ReceiverGroup.All);
        }

        private static bool lastLasering = false;
        public static void AdminLaser()
        {
            if (leftPrimary || rightPrimary)
            {
                Vector3 dir = rightPrimary ? VRRig.LocalRig.rightHandTransform.right : -VRRig.LocalRig.leftHandTransform.right;
                Vector3 startPos = (rightPrimary ? VRRig.LocalRig.rightHandTransform.position : VRRig.LocalRig.leftHandTransform.position) + (dir * 0.1f);
                try
                {
                    Physics.Raycast(startPos + (dir / 3f), dir, out var Ray, 512f, NoInvisLayerMask());
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                        Classes.Console.ExecuteCommand("silkick", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                } catch { }
                if (Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Classes.Console.ExecuteCommand("laser", ReceiverGroup.All, true, rightPrimary);
                }
            }
            bool isLasering = leftPrimary || rightPrimary;
            if (lastLasering && !isLasering)
                Classes.Console.ExecuteCommand("laser", ReceiverGroup.All, false, false);
            
            lastLasering = isLasering;
        }

        private static float beamDelay = 0f;
        public static void AdminBeam()
        {
            if (rightTrigger > 0.5f && Time.time > beamDelay)
            {
                beamDelay = Time.time + 0.05f;
                float h = (Time.frameCount / 180f) % 1f;
                Color color = Color.HSVToRGB(h, 1f, 1f);
                Classes.Console.ExecuteCommand("lr", ReceiverGroup.All, color.r, color.g, color.b, color.a, 0.5f, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f), GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30) * 100f, 0.5f, Mathf.Sin((float)Time.frameCount / 30) * 100f), 0.1f);
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
                Color color = Color.HSVToRGB(h, 1f, 1f);
                Classes.Console.ExecuteCommand("lr", ReceiverGroup.All, "lr", 0f, 1f, 1f, 0.3f, 0.25f, GorillaTagger.Instance.bodyCollider.transform.position, GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 1000f, 20f - (Time.time - startTimeTrigger));
            }
        }

        public static void FlyAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Classes.Console.ExecuteCommand("vel", ReceiverGroup.Others, new Vector3(0f, 10f, 0f));
            }
        }

        public static void AdminBringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Classes.Console.ExecuteCommand("tpnv", GetPlayerFromVRRig(gunTarget).ActorNumber, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
                    }
                }
            }
        }

        public static void BringAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Classes.Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
            }
        }

        public static void BringHandAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Classes.Console.ExecuteCommand("tpnv", ReceiverGroup.Others, TrueRightHand().position + TrueRightHand().forward);
            }
        }

        public static void BringHeadAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Classes.Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward);
            }
        }

        public static void OrbitAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Classes.Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f)));
            }
        }

        public static void ConfirmNotifyAllUsing() =>
            Classes.Console.ExecuteCommand("notify", ReceiverGroup.All, ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] == "goldentrophy" ? "Yes, I am @goldentrophy. I made the menu." : "Yes, I am " + ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] + ". I am a Console admin.");

        public static int[] oldCosmetics;
        public static int[] oldTryOn;
        public static void AdminSpoofCosmetics(bool forceRun = false)
        {
            if (PhotonNetwork.InRoom)
            {
                if (oldCosmetics != CosmeticsController.instance.currentWornSet.ToPackedIDArray() || forceRun)
                {
                    oldCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
                    string concat = "";
                    foreach (string cosmetic in CosmeticsController.instance.currentWornSet.ToDisplayNameArray())
                        concat += cosmetic;

                    if (!string.IsNullOrEmpty(concat))
                    {
                        Classes.Console.ExecuteCommand("cosmetic", ReceiverGroup.Others, concat);
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray());
                    }
                }
            }
        }

        public static void OnPlayerJoinSpoof(NetPlayer player)
        {
            string concat = "";
            foreach (string cosmetic in CosmeticsController.instance.currentWornSet.ToDisplayNameArray())
                concat += cosmetic;

            if (!string.IsNullOrEmpty(concat))
            {
                Classes.Console.ExecuteCommand("cosmetic", new int[] { player.ActorNumber }, concat);
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray());
            }
        }
    }
}
