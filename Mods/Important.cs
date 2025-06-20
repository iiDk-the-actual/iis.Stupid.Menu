using Cinemachine;
using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Important
    {
        public static void Reconnect()
        {
            rejRoom = PhotonNetwork.CurrentRoom.Name;
            NetworkSystem.Instance.ReturnToSinglePlayer();
        }

        public static void DisconnectR()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && rightJoystickClick) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
                NetworkSystem.Instance.ReturnToSinglePlayer();
        }

        public static void ReconnectR()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled && rightJoystickClick) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
                Reconnect();
        }

        public static void CancelReconnect()
        {
            rejRoom = null;
            partyLastCode = null;
            phaseTwo = false;
        }

        public static void JoinRandom()
        {
            if (PhotonNetwork.InRoom)
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
                CoroutineManager.RunCoroutine(JoinRandomDelay());
                return;
            }

            string gamemode = PhotonNetworkController.Instance.currentJoinTrigger == null ? "forest" : PhotonNetworkController.Instance.currentJoinTrigger.networkZone;
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.GetJoinTriggerForZone(gamemode), JoinType.Solo);
        }

        public static IEnumerator JoinRandomDelay()
        {
            yield return new WaitForSeconds(1f);
            JoinRandom();
        }

        public static void JoinRandomR()
        {
            if ((GetIndex("Primary Room Mods").enabled && rightPrimary) || (GetIndex("Secondary Room Mods").enabled && rightSecondary) || (GetIndex("Joystick Room Mods").enabled &&  rightJoystickClick) || !(GetIndex("Primary Room Mods").enabled || GetIndex("Secondary Room Mods").enabled || GetIndex("Joystick Room Mods").enabled))
                JoinRandom();
        }

        public static void CreateRoom(string roomName, bool isPublic)
        {
            RoomConfig roomConfig = new RoomConfig()
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = isPublic,
                MaxPlayers = RoomSystem.GetRoomSizeForCreate(PhotonNetworkController.Instance.currentJoinTrigger.networkZone),
                CustomProps = new ExitGames.Client.Photon.Hashtable()
                {
                    { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.GetFullDesiredGameModeString() },
                    { "platform", PhotonNetworkController.Instance.platformTag },
                    { "queueName", GorillaComputer.instance.currentQueue }
                }
            };
            NetworkSystem.Instance.ConnectToRoom(roomName, roomConfig);
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
                wasenabled = TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled;
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

        private static bool reportMenuToggle;
        public static void OculusReportMenu()
        {
            if (leftPrimary && !reportMenuToggle)
            {
                GorillaMetaReport metaReporting = GameObject.Find("Miscellaneous Scripts").transform.Find("MetaReporting").GetComponent<GorillaMetaReport>();
                metaReporting.gameObject.SetActive(true);
                metaReporting.enabled = true;

                metaReporting.StartOverlay();
            }
            reportMenuToggle = leftPrimary;
        }

        private static float acceptTOSCheckDelay;
        public static void AcceptTOS()
        {
            Patches.TOSPatch.enabled = true;

            if (Time.time > acceptTOSCheckDelay)
            {
                acceptTOSCheckDelay = Time.time + 1f;

                Transform MiscellaneousScripts = GameObject.Find("Miscellaneous Scripts").transform;

                GameObject popupMessage = MiscellaneousScripts.Find("PopUpMessage").gameObject;

                popupMessage?.SetActive(false);
                GameObject metaReporting = MiscellaneousScripts.Find("MetaReporting").gameObject;

                metaReporting?.SetActive(false);
                GameObject RoomObject = MiscellaneousScripts.Find("PrivateUIRoom_HandRays").gameObject;
                if (RoomObject == null)
                    return;

                PrivateUIRoom Room = RoomObject.GetComponent<PrivateUIRoom>();

                if (Room.inOverlay)
                    PrivateUIRoom.StopOverlay();

                HandRayController.instance.enabled = false;
                HandRayController.instance.DisableHandRays();

                if (HandRayController.instance._leftHandRay.gameObject.activeSelf || HandRayController.instance._rightHandRay.gameObject.activeSelf)
                    HandRayController.instance.HideHands();
                HandRayController.instance.transform.Find("UIRoot").gameObject.SetActive(false);
            }
        }

        public static void JoinDiscord() =>
            Process.Start(serverLink);

        public static void CopyPlayerPosition()
        {
            string text = "Body\n";
            Transform p = GorillaTagger.Instance.bodyCollider.transform;
            text += $"new Vector3({p.position.x.ToString()}f, {p.position.y.ToString()}f, {p.position.z.ToString()}f);";
            text += $"new Quaternion({p.rotation.x.ToString()}f, {p.rotation.y.ToString()}f, {p.rotation.z.ToString()}f, {p.rotation.w.ToString()}f);\n\n";

            text += "Head\n";
            p = GorillaTagger.Instance.headCollider.transform;
            text += $"new Vector3({p.position.x.ToString()}f, {p.position.y.ToString()}f, {p.position.z.ToString()}f);";
            text += $"new Quaternion({p.rotation.x.ToString()}f, {p.rotation.y.ToString()}f, {p.rotation.z.ToString()}f, {p.rotation.w.ToString()}f);\n\n";

            text += "Left Hand\n";
            p = VRRig.LocalRig.leftHand.rigTarget.transform;
            text += $"new Vector3({p.position.x.ToString()}f, {p.position.y.ToString()}f, {p.position.z.ToString()}f);";
            text += $"new Quaternion({p.rotation.x.ToString()}f, {p.rotation.y.ToString()}f, {p.rotation.z.ToString()}f, {p.rotation.w.ToString()}f);\n\n";

            text += "Right Hand\n";
            p = VRRig.LocalRig.rightHand.rigTarget.transform;
            text += $"new Vector3({p.position.x.ToString()}f, {p.position.y.ToString()}f, {p.position.z.ToString()}f);";
            text += $"new Quaternion({p.rotation.x.ToString()}f, {p.rotation.y.ToString()}f, {p.rotation.z.ToString()}f, {p.rotation.w.ToString()}f);";

            GUIUtility.systemCopyBuffer = text;
        }

        public static GameObject theboxlol = null;
        public static void PhysicalQuitbox()
        {
            GameObject thequitbox = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox");
            theboxlol = GameObject.CreatePrimitive(PrimitiveType.Cube);
            theboxlol.transform.position = thequitbox.transform.position;
            theboxlol.transform.rotation = thequitbox.transform.rotation;
            theboxlol.transform.localScale = thequitbox.transform.localScale;
            theboxlol.GetComponent<Renderer>().material = OrangeUI;
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(false);
        }

        public static void NotPhysicalQuitbox()
        {
            Object.Destroy(theboxlol);
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true);
        }

        public static void DisableMouthMovement()
        {
            VRRig.LocalRig.shouldSendSpeakingLoudness = false;
            Patches.MicPatch.returnAsNone = true;
        }

        public static void EnableMouthMovement()
        {
            VRRig.LocalRig.shouldSendSpeakingLoudness = true;
            Patches.MicPatch.returnAsNone = false;
        }

        private static float lastTime;
        public static void CapFPS(int fps)
        {
            float targetDelta = 1f / fps;
            float elapsed = Time.realtimeSinceStartup - lastTime;

            if (elapsed < targetDelta)
            {
                int sleepMs = Mathf.FloorToInt((targetDelta - elapsed) * 1000);
                if (sleepMs > 0)
                    Thread.Sleep(sleepMs);
            }

            lastTime = Time.realtimeSinceStartup;
        }

        public static void UncapFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = int.MaxValue;
        }

        private static float keyboardDelay = 0f;
        public static void PCButtonClick()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out var Ray, 512f, NoInvisLayerMask());

                foreach (Component component in Ray.collider.GetComponents<Component>())
                {
                    System.Type compType = component.GetType();
                    string compName = compType.Name;

                    if (compName == "GorillaPressableButton" || typeof(GorillaPressableButton).IsAssignableFrom(compType) || (compName == "GorillaPlayerLineButton" && Time.time > keyboardDelay))
                        compType.GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[] { GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });

                    if (compName == "CustomKeyboardKey" && Time.time > keyboardDelay)
                    {
                        keyboardDelay = Time.time + 0.1f;
                        compType.GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[] { GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                    }

                    if (compName == "GorillaKeyboardButton" && Time.time > keyboardDelay)
                    {
                        keyboardDelay = Time.time + 0.1f;
                        GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(Traverse.Create(component).Field("Binding").GetValue<GorillaKeyboardBindings>());
                    }
                }
            }
        }


        public static Quaternion lastHeadQuat = Quaternion.identity;
        public static Quaternion lastLHQuat = Quaternion.identity;
        public static Quaternion lastRHQuat = Quaternion.identity;

        public static bool lastTagLag = false;
        public static int tagLagFrames = 0;

        public static void TagLagDetector()
        {
            if (PhotonNetwork.InRoom && !NetworkSystem.Instance.IsMasterClient)
            {
                if (Quaternion.Angle(RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).headMesh.transform.rotation, lastHeadQuat) <= 0.01f && Quaternion.Angle(RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHandTransform.rotation, lastLHQuat) <= 0.01f && Quaternion.Angle(RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).rightHandTransform.rotation, lastRHQuat) <= 0.01f)
                {
                    tagLagFrames++;
                } else
                {
                    tagLagFrames = 0;
                }

                lastLHQuat = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHandTransform.rotation;
                lastRHQuat = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).rightHandTransform.rotation;
                lastHeadQuat = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).headMesh.transform.rotation;

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

        public static string RandomRoomName()
        {
            string text = GenerateRandomString(4);

            if (GorillaComputer.instance.CheckAutoBanListForName(text))
                return text;
            
            return RandomRoomName();
        }
    }
}
