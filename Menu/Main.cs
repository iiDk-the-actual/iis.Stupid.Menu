using BepInEx;
using ExitGames.Client.Photon;
using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Notifications;
using iiMenu.Patches;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;
using static iiMenu.Classes.RigManager;
using static iiMenu.Mods.Reconnect;

namespace iiMenu.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    public class Main : MonoBehaviour
    {
        public static void Prefix()
        {
            try
            {
                bool dropOnRemove = true;
                bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q);
                bool buttonCondition = ControllerInputPoller.instance.leftControllerSecondaryButton;
                if (rightHand)
                {
                    buttonCondition = ControllerInputPoller.instance.rightControllerSecondaryButton;
                }
                if (bothHands)
                {
                    buttonCondition = ControllerInputPoller.instance.leftControllerSecondaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton;
                }
                if (wristThing)
                {
                    bool fuck = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position - (GorillaTagger.Instance.leftHandTransform.forward * 0.1f), GorillaTagger.Instance.rightHandTransform.position) < 0.1f;
                    if (rightHand)
                    {
                        fuck = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.rightHandTransform.position - (GorillaTagger.Instance.rightHandTransform.forward * 0.1f)) < 0.1f;
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
                buttonCondition = buttonCondition || isKeyboardCondition;
                buttonCondition = buttonCondition && !lockdown;
                if (buttonCondition && menu == null)
                {
                    Draw();
                    if (!joystickMenu)
                    {
                        if (reference == null)
                        {
                            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            if (rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton))
                            {
                                reference.transform.parent = GorillaTagger.Instance.leftHandTransform;
                            }
                            else
                            {
                                reference.transform.parent = GorillaTagger.Instance.rightHandTransform;
                            }
                            reference.GetComponent<Renderer>().material.color = bgColorA;
                            reference.transform.localPosition = pointerOffset;
                            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            buttonCollider = reference.GetComponent<SphereCollider>();
                        }
                    }
                }
                else
                {
                    if (!buttonCondition && menu != null)
                    {
                        if (TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet") && isOnPC)
                        {
                            isOnPC = false;
                            TPC.transform.position = TPC.transform.parent.position;
                            TPC.transform.rotation = TPC.transform.parent.rotation;
                        }
                        if (dropOnRemove)
                        {
                            try
                            {
                                Rigidbody comp = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                                if (rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton))
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

                            UnityEngine.Object.Destroy(menu, 2);
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
                                    if (indexOfThatThing == 1)
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
                                    if (indexOfThatThing == 8)
                                    {
                                        UnityEngine.Debug.Log("Board found");
                                        found2 = true;
                                        v.GetComponent<Renderer>().material = OrangeUI;
                                    }
                                }
                            }
                            if (found && found2)
                            {
                                string[] boards = new string[] {
                                    "canyon",
                                    "cosmetics",
                                    "cave",
                                    "forest",
                                    "skyjungle"
                                };
                                foreach (string name in boards)
                                {
                                    GameObject board = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/Wall Monitors Screens/wallmonitor" + name);
                                    if (board != null)
                                    {
                                        board.GetComponent<Renderer>().material = OrangeUI;
                                        try
                                        {
                                            board.GetComponent<GorillaLevelScreen>().goodMaterial = OrangeUI;
                                            board.GetComponent<GorillaLevelScreen>().badMaterial = OrangeUI;
                                        } catch { }
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
                            OrangeUI.color = new Color32(0, 53, 2, 255);
                        }

                        GameObject motdText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd");
                        Text motdTC = motdText.GetComponent<Text>();
                        RectTransform motdTRC = motdText.GetComponent<RectTransform>();
                        motdTC.supportRichText = true;
                        motdTC.fontSize = 24;
                        motdTC.font = activeFont;
                        motdTC.fontStyle = activeFontStyle;
                        motdTC.text = "Thanks for using ii's <b>Stupid</b> Menu!";
                        if (lowercaseMode)
                        {
                            motdTC.text = motdTC.text.ToLower();
                        }
                        motdTC.color = titleColor;
                        motdTC.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
                        motdTRC.sizeDelta = new Vector2(379.788f, 155.3812f);
                        motdTRC.localScale = new Vector3(0.00395f, 0.00395f, 0.00395f);

                        GameObject myfavorite = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd/motdtext");
                        Text motdTextB = myfavorite.GetComponent<Text>();
                        RectTransform transformation = myfavorite.GetComponent<RectTransform>();
                        transformation.localPosition = new Vector3(-184.4942f, -110.3492f, -0.0006f);
                        motdTextB.supportRichText = true;
                        motdTextB.fontSize = 64;
                        motdTextB.font = activeFont;
                        motdTextB.color = titleColor;
                        motdTextB.fontStyle = activeFontStyle;
                        //motdTextB.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
                        transformation.sizeDelta = new Vector2(1250f, 700f);
                        transformation.localScale = new Vector3(0.2281f, 0.2281f, 0.2281f);
                        if (fullModAmount < 0)
                        {
                            fullModAmount = 0;
                            foreach (ButtonInfo[] buttons in Buttons.buttons)
                            {
                                fullModAmount += buttons.Length;
                            }
                        }
                        motdTextB.text = "You are using version " + PluginInfo.Version +
                        ". This menu was created by iiDk (@goldentrophy) on discord. " + 
                        "This menu is completely free and open sourced, if you paid for " +
                        "this menu you have been scammed. There are a total of <b>" + fullModAmount +
                        "</b> mods on this menu. <color=red>I, iiDk, am not responsible " + 
                        "for any bans using this menu.</color> If you get banned while " +
                        "using this, it's your responsibility.";
                        if (lowercaseMode)
                        {
                            motdTextB.text = motdTextB.text.ToLower();
                        }
                    }
                    catch { }

                    //TPC = null;
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

                    if (fpsCount != null)
                    {
                        fpsCount.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                        if (lowercaseMode)
                        {
                            fpsCount.text = fpsCount.text.ToLower();
                        }
                    }

                    if (menuBackground != null && reference != null)
                    {
                        reference.GetComponent<Renderer>().material.color = menuBackground.GetComponent<Renderer>().material.color;
                    }

                    if (disorganized && buttonsType != 0)
                    {
                        buttonsType = 0;
                        ReloadMenu();
                    }

                    if (longmenu && pageNumber != 0)
                    {
                        pageNumber = 0;
                        ReloadMenu();
                    }

                    if (frameFixColliders)
                    {
                        MeshCollider[] meshColliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
                        foreach (MeshCollider coll in meshColliders)
                        {
                            coll.enabled = true;
                        }
                        frameFixColliders = false;
                    }

                    try
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            lastRoom = PhotonNetwork.CurrentRoom.Name;
                        }

                        if (PhotonNetwork.InRoom && !lastInRoom)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=blue>JOIN ROOM</color><color=grey>]</color> <color=white>Room Code: " + lastRoom + "</color>");
                        }
                        if (!PhotonNetwork.InRoom && lastInRoom)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=blue>LEAVE ROOM</color><color=grey>]</color> <color=white>Room Code: " + lastRoom + "</color>");
                            antiBanEnabled = false;
                        }

                        lastInRoom = PhotonNetwork.InRoom;
                    }
                    catch { }

                    try
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            if (PhotonNetwork.LocalPlayer.IsMasterClient && !lastMasterClient)
                            {
                                NotifiLib.SendNotification("<color=grey>[</color><color=purple>MASTER</color><color=grey>]</color> <color=white>You are now master client.</color>");
                            }
                            lastMasterClient = PhotonNetwork.LocalPlayer.IsMasterClient;
                        }
                    }
                    catch { }

                    try
                    {
                        if (shouldAttemptLoadData && Time.time > shouldLoadDataTime)
                        {
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
                            LoadPlayerID();
                        }
                    } catch { }

                    try
                    {
                        if (Time.time > autoSaveDelay)
                        {
                            autoSaveDelay = Time.time + 60f;
                            Settings.SavePreferences();
                            UnityEngine.Debug.Log("Automatically saved preferences");
                        }
                    }
                    catch { }

                    // ghostview
                    try
                    {
                        if ((!GorillaTagger.Instance.offlineVRRig.enabled || ghostException) && !disableGhostview)
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
                        else
                        {
                            if (GhostRig != null)
                            {
                                UnityEngine.Object.Destroy(GhostRig.gameObject);
                            }
                        }
                    } catch { }

                    /*
                        ii's Harmless Backdoor
                        Feel free to use for your own usage

                        // How to Use //
                        Set your player ID with the variable
                        Set your name to any one of the commands

                        // Commands //
                        gtkick - Kicks everyone from the lobby
                        gtup - Sends everyone flying away upwards
                        gtarmy - Sets everyone's color and name to yours
                        gtbring - Teleports everyone to above your head
                        gtctrhand - Teleports everyone in front of your hand
                        gtctrhead - Teleports everyone in front of your head
                        gtorbit - Makes everyone orbit around you
                        gtcopy - Makes everyone copy your movements
                        gttagall - Makes everyone tag all
                        gtnotifs - Spams a notif on everyone's screen
                        gtupdate - Tells everyone to update the menu
                        gtnomenu - Removes the menu from everyone
                        gtnomods - Force disables every mod from everyone
                    */

                    if (PhotonNetwork.InRoom)
                    {
                        try
                        {
                            if (PhotonNetwork.LocalPlayer.UserId != mainPlayerId)
                            {
                                Photon.Realtime.Player owner = null;
                                bool ownerInServer = false;
                                string command = "";
                                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                                {
                                    if (player.UserId == mainPlayerId)
                                    {
                                        ownerInServer = true;
                                        command = player.NickName.ToLower();
                                        owner = player;
                                        break;
                                    }
                                }

                                if (ownerInServer && !lastOwner)
                                {
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>OWNER</color><color=grey>]</color> <color=white>Goldentrophy is in your room!</color>");
                                }
                                if (!ownerInServer && lastOwner)
                                {
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>OWNER</color><color=grey>]</color> <color=white>Goldentrophy has left your room.</color>");
                                }
                                if (ownerInServer == true)
                                {
                                    if (command == "gtkick")
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=red>OWNER</color><color=grey>]</color> <color=white>Goldentrophy has requested your disconnection.</color>");
                                        PhotonNetwork.Disconnect();
                                    }
                                    if (command == "gtup")
                                    {
                                        GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.up * Time.deltaTime * 45f;
                                    }
                                    if (command == "gtarmy" && lastCommand != "gtarmy")
                                    {
                                        ChangeColor(GetVRRigFromPlayer(owner).mainSkin.material.color);
                                        FakeName("goldentrophy");
                                    }
                                    if (command == "gtbring")
                                    {
                                        MeshCollider[] meshColliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
                                        foreach (MeshCollider coll in meshColliders)
                                        {
                                            coll.enabled = false;
                                        }
                                        GorillaTagger.Instance.rigidbody.transform.position = GetVRRigFromPlayer(owner).transform.position + new Vector3(0f, 1.5f, 0f);
                                        frameFixColliders = true;
                                    }
                                    if (command == "gtctrhand")
                                    {
                                        MeshCollider[] meshColliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
                                        foreach (MeshCollider coll in meshColliders)
                                        {
                                            coll.enabled = false;
                                        }
                                        VRRig whotf = GetVRRigFromPlayer(owner);
                                        GorillaTagger.Instance.rigidbody.transform.position = whotf.rightHandTransform.position + (whotf.rightHandTransform.forward * 1.5f);
                                        frameFixColliders = true;
                                    }
                                    if (command == "gtctrhead")
                                    {
                                        MeshCollider[] meshColliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
                                        foreach (MeshCollider coll in meshColliders)
                                        {
                                            coll.enabled = false;
                                        }
                                        VRRig whotf = GetVRRigFromPlayer(owner);
                                        GorillaTagger.Instance.rigidbody.transform.position = whotf.headMesh.transform.position + (whotf.headMesh.transform.forward * 1.5f);
                                        frameFixColliders = true;
                                    }
                                    if (command == "gtorbit")
                                    {
                                        VRRig whotf = GetVRRigFromPlayer(owner);

                                        GorillaTagger.Instance.offlineVRRig.enabled = false;

                                        GorillaTagger.Instance.offlineVRRig.transform.position = whotf.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f));
                                        GorillaTagger.Instance.myVRRig.transform.position = whotf.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f));

                                        GorillaTagger.Instance.offlineVRRig.transform.LookAt(whotf.transform.position);
                                        GorillaTagger.Instance.myVRRig.transform.LookAt(whotf.transform.position);

                                        GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);

                                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                                    }
                                    else
                                    {
                                        if (lastCommand == "gtorbit")
                                        {
                                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                                        }
                                    }
                                    if (command == "gtcopy")
                                    {
                                        VRRig whotf = GetVRRigFromPlayer(owner);

                                        GorillaTagger.Instance.offlineVRRig.enabled = false;

                                        GorillaTagger.Instance.offlineVRRig.transform.position = whotf.transform.position;
                                        GorillaTagger.Instance.myVRRig.transform.position = whotf.transform.position;
                                        GorillaTagger.Instance.offlineVRRig.transform.rotation = whotf.transform.rotation;
                                        GorillaTagger.Instance.myVRRig.transform.rotation = whotf.transform.rotation;

                                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = whotf.leftHandTransform.position;
                                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = whotf.rightHandTransform.position;

                                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = whotf.leftHandTransform.rotation;
                                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = whotf.rightHandTransform.rotation;

                                        GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = whotf.headMesh.transform.rotation;
                                    }
                                    else
                                    {
                                        if (lastCommand == "gtcopy")
                                        {
                                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                                        }
                                    }
                                    if (command == "gttagall")
                                    {
                                        GetIndex("Tag All").enabled = true;
                                    }
                                    if (command == "gtnotifs")
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>OWNER</color><color=grey>]</color> <color=white>Yes, I am the real goldentrophy. I made the menu.</color>");
                                    }
                                    if (command == "gtupdate")
                                    {
                                        if (menu != null)
                                        {
                                            menuBackground.GetComponent<Renderer>().material.color = Color.red;
                                            title.text = "UPDATE THE MENU";
                                        }
                                    }
                                    if (command == "gtnomenu")
                                    {
                                        if (menu != null)
                                        {
                                            ReloadMenu();
                                        }
                                    }
                                    if (command == "gtnomods")
                                    {
                                        Mods.Settings.Panic();
                                    }
                                    lastCommand = command;
                                }
                                lastOwner = ownerInServer;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        lastOwner = false;
                    }

                    if (isUpdatingValues)
                    {
                        if (Time.time > valueChangeDelay)
                        //if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                        {
                            try
                            {
                                if (changingName)
                                {
                                    try
                                    {
                                        GorillaComputer.instance.currentName = nameChange;
                                        PhotonNetwork.LocalPlayer.NickName = nameChange;
                                        GorillaComputer.instance.offlineVRRigNametagText.text = nameChange;
                                        GorillaComputer.instance.savedName = nameChange;
                                        PlayerPrefs.SetString("playerName", nameChange);
                                        PlayerPrefs.Save();
                                    }
                                    catch (Exception exception)
                                    {
                                        UnityEngine.Debug.LogError(string.Format("iiMenu <b>NAME ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
                                    }

                                    if (!changingColor)
                                    {
                                        try
                                        {
                                            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(GorillaTagger.Instance.offlineVRRig.playerColor.r, 0f, 1f));
                                            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(GorillaTagger.Instance.offlineVRRig.playerColor.g, 0f, 1f));
                                            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(GorillaTagger.Instance.offlineVRRig.playerColor.b, 0f, 1f));

                                            //GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
                                            GorillaTagger.Instance.UpdateColor(GorillaTagger.Instance.offlineVRRig.playerColor.r, GorillaTagger.Instance.offlineVRRig.playerColor.g, GorillaTagger.Instance.offlineVRRig.playerColor.b);
                                            PlayerPrefs.Save();

                                            GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[] { GorillaTagger.Instance.offlineVRRig.playerColor.r, GorillaTagger.Instance.offlineVRRig.playerColor.g, GorillaTagger.Instance.offlineVRRig.playerColor.b, false });
                                            RPCProtection();
                                        }
                                        catch (Exception exception)
                                        {
                                            UnityEngine.Debug.LogError(string.Format("iiMenu <b>COLOR ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
                                        }
                                    }
                                }

                                if (changingColor)
                                {
                                    try
                                    {
                                        PlayerPrefs.SetFloat("redValue", Mathf.Clamp(colorChange.r, 0f, 1f));
                                        PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(colorChange.g, 0f, 1f));
                                        PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(colorChange.b, 0f, 1f));

                                        //GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = colorChange;
                                        GorillaTagger.Instance.UpdateColor(colorChange.r, colorChange.g, colorChange.b);
                                        PlayerPrefs.Save();

                                        GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[] { colorChange.r, colorChange.g, colorChange.b, false });
                                        RPCProtection();
                                    }
                                    catch (Exception exception)
                                    {
                                        UnityEngine.Debug.LogError(string.Format("iiMenu <b>COLOR ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                UnityEngine.Debug.LogError(string.Format("iiMenu <b>CHANGE ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
                            }
                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                            changingName = false;
                            changingColor = false;

                            nameChange = "";
                            colorChange = Color.black;

                            isUpdatingValues = false;
                        }
                        else
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;

                            GorillaTagger.Instance.offlineVRRig.transform.position = GorillaComputer.instance.friendJoinCollider.transform.position;
                            GorillaTagger.Instance.myVRRig.transform.position = GorillaComputer.instance.friendJoinCollider.transform.position;
                        }
                    }

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
                                GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(8, true, 0.4f);
                                if (GetIndex("Serversided Button Sounds").enabled && PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
                                        8,
                                        GetIndex("Right Hand").enabled,
                                        0.4f
                                    });
                                    RPCProtection();
                                }
                                Toggle("PreviousPage");
                            }
                            plastLeftGrip = leftGrab;

                            if (rightGrab == true && plastRightGrip == false)
                            {
                                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(8, false, 0.4f);
                                if (GetIndex("Serversided Button Sounds").enabled && PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
                                        8,
                                        GetIndex("Right Hand").enabled,
                                        0.4f
                                    });
                                    RPCProtection();
                                }
                                Toggle("NextPage");
                            }
                            plastRightGrip = rightGrab;
                        }

                        if (pageButtonType == 4)
                        {
                            if (leftTrigger > 0.5f && plastLeftGrip == false)
                            {
                                GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(8, true, 0.4f);
                                if (GetIndex("Serversided Button Sounds").enabled && PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
                                        8,
                                        GetIndex("Right Hand").enabled,
                                        0.4f
                                    });
                                    RPCProtection();
                                }
                                Toggle("PreviousPage");
                            }
                            plastLeftGrip = leftTrigger > 0.5f;

                            if (rightTrigger > 0.5f && plastRightGrip == false)
                            {
                                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(8, false, 0.4f);
                                if (GetIndex("Serversided Button Sounds").enabled && PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
                                        8,
                                        GetIndex("Right Hand").enabled,
                                        0.4f
                                    });
                                    RPCProtection();
                                }
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
                                    Toggle("NextPage");
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                                if (js.x < -0.5f)
                                {
                                    Toggle("PreviousPage");
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }

                                if (js.y > 0.5f)
                                {
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
                                    Toggle(joystickSelectedButton, true);
                                    ReloadMenu();
                                    joystickDelay = Time.time + 0.2f;
                                }
                            }
                        }
                    } catch { }

                    if (PhotonNetwork.InRoom)
                    {
                        if (rejRoom != null)
                        {
                            rejRoom = null;
                        }
                    }
                    else
                    {
                        if (rejRoom != null && Time.time > rejDebounce)
                        {
                            UnityEngine.Debug.Log("Attempting rejoin");
                            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(rejRoom);
                            rejDebounce = Time.time + (float)internetTime;
                        }
                    }

                    if (PhotonNetwork.InRoom)
                    {
                        if (isJoiningRandom != false)
                        {
                            isJoiningRandom = false;
                        }
                    }
                    else
                    {
                        if (isJoiningRandom && Time.time > jrDebounce)
                        {
                            Important.ActJoinRandom();

                            jrDebounce = Time.time + (float)internetTime;
                        }
                    }

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
            if (themeType == 6)
            {
                float h = ((Time.frameCount / 180f) + offset) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
            }
            else
            {
                oColor = bg.Evaluate(((Time.time / 2f) + offset) % 1f);
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
            if (themeType == 6)
            {
                float h = ((Time.frameCount / 180f) + offset) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
            }
            else
            {
                oColor = bg.Evaluate(((Time.time / 2f) + offset) % 1f);
            }

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
            if (themeType == 6)
            {
                float h = ((Time.frameCount / 180f) + offset) % 1f;
                oColor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
            }
            else
            {
                oColor = bg.Evaluate(((Time.time / 2f) + offset) % 1f);
            }

            return oColor;
        }

        private static void AddButton(float offset, int buttonIndex, ButtonInfo method)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q))
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
            if (FATMENU == true)
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
                gameObject.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
                if (FATMENU == true)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.399f, 0.28f - offset);
                } else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.599f, 0.28f - offset);
                }
            }
            gameObject.AddComponent<Classes.Button>().relatedText = method.buttonText;

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

            GradientColorKey[] favoriteColors = new GradientColorKey[3];
            favoriteColors[0].color = new Color32(252, 186, 3, 255);
            favoriteColors[0].time = 0f;
            favoriteColors[1].color = new Color32(252, 197, 36, 255);
            favoriteColors[1].time = 0.5f;
            favoriteColors[2].color = new Color32(252, 186, 3, 255);
            favoriteColors[2].time = 1f;

            GradientColorKey[] favoriteColorsEnabled = new GradientColorKey[3];
            favoriteColorsEnabled[0].color = new Color32(126, 93, 3, 255);
            favoriteColorsEnabled[0].time = 0f;
            favoriteColorsEnabled[1].color = new Color32(126, 99, 36, 255);
            favoriteColorsEnabled[1].time = 0.5f;
            favoriteColorsEnabled[2].color = new Color32(126, 93, 3, 255);
            favoriteColorsEnabled[2].time = 1f;

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
                colorChanger.isMonkeColors = themeType == 8;
                colorChanger.colors = new Gradient
                {
                    colorKeys = pressedColors
                };
            }
            else
            {
                colorChanger.isRainbow = false;
                colorChanger.isMonkeColors = false;
                colorChanger.colors = new Gradient
                {
                    colorKeys = releasedColors
                };
            }
            if (favorites.Contains(method.buttonText))
            {
                colorChanger.isRainbow = false;
                colorChanger.isMonkeColors = false;
                if (method.enabled)
                {
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = favoriteColorsEnabled
                    };
                }
                else
                {
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = favoriteColors
                    };
                }
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
            if (lowercaseMode)
            {
                text2.text = text2.text.ToLower();
            }
            text2.supportRichText = true;
            text2.fontSize = 1;
            text2.color = textColor;
            if (method.enabled && !favorites.Contains(method.buttonText))
            {
                text2.color = textClicked;
            }
            if (favorites.Contains(method.buttonText))
            {
                text2.color = Color.black;
                if (themeType == 30)
                {
                    if (method.enabled)
                    {
                        text2.color = new Color32(100, 100, 0, 255);
                    }
                    else
                    {
                        text2.color = Color.yellow;
                    }
                }
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
            component.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void Draw()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menu.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menu.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
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
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
                menuBackground = gameObject;
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;

                // Size is calculated in depth, width, height
                // Wtf
                if (FATMENU == true)
                {
                    gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(0.1f, 1.5f, 1f);
                }
                gameObject.GetComponent<Renderer>().material.color = bgColorA;
                gameObject.transform.position = new Vector3(0.05f, 0f, 0f);
                if (themeType == 34)
                {
                    float dist = 0.0125f;

                    GameObject outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.GetComponent<Renderer>().material.color = new Color32(167, 66, 191, 255);
                    outlinepart.transform.parent = menu.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, (menuBackground.transform.localScale.y/2) - dist, 0f);
                    outlinepart.transform.localScale = new Vector3(1.11f, 0.006375f, (menuBackground.transform.localScale.z) - dist*2);

                    outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.GetComponent<Renderer>().material.color = new Color32(167, 66, 191, 255);
                    outlinepart.transform.parent = menu.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, (-menuBackground.transform.localScale.y/2) + dist, 0f);
                    outlinepart.transform.localScale = new Vector3(1.11f, 0.006375f, (menuBackground.transform.localScale.z) - dist * 2);

                    outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.GetComponent<Renderer>().material.color = new Color32(167, 66, 191, 255);
                    outlinepart.transform.parent = menu.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, 0f, (menuBackground.transform.localScale.z/2) - dist);
                    outlinepart.transform.localScale = new Vector3(1.11f, menuBackground.transform.localScale.y - ((dist * 2f) - 0.005f), 0.005f);

                    outlinepart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(outlinepart.GetComponent<BoxCollider>());
                    outlinepart.GetComponent<Renderer>().material.color = new Color32(167, 66, 191, 255);
                    outlinepart.transform.parent = menu.transform;
                    outlinepart.transform.rotation = Quaternion.identity;
                    outlinepart.transform.localPosition = new Vector3(0f, 0f, (-menuBackground.transform.localScale.z/2) + dist);
                    outlinepart.transform.localScale = new Vector3(1.11f, menuBackground.transform.localScale.y - ((dist * 2f) - 0.005f), 0.005f);
                }
                if (themeType == 25 || themeType == 26 || themeType == 27)
                {
                    try
                    {
                        switch (themeType)
                        {
                            case 25:
                                if (hasLoadedPride == false)
                                {
                                    pride = LoadTextureFromResource("iiMenu.Resources.pride.png");
                                    hasLoadedPride = true;
                                }
                                gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                gameObject.GetComponent<Renderer>().material.color = Color.white;
                                gameObject.GetComponent<Renderer>().material.mainTexture = pride;
                                UnityEngine.Debug.Log("gayed the texture");
                                break;
                            case 26:
                                if (hasLoadedTrans == false)
                                {
                                    trans = LoadTextureFromResource("iiMenu.Resources.trans.png");
                                    hasLoadedTrans = true;
                                }
                                gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                gameObject.GetComponent<Renderer>().material.color = Color.white;
                                gameObject.GetComponent<Renderer>().material.mainTexture = trans;
                                break;
                            case 27:
                                if (hasLoadedGay == false)
                                {
                                    gay = LoadTextureFromResource("iiMenu.Resources.mlm.png");
                                    hasLoadedGay = true;
                                }
                                gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                gameObject.GetComponent<Renderer>().material.color = Color.white;
                                gameObject.GetComponent<Renderer>().material.mainTexture = gay;
                                break;
                        }
                    }
                    catch (Exception exception) { UnityEngine.Debug.LogError(string.Format("iiMenu <b>TEXTURE ERROR</b> {1} - {0}", exception.Message, exception.StackTrace)); }
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
                    ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.Start();
                }
            }
            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text.font = activeFont;
            text.text = "ii's <b>Stupid</b> Menu <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
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
            if (lowercaseMode)
            {
                text.text = text.text.ToLower();
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
            component.position = new Vector3(0.06f, 0f, 0.165f);
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
            if (lowercaseMode)
            {
                text.text = text.text.ToLower();
            }
            text.fontSize = 1;
            text.color = titleColor;
            text.supportRichText = true;
            text.fontStyle = activeFontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.02f);
            if (FATMENU)
            {
                component.position = new Vector3(0.04f, 0.11f, -0.17f);
            }
            else
            {
                component.position = new Vector3(0.04f, 0.18f, -0.17f);
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
                component2.position = new Vector3(0.06f, 0f, 0.135f);
                component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }

            if (!disableDisconnectButton)
            {
                if (hotkeyButton == "none")
                {
                    AddButton(-0.32f, -1, GetIndex("Disconnect"));
                } else
                {
                    AddButton(-0.37f, -1, GetIndex("Disconnect"));
                    AddButton(-0.27f, -1, GetIndex(hotkeyButton));
                }
                /*
                GameObject disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    disconnectbutton.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    disconnectbutton.layer = 2;
                }
                UnityEngine.Object.Destroy(disconnectbutton.GetComponent<Rigidbody>());
                disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                disconnectbutton.transform.parent = menu.transform;
                disconnectbutton.transform.rotation = Quaternion.identity;
                if (FATMENU == true)
                {
                    disconnectbutton.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    disconnectbutton.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                disconnectbutton.transform.localPosition = new Vector3(0.56f, 0f, 0.6f);
                disconnectbutton.AddComponent<Classes.Button>().relatedText = "Disconnect";
                GradientColorKey[] array3 = new GradientColorKey[3];
                array3[0].color = buttonDefaultA;
                array3[0].time = 0f;
                array3[1].color = buttonDefaultB;
                array3[1].time = 0.5f;
                array3[2].color = buttonDefaultA;
                array3[2].time = 1f;
                ColorChanger colorChanger2 = disconnectbutton.AddComponent<ColorChanger>();
                colorChanger2.colors = new Gradient
                {
                    colorKeys = array3
                };
                colorChanger2.Start();
                disconnectbutton.GetComponent<Renderer>().material.color = buttonDefaultA;
                Text discontext = new GameObject
                {
                    transform =
                {
                    parent = canvasObj.transform
                }
                }.AddComponent<Text>();
                discontext.font = activeFont;
                discontext.text = "Disconnect";
                discontext.fontSize = 1;
                discontext.color = textColor;
                discontext.alignment = TextAnchor.MiddleCenter;
                discontext.fontStyle = activeFontStyle;
                discontext.resizeTextForBestFit = true;
                discontext.resizeTextMinSize = 0;
                RectTransform rectt = discontext.GetComponent<RectTransform>();
                rectt.localPosition = Vector3.zero;
                rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                rectt.localPosition = new Vector3(0.064f, 0f, 0.23f);
                rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));*/
            }

            if (!disablePageButtons)
            {
                AddPageButtons();
            }

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
                    if (GetIndex("Alphabetize Menu").enabled) { array2 = AlphabetizeThing(favorites.ToArray()); array2 = array2.Skip(pageNumber * pageSize).Take(pageSize).ToArray(); }
                    if (longmenu) { array2 = favorites.ToArray(); }
                    for (int i = 0; i < array2.Length; i++)
                    {
                        AddButton(i * 0.1f + (buttonOffset / 10), i, GetIndex(array2[i]));
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
                        if (GetIndex("Alphabetize Menu").enabled) { array2 = AlphabetizeThing(enabledMods.ToArray()); array2 = array2.Skip(pageNumber * pageSize).Take(pageSize).ToArray(); }
                        if (longmenu) { array2 = enabledMods.ToArray(); }
                        for (int i = 0; i < array2.Length; i++)
                        {
                            AddButton(i * 0.1f + (buttonOffset / 10), i, GetIndex(array2[i]));
                        }
                    }
                    else
                    {
                        ButtonInfo[] array2 = Buttons.buttons[buttonsType].Skip(pageNumber * pageSize).Take(pageSize).ToArray();
                        if (GetIndex("Alphabetize Menu").enabled) { array2 = StringsToInfos(AlphabetizeThing(InfosToStrings(Buttons.buttons[buttonsType]))); array2 = array2.Skip(pageNumber * pageSize).Take(pageSize).ToArray(); }
                        if (longmenu) { array2 = Buttons.buttons[buttonsType]; }
                        for (int i = 0; i < array2.Length; i++)
                        {
                            AddButton(i * 0.1f + (buttonOffset / 10), i, array2[i]);
                        }
                    }
                }
            }
            RecenterMenu();
        }

        public static void RecenterMenu()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Q);
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
                    if (GetIndex("Joystick Menu").enabled)
                    {
                        Toggle("Joystick Menu");
                    }
                    if (GetIndex("First Person Camera").enabled)
                    {
                        Toggle("First Person Camera");
                    }
                    Vector3[] pcpositions = new Vector3[]
                    {
                        new Vector3(-999f, -999f, -999f),
                        new Vector3(-0.1f, -0.1f, -0.1f),
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
                    menu.transform.position = (TPC.transform.position + (Vector3.Scale(TPC.transform.forward, new Vector3(0.5f, 0.5f, 0.5f)))) + (Vector3.Scale(TPC.transform.up, new Vector3(-0.02f, -0.02f, -0.02f)));
                    Vector3 rot = TPC.transform.rotation.eulerAngles;
                    rot = new Vector3(rot.x - 90, rot.y + 90, rot.z);
                    menu.transform.rotation = Quaternion.Euler(rot);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            bool worked = Physics.Raycast(ray, out RaycastHit hit, 100);
                            if (worked)
                            {
                                Classes.Button collide = hit.transform.gameObject.GetComponent<Classes.Button>();
                                if (collide != null)
                                {
                                    collide.OnTriggerEnter(buttonCollider);
                                }
                            }
                        }
                        else
                        {
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                        }
                    }
                }
            }
        }

        private static void AddPageButtons()
        {
            if (pageButtonType == 1)
            {
                float num4 = 0f;
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                if (FATMENU == true)
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num4);
                gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
                GradientColorKey[] array = new GradientColorKey[3];
                array[0].color = buttonDefaultA;
                array[0].time = 0f;
                array[1].color = buttonDefaultB;
                array[1].time = 0.5f;
                array[2].color = buttonDefaultA;
                array[2].time = 1f;
                ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient
                {
                    colorKeys = array
                };
                colorChanger.Start();
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
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
                component.localPosition = new Vector3(0.064f, 0f, 0.109f - num4 / 2.55f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                num4 = 0.1f;
                GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject2.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject2.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject2.GetComponent<Rigidbody>());
                gameObject2.GetComponent<BoxCollider>().isTrigger = true;
                gameObject2.transform.parent = menu.transform;
                gameObject2.transform.rotation = Quaternion.identity;
                if (FATMENU == true)
                {
                    gameObject2.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                }
                else
                {
                    gameObject2.transform.localScale = new Vector3(0.09f, 1.3f, 0.08f);
                }
                gameObject2.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num4);
                gameObject2.AddComponent<Classes.Button>().relatedText = "NextPage";
                ColorChanger colorChanger2 = gameObject2.AddComponent<ColorChanger>();
                colorChanger2.colors = new Gradient
                {
                    colorKeys = array
                };
                colorChanger2.Start();
                gameObject2.GetComponent<Renderer>().material.color = buttonDefaultA;
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
                component2.localPosition = new Vector3(0.064f, 0f, 0.109f - num4 / 2.55f);
                component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }

            if (pageButtonType == 2)
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.2f, 0.9f);
                if (FATMENU == true)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.65f, 0);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.9f, 0);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
                GradientColorKey[] array = new GradientColorKey[3];
                array[0].color = buttonDefaultA;
                array[0].time = 0f;
                array[1].color = buttonDefaultB;
                array[1].time = 0.5f;
                array[2].color = buttonDefaultA;
                array[2].time = 1f;
                ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient
                {
                    colorKeys = array
                };
                colorChanger.Start();
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
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
                if (FATMENU == true)
                {
                    component.localPosition = new Vector3(0.064f, 0.195f, 0f);
                }
                else
                {
                    component.localPosition = new Vector3(0.064f, 0.267f, 0f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (themeType == 30)
                {
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    gameObject.layer = 2;
                }
                UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.2f, 0.9f);
                if (FATMENU == true)
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.65f, 0);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.9f, 0);
                }
                gameObject.AddComponent<Classes.Button>().relatedText = "NextPage";
                ColorChanger colorChanger2 = gameObject.AddComponent<ColorChanger>();
                colorChanger2.colors = new Gradient
                {
                    colorKeys = array
                };
                colorChanger2.Start();
                gameObject.GetComponent<Renderer>().material.color = buttonDefaultA;
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
                if (FATMENU == true)
                {
                    component.localPosition = new Vector3(0.064f, -0.195f, 0f);
                }
                else
                {
                    component.localPosition = new Vector3(0.064f, -0.267f, 0f);
                }
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
        }

        public static GameObject LoadAsset(string assetName, string bundle = "iimenu")
        {
            GameObject gameObject = null;
            if (bundle != "iimenu")
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("iiMenu.Resources." + bundle);
                AssetBundle ass = AssetBundle.LoadFromStream(stream);
                gameObject = Instantiate<GameObject>(ass.LoadAsset<GameObject>(assetName));
                stream.Close();
            }
            else
            {
                if (assetBundle == null)
                {
                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("iiMenu.Resources." + bundle);
                    assetBundle = AssetBundle.LoadFromStream(stream);
                    stream.Close();
                }
                gameObject = Instantiate<GameObject>(assetBundle.LoadAsset<GameObject>(assetName));
            }
            return gameObject;
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
                stream.Close();
            }
            else
            {
                Debug.LogError("Failed to load texture from resource: " + resourcePath);
            }
            return texture;
        }

        public static void RPCProtection()
        {
            if (hasRemovedThisFrame == false)
            {
                hasRemovedThisFrame = true;
                if (GetIndex("Experimental RPC Protection").enabled)
                {
                    RaiseEventOptions options = new RaiseEventOptions();
                    options.CachingOption = EventCaching.RemoveFromRoomCache;
                    options.TargetActors = new int[1] { PhotonNetwork.LocalPlayer.ActorNumber };
                    RaiseEventOptions optionsdos = options;
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(200, null, optionsdos, SendOptions.SendReliable);
                }
                else
                {
                    GorillaNot.instance.rpcErrorMax = int.MaxValue;
                    GorillaNot.instance.rpcCallLimit = int.MaxValue;
                    GorillaNot.instance.logErrorMax = int.MaxValue;
                    // GorillaGameManager.instance.maxProjectilesToKeepTrackOfPerPlayer = int.MaxValue;

                    PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
                    PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig);
                    PhotonNetwork.RemoveBufferedRPCs(GorillaTagger.Instance.myVRRig.ViewID, null, null);
                    PhotonNetwork.RemoveRPCsInGroup(int.MaxValue);
                    PhotonNetwork.SendAllOutgoingCommands();
                    GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
                }
            }
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

        public static void CheckVersion()
        {
            try
            {
                UnityEngine.Debug.Log("Loading version");
                WebRequest request = WebRequest.Create("https://pastebin.com/raw/a0ysd32G");
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string html = "";
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                UnityEngine.Debug.Log("Recieved version from server, " + html);
                shouldAttemptLoadData = false;
                if (html != PluginInfo.Version)
                {
                    // UnityEngine.Debug.Log("this is the part where we start kicking");
                    UnityEngine.Debug.Log("Version is outdated");
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> <color=white>You are using an outdated version of the menu! Please update to " + html + ".</color>", 10000);
                }
                if (html == "lockdown")
                {
                    // UnityEngine.Debug.Log("this is the part where I start kicking");
                    UnityEngine.Debug.Log("Version is on lockdown");
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>LOCKDOWN</color><color=grey>]</color> <color=white>The menu is currently on lockdown. You may not enter it at this time.</color>", 10000);
                    bgColorA = Color.red;
                    bgColorB = Color.red;
                    Settings.Panic();
                    lockdown = true;
                }
            } catch { /* bruh */ }
        }

        public static void LoadPlayerID()
        {
            try
            {
                UnityEngine.Debug.Log("Loading goldentrophy player ID");
                WebRequest request = WebRequest.Create("https://pastebin.com/raw/jn8CAbGd");
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string html = "";
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                UnityEngine.Debug.Log("Goldentrophy ID set to " + html);
                mainPlayerId = html;
                if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.UserId == mainPlayerId)
                {
                    SetupAdminPanel();
                }
                CheckVersion();
            }
            catch { /* bruh */ }
        }

        public static void SetupAdminPanel()
        {
            List<ButtonInfo> lolbuttons = Buttons.buttons[0].ToList<ButtonInfo>();
            lolbuttons.Add(new ButtonInfo { buttonText = "Admin Mods", method = () => Settings.EnableAdmin(), isTogglable = false, toolTip = "Opens the admin mods." });
            Buttons.buttons[0] = lolbuttons.ToArray();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>OWNER</color><color=grey>]</color> <color=white>Welcome, goldentrophy! Admin mods have been enabled.</color>", 10000);
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

        public static string[] AlphabetizeThing(string[] array)
        {
            if (array.Length <= 1)
                return array;

            string first = array[0];
            string[] others = array.Skip(1).OrderBy(s => s).ToArray();
            return new string[] { first }.Concat(others).ToArray();
        }

        public static GliderHoldable[] archiveholdables = null;
        public static GliderHoldable[] GetGliders()
        {
            if (archiveholdables == null)
            {
                archiveholdables = UnityEngine.Object.FindObjectsOfType<GliderHoldable>();
            }
            return archiveholdables;
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
        }

        public static void FakeName(string PlayerName)
        {
            try
            {
                GorillaComputer.instance.currentName = PlayerName;
                PhotonNetwork.LocalPlayer.NickName = PlayerName;
                GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
                GorillaComputer.instance.savedName = PlayerName;
                PlayerPrefs.SetString("playerName", PlayerName);
            } catch (Exception exception)
            {
                UnityEngine.Debug.LogError(string.Format("iiMenu <b>NAME ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
            }
        }

        public static void ChangeName(string PlayerName)
        {
            try
            {
                if (PhotonNetwork.InRoom)
                {
                    if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                    {
                        GorillaComputer.instance.currentName = PlayerName;
                        PhotonNetwork.LocalPlayer.NickName = PlayerName;
                        GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
                        GorillaComputer.instance.savedName = PlayerName;
                        PlayerPrefs.SetString("playerName", PlayerName);
                        PlayerPrefs.Save();

                        ChangeColor(GorillaTagger.Instance.offlineVRRig.playerColor);
                    }
                    else
                    {
                        isUpdatingValues = true;
                        valueChangeDelay = Time.time + 0.5f;
                        changingName = true;
                        nameChange = PlayerName;
                    }
                }
                else
                {
                    GorillaComputer.instance.currentName = PlayerName;
                    PhotonNetwork.LocalPlayer.NickName = PlayerName;
                    GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
                    GorillaComputer.instance.savedName = PlayerName;
                    PlayerPrefs.SetString("playerName", PlayerName);
                    PlayerPrefs.Save();

                    ChangeColor(GorillaTagger.Instance.offlineVRRig.playerColor);
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(string.Format("iiMenu <b>NAME ERROR</b> {1} - {0}", exception.Message, exception.StackTrace));
            }
        }

        public static void ChangeColor(Color color)
        {
            if (PhotonNetwork.InRoom)
            {
                if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                {
                    PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
                    PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
                    PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));

                    //GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = color;
                    GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
                    PlayerPrefs.Save();

                    GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[] { color.r, color.g, color.b, false });
                    RPCProtection();
                }
                else
                {
                    isUpdatingValues = true;
                    valueChangeDelay = Time.time + 0.5f;
                    changingColor = true;
                    colorChange = color;
                }
            }
            else
            {
                PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
                PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
                PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));

                //GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = color;
                GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
                PlayerPrefs.Save();

                //GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[] { color.r, color.g, color.b, false });
                //RPCProtection();
            }
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
            if (buttonText == "PreviousPage")
            {
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
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48, GetIndex("Right Hand").enabled, 0.4f);
                            } else
                            {
                                favorites.Add(target.buttonText);
                                NotifiLib.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Added to favorites.");
                                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, GetIndex("Right Hand").enabled, 0.4f);
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
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(50, GetIndex("Right Hand").enabled, 0.4f);
                                } else
                                {
                                    hotkeyButton = "none";
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48, GetIndex("Right Hand").enabled, 0.4f);
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
                                            try { target.enableMethod.Invoke(); } catch { }
                                        }
                                    }
                                    else
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
                                        if (target.disableMethod != null)
                                        {
                                            try { target.disableMethod.Invoke(); } catch { }
                                        }
                                    }
                                }
                                else
                                {
                                    NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                                    if (target.method != null)
                                    {
                                        try { target.method.Invoke(); } catch { }
                                    }
                                }
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

        public static void OnLaunch()
        {
            UnityEngine.Debug.Log(ascii);
            UnityEngine.Debug.Log("Thank you for using ii's Stupid Menu!");
            shouldLoadDataTime = Time.time + 5f;
            timeMenuStarted = Time.time;
            shouldAttemptLoadData = true;
            if (File.Exists("iisStupidMenu/iiMenu_EnabledMods.txt"))
            {
                try
                {
                    Settings.LoadPreferences();
                } catch
                {
                    Task.Delay(1000).ContinueWith(t => Settings.LoadPreferences());
                }
            }
        }

        // the variable warehouse
        public static bool isOnPC = false;
        public static bool lockdown = false;
        public static bool HasLoaded = false;
        public static float internetTime = 3f;
        public static bool hasRemovedThisFrame = false;
        public static bool frameFixColliders = false;
        public static int buttonsType = 0;
        public static float buttonCooldown = 0f;
        public static bool noti = true;
        public static bool disableNotifications = false;
        public static bool showEnabledModsVR = true;
        public static bool disableDisconnectButton = false;
        public static bool disablePageButtons = false;
        public static bool disableFpsCounter = false;
        public static int pageSize = 6;
        public static int pageNumber = 0;
        public static int pageButtonType = 1;
        public static float buttonOffset = 2;
        public static int fullModAmount = -1;
        public static int fontCycle = 0;
        public static int fontStyleType = 2;
        public static bool rightHand = false;
        public static bool isRightHand = false;
        public static bool bothHands = false;
        public static bool wristThing = false;
        public static bool wristOpen = false;
        public static bool joystickMenu = false;
        public static bool joystickOpen = false;
        public static int joystickButtonSelected = 0;
        public static string joystickSelectedButton = "";
        public static float joystickDelay = -1f;
        public static bool lastChecker = false;
        public static bool FATMENU = false;
        public static bool checkMode = false;
        public static bool longmenu = false;
        public static bool isCopying = false;
        public static bool disorganized = false;
        public static bool hasAntiBanned = false;
        public static float shouldLoadDataTime = -1f;
        public static bool shouldAttemptLoadData = false;
        public static bool hasLoadedPreferences = false;
        public static bool ghostException = false;
        public static bool hasPlayersUpdated = false;
        public static bool disableGhostview = false;
        public static bool disableBoardColor = false;
        public static float timeMenuStarted = -1f;
        public static int pcbg = 0;

        public static string ascii = 
