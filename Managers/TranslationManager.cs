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
            if (translateCache.TryGetValue(text, out var value))
            {
                onTranslated?.Invoke(value);

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
                string postData = JsonConvert.SerializeObject(new { text, lang = language });

                using UnityWebRequest request = new UnityWebRequest($"{PluginInfo.ServerAPI}/translate", "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    Match match = Regex.Match(json, "\"translation\"\\s*:\\s*\"(.*?)\"");
                    if (match.Success)
                    {
                        translation = match.Groups[1].Value;
                        File.WriteAllText(filePath, translation);
                    }
                }
            }
            else
                translation = File.ReadAllText(filePath);

            if (translation != null)
            {
                translateCache.Add(text, translation);
                onTranslated?.Invoke(translation);
            }
        }
    }
}
