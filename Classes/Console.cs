
using ExitGames.Client.Photon;
using HarmonyLib;
using iiMenu.Mods.Spammers;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using iiMenu.Notifications;
using static iiMenu.Menu.Main;
using static iiMenu.Classes.RigManager;

namespace iiMenu.Classes
{
    public class Console : MonoBehaviour
    {
        // Configuration
        public static string MenuName = "stupid";
        public static string MenuVersion = PluginInfo.Version;

        // Console code
        public static Console instance;

        public void Start()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
        }

        public void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;
        }

        public static float indicatorDelay = 0f;

        private static Dictionary<string, Color> menuColors = new Dictionary<string, Color> {
            { "stupid", new Color32(255, 128, 0, 255) },
            { "symex", new Color32(138, 43, 226, 255) },
            { "colossal", new Color32(204, 0, 255, 255) },
            { "ccm", new Color32(204, 0, 255, 255) },
            { "untitled", new Color32(45, 115, 175, 255) },
            { "genesis", Color.blue },
            { "steal", Color.gray }
        };

        public static Color GetMenuTypeName(string type)
        {
            if (menuColors.ContainsKey(type))
                return menuColors[type];

            return Color.red;
        }

        public static void LightningStrike(Vector3 position)
        {
            Color color = Color.cyan;

            GameObject line = new GameObject("LightningOuter");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = color; liner.endColor = color; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 5; liner.useWorldSpace = true;
            Vector3 victim = position;
            for (int i = 0; i < 5; i++)
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, false, 5f);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, true, 5f);
                liner.SetPosition(i, victim);
                victim += new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
            }
            liner.material.shader = Shader.Find("GUI/Text Shader");
            Destroy(line, 2f);

            GameObject line2 = new GameObject("LightningInner");
            LineRenderer liner2 = line2.AddComponent<LineRenderer>();
            liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.15f; liner2.endWidth = 0.15f; liner2.positionCount = 5; liner2.useWorldSpace = true;
            for (int i = 0; i < 5; i++)
                liner2.SetPosition(i, liner.GetPosition(i));
            
            liner2.material.shader = Shader.Find("GUI/Text Shader");
            liner2.material.renderQueue = liner.material.renderQueue + 1;
            Destroy(line2, 2f);
        }

        public static Coroutine laserCoroutine = null;
        public static IEnumerator RenderLaser(bool rightHand, VRRig rigTarget)
        {
            float stoplasar = Time.time + 0.2f;
            while (Time.time < stoplasar)
            {
                rigTarget.PlayHandTapLocal(18, !rightHand, 99999f);
                GameObject line = new GameObject("LaserOuter");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.startColor = Color.red; liner.endColor = Color.red; liner.startWidth = 0.15f + (Mathf.Sin(Time.time * 5f) * 0.01f); liner.endWidth = liner.startWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                Vector3 startPos = (rightHand ? rigTarget.rightHandTransform.position : rigTarget.leftHandTransform.position) + ((rightHand ? rigTarget.rightHandTransform.up : rigTarget.leftHandTransform.up) * 0.1f);
                Vector3 endPos = Vector3.zero;
                Vector3 dir = rightHand ? rigTarget.rightHandTransform.right : -rigTarget.leftHandTransform.right;
                try
                {
                    Physics.Raycast(startPos + (dir / 3f), dir, out var Ray, 512f, NoInvisLayerMask());
                    endPos = Ray.point;
                }
                catch { }
                liner.SetPosition(0, startPos + (dir * 0.1f));
                liner.SetPosition(1, endPos);
                liner.material.shader = Shader.Find("GUI/Text Shader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                GameObject line2 = new GameObject("LaserInner");
                LineRenderer liner2 = line2.AddComponent<LineRenderer>();
                liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.1f; liner2.endWidth = 0.1f; liner2.positionCount = 2; liner2.useWorldSpace = true;
                liner2.SetPosition(0, startPos + (dir * 0.1f));
                liner2.SetPosition(1, endPos);
                liner2.material.shader = Shader.Find("GUI/Text Shader");
                liner2.material.renderQueue = liner.material.renderQueue + 1;
                UnityEngine.Object.Destroy(line2, Time.deltaTime);

                GameObject whiteParticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(whiteParticle, 2f);
                UnityEngine.Object.Destroy(whiteParticle.GetComponent<Collider>());
                whiteParticle.GetComponent<Renderer>().material.color = Color.yellow;
                whiteParticle.AddComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-7.5f, 7.5f), UnityEngine.Random.Range(0f, 7.5f), UnityEngine.Random.Range(-7.5f, 7.5f));
                whiteParticle.transform.position = endPos + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
                whiteParticle.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                yield return null;
            }
        }

        private static Dictionary<VRRig, float> confirmUsingDelay = new Dictionary<VRRig, float> { };
        public static void EventReceived(EventData data)
        {
            try
            {
                if (data.Code == 68) // Admin mods, before you try anything yes it's player ID locked
                {
                    Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);

                    object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                    string command = args.Length > 0 ? (string)args[0] : "";

                    if (admins.ContainsKey(sender.UserId))
                    {
                        NetPlayer Target = null;

                        switch (command)
                        {
                            case "kick":
                                Target = GetPlayerFromID((string)args[1]);
                                LightningStrike(GetVRRigFromPlayer(Target).headMesh.transform.position);
                                if (!admins.ContainsKey(Target.UserId) || admins[sender.UserId] == "goldentrophy")
                                {
                                    if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                    {
                                        PhotonNetwork.Disconnect();
                                    }
                                }
                                break;
                            case "silkick":
                                Target = GetPlayerFromID((string)args[1]);
                                if (!admins.ContainsKey(Target.UserId) || admins[sender.UserId] == "goldentrophy")
                                {
                                    if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                    {
                                        PhotonNetwork.Disconnect();
                                    }
                                }
                                break;
                            case "join":
                                if (!admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || admins[sender.UserId] == "goldentrophy")
                                {
                                    rejRoom = (string)args[1];
                                    PhotonNetwork.Disconnect();
                                }
                                break;
                            case "kickall":
                                foreach (Photon.Realtime.Player plr in admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? PhotonNetwork.PlayerListOthers : PhotonNetwork.PlayerList)
                                    LightningStrike(GetVRRigFromPlayer(plr).headMesh.transform.position);
                                
                                if (!admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                                    PhotonNetwork.Disconnect();
                                break;
                            case "isusing":
                                PhotonNetwork.RaiseEvent(68, new object[] { "confirmusing", MenuVersion, MenuName }, new RaiseEventOptions { TargetActors = new int[] { sender.ActorNumber } }, SendOptions.SendReliable);
                                break;
                            case "forceenable":
                                string mod = (string)args[1];
                                bool shouldbeenabled = (bool)args[2];
                                ButtonInfo modd = GetIndex(mod);
                                if (!modd.isTogglable)
                                    modd.method.Invoke();
                                else
                                {
                                    modd.enabled = !shouldbeenabled;
                                    Toggle(modd.buttonText);
                                }
                                break;
                            case "toggle":
                                string moddd = (string)args[1];
                                ButtonInfo modddd = GetIndex(moddd);
                                Toggle(modddd.buttonText);
                                break;
                            case "tp":
                                TeleportPlayer((Vector3)args[1]);
                                break;
                            case "nocone":
                                adminConeExclusion = (bool)args[1] ? sender : null;
                                break;
                            case "vel":
                                GorillaTagger.Instance.rigidbody.velocity = (Vector3)args[1];
                                break;
                            case "tpnv":
                                TeleportPlayer((Vector3)args[1]);
                                GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
                                break;
                            case "scale":
                                VRRig player = GetVRRigFromPlayer(sender);
                                adminIsScaling = (float)args[1] == 1f ? false : true;
                                adminRigTarget = player;
                                adminScale = (float)args[1];
                                break;
                            case "cosmetic":
                                GetVRRigFromPlayer(sender).concatStringOfCosmeticsAllowed += (string)args[1];
                                break;
                            case "strike":
                                LightningStrike((Vector3)args[1]);
                                break;
                            case "laser":
                                if (laserCoroutine != null)
                                    CoroutineManager.EndCoroutine(laserCoroutine);

                                if ((bool)args[1])
                                    laserCoroutine = CoroutineManager.RunCoroutine(RenderLaser((bool)args[2], GetVRRigFromPlayer(sender)));

                                break;
                            case "lr":
                                // 1, 2, 3, 4 : r, g, b, a
                                // 5 : width
                                // 6, 7 : start pos, end pos
                                // 8 : time
                                GameObject lines = new GameObject("Line");
                                LineRenderer liner = lines.AddComponent<LineRenderer>();
                                UnityEngine.Color thecolor = new Color((float)args[1], (float)args[2], (float)args[3], (float)args[4]);
                                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = (float)args[5]; liner.endWidth = (float)args[5]; liner.positionCount = 2; liner.useWorldSpace = true;
                                liner.SetPosition(0, (Vector3)args[6]);
                                liner.SetPosition(1, (Vector3)args[7]);
                                liner.material.shader = Shader.Find("GUI/Text Shader");
                                UnityEngine.Object.Destroy(lines, (float)args[8]);
                                break;
                            case "soundcs":
                                string fileName = (string)args[1];
                                if (fileName.Contains(".."))
                                    fileName = fileName.Replace("..", "");

                                Play2DAudio(LoadSoundFromURL((string)args[2], "Sounds/" + fileName + "." + GetFileExtension((string)args[2])), 1f);
                                break;
                            case "soundboard":
                                if (!File.Exists("iisStupidMenu/Sounds/" + (string)args[1]))
                                    Sound.DownloadSound((string)args[1], (string)args[2]);

                                Sound.PlayAudio("Sounds/" + (string)args[1] + "." + GetFileExtension((string)args[2]));
                                break;
                            case "notify":
                                NotifiLib.SendNotification("<color=grey>[</color><color=red>ANNOUNCE</color><color=grey>]</color> " + (string)args[1], 5000);
                                break;
                            case "platf":
                                // 1 : position
                                // 2 : scale
                                // 3 : rotation
                                // 4 , 5, 6, 7: color
                                // 8 : time
                                GameObject lol = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                UnityEngine.Object.Destroy(lol, args.Length > 8 ? (float)args[8] : 60f);
                                lol.GetComponent<Renderer>().material.color = args.Length > 4 ? new Color((float)args[4], (float)args[5], (float)args[6], (float)args[7]) : Color.black;
                                lol.transform.position = (Vector3)args[1];
                                lol.transform.rotation = args.Length > 3 ? Quaternion.Euler((Vector3)args[3]) : Quaternion.identity;
                                lol.transform.localScale = args.Length > 2 ? (Vector3)args[2] : new Vector3(1f, 0.1f, 1f);
                                break;
                            case "muteall":
                                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                                {
                                    if (!line.playerVRRig.muted && admins.ContainsKey(line.linePlayer.UserId))
                                    {
                                        line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);
                                    }
                                }
                                break;
                            case "unmuteall":
                                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                                {
                                    if (line.playerVRRig.muted)
                                    {
                                        line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);
                                    }
                                }
                                break;
                        }
                    }
                    switch (command)
                    {
                        case "confirmusing":
                            if (admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            {
                                if (indicatorDelay > Time.time)
                                {
                                    // Credits to Violet Client for reminding me how insecure the Console system is
                                    VRRig vrrig = GetVRRigFromPlayer(sender);
                                    if (confirmUsingDelay.TryGetValue(vrrig, out float delay))
                                    {
                                        if (Time.time < delay)
                                            return;

                                        confirmUsingDelay.Remove(vrrig);
                                    }

                                    confirmUsingDelay.Add(vrrig, Time.time + 5f);

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = GetMenuTypeName((string)args[2]);

                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> " + sender.NickName + " is using version " + (string)args[1] + ".", 3000);
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(29, false, 99999f);
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(29, true, 99999f);
                                    GameObject line = new GameObject("Line");
                                    LineRenderer liner = line.AddComponent<LineRenderer>();
                                    liner.startColor = userColor; liner.endColor = userColor; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 2; liner.useWorldSpace = true;

                                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                                    liner.material.shader = Shader.Find("GUI/Text Shader");
                                    UnityEngine.Object.Destroy(line, 3f);
                                }
                            }
                            break;
                    }
                }
            }
            catch { }
        }
    }
}
