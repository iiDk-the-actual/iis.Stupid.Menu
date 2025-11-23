/*
 * ii's Stupid Menu  Menu/Main.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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
using GorillaGameModes;
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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Valve.VR;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.FileUtilities;
using static iiMenu.Utilities.RandomUtilities;
using static iiMenu.Utilities.RigUtilities;
using Button = iiMenu.Classes.Menu.Button;
using CommonUsages = UnityEngine.XR.CommonUsages;
using Console = iiMenu.Classes.Menu.Console;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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
                    bool shouldOpen = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position - GorillaTagger.Instance.leftHandTransform.forward * 0.1f, ControllerUtilities.GetTrueRightHand().position) < 0.1f;
                    if (rightHand)
                        shouldOpen = Vector3.Distance(ControllerUtilities.GetTrueLeftHand().position, GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.rightHandTransform.forward * 0.1f) < 0.1f;

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

                #region Custom Boards
                if (!hasFoundAllBoards)
                {
                    try
                    {
                        foreach (GameObject board in objectBoards.Values)
                            Destroy(board);

                        objectBoards.Clear();

                        CreateObjectBoard("City", "Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/CosmeticsScoreboardAnchor/GorillaScoreBoard");
                        CreateObjectBoard("Arcade", "Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/Arcade_prefab/Arcade_Room/CosmeticsScoreboardAnchor/GorillaScoreBoard", new Vector3(-22.1964f, -21.4581f, 1.4f), new Vector3(270.0593f, 0f, 0f), new Vector3(23f, 2.1f, 21.6f));

                        var stumpChildren = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.Children()
                           .Where(x => x.name.Contains("UnityTempFile"))
                           .ToList();

                        if (StumpLeaderboardIndex >= 0 && StumpLeaderboardIndex < stumpChildren.Count)
                        {
                            var stumpBoard = stumpChildren[StumpLeaderboardIndex];
                            if (stumpBoard != null)
                            {
                                if (StumpMat == null)
                                    StumpMat = stumpBoard.GetComponent<Renderer>().material;

                                stumpBoard.GetComponent<Renderer>().material = OrangeUI;
                            }
                        }

                        var forestChildren = GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.Children()
                            .Where(x => x.name.Contains("UnityTempFile"))
                            .ToList(); 

                        if (ForestLeaderboardIndex >= 0 && ForestLeaderboardIndex < forestChildren.Count)
                        {
                            var forestBoard = forestChildren[ForestLeaderboardIndex];
                            if (forestBoard != null)
                            {
                                if (ForestMat == null)
                                    ForestMat = forestBoard.GetComponent<Renderer>().material;

                                forestBoard.GetComponent<Renderer>().material = OrangeUI;
                            }
                        }

                        foreach (GorillaNetworkJoinTrigger joinTrigger in PhotonNetworkController.Instance.allJoinTriggers)
                        {
                            try
                            {
                                JoinTriggerUI ui = joinTrigger.ui;
                                JoinTriggerUITemplate temp = ui.template;

                                temp.ScreenBG_AbandonPartyAndSoloJoin = OrangeUI;
                                temp.ScreenBG_AlreadyInRoom = OrangeUI;
                                temp.ScreenBG_ChangingGameModeSoloJoin = OrangeUI;
                                temp.ScreenBG_Error = OrangeUI;
                                temp.ScreenBG_InPrivateRoom = OrangeUI;
                                temp.ScreenBG_LeaveRoomAndGroupJoin = OrangeUI;
                                temp.ScreenBG_LeaveRoomAndSoloJoin = OrangeUI;
                                temp.ScreenBG_NotConnectedSoloJoin = OrangeUI;

                                TextMeshPro text = ui.screenText;
                                if (!udTMP.Contains(text))
                                    udTMP.Add(text);
                            }
                            catch { }
                        }
                        PhotonNetworkController.Instance.UpdateTriggerScreens();

                        string[] objectsWithTMPro = {
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText",
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData",
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/Data",
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/FunctionSelect"
                        };
                        foreach (string objectName in objectsWithTMPro)
                        {
                            GameObject obj = GetObject(objectName);
                            if (obj != null)
                            {
                                TextMeshPro text = obj.GetComponent<TextMeshPro>();
                                if (!udTMP.Contains(text))
                                    udTMP.Add(text);
                            }
                            else
                                LogManager.Log("Could not find " + objectName);
                        }

                        Transform forestTransform = GetObject("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard").transform;
                        for (int i = 0; i < forestTransform.transform.childCount; i++)
                        {
                            GameObject v = forestTransform.GetChild(i).gameObject;
                            if ((!v.name.Contains("Board Text") && !v.name.Contains("Scoreboard_OfflineText")) ||
                                !v.activeSelf) continue;
                            TextMeshPro text = v.GetComponent<TextMeshPro>();
                            if (!udTMP.Contains(text))
                                udTMP.Add(text);
                        }

                        hasFoundAllBoards = true;
                    }
                    catch (Exception exc)
                    {
                        LogManager.LogError($"Error with board colors at {exc.StackTrace}: {exc.Message}");
                        hasFoundAllBoards = false;
                    }
                }

                if (computerMonitor == null)
                    computerMonitor = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");

                if (computerMonitor != null)
                    computerMonitor.GetComponent<Renderer>().material = OrangeUI;

                try
                {
                    if (!disableBoardColor)
                        OrangeUI.color = backgroundColor.GetCurrentColor();
                    else
                        OrangeUI.color = new Color32(0, 59, 4, 255);

                    if (motd == null)
                    {
                        GameObject motdObject = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
                        motd = Instantiate(motdObject, motdObject.transform.parent);
                        motdObject.SetActive(false);
                    }

                    TextMeshPro motdTc = motd.GetComponent<TextMeshPro>();
                    if (!udTMP.Contains(motdTc))
                        udTMP.Add(motdTc);

                    motdTc.richText = true;
                    motdTc.fontSize = 70;
                    motdTc.text = "Thanks for using ii's Stupid Menu!";

                    if (doCustomName)
                        motdTc.text = "Thanks for using " + NoRichtextTags(customMenuName) + "!";

                    if (translate)
                        motdTc.text = TranslateText(motdTc.text);

                    if (lowercaseMode)
                        motdTc.text = motdTc.text.ToLower();

                    if (uppercaseMode)
                        motdTc.text = motdTc.text.ToUpper();

                    motdTc.color = textColors[0].GetCurrentColor();
                    motdTc.overflowMode = TextOverflowModes.Overflow;

                    if (motdText == null)
                    {
                        GameObject motdObject = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
                        motdText = Instantiate(motdObject, motdObject.transform.parent);
                        motdObject.SetActive(false);

                        motdText.GetComponent<PlayFabTitleDataTextDisplay>().enabled = false;
                    }

                    TextMeshPro motdTextB = motdText.GetComponent<TextMeshPro>();
                    if (!udTMP.Contains(motdTextB))
                        udTMP.Add(motdTextB);

                    motdTextB.richText = true;
                    motdTextB.fontSize = 100;
                    motdTextB.color = textColors[0].GetCurrentColor();

                    fullModAmount ??= Buttons.buttons.SelectMany(list => list).ToArray().Length;

                    motdTextB.text = string.Format(motdTemplate, PluginInfo.Version, fullModAmount);

                    if (translate)
                        motdTextB.text = TranslateText(motdTextB.text);

                    if (lowercaseMode)
                        motdTextB.text = motdTextB.text.ToLower();

                    if (uppercaseMode)
                        motdTextB.text = motdTextB.text.ToUpper();
                } catch { }

                try
                {
                    Color targetColor = textColors[0].GetCurrentColor();

                    if (disableBoardColor || disableBoardTextColor)
                        targetColor = Color.white;

                    foreach (TextMeshPro txt in udTMP)
                        txt.color = targetColor;
                } catch { }
                #endregion

                #region PC Search Keyboard
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
                                                            if ((Buttons.categoryNames[categoryIndex].Contains("Admin") || Buttons.categoryNames[categoryIndex] == "Mod Givers") && !isAdmin)
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
                {
                    fpsAverageNumber = 1f / Time.unscaledDeltaTime;
                }
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

                    fpsCount.text = textToSet;
                    if (lowercaseMode)
                        fpsCount.text = fpsCount.text.ToLower();

                    if (uppercaseMode)
                        fpsCount.text = fpsCount.text.ToUpper();
                }

                if (animatedTitle && title != null)
                {
                    string targetString = doCustomName ? NoRichtextTags(customMenuName) : "ii's Stupid Menu";
                    int length = (int)Mathf.PingPong(Time.time / 0.25f, targetString.Length);
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
                {
                    keyboardInputObject.text = keyboardInput + (Time.frameCount / 45 % 2 == 0 ? "|" : " ");

                    if (lowercaseMode)
                        keyboardInputObject.text = keyboardInputObject.text.ToLower();

                    if (uppercaseMode)
                        keyboardInputObject.text = keyboardInputObject.text.ToUpper();
                }
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

                if (annoyingMode)
                {
                    OrangeUI.color = new Color32(226, 74, 44, 255);
                    int randy = Random.Range(1, 400);
                    if (randy == 21)
                    {
                        VRRig.LocalRig.PlayHandTapLocal(84, true, 0.4f);
                        VRRig.LocalRig.PlayHandTapLocal(84, false, 0.4f);
                        NotificationManager.SendNotification("<color=grey>[</color><color=magenta>FUN FACT</color><color=grey>]</color> <color=white>" + facts[Random.Range(0, facts.Length - 1)] + "</color>");
                    }
                }

                // Master client notification
                try
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (!disableMasterClientNotifications)
                            if (PhotonNetwork.IsMasterClient && !lastMasterClient)
                                NotificationManager.SendNotification("<color=grey>[</color><color=purple>MASTER</color><color=grey>]</color> You are now master client.");

                        lastMasterClient = PhotonNetwork.IsMasterClient;
                    }
                }
                catch { }

                if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                    Buttons.GetIndex("MasterLabel").overlapText = "You are master client.";
                else
                    Buttons.GetIndex("MasterLabel").overlapText = "You are not master client.";

                // Party kick code (to return back to the main lobby when you're done)
                if (PhotonNetwork.InRoom)
                {
                    if (phaseTwo)
                    {
                        partyLastCode = null;
                        phaseTwo = false;
                        NotificationManager.ClearAllNotifications();
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Successfully " + (waitForPlayerJoin ? "banned" : "kicked") + " " + amountPartying + " party member!</color>");
                        FriendshipGroupDetection.Instance.LeaveParty();
                    }
                    else
                    {
                        if (partyLastCode != null && Time.time > partyTime && (!waitForPlayerJoin || PhotonNetwork.PlayerListOthers.Length > 0))
                        {
                            LogManager.Log("Attempting rejoin");
                            NetworkSystem.Instance.ReturnToSinglePlayer();
                            phaseTwo = true;
                        }
                    }
                }
                else
                {
                    if (phaseTwo)
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

                        if (BackupPreferences)
                        {
                            if (PreferenceBackupCount >= 5)
                            {
                                File.WriteAllText($"{PluginInfo.BaseDirectory}/Backups/{ISO8601().Replace(":", ".")}.txt", Settings.SavePreferencesToText());
                                PreferenceBackupCount = 0;
                            }

                            PreferenceBackupCount++;
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

                try
                {
                    if (PhotonNetwork.InRoom)
                    {
                        foreach (string id in muteIDs)
                        {
                            if (mutedIDs.Contains(id)) continue;
                            string randomName = "gorilla";
                            for (var i = 0; i < 4; i++)
                                randomName += Random.Range(0, 9).ToString();

                            object[] content = {
                                id,
                                true,
                                randomName,
                                NetworkSystem.Instance.LocalPlayer.NickName,
                                true,
                                NetworkSystem.Instance.RoomStringStripped()
                            };

                            PhotonNetwork.RaiseEvent(51, content, new RaiseEventOptions
                            {
                                TargetActors = new[] { -1 },
                                Receivers = ReceiverGroup.All,
                                Flags = new WebFlags(1)
                            }, SendOptions.SendReliable);

                            mutedIDs.Add(id);
                        }
                    }
                }
                catch { }

                ServerPos = ServerPos == Vector3.zero ? ServerSyncPos : Vector3.Lerp(ServerPos, VRRig.LocalRig.SanitizeVector3(ServerSyncPos), VRRig.LocalRig.lerpValueBody * 0.66f);
                ServerLeftHandPos = ServerLeftHandPos == Vector3.zero ? ServerSyncLeftHandPos : Vector3.Lerp(ServerLeftHandPos, VRRig.LocalRig.SanitizeVector3(ServerSyncLeftHandPos), VRRig.LocalRig.lerpValueBody);
                ServerRightHandPos = ServerRightHandPos == Vector3.zero ? ServerSyncRightHandPos : Vector3.Lerp(ServerRightHandPos, VRRig.LocalRig.SanitizeVector3(ServerSyncRightHandPos), VRRig.LocalRig.lerpValueBody);
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
                        int lastPage = pageSize;

                        if (joystickMenuSearching)
                            lastPage++;

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
                                joystickButtonSelected = lastPage - 1;

                            ReloadMenu();
                            joystickDelay = Time.time + 0.2f;
                        }
                        if (js.y < -0.5f)
                        {
                            if (dynamicSounds)
                                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/down.ogg"), buttonClickVolume / 10f);

                            joystickButtonSelected++;
                            joystickButtonSelected %= lastPage;

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

                if (pageScrolling)
                {
                    bool shouldReload = false;
                    if (pageNumber != 0)
                    {
                        pageNumber = 0;
                        shouldReload = true;
                    }

                    Vector2 js = leftJoystick;
                    if (Time.time > scrollDelay)
                    {
                        int lastPage = pageSize;

                        if (joystickMenuSearching)
                            lastPage++;

                        if (js.y > 0.5f)
                        {
                            pageOffset = Mathf.Clamp(pageOffset - 1, 0, Buttons.buttons[currentCategoryIndex].Length - pageSize);

                            shouldReload = true;
                            scrollDelay = Time.time + 0.1f;
                        }
                        if (js.y < -0.5f)
                        {
                            pageOffset = Mathf.Clamp(pageOffset + 1, 0, Buttons.buttons[currentCategoryIndex].Length - pageSize);

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
                        watchShell.GetComponent<Renderer>().material = OrangeUI;
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

                        if (lowercaseMode)
                            watchTextText.text = watchTextText.text.ToLower();

                        if (uppercaseMode)
                            watchTextText.text = watchTextText.text.ToUpper();

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
                } catch { }
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
                    foreach (KeyValuePair<(long, float), GameObject> key in auraPool)
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
                        auraPool.Remove(item);

                    List<(Vector3, Quaternion, Vector3)> toRemoveCube = new List<(Vector3, Quaternion, Vector3)>();
                    foreach (KeyValuePair<(Vector3, Quaternion, Vector3), GameObject> key in cubePool)
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
                        cubePool.Remove(item);

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
                    .Where(button => button.enabled && button.method != null))
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
                            button.method.Invoke();
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

        private static void AddButton(float offset, int buttonIndex, ButtonInfo method)
        {
            if (!method.label)
            {
                GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isKeyboardPc)
                    buttonObject.layer = 2;

                if (themeType == 63 && buttonIndex >= 0)
                    buttonObject.GetComponent<Renderer>().enabled = false;

                buttonObject.GetComponent<BoxCollider>().isTrigger = true;
                buttonObject.transform.parent = menu.transform;
                buttonObject.transform.rotation = Quaternion.identity;

                buttonObject.transform.localScale = thinMenu ? new Vector3(0.09f, 0.9f, buttonDistance * 0.8f) : new Vector3(0.09f, 1.3f, buttonDistance * 0.8f);

                if (longmenu && buttonIndex >= pageSize)
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
                if (shouldOutline && !(themeType == 63 && buttonIndex >= 0))
                    OutlineObj(buttonObject, shouldSwap ? method.enabled : !method.enabled);

                if (lastClickedName != method.buttonText)
                {
                    ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[shouldSwap ^ method.enabled ? 1 : 0];

                    if (joystickMenu && buttonIndex == joystickButtonSelected)
                    {
                        joystickSelectedButton = method.buttonText;

                        if (themeType != 30)
                        {
                            ExtGradient gradient = colorChanger.colors.Clone();
                            gradient.SetColor(0, Color.red);

                            colorChanger.colors = gradient;
                        }
                    }
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(buttonIndex, buttonObject.GetComponent<Renderer>()));

                if (shouldRound)
                    RoundObj(buttonObject);
            }

            Text buttonText = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();

            buttonText.font = activeFont;
            buttonText.text = method.buttonText;

            if (method.overlapText != null)
                buttonText.text = method.overlapText;

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
                {
                    string button = buttonText.text.Split("<color=grey>[</color><color=green>")[1].Split("</color><color=grey>]</color>")[0];
                    buttonText.text = buttonText.text.Split("<color=grey>[</color><color=green>")[0] + "<color=grey>[</color><color=green>" + method.rebindKey + "</color><color=grey>]</color>";
                }
            }
            
            if (translate)
                buttonText.text = TranslateText(buttonText.text, output => ReloadMenu());
            
            if (method.customBind != null)
            {
                if (buttonText.text.Contains("</color><color=grey>]</color>"))
                    buttonText.text = buttonText.text.Replace("</color><color=grey>]</color>", $"/{method.customBind}</color><color=grey>]</color>");
                else
                    buttonText.text += $" <color=grey>[</color><color=green>{method.customBind}</color><color=grey>]</color>";
            }

            if (inputTextColor != "green")
                buttonText.text = buttonText.text.Replace(" <color=grey>[</color><color=green>", $" <color=grey>[</color><color={inputTextColor}>");

            if (lowercaseMode)
                buttonText.text = buttonText.text.ToLower();

            if (uppercaseMode)
                buttonText.text = buttonText.text.ToUpper();

            if (favorites.Contains(method.buttonText))
                buttonText.text += " ✦";

            buttonText.supportRichText = true;
            buttonText.fontSize = 1;

            if (joystickMenu && buttonIndex == joystickButtonSelected && themeType == 30)
                buttonText.color = Color.red;
            else
                buttonText.AddComponent<TextColorChanger>().colors = textColors[method.enabled ? 2 : 1];

            buttonText.alignment = checkMode ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;
            buttonText.fontStyle = activeFontStyle;
            buttonText.resizeTextForBestFit = true;
            buttonText.resizeTextMinSize = 0;

            RectTransform textTransform = buttonText.GetComponent<RectTransform>();
            textTransform.localPosition = Vector3.zero;
            textTransform.sizeDelta = new Vector2(method.incremental && incrementalButtons ? .18f : .2f, .03f * (buttonDistance / 0.1f));
            if (NoAutoSizeText)
                textTransform.sizeDelta = new Vector2(9f, 0.015f);

            textTransform.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(buttonText);
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
            // Fat menu theorem
            // To get the fat position of a button:
            // original x * (0.7 / 0.45) or 1.555555556
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, -0.450f, -0.58f) : new Vector3(0.56f, -0.7f, -0.58f);

            buttonObject.AddComponent<Button>().relatedText = "Search";

            if (shouldOutline)
                OutlineObj(buttonObject, isSearching ^ !swapButtonColors);

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[isSearching ^ !swapButtonColors ? 0 : 1];

            if (joystickMenuSearching && joystickButtonSelected == pageSize)
            {
                joystickSelectedButton = "Search";

                ExtGradient gradient = colorChanger.colors.Clone();
                gradient.SetColor(0, Color.red);

                colorChanger.colors = gradient;
            }

            if (shouldRound)
                RoundObj(buttonObject);

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

            if (outlineText)
                OutlineCanvasObject(searchImage, 0);
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

            if (shouldOutline)
                OutlineObj(buttonObject, infoScreenEnabled ^ swapButtonColors);

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[infoScreenEnabled ^ swapButtonColors ? 0 : 1];

            if (shouldRound)
                RoundObj(buttonObject);

            Image searchImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (debugIcon == null)
                debugIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.debug.png");

            if (debugMat == null)
                debugMat = new Material(searchImage.material);

            searchImage.material = debugMat;
            searchImage.material.SetTexture("_MainTex", debugIcon);
            searchImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = searchImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(searchImage, 2);
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

            if (shouldOutline)
                OutlineObj(buttonObject, !swapButtonColors);

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];

            if (shouldRound)
                RoundObj(buttonObject);

            Image searchImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (donateIcon == null)
                donateIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.donate.png");

            if (donateMat == null)
                donateMat = new Material(searchImage.material);

            searchImage.material = donateMat;
            searchImage.material.SetTexture("_MainTex", donateIcon);
            searchImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = searchImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(searchImage, 3);
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

            if (shouldOutline)
                OutlineObj(buttonObject, !swapButtonColors);

            ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];

            if (shouldRound)
                RoundObj(buttonObject);

            Image searchImage = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (updateIcon == null)
                updateIcon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.update.png");

            if (updateMat == null)
                updateMat = new Material(searchImage.material);

            searchImage.material = updateMat;
            searchImage.material.SetTexture("_MainTex", updateIcon);
            searchImage.AddComponent<ImageColorChanger>().colors = textColors[1];

            RectTransform imageTransform = searchImage.GetComponent<RectTransform>();
            imageTransform.localPosition = Vector3.zero;
            imageTransform.sizeDelta = new Vector2(.03f, .03f);

            imageTransform.localPosition = thinMenu ? new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.6f) : new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.6f);

            imageTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(searchImage, 4);
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
            // Fat menu theorem
            // To get the fat position of a button:
            // original x * (0.7 / 0.45) or 1.555555556
            buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, -0.450f, -0.58f) : new Vector3(0.56f, -0.7f, -0.58f);

            if (offcenteredPosition)
                buttonObject.transform.localPosition += new Vector3(0f, 0.16f, 0f);

            buttonObject.AddComponent<Button>().relatedText = "Global Return";

            if (shouldOutline)
                OutlineObj(buttonObject, !swapButtonColors);

            if (lastClickedName != "Global Return")
            {
                ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                colorChanger.colors = colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];
            }
            else
                CoroutineManager.instance.StartCoroutine(ButtonClick(-99, buttonObject.GetComponent<Renderer>()));

            if (shouldRound)
                RoundObj(buttonObject);

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

            if (outlineText)
                OutlineCanvasObject(returnImage, 1);
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

                buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, buttonDistance * 0.8f);
                buttonObject.transform.localPosition = thinMenu ? new Vector3(0.56f, 0.399f, 0.28f - offset) : new Vector3(0.56f, 0.599f, 0.28f - offset);

                Button button = buttonObject.AddComponent<Button>();
                button.relatedText = method.buttonText;
                button.incremental = true;
                button.positive = increment;

                if (increment)
                    buttonObject.transform.localPosition = new Vector3(buttonObject.transform.localPosition.x, -buttonObject.transform.localPosition.y, buttonObject.transform.localPosition.z);

                if (shouldOutline)
                    OutlineObj(buttonObject, true);

                if (lastClickedName != method.buttonText + (increment ? "+" : "-"))
                {
                    ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(buttonIndex, buttonObject.GetComponent<Renderer>()));

                if (shouldRound)
                    RoundObj(buttonObject);
            }

            RenderIncrementalText(increment, offset);
        }

        public static void RenderIncrementalText(bool increment, float offset)
        {
            Text buttonText = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();

            buttonText.font = activeFont;
            buttonText.text = increment ? "+" : "-";
            buttonText.supportRichText = true;
            buttonText.fontSize = 1;
            buttonText.AddComponent<TextColorChanger>().colors = textColors[1];

            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.fontStyle = activeFontStyle;
            buttonText.resizeTextForBestFit = true;
            buttonText.resizeTextMinSize = 0;

            RectTransform textTransform = buttonText.GetComponent<RectTransform>();
            textTransform.localPosition = Vector3.zero;
            textTransform.sizeDelta = new Vector2(.2f, .03f * (buttonDistance / 0.1f));
            if (NoAutoSizeText)
                textTransform.sizeDelta = new Vector2(9f, 0.015f);

            textTransform.localPosition = thinMenu ? new Vector3(.064f, increment ? -0.12f : 0.12f, .111f - offset / 2.6f) : new Vector3(.064f, increment ? -0.18f : 0.18f, .111f - offset / 2.6f);
            textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(buttonText);
        }

        public static void CreateReference()
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton) ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
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

                if (shouldOutline)
                    OutlineObj(menuBackground, false);

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

                if (shouldRound)
                    RoundObj(menuBackground);
            }

            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            if (hideTextOnCamera)
                canvasObj.layer = 19;

            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = highQualityText ? 2500f : 1000f;

            canvasObj.AddComponent<GraphicRaycaster>();

            title = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
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
            if (translate)
                title.text = TranslateText(title.text, output => ReloadMenu());

            if (lowercaseMode)
                title.text = title.text.ToLower();

            if (uppercaseMode)
                title.text = title.text.ToUpper();

            if (!noPageNumber)
                title.text += $" <color=grey>[</color><color=white>{(pageScrolling ? pageOffset : pageNumber) + 1}</color><color=grey>]</color>";
            
            if (hidetitle)
                title.text = "";

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

            title.supportRichText = true;
            title.fontStyle = activeFontStyle;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform component = title.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            if (NoAutoSizeText)
                component.sizeDelta = new Vector2(0.28f, 0.015f);

            component.localPosition = new Vector3(0.06f, 0f, 0.165f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(title, true);

            Text buildLabel = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            buildLabel.font = activeFont;
            buildLabel.text = $"Build {PluginInfo.Version}";
            if (themeType == 30)
                buildLabel.text = "";

            if (translate)
                buildLabel.text = TranslateText(buildLabel.text, output => ReloadMenu());

            if (lowercaseMode)
                buildLabel.text = buildLabel.text.ToLower();

            if (uppercaseMode)
                buildLabel.text = buildLabel.text.ToUpper();

            buildLabel.fontSize = 1;
            buildLabel.AddComponent<TextColorChanger>().colors = textColors[0];
            buildLabel.supportRichText = true;
            buildLabel.fontStyle = activeFontStyle;
            buildLabel.alignment = TextAnchor.MiddleRight;
            buildLabel.resizeTextForBestFit = true;
            buildLabel.resizeTextMinSize = 0;
            component = buildLabel.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.02f);
            component.position = thinMenu ? new Vector3(0.04f, 0.0f, -0.17f) : new Vector3(0.04f, 0.07f, -0.17f);

            component.rotation = Quaternion.Euler(new Vector3(0f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(buildLabel);
            
            Text fps = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            fps.font = activeFont;

            string textToSet = ftCount ? $"FT: {Mathf.Floor(1f / lastDeltaTime * 10000f) / 10f} ms" : $"FPS: {lastDeltaTime}";
            if (hidetitle && !noPageNumber) textToSet += "      ";
            if (disableFpsCounter) textToSet = "";
            if (hidetitle && !noPageNumber) textToSet += "Page " + (pageNumber + 1);

            fps.text = textToSet;
            if (lowercaseMode)
                fps.text = fps.text.ToLower();

            if (uppercaseMode)
                fps.text = fps.text.ToUpper();

            fps.AddComponent<TextColorChanger>().colors = textColors[0];
            fpsCount = fps;
            fps.fontSize = 1;
            fps.supportRichText = true;
            fps.fontStyle = activeFontStyle;
            fps.alignment = TextAnchor.MiddleCenter;
            fps.horizontalOverflow = HorizontalWrapMode.Overflow;
            fps.resizeTextForBestFit = true;
            fps.resizeTextMinSize = 0;
            RectTransform component2 = fps.GetComponent<RectTransform>();
            component2.localPosition = Vector3.zero;
            component2.sizeDelta = new Vector2(0.28f, 0.02f);
            component2.localPosition = new Vector3(0.06f, 0f, hidetitle ? 0.175f : 0.135f);

            if (NoAutoSizeText)
                component2.sizeDelta = new Vector2(9f, 0.015f);
            component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(fps, true);

            float hkbStartTime = -0.3f;
            if (!disableDisconnectButton)
            {
                AddButton(-0.3f, -1, Buttons.GetIndex("Disconnect"));
                hkbStartTime -= buttonDistance;
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
                    hkbStartTime -= buttonDistance;
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

                searchBoxObject.transform.localScale = thinMenu ? new Vector3(0.09f, 0.9f, buttonDistance * 0.8f) : new Vector3(0.09f, 1.3f, buttonDistance * 0.8f);

                searchBoxObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - buttonOffset * buttonDistance);

                if (shouldOutline)
                    OutlineObj(searchBoxObject, true);

                ColorChanger colorChanger = searchBoxObject.AddComponent<ColorChanger>();
                colorChanger.colors = buttonColors[0];

                if (shouldRound)
                    RoundObj(searchBoxObject);

                keyboardInputObject = new GameObject
                {
                    transform =
                        {
                            parent = canvasObj.transform
                        }
                }.AddComponent<Text>();

                keyboardInputObject.font = activeFont;
                keyboardInputObject.text = keyboardInput + ((Time.time % 1f) > 0.5f ? "|" : "");
                if (lowercaseMode)
                    keyboardInputObject.text = keyboardInputObject.text.ToLower();

                if (uppercaseMode)
                    keyboardInputObject.text = keyboardInputObject.text.ToUpper();

                keyboardInputObject.supportRichText = true;
                keyboardInputObject.fontSize = 1;

                if (joystickMenu && joystickButtonSelected == 0 && themeType == 30)
                    keyboardInputObject.color = Color.red;
                else
                    keyboardInputObject.AddComponent<TextColorChanger>().colors = textColors[1];

                keyboardInputObject.alignment = TextAnchor.MiddleCenter;
                keyboardInputObject.fontStyle = activeFontStyle;
                keyboardInputObject.resizeTextForBestFit = true;
                keyboardInputObject.resizeTextMinSize = 0;

                RectTransform textTransform = keyboardInputObject.GetComponent<RectTransform>();
                textTransform.localPosition = Vector3.zero;
                textTransform.sizeDelta = new Vector2(.2f, .03f * (buttonDistance / 0.1f));
                if (NoAutoSizeText)
                    textTransform.sizeDelta = new Vector2(9f, 0.015f);

                textTransform.localPosition = new Vector3(.064f, 0, .111f - buttonOffset * buttonDistance / 2.6f);
                textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (outlineText)
                    OutlineCanvasObject(keyboardInputObject, true);
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
                                        if ((Buttons.categoryNames[categoryIndex].Contains("Admin") || Buttons.categoryNames[categoryIndex] == "Mod Givers") && !isAdmin)
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
                            .Skip(pageNumber * (pageSize - buttonIndexOffset) + pageOffset)
                            .Take(pageSize - buttonIndexOffset)
                            .ToArray();

                    for (int i = 0; i < renderButtons.Length; i++)
                        AddButton((i + buttonIndexOffset + buttonOffset) * buttonDistance, i, renderButtons[i]);
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

                    if (cannmat == null)
                    {
                        cannmat = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                        {
                            color = Color.white
                        };

                        if (cann == null)
                            cann = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Themes/cannabis.png", "cannabis.png");

                        cannmat.mainTexture = cann;

                        cannmat.SetFloat("_Surface", 1);
                        cannmat.SetFloat("_Blend", 0);
                        cannmat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        cannmat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        cannmat.SetFloat("_ZWrite", 0);
                        cannmat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        cannmat.renderQueue = (int)RenderQueue.Transparent;
                    }
                    particle.GetComponent<Renderer>().material = cannmat;

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
                    else
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
                if (Vector3.Distance(VRKeyboard.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) > menuScale && !leftSecondary)
                {
                    VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                    VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                }
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
                        if (Mouse.current.leftButton.isPressed && !lastclicking)
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

                        lastclicking = Mouse.current.leftButton.isPressed;
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

        public static event Action OnMenuOpened;
        public static void OpenMenu()
        {
            OnMenuOpened?.Invoke();
            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/open.ogg", "Audio/Menu/open.ogg"), buttonClickVolume / 10f);

            CreateMenu();

            if (dynamicAnimations)
                CoroutineManager.instance.StartCoroutine(GrowCoroutine());

            if (joystickMenu) return;
            if (reference == null)
                CreateReference();
        }

        public static event Action OnMenuClosed;
        public static void CloseMenu()
        {
            OnMenuClosed?.Invoke();
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
                        new Vector3(0.09f, thinMenu ? 0.9f : 1.3f, buttonDistance * 0.8f),
                        new Vector3(0.56f, 0f, 0.28f - buttonDistance * (buttonOffset - 2)),
                        new Vector3(0.56f, 0f, 0.28f - buttonDistance * (buttonOffset - 1)),
                        new Vector3(0.064f, 0f, 0.109f - buttonDistance * (buttonOffset - 2) / 2.55f),
                        new Vector3(0.064f, 0f, 0.109f - buttonDistance * (buttonOffset - 1) / 2.55f),
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
            Text promptText = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            promptText.font = activeFont;
            promptText.text = CurrentPrompt.Message;

            string promptImageUrl = ExtractPromptImage(CurrentPrompt.Message);
            if (promptImageUrl != null)
                promptText.text = promptText.text.Replace($"<{promptImageUrl}>", "");

            if (translate)
                promptText.text = TranslateText(promptText.text, output => ReloadMenu());

            if (lowercaseMode)
                promptText.text = promptText.text.ToLower();

            if (uppercaseMode)
                promptText.text = promptText.text.ToUpper();

            promptText.fontSize = 1;
            promptText.lineSpacing = 0.8f;
            promptText.AddComponent<TextColorChanger>().colors = textColors[0];

            promptText.supportRichText = true;
            promptText.fontStyle = activeFontStyle;
            promptText.alignment = TextAnchor.MiddleCenter;
            promptText.resizeTextForBestFit = true;
            promptText.resizeTextMinSize = 0;
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

            if (outlineText)
                OutlineCanvasObject(promptText);

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

                Text text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
                text.font = activeFont;
                text.fontStyle = activeFontStyle;
                text.text = CurrentPrompt.AcceptText;
                text.fontSize = 1;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;

                text.AddComponent<TextColorChanger>().colors = textColors[1];

                if (translate)
                    text.text = TranslateText(text.text, output => ReloadMenu());

                if (lowercaseMode)
                    text.text = text.text.ToLower();

                if (uppercaseMode)
                    text.text = text.text.ToUpper();

                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.sizeDelta = new Vector2(0.2f, 0.03f);

                if (arrowType == 11)
                    textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y * 6f);

                if (NoAutoSizeText)
                    textRect.sizeDelta = new Vector2(9f, 0.015f);

                textRect.localPosition = new Vector3(0.064f, CurrentPrompt.DeclineText != null ? 0.075f : 0f, -0.16f);
                textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (outlineText)
                    OutlineCanvasObject(text);

                if (shouldOutline)
                    OutlineObj(button, true);

                if (shouldRound)
                    RoundObj(button);
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

                Text text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
                text.font = activeFont;
                text.fontStyle = activeFontStyle;
                text.text = CurrentPrompt.DeclineText;
                text.fontSize = 1;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;

                text.AddComponent<TextColorChanger>().colors = textColors[1];

                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.sizeDelta = new Vector2(0.2f, 0.03f);

                if (arrowType == 11)
                    textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y * 6f);

                if (NoAutoSizeText)
                    textRect.sizeDelta = new Vector2(9f, 0.015f);

                textRect.localPosition = new Vector3(0.064f, -0.075f, -0.16f);
                textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (translate)
                    text.text = TranslateText(text.text, output => ReloadMenu());

                if (lowercaseMode)
                    text.text = text.text.ToLower();

                if (uppercaseMode)
                    text.text = text.text.ToUpper();

                if (outlineText)
                    OutlineCanvasObject(text);

                if (shouldOutline)
                    OutlineObj(button, true);

                if (shouldRound)
                    RoundObj(button);
            }
        }

        private static void CreatePageButtonPair(string prevButtonName, string nextButtonName, Vector3 buttonScale, Vector3 prevButtonPos, Vector3 nextButtonPos, Vector3 prevTextPos, Vector3 nextTextPos, ExtGradient color, Vector2? textSize = null)
        {
            GameObject prevButton = AdvancedAddButton(prevButtonName, buttonScale, prevButtonPos, prevTextPos, color, textSize, 0);
            GameObject nextButton = AdvancedAddButton(nextButtonName, buttonScale, nextButtonPos, nextTextPos, color, textSize, 1);

            if (shouldOutline)
            {
                OutlineObj(prevButton, !swapButtonColors);
                OutlineObj(nextButton, !swapButtonColors);
            }

            if (shouldRound)
            {
                RoundObj(prevButton);
                RoundObj(nextButton);
            }
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

            Text text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
            text.font = activeFont;
            text.text = arrowTypes[arrowType][arrowIndex];
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;

            text.AddComponent<TextColorChanger>().colors = textColors[1];

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.sizeDelta = textSize ?? new Vector2(0.2f, 0.03f);

            if (arrowType == 11)
                textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y * 6f);

            if (NoAutoSizeText)
                textRect.sizeDelta = new Vector2(9f, 0.015f);

            textRect.localPosition = textPosition;
            textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (outlineText)
                OutlineCanvasObject(text);

            return button;
        }

        public static void OutlineObj(GameObject toOut, bool shouldBeEnabled)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localPosition = toOut.transform.localPosition;
            gameObject.transform.localScale = toOut.transform.localScale + new Vector3(-0.01f, 0.01f, 0.0075f);

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[shouldBeEnabled ? 1 : 0];

            if (shouldRound)
                RoundObj(gameObject, 0.024f);
        }

        public static void OutlineObjNonMenu(GameObject toOut, bool shouldBeEnabled)
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

        public static readonly Material outlineMat = new Material(Shader.Find("Sprites/Default"));
        public static void OutlineCanvasObject(Text text, bool clamp = false)
        {
            foreach (Vector3 offset in new[] { new Vector3(0f, 1f, 1f), new Vector3(0f, -1f, 1f), new Vector3(0f, 1f, -1f), new Vector3(0f, -1f, -1f) })
            {
                Text newText = Instantiate(text, text.transform.parent, false);
                newText.text = NoColorTags(text.text);

                newText.rectTransform.localPosition = text.rectTransform.localPosition + offset * 0.001f;// + new Vector3(-0.0025f, 0f, 0f);

                newText.material = outlineMat;
                newText.color = Color.black;
                newText.material.renderQueue = text.material.renderQueue - 2;

                if (clamp)
                    newText.AddComponent<ClampText>().targetText = text;
            }
        }

        private static readonly List<Material> imageMaterials = new List<Material>();
        public static void OutlineCanvasObject(Image image, int index)
        {
            while (imageMaterials.Count <= index)
                imageMaterials.Add(null);

            if (imageMaterials[index] == null)
            {
                Material material = new Material(Shader.Find("Sprites/Default"));

                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                material.mainTexture = image.material.mainTexture;

                imageMaterials[index] = material;
            }

            Material targetMaterial = imageMaterials[index];

            foreach (Vector3 offset in new[] { new Vector3(0f, 1f, 1f), new Vector3(0f, -1f, 1f), new Vector3(0f, 1f, -1f), new Vector3(0f, -1f, -1f) })
            {
                Image newImage = Instantiate(image, image.transform.parent, false);

                newImage.rectTransform.localPosition = image.rectTransform.localPosition + offset * 0.001f;
                
                newImage.material = targetMaterial;
                newImage.color = Color.black;

                newImage.material.renderQueue = image.material.renderQueue - 2;
            }
        }

        public static void RoundObj(GameObject toRound, float Bevel = 0.02f)
        {
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
            get {
                if (prompts.Count > 0)
                    return prompts[0];
                else
                    return null;
            }
        }

        public static Material promptMaterial;

        public static void Prompt(string Message, Action Accept = null, Action Decline = null, string AcceptButton = "Yes", string DeclineButton = "No")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = Decline, AcceptText = AcceptButton, DeclineText = DeclineButton, IsText = false });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

        public static void PromptSingle(string Message, Action Accept = null, string AcceptButton = "Yes")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = null, AcceptText = AcceptButton, DeclineText = null, IsText = false });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

        public static void PromptText(string Message, Action Accept = null, Action Decline = null, string AcceptButton = "Yes", string DeclineButton = "No")
        {
            prompts.Add(new PromptData { Message = Message, AcceptAction = Accept, DeclineAction = Decline, AcceptText = AcceptButton, DeclineText = DeclineButton, IsText = true });

            if (menu != null && prompts.Count <= 1)
                ReloadMenu();
        }

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

        public static void RPCProtection()
        {
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

        private static GameObject GunPointer;
        private static LineRenderer GunLine;

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

            if (giveGunTarget != null)
            {
                GunTransform = SwapGunHand ? giveGunTarget.leftHandTransform : giveGunTarget.rightHandTransform;

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
                GunLine.positionCount = 2;
                GunLine.useWorldSpace = true;
                if (smoothLines)
                {
                    GunLine.numCapVertices = 10;
                    GunLine.numCornerVertices = 5;
                }
                GunLine.SetPosition(0, StartPosition);
                GunLine.SetPosition(1, EndPosition);

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
                }
            }

            return (Ray, GunPointer);
        }

        public static bool GetGunInput(bool isShooting)
        {
            if (giveGunTarget != null)
            {
                if (isShooting)
                    return TriggerlessGuns || (SwapGunHand ? giveGunTarget.leftIndex.calcT > 0.5f : giveGunTarget.rightIndex.calcT > 0.5f);
                return GriplessGuns || (SwapGunHand ? giveGunTarget.leftMiddle.calcT > 0.5f : giveGunTarget.rightMiddle.calcT > 0.5f);
            }

            if (isShooting)
                return TriggerlessGuns || (SwapGunHand ? leftTrigger > 0.5f : rightTrigger > 0.5f) || Mouse.current.leftButton.isPressed;
            return GriplessGuns || (SwapGunHand ? leftGrab : rightGrab) || (HardGunLocks && gunLocked && !rightSecondary) || Mouse.current.rightButton.isPressed;
        }

        public static Vector3 GetGunDirection(Transform transform) =>
            new[] { transform.forward, - transform.up, transform == GorillaTagger.Instance.rightHandTransform ? ControllerUtilities.GetTrueRightHand().forward : ControllerUtilities.GetTrueLeftHand().forward, GorillaTagger.Instance.headCollider.transform.forward } [GunDirection];

        public static IEnumerator TranscribeText(string text, Action<AudioClip> onComplete)
        {
            if (Time.time < timeMenuStarted + 5f)
            {
                onComplete?.Invoke(null);
                yield break;
            }

            string fileName = $"{GetSHA256(text)}{(narratorIndex == 0 ? ".wav" : ".mp3")}";
            string directoryPath = $"{PluginInfo.BaseDirectory}/TTS{(narratorName == "Default" ? "" : narratorName)}";
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

                            using UnityWebRequest request = new UnityWebRequest("https://iidk.online/tts", "POST");
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

                    // StreamElements TTS voices
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        {
                            using UnityWebRequest request = UnityWebRequest.Get("https://api.streamelements.com/kappa/v2/speech?voice=" + narratorName + "&text=" + UnityWebRequest.EscapeURL(text));
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + request.error);
                            else
                                File.WriteAllBytes(filePath, request.downloadHandler.data);

                            break;
                        }

                    // TikTok via "gesserit"
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        {
                            Dictionary<int, string> voiceCodenames = new Dictionary<int, string>
                            {
                                { 9, "en_us_001" },
                                { 10, "en_female_grandma" },
                                { 11, "en_male_grinch" },
                                { 12, "en_male_ukneighbor" },
                                { 13, "en_us_ghostface" },
                            };

                            string postData = JsonConvert.SerializeObject(new { text, voice = voiceCodenames[narratorIndex] });

                            using UnityWebRequest request = new UnityWebRequest("https://gesserit.co/api/tiktok-tts", "POST");
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

                            string jsonResponse = request.downloadHandler.text;
                            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

                            if (responseData != null && responseData.ContainsKey("audioUrl"))
                            {
                                string audioUrl = responseData["audioUrl"];

                                if (audioUrl.StartsWith("data:audio/mp3;base64,"))
                                {
                                    string base64Data = audioUrl["data:audio/mp3;base64,".Length..];

                                    try
                                    {
                                        byte[] audioBytes = Convert.FromBase64String(base64Data);

                                        if (!filePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                                            filePath = Path.ChangeExtension(filePath, ".mp3");

                                        File.WriteAllBytes(filePath, audioBytes);
                                        LogManager.Log("TTS audio saved to: " + filePath);
                                    }
                                    catch (FormatException ex)
                                    {
                                        LogManager.LogError("Invalid base64 data in audioUrl: " + ex.Message);
                                        onComplete?.Invoke(null);
                                    }
                                }
                                else
                                {
                                    LogManager.LogError("Unexpected audioUrl format: " + audioUrl);
                                    onComplete?.Invoke(null);
                                }
                            }
                            else
                            {
                                LogManager.LogError("No audioUrl found in response");
                                onComplete?.Invoke(null);
                                yield break;
                            }

                            break;
                        }
                }
            }

            onComplete?.Invoke(LoadSoundFromFile($"{directoryPath[$"{PluginInfo.BaseDirectory}/".Length..]}/{fileName}"));
        }

        public static void NarrateText(string text) =>
            CoroutineManager.instance.StartCoroutine(TranscribeText(text, (audio) => Play2DAudio(audio, buttonClickSound / 10f)));

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

        public static SnowballThrowable[] snowballs = { };
        public static Dictionary<string, SnowballThrowable> snowballDict;
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

        public static void ClearType<T>() where T : Object
        {
            Type type = typeof(T);

            typePool.Remove(type);
        }

        private static VRRig rigTarget;
        private static float rigTargetChange;
        public static VRRig GetCurrentTargetRig(float targetChangeDelay = 1f)
        {
            if (Time.time > rigTargetChange || !GorillaParent.instance.vrrigs.Contains(rigTarget))
            {
                rigTargetChange = Time.time + targetChangeDelay;
                rigTarget = GetRandomVRRig(false);
            }

            return rigTarget;
        }

        public static BuilderTable GetBuilderTable()
        {
            BuilderTable.TryGetBuilderTableForZone(VRRig.LocalRig.zoneEntity.currentZone, out BuilderTable table);
            return table;
        }

        private static readonly Dictionary<string, GameObject> objectPool = new Dictionary<string, GameObject>();
        public static GameObject GetObject(string find)
        {
            if (objectPool.TryGetValue(find, out GameObject go))
                return go;

            GameObject tgo = GameObject.Find(find);
            if (tgo != null)
                objectPool.Add(find, tgo);

            return tgo;
        }

        public static bool PlayerIsTagged(VRRig Player)
        {
            List<NetPlayer> infectedPlayers = InfectedList();
            NetPlayer targetPlayer = GetPlayerFromVRRig(Player);

            return infectedPlayers.Contains(targetPlayer);
        }

        public static bool PlayerIsLocal(VRRig Player) => 
            Player.isLocal || Player == GhostRig;

        // Credits to zvbex for the 'FIRST LOGIN' concat check
        // Credits to HanSolo1000Falcon/WhoIsThatMonke for improved checks

        public static bool PlayerIsSteam(VRRig Player)
        {
            string concat = Player.concatStringOfCosmeticsAllowed;
            int customPropsCount = NetPlayerToPlayer(GetPlayerFromVRRig(Player)).CustomProperties.Count;

            if (concat.Contains("S. FIRST LOGIN")) return true;
            if (concat.Contains("FIRST LOGIN") || customPropsCount >= 2) return true;

            return false;
        }

        public static bool ShouldBypassChecks(NetPlayer Player) =>
             Player == NetworkSystem.Instance.LocalPlayer || FriendManager.IsPlayerFriend(Player) || ServerData.Administrators.ContainsKey(Player.UserId);

        // Credits to The-Graze/WhoIsTalking for the color detection
        public static Color GetPlayerColor(VRRig Player)
        {
            if (Buttons.GetIndex("Follow Player Colors").enabled)
                return Player.playerColor;

            if (Player.bodyRenderer.cosmeticBodyType == GorillaBodyType.Skeleton)
                return Color.green;

            switch (Player.setMatIndex)
            {
                case 1:
                    return Color.red;
                case 2:
                case 11:
                    return new Color32(255, 128, 0, 255);
                case 3:
                case 7:
                    return Color.blue;
                case 12:
                    return Color.green;
                default:
                    return Player.playerColor;
            }
        }

        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer>();

            if (!PhotonNetwork.InRoom)
                return infected;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.isCurrentlyTag)
                        infected.Add(tagManager.currentIt);
                    else
                        infected.AddRange(tagManager.currentInfected);
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.isCurrentlyTag)
                        infected.Add(ghostManager.currentIt);
                    else
                        infected.AddRange(ghostManager.currentInfected);
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;

                    infected.AddRange(paintbrawlManager.playerLives.Where(element => element.Value <= 0).Select(element => element.Key).ToArray().Select(deadPlayer => PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(deadPlayer)).Select(dummy => (NetPlayer)dummy));

                    if (!infected.Contains(NetworkSystem.Instance.LocalPlayer))
                        infected.Add(NetworkSystem.Instance.LocalPlayer);

                    break;
            }

            return infected;
        }

        public static void AddInfected(NetPlayer plr)
        {
            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.isCurrentlyTag)
                        tagManager.ChangeCurrentIt(plr);
                    else if (!tagManager.currentInfected.Contains(plr))
                        tagManager.AddInfectedPlayer(plr);
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.isCurrentlyTag)
                        ghostManager.ChangeCurrentIt(plr);
                    else if (!ghostManager.currentInfected.Contains(plr))
                        ghostManager.AddInfectedPlayer(plr);
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 0;

                    break;
            }
        }

        public static void RemoveInfected(NetPlayer plr)
        {
            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    switch (tagManager.isCurrentlyTag)
                    {
                        case true when tagManager.currentIt == plr:
                            tagManager.currentIt = null;
                            break;
                        case false when tagManager.currentInfected.Contains(plr):
                            tagManager.currentInfected.Remove(plr);
                            break;
                    }
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    switch (ghostManager.isCurrentlyTag)
                    {
                        case true when ghostManager.currentIt == plr:
                            ghostManager.currentIt = null;
                            break;
                        case false when ghostManager.currentInfected.Contains(plr):
                            ghostManager.currentInfected.Remove(plr);
                            break;
                    }
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 3;

                    break;
            }
        }

        public static void AddRock(NetPlayer plr)
        {
            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    tagManager.ChangeCurrentIt(plr);

                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    ghostManager.ChangeCurrentIt(plr);

                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 0;

                    break;
            }
        }

        public static void RemoveRock(NetPlayer plr)
        {
            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.currentIt == plr)
                        tagManager.ChangeCurrentIt(null);

                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.currentIt == plr)
                        ghostManager.ChangeCurrentIt(null);

                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 3;

                    break;
            }
        }

        // SteamVR bug causes teleporting of the player to the center of your playspace
        public static Vector3 World2Player(Vector3 world) => 
            world - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position;

        // True left and right hand get the exact position and rotation of the middle of the hand
        [Obsolete("TrueLeftHand is obsolete. Use ControllerUtilities.GetTrueLeftHand instead.")]
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand() =>
            ControllerUtilities.GetTrueLeftHand();

        [Obsolete("TrueRightHand is obsolete. Use ControllerUtilities.GetTrueRightHand instead.")]
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand() =>
            ControllerUtilities.GetTrueRightHand();

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

        public static readonly Dictionary<(long, float), GameObject> auraPool = new Dictionary<(long, float), GameObject>();
        public static void VisualizeAura(Vector3 position, float range, Color color, long? indexId = null, float alpha = 0.25f)
        {
            long index = indexId ?? BitPackUtils.PackWorldPosForNetwork(position);
            var key = (index, range);

            if (!auraPool.TryGetValue(key, out GameObject visualizeGO))
            {
                visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(visualizeGO.GetComponent<Collider>());

                auraPool.Add(key, visualizeGO);
            }

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = new Vector3(range, range, range);

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;
        }

        public static readonly Dictionary<(Vector3, Quaternion, Vector3), GameObject> cubePool = new Dictionary<(Vector3, Quaternion, Vector3), GameObject>();
        public static void VisualizeCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float alpha = 0.25f)
        {
            var key = (position, rotation, scale);

            if (!cubePool.TryGetValue(key, out GameObject visualizeGO))
            {
                visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(visualizeGO.GetComponent<Collider>());

                cubePool.Add(key, visualizeGO);
            }

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = scale;
            visualizeGO.transform.rotation = rotation;

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;
        }

        public static GameObject VisualizeAuraObject(Vector3 position, float range, Color color, float alpha = 0.25f)
        {
            GameObject visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(visualizeGO.GetComponent<Collider>());

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = new Vector3(range, range, range);

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;

            return visualizeGO;
        }

        public static GameObject VisualizeCubeObject(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float alpha = 0.25f)
        {
            GameObject visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(visualizeGO.GetComponent<Collider>());

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = scale;
            visualizeGO.transform.rotation = rotation;

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;

            return visualizeGO;
        }

        public static GameObject audioMgr;
        public static void Play2DAudio(AudioClip sound, float volume = 1f)
        {
            if (audioMgr == null)
            {
                audioMgr = new GameObject("2DAudioMgr");
                AudioSource temp = audioMgr.AddComponent<AudioSource>();
                temp.spatialBlend = 0f;
            }
            AudioSource ausrc = audioMgr.GetComponent<AudioSource>();
            ausrc.volume = volume;
            ausrc.PlayOneShot(sound);
        }

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

        public static readonly Dictionary<string, float> waitingForTranslate = new Dictionary<string, float>();
        public static readonly Dictionary<string, string> translateCache = new Dictionary<string, string>();
        public static string TranslateText(string input, Action<string> onTranslated = null)
        {
            if (translateCache.TryGetValue(input, out var text))
                return text;
            if (!waitingForTranslate.ContainsKey(input))
            {
                waitingForTranslate.Add(input, Time.time + 10f);
                CoroutineManager.instance.StartCoroutine(GetTranslation(input, onTranslated));
            } else
            {
                if (!(Time.time > waitingForTranslate[input])) return "Loading...";
                waitingForTranslate.Remove(input);

                waitingForTranslate.Add(input, Time.time + 10f);
                CoroutineManager.instance.StartCoroutine(GetTranslation(input, onTranslated));
            }

            return "Loading...";
        }

        public static IEnumerator GetTranslation(string text, Action<string> onTranslated = null)
        {
            if (translateCache.TryGetValue(text, out var value))
            {
                onTranslated?.Invoke(value);

                yield break;
            }

            string fileName = GetSHA256(text) + ".txt";
            string directoryPath = $"{PluginInfo.BaseDirectory}/TranslationData{language.ToUpper()}";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, fileName);
            string translation = null;

            if (!File.Exists(filePath))
            {
                string postData = "{\"text\": \"" + text.Replace("\n", "").Replace("\r", "").Replace("\"", "") + "\", \"lang\": \"" + language + "\"}";

                using UnityWebRequest request = new UnityWebRequest("https://iidk.online/translate", "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    Match match = Regex.Match(json, "\"translation\"\\s*:\\s*\"(.*?)\"");
                    if (match.Success)
                    {
                        translation = match.Groups[1].Value;
                        File.WriteAllText(filePath, translation);
                    }
                }
            }
            else
                translation = File.ReadAllText(filePath);

            if (translation != null)
            {
                translateCache.Add(text, translation);

                onTranslated?.Invoke(translation);
            }
        }

        public static string FormatUnix(int seconds)
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            string timeString = $"{minutes:D2}:{remainingSeconds:D2}";

            return timeString;
        }

        public static string GetSHA256(string input)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in bytes)
                stringBuilder.Append(b.ToString("x2"));

            return stringBuilder.ToString();
        }

        public static string ISO8601()
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

        public static string RichtextGradient(string input, GradientColorKey[] Colors)
        {
            Gradient bg = new Gradient
            {
                colorKeys = Colors
            };

            char[] chars = input.ToCharArray();
            string finalOutput = "";
            for (int i = 0; i < chars.Length; i++)
            {
                char character = chars[i];
                Color characterColor = bg.Evaluate((Time.time / 2f + i / 25f) % 1f);
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

        // To get the optimal delay from call limiter
        public static float GetCallLimiterDelay(CallLimiter limiter) =>
            limiter.timeCooldown / limiter.callHistoryLength;

        public static void EventReceived(EventData data)
        {
            /*
            Incase I don't remember, very high chance:
                PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false) to get the player
                data.Code is the event ID, 200 is for RPCs and 50 is for p2p reports
                PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())] is for the RPC name if you shall dare receive it
                (object[])((Hashtable)data.CustomData)[(byte)4] to decrypt the args from RPCs
                (string)((object[])data.CustomData)[(byte)0] to decrypt the args from events

            Thanks for listening to my ted talk
            */

            try
            {
                if (data.Code == 200)
                {
                    string rpcName = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[5].ToString())];
                    object[] args = (object[])((Hashtable)data.CustomData)[4];
                    switch (rpcName)
                    {
                        case "RPC_PlayHandTap" when AntiOculusReport:
                            if ((int)args[0] == 67)
                            {
                                VRRig target = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender));
                                if (Vector3.Distance(target.leftHandTransform.position, target.rightHandTransform.position) < 0.1f)
                                    Safety.AntiReportFRT(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender));
                            }
                            break;
                        case "RPC_PlayHandTap" when Safety.smartAntiReport:
                            if ((int)args[0] == 67)
                            {
                                Safety.buttonClickTime = Time.frameCount;
                                Safety.buttonClickPlayer = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender).UserId;
                            }
                            break;
                        case "RPC_PlayHandTap" when Fun.keyboardTrackerEnabled:
                            if ((int)args[0] == 66)
                            {
                                VRRig target = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender));

                                Transform keyboardTransform = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)").transform;
                                if (Vector3.Distance(target.transform.position, keyboardTransform.position) < 3f)
                                {
                                    string handPath = (bool)args[1]
                                     ? "GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/f_index.01.L/f_index.02.L/f_index.03.L/f_index.03.L_end"
                                     : "GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/f_index.01.R/f_index.02.R/f_index.03.R/f_index.03.R_end";

                                    Vector3 position = target.gameObject.transform.Find(handPath).position;

                                    GameObject keysParent = keyboardTransform.Find("Buttons/Keys").gameObject;
                                    float minimalDist = float.MaxValue;
                                    string closestKey = "[Null]";

                                    foreach (Transform child in keysParent.transform)
                                    {
                                        float dist = Vector3.Distance(child.position, position);
                                        if (dist < minimalDist)
                                        {
                                            minimalDist = dist;
                                            closestKey = ToTitleCase(child.name);
                                        }
                                    }

                                    if (closestKey.Length > 1)
                                        closestKey = "[" + closestKey + "]";

                                    bool isKeyLogged = false;
                                    for (int i = 0; i < Fun.keyLogs.Count; i++)
                                    {
                                        object[] keyLog = Fun.keyLogs[i];
                                        if ((VRRig)keyLog[0] == target)
                                        {
                                            isKeyLogged = true;

                                            string currentText = (string)keyLog[1];

                                            if (closestKey.Contains("Delete"))
                                                Fun.keyLogs[i][1] = currentText.Length == 0 ? currentText : currentText[..^1];
                                            else
                                                Fun.keyLogs[i][1] = currentText + closestKey;

                                            Fun.keyLogs[i][2] = Time.time + 5f;
                                            break;
                                        }
                                    }

                                    if (!isKeyLogged)
                                    {
                                        if (!closestKey.Contains("Delete"))
                                            Fun.keyLogs.Add(new object[] { target, closestKey, Time.time + 5f });
                                    }
                                }
                            }
                            break;
                    }
                }
            } catch { }
        }

        public static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (disableBoardColor) return;

            if (!BoardInformations.TryGetValue(scene.name, out var config)) return;

            CreateObjectBoard(scene.name, config.GameObjectPath, config.Position, config.Rotation, config.Scale);
        }

        private readonly struct BoardInformation
        {
            public readonly string GameObjectPath;
            public readonly Vector3 Position;
            public readonly Vector3 Rotation;
            public readonly Vector3 Scale;

            public BoardInformation(string path, Vector3 pos, Vector3 rot, Vector3 scale)
            {
                GameObjectPath = path;
                Position = pos;
                Rotation = rot;
                Scale = scale;
            }
        }

        private static readonly Dictionary<string, BoardInformation> BoardInformations = new Dictionary<string, BoardInformation>
        {
            ["Canyon2"] = new BoardInformation(
                "Canyon/CanyonScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-24.5019f, -28.7746f, 0.1f),
                new Vector3(270f, 0f, 0f),
                new Vector3(21.5946f, 1f, 22.1782f)
            ),
            ["Skyjungle"] = new BoardInformation(
                "skyjungle/UI/Scoreboard/GorillaScoreBoard",
                new Vector3(-21.2764f, -32.1928f, 0f),
                new Vector3(270.2987f, 0.2f, 359.9f),
                new Vector3(21.6f, 0.1f, 20.4909f)
            ),
            ["Mountain"] = new BoardInformation(
                "Mountain/MountainScoreboardAnchor/GorillaScoreBoard",
                Vector3.zero,
                Vector3.zero,
                Vector3.one
            ),
            ["Metropolis"] = new BoardInformation(
                "MetroMain/ComputerArea/Scoreboard/GorillaScoreBoard",
                new Vector3(-25.1f, -31f, 0.1502f),
                new Vector3(270.1958f, 0.2086f, 0f),
                new Vector3(21f, 102.9727f, 21.4f)
            ),
            ["Bayou"] = new BoardInformation(
                "BayouMain/ComputerArea/GorillaScoreBoardPhysical",
                new Vector3(-28.3419f, -26.851f, 0.3f),
                new Vector3(270f, 0f, 0f),
                new Vector3(21.3636f, 38f, 21f)
            ),
            ["Beach"] = new BoardInformation(
                "BeachScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -33.7126f, 0.1f),
                new Vector3(270.056f, 0f, 0f),
                new Vector3(21.2f, 2f, 21.6f)
            ),
            ["Cave"] = new BoardInformation(
                "Cave_Main_Prefab/CrystalCaveScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -33.7126f, 0.1f),
                new Vector3(270.056f, 0f, 0f),
                new Vector3(21.2f, 2f, 21.6f)
            ),
            ["Rotating"] = new BoardInformation(
                "RotatingPermanentEntrance/UI (1)/RotatingScoreboard/RotatingScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -33.7126f, 0.1f),
                new Vector3(270.056f, 0f, 0f),
                new Vector3(21.2f, 2f, 21.6f)
            ),
            ["MonkeBlocks"] = new BoardInformation(
                "Environment Objects/MonkeBlocksRoomPersistent/AtticScoreBoard/AtticScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -24.5091f, 0.57f),
                new Vector3(270.1856f, 0.1f, 0f),
                new Vector3(21.6f, 1.2f, 20.8f)
            ),
            ["Basement"] = new BoardInformation(
                "Basement/BasementScoreboardAnchor/GorillaScoreBoard/",
                new Vector3(-22.1964f, -24.5091f, 0.57f),
                new Vector3(270.1856f, 0.1f, 0f),
                new Vector3(21.6f, 1.2f, 20.8f)
            )
        };

        public static void CreateObjectBoard(string scene, string gameObject, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
        {
            if (objectBoards.TryGetValue(scene, out GameObject existingBoard))
            {
                Destroy(existingBoard);
                objectBoards.Remove(scene);
            }

            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Plane);
            board.transform.parent = GetObject(gameObject).transform;
            board.transform.localPosition = position ?? new Vector3(-22.1964f, -34.9f, 0.57f);
            board.transform.localRotation = Quaternion.Euler(rotation ?? new Vector3(270f, 0f, 0f));
            board.transform.localScale = scale ?? new Vector3(21.6f, 2.4f, 22f);

            Destroy(board.GetComponent<Collider>());
            board.GetComponent<Renderer>().material = OrangeUI;

            objectBoards.Add(scene, board);
        }

        public static bool inRoomStatus;

        public static void OnJoinRoom()
        {
            if (inRoomStatus)
                return;

            inRoomStatus = true;
            lastRoom = PhotonNetwork.CurrentRoom.Name;

            if (!disableRoomNotifications)
                NotificationManager.SendNotification("<color=grey>[</color><color=blue>JOIN ROOM</color><color=grey>]</color> Room Code: " + lastRoom + "");

            RPCProtection();
        }

        public static void OnLeaveRoom()
        {
            if (!inRoomStatus)
                return;

            inRoomStatus = false;

            if (clearNotificationsOnDisconnect)
                NotificationManager.ClearAllNotifications();

            if (!disableRoomNotifications)
                NotificationManager.SendNotification("<color=grey>[</color><color=blue>LEAVE ROOM</color><color=grey>]</color> Room Code: " + lastRoom + "");
            RPCProtection();
            lastMasterClient = false;
        }

        public static string CleanPlayerName(string input, int length = 12)
        {
            input = NoRichtextTags(input);

            if (input.Length > length)
                input = input[..(length - 1)];

            return input;
        }

        public static void OnPlayerJoin(NetPlayer Player)
        {
            if (Player != NetworkSystem.Instance.LocalPlayer && !disablePlayerNotifications)
                NotificationManager.SendNotification($"<color=grey>[</color><color=green>JOIN</color><color=grey>]</color> Name: {CleanPlayerName(Player.NickName)}");
        }

        public static void OnPlayerLeave(NetPlayer Player)
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

        public static void OnSerialize()
        {
            ServerSyncPos = VRRig.LocalRig?.transform.position ?? ServerSyncPos;
            ServerSyncLeftHandPos = VRRig.LocalRig?.leftHand?.rigTarget?.transform.position ?? ServerSyncLeftHandPos;
            ServerSyncRightHandPos = VRRig.LocalRig?.rightHand?.rigTarget?.transform.position ?? ServerSyncRightHandPos;
        }

        public static readonly Dictionary<VRRig, int> playerPing = new Dictionary<VRRig, int>();

        public static void OnPlayerSerialize(VRRig rig) =>
            playerPing[rig] = GetTruePing(rig);

        public static int GetTruePing(VRRig rig)
        {
            double ping = Math.Abs((rig.velocityHistoryList[0].time - PhotonNetwork.Time) * 1000);
            int safePing = (int)Math.Clamp(Math.Round(ping), 0, int.MaxValue);

            return safePing;
        }

        public static bool onlySerializeNecessary;
        public static void MassSerialize(bool exclude = false, PhotonView[] viewFilter = null, int timeOffset = 0)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (viewFilter != null)
            {
                NonAllocDictionary<int, PhotonView> photonViewList = PhotonNetwork.photonViewList;
                List<PhotonView> viewsToSerialize = new List<PhotonView>();

                if (onlySerializeNecessary)
                    photonViewList = new NonAllocDictionary<int, PhotonView> { { 0, GorillaTagger.Instance.myVRRig.GetView } };

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
                    SendSerialize(view, null, timeOffset);
            }
        }

        public static void SendSerialize(PhotonView pv, RaiseEventOptions options = null, int timeOffset = 0)
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

            PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206 : 201), objectUpdate, finalOptions,
                reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);

            serializeViewBatch.Clear();
        }

        public static void TeleportPlayer(Vector3 pos, bool keepVelocity = false) // Prevents your hands from getting stuck on trees
        {
            GTPlayer.Instance.TeleportTo(World2Player(pos), GTPlayer.Instance.transform.rotation, keepVelocity);
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
            GTPlayer.Instance.Turn(rotation.y - GTPlayer.Instance.mainCamera.transform.eulerAngles.y);

        public static void SetRotation(float rotation) =>
           GTPlayer.Instance.Turn(rotation - GTPlayer.Instance.mainCamera.transform.eulerAngles.y);

        public static int[] AllActorNumbers() =>
            PhotonNetwork.PlayerList.Select(plr => plr.ActorNumber).ToArray();

        public static int[] AllActorNumbersExcept(int actorNumber) =>
            AllActorNumbersExcept(new[] { actorNumber });

        public static int[] AllActorNumbersExcept(int[] actorNumbers) =>
            PhotonNetwork.PlayerList.Where(plr => !actorNumbers.Contains(plr.ActorNumber)).Select(plr => plr.ActorNumber).ToArray();

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

                if (buttonClickIndex <= 3 || buttonClickIndex == 11)
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
                        { 22, "windows" }
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

        public static void Toggle(string buttonText, bool fromMenu = false, bool ignoreForce = false)
        {
            int lastPage = (Buttons.buttons[currentCategoryIndex].Length + pageSize - 1) / pageSize - 1;
            if (currentCategoryName == "Favorite Mods")
                lastPage = (favorites.Count + pageSize - 1) / pageSize - 1;
            
            if (currentCategoryName == "Enabled Mods")
            {
                List<string> enabledMods = new List<string> { "Exit Enabled Mods" };
                int categoryIndex = 0;
                foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                {
                    enabledMods.AddRange(from v in buttonlist where v.enabled && (!hideSettings || !Buttons.categoryNames[categoryIndex].Contains("Settings")) && (!hideMacros || !Buttons.categoryNames[categoryIndex].Contains("Macro")) select v.buttonText);
                    categoryIndex++;
                }
                lastPage = (enabledMods.Count + pageSize - 1) / pageSize - 1;
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
                            string buttonTextt = v.buttonText;
                            if (v.overlapText != null)
                                buttonTextt = v.overlapText;

                            if (buttonTextt.ClearTags().Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
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
                                if ((Buttons.categoryNames[categoryIndex].Contains("Admin") || Buttons.categoryNames[categoryIndex] == "Mod Givers") && !isAdmin)
                                    continue;
                                
                                string buttonTextt = v.buttonText;
                                if (v.overlapText != null)
                                    buttonTextt = v.overlapText;

                                if (buttonTextt.Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
                                    searchedMods.Add(v);
                            }
                            catch { }
                        }
                        categoryIndex++;
                    }
                }

                // ReSharper disable once PossibleLossOfFraction
                lastPage = (int)Mathf.Ceil(searchedMods.ToArray().Length / (pageSize - 1));
            }

            switch (buttonText)
            {
                case "PreviousPage":
                {
                    if (dynamicAnimations)
                        lastClickedName = "PreviousPage";

                    pageNumber--;
                    if (pageNumber < 0)
                        pageNumber = lastPage;
                    break;
                }
                case "NextPage":
                {
                    if (dynamicAnimations)
                        lastClickedName = "NextPage";

                    pageNumber++;
                    pageNumber %= lastPage + 1;
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
                                            NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully binded mod to " + BindInput + ".");
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
                                                NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully rebinded mod to " + BindInput + ".");
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
                            default:
                            {
                                if (target.isTogglable)
                                {
                                    target.enabled = !target.enabled;
                                    if (target.enabled)
                                    {
                                        if (fromMenu)
                                            NotificationManager.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                                        
                                        if (target.enableMethod != null)
                                            try { target.enableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(
                                                $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                                    }
                                    else
                                    {
                                        if (fromMenu)
                                            NotificationManager.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
                                        
                                        if (target.disableMethod != null)
                                            try { target.disableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(
                                                $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                                    }
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

        public static void Toggle(ButtonInfo buttonInfo, bool fromMenu = false, bool ignoreForce = false) =>
            Toggle(buttonInfo.buttonText, fromMenu, ignoreForce);

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

                if (increment)
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>INCREMENT</color><color=grey>]</color> " + target.toolTip);
                    if (target.enableMethod != null)
                        try { target.enableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(
                            $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                }
                else
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>DECREMENT</color><color=grey>]</color> " + target.toolTip);
                    if (target.disableMethod != null)
                        try { target.disableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(
                            $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                }
            }
            ReloadMenu();
        }

        public static IEnumerator DelayLoadPreferences()
        {
            yield return new WaitForSeconds(1);
            Settings.LoadPreferences();
        }

        public static void OnLaunch()
        {
            timeMenuStarted = Time.time;
            IsSteam = PlayFabAuthenticator.instance.platform;

            if (!Font.GetOSInstalledFontNames().Contains("Agency FB"))
                AgencyFB = LoadAsset<Font>("Agency");

            if (Plugin.FirstLaunch)
                Prompt("It seems like this is your first time using the menu. Would you like to watch a quick tutorial to get to know how to use it?", Settings.ShowTutorial);
            else
                acceptedDonations = File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_HideDonationButton.txt");

            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
            SceneManager.sceneLoaded += SceneLoaded;

            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinRoom;
            NetworkSystem.Instance.OnReturnedToSinglePlayer += OnLeaveRoom;

            NetworkSystem.Instance.OnPlayerJoined += OnPlayerJoin;
            NetworkSystem.Instance.OnPlayerLeft += OnPlayerLeave;

            SerializePatch.OnSerialize += OnSerialize;
            PlayerSerializePatch.OnPlayerSerialize += OnPlayerSerialize;

            CrystalMaterial = GetObject("Environment Objects/LocalObjects_Prefab/ForestToCave/C_Crystal_Chunk")?.GetComponent<Renderer>()?.material;
            TryOnRoom = GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom");

            string ConsoleGUID = "goldentrophy_Console"; // Do not change this, it's used to get other instances of Console
            GameObject ConsoleObject = GameObject.Find(ConsoleGUID);

            if (ConsoleObject == null)
                ConsoleObject = new GameObject(ConsoleGUID);
            else
            {
                if (ConsoleObject.GetComponents<Component>()
                    .Select(c => c.GetType().GetField("ConsoleVersion",
                        BindingFlags.Public |
                        BindingFlags.Static |
                        BindingFlags.FlattenHierarchy))
                    .Where(f => f != null && f.IsLiteral && !f.IsInitOnly)
                    .Select(f => f.GetValue(null))
                    .FirstOrDefault() is string consoleVersion)
                {
                    if (ServerData.VersionToNumber(consoleVersion) < ServerData.VersionToNumber(Console.ConsoleVersion))
                    {
                        Destroy(ConsoleObject);
                        ConsoleObject = new GameObject(ConsoleGUID);
                    }
                }
            }

            ConsoleObject.AddComponent<Console>();

            if (ServerData.ServerDataEnabled)
            {
                ConsoleObject.AddComponent<ServerData>();
                ConsoleObject.AddComponent<FriendManager>();
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
            catch (Exception exc) { LogManager.LogError(
                $"Error with Settings.LoadPlugins() at {exc.StackTrace}: {exc.Message}"); }

            try
            {
                Sound.LoadSoundboard(false);
            }
            catch (Exception exc) { LogManager.LogError(
                $"Error with Sound.LoadSoundboard() at {exc.StackTrace}: {exc.Message}"); }

            try
            {
                Movement.LoadMacros();
            }
            catch (Exception exc) { LogManager.LogError(
                $"Error with Movement.LoadMacros() at {exc.StackTrace}: {exc.Message}"); }

            loadPreferencesTime = Time.time;
            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt"))
            {
                try
                {
                    Settings.LoadPreferences();
                } catch
                {
                    CoroutineManager.instance.StartCoroutine(DelayLoadPreferences());
                }
            }

            if (PatchHandler.PatchErrors > 0)
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> {PatchHandler.PatchErrors} patch{(PatchHandler.PatchErrors > 1 ? "es" : "")} failed to initialize. Please report this as an issue to the GitHub repository.", 10000);
        }

        public static void UnloadMenu()
        {
            Settings.Panic();
            Settings.DisableBoardColors();

            PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;

            NetworkSystem.Instance.OnJoinedRoomEvent -= OnJoinRoom;
            NetworkSystem.Instance.OnReturnedToSinglePlayer -= OnLeaveRoom;

            NetworkSystem.Instance.OnPlayerJoined -= OnPlayerJoin;
            NetworkSystem.Instance.OnPlayerLeft -= OnPlayerLeave;

            if (Console.instance != null)
                Destroy(Console.instance.gameObject);

            if (NotificationManager.Instance != null)
            {
                Destroy(NotificationManager.Instance.HUDObj);
                Destroy(NotificationManager.Instance.HUDObj2);
                Destroy(NotificationManager.ModText);
                Destroy(NotificationManager.NotifiText);
                Destroy(NotificationManager.Instance.gameObject);
            }

            if (VRKeyboard != null)
            {
                Destroy(VRKeyboard);
                motd = null;
            }

            if (motd != null)
            {
                Destroy(motd);
                motd = null;
            }

            if (motdText != null)
            {
                Destroy(motdText);
                motdText = null;
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
        public static bool crystallizemenu;
        public static bool zeroGravityMenu;
        public static bool menuCollisions;
        public static bool dropOnRemove = true;
        public static bool shouldOutline;
        public static bool outlineText;
        public static bool innerOutline;
        public static bool smoothLines;
        public static bool shouldRound;
        public static bool lastclicking;
        public static bool openedwithright;
        public static bool oneHand;

        public static int _pageSize = 8;
        public static int pageSize
        {
            get => _pageSize - buttonOffset;
            set => _pageSize = value;
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

        private static VRRig _giveGunTarget;
        public static VRRig giveGunTarget
        {
            get
            {
                if (!GorillaParent.instance.vrrigs.Contains(_giveGunTarget))
                    _giveGunTarget = null;

                return _giveGunTarget;
            }
            set => _giveGunTarget = value;
        }

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

        // Compatiblity
        public static int buttonsType
        {
            get => currentCategoryIndex;
            set => currentCategoryIndex = value;
        }

        public static int buttonClickSound = 8;
        public static int buttonClickIndex;
        public static int buttonClickVolume = 4;
        public static int buttonOffset = 2;
        public static int menuButtonIndex = 1;
        public static float buttonDistance
        {
            get => 0.8f / (pageSize + buttonOffset);
        }

        public static bool doButtonsVibrate = true;
        public static bool serversidedButtonSounds;

        public static bool joystickMenu;
        public static bool joystickMenuSearching;
        public static bool physicalMenu;
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
        public static bool highQualityText;
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
        public static int fontStyleType = 2;
        public static bool NoAutoSizeText;

        public static bool doCustomName;
        public static string customMenuName = "your text here";
        public static bool doCustomMenuBackground;
        public static bool disableBoardColor;
        public static bool disableBoardTextColor;
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
        public static bool animatedTitle;
        public static bool gradientTitle;
        public static string lastClickedName = "";

        public static string motdTemplate = "You are using build {0}. This menu was created by iiDk (@crimsoncauldron) on Discord. " +
        "This menu is completely free and open sourced, if you paid for this menu you have been scammed. " +
        "There are a total of {1} mods on this menu. " +
        "<color=red>I, iiDk, am not responsible for any bans using this menu.</color> " +
        "If you get banned while using this, it's your responsibility.";

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
        public static Text fpsCount;
        private static float fpsAvgTime;
        private static float fpsAverageNumber;
        public static bool fpsCountTimed;
        public static bool fpsCountAverage;
        public static bool ftCount;
        public static bool acceptedDonations;
        public static float lastDeltaTime = 1f;
        public static Text keyboardInputObject;
        public static Text title;
        public static VRRig GhostRig;
        public static GameObject legacyGhostViewLeft;
        public static GameObject legacyGhostViewRight;
        public static Material GhostMaterial;
        public static Material CrystalMaterial;
        public static Material searchMat;
        public static Material updateMat;
        public static Material promptMat;
        public static Material returnMat;
        public static Material debugMat;
        public static Material donateMat;

        public static Font AgencyFB = Font.CreateDynamicFontFromOSFont("Agency FB", 24);
        public static readonly Font Arial = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        public static readonly Font Verdana = Font.CreateDynamicFontFromOSFont("Verdana", 24);
        public static readonly Font ComicSans = Font.CreateDynamicFontFromOSFont("Comic Sans MS", 24);
        public static readonly Font Consolas = Font.CreateDynamicFontFromOSFont("Consolas", 24);
        public static readonly Font Candara = Font.CreateDynamicFontFromOSFont("Candara", 24);
        public static readonly Font MSGothic = Font.CreateDynamicFontFromOSFont("MS Gothic", 24);
        public static readonly Font Impact = Font.CreateDynamicFontFromOSFont("Impact", 24);
        public static readonly Font SimSun = Font.CreateDynamicFontFromOSFont("SimSun", 24);
        public static Font GTFont;
        public static Font Minecraft;
        public static Font Terminal;
        public static Font OpenDyslexic;
        public static Font activeFont = AgencyFB;
        public static FontStyle activeFontStyle = FontStyle.Italic;

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

        public static Material OrangeUI = new Material(Shader.Find("GorillaTag/UberShader"));
        public static Material ForestMat;
        public static Material StumpMat;
        public static GameObject motd;
        public static GameObject motdText;
        public static Material glass;

        public static Material cannmat;
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

        public static readonly List<string> favorites = new List<string> { "Exit Favorite Mods" };

        public static readonly Dictionary<string, GameObject> objectBoards = new Dictionary<string, GameObject>();
        public static List<GorillaNetworkJoinTrigger> triggers = new List<GorillaNetworkJoinTrigger>();
        public static readonly List<TextMeshPro> udTMP = new List<TextMeshPro>();
        public static GameObject computerMonitor;

        public static readonly int StumpLeaderboardIndex = 4;
        public static readonly int ForestLeaderboardIndex = 8;
        
        public static Material[] ogScreenMats = { };

        public static bool translate;
        public static string language;

        public static List<string> muteIDs = new List<string>();
        public static readonly List<string> mutedIDs = new List<string>();

        public static string serverLink = "https://discord.gg/iidk";

        public static readonly int[] bones = {
            4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7
        };

        public static int arrowType;
        public static readonly string[][] arrowTypes = {
            new[] {"<", ">"},
            new[] {"←", "→"},
            new[] {"↞", "↠"},
            new[] {"◄", "►"},
            new[] {"〈 ", " 〉"},
            new[] {"‹", "›"},
            new[] {"«", "»"},
            new[] {"◀", "▶"},
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

        public static bool lastMasterClient;
        public static string lastRoom = "";

        public static string partyLastCode;
        public static float partyTime;
        public static bool phaseTwo;

        public static float timeMenuStarted = -1f;
        public static float kgDebounce;
        public static float stealIdentityDelay;
        public static float beesDelay;
        public static float laggyRigDelay;
        public static float jrDebounce;
        public static float soundDebounce;
        public static float buttonCooldown;
        public static float colorChangerDelay;
        public static float autoSaveDelay = Time.time + 60f;
        public static bool BackupPreferences;
        public static int PreferenceBackupCount;

        public static int notificationDecayTime = 1000;
        public static int notificationSoundIndex;

        public static float oldSlide;

        public static int soundId;

        public static string inputText = "";
        public static string lastCommand = "";

        public static float ShootStrength = 19.44f;

        public static bool lastprimaryhit;

        public static int colorChangeType;
        public static bool strobeColor;

        public static bool AntiOculusReport;

        public static bool shift;
        public static bool lockShift; // Konekokitten KTOH

        public static bool lastHit;
        public static bool lastHit2;
        public static bool lastRG;

        public static int tindex = 1;

        public static bool lastHitL;
        public static bool lastHitR;
        public static bool lastHitLP;
        public static bool lastHitRP;
        public static bool lastHitRS;

        public static bool headspazType;

        public static bool hasFoundAllBoards;

        public static float sizeScale = 1f;

        public static float turnAmnt;
        public static float TagAuraDelay;

        public static bool lowercaseMode;
        public static bool uppercaseMode;
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
        };
    }
}