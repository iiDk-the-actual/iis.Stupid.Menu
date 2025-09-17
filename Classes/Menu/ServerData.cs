using GorillaNetworking;
using iiMenu.Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
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
        public static bool ServerDataEnabled = true; // Disables Console, telemetry, and admin panel
        public static bool DisableTelemetry = false; // Disables telemetry data being sent to the server

        public static string ServerEndpoint = "https://iidk.online";
        public static string ServerDataEndpoint = "https://iidk.online/serverdata";

        public static void SetupAdminPanel(string playername) => // Method used to spawn admin panel
            iiMenu.Menu.Main.SetupAdminPanel(playername);

        public static void JoinDiscordServer() => // Method used to join the Discord server
            iiMenu.Mods.Important.JoinDiscord();

        #endregion

        #region Server Data Code
        private static ServerData instance;

        private static readonly List<string> DetectedModsLabelled = new List<string>();

        private static float DataLoadTime = -1f;
        private static float ReloadTime = -1f;

        private static int LoadAttempts;

        private static bool VersionWarning;
        private static bool GivenAdminMods;

        public void Awake()
        {
            instance = this;
            DataLoadTime = Time.time + 5f;

            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinRoom;

            NetworkSystem.Instance.OnPlayerJoined += UpdatePlayerCount;
            NetworkSystem.Instance.OnPlayerLeft += UpdatePlayerCount;
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
                CoroutineManager.RunCoroutine(LoadServerData());
            }

            if (ReloadTime > 0f)
            {
                if (Time.time > ReloadTime)
                {
                    ReloadTime = Time.time + 60f;
                    CoroutineManager.RunCoroutine(LoadServerData());
                }
            }
            else
            {
                if (GorillaComputer.instance.isConnectedToMaster)
                    ReloadTime = Time.time + 5f;
            }

            if (Time.time > DataSyncDelay || !PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length != PlayerCount)
                    CoroutineManager.RunCoroutine(PlayerDataSync(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CloudRegion));

                PlayerCount = PhotonNetwork.InRoom ? PhotonNetwork.PlayerList.Length : -1;
            }
        }

        public static void OnJoinRoom() =>
            CoroutineManager.RunCoroutine(TelementryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.CurrentRoom.IsVisible, PhotonNetwork.PlayerList.Length, NetworkSystem.Instance.GameModeString));

        public static string CleanString(string input, int maxLength = 12)
        {
            input = new string(Array.FindAll(input.ToCharArray(), c => Utils.IsASCIILetterOrDigit(c)));

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

        public static Dictionary<string, string> Administrators = new Dictionary<string, string>();
        public static List<string> SuperAdministrators = new List<string>();
        public static System.Collections.IEnumerator LoadServerData()
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

                iiMenu.Menu.Main.serverLink = (string)data["discord-invite"];
                iiMenu.Menu.Main.motdTemplate = (string)data["motd"];

                // Version Check
                string version = (string)data["menu-version"];
                if (!VersionWarning)
                {
                    VersionWarning = true;

                    if (PluginInfo.BetaBuild)
                    {
                        Console.Log("User is on beta build");
                        Console.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a testing build of the menu. Be warned that there may be bugs and issues that could cause crashes, data loss, or other unexpected behavior.", 10000);
                    }
                    else if (VersionToNumber(version) > VersionToNumber(PluginInfo.Version))
                    {
                        Console.Log("Version is outdated");
                        JoinDiscordServer();
                        Console.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to " + version + ".", 10000);
                    }
                }

                // Lockdown check
                if (version == "lockdown")
                {
                    Console.SendNotification($"<color=grey>[</color><color=red>LOCKDOWN</color><color=grey>]</color> {iiMenu.Menu.Main.motdTemplate}", 10000);
                    Console.DisableMenu = true;
                }

                // Admin dictionary
                Administrators.Clear();

                JArray admins = (JArray)data["admins"];
                foreach (var admin in admins)
                {
                    string name = admin["name"].ToString();
                    string userId = admin["user-id"].ToString();
                    Administrators[userId] = name;
                }

                SuperAdministrators.Clear();

                JArray superAdmins = (JArray)data["super-admins"];
                foreach (var superAdmin in superAdmins)
                    SuperAdministrators.Add(superAdmin.ToString());

                // Give admin panel if on list
                if (!GivenAdminMods && PhotonNetwork.LocalPlayer.UserId != null && Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                {
                    GivenAdminMods = true;
                    SetupAdminPanel(Administrators[PhotonNetwork.LocalPlayer.UserId]);
                }

                // Detected mod labels
                JArray detectedMods = (JArray)data["detected-mods"];
                foreach (var detectedMod in detectedMods)
                {
                    string detectedModName = detectedMod.ToString();
                    if (!DetectedModsLabelled.Contains(detectedModName))
                    {
                        ButtonInfo Button = iiMenu.Menu.Main.GetIndex(detectedModName);
                        if (Button != null)
                        {
                            string overlapText = Button.overlapText ?? Button.buttonText;

                            Button.overlapText = overlapText + " <color=grey>[</color><color=red>Detected</color><color=grey>]</color>";
                            Button.isTogglable = false;
                            Button.enabled = false;

                            Button.method = delegate { Console.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is currently disabled, as it is detected."); };
                            Button.enableMethod = Button.method;
                            Button.disableMethod = Button.method;
                        }
                        DetectedModsLabelled.Add(detectedModName);
                    }
                }

                List<string> muteIds = new List<string>();
                JArray muteIdData = (JArray)data["blacklisted-ids"];
                foreach (var targetMutedData in muteIdData)
                    muteIds.Add(targetMutedData.ToString());

                iiMenu.Menu.Main.muteIDs = muteIds;
            }

            yield return null;
        }

        public static System.Collections.IEnumerator TelementryRequest(string directory, string identity, string region, string userid, bool isPrivate, int playerCount, string gameMode)
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
            string concat = Player.concatStringOfCosmeticsAllowed;
            int customPropsCount = Player.Creator.GetPlayerRef().CustomProperties.Count;

            if (concat.Contains("S. FIRST LOGIN")) return true;
            if (concat.Contains("FIRST LOGIN") || customPropsCount >= 2) return true;
            if (concat.Contains("LMAKT.")) return false;

            return false;
        }

        public static System.Collections.IEnumerator PlayerDataSync(string directory, string region)
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
                data.Add(identification.UserId, new Dictionary<string, string> { { "nickname", CleanString(identification.NickName) }, { "cosmetics", rig.concatStringOfCosmeticsAllowed }, { "color", $"{Math.Round(rig.playerColor.r * 255)} {Math.Round(rig.playerColor.g * 255)} {Math.Round(rig.playerColor.b * 255)}" }, { "platform", IsPlayerSteam(rig) ? "STEAM" : "QUEST" } });
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

        public static System.Collections.IEnumerator ReportFailureMessage(string error)
        {
            if (DisableTelemetry)
                yield break;

            List<string> enabledMods = new List<string>();

            int categoryIndex = 0;
            foreach (ButtonInfo[] category in iiMenu.Menu.Buttons.buttons)
            {
                foreach (ButtonInfo button in category)
                {
                    if (button.enabled && !iiMenu.Menu.Buttons.categoryNames[categoryIndex].Contains("Settings"))
                        enabledMods.Add(NoASCIIStringCheck(iiMenu.Menu.Main.NoRichtextTags(button.overlapText ?? button.buttonText), 128));
                }

                categoryIndex++;
            }

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

        #endregion
    }
}
