/*
 * ii's Stupid Menu  Mods/Important.cs
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

using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Patches.Menu;
using Photon.Pun;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RandomUtilities;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;

namespace iiMenu.Mods
{
    public static class Important
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

            RoomSystem.GetRoomSizeForCreate(PhotonNetworkController.Instance.currentJoinTrigger?.networkZone ?? "forest");

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

            NetworkSystem.Instance.netState = NetworkSystem.Instance.InRoom ? NetSystemState.InGame : NetSystemState.Idle;

            partyLastCode = null;
            phaseTwo = false;
        }

        public static void JoinRandom()
        {
            if (PhotonNetwork.InRoom)
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
                CoroutineManager.instance.StartCoroutine(JoinRandomDelay());
                return;
            }

            GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest");
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger);
        }

        public static IEnumerator JoinRandomDelay()
        {
            yield return new WaitForSeconds(1.5f);
            JoinRandom();
        }

        public static async Task ForceCreateRoom(string name, RoomConfig options)
        {
            if (NetworkSystem.Instance.InRoom)
                await NetworkSystem.Instance.ReturnToSinglePlayer();

            await (NetworkSystem.Instance as NetworkSystemPUN).TryCreateRoom(name, options);
        }

        public static bool instantCreate;
        public static void CreateRoom(string roomName, bool isPublic, JoinType roomJoinType = JoinType.Solo)
        {
            RoomConfig roomConfig = new RoomConfig
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = isPublic,
                MaxPlayers = RoomSystem.GetRoomSizeForCreate(PhotonNetworkController.Instance.currentJoinTrigger.networkZone),
                CustomProps = new Hashtable
                {
                    { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.GetFullDesiredGameModeString() },
                    { "platform", PhotonNetworkController.Instance.platformTag },
                    { "queueName", GorillaComputer.instance.currentQueue }
                }
            };


            PhotonNetworkController.Instance.currentJoinType = roomJoinType;

            if (roomJoinType == JoinType.JoinWithParty || roomJoinType == JoinType.ForceJoinWithParty)
                Task.Run(PhotonNetworkController.Instance.SendPartyFollowCommands);

            if (roomJoinType == JoinType.JoinWithNearby || roomJoinType == JoinType.JoinWithElevator)
                roomConfig.SetFriendIDs(PhotonNetworkController.Instance.FriendIDList);
            else if (roomJoinType == JoinType.JoinWithParty || roomJoinType == JoinType.ForceJoinWithParty)
                roomConfig.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList());

            if (instantCreate)
            {
                (NetworkSystem.Instance as NetworkSystemPUN).internalState = NetworkSystemPUN.InternalState.Searching_Creating;
                _ = ForceCreateRoom(roomName, roomConfig);
            }
            else
                NetworkSystem.Instance.ConnectToRoom(roomName, roomConfig);
        }

        public static void BroadcastRoom(string roomName, bool create, string key, string shuffler)
        {
            string text = NetworkSystem.ShuffleRoomName(roomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler[..2], true);

            BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest
            {
                KeyToFollow = key,
                RoomToJoin = text,
                Set = create
            };

            GorillaServer.Instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate (ExecuteFunctionResult result) {}, delegate (PlayFabError error) { });
        }

        // The code below is fully safe. I know, it seems suspicious.
        public static void RestartGame()
        {
            string logoLines = "";
            foreach (string line in PluginInfo.Logo.Split(@"
"))
                logoLines += System.Environment.NewLine + "echo      " + line;
            
            string restartScript = @"@echo off
title ii's Stupid Menu
color 0E

cls
echo." + logoLines + @"
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

        public static void OpenGorillaTagFolder()
        {
            string filePath = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0];
            Process.Start(filePath);
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
            if (!XRSettings.isDeviceActive)
                return;

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

                RoomObject.SetActive(false);
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
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");

            GetPlayerData_Data newSessionData = newSessionDataTask.Result;
            if (newSessionData.responseType == GetSessionResponseType.NOT_FOUND)
            {
                Task optInTask = KIDManager.Server_OptIn();

                while (!optInTask.IsCompleted)
                    yield return null;
                if (optInTask.IsFaulted)
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");

                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully redeemed shiny rocks!");
                CosmeticsController.instance.GetCurrencyBalance();
            }
            else
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You have already redeemed the shiny rocks.");
            yield break;
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
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                vrrig.voicePitchForRelativeScale = new AnimationCurve(
                    new Keyframe(0f, 1f, 0f, 0f),
                    new Keyframe(1f, 1f, 0f, 0f)
                );
            }
        }

        public static void EnablePitchScaling()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
                vrrig.voicePitchForRelativeScale = VRRig.LocalRig.voicePitchForRelativeScale;
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

        private static float keyboardDelay;
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
                        Type compType = component.GetType();
                        string compName = compType.Name;

                        if (typeof(GorillaPressableButton).IsAssignableFrom(compType) || compName == "GorillaPressableButton" || compName == "GorillaPlayerLineButton" || compName == "CustomKeyboardKey")
                            compType.GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(component, new object[] { GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                        else
                            switch (compName)
                            {
                                case "GorillaKeyboardButton":
                                    keyboardDelay = Time.time + 0.1f;
                                    GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(Traverse.Create(component).Field("Binding").GetValue<GorillaKeyboardBindings>());
                                    break;
                            }
                    }
                }
            }
        }

        private static bool lastTagLag;
        public static void TagLagDetector()
        {
            if (PhotonNetwork.InRoom && !NetworkSystem.Instance.IsMasterClient)
            {
                VRRig masterRig = PhotonNetwork.MasterClient.VRRig();
                bool thereIsTagLag = masterRig.GetTruePing() > 1000;

                switch (thereIsTagLag)
                {
                    case true when !lastTagLag:
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>TAG LAG</color><color=grey>]</color> There is currently tag lag.");
                        break;
                    case false when lastTagLag:
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> There is no longer tag lag.");
                        break;
                }

                lastTagLag = thereIsTagLag;
            } else
            {
                if (lastTagLag)
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> There is no longer tag lag.");
                lastTagLag = false;
            }
        }

        private static bool lastSteam;
        public static void SteamDetector()
        {
            bool playerOnSteam = GorillaParent.instance.vrrigs.Any(vrrig => !vrrig.IsLocal() && vrrig.IsSteam());
            if (playerOnSteam && !lastSteam)
            {
                VRRig vrrig = GorillaParent.instance.vrrigs.First(vrrig => !vrrig.IsLocal() && vrrig.IsSteam());
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>STEAM</color><color=grey>]</color> {vrrig.GetName()} is on Steam.");

                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Safety/steam.ogg", "Audio/Mods/Safety/steam.ogg"), buttonClickVolume / 10f);
            }

            lastSteam = playerOnSteam;
        }

        public static string RandomRoomName()
        {
            while (true)
            {
                string text = RandomString();
                if (GorillaComputer.instance.CheckAutoBanListForName(text)) return text;
            }
        }
    }
}
