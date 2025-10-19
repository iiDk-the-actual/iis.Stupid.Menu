/*
 * ii's Stupid Menu  Managers/AIManager.cs
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

// clanker
using System;
using System.Collections;
using System.IO;
using iiMenu.Menu;
using iiMenu.Mods;
using iiMenu.Notifications;
using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;

namespace iiMenu.Managers
{
    public class AIManager : MonoBehaviour
    {
        public static string SystemPrompt = "";
        public static string URLEncode(string input) => Uri.EscapeDataString(input);

        public static int Duration(string input)
        {
            int count = input.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return (count * 400) + 500;
        }
        public static IEnumerator AskAI(string text)
        {
            if (Time.time < Main.timeMenuStarted + 5f)
                yield break;

            text = URLEncode(text);
            string prompt = URLEncode(SystemPrompt);
            string api = $"https://text.pollinations.ai/{text}?system={prompt}?private=true";

            using UnityWebRequest request = UnityWebRequest.Get(api);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                LogManager.LogError($"Error contacting AI api {request.error}");
                NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> There was an issue generating your response.", 4000);
                Settings.listening = false; 
                yield break;
            }

            string response = request.downloadHandler.text;
            if (Main.dynamicSounds)
                Main.Play2DAudio(Main.LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg"), Main.buttonClickVolume / 10f);
            NotifiLib.SendNotification($"<color=grey>[</color><color=blue>AI</color><color=grey>]</color> {response}", Duration(response));
            Settings.listening = false;
        }
    }
}
