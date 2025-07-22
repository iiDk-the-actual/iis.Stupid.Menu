using BepInEx;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Mods;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Menu
{
    public class UI : MonoBehaviour
    {
        private static UI instance;

        private string inputText = "goldentrophy";
        private static GUIStyle labelStyle;

        private static Dictionary<string, Vector2> textWidthPool = new Dictionary<string, Vector2> { };
        public static Vector2 ExternalCalcSize(GUIContent text)
        {
            if (textWidthPool.ContainsKey(text.text))
                return textWidthPool[text.text];

            return new Vector2(text.text.Length, 12);
        }

        private string r = "255";
        private string g = "128";
        private string b = "0";

        public static bool isOpen = true;
        public static bool lastCondition = false;

        public static Texture2D icon;

        private void Start()
        {
            instance = this;

            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_HideGUI.txt"))
                isOpen = false;
        }

        private void OnGUI()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Backslash);

            if (isKeyboardCondition && !lastCondition)
            {
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

                Color guiColor = GetIndex("Swap GUI Colors").enabled ? textColor : GetBGColor(0f);

                GUI.color = guiColor;
                GUI.backgroundColor = guiColor;

                string roomText = translate ? TranslateText("Not connected to room") : "Not connected to room";
                try
                {
                    if (PhotonNetwork.InRoom)
                        roomText = (translate ? TranslateText("Connected to room") : "Connected to room") + " " + PhotonNetwork.CurrentRoom.Name;
                } catch { }
                GUI.Label(new Rect(10, Screen.height - 35, Screen.width, 40), roomText);
                
                if (icon == null)
                    icon = LoadTextureFromResource("iiMenu.Resources.icon.png");

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
                        GUI.Label(new Rect(Screen.width - 590, Screen.height - 75, 512, 64), (translate ? TranslateText("Build") : "Build")+" "+PluginInfo.Version+"\n"+(serverLink.Replace("https://", "")), style);
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
                    if (flipArraylist)
                        labelStyle.alignment = TextAnchor.UpperRight;

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

                                    alphabetized.Add(buttonText);
                                }
                            }
                            catch { }
                        }
                        categoryIndex++;
                    }

                    Regex notags = new Regex("<.*?>");
                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => labelStyle.CalcSize(new GUIContent(NoRichtextTags(s))).x)
                        .ToArray();

                    int index = 0;
                    float y = 10;
                    foreach (string v in sortedButtons)
                    {
                        if (!textWidthPool.ContainsKey(NoRichtextTags(v)))
                            textWidthPool.Add(NoRichtextTags(v), labelStyle.CalcSize(new GUIContent(NoRichtextTags(v))));

                        if (!textWidthPool.ContainsKey(v))
                            textWidthPool.Add(v, labelStyle.CalcSize(new GUIContent(v)));

                        if (advancedArraylist)
                        {
                            string text = flipArraylist ? 
                                  $"<color=#{ColorToHex(textColor)}>{v}</color><color=#{ColorToHex(GetBGColor(index * -0.1f))}> |</color>"
                                : $"<color=#{ColorToHex(GetBGColor(index * -0.1f))}>| </color><color=#{ColorToHex(textColor)}>{v}</color>";

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

                foreach (KeyValuePair<string, Assembly> Plugin in Settings.LoadedPlugins)
                {
                    try
                    {
                        if (!Settings.disabledPlugins.Contains(Plugin.Key))
                            PluginOnGUI(Plugin.Value);
                    }
                    catch (Exception e) { LogManager.Log("Error with OnGUI plugin " + Plugin.Key + ": " + e.ToString()); }
                }
            }
        }
    }
}