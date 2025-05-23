using BepInEx;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Mods.Spammers;
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
using System.Security.Cryptography;
using System.Text;
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

penguinpro_24 iiMenu.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        public static void Prefix()
        {
            try
            {
                bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q) || (isSearching && isPcWhenSearching);
                bool buttonCondition = ControllerInputPoller.instance.leftControllerSecondaryButton;
                if (rightHand)
                    buttonCondition = ControllerInputPoller.instance.rightControllerSecondaryButton;

                if (likebark)
                    buttonCondition = rightHand ? ControllerInputPoller.instance.leftControllerSecondaryButton : ControllerInputPoller.instance.rightControllerSecondaryButton;
                
                if (bothHands)
                {
                    buttonCondition = ControllerInputPoller.instance.leftControllerSecondaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton;
                    if (buttonCondition)
                        openedwithright = ControllerInputPoller.instance.rightControllerSecondaryButton;
                }
                if (wristThing)
                {
                    bool shouldOpen = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position - (GorillaTagger.Instance.leftHandTransform.forward * 0.1f), TrueRightHand().position) < 0.1f;
                    if (rightHand)
                        shouldOpen = Vector3.Distance(TrueLeftHand().position, GorillaTagger.Instance.rightHandTransform.position - (GorillaTagger.Instance.rightHandTransform.forward * 0.1f)) < 0.1f;
                    
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
                buttonCondition = buttonCondition || isKeyboardCondition;
                buttonCondition = buttonCondition && !Lockdown;
                buttonCondition = buttonCondition || isSearching;

                if (wristThingV2)
                    buttonCondition = isKeyboardCondition;

                isMenuButtonHeld = buttonCondition;
                if (buttonCondition && menu == null)
                {
                    if (dynamicSounds)
                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/open.wav", "open.wav"), buttonClickVolume / 10f);

                    Draw();

                    if (dynamicAnimations)
                        CoroutineManager.RunCoroutine(GrowCoroutine());

                    if (!joystickMenu)
                    {
                        if (reference == null)
                            CreateReference();
                    }
                }
                else
                {
                    if (!buttonCondition && menu != null)
                    {
                        GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(true);

                        if (dynamicSounds)
                            Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/close.wav", "close.wav"), buttonClickVolume / 10f);
                        
                        try
                        {
                            if (isOnPC && TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet"))
                            {
                                isOnPC = false;
                                TPC.transform.position = TPC.transform.parent.position;
                                TPC.transform.rotation = TPC.transform.parent.rotation;
                            }
                        } catch { }

                        if (!dynamicAnimations)
                        {
                            if (dropOnRemove)
                            {
                                try
                                {
                                    Rigidbody comp = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;

                                    if (GetIndex("Zero Gravity Menu").enabled)
                                        comp.useGravity = false;

                                    if (rightHand || (bothHands && openedwithright))
                                    {
                                        comp.velocity = GorillaLocomotion.GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                                        comp.angularVelocity = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetOrAddComponent<GorillaVelocityEstimator>().angularVelocity;
                                    }
                                    else
                                    {
                                        comp.velocity = GorillaLocomotion.GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                                        comp.angularVelocity = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetOrAddComponent<GorillaVelocityEstimator>().angularVelocity;
                                    }

                                    if (annoyingMode)
                                    {
                                        comp.velocity = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                                        comp.angularVelocity = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                                    }
                                }
                                catch { }

                                if (menuTrail)
                                {
                                    try
                                    {
                                        TrailRenderer trail = menu.AddComponent<TrailRenderer>();

                                        trail.startColor = bgColorA;
                                        trail.endColor = bgColorB;
                                        trail.startWidth = 0.025f;
                                        trail.endWidth = 0f;
                                        trail.minVertexDistance = 0.05f;

                                        trail.material.shader = Shader.Find("Sprites/Default");
                                        trail.time = 2f;
                                    } catch { }
                                }

                                Destroy(menu, 5f);
                                menu = null;
                                Destroy(reference);
                                reference = null;
                            }
                            else
                            {
                                Destroy(menu);
                                menu = null;
                                Destroy(reference);
                                reference = null;
                            }
                        } else
                        {
                            CoroutineManager.RunCoroutine(ShrinkCoroutine());
                            Destroy(reference);
                            reference = null;
                        }
                    }
                }
                if (buttonCondition && menu != null)
                    RecenterMenu();

                {
                    hasRemovedThisFrame = false;

                    if (!hasFoundAllBoards)
                    {
                        try
                        {
                            //LogManager.Log("Looking for boards");
                            bool found = false;
                            int indexOfThatThing = 0;
                            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
                            {
                                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                                if (v.name.Contains(StumpLeaderboardID))
                                {
                                    indexOfThatThing++;
                                    if (indexOfThatThing == StumpLeaderboardIndex)
                                    {
                                        found = true;
                                        if (StumpMat == null)
                                            StumpMat = v.GetComponent<Renderer>().material;

                                        v.GetComponent<Renderer>().material = OrangeUI;
                                    }
                                }
                            }

                            bool found2 = false;
                            indexOfThatThing = 0;
                            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
                            {
                                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                                if (v.name.Contains(ForestLeaderboardID))
                                {
                                    indexOfThatThing++;
                                    if (indexOfThatThing == ForestLeaderboardIndex)
                                    {
                                        found2 = true;
                                        if (ForestMat == null)
                                            ForestMat = v.GetComponent<Renderer>().material;

                                        v.GetComponent<Renderer>().material = OrangeUI;
                                    }
                                }
                            }
                            if (found && found2)
                            {
                                GameObject vr = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomBoundaryStones/BoundaryStoneSet_Forest/wallmonitorforestbg");
                                if (vr != null)
                                    vr.GetComponent<Renderer>().material = OrangeUI;

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
                                            udTMP.Add(text);
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
                                            udTMP.Add(text);
                                    }
                                    else
                                        LogManager.Log("Could not find " + lol);
                                }

                                Transform targettransform = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard").transform;
                                for (int i = 0; i < targettransform.transform.childCount; i++)
                                {
                                    GameObject v = targettransform.GetChild(i).gameObject;
                                    if ((v.name.Contains("Board Text") || v.name.Contains("Scoreboard_OfflineText")) && v.activeSelf)
                                    {
                                        TextMeshPro text = v.GetComponent<TextMeshPro>();
                                        if (!udTMP.Contains(text))
                                            udTMP.Add(text);
                                    }
                                }

                                hasFoundAllBoards = true;
                                LogManager.Log("Found all boards");
                            }
                        }
                        catch (Exception exception)
                        {
                            LogManager.LogError(string.Format("iiMenu <b>COLOR ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
                            hasFoundAllBoards = false;
                        }
                    }

                    try
                    {
                        GameObject computerMonitor = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
                        if (computerMonitor != null)
                            computerMonitor.GetComponent<Renderer>().material = OrangeUI;
                    } catch { }

                    try
                    {
                        if (!disableBoardColor)
                            OrangeUI.color = GetBGColor(0f);
                        else
                            OrangeUI.color = new Color32(0, 59, 4, 255);

                        if (motd == null)
                        {
                            GameObject motdThing = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motd (1)");
                            motd = UnityEngine.Object.Instantiate(motdThing, motdThing.transform.parent);
                            motdThing.SetActive(false);
                        }

                        TextMeshPro motdTC = motd.GetComponent<TextMeshPro>();
                        if (!udTMP.Contains(motdTC))
                            udTMP.Add(motdTC);

                        motdTC.richText = true;
                        motdTC.fontSize = 70;
                        motdTC.text = "Thanks for using ii's <b>Stupid</b> Menu!";

                        if (doCustomName)
                            motdTC.text = "Thanks for using " + customMenuName + "!";
                        
                        if (translate)
                            motdTC.text = TranslateText(motdTC.text);
                        
                        if (lowercaseMode)
                            motdTC.text = motdTC.text.ToLower();
                        
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
                            udTMP.Add(motdTextB);

                        motdTextB.richText = true;
                        motdTextB.fontSize = 100;
                        motdTextB.color = titleColor;

                        if (fullModAmount < 0)
                        {
                            fullModAmount = 0;
                            foreach (ButtonInfo[] buttons in Buttons.buttons)
                                fullModAmount += buttons.Length;
                        }

                        motdTextB.text = string.Format(motdTemplate, PluginInfo.Version, fullModAmount);

                        if (translate)
                            motdTextB.text = TranslateText(motdTextB.text);
                        
                        if (lowercaseMode)
                            motdTextB.text = motdTextB.text.ToLower();
                    } catch { }

                    try
                    {
                        Color targetColor = titleColor;

                        if (disableBoardColor || disableBoardTextColor)
                            targetColor = Color.white;

                        foreach (TextMeshPro txt in udTMP)
                            txt.color = targetColor;
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
                                    keysPressed.Add(keyCode);
                                
                                if (!lastPressedKeys.Contains(keyCode))
                                {
                                    if (keyCode == KeyCode.Space)
                                        searchText += " ";
                                    
                                    else
                                    {
                                        if (keyCode == KeyCode.Backspace)
                                        {
                                            if (Time.time > lastBackspaceTime)
                                            {
                                                if (searchText.Length > 0)
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
                            lastRoom = PhotonNetwork.CurrentRoom.Name;

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

                    // Load preferences if failed
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
                                if (!Directory.Exists("iisStupidMenu"))
                                    Directory.CreateDirectory("iisStupidMenu");

                                if (!Directory.Exists("iisStupidMenu/Backups"))
                                    Directory.CreateDirectory("iisStupidMenu/Backups");

                                File.WriteAllText("iisStupidMenu/Backups/" + ISO8601().Replace(":", ".") + ".txt", Settings.SavePreferencesToText());
                            }
                        }
                    }
                    catch { }

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
                                    Destroy(reference);
                                    reference = null;
                                }
                            }
                        } catch { }
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
                                    AudioSource audioSource = SwapGunHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer;
                                    audioSource.volume = buttonClickVolume / 10f;
                                    audioSource.PlayOneShot(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/grip-press.wav", "grip-press.wav"));
                                }

                                if (GetGunInput(true) && (!lastGunTrigger || (audiomgrhand != null && !audiomgrhand.GetComponent<AudioSource>().isPlaying)))
                                {
                                    AudioSource audioSource = SwapGunHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer;
                                    audioSource.volume = buttonClickVolume / 10f;
                                    audioSource.PlayOneShot(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/trigger-press.wav", "trigger-press.wav"));

                                    PlayHandAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/trigger-hold.wav", "trigger-hold.wav"), buttonClickVolume / 10f, SwapGunHand);
                                }

                                if (!GetGunInput(true) && lastGunTrigger)
                                {
                                    AudioSource audioSource = SwapGunHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer;
                                    audioSource.volume = buttonClickVolume / 10f;
                                    audioSource.PlayOneShot(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/trigger-release.wav", "trigger-release.wav"));

                                    if (audiomgrhand != null)
                                        audiomgrhand.GetComponent<AudioSource>().Stop();
                                }
                            } else
                            {
                                if (lastGunSpawned)
                                {
                                    AudioSource audioSource = SwapGunHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer;
                                    audioSource.volume = buttonClickVolume / 10f;
                                    audioSource.PlayOneShot(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/grip-release.wav", "grip-release.wav"));
                                }

                                if (audiomgrhand != null && audiomgrhand.GetComponent<AudioSource>().isPlaying)
                                {
                                    audiomgrhand.GetComponent<AudioSource>().Stop();

                                    AudioSource audioSource = SwapGunHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer;
                                    audioSource.volume = buttonClickVolume / 10f;
                                    audioSource.PlayOneShot(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/trigger-release.wav", "trigger-release.wav"));
                                }
                            }

                            lastGunSpawned = GunSpawned;
                            lastGunTrigger = GetGunInput(true);
                        }
                    } catch { }

                    GunSpawned = false;

                    // Ghostview
                    try
                    {
                        if ((!GorillaTagger.Instance.offlineVRRig.enabled || ghostException) && !disableGhostview)
                        {
                            if (legacyGhostview)
                            {
                                if (GhostRig != null)
                                    Destroy(GhostRig.gameObject);

                                GameObject l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                Destroy(l.GetComponent<SphereCollider>());

                                l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                l.transform.position = TrueLeftHand().position;

                                GameObject r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                Destroy(r.GetComponent<SphereCollider>());

                                r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                r.transform.position = TrueRightHand().position;

                                l.GetComponent<Renderer>().material.color = GetBGColor(0f);
                                r.GetComponent<Renderer>().material.color = GetBGColor(0f);

                                Destroy(l, Time.deltaTime);
                                Destroy(r, Time.deltaTime);
                            }
                            else
                            {
                                if (GhostRig == null)
                                {
                                    GhostRig = UnityEngine.Object.Instantiate<VRRig>(GorillaTagger.Instance.offlineVRRig, GorillaLocomotion.GTPlayer.Instance.transform.position, GorillaLocomotion.GTPlayer.Instance.transform.rotation);
                                    GhostRig.headBodyOffset = Vector3.zero;
                                    GhostRig.enabled = true;

                                    GhostRig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                                    GhostRig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);

                                    Visuals.FixRigMaterialESPColors(GhostRig);
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

                                GhostRig.headConstraint.transform.position = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.position;
                                GhostRig.headConstraint.transform.rotation = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.rotation;

                                GhostRig.leftHandTransform.position = GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position;
                                GhostRig.rightHandTransform.position = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position;

                                GhostRig.leftHandTransform.rotation = GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.rotation;
                                GhostRig.rightHandTransform.rotation = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.rotation;

                                GhostRig.transform.position = GorillaLocomotion.GTPlayer.Instance.transform.position;
                                GhostRig.transform.rotation = GorillaLocomotion.GTPlayer.Instance.transform.rotation;
                            }
                        }
                        else
                        {
                            if (GhostRig != null)
                                Destroy(GhostRig.gameObject);
                        }
                    }
                    catch { }

                    // Admin indicator
                    if (PhotonNetwork.InRoom)
                    {
                        try
                        {
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
                                                    Destroy(crown.GetComponent<Collider>());
                                                    Destroy(crown, Time.deltaTime);
                                                    if (crownmat == null)
                                                    {
                                                        crownmat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                                                        if (admincrown == null)
                                                            admincrown = LoadTextureFromResource("iiMenu.Resources.icon.png");
                                                        
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
                            adminRigTarget.NativeScale = adminScale;
                    } catch { }

                    try
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            foreach (KeyValuePair<string, long> id in annoyingIDs)
                            {
                                if (!annoyedIDs.Contains(id.Key) && DateTime.UtcNow.Ticks > id.Value)
                                {
                                    string randomName = "gorilla";
                                    for (var i = 0; i < 4; i++)
                                        randomName = randomName + UnityEngine.Random.Range(0, 9).ToString();

                                    List<string> playerids = new List<string> { };

                                    foreach (NetPlayer netPlayer in PhotonNetwork.PlayerList)
                                        playerids.Add(netPlayer.UserId);

                                    if (playerids.Count < 10)
                                        playerids.Add(id.Key);
                                    else
                                    {
                                        int index = UnityEngine.Random.Range(1, 8);
                                        playerids.RemoveAt(index);
                                        playerids.Insert(index, id.Key);
                                    }

                                    object[] array = new object[] { NetworkSystem.Instance.RoomStringStripped(), playerids.ToArray(), id.Key, id.Key, randomName, repReason, NetworkSystemConfig.AppVersion };
                                    PhotonNetwork.RaiseEvent(8, array, new RaiseEventOptions
                                    {
                                        TargetActors = new int[] { -1 },
                                        Receivers = ReceiverGroup.All,
                                        Flags = new WebFlags(1)
                                    }, SendOptions.SendReliable);

                                    annoyedIDs.Add(id.Key);
                                }
                            }

                            foreach (string id in annoyedIDs)
                            {
                                if (!annoyingIDs.ContainsKey(id) || DateTime.UtcNow.Ticks < annoyingIDs[id])
                                    annoyedIDs.Remove(id);
                            }

                            foreach (string id in muteIDs)
                            {
                                if (!mutedIDs.Contains(id))
                                {
                                    string randomName = "gorilla";
                                    for (var i = 0; i < 4; i++)
                                        randomName = randomName + UnityEngine.Random.Range(0, 9).ToString();

                                    object[] content = new object[] { 
                                        id,
                                        true,
                                        randomName,
                                        NetworkSystem.Instance.LocalPlayer.NickName,
                                        true,
                                        NetworkSystem.Instance.RoomStringStripped()
                                    };

                                    PhotonNetwork.RaiseEvent(51, content, new RaiseEventOptions
                                    {
                                        TargetActors = new int[] { -1 },
                                        Receivers = ReceiverGroup.All,
                                        Flags = new WebFlags(1)
                                    }, SendOptions.SendReliable);

                                    mutedIDs.Add(id);
                                }
                            }
                        }
                    }
                    catch { }

                    if (!HasLoaded)
                    {
                        HasLoaded = true;
                        OnLaunch();
                    }

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
                            ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftJoystick);
                            ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightJoystick);

                            ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out leftJoystickClick);
                            ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out rightJoystickClick);
                        }

                        if (UnityInput.Current.GetKey(KeyCode.UpArrow) || UnityInput.Current.GetKey(KeyCode.DownArrow) || UnityInput.Current.GetKey(KeyCode.LeftArrow) || UnityInput.Current.GetKey(KeyCode.RightArrow))
                        {
                            Vector2 Direction = new Vector2((UnityInput.Current.GetKey(KeyCode.RightArrow) ? 1f : 0f) + (UnityInput.Current.GetKey(KeyCode.LeftArrow) ? -1f : 0f), (UnityInput.Current.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (UnityInput.Current.GetKey(KeyCode.DownArrow) ? -1f : 0f));
                            if (UnityInput.Current.GetKey(KeyCode.LeftAlt))
                                rightJoystick = Direction;
                            else
                                leftJoystick = Direction;
                        }

                        if (UnityInput.Current.GetKey(KeyCode.End))
                        {
                            if (UnityInput.Current.GetKey(KeyCode.LeftAlt))
                                rightJoystickClick = true;
                            else
                                leftJoystickClick = true;
                        }
                    } catch { }
                    
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
                            Vector2 js = leftJoystick;
                            if (Time.time > joystickDelay)
                            {
                                if (js.x > 0.5f)
                                {
                                    if (dynamicSounds)
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/next.wav", "next.wav"), buttonClickVolume / 10f);
                                    
                                    Toggle("NextPage");
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                                if (js.x < -0.5f)
                                {
                                    if (dynamicSounds)
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/prev.wav", "prev.wav"), buttonClickVolume / 10f);
                                    
                                    Toggle("PreviousPage");
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }

                                if (js.y > 0.5f)
                                {
                                    if (dynamicSounds)
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/open.wav", "open.wav"), buttonClickVolume / 10f);
                                    
                                    joystickButtonSelected--;
                                    if (joystickButtonSelected < 0)
                                        joystickButtonSelected = pageSize - 1;
                                    
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                                if (js.y < -0.5f)
                                {
                                    if (dynamicSounds)
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/close.wav", "close.wav"), buttonClickVolume / 10f);
                                    
                                    joystickButtonSelected++;
                                    if (joystickButtonSelected > pageSize - 1)
                                        joystickButtonSelected = 0;
                                    
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }

                                if (leftJoystickClick)
                                {
                                    if (dynamicSounds)
                                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/select.wav", "select.wav"), buttonClickVolume / 10f);
                                    
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
                                int categoryIndex = 0;
                                foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                                {
                                    foreach (ButtonInfo v in buttonlist)
                                    {
                                        if (v.enabled && (!hideSettings || (hideSettings && !Buttons.categoryNames[categoryIndex].Contains("Settings"))))
                                            enabledMods.Add(v.buttonText);
                                    }
                                    categoryIndex++;
                                }
                                enabledMods = Alphabetize(enabledMods.ToArray()).ToList();
                                toSortOf = StringsToInfos(enabledMods.ToArray());
                            }
                            watchText.GetComponent<Text>().text = toSortOf[currentSelectedModThing].buttonText;
                            if (toSortOf[currentSelectedModThing].overlapText != null)
                                watchText.GetComponent<Text>().text = toSortOf[currentSelectedModThing].overlapText;
                            
                            watchText.GetComponent<Text>().text += "\n<color=grey>[" + (currentSelectedModThing + 1).ToString() + "/" + toSortOf.Length.ToString() + "]\n" + DateTime.Now.ToString("hh:mm tt") + "</color>";
                            watchText.GetComponent<Text>().color = titleColor;

                            if (lowercaseMode)
                                watchText.GetComponent<Text>().text = watchText.GetComponent<Text>().text.ToLower();

                            if (watchIndicatorMat == null)
                                watchIndicatorMat = new Material(Shader.Find("GorillaTag/UberShader"));
                            
                            watchIndicatorMat.color = toSortOf[currentSelectedModThing].enabled ? GetBDColor(0f) : GetBRColor(0f);
                            watchEnabledIndicator.GetComponent<Image>().material = watchIndicatorMat;

                            Vector2 js = rightHand ? rightJoystick : leftJoystick;
                            if (Time.time > wristMenuDelay)
                            {
                                if (js.x > 0.5f || (rightHand ? (js.y < -0.5f) : (js.y > 0.5f)))
                                {
                                    currentSelectedModThing++;
                                    if (currentSelectedModThing > toSortOf.Length - 1)
                                        currentSelectedModThing = 0;
                                    
                                    wristMenuDelay = Time.time + 0.2f;
                                }
                                if (js.x < -0.5f || (rightHand ? (js.y > 0.5f) : (js.y < -0.5f)))
                                {
                                    currentSelectedModThing--;
                                    if (currentSelectedModThing < 0)
                                        currentSelectedModThing = toSortOf.Length - 1;

                                    wristMenuDelay = Time.time + 0.2f;
                                }
                                if (rightHand ? rightJoystickClick : leftJoystickClick)
                                {
                                    int archive = buttonsType;
                                    Toggle(toSortOf[currentSelectedModThing].buttonText, true);
                                    if (buttonsType != archive)
                                        currentSelectedModThing = 0;
                                    
                                    wristMenuDelay = Time.time + 0.2f;
                                }
                            }
                        }
                    } catch { }

                    // Reconnect code
                    if (PhotonNetwork.InRoom)
                    {
                        if (rejRoom != null)
                            rejRoom = null;
                    }
                    else
                    {
                        if (rejRoom != null && Time.time > rejDebounce/* && PhotonNetwork.NetworkingClient.State == ClientState.Disconnected*/)
                        {
                            LogManager.Log("Attempting rejoin");
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
                                LogManager.Log("Attempting rejoin");
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
                                LogManager.Log("Attempting rejoin");
                                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(partyLastCode, JoinType.Solo);
                                partyTime = Time.time + (float)internetTime;
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

                        foreach (KeyValuePair<string, List<string>> Bind in ModBindings)
                        {
                            string BindInput = Bind.Key;
                            List<string> BindedMods = Bind.Value;

                            if (BindedMods.Count > 0)
                            {
                                bool BindValue = Inputs[BindInput];
                                foreach (string ModName in BindedMods)
                                {
                                    ButtonInfo Mod = GetIndex(ModName);
                                    if (Mod != null)
                                    {
                                        Mod.customBind = BindInput;

                                        if (ToggleBindings || !Mod.isTogglable)
                                        {
                                            if (BindValue && !BindStates[BindInput])
                                                Toggle(ModName);
                                        }

                                        if (!ToggleBindings)
                                        {
                                            if ((BindValue && !Mod.enabled) || (!BindValue && Mod.enabled))
                                                Toggle(ModName);
                                        }
                                    }  
                                }

                                BindStates[BindInput] = BindValue;
                            }
                        }
                    } catch { }

                    try
                    {
                        Visuals.ClearLinePool();
                        Visuals.ClearNameTagPool();
                    } catch { }

                    if (Lockdown)
                        return;

                    // Execute plugin updates
                    foreach (KeyValuePair<string, Assembly> Plugin in Settings.LoadedPlugins)
                    {
                        try
                        {
                            if (!Settings.disabledPlugins.Contains(Plugin.Key))
                                PluginUpdate(Plugin.Value);
                        }
                        catch (Exception e) { LogManager.Log("Error with UPDATE plugin " + Plugin.Key + ": " + e.ToString()); }
                    }

                    // Execute mods
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
                                            LogManager.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, v.buttonText, exc.StackTrace, exc.Message));
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
                LogManager.LogError(string.Format("iiMenu <b>FATAL ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
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
                    gameObject.layer = 2;

                if (themeType == 30)
                    gameObject.GetComponent<Renderer>().enabled = false;
                
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;

                if (FATMENU)
                    gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                else
                    gameObject.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                
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
                        gameObject.transform.localPosition = new Vector3(0.56f, 0.399f, 0.28f - offset);
                    else
                        gameObject.transform.localPosition = new Vector3(0.56f, 0.599f, 0.28f - offset);
                }

                gameObject.AddComponent<Classes.Button>().relatedText = method.buttonText;

                if (shouldOutline)
                    OutlineObj(gameObject, !method.enabled);

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

                if (shouldRound)
                    RoundObj(gameObject);
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
                text2.text = method.overlapText;
            
            if (translate)
                text2.text = TranslateText(text2.text, (string output) => ReloadMenu());

            if (method.customBind != null)
            {
                if (text2.text.Contains("</color><color=grey>]</color>"))
                    text2.text = text2.text.Replace("</color><color=grey>]</color>", "/" + method.customBind + "</color><color=grey>]</color>");
                else
                    text2.text += " <color=grey>[</color><color=green>" + method.customBind + "</color><color=grey>]</color>";
            }

            if (inputTextColor != "green")
                text2.text = text2.text.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color="+inputTextColor+">");
            
            if (lowercaseMode)
                text2.text = text2.text.ToLower();
            
            if (favorites.Contains(method.buttonText))
                text2.text += " ✦";
            
            text2.supportRichText = true;
            text2.fontSize = 1;
            text2.color = method.enabled ? textClicked : textColor;
            if (joystickMenu && buttonIndex == joystickButtonSelected && themeType == 30)
                    text2.color = Color.red;

            text2.alignment = TextAnchor.MiddleCenter;
            text2.fontStyle = activeFontStyle;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;
            RectTransform component = text2.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            if (NoAutoSizeText)
                component.sizeDelta = new Vector2(9f, 0.015f);
            
            component.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        private static void AddSearchButton()
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q) && !isPcWhenSearching)
                gameObject.layer = 2;

            if (themeType == 30)
                gameObject.GetComponent<Renderer>().enabled = false;
            
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
                OutlineObj(gameObject, !isSearching);
            
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

            if (shouldRound)
                RoundObj(gameObject);

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
                gameObject.layer = 2;

            if (themeType == 30)
                gameObject.GetComponent<Renderer>().enabled = false;
            
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
                OutlineObj(gameObject, true);

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

            if (shouldRound)
                RoundObj(gameObject);

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

            Destroy(menu.GetComponent<BoxCollider>());
            Destroy(menu.GetComponent<Renderer>());

            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
            if (scaleWithPlayer)
            {
                menu.transform.localScale *= GorillaLocomotion.GTPlayer.Instance.scale;
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
                Destroy(menuBackground.GetComponent<BoxCollider>());

                if (themeType == 30)
                    menuBackground.GetComponent<Renderer>().enabled = false;

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
                    Destroy(outlinepart.GetComponent<BoxCollider>());
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
                    Destroy(outlinepart.GetComponent<BoxCollider>());
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
                    Destroy(outlinepart.GetComponent<BoxCollider>());
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
                    Destroy(outlinepart.GetComponent<BoxCollider>());
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
                    OutlineObj(menuBackground, false);

                if (shouldRound)
                    RoundObj(menuBackground);

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
                                LogManager.Log("gayed the texture");
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
                    catch (Exception exception) { LogManager.LogError(string.Format("iiMenu <b>TEXTURE ERROR</b> {1} - {0}", exception.Message, exception.StackTrace)); }
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
            canvasScaler.dynamicPixelsPerUnit = highQualityText ? 2500f : 1000f;
            if (scaleWithPlayer)
                canvas.transform.localScale *= GorillaLocomotion.GTPlayer.Instance.scale;

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text.font = activeFont;
            text.text = translate ? "ii's Stupid Menu" : "ii's <b>Stupid</b> Menu";

            if (doCustomName)
                text.text = customMenuName;

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
                    "steal.lol",
                    "Unttile menu"
                };

                if (UnityEngine.Random.Range(1, 5) == 2)
                    text.text = randomMenuNames[UnityEngine.Random.Range(0, randomMenuNames.Length - 1)] + " v" + UnityEngine.Random.Range(8, 159);
            }
            if (translate)
                text.text = TranslateText(text.text, (string output) => ReloadMenu());
            
            if (lowercaseMode)
                text.text = text.text.ToLower();
            
            if (!noPageNumber)
                text.text += " <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
            
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
                text.text = TranslateText(text.text, (string output) => ReloadMenu());
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

            if (!disablePageButtons)
                AddPageButtons();

            if (isSearching)
            {
                // Draw the search box
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q) && !isPcWhenSearching)
                    gameObject.layer = 2;

                if (themeType == 30)
                    gameObject.GetComponent<Renderer>().enabled = false;
                
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;

                if (FATMENU)
                    gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                else
                    gameObject.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);

                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - (buttonOffset / 10));

                if (shouldOutline)
                    OutlineObj(gameObject, true);

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

                if (shouldRound)
                    RoundObj(gameObject);

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

                    Destroy(particle.GetComponent<BoxCollider>());
                    Destroy(particle, 2f);

                    particle.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                    particle.GetComponent<Renderer>().material.color = Color.white;
                    if (cannmat == null)
                    {
                        cannmat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                        cannmat.color = Color.white;

                        if (cann == null)
                            cann = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/cannabis.png", "cannabis.png");
                        
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
                GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);

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

            switch (pageButtonType)
            {
                case 1:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, FATMENU ? 0.9f : 1.3f, 0.08f),
                        new Vector3(0.56f, 0f, 0.28f),
                        new Vector3(0.56f, 0f, 0.28f - 0.1f),
                        new Vector3(0.064f, 0f, 0.109f),
                        new Vector3(0.064f, 0f, 0.109f - 0.1f / 2.55f),
                        array
                    );
                    break;

                case 2:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, 0.2f, 0.9f),
                        new Vector3(0.56f, FATMENU ? 0.65f : 0.9f, 0f),
                        new Vector3(0.56f, FATMENU ? -0.65f : -0.9f, 0f),
                        new Vector3(0.064f, FATMENU ? 0.195f : 0.267f, 0f),
                        new Vector3(0.064f, FATMENU ? -0.195f : -0.267f, 0f),
                        array
                    );
                    break;

                case 5:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, 0.3f, 0.05f),
                        new Vector3(0.56f, FATMENU ? 0.299f : 0.499f, 0.355f),
                        new Vector3(0.56f, FATMENU ? -0.299f : -0.499f, 0.355f),
                        new Vector3(0.064f, FATMENU ? 0.09f : 0.15f, 0.135f),
                        new Vector3(0.064f, FATMENU ? -0.09f : -0.15f, 0.135f),
                        array
                    );
                    break;

                case 6:
                    CreatePageButtonPair(
                        "PreviousPage", "NextPage",
                        new Vector3(0.09f, 0.102f, 0.08f),
                        new Vector3(0.56f, FATMENU ? 0.450f : 0.7f, -0.58f),
                        new Vector3(0.56f, FATMENU ? 0.450f : 0.7f, -0.58f) - new Vector3(0f, 0.16f, 0f),
                        new Vector3(0.064f, FATMENU ? 0.35f / 2.6f : 0.54444444444f / 2.6f, -0.58f / 2.7f),
                        new Vector3(0.064f, FATMENU ? 0.35f / 2.6f : 0.54444444444f / 2.6f, -0.58f / 2.7f) - new Vector3(0f, 0.0475f, 0f),
                        array,
                        new Vector2(0.03f, 0.03f)
                    );
                    break;
            }
        }

        private static void CreatePageButtonPair(string prevButtonName, string nextButtonName, Vector3 buttonScale, Vector3 prevButtonPos, Vector3 nextButtonPos, Vector3 prevTextPos, Vector3 nextTextPos, GradientColorKey[] colorKeys, Vector2? textSize = null)
        {
            GameObject prevButton = CreatePageButton(prevButtonName, buttonScale, prevButtonPos, prevTextPos, colorKeys, textSize, 0);

            GameObject nextButton = CreatePageButton(nextButtonName, buttonScale, nextButtonPos, nextTextPos, colorKeys, textSize, 1);

            if (shouldOutline)
            {
                OutlineObj(prevButton, true);
                OutlineObj(nextButton, true);
            }

            if (shouldRound)
            {
                RoundObj(prevButton);
                RoundObj(nextButton);
            }
        }

        private static GameObject CreatePageButton(string buttonName, Vector3 scale, Vector3 position, Vector3 textPosition, GradientColorKey[] colorKeys, Vector2? textSize, int arrowIndex)
        {
            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);

            if (themeType == 30)
                button.GetComponent<Renderer>().enabled = false;

            if (!UnityInput.Current.GetKey(KeyCode.Q) && !(isSearching && isPcWhenSearching))
                button.layer = 2;

            button.GetComponent<BoxCollider>().isTrigger = true;
            button.transform.parent = menu.transform;
            button.transform.rotation = Quaternion.identity;
            button.transform.localScale = scale;
            button.transform.localPosition = position;

            button.AddComponent<Classes.Button>().relatedText = buttonName;
            button.GetComponent<Renderer>().material.color = buttonDefaultA;

            if (lastClickedName != buttonName)
            {
                ColorChanger colorChanger = button.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient { colorKeys = colorKeys };
                colorChanger.Start();
            }
            else
            {
                CoroutineManager.RunCoroutine(ButtonClick(-99, buttonName, button.GetComponent<Renderer>()));
            }

            Text text = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
            text.font = activeFont;
            text.text = arrowTypes[arrowType][arrowIndex];
            text.fontSize = 1;
            text.color = textColor;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.localPosition = Vector3.zero;
            textRect.sizeDelta = textSize ?? new Vector2(0.2f, 0.03f);

            if (NoAutoSizeText)
                textRect.sizeDelta = new Vector2(9f, 0.015f);

            textRect.localPosition = textPosition;
            textRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            return button;
        }

        public static void OutlineObj(GameObject toOut, bool shouldBeEnabled)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (themeType == 30)
                gameObject.GetComponent<Renderer>().enabled = false;
            
            Destroy(gameObject.GetComponent<BoxCollider>());
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

            if (shouldRound)
                RoundObj(gameObject);
        }

        public static void OutlineObjNonMenu(GameObject toOut, bool shouldBeEnabled)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (themeType == 30)
                gameObject.GetComponent<Renderer>().enabled = false;

            Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = toOut.transform.parent;
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
            colorChanger.isPastelRainbow = shouldBeEnabled && themeType == 51;
            colorChanger.isMonkeColors = shouldBeEnabled && themeType == 8;
            colorChanger.isEpileptic = shouldBeEnabled && themeType == 47;

            colorChanger.Start();
        }

        public static void RoundObj(GameObject toRound)
        {
            float Bevel = 0.02f;

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
            
            RoundCornerA.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, (toRound.transform.localScale.y / 2f) - (Bevel * 1.275f), (toRound.transform.localScale.z / 2f) - Bevel);
            RoundCornerA.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerB.GetComponent<Collider>());

            RoundCornerB.transform.parent = menu.transform;
            RoundCornerB.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerB.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + (Bevel * 1.275f), (toRound.transform.localScale.z / 2f) - Bevel);
            RoundCornerB.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerC.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerC.GetComponent<Collider>());

            RoundCornerC.transform.parent = menu.transform;
            RoundCornerC.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerC.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, (toRound.transform.localScale.y / 2f) - (Bevel * 1.275f), -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerC.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerD.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerD.GetComponent<Collider>());

            RoundCornerD.transform.parent = menu.transform;
            RoundCornerD.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerD.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + (Bevel * 1.275f), -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerD.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject[] ToChange = new GameObject[]
            {
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

                TargetChanger.Start();
            }

            ToRoundRenderer.enabled = false;
        }

        private static void LoadAssetBundle()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("iiMenu.Resources.iimenu");
            if (stream != null)
                assetBundle = AssetBundle.LoadFromStream(stream);
            else
                LogManager.LogError("Failed to load assetbundle");
        }

        public static GameObject LoadAsset(string assetName)
        {
            GameObject gameObject = null;
            
            if (assetBundle == null)
                LoadAssetBundle();
            
            gameObject = UnityEngine.Object.Instantiate(assetBundle.LoadAsset<GameObject>(assetName));
            
            return gameObject;
        }

        public static Dictionary<string, AudioClip> audioPool = new Dictionary<string, AudioClip> { };
        public static AudioClip LoadSoundFromResource(string resourcePath)
        {
            AudioClip sound = null;

            if (!audioPool.ContainsKey(resourcePath))
            {
                if (assetBundle == null)
                    LoadAssetBundle();

                sound = assetBundle.LoadAsset<AudioClip>(resourcePath);
                audioPool.Add(resourcePath, sound);
            } else
                sound = audioPool[resourcePath];
            

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
                Directory.CreateDirectory("iisStupidMenu");
            
            if (!File.Exists("iisStupidMenu/" + fileName))
            {
                LogManager.Log("Downloading " + fileName);
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
                LogManager.LogError("Failed to load texture from resource: " + resourcePath);
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
                LogManager.Log("Downloading " + fileName);
                WebClient stream = new WebClient();
                stream.DownloadFile(resourcePath, "iisStupidMenu/" + fileName);
            }

            byte[] bytes = File.ReadAllBytes("iisStupidMenu/" + fileName);
            texture.LoadImage(bytes);

            return texture;
        }

        private static Dictionary<(Color, Color), Texture2D> cacheGradients = new Dictionary<(Color, Color), Texture2D>();

        public static Texture2D GetGradientTexture(Color colorA, Color colorB)
        {
            var key = (colorA, colorB);
            if (cacheGradients.TryGetValue(key, out Texture2D cachedTexture))
                return cachedTexture;

            Texture2D txt2d = new Texture2D(128, 128);
            Color[] pixels = new Color[128 * 128];

            for (int i = 0; i < 128; i++)
            {
                Color rowColor = Color.Lerp(colorA, colorB, i / 128f);
                for (int j = 0; j < 128; j++)
                    pixels[j * 128 + i] = rowColor;
            }

            txt2d.SetPixels(pixels);
            txt2d.Apply();

            cacheGradients.Add(key, txt2d);
            return txt2d;
        }

        public static void RPCProtection()
        {
            try
            {
                if (hasRemovedThisFrame == false)
                {
                    if (NoOverlapRPCs)
                        hasRemovedThisFrame = true;

                    GorillaNot.instance.rpcErrorMax = int.MaxValue;
                    GorillaNot.instance.rpcCallLimit = int.MaxValue;
                    GorillaNot.instance.logErrorMax = int.MaxValue;

                    PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                    PhotonNetwork.QuickResends = int.MaxValue;

                    PhotonNetwork.SendAllOutgoingCommands();
                }
            } catch { LogManager.Log("RPC protection failed, are you in a lobby?"); }
        }

        public static string GetHttp(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = "";

            using (StreamReader sr = new StreamReader(data))
                html = sr.ReadToEnd();

            return html;
        }

        private static List<float> volumeArchive = new List<float> { };
        private static Vector3 GunPositionSmoothed = Vector3.zero;
        public static (RaycastHit Ray, GameObject NewPointer) RenderGun(int overrideLayerMask = -1)
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
                    Up = SwapGunHand ? TrueLeftHand().up : TrueRightHand().up;
                    Right = SwapGunHand ? TrueLeftHand().right : TrueRightHand().right;
                    Direction = SwapGunHand ? TrueLeftHand().forward : TrueRightHand().forward;
                    break;
            }

            Physics.Raycast(StartPosition + (Direction / 4f), Direction, out var Ray, 512f, overrideLayerMask > 0 ? overrideLayerMask : NoInvisLayerMask());
            if (shouldBePC)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
            }

            Vector3 EndPosition = gunLocked ? lockTarget.transform.position : Ray.point;

            if (EndPosition == Vector3.zero)
                EndPosition = StartPosition + (Direction * 512f);

            if (SmoothGunPointer)
            {
                GunPositionSmoothed = Vector3.Lerp(GunPositionSmoothed, EndPosition, Time.deltaTime * 4f);
                EndPosition = GunPositionSmoothed;
            }

            GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            NewPointer.GetComponent<Renderer>().material.color = (gunLocked || GetGunInput(true)) ? GetBDColor(0f) : GetBRColor(0f);
            NewPointer.transform.localScale = smallGunPointer ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.2f, 0.2f, 0.2f);
            NewPointer.transform.position = EndPosition;

            if (disableGunPointer)
                NewPointer.GetComponent<Renderer>().enabled = false;
            
            Destroy(NewPointer.GetComponent<Collider>());
            Destroy(NewPointer, Time.deltaTime);

            if (!disableGunLine)
            {
                GameObject line = new GameObject("iiMenu_GunLine");
                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
                lineRenderer.startColor = GetBGColor(0f);
                lineRenderer.endColor = GetBGColor(0.5f);
                lineRenderer.startWidth = 0.025f;
                lineRenderer.endWidth = 0.025f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;
                lineRenderer.SetPosition(0, StartPosition);
                lineRenderer.SetPosition(1, EndPosition);
                Destroy(line, Time.deltaTime);

                int Step = GunLineQuality;
                switch (gunVariation)
                {
                    case 1: // Lightning
                        if (GetGunInput(true) || gunLocked)
                        {
                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                    lineRenderer.SetPosition(i, Position + (UnityEngine.Random.Range(0f, 1f) > 0.75f ? new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) : Vector3.zero));
                                }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 2: // Wavy
                        if (GetGunInput(true) || gunLocked)
                        {
                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                float value = ((float)i / (float)Step) * 50f;

                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                lineRenderer.SetPosition(i, Position + (Up * Mathf.Sin((Time.time * -10f) + value) * 0.1f));
                            }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 3: // Blocky
                        if (GetGunInput(true) || gunLocked)
                        {
                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                lineRenderer.SetPosition(i, new Vector3(Mathf.Round(Position.x * 25f) / 25f, Mathf.Round(Position.y * 25f) / 25f, Mathf.Round(Position.z * 25f) / 25f));
                            }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 4: // Sinewave
                        Step = GunLineQuality / 2;

                        if (GetGunInput(true) || gunLocked)
                        {
                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                lineRenderer.SetPosition(i, Position + (Up * Mathf.Sin(Time.time * 10f) * (i % 2 == 0 ? 0.1f : -0.1f)));
                            }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 5: // Spring
                        if (GetGunInput(true) || gunLocked)
                        {
                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                float value = ((float)i / (float)Step) * 50f;

                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                lineRenderer.SetPosition(i, Position + (Right * Mathf.Cos((Time.time * -10f) + value) * 0.1f) + (Up * Mathf.Sin((Time.time * -10f) + value) * 0.1f));
                            }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                    case 6: // Bouncy
                        if (GetGunInput(true) || gunLocked)
                        {
                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                float value = ((float)i / (float)Step) * 15f;
                                lineRenderer.SetPosition(i, Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f)) + (Up * Mathf.Abs(Mathf.Sin((Time.time * -10f) + value)) * 0.3f));
                            }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
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

                            GorillaSpeakerLoudness localRecorder = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
                            if (localRecorder != null)
                                audioSize += localRecorder.Loudness * 3f;

                            volumeArchive.Insert(0, volumeArchive.Count == 0 ? 0 : (audioSize - volumeArchive[0] * 0.1f));

                            if (volumeArchive.Count > Step)
                                volumeArchive.Remove(Step);

                            lineRenderer.positionCount = Step;
                            lineRenderer.SetPosition(0, StartPosition);

                            for (int i = 1; i < (Step - 1); i++)
                            {
                                Vector3 Position = Vector3.Lerp(StartPosition, EndPosition, i / (Step - 1f));
                                lineRenderer.SetPosition(i, Position + (Up * (i >= volumeArchive.Count ? 0 : volumeArchive[i]) * (i % 2 == 0 ? 1f : -1f)));
                            }

                            lineRenderer.SetPosition(Step - 1, EndPosition);
                        }
                        break;
                }
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

        public static System.Collections.IEnumerator NarrateText(string text)
        {
            if (Time.time < (timeMenuStarted + 5f))
                yield break;

            string fileName = GetSHA256(text) + (narratorIndex == 0 ? ".wav" : ".mp3");
            string directoryPath = "iisStupidMenu/TTS" + (narratorName == "Default" ? "" : narratorName);

            if (!Directory.Exists("iisStupidMenu"))
                Directory.CreateDirectory("iisStupidMenu");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!File.Exists("iisStupidMenu/TTS" + (narratorName == "Default" ? "" : narratorName) + "/" + fileName))
            {
                string filePath = directoryPath + "/" + fileName;

                if (!File.Exists(filePath))
                {
                    string postData = "{\"text\": \"" + text.Replace("\n", "").Replace("\r", "").Replace("\"", "") + "\"}";

                    if (narratorIndex == 0)
                    {
                        using (UnityWebRequest request = new UnityWebRequest("https://iidk.online/tts", "POST"))
                        {
                            byte[] raw = Encoding.UTF8.GetBytes(postData);

                            request.uploadHandler = new UploadHandlerRaw(raw);
                            request.SetRequestHeader("Content-Type", "application/json");

                            request.downloadHandler = new DownloadHandlerBuffer();
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error downloading TTS: " + request.error);
                                yield break;
                            }

                            byte[] response = request.downloadHandler.data;
                            File.WriteAllBytes(filePath, response);
                        }
                    } else
                    {
                        using (UnityWebRequest request = UnityWebRequest.Get("https://api.streamelements.com/kappa/v2/speech?voice=" + narratorName + "&text=" + UnityWebRequest.EscapeURL(text)))
                        {
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + request.error);
                            else
                                File.WriteAllBytes(filePath, request.downloadHandler.data);
                        }
                    }
                }
            }

            AudioClip clip = LoadSoundFromFile("TTS" + (narratorName == "Default" ? "" : narratorName) + "/" + fileName);
            Play2DAudio(clip, buttonClickVolume / 10f);
        }

        public static System.Collections.IEnumerator SpeakText(string text)
        {
            if (Time.time < (timeMenuStarted + 5f))
                yield break;

            string fileName = GetSHA256(text) + (narratorIndex == 0 ? ".wav" : ".mp3");
            string directoryPath = "iisStupidMenu/TTS" + (narratorName == "Default" ? "" : narratorName);

            if (!Directory.Exists("iisStupidMenu"))
                Directory.CreateDirectory("iisStupidMenu");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!File.Exists("iisStupidMenu/TTS" + (narratorName == "Default" ? "" : narratorName) + "/" + fileName))
            {
                string filePath = directoryPath + "/" + fileName;

                if (!File.Exists(filePath))
                {
                    string postData = "{\"text\": \"" + text.Replace("\n", "").Replace("\r", "").Replace("\"", "") + "\"}";

                    if (narratorIndex == 0)
                    {
                        using (UnityWebRequest request = new UnityWebRequest("https://iidk.online/tts", "POST"))
                        {
                            byte[] raw = Encoding.UTF8.GetBytes(postData);

                            request.uploadHandler = new UploadHandlerRaw(raw);
                            request.SetRequestHeader("Content-Type", "application/json");

                            request.downloadHandler = new DownloadHandlerBuffer();
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                            {
                                LogManager.LogError("Error downloading TTS: " + request.error);
                                yield break;
                            }

                            byte[] response = request.downloadHandler.data;
                            File.WriteAllBytes(filePath, response);
                        }
                    }
                    else
                    {
                        using (UnityWebRequest request = UnityWebRequest.Get("https://api.streamelements.com/kappa/v2/speech?voice=" + narratorName + "&text=" + UnityWebRequest.EscapeURL(text)))
                        {
                            yield return request.SendWebRequest();

                            if (request.result != UnityWebRequest.Result.Success)
                                LogManager.LogError("Error downloading TTS: " + request.error);
                            else
                                File.WriteAllBytes(filePath, request.downloadHandler.data);
                        }
                    }
                }
            }

            Sound.PlayAudio("TTS" + (narratorName == "Default" ? "" : narratorName) + "/" + fileName);
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
            Vector3 target = (scaleWithPlayer) ? new Vector3(0.1f, 0.3f, 0.3825f) * GorillaLocomotion.GTPlayer.Instance.scale : new Vector3(0.1f, 0.3f, 0.3825f);
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
            Transform menuTransform = menu.transform;
            menu = null;

            Vector3 before = menuTransform.localScale;
            float elapsedTime = 0f;
            while (elapsedTime < 0.05f)
            {
                menuTransform.localScale = Vector3.Lerp(before, Vector3.zero, elapsedTime / 0.05f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(menuTransform.gameObject);
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

        public static SnowballThrowable[] snowballs = new SnowballThrowable[] { };
        public static Dictionary<string, SnowballThrowable> snowballDict = null;
        public static SnowballThrowable GetProjectile(string provided)
        {
            if (snowballDict == null)
            {
                snowballDict = new Dictionary<string, SnowballThrowable>();

                snowballs = UnityEngine.Object.FindObjectsOfType<SnowballThrowable>(true);
                foreach (SnowballThrowable lol in snowballs)
                {
                    try
                    {
                        if (GetFullPath(lol.transform.parent).ToLower() == "player objects/local vrrig/local gorilla player/holdables" || GetFullPath(lol.transform.parent).ToLower().Contains("player objects/local vrrig/local gorilla player/riganchor/rig/body/shoulder.l/upper_arm.l/forearm.l/hand.l/palm.01.l/transferrableitemlefthand") || GetFullPath(lol.transform.parent).ToLower().Contains("player objects/local vrrig/local gorilla player/riganchor/rig/body/shoulder.r/upper_arm.r/forearm.r/hand.r/palm.01.r/transferrableitemrighthand"))
                        {
                            LogManager.Log("Projectile " + lol.gameObject.name + " logged");
                            snowballDict.Add(lol.gameObject.name, lol);
                        }
                    } catch { }
                }
                if (snowballDict.Count < 18)
                {
                    LogManager.Log("Projectile dictionary unfinished ("+snowballDict.Count+"/18)");
                    snowballDict = null;
                }
            }
            if (snowballDict != null && snowballDict.ContainsKey(provided))
            {
                return snowballDict[provided];
            } else
            {
                LogManager.Log("No key found for " + provided);
                return null;
            }
        }

        public static Dictionary<Type, object[]> archiveTypes = new System.Collections.Generic.Dictionary<Type, object[]> { };
        private static float lastRecievedTime = -1f;

        public static T[] GetAllType<T>(float decayTime = 5f) where T : UnityEngine.Object
        {
            Type type = typeof(T);

            if (Time.time > lastRecievedTime)
            {
                if (archiveTypes.ContainsKey(type))
                    archiveTypes.Remove(type);

                lastRecievedTime = Time.time + decayTime;
            }

            if (!archiveTypes.ContainsKey(type))
                archiveTypes.Add(type, FindObjectsOfType<T>(true));

            return (T[])archiveTypes[type];
        }

        public static void ClearType<T>() where T : UnityEngine.Object
        {
            Type type = typeof(T);

            if (archiveTypes.ContainsKey(type))
                archiveTypes.Remove(type);
        }

        public static BuilderTable GetBuilderTable()
        {
            BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out BuilderTable table);
            return table;
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
                } catch { LogManager.Log("Failure to get ownership, is the PhotonView valid?"); }
            }
        }

        public static bool PlayerIsTagged(VRRig Player)
        {
            string name = Player.mainSkin.material.name.ToLower();
            return name.Contains("fected") || name.Contains("it") || name.Contains("stealth") || name.Contains("ice") || !Player.nameTagAnchor.activeSelf;
        }

        public static bool PlayerIsLocal(VRRig Player)
        {
            return Player == GorillaTagger.Instance.offlineVRRig || Player == GhostRig;
        }

        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer> { };

            if (!PhotonNetwork.InRoom)
                return infected;

            string gamemode = GorillaGameManager.instance.GameModeName().ToLower();

            if (gamemode.Contains("infection") || gamemode.Contains("tag"))
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (tagman.isCurrentlyTag)
                    infected.Add(tagman.currentIt);
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                        infected.Add(plr);
                }
            }
            else if (gamemode.Contains("ghost"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla GhostTag Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                    infected.Add(tagman.currentIt);
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                        infected.Add(plr);
                }
            }
            else if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                    infected.Add(tagman.currentIt);
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                        infected.Add(plr);
                }
            }
            else if (gamemode.Contains("freeze"))
            {
                GorillaFreezeTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Freeze Tag Manager").GetComponent<GorillaFreezeTagManager>();
                if (tagman.isCurrentlyTag)
                    infected.Add(tagman.currentIt);
                else
                {
                    foreach (NetPlayer plr in tagman.currentInfected)
                        infected.Add(plr);
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
                    tagman.ChangeCurrentIt(plr);
                else
                {
                    if (!tagman.currentInfected.Contains(plr))
                        tagman.AddInfectedPlayer(plr);
                }
            }
            else if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                    tagman.ChangeCurrentIt(plr);
                else
                {
                    if (!tagman.currentInfected.Contains(plr))
                        tagman.AddInfectedPlayer(plr);
                }
            }
            else if (gamemode.Contains("ghost"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla GhostTag Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                    tagman.ChangeCurrentIt(plr);
                else
                {
                    if (!tagman.currentInfected.Contains(plr))
                        tagman.AddInfectedPlayer(plr);
                }
            }
            else if (gamemode.Contains("freeze"))
            {
                GorillaFreezeTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Freeze Tag Manager").GetComponent<GorillaFreezeTagManager>();
                if (tagman.isCurrentlyTag)
                    tagman.ChangeCurrentIt(plr);
                else
                {
                    if (!tagman.currentInfected.Contains(plr))
                        tagman.AddInfectedPlayer(plr);
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
                        tagman.currentIt = null;
                }
                else
                {
                    if (tagman.currentInfected.Contains(plr))
                        tagman.currentInfected.Remove(plr);
                }
            }
            else if (gamemode.Contains("ambush") || gamemode.Contains("stealth"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    if (tagman.currentIt == plr)
                        tagman.currentIt = null;
                }
                else
                {
                    if (tagman.currentInfected.Contains(plr))
                        tagman.currentInfected.Remove(plr);
                }
            }
            else if (gamemode.Contains("ghost"))
            {
                GorillaAmbushManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla GhostTag Manager").GetComponent<GorillaAmbushManager>();
                if (tagman.isCurrentlyTag)
                {
                    if (tagman.currentIt == plr)
                        tagman.currentIt = null;
                }
                else
                {
                    if (tagman.currentInfected.Contains(plr))
                        tagman.currentInfected.Remove(plr);
                }
            }
            else if (gamemode.Contains("freeze"))
            {
                GorillaFreezeTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Freeze Tag Manager").GetComponent<GorillaFreezeTagManager>();
                if (tagman.isCurrentlyTag)
                {
                    if (tagman.currentIt == plr)
                        tagman.currentIt = null;
                }
                else
                {
                    if (tagman.currentInfected.Contains(plr))
                        tagman.currentInfected.Remove(plr);
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
            Quaternion rot = GorillaTagger.Instance.leftHandTransform.rotation * GorillaLocomotion.GTPlayer.Instance.leftHandRotOffset;
            return (GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GorillaLocomotion.GTPlayer.Instance.leftHandOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
        {
            Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GorillaLocomotion.GTPlayer.Instance.rightHandRotOffset;
            return (GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GorillaLocomotion.GTPlayer.Instance.rightHandOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
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
                    switch (platformMode)
                    {
                        case 5:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
                            break;
                        case 6:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 32;
                            break;
                        case 7:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 204;
                            break;
                        case 8:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 231;
                            break;
                        case 9:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 240;
                            break;
                        case 10:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 249;
                            break;
                        case 11:
                            side.AddComponent<GorillaSurfaceOverride>().overrideIndex = 252;
                            break;
                    }
                }
                catch { }
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
            GameObject visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(visualizeGO, Time.deltaTime);
            Destroy(visualizeGO.GetComponent<Collider>());
            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = new Vector3(range, range, range);
            Color clr = color;
            clr.a = 0.25f;
            visualizeGO.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            visualizeGO.GetComponent<Renderer>().material.color = clr;
        }

        public static void VisualizeCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            GameObject visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(visualizeGO, Visuals.PerformanceVisuals ? Visuals.PerformanceModeStep : Time.deltaTime);
            Destroy(visualizeGO.GetComponent<Collider>());
            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = scale;
            visualizeGO.transform.rotation = rotation;
            Color clr = color;
            clr.a = 0.25f;
            visualizeGO.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            visualizeGO.GetComponent<Renderer>().material.color = clr;
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

        private static GameObject audiomgrhand = null;
        public static void PlayHandAudio(AudioClip sound, float volume, bool left)
        {
            if (audiomgrhand == null)
            {
                audiomgrhand = new GameObject("2DAudioMgr-hand");
                AudioSource temp = audiomgrhand.AddComponent<AudioSource>();
                temp.spatialBlend = 1f;
                temp.rolloffMode = AudioRolloffMode.Logarithmic;
                temp.minDistance = 1f;
                temp.maxDistance = 15f;
                temp.spatialize = true;
            }
            audiomgrhand.transform.SetParent(left ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer.gameObject.transform : GorillaTagger.Instance.offlineVRRig.rightHandPlayer.gameObject.transform, false);

            AudioSource ausrc = audiomgrhand.GetComponent<AudioSource>();
            ausrc.clip = sound;
            ausrc.loop = true;
            ausrc.Play();
        }

        public static string ToTitleCase(string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        public static Dictionary<string, float> waitingForTranslate = new Dictionary<string, float> { };
        public static Dictionary<string, string> translateCache = new Dictionary<string, string> { };
        public static string TranslateText(string input, Action<string> onTranslated = null)
        {
            if (translateCache.ContainsKey(input))
                return translateCache[input];
            else
            {
                if (!waitingForTranslate.ContainsKey(input))
                {
                    waitingForTranslate.Add(input, Time.time + 10f);
                    CoroutineManager.instance.StartCoroutine(GetTranslation(input, onTranslated));
                } else
                {
                    if (Time.time > waitingForTranslate[input])
                    {
                        waitingForTranslate.Remove(input);

                        waitingForTranslate.Add(input, Time.time + 10f);
                        CoroutineManager.instance.StartCoroutine(GetTranslation(input, onTranslated));
                    }
                }

                return "Loading...";
            }
        }

        public static System.Collections.IEnumerator GetTranslation(string text, Action<string> onTranslated = null)
        {
            if (translateCache.ContainsKey(text))
            {
                if (onTranslated != null)
                    onTranslated.Invoke(translateCache[text]);

                yield break;
            }

            string fileName = GetSHA256(text) + ".txt";
            string directoryPath = "iisStupidMenu/TranslationData" + language.ToUpper();

            if (!Directory.Exists("iisStupidMenu"))
                Directory.CreateDirectory("iisStupidMenu");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, fileName);

            string translation = null;

            if (!File.Exists(filePath))
            {
                string postData = "{\"text\": \"" + text.Replace("\n", "").Replace("\r", "").Replace("\"", "") + "\", \"lang\": \"" + language + "\"}";

                using (UnityWebRequest request = new UnityWebRequest("https://iidk.online/translate", "POST"))
                {
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
            }
            else
            {
                translation = File.ReadAllText(filePath);
            }

            if (translation != null)
            {
                translateCache.Add(text, translation);

                if (onTranslated != null)
                    onTranslated.Invoke(translation);
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
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static string ISO8601()
        {
            DateTime utcNow = DateTime.UtcNow;
            return utcNow.ToString("o");
        }

        public static string ColorToHex(Color color) 
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        public static string CleanString(string input, int maxLength = 12)
        {
            input = new string(Array.FindAll<char>(input.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));

            if (input.Length > maxLength)
                input = input.Substring(0, maxLength - 1);

            input = input.ToUpper();
            return input;
        }

        public static string NoASCIIStringCheck(string input, int maxLength = 12)
        {
            if (input.Length > maxLength)
                input = input.Substring(0, maxLength - 1);

            input = input.ToUpper();
            return input;
        }

        public static string NoRichtextTags(string input, string replace = "")
        {
            Regex notags = new Regex("<.*?>");
            return notags.Replace(input, replace);
        }

        // To get the optimal delay from call limiter
        public static float GetCallLimiterDelay(CallLimiter limiter)
        {
            return ((float)Traverse.Create(limiter).Field("timeCooldown").GetValue() / (int)Traverse.Create(limiter).Field("callHistoryLength").GetValue()); // + (Time.deltaTime * 1.5f)
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
                                Safety.AntiReportFRT(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
                        }
                    }
                }

                if (Safety.smartarp && data.Code == 200)
                {
                    string rpcName = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];
                    if (rpcName == "RPC_PlayHandTap")
                    {
                        object[] args = (object[])((Hashtable)data.CustomData)[(byte)4];
                        if ((int)args[0] == 67)
                        {
                            Safety.buttonClickTime = Time.frameCount;
                            Safety.buttonClickPlayer = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).UserId;
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
                                            Fun.keyLogs[i][1] = currentText.Length == 0 ? currentText : currentText.Substring(0, currentText.Length - 1);
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
                    }
                }
            } catch { }
        }

        public static void TeleportPlayer(Vector3 pos) // Prevents your hands from getting stuck on trees
        {
            GorillaLocomotion.GTPlayer.Instance.TeleportTo(World2Player(pos), GorillaLocomotion.GTPlayer.Instance.transform.rotation);
            closePosition = Vector3.zero;
            if (isSearching && !isPcWhenSearching)
            {
                VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            }
        }

        private static Dictionary<string, (int Category, int Index)> cacheGetIndex = new Dictionary<string, (int Category, int Index)> { }; // Looping through 800 elements is not a light task :/
        public static ButtonInfo GetIndex(string buttonText)
        {
            if (cacheGetIndex.ContainsKey(buttonText))
            {
                var CacheData = cacheGetIndex[buttonText];
                try
                {
                    if (Buttons.buttons[CacheData.Category][CacheData.Index].buttonText == buttonText)
                        return Buttons.buttons[CacheData.Category][CacheData.Index];
                } catch { cacheGetIndex.Remove(buttonText); }
            }

            int categoryIndex = 0;
            foreach (ButtonInfo[] buttons in Menu.Buttons.buttons)
            {
                int buttonIndex = 0;
                foreach (ButtonInfo button in buttons)
                {
                    if (button.buttonText == buttonText)
                    {   try
                        {
                            cacheGetIndex.Add(buttonText, (categoryIndex, buttonIndex));
                        } catch
                        {
                            if (cacheGetIndex.ContainsKey(buttonText))
                                cacheGetIndex.Remove(buttonText);
                        }
                        
                        return button;
                    }
                    buttonIndex++;
                }
                categoryIndex++;
            }

            return null;
        }

        public static int GetCategory(string categoryName)
        {
            return Buttons.categoryNames.ToList().IndexOf(categoryName);
        }

        public static int AddCategory(string categoryName)
        {
            List<ButtonInfo[]> buttonInfoList = Buttons.buttons.ToList();
            buttonInfoList.Add(new ButtonInfo[] { });
            Buttons.buttons = buttonInfoList.ToArray();

            List<string> categoryList = Buttons.categoryNames.ToList();
            categoryList.Add(categoryName);
            Buttons.categoryNames = categoryList.ToArray();

            return Buttons.buttons.Length - 1;
        }

        public static void RemoveCategory(string categoryName)
        {
            List<ButtonInfo[]> buttonInfoList = Buttons.buttons.ToList();
            buttonInfoList.RemoveAt(GetCategory(categoryName));
            Buttons.buttons = buttonInfoList.ToArray();

            List<string> categoryList = Buttons.categoryNames.ToList();
            categoryList.Remove(categoryName);
            Buttons.categoryNames = categoryList.ToArray();
        }

        public static void AddButton(int category, ButtonInfo button, int index = -1)
        {
            List<ButtonInfo> buttonInfoList = Buttons.buttons[category].ToList();
            if (index > 0)
                buttonInfoList.Insert(index, button);
            else
                buttonInfoList.Add(button);

            Buttons.buttons[category] = buttonInfoList.ToArray();
        }

        public static void AddButtons(int category, ButtonInfo[] buttons, int index = -1)
        {
            List<ButtonInfo> buttonInfoList = Buttons.buttons[category].ToList();
            if (index > 0)
            {
                for (int i = 0; i < buttons.Length; i++)
                    buttonInfoList.Insert(index + i, buttons[i]);
            }
            else
            {
                foreach (ButtonInfo button in buttons)
                    buttonInfoList.Add(button);
            }

            Buttons.buttons[category] = buttonInfoList.ToArray();
        }

        public static void RemoveButton(int category, string name, int index = -1)
        {
            List<ButtonInfo> buttonInfoList = Buttons.buttons[category].ToList();
            if (index > 0)
                buttonInfoList.RemoveAt(index);
            else
            {
                foreach (ButtonInfo button in buttonInfoList)
                {
                    if (button.buttonText == name)
                    {
                        buttonInfoList.Remove(button);
                        break;
                    }
                }
            }

            Buttons.buttons[category] = buttonInfoList.ToArray();
        }

        public static Dictionary<string, Assembly> cacheAssembly = new Dictionary<string, Assembly> { };
        public static Assembly GetAssembly(string dllName)
        {
            if (cacheAssembly.ContainsKey(dllName))
                return cacheAssembly[dllName];

            Assembly Assembly = Assembly.Load(File.ReadAllBytes(dllName.Replace("/", "\\")));
            cacheAssembly.Add(dllName, Assembly);
            return Assembly;
        }

        public static string[] GetPluginInfo(Assembly Assembly)
        {
            Type[] Types = Assembly.GetTypes();
            foreach (Type Type in Types)
            {
                FieldInfo Name = Type.GetField("Name", BindingFlags.Public | BindingFlags.Static);
                FieldInfo Description = Type.GetField("Description", BindingFlags.Public | BindingFlags.Static);
                if (Name != null && Description != null)
                    return new string[] { (string)Name.GetValue(null), (string)Description.GetValue(null) };
            }

            return new string[] { "null", "null" };
        }

        public static void EnablePlugin(Assembly Assembly)
        {
            Type[] Types = Assembly.GetTypes();
            foreach (Type Type in Types)
            {
                MethodInfo Method = Type.GetMethod("OnEnable", BindingFlags.Public | BindingFlags.Static);
                if (Method != null)
                    Method.Invoke(null, null);
            }
        }

        public static void DisablePlugin(Assembly Assembly)
        {
            Type[] Types = Assembly.GetTypes();
            foreach (Type Type in Types)
            {
                MethodInfo Method = Type.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.Static);
                if (Method != null)
                    Method.Invoke(null, null);
            }
        }

        public static Dictionary<Assembly, MethodInfo[]> cacheOnGUI = new Dictionary<Assembly, MethodInfo[]> { };
        public static void PluginOnGUI(Assembly Assembly)
        {
            if (cacheOnGUI.ContainsKey(Assembly))
            {
                foreach (MethodInfo Method in cacheOnGUI[Assembly])
                    Method.Invoke(null, null);
            } else
            {
                List<MethodInfo> Methods = new List<MethodInfo> { };

                Type[] Types = Assembly.GetTypes();
                foreach (Type Type in Types)
                {
                    MethodInfo Method = Type.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static);
                    if (Method != null)
                        Methods.Add(Method);
                }

                cacheOnGUI.Add(Assembly, Methods.ToArray());

                foreach (MethodInfo Method in Methods)
                    Method.Invoke(null, null);
            }
        }

        public static Dictionary<Assembly, MethodInfo[]> cacheUpdate = new Dictionary<Assembly, MethodInfo[]> { };
        public static void PluginUpdate(Assembly Assembly)
        {
            if (cacheUpdate.ContainsKey(Assembly))
            {
                foreach (MethodInfo Method in cacheUpdate[Assembly])
                    Method.Invoke(null, null);
            }
            else
            {
                List<MethodInfo> Methods = new List<MethodInfo> { };

                Type[] Types = Assembly.GetTypes();
                foreach (Type Type in Types)
                {
                    MethodInfo Method = Type.GetMethod("Update", BindingFlags.Public | BindingFlags.Static);
                    if (Method != null)
                        Methods.Add(Method);
                }

                cacheUpdate.Add(Assembly, Methods.ToArray());

                foreach (MethodInfo Method in Methods)
                    Method.Invoke(null, null);
            }
        }

        public static void ReloadMenu()
        {
            if (menu != null)
            {
                Destroy(menu);
                menu = null;

                Draw();
            }

            if (reference != null)
            {
                Destroy(reference);
                reference = null;

                CreateReference();
            }
        }

        public static void ChangeName(string PlayerName)
        {
            GorillaComputer.instance.currentName = PlayerName;
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            GorillaTagger.Instance.offlineVRRig.playerText1.text = PlayerName;
            GorillaTagger.Instance.offlineVRRig.playerText2.text = PlayerName;
            GorillaComputer.instance.savedName = PlayerName;
            PlayerPrefs.SetString("playerName", PlayerName);
            PlayerPrefs.Save();

            try
            {
                if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) || CosmeticWardrobeProximityDetector.IsUserNearWardrobe(PhotonNetwork.LocalPlayer.UserId))
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { GorillaTagger.Instance.offlineVRRig.playerColor.r, GorillaTagger.Instance.offlineVRRig.playerColor.g, GorillaTagger.Instance.offlineVRRig.playerColor.b });
                    RPCProtection();
                }
            } catch { }
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
                        { 13, "slider" }
                    };
                    try
                    {
                        ButtonInfo button = GetIndex(buttonText);
                        if (button != null)
                        {
                            if (button.isTogglable)
                                namesToIds[6] = button.enabled ? "leverup" : "leverdown";
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
                    if (searchText.Length > 0)
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
                lastPage = ((favorites.Count + pageSize - 1) / pageSize) - 1;
            
            if (buttonsType == 24)
            {
                List<string> enabledMods = new List<string>() { "Exit Enabled Mods" };
                foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                {
                    foreach (ButtonInfo v in buttonlist)
                    {
                        if (v.enabled)
                            enabledMods.Add(v.buttonText);
                    }
                }
                lastPage = ((enabledMods.Count + pageSize - 1) / pageSize) - 1;
            }
            if (isSearching)
            {
                List<ButtonInfo> searchedMods = new List<ButtonInfo> { };
                if (nonGlobalSearch && buttonsType != 0)
                {
                    foreach (ButtonInfo v in Buttons.buttons[buttonsType])
                    {
                        try
                        {
                            string buttonTextt = v.buttonText;
                            if (v.overlapText != null)
                                buttonTextt = v.overlapText;

                            if (buttonTextt.Replace(" ", "").ToLower().Contains(searchText.Replace(" ", "").ToLower()))
                                searchedMods.Add(v);
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
                                    buttonTextt = v.overlapText;

                                if (buttonTextt.Replace(" ", "").ToLower().Contains(searchText.Replace(" ", "").ToLower()))
                                    searchedMods.Add(v);
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
                    pageNumber = lastPage;
            }
            else
            {
                if (buttonText == "NextPage")
                {
                    if (dynamicAnimations)
                        lastClickedName = "NextPage";

                    pageNumber++;
                    if (pageNumber > lastPage)
                        pageNumber = 0;
                }
                else
                {
                    ButtonInfo target = GetIndex(buttonText);
                    if (target != null)
                    {
                        if (fromMenu && ((leftGrab && !joystickMenu) || (joystickMenu && rightJoystick.y > 0.5f)))
                        {
                            if (IsBinding)
                            {
                                bool AlreadyBinded = false;
                                string BindedTo = "";
                                foreach (KeyValuePair<string, List<string>> Bind in ModBindings)
                                {
                                    if (Bind.Value.Contains(target.buttonText))
                                    {
                                        AlreadyBinded = true;
                                        BindedTo = Bind.Key;
                                        break;
                                    }
                                }

                                if (AlreadyBinded)
                                {
                                    target.customBind = null;
                                    ModBindings[BindedTo].Remove(target.buttonText);
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully unbinded mod.");
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48, rightHand, 0.4f);
                                } else
                                {
                                    target.customBind = BindInput;
                                    ModBindings[BindInput].Add(target.buttonText);
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully binded mod to " + BindInput + ".");
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, rightHand, 0.4f);
                                }
                            }
                            else
                            {
                                if (target.buttonText != "Exit Favorite Mods")
                                {
                                    if (favorites.Contains(target.buttonText))
                                    {
                                        favorites.Remove(target.buttonText);
                                        NotifiLib.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Removed from favorites.");
                                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48, rightHand, 0.4f);
                                    }
                                    else
                                    {
                                        favorites.Add(target.buttonText);
                                        NotifiLib.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Added to favorites.");
                                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, rightHand, 0.4f);
                                    }
                                }
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
                                            try { target.enableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(string.Format("{0} // Error with mod enableMethod {1} at {2}: {3}", PluginInfo.Name, target.buttonText, exc.StackTrace, exc.Message)); }
                                    }
                                    else
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
                                        if (target.disableMethod != null)
                                            try { target.disableMethod.Invoke(); } catch (Exception exc) { LogManager.LogError(string.Format("{0} // Error with mod disableMethod {1} at {2}: {3}", PluginInfo.Name, target.buttonText, exc.StackTrace, exc.Message)); }
                                    }
                                }
                                else
                                {
                                    if (dynamicAnimations)
                                        lastClickedName = target.buttonText;

                                    NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);

                                    if (target.method != null)
                                        try { target.method.Invoke(); } catch (Exception exc) { LogManager.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, target.buttonText, exc.StackTrace, exc.Message)); }
                                }
                                try
                                {
                                    if (fromMenu && admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? rightJoystickClick : false && PhotonNetwork.InRoom && !isOnPC)
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
                        LogManager.LogError(buttonText + " does not exist");
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
            LogManager.Log(ascii);
            LogManager.Log("Thank you for using ii's Stupid Menu!");

            timeMenuStarted = Time.time;
            IsSteam = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";

            try
            {
                if (!Font.GetOSInstalledFontNames().Contains("Agency FB"))
                {
                    GameObject AgencyGO = LoadAsset("agency");
                    agency = AgencyGO.transform.Find("text").gameObject.GetComponent<Text>().font;
                    Destroy(AgencyGO);
                }
            } catch { }

            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;

            string ConsoleGUID = "iiMenu_Console";
            try
            {
                if (!GameObject.Find(ConsoleGUID)) // Makes sure Admin mods do not activate twice
                    new GameObject(ConsoleGUID).AddComponent<Classes.Console>();
            } catch { new GameObject(ConsoleGUID).AddComponent<Classes.Console>(); } // It's worth a shot

            if (ServerData.ServerDataEnabled)
                new GameObject("iiMenu_ServerData").AddComponent<ServerData>();

            if (!Directory.Exists("iisStupidMenu"))
                Directory.CreateDirectory("iisStupidMenu");

            if (!Directory.Exists("iisStupidMenu/Sounds"))
                Directory.CreateDirectory("iisStupidMenu/Sounds");

            if (!Directory.Exists("iisStupidMenu/Plugins"))
                Directory.CreateDirectory("iisStupidMenu/Plugins");

            try
            {
                Settings.LoadPlugins();
            }
            catch (Exception e) { LogManager.Log("Error with Settings.LoadPlugins(): " + e.ToString()); }

            loadPreferencesTime = Time.time;
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

        public static bool FATMENU = true;
        public static bool longmenu;
        public static bool disorganized;
        public static bool flipMenu;
        public static bool shinymenu;
        public static bool dropOnRemove = true;
        public static bool shouldOutline;
        public static bool shouldRound;
        public static bool lastclicking;
        public static bool openedwithright;
        public static bool likebark;

        public static int pageSize = 6;
        public static int pageNumber;
        public static bool noPageNumber;
        public static bool disablePageButtons;
        public static int pageButtonType = 1;

        public static int buttonsType;
        public static int buttonClickSound = 8;
        public static int buttonClickIndex;
        public static int buttonClickVolume = 4;
        public static float buttonOffset = 2;
        public static bool doButtonsVibrate = true;

        public static bool joystickMenu;
        public static bool physicalMenu;
        public static Vector3 physicalOpenPosition = Vector3.zero;
        public static Quaternion physicalOpenRotation = Quaternion.identity;
        public static bool joystickOpen;
        public static int joystickButtonSelected;
        public static string joystickSelectedButton = "";
        public static float joystickDelay = -1f;

        public static bool rightHand;
        public static bool isRightHand;
        public static bool bothHands;
        public static bool wristThing;
        public static bool wristThingV2;
        public static bool wristOpen;
        public static float wristMenuDelay = -1f;

        public static bool disableNotifications;
        public static bool narrateNotifications;
        public static string narratorName = "Default";
        public static int narratorIndex;
        public static bool showEnabledModsVR = true;
        public static bool hideSettings;
        public static bool disableDisconnectButton;
        public static bool disableFpsCounter;
        public static bool disableSearchButton;
        public static bool disableReturnButton;

        public static bool ghostException;
        public static bool disableGhostview;
        public static bool legacyGhostview;
        public static bool checkMode;
        public static bool lastChecker;
        public static bool highQualityText;

        public static bool SmoothGunPointer;
        public static bool smallGunPointer;
        public static bool disableGunPointer;
        public static bool disableGunLine;
        public static bool SwapGunHand;
        public static bool GunSounds;
        public static int gunVariation;
        public static int GunDirection;
        public static int GunLineQuality = 50;

        public static bool GunSpawned;
        public static bool gunLocked;
        public static VRRig lockTarget;

        public static bool lastGunSpawned;
        public static bool lastGunTrigger;

        public static int fontCycle;
        public static int fontStyleType = 2;
        public static bool NoAutoSizeText;

        public static bool doCustomName;
        public static string customMenuName = "your text here";
        public static bool doCustomMenuBackground;
        public static bool disableBoardColor;
        public static bool disableBoardTextColor;
        public static bool menuTrail;
        public static int pcbg;

        public static bool isSearching;
        public static bool nonGlobalSearch;
        public static bool isPcWhenSearching;
        public static string searchText = "";
        public static float lastBackspaceTime;

        public static int fullModAmount = -1;
        public static int amountPartying;
        public static bool waitForPlayerJoin;
        public static bool scaleWithPlayer;

        public static bool dynamicSounds;
        public static bool dynamicAnimations;
        public static bool dynamicGradients;
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
        public static KeyCode[] allowedKeys = {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
            KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
            KeyCode.Z, KeyCode.Space, KeyCode.Backspace, KeyCode.Escape // it doesn't fit :(
        };

        public static bool ToggleBindings = true;
        public static bool IsBinding;
        public static string BindInput = "";

        public static Dictionary<string, List<string>> ModBindings = new Dictionary<string, List<string>> {
            { "A", new List<string> { } },
            { "B", new List<string> { } },
            { "X", new List<string> { } },
            { "Y", new List<string> { } },
            { "LG", new List<string> { } },
            { "RG", new List<string> { } },
            { "LT", new List<string> { } },
            { "RT", new List<string> { } },
            { "LJ", new List<string> { } },
            { "RJ", new List<string> { } },
        };

        public static Dictionary<string, bool> BindStates = new Dictionary<string, bool> {
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

        public static Dictionary<string, string> admins = new Dictionary<string, string> { { "47F316437B9BE495", "goldentrophy" } };

        public static string hotkeyButton = "none";

        public static int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static int Zone = LayerMask.NameToLayer("Zone");
        public static int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
        public static int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

        public static Camera TPC;
        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject reference;
        public static SphereCollider buttonCollider;
        public static GameObject canvasObj;
        public static AssetBundle assetBundle;
        public static Text fpsCount;
        public static Text searchTextObject;
        public static Text title;
        public static VRRig GhostRig;
        public static Material funnyghostmaterial;
        public static Material searchMat;
        public static Material returnMat;
        public static Material fixMat;

        public static Font agency = Font.CreateDynamicFontFromOSFont("Agency FB", 24);
        public static Font Arial = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        public static Font Verdana = Font.CreateDynamicFontFromOSFont("Verdana", 24);
        public static Font sans = Font.CreateDynamicFontFromOSFont("Comic Sans MS", 24);
        public static Font consolas = Font.CreateDynamicFontFromOSFont("Consolas", 24);
        public static Font ubuntu = Font.CreateDynamicFontFromOSFont("Candara", 24);
        public static Font MSGOTHIC = Font.CreateDynamicFontFromOSFont("MS Gothic", 24);
        public static Font impact = Font.CreateDynamicFontFromOSFont("Impact", 24);
        public static Font gtagfont;
        public static Font activeFont = agency;
        public static FontStyle activeFontStyle = FontStyle.Italic;

        public static GameObject lKeyReference;
        public static SphereCollider lKeyCollider;

        public static GameObject rKeyReference;
        public static SphereCollider rKeyCollider;

        public static GameObject VRKeyboard;
        public static GameObject menuSpawnPosition;

        public static GameObject watchobject;
        public static GameObject watchText;
        public static GameObject watchShell;
        public static GameObject watchEnabledIndicator;
        public static Material watchIndicatorMat;
        public static int currentSelectedModThing;

        public static GameObject regwatchobject;
        public static GameObject regwatchText;
        public static GameObject regwatchShell;

        public static Material OrangeUI = new Material(Shader.Find("GorillaTag/UberShader"));
        public static Material ForestMat = null;
        public static Material StumpMat = null;
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
        public static Texture2D fixTexture;
        public static Texture2D customMenuBackgroundImage;

        public static Material crownmat;
        public static Texture2D admincrown;

        public static List<string> favorites = new List<string> { "Exit Favorite Mods" };

        public static List<GorillaNetworkJoinTrigger> triggers = new List<GorillaNetworkJoinTrigger> { };
        public static List<TMPro.TextMeshPro> udTMP = new List<TMPro.TextMeshPro> { };

        public static string StumpLeaderboardID = "UnityTempFile";
        public static string ForestLeaderboardID = "UnityTempFile";

        public static int StumpLeaderboardIndex = 3;
        public static int ForestLeaderboardIndex = 10;

        public static Material[] ogScreenMats = new Material[] { };

        public static bool translate;
        public static string language;

        public static string repReason = "room host force changed";
        public static Dictionary<string, long> annoyingIDs = new Dictionary<string, long> { };
        public static List<string> annoyedIDs = new List<string> { };

        public static List<string> muteIDs = new List<string> { };
        public static List<string> mutedIDs = new List<string> { };

        public static string serverLink = "https://discord.gg/iidk";

        public static string[] letters = new string[]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M"
        };

        public static int[] bones = new int[] {
            4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7
        };

        public static int arrowType;
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
            new string[] {"", ""}
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

        public static string[] InternalProjectileNamesRight = new string[]
        {
            "LMACF. RIGHT.",
            "LMAEY. RIGHT.",
            "LMAGE. RIGHT.",
            "LMAHR. RIGHT.",
            "LMAIF. RIGHT.",
            "LMAIP. RIGHT.",
            "LMAMO. RIGHT.",
            "LMAMT. RIGHT.",
            "LMAMO. RIGHT.",
            "LMAMO. RIGHT.",
            "LMAMV."
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

        public static Vector3 closePosition;

        public static Vector3 pointerOffset = new Vector3(0f, -0.1f, 0f);
        public static int pointerIndex;

        public static bool lastInRoom;
        public static bool lastMasterClient;
        public static string lastRoom = "";

        public static int platformMode;
        public static int platformShape;

        public static bool customSoundOnJoin;

        public static string rejRoom;
        public static float rejDebounce;

        public static string partyLastCode;
        public static float partyTime;
        public static bool phaseTwo;

        public static bool adminIsScaling;
        public static VRRig adminRigTarget;
        public static float adminScale = 1f;
        public static Player adminConeExclusion;

        public static float timeMenuStarted = -1f;
        public static float kgDebounce;
        public static float nameCycleDelay;
        public static float stealIdentityDelay;
        public static float beesDelay;
        public static float laggyRigDelay;
        public static float jrDebounce;
        public static float projDebounce;
        public static float projDebounceType = 0.1f;
        public static float soundDebounce;
        public static float buttonCooldown;
        public static float colorChangerDelay;
        public static float teleDebounce;
        public static float splashDel;
        public static float headspazDelay;
        public static float internetTime = 5f;
        public static float autoSaveDelay = Time.time + 60f;
        public static bool BackupPreferences;

        public static int projmode;
        public static int trailmode;

        public static int notificationDecayTime = 1000;
        public static int notificationSoundIndex;

        public static float oldSlide;

        public static int accessoryType;
        public static int hat;

        public static int soundId;

        public static float red = 1f;
        public static float green = 0.5f;
        public static float blue;

        public static bool lastOwner;
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

        public static int nameCycleIndex;

        public static bool lastprimaryhit;

        public static int colorChangeType;
        public static bool strobeColor;

        public static bool AntiCrashToggle;
        public static bool AntiSoundToggle;
        public static bool AntiCheatSelf;
        public static bool AntiCheatAll;
        public static bool AntiACReport;
        public static bool AntiOculusReport;
        public static bool NoGliderRespawn;

        public static bool lastHit;
        public static bool lastHit2;
        public static bool lastRG;

        public static int tindex = 1;

        public static bool lastHitL;
        public static bool lastHitR;
        public static bool lastHitLP;
        public static bool lastHitRP;
        public static bool lastHitRS;

        public static bool plastLeftGrip;
        public static bool plastRightGrip;

        public static bool EverythingSlippery;
        public static bool EverythingGrippy;

        public static bool headspazType;

        public static bool hasFoundAllBoards;

        public static float lastBangTime;

        public static float subThingy;
        public static float subThingyZ;

        public static float sizeScale = 1f;

        public static float turnAmnt;
        public static float TagAuraDelay;
        public static float startX = -1f;
        public static float startY = -1f;

        public static bool lowercaseMode;
        public static string inputTextColor = "green";
        
        public static bool annoyingMode; // Build with this enabled for a surprise
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
