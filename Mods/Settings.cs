/*
 * ii's Stupid Menu  Mods/Settings.cs
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

using GorillaExtensions;
using GorillaLocomotion;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.RigUtilities;
using Console = iiMenu.Classes.Menu.Console;
using Object = UnityEngine.Object;

namespace iiMenu.Mods
{
    public static class Settings
    {
        public static void Search() // This took me like 4 hours
        {
            isSearching = !isSearching;

            pageNumber = 0;
            keyboardInput = "";

            if (isSearching)
                SpawnKeyboard();
            else
                DestroyKeyboard();
        }

        public static void SpawnKeyboard()
        {
            isKeyboardPc = isOnPC || toggleButtonActive && keyboardWithToggleButton;
            inTextInput = true;
            keyboardInput = "";

            shift = false;
            lockShift = false;

            if (isKeyboardPc)
                lastPressedKeys.Add(KeyCode.Q);

            if (!isKeyboardPc)
            {
                if (VRKeyboard == null)
                {
                    VRKeyboard = LoadObject<GameObject>("VRKeyboard");
                    VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                    VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                    menuSpawnPosition = VRKeyboard.transform.Find("MenuSpawnPosition").gameObject;
                    VRKeyboard.transform.Find("Canvas").AddComponent<ColorChanger>().colors = textColors[1];

                    VRKeyboard.transform.localScale *= scaleWithPlayer ? GTPlayer.Instance.scale * menuScale : menuScale;
                    menuSpawnPosition.transform.localScale *= scaleWithPlayer ? GTPlayer.Instance.scale * menuScale : menuScale;

                    ColorChanger backgroundColorChanger = VRKeyboard.transform.Find("Background").gameObject.AddComponent<ColorChanger>();
                    backgroundColorChanger.colors = backgroundColor;

                    foreach (GameObject key in VRKeyboard.transform.Find("Seperate").Children()
                        .Select(t => t.gameObject)
                        .Concat(new[] { VRKeyboard.transform.Find("Keys/default").gameObject }))
                    {
                        ColorChanger keyColorChanger = key.AddComponent<ColorChanger>();
                        keyColorChanger.colors = buttonColors[0];
                    }

                    if (shouldOutline)
                        OutlineObject(VRKeyboard.transform.Find("Background").gameObject, true);

                    var keys = new[] { "Numbers", "Letters", "Special", "Seperate" }
                        .Select(name => VRKeyboard.transform.Find(name))
                        .Where(t => t != null)
                        .SelectMany(t => t.Children())
                        .Select(t => t.gameObject); 

                    foreach (GameObject v in keys)
                    {
                        v.AddComponent<KeyboardKey>().key = v.name;
                        v.layer = 2;

                        if (shouldOutline)
                            OutlineObject(v, true);
                    }
                }
            }

            if (lKeyReference == null)
            {
                lKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                lKeyReference.transform.parent = GorillaTagger.Instance.leftHandTransform;
                lKeyReference.GetComponent<Renderer>().material.color = backgroundColor.GetColor(0);
                lKeyReference.transform.localPosition = pointerOffset;
                lKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                lKeyCollider = lKeyReference.GetComponent<SphereCollider>();

                ColorChanger colorChanger = lKeyReference.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;
            }

            if (rKeyReference == null)
            {
                rKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rKeyReference.transform.parent = GorillaTagger.Instance.rightHandTransform;
                rKeyReference.GetComponent<Renderer>().material.color = backgroundColor.GetColor(0);
                rKeyReference.transform.localPosition = pointerOffset;
                rKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                rKeyCollider = rKeyReference.GetComponent<SphereCollider>();

                ColorChanger colorChanger = rKeyReference.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;
            }
        }

        public static void DestroyKeyboard()
        {
            inTextInput = false;
            isKeyboardPc = false;

            if (lKeyReference != null)
            {
                Object.Destroy(lKeyReference);
                lKeyReference = null;
            }

            if (rKeyReference != null)
            {
                Object.Destroy(rKeyReference);
                rKeyReference = null;
            }

            if (VRKeyboard != null)
            {
                Object.Destroy(VRKeyboard);
                VRKeyboard = null;
            }

            if (TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet") && isOnPC)
            {
                isOnPC = false;
                TPC.transform.position = TPC.transform.parent.position;
                TPC.transform.rotation = TPC.transform.parent.rotation;
            }
        }

        public static void GlobalReturn()
        {
            NotificationManager.ClearAllNotifications();
            Toggle(Buttons.buttons[Buttons.CurrentCategoryIndex][Buttons.GetCategory("Main")].buttonText, true);
            StopCurrentPrompt();
        }

        public static void StopCurrentPrompt() =>
            prompts.RemoveAt(0);

        public static GameObject TutorialObject;
        public static LineRenderer TutorialSelector;
        public static void ShowTutorial()
        {
            if (TutorialObject != null)
                Object.Destroy(TutorialObject);

            TutorialObject = LoadObject<GameObject>("Tutorial");

            TutorialObject.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1f + Vector3.up * 0.25f;
            TutorialObject.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            string videoName = "q2";
            switch (ControllerUtilities.GetLeftControllerType())
            {
                case ControllerUtilities.ControllerType.Unknown:
                case ControllerUtilities.ControllerType.Quest2:
                    videoName = "q2";
                    break;
                case ControllerUtilities.ControllerType.Quest3:
                    videoName = "q3";
                    break;
                case ControllerUtilities.ControllerType.ValveIndex:
                    videoName = "index";
                    break;
                case ControllerUtilities.ControllerType.VIVE:
                    videoName = "vive";
                    break;
            }

            VideoPlayer videoPlayer = TutorialObject.transform.Find("Video").GetComponent<VideoPlayer>();
            videoPlayer.url = $"{PluginInfo.ServerResourcePath}/Videos/Tutorial/tutorial-{videoName}.mp4";
            videoPlayer.isLooping = true;

            videoPlayer.AddComponent<TutorialButton>().buttonType = TutorialButton.ButtonType.Pause;

            TutorialObject.transform.Find("Close").AddComponent<TutorialButton>().buttonType = TutorialButton.ButtonType.Close;
        }

        private static bool lastTrigger;
        public static void UpdateTutorial()
        {
            if (Vector3.Distance(TutorialObject.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) > 2f)
            {
                TutorialObject.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1f + Vector3.up * 0.25f;
                TutorialObject.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            }

            if (TutorialSelector == null)
            {
                TutorialSelector = new GameObject("iiMenu_TutorialSelector").AddComponent<LineRenderer>();
                TutorialSelector.material.shader = Shader.Find("Sprites/Default");

                TutorialSelector.startWidth = 0.01f;
                TutorialSelector.endWidth = 0.01f;

                TutorialSelector.positionCount = 2;

                TutorialSelector.useWorldSpace = true;
            }

            TutorialSelector.startColor = BrightenColor(new Color32(255, 128, 0, 128));
            TutorialSelector.endColor = BrightenColor(new Color32(255, 102, 0, 128));

            Vector3 Direction = ControllerUtilities.GetTrueRightHand().forward;
            Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position + Direction / 4f, Direction, out var Ray, 512f, NoInvisLayerMask());
            if (!XRSettings.isDeviceActive)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
            }

            TutorialSelector.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
            TutorialSelector.SetPosition(1, Ray.point == Vector3.zero ? GorillaTagger.Instance.rightHandTransform.position : Ray.point);

            if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && !lastTrigger)
            {
                TutorialButton gunTarget = Ray.collider.GetComponentInParent<TutorialButton>();
                if (gunTarget)
                    gunTarget.ClickButton();
            }

            lastTrigger = rightTrigger > 0.5f || Mouse.current.leftButton.isPressed;
        }

        public class TutorialButton : MonoBehaviour
        {
            public enum ButtonType
            {
                Pause,
                Close
            }

            public ButtonType buttonType;
            public void ClickButton()
            {
                switch (buttonType)
                {
                    case ButtonType.Pause:
                        VideoPlayer videoPlayer = TutorialObject.transform.Find("Video").GetComponent<VideoPlayer>();
                        if (videoPlayer.isPlaying)
                            videoPlayer.Pause();
                        else
                            videoPlayer.Play();

                        break;
                    case ButtonType.Close:
                        Destroy(TutorialObject);
                        Destroy(TutorialSelector.gameObject);
                        break;
                }
            }
        }

        public static void ShowDebug()
        {
            int category = Buttons.GetCategory("Temporary Category");

            string version = PluginInfo.Version;
            if (PluginInfo.BetaBuild) version = "<color=blue>Beta</color> " + version;
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugMenuName", overlapText = "<color=grey><b>ii's Stupid Menu </b></color>" + version, label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugColor", overlapText = "Loading...", label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugName", overlapText = "Loading...", label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugId", overlapText = "Loading...", label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugClip", overlapText = "Loading...", label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugFps", overlapText = "Loading...", label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugRoomA", overlapText = "Loading...", label = true });
            Buttons.AddButton(category, new ButtonInfo { buttonText = "DebugRoomB", overlapText = "Loading...", label = true });

            Debug();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static bool hideId;
        public static void Debug()
        {
            string red = "<color=red>" + MathF.Floor(PlayerPrefs.GetFloat("redValue") * 255f) + "</color>";
            string green = ", <color=green>" + MathF.Floor(PlayerPrefs.GetFloat("greenValue") * 255f) + "</color>";
            string blue = ", <color=blue>" + MathF.Floor(PlayerPrefs.GetFloat("blueValue") * 255f) + "</color>";
            Buttons.GetIndex("DebugColor").overlapText = "Color: " + red + green + blue;

            string master = PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient ? "<color=red> [Master]</color>" : "";
            Buttons.GetIndex("DebugName").overlapText = PhotonNetwork.LocalPlayer.NickName + master;

            Buttons.GetIndex("DebugId").overlapText = "<color=green>ID: </color>" + (hideId ? "Hidden" : PhotonNetwork.LocalPlayer.UserId);
            Buttons.GetIndex("DebugClip").overlapText = "<color=green>Clip: </color>" + (GUIUtility.systemCopyBuffer.Length > 25 ? GUIUtility.systemCopyBuffer[..25] : GUIUtility.systemCopyBuffer);
            Buttons.GetIndex("DebugFps").overlapText = "<b>" + lastDeltaTime + "</b> FPS <b>" + PhotonNetwork.GetPing() + "</b> Ping";
            Buttons.GetIndex("DebugRoomA").overlapText = "<color=blue>" + NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper() + "</color> " + PhotonNetwork.PlayerList.Length + " Players";

            string priv = PhotonNetwork.InRoom ? NetworkSystem.Instance.SessionIsPrivate ? "Private" : "Public" : "";
            Buttons.GetIndex("DebugRoomB").overlapText = "<color=blue>" + priv + "</color> " + (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room");
        }
        public static void HideDebug()
        {
            int category = Buttons.GetCategory("Temporary Category");

            Buttons.RemoveButton(category, "DebugMenuName");
            Buttons.RemoveButton(category, "DebugColor");
            Buttons.RemoveButton(category, "DebugName");
            Buttons.RemoveButton(category, "DebugId");
            Buttons.RemoveButton(category, "DebugClip");
            Buttons.RemoveButton(category, "DebugFps");
            Buttons.RemoveButton(category, "DebugRoomA");
            Buttons.RemoveButton(category, "DebugRoomB");
            Buttons.CurrentCategoryName = "Main";
        }

        public static void PlayersTab()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> { 
                new ButtonInfo { 
                    buttonText = "Exit Players", 
                    method =() => Buttons.CurrentCategoryName = "Main", 
                    isTogglable = false, 
                    toolTip = "Returns you back to the main page." 
                } 
            };

            if (!PhotonNetwork.InRoom)
                buttons.Add(new ButtonInfo { buttonText = "Not in a Room", label = true });
            else
            {
                for (int i = 0; i < NetworkSystem.Instance.PlayerListOthers.Length; i++)
                {
                    NetPlayer player = NetworkSystem.Instance.PlayerListOthers[i];
                    string playerColor = "#ffffff";
                    try
                    {
                        playerColor = $"#{ColorToHex(GetVRRigFromPlayer(player).playerColor)}";
                    }
                    catch { }

                    buttons.Add(new ButtonInfo
                    {
                        buttonText = $"PlayerButton{i}",
                        overlapText = $"<color={playerColor}>" + player.NickName + "</color>",
                        method =() => NavigatePlayer(player),
                        isTogglable = false,
                        toolTip = $"See information on the player {player.NickName}."
                    });
                }
            }

            Buttons.buttons[Buttons.GetCategory("Players")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Players";
        }

        public static void NavigatePlayer(NetPlayer player)
        {
            string targetName = player.NickName;

            VRRig playerRig = GetVRRigFromPlayer(player) ?? null;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Exit PlayerInspect",
                    overlapText = $"Exit {targetName}",
                    method =() => PlayersTab(),
                    isTogglable = false,
                    toolTip = "Returns you back to the players tab."
                },

                new ButtonInfo {
                    buttonText = "Spectate Player",
                    overlapText = $"Spectate {targetName}",
                    method =() => SpectatePlayer(playerRig),
                    isTogglable = false,
                    toolTip = $"Shows you what {targetName} sees."
                },
                new ButtonInfo {
                    buttonText = "Teleport to Player",
                    overlapText = $"Teleport to {targetName}",
                    method =() => Movement.TeleportToPlayer(player),
                    isTogglable = false,
                    toolTip = $"Teleports you to {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Give Player Guns",
                    overlapText = $"Give {targetName} Guns",
                    method =() => GiveGunTarget = playerRig,
                    disableMethod =() => GiveGunTarget = null,
                    toolTip = $"Gives {targetName} every gun on the menu."
                },
                new ButtonInfo {
                    buttonText = "Copy Movement",
                    overlapText = $"Copy Movement {targetName}",
                    method =() => Movement.CopyMovementPlayer(player),
                    disableMethod = Movement.EnableRig,
                    toolTip = $"Copies the movement of {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Follow Player",
                    overlapText = $"Follow {targetName}",
                    method =() => Movement.FollowPlayer(player),
                    disableMethod = Movement.EnableRig,
                    toolTip = $"Follows {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Tag Player",
                    overlapText = $"Tag {targetName}",
                    method =() => Advantages.TagPlayer(player),
                    disableMethod = Movement.EnableRig,
                    toolTip = $"Tags {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Snowball Fling Player",
                    overlapText = $"Snowball Fling {targetName}",
                    method =() => Overpowered.FlingPlayer(player),
                    toolTip = $"Flings {targetName} with snowballs."
                },
                new ButtonInfo {
                    buttonText = "Projectile Blind Player",
                    overlapText = $"Projectile Blind {targetName}",
                    method =() => Projectiles.ProjectileBlindPlayer(player),
                    toolTip = $"Blinds {targetName} using the egg projectiles."
                },
                new ButtonInfo {
                    buttonText = "Projectile Lag Player",
                    overlapText = $"Projectile Lag {targetName}",
                    method =() => Projectiles.ProjectileLagPlayer(player),
                    toolTip = $"Lags {targetName} using the firework projectiles."
                },
                new ButtonInfo {
                    buttonText = "Lag Player",
                    overlapText = $"Lag {targetName}",
                    method =() => Overpowered.LagTarget(player),
                    toolTip = $"Lags {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Destroy Player",
                    overlapText = $"Destroy {targetName}",
                    method =() => Overpowered.DestroyPlayer(player),
                    toolTip = $"Stops all new players from seeing {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Guardian Bring Player",
                    overlapText = $"Guardian Bring {targetName}",
                    method =() => Overpowered.BringPlayer(player),
                    toolTip = $"Brings {targetName} to you."
                },
                new ButtonInfo {
                    buttonText = "Guardian Bring Player Gun",
                    overlapText = $"Guardian Bring {targetName} Gun",
                    method =() => Overpowered.BringPlayerGun(player),
                    toolTip = $"Brings {targetName} to wherever your hand desires."
                },
                new ButtonInfo {
                    buttonText = "Guardian Kick Player",
                    overlapText = $"Guardian Kick {targetName}",
                    method =() => Overpowered.GuardianKickTarget(player),
                    toolTip = $"Kicks {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Guardian Obliterate Player",
                    overlapText = $"Guardian Obliterate {targetName}",
                    method =() => Overpowered.ObliteratePlayer(player),
                    toolTip = $"Obliterates {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Guardian Crash Player",
                    overlapText = $"Guardian Crash {targetName}",
                    method =() => Overpowered.CrashPlayer(player),
                    toolTip = $"Crashes {targetName}."
                }
            };

            if (PhotonNetwork.IsMasterClient)
            {
                buttons.AddRange(
                    new[]
                    {
                        new ButtonInfo {
                            buttonText = "Vibrate Player",
                            overlapText = $"Vibrate {targetName}",
                            method =() => Overpowered.BetaSetStatus(RoomSystem.StatusEffects.JoinedTaggedTime, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } }),
                            toolTip = $"Vibrates {targetName}'s controllers."
                        },
                        new ButtonInfo {
                            buttonText = "Slow Player",
                            overlapText = $"Slow {targetName}",
                            method =() => Overpowered.BetaSetStatus(RoomSystem.StatusEffects.TaggedTime, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } } ),
                            toolTip = $"Gives {targetName} tag freeze."
                        }
                    }
                );
            }

            if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
            {
                buttons.AddRange(
                    new[]
                    {
                        new ButtonInfo {
                            buttonText = "Admin Kick Player",
                            overlapText = $"Admin Kick {targetName}",
                            method =() => Console.ExecuteCommand("kick", ReceiverGroup.All, player.UserId),
                            isTogglable = false,
                            toolTip = $"Kicks {targetName} if they're using the menu."
                        },
                        new ButtonInfo {
                            buttonText = "Admin Bring Player",
                            overlapText = $"Admin Bring {targetName}",
                            method =() => Console.ExecuteCommand("tp", player.ActorNumber, GorillaTagger.Instance.headCollider.transform.position),
                            isTogglable = false,
                            toolTip = $"Brings {targetName} to you if they're using the menu."
                        },
                        new ButtonInfo {
                            buttonText = "Admin Crash Player",
                            overlapText = $"Admin Crash {targetName}",
                            method =() => Console.ExecuteCommand("crash", player.ActorNumber),
                            isTogglable = false,
                            toolTip = $"Crashes {targetName} if they're using the menu."
                        },
                    }
                );
            }

            Color playerColor = playerRig?.playerColor ?? Color.black;
            if (playerRig)
                buttons.AddRange(
                    new[]
                    {
                        new ButtonInfo
                        {
                            buttonText = "Player Name",
                            overlapText = $"Name: {player.NickName}",
                            method = () => ChangeName(player.NickName),
                            isTogglable = false,
                            toolTip = $"Sets your name to \"{player.NickName}\"."
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player Color",
                            overlapText =
                                $"Color: {playerColor.ToRichRGBString()}",
                            method = () => ChangeColor(playerColor),
                            isTogglable = false,
                            toolTip = $"Sets your color to the same as {targetName}."
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player User ID",
                            overlapText = $"User ID: {player.UserId}",
                            method = () =>
                            {
                                NotificationManager.SendNotification(
                                    $"<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully copied {player.UserId} to the clipboard!",
                                    5000);
                                GUIUtility.systemCopyBuffer = player.UserId;
                            },
                            isTogglable = false,
                            toolTip = $"Copies {player.UserId} to your clipboard."
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player Creation Date",
                            overlapText =
                                $"Creation Date: {GetCreationDate(player.UserId, creationDate => { Buttons.GetIndex("Player Creation Date").overlapText = $"Creation Date: {creationDate}"; ReloadMenu(); })}",
                            label = true
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player Platform",
                            overlapText =
                                $"Platform: {((playerRig?.IsSteam() ?? false) ? "Steam" : "Quest")}",
                            label = true
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player FPS",
                            overlapText = $"FPS: {playerRig.fps}",
                            label = true
                        }
                    }
                );

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void SpectatePlayer(VRRig rig)
        {
            GameObject cameraObject = new GameObject("iiMenu_SpectateCamera");
            RenderTexture renderTexture = new RenderTexture(512, 512, 16);
            cameraObject.AddComponent<Camera>().targetTexture = renderTexture;
            cameraObject.transform.SetParent(rig.headMesh.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0.25f, 0.25f);
            promptMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
            {
                mainTexture = renderTexture
            };
            PromptSingle("<https://.mat>", () => Object.Destroy(cameraObject), "Done");
        }

        public static void CategorySettings()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {new ButtonInfo { buttonText = "Exit Menu Settings", method =() => Buttons.CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."}};

            foreach (var button in Buttons.buttons[Buttons.GetCategory("Main")])
            {
                buttons.Add(new ButtonInfo
                {
                    buttonText = $"Category{button.buttonText.Hash()}",
                    overlapText = button.buttonText,
                    enabled = !skipButtons.Contains(button.buttonText),
                    enableMethod =() => skipButtons.Remove(button.buttonText),
                    disableMethod =() => skipButtons.Add(button.buttonText),
                    toolTip = "Toggles the visibility of the category " + button.buttonText + "."
                });
            }

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void RightHand()
        {
            rightHand = true;
            if (watchMenu)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotificationManager.ClearAllNotifications();
            }

            if (!Buttons.GetIndex("Info Watch").enabled) return;
            Toggle("Info Watch");
            Toggle("Info Watch");
            NotificationManager.ClearAllNotifications();
        }

        public static void LeftHand()
        {
            rightHand = false;
            if (watchMenu)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotificationManager.ClearAllNotifications();
            }

            if (!Buttons.GetIndex("Info Watch").enabled) return;
            Toggle("Info Watch");
            Toggle("Info Watch");
            NotificationManager.ClearAllNotifications();
        }

        public static void ClearAllKeybinds()
        {
            foreach (KeyValuePair<string, List<string>> bind in ModBindings)
            {
                foreach (string modName in bind.Value)
                    Buttons.GetIndex(modName).customBind = null;

                bind.Value.Clear();
            }
        }

        public static void StartBind(string bind)
        {
            if (IsRebinding)
                return;
            IsBinding = true;
            BindInput = bind;
        }
        public static void StartRebind(string bind)
        {
            if (IsBinding)
                return;
            IsRebinding = true;
            BindInput = bind;
        }

        public static void RemoveRebinds()
        {
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                    v.rebindKey = null;
            }
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Removed all rebinds.");
        }

        // The code below is fully safe. I know, it seems suspicious.
        public static void UpdateMenu()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                {
                    string logoLines = "";
                    foreach (string line in PluginInfo.Logo.Split(@"
"))
                        logoLines += Environment.NewLine + @" ""    " + line + @" """;
                    string updateScript = @"@echo off
title ii's Stupid Menu
color 0E

cls
echo." + logoLines + @"
echo.

echo Your menu is updating, please wait...
echo.

set ""PLUGIN_PATH=BepInEx\plugins""
dir ""%PLUGIN_PATH%\*iiMenu_AutoUpdater*.dll"" >nul 2>&1
if %ERRORLEVEL%==0 (
    goto restart
)

for %%F in (""%PLUGIN_PATH%\*stupid*menu*.dll"") do (
    set ""MENU_FILE=%%F""
    goto update
)

echo No menu file found, skipping update.
goto restart

:update
echo Downloading latest release of ii's Stupid Menu...

curl -L -o ""%MENU_FILE%"" ^
""https://github.com/iiDk-the-actual/iis.Stupid.Menu/releases/latest/download/iis_Stupid_Menu.dll""

goto restart

:restart

:WAIT_LOOP
tasklist /FI ""IMAGENAME eq Gorilla Tag.exe"" | find /I ""Gorilla Tag.exe"" >nul
if %ERRORLEVEL%==0 (
    timeout /t 1 >nul
    goto WAIT_LOOP
)

echo Launching Gorilla Tag...
start steam://run/1533390
exit";

                    string fileName = $"{PluginInfo.BaseDirectory}/UpdateScript.bat";

                    File.WriteAllText(fileName, updateScript);

                    string filePath = FileUtilities.GetGamePath() + "/" + fileName;
                    Process.Start(filePath);
                    Application.Quit();
                    break;
                }
                case OperatingSystemFamily.Linux:
                {
                    string logoLines = "";
                    foreach (string line in PluginInfo.Logo.Split(@"
"))
                        logoLines += Environment.NewLine + @" ""    " + line + @" """;
                    string updateScript = @"#!/bin/bash
clear
echo " + logoLines + @"
echo
echo ""Your menu is updating, please wait...""
echo

PLUGIN_PATH=""BepInEx/plugins""
MENU_FILE=""""

if ls ""$PLUGIN_PATH""/*iiMenu_AutoUpdater*.dll 1> /dev/null 2>&1; then
    echo ""Auto-updater found. Restarting game...""
else
    for f in ""$PLUGIN_PATH""/*stupid*menu*.dll; do
        if [ -f ""$f"" ]; then
            MENU_FILE=""$f""
            break
        fi
    done

    if [ -z ""$MENU_FILE"" ]; then
        echo ""No menu file found, skipping update.""
    else
        echo ""Downloading latest release of ii's Stupid Menu...""
        curl -L -o ""$MENU_FILE"" \
        ""https://github.com/iiDk-the-actual/iis.Stupid.Menu/releases/latest/download/iis_Stupid_Menu.dll""
    fi
fi

while pgrep -f ""GorillaTag.exe"" > /dev/null; do
    sleep 1
done

echo ""Launching Gorilla Tag...""
xdg-open ""steam://run/1533390""
exit 0";

                    string fileName = $"{PluginInfo.BaseDirectory}/UpdateScript.sh";
                    File.WriteAllText(fileName, updateScript);
                    Process.Start("chmod", $"+x \"{fileName}\"");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"\"{fileName}\"",
                        UseShellExecute = false
                    });
                    Application.Quit();
                    break;
                }
            }
        }

        public static void JoystickMenuOff()
        {
            joystickMenu = false;
            joystickOpen = false;
        }

        public static void PhysicalMenuOn()
        {
            physicalMenu = true;
            physicalOpenPosition = Vector3.zero;
        }

        public static void PhysicalMenuOff()
        {
            physicalMenu = false;
            physicalOpenPosition = Vector3.zero;
        }

        public static void WatchMenuOn()
        {
            watchMenu = true;
            GameObject mainwatch = VRRig.LocalRig.transform.Find("rig/hand.L/huntcomputer (1)").gameObject;
            watchobject = Object.Instantiate(mainwatch, 
                rightHand ?
                VRRig.LocalRig.transform.Find("rig/hand.R").transform :
                VRRig.LocalRig.transform.Find("rig/hand.L").transform, false);

            Object.Destroy(watchobject.GetComponent<GorillaHuntComputer>());
            watchobject.SetActive(true);

            Transform watchCanvas = watchobject.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
            watchCanvas.Find("Hat").gameObject.SetActive(false);
            watchCanvas.Find("Face").gameObject.SetActive(false);
            watchCanvas.Find("Badge").gameObject.SetActive(false);
            watchCanvas.Find("Material").gameObject.SetActive(false);
            watchCanvas.Find("Right Hand").gameObject.SetActive(false);

            watchText = watchCanvas.Find("Text").gameObject;
            watchEnabledIndicator = watchCanvas.Find("Left Hand").gameObject;
            watchShell = watchobject.transform.Find("HuntWatch_ScreenLocal").gameObject;

            watchShell.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;

            if (rightHand)
            {
                watchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
                watchShell.transform.parent.localPosition += new Vector3(0.025f, 0f, 0f);
                watchShell.transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
            }
        }
        public static void CheckWatchMenu()
        {
            if (watchTimer == 0)
                watchTimer = Time.time + 10f;

            if (leftJoystick.sqrMagnitude > 0.1f * 0.1f)
            {
                watchTimer = 0;
                watchUsed = true;
                return;
            }

            if (!watchUsed && Time.time >= watchTimer)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=purple>WATCH</color><color=grey>]</color> Seems that you got stuck using Watch Menu, automatically disabling..");
                Toggle("Watch Menu");
            }
        }
        public static void WatchMenuOff()
        {
            watchMenu = false;
            watchUsed = false;
            watchTimer = 0;
            Object.Destroy(watchobject);
        }

        public static int langInd;
        public static void ChangeMenuLanguage(bool positive = true)
        {
            string[] languageNames = {
                "English",
                "Español",
                "Français",
                "Deutsch",
                "日本語",
                "Italiano",
                "Português",
                "Nederlands",
                "Русский",
                "Polski"
            };

            string[] codenames = {
                "en",
                "es",
                "fr",
                "de",
                "ja",
                "it",
                "pt",
                "nl",
                "ru",
                "pl"
            };

            if (positive)
                langInd++;
            else
                langInd--;

            langInd %= languageNames.Length;
            if (langInd < 0)
                langInd = languageNames.Length - 1;

            TranslationManager.translateCache.Clear();
            TranslationManager.language = codenames[langInd];

            Buttons.GetIndex("Change Menu Language").overlapText = "Change Menu Language <color=grey>[</color><color=green>" + languageNames[langInd] + "</color><color=grey>]</color>";

            translate = langInd != 0;
        }

        public static void ChangeMenuButton(bool positive = true)
        {
            string[] buttonNames = {
                "Primary",
                "Secondary",
                "Grip",
                "Trigger",
                "Joystick"
            };

            if (positive)
                menuButtonIndex++;
            else
                menuButtonIndex--;

            menuButtonIndex %= buttonNames.Length;
            if (menuButtonIndex < 0)
                menuButtonIndex = buttonNames.Length - 1;

            Buttons.GetIndex("Change Menu Button").overlapText = "Change Menu Button <color=grey>[</color><color=green>" + buttonNames[menuButtonIndex] + "</color><color=grey>]</color>";
        }

        // I know there's better ways to do this. Trust me.
        public static void ChangeMenuTheme(bool increment = true)
        {
            if (increment) 
                themeType++; 
            else 
                themeType--;

            const int themeCount = 66;

            if (themeType > themeCount)
                themeType = 1;

            if (themeType < 1)
                themeType = themeCount;

            if (Buttons.GetIndex("Custom Menu Theme").enabled)
                return;

            switch (themeType)
            {
                case 1: // Orange
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 128, 0, 128), new Color32(255, 102, 0, 128))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(170, 85, 0, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(85, 42, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        }
                    };
                    break;
                case 2: // Blue Magenta
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.blue, Color.magenta)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 3: // Dark Mode
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(20, 20, 20, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 4: // Strobe
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.white, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.black, Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 5: // Kman
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(110, 0, 0, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(110, 0, 0, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(110, 0, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 6: // Rainbow
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        rainbow = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black),
                            rainbow = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 7: // Cone
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 128))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(170, 85, 0, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(85, 42, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        }
                    };
                    Buttons.GetIndex("Thin Menu").enabled = true;
                    thinMenu = true;
                    break;
                case 8: // Player Material
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        copyRigColor = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black),
                            copyRigColor = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 9: // Lava
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(255, 111, 0, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(255, 111, 0, 255), Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 10: // Rock
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, Color.red)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.red, Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 11: // Ice
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(0, 174, 255, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(0, 174, 255, 255), Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 12: // Water
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(0, 136, 255, 255), new Color32(0, 174, 255, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 100, 188, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(0, 174, 255, 255), new Color32(0, 136, 255, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 13: // Minty
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(0, 255, 246, 255), new Color32(0, 255, 144, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(0, 255, 144, 255), new Color32(0, 255, 246, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 14: // Pink
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 130, 255, 255), Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 130, 255, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 15: // Purple
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(122, 35, 159, 255), new Color32(60, 26, 89, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(60, 26, 89, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(122, 35, 159, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 16: // Magenta Cyan
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.cyan)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.cyan)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 17: // Red Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.red, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 18: // Orange Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 128, 0, 255), Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 19: // Yellow Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.yellow, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 20: // Green Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.green, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 21: // Blue Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.blue, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 22: // Purple Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(119, 0, 255, 255), Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(119, 0, 255, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(119, 0, 255, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(119, 0, 255, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 23: // Magenta Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 24: // Banana
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 255, 130, 255), Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 255, 130, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 25: // Pride
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.red, Color.green)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 26: // Trans
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(245, 169, 184, 255), new Color32(91, 206, 250, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(245, 169, 184, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(91, 206, 250, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(91, 206, 250, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(91, 206, 250, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(245, 169, 184, 255))
                        }
                    };
                    break;
                case 27: // MLM or Gay
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(7, 141, 112, 255), new Color32(61, 26, 220, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(7, 141, 112, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(61, 26, 220, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(61, 26, 220, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(61, 26, 220, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(7, 141, 112, 255))
                        }
                    };
                    break;
                case 28: // Steal (old)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(75, 75, 75, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 29: // Silence
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(80, 0, 80, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    break;
                case 30: // Transparent
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        transparent = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white),
                            transparent = true
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green),
                            transparent = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    break;
                case 31: // King
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(100, 60, 170, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(150, 100, 240, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(150, 100, 240, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.cyan)
                        }
                    };
                    break;
                case 32: // Scoreboard
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(0, 59, 4, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(192, 190, 171, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 33: // Scoreboard (banned)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(225, 73, 43, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(192, 190, 171, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 34: // Rift
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(25, 25, 25, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(40, 40, 40, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(167, 66, 191, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 35: // Blurple Dark
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(26, 26, 61, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(26, 26, 61, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(43, 17, 84, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 36: // ShibaGT Gold
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, Color.gray)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 37: // ShibaGT Genesis
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 38: // wyvern
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(199, 115, 173, 255), new Color32(165, 233, 185, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(99, 58, 86, 255), new Color32(83, 116, 92, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(99, 58, 86, 255), new Color32(83, 116, 92, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    break;
                case 39: // Steal (new)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(27, 27, 27, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(66, 66, 66, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 40: // USA Menu (lol)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(100, 25, 125, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(25, 25, 25, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 41: // Watch
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(27, 27, 27, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 42: // AZ Menu
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(100, 0, 0, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(100, 0, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 43: // ImGUI
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(21, 22, 23, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(32, 50, 77, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(60, 127, 206, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 44: // Clean Dark
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(10, 10, 10, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 45: // Discord Light Mode (lmfao)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(245, 245, 245, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 46: // The Hub
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 163, 26, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 47: // EPILEPTIC
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        epileptic = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black),
                            epileptic = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 48: // Discord Blurple
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(111, 143, 255, 255), new Color32(163, 184, 255, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(96, 125, 219, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(147, 167, 226, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(33, 33, 101, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(33, 33, 101, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(33, 33, 101, 255))
                        }
                    };
                    break;
                case 49: // VS Zero
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(19, 22, 27, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(19, 22, 27, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(16, 18, 22, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(82, 96, 122, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(82, 96, 122, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(82, 96, 122, 255))
                        }
                    };
                    break;
                case 50: // Weed theme (for v4.2.0) (also 50th theme)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(0, 136, 16, 255), new Color32(0, 127, 14, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 158, 15, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 112, 11, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 51: // Pastel Rainbow
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.white),
                        pastelRainbow = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white),
                            pastelRainbow = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 52: // Rift Light
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(25, 25, 25, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(40, 40, 40, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(165, 137, 255, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 53: // Rose (Solace)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(176, 12, 64, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(140, 10, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(250, 2, 81, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 54: // Tenacity (Solace)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(124, 25, 194, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(88, 9, 145, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(136, 9, 227, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 55: // e621 (for version 6.2.1)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(1, 73, 149, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(1, 46, 87, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 37, 74, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(252, 179, 40, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 56: // Catppuccin Mocha
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(30, 30, 46, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(88, 91, 112, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(49, 50, 68, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(205, 214, 244, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(186, 194, 222, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(166, 173, 200, 255))
                        }
                    };
                    break;
                case 57: // Rexon
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(45, 25, 75, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(40, 15, 60, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(100, 30, 140, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 58: // Tenacity (Minecraft)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(231, 133, 209, 255), new Color32(56, 155, 193, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 59: // Mint Blue (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(40, 94, 93, 255), new Color32(66, 158, 157, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
            };
                    break;
                case 60: // Pink Blood (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(255, 166, 201, 255), new Color32(228, 0, 70, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 61: // Purple Fire (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(177, 162, 202, 255), new Color32(104, 71, 141, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 62: // Deep Ocean (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(60, 82, 145, 255), new Color32(0, 20, 64, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 63: // Bad Apple (thanks random person in vc for idea)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            transparent = true
                        },
                        new ExtGradient // Pressed
                        {
                            transparent = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 64: // coolkidd
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.red)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 65: // Old ShibaGT RGB
                    backgroundColor = new ExtGradient
                    {
                        colors = new[]
                        {
                            new GradientColorKey(Color.red, 0f),
                            new GradientColorKey(Color.green, 0.333f),
                            new GradientColorKey(Color.blue, 0.666f),
                            new GradientColorKey(Color.red, 1f),
                        }
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = new[]
                            {
                                new GradientColorKey(Color.red, 0f),
                                new GradientColorKey(Color.green, 0.333f),
                                new GradientColorKey(Color.blue, 0.666f),
                                new GradientColorKey(Color.red, 1f),
                            }
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 66: // Old-ish ShibaGT RGB
                    backgroundColor = new ExtGradient
                    {
                        colors = new[]
                        {
                            new GradientColorKey(Color.yellow, 0f),
                            new GradientColorKey(Color.red, 0.2f),
                            new GradientColorKey(Color.magenta, 0.4f),
                            new GradientColorKey(Color.blue, 0.6f),
                            new GradientColorKey(Color.green, 0.8f),
                            new GradientColorKey(Color.yellow, 1f)
                        }
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = new[]
                            {
                                new GradientColorKey(Color.yellow, 0f),
                                new GradientColorKey(Color.red, 0.2f),
                                new GradientColorKey(Color.magenta, 0.4f),
                                new GradientColorKey(Color.blue, 0.6f),
                                new GradientColorKey(Color.green, 0.8f),
                                new GradientColorKey(Color.yellow, 1f)
                            }
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
            }
        }

        private static int menuScaleIndex = 10;
        public static void ChangeMenuScale(bool positive = true)
        {
            if (positive)
                menuScaleIndex++;
            else
                menuScaleIndex--;

            if (menuScaleIndex > 30)
                menuScaleIndex = 2;
            if (menuScaleIndex < 2)
                menuScaleIndex = 30;

            menuScale = menuScaleIndex / 10f;

            Buttons.GetIndex("Change Menu Scale").overlapText = "Change Menu Scale <color=grey>[</color><color=green>" + menuScale + "</color><color=grey>]</color>";
        }

        private static int notificationScaleIndex = 6;
        public static void ChangeNotificationScale(bool positive = true)
        {
            if (positive)
                notificationScaleIndex++;
            else
                notificationScaleIndex--;

            if (notificationScaleIndex > 20)
                notificationScaleIndex = 1;
            if (notificationScaleIndex < 1)
                notificationScaleIndex = 20;

            notificationScale = notificationScaleIndex * 5;

            Buttons.GetIndex("Change Notification Scale").overlapText = "Change Notification Scale <color=grey>[</color><color=green>" + notificationScaleIndex + "</color><color=grey>]</color>";
        }

        private static int arraylistScaleIndex = 4;
        public static void ChangeArraylistScale(bool positive = true)
        {
            if (positive)
                arraylistScaleIndex++;
            else
                arraylistScaleIndex--;

            if (arraylistScaleIndex > 20)
                arraylistScaleIndex = 1;
            if (arraylistScaleIndex < 1)
                arraylistScaleIndex = 20;

            arraylistScale = arraylistScaleIndex * 5;

            Buttons.GetIndex("Change Arraylist Scale").overlapText = "Change Arraylist Scale <color=grey>[</color><color=green>" + arraylistScaleIndex + "</color><color=grey>]</color>";
        }

        private static int overlayScaleIndex = 6;
        public static void ChangeOverlayScale(bool positive = true)
        {
            if (positive)
                overlayScaleIndex++;
            else
                overlayScaleIndex--;

            if (overlayScaleIndex > 20)
                overlayScaleIndex = 1;
            if (overlayScaleIndex < 1)
                overlayScaleIndex = 20;

            overlayScale = overlayScaleIndex * 5;

            Buttons.GetIndex("Change Overlay Scale").overlapText = "Change Overlay Scale <color=grey>[</color><color=green>" + overlayScaleIndex + "</color><color=grey>]</color>";
        }

        private static int modifyWhatId;
        public static void CMTRed(bool increase = true)
        {
            switch (modifyWhatId)
            {
                case 0:
                    {
                        int r = (int)Math.Round(backgroundColor.GetColor(0).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            backgroundColor.SetColor(0, new Color(r / 10f, backgroundColor.GetColor(0).g, backgroundColor.GetColor(0).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 1:
                    {
                        int r = (int)Math.Round(backgroundColor.GetColor(1).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            backgroundColor.SetColor(1, new Color(r / 10f, backgroundColor.GetColor(1).g, backgroundColor.GetColor(1).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 2:
                    {
                        int r = (int)Math.Round(buttonColors[0].GetColor(0).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[0].SetColor(0, new Color(r / 10f, buttonColors[0].GetColor(0).g, buttonColors[0].GetColor(0).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 3:
                    {
                        int r = (int)Math.Round(buttonColors[0].GetColor(1).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[0].SetColor(1, new Color(r / 10f, buttonColors[0].GetColor(1).g, buttonColors[0].GetColor(1).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 4:
                    {
                        int r = (int)Math.Round(buttonColors[1].GetColor(0).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[1].SetColor(0, new Color(r / 10f, buttonColors[1].GetColor(0).g, buttonColors[1].GetColor(0).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 5:
                    {
                        int r = (int)Math.Round(buttonColors[1].GetColor(1).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[1].SetColor(1, new Color(r / 10f, buttonColors[1].GetColor(1).g, buttonColors[1].GetColor(1).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 6:
                    {
                        int r = (int)Math.Round(textColors[0].GetColor(0).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[0].SetColors(new Color(r / 10f, textColors[0].GetColor(0).g, textColors[0].GetColor(0).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 7:
                    {
                        int r = (int)Math.Round(textColors[1].GetColor(0).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        textColors[1].SetColors(new Color(r / 10f, textColors[1].GetColor(0).g, textColors[1].GetColor(0).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 8:
                    {
                        int r = (int)Math.Round(textColors[2].GetColor(0).r * 10f);

                        if (increase)
                            r++;
                        else
                            r--;

                        r %= 11;
                        if (r < 0)
                            r = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[2].SetColors(new Color(r / 10f, textColors[2].GetColor(0).g, textColors[2].GetColor(0).b));

                        Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>";
                        break;
                    }
            }
            WriteCustomTheme();
        }

        public static void CMTGreen(bool increase = true)
        {
            switch (modifyWhatId)
            {
                case 0:
                    {
                        int g = (int)Math.Round(backgroundColor.GetColor(0).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            backgroundColor.SetColor(0, new Color(backgroundColor.GetColor(0).r, g / 10f, backgroundColor.GetColor(0).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 1:
                    {
                        int g = (int)Math.Round(backgroundColor.GetColor(1).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            backgroundColor.SetColor(1, new Color(backgroundColor.GetColor(1).r, g / 10f, backgroundColor.GetColor(1).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 2:
                    {
                        int g = (int)Math.Round(buttonColors[0].GetColor(0).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[0].SetColor(0, new Color(buttonColors[0].GetColor(0).r, g / 10f, buttonColors[0].GetColor(0).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 3:
                    {
                        int g = (int)Math.Round(buttonColors[0].GetColor(1).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[0].SetColor(1, new Color(buttonColors[0].GetColor(1).r, g / 10f, buttonColors[0].GetColor(1).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 4:
                    {
                        int g = (int)Math.Round(buttonColors[1].GetColor(0).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[1].SetColor(0, new Color(buttonColors[1].GetColor(0).r, g / 10f, buttonColors[1].GetColor(0).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 5:
                    {
                        int g = (int)Math.Round(buttonColors[1].GetColor(1).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[1].SetColor(1, new Color(buttonColors[1].GetColor(1).r, g / 10f, buttonColors[1].GetColor(1).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 6:
                    {
                        int g = (int)Math.Round(textColors[0].GetColor(0).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[0].SetColors(new Color(textColors[0].GetColor(0).r, g / 10f, textColors[0].GetColor(0).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 7:
                    {
                        int g = (int)Math.Round(textColors[1].GetColor(0).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[1].SetColors(new Color(textColors[1].GetColor(0).r, g / 10f, textColors[1].GetColor(0).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 8:
                    {
                        int g = (int)Math.Round(textColors[2].GetColor(0).g * 10f);

                        if (increase)
                            g++;
                        else
                            g--;

                        g %= 11;
                        if (g < 0)
                            g = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[2].SetColors(new Color(textColors[2].GetColor(0).r, g / 10f, textColors[2].GetColor(0).b));

                        Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>";
                        break;
                    }
            }
            WriteCustomTheme();
        }
        public static void CMTBlue(bool increase = true)
        {
            switch (modifyWhatId)
            {
                case 0:
                    {
                        int b = (int)Math.Round(backgroundColor.GetColor(0).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            backgroundColor.SetColor(0, new Color(backgroundColor.GetColor(0).r, backgroundColor.GetColor(0).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 1:
                    {
                        int b = (int)Math.Round(backgroundColor.GetColor(1).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            backgroundColor.SetColor(1, new Color(backgroundColor.GetColor(1).r, backgroundColor.GetColor(1).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 2:
                    {
                        int b = (int)Math.Round(buttonColors[0].GetColor(0).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[0].SetColor(0, new Color(buttonColors[0].GetColor(0).r, buttonColors[0].GetColor(0).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 3:
                    {
                        int b = (int)Math.Round(buttonColors[0].GetColor(1).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[0].SetColor(1, new Color(buttonColors[0].GetColor(1).r, buttonColors[0].GetColor(1).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 4:
                    {
                        int b = (int)Math.Round(buttonColors[1].GetColor(0).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[1].SetColor(0, new Color(buttonColors[1].GetColor(0).r, buttonColors[1].GetColor(0).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 5:
                    {
                        int b = (int)Math.Round(buttonColors[1].GetColor(1).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            buttonColors[1].SetColor(1, new Color(buttonColors[1].GetColor(1).r, buttonColors[1].GetColor(1).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>";
                        break;
                    }
                case 6:
                    {
                        int b = (int)Math.Round(textColors[0].GetColor(0).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[0].SetColors(new Color(textColors[0].GetColor(0).r, textColors[0].GetColor(0).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 7:
                    {
                        int b = (int)Math.Round(textColors[1].GetColor(0).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[1].SetColors(new Color(textColors[1].GetColor(0).r, textColors[1].GetColor(0).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>";
                        break;
                    }
                case 8:
                    {
                        int b = (int)Math.Round(textColors[2].GetColor(0).b * 10f);

                        if (increase)
                            b++;
                        else
                            b--;

                        b %= 11;
                        if (b < 0)
                            b = 10;

                        if (Buttons.GetIndex("Custom Menu Theme").enabled)
                            textColors[2].SetColors(new Color(textColors[2].GetColor(0).r, textColors[2].GetColor(0).g, b / 10f));

                        Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                        Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>";
                        break;
                    }
            }
            WriteCustomTheme();
        }

        private static int previousPage;
        public static void CustomMenuTheme()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt"))
                WriteCustomTheme();

            ReadCustomTheme();
        }

        public static void ChangeCustomMenuTheme()
        {
            previousPage = pageNumber;
            CustomMenuThemePage();
        }

        public static void CustomMenuThemePage()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Custom Menu Theme", method = () => ExitCustomMenuTheme(), isTogglable = false, toolTip = "Returns you back to the settings menu." },
                new ButtonInfo { buttonText = "Background", method = () => CMTBackground(), isTogglable = false, toolTip = "Choose what segment of the background you would like to modify." },
                new ButtonInfo { buttonText = "Buttons", method = () => CMTButton(), isTogglable = false, toolTip = "Choose what segment of the button you would like to modify." },
                new ButtonInfo { buttonText = "Text", method = () => CMTText(), isTogglable = false, toolTip = "Choose what segment of the text you would like to modify." },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void CMTBackground()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Background", method = () => CustomMenuThemePage(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = () => CMTBackgroundFirst(), isTogglable = false, toolTip = "Change the color of the first color of the background." },
                new ButtonInfo { buttonText = "Second Color", method = () => CMTBackgroundSecond(), isTogglable = false, toolTip = "Change the color of the second color of the background." },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTBackgroundFirst()
        {
            modifyWhatId = 0;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTBackground(), isTogglable = false, toolTip = "Returns you back to the background menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the background." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the background." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the background." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTBackgroundSecond()
        {
            modifyWhatId = 1;
  
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTBackground(), isTogglable = false, toolTip = "Returns you back to the background menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(1).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the second color of the background." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(1).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the second color of the background." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(1).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the second color of the background." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void CMTButton()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Buttons", method = CustomMenuThemePage, isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "Enabled", method = CMTButtonEnabled, isTogglable = false, toolTip = "Choose what type of button color to modify." },
                new ButtonInfo { buttonText = "Disabled", method = CMTButtonDisabled, isTogglable = false, toolTip = "Change the color of the second color of the background." },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTButtonEnabled()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Enabled", method = CMTButton, isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = CMTButtonEnabledFirst, isTogglable = false, toolTip = "Change the color of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Second Color", method = () => CMTButtonEnabledSecond(), isTogglable = false, toolTip = "Change the color of the second color of the enabled button color." },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTButtonDisabled()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Enabled", method = () => CMTButton(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = () => CMTButtonDisabledFirst(), isTogglable = false, toolTip = "Change the color of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Second Color", method = () => CMTButtonDisabledSecond(), isTogglable = false, toolTip = "Change the color of the second color of the disabled button color." },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTButtonEnabledFirst()
        {
            modifyWhatId = 4;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTButtonEnabled(), isTogglable = false, toolTip = "Returns you back to the enabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTButtonEnabledSecond()
        {
            modifyWhatId = 5;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTButtonEnabled(), isTogglable = false, toolTip = "Returns you back to the enabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(1).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(1).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(1).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTButtonDisabledFirst()
        {
            modifyWhatId = 2;
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTButtonDisabled(), isTogglable = false, toolTip = "Returns you back to the disabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTButtonDisabledSecond()
        {
            modifyWhatId = 3;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = CMTButtonDisabled, isTogglable = false, toolTip = "Returns you back to the disabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(1).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(1).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(1).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void CMTText()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Text", method = CustomMenuThemePage, isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "Title", method = CMTTextTitle, isTogglable = false, toolTip = "Change the color of the title." },
                new ButtonInfo { buttonText = "Enabled", method = CMTTextEnabled, isTogglable = false, toolTip = "Change the color of the enabled text." },
                new ButtonInfo { buttonText = "Disabled", method = CMTTextDisabled, isTogglable = false, toolTip = "Change the color of the disabled text." },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTTextTitle()
        {
            modifyWhatId = 6;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Title", method = CMTText, isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(textColors[0].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the title color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(textColors[0].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the title color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(textColors[0].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the title color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTTextEnabled()
        {
            modifyWhatId = 8;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(textColors[2].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the enabled text color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(textColors[2].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the enabled text color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(textColors[2].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the enabled text color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }
        public static void CMTTextDisabled()
        {
            modifyWhatId = 7;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(textColors[1].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the disabled text color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(textColors[1].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the disabled text color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(textColors[1].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the disabled text color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = buttons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void ExitCustomMenuTheme()
        {
            pageNumber = previousPage;
            Buttons.CurrentCategoryName = "Menu Settings";
        }

        public static void ReadCustomTheme()
        {
            string[] linesplit = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt").Split("\n");

            string[] a = linesplit[0].Split(",");
            backgroundColor.SetColor(0, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[1].Split(",");
            backgroundColor.SetColor(1, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));

            a = linesplit[2].Split(",");
            buttonColors[0].SetColor(0, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[3].Split(",");
            buttonColors[0].SetColor(1, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));

            a = linesplit[4].Split(",");
            buttonColors[1].SetColor(0, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[5].Split(",");
            buttonColors[1].SetColor(1, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));

            a = linesplit[6].Split(",");
            textColors[0].SetColors(new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[7].Split(",");
            textColors[1].SetColors(new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[8].Split(",");
            textColors[2].SetColors(new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
        }

        public static void ImportCustomTheme(string theme)
        {
            File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt", theme);
            ReadCustomTheme();
        }

        public static string ExportCustomTheme()
        {
            Color[] clrs = {
                backgroundColor.GetColor(0),
                backgroundColor.GetColor(1),
                buttonColors[0].GetColor(0),
                buttonColors[0].GetColor(1),
                buttonColors[1].GetColor(0),
                buttonColors[1].GetColor(1),
                textColors[0].GetColor(0),
                textColors[1].GetColor(0),
                textColors[2].GetColor(0)
            };

            string output = "";
            foreach (Color clr in clrs)
            {
                if (output != "")
                    output += "\n";

                output += Math.Round(Mathf.Round(clr.r * 10) / 10 * 255f) + "," + Math.Round(Mathf.Round(clr.g * 10) / 10 * 255f) + "," + Math.Round(Mathf.Round(clr.b * 10) / 10 * 255f);
            }

            return output;
        }

        public static void WriteCustomTheme() =>
            File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt", ExportCustomTheme());

        public static void FixTheme()
        {
            themeType--;
            ChangeMenuTheme();
        }

        public static void CustomMenuBackground()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/CustomBackground.png"))
                LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/CustomBackground.png", "CustomBackground.png"); // Do not move outside of its path

            textureFileDirectory.Remove("CustomBackground.png");

            doCustomMenuBackground = true;
            customMenuBackgroundImage = LoadTextureFromFile("CustomBackground.png");
        }

        public static void FixMenuBackground()
        {
            customMenuBackgroundImage = null;
            doCustomMenuBackground = false;
        }
        
        public static void EnableWatermark()
        {
            bool enabled = Buttons.GetIndex("Custom Watermark").enabled;
            if (enabled)
            {
                if (!File.Exists($"{PluginInfo.BaseDirectory}/CustomWatermark.png"))
                    LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/CustomWatermark.png", "CustomWatermark.png"); // Do not move outside of its path

                textureFileDirectory.Remove("CustomWatermark.png");
                customWatermark = LoadTextureFromFile("CustomWatermark.png");
            }
            else
            {
                watermarkImage = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Image>();

                if (watermarkMat == null)
                    watermarkMat = new Material(watermarkImage.material);

                watermarkImage.material = watermarkMat;
                watermarkImage.material.SetTexture("_MainTex", customWatermark ?? LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.icon.png"));
            }
        }

        public static void CustomWatermark()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/CustomWatermark.png"))
                LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/CustomWatermark.png", "CustomWatermark.png"); // Do not move outside of its path

            textureFileDirectory.Remove("CustomWatermark.png");
            customWatermark = LoadTextureFromFile("CustomWatermark.png");
        }

        private static TMP_FontAsset chosenFont;
        public static void CustomFontType()
        {
            string filePath = $"{PluginInfo.BaseDirectory}/CustomFont.ttf";
            if (!File.Exists(filePath))
            {
                LogManager.Log("Downloading CustomFont.ttf");
                WebClient stream = new WebClient();
                stream.DownloadFile($"{PluginInfo.ServerResourcePath}/Fonts/LiberationSans.ttf", filePath);
            }

            chosenFont = TMP_FontAsset.CreateFontAsset(new Font($"{FileUtilities.GetGamePath()}/{filePath}"));
            PersistCustomFont();
        }

        public static void PersistCustomFont()
        {
            if (activeFont != chosenFont)
                activeFont = chosenFont;
        }

        public static void DisableCustomFont()
        {
            fontCycle--;
            ChangeFontType();
        }

        public static void ChangePageType(bool positive = true)
        {
            if (positive)
                pageButtonType++;
            else
                pageButtonType--;

            if (pageButtonType > 6)
                pageButtonType = 1;

            if (pageButtonType < 1)
                pageButtonType = 6;

            buttonOffset = pageButtonType == 1 ? 2 : 0;
        }

        public static void ChangePageSize(bool positive = true)
        {
            if (positive)
                _pageSize++;
            else
                _pageSize--;

            if (_pageSize > 16)
                _pageSize = 4;

            if (_pageSize < 4)
                _pageSize = 16;

            Buttons.GetIndex("Change Page Size").overlapText = $"Change Page Size <color=grey>[</color><color=green>{_pageSize}</color><color=grey>]</color>";
        }

        public static void ChangeCharacterDistance(bool positive = true)
        {
            if (positive)
                characterDistance++;
            else
                characterDistance--;

            if (characterDistance > 15)
                characterDistance = 0;

            if (characterDistance < 0)
                characterDistance = 15;

            Buttons.GetIndex("Change Character Distance").overlapText = $"Change Character Distance <color=grey>[</color><color=green>{characterDistance + 1}</color><color=grey>]</color>";
        }

        public static void ChangeArrowType(bool positive = true)
        {
            if (positive)
                arrowType++;
            else
                arrowType--;

            arrowType %= arrowTypes.Length;
            if (arrowType < 0)
                arrowType = arrowTypes.Length - 1;
        }

        public static void ChangeFontType(bool positive = true)
        {
            if (positive)
                fontCycle++;
            else
                fontCycle--;

            fontCycle %= 15;
            if (fontCycle < 0)
                fontCycle = 14;

            switch (fontCycle) {
                case 0:
                    activeFont = AgencyFB;
                    return;
                case 1:
                    activeFont = FreeSans;
                    return;
                case 2:
                    activeFont = DejaVuSans;
                    return;
                case 3:
                    activeFont = Utopium;
                    return;
                case 4:
                    activeFont = ComicSans;
                    return;
                case 5:
                    activeFont = CascadiaMono;
                    return;
                case 6:
                    activeFont = Candara;
                    return;
                case 7:
                    activeFont = MSGothic;
                    return;
                case 8:
                    activeFont = Anton;
                    return;
                case 9:
                    activeFont = SimSun;
                    return;
                case 10:
                    activeFont = Minecraft;
                    return;
                case 11:
                    activeFont = Terminal;
                    return;
                case 12:
                    activeFont = OpenDyslexic;
                    return;
                case 13:
                    activeFont = Taiko;
                    return;
                case 14:
                    activeFont = LiberationSans;
                    return;
            }
        }

        public static float fontTime;
        public static void ChangeFontRapid()
        {
            if (Time.time > fontTime)
            {
                ChangeFontType();
                fontTime = Time.time + 0.4f;

                ReloadMenu();
            }
        }

        public static int fontStyleType = 2;
        public static void ChangeFontStyleType(bool positive = true)
        {
            if (positive)
                fontStyleType++;
            else
                fontStyleType--;

            fontStyleType %= 4;
            if (fontStyleType < 0)
                fontStyleType = 3;

            activeFontStyle = fontStyleType switch
            {
                0 => FontStyles.Normal,
                1 => FontStyles.Bold,
                2 => FontStyles.Italic,
                3 => FontStyles.Bold | FontStyles.Italic,
                _ => FontStyles.Normal
            };
        }

        public static int inputTextColorInt = 3;
        public static void ChangeInputTextColor(bool positive = true)
        {
            string[] textColors = {
                "Red",
                "Orange",
                "Yellow",
                "Green",
                "Blue",
                "Cyan",
                "Purple",
                "Pink",
                "White",
                "Grey",
                "Black",
                "Rose"
            };
            string[] realinputcolor = {
                "red",
                "#ff8000",
                "yellow",
                "green",
                "blue",
                "#00FFFF",
                "purple",
                "#FF00FF",
                "white",
                "grey",
                "black",
                "#ff005d"
            };

            if (positive)
                inputTextColorInt++;
            else
                inputTextColorInt--;

            inputTextColorInt %= realinputcolor.Length;
            if (inputTextColorInt < 0)
                inputTextColorInt = realinputcolor.Length - 1;

            inputTextColor = realinputcolor[inputTextColorInt];
            Buttons.GetIndex("Change Input Text Color").overlapText = $"Change Input Text Color <color=grey>[</color><color=green>{textColors[inputTextColorInt]}</color><color=grey>]</color>";
        }

        public static void ChangePCUI(bool positive = true)
        {
            if (positive)
                pcbg++;
            else
                pcbg--;

            pcbg %= 6;
            if (pcbg < 0)
                pcbg = 5;
        }

        public static void ChangeJoystickMenuPosition(bool positive = true)
        {
            if (positive)
                joystickMenuPosition++;
            else
                joystickMenuPosition--;

            joystickMenuPosition %= joystickMenuPositions.Length;
            if (joystickMenuPosition < 0)
                joystickMenuPosition = joystickMenuPositions.Length - 1;
        }

        public static void ChangeNotificationTime(bool positive = true)
        {
            if (positive)
                notificationDecayTime += 1000;
            else
                notificationDecayTime -= 1000;

            notificationDecayTime %= 6000;
            if (notificationDecayTime < 0)
                notificationDecayTime = 5000;

            Buttons.GetIndex("Change Notification Time").overlapText = "Change Notification Time <color=grey>[</color><color=green>" + notificationDecayTime / 1000 + "</color><color=grey>]</color>";
        }

        public static readonly Dictionary<string, string> notificationSounds = new Dictionary<string, string>
        {
            { "None",          "none"        },
            { "Pop",           "pop"         },
            { "Ding",          "ding"        },
            { "Twitter",       "twitter"     },
            { "Discord",       "discord"     },
            { "Whatsapp",      "whatsapp"    },
            { "Grindr",        "grindr"      },
            { "iOS",           "ios"         },
            { "XP Notify",     "xpnotify"    },
            { "XP Ding",       "xptrueding"  },
            { "XP Question",   "xpding"      },
            { "XP Error",      "xperror"     },
            { "Roblox Bass",   "robloxbass"  },
            { "Oculus",        "oculus"      },
            { "Nintendo",      "nintendo"    },
            { "Telegram",      "telegram"    },
            { "7 Ding",        "win7-ding"   },
            { "7 Error",       "win7-error"  },
            { "7 Exclamation", "win7-exc"    },
            { "AOL Alert",     "aol-alert"   },
            { "AOL Message",   "aol-msg"     },
            { "Thunderbird",   "thunderbird" },
            { "Pixie Dust",    "pixiedust"   },
            { "Moon Beam",     "moonbeam"    },
            { "Dog",           "dog"         },
            { "GMod Error",    "gmod-error"  }
        };

        public static void ChangeNotificationSound(bool positive = true, bool fromMenu = false)
        {
            if (positive)
                notificationSoundIndex++;
            else
                notificationSoundIndex--;

            notificationSoundIndex %= notificationSounds.Keys.Count;
            if (notificationSoundIndex < 0)
                notificationSoundIndex = notificationSounds.Keys.Count - 1;

            Buttons.GetIndex("Change Notification Sound").overlapText = "Change Notification Sound <color=grey>[</color><color=green>" + notificationSounds.Keys.ToArray()[notificationSoundIndex] + "</color><color=grey>]</color>";

            if (fromMenu)
            {
                audioManager.GetComponent<AudioSource>().Stop();
                NotificationManager.PlayNotificationSound();
            }
        }

        public static void ChangeNarrationVoice(bool positive = true)
        {
            string[] narratorNames = {
                "Default",
                "Kimberly",
                "Brian",
                "Matthew",
                "Joey",
                "Justin",
                "Cristiano",
                "Giorgio",
                "Ewa",
                "TikTok",
                "Grandma",
                "Trickster",
                "Elf",
                "Ghostface",
                "Zombie",
                "Narrator",
                "Pirate",
                "Song",
                "TikTok Joey",
                "Gingerbread Man",
                "Chris",
                "Thanksgiving",
                "Santa",
                "Google US",
                "Google UK",
                "Dog",
                "Jerkface",
                "Robot",
                "Vlad",
                "Obama"/*,
                "Mommy ASMR"*/
            };

            if (positive)
                narratorIndex++;
            else
                narratorIndex--;

            narratorIndex %= narratorNames.Length;
            if (narratorIndex < 0)
                narratorIndex = narratorNames.Length - 1;

            Buttons.GetIndex("Change Narration Voice").overlapText = "Change Narration Voice <color=grey>[</color><color=green>" + narratorNames[narratorIndex] + "</color><color=grey>]</color>";
            narratorName = narratorNames[narratorIndex];

            if (krec != null && krec.IsRunning && Time.time > dRestartTime)
            {
                DictationRestart();
                dRestartTime = Time.time + 1f;
            }
        }

        public static void KickToSpecificRoom()
        {
            if (Time.time < timeMenuStarted + 5f)
            {
                Buttons.GetIndex("Kick to Specific Room").enabled = false;
                return;
            }

            PromptText("What would you like the room code to be?", () => Overpowered.specificRoom = keyboardInput.ToUpper(), () => Toggle("Kick to Specific Room"), "Done", "Cancel");
        }
        public static void ChangePointerPosition(bool positive = true)
        {
            Vector3[] pointerPos = {
                new Vector3(0f, -0.1f, 0f),
                new Vector3(0f, -0.1f, -0.15f),
                new Vector3(0f, 0.1f, -0.05f),
                new Vector3(0f, 0.0666f, 0.1f)
            };

            if (positive)
                pointerIndex++;
            else
                pointerIndex--;

            pointerIndex %= pointerPos.Length;
            if (pointerIndex < 0)
                pointerIndex = pointerPos.Length - 1;

            pointerOffset = pointerPos[pointerIndex];
            try { reference.transform.localPosition = pointerOffset; } catch { }
        }

        // Credits to Scintilla for the idea
        public static void ChangeGunVariation(bool positive = true)
        {
            string[] VariationNames = {
                "Default",
                "Lightning",
                "Wavy",
                "Blocky",
                "Zigzag",
                "Spring",
                "Bouncy",
                "Audio",
                "Bezier",
                "Rope"
            };

            if (positive)
                gunVariation++;
            else
                gunVariation--;

            gunVariation %= VariationNames.Length;
            if (gunVariation < 0)
                gunVariation = VariationNames.Length - 1;

            Buttons.GetIndex("Change Gun Variation").overlapText = "Change Gun Variation <color=grey>[</color><color=green>" + VariationNames[gunVariation] + "</color><color=grey>]</color>";
        }

        public static void ChangeGunDirection(bool positive = true)
        {
            string[] DirectionNames = {
                "Default",
                "Legacy",
                "Laser",
                "Finger",
                "Face"
            };

            if (positive)
                GunDirection++;
            else
                GunDirection--;

            GunDirection %= DirectionNames.Length;
            if (GunDirection < 0)
                GunDirection = DirectionNames.Length - 1;

            Buttons.GetIndex("Change Gun Direction").overlapText = "Change Gun Direction <color=grey>[</color><color=green>" + DirectionNames[GunDirection] + "</color><color=grey>]</color>";
        }

        private static int gunLineQualityIndex = 2;
        public static void ChangeGunLineQuality(bool positive = true)
        {
            string[] Names = {
                "Potato",
                "Low",
                "Normal",
                "High",
                "Extreme"
            };

            int[] Qualities = {
                10,
                25,
                50,
                100,
                250
            };

            if (positive)
                gunLineQualityIndex++;
            else
                gunLineQualityIndex--;

            gunLineQualityIndex %= Names.Length;
            if (gunLineQualityIndex < 0)
                gunLineQualityIndex = Names.Length - 1;

            GunLineQuality = Qualities[gunLineQualityIndex];
            Buttons.GetIndex("Change Gun Line Quality").overlapText = "Change Gun Line Quality <color=grey>[</color><color=green>" + Names[gunLineQualityIndex] + "</color><color=grey>]</color>";
        }

        public static void FreezePlayerInMenu()
        {
            if (physicalMenu ? isMenuButtonHeld : menu != null)
            {
                if (closePosition == Vector3.zero)
                    closePosition = GorillaTagger.Instance.rigidbody.transform.position;
                else
                    GorillaTagger.Instance.rigidbody.transform.position = closePosition;
                GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
            } else
                closePosition = Vector3.zero;
        }
        
        public static bool currentmentalstate;
        public static void FreezeRigInMenu()
        {
            if (menu != null)
            {
                if (!currentmentalstate)
                {
                    currentmentalstate = true;
                    VRRig.LocalRig.enabled = false;
                }
            }
            else
            {
                if (currentmentalstate)
                {
                    currentmentalstate = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void DisorganizeMenu()
        {
            if (!disorganized)
            {
                disorganized = true;
                foreach (ButtonInfo[] buttonArray in Buttons.buttons)
                {
                    if (buttonArray.Length > 0)
                    {
                        for (int i = 0; i < buttonArray.Length; i++)
                            Buttons.buttons[Buttons.GetCategory("Main")] = Buttons.buttons[Buttons.GetCategory("Main")].Concat(new[] { buttonArray[i] }).ToArray();

                        Array.Clear(buttonArray, 0, buttonArray.Length);
                    }
                }
            }
        }

        public static void AnnoyingModeOff()
        {
            annoyingMode = false;
            themeType--;
            ChangeMenuTheme();
        }

        public static void DisablePageButtons()
        {
            if (Buttons.GetIndex("Joystick Menu").enabled) {
                disablePageButtons = true;
            } else
            {
                Buttons.GetIndex("Disable Page Buttons").enabled = false;
                NotificationManager.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> Disable Page Buttons can only be used when using Joystick Menu.");
            }
        }

        public static void CustomMenuName()
        {
            doCustomName = true;
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt", "Your Text Here");
            
            customMenuName = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt");
        }

        private static bool lastFocused;
        public static void CheckFocus()
        {
            if (!Application.isFocused && lastFocused && Time.time > timeMenuStarted + 5f)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not focused on Gorilla Tag. Voice transcription mods will not function. Please focus/click on the game.");

            lastFocused = Application.isFocused;
        }

        // Thanks to kingofnetflix for inspiration and support with voice recognition
        private static KeywordRecognizer mainPhrases;
        private static KeywordRecognizer modPhrases;
        private static string[] keyWords = { "jarvis", "ii", "i i", "eye eye", "siri", "google", "alexa", "dummy", "computer", "stinky", "silly", "stupid", "console", "go go gadget", "monika", "wikipedia", "gideon", "a i", "ai", "a.i", "chat gpt", "chatgpt", "grok", "grock", "garmin" };
        private static readonly string[] cancelKeywords = { "nevermind", "cancel", "never mind", "stop", "i hate you", "die" };
        public static void VoiceRecognitionOn()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt"))
                File.WriteAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt", keyWords);
            keyWords = File.ReadAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt");
            mainPhrases = new KeywordRecognizer(keyWords);
            mainPhrases.OnPhraseRecognized += ModRecognition;
            mainPhrases.Start();          
        }

        private static Coroutine timeoutCoroutine;
        public static void ModRecognition(PhraseRecognizedEventArgs args)
        {
            mainPhrases.Stop();

            if (!Buttons.GetIndex("Chain Voice Commands").enabled)
                timeoutCoroutine = CoroutineManager.instance.StartCoroutine(Timeout(string.Empty));
            
            List<string> rawbuttonnames = cancelKeywords.ToList();

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    string buttonName = v.overlapText ?? v.buttonText;

                    if (buttonName.Contains(" <color"))
                        buttonName = buttonName.Split(" <color")[0];

                    rawbuttonnames.Add(buttonName);
                }
            }


            modPhrases = new KeywordRecognizer(rawbuttonnames.ToArray());
            modPhrases.OnPhraseRecognized += ExecuteVoiceCommand;
            modPhrases.Start();

            if (dynamicSounds)
                DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/select.ogg", "Audio/Menu/select.ogg"), buttonClickVolume / 10f);
            
            NotificationManager.SendNotification("<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> Listening...", 3000);
        }

        public static void ExecuteVoiceCommand(PhraseRecognizedEventArgs args)
        {
            if (!Buttons.GetIndex("Chain Voice Commands").enabled)
            {
                modPhrases.Stop();
                mainPhrases.Start();
                CoroutineManager.instance.StopCoroutine(timeoutCoroutine);
            }

            if (cancelKeywords.Contains(args.text))
            {
                CancelModRecognition(args.text);
                return;
            }

            string modTarget = null;
            bool exactMatch = false;

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                if (exactMatch)
                    break;

                foreach (ButtonInfo v in buttonlist)
                {
                    if (exactMatch)
                        break;

                    string buttonName = v.overlapText ?? v.buttonText;

                    if (buttonName.Contains(" <color"))
                        buttonName = buttonName.Split(" <color")[0];

                    if (args.text.ToLower() == buttonName.ToLower())
                    {
                        modTarget = v.buttonText;
                        exactMatch = true;
                    } else
                    {
                        if (args.text.Contains(buttonName.ToLower()))
                            modTarget = v.buttonText;
                    }
                }
            }

            if (modTarget != null)
            {
                ButtonInfo mod = Buttons.GetIndex(modTarget);
                NotificationManager.SendNotification("<color=grey>[</color><color=" + (mod.enabled ? "red" : "green") + ">VOICE</color><color=grey>]</color> " + (mod.enabled ? "Disabling " : "Enabling ") + (mod.overlapText ?? mod.buttonText) +"...", 3000);
                if (dynamicSounds)
                    DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg"), buttonClickVolume / 10f);
                
                Toggle(modTarget, true, true);
            } else
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> No command found ("+args.text+").", 3000);
                if (dynamicSounds)
                    DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
            }
        }

        public static IEnumerator Timeout(string text)
        {
            yield return new WaitForSeconds(10f);
            CancelModRecognition(text);
        }

        public static void CancelModRecognition(string text)
        {
            modPhrases.Stop();
            mainPhrases.Start();
            try
            {
                CoroutineManager.instance.StopCoroutine(timeoutCoroutine);
            } catch { }
            
            NotificationManager.SendNotification($"<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> {(text == "i hate you" ? "I hate you too." : "Cancelling...")}", 3000);
            if (dynamicSounds)
                DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
        }

        public static void VoiceRecognitionOff()
        {
            mainPhrases?.Dispose();
            mainPhrases?.Stop();
            modPhrases?.Dispose();
            modPhrases?.Stop();
            mainPhrases = null;
            modPhrases = null;
            PhraseRecognitionSystem.Shutdown();
        }

        // Thanks to kingofnetflix for inspiration and support with voice recognition
        public static DictationRecognizer drec;
        public static KeywordRecognizer krec;
        public static bool debugDictation;
        public static bool restartOnFocus;
        public static float dRestartTime;

        public static IEnumerator DictationOn()
        {
            

            ButtonInfo mod = Buttons.GetIndex("AI Assistant");

            if (Application.platform == RuntimePlatform.WindowsPlayer && Environment.OSVersion.Version.Major < 10)
                PromptSingle("Your version of Windows is too old for this mod to run.", () => mod.enabled = false);
            else if (Application.platform != RuntimePlatform.WindowsPlayer)
                PromptSingle("You must be on Windows 10 or greater for this mod to run.", () => mod.enabled = false);


            ButtonInfo vc = Buttons.GetIndex("Voice Commands");
            if (vc.enabled)
                Prompt("You currently have Voice Commands enabled. Would you like to disable it?", () => vc.enabled = false, () => mod.enabled = false);
            else if (PhraseRecognitionSystem.Status != SpeechSystemStatus.Stopped)
                PromptSingle("You can not use AI Assistant while you have another voice-related mod on.", () => mod.enabled = false, "Ok");

            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt"))
                File.WriteAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt", keyWords);
            keyWords = File.ReadAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt");

            while (PhraseRecognitionSystem.Status != SpeechSystemStatus.Stopped)
                yield return null;

            string[] kw = keyWords;
            if (narratorName == "Mommy ASMR")
               kw = kw.Concat(new[] { "mommy", "momma" }).ToArray();

            krec = new KeywordRecognizer(kw);

            krec.OnPhraseRecognized += (args) => CoroutineManager.instance.StartCoroutine(DictationRecognizer());
            krec.Start();
            yield break;
        }

        public static IEnumerator DictationRecognizer()
        {
            ButtonInfo mod = Buttons.GetIndex("AI Assistant");

           

            PhraseRecognitionSystem.Shutdown();
            while (PhraseRecognitionSystem.Status != SpeechSystemStatus.Stopped)
                yield return null;

            switch (narratorName)
            {
                case "Mommy ASMR":
                    DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/TTS/yes_sweetheart.ogg", "Audio/TTS/yes_sweetheart.ogg"), buttonClickVolume / 10f);
                    NotificationManager.SendNotification("<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> Yes, sweetheart?", 3000);
                    break;
                default:
                    DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/select.ogg", "Audio/Menu/select.ogg"), buttonClickVolume / 10f);
                    NotificationManager.SendNotification("<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> Listening...", 3000);
                    break;
            }

            if (debugDictation)
                LogManager.Log("Dictation listening");

            drec = new DictationRecognizer();
            drec.DictationResult += (text, confidence) =>
            {
                if (debugDictation)
                    LogManager.Log($"Dictation result: {text}");
                if (cancelKeywords.Contains(text.ToLower()))
                {
                    if (dynamicSounds)
                        DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
                        
                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>AI</color><color=grey>]</color> {(text.ToLower() == "i hate you" ? "I hate you too." : "Cancelling...")}", 3000);
                    CoroutineManager.instance.StartCoroutine(DictationRestart());
                    return;
                }

                switch (narratorName)
                {
                    case "Mommy ASMR":
                        NotificationManager.SendNotification($"<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> Let me get that for you..");
                        break;
                    default:
                        NotificationManager.SendNotification($"<color=grey>[</color><color=blue>AI</color><color=grey>]</color> Generating response..");
                        break;

                }
                
                
                CoroutineManager.instance.StartCoroutine(AIManager.AskAI(text));
                return;
                    
            };

            drec.DictationComplete += (completionCause) =>
            {
                if (debugDictation)
                    LogManager.Log($"completion cause: {completionCause}");
                if (completionCause.ToString() == "TimeoutExceeded")
                {
                    if (dynamicSounds)
                        DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>AI</color><color=grey>]</color> Cancelling...", 3000);
                }
            };

            drec.DictationError += (error, hresult) =>
            {
                if (debugDictation)
                    LogManager.LogError($"Dictation error: {error}");
                if (error.Contains("Dictation support is not enabled on this device"))
                {
                    DictationOff();

                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Online Speech Recognition is not enabled on this device. Either open the menu to enable it, or check your internet connection.", 3000);
                    Prompt("Online Speech Recognition is not enabled on your device. Would you like to open the Settings page to enable it?", () => { Process.Start("ms-settings:privacy-speech"); PromptSingle("Once you enable Online Speech Recognition, turn this mod back on!", () => mod.enabled = false, "Ok"); }, () => PromptSingle("You will not be able to use this mod until you enable Online Speech Recognition.", () => mod.enabled = false, "Ok"));
                }
            };

            drec.DictationHypothesis += (text) =>
            {
                if (AIManager.generating)
                    return;
                if (debugDictation)
                    LogManager.Log($"Hypothesis: {text}");

                NotificationManager.ClearAllNotifications();
                NotificationManager.SendNotification($"<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> {text}");
            };

            drec?.Start();
            yield break;
        }

        public static IEnumerator DictationRestart()
        {
            DictationOff();
            while (PhraseRecognitionSystem.Status != SpeechSystemStatus.Stopped)
                yield return null;
            CoroutineManager.instance.StartCoroutine(DictationOn());
            yield break;
        }
        public static void DictationOff() 
        {
            drec?.Dispose();
            drec?.Stop();
            drec = null;
            PhraseRecognitionSystem.Shutdown();
        }

        public static void DictationPlay(AudioClip clip, float volume)
        {
            bool enabled = Buttons.GetIndex("Global Dynamic Sounds").enabled;
            switch (enabled)
            {
                case true:
                    Sound.PlayAudio(clip);
                    break;
                case false:
                    Play2DAudio(clip, volume);
                    break;
            }
        }

        private static LineRenderer clickGuiLine;
        private static bool lastTriggerClick;

        private static EventSystem eventSystem;
        private static PointerEventData pointerData;
        private static readonly List<RaycastResult> uiResults = new List<RaycastResult>();
        private static GameObject currentUI;

        private static GameObject pressedUI;
        private static GameObject draggedUI;
        private static Vector2 lastPointerPos;
        private static Canvas canvas;

        private static bool isDragging;

        public static void ReloadOnCategoryChange() =>
            ReloadMenu();

        public static void EnableClickGUI()
        {
            clickGUI = true; 
            ReloadMenu();

            Buttons.OnCategoryChanged += ReloadOnCategoryChange;
        }

        public static void DisableClickGUI()
        {
            clickGUI = false;
            Buttons.OnCategoryChanged -= ReloadOnCategoryChange;

            if (clickGuiLine != null)
            {
                Object.Destroy(clickGuiLine.gameObject);
                clickGuiLine = null;
            }
        }

        public static void InitializeClickGUI()
        {
            canvas = menu.transform.Find("Canvas").GetComponent<Canvas>();
            
            if (!XRSettings.isDeviceActive)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1;

                canvas.gameObject.transform.Find("Main").AddComponent<UIDragWindow>();
            }

            Transform canvasTransform = canvas.gameObject.transform;
            canvasTransform.Find("Main").AddComponent<UIColorChanger>().colors = backgroundColor;

            ExtGradient sidebarColor = buttonColors[1].Clone();
            for (int i = 0; i < sidebarColor.colors.Length; i++)
            {
                GradientColorKey colorKey = sidebarColor.colors[i];
                sidebarColor.colors[i] = new GradientColorKey { time = colorKey.time, color = DarkenColor(colorKey.color, 0.35f) };
            }

            canvasTransform.Find("Main/Sidebar").AddComponent<UIColorChanger>().colors = sidebarColor;
            canvasTransform.Find("Main/Separator").AddComponent<UIColorChanger>().colors = buttonColors[1];

            canvasTransform.Find("Main/Sidebar/Watermark").localRotation = Quaternion.Euler(0f, 0f, rockWatermark ? Mathf.Sin(Time.time * 2f) * 10f : 0f);

            List<MaskableGraphic> toRecolor = new List<MaskableGraphic>();
            foreach (string partName in new[]
            {
                "Main/Sidebar/Title",
                "Main/Sidebar/Watermark",
                "Main/Sidebar/Settings",
                "Main/Sidebar/Players",
                "Main/Sidebar/Friends",
                "Main/Sidebar/Scroll View/Scrollbar Vertical/Sliding Area/Handle"
            })
                toRecolor.Add(canvasTransform.Find(partName).GetComponent<MaskableGraphic>());

            Transform sidebarTransform = canvasTransform.Find("Main/Sidebar");
            foreach (string buttonName in new[]
            {
                "Settings", "Players", "Friends"
            })
                sidebarTransform.Find(buttonName).GetComponent<Button>().onClick.AddListener(() =>
                {
                    Toggle(buttonName);
                    PlayButtonSound();
                });

            var selection = canvasTransform.Find("Main/Sidebar/Scroll View/Viewport/Content/Home/Selection");
            selection.AddComponent<UIColorChanger>().colors = buttonColors[1];

            bool movedSelection = false;

            string[] ignoreButtons = {
                "Join Discord",
                "Settings",
                "Friends",
                "Players",
                "Favorite Mods",
                "Enabled Mods",
                "Room Mods",
                "Important Mods",
                "Safety Mods",
                "Movement Mods",
                "Advantage Mods",
                "Visual Mods",
                "Fun Mods",
                "Sound Mods",
                "Projectile Mods",
                "Master Mods",
                "Overpowered Mods",
                "Experimental Mods",
                "Detected Mods",
                "Achievements",
                "Credits"
            };

            GameObject otherBase = canvasTransform.Find("Main/Sidebar/Scroll View/Viewport/Content/Other").gameObject;
            foreach (ButtonInfo button in Buttons.buttons[Buttons.GetCategory("Main")])
            {
                if (!ignoreButtons.Contains(button.buttonText))
                {
                    GameObject otherButton = Object.Instantiate(otherBase, canvasTransform.Find("Main/Sidebar/Scroll View/Viewport/Content"), false);
                    otherButton.SetActive(true);
                    otherButton.name = button.buttonText;
                    otherButton.transform.Find("Title").GetComponent<TextMeshProUGUI>().SafeSetText(button.buttonText);
                }
            }

            foreach (GameObject tab in canvasTransform.Find("Main/Sidebar/Scroll View/Viewport/Content").Children())
            {
                if (!tab.activeSelf)
                    continue;

                toRecolor.Add(tab.transform.Find("Title").GetComponent<MaskableGraphic>());
                toRecolor.Add(tab.transform.Find("Image").GetComponent<MaskableGraphic>());

                tab.AddComponent<UIColorChanger>().colors = buttonColors[0];
                tab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Toggle(Buttons.buttons[Buttons.GetCategory("Main")].Where(button => button.buttonText.StartsWith(tab.name)).FirstOrDefault() ?? Buttons.GetIndex("Exit Settings"));
                    PlayButtonSound();
                });

                if (Buttons.CurrentCategoryName.StartsWith(tab.name == "Home" ? "Main" : tab.name))
                {
                    movedSelection = true;
                    selection.SetParent(tab.transform, false);
                }
                else
                {
                    tab.transform.Find("Title").GetComponent<RectTransform>().localPosition += Vector3.left * 10f;
                    tab.transform.Find("Image").GetComponent<RectTransform>().localPosition += Vector3.left * 10f;
                }
            }

            if (!movedSelection)
                selection.gameObject.SetActive(false);

            GameObject buttonTemplate = canvasTransform.Find("Main/Button").gameObject;
            void AddButton(Transform parent, ButtonInfo info)
            {
                static void UpdateButton(GameObject button, ButtonInfo info)
                {
                    Transform transform = button.transform;
                    string buttonText = info.overlapText ?? info.buttonText;

                    if (inputTextColor != "green")
                        buttonText = buttonText.Replace(" <color=grey>[</color><color=green>", $" <color=grey>[</color><color={inputTextColor}>");

                    buttonText = FixTMProTags(buttonText);
                    buttonText = FollowMenuSettings(buttonText);

                    transform.Find("Title").GetComponent<TextMeshProUGUI>().SafeSetText(buttonText);
                    transform.Find("Title").GetComponent<TextMeshProUGUI>().spriteAsset = ButtonSpriteSheet;

                    string toolTipText = info.toolTip;

                    if (inputTextColor != "green")
                        toolTipText = toolTipText.Replace("<color=green>", $"<color={inputTextColor}>");

                    toolTipText = FixTMProTags(toolTipText);
                    toolTipText = FollowMenuSettings(toolTipText);

                    transform.Find("ToolTip").GetComponent<TextMeshProUGUI>().SafeSetText(toolTipText);

                    transform.Find("Title").GetComponent<TextMeshProUGUI>().Chams();
                    transform.Find("ToolTip").GetComponent<TextMeshProUGUI>().Chams();

                    button.name = buttonText;

                    if (info.label)
                    {
                        RectTransform title = transform.Find("Title").gameObject.GetComponent<RectTransform>();
                        title.anchorMin = new Vector2(0.5f, 0.5f);
                        title.anchorMax = new Vector2(0.5f, 0.5f);

                        title.localPosition = new Vector3(0f, 0f, 0f);

                        transform.Find("ToolTip").gameObject.SetActive(false);
                        transform.Find("Toggle").gameObject.SetActive(false);
                        transform.Find("Increment").gameObject.SetActive(false);
                        transform.Find("Decrement").gameObject.SetActive(false);
                    }
                    else if (info.incremental)
                    {
                        transform.Find("Increment").gameObject.SetActive(true);
                        transform.Find("Decrement").gameObject.SetActive(true);

                        transform.Find("Increment").gameObject.GetOrAddComponent<UIColorChanger>().colors = buttonColors[0];
                        transform.Find("Decrement").gameObject.GetOrAddComponent<UIColorChanger>().colors = buttonColors[0];

                        transform.Find("Increment/Image").gameObject.GetOrAddComponent<UIColorChanger>().colors = textColors[1];
                        transform.Find("Decrement/Image").gameObject.GetOrAddComponent<UIColorChanger>().colors = textColors[1];
                    }
                    else
                    {
                        transform.Find("Toggle").gameObject.SetActive(true);
                        transform.Find("Toggle/Image").gameObject.SetActive(info.enabled);

                        transform.Find("Toggle").gameObject.GetOrAddComponent<UIColorChanger>().colors = info.enabled ? buttonColors[1] : buttonColors[0];
                        transform.Find("Toggle/Image").gameObject.GetOrAddComponent<UIColorChanger>().colors = info.enabled ? textColors[2] : textColors[1];
                    }

                    transform.Find("Title").AddComponent<UIColorChanger>().colors = textColors[1];
                    transform.Find("ToolTip").AddComponent<UIColorChanger>().colors = textColors[1];
                }

                GameObject button = Object.Instantiate(buttonTemplate, parent, false);
                button.SetActive(true);

                ExtGradient buttonBackgroundColor = backgroundColor.Clone();
                for (int i = 0; i < buttonBackgroundColor.colors.Length; i++)
                {
                    GradientColorKey colorKey = buttonBackgroundColor.colors[i];
                    buttonBackgroundColor.colors[i] = new GradientColorKey { time = colorKey.time, color = DarkenColor(colorKey.color, 0.75f) };
                }
                button.AddComponent<UIColorChanger>().colors = buttonBackgroundColor;

                UpdateButton(button, info);

                Transform transform = button.transform;
                if (info.incremental)
                    {
                        transform.Find("Increment").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ToggleIncremental(info.buttonText, true);
                            PlayButtonSound();
                            UpdateButton(button, info);
                        });
                        transform.Find("Decrement").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ToggleIncremental(info.buttonText, false);
                            PlayButtonSound();
                            UpdateButton(button, info);
                        });
                    }
                    else
                    {
                        transform.Find("Toggle").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            Toggle(info);
                            PlayButtonSound();
                            UpdateButton(button, info);
                        });
                    }
            }

            if (CurrentPrompt != null)
            {
                canvasTransform.Find("Main/PromptTab").gameObject.SetActive(true);

                foreach (string partName in new[]
                    {
                        "Main/PromptTab/Title",
                        "Main/PromptTab/Accept/Text",
                        "Main/PromptTab/Decline/Text"
                    })
                    toRecolor.Add(canvasTransform.Find(partName).GetComponent<MaskableGraphic>());

                GameObject title = canvasTransform.Find("Main/PromptTab/Title").gameObject;
                title.GetComponent<TextMeshProUGUI>().SafeSetText(CurrentPrompt.Message);

                GameObject accept = canvasTransform.Find("Main/PromptTab/Accept").gameObject;
                accept.transform.Find("Text").GetComponent<TextMeshProUGUI>().SafeSetText(CurrentPrompt.AcceptText);
                accept.transform.Find("Text").GetComponent<TextMeshProUGUI>().Chams();
                accept.GetOrAddComponent<UIColorChanger>().colors = buttonColors[0];
                accept.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Toggle("Accept Prompt");
                    PlayButtonSound();
                    ReloadMenu();
                });

                if (CurrentPrompt.DeclineText != null)
                {
                    GameObject decline = canvasTransform.Find("Main/PromptTab/Decline").gameObject;
                    decline.transform.Find("Text").GetComponent<TextMeshProUGUI>().SafeSetText(CurrentPrompt.DeclineText);
                    decline.GetOrAddComponent<UIColorChanger>().colors = buttonColors[0];
                    decline.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Toggle("Decline Prompt");
                        PlayButtonSound();
                        ReloadMenu();
                    });
                }
                else
                {
                    canvasTransform.Find("Main/PromptTab/Decline").gameObject.SetActive(false);

                    RectTransform rectTransform = accept.GetComponent<RectTransform>();
                    rectTransform.localPosition = new Vector3(title.GetComponent<RectTransform>().localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z);
                    rectTransform.localScale = new Vector3(rectTransform.localScale.y * 2.05f, rectTransform.localScale.y, rectTransform.localScale.z);

                    accept.transform.Find("Text").GetComponent<RectTransform>().localScale = new Vector3(rectTransform.localScale.y / 2.05f, rectTransform.localScale.y, rectTransform.localScale.z);
                }
            }
            else if (Buttons.CurrentCategoryIndex == 0)
            {
                canvasTransform.Find("Main/HomeTab").gameObject.SetActive(true);
                canvasTransform.Find("Main/HomeTab/Title").GetComponent<TextMeshProUGUI>().SafeSetText($"Hey, {PhotonNetwork.NickName ?? "null"}!");

                if (Buttons.CurrentCategoryIndex == 0)
                {
                    foreach (string partName in new[]
                    {
                        "Main/HomeTab/Title",
                        "Main/HomeTab/EnabledTitle",
                        "Main/HomeTab/FavoritesTitle",
                        "Main/HomeTab/EnabledIcon",
                        "Main/HomeTab/FavoritesIcon",
                        "Main/HomeTab/Enabled/Viewport/Content/None",
                        "Main/HomeTab/Favorites/Viewport/Content/None",
                        "Main/HomeTab/Enabled/Scrollbar Vertical/Sliding Area/Handle",
                        "Main/HomeTab/Favorites/Scrollbar Vertical/Sliding Area/Handle"
                    })
                        toRecolor.Add(canvasTransform.Find(partName).GetComponent<MaskableGraphic>());
                }

                Transform enabledModsTransform = canvasTransform.Find("Main/HomeTab/Enabled/Viewport/Content");

                List<ButtonInfo> enabledMods = new List<ButtonInfo>();
                int categoryIndex = 0;
                foreach (ButtonInfo[] buttonList in Buttons.buttons)
                {
                    enabledMods.AddRange(buttonList.Where(v => v.enabled && (!hideSettings || !Buttons.categoryNames[categoryIndex].Contains("Settings")) && (!hideMacros || !Buttons.categoryNames[categoryIndex].Contains("Macro"))));
                    categoryIndex++;
                }
                enabledMods = enabledMods.OrderBy(v => v.overlapText ?? v.buttonText).ToList();
                
                if (enabledMods.Count > 0)
                {
                    canvasTransform.Find("Main/HomeTab/Enabled/Viewport/Content/None").gameObject.SetActive(false);
                    foreach (ButtonInfo info in enabledMods)
                        AddButton(enabledModsTransform, info);
                }

                Transform favoritedModsTransform = canvasTransform.Find("Main/HomeTab/Favorites/Viewport/Content");
                List<ButtonInfo> favoriteMods = StringsToInfos(favorites.ToArray()).ToList();

                if (favoriteMods.Count > 0)
                    favoriteMods.RemoveAt(0);

                if (favoriteMods.Count > 0)
                {
                    canvasTransform.Find("Main/HomeTab/Favorites/Viewport/Content/None").gameObject.SetActive(false);
                    foreach (ButtonInfo info in favoriteMods)
                        AddButton(favoritedModsTransform, info);
                }
            }
            else
            {
                canvasTransform.Find("Main/ModuleTab").gameObject.SetActive(true);

                foreach (string partName in new[]
                    {
                        "Main/ModuleTab/Search/SearchIcon",
                        "Main/ModuleTab/Search/Text Area/Placeholder",
                        "Main/ModuleTab/Search/Text Area/Text",
                        "Main/ModuleTab/Modules/Scrollbar Vertical/Sliding Area/Handle"
                    })
                    toRecolor.Add(canvasTransform.Find(partName).GetComponent<MaskableGraphic>());

                List<ButtonInfo> buttons = Buttons.buttons[Buttons.CurrentCategoryIndex].ToList();

                if (buttons.Count > 0 && ignoreButtons.Contains(Buttons.CurrentCategoryName))
                    buttons.RemoveAt(0);

                if (buttons.Count > 0)
                {
                    Transform modulesTransform = canvasTransform.Find("Main/ModuleTab/Modules/Viewport/Content");
                    foreach (ButtonInfo button in buttons)
                        AddButton(modulesTransform, button);
                }

                Transform searchBar = canvasTransform.Find("Main/ModuleTab/Search");
                TMP_InputField inputField = searchBar.GetComponent<TMP_InputField>();

                inputField.onSelect.AddListener(_ =>
                {
                    if (!isSearching)
                        Search();
                });

                inputField.onDeselect.AddListener(_ =>
                {
                    if (isSearching && keyboardInput.IsNullOrEmpty())
                        Search();
                });
            }

            for (int i = 0; i < toRecolor.Count; i++)
            {
                MaskableGraphic graphic = toRecolor[i];
                graphic.gameObject.AddComponent<UIColorChanger>().colors = textColors[i <= 1 ? 0 : 1];

                if (graphic is TMP_Text text)
                    text.Chams();
            }

            ExtGradient buttonBackgroundColor = backgroundColor.Clone();
            for (int i = 0; i < buttonBackgroundColor.colors.Length; i++)
            {
                GradientColorKey colorKey = buttonBackgroundColor.colors[i];
                buttonBackgroundColor.colors[i] = new GradientColorKey { time = colorKey.time, color = DarkenColor(colorKey.color, 0.75f) };
            }
            canvasTransform.Find("Main/ModuleTab/Search").AddComponent<UIColorChanger>().colors = buttonBackgroundColor;

            Canvas.ForceUpdateCanvases();
        }

        public static void UpdateSearch()
        {
            Transform searchBar = canvas.transform.Find("Main/ModuleTab/Search");
            TMP_InputField inputField = searchBar.GetComponent<TMP_InputField>();

            inputField.text = keyboardInput;
            foreach (GameObject button in canvas.transform.Find("Main/ModuleTab/Modules/Viewport/Content").Children())
                button.SetActive(keyboardInput.IsNullOrEmpty() || button.name.ClearTags().Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()));
        }

        public static void ClickGUI()
        {
            if (menu == null)
            {
                if (clickGuiLine != null)
                {
                    Object.Destroy(clickGuiLine.gameObject);
                    clickGuiLine = null;
                }
            }
            else
            {
                canvas.transform.Find("Main/Sidebar/Watermark").localRotation = Quaternion.Euler(0f, 0f, rockWatermark ? Mathf.Sin(Time.time * 2f) * 10f : 0f);

                if (isSearching && Buttons.CurrentCategoryIndex != 0)
                {
                    Transform searchBar = canvas.transform.Find("Main/ModuleTab/Search");
                    TMP_InputField inputField = searchBar.GetComponent<TMP_InputField>();

                    if (inputField.text != keyboardInput)
                        UpdateSearch();
                }

                if (!XRSettings.isDeviceActive)
                    return;

                if (clickGuiLine == null)
                {
                    clickGuiLine = new GameObject("iiMenu_ClickGUILine")
                        .GetOrAddComponent<LineRenderer>();

                    clickGuiLine.material = new Material(Shader.Find("GUI/Text Shader"));
                    clickGuiLine.startWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                    clickGuiLine.endWidth = clickGuiLine.startWidth;
                    clickGuiLine.useWorldSpace = true;
                    clickGuiLine.positionCount = 2;

                    if (smoothLines)
                    {
                        clickGuiLine.numCapVertices = 10;
                        clickGuiLine.numCornerVertices = 5;
                    }
                }

                clickGuiLine.startColor = backgroundColor.GetCurrentColor();
                clickGuiLine.endColor = backgroundColor.GetCurrentColor(0.5f);

                var uiRaycaster = canvas.GetComponent<GraphicRaycaster>();
                eventSystem ??= EventSystem.current;

                pointerData ??= new PointerEventData(eventSystem);

                bool useLeft = rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton);

                var (_, _, _, forward, _) = useLeft
                    ? ControllerUtilities.GetTrueLeftHand()
                    : ControllerUtilities.GetTrueRightHand();

                Vector3 startPos = useLeft
                    ? GorillaTagger.Instance.leftHandTransform.position
                    : GorillaTagger.Instance.rightHandTransform.position;

                Vector3 direction = forward.normalized;

                Vector3 screenPoint = Camera.main.WorldToScreenPoint(startPos + direction * 5f);
                pointerData.position = screenPoint;

                uiResults.Clear();
                uiRaycaster.Raycast(pointerData, uiResults);

                currentUI = uiResults.Count > 0 ? uiResults[0].gameObject : null;

                Vector3 endPos = currentUI != null
                    ? uiResults[0].worldPosition
                    : startPos + direction * 5f;

                clickGuiLine.SetPosition(0, startPos);
                clickGuiLine.SetPosition(1, endPos);

                bool trigger = useLeft ? leftTrigger > 0.5f : rightTrigger > 0.5f;
                Vector2 currentPos = pointerData.position;
                pointerData.delta = currentPos - lastPointerPos;
                lastPointerPos = currentPos;

                if (trigger && !lastTriggerClick && currentUI != null)
                {
                    GameObject targetUI = null;
                    foreach (var result in uiResults)
                    {
                        var button = result.gameObject.GetComponent<Button>();
                        var toggle = result.gameObject.GetComponent<Toggle>();
                        var slider = result.gameObject.GetComponent<Slider>();
                        var inputField = result.gameObject.GetComponent<TMP_InputField>();

                        if (button != null || toggle != null || slider != null || inputField != null)
                        {
                            targetUI = result.gameObject;
                            break;
                        }
                    }

                    pressedUI = targetUI ?? currentUI;
                    pointerData.pressPosition = currentPos;
                    pointerData.pointerPressRaycast = uiResults[0];

                    ExecuteEvents.Execute(pressedUI, pointerData, ExecuteEvents.pointerDownHandler);
                    pointerData.pointerPress = pressedUI;

                    isDragging = false;
                    draggedUI = ExecuteEvents.GetEventHandler<IDragHandler>(currentUI);
                    pointerData.pointerDrag = draggedUI ?? null;
                }

                switch (trigger)
                {
                    case true when draggedUI != null:
                        {
                            if (!isDragging)
                            {
                                if (Vector2.Distance(pointerData.pressPosition, currentPos) > 15f)
                                {
                                    isDragging = true;
                                    ExecuteEvents.Execute(draggedUI, pointerData, ExecuteEvents.beginDragHandler);

                                    if (pressedUI != null && pressedUI != draggedUI)
                                    {
                                        ExecuteEvents.Execute(pressedUI, pointerData, ExecuteEvents.pointerUpHandler);
                                        pointerData.pointerPress = null;
                                    }
                                }
                            }

                            if (isDragging)
                                ExecuteEvents.Execute(draggedUI, pointerData, ExecuteEvents.dragHandler);
                            break;
                        }
                    case false when lastTriggerClick:
                        {
                            if (pressedUI != null && !isDragging)
                            {
                                ExecuteEvents.Execute(pressedUI, pointerData, ExecuteEvents.pointerUpHandler);
                                ExecuteEvents.Execute(pressedUI, pointerData, ExecuteEvents.pointerClickHandler);
                            }
                            else if (pressedUI != null)
                                ExecuteEvents.Execute(pressedUI, pointerData, ExecuteEvents.pointerUpHandler);

                            if (isDragging && draggedUI != null)
                                ExecuteEvents.Execute(draggedUI, pointerData, ExecuteEvents.endDragHandler);

                            pressedUI = null;
                            draggedUI = null;
                            pointerData.pointerDrag = null;
                            pointerData.pointerPress = null;
                            isDragging = false;
                            break;
                        }
                }

                lastTriggerClick = trigger;
            }
        }

        public static GameObject selectObject;
        public static VRRig lastTarget;
        public static bool lastTriggerSelect;
        public static void PlayerSelect()
        {
            bool leftHand = rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton);

            var (_, _, _, forward, _) = leftHand ? ControllerUtilities.GetTrueLeftHand() : ControllerUtilities.GetTrueRightHand();
            bool canSelect = NetworkSystem.Instance.InRoom && menu != null && reference != null && Vector3.Distance(menu.transform.position, reference.transform.position) > 0.5f;

            if (canSelect)
            {
                if (selectObject == null)
                    selectObject = new GameObject("iiMenu_PingLine");

                Color targetColor = Buttons.GetIndex("Swap GUI Colors").enabled ? buttonColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor();
                Color lineColor = targetColor;
                lineColor.a = 0.15f;

                LineRenderer pingLine = selectObject.GetOrAddComponent<LineRenderer>();
                pingLine.material.shader = Shader.Find("GUI/Text Shader");
                pingLine.startColor = lineColor;
                pingLine.endColor = lineColor;
                pingLine.startWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                pingLine.endWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                pingLine.positionCount = 2;
                pingLine.useWorldSpace = true;
                if (smoothLines)
                {
                    pingLine.numCapVertices = 10;
                    pingLine.numCornerVertices = 5;
                }

                Vector3 StartPosition = leftHand ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position;
                Vector3 Direction = forward;

                Physics.SphereCast(StartPosition + Direction / 4f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f), 0.15f, Direction, out var Ray, 512f, NoInvisLayerMask());
                Vector3 EndPosition = Ray.point == Vector3.zero ? StartPosition + (Direction * 512f) : Ray.point;

                pingLine.SetPosition(0, StartPosition);
                pingLine.SetPosition(1, EndPosition);

                VRRig rigTarget = Ray.collider.GetComponentInParent<VRRig>();
                if (Ray.collider != null && rigTarget != null && !rigTarget.IsLocal())
                {
                    if (lastTarget != null && lastTarget != rigTarget)
                    {
                        lastTarget.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (lastTarget.mainSkin.material.name.Contains("gorilla_body"))
                            lastTarget.mainSkin.material.color = lastTarget.playerColor;

                        lastTarget = null;
                    }

                    if (lastTarget == null)
                    {
                        Visuals.FixRigMaterialESPColors(rigTarget);

                        rigTarget.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        rigTarget.mainSkin.material.color = targetColor;

                        GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.05f);

                        lastTarget = rigTarget;
                    } else
                        lastTarget.mainSkin.material.color = targetColor;

                    bool trigger = leftHand ? leftTrigger > 0.5f : rightTrigger > 0.5f;

                    if (trigger && !lastTriggerSelect)
                    {
                        VRRig.LocalRig.PlayHandTapLocal(50, leftHand, 0.4f);
                        GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);

                        NavigatePlayer(GetPlayerFromVRRig(rigTarget));
                        ReloadMenu();

                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Selected player {GetPlayerFromVRRig(rigTarget).NickName}.");
                    }

                    lastTriggerSelect = trigger;
                } else
                {
                    if (lastTarget != null)
                    {
                        lastTarget.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (lastTarget.mainSkin.material.name.Contains("gorilla_body"))
                            lastTarget.mainSkin.material.color = lastTarget.playerColor;

                        lastTarget = null;
                    }
                }
            } else
            {
                if (selectObject != null)
                {
                    Object.Destroy(selectObject);
                    selectObject = null;
                }

                if (lastTarget != null)
                {
                    lastTarget.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (lastTarget.mainSkin.material.name.Contains("gorilla_body"))
                        lastTarget.mainSkin.material.color = lastTarget.playerColor;

                    lastTarget = null;
                }

                lastTriggerSelect = false;
            }
        }

        public static IEnumerator MenuIntroCoroutine()
        {
            if (Time.time < timeMenuStarted)
                yield return new WaitForSeconds(1f);

            float fps = 1f / Time.unscaledDeltaTime;
            yield return new WaitUntil(() => { fps = Mathf.Lerp(fps, 1f / Time.unscaledDeltaTime, 0.1f); return fps > 30f; });

            GameObject menuIntro = LoadObject<GameObject>("Intro");

            menuIntro.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
            menuIntro.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

            VideoPlayer videoPlayer = menuIntro.transform.Find("Video").GetComponent<VideoPlayer>();
            ParticleSystem particleSystem = menuIntro.transform.Find("Particles").GetComponent<ParticleSystem>();

            Color backgroundColor = Color.white;
            Fun.HueShift(Color.white);

            var main = particleSystem.main; // ????
            main.startColor = new ParticleSystem.MinMaxGradient(
                Main.backgroundColor.GetColor(0)
            );

            void EndImmediately()
            {
                Fun.HueShift(Color.clear);
                Object.Destroy(menuIntro);
            }

            float timeout = 0f;

            while (!videoPlayer.isPrepared)
            {
                timeout += Time.deltaTime;
                if (timeout > 5f)
                {
                    EndImmediately();
                    yield break;
                }
                yield return null;
            }

            bool videoEnded = false;
            videoPlayer.Play();
            videoPlayer.loopPointReached += (_) => videoEnded = true;

            yield return new WaitUntil(() => videoEnded);

            float fadeEnd = Time.time + 1f;
            Color transparentColor = backgroundColor;
            transparentColor.a = 0f;

            while (Time.time < fadeEnd)
            {
                float t = 1f - (fadeEnd - Time.time);
                Fun.HueShift(Color.Lerp(backgroundColor, transparentColor, t));
                videoPlayer.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.clear, t);
                main.startColor = new ParticleSystem.MinMaxGradient(
                    Color.Lerp(main.startColor.color, Color.clear, t)
                );

                yield return null;
            }

            EndImmediately();
        }

        public static void MenuIntro() =>
            CoroutineManager.instance.StartCoroutine(MenuIntroCoroutine());

        public static void ResetVoiceCommandsKeywords()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt"))
                File.WriteAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt", keyWords);
        }

        public static void ResetSystemPrompt()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_SystemPrompt.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_SystemPrompt.txt", AIManager.SystemPrompt);
        }

        public static string SavePreferencesToText()
        {
            string seperator = ";;";

            string enabledtext = "";
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (!v.detected && v.enabled && v.buttonText != "Save Preferences")
                    {
                        if (enabledtext == "")
                            enabledtext += v.buttonText;
                        else
                            enabledtext += seperator + v.buttonText;
                    }
                }
            }

            string favoritetext = "";
            foreach (string fav in favorites)
            {
                if (favoritetext == "")
                    favoritetext += fav;
                else
                    favoritetext += seperator + fav;
            }

            string[] settings = {
                Movement.platformMode.ToString(),
                Movement.platformShape.ToString(),
                Movement.flySpeedCycle.ToString(),
                Movement.longarmCycle.ToString(),
                Movement.speedboostCycle.ToString(),
                Projectiles.projMode.ToString(),
                Movement.timerPowerIndex.ToString(),
                Projectiles.shootCycle.ToString(),
                pointerIndex.ToString(),
                Advantages.tagAuraIndex.ToString(),
                notificationDecayTime.ToString(),
                fontStyleType.ToString(),
                arrowType.ToString(),
                pcbg.ToString(),
                Important.reconnectDelay.ToString(),
                Safety.fpsSpoofValue.ToString(),
                buttonClickIndex.ToString(),
                buttonClickVolume.ToString(),
                Safety.antiReportRangeIndex.ToString(),
                Advantages.tagRangeIndex.ToString(),
                Sound.BindMode.ToString(),
                Movement.driveInt.ToString(),
                langInd.ToString(),
                inputTextColorInt.ToString(),
                Movement.pullPowerInt.ToString(),
                notificationSoundIndex.ToString(),
                Visuals.PerformanceModeStepIndex.ToString(),
                gunVariation.ToString(),
                GunDirection.ToString(),
                narratorIndex.ToString(),
                Movement.predInt.ToString(),
                gunLineQualityIndex.ToString(),
                Projectiles.projDebounceIndex.ToString(),
                Projectiles.red.ToString(),
                Projectiles.green.ToString(),
                Projectiles.blue.ToString(),
                Safety.rankIndex.ToString(),
                Overpowered.snowballScale.ToString(),
                Overpowered.lagIndex.ToString(),
                Fun.blockDebounceIndex.ToString(),
                Fun.nameCycleIndex.ToString(),
                menuScaleIndex.ToString(),
                Sound.soundId.ToString(),
                Fun.targetQuestScore.ToString(),
                notificationScaleIndex.ToString(),
                overlayScaleIndex.ToString(),
                arraylistScaleIndex.ToString(),
                ((int)MathF.Ceiling(playTime)).ToString(),
                PhotonNetwork.LocalPlayer?.UserId ?? "null",
                _pageSize.ToString(),
                Overpowered.snowballMultiplicationFactor.ToString(),
                menuButtonIndex.ToString(),
                Safety.targetElo.ToString(),
                Safety.targetBadge.ToString(),
                Movement.playspaceAbuseIndex.ToString(),
                Movement.wallWalkStrengthIndex.ToString(),
                Fun.headSpinIndex.ToString(),
                Movement.macroPlaybackRangeIndex.ToString(),
                joystickMenuPosition.ToString(),
                Movement.multiplicationAmount.ToString(),
                Fun.targetFOV.ToString(),
                Projectiles.targetProjectileIndex.ToString(),
                Movement.fakeLagDelayIndex.ToString(),
                Projectiles.snowballIndex.ToString(),
                characterDistance.ToString(),
                Overpowered.lagTypeIndex.ToString(),
                Overpowered.masterVisualizationType.ToString(),
                Movement.targetHz.ToString(),
                Safety.pingSpoofValue.ToString()
            };

            string settingstext = string.Join(seperator, settings);

            string bindingtext = "";
            foreach (KeyValuePair<string, List<string>> Bind in ModBindings)
            {
                if (bindingtext != "")
                    bindingtext += "~~";

                string toAppend = Bind.Key;
                foreach (string modName in Bind.Value)
                    toAppend += seperator + modName;

                bindingtext += toAppend;
            }

            string quickActionString = string.Join(seperator, quickActions);
            
            string rebindingtext = "";
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.rebindKey != null)
                    {
                        if (rebindingtext == "")
                            rebindingtext += v.buttonText + ";" + v.rebindKey;
                        else
                            rebindingtext += seperator + v.buttonText + ";" + v.rebindKey;
                    }
                }
            }

            string skipButtonString = string.Join(seperator, skipButtons);

            string finaltext =
                enabledtext + "\n" +
                favoritetext + "\n" +
                settingstext + "\n" +
                pageButtonType + "\n" +
                themeType + "\n" +
                fontCycle + "\n" +
                bindingtext + "\n" +
                quickActionString + "\n" +
                rebindingtext + "\n" +
                skipButtonString;

            return finaltext;
        }

        public static void SavePreferences() =>
            File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt", SavePreferencesToText());

        public static int loadingPreferencesFrame;
        public static void LoadPreferencesFromText(string text)
        {
            loadingPreferencesFrame = Time.frameCount;

            Panic();
            string[] textData = text.Split("\n");

            string[] activebuttons = textData[0].Split(";;");
            for (int index = 0; index < activebuttons.Length; index++)
                Toggle(activebuttons[index]);

            string[] favoritesarray = textData[1].Split(";;");
            favorites.Clear();
            foreach (string favorite in favoritesarray)
                favorites.Add(favorite);

            try
            {
                string[] data = textData[2].Split(";;");
                Movement.platformMode = int.Parse(data[0]) - 1;
                Movement.ChangePlatformType();

                Movement.platformShape = int.Parse(data[1]) - 1;
                Movement.ChangePlatformShape();

                Movement.flySpeedCycle = int.Parse(data[2]) - 1;
                Movement.ChangeFlySpeed();

                Movement.longarmCycle = int.Parse(data[3]) - 1;
                Movement.ChangeArmLength();

                Movement.speedboostCycle = int.Parse(data[4]) - 1;
                Movement.ChangeSpeedBoostAmount();

                Projectiles.projMode = int.Parse(data[5]) - 1;
                Projectiles.ChangeProjectile();

                Movement.timerPowerIndex = int.Parse(data[6]) - 1;
                Movement.ChangeTimerSpeed();

                Projectiles.shootCycle = int.Parse(data[7]) - 1;
                Projectiles.ChangeShootSpeed();

                pointerIndex = int.Parse(data[8]) - 1;
                ChangePointerPosition();

                Advantages.tagAuraIndex = int.Parse(data[9]) - 1;
                Advantages.ChangeTagAuraRange();

                notificationDecayTime = int.Parse(data[10]) - 1000;
                ChangeNotificationTime();

                fontStyleType = int.Parse(data[11]) - 1;
                ChangeFontStyleType();

                arrowType = int.Parse(data[12]) - 1;
                ChangeArrowType();

                pcbg = int.Parse(data[13]) - 1;
                ChangePCUI();

                Important.reconnectDelay = int.Parse(data[14]) - 1;
                ChangeReconnectTime();

                Safety.fpsSpoofValue = string.IsNullOrWhiteSpace(data[15]) ? 85 : int.Parse(data[15]) - 5;
                Safety.ChangeFPSSpoofValue();

                buttonClickIndex = int.Parse(data[16]) - 1;
                ChangeButtonSound();

                buttonClickVolume = int.Parse(data[17]) - 1;
                ChangeButtonVolume();

                Safety.antiReportRangeIndex = int.Parse(data[18]) - 1;
                Safety.ChangeAntiReportRange();

                Advantages.tagRangeIndex = int.Parse(data[19]) - 1;
                Advantages.ChangeTagReachDistance();

                Sound.BindMode = int.Parse(data[20]) - 1;
                Sound.SoundBindings();

                Movement.driveInt = int.Parse(data[21]) - 1;
                Movement.ChangeDriveSpeed();

                langInd = int.Parse(data[22]) - 1;
                ChangeMenuLanguage();

                inputTextColorInt = int.Parse(data[23]) - 1;
                ChangeInputTextColor();

                Movement.pullPowerInt = int.Parse(data[24]) - 1;
                Movement.ChangePullModPower();

                notificationSoundIndex = int.Parse(data[25]) - 1;
                ChangeNotificationSound();

                Visuals.PerformanceModeStepIndex = int.Parse(data[26]) - 1;
                Visuals.ChangePerformanceModeVisualStep();

                gunVariation = int.Parse(data[27]) - 1;
                ChangeGunVariation();

                GunDirection = int.Parse(data[28]) - 1;
                ChangeGunDirection();

                narratorIndex = int.Parse(data[29]) - 1;
                ChangeNarrationVoice();

                Movement.predInt = int.Parse(data[30]) - 1;
                Movement.ChangePredictionAmount();

                gunLineQualityIndex = int.Parse(data[31]) - 1;
                ChangeGunLineQuality();

                Projectiles.projDebounceIndex = int.Parse(data[32]) - 1;
                Projectiles.ChangeProjectileDelay();

                Projectiles.red = int.Parse(data[33]) - 1;
                Projectiles.IncreaseRed();

                Projectiles.green = int.Parse(data[34]) - 1;
                Projectiles.IncreaseGreen();

                Projectiles.blue = int.Parse(data[35]) - 1;
                Projectiles.IncreaseBlue();

                Safety.rankIndex = int.Parse(data[36]) - 1;
                Safety.ChangeRankedTier();

                Overpowered.snowballScale = int.Parse(data[37]) - 1;
                Overpowered.ChangeSnowballScale();

                Overpowered.lagIndex = int.Parse(data[38]) - 1;
                Overpowered.ChangeLagPower();

                Fun.blockDebounceIndex = int.Parse(data[39]) - 1;
                Fun.ChangeBlockDelay();

                Fun.cycleSpeedIndex = int.Parse(data[40]) - 1;
                Fun.ChangeCycleDelay();

                menuScaleIndex = int.Parse(data[41]) - 1;
                ChangeMenuScale();

                Sound.soundId = int.Parse(data[42]) - 1;
                Sound.IncreaseSoundID();

                Fun.targetQuestScore = int.Parse(data[43]) - 1;
                Fun.ChangeCustomQuestScore();

                notificationScaleIndex = int.Parse(data[44]) - 1;
                ChangeNotificationScale();

                overlayScaleIndex = int.Parse(data[45]) - 1;
                ChangeOverlayScale();

                arraylistScaleIndex = int.Parse(data[46]) - 1;
                ChangeArraylistScale();

                playTime = int.Parse(data[47]);

                Important.oldId = data[48];

                _pageSize = int.Parse(data[49]) - 1;
                ChangePageSize();

                Overpowered.snowballMultiplicationFactor = int.Parse(data[50]) - 1;
                Overpowered.ChangeSnowballMultiplicationFactor();

                menuButtonIndex = int.Parse(data[51]) - 1;
                ChangeMenuButton();

                Safety.targetElo = int.Parse(data[52]) - 100;
                Safety.ChangeELOValue();

                Safety.targetBadge = int.Parse(data[53]) - 1;
                Safety.ChangeBadgeTier();

                Movement.playspaceAbuseIndex = int.Parse(data[54]) - 1;
                Movement.ChangePlayspaceAbuseSpeed();

                Movement.wallWalkStrengthIndex = int.Parse(data[55]) - 1;
                Movement.ChangeWallWalkStrength();

                Fun.headSpinIndex = int.Parse(data[56]) - 1;
                Fun.ChangeHeadSpinSpeed();

                Movement.macroPlaybackRangeIndex = int.Parse(data[57]) - 1;
                Movement.ChangeMacroPlaybackRange();

                joystickMenuPosition = int.Parse(data[58]) - 1;
                ChangeJoystickMenuPosition();

                Movement.multiplicationAmount = int.Parse(data[59]) - 1;
                Movement.MultiplicationAmount();

                Fun.targetFOV = int.Parse(data[60]) - 5;
                Fun.ChangeTargetFOV();

                Projectiles.targetProjectileIndex = int.Parse(data[61]) - 1;
                Projectiles.ChangeProjectileIndex();

                Movement.fakeLagDelayIndex = int.Parse(data[62]) - 1;
                Movement.ChangeFakeLagStrength();

                Projectiles.snowballIndex = int.Parse(data[63]) - 1;
                Projectiles.ChangeGrowingProjectile();

                characterDistance = int.Parse(data[64]) - 1;
                ChangeCharacterDistance();

                Overpowered.lagTypeIndex = int.Parse(data[65]) - 1;
                Overpowered.ChangeLagType();

                Overpowered.masterVisualizationType = int.Parse(data[66]) - 1;
                Overpowered.MasterVisualizationType();

                Movement.targetHz = int.Parse(data[67]) - 500;
                Movement.ChangeTinnitusHz();

                Safety.pingSpoofValue = int.Parse(data[68]) - 100;
                Safety.ChangePingSpoofValue();
            }
            catch { LogManager.Log("Save file out of date"); }

            pageButtonType = int.Parse(textData[3]) - 1;
            Toggle("Change Page Type");
            themeType = int.Parse(textData[4]) - 1;
            Toggle("Change Menu Theme");
            fontCycle = int.Parse(textData[5]) - 1;
            Toggle("Change Font Type");

            try
            {
                foreach (string Bindings in textData[6].Split("~~"))
                {
                    if (Bindings.Contains(";;"))
                    {
                        string[] BindData = Bindings.Split(";;");
                        string BindName = BindData[0];

                        List<string> Binds = new List<string>();

                        for (int i = 1; i < BindData.Length; i++)
                        {
                            string ModName = BindData[i];
                            if (Buttons.GetIndex(ModName) != null)
                                Binds.Add(ModName);
                        }

                        ModBindings[BindName] = Binds;
                    }
                }
            } catch { }

            try
            {
                quickActions.Clear();
                foreach (string quickAction in textData[7].Split(";;"))
                {
                    ButtonInfo button = Buttons.GetIndex(quickAction);
                    if (button != null)
                        quickActions.Add(quickAction);
                }
            } catch { }
            
            try
            {
                foreach (string bind in textData[8].Split(";;"))
                {
                    string rebindText = bind.Split(";")[0];
                    string rebindKey = bind.Split(";")[1];
                    ButtonInfo button = Buttons.GetIndex(rebindText);
                    if (button != null)
                        button.rebindKey = rebindKey;
                }
            } catch { }

            try
            {
                skipButtons.Clear();
                foreach (string skipButton in textData[9].Split(";;"))
                {
                    ButtonInfo button = Buttons.GetIndex(skipButton);
                    if (button != null)
                        skipButtons.Add(skipButton);
                }
            } catch { }

            hasLoadedPreferences = true;
        }

        public static void LoadPreferences()
        {
            try
            {
                if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt"))
                {
                    hasLoadedPreferences = true;
                    return;
                }

                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt");
                LoadPreferencesFromText(text);
            } catch (Exception e) { LogManager.Log("Error loading preferences: " + e.Message); }
        }

        public static void Panic()
        {
            AnnoyingModeOff();
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.enabled)
                        Toggle(v.buttonText);
                }
            }
        }

        public enum ControllerBinding
        {
            None,
            LeftTrigger,
            RightTrigger,
            LeftGrip,
            RightGrip,
            LeftPrimaryButton,
            RightPrimaryButton,
            LeftSecondaryButton,
            RightSecondaryButton,
            JoystickClick,
            LeftOverride
        }

        public static readonly Dictionary<ControllerBinding, KeyCode> pcBindings = new Dictionary<ControllerBinding, KeyCode>
        {
            { ControllerBinding.RightPrimaryButton, KeyCode.E },
            { ControllerBinding.RightSecondaryButton, KeyCode.R },
            { ControllerBinding.LeftPrimaryButton, KeyCode.F },
            { ControllerBinding.LeftSecondaryButton, KeyCode.G },
            { ControllerBinding.LeftGrip, KeyCode.LeftBracket },
            { ControllerBinding.RightGrip, KeyCode.RightBracket },
            { ControllerBinding.LeftTrigger, KeyCode.Minus },
            { ControllerBinding.RightTrigger, KeyCode.Equals },
            { ControllerBinding.JoystickClick, KeyCode.Return },
            { ControllerBinding.LeftOverride, KeyCode.LeftAlt }
        };

        public static void LoadPCControls()
        {
            string fileName = $"{PluginInfo.BaseDirectory}/iiMenu_PCControls.txt";

            if (File.Exists(fileName))
            {
                string data = File.ReadAllText(fileName);
                string[] lines = data.Split('\n');
                pcBindings.Clear();

                foreach (string line in lines)
                {
                    string finalLine = line.Trim();

                    if (!finalLine.Contains(" - "))
                        continue;

                    string[] splitData = finalLine.Split(" - ");

                    if (Enum.TryParse(splitData[1], out ControllerBinding binding) && Enum.TryParse(splitData[0], out KeyCode key))
                        pcBindings[binding] = key;
                }
            }
            else
            {
                var lines = new List<string>();

                foreach (var pair in pcBindings)
                    lines.Add($"{pair.Value} - {pair.Key}");

                File.WriteAllLines(fileName, lines);
            }
        }


        public static void ChangeReconnectTime(bool positive = true)
        {
            if (positive)
                Important.reconnectDelay++;
            else
                Important.reconnectDelay--;

            if (Important.reconnectDelay > 5)
                Important.reconnectDelay = 1;
            if (Important.reconnectDelay < 1)
                Important.reconnectDelay = 5;

            Buttons.GetIndex("crTime").overlapText = "Change Reconnect Time <color=grey>[</color><color=green>" + Important.reconnectDelay + "</color><color=grey>]</color>";
        }

        public static void ChangeButtonSound(bool positive = true, bool fromMenu = false)
        {
            int[] sounds = {
                8,
                66,
                67,
                84,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                106,
                189,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                338,
                66,
                66,
                66,
                66,
                66,
                66
            };
            string[] buttonSoundNames = {
                "Wood",
                "Keyboard",
                "Default",
                "Bubble",
                "Steal",
                "Anthrax",
                "Lever",
                "Minecraft",
                "Rec Room",
                "Watch",
                "Membrane",
                "Jar",
                "Wall",
                "Slider",
                "Can",
                "Cut",
                "Creamy",
                "Roblox Button",
                "Roblox Tick",
                "Mouse",
                "Valve",
                "Nintendo",
                "Windows",
                "Destiny",
                "Untitled",
                "Slap",
                "Dog",
                "GMod Spawn",
                "GMod Undo",
                "Half Life",
                "Mine",
                "Sensation"
            };

            if (positive)
                buttonClickIndex++;
            else
                buttonClickIndex--;

            buttonClickIndex %= sounds.Length;
            if (buttonClickIndex < 0)
                buttonClickIndex = sounds.Length - 1;

            buttonClickSound = sounds[buttonClickIndex];
            Buttons.GetIndex("Change Button Sound").overlapText = "Change Button Sound <color=grey>[</color><color=green>" + buttonSoundNames[buttonClickIndex] + "</color><color=grey>]</color>";

            if (fromMenu)
            {
                VRRig.LocalRig.leftHandPlayer.Stop();
                VRRig.LocalRig.rightHandPlayer.Stop();
                PlayButtonSound();
            }
        }

        public static void ChangeButtonVolume(bool positive = true, bool fromMenu = false)
        {
            if (positive)
                buttonClickVolume++;
            else
                buttonClickVolume--;

            buttonClickVolume %= 11;
            if (buttonClickVolume < 0)
                buttonClickVolume = 10;

            Buttons.GetIndex("Change Button Volume").overlapText = "Change Button Volume <color=grey>[</color><color=green>" + buttonClickVolume + "</color><color=grey>]</color>";

            if (fromMenu)
            {
                VRRig.LocalRig.leftHandPlayer.Stop();
                VRRig.LocalRig.rightHandPlayer.Stop();
                PlayButtonSound();
            }
        }
    }
}