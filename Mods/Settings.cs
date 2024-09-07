using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Settings
    {
        public static void Search() // This took me like 4 hours
        {
            isSearching = !isSearching;
            isPcWhenSearching = isOnPC;
            pageNumber = 0;
            searchText = "";
            lastPressedKeys.Add(KeyCode.Q);
            if (isSearching)
            {
                if (!isPcWhenSearching)
                {
                    if (VRKeyboard == null)
                    {
                        List<GameObject> archiveObjects = new List<GameObject> { };
                        GameObject background = null;
                        VRKeyboard = LoadAsset("keyboard");
                        VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                        VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                        for (int i = 0; i < VRKeyboard.transform.childCount; i++)
                        {
                            GameObject v = VRKeyboard.transform.GetChild(i).gameObject;
                            if (v.name == "Canvas")
                            {
                                for (int j = 0; j < v.transform.childCount; j++)
                                {
                                    Text lol = v.transform.GetChild(j).gameObject.GetComponent<Text>();
                                    lol.color = textColor;
                                }
                            } else
                            {
                                if (v.name == "MenuSpawnPosition")
                                {
                                    menuSpawnPosition = v;
                                } else
                                {
                                    if (v.name == "Background")
                                    {

                                        GradientColorKey[] array = new GradientColorKey[3];
                                        array[0].color = bgColorA;
                                        array[0].time = 0f;
                                        array[1].color = bgColorB;
                                        array[1].time = 0.5f;
                                        array[2].color = bgColorA;
                                        array[2].time = 1f;
                                        ColorChanger colorChanger = v.AddComponent<ColorChanger>();
                                        colorChanger.colors = new Gradient
                                        {
                                            colorKeys = array
                                        };
                                        colorChanger.isRainbow = themeType == 6;
                                        colorChanger.isEpileptic = themeType == 47;
                                        colorChanger.isMonkeColors = themeType == 8;
                                        colorChanger.Start();
                                        background = v;
                                    }
                                    else
                                    {
                                        GradientColorKey[] array = new GradientColorKey[3];
                                        array[0].color = buttonDefaultA;
                                        array[0].time = 0f;
                                        array[1].color = buttonDefaultB;
                                        array[1].time = 0.5f;
                                        array[2].color = buttonDefaultA;
                                        array[2].time = 1f;
                                        ColorChanger colorChanger = v.AddComponent<ColorChanger>();
                                        colorChanger.colors = new Gradient
                                        {
                                            colorKeys = array
                                        };
                                        colorChanger.isRainbow = false;
                                        colorChanger.isEpileptic = false;
                                        colorChanger.isMonkeColors = false;
                                        colorChanger.Start();
                                        v.AddComponent<KeyboardKey>().key = v.name;
                                        v.layer = 2;
                                        archiveObjects.Add(v);
                                    }
                                }
                            }
                        }
                        if (shouldOutline)
                        {
                            foreach (GameObject key in archiveObjects)
                            {
                                OutlineObjNonMenu(key, true);
                            }
                            OutlineObjNonMenu(background, false);
                        }
                    }
                }
                if (lKeyReference == null)
                {
                    lKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    lKeyReference.transform.parent = GorillaTagger.Instance.leftHandTransform;
                    lKeyReference.GetComponent<Renderer>().material.color = bgColorA;
                    lKeyReference.transform.localPosition = pointerOffset;
                    lKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    lKeyCollider = lKeyReference.GetComponent<SphereCollider>();

                    GradientColorKey[] array = new GradientColorKey[3];
                    array[0].color = bgColorA;
                    array[0].time = 0f;
                    array[1].color = bgColorB;
                    array[1].time = 0.5f;
                    array[2].color = bgColorA;
                    array[2].time = 1f;
                    ColorChanger colorChanger = lKeyReference.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.Start();
                }
                if (rKeyReference == null)
                {
                    rKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    rKeyReference.transform.parent = GorillaTagger.Instance.rightHandTransform;
                    rKeyReference.GetComponent<Renderer>().material.color = bgColorA;
                    rKeyReference.transform.localPosition = pointerOffset;
                    rKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    rKeyCollider = rKeyReference.GetComponent<SphereCollider>();

                    GradientColorKey[] array = new GradientColorKey[3];
                    array[0].color = bgColorA;
                    array[0].time = 0f;
                    array[1].color = bgColorB;
                    array[1].time = 0.5f;
                    array[2].color = bgColorA;
                    array[2].time = 1f;
                    ColorChanger colorChanger = rKeyReference.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.isMonkeColors = themeType == 8;
                    colorChanger.Start();
                }
            } else
            {
                if (lKeyReference != null)
                {
                    UnityEngine.Object.Destroy(lKeyReference);
                    lKeyReference = null;
                }
                if (rKeyReference != null)
                {
                    UnityEngine.Object.Destroy(rKeyReference);
                    rKeyReference = null;
                }
                if (VRKeyboard != null)
                {
                    UnityEngine.Object.Destroy(VRKeyboard);
                    VRKeyboard = null;
                }
                if (TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet") && isOnPC)
                {
                    isOnPC = false;
                    TPC.transform.position = TPC.transform.parent.position;
                    TPC.transform.rotation = TPC.transform.parent.rotation;
                }
            }
        }
        
        public static void GlobalReturn()
        {
            NotifiLib.ClearAllNotifications();
            Toggle(Buttons.buttons[buttonsType][0].buttonText);
        }

        public static void EnableSettings()
        {
            buttonsType = 1;
            pageNumber = 0;
        }

        public static void ReturnToMain()
        {
            buttonsType = 0;
            pageNumber = 0;
        }

        public static void EnableMenuSettings()
        {
            buttonsType = 2;
            pageNumber = 0;
        }

        public static void EnableRoomSettings()
        {
            buttonsType = 3;
            pageNumber = 0;
        }

        public static void EnableSafetySettings()
        {
            buttonsType = 28;
            pageNumber = 0;
        }

        public static void EnableMovementSettings()
        {
            buttonsType = 4;
            pageNumber = 0;
        }

        public static void EnableProjectileSettings()
        {
            buttonsType = 5;
            pageNumber = 0;
        }

        public static void EnableRoom()
        {
            buttonsType = 6;
            pageNumber = 0;
        }

        public static void EnableImportant()
        {
            buttonsType = 7;
            pageNumber = 0;
        }

        public static void EnableSafety()
        {
            buttonsType = 8;
            pageNumber = 0;
        }

        public static void EnableMovement()
        {
            buttonsType = 9;
            pageNumber = 0;
        }

        public static void EnableAdvantage()
        {
            buttonsType = 10;
            pageNumber = 0;
        }

        public static void EnableVisual()
        {
            buttonsType = 11;
            pageNumber = 0;
        }

        public static void EnableFun()
        {
            buttonsType = 12;
            pageNumber = 0;
        }

        public static void EnableSpam()
        {
            buttonsType = 13;
            pageNumber = 0;
        }

        public static void EnableSoundSpam()
        {
            buttonsType = 14;
            pageNumber = 0;
        }

        public static void EnableProjectileSpam()
        {
            buttonsType = 15;
            pageNumber = 0;
        }

        public static void EnableMaster()
        {
            buttonsType = 16;
            pageNumber = 0;
        }

        public static void EnableOverpowered()
        {
            buttonsType = 17;
            pageNumber = 0;
        }

        public static void EnableExperimental()
        {
            buttonsType = 18;
            pageNumber = 0;
        }

        public static void EnableRisky()
        {
            buttonsType = 26;
            pageNumber = 0;
        }

        public static void EnableBuggy()
        {
            buttonsType = 27;
            pageNumber = 0;
        }

        public static void EnableFavorites()
        {
            buttonsType = 19;
            pageNumber = 0;
        }

        public static void EnableMenuPresets()
        {
            buttonsType = 20;
            pageNumber = 0;
        }

        public static void EnableAdvantageSettings()
        {
            buttonsType = 21;
            pageNumber = 0;
        }

        public static void EnableVisualSettings()
        {
            buttonsType = 22;
            pageNumber = 0;
        }

        public static void EnableAdmin()
        {
            buttonsType = 23;
            pageNumber = 0;
        }

        public static void EnableEnabled()
        {
            buttonsType = 24;
            pageNumber = 0;
        }

        public static void RightHand()
        {
            rightHand = true;
            if (wristThingV2)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotifiLib.ClearAllNotifications();
            }
            if (GetIndex("Info Watch").enabled)
            {
                Toggle("Info Watch");
                Toggle("Info Watch");
                NotifiLib.ClearAllNotifications();
            }
        }

        public static void LeftHand()
        {
            rightHand = false;
            if (wristThingV2)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotifiLib.ClearAllNotifications();
            }
            if (GetIndex("Info Watch").enabled)
            {
                Toggle("Info Watch");
                Toggle("Info Watch");
                NotifiLib.ClearAllNotifications();
            }
        }

        public static void BothHandsOn()
        {
            bothHands = true;
        }

        public static void BothHandsOff()
        {
            bothHands = false;
        }

        public static void WristThingOn()
        {
            wristThing = true;
        }

        public static void WristThingOff()
        {
            wristThing = false;
            wristOpen = false;
        }

        public static void JoystickMenuOn()
        {
            joystickMenu = true;
        }

        public static void JoystickMenuOff()
        {
            joystickMenu = false;
            joystickOpen = false;
        }

        public static void OutlineMenuOn()
        {
            shouldOutline = true;
        }

        public static void OutlineMenuOff()
        {
            shouldOutline = false;
        }

        public static void WatchMenuOn()
        {
            wristThingV2 = true;
            GameObject mainwatch = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            watchobject = UnityEngine.Object.Instantiate(mainwatch,rightHand ? GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false); // See cause unlike skid.lol I actually clone the watch
            UnityEngine.Object.Destroy(watchobject.GetComponent<GorillaHuntComputer>());
            watchobject.SetActive(true);

            Transform thething = watchobject.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
            thething.Find("Hat").gameObject.SetActive(false);
            thething.Find("Face").gameObject.SetActive(false);
            thething.Find("Badge").gameObject.SetActive(false);
            thething.Find("Material").gameObject.SetActive(false);
            thething.Find("Right Hand").gameObject.SetActive(false);

            watchText = thething.Find("Text").gameObject;
            //watchText.GetComponent<Text>().horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
            watchEnabledIndicator = thething.Find("Left Hand").gameObject;
            watchShell = watchobject.transform.Find("HuntWatch_ScreenLocal").gameObject;

            watchShell.GetComponent<Renderer>().material = OrangeUI;

            if (rightHand)
            {
                watchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
                watchShell.transform.parent.localPosition += new Vector3(0.025f, 0f, 0f);
                watchShell.transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
            }
        }

        public static void WatchMenuOff()
        {
            wristThingV2 = false;
            UnityEngine.Object.Destroy(watchobject);
        }

        public static void ShinyMenu()
        {
            shinymenu = true;
        }

        public static void NoShinyMenu()
        {
            shinymenu = false;
        }

        public static void LongMenuOn()
        {
            longmenu = true;
        }

        public static void LongMenuOff()
        {
            longmenu = false;
        }

        public static void DropMenu()
        {
            dropOnRemove = false;
        }

        public static void DropMenuOff()
        {
            dropOnRemove = true;
        }

        public static void FlipMenu()
        {
            flipMenu = true;
        }

        public static void NonFlippedMenu()
        {
            flipMenu = false;
        }

        public static void ChangeMenuTheme()
        {
            themeType++;
            if (themeType > 51)
            {
                themeType = 1;
            }

            switch (themeType) {
                case 1: // Orange
                    bgColorA = new Color32(255, 128, 0, 128);
                    bgColorB = new Color32(255, 102, 0, 128);
                    buttonDefaultA = new Color32(170, 85, 0, 255);
                    buttonDefaultB = new Color32(170, 85, 0, 255);
                    buttonClickedA = new Color32(85, 42, 0, 255);
                    buttonClickedB = new Color32(85, 42, 0, 255);
                    titleColor = new Color32(255, 190, 125, 255);
                    textColor = new Color32(255, 190, 125, 255);
                    textClicked = new Color32(255, 190, 125, 255);
                    break;
                case 2: // Blue Magenta
                    bgColorA = Color.blue;
                    bgColorB = Color.magenta;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.blue;
                    buttonClickedB = Color.blue;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 3: // Dark Mode
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = new Color32(50, 50, 50, 255);
                    buttonDefaultB = new Color32(50, 50, 50, 255);
                    buttonClickedA = new Color32(20, 20, 20, 255);
                    buttonClickedB = new Color32(20, 20, 20, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 4: // Strobe
                    bgColorA = Color.white;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.white;
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 5: // Kman
                    bgColorA = Color.black;
                    bgColorB = new Color32(110, 0, 0, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = new Color32(110, 0, 0, 255);
                    buttonClickedA = new Color32(110, 0, 0, 255);
                    buttonClickedB = new Color32(110, 0, 0, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 6: // Rainbow
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 7: // Cone
                    bgColorA = new Color32(255, 128, 0, 128);
                    bgColorB = new Color32(255, 128, 0, 128);
                    buttonDefaultA = new Color32(170, 85, 0, 255);
                    buttonDefaultB = new Color32(170, 85, 0, 255);
                    buttonClickedA = new Color32(85, 42, 0, 255);
                    buttonClickedB = new Color32(85, 42, 0, 255);
                    titleColor = new Color32(255, 190, 125, 255);
                    textColor = new Color32(255, 190, 125, 255);
                    textClicked = new Color32(255, 190, 125, 255);
                    GetIndex("Thin Menu").enabled = true;
                    FATMENU = true;
                    break;
                case 8: // Player Material
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 9: // Lava
                    bgColorA = Color.black;
                    bgColorB = new Color32(255, 111, 0, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(255, 111, 0, 255);
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 10: // Rock
                    bgColorA = Color.black;
                    bgColorB = Color.red;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 11: // Ice
                    bgColorA = Color.black;
                    bgColorB = new Color32(0, 174, 255, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(0, 174, 255, 255);
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 12: // Water
                    bgColorA = new Color32(0, 136, 255, 255);
                    bgColorB = new Color32(0, 174, 255, 255);
                    buttonDefaultA = new Color32(0, 100, 188, 255);
                    buttonDefaultB = new Color32(0, 100, 188, 255);
                    buttonClickedA = new Color32(0, 174, 255, 255);
                    buttonClickedB = new Color32(0, 136, 255, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 13: // Minty
                    bgColorA = new Color32(0, 255, 246, 255);
                    bgColorB = new Color32(0, 255, 144, 255);
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = new Color32(0, 255, 144, 255);
                    buttonClickedB = new Color32(0, 255, 246, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 14: // Pink
                    bgColorA = new Color32(255, 130, 255, 255);
                    bgColorB = Color.white;
                    buttonDefaultA = new Color32(255, 130, 255, 255);
                    buttonDefaultB = new Color32(255, 130, 255, 255);
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 15: // Purple
                    bgColorA = new Color32(122, 35, 159, 255);
                    bgColorB = new Color32(60, 26, 89, 255);
                    buttonDefaultA = new Color32(60, 26, 89, 255);
                    buttonDefaultB = new Color32(60, 26, 89, 255);
                    buttonClickedA = new Color32(122, 35, 159, 255);
                    buttonClickedB = new Color32(122, 35, 159, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 16: // Magenta Cyan
                    bgColorA = Color.magenta;
                    bgColorB = Color.cyan;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.magenta;
                    buttonClickedB = Color.cyan;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.black;
                    break;
                case 17: // Red Fade
                    bgColorA = Color.red;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.red;
                    titleColor = Color.red;
                    textColor = Color.red;
                    textClicked = Color.black;
                    break;
                case 18: // Orange Fade
                    bgColorA = new Color32(255, 128, 0, 255);
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(255, 128, 0, 255);
                    buttonClickedB = new Color32(255, 128, 0, 255);
                    titleColor = new Color32(255, 128, 0, 255);
                    textColor = new Color32(255, 128, 0, 255);
                    textClicked = Color.black;
                    break;
                case 19: // Yellow Fade
                    bgColorA = Color.yellow;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.yellow;
                    buttonClickedB = Color.yellow;
                    titleColor = Color.yellow;
                    textColor = Color.yellow;
                    textClicked = Color.black;
                    break;
                case 20: // Green Fade
                    bgColorA = Color.green;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.green;
                    textColor = Color.green;
                    textClicked = Color.black;
                    break;
                case 21: // Blue Fade
                    bgColorA = Color.blue;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.blue;
                    buttonClickedB = Color.blue;
                    titleColor = Color.blue;
                    textColor = Color.blue;
                    textClicked = Color.black;
                    break;
                case 22: // Blurple Fade
                    bgColorA = new Color32(119, 0, 255, 255);
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(119, 0, 255, 255);
                    buttonClickedB = new Color32(119, 0, 255, 255);
                    titleColor = new Color32(119, 0, 255, 255);
                    textColor = new Color32(119, 0, 255, 255);
                    textClicked = Color.black;
                    break;
                case 23: // Magenta Fade
                    bgColorA = Color.magenta;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.magenta;
                    buttonClickedB = Color.magenta;
                    titleColor = Color.magenta;
                    textColor = Color.magenta;
                    textClicked = Color.black;
                    break;
                case 24: // Banana
                    bgColorA = new Color32(255, 255, 130, 255);
                    bgColorB = Color.white;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = new Color32(255, 255, 130, 255);
                    buttonClickedB = new Color32(255, 255, 130, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 25: // Pride
                    bgColorA = Color.red;
                    bgColorB = Color.green;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.white;
                    textClicked = Color.black;
                    break;
                case 26: // Trans
                    bgColorA = new Color32(245, 169, 184, 255);
                    bgColorB = new Color32(91, 206, 250, 255);
                    buttonDefaultA = new Color32(245, 169, 184, 255);
                    buttonDefaultB = new Color32(245, 169, 184, 255);
                    buttonClickedA = new Color32(91, 206, 250, 255);
                    buttonClickedB = new Color32(91, 206, 250, 255);
                    titleColor = new Color32(91, 206, 250, 255);
                    textColor = new Color32(91, 206, 250, 255);
                    textClicked = new Color32(245, 169, 184, 255);
                    break;
                case 27: // MLM or Gay
                    bgColorA = new Color32(7, 141, 112, 255);
                    bgColorB = new Color32(61, 26, 220, 255);
                    buttonDefaultA = new Color32(7, 141, 112, 255);
                    buttonDefaultB = new Color32(7, 141, 112, 255);
                    buttonClickedA = new Color32(61, 26, 220, 255);
                    buttonClickedB = new Color32(61, 26, 220, 255);
                    titleColor = new Color32(61, 26, 220, 255);
                    textColor = new Color32(61, 26, 220, 255);
                    textClicked = new Color32(7, 141, 112, 255);
                    break;
                case 28: // Steal (old)
                    bgColorA = new Color32(50, 50, 50, 255);
                    bgColorB = new Color32(50, 50, 50, 255);
                    buttonDefaultA = new Color32(50, 50, 50, 255);
                    buttonDefaultB = new Color32(50, 50, 50, 255);
                    buttonClickedA = new Color32(75, 75, 75, 255);
                    buttonClickedB = new Color32(75, 75, 75, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 29: // Silence
                    bgColorA = Color.black;
                    bgColorB = new Color32(80, 0, 80, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.green;
                    break;
                case 30: // Transparent
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.green;
                    break;
                case 31: // King
                    bgColorA = new Color32(100, 60, 170, 255);
                    bgColorB = new Color32(100, 60, 170, 255);
                    buttonDefaultA = new Color32(150, 100, 240, 255);
                    buttonDefaultB = new Color32(150, 100, 240, 255);
                    buttonClickedA = new Color32(150, 100, 240, 255);
                    buttonClickedB = new Color32(150, 100, 240, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.cyan;
                    break;
                case 32: // Scoreboard
                    bgColorA = new Color32(0, 53, 2, 255);
                    bgColorB = new Color32(0, 53, 2, 255);
                    buttonDefaultA = new Color32(192, 190, 171, 255);
                    buttonDefaultB = new Color32(192, 190, 171, 255);
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.red;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 33: // Scoreboard (banned)
                    bgColorA = new Color32(225, 73, 43, 255);
                    bgColorB = new Color32(225, 73, 43, 255);
                    buttonDefaultA = new Color32(192, 190, 171, 255);
                    buttonDefaultB = new Color32(192, 190, 171, 255);
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.red;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 34: // Rift
                    bgColorA = new Color32(25, 25, 25, 255);
                    bgColorB = new Color32(25, 25, 25, 255);
                    buttonDefaultA = new Color32(40, 40, 40, 255);
                    buttonDefaultB = new Color32(40, 40, 40, 255);
                    buttonClickedA = new Color32(167, 66, 191, 255);
                    buttonClickedB = new Color32(167, 66, 191, 255);
                    titleColor = new Color32(144, 144, 144, 255);
                    textColor = new Color32(144, 144, 144, 255);
                    textClicked = Color.white;
                    break;
                case 35: // Blurple Dark
                    bgColorA = new Color32(26, 26, 61, 255);
                    bgColorB = new Color32(26, 26, 61, 255);
                    buttonDefaultA = new Color32(26, 26, 61, 255);
                    buttonDefaultB = new Color32(26, 26, 61, 255);
                    buttonClickedA = new Color32(43, 17, 84, 255);
                    buttonClickedB = new Color32(43, 17, 84, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 36: // ShibaGT Gold
                    bgColorA = Color.black;
                    bgColorB = Color.gray;
                    buttonDefaultA = Color.yellow;
                    buttonDefaultB = Color.yellow;
                    buttonClickedA = Color.magenta;
                    buttonClickedB = Color.magenta;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 37: // ShibaGT Genesis
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = new Color32(32, 32, 32, 255);
                    buttonDefaultB = new Color32(32, 32, 32, 255);
                    buttonClickedA = new Color32(32, 32, 32, 255);
                    buttonClickedB = new Color32(32, 32, 32, 255);
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.white;
                    break;
                case 38: // wyvern
                    bgColorA = new Color32(199, 115, 173, 255);
                    bgColorB = new Color32(165, 233, 185, 255);
                    buttonDefaultA = new Color32(99, 58, 86, 255);
                    buttonDefaultB = new Color32(83, 116, 92, 255);
                    buttonClickedA = new Color32(99, 58, 86, 255);
                    buttonClickedB = new Color32(83, 116, 92, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.green;
                    break;
                case 39: // Steal (new)
                    bgColorA = new Color32(27, 27, 27, 255);
                    bgColorB = new Color32(27, 27, 27, 255);
                    buttonDefaultA = new Color32(50, 50, 50, 255);
                    buttonDefaultB = new Color32(50, 50, 50, 255);
                    buttonClickedA = new Color32(66, 66, 66, 255);
                    buttonClickedB = new Color32(66, 66, 66, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 40: // USA Menu (lol)
                    bgColorA = Color.black;
                    bgColorB = new Color32(100, 25, 125, 255);
                    buttonDefaultA = new Color32(25, 25, 25, 255);
                    buttonDefaultB = new Color32(25, 25, 25, 255);
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 41: // Watch
                    bgColorA = new Color32(27, 27, 27, 255);
                    bgColorB = new Color32(27, 27, 27, 255);
                    buttonDefaultA = Color.red;
                    buttonDefaultB = Color.red;
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 42: // AZ Menu
                    bgColorA = Color.black;
                    bgColorB = new Color32(100, 0, 0, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(100, 0, 0, 255);
                    buttonClickedB = new Color32(100, 0, 0, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 43: // ImGUI
                    bgColorA = new Color32(21, 22, 23, 255);
                    bgColorB = new Color32(21, 22, 23, 255);
                    buttonDefaultA = new Color32(32, 50, 77, 255);
                    buttonDefaultB = new Color32(32, 50, 77, 255);
                    buttonClickedA = new Color32(60, 127, 206, 255);
                    buttonClickedB = new Color32(60, 127, 206, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 44: // Clean Dark
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(10, 10, 10, 255);
                    buttonClickedB = new Color32(10, 10, 10, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 45: // Discord Light Mode (lmfao)
                    bgColorA = Color.white;
                    bgColorB = Color.white;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = new Color32(245, 245, 245, 255);
                    buttonClickedB = new Color32(245, 245, 245, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 46: // The Hub
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = new Color32(255, 163, 26, 255);
                    buttonDefaultB = new Color32(255, 163, 26, 255);
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.white;
                    break;
                case 47: // EPILEPTIC
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 48: // Discord Blurple
                    bgColorA = new Color32(111, 143, 255, 255);
                    bgColorB = new Color32(163, 184, 255, 255);
                    buttonDefaultA = new Color32(96, 125, 219, 255);
                    buttonDefaultB = new Color32(96, 125, 219, 255);
                    buttonClickedA = new Color32(147, 167, 226, 255);
                    buttonClickedB = new Color32(147, 167, 226, 255);
                    titleColor = new Color32(33, 33, 101, 255);
                    textColor = new Color32(33, 33, 101, 255);
                    textClicked = new Color32(33, 33, 101, 255);
                    break;
                case 49: // VS Zero
                    bgColorA = new Color32(19, 22, 27, 255);
                    bgColorB = new Color32(19, 22, 27, 255);
                    buttonDefaultA = new Color32(19, 22, 27, 255);
                    buttonDefaultB = new Color32(19, 22, 27, 255);
                    buttonClickedA = new Color32(16, 18, 22, 255);
                    buttonClickedB = new Color32(16, 18, 22, 255);
                    titleColor = new Color32(82, 96, 122, 255);
                    textColor = new Color32(82, 96, 122, 255);
                    textClicked = new Color32(82, 96, 122, 255);
                    break;
                case 50: // Weed theme (for v4.2.0) (also 50th theme)
                    bgColorA = new Color32(0, 136, 16, 255);
                    bgColorB = new Color32(0, 127, 14, 255);
                    buttonDefaultA = new Color32(0, 158, 15, 255);
                    buttonDefaultB = new Color32(0, 158, 15, 255);
                    buttonClickedA = new Color32(0, 112, 11, 255);
                    buttonClickedB = new Color32(0, 112, 11, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 51: // Pastel Rainbow
                    bgColorA = Color.white;
                    bgColorB = Color.white;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
            }
        }

        public static string fileData = null;
        public static void CustomMenuTheme()
        {
            bool loadedThisTime = false;
            if (fileData == null)
            {
                if (!Directory.Exists("iisStupidMenu"))
                {
                    Directory.CreateDirectory("iisStupidMenu");
                }
                if (!File.Exists("iisStupidMenu/iiMenu_CustomThemeColor.txt"))
                {
                    File.WriteAllText("iisStupidMenu/iiMenu_CustomThemeColor.txt", "255,128,0\n0,0,0\n255,0,0\n255,0,0\n0,255,0\n0,255,0\n255,255,255\n0,0,255\n255,0,255");
                }

                fileData = File.ReadAllText("iisStupidMenu/iiMenu_CustomThemeColor.txt");
                loadedThisTime = true;
            }
            string[] linesplit = fileData.Split("\n");

            // God DAMN
            string[] a = linesplit[0].Split(",");
            bgColorA = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[1].Split(",");
            bgColorB = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);

            a = linesplit[2].Split(",");
            buttonDefaultA = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[3].Split(",");
            buttonDefaultB = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);

            a = linesplit[4].Split(",");
            buttonClickedA = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[5].Split(",");
            buttonClickedB = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);

            a = linesplit[6].Split(",");
            titleColor = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[7].Split(",");
            textColor = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[8].Split(",");
            textClicked = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            if (loadedThisTime)
            {
                ReloadMenu();
            }
        }

        public static void CustomMenuBackground()
        {
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            if (!File.Exists("iisStupidMenu/iiMenu_CustomMenuBackground.txt"))
            {
                File.WriteAllText("iisStupidMenu/iiMenu_CustomMenuBackground.txt", "");
            }

            if (File.Exists("iisStupidMenu/MenuBG.png"))
            {
                File.Delete("iisStupidMenu/MenuBG.png");
            }
            doCustomMenuBackground = true;
            customMenuBackgroundImage = LoadTextureFromURL(File.ReadAllText("iisStupidMenu/iiMenu_CustomMenuBackground.txt"), "MenuBG.png");
            ReloadMenu();
        }

        public static void FixMenuBackground()
        {
            customMenuBackgroundImage = null;
            doCustomMenuBackground = false;
        }

        public static void FixTheme()
        {
            themeType--;
            ChangeMenuTheme();
            fileData = null;
        }

        public static void ChangePageType() // Say goodbye to daily lagspikes and dirty if spam with new else
        {
            pageButtonType++;
            if (pageButtonType > 6)
            {
                pageButtonType = 1;
            }

            if (pageButtonType == 1)
            {
                pageSize = 6;
                buttonOffset = 2;
            }
            else
            {
                pageSize = 8;
                buttonOffset = 0;
            }
        }

        public static void ChangeArrowType()
        {
            arrowType++;
            if (arrowType > arrowTypes.Length - 1)
            {
                arrowType = 0;
            }
        }

        public static void ChangeFontType() // Say goodbye to daily lagspikes and dirty if spam with new switch (x) { }
        {
            fontCycle++;
            if (fontCycle > 8)
            {
                fontCycle = 0;
            }

            switch (fontCycle) {
                case 0:
                    activeFont = agency;
                    return;
                case 1:
                    activeFont = Arial;
                    return;
                case 2:
                    activeFont = Verdana;
                    return;
                case 3:
                    if (gtagfont == null)
                    {
                        GameObject fart = LoadAsset("gtag");
                        gtagfont = fart.transform.Find("text").gameObject.GetComponent<Text>().font;
                        UnityEngine.Object.Destroy(fart);
                    }
                    activeFont = gtagfont;
                    return;
                case 4:
                    activeFont = sans;
                    return;
                case 5:
                    activeFont = consolas;
                    return;
                case 6:
                    activeFont = ubuntu;
                    return;
                case 7:
                    activeFont = MSGOTHIC;
                    return;
                case 8:
                    activeFont = impact;
                    return;
            }
        }

        public static void ChangeFontStyleType()
        {
            fontStyleType++;
            if (fontStyleType > 3)
            {
                fontStyleType = 0;
            }

            activeFontStyle = (FontStyle)fontStyleType;
        }

        public static void ChangePCUI()
        {
            pcbg++;
            if (pcbg > 3)
            {
                pcbg = 0;
            }
        }

        public static void ChangeNotificationTime()
        {
            notificationDecayTime += 1000;
            if (notificationDecayTime > 5500)
            {
                notificationDecayTime = 1000;
            }
            GetIndex("Change Notification Time").overlapText = "Change Notification Time <color=grey>[</color><color=green>" + (notificationDecayTime / 1000).ToString() + "</color><color=grey>]</color>";
        }

        public static void ChangePointerPosition()
        {
            pointerIndex++;
            if (pointerIndex > 3)
            {
                pointerIndex = 0;
            }

            Vector3[] pointerPos = new Vector3[]
            {
                new Vector3(0f, -0.1f, 0f),
                new Vector3(0f, -0.1f, -0.15f),
                new Vector3(0f, 0.1f, -0.05f),
                new Vector3(0f, 0.0666f, 0.1f)
            };
            pointerOffset = pointerPos[pointerIndex];
            try { reference.transform.localPosition = pointerOffset; } catch { }
        }

        public static void SmallGunPointer()
        {
            smallGunPointer = true;
        }

        public static void BigGunPointer()
        {
            smallGunPointer = false;
        }

        public static void NoGunPointer()
        {
            disableGunPointer = true;
        }

        public static void YesGunPointer()
        {
            disableGunPointer = false;
        }

        public static void NoGunLine()
        {
            disableGunLine = true;
        }

        public static void YesGunLine()
        {
            disableGunLine = false;
        }

        public static void LegacyGunDirection()
        {
            legacyGunDirection = true;
        }

        public static void NewGunDirection()
        {
            legacyGunDirection = false;
        }

        public static void FreezePlayerInMenu()
        {
            if (menu != null)
            {
                if (closePosition == Vector3.zero)
                {
                    closePosition = GorillaTagger.Instance.rigidbody.transform.position;
                } else
                {
                    GorillaTagger.Instance.rigidbody.transform.position = closePosition;
                }
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            } else
            {
                closePosition = Vector3.zero;
            }
        }

        public static void FreezePlayerInMenuEnabled()
        {
            closePosition = GorillaTagger.Instance.rigidbody.transform.position;
        }

        public static bool currentmentalstate = false;
        public static void FreezeRigInMenu()
        {
            if (menu != null)
            {
                if (!currentmentalstate)
                {
                    currentmentalstate = true;
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                }
            }
            else
            {
                if (currentmentalstate)
                {
                    currentmentalstate = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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
                        {
                            Buttons.buttons[0] = Buttons.buttons[0].Concat(new[] { buttonArray[i] }).ToArray();
                        }

                        Array.Clear(buttonArray, 0, buttonArray.Length);
                    }
                }
            }
        }

        public static void AnnoyingModeOn()
        {
            annoyingMode = true;
        }

        public static void AnnoyingModeOff()
        {
            annoyingMode = false;
            themeType--;
            ChangeMenuTheme();
        }

        public static void LowercaseMode()
        {
            lowercaseMode = true;
        }

        public static void NoLowercaseMode()
        {
            lowercaseMode = false;
        }

        public static void OverflowMode()
        {
            NoAutoSizeText = true;
        }

        public static void NoOverflowMode()
        {
            NoAutoSizeText = false;
        }

        public static void EnablePrimaryRoomMods()
        {
            GetIndex("Disconnect").isTogglable = true;
            GetIndex("Reconnect").isTogglable = true;
            GetIndex("Join Random").isTogglable = true;
        }

        public static void DisablePrimaryRoomMods()
        {
            GetIndex("Disconnect").enabled = false;
            GetIndex("Reconnect").enabled = false;
            GetIndex("Join Random").enabled = false;

            GetIndex("Disconnect").isTogglable = false;
            GetIndex("Reconnect").isTogglable = false;
            GetIndex("Join Random").isTogglable = false;
        }

        public static void DisableNotifications()
        {
            disableNotifications = true;
        }

        public static void EnableNotifications()
        {
            disableNotifications = false;
        }

        public static void DisableEnabledGUI()
        {
            showEnabledModsVR = false;
        }

        public static void EnableEnabledGUI()
        {
            showEnabledModsVR = true;
        }

        public static void DisableDisconnectButton()
        {
            disableDisconnectButton = true;
        }

        public static void EnableDisconnectButton()
        {
            disableDisconnectButton = false;
        }

        public static void DisableSearchButton()
        {
            disableSearchButton = true;
        }

        public static void EnableSearchButton()
        {
            disableSearchButton = false;
        }

        public static void DisableReturnButton()
        {
            disableReturnButton = true;
        }

        public static void EnableReturnButton()
        {
            disableReturnButton = false;
        }

        public static void DisablePageButtons()
        {
            if (GetIndex("Joystick Menu").enabled) {
                disablePageButtons = true;
            } else
            {
                GetIndex("Disable Page Buttons").enabled = false;
                NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> <color=white>Disable Page Buttons can only be used when using Joystick Menu.</color>");
            }
        }

        public static void EnablePageButtons()
        {
            disablePageButtons = false;
        }

        public static void DisablePageText()
        {
            noPageNumber = true;
        }

        public static void EnablePageText()
        {
            noPageNumber = false;
        }

        public static void CustomMenuName()
        {
            doCustomName = true;
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            if (!File.Exists("iisStupidMenu/iiMenu_CustomMenuName.txt"))
            {
                File.WriteAllText("iisStupidMenu/iiMenu_CustomMenuName.txt", "Your Text Here");
            }
            customMenuName = File.ReadAllText("iisStupidMenu/iiMenu_CustomMenuName.txt");
        }

        public static void NoCustomMenuName()
        {
            doCustomName = false;
        }

        public static void NoGlobalSearch()
        {
            nonGlobalSearch = true;
        }

        public static void PleaseGlobalSearch()
        {
            nonGlobalSearch = false;
        }

        public static void DisableFPSCounter()
        {
            disableFpsCounter = true;
        }

        public static void EnableFPSCounter()
        {
            disableFpsCounter = false;
        }

        public static string SavePreferencesToText()
        {
            string seperator = ";;";
            string text = "";
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.enabled && v.buttonText != "Save Preferences")
                    {
                        if (text == "")
                        {
                            text += v.buttonText;
                        }
                        else
                        {
                            text += seperator + v.buttonText;
                        }
                    }
                }
            }

            string favz = "";
            foreach (string fav in favorites)
            {
                if (favz == "")
                {
                    favz += fav;
                }
                else
                {
                    favz += seperator + fav;
                }
            }

            string ihateyouguys = platformMode + seperator + platformShape + seperator + flySpeedCycle + seperator + longarmCycle + seperator + speedboostCycle + seperator + projmode + seperator + trailmode + seperator + shootCycle + seperator + pointerIndex + seperator + tagAuraIndex + seperator + notificationDecayTime + seperator + fontStyleType + seperator + arrowType + seperator + pcbg + seperator + internetTime + seperator + hotkeyButton + seperator + buttonClickIndex + seperator + buttonClickVolume + seperator + Safety.antireportrangeindex + seperator + Advantages.tagRangeIndex;

            string finaltext =
                text + "\n" +
                favz + "\n" +
                ihateyouguys + "\n" +
                pageButtonType.ToString() + "\n" +
                themeType.ToString() + "\n" +
                fontCycle.ToString();

            return finaltext;
        }

        public static void SavePreferences()
        {
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            File.WriteAllText("iisStupidMenu/iiMenu_Preferences.txt", SavePreferencesToText());
        }

        public static void LegacyLoadPreferences()
        {
            Panic();
            try
            {
                string config = File.ReadAllText("iisStupidMenu/iiMenu_EnabledMods.txt");
                string[] activebuttons = config.Split("\n");
                for (int index = 0; index < activebuttons.Length; index++)
                {
                    Toggle(activebuttons[index]);
                }
            }
            catch { }

            try
            {
                string favez = File.ReadAllText("iisStupidMenu/iiMenu_FavoriteMods.txt");
                string[] favz = favez.Split("\n");

                favorites.Clear();
                foreach (string fav in favz)
                {
                    favorites.Add(fav);
                }
            }
            catch { }

            try
            {
                string MODDER = File.ReadAllText("iisStupidMenu/iiMenu_ModData.txt");
                string[] data = MODDER.Split("\n");

                platformMode = int.Parse(data[0]) - 1;
                Movement.ChangePlatformType();
                platformShape = int.Parse(data[1]) - 1;
                Movement.ChangePlatformShape();
                flySpeedCycle = int.Parse(data[2]) - 1;
                Movement.ChangeFlySpeed();
                longarmCycle = int.Parse(data[3]) - 1;
                Movement.ChangeArmLength();
                speedboostCycle = int.Parse(data[4]) - 1;
                Movement.ChangeSpeedBoostAmount();
                projmode = int.Parse(data[5]) - 1;
                Projectiles.ChangeProjectile();
                trailmode = int.Parse(data[6]) - 1;
                Projectiles.ChangeTrail();
                shootCycle = int.Parse(data[7]) - 1;
                Projectiles.ChangeShootSpeed();
                pointerIndex = int.Parse(data[8]) - 1;
                ChangePointerPosition();
                tagAuraIndex = int.Parse(data[9]) - 1;
                Advantages.ChangeTagAuraRange();
                notificationDecayTime = int.Parse(data[10]) - 1000;
                ChangeNotificationTime();
                fontStyleType = int.Parse(data[11]) - 1;
                Settings.ChangeFontStyleType();
                arrowType = int.Parse(data[12]) - 1;
                Settings.ChangeArrowType();
                pcbg = int.Parse(data[13]) - 1;
                Settings.ChangePCUI();
                internetTime = int.Parse(data[14]) - 1;
                Settings.ChangeReconnectTime();
                hotkeyButton = data[15];
                buttonClickIndex = int.Parse(data[16]) - 1;
                Settings.ChangeButtonSound();
                buttonClickVolume = int.Parse(data[17]) - 1;
                Settings.ChangeButtonVolume();
                Safety.antireportrangeindex = int.Parse(data[18]) - 1;
                Safety.ChangeAntiReportRange();
            }
            catch { }

            string pager = File.ReadAllText("iisStupidMenu/iiMenu_PageType.txt");
            string themer = File.ReadAllText("iisStupidMenu/iiMenu_Theme.txt");
            string fonter = File.ReadAllText("iisStupidMenu/iiMenu_Font.txt");

            pageButtonType = int.Parse(pager) - 1;
            Toggle("Change Page Type");
            themeType = int.Parse(themer) - 1;
            Toggle("Change Menu Theme");
            fontCycle = int.Parse(fonter) - 1;
            Toggle("Change Font Type");
            NotifiLib.ClearAllNotifications();
        }

        public static void LoadPreferencesFromText(string text)
        {
            Panic();
            string[] textData = text.Split("\n");

            string[] activebuttons = textData[0].Split(";;");
            for (int index = 0; index < activebuttons.Length; index++)
            {
                Toggle(activebuttons[index]);
            }

            string[] favz = textData[1].Split(";;");
            favorites.Clear();
            foreach (string fav in favz)
            {
                favorites.Add(fav);
            }

            try
            {
                string[] data = textData[2].Split(";;");
                platformMode = int.Parse(data[0]) - 1;
                Movement.ChangePlatformType();
                platformShape = int.Parse(data[1]) - 1;
                Movement.ChangePlatformShape();
                flySpeedCycle = int.Parse(data[2]) - 1;
                Movement.ChangeFlySpeed();
                longarmCycle = int.Parse(data[3]) - 1;
                Movement.ChangeArmLength();
                speedboostCycle = int.Parse(data[4]) - 1;
                Movement.ChangeSpeedBoostAmount();
                projmode = int.Parse(data[5]) - 1;
                Projectiles.ChangeProjectile();
                trailmode = int.Parse(data[6]) - 1;
                Projectiles.ChangeTrail();
                shootCycle = int.Parse(data[7]) - 1;
                Projectiles.ChangeShootSpeed();
                pointerIndex = int.Parse(data[8]) - 1;
                ChangePointerPosition();
                tagAuraIndex = int.Parse(data[9]) - 1;
                Advantages.ChangeTagAuraRange();
                notificationDecayTime = int.Parse(data[10]) - 1000;
                ChangeNotificationTime();
                fontStyleType = int.Parse(data[11]) - 1;
                ChangeFontStyleType();
                arrowType = int.Parse(data[12]) - 1;
                ChangeArrowType();
                pcbg = int.Parse(data[13]) - 1;
                ChangePCUI();
                internetTime = int.Parse(data[14]) - 1;
                ChangeReconnectTime();
                hotkeyButton = data[15];
                buttonClickIndex = int.Parse(data[16]) - 1;
                ChangeButtonSound();
                buttonClickVolume = int.Parse(data[17]) - 1;
                ChangeButtonVolume();
                Safety.antireportrangeindex = int.Parse(data[18]) - 1;
                Safety.ChangeAntiReportRange();
                Advantages.tagRangeIndex = int.Parse(data[19]) - 1;
                Advantages.ChangeTagReachDistance();
            } catch { UnityEngine.Debug.Log("Save file out of date"); }

            pageButtonType = int.Parse(textData[3]) - 1;
            Toggle("Change Page Type");
            themeType = int.Parse(textData[4]) - 1;
            Toggle("Change Menu Theme");
            fontCycle = int.Parse(textData[5]) - 1;
            Toggle("Change Font Type");

            NotifiLib.ClearAllNotifications();
            hasLoadedPreferences = true;
        }

        public static void LoadPreferences()
        {
            try
            {
                if (Directory.Exists("iisStupidMenu"))
                {
                    if (File.Exists("iisStupidMenu/iiMenu_EnabledMods.txt"))
                    {
                        LegacyLoadPreferences();
                        File.Delete("iisStupidMenu/iiMenu_EnabledMods.txt");
                        File.Delete("iisStupidMenu/iiMenu_FavoriteMods.txt");
                        File.Delete("iisStupidMenu/iiMenu_ModData.txt");
                        File.Delete("iisStupidMenu/iiMenu_PageType.txt");
                        File.Delete("iisStupidMenu/iiMenu_Theme.txt");
                        File.Delete("iisStupidMenu/iiMenu_Font.txt");
                        SavePreferences();
                    }
                    else
                    {
                        string text = File.ReadAllText("iisStupidMenu/iiMenu_Preferences.txt");
                        LoadPreferencesFromText(text);
                    }
                }
            } catch (Exception e) { UnityEngine.Debug.Log("Error loading preferences: " + e.Message); }
        }

        public static void NoAutoSave()
        {
            autoSaveDelay = Time.time + 1f;
        }


        public static void Panic()
        {
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.enabled)
                    {
                        Toggle(v.buttonText);
                    }
                }
            }
            NotifiLib.ClearAllNotifications();
        }

        public static void ChangeReconnectTime()
        {
            internetTime++;
            if (internetTime > 5)
            {
                internetTime = 1;
            }
            GetIndex("crTime").overlapText = "Change Reconnect Time <color=grey>[</color><color=green>" + internetTime.ToString() + "</color><color=grey>]</color>";
        }

        public static void ThinMenuOn()
        {
            FATMENU = false;
        }

        public static void ThinMenuOff()
        {
            FATMENU = true;
        }

        public static void BarkMenuOn()
        {
            likebark = true;
        }

        public static void BarkMenuOff()
        {
            likebark = false;
        }

        public static void CheckboxButtons()
        {
            checkMode = true;
        }

        public static void CheckboxButtonsOff()
        {
            checkMode = false;
        }

        public static void ChangeButtonSound()
        {
            int[] sounds = new int[]
            {
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
                106
            };
            string[] buttonSoundNames = new string[]
            {
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
                "Jar"
            };

            buttonClickIndex++;
            if (buttonClickIndex > buttonSoundNames.Length - 1)
            {
                buttonClickIndex = 0;
            }
            buttonClickSound = sounds[buttonClickIndex];
            GetIndex("cbsound").overlapText = "Change Button Sound <color=grey>[</color><color=green>" + buttonSoundNames[buttonClickIndex] + "</color><color=grey>]</color>";
        }

        public static void ChangeButtonVolume()
        {
            buttonClickVolume++;
            if (buttonClickVolume > 10)
            {
                buttonClickVolume = 0;
            }
            GetIndex("cbvol").overlapText = "Change Button Volume <color=grey>[</color><color=green>" + buttonClickVolume.ToString() + "</color><color=grey>]</color>";
        }

        public static void DisableButtonVibration()
        {
            doButtonsVibrate = false;
        }

        public static void EnableButtonVibration()
        {
            doButtonsVibrate = true;
        }

        public static void DisableGhostview()
        {
            disableGhostview = true;
        }

        public static void EnableGhostview()
        {
            disableGhostview = false;
        }

        public static void LegacyGhostview()
        {
            legacyGhostview = true;
        }

        public static void NewGhostview()
        {
            legacyGhostview = false;
        }

        public static void DisableBoardColors()
        {
            disableBoardColor = true;
            motd.SetActive(false);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd").SetActive(true);
        }

        public static void EnableBoardColors()
        {
            disableBoardColor = false;
            motd.SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd").SetActive(false);
        }

        public static void DisableBoardTextColors()
        {
            disableBoardTextColor = true;
        }

        public static void EnableBoardTextColors()
        {
            disableBoardTextColor = false;
        }
    }
}
