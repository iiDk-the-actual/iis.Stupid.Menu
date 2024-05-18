using GorillaNetworking;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Diagnostics;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.Reconnect;
using Valve.VR;
using Cinemachine;

namespace iiMenu.Mods
{
    internal class Important
    {
        public static void Disconnect()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && SteamVR_Actions.gorillaTag_RightJoystickClick.state) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
            {
                PhotonNetwork.Disconnect();
            }
        }

        public static void Reconnect()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && SteamVR_Actions.gorillaTag_RightJoystickClick.state) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
            {
                rejRoom = PhotonNetwork.CurrentRoom.Name;
                rejDebounce = Time.time + (float)internetTime;
                PhotonNetwork.Disconnect();
            }
        }

        public static void CancelReconnect()
        {
            rejRoom = null;
            isJoiningRandom = false;
        }

        public static void JoinLastRoom()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(lastRoom);
        }

        public static void ActJoinRandom()
        {
            string gamemode = PhotonNetworkController.Instance.currentJoinTrigger.gameModeName;

            if (gamemode == "forest")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "city")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "canyons")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "mountains")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain For Computer").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "beach")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach from Forest").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "sky")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "basement")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
            if (gamemode == "caves")
            {
                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
            }
        }

        public static void JoinRandom()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && SteamVR_Actions.gorillaTag_RightJoystickClick.state) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Disconnect();
                    isJoiningRandom = true;
                    jrDebounce = Time.time + (float)internetTime;
                }
                else
                {
                    GameObject forest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest");
                    GameObject city = GameObject.Find("Environment Objects/LocalObjects_Prefab/City");
                    GameObject canyons = GameObject.Find("Environment Objects/LocalObjects_Prefab/Canyon");
                    GameObject mountains = GameObject.Find("Environment Objects/LocalObjects_Prefab/Mountain");
                    GameObject beach = GameObject.Find("Environment Objects/LocalObjects_Prefab/Beach");
                    GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/skyjungle");
                    GameObject basement = GameObject.Find("Environment Objects/LocalObjects_Prefab/Basement");
                    GameObject caves = GameObject.Find("Environment Objects/LocalObjects_Prefab/Cave_Main_Prefab");

                    if (forest.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (city.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (canyons.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (mountains.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain For Computer").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (beach.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach from Forest").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (sky.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (basement.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                    if (caves.activeSelf == true)
                    {
                        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave").GetComponent<GorillaNetworkJoinTrigger>().OnBoxTriggered();
                    }
                }
            }
        }

        public static void CreatePublic()
        {
            Hashtable customRoomProperties;
            /*if (PhotonNetworkController.Instance.currentJoinTrigger.gameModeName != "city" && PhotonNetworkController.Instance.currentJoinTrigger.gameModeName != "basement")
            {*/
            customRoomProperties = new Hashtable
            {
                {
                    "gameMode",
                    PhotonNetworkController.Instance.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode.Value
                }
            };
            /*}
            else
            {
                customRoomProperties = new Hashtable
                {
                    {
                        "gameMode",
                        PhotonNetworkController.Instance.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "INFECTION"
                    }
                };
            }*/
            Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            roomOptions.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentJoinTrigger.gameModeName);
            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.PublishUserId = true;
            roomOptions.CustomRoomPropertiesForLobby = new string[]
            {
                "gameMode"
            };
            PhotonNetwork.CreateRoom(RandomRoomName(), roomOptions, null, null);
        }

        public static void RestartGame()
        {
            Process.Start("steam://rungameid/1533390");
            Application.Quit();
        }

        public static void EnableFPC()
        {
            try
            {
                cam = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            }
            catch { }
            if (cam != null)
            {
                cam.GetComponent<CinemachineVirtualCamera>().enabled = false;
                cam.GetComponent<Camera>().fieldOfView = 90;
                cam.transform.parent = GorillaTagger.Instance.mainCamera.transform;
                cam.transform.localPosition = Vector3.zero;
                cam.transform.localRotation = Quaternion.identity;
            }
        }

        public static void DisableFPC()
        {
            if (cam != null)
            {
                cam.GetComponent<Camera>().fieldOfView = 60f;
                cam.transform.parent = GameObject.Find("Player Objects/Third Person Camera").transform;
                cam.GetComponent<CinemachineVirtualCamera>().enabled = true;
            }
        }

        public static void ForceEnableHands()
        {
            GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").SetActive(true);
            GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").SetActive(true);
        }

        public static void OculusReportMenu()
        {
            if (leftPrimary)
            {
                GameObject GorillaMetaReport = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/MetaReporting");

                GorillaMetaReport.GetComponent<GorillaMetaReport>().enabled = true;
                GorillaMetaReport.GetComponent<GorillaMetaReport>().Invoke("StartOverlay", 0.1f);
            }
        }

        public static void AcceptTOS()
        {
            GameObject.Find("Miscellaneous Scripts/LegalAgreementCheck/Legal Agreements").GetComponent<LegalAgreements>().testFaceButtonPress = true;
        }

        public static void JoinDiscord()
        {
            Process.Start("https://discord.gg/iidk");
        }

        public static void EnableAntiAFK()
        {
            PhotonNetworkController.Instance.disableAFKKick = false;
        }

        public static void DisableAntiAFK()
        {
            PhotonNetworkController.Instance.disableAFKKick = true;
        }

        public static void DisableNetworkTriggers()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false);
        }

        public static void EnableNetworkTriggers()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true);
        }

        public static void DisableQuitBox()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(false);
        }

        public static void EnableQuitBox()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true);
        }

        public static void DisableMouthMovement()
        {
            GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = false;
        }

        public static void EnableMouthMovement()
        {
            GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = true;
        }

        public static void EnableFPSBoost()
        {
            QualitySettings.globalTextureMipmapLimit = 99999;
        }

        public static void DisableFPSBoost()
        {
            QualitySettings.globalTextureMipmapLimit = 1;
        }

        public static void ForceLagGame()
        {
            foreach (GameObject g in Object.FindObjectsByType<GameObject>(0)) { }
        }

        public static void UncapFPS()
        {
            Application.targetFrameRate = 9999;
        }

        public static void UnlockCompetitiveQueue()
        {
            GorillaComputer.instance.CompQueueUnlockButtonPress();
        }

        public static void EUServers()
        {
            PhotonNetwork.ConnectToRegion("eu");
        }

        public static void USServers()
        {
            PhotonNetwork.ConnectToRegion("us");
        }

        public static void USWServers()
        {
            PhotonNetwork.ConnectToRegion("usw");
        }

        public static string RandomRoomName()
        {
            string text = "";
            for (int i = 0; i < 4; i++)
            {
                text += NetworkSystem.roomCharacters.Substring(Random.Range(0, NetworkSystem.roomCharacters.Length), 1);
            }
            if (GorillaComputer.instance.CheckAutoBanListForName(text))
            {
                return text;
            }
            return RandomRoomName();
        }
    }
}
