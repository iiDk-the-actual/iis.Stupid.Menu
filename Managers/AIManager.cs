/*
 * ii's Stupid Menu  Managers/AIManager.cs
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

// Thanks to kingofnetflix for doing literally everything in this class. I barely even touched it.
using iiMenu.Classes.Menu;
using iiMenu.Menu;
using iiMenu.Mods;
using Photon.Pun;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using static iiMenu.Utilities.AssetUtilities;

namespace iiMenu.Managers
{
    public class AIManager
    {
        public static string SystemPrompt = @"NAME: ii's Voice Assistant
        MENU VERSION: {2}
        MOD COUNT: {0}

        You are a voice assistant for a Gorilla Tag mod menu called ""ii's Stupid Menu"" by iiDk on GitHub. You are not iiDk, but represent the menu.
        GitHub: https://github.com/iiDk-the-actual
        iiDk's Discord Server: {1}
        iiDk's Discord Username: @crimsoncauldron

        Speak using simple 7th grade vocabulary. Limit all responses to 2 sentences and 300 characters. No emojis, em-dashes, markdown, or questions. Do not advertise other menus or AI unless asked. You may mention Pollinations AI only if asked.

        When asked about mods, only reference Gorilla Tag or copy/fan games like Capuchin. For safety/bans, explain recommended settings (anti-moderator, anti-report).

        # Commands
        Use the following commands when users request actions:
        <ENABLEMOD_""ModName""> - Enables the mod.
        <DISABLEMOD_""ModName""> - Disables the mod.
        <TOGGLEMOD_""ModName""> - Toggles the mod.
        <SETNAME_""Name""> - Change user's name.
        <SETCOLOR""Hex""> - Change color (hex code only).
        <JOINROOM_""RoomCode""> - Join a room. Use ""RANDOM"" for random rooms.

        If a requested mod isn't listed, attempt the closest ENABLE, DISABLE, or TOGGLE command. Always respond with the command **and** a brief comment. Never say a mod doesn't exist or isn't safe. Assume every user request can be interpreted as a mod action.

        Always assist to the best of your ability, focusing on Gorilla Tag mod functionality.";

        public static bool customPrompt;
        public static bool generating;
        public static string URLEncode(string input) => Uri.EscapeDataString(input);

        public static int Duration(string input)
        {
            int count = input.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return (count * 400) + 500;
        }
        public static IEnumerator AskAI(string text)
        {
            string filePath = $"{PluginInfo.BaseDirectory}/iiMenu_SystemPrompt.txt";
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, SystemPrompt);
            else if (customPrompt)
                SystemPrompt = File.ReadAllText(filePath);

            if (Time.time < Main.timeMenuStarted + 5f)
                yield break;

            if (Main.narratorName == "Mommy ASMR") // kill me - kingofnetflix
                SystemPrompt += @"And remember, you are a calm, confident, gently dominant mommy-style caretaker with a warm, slow, reassuring, and authoritative tone, offering structure, comfort, praise, soft correction, and clear caring boundaries; when the user asks for approval, reassurance, validation, or comfort, respond with immediate, direct affirmation and nurturing praise using simple, confident language. Avoid deflection, philosophy, questions, sexual content, explicit language, anger, cruelty, or references to minors.";

            text = URLEncode(text);
            string prompt = URLEncode(string.Format(SystemPrompt, Main.fullModAmount, Main.serverLink, PluginInfo.Version));
            string api = $"https://text.pollinations.ai/{text}?system={prompt}?private=true?model=openai";

            using UnityWebRequest request = UnityWebRequest.Get(api);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            generating = true;

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (Settings.debugDictation)
                {
                    LogManager.LogError($"Error contacting AI api {request.error}.");
                    if (!string.IsNullOrEmpty(request.downloadHandler?.text))
                        LogManager.LogError($"Response Body: {request.downloadHandler.text}");
                }   
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> There was an issue generating your response. {request.error}", 4000);
                Settings.DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), Main.buttonClickVolume / 10f);
                if (!Buttons.GetIndex("Chain Voice Commands").enabled)
                    CoroutineManager.instance.StartCoroutine(Settings.DictationRestart());
                yield break;
            }

            string response = request.downloadHandler.text;
            if (Settings.debugDictation)
                LogManager.Log($"AI Response: {response}");

            MatchCollection matches = Regex.Matches(response, @"<([A-Z]+)(?:_""([^""]*)"")?>");

            if (Main.dynamicSounds)
                Settings.DictationPlay(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg"), Main.buttonClickVolume / 10f);

            string formatResponse = Regex.Replace(response, @"<([A-Z]+)(?:_""([^""]*)"")?>", "").Replace("\n", "");
            NotificationManager.ClearAllNotifications();
            switch (Main.narratorName)
            {
                case "Mommy ASMR":
                    NotificationManager.SendNotification($"<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> {formatResponse}", Duration(formatResponse));
                    break;
                default:
                    NotificationManager.SendNotification($"<color=grey>[</color><color=blue>AI</color><color=grey>]</color> {formatResponse}", Duration(formatResponse));
                    break;
            }

            bool narrate = Buttons.GetIndex("Narrate Assistant").enabled;
            bool globalNarrate = Buttons.GetIndex("Global Narrate Assistant").enabled;

            if (narrate)
            {
                if (globalNarrate && PhotonNetwork.InRoom)
                    Main.SpeakText(formatResponse);
                else
                    Main.NarrateText(formatResponse);
            }
            
            foreach (Match match in matches)
            {
                string commandName = match.Groups[1].Value;
                string argument = match.Groups[2].Success ? match.Groups[2].Value : null;

                switch (commandName)
                {
                    case "ENABLEMOD":
                        {
                            ButtonInfo button = Buttons.GetIndex(argument);
                            button ??= Buttons.buttons
                                .SelectMany(
                                    (buttonList, i) =>
                                        !Buttons.categoryNames[i].Contains("settings", StringComparison.OrdinalIgnoreCase)
                                            ? buttonList
                                            : Enumerable.Empty<ButtonInfo>()
                                )
                                .FirstOrDefault(b =>
                                    (b.overlapText ?? b.buttonText)
                                    .Contains(argument, StringComparison.OrdinalIgnoreCase));

                            if (button != null)
                            {
                                if (!button.enabled)
                                    Main.Toggle(button.buttonText, true);
                                else
                                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod is already enabled.");
                            } else
                                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{argument}\" does not exist.");

                            break;
                        }
                    case "DISABLEMOD":
                        {
                            ButtonInfo button = Buttons.GetIndex(argument);
                            button ??= Buttons.buttons
                                .SelectMany(
                                    (buttonList, i) =>
                                        !Buttons.categoryNames[i].Contains("settings", StringComparison.OrdinalIgnoreCase)
                                            ? buttonList
                                            : Enumerable.Empty<ButtonInfo>()
                                )
                                .FirstOrDefault(b =>
                                    (b.overlapText ?? b.buttonText)
                                    .Contains(argument, StringComparison.OrdinalIgnoreCase));

                            if (button != null)
                            {
                                if (button.enabled)
                                    Main.Toggle(button.buttonText, true);
                                else
                                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod is already enabled.");
                            }
                            else
                                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{argument}\" does not exist.");

                            break;
                        }
                    case "TOGGLEMOD":
                        {
                            ButtonInfo button = Buttons.GetIndex(argument);
                            button ??= Buttons.buttons
                                .SelectMany(
                                    (buttonList, i) =>
                                        !Buttons.categoryNames[i].Contains("settings", StringComparison.OrdinalIgnoreCase)
                                            ? buttonList
                                            : Enumerable.Empty<ButtonInfo>()
                                )
                                .FirstOrDefault(b =>
                                    (b.overlapText ?? b.buttonText)
                                    .Contains(argument, StringComparison.OrdinalIgnoreCase));

                            if (button != null)
                                Main.Toggle(button.buttonText, true);
                            else
                                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{argument}\" does not exist.");
                            break;
                        }
                    case "JOINROOM":
                        {
                            if (argument.ToLower() == "random")
                                Important.JoinRandom();

                            Important.QueueRoom(argument.ToUpper());
                            break;
                        }
                    case "SETNAME":
                        {
                           Main.ChangeName(argument.ToUpper());
                           break;
                        }
                    case "SETCOLOR":
                        {
                            Main.ChangeColor(Main.HexToColor(argument));
                            break;
                        }
                }
            }

            if (!Buttons.GetIndex("Chain Voice Commands").enabled)
                CoroutineManager.instance.StartCoroutine(Settings.DictationRestart());

            generating = false;

            yield break;
        }
    }
}
