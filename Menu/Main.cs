using BepInEx;
using ExitGames.Client.Photon;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTagScripts;
using GorillaTagScripts.ObstacleCourse;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;
using static iiMenu.Classes.RigManager;

/*
ii's Stupid Menu, written by @goldentrophy
Any comments are dev comments I wrote
Most comments are used to find certain parts of code faster with Ctrl + F
Feel free to read them if you want

ii's Stupid Menu falls under the GPL-3.0 license
https://github.com/iiDk-the-actual/iis.Stupid.Menu
*/

namespace iiMenu.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        public static void Prefix()
        {
            try
            {
                bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q) || (isSearching && isPcWhenSearching);
                bool buttonCondition = ControllerInputPoller.instance.leftControllerSecondaryButton;
                if (rightHand)
                {
                    buttonCondition = ControllerInputPoller.instance.rightControllerSecondaryButton;
                }
                if (likebark)
                {
                    buttonCondition = rightHand ? ControllerInputPoller.instance.leftControllerSecondaryButton : ControllerInputPoller.instance.rightControllerSecondaryButton;
                }
                if (bothHands)
                {
                    buttonCondition = ControllerInputPoller.instance.leftControllerSecondaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton;
                    if (buttonCondition)
                    {
                        openedwithright = ControllerInputPoller.instance.rightControllerSecondaryButton;
                    }
                }
                if (wristThing)
                {
                    bool fuck = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position - (GorillaTagger.Instance.leftHandTransform.forward * 0.1f), TrueRightHand().position) < 0.1f;
                    if (rightHand)
                    {
                        fuck = Vector3.Distance(TrueLeftHand().position, GorillaTagger.Instance.rightHandTransform.position - (GorillaTagger.Instance.rightHandTransform.forward * 0.1f)) < 0.1f;
                    }
                    if (fuck && !lastChecker)
                    {
                        wristOpen = !wristOpen;
                    }
                    lastChecker = fuck;

                    buttonCondition = wristOpen;
                }
                if (joystickMenu)
                {
                    bool fuck = SteamVR_Actions.gorillaTag_RightJoystickClick.state;

                    if (fuck && !lastChecker)
                    {
                        joystickOpen = !joystickOpen;
                        joystickDelay = Time.time + 0.2f;
                    }
                    lastChecker = fuck;

                    buttonCondition = joystickOpen;
                } else
                {
                    joystickButtonSelected = 0;
                }
                if (physicalMenu)
                {
                    if (buttonCondition)
                        physicalOpenPosition = Vector3.zero;

                    buttonCondition = true;
                }
                buttonCondition = buttonCondition || isKeyboardCondition;
                buttonCondition = buttonCondition && !lockdown;
                buttonCondition = buttonCondition || isSearching;
                if (wristThingV2)
                {
                    buttonCondition = isKeyboardCondition;
                }
                if (buttonCondition && menu == null)
                {
                    if (dynamicSounds)
                    {
                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/open.wav", "open.wav"), buttonClickVolume / 10f);
                    }
                    Draw();
                    if (dynamicAnimations)
                    {
                        CoroutineManager.RunCoroutine(GrowCoroutine());
                    }
                    if (!joystickMenu)
                    {
                        if (reference == null)
                        {
                            CreateReference();
                        }
                    }
                }
                else
                {
                    if (!buttonCondition && menu != null)
                    {
                        if (dynamicSounds)
                        {
                            Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/close.wav", "close.wav"), buttonClickVolume / 10f);
                        }
                        if (TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet") && isOnPC)
                        {
                            isOnPC = false;
                            TPC.transform.position = TPC.transform.parent.position;
                            TPC.transform.rotation = TPC.transform.parent.rotation;
                        }
                        if (!dynamicAnimations)
                        {
                            if (dropOnRemove)
                            {
                                try
                                {
                                    Rigidbody comp = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                                    if (GetIndex("Zero Gravity Menu").enabled)
                                    {
                                        comp.useGravity = false;
                                    }
                                    if (rightHand || (bothHands && openedwithright))
                                    {
                                        if (GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>() == null)
                                        {
                                            GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").AddComponent<GorillaVelocityEstimator>();
                                        }
                                        comp.velocity = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                                        comp.angularVelocity = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
                                    }
                                    else
                                    {
                                        if (GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>() == null)
                                        {
                                            GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").AddComponent<GorillaVelocityEstimator>();
                                        }
                                        comp.velocity = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                                        comp.angularVelocity = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
                                    }
                                    if (annoyingMode)
                                    {
                                        comp.velocity = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                                        comp.angularVelocity = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                                    }
                                }
                                catch { UnityEngine.Debug.Log("Rigidbody broken part A"); }

                                UnityEngine.Object.Destroy(menu, 5f);
                                menu = null;
                                UnityEngine.Object.Destroy(reference);
                                reference = null;
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(menu);
                                menu = null;
                                UnityEngine.Object.Destroy(reference);
                                reference = null;
                            }
                        } else
                        {
                            CoroutineManager.RunCoroutine(ShrinkCoroutine());
                            UnityEngine.Object.Destroy(reference);
                            reference = null;
                        }
                    }
                }
                if (buttonCondition && menu != null)
                {
                    RecenterMenu();
                }

                {
                    hasRemovedThisFrame = false;

                    if (!hasFoundAllBoards)
                    {
                        try
                        {
                            UnityEngine.Debug.Log("Looking for boards");
                            bool found = false;
                            int indexOfThatThing = 0;
                            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
                            {
                                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                                if (v.name.Contains("forestatlas"))
                                {
                                    indexOfThatThing++;
                                    if (indexOfThatThing == 2)
                                    {
                                        found = true;
                                        v.GetComponent<Renderer>().material = OrangeUI;
                                    }
                                }
                            }

                            bool found2 = false;
                            indexOfThatThing = 0;
                            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
                            {
                                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                                if (v.name.Contains("forestatlas"))
                                {
                                    indexOfThatThing++;
                                    if (indexOfThatThing == 2)
                                    {
                                        UnityEngine.Debug.Log("Board found");
                                        found2 = true;
                                        v.GetComponent<Renderer>().material = OrangeUI;
                                    }
                                }
                            }
                            if (found && found2)
                            {
                                GameObject vr = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomBoundaryStones/BoundaryStoneSet_Forest/wallmonitorforestbg");
                                if (vr != null)
                                {
                                    vr.GetComponent<Renderer>().material = OrangeUI;
                                }

                                foreach (GorillaNetworkJoinTrigger v in (List<GorillaNetworkJoinTrigger>)typeof(PhotonNetworkController).GetField("allJoinTriggers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PhotonNetworkController.Instance))
                                {
                                    try
                                    {
                                        JoinTriggerUI ui = (JoinTriggerUI)typeof(GorillaNetworkJoinTrigger).GetField("ui", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(v);
                                        JoinTriggerUITemplate temp = (JoinTriggerUITemplate)typeof(JoinTriggerUI).GetField("template", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ui);

                                        temp.ScreenBG_AbandonPartyAndSoloJoin = OrangeUI;
                                        temp.ScreenBG_AlreadyInRoom = OrangeUI;
                                        temp.ScreenBG_ChangingGameModeSoloJoin = OrangeUI;
                                        temp.ScreenBG_Error = OrangeUI;
                                        temp.ScreenBG_InPrivateRoom = OrangeUI;
                                        temp.ScreenBG_LeaveRoomAndGroupJoin = OrangeUI;
                                        temp.ScreenBG_LeaveRoomAndSoloJoin = OrangeUI;
                                        temp.ScreenBG_NotConnectedSoloJoin = OrangeUI;

                                        TextMeshPro text = (TextMeshPro)Traverse.Create(ui).Field("screenText").GetValue();
                                        if (!udTMP.Contains(text))
                                        {
                                            udTMP.Add(text);
                                        }
                                    }
                                    catch { }
                                }
                                PhotonNetworkController.Instance.UpdateTriggerScreens();

                                string[] objectsWithTMPro = new string[]
                                {
                                    "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConduct",
                                    "Environment Objects/LocalObjects_Prefab/TreeRoom/COC Text",
                                    "Environment Objects/LocalObjects_Prefab/TreeRoom/Data",
                                    "Environment Objects/LocalObjects_Prefab/TreeRoom/FunctionSelect"
                                };
                                foreach (string lol in objectsWithTMPro)
                                {
                                    GameObject obj = GameObject.Find(lol);
                                    if (obj != null)
                                    {
                                        TextMeshPro text = obj.GetComponent<TextMeshPro>();
                                        if (!udTMP.Contains(text))
                                        {
                                            udTMP.Add(text);
                                        }
                                    }
                                    else
                                    {
                                        UnityEngine.Debug.Log("Could not find " + lol);
                                    }
                                }

                                Transform targettransform = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard").transform;
                                for (int i = 0; i < targettransform.transform.childCount; i++)
                                {
                                    GameObject v = targettransform.GetChild(i).gameObject;
                                    if ((v.name.Contains("Board Text") || v.name.Contains("Scoreboard_OfflineText")) && v.activeSelf)
                                    {
                                        TextMeshPro text = v.GetComponent<TextMeshPro>();
                                        if (!udTMP.Contains(text))
                                        {
                                            udTMP.Add(text);
                                        }
                                    }
                                }

                                hasFoundAllBoards = true;
                                UnityEngine.Debug.Log("Found all boards");
                            }
                        }
                        catch (Exception exception)
                        {
                            UnityEngine.Debug.LogError(string.Format("iiMenu <b>COLOR ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
                            hasFoundAllBoards = false;
                        }
                    }

                    try
                    {
                        GameObject computerMonitor = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
                        if (computerMonitor != null)
                        {
                            computerMonitor.GetComponent<Renderer>().material = OrangeUI;
                        }
                    } catch { }

                    try
                    {
                        if (!disableBoardColor)
                        {
                            OrangeUI.color = GetBGColor(0f);
                        } else
                        {
                            OrangeUI.color = new Color32(0, 59, 4, 255);
                        }

                        if (motd == null)
                        {
                            GameObject motdThing = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motd (1)");
                            motd = UnityEngine.Object.Instantiate(motdThing, motdThing.transform.parent);
                            motdThing.SetActive(false);
                        }
                        TextMeshPro motdTC = motd.GetComponent<TextMeshPro>();
                        if (!udTMP.Contains(motdTC))
                        {
                            udTMP.Add(motdTC);
                        }
                        motdTC.richText = true;
                        motdTC.fontSize = 70;
                        motdTC.text = "Thanks for using ii's <b>Stupid</b> Menu!";
                        if (doCustomName)
                        {
                            motdTC.text = "Thanks for using " + customMenuName + "!";
                        }
                        if (translate)
                        {
                            motdTC.text = TranslateText(motdTC.text);
                        }
                        if (lowercaseMode)
                        {
                            motdTC.text = motdTC.text.ToLower();
                        }
                        motdTC.color = titleColor;
                        motdTC.overflowMode = TextOverflowModes.Overflow;

                        if (motdText == null)
                        {
                            GameObject motdThing = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext");
                            motdText = UnityEngine.Object.Instantiate(motdThing, motdThing.transform.parent);
                            motdThing.SetActive(false);

                            motdText.GetComponent<PlayFabTitleDataTextDisplay>().enabled = false;
                        }
                        TextMeshPro motdTextB = motdText.GetComponent<TextMeshPro>();
                        if (!udTMP.Contains(motdTextB))
                        {
                            udTMP.Add(motdTextB);
                        }
                        motdTextB.richText = true;
                        motdTextB.fontSize = 100;
                        motdTextB.color = titleColor;

                        if (fullModAmount < 0)
                        {
                            fullModAmount = 0;
                            foreach (ButtonInfo[] buttons in Buttons.buttons)
                            {
                                fullModAmount += buttons.Length;
                            }
                        }
                        motdTextB.text = string.Format(motdTemplate, PluginInfo.Version, fullModAmount);
                        if (translate)
                        {
                            motdTextB.text = TranslateText(motdTextB.text);
                        }
                        if (lowercaseMode)
                        {
                            motdTextB.text = motdTextB.text.ToLower();
                        }
                    } catch { }

                    try
                    {
                        Color targetColor = titleColor;
                        if (disableBoardColor || disableBoardTextColor)
                        {
                            targetColor = Color.white;
                        }

                        foreach (TextMeshPro txt in udTMP)
                        {
                            txt.color = targetColor;
                        }
                    } catch { }

                    // Search key press detector
                    if (isSearching)
                    {
                        List<KeyCode> keysPressed = new List<KeyCode>();
                        foreach (KeyCode keyCode in allowedKeys)
                        {
                            if (UnityInput.Current.GetKey(keyCode))
                            {
                                if (keyCode != KeyCode.Backspace)
                                {
                                    keysPressed.Add(keyCode);
                                }
                                if (!lastPressedKeys.Contains(keyCode))
                                {
                                    if (keyCode == KeyCode.Space)
                                    {
                                        searchText += " ";
                                    }
                                    else
                                    {
                                        if (keyCode == KeyCode.Backspace)
                                        {
                                            if (Time.time > lastBackspaceTime)
                                            {
                                                searchText = searchText.Substring(0, searchText.Length - 1);
                                            }
                                        }
                                        else
                                        {
                                            if (keyCode == KeyCode.Escape)
                                            {
                                                Toggle("Global Search");
                                            }
                                            else
                                            {
                                                searchText += UnityInput.Current.GetKey(KeyCode.LeftShift) || UnityInput.Current.GetKey(KeyCode.RightShift) ? keyCode.ToString().Capitalize() : keyCode.ToString().ToLower();
                                            }
                                        }
                                    }
                                    if (Time.time > lastBackspaceTime)
                                    {
                                        if (keyCode == KeyCode.Backspace)
                                        {
                                            lastBackspaceTime = Time.time + 0.1f;
                                        }
                                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, buttonClickVolume / 10f);
                                        pageNumber = 0;
                                        ReloadMenu();
                                    }
                                }
                            }
                        }
                        lastPressedKeys = keysPressed;
                    }

                    // Get the camera (compatible with Yizzi)
                    try
                    {
                        if (TPC == null)
                        {
                            try
                            {
                                TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                            }
                            catch
                            {
                                TPC = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                            }
                        }
                    } catch { }

                    // FPS counter
                    if (fpsCount != null)
                    {
                        fpsCount.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                        if (lowercaseMode)
                        {
                            fpsCount.text = fpsCount.text.ToLower();
                        }
                    }

                    if (searchTextObject != null)
                    {
                        searchTextObject.text = searchText + (((Time.frameCount / 45) % 2) == 0 ? "|" : " ");
                        if (lowercaseMode)
                        {
                            searchTextObject.text = searchTextObject.text.ToLower();
                        }
                    }

                    // Recolor the button collider
                    if (menuBackground != null && reference != null)
                    {
                        reference.GetComponent<Renderer>().material.color = menuBackground.GetComponent<Renderer>().material.color;
                    }

                    // Fix for disorganized
                    if (disorganized && buttonsType != 0)
                    {
                        buttonsType = 0;
                        ReloadMenu();
                    }

                    // Fix for longmenu
                    if (longmenu && pageNumber != 0)
                    {
                        pageNumber = 0;
                        ReloadMenu();
                    }

                    // Join / leave room reminders
                    try
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            lastRoom = PhotonNetwork.CurrentRoom.Name;
                        }

                        if (PhotonNetwork.InRoom && !lastInRoom)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=blue>JOIN ROOM</color><color=grey>]</color> Room Code: " + lastRoom + "");
                            RPCProtection();
                        }
                        if (!PhotonNetwork.InRoom && lastInRoom)
                        {
                            if (GetIndex("Clear Notifications on Disconnect").enabled)
                            {
                                NotifiLib.ClearAllNotifications();
                            }
                            NotifiLib.SendNotification("<color=grey>[</color><color=blue>LEAVE ROOM</color><color=grey>]</color> Room Code: " + lastRoom + "");
                            RPCProtection();
                            lastMasterClient = false;
                        }

                        lastInRoom = PhotonNetwork.InRoom;
                    }
                    catch { }

                    // Master client notification
                    try
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            if (PhotonNetwork.LocalPlayer.IsMasterClient && !lastMasterClient)
                            {
                                NotifiLib.SendNotification("<color=grey>[</color><color=purple>MASTER</color><color=grey>]</color> You are now master client.");
                            }
                            lastMasterClient = PhotonNetwork.LocalPlayer.IsMasterClient;
                        }
                    }
                    catch { }

                    // Load version and admin player ID
                    try
                    {
                        if (shouldAttemptLoadData && Time.time > shouldLoadDataTime && GorillaComputer.instance.isConnectedToMaster)
                        {
                            attemptsToLoad++;
                            if(attemptsToLoad >= 3)
                            {
                                UnityEngine.Debug.Log("Giving up on loading web data due to errors");
                                shouldAttemptLoadData = false;
                            }
                            UnityEngine.Debug.Log("Attempting to load web data");
                            shouldLoadDataTime = Time.time + 5f;
                            if (!hasLoadedPreferences)
                            {
                                try {
                                    UnityEngine.Debug.Log("Loading preferences due to load errors");
                                    Settings.LoadPreferences();
                                } catch
                                {
                                    UnityEngine.Debug.Log("Could not load preferences");
                                }
                            }
                            CoroutineManager.RunCoroutine(LoadServerData(false));
                        }
                    } catch { }

                    try
                    {
                        if (Time.time > autoSaveDelay && !lockdown)
                        {
                            autoSaveDelay = Time.time + 60f;
                            Settings.SavePreferences();
                            UnityEngine.Debug.Log("Automatically saved preferences");
                        }
                    }
                    catch { }

                    // Gradually reload data to ensure updated admin list
                    try
                    {
                        if (nextTimeUntilReload > 0f)
                        {
                            if (Time.time > nextTimeUntilReload)
                            {
                                nextTimeUntilReload = Time.time + 60f;
                                CoroutineManager.RunCoroutine(LoadServerData(true));
                            }
                        } else
                        {
                            if (GorillaComputer.instance.isConnectedToMaster)
                                nextTimeUntilReload = Time.time + 60f;
                        }
                    } catch { }

                    // Remove physical menu reference if too far away
                    if (physicalMenu && menu != null)
                    {
                        try
                        {
                            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, menu.transform.position) < 1.5f)
                            {
                                if (reference == null)
                                {
                                    CreateReference();
                                }
                            } else
                            {
                                if (reference != null)
                                {
                                    UnityEngine.Object.Destroy(reference);
                                    reference = null;
                                }
                            }
                        } catch { }
                    }

                    // Ghostview
                    try
                    {
                        if ((!GorillaTagger.Instance.offlineVRRig.enabled || ghostException) && !disableGhostview)
                        {
                            if (legacyGhostview)
                            {
                                if (GhostRig != null)
                                {
                                    UnityEngine.Object.Destroy(GhostRig.gameObject);
                                }

                                GameObject l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                UnityEngine.Object.Destroy(l.GetComponent<Rigidbody>());
                                UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

                                l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                l.transform.position = TrueLeftHand().position;

                                GameObject r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                UnityEngine.Object.Destroy(r.GetComponent<Rigidbody>());
                                UnityEngine.Object.Destroy(r.GetComponent<SphereCollider>());

                                r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                r.transform.position = TrueRightHand().position;

                                l.GetComponent<Renderer>().material.color = GetBGColor(0f);
                                r.GetComponent<Renderer>().material.color = GetBGColor(0f);

                                UnityEngine.Object.Destroy(l, Time.deltaTime);
                                UnityEngine.Object.Destroy(r, Time.deltaTime);
                            }
                            else
                            {
                                if (GhostRig == null)
                                {
                                    GhostRig = UnityEngine.Object.Instantiate<VRRig>(GorillaTagger.Instance.offlineVRRig, GorillaLocomotion.Player.Instance.transform.position, GorillaLocomotion.Player.Instance.transform.rotation);
                                    GhostRig.headBodyOffset = Vector3.zero;
                                    GhostRig.enabled = true;

                                    GhostRig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                                    GhostRig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);

                                    //GhostPatch.Prefix(GorillaTagger.Instance.offlineVRRig);
                                }

                                if (funnyghostmaterial == null)
                                {
                                    funnyghostmaterial = new Material(Shader.Find("GUI/Text Shader"));
                                }
                                Color ghm = GetBGColor(0f);
                                ghm.a = 0.5f;
                                funnyghostmaterial.color = ghm;
                                GhostRig.mainSkin.material = funnyghostmaterial;

                                GhostRig.headConstraint.transform.position = GorillaLocomotion.Player.Instance.headCollider.transform.position;
                                GhostRig.headConstraint.transform.rotation = GorillaLocomotion.Player.Instance.headCollider.transform.rotation;

                                GhostRig.leftHandTransform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                                GhostRig.rightHandTransform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;

                                GhostRig.leftHandTransform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                                GhostRig.rightHandTransform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;

                                GhostRig.transform.position = GorillaLocomotion.Player.Instance.transform.position;
                                GhostRig.transform.rotation = GorillaLocomotion.Player.Instance.transform.rotation;
                            }
                        }
                        else
                        {
                            if (GhostRig != null)
                            {
                                UnityEngine.Object.Destroy(GhostRig.gameObject);
                            }
                        }
                    }
                    catch { }

                    // Legacy Admin mods / ii's Harmless Backdoor
                    if (PhotonNetwork.InRoom)
                    {
                        try
                        {
                            // Admin indicator
                            if (!Experimental.daaind)
                            {
                                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                                {
                                    if (admins.ContainsKey(player.UserId))
                                    {
                                        if (player != adminConeExclusion)
                                        {
                                            try
                                            {
                                                VRRig obediantsubject = GetVRRigFromPlayer(player);
                                                if (obediantsubject != null)
                                                {
                                                    GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                                    UnityEngine.Object.Destroy(crown.GetComponent<Collider>());
                                                    UnityEngine.Object.Destroy(crown, Time.deltaTime);
                                                    if (crownmat == null)
                                                    {
                                                        crownmat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                                                        if (admincrown == null)
                                                        {
                                                            admincrown = LoadTextureFromResource("iiMenu.Resources.icon.png");
                                                        }
                                                        crownmat.mainTexture = admincrown;

                                                        crownmat.SetFloat("_Surface", 1);
                                                        crownmat.SetFloat("_Blend", 0);
                                                        crownmat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                                        crownmat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                                        crownmat.SetFloat("_ZWrite", 0);
                                                        crownmat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                                        crownmat.renderQueue = (int)RenderQueue.Transparent;

                                                        crownmat.SetFloat("_Glossiness", 0f);
                                                        crownmat.SetFloat("_Metallic", 0f);
                                                    }
                                                    crown.GetComponent<Renderer>().material = crownmat;
                                                    crown.GetComponent<Renderer>().material.color = obediantsubject.playerColor;
                                                    crown.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f);
                                                    crown.transform.position = obediantsubject.headMesh.transform.position + obediantsubject.headMesh.transform.up * 0.8f;
                                                    crown.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                                                    Vector3 rot = crown.transform.rotation.eulerAngles;
                                                    rot += new Vector3(0f, 0f, Mathf.Sin(Time.time * 2f) * 10f);
                                                    crown.transform.rotation = Quaternion.Euler(rot);
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        lastOwner = false;
                    }

                    try
                    {
                        if (adminIsScaling && adminRigTarget != null)
                        {
                            adminRigTarget.scaleFactor = adminScale;
                        }
                    } catch { }

                    if (!HasLoaded)
                    {
                        HasLoaded = true;
                        OnLaunch();
                    }

                    rightPrimary = ControllerInputPoller.instance.rightControllerPrimaryButton || UnityInput.Current.GetKey(KeyCode.E);
                    rightSecondary = ControllerInputPoller.instance.rightControllerSecondaryButton || UnityInput.Current.GetKey(KeyCode.R);
                    leftPrimary = ControllerInputPoller.instance.leftControllerPrimaryButton || UnityInput.Current.GetKey(KeyCode.F);
                    leftSecondary = ControllerInputPoller.instance.leftControllerSecondaryButton || UnityInput.Current.GetKey(KeyCode.G);
                    leftGrab = ControllerInputPoller.instance.leftGrab || UnityInput.Current.GetKey(KeyCode.LeftBracket);
                    rightGrab = ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKey(KeyCode.RightBracket);
                    leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
                    rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
                    if (UnityInput.Current.GetKey(KeyCode.Minus))
                    {
                        leftTrigger = 1f;
                    }
                    if (UnityInput.Current.GetKey(KeyCode.Equals))
                    {
                        rightTrigger = 1f;
                    }
                    shouldBePC = UnityInput.Current.GetKey(KeyCode.E) || UnityInput.Current.GetKey(KeyCode.R) || UnityInput.Current.GetKey(KeyCode.F) || UnityInput.Current.GetKey(KeyCode.G) || UnityInput.Current.GetKey(KeyCode.LeftBracket) || UnityInput.Current.GetKey(KeyCode.RightBracket) || UnityInput.Current.GetKey(KeyCode.Minus) || UnityInput.Current.GetKey(KeyCode.Equals) || Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;

                    if (menu != null)
                    {
                        if (pageButtonType == 3)
                        {
                            if (leftGrab == true && plastLeftGrip == false)
                            {
                                MakeButtonSound(null, true, true);
                                Toggle("PreviousPage");
                            }
                            plastLeftGrip = leftGrab;

                            if (rightGrab == true && plastRightGrip == false)
                            {
                                MakeButtonSound(null, true, false);
                                Toggle("NextPage");
                            }
                            plastRightGrip = rightGrab;
                        }

                        if (pageButtonType == 4)
                        {
                            if (leftTrigger > 0.5f && plastLeftGrip == false)
                            {
                                MakeButtonSound(null, true, true);
                                Toggle("PreviousPage");
                            }
                            plastLeftGrip = leftTrigger > 0.5f;

                            if (rightTrigger > 0.5f && plastRightGrip == false)
                            {
                                MakeButtonSound(null, true, false);
                                Toggle("NextPage");
                            }
                            plastRightGrip = rightTrigger > 0.5f;
                        }
                    }

                    try
                    {
                        if (joystickMenu && joystickOpen)
                        {
                            Vector2 js = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;
                            if (Time.time > joystickDelay)
                            {
                                if (js.x > 0.5f)
                                {
                                    if (dynamicSounds)
                                    {
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/next.wav", "next.wav"), buttonClickVolume / 10f);
                                    }
                                    Toggle("NextPage");
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                                if (js.x < -0.5f)
                                {
                                    if (dynamicSounds)
                                    {
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/prev.wav", "prev.wav"), buttonClickVolume / 10f);
                                    }
                                    Toggle("PreviousPage");
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }

                                if (js.y > 0.5f)
                                {
                                    if (dynamicSounds)
                                    {
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/open.wav", "open.wav"), buttonClickVolume / 10f);
                                    }
                                    joystickButtonSelected--;
                                    if (joystickButtonSelected < 0)
                                    {
                                        joystickButtonSelected = pageSize - 1;
                                    }
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                                if (js.y < -0.5f)
                                {
                                    if (dynamicSounds)
                                    {
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/close.wav", "close.wav"), buttonClickVolume / 10f);
                                    }
                                    joystickButtonSelected++;
                                    if (joystickButtonSelected > pageSize - 1)
                                    {
                                        joystickButtonSelected = 0;
                                    }
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }

                                if (SteamVR_Actions.gorillaTag_LeftJoystickClick.state)
                                {
                                    if (dynamicSounds)
                                    {
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/select.wav", "select.wav"), buttonClickVolume / 10f);
                                    }
                                    Toggle(joystickSelectedButton, true);
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                            }
                        }
                    } catch { }

                    try
                    {
                        if (wristThingV2)
                        {
                            watchShell.GetComponent<Renderer>().material = OrangeUI;
                            ButtonInfo[] toSortOf = Buttons.buttons[buttonsType];
                            if (buttonsType == 19)
                            {
                                toSortOf = StringsToInfos(favorites.ToArray());
                            }
                            if (buttonsType == 24)
                            {
                                List<string> enabledMods = new List<string>() { "Exit Enabled Mods" };
                                foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                                {
                                    foreach (ButtonInfo v in buttonlist)
                                    {
                                        if (v.enabled)
                                        {
                                            enabledMods.Add(v.buttonText);
                                        }
                                    }
                                }
                                enabledMods = Alphabetize(enabledMods.ToArray()).ToList();
                                toSortOf = StringsToInfos(enabledMods.ToArray());
                            }
                            watchText.GetComponent<Text>().text = toSortOf[currentSelectedModThing].buttonText;
                            if (toSortOf[currentSelectedModThing].overlapText != null)
                            {
                                watchText.GetComponent<Text>().text = toSortOf[currentSelectedModThing].overlapText;
                            }
                            watchText.GetComponent<Text>().text += "\n<color=grey>[" + (currentSelectedModThing + 1).ToString() + "/" + toSortOf.Length.ToString() + "]\n" + DateTime.Now.ToString("hh:mm tt") + "</color>";
                            watchText.GetComponent<Text>().color = titleColor;

                            if (lowercaseMode)
                            {
                                watchText.GetComponent<Text>().text = watchText.GetComponent<Text>().text.ToLower();
                            }

                            if (watchIndicatorMat == null)
                            {
                                watchIndicatorMat = new Material(Shader.Find("GorillaTag/UberShader"));
                            }
                            watchIndicatorMat.color = toSortOf[currentSelectedModThing].enabled ? GetBDColor(0f) : GetBRColor(0f);
                            watchEnabledIndicator.GetComponent<Image>().material = watchIndicatorMat;

                            Vector2 js = rightHand ? SteamVR_Actions.gorillaTag_RightJoystick2DAxis.axis : SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;
                            if (Time.time > wristMenuDelay)
                            {
                                if (js.x > 0.5f || (rightHand ? (js.y < -0.5f) : (js.y > 0.5f)))
                                {
                                    currentSelectedModThing++;
                                    if (currentSelectedModThing > toSortOf.Length - 1)
                                    {
                                        currentSelectedModThing = 0;
                                    }
                                    wristMenuDelay = Time.time + 0.2f;
                                }
                                if (js.x < -0.5f || (rightHand ? (js.y > 0.5f) : (js.y < -0.5f)))
                                {
                                    currentSelectedModThing--;
                                    if (currentSelectedModThing < 0)
                                    {
                                        currentSelectedModThing = toSortOf.Length - 1;
                                    }
                                    wristMenuDelay = Time.time + 0.2f;
                                }
                                if (rightHand ? SteamVR_Actions.gorillaTag_RightJoystickClick.state : SteamVR_Actions.gorillaTag_LeftJoystickClick.state)
                                {
                                    int archive = buttonsType;
                                    Toggle(toSortOf[currentSelectedModThing].buttonText, true);
                                    if (buttonsType != archive)
                                    {
                                        currentSelectedModThing = 0;
                                    }
                                    wristMenuDelay = Time.time + 0.2f;
                                }
                            }
                        }
                    } catch { }

                    // Reconnect code
                    if (PhotonNetwork.InRoom)
                    {
                        if (rejRoom != null)
                        {
                            rejRoom = null;
                        }
                    }
                    else
                    {
                        if (rejRoom != null && Time.time > rejDebounce/* && PhotonNetwork.NetworkingClient.State == ClientState.Disconnected*/)
                        {
                            UnityEngine.Debug.Log("Attempting rejoin");
                            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(rejRoom, JoinType.Solo);
                            rejDebounce = Time.time + (float)internetTime;
                        }
                    }

                    // Party kick code (to return back to the main lobby when you're done
                    if (PhotonNetwork.InRoom)
                    {
                        if (phaseTwo)
                        {
                            partyLastCode = null;
                            phaseTwo = false;
                            NotifiLib.ClearAllNotifications();
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Successfully " + (waitForPlayerJoin ? "banned" : "kicked") + " " + amountPartying.ToString() + " party member!</color>");
                            FriendshipGroupDetection.Instance.LeaveParty();
                        }
                        else
                        {
                            if (partyLastCode != null && Time.time > partyTime && (waitForPlayerJoin ? PhotonNetwork.PlayerListOthers.Length > 0 : true))
                            {
                                UnityEngine.Debug.Log("Attempting rejoin");
                                PhotonNetwork.Disconnect();
                                phaseTwo = true;
                            }
                        }
                    } else
                    {
                        if (phaseTwo)
                        {
                            if (partyLastCode != null && Time.time > partyTime && (waitForPlayerJoin ? PhotonNetwork.PlayerListOthers.Length > 0 : true))
                            {
                                UnityEngine.Debug.Log("Attempting rejoin");
                                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(partyLastCode, JoinType.Solo);
                                partyTime = Time.time + (float)internetTime;
                            }
                        }
                    }

                    // Recover from playing sound on soundboard code
                    try
                    {
                        if (iiMenu.Mods.Spammers.Sound.AudioIsPlaying)
                        {
                            if (Time.time > iiMenu.Mods.Spammers.Sound.RecoverTime)
                            {
                                iiMenu.Mods.Spammers.Sound.FixMicrophone();
                            }
                        }
                    } catch { }

                    if (annoyingMode)
                    {
                        OrangeUI.color = new Color32(226, 74, 44, 255);
                        int randy = UnityEngine.Random.Range(1, 400);
                        if (randy == 21)
                        {
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(84, true, 0.4f);
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(84, false, 0.4f);
                            NotifiLib.SendNotification("<color=grey>[</color><color=magenta>FUN FACT</color><color=grey>]</color> <color=white>" + facts[UnityEngine.Random.Range(0, facts.Length - 1)] + "</color>");
                        }
                    }

                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            try
                            {
                                if (v.enabled)
                                {
                                    if (v.method != null)
                                    {
                                        try
                                        {
                                            v.method.Invoke();
                                        }
                                        catch (Exception exc)
                                        {
                                            UnityEngine.Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, v.buttonText, exc.StackTrace, exc.Message));
                                        }
                                    }
                                }
                            } catch { }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(string.Format("iiMenu <b>FATAL ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
            }
        }

        public static Color GetBGColor(float offset)
        {
            Color oColor = bgColorA;
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = bgColorA;
            array[0].time = 0f;
            array[1].color = bgColorB;
            array[1].time = 0.5f;
            array[2].color = bgColorA;
            array[2].time = 1f;

            Gradient bg = new Gradient
            {
                colorKeys = array
            };
            oColor = bg.Evaluate(((Time.time / 2f) + offset) % 1f);
            if (themeType == 6)
            {
                float h = ((Time.frameCount / 180f) + offset) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
            }
            if (themeType == 47)
            {
                oColor = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
            }
            if (themeType == 51)
            {
                float h = (Time.frameCount / 180f) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 0.3f, 1f);
            }
             if (themeType == 8)
            {
                if (!Menu.Main.PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    oColor = GorillaTagger.Instance.offlineVRRig.mainSkin.material.color;
                }
                else
                {
                    oColor = new Color32(255, 111, 0, 255);
                }
            }

            return oColor;
        }

        public static Color GetBRColor(float offset)
        {
            Color oColor = buttonDefaultA;
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = buttonDefaultA;
            array[0].time = 0f;
            array[1].color = buttonDefaultB;
            array[1].time = 0.5f;
            array[2].color = buttonDefaultA;
            array[2].time = 1f;

            Gradient bg = new Gradient
            {
                colorKeys = array
            };
            oColor = bg.Evaluate(((Time.time / 2f) + offset) % 1f);

            return oColor;
        }

        public static Color GetBDColor(float offset)
        {
            Color oColor = buttonClickedA;
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = buttonClickedA;
            array[0].time = 0f;
            array[1].color = buttonClickedB;
            array[1].time = 0.5f;
            array[2].color = buttonClickedA;
            array[2].time = 1f;

            Gradient bg = new Gradient
            {
                colorKeys = array
            };
            oColor = bg.Evaluate(((Time.time / 2f) + offset) % 1f);
            if (themeType == 6)
            {
                float h = ((Time.frameCount / 180f) + offset) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
            }
            if (themeType == 47)
            {
                oColor = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
            }
            if (themeType == 51)
            {
                float h = (Time.frameCount / 180f) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 0.3f, 1f);
            }
            if (themeType == 8)
            {
                if (!Menu.Main.PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    oColor = GorillaTagger.Instance.offlineVRRig.mainSkin.material.color;
                }
                else
                {
                    oColor = new Color32(255, 111, 0, 255);
                }
            }

            return oColor;
        }

        private static void AddButton(float offset, int buttonIndex, ButtonInfo method)
        {
            if (!method.label)
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isPcWhenSearching)
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                if (FATMENU)
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                if (longmenu && buttonIndex > (pageSize - 1))
                {
                    menuBackground.transform.localScale += new Vector3(0f, 0f, 0.1f);
                    menuBackground.transform.localPosition += new Vector3(0f, 0f, -0.05f);
                }
                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
                if (checkMode && buttonIndex > -1)
                {
                    // The Checkbox Theorem ; TO BE THE SQUARE, YOU MUST circumvent the inconvenient menu localScale parameter
                    // Variable calculations: (menu scale y)0.3825 / (menu scale z)0.3 = 1.275 = Y
                    // 0.08 x Y = 0.102
                    gameObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
                    if (FATMENU)
                    {
                        gameObject.transform.localPosition = new Vector3(0.56f, 0.399f, 0.28f - offset);
                    }
                    else
                    {
                        gameObject.transform.localPosition = new Vector3(0.56f, 0.599f, 0.28f - offset);
                    }
                }
                gameObject.AddComponent<Classes.Button>().relatedText = method.buttonText;

                if (shouldOutline)
                {
                    OutlineObj(gameObject, !method.enabled);
                }

                if (lastClickedName != method.buttonText)
                {
                    GradientColorKey[] pressedColors = new GradientColorKey[3];
                    pressedColors[0].color = buttonClickedA;
                    pressedColors[0].time = 0f;
                    pressedColors[1].color = buttonClickedB;
                    pressedColors[1].time = 0.5f;
                    pressedColors[2].color = buttonClickedA;
                    pressedColors[2].time = 1f;

                    GradientColorKey[] releasedColors = new GradientColorKey[3];
                    releasedColors[0].color = buttonDefaultA;
                    releasedColors[0].time = 0f;
                    releasedColors[1].color = buttonDefaultB;
                    releasedColors[1].time = 0.5f;
                    releasedColors[2].color = buttonDefaultA;
                    releasedColors[2].time = 1f;

                    GradientColorKey[] selectedColors = new GradientColorKey[3];
                    selectedColors[0].color = Color.red;
                    selectedColors[0].time = 0f;
                    selectedColors[1].color = buttonDefaultB;
                    selectedColors[1].time = 0.5f;
                    selectedColors[2].color = Color.red;
                    selectedColors[2].time = 1f;

                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    if (method.enabled)
                    {
                        colorChanger.isRainbow = themeType == 6;
                        colorChanger.isPastelRainbow = themeType == 51;
                        colorChanger.isEpileptic = themeType == 47;
                        colorChanger.isMonkeColors = themeType == 8;
                        colorChanger.colors = new Gradient
                        {
                            colorKeys = pressedColors
                        };
                    }
                    else
                    {
                        colorChanger.isRainbow = false;
                        colorChanger.isPastelRainbow = false;
                        colorChanger.isEpileptic = false;
                        colorChanger.isMonkeColors = false;
                        colorChanger.colors = new Gradient
                        {
                            colorKeys = releasedColors
                        };
                    }
                    if (joystickMenu && buttonIndex == joystickButtonSelected)
                    {
                        joystickSelectedButton = method.buttonText;
                        colorChanger.isRainbow = false;
                        colorChanger.isMonkeColors = false;
                        if (method.enabled)
                        {
                            selectedColors[1].color = buttonClickedB;
                        }
                        colorChanger.colors = new Gradient
                        {
                            colorKeys = selectedColors
                        };
                    }
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(buttonIndex, method.buttonText, gameObject.GetComponent<Renderer>()));
                }
            }

            Text text2 = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text2.font = activeFont;
            text2.text = method.buttonText;
            if (method.overlapText != null)
            {
                text2.text = method.overlapText;
            }
            if (translate)
            {
                text2.text = TranslateText(text2.text);
            }
            if (inputTextColor != "green")
            {
                text2.text = text2.text.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color="+inputTextColor+">");
            }
            if (lowercaseMode)
            {
                text2.text = text2.text.ToLower();
            }
            if (favorites.Contains(method.buttonText))
            {
                text2.text += " ✦";
            }
            text2.supportRichText = true;
            text2.fontSize = 1;
            text2.color = textColor;
            if (method.enabled)
            {
                text2.color = textClicked;
            }
            if (joystickMenu && buttonIndex == joystickButtonSelected)
            {
                if (themeType == 30)
                {
                    text2.color = Color.red;
                }
            }
            text2.alignment = TextAnchor.MiddleCenter;
            text2.fontStyle = activeFontStyle;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;
            RectTransform component = text2.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            if (NoAutoSizeText)
            {
                component.sizeDelta = new Vector2(9f, 0.015f);
            }
            component.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        private static void AddSearchButton()
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isPcWhenSearching)
            {
                gameObject.layer = 2;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            if (themeType == 30)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            
            gameObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            // Fat menu theorem
            // To get the fat position of a button:
            // original x * (0.7 / 0.45) or 1.555555556
            if (FATMENU)
            {
                gameObject.transform.localPosition = new Vector3(0.56f, -0.450f, -0.58f);
            } else
            {
                gameObject.transform.localPosition = new Vector3(0.56f, -0.7f, -0.58f);
            }
            gameObject.AddComponent<Classes.Button>().relatedText = "Search";

            if (shouldOutline)
            {
                OutlineObj(gameObject, !isSearching);
            }

            GradientColorKey[] pressedColors = new GradientColorKey[3];
            pressedColors[0].color = buttonClickedA;
            pressedColors[0].time = 0f;
            pressedColors[1].color = buttonClickedB;
            pressedColors[1].time = 0.5f;
            pressedColors[2].color = buttonClickedA;
            pressedColors[2].time = 1f;

            GradientColorKey[] releasedColors = new GradientColorKey[3];
            releasedColors[0].color = buttonDefaultA;
            releasedColors[0].time = 0f;
            releasedColors[1].color = buttonDefaultB;
            releasedColors[1].time = 0.5f;
            releasedColors[2].color = buttonDefaultA;
            releasedColors[2].time = 1f;

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            if (isSearching)
            {
                colorChanger.isRainbow = themeType == 6;
                colorChanger.isPastelRainbow = themeType == 51;
                colorChanger.isEpileptic = themeType == 47;
                colorChanger.isMonkeColors = themeType == 8;
                colorChanger.colors = new Gradient
                {
                    colorKeys = pressedColors
                };
            }
            else
            {
                colorChanger.isRainbow = false;
                colorChanger.isEpileptic = false;
                colorChanger.isMonkeColors = false;
                colorChanger.colors = new Gradient
                {
                    colorKeys = releasedColors
                };
            }
            colorChanger.Start();
            Image image = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (searchIcon == null)
            {
                searchIcon = LoadTextureFromResource("iiMenu.Resources.search.png");
            }
            if (searchMat == null)
            {
                searchMat = new Material(image.material);
            }
            image.material = searchMat;
            image.material.SetTexture("_MainTex", searchIcon);
            image.color = isSearching ? textClicked : textColor;
            RectTransform component = image.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.03f, .03f);
            if (FATMENU)
            {
                component.localPosition = new Vector3(.064f, -0.35f / 2.6f, -0.58f / 2.6f);
            } else
            {
                component.localPosition = new Vector3(.064f, -0.54444444444f / 2.6f, -0.58f / 2.6f);
            }
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        private static void AddReturnButton(bool offcenteredPosition)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isPcWhenSearching)
            {
                gameObject.layer = 2;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            if (themeType == 30)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;

            gameObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
            // Fat menu theorem
            // To get the fat position of a button:
            // original x * (0.7 / 0.45) or 1.555555556
            if (FATMENU)
            {
                gameObject.transform.localPosition = new Vector3(0.56f, -0.450f, -0.58f);
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(0.56f, -0.7f, -0.58f);
            }
            if (offcenteredPosition)
            {
                gameObject.transform.localPosition += new Vector3(0f, 0.16f, 0f);
            }
            gameObject.AddComponent<Classes.Button>().relatedText = "Global Return";

            if (shouldOutline)
            {
                OutlineObj(gameObject, true);
            }

            GradientColorKey[] releasedColors = new GradientColorKey[3];
            releasedColors[0].color = buttonDefaultA;
            releasedColors[0].time = 0f;
            releasedColors[1].color = buttonDefaultB;
            releasedColors[1].time = 0.5f;
            releasedColors[2].color = buttonDefaultA;
            releasedColors[2].time = 1f;

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.isRainbow = false;
            colorChanger.isEpileptic = false;
            colorChanger.isMonkeColors = false;
            colorChanger.colors = new Gradient
            {
                colorKeys = releasedColors
            };
            colorChanger.Start();
            Image image = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (returnIcon == null)
            {
                returnIcon = LoadTextureFromResource("iiMenu.Resources.return.png");
            }
            if (returnMat == null)
            {
                returnMat = new Material(image.material);
            }
            image.material = returnMat;
            image.material.SetTexture("_MainTex", returnIcon);
            image.color = textColor;
            RectTransform component = image.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.03f, .03f);
            if (FATMENU)
            {
                component.localPosition = new Vector3(.064f, -0.35f / 2.6f, -0.58f / 2.6f);
            }
            else
            {
                component.localPosition = new Vector3(.064f, -0.54444444444f / 2.6f, -0.58f / 2.6f);
            }
            if (offcenteredPosition)
            {
                component.localPosition += new Vector3(0f, 0.0475f, 0f);
            }
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void CreateReference()
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton) ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            reference.GetComponent<Renderer>().material.color = bgColorA;
            reference.transform.localPosition = pointerOffset;
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();
        }

        public static void Draw()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menu.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menu.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
            if (scaleWithPlayer)
            {
                menu.transform.localScale *= GorillaLocomotion.Player.Instance.scale;
            }
            if (annoyingMode)
            {
                menu.transform.localScale = new Vector3(0.1f, UnityEngine.Random.Range(10f, 40f) / 100f, 0.3825f);
                bgColorA = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                bgColorB = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                textColor = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                buttonClickedA = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                buttonClickedB = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                buttonDefaultA = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                buttonDefaultB = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
            }

            if (themeType == 7)
            {
                GameObject gameObject = LoadAsset("Cone");

                gameObject.transform.parent = menu.transform;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(menuBackground.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(menuBackground.GetComponent<BoxCollider>());

                if (themeType == 30)
                {
                    menuBackground.GetComponent<Renderer>().enabled = false;
                }

                menuBackground.transform.parent = menu.transform;
                menuBackground.transform.localPosition = new Vector3(0.50f, 0f, 0f);
                menuBackground.transform.rotation = Quaternion.identity;

                // Size is calculated in depth, width, height
                // Wtf
                if (FATMENU)
                {
                    menuBackground.transform.localScale = new Vector3(0.1f, 1f, 1f);
                }
                else
                {
                    menuBackground.transform.localScale = new Vector3(0.1f, 1.5f, 1f);
                }
                menuBackground.GetComponent<Renderer>().material.color = bgColorA;
                
                if (GetIndex("Inner Outline Menu").enabled || themeType == 34)
                {
                    GameObject outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.transform.parent = menuBackground.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, -0.4840625f, 0f);
                    outlinepart.transform.localScale = new Vector3(1.025f, 0.0065f, 0.98f);
                    GradientColorKey[] array = new GradientColorKey[3];
                    array[0].color = buttonClickedA;
                    array[0].time = 0f;
                    array[1].color = buttonClickedB;
                    array[1].time = 0.5f;
                    array[2].color = buttonClickedA;
                    array[2].time = 1f;
                    ColorChanger colorChanger = outlinepart.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isPastelRainbow = themeType == 51;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.Start();

                    outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.transform.parent = menuBackground.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, 0.4840625f, 0f);
                    outlinepart.transform.localScale = new Vector3(1.025f, 0.0065f, 0.98f);
                    array = new GradientColorKey[3];
                    array[0].color = buttonClickedA;
                    array[0].time = 0f;
                    array[1].color = buttonClickedB;
                    array[1].time = 0.5f;
                    array[2].color = buttonClickedA;
                    array[2].time = 1f;
                    colorChanger = outlinepart.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isPastelRainbow = themeType == 51;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.Start();

                    outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.transform.parent = menuBackground.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, 0f, -0.4875f);
                    outlinepart.transform.localScale = new Vector3(1.025f, 0.968125f, 0.005f);
                    array = new GradientColorKey[3];
                    array[0].color = buttonClickedA;
                    array[0].time = 0f;
                    array[1].color = buttonClickedB;
                    array[1].time = 0.5f;
                    array[2].color = buttonClickedA;
                    array[2].time = 1f;
                    colorChanger = outlinepart.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isPastelRainbow = themeType == 51;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.Start();

                    outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.transform.parent = menuBackground.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, 0f, 0.4875f);
                    outlinepart.transform.localScale = new Vector3(1.025f, 0.968125f, 0.005f);
                    array = new GradientColorKey[3];
                    array[0].color = buttonClickedA;
                    array[0].time = 0f;
                    array[1].color = buttonClickedB;
                    array[1].time = 0.5f;
                    array[2].color = buttonClickedA;
                    array[2].time = 1f;
                    colorChanger = outlinepart.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isPastelRainbow = themeType == 51;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.Start();
                }
                if (shouldOutline)
                {
                    OutlineObj(menuBackground, false);
                }
                if (themeType == 25 || themeType == 26 || themeType == 27)
                {
                    try
                    {
                        switch (themeType)
                        {
                            case 25:
                                if (pride == null)
                                {
                                    pride = LoadTextureFromResource("iiMenu.Resources.pride.png");
                                    pride.filterMode = FilterMode.Point;
                                    pride.wrapMode = TextureWrapMode.Clamp;
                                }
                                menuBackground.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                menuBackground.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                                menuBackground.GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
                                menuBackground.GetComponent<Renderer>().material.color = Color.white;
                                menuBackground.GetComponent<Renderer>().material.mainTexture = pride;
                                UnityEngine.Debug.Log("gayed the texture");
                                break;
                            case 26:
                                if (trans == null)
                                {
                                    trans = LoadTextureFromResource("iiMenu.Resources.trans.png");
                                    trans.filterMode = FilterMode.Point;
                                    trans.wrapMode = TextureWrapMode.Clamp;
                                }
                                menuBackground.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                menuBackground.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                                menuBackground.GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
                                menuBackground.GetComponent<Renderer>().material.color = Color.white;
                                menuBackground.GetComponent<Renderer>().material.mainTexture = trans;
                                break;
                            case 27:
                                if (gay == null)
                                {
                                    gay = LoadTextureFromResource("iiMenu.Resources.mlm.png");
                                    gay.filterMode = FilterMode.Point;
                                    gay.wrapMode = TextureWrapMode.Clamp;
                                }
                                menuBackground.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                menuBackground.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                                menuBackground.GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
                                menuBackground.GetComponent<Renderer>().material.color = Color.white;
                                menuBackground.GetComponent<Renderer>().material.mainTexture = gay;
                                break;
                        }
                    }
                    catch (Exception exception) { UnityEngine.Debug.LogError(string.Format("iiMenu <b>TEXTURE ERROR</b> {1} - {0}", exception.Message, exception.StackTrace)); }
                }
                else
                {
                    if (doCustomMenuBackground)
                    {
                        menuBackground.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        menuBackground.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                        menuBackground.GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
                        menuBackground.GetComponent<Renderer>().material.color = Color.white;
                        menuBackground.GetComponent<Renderer>().material.mainTexture = customMenuBackgroundImage;
                    }
                    else
                    {
                        GradientColorKey[] array = new GradientColorKey[3];
                        array[0].color = bgColorA;
                        array[0].time = 0f;
                        array[1].color = bgColorB;
                        array[1].time = 0.5f;
                        array[2].color = bgColorA;
                        array[2].time = 1f;
                        ColorChanger colorChanger = menuBackground.AddComponent<ColorChanger>();
                        colorChanger.colors = new Gradient
                        {
                            colorKeys = array
                        };
                        colorChanger.isRainbow = themeType == 6;
                        colorChanger.isPastelRainbow = themeType == 51;
                        colorChanger.isEpileptic = themeType == 47;
                        colorChanger.isMonkeColors = themeType == 8;
                        colorChanger.Start();
                    }
                }
            }
            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            if (GetIndex("Hide Text on Camera").enabled) { canvasObj.layer = 19; }
            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;
            if (scaleWithPlayer)
            {
                canvas.transform.localScale *= GorillaLocomotion.Player.Instance.scale;
            }

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text.font = activeFont;
            text.text = "ii's <b>Stupid</b> Menu";
            if (doCustomName)
            {
                text.text = customMenuName;
            }
            if (annoyingMode)
            {
                string[] randomMenuNames = new string[]
                {
                    "ModderX",
                    "ShibaGT Gold",
                    "Kman Menu",
                    "WM TROLLING MENU",
                    "ShibaGT Dark",
                    "ShibaGT-X v5.5",
                    "bvunt menu",
                    "GorillaTaggingKid Menu",
                    "fart",
                    "steal.lol"
                };
                if (UnityEngine.Random.Range(1, 5) == 2)
                {
                    text.text = randomMenuNames[UnityEngine.Random.Range(0, randomMenuNames.Length - 1)] + " v" + UnityEngine.Random.Range(8, 159);
                }
            }
            if (translate)
            {
                text.text = TranslateText(text.text);
            }
            if (lowercaseMode)
            {
                text.text = text.text.ToLower();
            }
            if (!noPageNumber)
            {
                text.text += " <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
            }
            text.fontSize = 1;
            text.color = titleColor;
            title = text;
            text.supportRichText = true;
            text.fontStyle = activeFontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            if (NoAutoSizeText)
            {
                component.sizeDelta = new Vector2(0.28f, 0.015f);
            }
            component.localPosition = new Vector3(0.06f, 0f, 0.165f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            text = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text.font = activeFont;
            text.text = "Build " + PluginInfo.Version;
            if (themeType == 30)
            {
                text.text = "";
            }
            if (translate)
            {
                text.text = TranslateText(text.text);
            }
            if (lowercaseMode)
            {
                text.text = text.text.ToLower();
            }
            text.fontSize = 1;
            text.color = titleColor;
            text.supportRichText = true;
            text.fontStyle = activeFontStyle;
            text.alignment = TextAnchor.MiddleRight;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.02f);
            if (FATMENU)
            {
                component.position = new Vector3(0.04f, 0.0f, -0.17f);
            }
            else
            {
                component.position = new Vector3(0.04f, 0.07f, -0.17f);
            }
            component.rotation = Quaternion.Euler(new Vector3(0f, 90f, 90f));

            if (!disableFpsCounter)
            {
                Text fps = new GameObject
                {
                    transform =
                {
                    parent = canvasObj.transform
                }
                }.AddComponent<Text>();
                fps.font = activeFont;
                fps.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                if (lowercaseMode)
                {
                    fps.text = fps.text.ToLower();
                }
                fps.color = titleColor;
                fpsCount = fps;
                fps.fontSize = 1;
                fps.supportRichText = true;
                fps.fontStyle = activeFontStyle;
                fps.alignment = TextAnchor.MiddleCenter;
                fps.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
                fps.resizeTextForBestFit = true;
                fps.resizeTextMinSize = 0;
                RectTransform component2 = fps.GetComponent<RectTransform>();
                component2.localPosition = Vector3.zero;
                component2.sizeDelta = new Vector2(0.28f, 0.02f);
                component2.localPosition = new Vector3(0.06f, 0f, 0.135f);
                if (NoAutoSizeText)
                {
                    component2.sizeDelta = new Vector2(9f, 0.015f);
                }
                component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }

            if (!disableDisconnectButton)
            {
                if (hotkeyButton == "none")
                {
                    AddButton(-0.30f, -1, GetIndex("Disconnect"));
                } else
                {
                    AddButton(-0.30f, -1, GetIndex("Disconnect"));
                    ButtonInfo hkb = GetIndex(hotkeyButton);
                    if (hkb != null)
                    {
                        AddButton(-0.40f, -1, hkb);
                    }
                }
            }

            // Search button
            if (!disableSearchButton)
            {
                AddSearchButton();
                if (!disableReturnButton && buttonsType != 0)
                {
                    AddReturnButton(true);
                }
            }
            else
            {
                if (!disableReturnButton && buttonsType != 0)
                {
                    AddReturnButton(false);
                }
            }

            /*
            // Unity bug where all Image objects have their material property shared, manual buggy fix
            // It was not a Unity bug I was just being retarded
            Image image = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Image>();
            if (fixTexture == null)
            {
                fixTexture = new Texture2D(2, 2);
            }
            if (fixMat == null)
            {
                fixMat = new Material(image.material);
            }
            image.material = fixMat;
            image.material.SetTexture("_MainTex", fixTexture);
            UnityEngine.Object.Destroy(image);
            */

            if (!disablePageButtons)
            {
                AddPageButtons();
            }

            if (isSearching)
            {
                // Draw the search box
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isPcWhenSearching)
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                if (FATMENU)
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - (buttonOffset / 10));

                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }

                GradientColorKey[] releasedColors = new GradientColorKey[3];
                releasedColors[0].color = buttonDefaultA;
                releasedColors[0].time = 0f;
                releasedColors[1].color = buttonDefaultB;
                releasedColors[1].time = 0.5f;
                releasedColors[2].color = buttonDefaultA;
                releasedColors[2].time = 1f;

                GradientColorKey[] selectedColors = new GradientColorKey[3];
                selectedColors[0].color = Color.red;
                selectedColors[0].time = 0f;
                selectedColors[1].color = buttonDefaultB;
                selectedColors[1].time = 0.5f;
                selectedColors[2].color = Color.red;
                selectedColors[2].time = 1f;

                ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                colorChanger.isRainbow = false;
                colorChanger.isEpileptic = false;
                colorChanger.isMonkeColors = false;
                colorChanger.colors = new Gradient
                {
                    colorKeys = releasedColors
                };
                if (joystickMenu && joystickButtonSelected == 0)
                {
                    joystickSelectedButton = "literallythesearchbar";
                    colorChanger.isRainbow = false;
                    colorChanger.isMonkeColors = false;
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = selectedColors
                    };
                }
                colorChanger.Start();
                Text text2 = new GameObject
                {
                    transform =
                {
                    parent = canvasObj.transform
                }
                }.AddComponent<Text>();
                searchTextObject = text2;
                text2.font = activeFont;
                text2.text = searchText + (((Time.frameCount / 45) % 2) == 0 ? "|" : "");
                if (lowercaseMode)
                {
                    text2.text = text2.text.ToLower();
                }
                text2.supportRichText = true;
                text2.fontSize = 1;
                text2.color = textColor;
                if (joystickMenu && joystickButtonSelected == 0)
                {
                    if (themeType == 30)
                    {
                        text2.color = Color.red;
                    }
                }
                text2.alignment = TextAnchor.MiddleCenter;
                text2.fontStyle = activeFontStyle;
                text2.resizeTextForBestFit = true;
                text2.resizeTextMinSize = 0;
                RectTransform componentdos = text2.GetComponent<RectTransform>();
                componentdos.localPosition = Vector3.zero;
                componentdos.sizeDelta = new Vector2(.2f, .03f);
                if (NoAutoSizeText)
                {
                    componentdos.sizeDelta = new Vector2(9f, 0.015f);
                }
                componentdos.localPosition = new Vector3(.064f, 0, .111f - (buttonOffset / 10) / 2.6f);
                componentdos.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                // Search the mod database
                List<ButtonInfo> searchedMods = new List<ButtonInfo> { };
                Regex notags = new Regex("<.*?>");
                if (nonGlobalSearch && buttonsType != 0)
                {
                    foreach (ButtonInfo v in Buttons.buttons[buttonsType])
                    {
                        try
                        {
                            string buttonText = v.buttonText;
                            if (v.overlapText != null)
                            {
                                buttonText = v.overlapText;
                            }

                            if (translate)
                            {
                                buttonText = TranslateText(buttonText);
                            }

                            if (buttonText.Replace(" ", "").ToLower().Contains(searchText.Replace(" ", "").ToLower()))
                            {
                                searchedMods.Add(v);
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            try
                            {
                                string buttonText = v.buttonText;
                                if (v.overlapText != null)
                                {
                                    buttonText = v.overlapText;
                                }

                                if (translate)
                                {
                                    buttonText = TranslateText(buttonText);
                                }

                                if (buttonText.Replace(" ", "").ToLower().Contains(searchText.Replace(" ", "").ToLower()))
                                {
                                    searchedMods.Add(v);
                                }
                            }
                            catch { }
                        }
                    }
                }
                ButtonInfo[] array2 = StringsToInfos(Alphabetize(InfosToStrings(searchedMods.ToArray())));
                array2 = array2.Skip(pageNumber * (pageSize-1)).Take(pageSize-1).ToArray();
                if (longmenu) { array2 = searchedMods.ToArray(); }
                for (int i = 0; i < array2.Length; i++)
                {
                    AddButton((i + 1) * 0.1f + (buttonOffset / 10), i , array2[i]);
                }
            }
            else
            {
                if (annoyingMode && UnityEngine.Random.Range(1, 5) == 3)
                {
                    ButtonInfo disconnect = GetIndex("Disconnect");
                    ButtonInfo[] array2 = new ButtonInfo[] { disconnect, disconnect, disconnect, disconnect, disconnect, disconnect, disconnect, disconnect, disconnect, disconnect };
                    array2 = array2.Take(pageSize).ToArray();
                    if (longmenu) { array2 = Buttons.buttons[buttonsType]; }
                    for (int i = 0; i < array2.Length; i++)
                    {
                        AddButton(i * 0.1f + (buttonOffset / 10), i, array2[i]);
                    }
                }
                else
                {
                    if (buttonsType == 19)
                    {
                        string[] array2 = favorites.Skip(pageNumber * pageSize).Take(pageSize).ToArray();
                        if (GetIndex("Alphabetize Menu").enabled) { array2 = Alphabetize(favorites.ToArray()); array2 = array2.Skip(pageNumber * pageSize).Take(pageSize).ToArray(); }
                        if (longmenu) { array2 = favorites.ToArray(); }
                        for (int i = 0; i < array2.Length; i++)
                        {
                            ButtonInfo fav = GetIndex(array2[i]);
                            if (fav != null)
                            {
                                AddButton(i * 0.1f + (buttonOffset / 10), i, fav);
                            } else
                            {
                                favorites.Remove(array2[i]);
                            }
                        }
                    }
                    else
                    {
                        if (buttonsType == 24)
                        {
                            List<string> enabledMods = new List<string>() { "Exit Enabled Mods" };
                            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                            {
                                foreach (ButtonInfo v in buttonlist)
                                {
                                    if (v.enabled)
                                    {
                                        enabledMods.Add(v.buttonText);
                                    }
                                }
                            }

                            string[] array2 = enabledMods.ToArray().Skip(pageNumber * pageSize).Take(pageSize).ToArray();
                            if (GetIndex("Alphabetize Menu").enabled) { array2 = Alphabetize(enabledMods.ToArray()); array2 = array2.Skip(pageNumber * pageSize).Take(pageSize).ToArray(); }
                            if (longmenu) { array2 = enabledMods.ToArray(); }
                            for (int i = 0; i < array2.Length; i++)
                            {
                                AddButton(i * 0.1f + (buttonOffset / 10), i, GetIndex(array2[i]));
                            }
                        }
                        else
                        {
                            ButtonInfo[] array2 = Buttons.buttons[buttonsType].Skip(pageNumber * pageSize).Take(pageSize).ToArray();
                            if (GetIndex("Alphabetize Menu").enabled) { array2 = StringsToInfos(Alphabetize(InfosToStrings(Buttons.buttons[buttonsType]))); array2 = array2.Skip(pageNumber * pageSize).Take(pageSize).ToArray(); }
                            if (longmenu) { array2 = Buttons.buttons[buttonsType]; }
                            for (int i = 0; i < array2.Length; i++)
                            {
                                AddButton(i * 0.1f + (buttonOffset / 10), i, array2[i]);
                            }
                        }
                    }
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
                    UnityEngine.Object.Destroy(particle.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(particle.GetComponent<BoxCollider>());

                    UnityEngine.Object.Destroy(particle, 2f);

                    particle.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                    particle.GetComponent<Renderer>().material.color = Color.white;
                    if (cannmat == null)
                    {
                        cannmat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                        cannmat.color = Color.white;
                        if (cann == null)
                        {
                            cann = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/cannabis.png", "cannabis.png");
                        }
                        cannmat.mainTexture = cann;

                        cannmat.SetFloat("_Surface", 1);
                        cannmat.SetFloat("_Blend", 0);
                        cannmat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        cannmat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        cannmat.SetFloat("_ZWrite", 0);
                        cannmat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        cannmat.renderQueue = (int)RenderQueue.Transparent;

                        cannmat.SetFloat("_Glossiness", 0.0f);
                        cannmat.SetFloat("_Metallic", 0.0f);
                    }
                    particle.GetComponent<Renderer>().material = cannmat;

                    Rigidbody comp = particle.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.position = menuBackground.transform.position;
                    comp.velocity = new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(3f, 5f), UnityEngine.Random.Range(-3f, 3f));
                    comp.angularVelocity = new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f));
                }
            }
        }

        public static void RecenterMenu()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q) || (isSearching && isPcWhenSearching);
            if (joystickMenu)
            {
                menu.transform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward + GorillaTagger.Instance.headCollider.transform.right * 0.3f + GorillaTagger.Instance.headCollider.transform.up * 0.2f;
                menu.transform.LookAt(GorillaTagger.Instance.headCollider.transform);
                Vector3 rotModify = menu.transform.rotation.eulerAngles;
                rotModify += new Vector3(-90f, 0f, -90f);
                menu.transform.rotation = Quaternion.Euler(rotModify);
            }
            else
            {
                if (!wristThing)
                {
                    if (likebark)
                    {
                        menu.transform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * 0.5f + GorillaTagger.Instance.headCollider.transform.up * -0.1f;
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
                        if (isSearching && !isPcWhenSearching)
                        {
                            if (Vector3.Distance(VRKeyboard.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) > 1f)
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
                    {
                        menu.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.3f, 0f);
                    }
                    else
                    {
                        menu.transform.position = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, 0.3f, 0f);
                    }
                    menu.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    Vector3 rotModify = menu.transform.rotation.eulerAngles;
                    rotModify += new Vector3(-90f, 0f, -90f);
                    menu.transform.rotation = Quaternion.Euler(rotModify);
                }
            }
            if (isKeyboardCondition)
            {
                /*
                TPC = null;
                try
                {
                    TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                }
                catch { }*/
                if (TPC != null)
                {
                    isOnPC = true;
                    if (joystickMenu)
                    {
                        Toggle("Joystick Menu");
                    }
                    if (wristThingV2)
                    {
                        Toggle("Watch Menu");
                    }
                    if (GetIndex("First Person Camera").enabled)
                    {
                        Toggle("First Person Camera");
                    }
                    Vector3[] pcpositions = new Vector3[]
                    {
                        new Vector3(10f, 10f, 10f),
                        new Vector3(10f, 10f, 10f),
                        new Vector3(-67.9299f, 11.9144f, -84.2019f),
                        new Vector3(-63f, 3.634f, -65f)
                    };
                    TPC.transform.position = pcpositions[pcbg];
                    TPC.transform.rotation = Quaternion.identity;
                    if (pcbg == 0)
                    {
                        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        bg.transform.localScale = new Vector3(10f, 10f, 0.01f);
                        bg.transform.transform.position = TPC.transform.position + TPC.transform.forward;
                        Color realcolor = GetBGColor(0f);
                        bg.GetComponent<Renderer>().material.color = new Color32((byte)(realcolor.r * 50), (byte)(realcolor.g * 50), (byte)(realcolor.b * 50), 255);
                        GameObject.Destroy(bg, Time.deltaTime * 3f);
                    }
                    menu.transform.parent = TPC.transform;
                    menu.transform.position = TPC.transform.position + (TPC.transform.forward * 0.5f) + (TPC.transform.up * 0f);
                    Vector3 rot = TPC.transform.rotation.eulerAngles;
                    rot += new Vector3(-90f, 90f, 0f);
                    menu.transform.rotation = Quaternion.Euler(rot);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed && !lastclicking)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            bool worked = Physics.Raycast(ray, out RaycastHit hit, 512f);
                            if (worked)
                            {
                                Classes.Button collide = hit.transform.gameObject.GetComponent<Classes.Button>();
                                if (collide != null)
                                {
                                    collide.OnTriggerEnter(buttonCollider);
                                    buttonCooldown = -1f;
                                }
                            }
                        }
                        else
                        {
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                        }
                        lastclicking = Mouse.current.leftButton.isPressed;
                    }
                }
            } else
            {
                isOnPC = false;
            }

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
        }

        private static void AddPageButtons()
        {
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = buttonDefaultA;
            array[0].time = 0f;
            array[1].color = buttonDefaultB;
            array[1].time = 0.5f;
            array[2].color = buttonDefaultA;
            array[2].time = 1f;

            if (pageButtonType == 1)
            {
                float num4 = 0f;
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                if (FATMENU)
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num4);
                gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "PreviousPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                } else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "PreviousPage", gameObject.GetComponent<Renderer>()));
                }
                
                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][0];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                if (NoAutoSizeText)
                {
                    component.sizeDelta = new Vector2(9f, 0.015f);
                }
                component.localPosition = new Vector3(0.064f, 0f, 0.109f - num4 / 2.55f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                num4 = 0.1f;
                GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject2.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject2.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject2.GetComponent<Rigidbody>());
                gameObject2.GetComponent<BoxCollider>().isTrigger = true;
                gameObject2.transform.parent = menu.transform;
                gameObject2.transform.rotation = Quaternion.identity;
                if (FATMENU)
                {
                    gameObject2.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    gameObject2.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                gameObject2.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num4);
                gameObject2.AddComponent<Classes.Button>().relatedText = "NextPage";
                gameObject2.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "NextPage")
                {
                    ColorChanger colorChanger = gameObject2.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "NextPage", gameObject2.GetComponent<Renderer>()));
                }
                Text text2 = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text2.font = activeFont;
                text2.text = arrowTypes[arrowType][1];
                text2.fontSize = 1;
                text2.color = textColor;
                text2.alignment = TextAnchor.MiddleCenter;
                text2.resizeTextForBestFit = true;
                text2.resizeTextMinSize = 0;
                RectTransform component2 = text2.GetComponent<RectTransform>();
                component2.localPosition = Vector3.zero;
                component2.sizeDelta = new Vector2(0.2f, 0.03f);
                if (NoAutoSizeText)
                {
                    component2.sizeDelta = new Vector2(9f, 0.015f);
                }
                component2.localPosition = new Vector3(0.064f, 0f, 0.109f - num4 / 2.55f);
                component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                    OutlineObj(gameObject2, true);
                }
            }

            if (pageButtonType == 2)
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.2f, 0.9f);
                if (FATMENU)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.65f, 0);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.9f, 0);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "PreviousPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "PreviousPage", gameObject.GetComponent<Renderer>()));
                }
                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][0];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                if (NoAutoSizeText)
                {
                    component.sizeDelta = new Vector2(9f, 0.015f);
                }
                if (FATMENU)
                {
                    component.localPosition = new Vector3(0.064f, 0.195f, 0f);
                }
                else
                {
                    component.localPosition = new Vector3(0.064f, 0.267f, 0f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.2f, 0.9f);
                if (FATMENU)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.65f, 0);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.9f, 0);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "NextPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "NextPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "NextPage", gameObject.GetComponent<Renderer>()));
                }
                text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][1];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                if (NoAutoSizeText)
                {
                    component.sizeDelta = new Vector2(9f, 0.015f);
                }
                if (FATMENU)
                {
                    component.localPosition = new Vector3(0.064f, -0.195f, 0f);
                }
                else
                {
                    component.localPosition = new Vector3(0.064f, -0.267f, 0f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }
            }

            if (pageButtonType == 5)
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.3f, 0.05f);
                if (FATMENU)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.299f, 0.355f);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.499f, 0.355f);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "PreviousPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "PreviousPage", gameObject.GetComponent<Renderer>()));
                }
                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][0];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                if (NoAutoSizeText)
                {
                    component.sizeDelta = new Vector2(9f, 0.015f);
                }
                if (FATMENU)
                {
                    component.localPosition = new Vector3(0.064f, 0.09f, 0.135f);
                }
                else
                {
                    component.localPosition = new Vector3(0.064f, 0.15f, 0.135f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }

                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.3f, 0.05f);
                if (FATMENU)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.299f, 0.355f);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.499f, 0.355f);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "NextPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "NextPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "NextPage", gameObject.GetComponent<Renderer>()));
                }
                text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][1];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                if (NoAutoSizeText)
                {
                    component.sizeDelta = new Vector2(9f, 0.015f);
                }
                if (FATMENU)
                {
                    component.localPosition = new Vector3(0.064f, -0.09f, 0.135f);
                }
                else
                {
                    component.localPosition = new Vector3(0.064f, -0.15f, 0.135f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }
            }

            if (pageButtonType == 6)
            { // Inverse of the search button
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
                if (FATMENU)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.450f, -0.58f);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.7f, -0.58f);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "PreviousPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "PreviousPage", gameObject.GetComponent<Renderer>()));
                }
                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][0];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(.03f, .03f);
                if (FATMENU)
                {
                    component.localPosition = new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.7f);
                }
                else
                {
                    component.localPosition = new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.7f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }

                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
                if (FATMENU)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.450f, -0.58f);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.7f, -0.58f);
                }
                gameObject.transform.localPosition -= new Vector3(0f, 0.16f, 0f);
                gameObject.AddComponent<Classes.Button>().relatedText = "NextPage";
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                if (lastClickedName != "NextPage")
                {
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.Start();
                }
                else
                {
                    CoroutineManager.RunCoroutine(ButtonClick(-99, "NextPage", gameObject.GetComponent<Renderer>()));
                }
                text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObj.transform
                    }
                }.AddComponent<Text>();
                text.font = activeFont;
                text.text = arrowTypes[arrowType][1];
                text.fontSize = 1;
                text.color = textColor;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(.03f, .03f);
                if (FATMENU)
                {
                    component.localPosition = new Vector3(.064f, 0.35f / 2.6f, -0.58f / 2.7f);
                }
                else
                {
                    component.localPosition = new Vector3(.064f, 0.54444444444f / 2.6f, -0.58f / 2.7f);
                }
                component.localPosition -= new Vector3(0f, 0.0475f, 0f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (shouldOutline)
                {
                    OutlineObj(gameObject, true);
                }
            }
        }

        public static void OutlineObj(GameObject toOut, bool shouldBeEnabled)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (themeType == 30)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localPosition = toOut.transform.localPosition;
            gameObject.transform.localScale = toOut.transform.localScale + new Vector3(-0.01f, 0.01f, 0.0075f);
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = shouldBeEnabled ? buttonClickedA : buttonDefaultA;
            array[0].time = 0f;
            array[1].color = shouldBeEnabled ? buttonClickedB : buttonDefaultB;
            array[1].time = 0.5f;
            array[2].color = shouldBeEnabled ? buttonClickedA : buttonDefaultA;
            array[2].time = 1f;
            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = new Gradient
            {
                colorKeys = array
            };
            colorChanger.isRainbow = shouldBeEnabled && themeType == 6;
            colorChanger.isMonkeColors = shouldBeEnabled && themeType == 8;
            colorChanger.isEpileptic = shouldBeEnabled && themeType == 47;
            colorChanger.Start();
        }

        public static void OutlineObjNonMenu(GameObject toOut, bool shouldBeEnabled)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (themeType == 30)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = toOut.transform.parent;
            gameObject.transform.rotation = toOut.transform.rotation;
            gameObject.transform.localPosition = toOut.transform.localPosition;
            gameObject.transform.localScale = toOut.transform.localScale + new Vector3(0.005f, 0.005f, -0.001f);
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = shouldBeEnabled ? buttonClickedA : buttonDefaultA;
            array[0].time = 0f;
            array[1].color = shouldBeEnabled ? buttonClickedB : buttonDefaultB;
            array[1].time = 0.5f;
            array[2].color = shouldBeEnabled ? buttonClickedA : buttonDefaultA;
            array[2].time = 1f;
            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = new Gradient
            {
                colorKeys = array
            };
            colorChanger.isRainbow = shouldBeEnabled && themeType == 6;
            colorChanger.isMonkeColors = shouldBeEnabled && themeType == 8;
            colorChanger.isEpileptic = shouldBeEnabled && themeType == 47;
            colorChanger.Start();
        }

        public static GameObject LoadAsset(string assetName)
        {
            GameObject gameObject = null;

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("iiMenu.Resources.iimenu");
            if (stream != null)
            {
                if (assetBundle == null)
                {
                    assetBundle = AssetBundle.LoadFromStream(stream);
                }
                gameObject = Instantiate<GameObject>(assetBundle.LoadAsset<GameObject>(assetName));
            }
            else
            {
                Debug.LogError("Failed to load asset from resource: " + assetName);
            }
            
            return gameObject;
        }

        public static Dictionary<string, AudioClip> audioPool = new Dictionary<string, AudioClip> { };
        public static AudioClip LoadSoundFromResource(string resourcePath)
        {
            AudioClip sound = null;

            if (!audioPool.ContainsKey(resourcePath))
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("iiMenu.Resources.iimenu");
                if (stream != null)
                {
                    if (assetBundle == null)
                    {
                        assetBundle = AssetBundle.LoadFromStream(stream);
                    }
                    sound = assetBundle.LoadAsset(resourcePath) as AudioClip;
                    audioPool.Add(resourcePath, sound);
                }
                else
                {
                    Debug.LogError("Failed to load sound from resource: " + resourcePath);
                }
            } else
            {
                sound = audioPool[resourcePath];
            }

            return sound;
        }

        public static Dictionary<string, AudioClip> audioFilePool = new Dictionary<string, AudioClip> { };
        public static AudioClip LoadSoundFromFile(string fileName) // Thanks to ShibaGT for help with loading the audio from file
        {
            AudioClip sound = null;

            if (!audioFilePool.ContainsKey(fileName))
            {
                if (!Directory.Exists("iisStupidMenu"))
                {
                    Directory.CreateDirectory("iisStupidMenu");
                }
                string filePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, "iisStupidMenu/" + fileName);
                filePath = filePath.Split("BepInEx\\")[0] + "iisStupidMenu/" + fileName;
                filePath = filePath.Replace("\\", "/");

                UnityWebRequest actualrequest = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, GetAudioType(GetFileExtension(fileName)));
                UnityWebRequestAsyncOperation newvar = actualrequest.SendWebRequest();
                while (!newvar.isDone) { }

                AudioClip actualclip = DownloadHandlerAudioClip.GetContent(actualrequest);
                sound = Task.FromResult(actualclip).Result;

                audioFilePool.Add(fileName, sound);
            }
            else
            {
                sound = audioFilePool[fileName];
            }

            return sound;
        }

        public static AudioClip LoadSoundFromURL(string resourcePath, string fileName)
        {
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            if (!File.Exists("iisStupidMenu/" + fileName))
            {
                UnityEngine.Debug.Log("Downloading " + fileName);
                WebClient stream = new WebClient();
                stream.DownloadFile(resourcePath, "iisStupidMenu/" + fileName);
            }

            return LoadSoundFromFile(fileName);
        }

        public static Texture2D LoadTextureFromResource(string resourcePath)
        {
            Texture2D texture = new Texture2D(2, 2);

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                byte[] fileData = new byte[stream.Length];
                stream.Read(fileData, 0, (int)stream.Length);
                texture.LoadImage(fileData);
            }
            else
            {
                Debug.LogError("Failed to load texture from resource: " + resourcePath);
            }
            return texture;
        }

        public static Texture2D LoadTextureFromURL(string resourcePath, string fileName)
        {
            Texture2D texture = new Texture2D(2, 2);
            
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            if (!File.Exists("iisStupidMenu/" + fileName))
            {
                UnityEngine.Debug.Log("Downloading " + fileName);
                WebClient stream = new WebClient();
                stream.DownloadFile(resourcePath, "iisStupidMenu/" + fileName);
            }

            byte[] bytes = File.ReadAllBytes("iisStupidMenu/" + fileName);
            texture.LoadImage(bytes);

            return texture;
        }

        private static Dictionary<Color[], Texture2D> cacheGradients = new Dictionary<Color[], Texture2D> { };
        public static Texture2D GetGradientTexture(Color colorA, Color colorB)
        {
            if (cacheGradients.ContainsKey(new Color[] { colorA, colorB }))
                return cacheGradients[new Color[] { colorA, colorB }];

            Texture2D txt2d = new Texture2D(128, 128);
            for (int i = 1; i <= 128; i++)
            {
                for (int j = 1; j <= 128; j++)
                {
                    Color clr = Color.Lerp(colorA, colorB, i / 128f);
                    txt2d.SetPixel(i, j, clr);
                }
            }
            txt2d.Apply();
            return txt2d;
        }

        public static void RPCProtection()
        {
            try
            {
                if (hasRemovedThisFrame == false)
                {
                    hasRemovedThisFrame = true;
                    if (GetIndex("Experimental RPC Protection").enabled)
                    {
                        PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            TargetActors = new int[]
                            {
                                PhotonNetwork.LocalPlayer.ActorNumber
                            }
                        }, SendOptions.SendReliable);
                    }
                    else
                    {
                        GorillaNot.instance.rpcErrorMax = int.MaxValue;
                        GorillaNot.instance.rpcCallLimit = int.MaxValue;
                        GorillaNot.instance.logErrorMax = int.MaxValue;

                        PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                        PhotonNetwork.QuickResends = int.MaxValue;

                        //PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig.GetView);
                        PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
                        PhotonNetwork.SendAllOutgoingCommands();

                        GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
                    }
                }
            } catch { UnityEngine.Debug.Log("RPC protection failed, are you in a lobby?"); }
        }

        public static string GetHttp(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = "";
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            return html;
        }

        private static Vector3 GunPositionSmoothed = Vector3.zero;
        public static (RaycastHit Ray, GameObject NewPointer) RenderGun()
        {
            Physics.Raycast(SwapGunHand ? (GorillaTagger.Instance.leftHandTransform.position - (legacyGunDirection ? GorillaTagger.Instance.leftHandTransform.up / 4f : Vector3.zero)) : (GorillaTagger.Instance.rightHandTransform.position - (legacyGunDirection ? GorillaTagger.Instance.rightHandTransform.up / 4f : Vector3.zero)), SwapGunHand ? (legacyGunDirection ? -GorillaTagger.Instance.leftHandTransform.up : GorillaTagger.Instance.leftHandTransform.forward) : (legacyGunDirection ? -GorillaTagger.Instance.rightHandTransform.up : GorillaTagger.Instance.rightHandTransform.forward), out var Ray, 512f, NoInvisLayerMask());
            if (shouldBePC)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
            }

            Vector3 StartPosition = SwapGunHand ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position;
            Vector3 EndPosition = isCopying ? whoCopy.transform.position : Ray.point;
            if (SmoothGunPointer)
            {
                GunPositionSmoothed = Vector3.Lerp(GunPositionSmoothed, EndPosition, Time.deltaTime * 4f);
                EndPosition = GunPositionSmoothed;
            }

            GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            NewPointer.GetComponent<Renderer>().material.color = (isCopying || GetGunInput(true)) ? GetBDColor(0f) : GetBRColor(0f);
            NewPointer.transform.localScale = smallGunPointer ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.2f, 0.2f, 0.2f);
            NewPointer.transform.position = EndPosition;
            if (disableGunPointer)
            {
                NewPointer.GetComponent<Renderer>().enabled = false;
            }
            UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
            UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

            if (!disableGunLine)
            {
                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f);
                liner.endColor = GetBGColor(0.5f);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, StartPosition);
                liner.SetPosition(1, EndPosition);
                UnityEngine.Object.Destroy(line, Time.deltaTime);
            }

            return (Ray, NewPointer);
        }

        public static bool GetGunInput(bool isShooting)
        {
            if (isShooting)
                return (SwapGunHand ? leftTrigger > 0.5f : rightTrigger > 0.5f) || Mouse.current.leftButton.isPressed;
            else
                return (SwapGunHand ? leftGrab : rightGrab) || Mouse.current.rightButton.isPressed;
        }

        public static float nextTimeUntilReload = -1f;
        private static bool hasWarnedVersionBefore = false;
        private static bool hasadminedbefore = false;
        public static System.Collections.IEnumerator LoadServerData(bool reloading)
        {
            try
            {
                WebRequest request = WebRequest.Create("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/iiMenu_ServerData.txt");
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string html = "";
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                shouldAttemptLoadData = false;
                string[] Data = html.Split("\n");

                if (Data[3] != null)
                {
                    serverLink = Data[3];
                }

                if (!hasWarnedVersionBefore)
                {
                    if (Data[0] != PluginInfo.Version)
                    {
                        if (!isBetaTestVersion)
                        {
                            hasWarnedVersionBefore = true;
                            UnityEngine.Debug.Log("Version is outdated");
                            Important.JoinDiscord();
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to " + Data[0] + ".", 10000);
                        }
                        else
                        {
                            hasWarnedVersionBefore = true;
                            UnityEngine.Debug.Log("Version is outdated, but user is on beta");
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>BETA</color><color=grey>]</color> You are using a testing build of the menu. The latest release build is " + Data[0] + ".", 10000);
                        }
                    }
                    else
                    {
                        if (isBetaTestVersion)
                        {
                            hasWarnedVersionBefore = true;
                            UnityEngine.Debug.Log("Version is outdated, user is on early build of latest");
                            Important.JoinDiscord();
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a testing build of the menu. Please update to " + Data[0] + ".", 10000);
                        }
                    }
                }
                if (Data[0] == "lockdown")
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>LOCKDOWN</color><color=grey>]</color> " + Data[2], 10000);
                    bgColorA = Color.red;
                    bgColorB = Color.red;
                    Settings.Panic();
                    lockdown = true;
                }

                admins.Clear();
                string[] Data0 = Data[1].Split(",");
                foreach (string Data1 in Data0)
                {
                    string[] Data2 = Data1.Split(";");
                    admins.Add(Data2[0], Data2[1]);
                }
                try
                {
                    if (admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) && !hasadminedbefore)
                    {
                        hasadminedbefore = true;
                        SetupAdminPanel(admins[PhotonNetwork.LocalPlayer.UserId]);
                    }
                } catch { }

                motdTemplate = Data[2];
            }
            catch { }
            yield return null;
        }

        public static void SetupAdminPanel(string playername)
        {
            List<ButtonInfo> lolbuttons = Buttons.buttons[0].ToList<ButtonInfo>();
            lolbuttons.Add(new ButtonInfo { buttonText = "Admin Mods", method = () => Settings.EnableAdmin(), isTogglable = false, toolTip = "Opens the admin mods." });
            Buttons.buttons[0] = lolbuttons.ToArray();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>" + (playername == "goldentrophy" ? "OWNER" : "ADMIN") + "</color><color=grey>]</color> Welcome, " + playername + "! Admin mods have been enabled.", 10000);
        }

        public static string[] InfosToStrings(ButtonInfo[] array)
        {
            List<string> lol = new List<string>();
            foreach (ButtonInfo help in array)
            {
                lol.Add(help.buttonText);
            }
            return lol.ToArray();
        }

        public static ButtonInfo[] StringsToInfos(string[] array)
        {
            List<ButtonInfo> lol = new List<ButtonInfo>();
            foreach (string help in array)
            {
                lol.Add(GetIndex(help));
            }
            return lol.ToArray();
        }

        public static string[] Alphabetize(string[] array)
        {
            if (array.Length <= 1)
                return array;

            string first = array[0];
            string[] others = array.Skip(1).OrderBy(s => s).ToArray();
            return new string[] { first }.Concat(others).ToArray();
        }

        public static string[] AlphabetizeNoSkip(string[] array)
        {
            if (array.Length <= 1)
                return array;

            string first = array[0];
            string[] others = array.OrderBy(s => s).ToArray();
            return others.ToArray();
        }

        public static string GetFileExtension(string fileName)
        {
            return fileName.ToLower().Split(".")[fileName.Split(".").Length - 1];
        }

        public static string RemoveLastDirectory(string directory)
        {
            return directory == "" || directory.LastIndexOf('/') <= 0 ? "" : directory.Substring(0, directory.LastIndexOf('/'));
        }

        public static string RemoveFileExtension(string file)
        {
            int index = 0;
            string output = "";
            string[] split = file.Split(".");
            foreach (string lol in split)
            {
                index++;
                if (index != split.Length)
                {
                    if (index > 1)
                    {
                        output += ".";
                    }
                    output += lol;
                }
            }
            return output;
        }

        public static AudioType GetAudioType(string extension)
        {
            switch (extension.ToLower())
            {
                case "mp3":
                    return AudioType.MPEG;
                case "wav":
                    return AudioType.WAV;
                case "ogg":
                    return AudioType.OGGVORBIS;
                case "aiff":
                    return AudioType.AIFF;
            }
            return AudioType.WAV;
        }

        public static string GetFullPath(Transform transform)
        {
            string path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                if (path == "")
                {
                    path = transform.name;
                }
                else
                {
                    path = transform.name + "/" + path;
                }
            }
            return path;
        }

        public static System.Collections.IEnumerator GrowCoroutine()
        {
            float elapsedTime = 0f;
            Vector3 target = (scaleWithPlayer) ? new Vector3(0.1f, 0.3f, 0.3825f) * GorillaLocomotion.Player.Instance.scale : new Vector3(0.1f, 0.3f, 0.3825f);
            while (elapsedTime < 0.05f)
            {
                menu.transform.localScale = Vector3.Lerp(Vector3.zero, target, elapsedTime / 0.05f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            menu.transform.localScale = target;
        }

        public static System.Collections.IEnumerator ShrinkCoroutine()
        {
            Transform menuu = menu.transform;
            menu = null;

            Vector3 before = menuu.localScale;
            float elapsedTime = 0f;
            while (elapsedTime < 0.05f)
            {
                menuu.localScale = Vector3.Lerp(before, Vector3.zero, elapsedTime / 0.05f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            UnityEngine.Object.Destroy(menuu.gameObject);
        }

        public static System.Collections.IEnumerator ButtonClick(int buttonIndex, string buttonText, Renderer render)
        {
            lastClickedName = "";
            float elapsedTime = 0f;
            while (elapsedTime < 0.1f)
            {
                render.material.color = Color.Lerp(GetBDColor(0f), GetBRColor(0f), elapsedTime / 0.1f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            GradientColorKey[] releasedColors = new GradientColorKey[3];
            releasedColors[0].color = buttonDefaultA;
            releasedColors[0].time = 0f;
            releasedColors[1].color = buttonDefaultB;
            releasedColors[1].time = 0.5f;
            releasedColors[2].color = buttonDefaultA;
            releasedColors[2].time = 1f;

            GradientColorKey[] selectedColors = new GradientColorKey[3];
            selectedColors[0].color = Color.red;
            selectedColors[0].time = 0f;
            selectedColors[1].color = buttonDefaultB;
            selectedColors[1].time = 0.5f;
            selectedColors[2].color = Color.red;
            selectedColors[2].time = 1f;

            ColorChanger colorChanger = render.gameObject.AddComponent<ColorChanger>();
            colorChanger.isRainbow = false;
            colorChanger.isPastelRainbow = false;
            colorChanger.isEpileptic = false;
            colorChanger.isMonkeColors = false;
            colorChanger.colors = new Gradient
            {
                colorKeys = releasedColors
            };
            if (joystickMenu && buttonIndex == joystickButtonSelected)
            {
                joystickSelectedButton = buttonText;
                colorChanger.isRainbow = false;
                colorChanger.isMonkeColors = false;
                colorChanger.colors = new Gradient
                {
                    colorKeys = selectedColors
                };
            }
            colorChanger.Start();
        }

        private static float lastRecievedTime = -1f;

        public static GliderHoldable[] archiveholdables = null;
        public static GliderHoldable[] GetGliders()
        {
            if (Time.time > lastRecievedTime)
            {
                archivemonsters = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archiveholdables == null)
            {
                archiveholdables = UnityEngine.Object.FindObjectsOfType<GliderHoldable>();
            }
            return archiveholdables;
        }
        
        public static BuilderPiece[] archivepieces = null;
        public static BuilderPiece[] GetPieces()
        {
            if (Time.time > lastRecievedTime)
            {
                archivepieces = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivepieces == null)
            {
                archivepieces = UnityEngine.Object.FindObjectsOfType<BuilderPiece>(true);
            }
            return archivepieces.ToArray();
        }

        public static Dictionary<string, SnowballThrowable> snowballDict = null;
        public static SnowballThrowable GetProjectile(string provided)
        {
            if (snowballDict == null)
            {
                snowballDict = new Dictionary<string, SnowballThrowable>();

                foreach (SnowballThrowable lol in UnityEngine.Object.FindObjectsOfType<SnowballThrowable>(true))
                {
                    try
                    {
                        if (GetFullPath(lol.transform.parent).ToLower() == "player objects/local vrrig/local gorilla player/holdables")
                        {
                            UnityEngine.Debug.Log("Projectile " + lol.gameObject.name + " logged");
                            snowballDict.Add(lol.gameObject.name, lol);
                        }
                    } catch { }
                }
                if (snowballDict.Count < 18)
                {
                    UnityEngine.Debug.Log("Projectile dictionary unfinished ("+snowballDict.Count+"/18)");
                    snowballDict = null;
                }
            }
            if (snowballDict != null && snowballDict.ContainsKey(provided))
            {
                return snowballDict[provided];
            } else
            {
                UnityEngine.Debug.Log("No key found for " + provided);
                return null;
            }
        }

        public static MonkeyeAI[] archivemonsters = null;
        public static MonkeyeAI[] GetMonsters()
        {
            if (Time.time > lastRecievedTime)
            {
                archivemonsters = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivemonsters == null)
            {
                archivemonsters = UnityEngine.Object.FindObjectsOfType<MonkeyeAI>();
            }
            return archivemonsters;
        }

        public static BalloonHoldable[] archiveballoons = null;
        public static BalloonHoldable[] GetBalloons()
        {
            if (Time.time > lastRecievedTime)
            {
                archiveballoons = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archiveballoons == null)
            {
                archiveballoons = UnityEngine.Object.FindObjectsOfType<BalloonHoldable>();
            }
            return archiveballoons;
        }

        public static TappableBell[] archivebells = null;
        public static TappableBell[] GetBells()
        {
            if (Time.time > lastRecievedTime)
            {
                archivebells = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivebells == null)
            {
                archivebells = UnityEngine.Object.FindObjectsOfType<TappableBell>();
            }
            return archivebells;
        }

        public static WhackAMole[] archivewamoles = null;
        public static WhackAMole[] GetWAMoles()
        {
            if (Time.time > lastRecievedTime)
            {
                archivewamoles = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivewamoles == null)
            {
                archivewamoles = UnityEngine.Object.FindObjectsOfType<WhackAMole>();
            }
            return archivewamoles;
        }

        public static Mole[] archivemoles = null;
        public static Mole[] GetMoles()
        {
            if (Time.time > lastRecievedTime)
            {
                archivemoles = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivemoles == null)
            {
                archivemoles = UnityEngine.Object.FindObjectsOfType<Mole>();
            }
            return archivemoles;
        }

        public static GhostLabButton[] archivelabbuttons = null;
        public static GhostLabButton[] GetLabButtons()
        {
            if (Time.time > lastRecievedTime)
            {
                archivelabbuttons = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivelabbuttons == null)
            {
                archivelabbuttons = UnityEngine.Object.FindObjectsOfType<GhostLabButton>();
            }
            return archivelabbuttons;
        }

        public static GorillaCaveCrystal[] archivecrystals = null;
        public static GorillaCaveCrystal[] GetCrystals() // JESSE
        {
            if (Time.time > lastRecievedTime)
            {
                archivecrystals = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archivecrystals == null)
            {
                archivecrystals = UnityEngine.Object.FindObjectsOfType<GorillaCaveCrystal>();
            }
            return archivecrystals;
        }

        public static GorillaRopeSwing[] archiveropeswing = null;
        public static GorillaRopeSwing[] GetRopes()
        {
            if (Time.time > lastRecievedTime)
            {
                archiveropeswing = null;
                lastRecievedTime = Time.time + 5f;
            }
            if (archiveropeswing == null)
            {
                archiveropeswing = UnityEngine.Object.FindObjectsOfType<GorillaRopeSwing>();
            }
            return archiveropeswing;
        }

        public static TransferrableObject[] archivetransobjs = null;
        public static TransferrableObject[] GetTransferrableObjects()
        {
            if (Time.time > lastRecievedTime)
            {
                archivetransobjs = null;
                lastRecievedTime = Time.time + 30f;
            }
            if (archivetransobjs == null)
            {
                archivetransobjs = UnityEngine.Object.FindObjectsOfType<TransferrableObject>();
            }
            return archivetransobjs;
        }

        public static TappableGuardianIdol[] archivetgi = null;
        public static TappableGuardianIdol[] GetGuardianIdols()
        {
            if (Time.time > lastRecievedTime)
            {
                archivetgi = null;
                lastRecievedTime = Time.time + 30f;
            }
            if (archivetgi == null)
            {
                archivetgi = UnityEngine.Object.FindObjectsOfType<TappableGuardianIdol>();
            }
            return archivetgi;
        }

        public static ForceVolume[] archivefvol = null;
        public static ForceVolume[] GetForceVolumes()
        {
            if (Time.time > lastRecievedTime)
            {
                archivefvol = null;
                lastRecievedTime = Time.time + 30f;
            }
            if (archivefvol == null)
            {
                archivefvol = UnityEngine.Object.FindObjectsOfType<ForceVolume>();
            }
            return archivefvol;
        }

        public static void GetOwnership(PhotonView view)
        {
            if (!view.AmOwner)
            {
                try
                {
                    //view.OwnershipTransfer = OwnershipOption.Request;
                    view.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    view.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    view.RequestOwnership();
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);

                    RequestableOwnershipGuard rog = view.GetComponent<RequestableOwnershipGuard>();
                    if (rog != null)
                    {
                        view.GetComponent<RequestableOwnershipGuard>().actualOwner = PhotonNetwork.LocalPlayer;
                        view.GetComponent<RequestableOwnershipGuard>().currentOwner = PhotonNetwork.LocalPlayer;
                        view.GetComponent<RequestableOwnershipGuard>().RequestTheCurrentOwnerFromAuthority();
                        view.GetComponent<RequestableOwnershipGuard>().TransferOwnership(PhotonNetwork.LocalPlayer);
                        view.GetComponent<RequestableOwnershipGuard>().TransferOwnershipFromToRPC(PhotonNetwork.LocalPlayer, view.GetComponent<RequestableOwnershipGuard>().ownershipRequestNonce, default(PhotonMessageInfo));
                    }
                    RPCProtection();
                } catch { UnityEngine.Debug.Log("Faliure to get ownership, is the PhotonView valid?"); }
            //} else
            //{
                //view.OwnershipTransfer = OwnershipOption.Fixed;
            }
        }

        public static bool PlayerIsTagged(VRRig who)
        {
            string name = who.mainSkin.material.name.ToLower();
            return name.Contains("fected") || name.Contains("it") || name.Contains("stealth") || !who.nameTagAnchor.activeSelf;
            //return PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
        }

        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer> { };
            string gamemode = GorillaGameManager.instance.GameModeName().ToLower();
            if (gamemode.Contains("infection") || gamemode.Contains("tag"))
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (tagman.isCurrentlyTag)
                {
                    infected.Add(tagman.currentIt);
                }
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                    {
                        infected.Add(plr);
                    }
                }
            }
            if (gamemode.Contains("ghost"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla GhostTag Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    infected.Add(tagman.currentIt);
                }
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                    {
                        infected.Add(plr);
                    }
                }
            }
            if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    infected.Add(tagman.currentIt);
                }
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                    {
                        infected.Add(plr);
                    }
                }
            }
            if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    infected.Add(tagman.currentIt);
                }
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                    {
                        infected.Add(plr);
                    }
                }
            }
            return infected;
        }

        public static void AddInfected(NetPlayer plr)
        {
            string gamemode = GorillaGameManager.instance.GameModeName().ToLower();
            if (gamemode.Contains("infection") || gamemode.Contains("tag"))
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (tagman.isCurrentlyTag)
                {
                    tagman.ChangeCurrentIt(plr);
                }
                else
                {
                    if (!tagman.currentInfected.Contains(plr))
                    {
                        tagman.AddInfectedPlayer(plr);
                    }
                }
            }
            if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    tagman.ChangeCurrentIt(plr);
                }
                else
                {
                    if (!tagman.currentInfected.Contains(plr))
                    {
                        tagman.AddInfectedPlayer(plr);
                    }
                }
            }
        }

        public static void RemoveInfected(NetPlayer plr)
        {
            string gamemode = GorillaGameManager.instance.GameModeName().ToLower();
            if (gamemode.Contains("infection") || gamemode.Contains("tag"))
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (tagman.isCurrentlyTag)
                {
                    if (tagman.currentIt == plr)
                    {
                        tagman.currentIt = null;
                    }
                }
                else
                {
                    if (tagman.currentInfected.Contains(plr))
                    {
                        tagman.currentInfected.Remove(plr);
                    }
                }
            }
            if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    if (tagman.currentIt == plr)
                    {
                        tagman.currentIt = null;
                    }
                }
                else
                {
                    if (tagman.currentInfected.Contains(plr))
                    {
                        tagman.currentInfected.Remove(plr);
                    }
                }
            }
        }

        public static Vector3 World2Player(Vector3 world) // SteamVR bug causes teleporting of the player to the center of your playspace
        {
            return world - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position;
        }

        // True left and right hand get the exact position and rotation of the middle of the hand
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
        {
            Quaternion rot = GorillaTagger.Instance.leftHandTransform.rotation * GorillaLocomotion.Player.Instance.leftHandRotOffset;
            return (GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GorillaLocomotion.Player.Instance.leftHandOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
        {
            Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GorillaLocomotion.Player.Instance.rightHandRotOffset;
            return (GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GorillaLocomotion.Player.Instance.rightHandOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static void WorldScale(GameObject obj, Vector3 targetWorldScale)
        {
            Vector3 parentScale = obj.transform.parent.lossyScale;
            obj.transform.localScale = new Vector3(targetWorldScale.x / parentScale.x, targetWorldScale.y / parentScale.y, targetWorldScale.z / parentScale.z);
        }

        public static void FixStickyColliders(GameObject platform) // Object must be at true hand position
        {
            Vector3[] localPositions = new Vector3[]
            {
                new Vector3(0, 1f, 0),
                new Vector3(0, -1f, 0),
                new Vector3(1f, 0, 0),
                new Vector3(-1f, 0, 0),
                new Vector3(0, 0, 1f),
                new Vector3(0, 0, -1f)
            };
            Quaternion[] localRotations = new Quaternion[]
            {
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
                try
                {
                    if (platformMode == 5)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
                    }
                    if (platformMode == 6)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 32;
                    }
                    if (platformMode == 7)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 204;
                    }
                    if (platformMode == 8)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 231;
                    }
                    if (platformMode == 9)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 240;
                    }
                    if (platformMode == 10)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 249;
                    }
                    if (platformMode == 11)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 252;
                    }
                } catch { }
                float size = 0.025f;
                side.transform.SetParent(platform.transform);
                side.transform.position = localPositions[i] * (size / 2);
                side.transform.rotation = localRotations[i];
                WorldScale(side, new Vector3(size, size, 0.01f));
                side.GetComponent<Renderer>().enabled = false;
            }
        }

        public static void VisualizeAura(Vector3 position, float range, Color color)
        {
            GameObject what = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(what, Time.deltaTime);
            UnityEngine.Object.Destroy(what.GetComponent<Collider>());
            UnityEngine.Object.Destroy(what.GetComponent<Rigidbody>());
            what.transform.position = position;
            what.transform.localScale = new Vector3(range, range, range);
            Color clr = color;
            clr.a = 0.25f;
            what.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            what.GetComponent<Renderer>().material.color = clr;
        }

        public static void VisualizeCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            GameObject what = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(what, Time.deltaTime);
            UnityEngine.Object.Destroy(what.GetComponent<Collider>());
            UnityEngine.Object.Destroy(what.GetComponent<Rigidbody>());
            what.transform.position = position;
            what.transform.localScale = scale;
            what.transform.rotation = rotation;
            Color clr = color;
            clr.a = 0.25f;
            what.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            what.GetComponent<Renderer>().material.color = clr;
        }

        private static GameObject audiomgr = null;
        public static void Play2DAudio(AudioClip sound, float volume)
        {
            if (audiomgr == null)
            {
                audiomgr = new GameObject("2DAudioMgr");
                AudioSource temp = audiomgr.AddComponent<AudioSource>();
                temp.spatialBlend = 0f;
            }
            AudioSource ausrc = audiomgr.GetComponent<AudioSource>();
            ausrc.volume = volume;
            ausrc.PlayOneShot(sound);
        }

        public static string ToTitleCase(string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        public static void LoadLanguage(string lang)
        {
            UnityEngine.Debug.Log("Loading language from server " + lang);
            WebRequest request = WebRequest.Create("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/iiMenu_Translate_" + lang + ".txt");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = "";
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            UnityEngine.Debug.Log("Data received");

            translations.Clear();

            string[] Data0 = html.Split("\n");
            foreach (string Data1 in Data0)
            {
                try
                {
                    string[] Data2 = Data1.Split(";");
                    try
                    {
                        translations.Add(Data2[0], Data2[1]);
                    }
                    catch { }
                    try
                    {
                        translations.Add(Data2[0].ToUpper(), Data2[1].ToUpper());
                    }
                    catch { }
                    try
                    {
                        translations.Add(Data2[0].ToLower(), Data2[1].ToLower());
                    }
                    catch { }
                    try
                    {
                        translations.Add(ToTitleCase(Data2[0]), ToTitleCase(Data2[1]));
                    }
                    catch { }
                } catch { }
            }
            UnityEngine.Debug.Log("Language parsed");
        }

        public static Dictionary<string, string> translateCache = new Dictionary<string, string> { };
        public static string TranslateText(string input)
        {
            if (translateCache.ContainsKey(input))
                return translateCache[input];

            if (translations.ContainsKey(input))
                return translations[input];

            string[] words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (translations.ContainsKey(word))
                {
                    words[i] = translations[word];
                }
                else
                {
                    foreach (KeyValuePair<string, string> kvp in translations)
                    {
                        if (word.Contains(kvp.Key, StringComparison.Ordinal))
                        {
                            words[i] = word.Replace(kvp.Key, kvp.Value);
                            break;
                        }
                    }
                }
            }

            return string.Join(" ", words);
        }

        public static string FormatUnix(int seconds)
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            string timeString = $"{minutes:D2}:{remainingSeconds:D2}";

            return timeString;
        }

        public static string ColorToHex(Color color) 
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

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
                if (AntiOculusReport && data.Code == 50)
                {
                    object[] args = (object[])data.CustomData;
                    if ((string)args[0] == PhotonNetwork.LocalPlayer.UserId)
                    {
                        Mods.Safety.AntiReportFRT(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
                    }
                }

                if (AntiOculusReport && data.Code == 200) // Credits to Gorilla Dev for the idea, fully coded by myself
                {
                    string rpcName = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];
                    if (rpcName == "RPC_PlayHandTap")
                    {
                        object[] args = (object[])((Hashtable)data.CustomData)[(byte)4];
                        if ((int)args[0] == 67)
                        {
                            VRRig target = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
                            if (Vector3.Distance(target.leftHandTransform.position, target.rightHandTransform.position) < 0.1f)
                            {
                                Mods.Safety.AntiReportFRT(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
                            }
                        }
                    }
                }

                if (Fun.keyboardTrackerEnabled && data.Code == 200)
                {
                    string rpcName = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];

                    if (rpcName == "RPC_PlayHandTap")
                    {
                        object[] args = (object[])((Hashtable)data.CustomData)[(byte)4];
                        if ((int)args[0] == 66)
                        {
                            VRRig target = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));

                            Transform keyboardTransform = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)").transform;
                            if (Vector3.Distance(target.transform.position, keyboardTransform.position) < 3f)
                            {
                                string handPath = (bool)args[1]
                                 ? "RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/f_index.01.L/f_index.02.L/f_index.03.L/f_index.03.L_end"
                                 : "RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/f_index.01.R/f_index.02.R/f_index.03.R/f_index.03.R_end";

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
                                {
                                    closestKey = "[" + closestKey + "]";
                                }

                                bool isKeyLogged = false;
                                for (int i = 0; i < Fun.keyLogs.Count; i++)
                                {
                                    object[] keyLog = Fun.keyLogs[i];
                                    if ((VRRig)keyLog[0] == target)
                                    {
                                        isKeyLogged = true;

                                        string currentText = (string)keyLog[1];
                                        if (closestKey.Contains("Delete"))
                                        {
                                            Fun.keyLogs[i][1] = currentText.Substring(0, currentText.Length - 1);
                                        }
                                        else
                                        {
                                            Fun.keyLogs[i][1] = currentText + closestKey;
                                        }

                                        Fun.keyLogs[i][2] = Time.time + 5f;
                                        break;
                                    }
                                }

                                if (!isKeyLogged)
                                {
                                    if (!closestKey.Contains("Delete"))
                                    {
                                        Fun.keyLogs.Add(new object[] { target, closestKey, Time.time + 5f });
                                    }
                                }
                            }
                        }
                    }
                }
            } catch { }
        }

        public static void TeleportPlayer(Vector3 pos) // Prevents your hands from getting stuck on trees
        {
            Patches.TeleportPatch.doTeleport = true;
            Patches.TeleportPatch.telePos = pos;
            closePosition = Vector3.zero;
            if (isSearching && !isPcWhenSearching)
            {
                VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            }
        }

        public static ButtonInfo GetIndex(string buttonText)
        {
            foreach (ButtonInfo[] buttons in Menu.Buttons.buttons)
            {
                foreach (ButtonInfo button in buttons)
                {
                    try
                    {
                        if (button.buttonText == buttonText)
                        {
                            return button;
                        }
                    } catch { }
                }
            }

            return null;
        }

        public static void ReloadMenu()
        {
            if (menu != null)
            {
                UnityEngine.Object.Destroy(menu);
                menu = null;

                Draw();
            }

            if (reference != null)
            {
                UnityEngine.Object.Destroy(reference);
                reference = null;

                CreateReference();
            }
        }

        public static void ChangeName(string PlayerName)
        {
            try
            {
                GorillaComputer.instance.currentName = PlayerName;
                PhotonNetwork.LocalPlayer.NickName = PlayerName;
                GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
                GorillaComputer.instance.savedName = PlayerName;
                PlayerPrefs.SetString("playerName", PlayerName);
                PlayerPrefs.Save();
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(string.Format("iiMenu <b>NAME ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
            }
        }

        public static void ChangeColor(Color color)
        {
            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));

            //GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = color;
            GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
            PlayerPrefs.Save();

            try
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { color.r, color.g, color.b });
                RPCProtection();
            } catch { }
        }

        public static void MakeButtonSound(string buttonText = null, bool overlapHand = false, bool leftOverlap = false)
        {
            bool archiveRightHand = rightHand;
            if (overlapHand)
                rightHand = leftOverlap;
            try
            {
                if (doButtonsVibrate)
                {
                    GorillaTagger.Instance.StartVibration(rightHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                }

                if (buttonClickIndex <= 3 || buttonClickIndex == 11)
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(buttonClickSound, rightHand, buttonClickVolume / 10f);
                    if (PhotonNetwork.InRoom && GetIndex("Serversided Button Sounds").enabled)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] {
                            buttonClickSound,
                            rightHand,
                            buttonClickVolume / 10f
                        });
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
                };
                    try
                    {
                        ButtonInfo button = GetIndex(buttonText);
                        if (button != null)
                        {
                            if (button.isTogglable)
                            {
                                namesToIds[6] = button.enabled ? "leverup" : "leverdown";
                            }
                        }
                    }
                    catch { }

                    AudioSource audioSource = rightHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer;
                    audioSource.volume = buttonClickVolume / 10f;
                    audioSource.PlayOneShot(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/" + namesToIds[buttonClickIndex] + ".ogg", namesToIds[buttonClickIndex] + ".ogg"));
                }
            } catch { }
            rightHand = archiveRightHand;
        }

        public static void PressKeyboardKey(string key)
        {
            if (key == "Space")
            {
                searchText += " ";
            }
            else
            {
                if (key == "Backspace")
                {
                    searchText = searchText.Substring(0, searchText.Length - 1);
                }
                else
                {
                    searchText += key.ToLower();
                }
            }
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, buttonClickVolume / 10f);
            pageNumber = 0;
            ReloadMenu();
        }

        public static int NoInvisLayerMask()
        {
            return ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone | 1 << GorillaTrigger | 1 << GorillaBoundary | 1 << GorillaCosmetics | 1 << GorillaParticle);
        }

        public static void Toggle(string buttonText, bool fromMenu = false)
        {
            if (annoyingMode)
            {
                if (UnityEngine.Random.Range(1, 5) == 2)
                {
                    NotifiLib.SendNotification("<color=red>try again</color>");
                    return;
                }
            }
            int lastPage = ((Buttons.buttons[buttonsType].Length + pageSize - 1) / pageSize) - 1;
            if (buttonsType == 19)
            {
                lastPage = ((favorites.Count + pageSize - 1) / pageSize) - 1;
            }
            if (buttonsType == 24)
            {
                List<string> enabledMods = new List<string>() { "Exit Enabled Mods" };
                foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                {
                    foreach (ButtonInfo v in buttonlist)
                    {
                        if (v.enabled)
                        {
                            enabledMods.Add(v.buttonText);
                        }
                    }
                }
                lastPage = ((enabledMods.Count + pageSize - 1) / pageSize) - 1;
            }
            if (isSearching)
            {
                List<ButtonInfo> searchedMods = new List<ButtonInfo> { };
                Regex notags = new Regex("<.*?>");
                if (nonGlobalSearch && buttonsType != 0)
                {
                    foreach (ButtonInfo v in Buttons.buttons[buttonsType])
                    {
                        try
                        {
                            string buttonTextt = v.buttonText;
                            if (v.overlapText != null)
                            {
                                buttonTextt = v.overlapText;
                            }

                            if (buttonTextt.Replace(" ", "").ToLower().Contains(searchText.Replace(" ", "").ToLower()))
                            {
                                searchedMods.Add(v);
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            try
                            {
                                string buttonTextt = v.buttonText;
                                if (v.overlapText != null)
                                {
                                    buttonTextt = v.overlapText;
                                }

                                if (buttonTextt.Replace(" ", "").ToLower().Contains(searchText.Replace(" ", "").ToLower()))
                                {
                                    searchedMods.Add(v);
                                }
                            }
                            catch { }
                        }
                    }
                }
                lastPage = (int)Mathf.Ceil(searchedMods.ToArray().Length / (pageSize - 1));
            }
            if (buttonText == "PreviousPage")
            {
                if (dynamicAnimations)
                    lastClickedName = "PreviousPage";
                pageNumber--;
                if (pageNumber < 0)
                {
                    pageNumber = lastPage;
                }
            }
            else
            {
                if (buttonText == "NextPage")
                {
                    if (dynamicAnimations)
                        lastClickedName = "NextPage";
                    pageNumber++;
                    if (pageNumber > lastPage)
                    {
                        pageNumber = 0;
                    }
                }
                else
                {
                    ButtonInfo target = GetIndex(buttonText);
                    if (target != null)
                    {
                        if (fromMenu && ((leftGrab && !joystickMenu) || (joystickMenu && SteamVR_Actions.gorillaTag_RightJoystick2DAxis.axis.y > 0.5f)) && target.buttonText != "Exit Favorite Mods")
                        {
                            if (favorites.Contains(target.buttonText))
                            {
                                favorites.Remove(target.buttonText);
                                NotifiLib.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Removed from favorites.");
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48, rightHand, 0.4f);
                            } else
                            {
                                favorites.Add(target.buttonText);
                                NotifiLib.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Added to favorites.");
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, rightHand, 0.4f);
                            }
                        }
                        else
                        {
                            if (fromMenu && (leftTrigger > 0.5f) && !joystickMenu)
                            {
                                if (hotkeyButton != target.buttonText)
                                {
                                    hotkeyButton = target.buttonText;
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>HOTKEY</color><color=grey>]</color> Set hotkey button.");
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, rightHand, 0.4f);
                                } else
                                {
                                    hotkeyButton = "none";
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48, rightHand, 0.4f);
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>HOTKEY</color><color=grey>]</color> Reset hotkey button.");
                                }
                            }
                            else
                            {
                                if (target.isTogglable)
                                {
                                    target.enabled = !target.enabled;
                                    if (target.enabled)
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                                        if (target.enableMethod != null)
                                        {
                                            try { target.enableMethod.Invoke(); } catch (Exception exc) { UnityEngine.Debug.LogError(string.Format("{0} // Error with mod enableMethod {1} at {2}: {3}", PluginInfo.Name, target.buttonText, exc.StackTrace, exc.Message)); }
                                        }
                                    }
                                    else
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
                                        if (target.disableMethod != null)
                                        {
                                            try { target.disableMethod.Invoke(); } catch (Exception exc) { UnityEngine.Debug.LogError(string.Format("{0} // Error with mod disableMethod {1} at {2}: {3}", PluginInfo.Name, target.buttonText, exc.StackTrace, exc.Message)); }
                                        }
                                    }
                                }
                                else
                                {
                                    if (dynamicAnimations)
                                        lastClickedName = target.buttonText;
                                    NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                                    if (target.method != null)
                                    {
                                        try { target.method.Invoke(); } catch (Exception exc) { UnityEngine.Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, target.buttonText, exc.StackTrace, exc.Message)); }
                                    }
                                }
                                try
                                {
                                    if (fromMenu && admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? SteamVR_Actions.gorillaTag_RightJoystickClick.state : false && PhotonNetwork.InRoom && !isOnPC)
                                    {
                                        PhotonNetwork.RaiseEvent(68, new object[] { "forceenable", target.buttonText, target.enabled }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> Force enabled mod for other menu users.");
                                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, rightHand, 0.4f);
                                    }
                                } catch { }
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(buttonText + " does not exist");
                    }
                }
            }
            ReloadMenu();
        }

        public static System.Collections.IEnumerator DelayLoadPreferences()
        {
            yield return new WaitForSeconds(1);
            Settings.LoadPreferences();
        }

        public static void OnLaunch()
        {
            UnityEngine.Debug.Log(ascii);
            UnityEngine.Debug.Log("Thank you for using ii's Stupid Menu!");
            try
            {
                if (!Font.GetOSInstalledFontNames().Contains("Agency FB"))
                {
                    GameObject fart = LoadAsset("agency");
                    agency = fart.transform.Find("text").gameObject.GetComponent<Text>().font;
                    UnityEngine.Object.Destroy(fart);
                }
            } catch { }
            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
            try
            {
                if (!GameObject.Find("elo_snoc_ii")) // Makes sure Admin mods do not activate twice
                {
                    new GameObject("elo_snoc_ii");
                    PhotonNetwork.NetworkingClient.EventReceived += Experimental.Console;
                }
            } catch { PhotonNetwork.NetworkingClient.EventReceived += Experimental.Console; } // it's worth a shot
            shouldLoadDataTime = Time.time + 5f;
            timeMenuStarted = Time.time;
            shouldAttemptLoadData = true;
            if (File.Exists("iisStupidMenu/iiMenu_EnabledMods.txt") || File.Exists("iisStupidMenu/iiMenu_Preferences.txt"))
            {
                try
                {
                    Settings.LoadPreferences();
                } catch
                {
                    CoroutineManager.RunCoroutine(DelayLoadPreferences());
                }
            }
        }

        // The variable warehouse
        public static bool isBetaTestVersion = false;
        public static bool lockdown = false;
        public static bool isOnPC = false;

        public static bool HasLoaded = false;
        public static bool hasLoadedPreferences = false;
        public static bool hasRemovedThisFrame = false;
        public static bool shouldAttemptLoadData = false;
        public static float shouldLoadDataTime = -1f;
        public static int attemptsToLoad = 0;

        public static bool FATMENU = true;
        public static bool longmenu = false;
        public static bool disorganized = false;
        public static bool flipMenu = false;
        public static bool shinymenu = false;
        public static bool dropOnRemove = true;
        public static bool shouldOutline = false;
        public static bool lastclicking = false;
        public static bool openedwithright = false;
        public static bool likebark = false;

        public static int pageSize = 6;
        public static int pageNumber = 0;
        public static bool noPageNumber = false;
        public static bool disablePageButtons = false;
        public static int pageButtonType = 1;

        public static int buttonsType = 0;
        public static int buttonClickSound = 8;
        public static int buttonClickIndex = 0;
        public static int buttonClickVolume = 4;
        public static float buttonOffset = 2;
        public static bool doButtonsVibrate = true;

        public static bool joystickMenu = false;
        public static bool physicalMenu = false;
        public static Vector3 physicalOpenPosition = Vector3.zero;
        public static Quaternion physicalOpenRotation = Quaternion.identity;
        public static bool joystickOpen = false;
        public static int joystickButtonSelected = 0;
        public static string joystickSelectedButton = "";
        public static float joystickDelay = -1f;

        public static bool rightHand = false;
        public static bool isRightHand = false;
        public static bool bothHands = false;
        public static bool wristThing = false;
        public static bool wristThingV2 = false;
        public static bool wristOpen = false;
        public static float wristMenuDelay = -1f;

        public static bool disableNotifications = false;
        public static bool showEnabledModsVR = true;
        public static bool disableDisconnectButton = false;
        public static bool disableFpsCounter = false;
        public static bool disableSearchButton = false;
        public static bool disableReturnButton = false;

        public static bool ghostException = false;
        public static bool disableGhostview = false;
        public static bool legacyGhostview = false;
        public static bool checkMode = false;
        public static bool lastChecker = false;

        public static bool SmoothGunPointer = false;
        public static bool smallGunPointer = false;
        public static bool disableGunPointer = false;
        public static bool disableGunLine = false;
        public static bool legacyGunDirection = false;
        public static bool SwapGunHand = false;

        public static int fontCycle = 0;
        public static int fontStyleType = 2;
        public static bool NoAutoSizeText = false;

        public static bool doCustomName = false;
        public static string customMenuName = "your text here";
        public static bool doCustomMenuBackground = false;
        public static bool disableBoardColor = false;
        public static bool disableBoardTextColor = false;
        public static int pcbg = 0;

        public static bool isSearching = false;
        public static bool nonGlobalSearch = false;
        public static bool isPcWhenSearching = false;
        public static string searchText = "";
        public static float lastBackspaceTime = 0f;

        public static int fullModAmount = -1;
        public static int amountPartying = 0;
        public static bool waitForPlayerJoin = false;
        public static bool riskyModsEnabled = false;
        public static bool scaleWithPlayer = false;

        public static bool dynamicSounds = false;
        public static bool dynamicAnimations = false;
        public static bool dynamicGradients = false;
        public static string lastClickedName = "";

        public static string ascii = 
