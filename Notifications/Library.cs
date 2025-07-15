using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Menu.Main;

namespace iiMenu.Notifications
{
    // Originally created by lars, he gave me permission
    // Modified by ii, not much though

    public class NotifiLib : MonoBehaviour
    {
        public static NotifiLib instance;
        public GameObject HUDObj;
        public GameObject HUDObj2;

        private GameObject MainCamera;

        private Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

        public static string PreviousNotifi;

        public static Dictionary<string, string> information = new Dictionary<string, string> { };

        public static Text NotifiText;
        public static Text ModText;
        public static Text StatsText;

        private bool HasInit;

        public static int NotifiCounter = 0;

        private void Start()
        {
            instance = this;
            LogManager.Log("Notifications loaded");
        }

        private void Init()
        {
            MainCamera = Camera.main.gameObject;
            HUDObj = new GameObject();
            HUDObj2 = new GameObject
            {
                name = "NOTIFICATIONLIB_HUD_OBJ"
            };
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
            HUDObj.transform.localScale = Vector3.one;
            HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);
            NotifiText = new GameObject
            {
                transform =
                {
                    parent = HUDObj.transform
                }
            }.AddComponent<Text>();
            NotifiText.text = "";
            NotifiText.fontSize = 30;
            NotifiText.font = AgencyFB;
            NotifiText.rectTransform.sizeDelta = new Vector2(450f, 210f);
            NotifiText.alignment = TextAnchor.LowerLeft;
            NotifiText.verticalOverflow = VerticalWrapMode.Overflow;
            NotifiText.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            NotifiText.rectTransform.localPosition = new Vector3(-1f, -1f, -0.5f);
            NotifiText.material = AlertText;

            ModText = new GameObject
            {
                transform =
                {
                    parent = HUDObj.transform
                }
            }.AddComponent<Text>();
            ModText.text = "";
            ModText.fontSize = 20;
            ModText.font = AgencyFB;
            ModText.rectTransform.sizeDelta = new Vector2(450f, 1000f);
            ModText.alignment = TextAnchor.UpperLeft;
            ModText.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            ModText.rectTransform.localPosition = new Vector3(-1f, -1f, -0.5f);
            ModText.material = AlertText;

            StatsText = new GameObject
            {
                transform =
                {
                    parent = HUDObj.transform
                }
            }.AddComponent<Text>();
            StatsText.text = "";
            StatsText.fontSize = 30;
            StatsText.font = AgencyFB;
            StatsText.rectTransform.sizeDelta = new Vector2(450f, 1000f);
            StatsText.alignment = TextAnchor.UpperRight;
            StatsText.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            StatsText.rectTransform.localPosition = new Vector3(-1f, -1f, 0.5f);
            StatsText.material = AlertText;
        }

