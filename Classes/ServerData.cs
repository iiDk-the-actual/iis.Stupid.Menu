using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

namespace iiMenu.Classes
{
    public class ServerData : MonoBehaviour
    {
        #region Configuration
        public static bool ServerDataEnabled = true;
        public static string ServerEndpoint = "https://iidk.online";
        public static string ServerDataEndpoint = "https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/iiMenu_ServerData.txt";

        public static void SetupAdminPanel(string playername) => // Method used to spawn admin panel
            Menu.Main.SetupAdminPanel(playername);

        public static void JoinDiscordServer() => // Method used to join the Discord server
            Mods.Important.JoinDiscord();

        #endregion

        #region Server Data Code
        private static ServerData instance;

        private static List<string> DetectedModsLabelled = new List<string> { };

        private static float DataLoadTime = -1f;
        private static float ReloadTime = -1f;
        private static float HeartbeatTime = -1f;

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
            if (DataLoadTime > 0 && Time.time > DataLoadTime && GorillaComputer.instance.isConnectedToMaster)
            {
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

            if (Time.time > HeartbeatTime)
            {
                HeartbeatTime = Time.time + 60f;
                CoroutineManager.RunCoroutine(Heartbeat());
            }
        }

        public static void OnJoinRoom() =>
            CoroutineManager.RunCoroutine(TelementryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId));

        public static string CleanString(string input, int maxLength = 12)
        {
            input = new string(Array.FindAll<char>(input.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));

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

        public static Dictionary<string, string> Administrators = new Dictionary<string, string> { };
        public static System.Collections.IEnumerator LoadServerData()
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{ServerDataEndpoint}?q={DateTime.UtcNow.Ticks}"))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Console.Log("Failed to load server data: " + request.error);
                    yield break;
                }

                string response = request.downloadHandler.text;
                DataLoadTime = -1f;

                string[] ResponseData = response.Split("\n");

                // Version Check
                if (!VersionWarning)
                {
                    if (ResponseData[0] != PluginInfo.Version)
                    {
                        if (!PluginInfo.BetaBuild)
                        {
                            VersionWarning = true;
                            Console.Log("Version is outdated");
                            JoinDiscordServer();
                            Console.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to " + ResponseData[0] + ".", 10000);
                        }
                        else
                        {
                            VersionWarning = true;
                            Console.Log("Version is outdated, but user is on beta");
                            Console.SendNotification("<color=grey>[</color><color=purple>BETA</color><color=grey>]</color> You are using a testing build of the menu. The latest release build is " + ResponseData[0] + ".", 10000);
                        }
                    }
                    else
                    {
                        if (PluginInfo.BetaBuild)
                        {
                            VersionWarning = true;
                            Console.Log("Version is outdated, user is on early build of latest");
                            JoinDiscordServer();
                            Console.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a testing build of the menu. Please update to " + ResponseData[0] + ".", 10000);
                        }
                    }
                }

                // Lockdown check
                if (ResponseData[0] == "lockdown")
                {
                    Console.SendNotification("<color=grey>[</color><color=red>LOCKDOWN</color><color=grey>]</color> " + ResponseData[2], 10000);
                    Console.DisableMenu = true;
                }

                // Admin dictionary
                Administrators.Clear();
                string[] AdminList = ResponseData[1].Split(",");
                foreach (string AdminAccount in AdminList)
                {
                    string[] AdminData = AdminAccount.Split(";");
                    Administrators.Add(AdminData[0], AdminData[1]);
                }

                // Give admin panel if on list
                if (!GivenAdminMods && PhotonNetwork.LocalPlayer.UserId != null && Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                {
                    GivenAdminMods = true;
                    SetupAdminPanel(Administrators[PhotonNetwork.LocalPlayer.UserId]);
                }

                // Basic string data
                Menu.Main.motdTemplate = ResponseData[2];
                Menu.Main.serverLink = ResponseData[3];
                Menu.Main.repReason = ResponseData[8];

                // Custom board data
                string[] Data2 = ResponseData[4].Split(";;");
                Menu.Main.StumpLeaderboardID = Data2[0];
                Menu.Main.ForestLeaderboardID = Data2[1];
                Menu.Main.StumpLeaderboardIndex = int.Parse(Data2[2]);
                Menu.Main.ForestLeaderboardIndex = int.Parse(Data2[3]);

                // Detected mod labels
                string[] DetectedMods = null;

                if (ResponseData[5].Contains(";;"))
                    DetectedMods = ResponseData[5].Split(";;");
                else
                    DetectedMods = new string[] { ResponseData[5] };

                foreach (string DetectedMod in DetectedMods)
                {
                    if (!DetectedModsLabelled.Contains(DetectedMod))
                    {
                        ButtonInfo Button = Menu.Main.GetIndex(DetectedMod);
                        if (Button != null)
                        {
                            string overlapText = Button.overlapText ?? Button.buttonText;

                            Button.overlapText = overlapText + " <color=grey>[</color><color=red>Detected</color><color=grey>]</color>";
                            Button.isTogglable = false;
                            Button.enabled = false;

                            Button.method = delegate { Console.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is currently disabled, as it is detected."); };
                        }
                        DetectedModsLabelled.Add(DetectedMod);
                    }
                }

                Menu.Main.annoyingIDs.Clear();
                string[] AnnoyingList = ResponseData[6].Split(",");
                foreach (string AnnoyingPlayer in AnnoyingList)
                {
                    string[] AnnoyingData = AnnoyingPlayer.Split(";");
                    Menu.Main.annoyingIDs.Add(AnnoyingData[0], long.Parse(AnnoyingData[1]));
                }

                string[] AnnoyingPeople = new string[] { };
                if (ResponseData[7].Contains(";"))
                    AnnoyingPeople = ResponseData[7].Split(";");
                else
                    AnnoyingPeople = ResponseData[7].Length < 1 ? new string[] { } : new string[] { ResponseData[7] };

                Menu.Main.muteIDs = AnnoyingPeople.ToList();

                // leaves name
                Mods.Visuals.leavesName = ResponseData[8].Replace("\n","");
            }

            yield return null;
        }

        public static System.Collections.IEnumerator TelementryRequest(string directory, string identity, string region, string userid)
        {
            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/telemetry", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                directory = CleanString(directory),
                identity = CleanString(identity),
                region = CleanString(region, 3),
                userid = CleanString(userid, 20)
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

        public static System.Collections.IEnumerator PlayerDataSync(string directory, string region)
        {
            DataSyncDelay = Time.time + 3f;
            yield return new WaitForSeconds(3f);

            if (!PhotonNetwork.InRoom)
                yield break;

            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>> { };

            foreach (Player identification in PhotonNetwork.PlayerList)
                data.Add(identification.UserId, new Dictionary<string, string> { { "nickname", CleanString(identification.NickName) }, { "cosmetics", Console.GetVRRigFromPlayer(identification).concatStringOfCosmeticsAllowed } });

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
            List<string> enabledMods = new List<string> { };

            int categoryIndex = 0;
            foreach (ButtonInfo[] category in Menu.Buttons.buttons)
            {
                foreach (ButtonInfo button in category)
                {
                    if (button.enabled && !Menu.Buttons.categoryNames[categoryIndex].Contains("Settings"))
                        enabledMods.Add(NoASCIIStringCheck(Menu.Main.NoRichtextTags(button.overlapText ?? button.buttonText), 128));
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

        public static System.Collections.IEnumerator Heartbeat()
        {
            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/heartbeat", "POST")
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
        }

        #endregion
    }
}
