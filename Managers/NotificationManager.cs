/*
 * ii's Stupid Menu  Managers/NotificationManager.cs
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

using GorillaLocomotion;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Menu;
using iiMenu.Mods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;

namespace iiMenu.Managers
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        public GameObject canvas;
        private GameObject mainCamera;
        private Material textMaterial;

        public static string PreviousNotifi;
        /// <summary>
        /// Elements in this dictionary are displayed on the user's
        /// screen as an overlay in the top left or right corner of the view.
        /// </summary>
        public static readonly Dictionary<string, string> information = new Dictionary<string, string>();

        public static TextMeshProUGUI notificationText;
        public static TextMeshProUGUI arraylistText;
        public static TextMeshProUGUI informationText;

        private bool hasInitialized;
        public static bool noRichText;
        public static bool soundOnError;
        public static bool noPrefix;
        public static bool narrateNotifications;

        public static int NotifiCounter;
        private static readonly List<Coroutine> clearCoroutines = new List<Coroutine>();

        private void Start()
        {
            Instance = this;
            LogManager.Log("Notifications loaded");
        }

        private void Init()
        {
            mainCamera = Camera.main.gameObject;

            GameObject canvasParent = new GameObject("iiMenu_NotificationParent");
            canvasParent.transform.position = mainCamera.transform.position;

            canvas = new GameObject("Canvas");
            canvas.AddComponent<Canvas>();
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            Canvas canvasComponent = canvas.GetComponent<Canvas>();
            canvasComponent.enabled = true;
            canvasComponent.renderMode = RenderMode.WorldSpace;
            canvasComponent.worldCamera = mainCamera.GetComponent<Camera>();

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(5f, 5f);
            canvasRect.position = mainCamera.transform.position;

            canvas.transform.parent = canvasParent.transform;
            canvasRect.localPosition = new Vector3(0f, 0f, 1.6f);
            canvasRect.localScale = Vector3.one;

            Vector3 rotation = canvasRect.rotation.eulerAngles;
            rotation.y = -270f;
            canvasRect.rotation = Quaternion.Euler(rotation);

            textMaterial = new Material(Shader.Find("GUI/Text Shader"));

            notificationText = CreateText(canvas.transform, new Vector3(-1f, -1f, -0.5f),
                new Vector2(450f, 210f), 30, TextAlignmentOptions.BottomLeft);

            arraylistText = CreateText(canvas.transform, new Vector3(-1f, -1f, -0.5f),
                new Vector2(450f, 1000f), 20, TextAlignmentOptions.TopLeft);

            informationText = CreateText(canvas.transform, new Vector3(-1f, -1f, 0.5f),
                new Vector2(450f, 1000f), 30, TextAlignmentOptions.TopRight);

            StartCoroutine(SetShaderAfterInit());
        }

        private TextMeshProUGUI CreateText(Transform parent, Vector3 localPos, Vector2 size, int fontSize, TextAlignmentOptions anchor)
        {
            GameObject textObj = new GameObject { transform = { parent = parent } };
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

            text.SafeSetText("");
            text.fontSize = fontSize;
            text.SafeSetFont(AgencyFB);
            text.rectTransform.sizeDelta = size;
            text.alignment = anchor;
            text.overflowMode = anchor == TextAlignmentOptions.BottomLeft ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
            text.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            text.rectTransform.localPosition = localPos;
            text.material = textMaterial;
            text.characterSpacing = -9f;

            return text;
        }

        private float updateArraylistTimer;
        private void FixedUpdate()
        {
            try
            {
                if (!hasInitialized && Camera.main != null)
                {
                    Init();
                    hasInitialized = true;
                }

                canvas.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 2f;

                canvas.transform.position = mainCamera.transform.TransformPoint(0f, 0f, 1.6f);
                canvas.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(0, 90, 0);
                canvas.transform.localScale = Vector3.one * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

                try
                {
                    arraylistText.SafeSetFont(activeFont);
                    arraylistText.SafeSetFontStyle(activeFontStyle);
                    arraylistText.SafeSetFontSize(arraylistScale);
                    arraylistText.Chams();

                    notificationText.SafeSetFont(activeFont);
                    notificationText.SafeSetFontStyle(activeFontStyle);
                    notificationText.SafeSetFontSize(notificationScale);
                    notificationText.rectTransform.localPosition = new Vector3(-1f, disableNotifications ? -100f : -1f, -0.5f);
                    notificationText.Chams();

                    informationText.SafeSetFont(activeFont);
                    informationText.SafeSetFontStyle(activeFontStyle);
                    informationText.SafeSetFontSize(overlayScale);
                    informationText.Chams();

                    FollowMenuSettings(arraylistText);
                    FollowMenuSettings(notificationText);
                    FollowMenuSettings(informationText);
                }
                catch { }

                arraylistText.rectTransform.localPosition = new Vector3(-1f, -1f, flipArraylist ? 0.5f : -0.5f);
                arraylistText.alignment = flipArraylist ? TextAlignmentOptions.TopRight : TextAlignmentOptions.TopLeft;

                informationText.rectTransform.localPosition = new Vector3(-1f, -1f, flipArraylist ? -0.5f : 0.5f);
                informationText.alignment = flipArraylist ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.TopRight;

                if (information.Count > 0)
                {
                    Color targetColor = Buttons.GetIndex("Swap GUI Colors").enabled ? buttonColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor();

                    List<string> statsLines = information
                        .Select(item => $"<color=#{ColorToHex(targetColor)}>{item.Key}</color> <color=#{ColorToHex(textColors[1].GetColor(0))}>{item.Value}</color>")
                        .OrderByDescending(item => informationText.GetPreferredValues(NoRichtextTags(item)).x)
                        .ToList();

                    informationText.SafeSetText(string.Join("\n", statsLines));
                    informationText.color = Color.white;
                }
                else if (!informationText.text.IsNullOrEmpty())
                    informationText.SafeSetText("");

                if (showEnabledModsVR)
                {
                    if (Time.time > updateArraylistTimer)
                    {
                        updateArraylistTimer = Time.time + (advancedArraylist ? 0.1f : 0.5f);
                        List<string> enabledMods = new List<string>();
                        int categoryIndex = 0;

                        foreach (ButtonInfo[] buttonList in Buttons.buttons)
                        {
                            foreach (ButtonInfo button in buttonList)
                            {
                                try
                                {
                                    if (!button.enabled || (hideSettings && (!hideSettings ||
                                                                             Buttons.categoryNames[categoryIndex]
                                                                                 .Contains("Settings")))) continue;
                                    string buttonText = button.overlapText ?? button.buttonText;

                                    if (inputTextColor != "green")
                                        buttonText = buttonText.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + inputTextColor + ">");

                                    buttonText = FixTMProTags(buttonText);
                                    buttonText = FollowMenuSettings(buttonText);
                                    enabledMods.Add(buttonText);
                                }
                                catch { }
                            }
                            categoryIndex++;
                        }

                        string[] sortedMods = enabledMods
                            .OrderByDescending(s => arraylistText.GetPreferredValues(NoRichtextTags(s)).x)
                            .ToArray();

                        string modListText = "";
                        for (int i = 0; i < sortedMods.Length; i++)
                        {
                            if (advancedArraylist)
                                modListText += (flipArraylist ?
                                /* Flipped */ $"<mark=#{ColorToHex(backgroundColor.GetCurrentColor(i * -0.1f))}80>{sortedMods[i]}</mark><mark=#{ColorToHex(buttonColors[1].GetCurrentColor(i * -0.1f))}> </mark>" :
                                /* Normal  */ $"<mark=#{ColorToHex(buttonColors[1].GetCurrentColor(i * -0.1f))}> </mark><mark=#{ColorToHex(backgroundColor.GetCurrentColor(i * -0.1f))}80>{sortedMods[i]}</mark>") + "\n";
                            else
                                modListText += sortedMods[i] + "\n";
                        }

                        arraylistText.SafeSetText(modListText);
                        arraylistText.color = Buttons.GetIndex("Swap GUI Colors").enabled ? textColors[1].GetColor(0) : backgroundColor.GetCurrentColor();
                    }
                }
                else if (!arraylistText.text.IsNullOrEmpty())
                    arraylistText.SafeSetText("");

                if (lowercaseMode)
                {
                    if (!arraylistText.text.IsNullOrEmpty())
                        arraylistText.SafeSetText(arraylistText.text.ToLower());

                    if (!notificationText.text.IsNullOrEmpty())
                        notificationText.SafeSetText(notificationText.text.ToLower());

                    if (!informationText.text.IsNullOrEmpty())
                        informationText.SafeSetText(informationText.text.ToLower());
                }

                if (uppercaseMode)
                {
                    if (!arraylistText.text.IsNullOrEmpty())
                        arraylistText.SafeSetText(arraylistText.text.ToUpper());

                    if (!notificationText.text.IsNullOrEmpty())
                        notificationText.SafeSetText(notificationText.text.ToUpper());

                    if (!informationText.text.IsNullOrEmpty())
                        informationText.SafeSetText(informationText.text.ToUpper());
                }

                canvas.layer = Buttons.GetIndex("Hide Notifications on Camera").enabled ? 19 : 0;
            }
            catch (Exception e) { LogManager.Log(e); }
        }

        /// <summary>
        /// Displays a notification message to the user, with optional customization for display duration and
        /// formatting.
        /// </summary>
        /// <remarks>If the notification text matches the previous notification and notification stacking
        /// is enabled, the notification count is incremented instead of displaying a new message. The method applies
        /// various formatting and translation options based on current settings, and may play a notification sound or
        /// narrate the message if those features are enabled. Rich text support and text casing are also configurable.
        /// This method is thread-unsafe and should be called from the main UI thread.</remarks>
        /// <param name="notificationText">The text of the notification to display. May include rich text formatting tags. If translation is enabled,
        /// the text will be translated before display.</param>
        /// <param name="clearTime">The time, in milliseconds, before the notification is cleared. Specify -1 to use the default notification
        /// decay time.</param>
        public static void SendNotification(string notificationText, int clearTime = -1)
        {
            if (clearTime < 0)
                clearTime = notificationDecayTime;

            if (disableNotifications && !Buttons.GetIndex("Conduct Notifications").enabled) return;
            try
            {
                if (translate)
                {
                    if (TranslationManager.translateCache.ContainsKey(notificationText))
                        notificationText = TranslationManager.TranslateText(notificationText);
                    else
                    {
                        TranslationManager.TranslateText(notificationText, delegate { SendNotification(notificationText, clearTime); });
                        return;
                    }
                }

                if (notificationSoundIndex != 0 && (!soundOnError || notificationText.Contains("<color=red>ERROR</color>")) && Time.time > timeMenuStarted + 5f)
                    PlayNotificationSound();

                if (inputTextColor != "green")
                    notificationText = notificationText.Replace("<color=green>", "<color=" + inputTextColor + ">");

                notificationText = FixTMProTags(notificationText);

                if (hideBrackets)
                    notificationText = notificationText.Replace("[", "").Replace("]", "");

                notificationText = notificationText.TrimEnd('\n', '\r');

                if (PreviousNotifi == notificationText && stackNotifications)
                {
                    NotifiCounter++;

                    string[] lines = NotificationManager.notificationText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length > 0)
                    {
                        string lastLine = lines[^1];
                        int counterIndex = lastLine.IndexOf(" <color=grey>(x", StringComparison.Ordinal);
                        if (counterIndex > 0)
                            lastLine = lastLine[..counterIndex];

                        lines[^1] = $"{lastLine} <color=grey>(x{NotifiCounter + 1})</color>";
                        NotificationManager.notificationText.SafeSetText(string.Join(Environment.NewLine, lines));
                    }

                    if (clearCoroutines.Count > 0)
                        CancelClear(clearCoroutines[0]);
                }
                else
                {
                    NotifiCounter = 0;
                    PreviousNotifi = notificationText;

                    if (!string.IsNullOrEmpty(NotificationManager.notificationText.text))
                    {
                        string currentText = NotificationManager.notificationText.text.TrimEnd('\n', '\r');
                        NotificationManager.notificationText.SafeSetText(currentText + Environment.NewLine + notificationText);
                    }
                    else
                        NotificationManager.notificationText.SafeSetText(notificationText);
                }

                CoroutineManager.instance.StartCoroutine(TrackCoroutine(ClearHolder(clearTime / 1000f)));

                if (noRichText)
                    NotificationManager.notificationText.SafeSetText(NoRichtextTags(NotificationManager.notificationText.text));

                if (lowercaseMode)
                    NotificationManager.notificationText.SafeSetText(NotificationManager.notificationText.text.ToLower());

                if (uppercaseMode)
                    NotificationManager.notificationText.SafeSetText(NotificationManager.notificationText.text.ToUpper());

                NotificationManager.notificationText.richText = !noRichText;

                if (narrateNotifications)
                    NarrateText(NoRichtextTags(noPrefix ? RemovePrefix(notificationText) : notificationText));
            }
            catch (Exception e)
            {
                LogManager.LogError($"Notification failed, object probably nil due to third person ; {notificationText} {e.Message}");
            }
        }

        public static void PlayNotificationSound() =>
            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/Notifications/{Settings.notificationSounds.Values.ToArray()[notificationSoundIndex]}.ogg", $"Audio/Menu/Notifications/{Settings.notificationSounds.Values.ToArray()[notificationSoundIndex]}.ogg"), buttonClickVolume / 10f);

        /// <summary>
        /// Clears all active notifications and stops any ongoing notification clearing operations.
        /// </summary>
        /// <remarks>Call this method to immediately remove all notification text and halt any scheduled
        /// notification clearing. This method is typically used to reset the notification system or when notifications
        /// are no longer relevant.</remarks>
        public static void ClearAllNotifications()
        {
            notificationText.SafeSetText("");

            foreach (Coroutine coroutine in clearCoroutines)
                CoroutineManager.instance.StopCoroutine(coroutine);

            clearCoroutines.Clear();
        }

        /// <summary>
        /// Removes a specified number of past notification entries from the notification text.
        /// </summary>
        /// <param name="amount">The number of past notification lines to remove. Must be zero or greater. If the value is greater than or
        /// equal to the total number of notification lines, all notifications are cleared.</param>
        public static void ClearPastNotifications(int amount)
        {
            if (string.IsNullOrEmpty(notificationText.text))
                return;

            string[] lines = notificationText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (amount >= lines.Length)
            {
                notificationText.SafeSetText("");
                return;
            }

            List<string> remainingLines = new List<string>();
            for (int i = amount; i < lines.Length; i++)
                remainingLines.Add(lines[i]);

            notificationText.SafeSetText(string.Join(Environment.NewLine, remainingLines));
            notificationText.SafeSetText(notificationText.text.TrimEnd('\n', '\r'));
        }

        private static IEnumerator TrackCoroutine(IEnumerator routine)
        {
            IEnumerator Wrapper()
            {
                Coroutine self = CoroutineManager.instance.StartCoroutine(routine);
                clearCoroutines.Add(self);
                yield return self;
                clearCoroutines.Remove(self);
            }

            yield return Wrapper();
        }

        private static IEnumerator ClearHolder(float time = 1f)
        {
            yield return new WaitForSeconds(time);
            ClearPastNotifications(1);
        }

        private IEnumerator SetShaderAfterInit()
        {
            yield return null; yield return null; yield return null; yield return null; yield return null;

            notificationText.Chams();
            arraylistText.Chams();
            informationText.Chams();
        }

        private static void CancelClear(Coroutine coroutine)
        {
            if (!clearCoroutines.Contains(coroutine)) return;
            clearCoroutines.Remove(coroutine);
            CoroutineManager.instance.StopCoroutine(coroutine);
        }

        private static string RemovePrefix(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string pattern = @"^<color=grey>\[</color><color=[^>]+>.*?</color><color=grey>\]</color> ";
            return System.Text.RegularExpressions.Regex.Replace(text, pattern, "");
        }
    }
}