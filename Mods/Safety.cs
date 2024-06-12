using GorillaNetworking;
using iiMenu.Notifications;
using Photon.Pun;
using System.IO;
using UnityEngine;
using Valve.VR;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Safety
    {
        public static void NoFinger()
        {
            ControllerInputPoller.instance.leftControllerGripFloat = 0f;
            ControllerInputPoller.instance.rightControllerGripFloat = 0f;
            ControllerInputPoller.instance.leftControllerIndexFloat = 0f;
            ControllerInputPoller.instance.rightControllerIndexFloat = 0f;
            ControllerInputPoller.instance.leftControllerPrimaryButton = false;
            ControllerInputPoller.instance.leftControllerSecondaryButton = false;
            ControllerInputPoller.instance.rightControllerPrimaryButton = false;
            ControllerInputPoller.instance.rightControllerSecondaryButton = false;
            ControllerInputPoller.instance.leftControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.leftControllerSecondaryButtonTouch = false;
            ControllerInputPoller.instance.rightControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.rightControllerSecondaryButtonTouch = false;
        }

        public static bool lastjsi = false;
        public static bool isActive = true;
        public static void ToggleIgloo()
        {
            bool fuck = SteamVR_Actions.gorillaTag_RightJoystickClick.state;
            if (fuck && !lastjsi)
            {
                isActive = !isActive;
                GameObject.Find("Mountain/Geometry/goodigloo").SetActive(isActive);
            }
            lastjsi = fuck;
        }

        public static void DisableGamemodeButtons()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Selector Buttons/anchor/ENABLE FOR BETA").SetActive(false);
        }

        public static void EnableGamemodeButtons()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Selector Buttons/anchor/ENABLE FOR BETA").SetActive(true);
        }

        public static void SpoofSupportPage()
        {
            GorillaComputer.instance.screenText.Text = GorillaComputer.instance.screenText.Text.Replace("STEAM", "QUEST").Replace(GorillaComputer.instance.buildDate, "05/30/2024 16:50:12\nBUILD CODE 4893\nMANAGED ACCOUNT: NO");
        }

        public static void AntiCrashEnabled()
        {
            //GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools").SetActive(false);
            AntiCrashToggle = true;
        }

        public static void AntiCrashDisabled()
        {
            //GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools").SetActive(true);
            AntiCrashToggle = false;
        }

        public static void EnableAntiHandTap()
        {
            //GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools").SetActive(false);
            AntiSoundToggle = true;
        }

        public static void DisableAntiHandTap()
        {
            //GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools").SetActive(true);
            AntiSoundToggle = false;
        }

        public static void AntiReportDisconnect()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                float threshold = 0.35f;

                                if (D1 < threshold || D2 < threshold)
                                {
                                    PhotonNetwork.Disconnect();
                                    RPCProtection();
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>Someone attempted to report you, you have been disconnected.</color>");
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportReconnect()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                float threshold = 0.35f;

                                if (D1 < threshold || D2 < threshold)
                                {
                                    rejRoom = PhotonNetwork.CurrentRoom.Name;
                                    // rejDebounce = Time.time + 2f;
                                    PhotonNetwork.Disconnect();
                                    RPCProtection();
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>" + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.</color>");
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportJoinRandom()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                float threshold = 0.35f;

                                if (D1 < threshold || D2 < threshold)
                                {
                                    PhotonNetwork.Disconnect();
                                    RPCProtection();
                                    isJoiningRandom = true;
                                    //jrDebounce = Time.time + (float)internetTime;
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>" + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be connected to a random lobby shortly.</color>");
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static bool lastannoy = false;
        public static void AntiReportLag()
        {
            try
            {
                bool hasFoundAnnoyance = false;
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                float threshold = 0.35f;

                                if ((D1 < threshold || D2 < threshold) && Time.time > kgDebounce)
                                {
                                    hasFoundAnnoyance = true;
                                    Overpowered.StepCrashMethod(GetPlayerFromVRRig(vrrig));

                                    vrrig.leftHandTransform.position = Vector3.zero;
                                    vrrig.rightHandTransform.position = Vector3.zero;

                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>" + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they are being lagged.</color>");
                                }
                            }
                        }
                    }
                }
                if (hasFoundAnnoyance && !lastannoy)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
                lastannoy = hasFoundAnnoyance;
            }
            catch { } // Not connected
        }
        
        public static void AntiReportCrash()
        {
            try
            {
                bool hasFoundAnnoyance = false;
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                float threshold = 0.35f;

                                if (D1 < threshold || D2 < threshold)
                                {
                                    hasFoundAnnoyance = true;
                                    Overpowered.StepCrashMethod(GetPlayerFromVRRig(vrrig));

                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>" + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they are being crashed.</color>");
                                }
                            }
                        }
                    }
                }
                if (hasFoundAnnoyance && !lastannoy)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
                lastannoy = hasFoundAnnoyance;
            }
            catch { } // Not connected
        }

        public static void AntiModerator()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig && vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK"))
                {
                    try
                    {

                        VRRig plr = vrrig;
                        Photon.Realtime.Player player = GetPlayerFromVRRig(plr);
                        if (player != null)
                        {
                            string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
                            float r = 0f;
                            float g = 0f;
                            float b = 0f;
                            try
                            {
                                
                                r = plr.playerColor.r * 255;
                                g = plr.playerColor.r * 255;
                                b = plr.playerColor.r * 255;
                            }
                            catch { UnityEngine.Debug.Log("Failed to log colors, rig most likely nonexistent"); }

                            try
                            {
                                text += "\n====================================\n";
                                text += string.Concat(new string[]
                                {
                                    "Player Name: \"",
                                    player.NickName,
                                    "\", Player ID: \"",
                                    player.UserId,
                                    "\", Player Color: (R: ",
                                    r.ToString(),
                                    ", G: ",
                                    g.ToString(),
                                    ", B: ",
                                    b.ToString(),
                                    ")"
                                });
                            }
                            catch { UnityEngine.Debug.Log("Failed to log player"); }

                            text += "\n====================================\n";
                            text += "Text file generated with ii's Stupid Menu";
                            string fileName = "iisStupidMenu/" + player.NickName + " - Anti Moderator.txt";
                            if (!Directory.Exists("iisStupidMenu"))
                            {
                                Directory.CreateDirectory("iisStupidMenu");
                            }
                            File.WriteAllText(fileName, text);
                        }
                    }
                    catch { }
                    PhotonNetwork.Disconnect();
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>There was a moderator in your lobby, you have been disconnected. Their Player ID and Room Code have been saved to a file.</color>");
                }
            }
        }

        public static void EnableACReportSelf()
        {
            AntiCheatSelf = true;
        }

        public static void DisableACReportSelf()
        {
            AntiCheatSelf = false;
        }

        public static void EnableACReportAll()
        {
            AntiCheatAll = true;
        }

        public static void DisableACReportAll()
        {
            AntiCheatAll = false;
        }

        public static void ChangeIdentity()
        {
            string randomName = "GORILLA";
            for (var i = 0; i < 4; i++)
            {
                randomName = randomName + UnityEngine.Random.Range(0, 9).ToString();
            }

            ChangeName(randomName);

            byte randA = (byte)UnityEngine.Random.Range(0, 255);
            byte randB = (byte)UnityEngine.Random.Range(0, 255);
            byte randC = (byte)UnityEngine.Random.Range(0, 255);
            ChangeColor(new Color32(randA, randB, randC, 255));

            for (var i = 0; i < 50; i++)
            {
                Fun.SpazAccessories();
            }
        }
    }
}
