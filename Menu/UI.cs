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

        private void Start()
        {
            Instance = this;

            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt"))
                isOpen = false;
        }

        private void OnGUI()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Backslash);

            if (isKeyboardCondition && !lastCondition)
            {
                hidBefore = true;

                isOpen = !isOpen;
                if (isOpen)
                {
                    if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt"))
                        File.Delete($"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt");
                } else
                {
                    if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt"))
                        File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt", "true");
                }
            }
            lastCondition = isKeyboardCondition;

            if (isOpen)
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

                Color guiColor = Buttons.GetIndex("Swap GUI Colors").enabled ? textColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor();

                GUI.color = guiColor;
                GUI.backgroundColor = guiColor;

                string roomText = translate ? TranslateText("Not connected to room") : "Not connected to room";
                try
                {
                    if (PhotonNetwork.InRoom)
                        roomText = (translate ? TranslateText("Connected to room") : "Connected to room") + " " + PhotonNetwork.CurrentRoom.Name;
                } catch { }
                GUI.Label(new Rect(10, Screen.height - 35, Screen.width, 40), roomText);

                if (Plugin.FirstLaunch && !hidBefore)
                {
                    string hideText = translate ? TranslateText("Press Backslash to hide this UI") : "Press Backslash to hide this UI";
                    GUI.Label(new Rect((Screen.width / 2f) - (GUI.skin.label.CalcSize(new GUIContent(hideText)).x / 2f), Screen.height - 35, Screen.width, 40), hideText);
                }

                if (icon == null)
                    icon = LoadTextureFromResource($"{PluginInfo.ClientResourcePath}.icon.png");

                try
                {
                    if (icon != null)
                    {
                        Rect pos = new Rect(Screen.width - 70, Screen.height - 70, 64, 64);
                        Matrix4x4 matrix = GUI.matrix;

                        GUIUtility.RotateAroundPivot(Mathf.Sin(Time.time * 2f) * 10f, pos.center);
                        GUI.DrawTexture(pos, icon);
                        GUI.matrix = matrix;

                        GUIStyle style = new GUIStyle(GUI.skin.label)
                        {
                            alignment = TextAnchor.LowerRight
                        };
                        GUI.Label(new Rect(Screen.width - 590, Screen.height - 75, 512, 64), (translate ? TranslateText("Build") : "Build")+" "+PluginInfo.Version+"\n"+serverLink.Replace("https://", ""), style);
                    }
                }
                catch { }

                GUI.Box(
                    flipArraylist ? new Rect(10, 10, 240, 120) : 
                    new Rect(Screen.width - 250, 10, 240, 120),
                    "", GUI.skin.box);

                inputText = GUI.TextField(
                    flipArraylist ? new Rect(60, 20, 180, 20) :
                    new Rect(Screen.width - 200, 20, 180, 20), 
                    inputText);

                r = GUI.TextField(
                    flipArraylist ? new Rect(20, 20, 30, 20) :
                    new Rect(Screen.width - 240, 20, 30, 20), 
                    r);
                g = GUI.TextField(flipArraylist ? new Rect(20, 50, 30, 20) :
                    new Rect(Screen.width - 240, 50, 30, 20),
                    g);
                b = GUI.TextField(flipArraylist ? new Rect(20, 80, 30, 20) :
                    new Rect(Screen.width - 240, 80, 30, 20),
                    b);

                if (GUI.Button(flipArraylist ? new Rect(60, 50, 85, 30) : 
                                               new Rect(Screen.width - 200, 50, 85, 30), 
                                               translate ? TranslateText("Name") : "Name"))
                    ChangeName(inputText.Replace("\\n", "\n"));
                
                if (GUI.Button(flipArraylist ? new Rect(155, 50, 85, 30) :
                                               new Rect(Screen.width - 105, 50, 85, 30),
                                                translate ? TranslateText("Color") : "Color"))
                {
                    Color color = new Color32(byte.Parse(r), byte.Parse(g), byte.Parse(b), 255);
                    ChangeColor(color);
                }

                bool Create = UnityInput.Current.GetKey(KeyCode.LeftControl);

                string targetText = Create ? "Create" : "Join";
                if (GUI.Button(flipArraylist ? new Rect(60, 90, 85, 30) :
                               new Rect(Screen.width - 200, 90, 85, 30), 
                               translate ? TranslateText(targetText) : targetText))
                {
                    if (Create)
                        Important.CreateRoom(inputText.Replace("\\n", "\n"), true);
                    else
                        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(inputText.Replace("\\n", "\n"), JoinType.Solo);
                }
                if (GUI.Button(flipArraylist ? new Rect(155, 90, 85, 30) :
                               new Rect(Screen.width - 105, 90, 85, 30),
                               "Queue"))
                {
                    NetworkSystem.Instance.ReturnToSinglePlayer();
                    Important.QueueRoom(inputText);
                }

                try
                {
                    labelStyle = new GUIStyle(GUI.skin.label)
                    {
                        richText = true
                    };

                    if (advancedArraylist)
                        labelStyle.fontStyle = (FontStyle)((int)activeFontStyle % 2);
                    else if (flipArraylist)
                        labelStyle.alignment = TextAnchor.UpperLeft;


                    GUI.color = guiColor;
                    List<string> alphabetized = new List<string>();

                    int categoryIndex = 0;
                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            try
                            {
                                if (v.enabled && (!hideSettings || (hideSettings && !Buttons.categoryNames[categoryIndex].Contains("Settings"))))
                                {
                                    string buttonText = v.overlapText ?? v.buttonText;
                                    if (translate)
                                        buttonText = TranslateText(buttonText);

                                    if (inputTextColor != "green")
                                        buttonText = buttonText.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + inputTextColor + ">");

                                    if (lowercaseMode)
                                        buttonText = buttonText.ToLower();

                                    if (uppercaseMode)
                                        buttonText = buttonText.ToUpper();

                                    alphabetized.Add(buttonText);
                                }
                            }
                            catch { }
                        }
                        categoryIndex++;
                    }

                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => labelStyle.CalcSize(new GUIContent(NoRichtextTags(s))).x)
                        .ToArray();

                    int index = 0;
                    float y = 10;
                    foreach (string v in sortedButtons)
                    {
                        if (advancedArraylist)
                        {
                            string text = flipArraylist ?
                                  $"<color=#{ColorToHex(textColors[1].GetCurrentColor())}>{v}</color><color=#{ColorToHex(backgroundColor.GetCurrentColor(index * -0.1f))}> |</color>"
                                : $"<color=#{ColorToHex(backgroundColor.GetCurrentColor(index * -0.1f))}>| </color><color=#{ColorToHex(textColors[1].GetCurrentColor())}>{v}</color>";

                            Vector2 size = labelStyle.CalcSize(new GUIContent(text));
                            Rect labelRect = new Rect(flipArraylist ? Screen.width - (size.x + 15) : 10, y, size.x + 8, index == sortedButtons.Length - 1 ? size.y + 5 : size.y);

                            y += size.y;

                            Color oldColor = GUI.color;
                            GUI.color = new Color(0f, 0f, 0f, 0.5f);
                            GUI.DrawTexture(labelRect, Texture2D.whiteTexture);
                            GUI.color = oldColor;

                            GUI.Label(new Rect(labelRect.x, labelRect.y, labelRect.width + 100f, labelRect.height + 100f), text, labelStyle);
                        }
                        else
                        {
                            Vector2 size = labelStyle.CalcSize(new GUIContent(v));
                            GUI.Label(new Rect(flipArraylist ? Screen.width - (size.x + 15) : 10, y, size.x + 100f, size.y + 100f), v, labelStyle);

                            y += size.y;
                        }

                        index++;
                    }
                }
                catch { }

                PluginManager.ExecuteOnGUI();
            }
        }
    }
}