/*
 * ii's Stupid Menu  Classes/Menu/ServerData.cs
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

using GorillaNetworking;
using iiMenu.Managers;
using iiMenu.Menu;
using MonoMod.Utils;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace iiMenu.Classes.Menu
{
    public class ServerData : MonoBehaviour
    {
        #region Configuration
        public static readonly bool ServerDataEnabled = true; // Disables Console and admin panel
        public static bool DisableTelemetry = false; // Disables telemetry data being sent to the server

        // Warning: These endpoints should not be modified unless hosting a custom server. Use with caution.
        public const string ServerEndpoint = "https://iidk.online";
        public static readonly string ServerDataEndpoint = $"{ServerEndpoint}/serverdata";

        // The dictionary used to assign the admins only seen in your mod.
        public static readonly Dictionary<string, string> LocalAdmins = new Dictionary<string, string>()
        {
            // { "Placeholder Admin UserID", "Placeholder Admin Name" },
        };

        public static void SetupAdminPanel(string playername) => // Method used to spawn admin panel
            Main.SetupAdminPanel(playername);
        #endregion

        #region Server Data Code
        private static ServerData instance;

        private static readonly List<string> DetectedModsLabelled = new List<string>();

        private static float DataLoadTime = -1f;
        private static float ReloadTime = -1f;

        private static int LoadAttempts;

        private static bool BetaBuildWarning;
        public static bool OutdatedVersion;

        private static bool GivenAdminMods;
        private static bool GivenPateronMods;

        private static string LastPollAnswered;

        private static string CurrentPoll = "What goes well with cheeseburgers?";
        private static string OptionA = "Fries";
        private static string OptionB = "Chips";

        public void Awake()
        {
            instance = this;
            DataLoadTime = Time.time + 5f;

            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinRoom;

            NetworkSystem.Instance.OnPlayerJoined += UpdatePlayerCount;
            NetworkSystem.Instance.OnPlayerLeft += UpdatePlayerCount;

            if (File.Exists($"{PluginInfo.BaseDirectory}/LastPollAnswered.txt"))
                LastPollAnswered = File.ReadAllText($"{PluginInfo.BaseDirectory}/LastPollAnswered.txt");
        }

        public void Update()
        {
            if (DataLoadTime > 0f && Time.time > DataLoadTime && GorillaComputer.instance.isConnectedToMaster)
            {
                DataLoadTime = Time.time + 5f;

                LoadAttempts++;
                if (LoadAttempts >= 3)
                {
                    Console.Log("Server data could not be loaded");
                    DataLoadTime = -1f;
                    return;
                }

                Console.Log("Attempting to load web data");
                instance.StartCoroutine(LoadServerData());
            }

            if (ReloadTime > 0f)
            {
                if (Time.time > ReloadTime)
                {
                    ReloadTime = Time.time + 60f;
                    instance.StartCoroutine(LoadServerData());
                }
            }
            else
            {
                if (GorillaComputer.instance.isConnectedToMaster)
                    ReloadTime = Time.time + 5f;
            }

            if (!(Time.time > DataSyncDelay) && PhotonNetwork.InRoom) return;
            if (PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length != PlayerCount)
                instance.StartCoroutine(PlayerDataSync(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CloudRegion));

            PlayerCount = PhotonNetwork.InRoom ? PhotonNetwork.PlayerList.Length : -1;
        }

        public static void OnJoinRoom() =>
            instance.StartCoroutine(TelemetryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.CurrentRoom.IsVisible, PhotonNetwork.PlayerList.Length, NetworkSystem.Instance.GameModeString));

        public static string CleanString(string input, int maxLength = 12)
        {
            input = new string(Array.FindAll(input.ToCharArray(), Utils.IsASCIILetterOrDigit));

            if (input.Length > maxLength)
                input = input[..(maxLength - 1)];

            input = input.ToUpper();
            return input;
        }

        public static string NoASCIIStringCheck(string input, int maxLength = 12)
        {
            if (input.Length > maxLength)
                input = input[..(maxLength - 1)];

            input = input.ToUpper();
            return input;
        }

        public static int VersionToNumber(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length != 3)
                return -1; // Version must be in 'major.minor.patch' format

            return int.Parse(parts[0]) * 100 + int.Parse(parts[1]) * 10 + int.Parse(parts[2]);
        }

        public static readonly Dictionary<string, string> Administrators = new Dictionary<string, string>();
        public static readonly List<string> SuperAdministrators = new List<string>();
        public static IEnumerator LoadServerData()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(ServerDataEndpoint))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Console.Log("Failed to load server data: " + request.error);
                    yield break;
                }

                string json = request.downloadHandler.text;
                DataLoadTime = -1f;

                JObject data = JObject.Parse(json);

                Main.serverLink = (string)data["discord-invite"];
                CustomBoardManager.motdTemplate = (string)data["motd"];

                // Version Check
                string minimumVersion = (string)data["min-version"];
                string version = (string)data["menu-version"];
                bool shownPrompt = false;

                if (PluginInfo.BetaBuild)
                {
                    if (!BetaBuildWarning)
                    {
                        BetaBuildWarning = true;
                        Console.Log("User is on beta build");
                        Console.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> You are using a testing build of the menu. Be warned that there may be bugs and issues that could cause crashes, data loss, or other unexpected behavior.", 10000);
                    }
                }
                else if (VersionToNumber(PluginInfo.Version) < VersionToNumber(minimumVersion))
                {
                    if (!OutdatedVersion)
                    {
                        OutdatedVersion = true;
                        Console.DisableMenu = true;
                        Console.SendNotification($"<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a severely outdated version of the menu. For security, it has been disabled. Please update your menu.", 10000);
                        Main.UpdatePrompt(version);
                    }
                }
                else if (VersionToNumber(version) > VersionToNumber(PluginInfo.Version))
                {
                    if (!OutdatedVersion)
                    {
                        OutdatedVersion = true;
                        Console.Log("Version is outdated");
                        Console.SendNotification($"<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to version {version}.", 10000);
                        Main.UpdatePrompt(version);
                        shownPrompt = true;
                    }
                }

                string minConsoleVersion = (string)data["min-console-version"];
                if (VersionToNumber(Console.ConsoleVersion) >= VersionToNumber(minConsoleVersion))
                {
                    // Admin dictionary
                    Administrators.Clear();

                    JArray admins = (JArray)data["admins"];
                    foreach (var admin in admins)
                    {
                        string name = admin["name"].ToString();
                        string userId = admin["user-id"].ToString();
                        Administrators[userId] = name;
                    }

                    Administrators.AddRange(LocalAdmins);

                    SuperAdministrators.Clear();

                    JArray superAdmins = (JArray)data["super-admins"];
                    foreach (var superAdmin in superAdmins)
                        SuperAdministrators.Add(superAdmin.ToString());

                    // Give admin panel if on list
                    if (!GivenAdminMods && PhotonNetwork.LocalPlayer.UserId != null && Administrators.TryGetValue(PhotonNetwork.LocalPlayer.UserId, out var administrator))
                    {
                        GivenAdminMods = true;
                        SetupAdminPanel(administrator);
                    }
                } else
                    Console.Log("On extreme outdated version of Console, not loading administrators");

                // Patreon members
                if (PatreonManager.instance != null)
                {
                    PatreonManager.instance.PatreonMembers.Clear();

                    JArray members = (JArray)data["patreon"];
                    foreach (var member in members)
                        PatreonManager.instance.PatreonMembers.Add(member["user-id"].ToString(), new PatreonManager.PatreonMembership(member["name"].ToString(), member["photo"].ToString()));

                    // Give patreon if on list
                    if (!GivenPateronMods && PhotonNetwork.LocalPlayer.UserId != null && PatreonManager.instance.PatreonMembers.TryGetValue(PhotonNetwork.LocalPlayer.UserId, out var membership))
                    {
                        GivenPateronMods = true;
                        PatreonManager.SetupPatreonMods(membership.TierName);
                    }
                }

                // Polls
                CurrentPoll = (string)data["poll"];
                OptionA = (string)data["option-a"];
                OptionB = (string)data["option-b"];

                if (!Plugin.FirstLaunch && LastPollAnswered != CurrentPoll)
                {
                    if (!shownPrompt)
                    {
                        Main.Prompt(CurrentPoll, () => CoroutineManager.instance.StartCoroutine(SendVote("a-votes")), () => CoroutineManager.instance.StartCoroutine(SendVote("b-votes")), OptionA, OptionB);
                        Console.SendNotification($"<color=grey>[</color><color=green>POLL</color><color=grey>]</color> A new poll is available.", 10000);
                    }

                    LastPollAnswered = CurrentPoll;
                    File.WriteAllText($"{PluginInfo.BaseDirectory}/LastPollAnswered.txt", CurrentPoll);
                }

                // Detected mod labels
                JArray detectedMods = (JArray)data["detected-mods"];
                foreach (var detectedMod in detectedMods)
                {
                    string detectedModName = detectedMod.ToString();
                    if (DetectedModsLabelled.Contains(detectedModName)) continue;
                    ButtonInfo button = Buttons.GetIndex(detectedModName);
                    if (button != null)
                    {
                        string overlapText = button.overlapText ?? button.buttonText;

                        button.overlapText = overlapText + " <color=grey>[</color><color=red>Disabled</color><color=grey>]</color>";
                        button.isTogglable = false;
                        button.enabled = false;

                        button.method = delegate { Console.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is currently disabled, as it is detected."); };
                        button.enableMethod = button.method;
                        button.disableMethod = button.method;
                    }
                    DetectedModsLabelled.Add(detectedModName);
                }
            }

            yield return null;
        }

        public static IEnumerator TelemetryRequest(string directory, string identity, string region, string userid, bool isPrivate, int playerCount, string gameMode)
        {
            if (DisableTelemetry)
                yield break;

            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/telemetry", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                directory = CleanString(directory),
                identity = CleanString(identity),
                region = CleanString(region, 3),
                userid = CleanString(userid, 20),
                isPrivate,
                playerCount,
                gameMode = CleanString(gameMode, 128),
                consoleVersion = Console.ConsoleVersion,
                menuName = Console.MenuName,
                menuVersion = Console.MenuVersion
            });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
        }

        private static float DataSyncDelay;
        public static int PlayerCount;

        public static void UpdatePlayerCount(NetPlayer Player) =>
            PlayerCount = -1;

        public static bool IsPlayerSteam(VRRig Player)
        {
            string concat = Player.rawCosmeticString;
            int customPropsCount = Player.Creator.GetPlayerRef().CustomProperties.Count;

            return concat.Contains("S. FIRST LOGIN") ? true : concat.Contains("FIRST LOGIN") || customPropsCount >= 2;
        }

        public static IEnumerator PlayerDataSync(string directory, string region)
        {
            if (DisableTelemetry)
                yield break;

            DataSyncDelay = Time.time + 3f;
            yield return new WaitForSeconds(3f);

            if (!PhotonNetwork.InRoom)
                yield break;

            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

            foreach (Player identification in PhotonNetwork.PlayerList)
            {
                VRRig rig = Console.GetVRRigFromPlayer(identification) ?? VRRig.LocalRig;
                data.Add(identification.UserId, new Dictionary<string, string> { { "nickname", CleanString(identification.NickName) }, { "cosmetics", rig.rawCosmeticString }, { "color", $"{Math.Round(rig.playerColor.r * 255)} {Math.Round(rig.playerColor.g * 255)} {Math.Round(rig.playerColor.b * 255)}" }, { "platform", IsPlayerSteam(rig) ? "STEAM" : "QUEST" } });
            }

            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/syncdata", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                directory = CleanString(directory),
                region = CleanString(region, 3),
                data
            });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
        }
        #endregion

        #region Menu Specific
        public static IEnumerator ReportFailureMessage(string error)
        {
            if (DisableTelemetry)
                yield break;

            List<string> enabledMods = new List<string>();

            int categoryIndex = 0;
            foreach (ButtonInfo[] category in Buttons.buttons)
            {
                enabledMods.AddRange(from button in category where button.enabled && !Buttons.categoryNames[categoryIndex].Contains("Settings") select NoASCIIStringCheck(Main.NoRichtextTags(button.overlapText ?? button.buttonText), 128));

                categoryIndex++;
            }

            AchievementManager.UnlockAchievement(new AchievementManager.Achievement
            {
                name = "Purgatory",
                description = "Get banned with the menu.",
                icon = "Images/Achievements/banned.png"
            });

            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/reportban", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                error = NoASCIIStringCheck(error, 512),
                version = PluginInfo.Version,
                data = enabledMods
            });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
        }

        public static IEnumerator SendVote(string category)
        {
            UnityWebRequest request = new UnityWebRequest($"{ServerEndpoint}/vote", "POST");

            string json = JsonConvert.SerializeObject(new { option = category });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) yield break;
            try
            {
                string responseText = request.downloadHandler.text;
                Dictionary<string, object> responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

                int avotes = Convert.ToInt32(responseJson["a-votes"]);
                int bvotes = Convert.ToInt32(responseJson["b-votes"]);

                int total = avotes + bvotes;

                string result;
                if (total > 0)
                {
                    double aPercent = (double)avotes / total * 100;
                    double bPercent = (double)bvotes / total * 100;

                    result = $"Total Votes: {total}\n{OptionA}: {aPercent:F2}%\n{OptionB}: {bPercent:F2}%";
                }
                else
                    result = "No votes yet.";

                Main.PromptSingle(result, null, "Ok");
            }
            catch { }
        }
        #endregion
    }
}
