using iiMenu.Notifications;
using Photon.Pun;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.Reconnect;

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
        }

        public static void AntiCrashEnabled()
        {
            GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools").SetActive(false);
        }

        public static void AntiCrashDisabled()
        {
            GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools").SetActive(true);
        }

        public static void AntiReportDisconnect()
        {
            try
            {
                GameObject boards = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GorillaUI");
                Transform boardsTransform = boards.transform;
                for (int i = 0; i < boardsTransform.childCount; i++)
                {
                    Transform v = boardsTransform.GetChild(i);
                    if (v.gameObject.name.Contains("Anchor") && v.gameObject.activeSelf)
                    {
                        string Name = v.gameObject.name;
                        v = v.Find("GorillaScoreBoard/LineParent");
                        for (int i2 = 0; i2 < v.childCount; i2++)
                        {
                            Transform v2 = v.GetChild(i2);
                            if (v2.name.Contains("GorillaPlayerScoreboardLine"))
                            {
                                Text name = v2.Find("Player Name").GetComponent<Text>();
                                Transform report = v2.Find("ReportButton");
                                if (!report.gameObject.activeSelf)
                                {
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
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportReconnect()
        {
            try
            {
                GameObject boards = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GorillaUI");
                Transform boardsTransform = boards.transform;
                for (int i = 0; i < boardsTransform.childCount; i++)
                {
                    Transform v = boardsTransform.GetChild(i);
                    if (v.gameObject.name.Contains("Anchor") && v.gameObject.activeSelf)
                    {
                        string Name = v.gameObject.name;
                        v = v.Find("GorillaScoreBoard/LineParent");
                        for (int i2 = 0; i2 < v.childCount; i2++)
                        {
                            Transform v2 = v.GetChild(i2);
                            if (v2.name.Contains("GorillaPlayerScoreboardLine"))
                            {
                                Text name = v2.Find("Player Name").GetComponent<Text>();
                                Transform report = v2.Find("ReportButton");
                                if (!report.gameObject.activeSelf)
                                {
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
                                                rejDebounce = Time.time + 2f;
                                                PhotonNetwork.Disconnect();
                                                RPCProtection();
                                                NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>Someone attempted to report you, you have been disconnected and will be reconnected shortly.</color>");
                                            }
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
                GameObject boards = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GorillaUI");
                Transform boardsTransform = boards.transform;
                for (int i = 0; i < boardsTransform.childCount; i++)
                {
                    Transform v = boardsTransform.GetChild(i);
                    if (v.gameObject.name.Contains("Anchor") && v.gameObject.activeSelf)
                    {
                        string Name = v.gameObject.name;
                        v = v.Find("GorillaScoreBoard/LineParent");
                        for (int i2 = 0; i2 < v.childCount; i2++)
                        {
                            Transform v2 = v.GetChild(i2);
                            if (v2.name.Contains("GorillaPlayerScoreboardLine"))
                            {
                                Text name = v2.Find("Player Name").GetComponent<Text>();
                                Transform report = v2.Find("ReportButton");
                                if (!report.gameObject.activeSelf)
                                {
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
                                                jrDebounce = Time.time + internetFloat;
                                                NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>Someone attempted to report you, you have been disconnected and will be connected to a random lobby shortly.</color>");
                                            }
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

        public static void AntiModerator()
        {
            foreach(VRRig vrrig in GorillaParent.instance.vrrigs)
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
        }
    }
}
