using BepInEx;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Menu;
using Photon.Pun;
using System;
using System.Drawing;
using UnityEngine;
using static iiMenu.Mods.Reconnect;

namespace iiMenu.UI
{
    [BepInPlugin("org.iidk.roomjoiner", "Room joiner @ ii's Stupid Menu", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        private string inputText = "goldentrophy";

        private string r = "255";

        private string g = "128";

        private string b = "0";

        public static Font agency = Font.CreateDynamicFontFromOSFont("Agency FB", 24);

        public static bool isOpen = true;

        public static bool lastCondition = false;

        private void OnGUI()
        {
            bool isKeyboardCondition = UnityInput.Current.GetKey(KeyCode.Backslash);

            if (isKeyboardCondition && !lastCondition)
            {
                isOpen = !isOpen;
            }
            lastCondition = isKeyboardCondition;

            if (isOpen)
            {
                GUI.skin.textField.fontSize = 13;
                GUI.skin.button.fontSize = 20;
                GUI.skin.textField.font = agency;
                GUI.skin.button.font = agency;
                GUI.skin.label.font = agency;
                GUI.skin.textField.fontStyle = FontStyle.Italic;
                GUI.skin.button.fontStyle = FontStyle.Italic;
                GUI.skin.label.fontStyle = FontStyle.Italic;

                GUI.color = Menu.UIColorHelper.bgc;
                GUI.backgroundColor = Menu.UIColorHelper.bgc;

                GUI.Box(new Rect(Screen.width - 250, 10, 240, 120), "", GUI.skin.box);

                string roomText = "Not connected to room";
                try
                {
                    if (PhotonNetwork.InRoom)
                    {
                        roomText = "Connected to room "+PhotonNetwork.CurrentRoom.Name;
                    }
                } catch { } // shitty ass code
                GUI.Label(new Rect(0, Screen.height - 25, Screen.width, 40), roomText);

                inputText = GUI.TextField(new Rect(Screen.width - 200, 20, 180, 20), inputText);
                // inputText = inputText.ToUpper(); i dont need this

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

                    PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
                    PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
                    PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));

                    //GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = color;
                    GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
                    PlayerPrefs.Save();

                    GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[] { color.r, color.g, color.b, false });
                }
                if (GUI.Button(new Rect(Screen.width - 200, 90, 85, 30), "Join"))
                {
                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(inputText);
                }
                if (GUI.Button(new Rect(Screen.width - 105, 90, 85, 30), "Queue"))
                {
                    PhotonNetwork.Disconnect();
                    rejRoom = inputText;
                    rejDebounce = Time.time + 2f;
                }

                try
                {
                    GUI.color = Menu.UIColorHelper.bgc;
                    GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            if (v.enabled)
                            {
                                GUILayout.Label(v.buttonText, Array.Empty<GUILayoutOption>());
                            }
                        }
                    }
                }
                catch
                {
                    UnityEngine.Debug.Log("FUCKKK");
                }
            }
        }
    }
}