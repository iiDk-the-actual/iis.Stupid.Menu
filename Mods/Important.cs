using Cinemachine;
using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Notifications;
using Photon.Pun;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;
using static iiMenu.Menu.Main;

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
                //rejDebounce = Time.time + (float)internetTime;
                PhotonNetwork.Disconnect();
            }
        }

        public static void CancelReconnect()
        {
            rejRoom = null;
            isJoiningRandom = false;
            partyLastCode = null;
            phaseTwo = false;
        }

        public static void JoinLastRoom()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(lastRoom, JoinType.Solo);
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
                    //jrDebounce = Time.time + (float)internetTime;
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
            string name = RandomRoomName();
            PhotonNetwork.CreateRoom(name, roomOptions, null, null);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(name, JoinType.Solo);
        }

        public static void AutoJoinRoomRUN()
        {
            rejRoom = "RUN";
            // rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomDAISY()
        {
            rejRoom = "DAISY";
            // rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomDAISY09()
        {
            rejRoom = "DAISY09";
            // rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomPBBV()
        {
            rejRoom = "PBBV";
            // rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomBOT()
        {
            rejRoom = "BOT";
            // rejDebounce = Time.time + 2f;
        }

        public static void RestartGame()
        {
            Process.Start("steam://rungameid/1533390");
            Application.Quit();
        }

        public static void DisableFPC()
        {
            if (TPC != null)
            {
                TPC.GetComponent<Camera>().fieldOfView = 60f;
                TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = true;
            }
        }

        public static void MoveFPC()
        {
            if (TPC != null)
            {
                TPC.fieldOfView = 90f;
                TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = false;
                TPC.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
                TPC.gameObject.transform.rotation = Quaternion.Lerp(TPC.transform.rotation, GorillaTagger.Instance.headCollider.transform.rotation, 0.075f);
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

        public static void CopyPlayerPosition()
        {
            string text = "Body\n";
            Transform p = GorillaTagger.Instance.bodyCollider.transform;
            text += "new Vector3(" + p.position.x.ToString() + ", " + p.position.y.ToString() + ", " + p.position.z.ToString() + ");";
            text += "new Quaternion(" + p.rotation.x.ToString() + ", " + p.rotation.y.ToString() + ", " + p.rotation.z.ToString() + ", " + p.rotation.w.ToString() + ");\n\n";

            text += "Head\n";
            p = GorillaTagger.Instance.headCollider.transform;
            text += "new Vector3(" + p.position.x.ToString() + ", " + p.position.y.ToString() + ", " + p.position.z.ToString() + ");";
            text += "new Quaternion(" + p.rotation.x.ToString() + ", " + p.rotation.y.ToString() + ", " + p.rotation.z.ToString() + ", " + p.rotation.w.ToString() + ");\n\n";

            text += "Left Hand\n";
            p = GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform;
            text += "new Vector3(" + p.position.x.ToString() + ", " + p.position.y.ToString() + ", " + p.position.z.ToString() + ");";
            text += "new Quaternion(" + p.rotation.x.ToString() + ", " + p.rotation.y.ToString() + ", " + p.rotation.z.ToString() + ", " + p.rotation.w.ToString() + ");\n\n";

            text += "Right Hand\n";
            p = GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform;
            text += "new Vector3(" + p.position.x.ToString() + ", " + p.position.y.ToString() + ", " + p.position.z.ToString() + ");";
            text += "new Quaternion(" + p.rotation.x.ToString() + ", " + p.rotation.y.ToString() + ", " + p.rotation.z.ToString() + ", " + p.rotation.w.ToString() + ");";

            GUIUtility.systemCopyBuffer = text;
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

        public static GameObject theboxlol = null;
        public static void PhysicalQuitbox()
        {
            GameObject thequitbox = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox");
            theboxlol = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(theboxlol.GetComponent<Rigidbody>());
            theboxlol.transform.position = thequitbox.transform.position;
            theboxlol.transform.rotation = thequitbox.transform.rotation;
            theboxlol.transform.localScale = thequitbox.transform.localScale;
            theboxlol.GetComponent<Renderer>().material = OrangeUI;
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(false);
        }

        public static void NotPhysicalQuitbox()
        {
            UnityEngine.Object.Destroy(theboxlol);
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true);
        }

        public static void DisableMouthMovement()
        {
            /*GorillaMouthFlap victim = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>();
            GorillaSpeakerLoudness victimm = (GorillaSpeakerLoudness)typeof(GorillaMouthFlap).GetField("speaker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(victim);
            typeof(GorillaSpeakerLoudness).GetField("micConnected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(victimm, false);*/
            Patches.MicPatch.returnAsNone = true;
        }

        public static void EnableMouthMovement()
        {
            /*GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = true;
            GorillaMouthFlap victim = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>();
            GorillaSpeakerLoudness victimm = (GorillaSpeakerLoudness)typeof(GorillaMouthFlap).GetField("speaker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(victim);
            typeof(GorillaSpeakerLoudness).GetField("micConnected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(victimm, true);*/
            Patches.MicPatch.returnAsNone = false;
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

        public static void GripForceLagGame()
        {
            if (rightGrab)
            {
                foreach (GameObject g in Object.FindObjectsByType<GameObject>(0)) { }
            }
        }

        public static void UncapFPS()
        {
            Application.targetFrameRate = 9999;
        }

        public static void UnlockCompetitiveQueue()
        {
            GorillaComputer.instance.CompQueueUnlockButtonPress();
        }

        public static Quaternion lastHeadQuat = Quaternion.identity;
        public static Quaternion lastLHQuat = Quaternion.identity;
        public static Quaternion lastRHQuat = Quaternion.identity;

        public static bool lastTagLag = false;
        public static int tagLagFrames = 0;

        public static void TagLagDetector()
        {
            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            {
                if (Quaternion.Angle(Classes.RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).headMesh.transform.rotation, lastHeadQuat) <= 0.01f && Quaternion.Angle(Classes.RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHandTransform.rotation, lastLHQuat) <= 0.01f && Quaternion.Angle(Classes.RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).rightHandTransform.rotation, lastRHQuat) <= 0.01f)
                {
                    tagLagFrames++;
                } else
                {
                    tagLagFrames = 0;
                }

                lastLHQuat = Classes.RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHandTransform.rotation;
                lastRHQuat = Classes.RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).rightHandTransform.rotation;
                lastHeadQuat = Classes.RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).headMesh.transform.rotation;

                bool thereIsTagLag = tagLagFrames > 512;
                if (thereIsTagLag && !lastTagLag)
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>TAG LAG</color><color=grey>]</color> <color=white>There is currently tag lag.</color>");
                }
                if (!thereIsTagLag && lastTagLag)
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> <color=white>There is no longer tag lag.</color>");
                }
                lastTagLag = thereIsTagLag;
            } else
            {
                if (lastTagLag)
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> <color=white>There is no longer tag lag.</color>");
                }
                lastTagLag = false;
            }
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
