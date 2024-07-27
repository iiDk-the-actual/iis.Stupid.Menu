using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
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
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/ModeSelector_Group").SetActive(false);
        }

        public static void EnableGamemodeButtons()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/ModeSelector_Group").SetActive(true);
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

        public static int antireportrangeindex = 0;
        private static float threshold = 0.35f;

        public static void ChangeAntiReportRange()
        {
            string[] names = new string[]
            {
                "Default", // The report button
                "Large", // The report button within the range of 5 people
                "Massive" // The entire fucking board
            };
            float[] distances = new float[]
            {
                0.35f,
                0.7f,
                1.5f
            };
            antireportrangeindex++;
            if (antireportrangeindex > names.Length - 1)
            {
                antireportrangeindex = 0;
            }

            threshold = distances[antireportrangeindex];
            GetIndex("carrg").overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>" + names[antireportrangeindex] + "</color><color=grey>]</color>";
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

                                if (D1 < threshold || D2 < threshold)
                                {
                                    PhotonNetwork.Disconnect();
                                    RPCProtection();
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected.");
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

                                if (D1 < threshold || D2 < threshold)
                                {
                                    rejRoom = PhotonNetwork.CurrentRoom.Name;
                                    // rejDebounce = Time.time + 2f;
                                    PhotonNetwork.Disconnect();
                                    RPCProtection();
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
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

                                if (D1 < threshold || D2 < threshold)
                                {
                                    PhotonNetwork.Disconnect();
                                    RPCProtection();
                                    isJoiningRandom = true;
                                    //jrDebounce = Time.time + (float)internetTime;
                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be connected to a random lobby shortly.");
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

                                if ((D1 < threshold || D2 < threshold) && Time.time > kgDebounce)
                                {
                                    hasFoundAnnoyance = true;
                                    if (!riskyModsEnabled)
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod has been disabled due to security.");
                                    }
                                    else
                                    {
                                        int num = RigManager.GetPhotonViewFromVRRig(vrrig).ViewID;
                                        Hashtable ServerCleanDestroyEvent = new Hashtable();
                                        RaiseEventOptions ServerCleanOptions = new RaiseEventOptions
                                        {
                                            CachingOption = EventCaching.RemoveFromRoomCache
                                        };
                                        ServerCleanDestroyEvent[0] = num;
                                        ServerCleanOptions.CachingOption = EventCaching.AddToRoomCache;
                                        PhotonNetwork.NetworkingClient.OpRaiseEvent(204, ServerCleanDestroyEvent, ServerCleanOptions, SendOptions.SendUnreliable);
                                        RPCProtection();
                                    }

                                    vrrig.leftHandTransform.position = Vector3.zero;
                                    vrrig.rightHandTransform.position = Vector3.zero;

                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they are being lagged.");
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

                                if (D1 < threshold || D2 < threshold)
                                {
                                    hasFoundAnnoyance = true;
                                    if (!riskyModsEnabled)
                                    {
                                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod has been disabled due to security.");
                                    }
                                    else
                                    {
                                        int num = RigManager.GetPhotonViewFromVRRig(vrrig).ViewID;
                                        Hashtable ServerCleanDestroyEvent = new Hashtable();
                                        RaiseEventOptions ServerCleanOptions = new RaiseEventOptions
                                        {
                                            CachingOption = EventCaching.RemoveFromRoomCache
                                        };
                                        ServerCleanDestroyEvent[0] = num;
                                        ServerCleanOptions.CachingOption = EventCaching.AddToRoomCache;
                                        PhotonNetwork.NetworkingClient.OpRaiseEvent(204, ServerCleanDestroyEvent, ServerCleanOptions, SendOptions.SendUnreliable);
                                        RPCProtection();
                                    }

                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they are being crashed.");
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
                if (!vrrig.isOfflineVRRig && vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK") || vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAD"))
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
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-MODERATOR</color><color=grey>]</color> There was a moderator in your lobby, you have been disconnected. Their Player ID and Room Code have been saved to a file.");
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

        public static void AntiOculusReportOn()
        {
            AntiOculusReport = true;
        }

        public static void AntiOculusReportOff()
        {
            AntiOculusReport = false;
        }

        public static void AntiACReportOn()
        {
            AntiACReport = true;
        }

        public static void AntiACReportOff()
        {
            AntiACReport = false;
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
            string randomName = "gorilla";
            for (var i = 0; i < 4; i++)
            {
                randomName = randomName + UnityEngine.Random.Range(0, 9).ToString();
            }

            ChangeName(randomName);

            byte randA = (byte)UnityEngine.Random.Range(0, 255);
            byte randB = (byte)UnityEngine.Random.Range(0, 255);
            byte randC = (byte)UnityEngine.Random.Range(0, 255);
            ChangeColor(new Color32(randA, randB, randC, 255));
        }

        public static void ChangeIdentityRegular()
        {
            string[] names = new string[]
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "SHIBAGT",
                "PBBV",
                "J3VU",
                "BEES",
                "NAMO",
                "MANGO",
                "FROSTY",
                "FRISH",
                "LITTLETIMMY",
                "SILLYBILLY",
                "TIMMY",
                "MINIGAMES",
                "MINIGAMESKID",
                "JMANCURLY",
                "VMT",
                "ELLIOT",
                "DEEP",
                "BTC",
                "KMAN",
                "YOSEF",
                "POLAR",
                "3CLIPCE",
                "GORILLAVR",
                "GORILLAVRGT",
                "GORILLAGTVR",
                "GORILLAGT",
                "SHARKPUPPET",
                "DUCKY",
                "EDDIE",
                "EDDY",
                "CASEOH",
                "SKETCH",
                "WATERMELON",
                "CRAZY",
                "MONK",
                "MONKE",
                "MONKI",
                "MONKEY",
                "MONKIY",
                "GORILL",
                "GOORILA",
                "GORILLA",
                "REDBERRY",
                "FOX",
                "RUFUS"
            };

            ChangeName(names[UnityEngine.Random.Range(0, names.Length - 1)]);

            Color[] colors = new Color[]
            {
                Color.cyan,
                Color.yellow,
                Color.blue,
                Color.gray,
                Color.black,
                Color.white,
                Color.magenta,
                Color.yellow,
                Color.green,
                new Color(1f, 0.5f, 1f, 255f),
                new Color(0f, 0.5f, 0f, 255f)
            };
            ChangeColor(colors[UnityEngine.Random.Range(0, colors.Length - 1)]);
        }

        private static bool lastinlobbyagain = false;
        public static void ChangeIdentityOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
            {
                ChangeIdentity();
            }
            lastinlobbyagain = PhotonNetwork.InRoom;
        }
        public static void ChangeIdentityRegularOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
            {
                ChangeIdentityRegular();
            }
            lastinlobbyagain = PhotonNetwork.InRoom;
        }
        public static void ChangeIdentityMinigamesOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
            {
                Fun.BecomeMinigamesKid();
            }
            lastinlobbyagain = PhotonNetwork.InRoom;
        }

        private static float stupidannoyingthing = 0f;
        private static int lastPlayerCount = -1;
        public static void NameSpoof()
        {
            string randomName = "";
            for (int i = 0; i < 12; i++)
            {
                randomName += letters[UnityEngine.Random.Range(0, letters.Length - 1)];
            }

            if (PhotonNetwork.InRoom)
            {
                if (lastPlayerCount != -1)
                {
                    if (PhotonNetwork.PlayerList.Length != lastPlayerCount)
                    {
                        stupidannoyingthing = Time.time + 1f;
                    }
                }
                lastPlayerCount = PhotonNetwork.PlayerList.Length;
                if (Time.time > stupidannoyingthing && stupidannoyingthing != -1)
                {
                    FakeName(randomName);
                    stupidannoyingthing = -1f;
                }
            } else
            {
                lastPlayerCount = -1;
                stupidannoyingthing = Time.time + 1f;
            }
        }
    }
}