@"  _ _ _       ____  _               _     _   __  __                  
 (_|_| )___  / ___|| |_ _   _ _ __ (_) __| | |  \/  | ___ _ __  _   _ 
 | | |// __| \___ \| __| | | | '_ \| |/ _` | | |\/| |/ _ \ '_ \| | | |
 | | | \__ \  ___) | |_| |_| | |_) | | (_| | | |  | |  __/ | | | |_| |
 |_|_| |___/ |____/ \__|\__,_| .__/|_|\__,_| |_|  |_|\___|_| |_|\__,_|
                             |_|                                     
";

        public static string motdTemplate = "You are using build {0}. This menu was created by iiDk (@goldentrophy) on discord. " +
        "This menu is completely free and open sourced, if you paid for this menu you have been scammed. " +
        "There are a total of <b>{1}</b> mods on this menu. " +
        "<color=red>I, iiDk, am not responsible for any bans using this menu.</color> " +
        "If you get banned while using this, it's your responsibility.";

        public static bool shouldBePC = false;
        public static bool leftPrimary = false;
        public static bool leftSecondary = false;
        public static bool rightPrimary = false;
        public static bool rightSecondary = false;
        public static bool leftGrab = false;
        public static bool rightGrab = false;
        public static float leftTrigger = 0f;
        public static float rightTrigger = 0f;

        public static List<KeyCode> lastPressedKeys = new List<KeyCode>();
        public static KeyCode[] allowedKeys = {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
            KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
            KeyCode.Z, KeyCode.Space, KeyCode.Backspace, KeyCode.Escape // it doesn't fit :(
        };

        public static Dictionary<string, string> admins = new Dictionary<string, string> { { "47F316437B9BE495", "goldentrophy" } };

        public static string hotkeyButton = "none";

        public static int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static int Zone = LayerMask.NameToLayer("Zone");
        public static int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
        public static int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

        public static Camera TPC = null;
        public static GameObject menu = null;
        public static GameObject menuBackground = null;
        public static GameObject reference = null;
        public static SphereCollider buttonCollider = null;
        public static GameObject canvasObj = null;
        public static AssetBundle assetBundle = null;
        public static Text fpsCount = null;
        public static Text searchTextObject = null;
        public static Text title = null;
        public static VRRig whoCopy = null;
        public static VRRig GhostRig = null;
        public static Material funnyghostmaterial = null;
        public static Material searchMat = null;
        public static Material returnMat = null;
        public static Material fixMat = null;

        public static Font agency = Font.CreateDynamicFontFromOSFont("Agency FB", 24);
        public static Font Arial = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        public static Font Verdana = Font.CreateDynamicFontFromOSFont("Verdana", 24);
        public static Font sans = Font.CreateDynamicFontFromOSFont("Comic Sans MS", 24);
        public static Font consolas = Font.CreateDynamicFontFromOSFont("Consolas", 24);
        public static Font ubuntu = Font.CreateDynamicFontFromOSFont("Candara", 24);
        public static Font MSGOTHIC = Font.CreateDynamicFontFromOSFont("MS Gothic", 24);
        public static Font impact = Font.CreateDynamicFontFromOSFont("Impact", 24);
        public static Font gtagfont = null;
        public static Font activeFont = agency;
        public static FontStyle activeFontStyle = FontStyle.Italic;

        public static GameObject lKeyReference = null;
        public static SphereCollider lKeyCollider = null;

        public static GameObject rKeyReference = null;
        public static SphereCollider rKeyCollider = null;

        public static GameObject VRKeyboard = null;
        public static GameObject menuSpawnPosition = null;

        public static GameObject watchobject = null;
        public static GameObject watchText = null;
        public static GameObject watchShell = null;
        public static GameObject watchEnabledIndicator = null;
        public static Material watchIndicatorMat = null;
        public static int currentSelectedModThing = 0;

        public static GameObject regwatchobject = null;
        public static GameObject regwatchText = null;
        public static GameObject regwatchShell = null;

        public static Material OrangeUI = new Material(Shader.Find("GorillaTag/UberShader"));
        public static GameObject motd = null;
        public static GameObject motdText = null;
        public static Material glass = null;

        public static Material cannmat = null;
        public static Texture2D cann = null;
        public static Texture2D pride = null;
        public static Texture2D trans = null;
        public static Texture2D gay = null;
        public static Texture2D searchIcon = null;
        public static Texture2D returnIcon = null;
        public static Texture2D fixTexture = null;
        public static Texture2D customMenuBackgroundImage = null;

        public static Material crownmat = null;
        public static Texture2D admincrown = null;

        public static List<string> favorites = new List<string> { "Exit Favorite Mods" };

        public static List<GorillaNetworkJoinTrigger> triggers = new List<GorillaNetworkJoinTrigger> { };
        public static List<TMPro.TextMeshPro> udTMP = new List<TMPro.TextMeshPro> { };

        public static Material[] ogScreenMats = new Material[] { };

        public static Dictionary<string, string> translations = new Dictionary<string, string> { };
        public static bool translate = false;

        public static string serverLink = "https://discord.gg/iidk";

        public static string[] letters = new string[]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M"
        };

        public static int[] bones = new int[] {
            4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7
        };

        public static int arrowType = 0;
        public static string[][] arrowTypes = new string[][] // http://xahlee.info/comp/unicode_index.html
        {
            new string[] {"<", ">"},
            new string[] {"←", "→"},
            new string[] {"↞", "↠"},
            new string[] {"◄", "►"},
            new string[] {"〈 ", " 〉"},
            new string[] {"‹", "›"},
            new string[] {"«", "»"},
            new string[] {"◀", "▶"},
            new string[] {"-", "+"},
            new string[] {"", ""},
        };

        public static Dictionary<char, char> superscript = new Dictionary<char, char>()
        {
            { 'a', 'ᵃ' }, { 'b', 'ᵇ' }, { 'c', 'ᶜ' }, { 'd', 'ᵈ' },
            { 'e', 'ᵉ' }, { 'f', 'ᶠ' }, { 'g', 'ᵍ' }, { 'h', 'ʰ' },
            { 'i', 'ᶤ' }, { 'j', 'ʲ' }, { 'k', 'ᵏ' }, { 'l', 'ˡ' },
            { 'm', 'ᵐ' }, { 'n', 'ᶮ' }, { 'o', 'ᵒ' }, { 'p', 'ᵖ' },
            { 'q', 'ᵝ' }, { 'r', 'ʳ' }, { 's', 'ˢ' }, { 't', 'ᵗ' },
            { 'u', 'ᵘ' }, { 'v', 'ᵥ' }, { 'w', 'ʷ' }, { 'x', 'ˣ' },
            { 'y', 'ʸ' }, { 'z', 'ᶻ' }
        };

        public static string[] ExternalProjectileNames = new string[]
        {
            "SnowballLeft",
            "WaterBalloonLeft",
            "LavaRockLeft",
            "BucketGiftFunctional",
            "ScienceCandyLeft",
            "FishFoodLeft",
            "TrickTreatFunctionalAnchor",
            "VotingRockAnchor_LEFT",
            "TrickTreatFunctionalAnchor",
            "TrickTreatFunctionalAnchor",
            "AppleLeftAnchor"
        };
        public static string[] InternalProjectileNames = new string[]
        {
            "LMACE. LEFT.",
            "LMAEX. LEFT.",
            "LMAGD. LEFT.",
            "LMAHQ. LEFT.",
            "LMAIE. RIGHT.",
            "LMAIO. LEFT.",
            "LMAMN. LEFT.",
            "LMAMS. LEFT.",
            "LMAMN. LEFT.",
            "LMAMN. LEFT.",
            "LMAMU. LEFT."
        };

        public static int themeType = 1;
        public static Color bgColorA = new Color32(255, 128, 0, 128);
        public static Color bgColorB = new Color32(255, 102, 0, 128);

        public static Color buttonDefaultA = new Color32(170, 85, 0, 255);
        public static Color buttonDefaultB = new Color32(170, 85, 0, 255);

        public static Color buttonClickedA = new Color32(85, 42, 0, 255);
        public static Color buttonClickedB = new Color32(85, 42, 0, 255);

        public static Color textColor = new Color32(255, 190, 125, 255);
        public static Color titleColor = new Color32(255, 190, 125, 255);
        public static Color textClicked = new Color32(255, 190, 125, 255);
        public static Color colorChange = Color.black;

        public static Vector3 walkPos;
        public static Vector3 walkNormal;

        public static Vector3 closePosition;

        public static Vector3 pointerOffset = new Vector3(0f, -0.1f, 0f);
        public static int pointerIndex = 0;

        public static float tagAuraDistance = 1.666f;
        public static int tagAuraIndex = 1;

        public static bool lastSlingThing = false;
        public static bool noclip = false;

        public static bool isCopying = false;

        public static bool lastInRoom = false;
        public static bool lastMasterClient = false;
        public static string lastRoom = "";

        public static int platformMode = 0;
        public static int platformShape = 0;

        public static bool customSoundOnJoin = false;
        public static float partDelay = 0f;

        public static string rejRoom = null;
        public static float rejDebounce = 0f;

        public static string partyLastCode = null;
        public static float partyTime = 0f;
        public static bool phaseTwo = false;

        public static bool adminIsScaling = false;
        public static VRRig adminRigTarget = null;
        public static float adminScale = 1f;
        public static Player adminConeExclusion = null;

        public static float timeMenuStarted = -1f;
        public static float delaythinggg = 0f;
        public static float debounce = 0f;
        public static float kgDebounce = 0f;
        public static float nameCycleDelay = 0f;
        public static float stealIdentityDelay = 0f;
        public static float beesDelay = 0f;
        public static float laggyRigDelay = 0f;
        public static float jrDebounce = 0f;
        public static float projDebounce = 0f;
        public static float projDebounceType = 0.1f;
        public static float soundDebounce = 0f;
        public static float buttonCooldown = 0f;
        public static float colorChangerDelay = 0f;
        public static float teleDebounce = 0f;
        public static float splashDel = 0f;
        public static float headspazDelay = 0f;
        public static float internetTime = 5f;
        public static float autoSaveDelay = Time.time + 60f;

        public static int projmode = 0;
        public static int trailmode = 0;

        public static int notificationDecayTime = 1000;

        public static float oldSlide = 0f;

        public static int accessoryType = 0;
        public static int hat = 0;

        public static int soundId = 0;

        public static float red = 1f;
        public static float green = 0.5f;
        public static float blue = 0f;

        public static bool lastOwner = false;
        public static string inputText = "";
        public static string lastCommand = "";

        public static int shootCycle = 1;
        public static float ShootStrength = 19.44f;

        public static int flySpeedCycle = 1;
        public static float flySpeed = 10f;

        public static int speedboostCycle = 1;
        public static float jspeed = 7.5f;
        public static float jmulti = 1.25f;

        public static int longarmCycle = 2;
        public static float armlength = 1.25f;

        public static int nameCycleIndex = 0;

        public static bool lastprimaryhit = false;
        public static bool idiotfixthingy = false;

        public static int colorChangeType = 0;
        public static bool strobeColor = false;

        public static bool AntiCrashToggle = false;
        public static bool AntiSoundToggle = false;
        public static bool AntiCheatSelf = false;
        public static bool AntiCheatAll = false;
        public static bool AntiACReport = false;
        public static bool AntiOculusReport = false;
        public static bool NoGliderRespawn = false;

        public static bool lastHit = false;
        public static bool lastHit2 = false;
        public static bool lastRG;

        public static int tindex = 1;

        public static bool lastHitL = false;
        public static bool lastHitR = false;
        public static bool lastHitLP = false;
        public static bool lastHitRP = false;
        public static bool lastHitRS = false;

        public static bool plastLeftGrip = false;
        public static bool plastRightGrip = false;

        public static bool EverythingSlippery = false;
        public static bool EverythingGrippy = false;

        public static bool headspazType = false;

        public static bool hasFoundAllBoards = false;

        public static float lastBangTime = 0f;

        public static float subThingy = 0f;
        public static float subThingyZ = 0f;

        public static float sizeScale = 1f;

        public static float turnAmnt = 0f;
        public static float TagAuraDelay = 0f;
        public static float startX = -1f;
        public static float startY = -1f;

        public static bool lowercaseMode = false;
        public static string inputTextColor = "green";
        
        public static bool annoyingMode = false; // Build with this enabled for a surprise
        public static string[] facts = new string[] {
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
            "Penguins give each other pebbles as a way of proposing."
        };
    }
}