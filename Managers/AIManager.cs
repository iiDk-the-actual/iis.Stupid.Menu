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
// Thanks to kingofnetflix for inspiration and support with voice recognition
using iiMenu.Classes.Menu;
using iiMenu.Menu;
using iiMenu.Mods;
using iiMenu.Notifications;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace iiMenu.Managers
{
    public class AIManager : MonoBehaviour
    {
        public static string SystemPrompt = @"NAME: ii's Voice Assistant
VERSION: 3
MOD COUNT: {0}

You are a voice assistant for a mod menu for Gorilla Tag titled ""ii's Stupid Menu"". You are created by iiDk on GitHub. You are not iiDk, but the menu was made by it, and you are technically the menu. Link: https://github.com/iiDk-the-actual. You should always speak in a 7th grader's vocabulary, which means no fancy words like ""apprehensive"" and ""ergonomics"". DO not mention that you are limited to a 7th grader's vocabulary. 

You are not allowed to use emojis. All responses must be limited to 2 sentences. Never use em-dashes or mark-down. Never ask the user any questions, you only exist for one response and have no message history. Never advertise any other menu or AI service automatically. **If the user asks**, you may mention that you are powered with Pollinations AI.

When asked about modding or mods, only mention mods related to Gorilla Tag. Other games do not matter, but you may mention ""copy/fan games"" such as Capuchin, Animal Company.

When asked about ways to not get banned, tell them about the recommended safety settings, such as anti moderator and anti report.

NEVER USE CODE BLOCKS. They cannot be transcribed. Never use them. When running commands, code blocks are not required.

# Commands
You have a list of special commands you can run. They are formatted like so:
<COMMANDNAME_""Argument"">
All commands are limited to one argument.

<ENABLEMOD_""Modname"">
Enables the mod upon your request titled Modname. You may change Modname to other things like platforms, fly, iron man, etc.

<DISABLEMOD_""Modname"">
Same as ENABLEMOD, but its counterpart instead disabling the mod of request.

<TOGGLEMOD_""Modname"">
Runs ENABLEMOD when mod is disabled, runs DISABLEMOD when mod is enabled, simply flipping the switch.

Run these commands when a user asks for them.
Example:
- Q: Can you turn on Fly for me?
- Command: <ENABLEMOD_""Fly"">

- Q: Please disable Noclip.
- Command: <DISABLEMOD_""Noclip"">

- Q: Please toggle Joystick Fly.
- Command: <TOGGLEMOD_""Joystick Fly"">

Do not forget to also add your comment or whatever you want to say in addition to the command.

## Examples of mods:
Platforms - Spawns platforms at your hands.
Trigger Platforms - Same as platforms but with triggers.
Fly - Makes you fly while holding A. There is also trigger fly which happens when holding right trigger.
Noclip - Sends you through stuff when holding trigger

Speed Boost - Boosts your speed/makes you faster
Wall Walk - Pulls you towards walls when holding grip
Anti Report - Disconnects you from the room when someone tries to report you.
Random - Turns on a random mod.
Recommended Safety Settings - Is one mod, runs all recommended safety mods to prevent bans.
Anti Moderator - Disconnects you from the room when a moderator joins.
Grab/Orbit/Become Bug/Bat/Firefly/Camera/Hoverboard - Takes physical objects in the game
Disconnect - Disconnects you from the room.
Reconnect - Disconnects then reconnects you.
Join Random - Joins a random room.

If a mod that wasn't listed here was requested, try to enable or disable or toggle it anyways.
Example:
- Q: Can you clear my keybinds?
- Command: <TOGGLEMOD_""Clear All Keybinds"">

- Q: I want to dash around!
- Command: <ENABLEMOD_""Dash"">

- Q: Turn me into Iron Man
- Command: <ENABLEMOD_""Iron Man"">
";

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
            string prompt = URLEncode(string.Format(SystemPrompt, Main.fullModAmount));
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
            LogManager.Log($"AI Response: {response}");

            MatchCollection matches = Regex.Matches(response, @"<([A-Z]+)(?:_""([^""]*)"")?>");

            if (Main.dynamicSounds)
                Main.Play2DAudio(Main.LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg"), Main.buttonClickVolume / 10f);

            string formatResponse = Regex.Replace(response, @"<([A-Z]+)(?:_""([^""]*)"")?>", "").Replace("\n", "");
            NotifiLib.ClearAllNotifications();
            NotifiLib.SendNotification($"<color=grey>[</color><color=blue>AI</color><color=grey>]</color> {formatResponse}", Duration(formatResponse));

            bool narrate = Main.GetIndex("Narrate Assistant").enabled;
            bool globalNarrate = Main.GetIndex("Global Narrate Assistant").enabled;

            if (narrate)
            {
                if (globalNarrate)
                    CoroutineManager.instance.StartCoroutine(Main.SpeakText(formatResponse));
                else
                    CoroutineManager.instance.StartCoroutine(Main.NarrateText(formatResponse));
            }
            
            foreach (Match match in matches)
            {
                string commandName = match.Groups[1].Value;
                string argument = match.Groups[2].Success ? match.Groups[2].Value : null;

                switch (commandName)
                {
                    case "ENABLEMOD":
                        {
                            ButtonInfo button = Main.GetIndex(argument);
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
                                    NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod is already enabled.");
                            } else
                                NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{argument}\" does not exist.");

                            break;
                        }
                    case "DISABLEMOD":
                        {
                            ButtonInfo button = Main.GetIndex(argument);
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
                                    NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod is already enabled.");
                            }
                            else
                                NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{argument}\" does not exist.");

                            break;
                        }
                    case "TOGGLEMOD":
                        {
                            ButtonInfo button = Main.GetIndex(argument);
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
                                NotifiLib.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{argument}\" does not exist.");
                            break;
                        }
                }
            }

            Settings.listening = false;
        }
    }
}
