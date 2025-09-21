using GorillaNetworking;
using HarmonyLib;
using iiMenu.Managers;
using iiMenu.Notifications;
using iiMenu.Patches.Menu;
using Photon.Pun;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Important
    {
        public static string oldId = "";

        public async static void CheckNewAcc()
        {
            await Task.Delay(10000);

            if (PhotonNetwork.LocalPlayer.UserId != oldId)
                playTime = 0f;
        }

        public static Coroutine queueCoroutine;
        public static int reconnectDelay = 1;

        public static IEnumerator QueueRoomCoroutine(string roomName)
        {
            NetworkSystemPUN instance = (NetworkSystemPUN)NetworkSystem.Instance;

            instance.ReturnToSinglePlayer();
            yield return new WaitUntil(() => instance.netState == NetSystemState.Idle);
            yield return new WaitForSeconds(0.5f);

            instance.netState = NetSystemState.Connecting;

            byte maxPlayers = RoomSystem.GetRoomSizeForCreate(PhotonNetworkController.Instance.currentJoinTrigger?.networkZone ?? "forest");
            RoomConfig opts = new RoomConfig()
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = false,
                MaxPlayers = maxPlayers,
                CustomProps = new ExitGames.Client.Photon.Hashtable()
                {
                    { "gameMode", (PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest")).GetFullDesiredGameModeString() },
                    { "platform", PhotonNetworkController.Instance.platformTag },
                    { "queueName", GorillaComputer.instance.currentQueue }
                }
            };

            while (!instance.InRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomName, JoinType.Solo);
                yield return new WaitForSeconds(reconnectDelay);
            }
        }

        public static void QueueRoom(string roomName)
        {
            if (queueCoroutine != null)
                CoroutineManager.instance.StopCoroutine(queueCoroutine);

            queueCoroutine = CoroutineManager.instance.StartCoroutine(QueueRoomCoroutine(roomName));
        }

        public static void Reconnect()
        {
            string roomName = NetworkSystem.Instance.RoomName;

            NetworkSystem.Instance.ReturnToSinglePlayer();
            QueueRoom(roomName);
        }

        public static void CancelReconnect()
        {
            if (queueCoroutine != null)
                CoroutineManager.instance.StopCoroutine(queueCoroutine);

            if (NetworkSystem.Instance.InRoom)
                NetworkSystem.Instance.netState = NetSystemState.InGame;
            else
                NetworkSystem.Instance.netState = NetSystemState.Idle;

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

            GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest");
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger, JoinType.Solo);
        }

        public static IEnumerator JoinRandomDelay()
        {
            yield return new WaitForSeconds(1.5f);
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
            string restartScript = @"@echo off
title ii's Stupid Menu
color 0E

cls
echo.
echo      ••╹   ┏┓     • ┓  ┳┳┓      
echo      ┓┓ ┏  ┗┓╋┓┏┏┓┓┏┫  ┃┃┃┏┓┏┓┓┏
echo      ┗┗ ┛  ┗┛┗┗┻┣┛┗┗┻  ┛ ┗┗ ┛┗┗┻
echo                 ┛               
echo.

echo Your game is restarting, please wait...
echo.

:WAIT_LOOP
tasklist /FI ""IMAGENAME eq Gorilla Tag.exe"" | find /I ""Gorilla Tag.exe"" >nul
if %ERRORLEVEL%==0 (
    timeout /t 1 >nul
    goto WAIT_LOOP
)

start steam://run/1533390
exit";

            string fileName = $"{PluginInfo.BaseDirectory}/RestartScript.bat";

            File.WriteAllText(fileName, restartScript);

            string filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;

            Process.Start(filePath);
            Application.Quit();
        }