        private void FixedUpdate()
        {
            try
            {
                if (!HasInit && Camera.main != null)
                {
                    Init();
                    HasInit = true;
                }

                HUDObj.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = highQualityText ? 2f : 1f;

                HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
                HUDObj2.transform.rotation = MainCamera.transform.rotation;
                try
                {
                    ModText.font = activeFont;
                    ModText.fontStyle = activeFontStyle;

                    NotifiText.font = activeFont;
                    NotifiText.fontStyle = activeFontStyle;

                    StatsText.font = activeFont;
                    StatsText.fontStyle = activeFontStyle;

                    if (advancedArraylist)
                        ModText.fontStyle = (FontStyle)((int)activeFontStyle % 2);
                }
                catch { }
                ModText.rectTransform.localPosition = new Vector3(-1f, -1f, flipArraylist ? 0.5f : -0.5f);
                ModText.alignment = flipArraylist ? TextAnchor.UpperRight : TextAnchor.UpperLeft;

                StatsText.rectTransform.localPosition = new Vector3(-1f, -1f, flipArraylist ? -0.5f : 0.5f);
                StatsText.alignment = flipArraylist ? TextAnchor.UpperLeft : TextAnchor.UpperRight;

                if (information.Count > 0)
                {
                    Color targetColor = GetIndex("Swap GUI Colors").enabled ? GetBDColor(0f) : GetBGColor(0f);

                    List<string> statsAlphabetized = information
                        .Select(item => $"<color=#{ColorToHex(targetColor)}>{item.Key}</color> <color=#{ColorToHex(textColor)}>{item.Value}</color>")
                        .OrderByDescending(item => item.Length)
                        .ToList();

                    StatsText.text = string.Join("\n", statsAlphabetized.ToArray());
                    StatsText.color = Color.white;

                    if (lowercaseMode)
                        StatsText.text = StatsText.text.ToLower();
                } else
                    StatsText.text = "";

                if (showEnabledModsVR)
                {
                    string enabledModsText = "";
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

                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => UI.ExternalCalcSize(new GUIContent(NoRichtextTags(s))).x)
                        .ToArray();

                    int index = 0;
                    foreach (string v in sortedButtons)
                    {
                        Color targetColor = GetIndex("Swap GUI Colors").enabled ? GetBDColor(index * -0.1f) : GetBGColor(index * -0.1f);

                        if (advancedArraylist)
                            enabledModsText += (flipArraylist ?
                                  $"<color=#{ColorToHex(textColor)}>{v}</color><color=#{ColorToHex(targetColor)}> |</color>"
                                : $"<color=#{ColorToHex(GetBGColor(index * -0.1f))}>| </color><color=#{ColorToHex(textColor)}>{v}</color>") + "\n";
                        else
                            enabledModsText += v + "\n";

                        index++;
                    }

                    ModText.text = enabledModsText;
                    ModText.color = GetIndex("Swap GUI Colors").enabled ? textColor : GetBGColor(0f);
                }
                else
                    ModText.text = "";

                if (lowercaseMode)
                {
                    ModText.text = ModText.text.ToLower();
                    NotifiText.text = NotifiText.text.ToLower();
                }
                HUDObj.layer = GetIndex("Hide Notifications on Camera").enabled ? 19 : 0;
            }
            catch (Exception e) { LogManager.Log(e); }
        }

        public static void SendNotification(string NotificationText, int clearTime = -1)
        {
            if (clearTime < 0)
                clearTime = notificationDecayTime;

            if (!disableNotifications)
            {
                try
                {
                    if (translate)
                    {
                        if (translateCache.ContainsKey(NotificationText))
                            NotificationText = TranslateText(NotificationText);
                        else
                        {
                            TranslateText(NotificationText, delegate { SendNotification(NotificationText, clearTime); });
                            return;
                        }
                    }

                    if (notificationSoundIndex != 0 && (Time.time > (timeMenuStarted + 5f)))
                        Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/" + Settings.notificationSounds.Values.ToArray()[notificationSoundIndex] + ".wav", Settings.notificationSounds.Values.ToArray()[notificationSoundIndex] + ".wav"), buttonClickVolume / 10f);

                    if (inputTextColor != "green")
                        NotificationText = NotificationText.Replace("<color=green>", "<color=" + inputTextColor + ">");

                    if (PreviousNotifi == NotificationText && stackNotifications)
                    {
                        NotifiCounter++;
                        NotifiText.text = $"{NotificationText} {(NotifiCounter >= 1 ? $"<color=grey>(x{NotifiCounter + 1})</color>" : "")}";

                        if (clearCoroutines.Count > 0)
                            CancelClear(clearCoroutines[0]);
                    }
                    else
                    {
                        NotifiCounter = 0;

                        PreviousNotifi = NotificationText;
                        if (!NotificationText.Contains(Environment.NewLine))
                            NotificationText += Environment.NewLine;
                        NotifiText.text += NotificationText;
                    }

                    CoroutineManager.RunCoroutine(TrackCoroutine(ClearHolder(clearTime / 1000f)));

                    if (lowercaseMode)
                        NotifiText.text = NotifiText.text.ToLower();

                    NotifiText.supportRichText = true;

                    if (narrateNotifications)
                    {
                        try
                        {
                            CoroutineManager.RunCoroutine(NarrateText(NoRichtextTags(NotificationText, "")));
                        }
                        catch { }
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogError($"Notification failed, object probably nil due to third person ; {NotificationText} {e.Message}");
                }
            }
        }

        public static void ClearAllNotifications() =>
            NotifiText.text = "";

        public static void ClearPastNotifications(int amount)
        {
            string text = "";
            foreach (string text2 in Enumerable.Skip(NotifiText.text.Split(Environment.NewLine.ToCharArray()), amount))
            {
                if (text2 != "")
                    text = text + text2 + "\n";
            }
            NotifiText.text = text;
        }

        private static IEnumerator TrackCoroutine(IEnumerator routine)
        {
            Coroutine self = null;

            IEnumerator Wrapper()
            {
                self = CoroutineManager.instance.StartCoroutine(routine);
                clearCoroutines.Add(self);
                yield return self;
                clearCoroutines.Remove(self);
            }

            yield return Wrapper();
        }

        public static IEnumerator ClearHolder(float time = 1f)
        {
            yield return new WaitForSeconds(time);
            ClearPastNotifications(1);
        }

        public static void CancelClear(Coroutine coroutine)
        {
            if (clearCoroutines.Contains(coroutine))
            {
                clearCoroutines.Remove(coroutine);
                CoroutineManager.instance.StopCoroutine(coroutine);
            }
        }

        public static List<Coroutine> clearCoroutines = new List<Coroutine> { };

    }
}
