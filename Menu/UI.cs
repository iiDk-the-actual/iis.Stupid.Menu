/*
 * ii's Stupid Menu  Menu/UI.cs
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
using GorillaNetworking;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Managers;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;

namespace iiMenu.Menu
{
    public class UI : MonoBehaviour
    {
        // TODO: Convert this class to the assetbundle during TMPro migration
        public static UI Instance;
        public static Texture2D watermarkImage;

        private void Awake()
        {
            Instance = this;

            if (File.Exists(hideGUIPath))
                isOpen = false;

            uiPrefab = LoadObject<GameObject>("UI");

            Transform canvas = uiPrefab.transform.Find("Canvas");
            watermark = canvas.Find("Watermark").GetComponent<Image>();
            versionLabel = canvas.Find("VersionLabel").GetComponent<TextMeshProUGUI>();
            roomStatus = canvas.Find("RoomStatus").GetComponent<TextMeshProUGUI>();
            arraylist = canvas.Find("Arraylist").GetComponent<TextMeshProUGUI>();
            controlBackground = canvas.Find("ControlUI").GetComponent<Image>();

            r = canvas.Find("ControlUI/R").GetComponent<TMP_InputField>();
            g = canvas.Find("ControlUI/G").GetComponent<TMP_InputField>();
            b = canvas.Find("ControlUI/B").GetComponent<TMP_InputField>();
            textInput = canvas.Find("ControlUI/TextInput").GetComponent<TMP_InputField>();
            LogManager.Log(canvas.Find("ControlUI/QueueButton"));
            canvas.Find("ControlUI/QueueButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                Mods.Important.QueueRoom(textInput.text);
            });

            canvas.Find("ControlUI/JoinButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(textInput.text, JoinType.Solo);
            });

            canvas.Find("ControlUI/ColorButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                ChangeColor(new Color32(byte.Parse(r.text), byte.Parse(g.text), byte.Parse(b.text), 255));
            });

            canvas.Find("ControlUI/NameButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                ChangeName(textInput.text);
            });

            textObjects = new List<TextMeshProUGUI>
            {
                canvas.Find("ControlUI/TextInput/Text Area/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/R/Text Area/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/G/Text Area/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/B/Text Area/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/QueueButton/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/JoinButton/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/ColorButton/Text").GetComponent<TextMeshProUGUI>(),
                canvas.Find("ControlUI/NameButton/Text").GetComponent<TextMeshProUGUI>()
            };

            imageObjects = new List<Image>
            {
                canvas.Find("ControlUI/TextInput").GetComponent<Image>(),
                canvas.Find("ControlUI/R").GetComponent<Image>(),
                canvas.Find("ControlUI/G").GetComponent<Image>(),
                canvas.Find("ControlUI/B").GetComponent<Image>(),
                canvas.Find("ControlUI/QueueButton").GetComponent<Image>(),
                canvas.Find("ControlUI/JoinButton").GetComponent<Image>(),
                canvas.Find("ControlUI/ColorButton").GetComponent<Image>(),
                canvas.Find("ControlUI/NameButton").GetComponent<Image>()
            };

            watermark.material = new Material(watermark.material);
            watermarkImage = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.icon.png");

            if (!Plugin.FirstLaunch)
            {
                GameObject closeMessage = uiPrefab.transform?.Find("Canvas")?.Find("HideMessage")?.gameObject;
                closeMessage?.SetActive(false);
            }

            Update();
        }

        private bool isOpen = true;

        private GameObject uiPrefab;

        private Image watermark;
        private TextMeshProUGUI versionLabel;
        private TextMeshProUGUI roomStatus;
        private TextMeshProUGUI arraylist;

        private TMP_InputField r;
        private TMP_InputField g;
        private TMP_InputField b;
        private TMP_InputField textInput;

        private Image controlBackground;
        private List<TextMeshProUGUI> textObjects;
        private List<Image> imageObjects = new List<Image>();

        private bool wasKeyboardCondition;
        private float uiUpdateDelay;

        private void Update()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Backslash);

            if (isKeyboardCondition && !wasKeyboardCondition)
                ToggleGUI();
            
            wasKeyboardCondition = isKeyboardCondition;

            if (isOpen)
            {
                uiPrefab.SetActive(true);
                
                Color guiColor = Buttons.GetIndex("Swap GUI Colors").enabled
                    ? textColors[1].GetCurrentColor()
                    : backgroundColor.GetCurrentColor();

                versionLabel.color = guiColor;
                roomStatus.color = guiColor;
                arraylist.color = guiColor;
                watermark.color = guiColor;

                versionLabel.SafeSetFont(activeFont);
                roomStatus.SafeSetFont(activeFont);
                arraylist.SafeSetFont(activeFont);

                versionLabel.SafeSetFontStyle(activeFontStyle);
                roomStatus.SafeSetFontStyle(activeFontStyle);
                arraylist.SafeSetFontStyle(activeFontStyle);

                controlBackground.color = backgroundColor.GetCurrentColor();

                foreach (var textObject in textObjects)
                {
                    textObject.color = textColors[1].GetCurrentColor();
                    textObject.SafeSetFont(activeFont);
                    textObject.SafeSetFontStyle(activeFontStyle);
                }

                foreach (var imageObject in imageObjects)
                    imageObject.color = buttonColors[0].GetCurrentColor();

                watermark.transform.rotation = Quaternion.Euler(0f, 0f, rockWatermark ? Mathf.Sin(Time.time * 2f) * 10f : 0f);
                versionLabel.SafeSetText(FollowMenuSettings("Build") + " " + PluginInfo.Version + "\n" +
                                    serverLink.Replace("https://", ""));

                roomStatus.SafeSetText(FollowMenuSettings(!PhotonNetwork.InRoom ? "Not connected to room" : "Connected to room ") +
                   (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : ""));

                if (Time.time > uiUpdateDelay)
                {
                    Texture2D watermarkTexture = customWatermark ?? watermarkImage;

                    if (watermark.sprite == null || watermark.sprite.texture == null || watermark.sprite.texture != watermarkTexture)
                    {
                        Sprite sprite = Sprite.Create(
                            watermarkTexture,
                            new Rect(0, 0, watermarkTexture.width, watermarkTexture.height),
                            new Vector2(0.5f, 0.5f),
                            100f
                        );

                        watermark.sprite = sprite;
                    }
                   
                    if (flipArraylist)
                    {
                        controlBackground.rectTransform.anchoredPosition = new Vector2(10f, -10f);
                        controlBackground.rectTransform.anchorMin = new Vector2(0f, 1f);
                        controlBackground.rectTransform.anchorMax = new Vector2(0f, 1f);

                        arraylist.rectTransform.anchoredPosition = new Vector2(-837.5001f, -523f);
                        arraylist.rectTransform.anchorMin = new Vector2(1f, 1f);
                        arraylist.rectTransform.anchorMax = new Vector2(1f, 1f);

                        arraylist.alignment = TextAlignmentOptions.TopRight;
                    }
                    else
                    {
                        controlBackground.rectTransform.anchoredPosition = new Vector2(-250f, -10f);
                        controlBackground.rectTransform.anchorMin = new Vector2(1f, 1f);
                        controlBackground.rectTransform.anchorMax = new Vector2(1f, 1f);

                        arraylist.rectTransform.anchoredPosition = new Vector2(837.5001f, -523f);
                        arraylist.rectTransform.anchorMin = new Vector2(0f, 1f);
                        arraylist.rectTransform.anchorMax = new Vector2(0f, 1f);

                        arraylist.alignment = TextAlignmentOptions.TopLeft;
                    }

                    uiUpdateDelay = Time.time + (advancedArraylist ? 0.1f : 0.5f);

                    List<string> enabledMods = new List<string>();
                    int categoryIndex = 0;

                    foreach (ButtonInfo[] buttonList in Buttons.buttons)
                    {
                        foreach (ButtonInfo button in buttonList)
                        {
                            try
                            {
                                if (button.enabled && (!hideSettings || (hideSettings && !Buttons.categoryNames[categoryIndex].Contains("Settings"))))
                                {
                                    string buttonText = button.overlapText ?? button.buttonText;

                                    if (inputTextColor != "green")
                                        buttonText = buttonText.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + inputTextColor + ">");

                                    buttonText = FollowMenuSettings(buttonText);
                                    enabledMods.Add(buttonText);
                                }
                            }
                            catch { }
                        }
                        categoryIndex++;
                    }

                    string[] sortedMods = enabledMods
                        .OrderByDescending(s => arraylist.GetPreferredValues(NoRichtextTags(s)).x)
                        .ToArray();

                    string modListText = "";
                    for (int i = 0; i < sortedMods.Length; i++)
                    {
                        Color targetColor = Buttons.GetIndex("Swap GUI Colors").enabled ? buttonColors[1].GetCurrentColor(i * -0.1f) : backgroundColor.GetCurrentColor(i * -0.1f);

                        if (advancedArraylist)
                            modListText += (flipArraylist ?
                            /* Flipped */ $"<mark=#{ColorToHex(backgroundColor.GetCurrentColor(i * -0.1f))}C0> {sortedMods[i]} </mark><mark=#{ColorToHex(buttonColors[1].GetCurrentColor(i * -0.1f))}> </mark>" :
                            /* Normal  */ $"<mark=#{ColorToHex(buttonColors[1].GetCurrentColor(i * -0.1f))}> </mark><mark=#{ColorToHex(backgroundColor.GetCurrentColor(i * -0.1f))}C0> {sortedMods[i]} </mark>") + "\n";
                        else
                            modListText += sortedMods[i] + "\n";
                    }

                    arraylist.SafeSetText(modListText);
                }
            } else
                uiPrefab.SetActive(false);
        }

        private readonly string hideGUIPath = $"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt";
        private void ToggleGUI()
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                if (File.Exists(hideGUIPath))
                    File.Delete(hideGUIPath);
            }
            else
            {
                if (!File.Exists(hideGUIPath))
                    File.WriteAllText(hideGUIPath, "Text file generated with ii's Stupid Menu");
            }

            GameObject closeMessage = uiPrefab.transform?.Find("Canvas")?.Find("HideMessage")?.gameObject;
            closeMessage?.SetActive(false);
        }

        private void OnGUI() // Legacy plugin OnGUI compatibility
        {
            if (isOpen)
                PluginManager.ExecuteOnGUI();
        }
    }
}