using BepInEx;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Menu;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.UI
{
    public class Main : MonoBehaviour
    {
        private string inputText = "goldentrophy";

        private string r = "255";

        private string g = "128";

        private string b = "0";

        public static bool isOpen = true;

        public static bool lastCondition = false;

        public static float lasttimeiconupdated = -1f;

        public static Texture2D icon;

        private Texture2D LoadTextureFromResource(string resourcePath)
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

        private void Start()
        {
            if (File.Exists("iisStupidMenu/iiMenu_HideGUI.txt"))
            {
                isOpen = false;
            }
        }

        private void OnGUI()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Backslash);

            if (isKeyboardCondition && !lastCondition)
            {
                isOpen = !isOpen;
                if (isOpen)
                {
                    if (File.Exists("iisStupidMenu/iiMenu_HideGUI.txt"))
                    {
                        File.Delete("iisStupidMenu/iiMenu_HideGUI.txt");
                    }
                } else
                {
                    if (!File.Exists("iisStupidMenu/iiMenu_HideGUI.txt"))
                    {
                        File.WriteAllText("iisStupidMenu/iiMenu_HideGUI.txt", "true");
                    }
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
                GUI.skin.textField.fontStyle = activeFontStyle;
                GUI.skin.button.fontStyle = activeFontStyle;
                GUI.skin.label.fontStyle = activeFontStyle;

                Color victimColor = GetIndex("Swap GUI Colors").enabled ? textColor : GetBGColor(0f);

                GUI.color = victimColor;
                GUI.backgroundColor = victimColor;

                string roomText = translate ? TranslateText("Not connected to room") : "Not connected to room";
                try
                {
                    if (PhotonNetwork.InRoom)
                    {
                        roomText = (translate ? TranslateText("Connected to room ") : "Connected to room ") + PhotonNetwork.CurrentRoom.Name;
                    }
                } catch { }
                GUI.Label(new Rect(10, Screen.height - 35, Screen.width, 40), roomText);
                
                try
                {
                    if (icon == null)
                    {
                        icon = LoadTextureFromResource("iiMenu.Resources.icon.png");
                    }
                }
                catch { }

                try
                {
                    if (icon != null)
                    {
                        Rect pos = new Rect(Screen.width - 70, Screen.height - 70, 64, 64);
                        Matrix4x4 matrix = GUI.matrix;

                        GUIUtility.RotateAroundPivot(Mathf.Sin(Time.time * 2f) * 10f, pos.center);
                        GUI.DrawTexture(pos, icon);
                        GUI.matrix = matrix;

                        GUIStyle style = new GUIStyle(GUI.skin.label);
                        style.alignment = TextAnchor.LowerRight;
                        GUI.Label(new Rect(Screen.width - 590, Screen.height - 75, 512, 64), (translate ? TranslateText("Build ") : "Build ")+PluginInfo.Version+"\n"+(serverLink.Replace("https://", "")), style);
                    }
                }
                catch { }

                GUI.Box(new Rect(Screen.width - 250, 10, 240, 120), "", GUI.skin.box);

                inputText = GUI.TextField(new Rect(Screen.width - 200, 20, 180, 20), inputText);

                r = GUI.TextField(new Rect(Screen.width - 240, 20, 30, 20), r);

                g = GUI.TextField(new Rect(Screen.width - 240, 50, 30, 20), g);

                b = GUI.TextField(new Rect(Screen.width - 240, 80, 30, 20), b);

                if (GUI.Button(new Rect(Screen.width - 200, 50, 85, 30), "Name"))
                {
                    try
                    {
                        GorillaComputer.instance.currentName = inputText;
                        PhotonNetwork.LocalPlayer.NickName = inputText;
                        GorillaComputer.instance.offlineVRRigNametagText.text = inputText;
                        GorillaComputer.instance.savedName = inputText;
                        PlayerPrefs.SetString("playerName", inputText);
                        PlayerPrefs.Save();
                    }
                    catch
                    {
                        UnityEngine.Debug.Log("lemming is yet to fix me");
                    }
                }
                if (GUI.Button(new Rect(Screen.width - 105, 50, 85, 30), "Color"))
                {
                    UnityEngine.Color color = new Color32(byte.Parse(r), byte.Parse(g), byte.Parse(b), 255);

                    ChangeColor(color);
                }
                bool Create = false;
                try
                {
                    Create = UnityInput.Current.GetKey(KeyCode.LeftControl);
                } catch { }
                if (GUI.Button(new Rect(Screen.width - 200, 90, 85, 30), Create ? "Create" : "Join"))
                {
                    if (Create)
                    {
                        string toJoin = inputText.Replace("\\n", "\n");
                        iiMenu.Mods.Important.CreateRoom(toJoin, true);
                    } else
                    {
                        string toJoin = inputText.Replace("\\n", "\n");
                        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(toJoin, JoinType.Solo);
                    }
                    
                }
                if (GUI.Button(new Rect(Screen.width - 105, 90, 85, 30), "Queue"))
                {
                    PhotonNetwork.Disconnect();
                    rejRoom = inputText;
                }

                try
                {
                    GUI.color = victimColor;
                    GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                    GUILayout.Space(5f);
                    GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

                    GUILayout.Space(5f);

                    List<string> alphabetized = new List<string>();
                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            try
                            {
                                if (v.enabled)
                                {
                                    string toadd = (v.overlapText == null) ? v.buttonText : v.overlapText;
                                    if (translate)
                                    {
                                        toadd = TranslateText(toadd);
                                    }
                                    if (inputTextColor != "green")
                                    {
                                        toadd = toadd.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + inputTextColor + ">");
                                    }
                                    if (lowercaseMode)
                                    {
                                        toadd = toadd.ToLower();
                                    }
                                    alphabetized.Add(toadd);
                                }
                            } catch { }
                        }
                    }

                    Regex notags = new Regex("<.*?>");
                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => (notags.Replace(s,"")).Length)
                        .ToArray();

                    foreach (string v in sortedButtons)
                    {
                        GUILayout.Label(v, Array.Empty<GUILayoutOption>());
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                catch
                {
                    UnityEngine.Debug.Log("FUCKKK");
                }
            }
        }
    }
}