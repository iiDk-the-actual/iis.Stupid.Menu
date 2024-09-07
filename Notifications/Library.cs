using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BepInEx;
using iiMenu.Classes;
using iiMenu.Menu;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Menu.Main;

namespace iiMenu.Notifications
{
    // Originally created by lars, he gave me permission
    // Modified by ii, not much though
    //[BepInPlugin("org.gorillatag.lars.notifications2", "NotificationLibrary", "1.0.5")]
    public class NotifiLib : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.Debug.Log("Notifications loaded");
        }

        private void Init()
        {
            MainCamera = GameObject.Find("Main Camera");
            HUDObj = new GameObject();
            HUDObj2 = new GameObject();
            HUDObj2.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.AddComponent<Canvas>();
            HUDObj.AddComponent<CanvasScaler>();
            HUDObj.AddComponent<GraphicRaycaster>();
            HUDObj.GetComponent<Canvas>().enabled = true;
            HUDObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            HUDObj.GetComponent<Canvas>().worldCamera = MainCamera.GetComponent<Camera>();
            HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
            HUDObj.GetComponent<RectTransform>().position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
            HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z - 4.6f);
            HUDObj.transform.parent = HUDObj2.transform;
            HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            Vector3 eulerAngles = HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
            eulerAngles.y = -270f;
            HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
            HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);
            Testtext = new GameObject
            {
                transform =
                {
                    parent = HUDObj.transform
                }
            }.AddComponent<Text>();
            Testtext.text = "";
            Testtext.fontSize = 30;
            Testtext.font = agency;
            Testtext.rectTransform.sizeDelta = new Vector2(450f, 210f);
            Testtext.alignment = TextAnchor.LowerLeft;
            Testtext.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            Testtext.rectTransform.localPosition = new Vector3(-1f, -1f, -0.5f);
            Testtext.material = AlertText;
            NotifiLib.NotifiText = Testtext;

            Text Text2 = new GameObject
            {
                transform =
                {
                    parent = HUDObj.transform
                }
            }.AddComponent<Text>();
            Text2.text = "";
            Text2.fontSize = 20;
            Text2.font = agency;
            Text2.rectTransform.sizeDelta = new Vector2(450f, 1000f);
            Text2.alignment = TextAnchor.UpperLeft;
            Text2.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            Text2.rectTransform.localPosition = new Vector3(-1f, -0.7f, -0.5f);
            Text2.material = AlertText;
            NotifiLib.ModText = Text2;
        }

        private void FixedUpdate()
        {
            try
            {
                if (!HasInit && GameObject.Find("Main Camera") != null)
                {
                    Init();
                    HasInit = true;
                }
                HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
                HUDObj2.transform.rotation = MainCamera.transform.rotation;
                try
                {
                    Testtext.font = activeFont;
                    ModText.font = activeFont;

                    Testtext.fontStyle = activeFontStyle;
                    ModText.fontStyle = activeFontStyle;
                }
                catch { }
                if (showEnabledModsVR)
                {
                    string lol = "";
                    List<string> alphabetized = new List<string>();
                    foreach (ButtonInfo[] buttonlist in Buttons.buttons)
                    {
                        foreach (ButtonInfo v in buttonlist)
                        {
                            try
                            {
                                if (v.enabled)
                                {
                                    alphabetized.Add((v.overlapText == null) ? v.buttonText : v.overlapText);
                                }
                            }
                            catch { }
                        }
                    }

                    Regex notags = new Regex("<.*?>");
                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => (notags.Replace(s, "")).Length)
                        .ToArray();

                    foreach (string v in sortedButtons)
                    {
                        lol += v + "\n";
                    }
                    ModText.text = lol;
                    ModText.color = GetIndex("Swap GUI Colors").enabled ? GetBDColor(0f) : GetBGColor(0f);
                }
                else
                {
                    ModText.text = "";
                }
                if (lowercaseMode)
                {
                    ModText.text = ModText.text.ToLower();
                    NotifiText.text = NotifiText.text.ToLower();
                }
            } catch { /* Game not initialized */ }
        }

        public static void SendNotification(string NotificationText, int clearTime = -1)
        {
            if (clearTime < 0)
            {
                clearTime = notificationDecayTime;
            }
            if (!disableNotifications)
            {
                try
                {
                    if (NotifiLib.IsEnabled && NotifiLib.PreviousNotifi != NotificationText)
                    {
                        if (!NotificationText.Contains(Environment.NewLine))
                        {
                            NotificationText += Environment.NewLine;
                        }
                        NotifiLib.NotifiText.text = NotifiLib.NotifiText.text + NotificationText;
                        if (lowercaseMode)
                        {
                            NotifiText.text = NotifiText.text.ToLower();
                        }
                        NotifiLib.NotifiText.supportRichText = true;
                        NotifiLib.PreviousNotifi = NotificationText;
                        try
                        {
                            Task.Delay(clearTime).ContinueWith(t => ClearLast());
                        } catch { /* cheeseburger */ }
                    }
                }
                catch
                {
                    UnityEngine.Debug.LogError("Notification failed, object probably nil due to third person ; " + NotificationText);
                }
            }
        }

        public static void ClearAllNotifications()
        {
            //NotifiLib.NotifiText.text = "<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Notifications cleared.</color>" + Environment.NewLine;
            NotifiLib.NotifiText.text = "";
        }

        public static void ClearPastNotifications(int amount)
        {
            string text = "";
            foreach (string text2 in Enumerable.ToArray<string>(Enumerable.Skip<string>(NotifiLib.NotifiText.text.Split(Environment.NewLine.ToCharArray()), amount)))
            {
                if (text2 != "")
                {
                    text = text + text2 + "\n";
                }
            }
            NotifiLib.NotifiText.text = text;
        }

        public static void ClearLast()
        {
            ClearPastNotifications(1);
        }

        private GameObject HUDObj;

        private GameObject HUDObj2;

        private GameObject MainCamera;

        private Text Testtext;

        private Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

        //private int NotificationDecayTime = 144;

        //private int NotificationDecayTimeCounter;

        public static int NoticationThreshold = 30;

        //private string[] Notifilines;

        //private string newtext;

        public static string PreviousNotifi;

        private bool HasInit;

        private static Text NotifiText;
        private static Text ModText;

        public static bool IsEnabled = true;
    }
}