#pragma warning disable CS0618 // Type or member is obsolete
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
                if (menu != null && !XRSettings.isDeviceActive)
                    return;

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
#pragma warning restore CS0618 // Type or member is obsolete

        public static void ForceEnableHands()
        {
            ConnectedControllerHandler.Instance.leftControllerValid = true;
            ConnectedControllerHandler.Instance.rightControllerValid = true;

            ConnectedControllerHandler.Instance.leftValid = true;
            ConnectedControllerHandler.Instance.rightValid = true;
        }

        private static bool reportMenuToggle;
        public static void OculusReportMenu()
        {
            if (leftPrimary && !reportMenuToggle)
            {
                GorillaMetaReport metaReporting = GetObject("Miscellaneous Scripts").transform.Find("MetaReporting").GetComponent<GorillaMetaReport>();
                metaReporting.gameObject.SetActive(true);
                metaReporting.enabled = true;

                metaReporting.StartOverlay();
            }
            reportMenuToggle = leftPrimary;
        }

        private static bool acceptedTOS;
        public static void AcceptTOS()
        {
            GameObject RoomObject = GetObject("Miscellaneous Scripts").transform.Find("PrivateUIRoom_HandRays").gameObject;
            if (RoomObject == null)
                return;

            HandRayController HandRayController = RoomObject.GetComponent<HandRayController>();
            PrivateUIRoom PrivateUIRoom = RoomObject.GetComponent<PrivateUIRoom>();

            if (!acceptedTOS && PrivateUIRoom.inOverlay)
            {
                HandRayController.DisableHandRays();

                PrivateUIRoom.overlayForcedActive = false;
                PrivateUIRoom.StopOverlay();

                RoomObject?.SetActive(false);
                if (!TOSPatch.enabled)
                {
                    GorillaTagger.Instance.tapHapticStrength = 0.5f;
                    GorillaSnapTurn.LoadSettingsFromCache();
                    TOSPatch.enabled = true;
                }

                acceptedTOS = true;
            }
        }
        
        public static IEnumerator RedeemShinyRocks()
        {
            Task<GetPlayerData_Data> newSessionDataTask = KIDManager.TryGetPlayerData(true); 

            while (!newSessionDataTask.IsCompleted)
                yield return null;
            if (newSessionDataTask.IsFaulted)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");

            GetPlayerData_Data newSessionData = newSessionDataTask.Result;
            if (newSessionData.responseType == GetSessionResponseType.NOT_FOUND)
            {
                Task optInTask = KIDManager.Server_OptIn();

                while (!optInTask.IsCompleted)
                    yield return null;
                if (optInTask.IsFaulted)
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");

                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully redeemed shiny rocks!");
                CosmeticsController.instance.GetCurrencyBalance();
            }
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You have already redeemed the shiny rocks.");
        }

        public static void JoinDiscord() =>
            Process.Start(serverLink);

        public static void CopyPlayerPosition()
        {
            string text = "Body\n";
            Transform p = GorillaTagger.Instance.bodyCollider.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);\n\n";

            text += "Head\n";
            p = GorillaTagger.Instance.headCollider.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);\n\n";

            text += "Left Hand\n";
            p = VRRig.LocalRig.leftHand.rigTarget.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);\n\n";

            text += "Right Hand\n";
            p = VRRig.LocalRig.rightHand.rigTarget.transform;
            text += $"new Vector3({p.position.x}f, {p.position.y}f, {p.position.z}f);";
            text += $"new Quaternion({p.rotation.x}f, {p.rotation.y}f, {p.rotation.z}f, {p.rotation.w}f);";

            GUIUtility.systemCopyBuffer = text;
        }

        public static GameObject physicalQuitBox;
        public static void PhysicalQuitbox()
        {
            GameObject quitBox = GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox");
            physicalQuitBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            physicalQuitBox.transform.position = quitBox.transform.position;
            physicalQuitBox.transform.rotation = quitBox.transform.rotation;
            physicalQuitBox.transform.localScale = quitBox.transform.localScale;
            physicalQuitBox.GetComponent<Renderer>().material = OrangeUI;

            quitBox.SetActive(false);
        }

        public static void DisablePhysicalQuitbox()
        {
            Object.Destroy(physicalQuitBox);
            GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true);
        }

        public static void DisablePitchScaling()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                    vrrig.voicePitchForRelativeScale = new AnimationCurve(
                        new Keyframe(0f, 1f, 0f, 0f),
                        new Keyframe(1f, 1f, 0f, 0f)
                    );
            }
        }

        public static void EnablePitchScaling()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                    vrrig.voicePitchForRelativeScale = VRRig.LocalRig.voicePitchForRelativeScale;
            }
        }

        public static void DisableMouthMovement()
        {
            VRRig.LocalRig.shouldSendSpeakingLoudness = false;
            MicPatch.enabled = true;
        }

        public static void EnableMouthMovement()
        {
            VRRig.LocalRig.shouldSendSpeakingLoudness = true;
            MicPatch.enabled = false;
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

                if (Time.time > keyboardDelay)
                {
                    foreach (Component component in Ray.collider.GetComponents<Component>())
                    {
                        System.Type compType = component.GetType();
                        string compName = compType.Name;

                        if (compName == "GorillaPressableButton" || typeof(GorillaPressableButton).IsAssignableFrom(compType) || (compName == "GorillaPlayerLineButton"))
                            compType.GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[] { GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });

                        if (compName == "CustomKeyboardKey")
                        {
                            keyboardDelay = Time.time + 0.1f;
                            compType.GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[] { GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                        }

                        if (compName == "GorillaKeyboardButton")
                        {
                            keyboardDelay = Time.time + 0.1f;
                            GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(Traverse.Create(component).Field("Binding").GetValue<GorillaKeyboardBindings>());
                        }
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
                if (Quaternion.Angle(RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).head.syncRotation, lastHeadQuat) <= 0.01f && Quaternion.Angle(RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHand.syncRotation, lastLHQuat) <= 0.01f && Quaternion.Angle(RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).rightHand.syncRotation, lastRHQuat) <= 0.01f)
                    tagLagFrames++;
                else
                    tagLagFrames = 0;

                lastLHQuat = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHand.syncRotation;
                lastRHQuat = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).leftHand.syncRotation;
                lastHeadQuat = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient).head.syncRotation;

                bool thereIsTagLag = tagLagFrames > 255;
                if (thereIsTagLag && !lastTagLag)
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>TAG LAG</color><color=grey>]</color> <color=white>There is currently tag lag.</color>");
                if (!thereIsTagLag && lastTagLag)
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> <color=white>There is no longer tag lag.</color>");
                lastTagLag = thereIsTagLag;
            } else
            {
                if (lastTagLag)
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> <color=white>There is no longer tag lag.</color>");
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
