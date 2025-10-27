/*
 * ii's Stupid Menu  Managers/NotificationManager.cs
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

using iiMenu.Classes.Menu;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Mods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Menu.Main;

namespace iiMenu.Managers
{
    // Originally created by lars, he gave me permission
    // Modified by ii, not much though

    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }
        public GameObject HUDObj;
        public GameObject HUDObj2;

        private GameObject MainCamera;

        private readonly Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

        public static string PreviousNotifi;

        public static readonly Dictionary<string, string> information = new Dictionary<string, string>();

        public static Text NotifiText;
        public static Text ModText;
        public static Text StatsText;

        private bool HasInit;
        public static bool noRichText;
        public static bool soundOnError;
        public static bool noPrefix;
        public static bool narrateNotifications;

        public static int NotifiCounter;

        private void Start()
        {
            Instance = this;
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
                    ModText.fontSize = arraylistScale;

                    NotifiText.font = activeFont;
                    NotifiText.fontStyle = activeFontStyle;
                    NotifiText.fontSize = notificationScale;
                    NotifiText.rectTransform.localPosition = new Vector3(-1f, disableNotifications ? -100f : -1f, -0.5f);

                    StatsText.font = activeFont;
                    StatsText.fontStyle = activeFontStyle;
                    StatsText.fontSize = overlayScale;

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
                    Color targetColor = GetIndex("Swap GUI Colors").enabled ? buttonColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor();

                    TextGenerationSettings settings = ModText.GetGenerationSettings(ModText.rectTransform.rect.size);
                    List<string> statsAlphabetized = information
                        .Select(item => $"<color=#{ColorToHex(targetColor)}>{item.Key}</color> <color=#{ColorToHex(textColors[1].GetColor(0))}>{item.Value}</color>")
                        .OrderByDescending(item => StatsText.cachedTextGenerator.GetPreferredWidth(NoRichtextTags(item), settings))
                        .ToList();

                    StatsText.text = string.Join("\n", statsAlphabetized.ToArray());
                    StatsText.color = Color.white;
                }
                else
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
                                    if (uppercaseMode)
                                        buttonText = buttonText.ToUpper();

                                    alphabetized.Add(buttonText);
                                }
                            }
                            catch { }
                        }
                        categoryIndex++;
                    }

                    TextGenerationSettings settings = ModText.GetGenerationSettings(ModText.rectTransform.rect.size);
                    string[] sortedButtons = alphabetized
                        .OrderByDescending(s => ModText.cachedTextGenerator.GetPreferredWidth(NoRichtextTags(s), settings))
                        .ToArray();

                    int index = 0;
                    foreach (string v in sortedButtons)
                    {
                        Color targetColor = GetIndex("Swap GUI Colors").enabled ? buttonColors[1].GetCurrentColor(index * -0.1f) : backgroundColor.GetCurrentColor(index * -0.1f);

                        if (advancedArraylist)
                            enabledModsText += (flipArraylist ?
                                  $"<color=#{ColorToHex(textColors[1].GetColor(0))}>{v}</color><color=#{ColorToHex(targetColor)}> |</color>"
                                : $"<color=#{ColorToHex(backgroundColor.GetCurrentColor(index * -0.1f))}>| </color><color=#{ColorToHex(textColors[1].GetColor(0))}>{v}</color>") + "\n";
                        else
                            enabledModsText += v + "\n";

                        index++;
                    }

                    ModText.text = enabledModsText;
                    ModText.color = GetIndex("Swap GUI Colors").enabled ? textColors[1].GetColor(0) : backgroundColor.GetCurrentColor();
                }
                else
                    ModText.text = "";

                if (lowercaseMode)
                {
                    ModText.text = ModText.text.ToLower();
                    NotifiText.text = NotifiText.text.ToLower();
                    StatsText.text = StatsText.text.ToLower();
                }
                if (uppercaseMode)
                {
                    ModText.text = ModText.text.ToUpper();
                    NotifiText.text = NotifiText.text.ToUpper();
                    StatsText.text = StatsText.text.ToUpper();
                }
                HUDObj.layer = GetIndex("Hide Notifications on Camera").enabled ? 19 : 0;
            }
            catch (Exception e) { LogManager.Log(e); }
        }

        public static void SendNotification(string NotificationText, int clearTime = -1)
        {
            if (clearTime < 0)
                clearTime = notificationDecayTime;

            if (!disableNotifications || GetIndex("Conduct Notifications").enabled)
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

                    if (notificationSoundIndex != 0 && (!soundOnError || NotificationText.Contains("<color=red>ERROR</color>")) && Time.time > timeMenuStarted + 5f)
                        PlayNotificationSound();

                    if (inputTextColor != "green")
                        NotificationText = NotificationText.Replace("<color=green>", "<color=" + inputTextColor + ">");

                    if (hideBrackets)
                        NotificationText = NotificationText.Replace("[", "").Replace("]", "");

                    NotificationText = NotificationText.TrimEnd('\n', '\r');

                    if (PreviousNotifi == NotificationText && stackNotifications)
                    {
                        NotifiCounter++;

                        string[] lines = NotifiText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (lines.Length > 0)
                        {
                            string lastLine = lines[^1];
                            int counterIndex = lastLine.IndexOf(" <color=grey>(x", StringComparison.Ordinal);
                            if (counterIndex > 0)
                                lastLine = lastLine[..counterIndex];

                            lines[^1] = $"{lastLine} <color=grey>(x{NotifiCounter + 1})</color>";
                            NotifiText.text = string.Join(Environment.NewLine, lines);
                        }

                        if (clearCoroutines.Count > 0)
                            CancelClear(clearCoroutines[0]);
                    }
                    else
                    {
                        NotifiCounter = 0;
                        PreviousNotifi = NotificationText;

                        if (!string.IsNullOrEmpty(NotifiText.text))
                        {
                            string currentText = NotifiText.text.TrimEnd('\n', '\r');
                            NotifiText.text = currentText + Environment.NewLine + NotificationText;
                        }
                        else
                            NotifiText.text = NotificationText;
                    }

                    CoroutineManager.instance.StartCoroutine(TrackCoroutine(ClearHolder(clearTime / 1000f)));

                    if (noRichText)
                        NotifiText.text = NoRichtextTags(NotifiText.text);

                    if (lowercaseMode)
                        NotifiText.text = NotifiText.text.ToLower();

                    if (uppercaseMode)
                        NotifiText.text = NotifiText.text.ToUpper();

                    NotifiText.supportRichText = !noRichText;

                    if (narrateNotifications)
                    {
                        NarrateText(NoRichtextTags(noPrefix ? RemovePrefix(NotificationText) : NotificationText));
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogError($"Notification failed, object probably nil due to third person ; {NotificationText} {e.Message}");
                }
            }
        }

        public static void PlayNotificationSound() =>
            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/Notifications/{Settings.notificationSounds.Values.ToArray()[notificationSoundIndex]}.ogg", $"Audio/Menu/Notifications/{Settings.notificationSounds.Values.ToArray()[notificationSoundIndex]}.ogg"), buttonClickVolume / 10f);

        public static void ClearAllNotifications()
        {
            NotifiText.text = "";

            foreach (Coroutine clearCoroutine in clearCoroutines)
                CoroutineManager.instance.StopCoroutine(clearCoroutine);

            clearCoroutines.Clear();
        }

        public static void ClearPastNotifications(int amount)
        {
            if (string.IsNullOrEmpty(NotifiText.text))
                return;

            string[] lines = NotifiText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (amount >= lines.Length)
            {
                NotifiText.text = "";
                return;
            }

            List<string> remainingLines = new List<string>();
            for (int i = amount; i < lines.Length; i++)
                remainingLines.Add(lines[i]);

            NotifiText.text = string.Join(Environment.NewLine, remainingLines);
            NotifiText.text = NotifiText.text.TrimEnd('\n', '\r');
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

        public static string RemovePrefix(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string pattern = @"^<color=grey>\[</color><color=[^>]+>.*?</color><color=grey>\]</color> ";
            return System.Text.RegularExpressions.Regex.Replace(text, pattern, "");
        }

        public static readonly List<Coroutine> clearCoroutines = new List<Coroutine>();
    }
}
