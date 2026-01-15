/*
 * ii's Stupid Menu  Menu/Main.cs
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

using BepInEx;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Classes.Menu;
using iiMenu.Classes.Mods;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Mods;
using iiMenu.Patches;
using iiMenu.Patches.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Valve.VR;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.FileUtilities;
using static iiMenu.Utilities.RandomUtilities;
using Button = iiMenu.Classes.Menu.Button;
using CommonUsages = UnityEngine.XR.CommonUsages;
using Console = iiMenu.Classes.Menu.Console;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/*
 * ii's Stupid Menu, written by @goldentrophy
 * Any comments are developer comments I wrote
 * Most comments are used to find certain parts of code faster with Ctrl + F
 * Feel free to read them if you want
 *
 * ii's Stupid Menu falls under the GPL-3.0 license
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * If you want to support my, check out my Patreon: https://patreon.com/iiDk
 * Any support is appreciated, and it helps me make more free content for you all
 */

namespace iiMenu.Menu
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour // Do not get rid of this. I don't know why, the entire class kills itself.
    {
        /// <summary>
        /// Runs on first frame of <see cref="GTPlayer.LateUpdate"/> after menu is launched
        /// </summary>
        ///
        public static void OnLaunch()
        {
            if (CoroutineManager.instance == null)
                LogManager.LogError("CoroutineManager instance is null on menu launch. Features may not function properly.");

            if (NotificationManager.Instance == null)
                LogManager.LogError("CoroutineManager instance is null on menu launch. Features may not function properly.");

            timeMenuStarted = Time.time;
            IsSteam = PlayFabAuthenticator.instance.platform;

            InitializeFonts();
            activeFont = AgencyFB;

            if (Plugin.FirstLaunch)
                Prompt("It seems like this is your first time using the menu. Would you like to watch a quick tutorial to get to know how to use it?", Settings.ShowTutorial);
            else
                acceptedDonations = File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_HideDonationButton.txt");

            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinRoom;
            NetworkSystem.Instance.OnReturnedToSinglePlayer += OnLeaveRoom;
            NetworkSystem.Instance.OnMasterClientSwitchedEvent += OnMasterClientSwitch;

            NetworkSystem.Instance.OnPlayerJoined += OnPlayerJoin;
            NetworkSystem.Instance.OnPlayerLeft += OnPlayerLeave;

            SerializePatch.OnSerialize += OnSerialize;
            PlayerSerializePatch.OnPlayerSerialize += OnPlayerSerialize;

            CrystalMaterial = GetObject("Environment Objects/LocalObjects_Prefab/ForestToCave/C_Crystal_Chunk")?.GetComponent<Renderer>()?.material;
            TryOnRoom = GetObject("NetworkTriggers/Cosmetics Room Triggers/TryOnRoom");

            fullModAmount ??= Buttons.buttons.SelectMany(list => list).ToArray().Length;

            GameObject ConsoleObject = Console.LoadConsoleImmediately();

            if (ServerData.ServerDataEnabled)
            {
                ConsoleObject.AddComponent<FriendManager>();
                ConsoleObject.AddComponent<PatreonManager>();
            }

            try
            {
                string allButtonsPath = $"{PluginInfo.BaseDirectory}/AllButtons.txt";

                string[] newButtonNames = Buttons.buttons
                        .SelectMany(list => list)
                        .Select(button => button.buttonText)
                        .ToArray();

                if (File.Exists(allButtonsPath))
                {
                    string[] oldButtonNames = File.ReadAllText(allButtonsPath).Split("\n");

                    foreach (string name in newButtonNames)
                    {
                        if (!oldButtonNames.Contains(name))
                        {
                            ButtonInfo button = Buttons.GetIndex(name);
                            string buttonText = button.overlapText ?? button.buttonText;
                            button.overlapText ??= buttonText + " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
                        }
                    }
                }

                File.WriteAllText(allButtonsPath, string.Join("\n", newButtonNames));
            }
            catch { }

            try
            {
                PluginManager.LoadPlugins();
            }
            catch (Exception exc)
            {
                LogManager.LogError(
                $"Error with Settings.LoadPlugins() at {exc.StackTrace}: {exc.Message}");
            }

            try
            {
                Sound.LoadSoundboard(false);
            }
            catch (Exception exc)
            {
                LogManager.LogError(
                $"Error with Sound.LoadSoundboard() at {exc.StackTrace}: {exc.Message}");
            }

            try
            {
                Movement.LoadMacros();
            }
            catch (Exception exc)
            {
                LogManager.LogError(
                $"Error with Movement.LoadMacros() at {exc.StackTrace}: {exc.Message}");
            }

            loadPreferencesTime = Time.time;
            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt"))
            {
                try
                {
                    Settings.LoadPreferences();
                }
                catch
                {
                    CoroutineManager.instance.StartCoroutine(DelayLoadPreferences());
                }
            }

            if (new DirectoryInfo(Path.Combine(GetGamePath(), PluginInfo.ClientResourcePath)).CreationTime <= DateTime.Now.AddYears(-1))
                AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                {
                    name = "Veteran",
                    description = "Use the menu for over a year.",
                    icon = "Images/Achievements/veteran.png"
                });


            if (PatchHandler.PatchErrors > 0)
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> {PatchHandler.PatchErrors} patch{(PatchHandler.PatchErrors > 1 ? "es" : "")} failed to initialize. Please report this as an issue to the GitHub repository.", 10000);
        }

        public static void Prefix()
        {
            #region Controls
            try
            {
                rightPrimary = ControllerInputPoller.instance.rightControllerPrimaryButton || UnityInput.Current.GetKey(KeyCode.E);
                rightSecondary = ControllerInputPoller.instance.rightControllerSecondaryButton || UnityInput.Current.GetKey(KeyCode.R);
                leftPrimary = ControllerInputPoller.instance.leftControllerPrimaryButton || UnityInput.Current.GetKey(KeyCode.F);
                leftSecondary = ControllerInputPoller.instance.leftControllerSecondaryButton || UnityInput.Current.GetKey(KeyCode.G);
                leftGrab = ControllerInputPoller.instance.leftGrab || UnityInput.Current.GetKey(KeyCode.LeftBracket);
                rightGrab = ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKey(KeyCode.RightBracket);
                leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
                rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);

                if (UnityInput.Current.GetKey(KeyCode.Minus))
                    leftTrigger = 1f;

                if (UnityInput.Current.GetKey(KeyCode.Equals))
                    rightTrigger = 1f;

                if (IsSteam)
                {
                    leftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
                    rightJoystick = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);

                    leftJoystickClick = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
                    rightJoystickClick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
                }
                else
                {
                    ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftJoystick);
                    ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightJoystick);

                    ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out leftJoystickClick);
                    ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out rightJoystickClick);
                }

                bool arrowKeysPressed = UnityInput.Current.GetKey(KeyCode.UpArrow) || UnityInput.Current.GetKey(KeyCode.DownArrow) || UnityInput.Current.GetKey(KeyCode.LeftArrow) || UnityInput.Current.GetKey(KeyCode.RightArrow);
                if (arrowKeysPressed)
                {
                    Vector2 direction = new Vector2((UnityInput.Current.GetKey(KeyCode.RightArrow) ? 1f : 0f) + (UnityInput.Current.GetKey(KeyCode.LeftArrow) ? -1f : 0f), (UnityInput.Current.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (UnityInput.Current.GetKey(KeyCode.DownArrow) ? -1f : 0f));
                    if (UnityInput.Current.GetKey(KeyCode.LeftAlt))
                        rightJoystick = direction;
                    else
                        leftJoystick = direction;
                }

                if (UnityInput.Current.GetKey(KeyCode.Return))
                {
                    if (UnityInput.Current.GetKey(KeyCode.LeftAlt))
                        rightJoystickClick = true;
                    else
                        leftJoystickClick = true;
                }

                if (adaptiveButtons)
                {
                    switch (ControllerUtilities.GetLeftControllerType())
                    {
                        case ControllerUtilities.ControllerType.ValveIndex:
                            leftGrab = ControllerInputPoller.instance.leftControllerGripFloat > 0.75f;
                            break;
                        case ControllerUtilities.ControllerType.VIVE:
                            leftPrimary = leftJoystickClick;
                            break;
                    }

                    switch (ControllerUtilities.GetRightControllerType())
                    {
                        case ControllerUtilities.ControllerType.ValveIndex:
                            rightGrab = ControllerInputPoller.instance.rightControllerGripFloat > 0.75f;
                            break;
                        case ControllerUtilities.ControllerType.VIVE:
                            rightPrimary = rightJoystickClick;
                            break;
                    }
                }

                shouldBePC = UnityInput.Current.GetKey(KeyCode.E)
                            || UnityInput.Current.GetKey(KeyCode.R)
                            || UnityInput.Current.GetKey(KeyCode.F)
                            || UnityInput.Current.GetKey(KeyCode.G)
                            || UnityInput.Current.GetKey(KeyCode.LeftBracket)
                            || UnityInput.Current.GetKey(KeyCode.RightBracket)
                            || UnityInput.Current.GetKey(KeyCode.Minus)
                            || UnityInput.Current.GetKey(KeyCode.Equals)
                            || Mouse.current.leftButton.isPressed
                            || Mouse.current.rightButton.isPressed
                            || arrowKeysPressed;
            }
            catch { }
            #endregion

            try
            {
                #region First Load Condition
                if (!HasLoaded)
                {
                    HasLoaded = true;
                    OnLaunch();
                }
                #endregion

                #region Menu Spawn Condition
                Dictionary<int, bool> leftInputs = new Dictionary<int, bool> {
                    { 0, leftPrimary },
                    { 1, leftSecondary },
                    { 2, leftGrab },
                    { 3, leftTrigger > 0.5f },
                    { 4, leftJoystickClick }
                };

                Dictionary<int, bool> rightInputs = new Dictionary<int, bool> {
                    { 0, rightPrimary },
                    { 1, rightSecondary },
                    { 2, rightGrab },
                    { 3, rightTrigger > 0.5f },
                    { 4, rightJoystickClick }
                };

                bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q) || (inTextInput && isKeyboardPc);
                bool buttonCondition = rightHand ? rightInputs[menuButtonIndex] : leftInputs[menuButtonIndex];

                if (oneHand)
                    buttonCondition = rightHand ? leftInputs[menuButtonIndex] : rightInputs[menuButtonIndex];

                if (bothHands)
                {
                    buttonCondition = rightInputs[menuButtonIndex] || leftInputs[menuButtonIndex];
                    if (buttonCondition)
                        openedwithright = rightInputs[menuButtonIndex];
                }

                if (!XRSettings.isDeviceActive)
                    buttonCondition = false;

                if (wristMenu)
                {
                    bool shouldOpen = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 0.1f, ControllerUtilities.GetTrueRightHand().position) < 0.1f;
                    if (rightHand)
                        shouldOpen = Vector3.Distance(ControllerUtilities.GetTrueLeftHand().position, GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 0.1f) < 0.1f;

                    if (shouldOpen && !lastChecker)
                        wristOpen = !wristOpen;

                    lastChecker = shouldOpen;

                    buttonCondition = wristOpen;
                }

                if (joystickMenu)
                {
                    bool shouldOpen = rightJoystickClick;

                    if (shouldOpen && !lastChecker)
                    {
                        joystickOpen = !joystickOpen;
                        joystickDelay = Time.time + 0.2f;
                    }
                    lastChecker = shouldOpen;

                    buttonCondition = joystickOpen;
                } else
                    joystickButtonSelected = 0;

                if (physicalMenu)
                {
                    if (buttonCondition)
                        physicalOpenPosition = Vector3.zero;

                    buttonCondition = true;
                }
                buttonCondition |= isKeyboardCondition;
                buttonCondition |= inTextInput;
                buttonCondition &= !Lockdown;

                if (watchMenu)
                    buttonCondition = isKeyboardCondition;

                if (barkMenu)
                    buttonCondition = isKeyboardCondition || barkMenuOpen;

                isMenuButtonHeld = buttonCondition;
                switch (buttonCondition)
                {
                    case true when menu == null:
                        OpenMenu();
                        break;
                    case false when menu != null:
                        CloseMenu();
                        break;
                }

                if (buttonCondition && menu != null)
                    RecenterMenu();
                #endregion

                #region Get Camera
                try
                {
                    if (TPC == null)
                    {
                        try
                        {
                            TPC = GetObject("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                        }
                        catch
                        {
                            TPC = GetObject("Shoulder Camera").GetComponent<Camera>();
                        }
                    }
                }
                catch { }
                #endregion

                #region Menu Animations
                if (fpsCountAverage)
                {
                    fpsAverageNumber *= 99f;
                    fpsAverageNumber += 1f / Time.unscaledDeltaTime;
                    fpsAverageNumber /= 100f;
                }
                else
                    fpsAverageNumber = 1f / Time.unscaledDeltaTime;
                
                if (Time.time > fpsAvgTime || !fpsCountTimed)
                {
                    lastDeltaTime = Mathf.Ceil(fpsAverageNumber);
                    fpsAvgTime = Time.time + 1f;
                }

                if (fpsCount != null)
                {
                    string textToSet = ftCount ? $"FT: {Mathf.Floor(1f / lastDeltaTime * 10000f) / 10f} ms" : $"FPS: {lastDeltaTime}";
                    if (hidetitle && !noPageNumber) textToSet += "      ";
                    if (disableFpsCounter) textToSet = "";
                    if (hidetitle && !noPageNumber) textToSet += "Page " + (pageNumber + 1);

                    fpsCount.text = FollowMenuSettings(textToSet, false);
                }

                if (potatoTime != null)
                {
                    if (1f / Time.unscaledDeltaTime < 15f)
                    {
                        potatoTime += Time.unscaledDeltaTime;
                        if (potatoTime > 60f)
                        {
                            potatoTime = null;
                            AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                            {
                                name = "Potato",
                                description = "Have 15 FPS for over a minute.",
                                icon = "Images/Achievements/potato.png"
                            });
                        }
                    }
                    else
                        potatoTime = 0f;
                }

                if (adminTime != null && PhotonNetwork.InRoom)
                {
                    if (PhotonNetwork.PlayerListOthers.Any(player => ServerData.Administrators.ContainsKey(player.UserId) && !Console.excludedCones.Contains(player)))
                    {
                        adminTime += Time.unscaledDeltaTime;
                        if (adminTime > 10f)
                        {
                            adminTime = null;
                            AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                            {
                                name = "EEEEKK!",
                                description = "Be in the same room as a Console administrator.",
                                icon = "Images/Achievements/eeeekk.png"
                            });
                        }
                    }
                    else
                        adminTime = 0f;
                }

                if (watermarkImage != null)
                    watermarkImage.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0f, 90f, 90f - (rockWatermark ? (Mathf.Sin(Time.time * 2f) * 10f) : 0f)));

                if (animatedTitle && title != null)
                {
                    string targetString = doCustomName ? NoRichtextTags(customMenuName) : "ii's Stupid Menu";
                    int length = (int)Mathf.PingPong(Time.time / 0.25f, targetString.Length + 1);
                    title.text = length > 0 ? targetString[..length] : "";
                }

                if (gradientTitle && title != null)
                    title.text = RichtextGradient(NoRichtextTags(title.text),
                        new[]
                        {
                            new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 0f),
                            new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0), 0.95f), 0.5f),
                            new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 1f)
                        });

                if (keyboardInputObject != null)
                    keyboardInputObject.text = FollowMenuSettings(keyboardInput, false) + (Time.frameCount / 45 % 2 == 0 ? "|" : " ");
                #endregion

                #region Menu Features
                // Fix for disorganized menu
                if (disorganized && currentCategoryName != "Main")
                {
                    currentCategoryName = "Main";
                    ReloadMenu();
                }

                // Fix for long menu
                if (longmenu && pageNumber != 0)
                {
                    pageNumber = 0;
                    ReloadMenu();
                }

                // Remove physical menu reference if too far away
                if (physicalMenu && menu != null)
                {
                    try
                    {
                        if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, menu.transform.position) < 1.5f)
                        {
                            if (reference == null)
                                CreateReference();
                        }
                        else
                        {
                            if (reference != null)
                            {
                                Destroy(reference);
                                reference = null;
                            }
                        }
                    }
                    catch { }
                }

                // Bad apple theme
                if (themeType == 63)
                {
                    if (menu != null)
                    {
                        badAppleTime += Time.deltaTime;
                        badAppleTime %= 203f;
                    }

                    if (videoPlayer != null)
                    {
                        if (videoPlayer.isPlaying && menu == null)
                            videoPlayer.Stop();

                        if (!videoPlayer.isPlaying && menu != null)
                            videoPlayer.Play();
                    }
                }
                else
                {
                    if (videoPlayer != null)
                    {
                        Destroy(videoPlayer.gameObject);
                        videoPlayer = null;
                    }
                }

                // Gun sounds
                try
                {
                    if (GunSounds)
                    {
                        if (GunSpawned)
                        {
                            if (!lastGunSpawned)
                            {
                                AudioSource audioSource = SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer;
                                audioSource.volume = buttonClickVolume / 10f;
                                audioSource.PlayOneShot(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Guns/grip-press.ogg", "Audio/Guns/grip-press.ogg"));
                            }

                            if (GetGunInput(true) && (!lastGunTrigger || (handAudioManager != null && !handAudioManager.GetComponent<AudioSource>().isPlaying)))
                            {
                                AudioSource audioSource = SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer;
                                audioSource.volume = buttonClickVolume / 10f;
                                audioSource.PlayOneShot(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Guns/trigger-press.ogg", "Audio/Guns/trigger-press.ogg"));

                                PlayHandAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Guns/trigger-hold.ogg", "Audio/Guns/trigger-hold.ogg"), buttonClickVolume / 10f, SwapGunHand);
                            }

                            if (!GetGunInput(true) && lastGunTrigger)
                            {
                                AudioSource audioSource = SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer;
                                audioSource.volume = buttonClickVolume / 10f;
                                audioSource.PlayOneShot(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Guns/trigger-release.ogg", "Audio/Guns/trigger-release.ogg"));

                                handAudioManager?.GetComponent<AudioSource>().Stop();
                            }
                        }
                        else
                        {
                            if (lastGunSpawned)
                            {
                                AudioSource audioSource = SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer;
                                audioSource.volume = buttonClickVolume / 10f;
                                audioSource.PlayOneShot(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Guns/grip-release.ogg", "Audio/Guns/grip-release.ogg"));
                            }

                            if (handAudioManager != null && handAudioManager.GetComponent<AudioSource>().isPlaying)
                            {
                                handAudioManager.GetComponent<AudioSource>().Stop();

                                AudioSource audioSource = SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer;
                                audioSource.volume = buttonClickVolume / 10f;
                                audioSource.PlayOneShot(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Guns/trigger-release.ogg", "Audio/Guns/trigger-release.ogg"));
                            }
                        }

                        lastGunSpawned = GunSpawned;
                        lastGunTrigger = GetGunInput(true);
                    }
                }
                catch { }

                // Gun Vibrations
                try
                {
                    if (GunVibrations)
                    {
                        if (GunSpawned)
                        {
                            if (!lastGunSpawnedVibration)
                                GorillaTagger.Instance.StartVibration(SwapGunHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.05f);

                            if (GetGunInput(true))
                                GorillaTagger.Instance.StartVibration(SwapGunHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                        }
                        else
                        {
                            if (lastGunSpawnedVibration)
                                GorillaTagger.Instance.StartVibration(SwapGunHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.015f);
                        }

                        lastGunSpawnedVibration = GunSpawned;
                    }
                }
                catch { }

                GunSpawned = false;

                UpdateKeyboard();

                if (annoyingMode)
                {
                    CustomBoardManager.BoardMaterial.color = new Color32(226, 74, 44, 255);
                    int randomChance = Random.Range(1, 400);
                    if (randomChance == 21)
                    {
                        VRRig.LocalRig.PlayHandTapLocal(84, true, 0.4f);
                        VRRig.LocalRig.PlayHandTapLocal(84, false, 0.4f);
                        NotificationManager.SendNotification("<color=grey>[</color><color=#FF00FF>FUN FACT</color><color=grey>]</color> " + facts[Random.Range(0, facts.Length - 1)] + "");
                    }
                }

                // Party kick code (to return back to the main lobby when you're done)
                if (PhotonNetwork.InRoom)
                {
                    if (partyKickReconnecting)
                    {
                        partyLastCode = null;
                        partyKickReconnecting = false;
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully " + (waitForPlayerJoin ? "banned" : "kicked") + " " + amountPartying + " party member.");
                        FriendshipGroupDetection.Instance.LeaveParty();
                    }
                    else
                    {
                        if (partyLastCode != null && Time.time > partyTime && (!waitForPlayerJoin || PhotonNetwork.PlayerListOthers.Length > 0))
                        {
                            if (Buttons.GetIndex("Rejoin on Kick").enabled)
                            {
                                LogManager.Log("Attempting rejoin");
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                                partyKickReconnecting = true;
                            } else
                            {
                                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully " + (waitForPlayerJoin ? "banned" : "kicked") + " " + amountPartying + " party member.");
                                partyKickReconnecting = false;
                                partyLastCode = null;
                            }
                        }
                    }
                }
                else
                {
                    if (partyKickReconnecting)
                    {
                        if (partyLastCode != null && Time.time > partyTime && (!waitForPlayerJoin || PhotonNetwork.PlayerListOthers.Length > 0))
                        {
                            LogManager.Log("Attempting rejoin");
                            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(partyLastCode, JoinType.Solo);
                            partyTime = Time.time + Important.reconnectDelay;
                        }
                    }
                }

                // Recover from playing sound on soundboard code
                try
                {
                    if (Sound.AudioIsPlaying)
                    {
                        if (Time.time > Sound.RecoverTime)
                            Sound.FixMicrophone();
                    }
                }
                catch { }

                if (CurrentPrompt != null && CurrentPrompt.IsText && !inTextInput)
                    Settings.SpawnKeyboard();
                #endregion

                #region Menu Navigation Features
                if (menu != null)
                {
                    if (pageButtonType == 3 || pageButtonType == 4)
                    {
                        bool previousButton = pageButtonType == 3 ? leftGrab : leftTrigger > 0.5f;
                        bool nextButton = pageButtonType == 3 ? rightGrab : rightTrigger > 0.5f;

                        if (Time.time > pageButtonChangeDelay)
                        {
                            if (previousButton)
                            {
                                pageButtonChangeDelay = Time.time + 0.2f;
                                PlayButtonSound("PreviousPage", true, true);
                                Toggle("PreviousPage");
                            }

                            if (nextButton)
                            {
                                pageButtonChangeDelay = Time.time + 0.2f;
                                PlayButtonSound("NextPage", true);
                                Toggle("NextPage");
                            }
                        }

                        if (!previousButton && !nextButton)
                            pageButtonChangeDelay = -1f;
                    }
                }

                if (joystickMenu && joystickOpen)
                {
                    Vector2 js = leftJoystick;
                    if (Time.time > joystickDelay)
                    {
                        int lastButton = PageSize;

                        if (joystickMenuSearching)
                            lastButton++;

                        if (js.x > 0.5f)
                        {
                            if (dynamicSounds)
                                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/next.ogg", "Audio/Menu/next.ogg"), buttonClickVolume / 10f);

                            Toggle("NextPage");
                            joystickDelay = Time.time + 0.2f;
                        }
                        if (js.x < -0.5f)
                        {
                            if (dynamicSounds)
                                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/prev.ogg", "Audio/Menu/prev.ogg"), buttonClickVolume / 10f);

                            Toggle("PreviousPage");
                            joystickDelay = Time.time + 0.2f;
                        }

                        if (js.y > 0.5f)
                        {
                            if (dynamicSounds)
                                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/open.ogg", "Audio/Menu/up.ogg"), buttonClickVolume / 10f);

                            joystickButtonSelected--;
                            if (joystickButtonSelected < 0)
                                joystickButtonSelected = lastButton - 1;

                            ReloadMenu();
                            joystickDelay = Time.time + 0.2f;
                        }
                        if (js.y < -0.5f)
                        {
                            if (dynamicSounds)
                                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/down.ogg"), buttonClickVolume / 10f);

                            joystickButtonSelected++;
                            joystickButtonSelected %= lastButton;

                            ReloadMenu();
                            joystickDelay = Time.time + 0.2f;
                        }

                        if (leftJoystickClick)
                        {
                            if (dynamicSounds)
                                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/select.ogg", "Audio/Menu/select.ogg"), buttonClickVolume / 10f);

                            ButtonInfo button = Buttons.GetIndex(joystickSelectedButton);
                            if (button.incremental)
                                ToggleIncremental(joystickSelectedButton, leftTrigger < 0.5f);
                            else
                                Toggle(joystickSelectedButton, true);
                            ReloadMenu();
                            joystickDelay = Time.time + 0.2f;
                        }
                    }
                }

                if (barkMenu)
                {
                    if (barkMenuOpen)
                    {
                        if (barkMenuGrabbed == null)
                        {
                            if (menuBackground != null)
                            {
                                bool leftGrabbing = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, menuBackground.transform.position) < 0.4f && leftGrab;
                                bool rightGrabbing = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, menuBackground.transform.position) < 0.4f && rightGrab;

                                if (leftGrabbing || rightGrabbing)
                                {
                                    barkMenuGrabbed = leftGrabbing;
                                    rightHand = rightGrabbing;
                                    
                                    if (reference != null)
                                    {
                                        Destroy(reference);
                                        reference = null;

                                        CreateReference();
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool gripping = barkMenuGrabbed.Value ? leftGrab : rightGrab;

                            if (!gripping)
                            {
                                barkMenuGrabbed = null;
                                barkMenuOpen = false;
                            }
                        }
                    } else
                    {
                        static bool IsBangingPosition(Vector3 position)
                        {
                            Vector3 closestPoint = GorillaTagger.Instance.bodyCollider.ClosestPoint(position);
                            return closestPoint.Distance(position) <= 0.1f;
                        }

                        bool bangingChest = IsBangingPosition(ControllerUtilities.GetTrueLeftHand().position) || IsBangingPosition(ControllerUtilities.GetTrueRightHand().position);

                        if (bangingChest && !previousBarkBangState)
                        {
                            if (Time.time > barkBangDelay)
                                barkBangCount = 0;

                            barkBangDelay = Time.time + 0.5f;
                            barkBangCount++;

                            if (barkBangCount >= 3)
                                barkMenuOpen = true;
                        }

                        previousBarkBangState = bangingChest;
                    }
                }

                if (pageScrolling)
                {
                    bool shouldReload = false;
                    if (pageNumber != 0)
                    {
                        pageNumber = 0;
                        shouldReload = true;
                    }

                    Vector2 js = rightHand ? leftJoystick : rightJoystick;
                    if (Time.time > scrollDelay)
                    {
                        if (js.y > 0.5f)
                        {
                            pageOffset = Mathf.Clamp(pageOffset - 1, 0, DisplayedItemCount - PageSize);

                            shouldReload = true;
                            scrollDelay = Time.time + 0.1f;
                        }

                        if (js.y < -0.5f)
                        {
                            pageOffset = Mathf.Clamp(pageOffset + 1, 0, DisplayedItemCount - PageSize);

                            shouldReload = true;
                            scrollDelay = Time.time + 0.1f;
                        }
                    }

                    if (shouldReload)
                        ReloadMenu();
                }

                try
                {
                    if (watchMenu)
                    {
                        watchShell.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;
                        ButtonInfo[] toSortOf = Buttons.buttons[currentCategoryIndex];

                        if (currentCategoryName == "Favorite Mods")
                            toSortOf = StringsToInfos(favorites.ToArray());

                        if (currentCategoryName == "Enabled Mods")
                        {
                            List<ButtonInfo> enabledMods = new List<ButtonInfo>();
                            int categoryIndex = 0;
                            foreach (ButtonInfo[] buttonList in Buttons.buttons)
                            {
                                enabledMods.AddRange(buttonList.Where(v => v.enabled && (!hideSettings || !Buttons.categoryNames[categoryIndex].Contains("Settings")) && (!hideMacros || !Buttons.categoryNames[categoryIndex].Contains("Macro"))));
                                categoryIndex++;
                            }
                            enabledMods = enabledMods.OrderBy(v => v.overlapText ?? v.buttonText).ToList();
                            enabledMods.Insert(0, Buttons.GetIndex("Exit Enabled Mods"));
                            toSortOf = enabledMods.ToArray();
                        }

                        Text watchTextText = watchText.GetComponent<Text>();

                        watchTextText.text = toSortOf[watchMenuIndex].buttonText;
                        if (toSortOf[watchMenuIndex].overlapText != null)
                            watchTextText.text = toSortOf[watchMenuIndex].overlapText;

                        watchTextText.text += $"\n<color=grey>[{watchMenuIndex + 1}/{toSortOf.Length}]\n{DateTime.Now:hh:mm tt}</color>";
                        watchTextText.color = textColors[0].GetCurrentColor();

                        watchTextText.text = FollowMenuSettings(watchTextText.text, false);

                        if (watchIndicatorMat == null)
                            watchIndicatorMat = new Material(Shader.Find("GorillaTag/UberShader"));

                        watchIndicatorMat.color = toSortOf[watchMenuIndex].enabled ? buttonColors[1].GetCurrentColor() : buttonColors[0].GetCurrentColor();
                        watchEnabledIndicator.GetComponent<Image>().material = watchIndicatorMat;

                        Vector2 js = rightHand ? rightJoystick : leftJoystick;
                        if (Time.time > wristMenuDelay)
                        {
                            if (js.x > 0.5f || (rightHand ? js.y < -0.5f : js.y > 0.5f))
                            {
                                watchMenuIndex++;
                                if (watchMenuIndex > toSortOf.Length - 1)
                                    watchMenuIndex = 0;

                                wristMenuDelay = Time.time + 0.2f;
                            }
                            if (js.x < -0.5f || (rightHand ? js.y > 0.5f : js.y < -0.5f))
                            {
                                watchMenuIndex--;
                                if (watchMenuIndex < 0)
                                    watchMenuIndex = toSortOf.Length - 1;

                                wristMenuDelay = Time.time + 0.2f;
                            }
                            if (rightHand ? rightJoystickClick : leftJoystickClick)
                            {
                                int archive = currentCategoryIndex;
                                Toggle(toSortOf[watchMenuIndex].buttonText, true);
                                if (currentCategoryIndex != archive)
                                    watchMenuIndex = 0;

                                wristMenuDelay = Time.time + 0.2f;
                            }
                        }
                    }
                }
                catch { }
                #endregion

                #region Preferences
                try
                {
                    if (!hasLoadedPreferences && Time.time > loadPreferencesTime + 5f)
                    {
                        loadPreferencesTime = Time.time;

                        try {
                            LogManager.Log("Loading preferences due to load errors");
                            Settings.LoadPreferences();
                        } catch
                        {
                            LogManager.Log("Could not load preferences");
                        }
                    }
                } catch { }

                try
                {
                    if (Time.time > autoSaveDelay && !Lockdown)
                    {
                        autoSaveDelay = Time.time + 60f;
                        Settings.SavePreferences();
                        LogManager.Log("Automatically saved preferences");

                        if (backupPreferences)
                        {
                            if (preferenceBackupCount >= 5)
                            {
                                File.WriteAllText($"{PluginInfo.BaseDirectory}/Backups/{CurrentTimestamp().Replace(":", ".")}.txt", Settings.SavePreferencesToText());
                                preferenceBackupCount = 0;
                            }

                            preferenceBackupCount++;
                        }
                    }
                }
                catch { }
                #endregion

                #region Ghostview
                try
                {
                    if (!legacyGhostview && GhostRig == null)
                    {
                        GameObject ghostRigHolder = new GameObject("ghostRigHolder");
                        ghostRigHolder.SetActive(false);

                        GhostRig = Instantiate(VRRig.LocalRig, GTPlayer.Instance.transform.position, GTPlayer.Instance.transform.rotation, ghostRigHolder.transform);
                        GhostRig.headBodyOffset = Vector3.zero;

                        GhostRig.gameObject.SetActive(false);
                        GhostRig.transform.SetParent(VRRig.LocalRig.transform.parent);

                        Destroy(ghostRigHolder);

                        GhostRig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                        GhostRig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);
                        GhostRig.transform.Find("GorillaPlayerNetworkedRigAnchor/rig/bodySlideAudio").gameObject.SetActive(false);
                        GhostRig.GetComponent<OwnershipGaurd>().enabled = false;

                        Visuals.FixRigMaterialESPColors(GhostRig);

                        GhostRig.transform.position = Vector3.one * float.MaxValue;
                    }

                    if (GhostMaterial == null)
                        GhostMaterial = new Material(Shader.Find("GUI/Text Shader"));

                    if (legacyGhostViewLeft == null)
                    {
                        legacyGhostViewLeft = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(legacyGhostViewLeft.GetComponent<SphereCollider>());

                        legacyGhostViewLeft.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }

                    if (legacyGhostViewRight == null)
                    {
                        legacyGhostViewRight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(legacyGhostViewRight.GetComponent<SphereCollider>());

                        legacyGhostViewRight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }

                    if ((!VRRig.LocalRig.enabled || ghostException) && !disableGhostview)
                    {
                        Color color = Buttons.GetIndex("Swap Ghostview Colors").enabled ? buttonColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor();

                        if (legacyGhostview)
                        {
                            if (GhostRig.gameObject.activeSelf)
                            {
                                GhostRig.gameObject.SetActive(false);
                                GhostRig.transform.position = Vector3.one * float.MaxValue;
                            }

                            legacyGhostViewLeft.SetActive(true);
                            legacyGhostViewLeft.transform.position = ControllerUtilities.GetTrueLeftHand().position;
                            legacyGhostViewLeft.GetComponent<Renderer>().material.color = color;

                            legacyGhostViewRight.SetActive(true);
                            legacyGhostViewRight.transform.position = ControllerUtilities.GetTrueRightHand().position;
                            legacyGhostViewRight.GetComponent<Renderer>().material.color = color;
                        }
                        else
                        {
                            if (legacyGhostViewLeft.activeSelf)
                                legacyGhostViewLeft.SetActive(false);
                            if (legacyGhostViewRight.activeSelf)
                                legacyGhostViewRight.SetActive(false);
                            GhostRig.gameObject.SetActive(true);

                            Color ghm = color;
                            ghm.a = 0.5f;

                            GhostMaterial.color = ghm;
                            GhostRig.mainSkin.material = GhostMaterial;
                        }
                    }
                    else
                    {
                        GhostRig.gameObject.SetActive(false);
                        GhostRig.transform.position = Vector3.one * float.MaxValue;

                        legacyGhostViewLeft.SetActive(false);
                        legacyGhostViewRight.SetActive(false);
                    }
                }
                catch { }
                #endregion

                #region Miscellaneous
                hasRemovedThisFrame = false;
                frameCount++;
                playTime += Time.unscaledDeltaTime;

                if (Settings.TutorialObject != null)
                    Settings.UpdateTutorial();

                ServerPos = ServerPos == Vector3.zero ? ServerSyncPos : Vector3.Lerp(ServerPos, VRRig.LocalRig.SanitizeVector3(ServerSyncPos), VRRig.LocalRig.lerpValueBody * 0.66f);
                ServerLeftHandPos = ServerLeftHandPos == Vector3.zero ? ServerSyncLeftHandPos : Vector3.Lerp(ServerLeftHandPos, VRRig.LocalRig.SanitizeVector3(ServerSyncLeftHandPos), VRRig.LocalRig.lerpValueBody);
                ServerRightHandPos = ServerRightHandPos == Vector3.zero ? ServerSyncRightHandPos : Vector3.Lerp(ServerRightHandPos, VRRig.LocalRig.SanitizeVector3(ServerSyncRightHandPos), VRRig.LocalRig.lerpValueBody);
                #endregion

                #region Mod Bindings
                try
                {
                    // Custom mod binds
                    Dictionary<string, bool> Inputs = new Dictionary<string, bool>
                    {
                        { "A", rightPrimary },
                        { "B", rightSecondary },
                        { "X", leftPrimary },
                        { "Y", leftSecondary },
                        { "LG", leftGrab },
                        { "RG", rightGrab },
                        { "LT", leftTrigger > 0.5f },
                        { "RT", rightTrigger > 0.5f },
                        { "LJ", leftJoystickClick },
                        { "RJ", rightJoystickClick }
                    };

                    foreach (KeyValuePair<string, List<string>> binding in ModBindings)
                    {
                        string bindInput = binding.Key;
                        List<string> boundMods = binding.Value;

                        if (boundMods.Count > 0)
                        {
                            bool bindValue = Inputs[bindInput];
                            foreach (string modName in boundMods)
                            {
                                ButtonInfo buttonInfo = Buttons.GetIndex(modName);
                                if (buttonInfo == null) continue;
                                buttonInfo.customBind = bindInput;

                                if (ToggleBindings || !buttonInfo.isTogglable)
                                {
                                    if (bindValue && !BindStates[bindInput])
                                        Toggle(modName, true, true);
                                }

                                if (ToggleBindings) continue;
                                if ((bindValue && !buttonInfo.enabled) || (!bindValue && buttonInfo.enabled))
                                    Toggle(modName, true, true);
                            }

                            BindStates[bindInput] = bindValue;
                        }
                    }
                } catch { }
                #endregion

                #region Visual Clean Up
                try
                {
                    Visuals.ClearLinePool();
                    Visuals.ClearNameTagPool();

                    if (GunPointer != null)
                    {
                        if (!GunPointer.activeSelf)
                            Destroy(GunPointer);
                        else
                            GunPointer.SetActive(false);
                    }

                    if (GunLine != null)
                    {
                        if (!GunLine.gameObject.activeSelf)
                        {
                            Destroy(GunLine.gameObject);
                            GunLine = null;
                        }
                        else
                            GunLine.gameObject.SetActive(false);
                    }

                    List<(long, float)> toRemoveAura = new List<(long, float)>();
                    foreach (KeyValuePair<(long, float), GameObject> key in Visuals.auraPool)
                    {
                        if (!key.Value.activeSelf)
                        {
                            toRemoveAura.Add(key.Key);
                            Destroy(key.Value);
                        }
                        else
                            key.Value.SetActive(false);
                    }

                    foreach ((long, float) item in toRemoveAura)
                        Visuals.auraPool.Remove(item);

                    List<(Vector3, Quaternion, Vector3)> toRemoveCube = new List<(Vector3, Quaternion, Vector3)>();
                    foreach (KeyValuePair<(Vector3, Quaternion, Vector3), GameObject> key in Visuals.cubePool)
                    {
                        if (!key.Value.activeSelf)
                        {
                            toRemoveCube.Add(key.Key);
                            Destroy(key.Value);
                        }
                        else
                            key.Value.SetActive(false);
                    }

                    foreach ((Vector3, Quaternion, Vector3) item in toRemoveCube)
                        Visuals.cubePool.Remove(item);

                    List<string> toRemoveLabel = new List<string>();
                    foreach (KeyValuePair<string, GameObject> label in Visuals.labelDictionary)
                    {
                        if (!label.Value.activeSelf)
                        {
                            toRemoveLabel.Add(label.Key);
                            Destroy(label.Value);
                        }
                        else
                            label.Value.SetActive(false);
                    }

                    foreach (string item in toRemoveLabel)
                        Visuals.labelDictionary.Remove(item);
                } catch { }
                #endregion

                #region Execute Mods
                // Plugins
                PluginManager.ExecuteUpdate();

                // Menu
                foreach (ButtonInfo button in Buttons.buttons
                    .SelectMany(list => list)
                    .Where(button => button.enabled && (button.method != null || button.postMethod != null)))
                {
                    try
                    {
                        bool _leftPrimary = leftPrimary;
                        bool _leftSecondary = leftSecondary;
                        bool _rightPrimary = rightPrimary;
                        bool _rightSecondary = rightSecondary;
                        bool _leftGrab = leftGrab;
                        bool _rightGrab = rightGrab;
                        float _leftTrigger = leftTrigger;
                        float _rightTrigger = rightTrigger;
                        bool _leftJoystickClick = leftJoystickClick;
                        bool _rightJoystickClick = rightJoystickClick;

                        if (OverwriteKeybinds && button.customBind != null)
                        {
                            leftPrimary = true;
                            leftSecondary = true;
                            rightPrimary = true;
                            rightSecondary = true;
                            leftGrab = true;
                            rightGrab = true;
                            leftTrigger = 1f;
                            rightTrigger = 1f;
                            leftJoystickClick = true;
                            rightJoystickClick = true;
                        }

                        try
                        {
                            if (button.rebindKey != null)
                            {
                                float buttonAmount = 0f;
                                switch (button.rebindKey)
                                {
                                    case "A":
                                        buttonAmount = _rightPrimary ? 1f : 0f;
                                        break;
                                    case "B":
                                        buttonAmount = _rightSecondary ? 1f : 0f;
                                        break;
                                    case "X":
                                        buttonAmount = _leftPrimary ? 1f : 0f;
                                        break;
                                    case "Y":
                                        buttonAmount = _leftSecondary ? 1f : 0f;
                                        break;
                                    case "LG":
                                        buttonAmount = _leftGrab ? 1f : 0f;
                                        break;
                                    case "RG":
                                        buttonAmount = _rightGrab ? 1f : 0f;
                                        break;
                                    case "LT":
                                        buttonAmount = _leftTrigger;
                                        break;
                                    case "RT":
                                        buttonAmount = _rightTrigger;
                                        break;
                                    case "LJ":
                                        buttonAmount = _leftJoystickClick ? 1f : 0f;
                                        break;
                                    case "RJ":
                                        buttonAmount = _rightJoystickClick ? 1f : 0f;
                                        break;
                                }
                                leftPrimary = buttonAmount > 0.5f;
                                leftSecondary = buttonAmount > 0.5f;
                                rightPrimary = buttonAmount > 0.5f;
                                rightSecondary = buttonAmount > 0.5f;
                                leftGrab = buttonAmount > 0.5f;
                                rightGrab = buttonAmount > 0.5f;
                                leftTrigger = buttonAmount;
                                rightTrigger = buttonAmount;
                                leftJoystickClick = buttonAmount > 0.5f;
                                rightJoystickClick = buttonAmount > 0.5f;
                            }
                            if (button.postMethod != null)
                                postActions.Add(button.buttonText);
                            button.method?.Invoke();
                            if (button.rebindKey != null)
                            {
                                leftPrimary = _leftPrimary;
                                leftSecondary = _leftSecondary;
                                rightPrimary = _rightPrimary;
                                rightSecondary = _rightSecondary;
                                leftGrab = _leftGrab;
                                rightGrab = _rightGrab;
                                leftTrigger = _leftTrigger;
                                rightTrigger = _rightTrigger;
                                leftJoystickClick = _leftJoystickClick;
                                rightJoystickClick = _rightJoystickClick;
                            }
                        }
                        catch (Exception exc)
                        {
                            LogManager.LogError(
                                $"Error with mod method {button.buttonText} at {exc.StackTrace}: {exc.Message}");
                        }

                        if (OverwriteKeybinds && button.customBind != null)
                        {
                            leftPrimary = _leftPrimary;
                            leftSecondary = _leftSecondary;
                            rightPrimary = _rightPrimary;
                            rightSecondary = _rightSecondary;
                            leftGrab = _leftGrab;
                            rightGrab = _rightGrab;
                            leftTrigger = _leftTrigger;
                            rightTrigger = _rightTrigger;
                            leftJoystickClick = _leftJoystickClick;
                            rightJoystickClick = _rightJoystickClick;
                        }
                    } catch { }
                }
                #endregion
            }
            catch (Exception exc)
            {
                LogManager.LogError($"Error with prefix at {exc.StackTrace}: {exc.Message}");
            }
        }

        private static readonly List<string> postActions = new List<string>();
        public static void Postfix()
        {
            try
            {
                foreach (string buttonName in postActions)
                {
                    try
                    {
                        ButtonInfo button = Buttons.GetIndex(buttonName);
                        bool _leftPrimary = leftPrimary;
                        bool _leftSecondary = leftSecondary;
                        bool _rightPrimary = rightPrimary;
                        bool _rightSecondary = rightSecondary;
                        bool _leftGrab = leftGrab;
                        bool _rightGrab = rightGrab;
                        float _leftTrigger = leftTrigger;
                        float _rightTrigger = rightTrigger;
                        bool _leftJoystickClick = leftJoystickClick;
                        bool _rightJoystickClick = rightJoystickClick;

                        if (OverwriteKeybinds && button.customBind != null)
                        {
                            leftPrimary = true;
                            leftSecondary = true;
                            rightPrimary = true;
                            rightSecondary = true;
                            leftGrab = true;
                            rightGrab = true;
                            leftTrigger = 1f;
                            rightTrigger = 1f;
                            leftJoystickClick = true;
                            rightJoystickClick = true;
                        }

                        try
                        {
                            if (button.rebindKey != null)
                            {
                                float buttonAmount = 0f;
                                switch (button.rebindKey)
                                {
                                    case "A":
                                        buttonAmount = _rightPrimary ? 1f : 0f;
                                        break;
                                    case "B":
                                        buttonAmount = _rightSecondary ? 1f : 0f;
                                        break;
                                    case "X":
                                        buttonAmount = _leftPrimary ? 1f : 0f;
                                        break;
                                    case "Y":
                                        buttonAmount = _leftSecondary ? 1f : 0f;
                                        break;
                                    case "LG":
                                        buttonAmount = _leftGrab ? 1f : 0f;
                                        break;
                                    case "RG":
                                        buttonAmount = _rightGrab ? 1f : 0f;
                                        break;
                                    case "LT":
                                        buttonAmount = _leftTrigger;
                                        break;
                                    case "RT":
                                        buttonAmount = _rightTrigger;
                                        break;
                                    case "LJ":
                                        buttonAmount = _leftJoystickClick ? 1f : 0f;
                                        break;
                                    case "RJ":
                                        buttonAmount = _rightJoystickClick ? 1f : 0f;
                                        break;
                                }
                                leftPrimary = buttonAmount > 0.5f;
                                leftSecondary = buttonAmount > 0.5f;
                                rightPrimary = buttonAmount > 0.5f;
                                rightSecondary = buttonAmount > 0.5f;
                                leftGrab = buttonAmount > 0.5f;
                                rightGrab = buttonAmount > 0.5f;
                                leftTrigger = buttonAmount;
                                rightTrigger = buttonAmount;
                                leftJoystickClick = buttonAmount > 0.5f;
                                rightJoystickClick = buttonAmount > 0.5f;
                            }
                            button.postMethod.Invoke();
                            if (button.rebindKey != null)
                            {
                                leftPrimary = _leftPrimary;
                                leftSecondary = _leftSecondary;
                                rightPrimary = _rightPrimary;
                                rightSecondary = _rightSecondary;
                                leftGrab = _leftGrab;
                                rightGrab = _rightGrab;
                                leftTrigger = _leftTrigger;
                                rightTrigger = _rightTrigger;
                                leftJoystickClick = _leftJoystickClick;
                                rightJoystickClick = _rightJoystickClick;
                            }
                        }
                        catch (Exception exc)
                        {
                            LogManager.LogError(
                                $"Error with mod postMethod {button.buttonText} at {exc.StackTrace}: {exc.Message}");
                        }

                        if (OverwriteKeybinds && button.customBind != null)
                        {
                            leftPrimary = _leftPrimary;
                            leftSecondary = _leftSecondary;
                            rightPrimary = _rightPrimary;
                            rightSecondary = _rightSecondary;
                            leftGrab = _leftGrab;
                            rightGrab = _rightGrab;
                            leftTrigger = _leftTrigger;
                            rightTrigger = _rightTrigger;
                            leftJoystickClick = _leftJoystickClick;
                            rightJoystickClick = _rightJoystickClick;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception exc)
            {
                LogManager.LogError($"Error with postfix at {exc.StackTrace}: {exc.Message}");
            }

            postActions.Clear();
        }

        public static List<KeyCode> lastPressedKeys = new List<KeyCode>();
        public static readonly Dictionary<KeyCode, (float, float)> keyPressedTimes = new Dictionary<KeyCode, (float, float)>();
        public static readonly KeyCode[] detectedKeyCodes = {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
            KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
            KeyCode.Z,

            KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
            KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
            KeyCode.Alpha8, KeyCode.Alpha9,

            KeyCode.Comma, KeyCode.Period, KeyCode.Slash, KeyCode.Backslash,
            KeyCode.Minus, KeyCode.Equals,KeyCode.Semicolon, KeyCode.Quote,
            KeyCode.LeftBracket, KeyCode.RightBracket,

            KeyCode.Space, KeyCode.Backspace, KeyCode.Return, KeyCode.Escape
        };

        private static void UpdateKeyboard()
        {
            if (VRKeyboard != null)
            {
                if (Vector3.Distance(VRKeyboard.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) > menuScale && !leftSecondary)
                {
                    VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                    VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                }
            }

            if (inTextInput && isKeyboardPc)
            {
                List<KeyCode> keysPressed = new List<KeyCode>();
                foreach (KeyCode keyCode in detectedKeyCodes)
                {
                    if (UnityInput.Current.GetKey(keyCode))
                    {
                        if (keyPressedTimes.TryGetValue(keyCode, out (float, float) delay))
                        {
                            float newDelay = Mathf.Max(delay.Item2 * 0.75f, 0.05f);

                            if (Time.time > delay.Item1)
                                keyPressedTimes[keyCode] = (Time.time + newDelay, newDelay);
                            else
                                continue;
                        }
                        else
                            keyPressedTimes[keyCode] = (Time.time + 0.5f, 0.5f);

                        keysPressed.Add(keyCode);

                        if (lastPressedKeys.Contains(keyCode)) continue;
                        if (UnityInput.Current.GetKey(KeyCode.LeftControl))
                        {
                            switch (keyCode)
                            {
                                case KeyCode.A:
                                    keyboardInput = "";
                                    break;
                                case KeyCode.C:
                                    GUIUtility.systemCopyBuffer = keyboardInput;
                                    break;
                                case KeyCode.V:
                                    keyboardInput += GUIUtility.systemCopyBuffer;
                                    break;
                                case KeyCode.Backspace:
                                    keyboardInput = keyboardInput[..^1];
                                    break;
                            }
                        }
                        else
                        {
                            switch (keyCode)
                            {
                                case KeyCode.Backspace:
                                    if (keyboardInput.Length > 0)
                                        keyboardInput = keyboardInput[..^1];
                                    break;
                                case KeyCode.Escape:
                                    Toggle(isSearching ? "Search" : "Decline Prompt");

                                    break;
                                case KeyCode.Return:
                                    if (isSearching)
                                    {
                                        List<ButtonInfo> searchedMods = new List<ButtonInfo>();
                                        if (nonGlobalSearch && currentCategoryName != "Main")
                                        {
                                            foreach (ButtonInfo v in Buttons.buttons[currentCategoryIndex])
                                            {
                                                try
                                                {
                                                    string buttonText = v.overlapText ?? v.buttonText;

                                                    if (buttonText.ClearTags().Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                                        searchedMods.Add(v);
                                                }
                                                catch { }
                                            }
                                        }
                                        else
                                        {
                                            int categoryIndex = 0;
                                            foreach (ButtonInfo[] buttonInfos in Buttons.buttons)
                                            {
                                                foreach (ButtonInfo v in buttonInfos)
                                                {
                                                    try
                                                    {
                                                        if (((Buttons.categoryNames[categoryIndex].Contains("Admin") ||
                                                             Buttons.categoryNames[categoryIndex] == "Mod Givers") &&
                                                            !isAdmin)
                                                            || (v.detected && !allowDetected))
                                                            continue;

                                                        string buttonText = v.overlapText ?? v.buttonText;

                                                        if (buttonText.Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                                            searchedMods.Add(v);
                                                    }
                                                    catch { }
                                                }
                                                categoryIndex++;
                                            }
                                        }

                                        ButtonInfo[] buttons = StringsToInfos(Alphabetize(InfosToStrings(searchedMods.ToArray())));
                                        ButtonInfo button = buttons[0];

                                        if (button.incremental)
                                            ToggleIncremental(button.buttonText, UnityInput.Current.GetKey(KeyCode.LeftShift));
                                        else
                                            Toggle(buttons[0].buttonText, true);
                                    }
                                    else if (CurrentPrompt != null && CurrentPrompt.IsText)
                                        Toggle("Accept Prompt");

                                    break;
                                default:
                                    keyboardInput +=
                                        UnityInput.Current.GetKey(KeyCode.LeftShift) || UnityInput.Current.GetKey(KeyCode.RightShift) ?
                                            keyCode.ShiftedKey().ToUpper() : keyCode.Key().ToLower();
                                    break;
                            }
                        }

                        if (pcKeyboardSounds)
                            VRRig.LocalRig.PlayHandTapLocal(66, false, buttonClickVolume / 10f);
                        pageNumber = 0;
                        ReloadMenu();
                    }
                    else
                        keyPressedTimes.Remove(keyCode);
                }

                lastPressedKeys = keysPressed;
            }
        }

        public static void PressKeyboardKey(string key)
        {
            switch (key)
            {
                case "Space":
                    keyboardInput += " ";
                    break;
                case "Backspace":
                    if (keyboardInput.Length > 0)
                        keyboardInput = keyboardInput[..^1];
                    break;
                case "Shift":
                    shift = !shift;
                    break;
                case "CapsLock":
                    lockShift = !lockShift;
                    break;

                case "Clear":
                    keyboardInput = "";
                    break;
                case "Copy":
                    GUIUtility.systemCopyBuffer = keyboardInput;
                    break;
                case "Paste":
                    keyboardInput += GUIUtility.systemCopyBuffer;
                    break;

                default:
                    Dictionary<string, string> shiftMap = new Dictionary<string, string>
                    {
                        { "1", "!" }, { "2", "@" }, { "3", "#" }, { "4", "$" }, { "5", "%" },
                        { "6", "^" }, { "7", "&" }, { "8", "*" }, { "9", "(" }, { "0", ")" },
                        { "-", "_" }, { "=", "+" }, { "[", "{" }, { "]", "}" }, { "\\", "|" },
                        { ";", ":" }, { "'", "\"" }, { ",", "<" }, { ".", ">" }, { "/", "?" },
                        { "`", "~" }
                    };

                    bool isShifted = lockShift ^ shift;
                    string keyStr = key.ToLower();

                    if (isShifted)
                    {
                        if (shiftMap.ContainsKey(keyStr))
                            keyboardInput += shiftMap[keyStr];
                        else
                            keyboardInput += keyStr.ToUpper();
                    }
                    else
                        keyboardInput += keyStr.ToLower();

                    shift = false;
                    break;

            }

            KeyboardKey.keyLookupDictionary["CapsLock"].gameObject.GetOrAddComponent<ColorChanger>().colors = buttonColors[lockShift ? 1 : 0];
            KeyboardKey.keyLookupDictionary["Shift"].gameObject.GetOrAddComponent<ColorChanger>().colors = buttonColors[shift ? 1 : 0];

            pageNumber = 0;

            ReloadMenu();
        }

        private static void AddButton(float offset, int buttonIndex, ButtonInfo method)
        {
            if (!method.label)
            {
                GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                    buttonObject.layer = 2;

                buttonObject.GetComponent<BoxCollider>().isTrigger = true;
                buttonObject.transform.parent = menu.transform;
                buttonObject.transform.rotation = Quaternion.identity;

                buttonObject.transform.localScale = thinMenu ? new Vector3(0.09f, 0.9f, ButtonDistance * 0.8f) : new Vector3(0.09f, 1.3f, ButtonDistance * 0.8f);

                if (longmenu && buttonIndex >= PageSize)
                {
                    menuBackground.transform.localScale += new Vector3(0f, 0f, 0.1f);
                    menuBackground.transform.localPosition += new Vector3(0f, 0f, -0.05f);
                }
                
                buttonObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
                if (checkMode && buttonIndex > -1)
                {
                    // The Checkbox Theorem ; TO BE THE SQUARE, YOU MUST circumvent the inconvenient menu localScale parameter
                    // Variable calculations: (menu scale y)0.3825 / (menu scale z)0.3 = 1.275 = Y
                    // 0.08 x Y = 0.102

                    buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
                    buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, 0.399f, 0.28f - offset) : new Vector3(0.56f, 0.599f, 0.28f - offset);
                }

                Button Button = buttonObject.AddComponent<Button>();
                Button.relatedText = method.buttonText;

                if (incrementalButtons)
                {
                    if (method.incremental)
                    {
                        if (checkMode && buttonIndex > -1)
                        {
                            Button.incremental = true;
                            Button.positive = false;

                            RenderIncrementalText(false, offset);
                            RenderIncrementalButton(true, offset, buttonIndex, method);
                        } else
                        {
                            buttonObject.transform.localScale -= new Vector3(0f, 0.254f, 0f);
                            Destroy(Button);

                            RenderIncrementalButton(false, offset, buttonIndex, method);
                            RenderIncrementalButton(true, offset, buttonIndex, method);
                        }
                    }
                }

                bool shouldSwap = swapButtonColors && buttonIndex < 0;

                if (lastClickedName != method.buttonText)
                {
                    ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[shouldSwap ^ method.enabled ? 1 : 0];

                    if (joystickMenu && buttonIndex == joystickButtonSelected)
                    {
                        joystickSelectedButton = method.buttonText;

                        if (!colorChanger.colors.transparent)
                        {
                            ExtGradient gradient = colorChanger.colors.Clone();
                            gradient.SetColor(0, Color.red);

                            colorChanger.colors = gradient;
                        }
                    }
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(buttonIndex, buttonObject.GetComponent<Renderer>()));

                FollowMenuSettings(buttonObject, shouldSwap ? method.enabled : !method.enabled);
            }

            TextMeshPro buttonText = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<TextMeshPro>();

            buttonText.font = activeFont;
            buttonText.text = method.buttonText;

            if (method.overlapText != null)
                buttonText.text = method.overlapText;

            if (method.detected)
            {
                buttonText.text = $"<color=red>{buttonText.text}</color>";
            }

            if (adaptiveButtons)
            {
                switch (ControllerUtilities.GetLeftControllerType())
                {
                    case ControllerUtilities.ControllerType.ValveIndex:
                        {
                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                { "x", "la" },
                                { "y", "lb" }
                            };

                            foreach (var replacement in replacements)
                                buttonText.text = buttonText.text.Replace($"<color=green>{replacement.Key.ToUpper()}</color>", $"<color=green>{replacement.Value.ToUpper()}</color>");

                            break;
                        }
                    case ControllerUtilities.ControllerType.VIVE:
                        {
                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                { "x", "ltp" }
                            };

                            foreach (var replacement in replacements)
                                buttonText.text = buttonText.text.Replace($"<color=green>{replacement.Key.ToUpper()}</color>", $"<color=green>{replacement.Value.ToUpper()}</color>");

                            break;
                        }
                }

                switch (ControllerUtilities.GetRightControllerType())
                {
                    case ControllerUtilities.ControllerType.ValveIndex:
                        {
                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                { "a", "ra" },
                                { "b", "rb" }
                            };

                            foreach (var replacement in replacements)
                                buttonText.text = buttonText.text.Replace($"<color=green>{replacement.Key.ToUpper()}</color>", $"<color=green>{replacement.Value.ToUpper()}</color>");

                            break;
                        }
                    case ControllerUtilities.ControllerType.VIVE:
                        {
                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                { "a", "rtp" }
                            };

                            foreach (var replacement in replacements)
                                buttonText.text = buttonText.text.Replace($"<color=green>{replacement.Key.ToUpper()}</color>", $"<color=green>{replacement.Value.ToUpper()}</color>");

                            break;
                        }
                }
            }
            
            if (method.rebindKey != null)
            {
                if (buttonText.text.Contains("</color><color=grey>]</color>"))
                    buttonText.text = buttonText.text.Split("<color=grey>[</color><color=green>")[0] + "<color=grey>[</color><color=green>" + method.rebindKey + "</color><color=grey>]</color>";
            }
            
            if (method.customBind != null)
            {
                if (buttonText.text.Contains("</color><color=grey>]</color>"))
                    buttonText.text = buttonText.text.Replace("</color><color=grey>]</color>", $"/{method.customBind}</color><color=grey>]</color>");
                else
                    buttonText.text += $" <color=grey>[</color><color=green>{method.customBind}</color><color=grey>]</color>";
            }

            if (inputTextColor != "green")
                buttonText.text = buttonText.text.Replace(" <color=grey>[</color><color=green>", $" <color=grey>[</color><color={inputTextColor}>");

            buttonText.text = FollowMenuSettings(buttonText.text);
            buttonText.spriteAsset = ButtonSpriteSheet;

            if (favorites.Contains(method.buttonText))
                buttonText.text = $"    {buttonText.text}    <sprite name=\"Favorite\">";

            buttonText.richText = true;
            buttonText.fontSize = 1;

            if (joystickMenu && buttonIndex == joystickButtonSelected && themeType == 30)
                buttonText.color = Color.red;
            else
                buttonText.AddComponent<TextColorChanger>().colors = textColors[method.enabled ? 2 : 1];

            buttonText.alignment = checkMode ? TextAlignmentOptions.Left : TextAlignmentOptions.Center;
            buttonText.fontStyle = activeFontStyle;
            buttonText.enableAutoSizing = true;
            buttonText.fontSizeMin = 0;

            RectTransform textTransform = buttonText.GetComponent<RectTransform>();
            textTransform.localPosition = Vector3.zero;
            textTransform.sizeDelta = new Vector2(method.incremental && incrementalButtons ? .18f : .2f, .03f * (ButtonDistance / 0.1f));
            if (NoAutoSizeText)
                textTransform.sizeDelta = new Vector2(9f, 0.015f);

            textTransform.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(buttonText);
        }

        private static void AddSearchButton()
        {
            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                buttonObject.layer = 2;

            buttonObject.GetComponent<BoxCollider>().isTrigger = true;
            buttonObject.transform.parent = menu.transform;
            buttonObject.transform.rotation = Quaternion.identity;

            buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, -0.450f, -0.58f) : new Vector3(0.56f, -0.7f, -0.58f);

            buttonObject.AddComponent<Button>().relatedText = "Search";

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[isSearching ^ !swapButtonColors ? 0 : 1];

            if (joystickMenuSearching && joystickButtonSelected == PageSize)
            {
                joystickSelectedButton = "Search";

                ExtGradient gradient = colorChanger.colors.Clone();
                gradient.SetColor(0, Color.red);

                colorChanger.colors = gradient;
            }

            FollowMenuSettings(buttonObject, isSearching ^ !swapButtonColors);

            Image searchImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (searchIcon == null)
                searchIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.search.png");

            if (searchMat == null)
                searchMat = new Material(searchImage.material);

            searchImage.material = searchMat;
            searchImage.material.SetTexture("_MainTex", searchIcon);
            searchImage.AddComponent<ImageColorChanger>().colors = textColors[isSearching ? 2 : 1];

            RectTransform imageTransform = searchImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, -0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, -0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(searchImage);
        }

        private static TMP_SpriteAsset _buttonSpriteSheet;
        public static TMP_SpriteAsset ButtonSpriteSheet
        {
            get
            {
                if (_buttonSpriteSheet == null)
                {
                    _buttonSpriteSheet = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
                    _buttonSpriteSheet.name = "iiMenu_SpriteSheet";

                    var textureList = new List<Texture2D>();
                    var spriteDataList = new List<(string name, int index)>();

                    void AddSprite(string name, Texture2D tex)
                    {
                        spriteDataList.Add((name, textureList.Count));
                        textureList.Add(tex);
                    }

                    AddSprite("Favorite", LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.favorite.png"));
                    AddSprite("Folder", LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.folder.png"));

                    for (int i = 1; i <= 3; i++)
                    {
                        AddSprite("Left" + i, LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.left{i}.png"));
                        AddSprite("Right" + i, LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.right{i}.png"));
                    }

                    int maxSize = 512;
                    Texture2D spriteSheet = new Texture2D(maxSize, maxSize);
                    Rect[] rects = spriteSheet.PackTextures(textureList.ToArray(), 2, maxSize);

                    _buttonSpriteSheet.spriteSheet = spriteSheet;
                    _buttonSpriteSheet.material = new Material(Shader.Find("TextMeshPro/Sprite"))
                    {
                        mainTexture = spriteSheet
                    };

                    _buttonSpriteSheet.spriteInfoList = new List<TMP_Sprite>();
                    Traverse.Create(_buttonSpriteSheet).Field("m_Version").SetValue("1.1.0"); // TextMeshPro kills itself unless this is set.

                    _buttonSpriteSheet.spriteGlyphTable.Clear();
                    for (int i = 0; i < spriteDataList.Count; i++)
                    {
                        var rect = rects[i];

                        var glyph = new TMP_SpriteGlyph
                        {
                            index = (uint)i,
                            metrics = new GlyphMetrics(
                                width: rect.width * spriteSheet.width,
                                height: rect.height * spriteSheet.height,
                                bearingX: -(rect.width * spriteSheet.width) / 2f,
                                bearingY: rect.height * spriteSheet.height * 0.8f,
                                advance: rect.width * spriteSheet.width
                            ),
                            glyphRect = new GlyphRect(
                                x: (int)(rect.x * spriteSheet.width),
                                y: (int)(rect.y * spriteSheet.height),
                                width: (int)(rect.width * spriteSheet.width),
                                height: (int)(rect.height * spriteSheet.height)
                            ),
                            scale = 1f,
                            atlasIndex = 0
                        };
                        _buttonSpriteSheet.spriteGlyphTable.Add(glyph);
                    }

                    _buttonSpriteSheet.spriteCharacterTable.Clear();
                    for (int i = 0; i < spriteDataList.Count; i++)
                    {
                        var (name, _) = spriteDataList[i];

                        var character = new TMP_SpriteCharacter(0xFFFE, _buttonSpriteSheet.spriteGlyphTable[i])
                        {
                            name = name,
                            scale = 1f,
                            glyphIndex = (uint)i
                        };
                        _buttonSpriteSheet.spriteCharacterTable.Add(character);
                    }

                    _buttonSpriteSheet.UpdateLookupTables();
                }
                return _buttonSpriteSheet;
            }
        }

        private static void AddDebugButton()
        {
            bool infoScreenEnabled = Buttons.GetIndex("Info Screen").enabled;

            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                buttonObject.layer = 2;

            buttonObject.GetComponent<BoxCollider>().isTrigger = true;
            buttonObject.transform.parent = menu.transform;
            buttonObject.transform.rotation = Quaternion.identity;

            buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, 0.450f, -0.58f) : new Vector3(0.56f, 0.7f, -0.58f);

            buttonObject.AddComponent<Button>().relatedText = "Info Screen";

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[infoScreenEnabled ^ swapButtonColors ? 0 : 1];

            FollowMenuSettings(buttonObject, infoScreenEnabled ^ swapButtonColors);

            Image debugImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (debugIcon == null)
                debugIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.debug.png");

            if (debugMat == null)
                debugMat = new Material(debugImage.material);

            debugImage.material = debugMat;
            debugImage.material.SetTexture("_MainTex", debugIcon);
            debugImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = debugImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(debugImage);
        }

        private static void AddDonateButton()
        {
            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                buttonObject.layer = 2;

            buttonObject.GetComponent<BoxCollider>().isTrigger = true;
            buttonObject.transform.parent = menu.transform;
            buttonObject.transform.rotation = Quaternion.identity;

            buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, 0.450f, -0.58f) : new Vector3(0.56f, 0.7f, -0.58f);

            buttonObject.AddComponent<Button>().relatedText = "Donate Button";

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];

            FollowMenuSettings(buttonObject, !swapButtonColors);

            Image donateImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (donateIcon == null)
                donateIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.donate.png");

            if (donateMat == null)
                donateMat = new Material(donateImage.material);

            donateImage.material = donateMat;
            donateImage.material.SetTexture("_MainTex", donateIcon);
            donateImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = donateImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(donateImage);
        }

        private static void AddUpdateButton()
        {
            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                buttonObject.layer = 2;

            buttonObject.GetComponent<BoxCollider>().isTrigger = true;
            buttonObject.transform.parent = menu.transform;
            buttonObject.transform.rotation = Quaternion.identity;

            buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, 0.450f, -0.58f) : new Vector3(0.56f, 0.7f, -0.58f);

            buttonObject.AddComponent<Button>().relatedText = "Update Button";

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];

            FollowMenuSettings(buttonObject, !swapButtonColors);

            Image updateImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (updateIcon == null)
                updateIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.update.png");

            if (updateMat == null)
                updateMat = new Material(updateImage.material);

            updateImage.material = updateMat;
            updateImage.material.SetTexture("_MainTex", updateIcon);
            updateImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = updateImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(updateImage);
        }

        private static void AddReturnButton(bool offcenteredPosition)
        {
            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                buttonObject.layer = 2;

            buttonObject.GetComponent<BoxCollider>().isTrigger = true;
            buttonObject.transform.parent = menu.transform;
            buttonObject.transform.rotation = Quaternion.identity;

            buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, -0.450f, -0.58f) : new Vector3(0.56f, -0.7f, -0.58f);

            if (offcenteredPosition)
                buttonObject.transform.localPosition += new Vector3(0f, 0.16f, 0f);

            buttonObject.AddComponent<Button>().relatedText = "Global Return";

            if (lastClickedName != "Global Return")
            {
                ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                colorChanger.colors = colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];
            }
            else
                CoroutineManager.instance.StartCoroutine(ButtonClick(-99, buttonObject.GetComponent<Renderer>()));

            FollowMenuSettings(buttonObject, !swapButtonColors);

            Image returnImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();

            if (returnIcon == null)
                returnIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.return.png");

            if (returnMat == null)
                returnMat = new Material(returnImage.material);

            returnImage.material = returnMat;
            returnImage.material.SetTexture("_MainTex", returnIcon);
            returnImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = returnImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, -0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, -0.54444444444f / 2.6f, -0.58f / 2.6f);

            if (offcenteredPosition)
                imageTransform.localPosition += new Vector3(0f, 0.0475f, 0f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(returnImage);
        }

        private static void RenderIncrementalButton(bool increment, float offset, int buttonIndex, ButtonInfo method)
        {
            if (!method.label)
            {
                GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                    buttonObject.layer = 2;

                buttonObject.GetComponent<BoxCollider>().isTrigger = true;
                buttonObject.transform.parent = menu.transform;
                buttonObject.transform.rotation = Quaternion.identity;

                buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, ButtonDistance * 0.8f);
                buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, 0.399f, 0.28f - offset) : new Vector3(0.56f, 0.599f, 0.28f - offset);

                Button button = buttonObject.AddComponent<Button>();
                button.relatedText = method.buttonText;
                button.incremental = true;
                button.positive = increment;

                if (increment)
                    buttonObject.transform.localPosition = new Vector3(buttonObject.transform.localPosition.x, -buttonObject.transform.localPosition.y, buttonObject.transform.localPosition.z);

                if (lastClickedName != method.buttonText + (increment ? "+" : "-"))
                {
                    ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(buttonIndex, buttonObject.GetComponent<Renderer>()));

                FollowMenuSettings(buttonObject, true);
            }

            RenderIncrementalText(increment, offset);
        }

        public static void RenderIncrementalText(bool increment, float offset)
        {
            TextMeshPro buttonText = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<TextMeshPro>();

            buttonText.font = activeFont;
            buttonText.text = increment ? "+" : "-";
            buttonText.richText = true;
            buttonText.fontSize = 1;
            buttonText.AddComponent<TextColorChanger>().colors = textColors[1];

            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.fontStyle = activeFontStyle;
            buttonText.enableAutoSizing = true;
            buttonText.fontSizeMin = 0;

            RectTransform textTransform = buttonText.GetComponent<RectTransform>();
            textTransform.localPosition = Vector3.zero;
            textTransform.sizeDelta = new Vector2(.2f, .03f * (ButtonDistance / 0.1f));
            if (NoAutoSizeText)
                textTransform.sizeDelta = new Vector2(9f, 0.015f);

            textTransform.localPosition = thinMenu ? new Vector3(.064f, increment ? -0.12f : 0.12f, .111f - offset / 2.6f) : new Vector3(.064f, increment ? -0.18f : 0.18f, .111f - offset / 2.6f);
            textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(buttonText);
        }

        public static void CreateReference(bool? rightHandOverride = null)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = rightHandOverride ?? (rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton)) ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            reference.transform.localPosition = pointerOffset;
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();

            if (hidePointer)
                reference.GetComponent<Renderer>().enabled = false;
            else
            {
                ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;
            }
        }

        public static GameObject CreateMenu()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(menu.GetComponent<BoxCollider>());
            Destroy(menu.GetComponent<Renderer>());

            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);

            if (annoyingMode)
            {
                menu.transform.localScale = new Vector3(0.1f, Random.Range(10f, 40f) / 100f, 0.3825f);
                backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(RandomColor(), RandomColor()) };

                buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSimpleGradient(RandomColor(), RandomColor()) };
                buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSimpleGradient(RandomColor(), RandomColor()) };

                textColors[0] = new ExtGradient { colors = ExtGradient.GetSimpleGradient(RandomColor(), RandomColor()) };
                textColors[1] = new ExtGradient { colors = ExtGradient.GetSimpleGradient(RandomColor(), RandomColor()) };
                textColors[2] = new ExtGradient { colors = ExtGradient.GetSimpleGradient(RandomColor(), RandomColor()) };
            }

            if (themeType == 7)
            {
                GameObject coneBackground = LoadObject<GameObject>("Cone");

                coneBackground.transform.parent = menu.transform;
                coneBackground.transform.localPosition = Vector3.zero;
                coneBackground.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menuBackground.GetComponent<BoxCollider>());

                menuBackground.transform.parent = menu.transform;
                menuBackground.transform.localPosition = new Vector3(0.50f, 0f, 0f);
                menuBackground.transform.rotation = Quaternion.identity;

                // Size is calculated in depth, width, height
                menuBackground.transform.localScale = thinMenu ? new Vector3(0.1f, 1f, 1f) : new Vector3(0.1f, 1.5f, 1f);

                if (innerOutline || themeType == 34)
                {
                    GameObject innerOutlineSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(innerOutlineSegment.GetComponent<BoxCollider>());
                    innerOutlineSegment.transform.parent = menuBackground.transform;
                    innerOutlineSegment.transform.rotation = Quaternion.identity;
                    innerOutlineSegment.transform.localPosition = new Vector3(0f, -0.4840625f, 0f);
                    innerOutlineSegment.transform.localScale = new Vector3(1.025f, 0.0065f, 0.98f);

                    ColorChanger colorChanger = innerOutlineSegment.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[1];

                    innerOutlineSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(innerOutlineSegment.GetComponent<BoxCollider>());
                    innerOutlineSegment.transform.parent = menuBackground.transform;
                    innerOutlineSegment.transform.rotation = Quaternion.identity;
                    innerOutlineSegment.transform.localPosition = new Vector3(0f, 0.4840625f, 0f);
                    innerOutlineSegment.transform.localScale = new Vector3(1.025f, 0.0065f, 0.98f);

                    colorChanger = innerOutlineSegment.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[1];

                    innerOutlineSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(innerOutlineSegment.GetComponent<BoxCollider>());
                    innerOutlineSegment.transform.parent = menuBackground.transform;
                    innerOutlineSegment.transform.rotation = Quaternion.identity;
                    innerOutlineSegment.transform.localPosition = new Vector3(0f, 0f, -0.4875f);
                    innerOutlineSegment.transform.localScale = new Vector3(1.025f, 0.968125f, 0.005f);

                    colorChanger = innerOutlineSegment.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[1];

                    innerOutlineSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(innerOutlineSegment.GetComponent<BoxCollider>());
                    innerOutlineSegment.transform.parent = menuBackground.transform;
                    innerOutlineSegment.transform.rotation = Quaternion.identity;
                    innerOutlineSegment.transform.localPosition = new Vector3(0f, 0f, 0.4875f);
                    innerOutlineSegment.transform.localScale = new Vector3(1.025f, 0.968125f, 0.005f);

                    colorChanger = innerOutlineSegment.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[1];
                }

                if (themeType == 25 || themeType == 26 || themeType == 27 || themeType == 63)
                {
                    Renderer menuBackgroundRenderer = menuBackground.GetComponent<Renderer>();
                    switch (themeType)
                    {
                        case 25:
                            if (pride == null)
                            {
                                pride = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Themes/pride.png", "Images/Themes/pride.png");
                                pride.filterMode = FilterMode.Point;
                                pride.wrapMode = TextureWrapMode.Clamp;
                            }
                            menuBackgroundRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                            menuBackgroundRenderer.material.color = Color.white;
                            menuBackgroundRenderer.material.mainTexture = pride;
                            break;
                        case 26:
                            if (trans == null)
                            {
                                trans = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Themes/trans.png", "Images/Themes/trans.png");
                                trans.filterMode = FilterMode.Point;
                                trans.wrapMode = TextureWrapMode.Clamp;
                            }
                            menuBackgroundRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                            menuBackgroundRenderer.material.color = Color.white;
                            menuBackgroundRenderer.material.mainTexture = trans;
                            break;
                        case 27:
                            if (gay == null)
                            {
                                gay = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Themes/mlm.png", "Images/Themes/mlm.png");
                                gay.filterMode = FilterMode.Point;
                                gay.wrapMode = TextureWrapMode.Clamp;
                            }
                            menuBackgroundRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                            menuBackgroundRenderer.material.color = Color.white;
                            menuBackgroundRenderer.material.mainTexture = gay;
                            break;
                        case 63:
                            if (videoPlayer == null)
                            {
                                videoPlayer = new GameObject("iiMenu_VideoPlayer").AddComponent<VideoPlayer>();
                                videoPlayer.playOnAwake = true;
                                videoPlayer.isLooping = true;
                                videoPlayer.url = $"{PluginInfo.ServerResourcePath}/Videos/Themes/badapple.mp4";

                                RenderTexture rt = new RenderTexture(192, 144, 0);
                                rt.Create();

                                videoPlayer.targetTexture = rt;
                            }

                            menuBackgroundRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                            menuBackgroundRenderer.material.color = Color.white;
                            menuBackgroundRenderer.material.SetTexture("_BaseMap", videoPlayer.targetTexture);

                            videoPlayer.time = badAppleTime;

                            break;
                    }
                }
                else
                {
                    if (doCustomMenuBackground)
                    {
                        menuBackground.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                        menuBackground.GetComponent<Renderer>().material.color = Color.white;
                        menuBackground.GetComponent<Renderer>().material.mainTexture = customMenuBackgroundImage;
                    }
                    else
                    {
                        ColorChanger colorChanger = menuBackground.AddComponent<ColorChanger>();
                        colorChanger.colors = backgroundColor;
                    }
                }

                FollowMenuSettings(menuBackground, false);
            }

            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            if (hideTextOnCamera)
                canvasObj.layer = 19;

            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 2500f;

            canvasObj.AddComponent<GraphicRaycaster>();

            if (!hidetitle)
            {
                title = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<TextMeshPro>();
                title.font = activeFont;
                title.text = translate ? "ii's Stupid Menu" : "ii's <b>Stupid</b> Menu";

                if (doCustomName)
                    title.text = customMenuName;

                if (annoyingMode)
                {
                    string[] randomMenuNames = {
                    "ModderX",
                    "ShibaGT Gold",
                    "Kman Menu",
                    "WM TROLLING MENU",
                    "ShibaGT Dark",
                    "ShibaGT-X v5.5",
                    "ii stupid",
                    "bvunt menu",
                    "GorillaTaggingKid Menu",
                    "fart",
                    "steal.lol",
                    "Unttile menu"
                };

                    if (Random.Range(1, 5) == 2)
                        title.text = randomMenuNames[Random.Range(0, randomMenuNames.Length)] + " v" + Random.Range(8, 159);
                }

                title.text = FollowMenuSettings(title.text, !doCustomName);

                if (!noPageNumber)
                    title.text += $" <color=grey>[</color><color=white>{(pageScrolling ? pageOffset : pageNumber) + 1}</color><color=grey>]</color>";

                if (gradientTitle)
                    title.text = RichtextGradient(NoRichtextTags(title.text),
                        new[]
                        {
                            new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 0f),
                            new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0), 0.95f), 0.5f),
                            new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 1f)
                        });

                if (animatedTitle)
                {
                    string targetString = doCustomName ? NoRichtextTags(customMenuName) : "ii's Stupid Menu";
                    int length = (int)Mathf.PingPong(Time.time / 0.25f, targetString.Length);
                    title.text = length > 0 ? targetString[..length] : "";
                }

                title.fontSize = 1;
                title.AddComponent<TextColorChanger>().colors = textColors[0];

                title.richText = true;
                title.fontStyle = activeFontStyle;
                title.alignment = TextAlignmentOptions.Center;
                title.enableAutoSizing = true;
                title.fontSizeMin = 0;
                RectTransform component = title.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.28f, 0.05f);
                if (NoAutoSizeText)
                    component.sizeDelta = new Vector2(0.28f, 0.015f);

                component.localPosition = new Vector3(0.06f, 0f, 0.165f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                FollowMenuSettings(title);
            }

            if (!backgroundColor.transparent)
            {
                TextMeshPro buildLabel = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<TextMeshPro>();
                buildLabel.font = activeFont;
                buildLabel.text = $"Build {PluginInfo.Version}";

                buildLabel.text = FollowMenuSettings(buildLabel.text);

                buildLabel.fontSize = 1;
                buildLabel.AddComponent<TextColorChanger>().colors = textColors[0];
                buildLabel.richText = true;
                buildLabel.fontStyle = activeFontStyle;
                buildLabel.alignment = TextAlignmentOptions.Right;
                buildLabel.enableAutoSizing = true;
                buildLabel.fontSizeMin = 0;

                RectTransform component = buildLabel.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.28f, 0.02f);
                component.position = thinMenu ? new Vector3(0.04f, 0.0f, -0.17f) : new Vector3(0.04f, 0.07f, -0.17f);
                component.rotation = Quaternion.Euler(new Vector3(0f, 90f, 90f));

                FollowMenuSettings(buildLabel);

                if (!disableWatermark)
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

                    RectTransform imageTransform = watermarkImage.GetComponent<RectTransform>();
                    imageTransform.localPosition = Vector3.zero;
                    imageTransform.sizeDelta = new Vector2(.15f, .15f);

                    imageTransform.localPosition = new Vector3(0.04f, 0f, 0f);

                    FollowMenuSettings(watermarkImage);

                    imageTransform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 90f - (rockWatermark ? (Mathf.Sin(Time.time * 2f) * 10f) : 0f)));

                    if (customWatermark == null)
                        watermarkImage.AddComponent<ImageColorChanger>().colors = textColors[0];
                    else
                        watermarkImage.material.color = Color.white;
                }
            }

            if (!disableFpsCounter)
            {
                TextMeshPro fps = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<TextMeshPro>();
                fps.font = activeFont;

                string textToSet = ftCount ? $"FT: {Mathf.Floor(1f / lastDeltaTime * 10000f) / 10f} ms" : $"FPS: {lastDeltaTime}";
                if (hidetitle && !noPageNumber) textToSet += "      ";
                if (hidetitle && !noPageNumber) textToSet += "Page " + (pageNumber + 1);

                fps.text = FollowMenuSettings(textToSet, false);

                fps.AddComponent<TextColorChanger>().colors = textColors[0];
                fpsCount = fps;
                fps.fontSize = 1;
                fps.richText = true;
                fps.fontStyle = activeFontStyle;
                fps.alignment = TextAlignmentOptions.Center;
                fps.overflowMode = TextOverflowModes.Overflow;
                fps.enableAutoSizing = true;
                fps.fontSizeMin = 0;
                RectTransform fpsTransform = fps.GetComponent<RectTransform>();
                fpsTransform.sizeDelta = NoAutoSizeText ? new Vector2(9f, 0.015f) : new Vector2(0.28f, 0.02f);
                fpsTransform.localPosition = new Vector3(0.06f, 0f, hidetitle ? 0.175f : 0.135f);
                fpsTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                FollowMenuSettings(fps);
            }

            float hkbStartTime = -0.3f;
            if (!disableDisconnectButton)
            {
                AddButton(-0.3f, -1, Buttons.GetIndex("Disconnect"));
                hkbStartTime -= ButtonDistance;
            }

            if (quickActions.Count > 0)
            {
                foreach (string action in quickActions.ToList())
                {
                    ButtonInfo button = Buttons.GetIndex(action);
                    if (button == null)
                    {
                        quickActions.Remove(action);
                        continue;
                    }

                    AddButton(hkbStartTime, -1, button);
                    hkbStartTime -= ButtonDistance;
                }
            }

            // Search button
            if (!disableSearchButton)
            {
                AddSearchButton();
                if (!disableReturnButton && currentCategoryName != "Main")
                    AddReturnButton(true);
            }
            else
            {
                if (!disableReturnButton && currentCategoryName != "Main")
                    AddReturnButton(false);
            }
            
            if (enableDebugButton)
                AddDebugButton();
            else
            {
                if (!acceptedDonations)
                    AddDonateButton();
                else if (ServerData.OutdatedVersion)
                    AddUpdateButton();
            }

            if (!disablePageButtons && CurrentPrompt == null && !pageScrolling)
                AddPageButtons();

            if (inTextInput)
            {
                GameObject searchBoxObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                    searchBoxObject.layer = 2;

                searchBoxObject.GetComponent<BoxCollider>().isTrigger = true;
                searchBoxObject.transform.parent = menu.transform;
                searchBoxObject.transform.rotation = Quaternion.identity;

                searchBoxObject.transform.localScale = thinMenu ? new Vector3(0.09f, 0.9f, ButtonDistance * 0.8f) : new Vector3(0.09f, 1.3f, ButtonDistance * 0.8f);

                searchBoxObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - buttonOffset * ButtonDistance);

                ColorChanger colorChanger = searchBoxObject.AddComponent<ColorChanger>();
                colorChanger.colors = buttonColors[0];

                FollowMenuSettings(searchBoxObject, true);

                keyboardInputObject = new GameObject
                {
                    transform =
                        {
                            parent = canvasObj.transform
                        }
                }.AddComponent<TextMeshPro>();

                keyboardInputObject.font = activeFont;
                keyboardInputObject.text = FollowMenuSettings(keyboardInput) + ((Time.time % 1f) > 0.5f ? "|" : "");

                keyboardInputObject.richText = true;
                keyboardInputObject.fontSize = 1;

                if (joystickMenu && joystickButtonSelected == 0 && themeType == 30)
                    keyboardInputObject.color = Color.red;
                else
                    keyboardInputObject.AddComponent<TextColorChanger>().colors = textColors[1];

                keyboardInputObject.alignment = TextAlignmentOptions.Center;
                keyboardInputObject.fontStyle = activeFontStyle;
                keyboardInputObject.enableAutoSizing = true;
                keyboardInputObject.fontSizeMin = 0;

                RectTransform textTransform = keyboardInputObject.GetComponent<RectTransform>();
                textTransform.localPosition = Vector3.zero;
                textTransform.sizeDelta = new Vector2(.2f, .03f * (ButtonDistance / 0.1f));
                if (NoAutoSizeText)
                    textTransform.sizeDelta = new Vector2(9f, 0.015f);

                textTransform.localPosition = new Vector3(.064f, 0, .111f - buttonOffset * ButtonDistance / 2.6f);
                textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                FollowMenuSettings(keyboardInputObject);
            }

            if (promptVideoPlayer != null)
            {
                promptVideoPlayer.Stop();
                promptVideoPlayer.gameObject.Destroy();
                promptVideoPlayer = null;
            }

            if (CurrentPrompt != null)
                RenderPrompt();
            else
            {
                // Button render code
                int buttonIndexOffset = 0;
                ButtonInfo[] renderButtons;

                try
                {
                    if (isSearching)
                    {
                        List<ButtonInfo> searchedMods = new List<ButtonInfo>();
                        if (nonGlobalSearch && currentCategoryName != "Main")
                        {
                            foreach (ButtonInfo v in Buttons.buttons[currentCategoryIndex])
                            {
                                try
                                {
                                    string buttonText = v.buttonText;
                                    if (v.overlapText != null)
                                        buttonText = v.overlapText;

                                    if (buttonText.ClearTags().Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                        searchedMods.Add(v);
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            int categoryIndex = 0;
                            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                            {
                                foreach (ButtonInfo v in buttonlist)
                                {
                                    try
                                    {
                                        if (((Buttons.categoryNames[categoryIndex].Contains("Admin") ||
                                            Buttons.categoryNames[categoryIndex] == "Mod Givers") &&
                                            !isAdmin)
                                            || (v.detected && !allowDetected))
                                            continue;

                                        string buttonText = v.buttonText;
                                        if (v.overlapText != null)
                                            buttonText = v.overlapText;

                                        if (buttonText.Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                            searchedMods.Add(v);
                                    }
                                    catch { }
                                }
                                categoryIndex++;
                            }
                        }

                        buttonIndexOffset += 1;
                        renderButtons = searchedMods.ToArray();
                    }
                    else if (annoyingMode && Random.Range(1, 5) == 3)
                    {
                        ButtonInfo disconnectButton = Buttons.GetIndex("Disconnect");
                        renderButtons = Enumerable.Repeat(disconnectButton, 15).ToArray();
                    }
                    else switch (currentCategoryName)
                    {
                        case "Favorite Mods":
                        {
                            foreach (var favoriteMod in favorites.Where(favoriteMod => Buttons.GetIndex(favoriteMod) == null).ToList())
                                favorites.Remove(favoriteMod);

                            renderButtons = StringsToInfos(favorites.ToArray());
                            break;
                        }
                        case "Enabled Mods":
                        {
                            List<ButtonInfo> enabledMods = new List<ButtonInfo>();
                            int categoryIndex = 0;
                            foreach (ButtonInfo[] buttonList in Buttons.buttons)
                            {
                                enabledMods.AddRange(buttonList.Where(v => v.enabled && (!hideSettings || !Buttons.categoryNames[categoryIndex].Contains("Settings")) && (!hideMacros || !Buttons.categoryNames[categoryIndex].Contains("Macro"))));
                                categoryIndex++;
                            }
                            enabledMods = enabledMods.OrderBy(v => v.buttonText).ToList();
                            enabledMods.Insert(0, Buttons.GetIndex("Exit Enabled Mods"));

                            renderButtons = enabledMods.ToArray();
                            break;
                        }
                        default:
                            renderButtons = Buttons.buttons[currentCategoryIndex];
                            break;
                    }

                    if (Buttons.GetIndex("Alphabetize Menu").enabled || isSearching)
                        renderButtons = StringsToInfos(Alphabetize(InfosToStrings(renderButtons)));

                    if (!longmenu)
                        renderButtons = renderButtons
                            .Skip(pageNumber * (PageSize - buttonIndexOffset) + pageOffset)
                            .Take(PageSize - buttonIndexOffset)
                            .ToArray();

                    for (int i = 0; i < renderButtons.Length; i++)
                        AddButton((i + buttonIndexOffset + buttonOffset) * ButtonDistance, i, renderButtons[i]);
                }
                catch
                {
                    LogManager.Log("Menu draw is erroring, returning to home page");
                    currentCategoryName = "Main";
                }
            }

            RecenterMenu();

            if (themeType == 50)
            {
                for (int i = 0; i < 15; i++)
                {
                    GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    particle.transform.position = menuBackground.transform.position;
                    particle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    Destroy(particle.GetComponent<BoxCollider>());
                    Destroy(particle, 2f);

                    particle.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                    particle.GetComponent<Renderer>().material.color = Color.white;

                    if (cannabisMat == null)
                    {
                        cannabisMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                        {
                            color = Color.white
                        };

                        if (cann == null)
                            cann = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Themes/cannabis.png", "Images/Themes/cannabis.png");

                        cannabisMat.mainTexture = cann;

                        cannabisMat.SetFloat("_Surface", 1);
                        cannabisMat.SetFloat("_Blend", 0);
                        cannabisMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        cannabisMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        cannabisMat.SetFloat("_ZWrite", 0);
                        cannabisMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        cannabisMat.renderQueue = (int)RenderQueue.Transparent;
                    }
                    particle.GetComponent<Renderer>().material = cannabisMat;

                    Rigidbody comp = particle.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.position = menuBackground.transform.position;
                    comp.linearVelocity = new Vector3(Random.Range(-3f, 3f), Random.Range(3f, 5f), Random.Range(-3f, 3f));
                    comp.angularVelocity = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                }
            }

            menu.transform.localScale *= scaleWithPlayer && XRSettings.isDeviceActive ? GTPlayer.Instance.scale * menuScale : menuScale;
            return menu;
        }

        public static void RecenterMenu()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q) || (inTextInput && isKeyboardPc);
            if (joystickMenu)
            {
                menu.transform.position = GorillaTagger.Instance.headCollider.transform.TransformPoint(joystickMenuPositions[joystickMenuPosition]);
                menu.transform.LookAt(GorillaTagger.Instance.headCollider.transform);
                Vector3 rotModify = menu.transform.rotation.eulerAngles;
                rotModify += new Vector3(-90f, 0f, -90f);
                menu.transform.rotation = Quaternion.Euler(rotModify);
            }
            else
            {
                if (!wristMenu)
                {
                    if (oneHand)
                    {
                        menu.transform.position = GorillaTagger.Instance.headCollider.transform.TransformPoint(new Vector3(0f, -0.1f, 0.5f));
                        menu.transform.LookAt(GorillaTagger.Instance.headCollider.transform);
                        Vector3 rotModify = menu.transform.rotation.eulerAngles;
                        rotModify += new Vector3(-90f, 0f, -90f);
                        menu.transform.rotation = Quaternion.Euler(rotModify);
                    }
                    else if (barkMenu && barkMenuOpen)
                    {
                        if (barkMenuGrabbed != null)
                        {
                            if (barkMenuGrabbed.Value)
                            {
                                menu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                                menu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                            }
                            else
                            {
                                menu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                                Vector3 rotation = GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles;
                                rotation += new Vector3(0f, 0f, 180f);
                                menu.transform.rotation = Quaternion.Euler(rotation);
                            }
                        } else
                        {
                            menu.transform.position = GorillaTagger.Instance.bodyCollider.transform.TransformPoint(new Vector3(0f, 0f, 0.5f));
                            menu.transform.position = new Vector3(menu.transform.position.x, GorillaTagger.Instance.headCollider.transform.position.y, menu.transform.position.z);
                            menu.transform.LookAt(GorillaTagger.Instance.bodyCollider.transform);
                            menu.transform.rotation = Quaternion.Euler(0f, menu.transform.eulerAngles.y, 0f);
                            Vector3 rotModify = menu.transform.rotation.eulerAngles;
                            rotModify += new Vector3(-90f, 0f, -90f);
                            menu.transform.rotation = Quaternion.Euler(rotModify);
                        }
                    } else
                    {
                        if (rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton))
                        {
                            menu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                            Vector3 rotation = GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles;
                            rotation += new Vector3(0f, 0f, 180f);
                            menu.transform.rotation = Quaternion.Euler(rotation);
                        }
                        else
                        {
                            menu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                            menu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                        }
                        if (flipMenu)
                        {
                            Vector3 rotation = menu.transform.rotation.eulerAngles;
                            rotation += new Vector3(0f, 0f, 180f);
                            menu.transform.rotation = Quaternion.Euler(rotation);
                        }
                    }
                }
                else
                {
                    menu.transform.localPosition = Vector3.zero;
                    menu.transform.localRotation = Quaternion.identity;
                    if (rightHand)
                        menu.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.3f, 0f);
                    else
                        menu.transform.position = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, 0.3f, 0f);
                    
                    menu.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    Vector3 rotModify = menu.transform.rotation.eulerAngles;
                    rotModify += new Vector3(-90f, 0f, -90f);
                    menu.transform.rotation = Quaternion.Euler(rotModify);
                }
            }
            if (inTextInput && !isKeyboardPc)
            {
                menu.transform.position = menuSpawnPosition.transform.position;
                menu.transform.rotation = menuSpawnPosition.transform.rotation;
                Vector3 rotModify = menu.transform.rotation.eulerAngles;
                rotModify += new Vector3(-90f, 90f, -90f);
                menu.transform.rotation = Quaternion.Euler(rotModify);
            }
            if (isKeyboardCondition)
            {
                GetObject("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);
                if (TPC != null)
                {
                    isOnPC = true;

                    if (!XRSettings.isDeviceActive)
                        PrivateUIRoom.instance.ToggleLevelVisibility(true);

                    if (joystickMenu)
                        Toggle("Joystick Menu");

                    if (watchMenu)
                        Toggle("Watch Menu");

                    if (physicalMenu)
                        Toggle("Physical Menu");

                    Vector3[] pcPositions = {
                        new Vector3(10f, 10f, 10f),
                        new Vector3(10f, 10f, 10f),
                        new Vector3(-67.9299f, 11.9144f, -84.2019f),
                        new Vector3(-63f, 3.634f, -65f),
                        VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.forward * 1.2f,
                        TPC?.transform.position ?? GorillaTagger.Instance.headCollider.transform.position
                    };

                    TPC.transform.position = pcPositions[pcbg];
                    if (pcbg != 4 && pcbg != 5)
                        TPC.transform.rotation = Quaternion.identity;

                    if (pcbg == 0)
                    {
                        if (pcBackground == null)
                        {
                            pcBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            pcBackground.transform.localScale = new Vector3(10f, 10f, 0.01f);
                            pcBackground.transform.transform.position = TPC.transform.position + TPC.transform.forward;

                            OnMenuClosed += () => Destroy(pcBackground);
                        }
                        
                        Color realcolor = backgroundColor.GetCurrentColor();
                        pcBackground.GetComponent<Renderer>().material.color = new Color32((byte)(realcolor.r * 50), (byte)(realcolor.g * 50), (byte)(realcolor.b * 50), 255);
                    }

                    menu.transform.parent = TPC.transform;
                    menu.transform.position = TPC.transform.position + TPC.transform.forward * 0.5f;
                    menu.transform.rotation = TPC.transform.rotation * Quaternion.Euler(-90f, 90f, 0f);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed && !isMouseDown)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            bool worked = Physics.Raycast(ray, out RaycastHit hit, 512f, NoInvisLayerMask());
                            if (worked)
                            {
                                Button collide = hit.transform.gameObject.GetComponent<Button>();
                                if (collide != null)
                                {
                                    collide.OnTriggerEnter(buttonCollider);
                                    buttonCooldown = -1f;
                                }
                            }
                        }
                        else
                            reference.transform.position = new Vector3(999f, -999f, -999f);

                        isMouseDown = Mouse.current.leftButton.isPressed;
                    }
                }
            } else
                isOnPC = false;

            if (physicalMenu)
            {
                if (physicalOpenPosition == Vector3.zero)
                {
                    physicalOpenPosition = menu.transform.position;
                    physicalOpenRotation = menu.transform.rotation;
                }

                menu.transform.position = physicalOpenPosition;
                menu.transform.rotation = physicalOpenRotation;
            }

            if (smoothMenuPosition)
            {
                smoothTargetPosition = smoothTargetPosition == Vector3.zero ? menu.transform.position : Vector3.Lerp(smoothTargetPosition, menu.transform.position, Time.deltaTime * 10f);

                menu.transform.position = smoothTargetPosition;
            }

            if (!smoothMenuRotation) return;
            smoothTargetRotation = smoothTargetRotation == Quaternion.identity ? menu.transform.rotation : Quaternion.Lerp(smoothTargetRotation, menu.transform.rotation, Time.deltaTime * 10f);

            menu.transform.rotation = smoothTargetRotation;
        }

        private static int menuOpenCount;

        public static event Action OnMenuOpened;
        public static void OpenMenu()
        {
            try
            {
                OnMenuOpened?.Invoke();
            } catch { }
            
            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/open.ogg", "Audio/Menu/open.ogg"), buttonClickVolume / 10f);

            CreateMenu();

            if (dynamicAnimations)
                CoroutineManager.instance.StartCoroutine(GrowCoroutine());

            if (particleSpawnEffect)
            {
                for (int i = 0; i < 25; i++)
                {
                    GameObject Particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Particle.transform.position = menu.transform.position;
                    Particle.transform.localScale = Vector3.one * (0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
                    Particle.AddComponent<CustomParticle>();
                    Destroy(Particle.GetComponent<Collider>());
                }
            }

            menuOpenCount++;
            if (menuOpenCount == 100)
                AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                {
                    name = "Persistent",
                    description = "Open the menu 100 times.",
                    icon = "Images/Achievements/persistent.png"

                });

            if (joystickMenu) return;
            if (reference == null)
                CreateReference();
        }

        public static event Action OnMenuClosed;
        public static void CloseMenu()
        {
            try
            {
                OnMenuClosed?.Invoke();
            } catch { }
            
            GetObject("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(true);
            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);

            try
            {
                if (isOnPC && TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet"))
                {
                    isOnPC = false;
                    TPC.transform.position = TPC.transform.parent.position;
                    TPC.transform.rotation = TPC.transform.parent.rotation;
                }
            }
            catch { }

            promptVideoPlayer?.Stop();

            smoothTargetPosition = Vector3.zero;
            smoothTargetRotation = Quaternion.identity;
            if (!dynamicAnimations || explodeMenu)
            {
                if (!dropOnRemove)
                {
                    Destroy(menu);
                    menu = null;

                    Destroy(reference);
                    reference = null;

                    return;
                }

                if (explodeMenu)
                {
                    try
                    {
                        if (dynamicSounds)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/explosion.ogg", "Audio/Menu/explosion.ogg"), buttonClickVolume / 10f);

                        foreach (GameObject gameObject in menu.transform.Children().Select(transform => transform.gameObject))
                        {
                            gameObject.transform.SetParent(null, true);
                            Rigidbody comp = gameObject.GetOrAddComponent<Rigidbody>();

                            if (zeroGravityMenu)
                                comp.useGravity = false;

                            if (menuCollisions)
                            {
                                GameObject collision = new GameObject("Collision");
                                collision.transform.SetParent(gameObject.transform, false);
                                collision.layer = 3;
                                collision.AddComponent<BoxCollider>();
                            }

                            comp.linearVelocity = RandomVector3(5f);
                            comp.angularVelocity = RandomVector3(50f);

                            Destroy(gameObject, 5f);
                        }
                    } catch { }
                } else
                {
                    try
                    {
                        Rigidbody comp = menu.GetOrAddComponent<Rigidbody>();

                        if (zeroGravityMenu)
                            comp.useGravity = false;

                        if (menuCollisions)
                        {
                            GameObject collision = new GameObject("Collision");
                            collision.transform.SetParent(menuBackground.transform, false);
                            collision.layer = 3;
                            collision.AddComponent<BoxCollider>();
                        }

                        if (rightHand || (bothHands && openedwithright))
                        {
                            comp.linearVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
                            comp.angularVelocity = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetOrAddComponent<GorillaVelocityEstimator>().angularVelocity;
                        }
                        else
                        {
                            comp.linearVelocity = GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0);
                            comp.angularVelocity = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetOrAddComponent<GorillaVelocityEstimator>().angularVelocity;
                        }

                        if (annoyingMode)
                        {
                            comp.linearVelocity = RandomVector3(5f);
                            comp.angularVelocity = RandomVector3(50f);
                        }
                    }
                    catch { }

                    if (menuTrail)
                    {
                        try
                        {
                            TrailRenderer trail = menu.AddComponent<TrailRenderer>();

                            trail.startColor = backgroundColor.GetColor(0);
                            trail.endColor = backgroundColor.GetColor(1);
                            trail.startWidth = 0.025f;
                            trail.endWidth = 0f;
                            trail.minVertexDistance = 0.05f;

                            if (smoothLines)
                            {
                                trail.numCapVertices = 10;
                                trail.numCornerVertices = 5;
                            }

                            trail.material.shader = Shader.Find("Sprites/Default");
                            trail.time = 2f;
                        }
                        catch { }
                    }
                }


                Destroy(menu, 5f);
                menu = null;

                Destroy(reference);
                reference = null;
            }
            else
            {
                CoroutineManager.instance.StartCoroutine(ShrinkCoroutine());

                Destroy(reference);
                reference = null;
            }
        }

        private static void AddPageButtons()
        {
            ExtGradient Gradient = buttonColors[swapButtonColors ? 1 : 0];

            switch (pageButtonType)
            {
                case 1:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, thinMenu ? 0.9f : 1.3f, ButtonDistance * 0.8f),
                        new Vector3(0.56f, 0f, 0.28f - ButtonDistance * (buttonOffset - 2)),
                        new Vector3(0.56f, 0f, 0.28f - ButtonDistance * (buttonOffset - 1)),
                        new Vector3(0.064f, 0f, 0.109f - ButtonDistance * (buttonOffset - 2) / 2.55f),
                        new Vector3(0.064f, 0f, 0.109f - ButtonDistance * (buttonOffset - 1) / 2.55f),
                        Gradient
                    );
                    break;

                case 2:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, 0.2f, 0.9f),
                        new Vector3(0.56f, thinMenu ? 0.65f : 0.9f, 0f),
                        new Vector3(0.56f, thinMenu ? -0.65f : -0.9f, 0f),
                        new Vector3(0.064f, thinMenu ? 0.195f : 0.267f, 0f),
                        new Vector3(0.064f, thinMenu ? -0.195f : -0.267f, 0f),
                        Gradient
                    );
                    break;

                case 5:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, hidetitle ? 0.1f : 0.3f, 0.05f),
                        new Vector3(0.56f, (thinMenu ? 0.299f : 0.499f) + (hidetitle ? 0.1f : 0f), 0.355f + (hidetitle ? 0.1f : 0f)),
                        new Vector3(0.56f, (thinMenu ? -0.299f : -0.499f) - (hidetitle ? 0.1f : 0f), 0.355f + (hidetitle ? 0.1f : 0f)),
                        new Vector3(0.064f, (thinMenu ? 0.09f : 0.15f) + (hidetitle ? 0.035f : 0f), 0.135f + (hidetitle ? 0.0375f : 0f)),
                        new Vector3(0.064f, (thinMenu ? -0.09f : -0.15f) - (hidetitle ? 0.035f : 0f), 0.135f + (hidetitle ? 0.0375f : 0f)),
                        Gradient
                    );
                    break;

                case 6:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, 0.102f, 0.08f),
                        new Vector3(0.56f, thinMenu ? 0.450f : 0.7f, -0.58f),
                        new Vector3(0.56f, thinMenu ? 0.450f : 0.7f, -0.58f) - new Vector3(0f, 0.16f, 0f),
                        new Vector3(0.064f, thinMenu ? 0.35f / 2.6f : 0.54444444444f / 2.6f, -0.58f / 2.7f),
                        new Vector3(0.064f, thinMenu ? 0.35f / 2.6f : 0.54444444444f / 2.6f, -0.58f / 2.7f) - new Vector3(0f, 0.0475f, 0f),
                        Gradient,
                        new Vector2(0.03f, 0.03f)
                    );
                    break;
            }
        }

        private static void RenderPrompt()
        {
            TextMeshPro promptText = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<TextMeshPro>();
            promptText.font = activeFont;
            promptText.text = CurrentPrompt.Message;

            string promptImageUrl = ExtractPromptImage(CurrentPrompt.Message);
            if (promptImageUrl != null)
                promptText.text = promptText.text.Replace($"<{promptImageUrl}>", "");

            promptText.text = FollowMenuSettings(promptText.text);

            promptText.fontSize = 1;
            promptText.lineSpacing = 0.8f;
            promptText.AddComponent<TextColorChanger>().colors = textColors[0];

            promptText.richText = true;
            promptText.fontStyle = activeFontStyle;
            promptText.alignment = TextAlignmentOptions.Center;
            promptText.enableAutoSizing = true;
            promptText.fontSizeMin = 0;
            RectTransform component = promptText.GetComponent<RectTransform>();
            component.sizeDelta = new Vector2(0.28f, CurrentPrompt.IsText ? 0.25f : 0.28f);

            component.localPosition = new Vector3(0.06f, 0f, CurrentPrompt.IsText ? -0.025f : 0f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (promptImageUrl != null)
            {
                string fileName = promptImageUrl.Split("/")[^1];
                string fileExtension = GetFileExtension(fileName);

                Image promptImage = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Image>();

                component.sizeDelta = new Vector2(component.sizeDelta.x, 0.03f);
                component.localPosition = new Vector3(0.06f, 0f, 0.1f);

                if (promptMat == null)
                    promptMat = new Material(promptImage.material);

                promptImage.material = promptMat;

                RectTransform imageTransform = promptImage.GetComponent<RectTransform>();
                imageTransform.localPosition = Vector3.zero;
                imageTransform.sizeDelta = new Vector2(.2f, .2f);

                imageTransform.localPosition = new Vector3(0.06f, 0f, promptText.text.IsNullOrEmpty() ? 0f : -0.03f);
                imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                FollowMenuSettings(promptImage);

                switch (fileExtension)
                {
                    case "png":
                    case "jpg":
                        {
                            promptImage.material.SetTexture("_MainTex", LoadTextureFromURL(promptImageUrl, fileName));
                            promptImage.material.color = Color.white;
                        }
                        break;
                    case "mp4":
                    case "webm":
                    case "mov":
                        {
                            promptVideoPlayer = new GameObject("iiMenu_PromptVideoPlayer").AddComponent<VideoPlayer>();
                            promptVideoPlayer.playOnAwake = true;
                            promptVideoPlayer.isLooping = true;
                            promptVideoPlayer.url = promptImageUrl;

                            RenderTexture rt = new RenderTexture(192, 144, 0);
                            rt.Create();

                            promptVideoPlayer.targetTexture = rt;

                            promptImage.material = promptMat;
                            promptImage.material.color = Color.white;
                            promptImage.material.SetTexture("_MainTex", rt);
                        }
                        break;
                    case "mat":
                        {
                            promptImage.material = promptMaterial;
                            break;
                        }
                }
            }

            FollowMenuSettings(promptText);

            joystickButtonSelected %= 2;

            {
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(inTextInput && isKeyboardPc))
                    button.layer = 2;

                button.GetComponent<BoxCollider>().isTrigger = true;
                button.transform.parent = menu.transform;
                button.transform.rotation = Quaternion.identity;
                button.transform.localScale = new Vector3(0.09f, CurrentPrompt.DeclineText == null ? 0.9f : 0.4375f, 0.08f);
                button.transform.localPosition = new Vector3(0.56f, CurrentPrompt.DeclineText == null ? 0f : 0.2375f, -0.43f);

                button.AddComponent<Button>().relatedText = "Accept Prompt";

                if (lastClickedName != "Accept Prompt")
                {
                    ColorChanger colorChanger = button.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    if (joystickMenu && joystickButtonSelected == 0)
                    {
                        joystickSelectedButton = "Accept Prompt";

                        ExtGradient gradient = colorChanger.colors.Clone();
                        gradient.SetColor(0, Color.red);

                        colorChanger.colors = gradient;
                    }
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(0, button.GetComponent<Renderer>()));

                TextMeshPro text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<TextMeshPro>();
                text.font = activeFont;
                text.fontStyle = activeFontStyle;
                text.text = FollowMenuSettings(CurrentPrompt.AcceptText);
                text.fontSize = 1;
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 0;

                text.AddComponent<TextColorChanger>().colors = textColors[1];

                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.sizeDelta = new Vector2(0.2f, 0.03f);

                if (arrowType == 11)
                    textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y * 6f);

                if (NoAutoSizeText)
                    textRect.sizeDelta = new Vector2(9f, 0.015f);

                textRect.localPosition = new Vector3(0.064f, CurrentPrompt.DeclineText != null ? 0.075f : 0f, -0.16f);
                textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                FollowMenuSettings(text);
                FollowMenuSettings(button, true);
            }

            if (CurrentPrompt.DeclineText != null)
            {
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(inTextInput && isKeyboardPc))
                    button.layer = 2;

                button.GetComponent<BoxCollider>().isTrigger = true;
                button.transform.parent = menu.transform;
                button.transform.rotation = Quaternion.identity;
                button.transform.localScale = new Vector3(0.09f, 0.4375f, 0.08f);
                button.transform.localPosition = new Vector3(0.56f, -0.2375f, -0.43f);

                button.AddComponent<Button>().relatedText = "Decline Prompt";

                if (lastClickedName != "Decline Prompt")
                {
                    ColorChanger colorChanger = button.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    if (joystickMenu && joystickButtonSelected == 1)
                    {
                        joystickSelectedButton = "Decline Prompt";

                        ExtGradient gradient = colorChanger.colors.Clone();
                        gradient.SetColor(0, Color.red);

                        colorChanger.colors = gradient;
                    }
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(1, button.GetComponent<Renderer>()));

                TextMeshPro text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<TextMeshPro>();
                text.font = activeFont;
                text.fontStyle = activeFontStyle;
                text.text = FollowMenuSettings(CurrentPrompt.DeclineText);
                text.fontSize = 1;
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 0;

                text.AddComponent<TextColorChanger>().colors = textColors[1];

                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.sizeDelta = new Vector2(0.2f, 0.03f);

                if (arrowType == 11)
                    textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y * 6f);

                if (NoAutoSizeText)
                    textRect.sizeDelta = new Vector2(9f, 0.015f);

                textRect.localPosition = new Vector3(0.064f, -0.075f, -0.16f);
                textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                FollowMenuSettings(text);
                FollowMenuSettings(button, true);
            }
        }

        private static void CreatePageButtonPair(string prevButtonName, string nextButtonName, Vector3 buttonScale, Vector3 prevButtonPos, Vector3 nextButtonPos, Vector3 prevTextPos, Vector3 nextTextPos, ExtGradient color, Vector2? textSize = null)
        {
            AdvancedAddButton(prevButtonName, buttonScale, prevButtonPos, prevTextPos, color, textSize, 0);
            AdvancedAddButton(nextButtonName, buttonScale, nextButtonPos, nextTextPos, color, textSize, 1);
        }

        public static string ExtractPromptImage(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var match = Regex.Match(input, @"<(?<url>https?://[^>]+)>");
            if (match.Success)
                return match.Groups["url"].Value;

            return null;
        }

        private static GameObject AdvancedAddButton(string buttonName, Vector3 scale, Vector3 position, Vector3 textPosition, ExtGradient color, Vector2? textSize, int arrowIndex)
        {
            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);

            if (!UnityInput.Current.GetKey(KeyCode.Q) && !(inTextInput && isKeyboardPc))
                button.layer = 2;

            button.GetComponent<BoxCollider>().isTrigger = true;
            button.transform.parent = menu.transform;
            button.transform.rotation = Quaternion.identity;
            button.transform.localScale = scale;
            button.transform.localPosition = position;

            button.AddComponent<Button>().relatedText = buttonName;

            if (lastClickedName != buttonName)
            {
                ColorChanger colorChanger = button.AddComponent<ColorChanger>();
                colorChanger.colors = color;
            }
            else
                CoroutineManager.instance.StartCoroutine(ButtonClick(-99, button.GetComponent<Renderer>()));

            TextMeshPro text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<TextMeshPro>();
            text.font = activeFont;
            text.text = arrowTypes[arrowType][arrowIndex];
            text.fontSize = 1;
            text.alignment = TextAlignmentOptions.Center;
            text.enableAutoSizing = true;
            text.fontSizeMin = 0;
            text.spriteAsset = ButtonSpriteSheet;

            text.AddComponent<TextColorChanger>().colors = textColors[1];

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.sizeDelta = textSize ?? new Vector2(0.2f, 0.03f);

            if (arrowType == 11)
                textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y * 6f);

            if (NoAutoSizeText)
                textRect.sizeDelta = new Vector2(9f, 0.015f);

            textRect.localPosition = textPosition;
            textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            FollowMenuSettings(text);
            FollowMenuSettings(button, !swapButtonColors);

            return button;
        }

        /// <summary>
        /// Applies or removes an outline effect to the specified menu object, adjusting its appearance based on the
        /// provided state.
        /// </summary>
        /// <remarks>If the specified object is not a direct child of the menu, the outline effect is
        /// applied using a general method. For menu child objects, a new primitive is created and styled to visually
        /// represent the outline. The appearance of the outline may vary depending on the value of shouldBeEnabled and
        /// the configuration of the menu. This method does not modify the original object's geometry or
        /// components.</remarks>
        /// <param name="toOut">The GameObject to which the outline effect will be applied or removed. Must not be null.</param>
        /// <param name="shouldBeEnabled">A value indicating whether the outline effect should be enabled (<see langword="true"/>) or removed (<see
        /// langword="false"/>).</param>
        public static void OutlineMenuObject(GameObject toOut, bool shouldBeEnabled)
        {
            if (toOut.transform.parent != menu?.transform)
            {
                OutlineObject(toOut, shouldBeEnabled);
                return;
            }

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localPosition = toOut.transform.localPosition;
            gameObject.transform.localScale = toOut.transform.localScale + new Vector3(-0.01f, 0.01f, 0.0075f);

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[shouldBeEnabled ? 1 : 0];

            if (shouldRound)
                RoundMenuObject(gameObject, 0.024f);
        }

        /// <summary>
        /// Creates an outline effect around the specified GameObject to visually indicate its enabled or disabled
        /// state.
        /// </summary>
        /// <remarks>The outline is created as a separate GameObject, matching the position, rotation, and
        /// scale of the target object. The outline color reflects the enabled or disabled state as determined by the
        /// application's color settings. This method does not modify the original GameObject.</remarks>
        /// <param name="toOut">The GameObject to outline. Cannot be null.</param>
        /// <param name="shouldBeEnabled">A value indicating whether the outline should represent the enabled (<see langword="true"/>) or disabled
        /// (<see langword="false"/>) state.</param>
        public static void OutlineObject(GameObject toOut, bool shouldBeEnabled)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = toOut.transform.parent;
            gameObject.transform.parent = toOut.transform.parent;
            gameObject.transform.rotation = toOut.transform.rotation;
            gameObject.transform.localPosition = toOut.transform.localPosition;
            gameObject.transform.localScale = toOut.transform.localScale + new Vector3(0.005f, 0.005f, -0.001f);

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[shouldBeEnabled ? 1 : 0];
        }

        /// <summary>
        /// Adds a black outline effect to the specified UI canvas object.
        /// </summary>
        /// <remarks>This method adds a Unity UI Outline component to the target object's GameObject,
        /// configuring it with a thin black outline. If the object already has an Outline component, an additional one
        /// will be added, which may result in multiple outlines. The outline uses the graphic's alpha
        /// channel.</remarks>
        /// <param name="canvasObject">The UI element to which the outline effect will be applied. Must not be null.</param>
        public static void OutlineCanvasObject(MaskableGraphic canvasObject)
        {
            // Creds: libyyyreal for optimization tech
            Outline outline = canvasObject.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(0.001f, 0.001f);
            outline.useGraphicAlpha = true;
        }

        /// <summary>
        /// Replaces a menu object's appearance with a rounded version by constructing beveled edges and rounded corners
        /// around the specified GameObject.
        /// </summary>
        /// <remarks>If the specified GameObject is not a direct child of the menu, only a basic rounding
        /// operation is performed. When applied to a menu child, the method disables the original renderer and
        /// constructs new primitives to visually represent a rounded, beveled menu item. The method also ensures that
        /// color and transparency settings are preserved by copying relevant components.</remarks>
        /// <param name="toRound">The GameObject to be visually rounded. Must be a child of the menu GameObject to receive the full rounded
        /// treatment; otherwise, a standard rounding is applied.</param>
        /// <param name="Bevel">The width of the bevel applied to the edges and corners, in Unity units. Must be a positive value. The
        /// default is 0.02.</param>
        public static void RoundMenuObject(GameObject toRound, float Bevel = 0.02f)
        {
            if (toRound.transform.parent != menu?.transform)
            {
                RoundObject(toRound, Bevel);
                return;
            }

            Renderer ToRoundRenderer = toRound.GetComponent<Renderer>();
            GameObject BaseA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseA.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(BaseA.GetComponent<Collider>());

            BaseA.transform.parent = menu.transform;
            BaseA.transform.rotation = Quaternion.identity;
            BaseA.transform.localPosition = toRound.transform.localPosition;
            BaseA.transform.localScale = toRound.transform.localScale + new Vector3(0f, Bevel * -2.55f, 0f);

            GameObject BaseB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(BaseB.GetComponent<Collider>());

            BaseB.transform.parent = menu.transform;
            BaseB.transform.rotation = Quaternion.identity;
            BaseB.transform.localPosition = toRound.transform.localPosition;
            BaseB.transform.localScale = toRound.transform.localScale + new Vector3(0f, 0f, -Bevel * 2f);

            GameObject RoundCornerA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerA.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerA.GetComponent<Collider>());

            RoundCornerA.transform.parent = menu.transform;
            RoundCornerA.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerA.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, toRound.transform.localScale.y / 2f - Bevel * 1.275f, toRound.transform.localScale.z / 2f - Bevel);
            RoundCornerA.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerB.GetComponent<Collider>());

            RoundCornerB.transform.parent = menu.transform;
            RoundCornerB.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerB.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + Bevel * 1.275f, toRound.transform.localScale.z / 2f - Bevel);
            RoundCornerB.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerC.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerC.GetComponent<Collider>());

            RoundCornerC.transform.parent = menu.transform;
            RoundCornerC.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerC.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, toRound.transform.localScale.y / 2f - Bevel * 1.275f, -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerC.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerD.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerD.GetComponent<Collider>());

            RoundCornerD.transform.parent = menu.transform;
            RoundCornerD.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerD.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + Bevel * 1.275f, -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerD.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject[] ToChange = {
                BaseA,
                BaseB,
                RoundCornerA,
                RoundCornerB,
                RoundCornerC,
                RoundCornerD
            };

            foreach (GameObject Changed in ToChange)
            {
                ClampColor TargetChanger = Changed.AddComponent<ClampColor>();
                TargetChanger.targetRenderer = ToRoundRenderer;
            }

            ToRoundRenderer.enabled = false;

            ColorChanger colorChanger = ToRoundRenderer.GetComponent<ColorChanger>();
            if (colorChanger)
                colorChanger.overrideTransparency = false;
        }

        /// <summary>
        /// Replaces the specified GameObject with a visually rounded version by constructing a composite of primitive
        /// shapes with beveled edges.
        /// </summary>
        /// <remarks>This method disables the original object's Renderer and creates new child primitives
        /// to approximate a rounded appearance. The original object's ColorChanger component, if present, will have its
        /// overrideTransparency property set to false. The method does not modify the original object's collider or
        /// mesh, and is intended for visual effects only.</remarks>
        /// <param name="toRound">The GameObject to be visually rounded. Must have a Renderer component attached.</param>
        /// <param name="bevel">The width, in world units, of the bevel applied to the object's edges. Must be non-negative. The default
        /// value is 0.02.</param>
        public static void RoundObject(GameObject toRound, float bevel = 0.02f)
        {
            static GameObject CreatePrimitive(PrimitiveType type, Transform parent, bool rendererEnabled)
            {
                GameObject obj = GameObject.CreatePrimitive(type);
                obj.GetComponent<Renderer>().enabled = rendererEnabled;

                Collider collider = obj.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                obj.transform.SetParent(parent, false);
                return obj;
            }

            Renderer renderer = toRound.GetComponent<Renderer>();
            if (renderer == null) return;

            Transform parent = toRound.transform;
            Vector3 scale = parent.localScale;
            bool rendererEnabled = renderer.enabled;

            GameObject baseA = CreatePrimitive(PrimitiveType.Cube, parent, rendererEnabled);
            baseA.transform.localPosition = Vector3.zero;
            baseA.transform.localRotation = Quaternion.identity;
            baseA.transform.localScale = new Vector3(scale.x, scale.y - bevel * 2f, scale.z);

            GameObject baseB = CreatePrimitive(PrimitiveType.Cube, parent, rendererEnabled);
            baseB.transform.localPosition = Vector3.zero;
            baseB.transform.localRotation = Quaternion.identity;
            baseB.transform.localScale = new Vector3(scale.x, scale.y, scale.z - bevel * 2f);

            GameObject[] corners = new GameObject[4];
            Vector3[] cornerOffsets = {
                new Vector3(0f, scale.y / 2f - bevel, scale.z / 2f - bevel),
                new Vector3(0f, -scale.y / 2f + bevel, scale.z / 2f - bevel),
                new Vector3(0f, scale.y / 2f - bevel, -scale.z / 2f + bevel),
                new Vector3(0f, -scale.y / 2f + bevel, -scale.z / 2f + bevel)
            };

            for (int i = 0; i < 4; i++)
            {
                corners[i] = CreatePrimitive(PrimitiveType.Cylinder, parent, rendererEnabled);
                corners[i].transform.localPosition = cornerOffsets[i];
                corners[i].transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                corners[i].transform.localScale = new Vector3(bevel * 2f, scale.x / 2f, bevel * 2f);
            }

            GameObject[] allObjects = { baseA, baseB, corners[0], corners[1], corners[2], corners[3] };
            foreach (GameObject obj in allObjects)
            {
                ClampColor clampColor = obj.AddComponent<ClampColor>();
                clampColor.targetRenderer = renderer;
            }

            renderer.enabled = false;

            ColorChanger colorChanger = renderer.GetComponent<ColorChanger>();
            if (colorChanger != null)
                colorChanger.overrideTransparency = false;
        }

        public class PromptData
        {
            public bool IsText;
            public string Message;

            public string AcceptText;
            public string DeclineText;

            public Action AcceptAction;
            public Action DeclineAction;
        }

        public static List<PromptData> prompts = new List<PromptData>();
        public static PromptData CurrentPrompt
        {
            get 
            {
                if (prompts.Count > 0)
                    return prompts[0];
                else
                    return null;
            }
        }

        public static Material promptMaterial;

        /// <summary>
        /// Prompts the user with a message. They can choose to accept or deny it.
        /// </summary>
        /// <remarks>
        /// Prompts stack. If multiple prompts are added before the user responds to the first one, they will be shown in order.
        /// </remarks>
        /// <remarks>
        /// You may add images or videos to the prompt by encasing them in &lt;&gt; brackets. For example, "&lt;https://example.com/image.png&gt;" or "&lt;https://example.com/video.mp4&gt;"
        /// </remarks>
        /// <param name="Message">Title</param>
        /// <param name="Accept">Accept Action</param>
        /// <param name="Decline">Decline Action</param>
        /// <param name="AcceptButton">Accept Text</param>
        /// <param name="DeclineButton">Decline Text</param>
        public static void Prompt(string Message, Action Accept = null, Action Decline = null, string AcceptButton = "Yes", string DeclineButton = "No")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = Decline, AcceptText = AcceptButton, DeclineText = DeclineButton, IsText = false });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

        /// <summary>
        /// Prompts the user with a message. They can choose to accept it.
        /// </summary>
        /// <remarks>
        /// Prompts stack. If multiple prompts are added before the user responds to the first one, they will be shown in order.
        /// </remarks>
        /// <remarks>
        /// You may add images or videos to the prompt by encasing them in &lt;&gt; brackets. For example, "&lt;https://example.com/image.png&gt;" or "&lt;https://example.com/video.mp4&gt;"
        /// </remarks>
        /// <param name="Message">Title</param>
        /// <param name="Accept">Accept Action</param>
        /// <param name="AcceptButton">Accept Text</param>
        public static void PromptSingle(string Message, Action Accept = null, string AcceptButton = "Yes")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = null, AcceptText = AcceptButton, DeclineText = null, IsText = false });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

        /// <summary>
        /// Prompts the user with a message. This allows for keyboard input from the user. They may choose to accept or deny the prompt. To use the user's input, use the "keyboardInput" variable.
        /// </summary>
        /// <remarks>
        /// Prompts stack. If multiple prompts are added before the user responds to the first one, they will be shown in order.
        /// </remarks>
        /// <remarks>
        /// You may add images or videos to the prompt by encasing them in &lt;&gt; brackets. For example, "&lt;https://example.com/image.png&gt;" or "&lt;https://example.com/video.mp4&gt;"
        /// </remarks>
        /// <param name="Message">Title</param>
        /// <param name="Accept">Accept Action</param>
        /// <param name="Decline">Decline Action</param>
        /// <param name="AcceptButton">Accept Text</param>
        /// <param name="DeclineButton">Decline Text</param>
        public static void PromptText(string Message, Action Accept = null, Action Decline = null, string AcceptButton = "Yes", string DeclineButton = "No")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = Decline, AcceptText = AcceptButton, DeclineText = DeclineButton, IsText = true });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

        /// <summary>
        /// Prompts the user with a message. This allows for keyboard input from the user. They may choose to accept the prompt. To use the user's input, use the "keyboardInput" variable.
        /// </summary>
        /// <remarks>
        /// Prompts stack. If multiple prompts are added before the user responds to the first one, they will be shown in order.
        /// </remarks>
        /// <remarks>
        /// You may add images or videos to the prompt by encasing them in &lt;&gt; brackets. For example, "&lt;https://example.com/image.png&gt;" or "&lt;https://example.com/video.mp4&gt;"
        /// </remarks>
        /// <param name="Message">Title</param>
        /// <param name="Accept">Accept Action</param>
        /// <param name="AcceptButton">Accept Text</param>
        public static void PromptSingleText(string Message, Action Accept = null, string AcceptButton = "Yes")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = null, AcceptText = AcceptButton, DeclineText = null, IsText = true });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

        private static string versionArchive;
        public static void UpdatePrompt(string newVersion = null)
        {
            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/Notifications/win7-exc.ogg", "Audio/Menu/Notifications/win7-exc.ogg"), buttonClickVolume / 10f);

            versionArchive ??= newVersion;
            Prompt($"A new version is available ({versionArchive}). Would you like to update?", Settings.UpdateMenu);
        }

        public static readonly Dictionary<(Color, Color), Texture2D> cacheGradients = new Dictionary<(Color, Color), Texture2D>();

        public static Texture2D GetGradientTexture(Color colorA, Color colorB)
        {
            var key = (colorA, colorB);
            if (cacheGradients.TryGetValue(key, out Texture2D cachedTexture))
                return cachedTexture;

            Texture2D txt2d = new Texture2D(128, 128);
            Color[] pixels = new Color[128 * 128];

            if (horizontalGradients)
            {
                for (int y = 0; y < 128; y++)
                {
                    Color rowColor = Color.Lerp(colorA, colorB, y / 128f);
                    for (int x = 0; x < 128; x++)
                        pixels[y * 128 + x] = rowColor;
                }
            } else
            {
                for (int i = 0; i < 128; i++)
                {
                    Color rowColor = Color.Lerp(colorA, colorB, i / 128f);
                    for (int j = 0; j < 128; j++)
                        pixels[j * 128 + i] = rowColor;
                }
            }

            txt2d.SetPixels(pixels);
            txt2d.wrapMode = TextureWrapMode.Mirror;

            txt2d.Apply();

            cacheGradients.Add(key, txt2d);
            return txt2d;
        }

        /// <summary>
        /// Flushes/sends any queued RPCs to the server to prevent disconnection from RPC limits.
        /// </summary>
        public static void RPCProtection()
        {
            if (!PhotonNetwork.InRoom)
                return;

            try
            {
                if (hasRemovedThisFrame) return;
                if (NoOverlapRPCs)
                    hasRemovedThisFrame = true;

                GorillaNot.instance.rpcErrorMax = int.MaxValue;
                GorillaNot.instance.rpcCallLimit = int.MaxValue;
                GorillaNot.instance.logErrorMax = int.MaxValue;

                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;

                PhotonNetwork.SendAllOutgoingCommands();
            } catch { LogManager.Log("RPC protection failed, are you in a lobby?"); }
        }

        /// <summary>
        /// Returns the given URL's raw output as a string.
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>Website Data</returns>
        public static string GetHttp(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = "";

            if (data == null) return html;
            using StreamReader sr = new StreamReader(data);
            html = sr.ReadToEnd();

            return html;
        }

        private static readonly List<float> volumeArchive = new List<float>();
        private static Vector3 GunPositionSmoothed = Vector3.zero;

        public static GameObject GunPointer;
        private static LineRenderer GunLine;

        /// <summary>
        /// Renders a gun pointer and line from the player's target gun position.
        /// </summary>
        /// <param name="overrideLayerMask">Layer Mask</param>
        /// <returns>Raycast and Pointer object</returns>
        public static (RaycastHit Ray, GameObject NewPointer) RenderGun(int? overrideLayerMask = null)
        {
            GunSpawned = true;
            Transform GunTransform = SwapGunHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;

            Vector3 StartPosition = GunTransform.position;
            Vector3 Direction = GunTransform.forward;

            Vector3 Up = -GunTransform.up;
            Vector3 Right = GunTransform.right;

            switch (GunDirection)
            {
                case 1:
                    Up = GunTransform.forward;
                    Direction = -GunTransform.up;
                    break;
                case 2:
                    Up = GunTransform.forward;
                    Right = -GunTransform.up;
                    Direction = GunTransform.right * (SwapGunHand ? 1f : -1f);
                    break;
                case 3:
                    var (_, _, up, forward, right) = SwapGunHand ? ControllerUtilities.GetTrueLeftHand() : ControllerUtilities.GetTrueRightHand();

                    Up = up;
                    Right = right;
                    Direction = forward;
                    break;
                case 4:
                    Up = GorillaTagger.Instance.headCollider.transform.up;
                    Right = GorillaTagger.Instance.headCollider.transform.right;
                    Direction = GorillaTagger.Instance.headCollider.transform.forward;
                    StartPosition = GorillaTagger.Instance.headCollider.transform.position + Up * 0.1f;
                    break;
            }

            if (GiveGunTarget != null)
            {
                GunTransform = SwapGunHand ? GiveGunTarget.leftHandTransform : GiveGunTarget.rightHandTransform;

                StartPosition = GunTransform.position;
                Direction = GunTransform.up;

                Up = GunTransform.forward;
                Right = GunTransform.right;
            }

            Physics.Raycast(StartPosition + Direction / 4f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f), Direction, out var Ray, 512f, overrideLayerMask ?? NoInvisLayerMask());
            if (shouldBePC)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
                Direction = ray.direction;
            }

            Vector3 EndPosition = gunLocked ? lockTarget.transform.position : Ray.point;

            if (EndPosition == Vector3.zero)
                EndPosition = StartPosition + Direction * 512f;

            if (SmoothGunPointer)
            {
                GunPositionSmoothed = Vector3.Lerp(GunPositionSmoothed, EndPosition, Time.deltaTime * 6f);
                EndPosition = GunPositionSmoothed;
            }

            if (GunPointer == null)
                GunPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            GunPointer.SetActive(true);
            GunPointer.transform.localScale = (smallGunPointer ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.2f, 0.2f, 0.2f)) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
            GunPointer.transform.position = EndPosition;

            Renderer PointerRenderer = GunPointer.GetComponent<Renderer>();
            PointerRenderer.material.shader = Shader.Find("GUI/Text Shader");
            PointerRenderer.material.color = gunLocked || GetGunInput(true) ? buttonColors[1].GetCurrentColor() : buttonColors[0].GetCurrentColor();

            if (disableGunPointer)
                PointerRenderer.enabled = false;

            if (GunParticles && (GetGunInput(true) || gunLocked))
            {
                GameObject Particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Particle.transform.position = EndPosition;
                Particle.transform.localScale = Vector3.one * (0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
                Particle.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                Particle.AddComponent<CustomParticle>();
                Destroy(Particle.GetComponent<Collider>());
            }

            Destroy(GunPointer.GetComponent<Collider>());

            if (!disableGunLine)
            {
                if (GunLine == null)
                {
                    GameObject line = new GameObject("iiMenu_GunLine");
                    GunLine = line.AddComponent<LineRenderer>();
                }

                GunLine.gameObject.SetActive(true);
                GunLine.material.shader = Shader.Find("GUI/Text Shader");
                GunLine.startColor = backgroundColor.GetCurrentColor();
                GunLine.endColor = backgroundColor.GetCurrentColor(0.5f);
                GunLine.startWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                GunLine.endWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                GunLine.useWorldSpace = true;
                if (smoothLines)
                {
                    GunLine.numCapVertices = 10;
                    GunLine.numCornerVertices = 5;
                }
                if (gunVariation != 9)
                {
                    GunLine.positionCount = 2;
                    GunLine.SetPosition(0, StartPosition);
                    GunLine.SetPosition(1, EndPosition);
                }
                int Step = GunLineQuality;
                switch (gunVariation)
                {
                    case 1: // Lightning
                        if (GetGunInput(true) || gunLocked)
                        {
                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, Position + (Random.Range(0f, 1f) > 0.75f ? new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) : Vector3.zero));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 2: // Wavy
                        if (GetGunInput(true) || gunLocked)
                        {
                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                float value = i / (float)Step * 50f;

                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, Position + Up * (Mathf.Sin(Time.time * -10f + value) * 0.1f));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 3: // Blocky
                        if (GetGunInput(true) || gunLocked)
                        {
                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, new Vector3(Mathf.Round(Position.x * 25f) / 25f, Mathf.Round(Position.y * 25f) / 25f, Mathf.Round(Position.z * 25f) / 25f));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 4: // Sinewave
                        Step = GunLineQuality / 2;

                        if (GetGunInput(true) || gunLocked)
                        {
                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, Position + Up * (Mathf.Sin(Time.time * 10f) * (i % 2 == 0 ? 0.1f : -0.1f)));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 5: // Spring
                        if (GetGunInput(true) || gunLocked)
                        {
                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                float value = i / (float)Step * 50f;

                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, Position + Right * (Mathf.Cos(Time.time * -10f + value) * 0.1f) + Up * (Mathf.Sin(Time.time * -10f + value) * 0.1f));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 6: // Bouncy
                        if (GetGunInput(true) || gunLocked)
                        {
                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                float value = i / (float)Step * 15f;
                                GunLine.SetPosition(i, Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f)) + Up * (Mathf.Abs(Mathf.Sin(Time.time * -10f + value)) * 0.3f));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 7: // Audio
                        if (GetGunInput(true) || gunLocked)
                        {
                            float audioSize = 0f;

                            if (gunLocked)
                            {
                                GorillaSpeakerLoudness targetRecorder = lockTarget.GetComponent<GorillaSpeakerLoudness>();
                                if (targetRecorder != null)
                                    audioSize += targetRecorder.Loudness * 3f;
                            }

                            GorillaSpeakerLoudness localRecorder = VRRig.LocalRig.GetComponent<GorillaSpeakerLoudness>();
                            if (localRecorder != null)
                                audioSize += localRecorder.Loudness * 3f;

                            volumeArchive.Insert(0, volumeArchive.Count == 0 ? 0 : audioSize - volumeArchive[0] * 0.1f);

                            if (volumeArchive.Count > Step)
                                volumeArchive.Remove(Step);

                            GunLine.positionCount = Step;
                            GunLine.SetPosition(0, StartPosition);

                            for (int i = 1; i < Step - 1; i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, Position + Up * ((i >= volumeArchive.Count ? 0 : volumeArchive[i]) * (i % 2 == 0 ? 1f : -1f)));
                            }

                            GunLine.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 8: // Bezier, credits to Crisp / Kman / Steal / Untitled One of those 4 I don't really know who
                        Vector3 BaseMid = Vector3.Lerp(StartPosition, EndPosition, 0.5f);

                        float angle = Time.time * 3f;
                        Vector3 wobbleOffset = Up * (Mathf.Sin(angle) * 0.15f) + Right * (Mathf.Cos(angle * 1.3f) * 0.15f);
                        Vector3 targetMid = BaseMid + wobbleOffset;

                        if (MidPosition == Vector3.zero) MidPosition = targetMid;

                        Vector3 force = (targetMid - MidPosition) * 40f;
                        MidVelocity += force * Time.deltaTime;
                        MidVelocity *= Mathf.Exp(-6f * Time.deltaTime);
                        MidPosition += MidVelocity * Time.deltaTime;

                        GunLine.positionCount = Step;
                        GunLine.SetPosition(0, StartPosition);

                        Vector3[] points = new Vector3[Step];
                        for (int i = 0; i < Step; i++)
                        {
                            float t = (float)i / (Step - 1);
                            points[i] = Mathf.Pow(1 - t, 2) * StartPosition +
                                        2 * (1 - t) * t * MidPosition +
                                        Mathf.Pow(t, 2) * EndPosition;
                        }

                        GunLine.positionCount = Step;
                        GunLine.SetPositions(points);
                        break;
                    case 9: // Rope
                        GunLine.positionCount = Step;

                        RopePhysics physics = GunLine.gameObject.GetComponent<RopePhysics>();
                        if (physics == null)
                        {
                            for (int i = 0; i < Step; i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                GunLine.SetPosition(i, Position);
                            }

                            physics = GunLine.gameObject.AddComponent<RopePhysics>();
                        }

                        physics.segmentLength = Vector3.Distance(StartPosition, EndPosition) / (Step - 1) * (GetGunInput(true) || gunLocked ? 1.1f : 1.2f);
                        physics.SetStartPosition(StartPosition);
                        physics.SetEndPosition(EndPosition);
                        break;
                }
            }

            return (Ray, GunPointer);
        }

        private static VRRig _giveGunTarget;
        public static VRRig GiveGunTarget
        {
            get
            {
                if (!GorillaParent.instance.vrrigs.Contains(_giveGunTarget))
                    _giveGunTarget = null;

                return _giveGunTarget;
            }
            set => _giveGunTarget = value;
        }

        /// <summary>
        /// Returns the gun input state.
        /// </summary>
        /// <param name="isShooting">Trigger</param>
        /// <returns>Holding Button</returns>
        public static bool GetGunInput(bool isShooting)
        {
            if (GiveGunTarget != null)
            {
                if (isShooting)
                    return TriggerlessGuns || (SwapGunHand ? GiveGunTarget.leftIndex.calcT > 0.5f : GiveGunTarget.rightIndex.calcT > 0.5f);
                return GriplessGuns || (SwapGunHand ? GiveGunTarget.leftMiddle.calcT > 0.5f : GiveGunTarget.rightMiddle.calcT > 0.5f);
            }

            if (isShooting)
                return TriggerlessGuns || (SwapGunHand ? leftTrigger > 0.5f : rightTrigger > 0.5f) || Mouse.current.leftButton.isPressed;
            return GriplessGuns || (SwapGunHand ? leftGrab : rightGrab) || (HardGunLocks && gunLocked && !rightSecondary) || Mouse.current.rightButton.isPressed;
        }

        /// <summary>
        /// Returns the gun direction vector based on the specified transform and the current GunDirection setting.
        /// </summary>
        /// <param name="transform">The transform used to determine the gun's orientation.</param>
        /// <returns>A Vector3 representing the selected gun direction.</returns>
        public static Vector3 GetGunDirection(Transform transform) =>
            new[] { transform.forward, - transform.up, transform == GorillaTagger.Instance.rightHandTransform ? ControllerUtilities.GetTrueRightHand().forward : ControllerUtilities.GetTrueLeftHand().forward, GorillaTagger.Instance.headCollider.transform.forward } [GunDirection];

        /// <summary>
        /// Generates a text-to-speech audio clip from the provided text using various TTS services and invokes a
        /// callback with the resulting AudioClip.
        /// </summary>
        /// <param name="text">The text to be converted to speech.</param>
        /// <param name="onComplete">Callback invoked with the generated AudioClip or null if the operation fails.</param>
        /// <param name="customFileName">Optional custom file name for the generated audio file.</param>
        /// <param name="customPath">Optional custom directory path for storing the generated audio file.</param>
        /// <returns>An enumerator for coroutine-based asynchronous execution.</returns>
        public static IEnumerator TranscribeText(string text, Action<AudioClip> onComplete, string customFileName = null, string customPath = null)
        {
            if (Time.time < timeMenuStarted + 5f)
            {
                onComplete?.Invoke(null);
                yield break;
            }

            string fileName = !string.IsNullOrEmpty(customPath) ? SanitizeFileName(customFileName) : $"{GetSHA256(text)}{(narratorIndex == 0 ? ".wav" : ".mp3")}";
            string directoryPath = !string.IsNullOrEmpty(customPath) ? customPath : $"{PluginInfo.BaseDirectory}/TTS{(narratorName == "Default" ? "" : narratorName)}";
            string filePath = directoryPath + "/" + fileName;

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!File.Exists(filePath))
            {
                switch (narratorIndex)
                {
                    // My endpoint
                    case 0:
                        {
                            string postData = JsonConvert.SerializeObject(new { text });

                            using UnityWebRequest request = new UnityWebRequest($"{PluginInfo.ServerAPI}/tts", "POST");
                            byte[] raw = Encoding.UTF8.GetBytes(postData);

                            request.uploadHandler = new UploadHandlerRaw(raw);
                            request.SetRequestHeader("Content-Type", "application/json");
                            request.downloadHandler = new DownloadHandlerBuffer();

                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error downloading TTS: " + request.error);
                                onComplete?.Invoke(null);
                                yield break;
                            }

                            byte[] response = request.downloadHandler.data;
                            File.WriteAllBytes(filePath, response);

                            break;
                        }

                    // Streamlabs TTS voices
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        {
                            if (text.Length > 550)
                                text = text[..550];

                            using UnityWebRequest request = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=Streamlabs&voice=" + narratorName + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
                            request.downloadHandler = new DownloadHandlerBuffer();
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error getting TTS: " + request.error);
                                onComplete?.Invoke(null);
                                yield break;
                            }

                            string jsonResponse = request.downloadHandler.text;
                            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                            using UnityWebRequest dataRequest = UnityWebRequest.Get(responseData["audio_url"].ToString().Replace("\\", ""));
                            yield return dataRequest.SendWebRequest();

                            if (dataRequest.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + responseData["audio_url"]);
                            else
                                File.WriteAllBytes(filePath, dataRequest.downloadHandler.data);

                            break;
                        }

                    // Tiktok TTS Voices
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                        {
                            Dictionary<int, string> voiceCodenames = new Dictionary<int, string>
                            {
                                { 9, "en_us_001" },
                                { 10, "en_female_grandma" },
                                { 11, "en_male_grinch" },
                                { 12, "en_male_ukneighbor" },
                                { 13, "en_us_ghostface" },
                                { 14, "en_female_zombie" },
                                { 15, "en_male_narration" },
                                { 16, "en_male_pirate" },
                                { 17, "en_male_m03_sunshine_soon" },
                                { 18, "en_us_006" },
                                { 19, "en_male_david_gingerman" },
                                { 20, "en_male_chris" },
                                { 21, "en_male_sing_funny_thanksgiving" },
                                { 22, "en_male_santa_effect" }
                            };

                            if (text.Length > 300)
                                text = text[..300];

                            using UnityWebRequest request = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=TikTok&voice=" + voiceCodenames[narratorIndex] + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
                            request.downloadHandler = new DownloadHandlerBuffer();
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error getting TTS: " + request.error);
                                onComplete?.Invoke(null);
                                yield break;
                            }

                            string jsonResponse = request.downloadHandler.text;
                            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                            using UnityWebRequest dataRequest = UnityWebRequest.Get(responseData["audio_url"].ToString().Replace("\\", ""));
                            yield return dataRequest.SendWebRequest();

                            if (dataRequest.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + responseData["audio_url"]);
                            else
                                File.WriteAllBytes(filePath, dataRequest.downloadHandler.data);

                            break;
                        }

                    case 23:
                    case 24:
                        {
                            Dictionary<int, string> voiceCodenames = new Dictionary<int, string>
                            {
                                { 23, "en-us" },
                                { 24, "en-gb" }
                            };

                            if (text.Length > 300)
                                text = text[..300];

                            using UnityWebRequest request = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=Google%20Translate&voice=" + voiceCodenames[narratorIndex] + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
                            request.downloadHandler = new DownloadHandlerBuffer();
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error getting TTS: " + request.error);
                                onComplete?.Invoke(null);
                                yield break;
                            }

                            string jsonResponse = request.downloadHandler.text;
                            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                            using UnityWebRequest dataRequest = UnityWebRequest.Get(responseData["audio_url"].ToString().Replace("\\", ""));
                            yield return dataRequest.SendWebRequest();

                            if (dataRequest.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + responseData["audio_url"]);
                            else
                                File.WriteAllBytes(filePath, dataRequest.downloadHandler.data);

                            break;
                        }

                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                        {
                            Dictionary<int, string> voiceCodenames = new Dictionary<int, string>
                            {
                                { 25, "Dog" },
                                { 26, "Jerkface" },
                                { 27, "Robot" },
                                { 28, "Vlad" },
                                { 29, "Obama" }
                            };

                            if (text.Length > 300)
                                text = text[..300];

                            using UnityWebRequest request = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=VoiceForge&voice=" + voiceCodenames[narratorIndex] + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
                            request.downloadHandler = new DownloadHandlerBuffer();
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error getting TTS: " + request.error);
                                onComplete?.Invoke(null);
                                yield break;
                            }

                            string jsonResponse = request.downloadHandler.text;
                            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                            using UnityWebRequest dataRequest = UnityWebRequest.Get(responseData["audio_url"].ToString().Replace("\\", ""));
                            yield return dataRequest.SendWebRequest();

                            if (dataRequest.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + responseData["audio_url"]);
                            else
                                File.WriteAllBytes(filePath, dataRequest.downloadHandler.data);

                            break;
                        }
                    case 30: // mommy asmr
                        {
                            using UnityWebRequest tokenRequest = UnityWebRequest.Get("https://iidk.online/gpt");
                            yield return tokenRequest.SendWebRequest();

                            if (tokenRequest.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error fetching puter token: " + tokenRequest.error);
                                yield break;
                            }

                            string tokenResponse = tokenRequest.downloadHandler.text;
                            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, object>>(tokenResponse);

                            if (!tokenData.TryGetValue("token", out var tokenObj))
                            {
                                LogManager.LogError("No token returned for puter");
                                yield break;
                            }

                            string authToken = tokenObj.ToString();
                            if (Settings.debugDictation)
                                LogManager.Log($"Puter token: {authToken}");

                            if (text.Length > 300)
                                text = text[..300];

                            var payload = new
                            {
                                @interface = "puter-tts",
                                driver = "elevenlabs-tts",
                                method = "synthesize",
                                args = new
                                {
                                    text,
                                    provider = "elevenlabs",
                                    voice = "j05EIz3iI3JmBTWC3CsA",
                                    model = "eleven_multilingual_v2",
                                    output_format = "mp3_44100_128"
                                },
                                auth_token = authToken,
                                test_mode = false
                            };

                            string jsonPayload = JsonConvert.SerializeObject(payload);

                            using UnityWebRequest ttsRequest = new UnityWebRequest("https://api.puter.com/drivers/call", "POST");
                            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                            ttsRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                            ttsRequest.downloadHandler = new DownloadHandlerBuffer();
                            ttsRequest.SetRequestHeader("Content-Type", "application/json");
                            ttsRequest.SetRequestHeader("Origin", "https://puter.com");

                            yield return ttsRequest.SendWebRequest();

                            if (ttsRequest.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error getting TTS: " + ttsRequest.error);
                                yield break;
                            }

                            File.WriteAllBytes(filePath, ttsRequest.downloadHandler.data);
                            break;
                        }

                }
            }

            onComplete?.Invoke(LoadSoundFromFile($"{directoryPath[$"{PluginInfo.BaseDirectory}/".Length..]}/{fileName}"));
        }

        /// <summary>
        /// Initiates narration of the specified text by transcribing it to audio and playing it with adjusted volume.
        /// </summary>
        /// <param name="text">The text to be narrated.</param>
        public static void NarrateText(string text) =>
            CoroutineManager.instance.StartCoroutine(TranscribeText(text, (audio) => Play2DAudio(audio, buttonClickSound / 10f)));

        /// <summary>
        /// Initiates narration of the specified text by transcribing it to audio and playing it through your microphone.
        /// </summary>
        /// <param name="text">The text to be narrated.</param>
        public static void SpeakText(string text) =>
            CoroutineManager.instance.StartCoroutine(TranscribeText(text, (audio) => Sound.PlayAudio(audio)));

        public static bool isAdmin;
        public static void SetupAdminPanel(string playername)
        {
            List<ButtonInfo> buttons = Buttons.buttons[0].ToList();
            buttons.Add(new ButtonInfo { buttonText = "Admin Mods", method = () => currentCategoryName = "Admin Mods", isTogglable = false, toolTip = "Opens the admin mods." });
            Buttons.buttons[0] = buttons.ToArray();
            NotificationManager.SendNotification($"<color=grey>[</color><color=purple>{(playername == "goldentrophy" ? "OWNER" : "ADMIN")}</color><color=grey>]</color> Welcome, {playername}! Admin mods have been enabled.", 10000);
            isAdmin = true;
        }

        public static string[] InfosToStrings(ButtonInfo[] array) =>
            array.Select(button => button.buttonText).ToArray();

        public static ButtonInfo[] StringsToInfos(string[] array) =>
            array.Select(Buttons.GetIndex).ToArray();

        public static string[] Alphabetize(string[] array)
        {
            if (array.Length <= 1)
                return array;

            string first = array[0];
            string[] others = array.Skip(1).OrderBy(s => s).ToArray();
            return new[] { first }.Concat(others).ToArray();
        }

        public static string[] AlphabetizeNoSkip(string[] array)
        {
            if (array.Length <= 1)
                return array;

            string[] others = array.OrderBy(s => s).ToArray();
            return others.ToArray();
        }

        public static IEnumerator GrowCoroutine()
        {
            GameObject menuObject = menu;

            float elapsedTime = 0f;
            Vector3 target = scaleWithPlayer ? new Vector3(0.1f, 0.3f, 0.3825f) * (menuScale * GTPlayer.Instance.scale) : new Vector3(0.1f, 0.3f, 0.3825f) * menuScale;
            while (elapsedTime < (slowDynamicAnimations ? 0.1f : 0.05f))
            {
                if (menuObject == null)
                    yield break;

                menuObject.transform.localScale = Vector3.Lerp(Vector3.zero, target, elapsedTime / (slowDynamicAnimations ? 0.1f : 0.05f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (menuObject == null)
                yield break;

            menuObject.transform.localScale = target;
        }

        public static IEnumerator ShrinkCoroutine()
        {
            Transform menuTransform = menu.transform;
            menu = null;

            Vector3 before = menuTransform.localScale;
            float elapsedTime = 0f;
            while (elapsedTime < (slowDynamicAnimations ? 0.1f : 0.05f))
            {
                menuTransform.localScale = Vector3.Lerp(before, Vector3.zero, elapsedTime / (slowDynamicAnimations ? 0.1f : 0.05f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(menuTransform.gameObject);
        }

        public static IEnumerator ButtonClick(int buttonIndex, Renderer render)
        {
            lastClickedName = "";
            float elapsedTime = 0f;
            while (elapsedTime < 0.1f)
            {
                int from = buttonIndex < 0 && swapButtonColors ? 0 : 1;
                int to = 1 - from;

                render.material.color = Color.Lerp(
                    buttonColors[from].GetCurrentColor(),
                    buttonColors[to].GetCurrentColor(),
                    elapsedTime / 0.1f
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ColorChanger colorChanger = render.gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[buttonIndex < 0 && swapButtonColors ? 1 : 0];

            if (joystickMenu && buttonIndex == joystickButtonSelected)
            {
                ExtGradient gradient = colorChanger.colors.Clone();
                gradient.SetColor(0, Color.red);

                colorChanger.colors = gradient;
            }
        }

        public static Dictionary<string, SnowballThrowable> snowballDict;

        /// <summary>
        /// Retrieves a SnowballThrowable instance by its projectile name.
        /// </summary>
        /// <param name="projectileName">The name of the projectile to retrieve.</param>
        /// <returns>The matching SnowballThrowable instance if found; otherwise, null.</returns>
        public static SnowballThrowable GetProjectile(string projectileName)
        {
            if (snowballDict == null)
            {
                if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
                    return null;

                snowballDict = new Dictionary<string, SnowballThrowable>();

                foreach (SnowballMaker Maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
                {
                    foreach (SnowballThrowable Throwable in Maker.snowballs)
                    {
                        try
                        {
                            snowballDict.Add(Throwable.transform.parent.gameObject.name, Throwable);
                        }
                        catch { }
                    }
                }
            }

            projectileName += "(Clone)";

            if (snowballDict != null && snowballDict.TryGetValue(projectileName, out var projectile))
                return projectile;
            return null;
        }

        public static readonly Dictionary<Type, object[]> typePool = new Dictionary<Type, object[]>();
        private static readonly Dictionary<Type, float> receiveTypeDelay = new Dictionary<Type, float>();

        /// <summary>
        /// Gets all objects of type <typeparamref name="T"/> in the scene using
        /// <see cref="UnityEngine.Object.FindObjectsByType{T}
        /// (UnityEngine.FindObjectsInactive, UnityEngine.FindObjectsSortMode)"/>
        /// with caching to reduce performance overhead.
        /// </summary>
        /// <typeparam name="T">The Unity object type to find</typeparam>
        /// <param name="decayTime">How long the cached results remain valid</param>
        /// <returns>An array of all instances of <typeparamref name="T"/></returns>
        public static T[] GetAllType<T>(float decayTime = 5f) where T : Object
        {
            Type type = typeof(T);

            float lastReceivedTime = receiveTypeDelay.GetValueOrDefault(type, -1f);

            if (Time.time > lastReceivedTime)
            {
                typePool.Remove(type);
                receiveTypeDelay[type] = Time.time + decayTime;
            }

            if (!typePool.ContainsKey(type))
                typePool.Add(type, FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None));

            return (T[])typePool[type];
        }

        private static float randomIndex;
        private static float randomDecayTime;

        /// <summary>
        /// Returns a random instance of <typeparamref name="T"/> from the scene,
        /// using the cached results from <see cref="GetAllType{T}(float)"/>.
        /// </summary>
        /// <typeparam name="T">The Unity object type to retrieve</typeparam>
        /// <param name="decayTime">How long the random selection remains unchanged</param>
        /// <returns>A random instance of <typeparamref name="T"/></returns>
        public static T GetRandomType<T>(float decayTime = 0f) where T : Object
        {
            T[] allOfType = GetAllType<T>();

            if (Time.time > randomDecayTime)
            {
                randomIndex = Random.Range(0f, 1f);
                randomDecayTime = Time.time + decayTime;
            }

            return allOfType[(int)(randomIndex * allOfType.Length)];
        }

        /// <summary>
        /// Clears the cached results created by <see cref="GetAllType{T}(float)"/>
        /// for the specified type.
        /// </summary>
        /// <typeparam name="T">The Unity object type to clear from the cache</typeparam>
        public static void ClearType<T>() where T : Object
        {
            Type type = typeof(T);

            typePool.Remove(type);
        }

        private static readonly Dictionary<string, GameObject> objectPool = new Dictionary<string, GameObject>();

        /// <summary>
        /// Retrieves a GameObject by name, using a cache to avoid repeated scene searches.
        /// </summary>
        /// <param name="find">
        /// The name of the GameObject to locate in the scene.
        /// </param>
        /// <returns>
        /// The found GameObject if it exists; otherwise, null.
        /// </returns>
        /// <remarks>
        /// The method first checks an internal object pool for a cached reference.
        /// If not found, it falls back to <see cref="GameObject.Find"> and caches the result
        /// for future lookups. This reduces the performance cost of repeated
        /// <see cref="GameObject.Find"/> calls.
        /// </remarks>
        public static GameObject GetObject(string find)
        {
            if (objectPool.TryGetValue(find, out GameObject go))
                return go;

            GameObject tgo = GameObject.Find(find);
            if (tgo != null)
                objectPool.Add(find, tgo);

            return tgo;
        }

        public static bool ShouldBypassChecks(NetPlayer Player) =>
             Player == NetworkSystem.Instance.LocalPlayer || FriendManager.IsPlayerFriend(Player) || ServerData.Administrators.ContainsKey(Player.UserId);

        [Obsolete("PlayerIsTagged is obsolete. Use VRRigExtensions.IsTagged instead.")]
        public static bool PlayerIsTagged(VRRig Player) =>
            Player.IsTagged();

        [Obsolete("PlayerIsLocal is obsolete. Use VRRigExtensions.IsLocal instead.")]
        public static bool PlayerIsLocal(VRRig Player) =>
            Player.IsLocal();

        [Obsolete("PlayerIsSteam is obsolete. Use VRRigExtensions.IsSteam instead.")]
        public static bool PlayerIsSteam(VRRig Player) =>
            Player.IsSteam();

        [Obsolete("GetPlayerColor is obsolete. Use VRRigExtensions.GetColor instead.")]
        public static Color GetPlayerColor(VRRig Player) =>
            Player.GetColor();

        [Obsolete("TrueLeftHand is obsolete. Use ControllerUtilities.GetTrueLeftHand instead.")]
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand() =>
            ControllerUtilities.GetTrueLeftHand();

        [Obsolete("TrueRightHand is obsolete. Use ControllerUtilities.GetTrueRightHand instead.")]
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand() =>
            ControllerUtilities.GetTrueRightHand();

        /// <summary>
        /// Converts a position from world coordinates to the player's local coordinate space.
        /// </summary>
        /// <remarks>This method is useful for determining the position of an object or point in relation
        /// to the player's current location and orientation within the game world.</remarks>
        /// <param name="world">The position in world coordinates to convert.</param>
        /// <returns>A <see cref="Vector3"/> representing the input position relative to the player's local coordinate system.</returns>
        public static Vector3 World2Player(Vector3 world) =>
            world - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position;

        /// <summary>
        /// Sets the world-scale of a GameObject to a specified value, 
        /// taking into account the scale of its parent.
        /// </summary>
        /// <param name="obj">The GameObject whose scale will be adjusted.</param>
        /// <param name="targetWorldScale">
        /// The desired scale of the GameObject in world space.
        /// Each component (x, y, z) represents the target size along that axis.
        /// </param>
        /// <remarks>
        /// This method adjusts the localScale of the GameObject so that its
        /// effective world scale matches the targetWorldScale, regardless of
        /// the parent's scale. It uses the parent's lossyScale to calculate
        /// the required localScale.
        /// </remarks>
        public static void WorldScale(GameObject obj, Vector3 targetWorldScale)
        {
            Vector3 parentScale = obj.transform.parent.lossyScale;
            obj.transform.localScale = new Vector3(targetWorldScale.x / parentScale.x, targetWorldScale.y / parentScale.y, targetWorldScale.z / parentScale.z);
        }

        public static void FixStickyColliders(GameObject platform)
        {
            Vector3[] localPositions = {
                new Vector3(0, 1f, 0),
                new Vector3(0, -1f, 0),
                new Vector3(1f, 0, 0),
                new Vector3(-1f, 0, 0),
                new Vector3(0, 0, 1f),
                new Vector3(0, 0, -1f)
            };
            Quaternion[] localRotations = {
                Quaternion.Euler(90, 0, 0),
                Quaternion.Euler(-90, 0, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.identity,
                Quaternion.Euler(0, 180, 0)
            };
            for (int i = 0; i < localPositions.Length; i++)
            {
                GameObject side = GameObject.CreatePrimitive(PrimitiveType.Cube);
                float size = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                side.transform.SetParent(platform.transform);
                side.transform.position = localPositions[i] * (size / 2);
                side.transform.rotation = localRotations[i];
                WorldScale(side, new Vector3(size, size, 0.01f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f)));
                side.GetComponent<Renderer>().enabled = false;
            }
        }

        public static GameObject audioManager;

        /// <summary>
        /// Plays a 2D audio clip at the specified volume using a singleton audio manager.
        /// </summary>
        /// <param name="sound">The audio clip to play.</param>
        /// <param name="volume">The volume at which to play the audio clip. Defaults to 1f.</param>
        public static void Play2DAudio(AudioClip sound, float volume = 1f)
        {
            if (audioManager == null)
            {
                audioManager = new GameObject("2DAudioMgr");
                AudioSource temp = audioManager.AddComponent<AudioSource>();
                temp.spatialBlend = 0f;
            }
            AudioSource ausrc = audioManager.GetComponent<AudioSource>();
            ausrc.volume = volume;
            ausrc.PlayOneShot(sound);
        }

        /// <summary>
        /// Plays an audio clip at a specified world position with configurable volume and spatial blend.
        /// </summary>
        /// <param name="sound">The audio clip to play.</param>
        /// <param name="position">The world position where the audio should be played.</param>
        /// <param name="volume">The volume at which to play the audio clip. Defaults to 1f.</param>
        /// <param name="spatialBlend">The spatial blend value for 3D audio. Defaults to 1f.</param>
        public static void PlayPositionAudio(AudioClip sound, Vector3 position, float volume = 1f, float spatialBlend = 1f)
        {
            GameObject audioManager = new GameObject("AudioMgr");
            audioManager.transform.position = position;

            AudioSource temp = audioManager.AddComponent<AudioSource>();
            temp.spatialBlend = spatialBlend;

            AudioSource ausrc = audioManager.GetComponent<AudioSource>();
            ausrc.volume = volume;
            ausrc.PlayOneShot(sound);

            Destroy(audioManager, sound.length);
        }

        public static GameObject handAudioManager;

        /// <summary>
        /// Plays a looping audio clip at the specified volume from either the left or right hand in the VR rig.
        /// </summary>
        /// <param name="sound">The audio clip to play.</param>
        /// <param name="volume">The playback volume for the audio clip.</param>
        /// <param name="left">True to play audio from the left hand; false for the right hand.</param>
        public static void PlayHandAudio(AudioClip sound, float volume, bool left)
        {
            if (handAudioManager == null)
            {
                handAudioManager = new GameObject("2DAudioMgr-hand");
                AudioSource temp = handAudioManager.AddComponent<AudioSource>();
                temp.spatialBlend = 1f;
                temp.rolloffMode = AudioRolloffMode.Logarithmic;
                temp.minDistance = 1f;
                temp.maxDistance = 15f;
                temp.spatialize = true;
            }
            handAudioManager.transform.SetParent(left ? VRRig.LocalRig.leftHandPlayer.gameObject.transform : VRRig.LocalRig.rightHandPlayer.gameObject.transform, false);

            AudioSource ausrc = handAudioManager.GetComponent<AudioSource>();
            ausrc.volume = volume;
            ausrc.clip = sound;
            ausrc.loop = true;
            ausrc.Play();
        }

        public static string ToTitleCase(string text) =>
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());

        /// <summary>
        /// Applies configured menu text transformations to the specified input string, including optional translation
        /// and case adjustments.
        /// </summary>
        /// <remarks>The transformations applied depend on the current values of the menu settings, such
        /// as translation, lowercase, and uppercase modes. If both lowercase and uppercase modes are enabled, the
        /// uppercase transformation takes precedence.</remarks>
        /// <param name="input">The input string to be processed and transformed according to the current menu settings.</param>
        /// <param name="translateText">true to translate the input string before applying case transformations; otherwise, false. The default is
        /// true.</param>
        /// <returns>A string containing the transformed input, with translation and case modifications applied as specified by
        /// the current settings.</returns>
        public static string FollowMenuSettings(string input, bool translateText = true)
        {
            if (translateText && translate)
                input = TranslationManager.TranslateText(input);

            if (lowercaseMode)
                input = input.ToLower();

            if (uppercaseMode)
                input = input.ToUpper();

            if (redactText)
            {
                var result = new char[input.Length];

                for (int i = 0; i < input.Length; i++)
                    result[i] = input[i] == ' ' ? ' ' : '█';

                input = new string(result);
            }

            return input;
        }

        /// <summary>
        /// Applies menu appearance settings to the specified game object, such as outlining and rounding, based on
        /// current configuration.
        /// </summary>
        /// <param name="gameObject">The game object to which the menu appearance settings will be applied.</param>
        /// <param name="shouldBeEnabled">A value indicating whether outlining should be enabled for the game object. The default is <see
        /// langword="true"/>.</param>
        public static void FollowMenuSettings(GameObject gameObject, bool shouldBeEnabled = true)
        {
            if (shouldOutline)
                OutlineMenuObject(gameObject, shouldBeEnabled);

            if (shouldRound)
                RoundMenuObject(gameObject);
        }

        /// <summary>
        /// Applies menu appearance settings to the specified canvas object, such as outlining, based on
        /// current configuration.
        /// </summary>
        /// <param name="tmp">The canvas object to which the menu appearance settings will be applied.</param>
        public static void FollowMenuSettings(TextMeshPro tmp)
        {
            if (tmp == null)
                return;

            float targetSpacing = -8f + characterDistance;
            if (redactText)
                targetSpacing -= 3f;

            if (!Mathf.Approximately(tmp.characterSpacing, targetSpacing))
                tmp.characterSpacing = targetSpacing;

            if (outlineText)
            {
                const float outlineWidth = 0.2f;

                if (!Mathf.Approximately(tmp.outlineWidth, outlineWidth))
                    tmp.outlineWidth = outlineWidth;

                if (tmp.outlineColor != Color.black)
                    tmp.outlineColor = Color.black;
            }
            else if (!Mathf.Approximately(tmp.outlineWidth, 0f))
                tmp.outlineWidth = 0f;

            FontStyles targetStyle = tmp.fontStyle;

            if (underlineText)
                targetStyle |= FontStyles.Underline;

            if (smallCapsText)
                targetStyle |= FontStyles.SmallCaps;

            if (strikethroughText)
                targetStyle |= FontStyles.Strikethrough;

            if (tmp.fontStyle != targetStyle)
                tmp.fontStyle = targetStyle;
        }

        /// <summary>
        /// Applies menu appearance settings to the specified canvas object, such as outlining, based on
        /// current configuration.
        /// </summary>
        /// <param name="tmp">The canvas object to which the menu appearance settings will be applied.</param>
        public static void FollowMenuSettings(TextMeshProUGUI tmp)
        {
            if (tmp == null)
                return;

            float targetSpacing = -8f + characterDistance;
            if (redactText)
                targetSpacing -= 3f;

            if (!Mathf.Approximately(tmp.characterSpacing, targetSpacing))
                tmp.characterSpacing = targetSpacing;

            if (outlineText)
            {
                const float outlineWidth = 0.2f;

                if (!Mathf.Approximately(tmp.outlineWidth, outlineWidth))
                    tmp.outlineWidth = outlineWidth;

                if (tmp.outlineColor != Color.black)
                    tmp.outlineColor = Color.black;
            }
            else if (!Mathf.Approximately(tmp.outlineWidth, 0f))
                tmp.outlineWidth = 0f;

            FontStyles targetStyle = tmp.fontStyle;

            if (underlineText)
                targetStyle |= FontStyles.Underline;

            if (smallCapsText)
                targetStyle |= FontStyles.SmallCaps;

            if (strikethroughText)
                targetStyle |= FontStyles.Strikethrough;

            if (tmp.fontStyle != targetStyle)
                tmp.fontStyle = targetStyle;
        }

        /// <summary>
        /// Applies menu appearance settings to the specified canvas object, such as outlining, based on
        /// current configuration.
        /// </summary>
        /// <param name="canvasObject">The canvas object to which the menu appearance settings will be applied.</param>
        public static void FollowMenuSettings(MaskableGraphic canvasObject)
        {
            if (outlineText)
                OutlineCanvasObject(canvasObject);
        }

        /// <summary>
        /// Hashes the input string using SHA256 and returns the hexadecimal representation.
        /// </summary>
        /// <param name="input">Input string to be hashed.</param>
        /// <returns>Output hashed string.</returns>
        public static string GetSHA256(string input)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in bytes)
                stringBuilder.Append(b.ToString("x2"));

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns the current UTC timestamp formatted as an ISO 8601 string.
        /// </summary>
        /// <returns>A string representing the current UTC date and time in ISO 8601 format.</returns>
        public static string CurrentTimestamp()
        {
            DateTime utcNow = DateTime.UtcNow;
            return utcNow.ToString("o");
        }

        public static string ColorToHex(Color color) =>
            ColorUtility.ToHtmlStringRGB(color);

        public static Color HexToColor(string hex)
        {
            if (!ColorUtility.TryParseHtmlString(hex, out var color))
                return Color.black;
            return color;
        }

        public static string NoRichtextTags(string input, string replace = "")
        {
            Regex notags = new Regex("<.*?>", RegexOptions.IgnoreCase);
            return notags.Replace(input, replace);
        }

        public static string NoColorTags(string input, string replace = "")
        {
            Regex notags = new Regex(@"<color=.*?>|</color>", RegexOptions.IgnoreCase);
            return notags.Replace(input, replace);
        }

        private static Gradient richtextGradientGradient = new Gradient();
        public static string RichtextGradient(string input, GradientColorKey[] Colors)
        {
            richtextGradientGradient ??= new Gradient();
            richtextGradientGradient.colorKeys = Colors;

            char[] chars = input.ToCharArray();
            string finalOutput = "";
            for (int i = 0; i < chars.Length; i++)
            {
                char character = chars[i];
                Color characterColor = richtextGradientGradient.Evaluate((Time.time / 2f + i / 25f) % 1f);
                finalOutput += $"<color=#{ColorToHex(characterColor)}>{character}</color>";
            }

            return finalOutput;
        }

        public static Color BrightenColor(Color color, float intensity = 0.5f)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            v = Mathf.Clamp01(v + (1f - v) * intensity * 1.2f);
            s = Mathf.Clamp01(s * (1f - intensity * 0.3f));
            return Color.HSVToRGB(h, s, v);
        }

        public static Color DarkenColor(Color color, float intensity = 0.5f) =>
            new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);

        public static bool inRoomStatus;
        public static string lastRoom;

        public static string CleanPlayerName(string input, int length = 12)
        {
            input = NoRichtextTags(input);

            if (input.Length > length)
                input = input[..(length - 1)];

            return input;
        }

        private static void OnJoinRoom()
        {
            if (inRoomStatus)
                return;

            inRoomStatus = true;
            lastRoom = PhotonNetwork.CurrentRoom.Name;

            if (!disableRoomNotifications)
                NotificationManager.SendNotification($"<color=grey>[</color><color=blue>JOIN ROOM</color><color=grey>]</color> Room Code: {lastRoom}");

            RPCProtection();
        }

        private static void OnLeaveRoom()
        {
            if (!inRoomStatus)
                return;

            inRoomStatus = false;

            if (clearNotificationsOnDisconnect)
                NotificationManager.ClearAllNotifications();

            if (!disableRoomNotifications)
                NotificationManager.SendNotification($"<color=grey>[</color><color=blue>LEAVE ROOM</color><color=grey>]</color> Room Code: {lastRoom}");

            RPCProtection();
        }

        private static void OnMasterClientSwitch(NetPlayer masterClient)
        {
            if (disableMasterClientNotifications)
                return;

            if (NetworkSystem.Instance.IsMasterClient)
            {
                Buttons.GetIndex("MasterLabel").overlapText = "You are master client.";
                NotificationManager.SendNotification("<color=grey>[</color><color=purple>MASTER</color><color=grey>]</color> You are now master client.");
            }
            else
                Buttons.GetIndex("MasterLabel").overlapText = "You are not master client.";
        }

        private static void OnPlayerJoin(NetPlayer Player)
        {
            if (Player != NetworkSystem.Instance.LocalPlayer && !disablePlayerNotifications)
                NotificationManager.SendNotification($"<color=grey>[</color><color=green>JOIN</color><color=grey>]</color> Name: {CleanPlayerName(Player.NickName)}");
        }

        private static void OnPlayerLeave(NetPlayer Player)
        {
            if (Player != NetworkSystem.Instance.LocalPlayer && !disablePlayerNotifications)
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>LEAVE</color><color=grey>]</color> Name: {CleanPlayerName(Player.NickName)}");
        }

        public static Vector3 ServerSyncPos;
        public static Vector3 ServerSyncLeftHandPos;
        public static Vector3 ServerSyncRightHandPos;

        public static Vector3 ServerPos;
        public static Vector3 ServerLeftHandPos;
        public static Vector3 ServerRightHandPos;

        private static void OnSerialize()
        {
            ServerSyncPos = VRRig.LocalRig?.transform.position ?? ServerSyncPos;
            ServerSyncLeftHandPos = VRRig.LocalRig?.leftHand?.rigTarget?.transform.position ?? ServerSyncLeftHandPos;
            ServerSyncRightHandPos = VRRig.LocalRig?.rightHand?.rigTarget?.transform.position ?? ServerSyncRightHandPos;
        }

        public static readonly Dictionary<VRRig, int> playerPing = new Dictionary<VRRig, int>();
        private static void OnPlayerSerialize(VRRig rig) =>
            playerPing[rig] = rig.GetTruePing();

        /// <summary>
        /// Serializes multiple PhotonViews owned by the local player, with optional filtering and timing adjustments.
        /// </summary>
        /// <param name="exclude">If true, serializes all owned PhotonViews except those in the viewFilter; if false, serializes only those in
        /// the viewFilter.</param>
        /// <param name="viewFilter">An array of PhotonViews to include or exclude from serialization, depending on the exclude parameter.</param>
        /// <param name="timeOffset">An integer offset to apply to the serialization time.</param>
        /// <param name="delay">A delay in seconds before serialization is sent.</param>
        public static void MassSerialize(bool exclude = false, PhotonView[] viewFilter = null, int timeOffset = 0, float delay = 0f)
        {
            if (!PhotonNetwork.InRoom)
                return;

            viewFilter ??= Array.Empty<PhotonView>();

            NonAllocDictionary<int, PhotonView> photonViewList = PhotonNetwork.photonViewList;
            List<PhotonView> viewsToSerialize = new List<PhotonView>();

            List<int> filteredViewIDs = viewFilter.Select(view => view.ViewID).ToList();

            foreach (PhotonView photonView in photonViewList.Values)
            {
                if (exclude)
                {
                    if (photonView.IsMine && !filteredViewIDs.Contains(photonView.ViewID))
                        viewsToSerialize.Add(photonView);
                }
                else
                {
                    if (photonView.IsMine && filteredViewIDs.Contains(photonView.ViewID))
                        viewsToSerialize.Add(photonView);
                }
            }

            foreach (PhotonView view in viewsToSerialize)
                SendSerialize(view, null, timeOffset, delay);
        }

        /// <summary>
        /// Serializes and sends the state of a PhotonView to other clients in the room, with optional event options,
        /// time offset, and delay.
        /// </summary>
        /// <param name="pv">The PhotonView to serialize and send.</param>
        /// <param name="options">Optional RaiseEventOptions to customize event sending.</param>
        /// <param name="timeOffset">Optional time offset to apply to the event timestamp.</param>
        /// <param name="delay">Optional delay in seconds before sending the event.</param>
        public static void SendSerialize(PhotonView pv, RaiseEventOptions options = null, int timeOffset = 0, float delay = 0f)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (pv == null)
            {
                LogManager.LogError("PhotonView is null. Cannot serialize.");
                return;
            }

            List<object> serializedData = PhotonNetwork.OnSerializeWrite(pv);

            PhotonNetwork.RaiseEventBatch raiseEventBatch = new PhotonNetwork.RaiseEventBatch();

            bool mixedReliable = pv.mixedModeIsReliable;
            raiseEventBatch.Reliable = pv.Synchronization == ViewSynchronization.ReliableDeltaCompressed || mixedReliable;
            raiseEventBatch.Group = pv.Group;

            IDictionary dictionary = PhotonNetwork.serializeViewBatches;

            PhotonNetwork.SerializeViewBatch serializeViewBatch = new PhotonNetwork.SerializeViewBatch(raiseEventBatch, 2);

            if (!dictionary.Contains(raiseEventBatch))
                dictionary[raiseEventBatch] = serializeViewBatch;

            serializeViewBatch.Add(serializedData);

            RaiseEventOptions sendOptions = PhotonNetwork.serializeRaiseEvOptions;
            RaiseEventOptions finalOptions = options != null ? new RaiseEventOptions
            {
                CachingOption = sendOptions.CachingOption,
                Flags = sendOptions.Flags,
                InterestGroup = sendOptions.InterestGroup,
                TargetActors = options.TargetActors,
                Receivers = options.Receivers
            } : sendOptions;

            bool reliable = serializeViewBatch.Batch.Reliable;
            List<object> objectUpdate = serializeViewBatch.ObjectUpdates;
            byte currentLevelPrefix = PhotonNetwork.currentLevelPrefix;

            objectUpdate[0] = PhotonNetwork.ServerTimestamp + timeOffset;
            objectUpdate[1] = currentLevelPrefix != 0 ? (object)currentLevelPrefix : null;


            if (delay <= 0f)
                PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206 : 201), objectUpdate, finalOptions,
                    reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
            else
            {
                objectUpdate = new List<object>(objectUpdate);
                CoroutineManager.instance.StartCoroutine(SerializationDelay(() =>
                    PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206 : 201), objectUpdate, finalOptions,
                        reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable), delay));
            }

            serializeViewBatch.Clear();
        }

        public static IEnumerator SerializationDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        public static void TeleportPlayer(Vector3 pos, bool keepVelocity = false) // Prevents your hands from getting stuck on trees
        {
            GTPlayer.Instance.TeleportTo(World2Player(pos), GTPlayer.Instance.transform.rotation, keepVelocity);
            VRRig.LocalRig.transform.position = pos;

            closePosition = Vector3.zero;
            Movement.lastPosition = Vector3.zero;
            if (VRKeyboard != null)
            {
                VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            }
        }

        public static void TeleportPlayer(Transform transform, bool matchDestinationRotation = true, bool maintainVelocity = true)
        {
            GTPlayer.Instance.TeleportTo(transform, matchDestinationRotation, maintainVelocity);
            closePosition = Vector3.zero;
            Movement.lastPosition = Vector3.zero;
            if (VRKeyboard != null)
            {
                VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            }
        }

        public static void SetRotation(Quaternion rotation) =>
            SetRotation(rotation.y);

        public static void SetRotation(float rotation) =>
           GTPlayer.Instance.Turn(rotation - GTPlayer.Instance.mainCamera.transform.eulerAngles.y);

        [Obsolete("GetIndex is obsolete. Use Buttons.GetIndex instead.")]
        public static ButtonInfo GetIndex(string buttonText) =>
            Buttons.GetIndex(buttonText);

        [Obsolete("GetCategory is obsolete. Use Buttons.GetCategory instead.")]
        public static int GetCategory(string categoryName) =>
            Buttons.GetCategory(categoryName);

        [Obsolete("AddCategory is obsolete. Use Buttons.AddCategory instead.")]
        public static int AddCategory(string categoryName) =>
            Buttons.AddCategory(categoryName);

        [Obsolete("RemoveCategory is obsolete. Use Buttons.RemoveCategory instead.")]
        public static void RemoveCategory(string categoryName) =>
            Buttons.RemoveCategory(categoryName);

        [Obsolete("AddButton is obsolete. Use Buttons.AddButton instead.")]
        public static void AddButton(int category, ButtonInfo button, int index = -1) =>
            Buttons.AddButton(category, button, index);

        [Obsolete("AddButtons is obsolete. Use Buttons.AddButtons instead.")]
        public static void AddButtons(int category, ButtonInfo[] buttons, int index = -1) =>
            Buttons.AddButtons(category, buttons, index);

        [Obsolete("RemoveButton is obsolete. Use Buttons.RemoveButton instead.")]
        public static void RemoveButton(int category, string name, int index = -1) =>
            Buttons.RemoveButton(category, name, index);

        public static void ReloadMenu()
        {
            if (menu != null)
            {
                Destroy(menu);
                menu = null;

                CreateMenu();
            }

            if (reference != null)
            {
                Destroy(reference);
                reference = null;

                CreateReference();
            }
        }

        public static void ChangeName(string PlayerName, bool noColor = false)
        {
            GorillaComputer.instance.currentName = PlayerName;

            GorillaComputer.instance.SetLocalNameTagText(GorillaComputer.instance.currentName);
            GorillaComputer.instance.savedName = GorillaComputer.instance.currentName;
            PlayerPrefs.SetString("playerName", GorillaComputer.instance.currentName);
            PlayerPrefs.Save();

            PhotonNetwork.LocalPlayer.NickName = PlayerName;

            if (!noColor)
            {
                try
                {
                    if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) || CosmeticWardrobeProximityDetector.IsUserNearWardrobe(PhotonNetwork.LocalPlayer.UserId))
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b);
                        RPCProtection();
                    }
                }
                catch { }
            }
        }

        public static void ChangeColor(Color color, object target = null)
        {
            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));

            GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
            PlayerPrefs.Save();

            try
            {
                switch (target)
                {
                    case null:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, color.r, color.g, color.b);
                        break;
                    case NetPlayer player:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", player, color.r, color.g, color.b);
                        break;
                    case RpcTarget targets:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", targets, color.r, color.g, color.b);
                        break;
                }

                RPCProtection();
            } catch { }
        }

        public static void PlayButtonSound(string buttonText = null, bool overlapHand = false, bool leftOverlap = false)
        {
            bool archiveRightHand = rightHand;
            if (overlapHand)
                rightHand = leftOverlap;
            try
            {
                if (doButtonsVibrate)
                    GorillaTagger.Instance.StartVibration(rightHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);

                if (exclusivePageSounds && buttonText != null && (buttonText == "PreviousPage" || buttonText == "NextPage"))
                {
                    string url = buttonText == "PreviousPage" ? "prev.ogg" : buttonText == "NextPage" ? "next.ogg" : null;
                    if (url != null) Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/{url}", $"Audio/Menu/{url}"), buttonClickVolume / 10f);
                    rightHand = archiveRightHand;
                    return;
                }

                if (buttonClickIndex <= 3 || buttonClickIndex == 11 || buttonClickIndex == 25)
                {
                    VRRig.LocalRig.PlayHandTapLocal(buttonClickSound, rightHand, buttonClickVolume / 10f);
                    if (PhotonNetwork.InRoom && serversidedButtonSounds)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, buttonClickSound, rightHand, buttonClickVolume / 10f);
                        RPCProtection();
                    }
                }
                else
                {
                    Dictionary<int, string> namesToIds = new Dictionary<int, string>
                    {
                        { 4, "creamy" },
                        { 5, "anthrax" },
                        { 6, "leverdown" },
                        { 7, "click" },
                        { 8, "rr" },
                        { 9, "watch" },
                        { 10, "membrane" },
                        { 13, "slider" },
                        { 14, "can" },
                        { 15, "cut" },
                        { 16, "creamy2" },
                        { 17, "robloxbutton" },
                        { 18, "robloxtick" },
                        { 19, "mouse" },
                        { 20, "valve" },
                        { 21, "nintendo" },
                        { 22, "windows" },
                        { 23, "destiny" },
                        { 24, "untitled" },
                        { 26, "dog" }
                    };

                    try
                    {
                        ButtonInfo button = Buttons.GetIndex(buttonText);
                        if (button != null)
                        {
                            if (button.isTogglable)
                                namesToIds[6] = button.enabled ? "leverup" : "leverdown";
                        }
                    }
                    catch { }

                    AudioSource audioSource = rightHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer;
                    audioSource.volume = buttonClickVolume / 10f;
                    audioSource.PlayOneShot(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/Buttons/{namesToIds[buttonClickIndex]}.ogg", $"Audio/Menu/Buttons/{namesToIds[buttonClickIndex]}.ogg"));
                }
            } catch { }
            rightHand = archiveRightHand;
        }

        private static int? noInvisLayerMask;
        public static int NoInvisLayerMask()
        {
            noInvisLayerMask ??= ~(
                1 << LayerMask.NameToLayer("TransparentFX") |
                1 << LayerMask.NameToLayer("Ignore Raycast") | 
                1 << LayerMask.NameToLayer("Zone") |
                1 << LayerMask.NameToLayer("Gorilla Trigger") |
                1 << LayerMask.NameToLayer("Gorilla Boundary") |
                1 << LayerMask.NameToLayer("GorillaCosmetics") |
                1 << LayerMask.NameToLayer("GorillaParticle"));

            return noInvisLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers;
        }

        /// <summary>
        /// Toggles the state or performs the associated action for the specified button, such as navigating pages,
        /// toggling mods, or updating quick actions and favorites.
        /// </summary>
        /// <remarks>This method supports a variety of button actions, including page navigation
        /// ("PreviousPage", "NextPage"), toggling mod states, managing quick actions and favorites, and handling custom
        /// bindings. Some actions may trigger notifications or require specific permissions, especially when invoked
        /// from the menu. If the specified button text does not correspond to a known button, an error is
        /// logged.</remarks>
        /// <param name="buttonText">The text identifier of the button to toggle or activate. This determines which action or toggle operation is
        /// performed.</param>
        /// <param name="fromMenu">true to indicate the toggle was initiated from the menu interface; otherwise, false. Affects notification
        /// display and certain toggle behaviors.</param>
        /// <param name="ignoreForce">true to bypass force-related checks and restrictions during the toggle operation; otherwise, false.</param>
        public static void Toggle(string buttonText, bool fromMenu = false, bool ignoreForce = false)
        {
            switch (buttonText)
            {
                case "PreviousPage":
                {
                    if (dynamicAnimations)
                        lastClickedName = "PreviousPage";

                    pageNumber--;
                    if (pageNumber < 0)
                        pageNumber = LastPage;
                    break;
                }
                case "NextPage":
                {
                    if (dynamicAnimations)
                        lastClickedName = "NextPage";

                    pageNumber++;
                    pageNumber %= LastPage + 1;
                    break;
                }
                default:
                {
                    ButtonInfo target = Buttons.GetIndex(buttonText);
                    if (target != null)
                    {
                        string newIndicator = " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
                        if (target.overlapText != null && target.overlapText.Contains(newIndicator))
                        {
                            target.overlapText = target.overlapText.Replace(newIndicator, "");
                            if (target.overlapText == target.buttonText)
                                target.overlapText = target.buttonText;
                        }

                        if (target.label)
                        {
                            ReloadMenu();
                            return;
                        }

                        switch (fromMenu)
                        {
                            case true when !ignoreForce && menuButtonIndex != 2 && ((leftGrab && !joystickMenu) || (joystickMenu && rightJoystick.y > 0.5f && leftTrigger > 0.5f)):
                            {
                                if (IsBinding)
                                {
                                    bool AlreadyBinded = false;
                                    string BindedTo = "";
                                    foreach (var Bind in ModBindings.Where(Bind => Bind.Value.Contains(target.buttonText)))
                                    {
                                        AlreadyBinded = true;
                                        BindedTo = Bind.Key;
                                        break;
                                    }

                                    if (AlreadyBinded)
                                    {
                                        target.customBind = null;
                                        ModBindings[BindedTo].Remove(target.buttonText);
                                        VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);

                                        if (fromMenu)
                                            NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully unbinded mod.");
                                    } else
                                    {
                                        target.customBind = BindInput;
                                        ModBindings[BindInput].Add(target.buttonText);
                                        VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);

                                        if (fromMenu)
                                            NotificationManager.SendNotification($"<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully binded mod to <color=green>{BindInput}</color>.");
                                    }
                                }
                                else
                                {
                                    if (IsRebinding)
                                    {
                                        if (target.rebindKey != null)
                                        {
                                            target.rebindKey = null;
                                            VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
                                            if (fromMenu)
                                                NotificationManager.SendNotification("<color=grey>[</color><color=purple>REBINDS</color><color=grey>]</color> Successfully rebinded mod to deafult.");
                                        }
                                        else
                                        {
                                            target.rebindKey = BindInput;
                                            VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
                                            if (fromMenu)
                                                NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully rebinded mod to {BindInput}.");
                                        }
                                    }
                                    else
                                    {
                                        if (target.buttonText != "Exit Favorite Mods")
                                        {
                                            if (favorites.Contains(target.buttonText))
                                            {
                                                favorites.Remove(target.buttonText);
                                                VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);

                                                if (fromMenu)
                                                    NotificationManager.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Removed from favorites.");
                                            }
                                            else
                                            {
                                                favorites.Add(target.buttonText);
                                                VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);

                                                if (fromMenu)
                                                    NotificationManager.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Added to favorites.");
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                            case true when !ignoreForce && menuButtonIndex != 3 && leftTrigger > 0.5f && !joystickMenu:
                            {
                                if (!quickActions.Contains(target.buttonText))
                                {
                                    quickActions.Add(target.buttonText);
                                    VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);

                                    if (fromMenu)
                                        NotificationManager.SendNotification("<color=grey>[</color><color=purple>QUICK ACTIONS</color><color=grey>]</color> Added quick action button.");
                                } else
                                {
                                    quickActions.Remove(target.buttonText);
                                    VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
                                    
                                    if (fromMenu)
                                        NotificationManager.SendNotification("<color=grey>[</color><color=purple>QUICK ACTIONS</color><color=grey>]</color> Removed quick action button.");
                                }

                                break;
                            }
                            case true when target.detected && !allowDetected:
                            {
                                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is detected and requires permission to run.");
                                break;
                            }
                            default:
                            {
                                if (target.isTogglable)
                                {
                                    target.enabled = !target.enabled;
                                    if (target.enabled)
                                    {
                                        if (fromMenu)
                                            NotificationManager.SendNotification($"<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> {target.toolTip}");
                                        
                                        if (target.enableMethod != null)
                                            try { target.enableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(
                                                $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                                    }
                                    else
                                    {
                                        if (fromMenu)
                                            NotificationManager.SendNotification($"<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> {target.toolTip}");
                                        
                                        if (target.disableMethod != null)
                                            try { target.disableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(
                                                $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                                    }

                                            int enabledButtons = Buttons.buttons
                                                .SelectMany(list => list)
                                                .Where(button => button.enabled).Count();

                                            if (enabledButtons >= 50)
                                                AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                                                {
                                                    name = "Dedicated",
                                                    description = "Enable 50 mods at the same time.",
                                                    icon = "Images/Achievements/award.png"
                                                });

                                            if (enabledButtons >= 100)
                                                AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                                                {
                                                    name = "Too Dedicated",
                                                    description = "Enable 100 mods at the same time.",
                                                    icon = "Images/Achievements/red-award.png"
                                                });
                                        }
                                else
                                {
                                    if (dynamicAnimations)
                                        lastClickedName = target.buttonText;

                                    if (fromMenu)
                                        NotificationManager.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);

                                    if (target.method != null)
                                        try { target.method.Invoke(); } catch (Exception exc) { LogManager.LogError(
                                            $"Error with mod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                                }
                                try
                                {
                                    if (fromMenu && !ignoreForce && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) && rightJoystickClick && PhotonNetwork.InRoom)
                                    {
                                        Console.ExecuteCommand("forceenable", ReceiverGroup.Others, target.buttonText, target.enabled);
                                        NotificationManager.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> Force enabled mod for other menu users.");
                                        VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
                                    }
                                } catch { }

                                break;
                            }
                        }
                    }
                    else
                        LogManager.LogError($"{buttonText} does not exist");

                    break;
                }
            }
            ReloadMenu();
        }

        /// <summary>
        /// Toggles the state of the specified button using the provided button information.
        /// </summary>
        /// <param name="buttonInfo">An object containing information about the button to toggle. Cannot be null.</param>
        /// <param name="fromMenu">Indicates whether the toggle action was initiated from a menu. Set to <see langword="true"/> if triggered
        /// from a menu; otherwise, <see langword="false"/>.</param>
        /// <param name="ignoreForce">Indicates whether to ignore any force constraints when toggling the button. Set to <see langword="true"/> to
        /// ignore force constraints; otherwise, <see langword="false"/>.</param>
        public static void Toggle(ButtonInfo buttonInfo, bool fromMenu = false, bool ignoreForce = false) =>
            Toggle(buttonInfo.buttonText, fromMenu, ignoreForce);

        /// <summary>
        /// Toggles the incremental or decremental state of a button and updates its associated UI and behavior
        /// accordingly.
        /// </summary>
        /// <remarks>This method updates the button's visual indicator and triggers the corresponding
        /// enable or disable method for the button. If certain boost conditions are met, the action may be performed
        /// multiple times. A notification is displayed to inform the user of the action taken.</remarks>
        /// <param name="buttonText">The text label of the button to be toggled. This is used to identify the target button.</param>
        /// <param name="increment">true to apply the incremental action; false to apply the decremental action.</param>
        public static void ToggleIncremental(string buttonText, bool increment)
        { 
            ButtonInfo target = Buttons.GetIndex(buttonText);
            if (target != null)
            {
                string newIndicator = " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
                if (target.overlapText != null && target.overlapText.Contains(newIndicator))
                {
                    target.overlapText = target.overlapText.Replace(newIndicator, "");
                    if (target.overlapText == target.buttonText)
                        target.overlapText = target.buttonText;
                }

                if (dynamicAnimations)
                    lastClickedName = buttonText + (increment ? "+" : "-");

                bool boost = incrementalBoost && rightGrab;
                if (increment)
                {
                    NotificationManager.SendNotification($"<color=grey>[</color><color=green>INCREMENT</color><color=grey>]</color> {target.toolTip}");

                    if (boost)
                        for (int i = 0; i < 5; i++)
                        {
                            if (target.enableMethod != null)
                                try { target.enableMethod.Invoke(); }
                                catch (Exception exc)
                                {
                                    LogManager.LogError(
                                    $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                                }
                        }
                    else
                        if (target.enableMethod != null)
                            try { target.enableMethod.Invoke(); }
                            catch (Exception exc)
                            {
                                LogManager.LogError(
                                $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                            }
                }
                else
                {
                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>DECREMENT</color><color=grey>]</color> {target.toolTip}");

                    if (boost)
                        for (int i = 0; i < 5; i++)
                        {
                            if (target.enableMethod != null)
                                if (target.disableMethod != null)
                                    try { target.disableMethod.Invoke(); }
                                    catch (Exception exc)
                                    {
                                        LogManager.LogError(
                                        $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                                    }
                        }
                    else
                        if (target.disableMethod != null)
                            try { target.disableMethod.Invoke(); }
                            catch (Exception exc)
                            {
                                LogManager.LogError(
                                $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                            }
                }
            }
            ReloadMenu();
        }

        public static IEnumerator DelayLoadPreferences()
        {
            yield return new WaitForSeconds(1);
            Settings.LoadPreferences();
        }

        public static void UnloadMenu()
        {
            Settings.Panic();
            CustomBoardManager.CustomBoardsEnabled = false;

            NetworkSystem.Instance.OnJoinedRoomEvent -= OnJoinRoom;
            NetworkSystem.Instance.OnReturnedToSinglePlayer -= OnLeaveRoom;

            NetworkSystem.Instance.OnPlayerJoined -= OnPlayerJoin;
            NetworkSystem.Instance.OnPlayerLeft -= OnPlayerLeave;

            if (Console.instance != null)
                Destroy(Console.instance.gameObject);

            if (NotificationManager.Instance != null)
            {
                Destroy(NotificationManager.Instance.canvas);
                Destroy(NotificationManager.arraylistText);
                Destroy(NotificationManager.notificationText);
                Destroy(NotificationManager.Instance.gameObject);
            }

            if (VRKeyboard != null)
            {
                Destroy(VRKeyboard);
                VRKeyboard = null;
            }

            if (CustomBoardManager.instance.motdTitle != null)
            {
                Destroy(CustomBoardManager.instance.motdTitle);
                CustomBoardManager.instance.motdTitle = null;
            }

            if (CustomBoardManager.instance.motdText != null)
            {
                Destroy(CustomBoardManager.instance.motdText);
                CustomBoardManager.instance.motdText = null;
            }

            if (menuBackground != null)
            {
                Destroy(menuBackground);
                menuBackground = null;
            }

            if (menu != null)
            {
                Destroy(menu);
                menu = null;
            }

            if (reference != null)
            {
                Destroy(reference);
                reference = null;
            }

            if (lKeyReference != null)
            {
                Destroy(lKeyReference);
                lKeyReference = null;
            }

            if (rKeyReference != null)
            {
                Destroy(rKeyReference);
                rKeyReference = null;
            }

            try
            {
                Visuals.ClearLinePool();
                Visuals.ClearNameTagPool();
            }
            catch { }

            HasLoaded = false;
            hasLoadedPreferences = false;
            loadPreferencesTime = -1;

            Lockdown = true;

            if (Plugin.instance != null)
                Destroy(Plugin.instance);

            PatchHandler.UnpatchAll();
        }

        public static void InitializeFonts()
        {
            AgencyFB ??= LoadAsset<TMP_FontAsset>("Agency");
            FreeSans ??= LoadAsset<TMP_FontAsset>("FreeSans");
            Candara ??= LoadAsset<TMP_FontAsset>("Candara");
            ComicSans ??= LoadAsset<TMP_FontAsset>("ComicSans");
            CascadiaMono ??= LoadAsset<TMP_FontAsset>("CascadiaMono");
            Anton ??= LoadAsset<TMP_FontAsset>("Anton");
            Minecraft ??= LoadAsset<TMP_FontAsset>("Minecraft");
            MSGothic ??= LoadAsset<TMP_FontAsset>("MSGothic");
            OpenDyslexic ??= LoadAsset<TMP_FontAsset>("OpenDyslexic");
            SimSun ??= LoadAsset<TMP_FontAsset>("SimSun");
            Taiko ??= LoadAsset<TMP_FontAsset>("Taiko");
            Terminal ??= LoadAsset<TMP_FontAsset>("Terminal");
            Utopium ??= LoadAsset<TMP_FontAsset>("Utopium");
            DejaVuSans ??= LoadAsset<TMP_FontAsset>("DejaVuSans");

            foreach (TMP_FontAsset font in new[] { AgencyFB, FreeSans, Candara, ComicSans, 
                CascadiaMono, Anton, Minecraft, MSGothic, OpenDyslexic, SimSun, Taiko, 
                Terminal, Utopium, DejaVuSans })
                font.fallbackFontAssetTable.Add(LiberationSans);
        }

        public static TMP_FontAsset activeFont = LiberationSans;
        public static FontStyles activeFontStyle = FontStyles.Italic;

        public static TMP_FontAsset AgencyFB;
        public static TMP_FontAsset FreeSans;
        public static TMP_FontAsset Candara;
        public static TMP_FontAsset ComicSans;
        public static TMP_FontAsset CascadiaMono;
        public static TMP_FontAsset Anton;
        public static TMP_FontAsset Minecraft;
        public static TMP_FontAsset MSGothic;
        public static TMP_FontAsset OpenDyslexic;
        public static TMP_FontAsset SimSun;
        public static TMP_FontAsset Taiko;
        public static TMP_FontAsset Terminal;
        public static TMP_FontAsset Utopium;
        public static TMP_FontAsset DejaVuSans;
        public static TMP_FontAsset LiberationSans = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        // The variable warehouse
        public static string rat = @"
     _   _
    (q\_/p)
.-.  |. .|
   \ =\,/=
    )/ _ \  |\
   (/\):(/\  )\
jgs \_   _/ |Oo\
    `\""^""` `""`
";

        public static bool isOnPC;
        public static bool IsSteam = true;
        public static bool Lockdown;

        public static bool HasLoaded;
        public static bool hasLoadedPreferences;
        public static bool allowDetected;
        public static bool hasRemovedThisFrame;
        public static bool NoOverlapRPCs = true;
        public static float loadPreferencesTime;
        public static float playTime;
        public static int frameCount;
        public static float badAppleTime;

        public static bool thinMenu = true;
        public static bool longmenu;
        public static bool hidetitle;
        public static bool disorganized;
        public static bool flipMenu;
        public static bool shinyMenu;
        public static bool transparentMenu;
        public static bool crystallizeMenu;
        public static bool zeroGravityMenu;
        public static bool menuCollisions;
        public static bool dropOnRemove = true;
        public static bool shouldOutline;
        public static bool outlineText;
        public static bool underlineText;
        public static bool strikethroughText;
        public static bool smallCapsText;
        public static bool innerOutline;
        public static bool smoothLines;
        public static bool shouldRound;
        public static bool isMouseDown;
        public static bool openedwithright;
        public static bool oneHand;

        public static int _pageSize = 8;
        public static int PageSize
        {
            get => _pageSize - buttonOffset;
            set => _pageSize = value;
        }

        public static int DisplayedItemCount
        {
            get
            {
                int count = Buttons.buttons[currentCategoryIndex].Length;

                if (currentCategoryName == "Favorite Mods")
                    count = favorites.Count;

                if (currentCategoryName == "Enabled Mods")
                {
                    var enabledMods = new List<string> { "Exit Enabled Mods" };

                    for (int i = 0; i < Buttons.buttons.Length; i++)
                    {
                        var buttonList = Buttons.buttons[i];

                        foreach (var v in buttonList)
                        {
                            if (v.enabled)
                                enabledMods.Add(v.buttonText);
                        }
                    }

                    count = enabledMods.Count - 1;
                }

                if (isSearching)
                {
                    List<ButtonInfo> searchedMods = new List<ButtonInfo>();

                    if (nonGlobalSearch && currentCategoryName != "Main")
                    {
                        foreach (ButtonInfo v in Buttons.buttons[currentCategoryIndex])
                        {
                            try
                            {
                                string buttonText = v.overlapText ?? v.buttonText;

                                if (buttonText.ClearTags().Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                    searchedMods.Add(v);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        int categoryIndex = 0;
                        foreach (ButtonInfo[] buttonList in Buttons.buttons)
                        {
                            foreach (ButtonInfo v in buttonList)
                            {
                                try
                                {
                                    if (((Buttons.categoryNames[categoryIndex].Contains("Admin") ||
                                        Buttons.categoryNames[categoryIndex] == "Mod Givers") &&
                                        !isAdmin)
                                        || (v.detected && !allowDetected))
                                        continue;

                                    string displayedText = v.overlapText ?? v.buttonText;

                                    if (displayedText.Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                        searchedMods.Add(v);
                                }
                                catch { }
                            }
                            categoryIndex++;
                        }
                    }

                    count = searchedMods.Count;
                }

                return count;
            }
        }

        public static float ButtonDistance
        {
            get => 0.8f / (PageSize + buttonOffset);
        }

        public static int LastPage => (DisplayedItemCount + PageSize - 1) / PageSize - 1;

        public static int _currentCategoryIndex;
        public static int currentCategoryIndex
        {
            get => _currentCategoryIndex;
            set
            {
                _currentCategoryIndex = value;
                pageNumber = 0;
                pageOffset = 0;
            }
        }

        public static string currentCategoryName
        {
            get => Buttons.categoryNames[currentCategoryIndex];
            set =>
                currentCategoryIndex = Buttons.GetCategory(value);
        }

        public static int pageOffset;
        public static int pageNumber;
        public static bool pageScrolling;
        public static bool noPageNumber;
        public static bool disablePageButtons;
        public static bool swapButtonColors;
        public static int pageButtonType = 1;
        public static float pageButtonChangeDelay;

        public static bool pcKeyboardSounds = true;

        public static int buttonClickSound = 8;
        public static int buttonClickIndex;
        public static int buttonClickVolume = 4;
        public static int buttonOffset = 2;
        public static int menuButtonIndex = 1;
        public static int characterDistance;

        public static bool doButtonsVibrate = true;
        public static bool serversidedButtonSounds;

        public static bool joystickMenu;
        public static bool joystickMenuSearching;

        public static bool physicalMenu;

        public static bool barkMenu;
        private static bool barkMenuOpen;
        private static bool? barkMenuGrabbed;

        private static float barkBangDelay;
        private static int barkBangCount;

        private static bool previousBarkBangState;

        public static Vector3 physicalOpenPosition = Vector3.zero;
        public static Quaternion physicalOpenRotation = Quaternion.identity;
        public static Vector3 smoothTargetPosition = Vector3.zero;
        public static Quaternion smoothTargetRotation = Quaternion.identity;
        public static bool joystickOpen;
        public static bool smoothMenuPosition;
        public static bool smoothMenuRotation;
        public static int joystickButtonSelected;
        public static string joystickSelectedButton = "";
        public static float joystickDelay;
        public static float scrollDelay;

        public static int joystickMenuPosition;
        public static Vector3[] joystickMenuPositions = {
            new Vector3(0.3f, 0.2f, 1f),
            new Vector3(-0.3f, 0.2f, 1f),
            new Vector3(0.3f, -0.2f, 1f),
            new Vector3(-0.3f, -0.2f, 1f),
            new Vector3(0f, -0.1f, 0.5f),
            new Vector3(0.3f, -0.1f, 1f),
            new Vector3(-0.3f, -0.1f, 1f),
            new Vector3(0f, 0.2f, 1f),
            new Vector3(0f, -0.2f, 1f),
        };

        public static bool rightHand;
        public static bool isRightHand;
        public static bool bothHands;
        public static bool wristMenu;
        public static bool explodeMenu;
        public static bool watchMenu;
        public static bool wristOpen;
        public static float wristMenuDelay;

        public static bool stackNotifications;
        public static bool hideBrackets;
        public static bool disableNotifications;
        public static bool disableMasterClientNotifications;
        public static bool disableRoomNotifications;
        public static bool disablePlayerNotifications;
        public static bool clearNotificationsOnDisconnect;
        public static string narratorName = "Default";
        public static int narratorIndex;
        public static bool showEnabledModsVR = true;
        public static bool advancedArraylist;
        public static bool flipArraylist;
        public static bool hideSettings;
        public static bool hideMacros;
        public static bool hideTextOnCamera;
        public static bool hidePointer;
        public static bool incrementalButtons = true;
        public static bool incrementalBoost;
        public static bool disableDisconnectButton;
        public static bool disableFpsCounter;
        public static bool disableSearchButton;
        public static bool disableReturnButton;
        public static bool enableDebugButton;

        public static bool ghostException;
        public static bool disableGhostview;
        public static bool legacyGhostview;
        public static bool checkMode;
        public static bool lastChecker;
        public static bool rockWatermark = true;
        public static bool disableWatermark;
        public static string CosmeticsOwned;

        public static Vector3 MidPosition;
        public static Vector3 MidVelocity;

        public static bool SmoothGunPointer;
        public static bool smallGunPointer;
        public static bool disableGunPointer;
        public static bool disableGunLine;
        public static bool SwapGunHand;
        public static bool GriplessGuns;
        public static bool TriggerlessGuns;
        public static bool HardGunLocks;
        public static bool GunSounds;
        public static bool GunVibrations;
        public static bool GunParticles;
        public static int gunVariation;
        public static int GunDirection;
        public static int GunLineQuality = 50;

        public static bool GunSpawned;
        public static bool gunLocked;
        public static VRRig lockTarget;

        public static bool lastGunSpawned;
        public static bool lastGunTrigger;

        public static bool lastGunSpawnedVibration;

        public static int fontCycle;
        public static bool NoAutoSizeText;

        public static bool doCustomName;
        public static string customMenuName = "Your Text Here";
        public static bool doCustomMenuBackground;
        public static bool menuTrail;
        public static bool adaptiveButtons = true;
        public static int pcbg;

        public static bool isSearching;
        public static bool nonGlobalSearch;
        public static bool isKeyboardPc;
        public static bool inTextInput;
        public static string keyboardInput = "";

        public static int? fullModAmount;
        public static int amountPartying;
        public static bool waitForPlayerJoin;
        public static bool scaleWithPlayer;
        public static float menuScale = 1f;
        public static int notificationScale = 30;
        public static int overlayScale = 30;
        public static int arraylistScale = 20;

        public static bool dynamicSounds;
        public static bool exclusivePageSounds;
        public static bool dynamicAnimations;
        public static bool slowDynamicAnimations;
        public static bool dynamicGradients;
        public static bool horizontalGradients;
        public static bool scrollingGradients;
        public static bool particleSpawnEffect;
        public static bool animatedTitle;
        public static bool gradientTitle;
        public static string lastClickedName = "";

        public static bool isMenuButtonHeld;
        public static bool shouldBePC;
        public static bool leftPrimary;
        public static bool leftSecondary;
        public static bool rightPrimary;
        public static bool rightSecondary;
        public static bool leftGrab;
        public static bool rightGrab;
        public static float leftTrigger;
        public static float rightTrigger;

        public static Vector2 leftJoystick = Vector2.zero;
        public static Vector2 rightJoystick = Vector2.zero;
        public static bool leftJoystickClick;
        public static bool rightJoystickClick;

        public static bool ToggleBindings = true;
        public static bool OverwriteKeybinds;
        public static bool IsBinding;
        public static string BindInput = "";
        public static bool IsRebinding;

        public static readonly Dictionary<string, List<string>> ModBindings = new Dictionary<string, List<string>> {
            { "A", new List<string>() },
            { "B", new List<string>() },
            { "X", new List<string>() },
            { "Y", new List<string>() },
            { "LG", new List<string>() },
            { "RG", new List<string>() },
            { "LT", new List<string>() },
            { "RT", new List<string>() },
            { "LJ", new List<string>() },
            { "RJ", new List<string>() },
        };

        public static readonly Dictionary<string, bool> BindStates = new Dictionary<string, bool> {
            { "A", false },
            { "B", false },
            { "X", false },
            { "Y", false },
            { "LG", false },
            { "RG", false },
            { "LT", false },
            { "RT", false },
            { "LJ", false },
            { "RJ", false },
        };

        public static readonly List<string> quickActions = new List<string>();

        public static Camera TPC;
        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject pcBackground;
        public static GameObject reference;
        public static VideoPlayer videoPlayer;
        public static VideoPlayer promptVideoPlayer;
        public static SphereCollider buttonCollider;
        public static GameObject canvasObj;
        public static TextMeshPro fpsCount;
        public static Image watermarkImage;
        private static float fpsAvgTime;
        private static float fpsAverageNumber;
        private static float? potatoTime = 0f;
        private static float? adminTime = 0f;
        public static bool fpsCountTimed;
        public static bool fpsCountAverage;
        public static bool ftCount;
        public static bool acceptedDonations;
        public static float lastDeltaTime = 1f;
        public static TextMeshPro keyboardInputObject;
        public static TextMeshPro title;
        public static VRRig GhostRig;
        public static GameObject legacyGhostViewLeft;
        public static GameObject legacyGhostViewRight;
        public static Material GhostMaterial;
        public static Material CrystalMaterial;
        public static Material searchMat;
        public static Material updateMat;
        public static Material promptMat;
        public static Material watermarkMat;
        public static Material returnMat;
        public static Material debugMat;
        public static Material donateMat;

        public static GameObject lKeyReference;
        public static SphereCollider lKeyCollider;

        public static GameObject rKeyReference;
        public static SphereCollider rKeyCollider;

        public static GameObject VRKeyboard;
        public static GameObject menuSpawnPosition;

        public static GameObject TryOnRoom;

        public static GameObject watchobject;
        public static GameObject watchText;
        public static GameObject watchShell;
        public static GameObject watchEnabledIndicator;
        public static Material watchIndicatorMat;
        public static int watchMenuIndex;

        public static GameObject regwatchobject;
        public static GameObject regwatchText;
        public static GameObject regwatchShell;
        public static Material glass;

        public static Material cannabisMat;
        public static Texture2D cann;
        public static Texture2D pride;
        public static Texture2D trans;
        public static Texture2D gay;
        public static Texture2D searchIcon;
        public static Texture2D returnIcon;
        public static Texture2D debugIcon;
        public static Texture2D donateIcon;
        public static Texture2D updateIcon;
        public static Texture2D fixTexture;
        public static Texture2D customMenuBackgroundImage;
        public static Texture2D customWatermark;

        public static readonly List<string> favorites = new List<string> { "Exit Favorite Mods" };
        public static bool translate;

        public static string serverLink = "https://discord.gg/iidk";

        public static int arrowType;
        public static readonly string[][] arrowTypes = {
            new[] {"<", ">"},
            new[] {"←", "→"},
            new[] { "    <sprite name=\"Left1\">", "    <sprite name=\"Right1\">"},
            new[] {"◄", "►"},
            new[] { "    <sprite name=\"Left2\">", "    <sprite name=\"Right2\">"},
            new[] {"‹", "›"},
            new[] {"«", "»"},
            new[] { "    <sprite name=\"Left3\">", "    <sprite name=\"Right3\">"},
            new[] {"-", "+"},
            new[] {"", ""},
            new[] {"v", "ʌ"},
            new[] { "v\nv\nv\nv\nv\nv", "ʌ\nʌ\nʌ\nʌ\nʌ\nʌ" }
        };

        public static int themeType = 1;
        public static bool slowFadeColors;

        public static ExtGradient backgroundColor = new ExtGradient
        {
            colors = ExtGradient.GetSimpleGradient(
                new Color32(255, 128, 0, 128),
                new Color32(255, 102, 0, 128)
            )
        };

        public static ExtGradient[] buttonColors = {
            new ExtGradient // Released
            {
                colors = ExtGradient.GetSolidGradient(new Color32(170, 85, 0, 255))
            },

            new ExtGradient // Pressed
            {
                colors = ExtGradient.GetSolidGradient(new Color32(85, 42, 0, 255))
            }
        };

        public static ExtGradient[] textColors = {
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

        public static Vector3 closePosition;

        public static Vector3 pointerOffset = new Vector3(0f, -0.1f, 0f);
        public static int pointerIndex;

        public static string partyLastCode;
        public static float partyTime;
        public static bool partyKickReconnecting;

        public static float timeMenuStarted = -1f;
        public static float buttonCooldown;
        public static float autoSaveDelay = Time.time + 60f;
        public static bool backupPreferences;
        public static int preferenceBackupCount;

        public static int notificationDecayTime = 1000;
        public static int notificationSoundIndex;

        public static float ShootStrength = 19.44f;

        public static bool shift;
        public static bool lockShift; // KonekoKitten KTOH

        public static bool lowercaseMode;
        public static bool uppercaseMode;
        public static bool redactText;

        public static string inputTextColor = "green";
        
        public static bool annoyingMode; // Build with this enabled for a surprise
        public static readonly string[] facts = {
            "The honeybee is the only insect that produces food eaten by humans.",
            "Bananas are berries, but strawberries aren't.",
            "The Eiffel Tower can be 15 cm taller during the summer due to thermal expansion.",
            "A group of flamingos is called a 'flamboyance.'",
            "The shortest war in history was between Britain and Zanzibar on August 27, 1896 – Zanzibar surrendered after 38 minutes.",
            "Cows have best friends and can become stressed when they are separated.",
            "The first computer programmer was a woman named Ada Lovelace.",
            "A 'jiffy' is an actual unit of time, equivalent to 1/100th of a second.",
            "Octopuses have three hearts and blue blood.",
            "The world's largest desert is Antarctica.",
            "Honey never spoils. Archaeologists have found pots of honey in ancient Egyptian tombs that are over 3,000 years old and still perfectly edible.",
            "The smell of freshly-cut grass is actually a plant distress call.",
            "The average person spends six months of their life waiting for red lights to turn green.",
            "A group of owls is called a parliament.",
            "The longest word in the English language without a vowel is 'rhythms.'",
            "The Great Wall of China is not visible from the moon without aid.",
            "Venus rotates so slowly on its axis that a day on Venus (one full rotation) is longer than a year on Venus (orbit around the sun).",
            "The world's largest recorded snowflake was 15 inches wide.",
            "There are more possible iterations of a game of chess than there are atoms in the known universe.",
            "A newborn kangaroo is the size of a lima bean and is unable to hop until it's about 8 months old.",
            "The longest hiccuping spree lasted for 68 years!",
            "A single cloud can weigh more than 1 million pounds.",
            "Honeybees can recognize human faces.",
            "Cats have five toes on their front paws but only four on their back paws.",
            "The inventor of the frisbee was turned into a frisbee. Walter Morrison, the inventor, was cremated, and his ashes were turned into a frisbee after he passed away.",
            "Penguins give each other pebbles as a way of proposing.",
            "You need oxygen to live.",
            "You need to be nourished to live.",
            "The letter \"A\" is at the beginning of the alphabet. The letter \"T\" is at the beginning of both of these sentences. Why are you looking there? You're wasting your time. You're wasting even MORE time reading this. Ok bye. STOP READING!!!",
            "iiDk has a shitty bluetooth keyboard.",
            "You're wasting your time reading this.",
            "rocklobster222 is awsum",
            "kingofnetflix was here </3",
            "multifactor was also here - kingofnetflix"
        };
    }
}