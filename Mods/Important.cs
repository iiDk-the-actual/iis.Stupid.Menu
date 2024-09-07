using Cinemachine;
using GorillaNetworking;
using iiMenu.Notifications;
using Photon.Pun;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Important
    {
        public static void Disconnect()
        {
            PhotonNetwork.Disconnect(); // bruh
        }

        public static void Reconnect()
        {
            rejRoom = PhotonNetwork.CurrentRoom.Name;
            //rejDebounce = Time.time + (float)internetTime;
            PhotonNetwork.Disconnect();
        }

        public static void DisconnectR()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && SteamVR_Actions.gorillaTag_RightJoystickClick.state) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
            {
                Disconnect();
            }
        }

        public static void ReconnectR()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && SteamVR_Actions.gorillaTag_RightJoystickClick.state) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
            {
                Reconnect();
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
            //PhotonNetworkController.Instance.currentJoinTrigger.OnBoxTriggered();

            string gamemode = PhotonNetworkController.Instance.currentJoinTrigger.networkZone;

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
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.Disconnect();
                isJoiningRandom = true;
            }
            else
            {
                ActJoinRandom();
            }
        }

        public static void JoinRandomR()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && SteamVR_Actions.gorillaTag_RightJoystickClick.state) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
            {
                JoinRandom();
            }
        }

        public static void CreateRoom(string roomName, bool isPublic) // Once again thanks to Shiny for discovering a thing that doesn't work anymore
        {
            PhotonNetworkController.Instance.currentJoinTrigger = GorillaComputer.instance.GetJoinTriggerForZone("forest");
            UnityEngine.Debug.Log((string)typeof(PhotonNetworkController).GetField("platformTag", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PhotonNetworkController.Instance));
            RoomConfig roomConfig = new RoomConfig()
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = isPublic,
                MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentJoinTrigger.networkZone),
                CustomProps = new ExitGames.Client.Photon.Hashtable()
                {
                    { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.GetFullDesiredGameModeString() },
                    { "platform", (string)typeof(PhotonNetworkController).GetField("platformTag", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PhotonNetworkController.Instance) }
                }
            };
            NetworkSystem.Instance.ConnectToRoom(roomName, roomConfig);
        }

        public static void CreatePublic() 
        {
            CreateRoom(RandomRoomName(), true);
        }

        public static void iisStupidMenuRoom()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("<$II_"+PluginInfo.Version+">", JoinType.Solo);
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

        private static bool wasenabled = true;

        public static void EnableFPC()
        {
            if (TPC != null)
            {
                TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = wasenabled;
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

        public static void DisableFPC()
        {
            if (TPC != null)
            {
                TPC.GetComponent<Camera>().fieldOfView = 60f;
                TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = wasenabled;
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
                GorillaMetaReport gr = GameObject.Find("Miscellaneous Scripts").transform.Find("MetaReporting").GetComponent<GorillaMetaReport>();
                gr.gameObject.SetActive(true);
                gr.enabled = true;
                MethodInfo inf = typeof(GorillaMetaReport).GetMethod("StartOverlay", BindingFlags.NonPublic | BindingFlags.Instance);
                inf.Invoke(gr, null);
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

        public static void DisableMapTriggers()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab").SetActive(false);
        }

        public static void EnableMapTriggers()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab").SetActive(true);
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
            GorillaTagger.Instance.offlineVRRig.shouldSendSpeakingLoudness = false;
            typeof(VRRig).GetField("speakingLoudness", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GorillaTagger.Instance.offlineVRRig, 0f);
            Patches.MicPatch.returnAsNone = true;
        }

        public static void EnableMouthMovement()
        {
            /*GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = true;
            GorillaMouthFlap victim = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>();
            GorillaSpeakerLoudness victimm = (GorillaSpeakerLoudness)typeof(GorillaMouthFlap).GetField("speaker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(victim);
            typeof(GorillaSpeakerLoudness).GetField("micConnected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(victimm, true);*/
            GorillaTagger.Instance.offlineVRRig.shouldSendSpeakingLoudness = true;
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
            Application.targetFrameRate = 1024;
        }

        public static void PCButtonClick()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out var Ray, 512f, NoInvisLayerMask());
                GorillaPressableButton possibly = Ray.collider.GetComponentInParent<GorillaPressableButton>();
                if (possibly)
                {
                    typeof(GorillaPressableButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(possibly, new object[] { GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                }
            }
        }

        public static void CapFPS()
        {
            Application.targetFrameRate = 144;
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
