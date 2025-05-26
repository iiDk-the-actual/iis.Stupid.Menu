using iiMenu.Mods;
using iiMenu.Notifications;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System;
using UnityEngine;
using GorillaNetworking;
using iiMenu.Menu;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using System.Text;
using Photon.Realtime;
using static iiMenu.Menu.Main;
using static iiMenu.Classes.RigManager;

namespace iiMenu.Classes
{
    public class ServerData : MonoBehaviour
    {
        // Configuration
        public static bool ServerDataEnabled = true;
        public static string ServerEndpoint = "https://iidk.online";
        public static string ServerDataEndpoint = "https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/iiMenu_ServerData.txt";

        // Server Data Code
        private static ServerData instance;

        private static List<string> DetectedModsLabelled = new List<string> { };

        private static float DataLoadTime = -1f;
        private static float ReloadTime = -1f;

        private static int LoadAttempts;

        private static bool VersionWarning;
        private static bool GivenAdminMods;

        public void Start()
        {
            instance = this;
            DataLoadTime = Time.time + 5f;
        }

        public void Update()
        {
            if (DataLoadTime > 0 && Time.time > DataLoadTime && GorillaComputer.instance.isConnectedToMaster)
            {
                LoadAttempts++;
                if (LoadAttempts >= 3)
                {
                    LogManager.Log("Server data could not be loaded");
                    DataLoadTime = -1f;
                    return;
                }

                LogManager.Log("Attempting to load web data");
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

            if (PhotonNetwork.InRoom && !InRoom)
                CoroutineManager.RunCoroutine(TelementeryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId));

            InRoom = PhotonNetwork.InRoom;

            if (Time.time > DataSyncDelay || !PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length != PlayerCount)
                    CoroutineManager.RunCoroutine(PlayerDataSync(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CloudRegion));

                PlayerCount = PhotonNetwork.InRoom ? PhotonNetwork.PlayerList.Length : -1;
            }
        }

        public static System.Collections.IEnumerator LoadServerData()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(ServerDataEndpoint + "?q=" + DateTime.UtcNow.Ticks);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                LogManager.Log("Failed to load server data: " + webRequest.error);
                yield break;
            }

            string response = webRequest.downloadHandler.text;
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
                        LogManager.Log("Version is outdated");
                        Important.JoinDiscord();
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to " + ResponseData[0] + ".", 10000);
                    }
                    else
                    {
                        VersionWarning = true;
                        LogManager.Log("Version is outdated, but user is on beta");
                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>BETA</color><color=grey>]</color> You are using a testing build of the menu. The latest release build is " + ResponseData[0] + ".", 10000);
                    }
                }
                else
                {
                    if (PluginInfo.BetaBuild)
                    {
                        VersionWarning = true;
                        LogManager.Log("Version is outdated, user is on early build of latest");
                        Important.JoinDiscord();
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a testing build of the menu. Please update to " + ResponseData[0] + ".", 10000);
                    }
                }
            }

            // Lockdown check
            if (ResponseData[0] == "lockdown")
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>LOCKDOWN</color><color=grey>]</color> " + ResponseData[2], 10000);
                bgColorA = Color.red;
                bgColorB = Color.red;

                Lockdown = true;
            }

            // Admin dictionary
            admins.Clear();
            string[] AdminList = ResponseData[1].Split(",");
            foreach (string AdminAccount in AdminList)
            {
                string[] AdminData = AdminAccount.Split(";");
                admins.Add(AdminData[0], AdminData[1]);
            }

            // Give admin panel if on list
            if (!GivenAdminMods && admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
            {
                GivenAdminMods = true;
                SetupAdminPanel(admins[PhotonNetwork.LocalPlayer.UserId]);
            }

            // Basic string data
            motdTemplate = ResponseData[2];
            serverLink = ResponseData[3];
            repReason = ResponseData[8];

            // Custom board data
            string[] Data2 = ResponseData[4].Split(";;");
            StumpLeaderboardID = Data2[0];
            ForestLeaderboardID = Data2[1];
            StumpLeaderboardIndex = int.Parse(Data2[2]);
            ForestLeaderboardIndex = int.Parse(Data2[3]);

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
                    ButtonInfo Button = GetIndex(DetectedMod);
                    if (Button != null)
                    {
                        string overlapText = Button.overlapText == null ? Button.buttonText : Button.overlapText;

                        Button.overlapText = overlapText + " <color=grey>[</color><color=red>Detected</color><color=grey>]</color>";
                        Button.isTogglable = false;
                        Button.enabled = false;

                        Button.method = delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is currently disabled, as it is detected."); };
                    }
                    DetectedModsLabelled.Add(DetectedMod);
                }
            }

            annoyingIDs.Clear();
            string[] AnnoyingList = ResponseData[6].Split(",");
            foreach (string AnnoyingPlayer in AnnoyingList)
            {
                string[] AnnoyingData = AnnoyingPlayer.Split(";");
                annoyingIDs.Add(AnnoyingData[0], long.Parse(AnnoyingData[1]));
            }

            string[] AnnoyingPeople = new string[] { };
            if (ResponseData[7].Contains(";"))
                AnnoyingPeople = ResponseData[7].Split(";");
            else
                AnnoyingPeople = ResponseData[7].Length < 1 ? new string[] { } : new string[] { ResponseData[7] };

            muteIDs = AnnoyingPeople.ToList();

            yield return null;
        }

        private static bool InRoom;
        public static System.Collections.IEnumerator TelementeryRequest(string directory, string identity, string region, string userid)
        {
            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/telementery", "POST");

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

        public static System.Collections.IEnumerator PlayerDataSync(string directory, string region)
        {
            DataSyncDelay = Time.time + 3f;
            yield return new WaitForSeconds(3f);

            if (!PhotonNetwork.InRoom)
                yield break;

            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>> { };

            foreach (Player identification in PhotonNetwork.PlayerList)
                data.Add(identification.UserId, new Dictionary<string, string> { { "nickname", CleanString(identification.NickName) }, { "cosmetics", GetVRRigFromPlayer(identification).concatStringOfCosmeticsAllowed } });

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
            foreach (ButtonInfo[] category in Buttons.buttons)
            {
                foreach (ButtonInfo button in category)
                {
                    if (button.enabled && !Buttons.categoryNames[categoryIndex].Contains("Settings"))
                        enabledMods.Add(NoASCIIStringCheck(NoRichtextTags(button.overlapText == null ? button.buttonText : button.overlapText), 128));
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
    }
}
