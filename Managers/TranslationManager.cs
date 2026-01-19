/*
 * ii's Stupid Menu  Managers/TranslationManager.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;
using static iiMenu.Menu.Main;

namespace iiMenu.Managers
{
    public class TranslationManager
    {
        public static readonly Dictionary<string, float> waitingForTranslate = new Dictionary<string, float>();
        public static readonly Dictionary<string, string> translateCache = new Dictionary<string, string>();

        /// <summary>
        /// Target language for translation. Format: "en", "fr", "de", "jp"
        /// </summary>
        public static string language;

        public static string TranslateText(string input, Action<string> onTranslated = null)
        {
            if (translateCache.TryGetValue(input, out var text))
                return text;
            if (!waitingForTranslate.ContainsKey(input))
            {
                waitingForTranslate.Add(input, Time.time + 10f);
                CoroutineManager.instance.StartCoroutine(GetTranslation(input, onTranslated));
            }
            else
            {
                if (!(Time.time > waitingForTranslate[input])) return "Loading...";
                waitingForTranslate.Remove(input);

                waitingForTranslate.Add(input, Time.time + 10f);
                CoroutineManager.instance.StartCoroutine(GetTranslation(input, onTranslated));
            }

            return "Loading...";
        }

        public static IEnumerator GetTranslation(string text, Action<string> onTranslated = null)
        {
            if (string.IsNullOrEmpty(text))
                yield break;

            if (translateCache.TryGetValue(text, out var cached))
            {
                onTranslated?.Invoke(cached);
                yield break;
            }

            string fileName = GetSHA256(text) + ".txt";
            string directoryPath = $"{PluginInfo.BaseDirectory}/TranslationData{language.ToUpper()}";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, fileName);
            string translation = null;

            if (!File.Exists(filePath))
            {
                string cleanText = Regex.Replace(text, @"([""'$`\\])", "\\$1");
                cleanText = cleanText[..Mathf.Min(cleanText.Length, 4096)];

                string cleanLang = Regex.Replace(language, @"[^a-zA-Z0-9]", "");
                cleanLang = cleanLang[..Mathf.Min(cleanLang.Length, 6)];

                string url =
                    "https://translate.googleapis.com/translate_a/single" +
                    "?client=gtx" +
                    "&sl=auto" +
                    $"&tl={cleanLang}" +
                    "&dt=t" +
                    $"&q={UnityWebRequest.EscapeURL(cleanText)}";

                using UnityWebRequest request = UnityWebRequest.Get(url);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var parsed = JsonConvert.DeserializeObject<List<object>>(request.downloadHandler.text);
                        var sentences = parsed[0] as JArray;

                        StringBuilder sb = new StringBuilder();

                        foreach (var sentence in sentences)
                            sb.Append(sentence[0]);

                        translation = sb.ToString();

                        File.WriteAllText(filePath, translation);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Translation parse error: {e}");
                    }
                }
                else
                    Debug.LogError($"Translation request failed: {request.error}");
            }
            else
                translation = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(translation)) yield break;
            translateCache[text] = translation;
            onTranslated?.Invoke(translation);
        }
    }
}
