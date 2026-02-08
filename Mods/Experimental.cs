/*
 * ii's Stupid Menu  Mods/Experimental.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RandomUtilities;
using static iiMenu.Utilities.RigUtilities;
using Console = iiMenu.Classes.Menu.Console;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace iiMenu.Mods
{
    public static class Experimental
    {
        public static void FixDuplicateButtons()
        {
            int duplicateButtons = 0;
            List<string> previousNames = new List<string>();
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
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully fixed " + duplicateButtons + " broken buttons.");
        }

        private static readonly Dictionary<Renderer, Material> oldMats = new Dictionary<Renderer, Material>();
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

        public static void OnlySerializeNecessary()
        {
            SerializePatch.OverrideSerialization = () =>
            {
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView);
                SendSerialize(GorillaTagger.Instance.myVRRig.reliableView);
                return false;
            };
        }

        public static void DumpSoundData()
        {
            string text = "Handtap Sound Data\n(from GorillaLocomotion.GTPlayer.Instance.materialData)";
            int i = 0;
            foreach (GTPlayer.MaterialData oneshot in GTPlayer.Instance.materialData)
            {
                try
                {
                    text += "\n====================================\n";
                    text += i + " ; " + oneshot.matName + " ; " + oneshot.slidePercent + "% ; " + (oneshot.audio == null ? "none" : oneshot.audio.name);
                }
                catch { LogManager.Log("Failed to log sound"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/SoundData.txt";

            File.WriteAllText(fileName, text);

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
            Process.Start(filePath);
        }

        public static void DumpCosmeticData()
        {
            string text = "Cosmetic Data\n(from CosmeticsController.instance.allCosmetics)";
            foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
            {
                try
                {
                    text += "\n====================================\n";
                    text += hat.itemName + " ; " + hat.displayName + " (override " + hat.overrideDisplayName + ") ; " + hat.cost + "SR ; canTryOn = " + hat.canTryOn;
                }
                catch { LogManager.Log("Failed to log hat"); }
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/CosmeticData.txt";

            File.WriteAllText(fileName, text);

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
            Process.Start(filePath);
        }

        public static void DecryptableCosmeticData()
        {
            string text = "";
            foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
            {
                try
                {
                    text += hat.itemName + ";;" + hat.overrideDisplayName + ";;" + hat.cost + "\n";
                }
                catch { LogManager.Log("Failed to log hat"); }
            }
            string fileName = $"{PluginInfo.BaseDirectory}/DecryptableCosmeticData.txt";

            File.WriteAllText(fileName, text);

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
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
                    text += i + " ; " + name;
                }
                catch { LogManager.Log("Failed to log RPC"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/RPCData.txt";

            File.WriteAllText(fileName, text);

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
            Process.Start(filePath);
        }

        public static void BlankPage()
        {
            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = Array.Empty<ButtonInfo>();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void CopyCustomGamemodeScript()
        {
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Copied map script to your clipboard.", 5000);
            GUIUtility.systemCopyBuffer = CustomGameMode.LuaScript;
        }

        public static void CopyCustomMapID()
        {
            string id = CustomMapManager.currentRoomMapModId._id.ToString();
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
            GUIUtility.systemCopyBuffer = id;
        }
        
        public static int restartIndex;
        public static float restartDelay;
        public static Vector3 restartPosition;
        public static string restartRoom;
        public static void SafeRestartGame()
        {
            string restartDataPath = $"{PluginInfo.BaseDirectory}/RestartData.txt";
            switch (restartIndex)
            {
                case 0:
                    if (File.Exists(restartDataPath))
                    {
                        string data = File.ReadAllText(restartDataPath);
                        restartRoom = data.Split(";")[0];
                        List<string> positionData = data.Split(";")[1].Split(",").ToList();
                        restartPosition = new Vector3(float.Parse(positionData[0]), float.Parse(positionData[1]), float.Parse(positionData[2]));
                        restartIndex = 3;
                    }
                    else
                    {
                        restartRoom = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "";
                        restartPosition = GTPlayer.Instance.transform.position;
                        restartIndex = 1;
                    }
                    restartDelay = Time.time + 6f;
                    break;
                case 1:
                    Settings.SavePreferences();
                    File.WriteAllText(restartDataPath, restartRoom + $";{restartPosition.x},{restartPosition.y},{restartPosition.z}");
                    restartIndex = 2;
                    break;
                case 2:
                    if (File.Exists(restartDataPath) && Time.time > restartDelay)
                    {
                        Important.RestartGame();
                        restartIndex = 4;
                    }
                    break;
                case 3:
                    if (!PhotonNetwork.InRoom && restartRoom != "")
                    {
                        if (Important.queueCoroutine == null && Time.time > restartDelay)
                            Important.QueueRoom(restartRoom);
                    }
                    else
                    {
                        TeleportPlayer(restartPosition);
                        File.Delete(restartDataPath);
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Restarted game with information.");
                        restartIndex = 4;
                        Buttons.GetIndex("Safe Restart Game").enabled = false;
                        Settings.SavePreferences();
                    }
                    break;
            }
        }

        private static float adminEventDelay;
        public static void AdminKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("kick", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                    }
                }
            }
        }

        public static List<string> platExcluded = new List<string>();
        public static void AdminPlatToggleGun(bool exclude)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        string id = GetPlayerFromVRRig(gunTarget).UserId;
                        adminEventDelay = Time.time + 0.1f;
                        if (exclude)
                        {
                            if (!platExcluded.Contains(id))
                            {
                                platExcluded.Add(id);
                                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Player is now excluded.");
                            } else
                                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Player is already excluded!");
                        } else
                        {
                            if (platExcluded.Contains(id))
                            {
                                platExcluded.Remove(id);
                                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Player is now included.");
                            } else
                                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Player is already included!");
                        }
                    }
                }
            }
        }

        public static void AdminKickAll() =>
            Console.ExecuteCommand("kickall", ReceiverGroup.All);
        
        public static void AdminCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("crash", GetPlayerFromVRRig(gunTarget).ActorNumber);
                    }
                }
            }
        }
        
        public static void AdminCrashAll() =>
            Console.ExecuteCommand("crash", ReceiverGroup.Others);
        
        public static void AdminLagSpikeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.5f;
                        Console.ExecuteCommand("sleep", GetPlayerFromVRRig(gunTarget).ActorNumber, 1000);
                    }
                }
            }
        }

        public static void AdminLagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > adminEventDelay)
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("sleep", GetPlayerFromVRRig(lockTarget).ActorNumber, 50);
                        RPCProtection();
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                gunLocked = false;
            }
        }

        public static void AdminLagSpikeAll() =>
            Console.ExecuteCommand("sleep", ReceiverGroup.Others, 1000);

        public static void AdminLagAll()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.1f;
                Console.ExecuteCommand("sleep", ReceiverGroup.Others, 50);
                RPCProtection();
            }
        }

        public static void AdminGiveFlyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > adminEventDelay)
                    {
                        if (lockTarget.rightThumb.calcT > 0.5f)
                        {
                            adminEventDelay = Time.time + 0.1f;
                            Console.ExecuteCommand("vel", GetPlayerFromVRRig(lockTarget).ActorNumber, lockTarget.headMesh.transform.forward * Movement._flySpeed);
                            RPCProtection();
                        }
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                gunLocked = false;
            }
        }

        public static bool AdminPlatformsLastLeft;
        public static bool AdminPlatformsLastRight;
        public static void AdminGivePlatforms()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > adminEventDelay)
                    {
                        if (lockTarget.leftMiddle.calcT > 0.5f && !AdminPlatformsLastLeft)
                        {
                            adminEventDelay = Time.time + 0.1f;
                            Console.ExecuteCommand("platf", GetPlayerFromVRRig(lockTarget).ActorNumber, lockTarget.leftHandTransform.position - new Vector3(0f, 0.2f, 0f), new Vector3(0.1f, 0.5f, 0.3f), lockTarget.leftHandTransform.eulerAngles, Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f, 10f );
                            RPCProtection();
                        }
                        if (lockTarget.rightMiddle.calcT > 0.5f && !AdminPlatformsLastRight)
                        {
                            adminEventDelay = Time.time + 0.1f;
                            Console.ExecuteCommand("platf", GetPlayerFromVRRig(lockTarget).ActorNumber, lockTarget.rightHandTransform.position - new Vector3(0f, 0.2f, 0f), new Vector3(0.1f, 0.5f, 0.3f), lockTarget.rightHandTransform.eulerAngles, Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f, 10f );
                            RPCProtection();
                        }
                        AdminPlatformsLastLeft = lockTarget.leftMiddle.calcT > 0.5f;
                        AdminPlatformsLastRight = lockTarget.rightMiddle.calcT > 0.5f;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                gunLocked = false;
            }
        }
        
        public static void AdminGiveTriggerFlyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > adminEventDelay)
                    {
                        if (lockTarget.rightIndex.calcT > 0.5f)
                        {
                            adminEventDelay = Time.time + 0.1f;
                            Console.ExecuteCommand("vel", GetPlayerFromVRRig(lockTarget).ActorNumber, lockTarget.headMesh.transform.forward * Movement._flySpeed);
                            RPCProtection();
                        }
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                gunLocked = false;
            }
        }

        public static Vector3 speedLastVel;
        public static void AdminGiveSpeedGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > adminEventDelay)
                    {
                        adminEventDelay = Time.time + 0.2f;
                        Console.ExecuteCommand("vel", GetPlayerFromVRRig(lockTarget).ActorNumber, (lockTarget.bodyTransform.position - speedLastVel) * 6f);
                        speedLastVel = lockTarget.bodyTransform.position;
                        RPCProtection();
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        speedLastVel = gunTarget.bodyTransform.position;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                gunLocked = false;
            }
        }
        
        public static void AdminGiveLowGravity()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > adminEventDelay)
                    {
                        adminEventDelay = Time.time + 0.2f;
                        Console.ExecuteCommand("vel", GetPlayerFromVRRig(lockTarget).ActorNumber, (lockTarget.bodyTransform.position - speedLastVel) * 5f + Vector3.up * 0.5f);
                        speedLastVel = lockTarget.bodyTransform.position;
                        RPCProtection();
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        speedLastVel = gunTarget.bodyTransform.position;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                gunLocked = false;
            }
        }

        public static void AdminVibrateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.2f;
                        Console.ExecuteCommand("vibrate", GetPlayerFromVRRig(gunTarget).ActorNumber, 3, 1f);
                    }
                }
            }
        }
        
        public static void AdminVibrateAll() =>
            Console.ExecuteCommand("vibrate", ReceiverGroup.Others, 3, 1f);
        
        public static void AdminBMuteGun(bool mute)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.5f;
                        Console.ExecuteCommand(mute ? "mute" : "unmute", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                    }
                }
            }
        }

        public static void AdminBlockGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 5f;
                        Console.ExecuteCommand("block", GetPlayerFromVRRig(gunTarget).ActorNumber, 300L);
                    }
                }
            }
        }

        public static void AdminABlockGun(bool Silent)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 5f;
                        Console.ExecuteCommand("notify", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).NickName + " has been blocked" + (Silent ? "" : " by " + ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId]) + ".");
                        Console.ExecuteCommand("block", GetPlayerFromVRRig(gunTarget).ActorNumber, 300L);
                        RPCProtection();
                    }
                }
            }
        }
        
        public static void AdminBMuteAll(bool mute) =>
            Console.ExecuteCommand(mute ? "muteall" : "unmuteall", ReceiverGroup.All);
        
        public static void AdminButtonPressGun(string key)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.8f;
                        Console.ExecuteCommand("controller", GetPlayerFromVRRig(gunTarget).ActorNumber, key, 1f, 1f);
                        RPCProtection();
                    }
                }
            }
        }

        public static void FlipMenuGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Right Hand");
                    }
                }
            }
        }

        public static void AdminEnableGun(bool enable, string mod)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("forceenable", GetPlayerFromVRRig(gunTarget).ActorNumber, mod, enable);
                    }
                }
            }
        }

        private static float jumpscareDelay;
        public static void AdminJumpscareGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > jumpscareDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        jumpscareDelay = Time.time + 0.2f;
                        Console.ExecuteCommand("toggle", GetPlayerFromVRRig(gunTarget).ActorNumber, "Jumpscare");
                    }
                }
            }
        }

        public static void AdminJumpscareAll() =>
            Console.ExecuteCommand("toggle", ReceiverGroup.Others, "Jumpscare");

        public static bool muted;
        public static void AdminMute()
        {
            if (leftTrigger > 0.5f && !muted)
            {
                Console.ExecuteCommand("forceenable", ReceiverGroup.Others, "Mute Microphone", true);
                muted = true;
            }
            else if (leftTrigger < 0.5f && muted)
            {
                Console.ExecuteCommand("forceenable", ReceiverGroup.Others, "Mute Microphone", false);
                muted = false;
            }
            
        }

        private static readonly Dictionary<VRRig, Coroutine> freezePool = new Dictionary<VRRig, Coroutine>();
        private static IEnumerator FreezeCoroutine(VRRig rig)
        {
            Console.ExecuteCommand("forceenable", GetPlayerFromVRRig(rig).ActorNumber, "Zero Gravity", true);
            Vector3 pos = rig.transform.position;
            while (GorillaParent.instance.vrrigs.Contains(rig))
            {
                Console.ExecuteCommand("tp", GetPlayerFromVRRig(rig).ActorNumber, pos);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public static void AdminFreezeGun(bool freeze)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        switch (freeze)
                        {
                            case true when !freezePool.ContainsKey(gunTarget):
                                freezePool.Add(gunTarget, CoroutineManager.instance.StartCoroutine(FreezeCoroutine(gunTarget)));
                                break;
                            case false when freezePool.ContainsKey(gunTarget):
                                CoroutineManager.instance.StopCoroutine(freezePool[gunTarget]);
                                Console.ExecuteCommand("forceenable", GetPlayerFromVRRig(gunTarget).ActorNumber, "Zero Gravity", false);
                                freezePool.Remove(gunTarget);
                                break;
                        }
                    }
                }
            }
        }

        public static void AdminTeleportGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("tp", ReceiverGroup.Others, NewPointer.transform.position);
                }
            }
        }

        public static void AdminFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("vel", GetPlayerFromVRRig(gunTarget).ActorNumber, new Vector3(0f, 50f, 0f));
                    }
                }
            }
        }

        public static void AdminCrashBypassGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        if (ServerData.Administrators.ContainsKey(GetPlayerFromVRRig(gunTarget).UserId))
                            return;
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("tp", GetPlayerFromVRRig(gunTarget).ActorNumber, new Vector3(0f, 1000000f, 0f));
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

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("togglemenu", GetPlayerFromVRRig(gunTarget).ActorNumber, enable);
                    }
                }
            }
        }

        private static readonly List<int> FullActorNumbers = new List<int>();
        public static void FullToggleMenu(int actorNumber, bool enable)
        {
            if (enable)
            {
                if (!FullActorNumbers.Contains(actorNumber))
                {
                    Console.ExecuteCommand("forceenable", actorNumber, "Disable Autosave", true);
                    Console.ExecuteCommand("forceenable", actorNumber, "Load Preferences");
                    FullActorNumbers.Add(actorNumber);
                }
            } else
            {
                if (FullActorNumbers.Contains(actorNumber))
                {
                    Console.ExecuteCommand("toggle", actorNumber, "Save Preferences");
                    Console.ExecuteCommand("forceenable", actorNumber, "Disable Autosave", true);
                    Console.ExecuteCommand("forceenable", actorNumber, "Panic", true);
                    FullActorNumbers.Remove(actorNumber);
                }
            }

            Console.ExecuteCommand("togglemenu", actorNumber, enable);
        }

        public static void AdminFullLockdownGun(bool enable)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        FullToggleMenu(GetPlayerFromVRRig(gunTarget).ActorNumber, enable);
                    }
                }
            }
        }

        private static bool lastInRoom2;
        private static int lastPlayerCount2 = -1;
        public static void AdminLockdownAll(bool enable)
        {
            if (PhotonNetwork.InRoom && (!lastInRoom2 || PhotonNetwork.PlayerList.Length != lastPlayerCount2))
                Console.ExecuteCommand("togglemenu", ReceiverGroup.Others, enable);

            lastInRoom2 = PhotonNetwork.InRoom;
            lastPlayerCount2 = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount2 = -1;
        }

        public static void AdminFullLockdownAll(bool enable)
        {
            foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                FullToggleMenu(Player.ActorNumber, enable);
        }

        private static float stdell;
        private static VRRig thestrangled;
        private static VRRig thestrangledleft;
        public static void AdminStrangle()
        {
            if (leftGrab)
            {
                if (thestrangledleft == null)
                {
                    foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal).Where(rig => Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 0.2f))
                    {
                        thestrangledleft = rig;
                        if (PhotonNetwork.InRoom)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, true, 999999f);
                        else
                            VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
                    }
                }
                else
                {
                    if (Time.time > stdell)
                    {
                        stdell = Time.time + 0.05f;
                        Console.ExecuteCommand("tp", GetPlayerFromVRRig(thestrangledleft).ActorNumber, GorillaTagger.Instance.leftHandTransform.position);
                    }
                }
            }
            else
            {
                if (thestrangledleft != null)
                {
                    try {
                        Console.ExecuteCommand("tp", GetPlayerFromVRRig(thestrangledleft).ActorNumber, GorillaTagger.Instance.leftHandTransform.position);
                        Console.ExecuteCommand("vel", GetPlayerFromVRRig(thestrangledleft).ActorNumber, GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0));
                    } catch { }
                    thestrangledleft = null;
                    if (PhotonNetwork.InRoom)
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, true, 999999f);
                    else
                        VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
                }
            }

            if (rightGrab)
            {
                if (thestrangled == null)
                {
                    foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal).Where(rig => Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 0.2f))
                    {
                        thestrangled = rig;
                        if (PhotonNetwork.InRoom)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, false, 999999f);
                        else
                            VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
                    }
                } else
                {
                    if (Time.time > adminEventDelay)
                    {
                        adminEventDelay = Time.time + 0.05f;
                        Console.ExecuteCommand("tp", GetPlayerFromVRRig(thestrangled).ActorNumber, GorillaTagger.Instance.rightHandTransform.position);
                    }
                }
            }
            else
            {
                if (thestrangled != null)
                {
                    try
                    {
                        Console.ExecuteCommand("tp", GetPlayerFromVRRig(thestrangled).ActorNumber, GorillaTagger.Instance.rightHandTransform.position);
                        Console.ExecuteCommand("vel", GetPlayerFromVRRig(thestrangled).ActorNumber, GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0));
                    } catch { }
                    thestrangled = null;
                    if (PhotonNetwork.InRoom)
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, false, 999999f);
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
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("platf", ReceiverGroup.All, NewPointer.transform.position);
                }
            }
        }

        public static void AdminRandomObjectGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("platf", ReceiverGroup.All, NewPointer.transform.position, RandomVector3(), RandomVector3(360f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                }
            }
        }

        private static float lastnetscale = 1f;
        private static float scalenetdel;
        private static int lastplayercount;
        public static void AdminNetworkScale()
        {
            if (Time.time > scalenetdel && (!Mathf.Approximately(lastnetscale, VRRig.LocalRig.scaleFactor) || PhotonNetwork.PlayerList.Length != lastplayercount))
            {
                Console.ExecuteCommand("scale", ReceiverGroup.All, VRRig.LocalRig.scaleFactor);
                scalenetdel = Time.time + 0.05f;
                lastnetscale = VRRig.LocalRig.scaleFactor;
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }

        public static void UnAdminNetworkScale() =>
            Console.ExecuteCommand("scale", ReceiverGroup.All, 1f);

        public static void LightningGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("strike", ReceiverGroup.All, NewPointer.transform.position);
                }
            }
        }

        public static void LightningAura()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("strike", ReceiverGroup.All, GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 1f, MathF.Sin((float)Time.frameCount / 30)));
            }
        }

        public static void LightningRain()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.1f;
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-10f, 10f), 10f, Random.Range(-10f, 10f)), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
                VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                if (gunTarget && !gunTarget.IsLocal())
                {
                    adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("kick", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                } else
                    Console.ExecuteCommand("strike", ReceiverGroup.All, Ray.point);
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

                if (gunLocked && lockTarget != null)
                {
                    TeleportPlayer(lockTarget.transform.position + lockTarget.transform.forward);
                    if (Time.time > adminEventDelay)
                        adminEventDelay = Time.time + 0.1f;
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        originalMePosition = GorillaTagger.Instance.bodyCollider.transform.position;
                        whereOriginalPlayerPos = gunTarget.transform.position;

                        int actorNumber = GetPlayerFromVRRig(gunTarget).ActorNumber;
                        Console.ExecuteCommand("platf", new[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 16f, 0f), new Vector3(10f, 1f, 10f));
                        Console.ExecuteCommand("platf", new[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 24f, 0f), new Vector3(10f, 1f, 10f));
                        
                        Console.ExecuteCommand("platf", new[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(4f, 20f, 0f), new Vector3(1f, 10f, 10f));
                        Console.ExecuteCommand("platf", new[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(-4f, 20f, 0f), new Vector3(1f, 10f, 10f));
                        
                        Console.ExecuteCommand("platf", new[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 20f, 4f), new Vector3(10f, 10f, 1f));
                        Console.ExecuteCommand("platf", new[] { actorNumber, PhotonNetwork.LocalPlayer.ActorNumber }, new Vector3(0f, 20f, -4f), new Vector3(10f, 10f, 1f));

                        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Object.Destroy(platform, 60f);
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
                    Console.ExecuteCommand("tpnv", GetPlayerFromVRRig(lockTarget).ActorNumber, whereOriginalPlayerPos);
                    Console.ExecuteCommand("unmuteall", GetPlayerFromVRRig(lockTarget).ActorNumber);
                }
            }
        }

        public static void EnableNoAdminIndicator()
        {
            Console.ExecuteCommand("nocone", ReceiverGroup.All, true);
            lastplayercount = -1;
        }

        public static void NoAdminIndicator()
        {
            if (!PhotonNetwork.InRoom)
                lastplayercount = -1;
            
            if (PhotonNetwork.PlayerList.Length != lastplayercount && PhotonNetwork.InRoom)
            {
                Console.ExecuteCommand("nocone", ReceiverGroup.All, true);
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }

        public static void AdminIndicatorBack() =>
            Console.ExecuteCommand("nocone", ReceiverGroup.All, false);

        public static void EnableAdminMenuUserTags()
        {
            if (!userTagHooked)
            {
                userTagHooked = true;
                PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;
            }
        }

        private static bool lastInRoom;
        private static int lastPlayerCount = -1;

        public static bool userTagHooked;
        public static void AdminUserTagSys(EventData data)
        {
            try
            {
                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);
                if (data.Code == Console.ConsoleByte && sender != PhotonNetwork.LocalPlayer)
                {
                    object[] args = (object[])data.CustomData;
                    string command = (string)args[0];
                    switch (command)
                    {
                        case "confirmusing":
                            if (Buttons.GetIndex("Menu User Name Tags").enabled && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            {
                                VRRig vrrig = GetVRRigFromPlayer(sender);
                                if (!nametags.TryGetValue(vrrig, out var nametag))
                                {
                                    GameObject go = new GameObject("iiMenu_Nametag");
                                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                    TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
                                    textMesh.fontSize = 4.8f;
                                    textMesh.alignment = TextAlignmentOptions.Center;

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Console.GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);

                                    nametags.Add(vrrig, go);
                                } else
                                {
                                    TextMeshPro textMesh = nametag.GetComponent<TextMeshPro>();

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Console.GetMenuTypeName((string)args[2]);

                                    if (Visuals.nameTagChams)
                                        textMesh.Chams();
                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);
                                }
                            }
                            if (Buttons.GetIndex("Conduct Menu Users").enabled)
                            {
                                if (!onConduct.ContainsKey(sender.UserId))
                                {
                                    bool add = ServerData.Administrators.ContainsKey(sender.UserId);
                                    string txt = sender.NickName + " - " + ToTitleCase((string)args[2]);
                                    if (add)
                                        txt = "<color=red>" + txt + "</color>";
                                    onConduct.Add(sender.UserId, txt);
                                }
                            }
                            if (Buttons.GetIndex("Admin Find User").enabled)
                                isUserFound = true;
                            break;
                    }
                }
            }
            catch { }
        }

        private static readonly Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject>();
        public static void AdminMenuUserTags()
        {
            if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
                Console.ExecuteCommand("isusing", ReceiverGroup.All);
            
            lastInRoom = PhotonNetwork.InRoom;
            lastPlayerCount = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount = -1;
            
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags.ToList())
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    Object.Destroy(nametag.Value);
                    nametags.Remove(nametag.Key);
                } else
                {
                    nametag.Value.GetComponent<TextMeshPro>().fontStyle = activeFontStyle;
                    nametag.Value.GetComponent<TextMeshPro>().font = activeFont;

                    if (Visuals.nameTagChams)
                        nametag.Value.GetComponent<TextMeshPro>().Chams();

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
                Object.Destroy(nametag.Value);

            nametags.Clear();
        }

        public static bool tracerTagHooked;
        public static void EnableAdminMenuUserTracers()
        {
            if (!tracerTagHooked)
            {
                tracerTagHooked = true;
                PhotonNetwork.NetworkingClient.EventReceived += AdminTracerSys;
            }
        }

        private static readonly Dictionary<VRRig, string> menuUsers = new Dictionary<VRRig, string>();
        public static void AdminTracerSys(EventData data)
        {
            try
            {
                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);
                if (data.Code == Console.ConsoleByte && sender != PhotonNetwork.LocalPlayer)
                {
                    object[] args = (object[])data.CustomData;
                    string command = (string)args[0];
                    switch (command)
                    {
                        case "confirmusing":
                            if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            {
                                VRRig vrrig = GetVRRigFromPlayer(sender);
                                if (!nametags.TryGetValue(vrrig, out var nametag))
                                {
                                    GameObject go = new GameObject("iiMenu_Nametag");
                                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                    TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
                                    textMesh.fontSize = 48;
                                    textMesh.alignment = TextAlignmentOptions.Center;

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Console.GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);

                                    nametags.Add(vrrig, go);
                                }
                                else
                                {
                                    TextMeshPro textMesh = nametag.GetComponent<TextMeshPro>();

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Console.GetMenuTypeName((string)args[2]);

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

        public static void MenuUserTracers()
        {
            if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
                Console.ExecuteCommand("isusing", ReceiverGroup.All);

            lastInRoom = PhotonNetwork.InRoom;
            lastPlayerCount = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount = -1;

            if (Visuals.DoPerformanceCheck())
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            _ = Buttons.GetIndex("Hidden on Camera").enabled;
            float lineWidth = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            Color menuColor = backgroundColor.GetCurrentColor();

            foreach (KeyValuePair<VRRig, string> userData in menuUsers)
            {
                VRRig playerRig = userData.Key;
                if (playerRig.isLocal)
                    continue;

                Color lineColor = Console.GetMenuTypeName(userData.Value);

                LineRenderer line = Visuals.GetLineRender();

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, playerRig.transform.position);
            }
        }

        public static readonly Dictionary<string, string> onConduct = new Dictionary<string, string>();
        public static void ConsoleOnConduct()
        {
            if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount) && !Buttons.GetIndex("Menu User Name Tags").enabled)
                Console.ExecuteCommand("isusing", ReceiverGroup.All);

            string conductText = "";
            conductText += "<color=red>"+PhotonNetwork.LocalPlayer.NickName+" - "+ToTitleCase(Console.MenuName)+"</color>\\n";
            foreach (KeyValuePair<string, string> item in onConduct)
            {
                if (GetPlayerFromID(item.Key) == null)
                    onConduct.Remove(item.Key);
                else
                    conductText += item.Value + "\\n";
            }
            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().text = conductText;
        }

        public static float FindUserTime;
        public static bool isUserFound;
        public static void AdminFindUser()
        {
            if (Time.time < FindUserTime)
                return;

            if (!PhotonNetwork.InRoom)
            {
                Important.JoinRandom();
                isUserFound = false;
                FindUserTime = Time.time + 7f;
            }
            else
            {
                if (isUserFound)
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Found menu user!");
                    Buttons.GetIndex("Admin Find User").enabled = false;
                    isUserFound = false;
                    return;
                }
                NotificationManager.SendNotification("Nobody found, searching for players.");
                NetworkSystem.Instance.ReturnToSinglePlayer();
                FindUserTime = Time.time + 2f;
            }
        }

        private static float thingdeb;
        public static void AdminPunchMod()
        {
            if (Time.time > thingdeb)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    bool leftHand = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f;
                    bool rightHand = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f;

                    if (!rig.isLocal && (leftHand || rightHand))
                    {
                        Vector3 vel = rightHand ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0);

                        Console.ExecuteCommand("vel", GetPlayerFromVRRig(rig).ActorNumber, vel);
                        thingdeb = Time.time + 0.1f;
                    }
                }
            }
        }

        public static string targetRoom;
        public static void GetTargetRoom() =>
            PromptText("What room would you like the users to join?", () => targetRoom = keyboardInput, null, "Done", "Cancel");

        public static void JoinGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("join", GetPlayerFromVRRig(gunTarget).ActorNumber, targetRoom.ToUpper());
                    }
                }
            }
        }

        public static void JoinAll() =>
            PromptText("What room would you like the users to join?", () => Console.ExecuteCommand("join", ReceiverGroup.Others, keyboardInput.ToUpper()), null, "Done", "Cancel");

        public static string targetNotification;
        public static void GetTargetNotification()
        {
            PromptText("What notification would you like to send?", () =>
            {
                targetNotification = keyboardInput;
                Buttons.GetIndex("NotifLabel").overlapText = "Notif: " + keyboardInput;
            }, null, "Done", "Cancel");
        }

        public static void NotifySelf() =>
            Console.ExecuteCommand("notify", PhotonNetwork.LocalPlayer.ActorNumber, targetNotification);

        public static void NotifyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("notify", GetPlayerFromVRRig(gunTarget).ActorNumber, targetNotification);
                    }
                }
            }
        }

        public static void NotifyAll() =>
            Console.ExecuteCommand("notify", ReceiverGroup.All, targetNotification);

        public static void GetMenuUsers()
        {
            Console.indicatorDelay = Time.time + 2f;
            Console.ExecuteCommand("isusing", ReceiverGroup.All);
        }

        private static bool lastLasering;
        public static void AdminLaser()
        {
            if (leftPrimary || rightPrimary)
            {
                Vector3 dir = rightPrimary ? VRRig.LocalRig.rightHandTransform.right : -VRRig.LocalRig.leftHandTransform.right;
                Vector3 startPos = (rightPrimary ? VRRig.LocalRig.rightHandTransform.position : VRRig.LocalRig.leftHandTransform.position) + dir * 0.1f;
                try
                {
                    Physics.Raycast(startPos + dir / 3f, dir, out var Ray, 512f, NoInvisLayerMask());
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                        Console.ExecuteCommand("silkick", ReceiverGroup.All, GetPlayerFromVRRig(gunTarget).UserId);
                } catch { }
                if (Time.time > adminEventDelay)
                {
                    adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("laser", ReceiverGroup.All, true, rightPrimary);
                }
            }
            bool isLasering = leftPrimary || rightPrimary;
            if (lastLasering && !isLasering)
                Console.ExecuteCommand("laser", ReceiverGroup.All, false, false);
            
            lastLasering = isLasering;
        }

        private static float beamDelay;
        public static void AdminBeam()
        {
            if (rightTrigger > 0.5f && Time.time > beamDelay)
            {
                beamDelay = Time.time + 0.05f;
                float h = Time.frameCount / 180f % 1f;
                Color color = Color.HSVToRGB(h, 1f, 1f);
                Console.ExecuteCommand("lr", ReceiverGroup.All, color.r, color.g, color.b, color.a, 0.5f, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f), GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30) * 100f, 0.5f, Mathf.Sin((float)Time.frameCount / 30) * 100f), 0.1f);
            }
        }

        private static float startTimeTrigger;
        private static bool lastTriggerLaserSpam;
        public static void AdminFractals()
        {
            if (rightTrigger > 0.5f && !lastTriggerLaserSpam)
                startTimeTrigger = Time.time;

            lastTriggerLaserSpam = rightTrigger > 0.5f;

            if (rightTrigger > 0.5f && Time.time > beamDelay)
            {
                beamDelay = Time.time + 0.5f;
                float h = Time.frameCount / 180f % 1f;
                Color.HSVToRGB(h, 1f, 1f);
                Console.ExecuteCommand("lr", ReceiverGroup.All, "lr", 0f, 1f, 1f, 0.3f, 0.25f, GorillaTagger.Instance.bodyCollider.transform.position, GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 1000f, 20f - (Time.time - startTimeTrigger));
            }
        }

        public static void FlyAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("vel", ReceiverGroup.Others, new Vector3(0f, 10f, 0f));
            }
        }

        public static void BouncyAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;

                var users = Console.userDictionary.Keys.Where(u => !u.IsLocal).ToList();

                foreach (var rig in users.Select(player => GetVRRigFromPlayer(player)))
                {
                    if (!Physics.Raycast(rig.bodyTransform.position - new Vector3(0f, 0.2f, 0f), Vector3.down,
                            out RaycastHit hit, 512f, GTPlayer.Instance.locomotionEnabledLayers)) continue;
                    if (!(hit.distance < 0.1f)) continue;
                    Vector3 surfaceNormal = hit.normal;
                    Vector3 bodyVelocity = rig.LatestVelocity();
                    Vector3 reflectedVelocity = Vector3.Reflect(bodyVelocity, surfaceNormal);
                    Vector3 finalVelocity = reflectedVelocity * 2f;
                    Console.ExecuteCommand("vel", rig.GetPlayer().ActorNumber, finalVelocity);
                }
            }
        }



        public static void AdminBringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        adminEventDelay = Time.time + 0.1f;
                        Console.ExecuteCommand("tpnv", GetPlayerFromVRRig(gunTarget).ActorNumber, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
                    }
                }
            }
        }

        public static void BringAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
            }
        }

        public static void AdminOrganizeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > adminEventDelay)
                {
                    var users = Console.userDictionary.Keys.Where(u => !u.IsLocal).ToList();
                    if (users.Count == 1)
                    {
                        Console.ExecuteCommand("tpnv", users.FirstOrDefault().ActorNumber, NewPointer.transform.position);
                        return;
                    }

                    float spacing = 0.8f;
                    for (int i = 0; i < users.Count; i++)
                    {
                        Console.ExecuteCommand("tpnv", users[i].ActorNumber, NewPointer.transform.position - Vector3.right * ((users.Count - 1) * spacing / 2f) + Vector3.right * (spacing * i));
                    }
                    adminEventDelay = Time.time + 0.05f;
                }
            }
        }

        public static void BringHandAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("tpnv", ReceiverGroup.Others, ControllerUtilities.GetTrueRightHand().position + ControllerUtilities.GetTrueRightHand().forward);
            }
        }

        public static void BringHeadAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward);
            }
        }

        public static void OrbitAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f)));
            }
        }

        public static void ConfirmNotifyAllUsing() =>
            Console.ExecuteCommand("notify", ReceiverGroup.All, ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] == "goldentrophy" ? "Yes, I am @goldentrophy. I made the menu." : ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] == "kingofnetflix" ? "Yes, I am @kingofnetflix. I am the developer for ii's Stupid Menu." : "Yes, I am " + ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] + ". I am a Console admin.");

        public static int[] oldCosmetics;
        public static int[] oldTryOn;
        public static void AdminSpoofCosmetics(bool forceRun = false)
        {
            if (PhotonNetwork.InRoom)
            {
                if (oldCosmetics != CosmeticsController.instance.currentWornSet.ToPackedIDArray() || forceRun)
                {
                    oldCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
                    string[] cosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray().Where(c => !string.Equals(c, "NOTHING", StringComparison.OrdinalIgnoreCase)).ToArray();

                    Console.ExecuteCommand("cosmetics", ReceiverGroup.Others, cosmetics);
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
                }
            }
        }

        public static void OnPlayerJoinSpoof(NetPlayer player)
        {
            string[] cosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray().Where(c => !string.Equals(c, "NOTHING", StringComparison.OrdinalIgnoreCase)).ToArray();

            Console.ExecuteCommand("cosmetics", new[] { player.ActorNumber }, cosmetics);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
        }
    }
}