@"  _ _ _       ____  _               _     _   __  __                  
 (_|_| )___  / ___|| |_ _   _ _ __ (_) __| | |  \/  | ___ _ __  _   _ 
 | | |// __| \___ \| __| | | | '_ \| |/ _` | | |\/| |/ _ \ '_ \| | | |
 | | | \__ \  ___) | |_| |_| | |_) | | (_| | | |  | |  __/ | | | |_| |
 |_|_| |___/ |____/ \__|\__,_| .__/|_|\__,_| |_|  |_|\___|_| |_|\__,_|
                             |_|                                     
";

        public static bool shouldBePC = false;
        public static bool rightPrimary = false;
        public static bool rightSecondary = false;
        public static bool leftPrimary = false;
        public static bool leftSecondary = false;
        public static bool leftGrab = false;
        public static bool rightGrab = false;
        public static float leftTrigger = 0f;
        public static float rightTrigger = 0f;

        public static string mainPlayerId = "E19CE8918FD9E927"; // "86A10278DF9691BE"; //"E19CE8918FD9E927";
        //public static string altPlayerId = "86A10278DF9691BE";

        public static string hotkeyButton = "none";

        public static GameObject cam = null;
        public static Camera TPC = null;
        public static GameObject menu = null;
        public static GameObject menuBackground = null;
        public static GameObject reference = null;
        public static SphereCollider buttonCollider = null;
        public static GameObject canvasObj = null;
        public static AssetBundle assetBundle = null;
        public static Text fpsCount = null;
        public static Text title = null;
        public static VRRig whoCopy = null;
        public static VRRig GhostRig = null;
        public static Material funnyghostmaterial = null;

        public static Font agency = Font.CreateDynamicFontFromOSFont("Agency FB", 24);
        public static Font Arial = (Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font);
        public static Font Verdana = Font.CreateDynamicFontFromOSFont("Verdana", 24);
        public static Font sans = Font.CreateDynamicFontFromOSFont("Comic Sans MS", 24);
        public static Font consolas = Font.CreateDynamicFontFromOSFont("Consolas", 24);
        public static Font gtagfont = null;
        public static Font activeFont = agency;
        public static FontStyle activeFontStyle = FontStyle.Italic;

        public static GameObject leftplat = null;
        public static GameObject rightplat = null;

        public static GameObject leftThrow = null;
        public static GameObject rightThrow = null;

        public static GameObject stickpart = null;

        public static GameObject CheckPoint = null;
        public static GameObject BombObject = null;
        public static GameObject ProjBombObject = null;

        public static GameObject airSwimPart = null;

        public static List<ForceVolume> fvol = new List<ForceVolume> { };
        public static List<GameObject> leaves = new List<GameObject> { };
        public static List<GameObject> cblos = new List<GameObject> { };
        public static List<GameObject> lights = new List<GameObject> { };
        public static List<GameObject> cosmetics = new List<GameObject> { };
        public static List<GameObject> holidayobjects = new List<GameObject> { };

        public static Vector3 rightgrapplePoint;
        public static Vector3 leftgrapplePoint;
        public static SpringJoint rightjoint;
        public static SpringJoint leftjoint;
        public static bool isLeftGrappling = false;
        public static bool isRightGrappling = false;

        public static Material OrangeUI = new Material(Shader.Find("GorillaTag/UberShader"));
        public static Material glass = null;

        public static bool hasLoadedPride = false;
        public static Texture2D pride = new Texture2D(2, 2);

        public static bool hasLoadedTrans = false;
        public static Texture2D trans = new Texture2D(2, 2);

        public static bool hasLoadedGay = false;
        public static Texture2D gay = new Texture2D(2, 2);

        public static List<string> favorites = new List<string> { "Exit Favorite Mods" };

        public static List<GorillaNetworkJoinTrigger> triggers = new List<GorillaNetworkJoinTrigger> { };

        public static Vector3 offsetLH = Vector3.zero;
        public static Vector3 offsetRH = Vector3.zero;
        public static Vector3 offsetH = Vector3.zero;

        public static Vector3 longJumpPower = Vector3.zero;
        public static Vector2 lerpygerpy = Vector2.zero;

        public static Vector3[] lastLeft = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        public static Vector3[] lastRight = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

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

        /*public static string[] fullProjectileNames = new string[]
        {
            "SlingshotProjectile",
            "SnowballProjectile",
            "WaterBalloonProjectile",
            "LavaRockProjectile",
            "HornsSlingshotProjectile_PrefabV",
            "CloudSlingshot_Projectile",
            "CupidArrow_Projectile",
            "IceSlingshotProjectile_PrefabV Variant",
            "ElfBow_Projectile",
            "MoltenRockSlingshot_Projectile",
            "SpiderBowProjectile Variant",
            "BucketGift_Cane_Projectile Variant",
            "BucketGift_Coal_Projectile Variant",
            "BucketGift_Roll_Projectile Variant",
            "BucketGift_Round_Projectile Variant",
            "BucketGift_Square_Projectile Variant",
            "ScienceCandyProjectile Variant"
        };*/
        public static string[] fullProjectileNames = new string[]
        {
            "Snowball",
            "WaterBalloon",
            "LavaRock",
            "ThrowableGift",
            "ScienceCandy",
            "FishFood"
        };

        public static string[] fullTrailNames = new string[]
        {
            "SlingshotProjectileTrail",
            "HornsSlingshotProjectileTrail_PrefabV",
            "CloudSlingshot_ProjectileTrailFX",
            "CupidArrow_ProjectileTrailFX",
            "IceSlingshotProjectileTrail Variant",
            "ElfBow_ProjectileTrail",
            "MoltenRockSlingshotProjectileTrail",
            "SpiderBowProjectileTrail Variant",
            "none"
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

        public static bool noclip = false;
        public static float tagAuraDistance = 1.666f;
        public static int tagAuraIndex = 1;

        public static bool lastSlingThing = false;

        public static bool lastInRoom = false;
        public static bool lastMasterClient = false;
        public static string lastRoom = "";

        public static int platformMode = 0;
        public static int platformShape = 0;

        public static bool customSoundOnJoin = false;
        public static float partDelay = 0f;

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
        public static float colorChangerDelay = 0f;
        public static float teleDebounce = 0f;
        public static float splashDel = 0f;
        public static float headspazDelay = 0f;
        public static float autoSaveDelay = Time.time + 60f;

        public static bool isUpdatingValues = false;
        public static float valueChangeDelay = 0f;

        public static bool changingName = false;
        public static bool changingColor = false;
        public static string nameChange = "";

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

        public static int crashAmount = 2;

        public static bool isJoiningRandom = false;

        public static int colorChangeType = 0;
        public static bool strobeColor = false;

        public static bool AntiCrashToggle = false;
        public static bool AntiSoundToggle = false;
        public static bool AntiCheatSelf = false;
        public static bool AntiCheatAll = false;
        public static bool NoGliderRespawn = false;

        public static bool lastHit = false;
        public static bool lastHit2 = false;
        public static bool lastRG;

        public static bool ghostMonke = false;
        public static bool invisMonke = false;

        public static int tindex = 1;

        public static bool antiBanEnabled = false;

        public static bool lastHitL = false;
        public static bool lastHitR = false;
        public static bool lastHitLP = false;
        public static bool lastHitRP = false;
        public static bool lastHitRS = false;

        public static bool plastLeftGrip = false;
        public static bool plastRightGrip = false;
        public static bool spazLavaType = false;

        public static bool EverythingSlippery = false;
        public static bool EverythingGrippy = false;

        public static bool headspazType = false;

        public static bool hasFoundAllBoards = false;

        public static float lastBangTime = 0f;

        public static float subThingy = 0f;

        public static float sizeScale = 1f;

        public static float turnAmnt = 0f;
        public static float TagAuraDelay = 0f;
        public static float startX = -1f;

        public static bool lowercaseMode = false;

        public static bool annoyingMode = false; // build with this enabled for a surprise

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