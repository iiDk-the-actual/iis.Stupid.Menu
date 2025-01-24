using System;
using System.Collections;
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
            Testtext.verticalOverflow = VerticalWrapMode.Overflow;
            Testtext.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            Testtext.rectTransform.localPosition = new Vector3(-1f, -1f, -0.5f);
            Testtext.material = AlertText;
            NotifiText = Testtext;

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
            ModText = Text2;
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
                                    string buttonText = (v.overlapText == null) ? v.buttonText : v.overlapText;
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
                    }

                    Regex notags = new Regex("<.*?>");
                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => (notags.Replace(s, "")).Length)
                        .ToArray();

                    foreach (string v in sortedButtons)
                        lol += v + "\n";
                    
                    ModText.text = lol;
                    ModText.color = GetIndex("Swap GUI Colors").enabled ? textColor : GetBGColor(0f);
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
                HUDObj.layer = GetIndex("Hide Notifications on Camera").enabled ? 19 : 0;
            } catch { /* Game not initialized */ }
        }

        public static void SendNotification(string NotificationText, int clearTime = -1)
        {
            if (clearTime < 0)
                clearTime = notificationDecayTime;
            
            if (!disableNotifications)
            {
                try
                {
                    if (PreviousNotifi != NotificationText)
                    {
                        if (notificationSoundIndex != 0)
                        {
                            string[] notificationServerNames = new string[]
                            {
                                "none",
                                "pop",
                                "ding",
                                "twitter",
                                "discord",
                                "whatsapp",
                                "grindr",
                                "ios"
                            };
                            Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/" + notificationServerNames[notificationSoundIndex] + ".wav", notificationServerNames[notificationSoundIndex] + ".wav"), buttonClickVolume / 10f);
                        }

                        if (!NotificationText.Contains(Environment.NewLine))
                            NotificationText += Environment.NewLine;
                        
                        if (translate)
                            NotificationText = TranslateText(NotificationText);
                        
                        if (inputTextColor != "green")
                            NotificationText = NotificationText.Replace("<color=green>", "<color=" + inputTextColor + ">");
                        NotifiText.text = NotifiText.text + NotificationText;
                        if (lowercaseMode)
                            NotifiText.text = NotifiText.text.ToLower();

                        NotifiText.supportRichText = true;
                        PreviousNotifi = NotificationText;

                        try
                        {
                            CoroutineManager.RunCoroutine(ClearLast());
                        } catch { /* cheeseburger */ }

                        if (narrateNotifications)
                        {
                            try
                            {
                                Regex notags = new Regex("<.*?>");
                                CoroutineManager.RunCoroutine(NarrateText(notags.Replace(NotificationText, "")));
                            }
                            catch { }
                        }
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
            //NotifiText.text = "<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Notifications cleared.</color>" + Environment.NewLine;
            NotifiText.text = "";
        }

        public static void ClearPastNotifications(int amount)
        {
            string text = "";
            foreach (string text2 in Enumerable.ToArray<string>(Enumerable.Skip<string>(NotifiText.text.Split(Environment.NewLine.ToCharArray()), amount)))
            {
                if (text2 != "")
                    text = text + text2 + "\n";
            }
            NotifiText.text = text;
        }

        public static IEnumerator ClearLast()
        {
            yield return new WaitForSeconds(1);
            ClearPastNotifications(1);
        }

        private GameObject HUDObj;
        private GameObject HUDObj2;

        private GameObject MainCamera;

        private Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

        public static string PreviousNotifi;

        private static Text NotifiText;
        private static Text ModText;
        private Text Testtext;

        private bool HasInit;

    }
}
