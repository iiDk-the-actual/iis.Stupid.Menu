/*
 * ii's Stupid Menu  Menu/UI.cs
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
using GorillaNetworking;
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using iiMenu.Mods;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;

namespace iiMenu.Menu
{
    public class UI : MonoBehaviour
    {
        public static UI Instance;

        private string inputText = "goldentrophy";
        private static GUIStyle labelStyle;

        private string r = "255";
        private string g = "128";
        private string b = "0";

        public static bool isOpen = true;
        public static bool hidBefore;
        public static bool lastCondition;

        public static Texture2D icon;

        // Cached values to avoid recalculation
        private readonly string hideGUIPath = $"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt";
        private Color cachedGuiColor;
        private bool guiColorDirty = true;
        private string cachedRoomText;
        private float roomTextUpdateTimer;
        private const float RoomTextUpdateInterval = 0.5f;

        // Cached GUI positions
        private Rect boxRect, inputRect, rRect, gRect, bRect;
        private Rect nameButtonRect, colorButtonRect, joinButtonRect, queueButtonRect;
        private Rect iconRect, versionLabelRect;
        private bool rectsDirty = true;
        private bool prevFlipArraylist;

        private void Awake() =>
            Instance = this;

        private void Start()
        {
            if (File.Exists(hideGUIPath))
                isOpen = false;
        }

        private void OnGUI()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Backslash);

            if (isKeyboardCondition && !lastCondition)
            {
                hidBefore = true;
                ToggleGUI();
            }
            lastCondition = isKeyboardCondition;

            if (!isOpen) return;

            if (prevFlipArraylist != flipArraylist)
            {
                prevFlipArraylist = flipArraylist;
                rectsDirty = true;
            }

            ConfigureGUIStyles();
            UpdateGuiColor();

            GUI.color = cachedGuiColor;
            GUI.backgroundColor = cachedGuiColor;

            DrawRoomText();
            DrawHidePrompt();
            DrawIcon();
            DrawControlPanel();
            DrawArrayList();

            PluginManager.ExecuteOnGUI();
        }

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
            rectsDirty = true;
        }

        private void ConfigureGUIStyles()
        {
            GUI.skin.textField.fontSize = 13;
            GUI.skin.button.fontSize = 20;
            GUI.skin.textField.font = activeFont;
            GUI.skin.button.font = activeFont;
            GUI.skin.label.font = activeFont;
            GUI.skin.label.richText = true;
            GUI.skin.textField.fontStyle = activeFontStyle;
            GUI.skin.button.fontStyle = activeFontStyle;
            GUI.skin.label.fontStyle = activeFontStyle;
        }

        private void UpdateGuiColor()
        {
            if (!guiColorDirty) return;

            cachedGuiColor = Buttons.GetIndex("Swap GUI Colors").enabled
                ? textColors[1].GetCurrentColor()
                : backgroundColor.GetCurrentColor();
            guiColorDirty = false;
        }

        private void DrawRoomText()
        {
            roomTextUpdateTimer += Time.deltaTime;
            if (roomTextUpdateTimer >= RoomTextUpdateInterval || cachedRoomText == null)
            {
                cachedRoomText = GetRoomText();
                roomTextUpdateTimer = 0f;
            }

            GUI.Label(new Rect(10, Screen.height - 35, Screen.width, 40), cachedRoomText);
        }

        private string GetRoomText()
        {
            string roomText = translate ? TranslateText("Not connected to room") : "Not connected to room";
            try
            {
                if (PhotonNetwork.InRoom)
                    roomText = (translate ? TranslateText("Connected to room") : "Connected to room") + " " + PhotonNetwork.CurrentRoom.Name;
            }
            catch { }
            return roomText;
        }

        private void DrawHidePrompt()
        {
            if (!Plugin.FirstLaunch || hidBefore) return;

            string hideText = translate ? TranslateText("Press Backslash to hide this UI") : "Press Backslash to hide this UI";
            GUI.Label(new Rect((Screen.width / 2f) - (GUI.skin.label.CalcSize(new GUIContent(hideText)).x / 2f),
                Screen.height - 35, Screen.width, 40), hideText);
        }

        private void DrawIcon()
        {
            if (icon == null)
                icon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.icon.png");

            if (icon == null) return;

            try
            {
                if (rectsDirty)
                {
                    iconRect = new Rect(Screen.width - 70, Screen.height - 70, 64, 64);
                    versionLabelRect = new Rect(Screen.width - 590, Screen.height - 75, 512, 64);
                }

                Matrix4x4 matrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(Mathf.Sin(Time.time * 2f) * 10f, iconRect.center);

                if (customWatermark)
                {
                    Color color = GUI.color;
                    GUI.color = Color.white;
                    GUI.DrawTexture(iconRect, customWatermark);
                    GUI.color = color;
                } else 
                    GUI.DrawTexture(iconRect, customWatermark ?? icon);

                GUI.matrix = matrix;

                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.LowerRight
                };
                GUI.Label(versionLabelRect,
                    (translate ? TranslateText("Build") : "Build") + " " + PluginInfo.Version + "\n" +
                    serverLink.Replace("https://", ""), style);
            }
            catch { }
        }

        private void DrawControlPanel()
        {
            if (rectsDirty)
                CacheControlPanelRects();

            GUI.Box(boxRect, "", GUI.skin.box);

            inputText = GUI.TextField(inputRect, inputText);
            r = GUI.TextField(rRect, r);
            g = GUI.TextField(gRect, g);
            b = GUI.TextField(bRect, b);

            if (GUI.Button(nameButtonRect, translate ? TranslateText("Name") : "Name"))
                ChangeName(inputText.Replace("\\n", "\n"));

            if (GUI.Button(colorButtonRect, translate ? TranslateText("Color") : "Color"))
            {
                Color color = new Color32(byte.Parse(r), byte.Parse(g), byte.Parse(b), 255);
                ChangeColor(color);
            }

            bool Create = UnityInput.Current.GetKey(KeyCode.LeftControl);
            string targetText = Create ? "Create" : "Join";

            if (GUI.Button(joinButtonRect, translate ? TranslateText(targetText) : targetText))
            {
                if (Create)
                    Important.CreateRoom(inputText.Replace("\\n", "\n"), true);
                else
                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(inputText.Replace("\\n", "\n"), JoinType.Solo);
            }

            if (GUI.Button(queueButtonRect, "Queue"))
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
                Important.QueueRoom(inputText);
            }

            rectsDirty = false;
        }

        private void CacheControlPanelRects()
        {
            float baseX = flipArraylist ? 10 : Screen.width - 250;

            boxRect = new Rect(baseX, 10, 240, 120);
            inputRect = new Rect(baseX + 50, 20, 180, 20);
            rRect = new Rect(baseX + 10, 20, 30, 20);
            gRect = new Rect(baseX + 10, 50, 30, 20);
            bRect = new Rect(baseX + 10, 80, 30, 20);
            nameButtonRect = new Rect(baseX + 50, 50, 85, 30);
            colorButtonRect = new Rect(baseX + 145, 50, 85, 30);
            joinButtonRect = new Rect(baseX + 50, 90, 85, 30);
            queueButtonRect = new Rect(baseX + 145, 90, 85, 30);
        }

        private void DrawArrayList()
        {
            try
            {
                labelStyle = new GUIStyle(GUI.skin.label) { richText = true };

                if (advancedArraylist)
                    labelStyle.fontStyle = (FontStyle)((int)activeFontStyle % 2);
                else if (flipArraylist)
                    labelStyle.alignment = TextAnchor.UpperLeft;

                GUI.color = cachedGuiColor;

                var sortedButtons = GetSortedEnabledButtons();
                DrawButtonList(sortedButtons);
            }
            catch { }
        }

        private string[] GetSortedEnabledButtons()
        {
            var alphabetized = new List<string>(64); // Pre-allocate capacity
            int categoryIndex = 0;

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                bool skipCategory = hideSettings && Buttons.categoryNames[categoryIndex].Contains("Settings");

                if (!skipCategory)
                {
                    foreach (ButtonInfo v in buttonlist)
                    {
                        if (v.enabled)
                        {
                            string buttonText = ProcessButtonText(v);
                            alphabetized.Add(buttonText);
                        }
                    }
                }
                categoryIndex++;
            }

            return alphabetized
                .OrderByDescending(s => labelStyle.CalcSize(new GUIContent(NoRichtextTags(s))).x)
                .ToArray();
        }

        private string ProcessButtonText(ButtonInfo button)
        {
            string buttonText = button.overlapText ?? button.buttonText;

            if (translate)
                buttonText = TranslateText(buttonText);

            if (inputTextColor != "green")
                buttonText = buttonText.Replace(" <color=grey>[</color><color=green>",
                    " <color=grey>[</color><color=" + inputTextColor + ">");

            if (lowercaseMode)
                buttonText = buttonText.ToLower();
            else if (uppercaseMode)
                buttonText = buttonText.ToUpper();

            return buttonText;
        }

        private void DrawButtonList(string[] sortedButtons)
        {
            int index = 0;
            float y = 10;

            foreach (string v in sortedButtons)
            {
                if (advancedArraylist)
                    DrawAdvancedButton(v, ref y, index, sortedButtons.Length);
                else
                    DrawSimpleButton(v, ref y);

                index++;
            }
        }

        private void DrawAdvancedButton(string text, ref float y, int index, int totalCount)
        {
            string formattedText = flipArraylist
                ? $"<color=#{ColorToHex(textColors[1].GetCurrentColor())}>{text}</color><color=#{ColorToHex(backgroundColor.GetCurrentColor(index * -0.1f))}> |</color>"
                : $"<color=#{ColorToHex(backgroundColor.GetCurrentColor(index * -0.1f))}>| </color><color=#{ColorToHex(textColors[1].GetCurrentColor())}>{text}</color>";

            Vector2 size = labelStyle.CalcSize(new GUIContent(formattedText));
            float height = index == totalCount - 1 ? size.y + 5 : size.y;
            Rect labelRect = new Rect(flipArraylist ? Screen.width - (size.x + 15) : 10, y, size.x + 8, height);

            y += size.y;

            Color oldColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.DrawTexture(labelRect, Texture2D.whiteTexture);
            GUI.color = oldColor;

            GUI.Label(new Rect(labelRect.x, labelRect.y, labelRect.width + 100f, labelRect.height + 100f),
                formattedText, labelStyle);
        }

        private void DrawSimpleButton(string text, ref float y)
        {
            Vector2 size = labelStyle.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(flipArraylist ? Screen.width - (size.x + 15) : 10, y, size.x + 100f, size.y + 100f),
                text, labelStyle);
            y += size.y;
        }
    }
}