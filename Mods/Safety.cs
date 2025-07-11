using GorillaLocomotion;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.IO;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Safety
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

        public static void SetGamemodeButtonActive(bool active = true) =>
            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/ModeSelector_Group").SetActive(active);

        public static void FakeOculusMenu()
        {
            if (leftPrimary)
            {
                NoFinger();
                GTPlayer.Instance.inOverlay = true;
                GTPlayer.Instance.leftControllerTransform.localPosition = new Vector3(238f, -90f, 0f);
                GTPlayer.Instance.rightControllerTransform.localPosition = new Vector3(-190f, 90f, 0f);
                GTPlayer.Instance.leftControllerTransform.rotation = Camera.main.transform.rotation * Quaternion.Euler(-55f, 90f, 0f);
                GTPlayer.Instance.rightControllerTransform.rotation = Camera.main.transform.rotation * Quaternion.Euler(-55f, -49f, 0f);
            }

        }

        public static void FakeReportMenu()
        {
            if (leftSecondary)
                NoFinger();

            GTPlayer.Instance.inOverlay = leftPrimary;
        }

        public static void FakeBrokenController()
        {
            Vector3 Position = leftPrimary ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position;
            Quaternion Rotation = leftPrimary ? GorillaTagger.Instance.leftHandTransform.rotation : GorillaTagger.Instance.rightHandTransform.rotation;

            GTPlayer.Instance.leftControllerTransform.localPosition = new Vector3(238f, -90f, 0f);
            GTPlayer.Instance.leftControllerTransform.rotation = Camera.main.transform.rotation * Quaternion.Euler(-55f, 90f, 0f);

            GTPlayer.Instance.rightControllerTransform.localPosition = Position;
            GTPlayer.Instance.rightControllerTransform.rotation = Rotation;

            ControllerInputPoller.instance.leftControllerGripFloat = 0f;
            ControllerInputPoller.instance.leftControllerIndexFloat = 0f;
            ControllerInputPoller.instance.leftControllerPrimaryButton = false;
            ControllerInputPoller.instance.leftControllerSecondaryButton = false;
            ControllerInputPoller.instance.leftControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.leftControllerSecondaryButtonTouch = false;
        }

        public static Vector3 deadPosition = Vector3.zero;
        public static Vector3 lvel = Vector3.zero;
        public static void FakePowerOff()
        {
            if (leftJoystickClick)
            {
                if (deadPosition == Vector3.zero)
                {
                    deadPosition = GorillaTagger.Instance.rigidbody.transform.position;
                    lvel = GorillaTagger.Instance.rigidbody.velocity;
                }
                RigManager.LocalRig.enabled = false;
                GorillaTagger.Instance.rigidbody.transform.position = deadPosition;
                GorillaTagger.Instance.rigidbody.velocity = lvel;
            }
            else
            {
                deadPosition = Vector3.zero;
                RigManager.LocalRig.enabled = true;
            }
        }

        public static void FakeValveTracking()
        {
            if (rightJoystickClick)
                RigManager.LocalRig.head.rigTarget.transform.rotation = Quaternion.identity;
        }

        public static void SpoofSupportPage() =>
            GorillaComputer.instance.screenText.Text = GorillaComputer.instance.screenText.Text.Replace("STEAM", "QUEST").Replace(GorillaComputer.instance.buildDate, "05/30/2024 16:50:12\nBUILD CODE 4893\nMANAGED ACCOUNT: NO");

        private static float lastCacheClearedTime = 0f;
        public static void AutoClearCache()
        {
            if (Time.time > lastCacheClearedTime)
            {
                lastCacheClearedTime = Time.time + 60f;
                System.GC.Collect();
            }
        }

        public static int antireportrangeindex;
        public static float threshold = 0.35f;

        public static void ChangeAntiReportRange(bool positive = true)
        {
            string[] names = new string[]
            {
                "Default", // The report button
                "Large", // The report button within the range of 3 people
                "Massive" // The entire fucking board
            };
            float[] distances = new float[]
            {
                0.35f,
                0.7f,
                1.5f
            };

            if (positive)
                antireportrangeindex++;
            else
                antireportrangeindex--;

            antireportrangeindex %= names.Length;
            if (antireportrangeindex < 0)
                antireportrangeindex = names.Length - 1;

            threshold = distances[antireportrangeindex];
            GetIndex("Change Anti Report Distance").overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>" + names[antireportrangeindex] + "</color><color=grey>]</color>";
        }

        public static bool smartarp;
        public static int buttonClickTime;
        public static string buttonClickPlayer;

        public static float antiReportNotificationDelay;
        public static void AntiReportDisconnect()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, threshold, Color.red);
                        
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isLocal)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        NetworkSystem.Instance.ReturnToSinglePlayer();
                                        RPCProtection();

                                        if (Time.time > antiReportNotificationDelay)
                                        {
                                            antiReportNotificationDelay = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected.");
                                        }
                                    }
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
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, threshold, Color.red);
                        
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isLocal)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        Important.Reconnect();
                                        RPCProtection();

                                        if (Time.time > antiReportNotificationDelay)
                                        {
                                            antiReportNotificationDelay = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
                                        }
                                    }
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
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, threshold, Color.red);
                        
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.isLocal)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        RPCProtection();
                                        Important.JoinRandom();

                                        if (Time.time > antiReportNotificationDelay)
                                        {
                                            antiReportNotificationDelay = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be connected to a random lobby shortly.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        private static float delaysonospam = 0f;
        public static void AntiReportNotify()
        {
            try
            {
                if (Time.time > delaysonospam)
                {
                    foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                    {
                        if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                        {
                            Transform report = line.reportButton.gameObject.transform;
                            if (GetIndex("Visualize Anti Report").enabled)
                                VisualizeAura(report.position, threshold, Color.red);
                            
                            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                            {
                                if (!vrrig.isLocal)
                                {
                                    float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                    float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                    if (D1 < threshold || D2 < threshold)
                                    {
                                        if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                        {
                                            delaysonospam = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " is reporting you.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportFRT(Player subject, bool doNotification = true)
        {
            if (!smartarp || (smartarp && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
            {
                int antiReportType = 0;
                string[] types = new string[]
                {
                    "Disconnect",
                    "Reconnect",
                    "Join Random"
                };
                for (int i = 0; i < types.Length - 1; i++)
                {
                    ButtonInfo buttonInfo = GetIndex("Anti Report <color=grey>[</color><color=green>" + types[i] + "</color><color=grey>]</color>");
                    if (buttonInfo.enabled)
                    {
                        antiReportType = i;
                        break;
                    }
                }
                switch (antiReportType)
                {
                    case 0:
                        NetworkSystem.Instance.ReturnToSinglePlayer();
                        RPCProtection();
                        if (doNotification)
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + subject.NickName + " attempted to report you, you have been disconnected.");
                        
                        break;
                    case 1:
                        Important.Reconnect();
                        RPCProtection();
                        if (doNotification)
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + subject.NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
                        
                        break;
                    case 2:
                        RPCProtection();
                        Important.JoinRandom();
                        if (doNotification)
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + subject.NickName + " attempted to report you, you have been disconnected and will be connected to a random lobby shortly.");
                        
                        break;
                }
            }
        }

        public static void AntiModerator()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig && vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK") || vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAD") || vrrig.concatStringOfCosmeticsAllowed.Contains("LMAPY."))
                {
                    try
                    {

                        VRRig plr = vrrig;
                        NetPlayer player = GetPlayerFromVRRig(plr);
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
                            catch { LogManager.Log("Failed to log colors, rig most likely nonexistent"); }

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
                            catch { LogManager.Log("Failed to log player"); }

                            text += "\n====================================\n";
                            text += "Text file generated with ii's Stupid Menu";
                            string fileName = $"{PluginInfo.BaseDirectory}/" + player.NickName + " - Anti Moderator.txt";

                            File.WriteAllText(fileName, text);
                        }
                    }
                    catch { }
                    NetworkSystem.Instance.ReturnToSinglePlayer();
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-MODERATOR</color><color=grey>]</color> There was a moderator in your lobby, you have been disconnected. Their Player ID and Room Code have been saved to a file.");
                }
            }
        }

        private static float lastVol;
        private static float startSilenceTime = -1f;
        private static bool reloaded;

        public static void BypassAutomod()
        {
            GorillaTagger.moderationMutedTime = -1f;

            if (GorillaComputer.instance.autoMuteType != "OFF")
            {
                GorillaComputer.instance.autoMuteType = "OFF";
                PlayerPrefs.SetInt("autoMute", 0);
                PlayerPrefs.Save();
            }

            Recorder mic = GorillaTagger.Instance.myRecorder;
            if (mic == null)
                return;

            float volume = 0f;
            GorillaSpeakerLoudness recorder = RigManager.LocalRig.GetComponent<GorillaSpeakerLoudness>();
            if (recorder != null)
                volume = recorder.Loudness;

            if (volume == 0f)
            {
                if (lastVol != 0f)
                {
                    startSilenceTime = Time.time;
                    reloaded = false;
                }

                if (startSilenceTime > 0f && !reloaded && Time.time - startSilenceTime >= 0.5f)
                {
                    mic.RestartRecording(true);
                    reloaded = true;
                }
            }
            else
            {
                startSilenceTime = -1f;
                reloaded = false;
            }

            lastVol = volume;
        }


        public static void ChangeIdentity()
        {
            string randomName = "gorilla";
            for (var i = 0; i < 4; i++)
                randomName += Random.Range(0, 9).ToString();

            ChangeName(randomName);

            byte randA = (byte)Random.Range(0, 255);
            byte randB = (byte)Random.Range(0, 255);
            byte randC = (byte)Random.Range(0, 255);
            ChangeColor(new Color32(randA, randB, randC, 255));
        }

        public static void ChangeIdentityRegular()
        {
            string prefix = Random.Range(0, 3) == 0 ? namePrefix[Random.Range(0, namePrefix.Length - 1)] : "";
            string suffix = Random.Range(0, 3) == 0 ? nameSuffix[Random.Range(0, nameSuffix.Length - 1)] : "";
            string fName = prefix + names[Random.Range(0, names.Length - 1)] + suffix;
            ChangeName( fName.Length > 12 ? fName[..12] : fName );

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
                new Color(0f, 0.5f, 0f, 255f),
                new Color32(113, 0, 198, 255),
                new Color32(170, 198, 170, 255),
                new Color32(170, 170, 170, 255),
                new Color32(227, 170, 85, 255),
                new Color32(0, 226, 255, 255)
            };
            ChangeColor(colors[Random.Range(0, colors.Length - 1)]);
        }

        private static bool lastinlobbyagain = false;
        public static void ChangeIdentityOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
                ChangeIdentity();
            
            lastinlobbyagain = PhotonNetwork.InRoom;
        }
        public static void ChangeIdentityRegularOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
                ChangeIdentityRegular();
            
            lastinlobbyagain = PhotonNetwork.InRoom;
        }
        public static void ChangeIdentityMinigamesOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
                Fun.BecomeMinigamesKid();
            
            lastinlobbyagain = PhotonNetwork.InRoom;
        }

        public static void FPSSpoof()
        {
            Patches.FPSPatch.enabled = true;
            Patches.FPSPatch.spoofFPSValue = Random.Range(80, 95);
        }

        public static string[] namePrefix = new string[]
        {
            "EPIC", "EPIK", "REAL", "NOT", "SILLY", "LITTLE", "BIG", "MAYBE", "MONKE", "SUB2", "OG"
        };
        public static string[] nameSuffix = new string[]
        {
            "GT", "VR", "LOL", "GTVR", "FAN", "XD", "LOL", "MONK", "YT"
        };
        public static string[] names = new string[]
        {
            "0", "SHIBA", "PBBV", "J3VU", "BEES", "NAMO", "MANGO", "FROSTY", "FRISH", "LEMMING", 
            "BILLY", "TIMMY", "MINIGAMES", "JMANCURLY", "VMT", "ELLIOT", "POLAR", "3CLIPCE", "DAISY09",
            "SHARKPUPPET", "DUCKY", "EDDIE", "EDDY", "RAKZZ", "CASEOH", "SKETCH", "SKY", "RETURN",
            "WATERMELON", "CRAZY", "MONK", "MONKE", "MONKI", "MONKEY", "MONKIY", "GORILL", "GOORILA", "GORILLA",
            "REDBERRY", "FOX", "RUFUS", "TTT", "TTTPIG", "PPPTIG", "K9", "BTC", "TICKLETIPJR", "BANANA",
            "PEANUTBUTTER", "GHOSTMONKE", "STATUE", "TURBOALLEN", "NOVA", "LUNAR", "MOON", "SUN", "RANDOM", "UNKNOWN",
            "GLITCH", "BUG", "ERROR", "CODE", "HACKER", "MODDER", "INVIS", "INVISIBLE", "TAGGER", "UNTAGGED",
            "BLUE", "RED", "GREEN", "PURPLE", "YELLOW", "BLACK", "WHITE", "BROWN", "CYAN", "GRAY",
            "GREY", "BANNED", "LEMON", "PLUSHIE", "CHEETO", "TIKTOK", "YOUTUBE", "TWITCH", "DISCORD"
        };

        public static string targetRank = "High";
        public static int rankIndex = 2;

        public static void ChangeRankedTier(bool positive = true)
        {
            if (positive)
                rankIndex++;
            else
                rankIndex--;

            rankIndex %= 3;
            if (rankIndex < 0)
                rankIndex = 2;

            targetRank = ((RankedProgressionManager.ERankedMatchmakingTier)rankIndex).ToString();
            GetIndex("Change Ranked Tier").overlapText = "Change Ranked Tier <color=grey>[</color><color=green>" + targetRank + "</color><color=grey>]</color>";
        }

        public static void SpoofRank(bool enabled, string tier = null)
        {
            Patches.RankedPatch.enabled = enabled;
            Patches.RankedPatch.targetTier = tier;
        }

        public static void SpoofPlatform(bool enabled, string target = null)
        {
            Patches.RankedPatch.enabled = enabled;
            Patches.RankedPatch.targetPlatform = target;
        }
    }
}
