using ExitGames.Client.Photon;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;
using static iiMenu.Classes.RigManager;
using System.IO;
using HarmonyLib;
using iiMenu.Menu;
using Fusion;

namespace iiMenu.Mods
{
    public class Experimental
    {
        private static Dictionary<string, Color> menuColors = new Dictionary<string, Color> { { "stupid", new Color32(255, 128, 0, 255) }, { "genesis", Color.blue }, { "steal", Color.gray }, { "symex", new Color32(138, 43, 226, 255) }, { "colossal", new Color32(204, 0, 255, 255) }, { "ccm", new Color32(204, 0, 255, 255) } };
        private static Color GetMenuTypeName(string type)
        {
            if (menuColors.ContainsKey(type))
                return menuColors[type];

            return Color.red;
        }

        public static void Console(EventData data)
        {
            try
            {
                if (data.Code == 68) // Admin mods, before you try anything yes it's player ID locked
                {
                    object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                    string command = args.Length > 0 ? (string)args[0] : "";
                    if (admins.ContainsKey(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).UserId))
                    {
                        switch (command)
                        {
                            case "kick":
                                NetPlayer victimm = GetPlayerFromID((string)args[1]);
                                Visuals.LightningStrike(GetVRRigFromPlayer(victimm).headMesh.transform.position);
                                if (!admins.ContainsKey(victimm.UserId) || admins[PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).UserId] == "goldentrophy")
                                {
                                    if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                    {
                                        PhotonNetwork.Disconnect();
                                    }
                                }
                                break;
                            case "silkick":
                                NetPlayer victimmm = GetPlayerFromID((string)args[1]);
                                if (!admins.ContainsKey(victimmm.UserId) || admins[PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).UserId] == "goldentrophy")
                                {
                                    if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                    {
                                        PhotonNetwork.Disconnect();
                                    }
                                }
                                break;
                            case "join":
                                if (!admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || admins[PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).UserId] == "goldentrophy")
                                {
                                    rejRoom = (string)args[1];
                                    PhotonNetwork.Disconnect();
                                }
                                break;
                            case "kickall":
                                foreach (Photon.Realtime.Player plr in admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? PhotonNetwork.PlayerListOthers : PhotonNetwork.PlayerList)
                                {
                                    Visuals.LightningStrike(GetVRRigFromPlayer(plr).headMesh.transform.position);
                                }
                                if (!admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                                {
                                    PhotonNetwork.Disconnect();
                                }
                                break;
                            case "isusing":
                                PhotonNetwork.RaiseEvent(68, new object[] { "confirmusing", PluginInfo.Version, "stupid" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                                break;
                            case "forceenable":
                                string mod = (string)args[1];
                                bool shouldbeenabled = (bool)args[2];
                                ButtonInfo modd = GetIndex(mod);
                                if (!modd.isTogglable)
                                {
                                    modd.method.Invoke();
                                }
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
                                adminConeExclusion = (bool)args[1] ? PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false) : null;
                                break;
                            case "vel":
                                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = (Vector3)args[1];
                                break;
                            case "tpnv":
                                TeleportPlayer((Vector3)args[1]);
                                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                break;
                            case "scale":
                                VRRig player = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
                                adminIsScaling = (float)args[1] == 1f ? false : true;
                                adminRigTarget = player;
                                adminScale = (float)args[1];
                                break;
                            case "cosmetic":
                                GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false)).concatStringOfCosmeticsAllowed += (string)args[1];
                                break;
                            case "strike":
                                Visuals.LightningStrike((Vector3)args[1]);
                                break;
                            case "laser":
                                if (laserCoroutine != null)
                                    CoroutineManager.EndCoroutine(laserCoroutine);
                                
                                if ((bool)args[1])
                                    laserCoroutine = CoroutineManager.RunCoroutine(Visuals.RenderLaser((bool)args[2], GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false))));
                                
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
                                if (Miscellaneous.indicatorDelay > Time.time)
                                {
                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = GetMenuTypeName((string)args[2]);

                                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> " + PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).NickName + " is using version " + (string)args[1] + ".", 3000);
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(29, false, 99999f);
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(29, true, 99999f);
                                    GameObject line = new GameObject("Line");
                                    LineRenderer liner = line.AddComponent<LineRenderer>();
                                    liner.startColor = userColor; liner.endColor = userColor; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 2; liner.useWorldSpace = true;
                                    VRRig vrrig = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
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

        public static void Hyperflush()
        {
            Traverse.Create(typeof(PhotonNetwork)).Field("serializeStreamOut").Method("ResetWriteStream").GetValue();
        }

        public static void FixDuplicateButtons()
        {
            int duplicateButtons = 0;
            List<string> previousNames = new List<string> { };
            foreach (ButtonInfo[] buttonn in Buttons.buttons)
            {
                foreach (ButtonInfo button in buttonn)
                {
                    if (previousNames.Contains(button.buttonText))
                    {
                        string buttonText = button.overlapText == null ? button.buttonText : button.overlapText;
                        button.overlapText = buttonText;
                        button.buttonText += "X";
                        duplicateButtons++;
                    }
                    previousNames.Add(button.buttonText);
                }
            }
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully fixed " + duplicateButtons.ToString() + " broken buttons.");
        }

        public static void AntiRPCBan()
        {
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);

            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);

            try
            {
                GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
                //GorillaNot.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
                //GorillaNot.instance.OnLeftRoom();
                //GorillaNot.instance.OnPreLeavingRoom();
                if (GorillaNot.instance != null)
                {
                    FieldInfo report = typeof(GorillaNot).GetField("sendReport", BindingFlags.NonPublic);
                    if (report != null)
                    {
                        report.SetValue(GorillaNot.instance, false);
                    }
                    report = typeof(GorillaNot).GetField("_sendReport", BindingFlags.NonPublic);
                    if (report != null)
                    {
                        report.SetValue(GorillaNot.instance, false);
                    }
                }
            }
            catch { }
            RPCProtection();
            //GorillaNot.instance.OnLeftRoom();
        }

        private static Dictionary<Renderer, Material> oldMats = new Dictionary<Renderer, Material> { };
        public static void BetterFPSBoost()
        {
            foreach (Renderer v in Resources.FindObjectsOfTypeAll<Renderer>())
            {
                try
                {
                    if (v.material.shader.name == "GorillaTag/UberShader")
                    {
                        oldMats.Add(v, v.material);
                        Material replacement = new Material(Shader.Find("GorillaTag/UberShader"));
                        replacement.color = v.material.color;
                        v.material = replacement;
                    }
                } catch (System.Exception exception) { UnityEngine.Debug.LogError(string.Format("mat error {1} - {0}", exception.Message, exception.StackTrace)); }
            }
        }
        public static void DisableBetterFPSBoost()
        {
            foreach (KeyValuePair<Renderer, Material> v in oldMats)
            {
                v.Key.material = v.Value;
            }
        }

        private static float stupiddelayihate = 0f;
        public static void AdminKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "kick", RigManager.GetPlayerFromVRRig(possibly).UserId }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void AdminKickAll()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "kickall" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void FlipMenuGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "toggle", "Right Hand" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void AdminTeleportGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "tp", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                }
            }
        }

        public static void AdminFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", new Vector3(0f, 50f, 0f) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        private static float stdell = 0f;
        private static VRRig thestrangled = null;
        private static VRRig thestrangledleft = null;
        public static void AdminStrangle()
        {
            if (leftGrab)
            {
                if (thestrangledleft == null)
                {
                    foreach (VRRig lol in GorillaParent.instance.vrrigs)
                    {
                        if (lol != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (Vector3.Distance(lol.headMesh.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 0.2f)
                            {
                                thestrangledleft = lol;
                                if (PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                        89,
                                        true,
                                        999999f
                                    });
                                }
                                else
                                {
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, true, 999999f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Time.time > stdell)
                    {
                        stdell = Time.time + 0.05f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.leftHandTransform.position }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
            else
            {
                if (thestrangledleft != null)
                {
                    try {
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber } }, SendOptions.SendReliable);
                    } catch { }
                thestrangledleft = null;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            true,
                            999999f
                        });
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, true, 999999f);
                    }
                }
            }

            if (rightGrab)
            {
                if (thestrangled == null)
                {
                    foreach (VRRig lol in GorillaParent.instance.vrrigs)
                    {
                        if (lol != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (Vector3.Distance(lol.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 0.2f)
                            {
                                thestrangled = lol;
                                if (PhotonNetwork.InRoom)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                                        89,
                                        false,
                                        999999f
                                    });
                                }
                                else
                                {
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, false, 999999f);
                                }
                            }
                        }
                    }
                } else
                {
                    if (Time.time > stupiddelayihate)
                    {
                        stupiddelayihate = Time.time + 0.05f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.rightHandTransform.position }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
            else
            {
                if (thestrangled != null)
                {
                    try
                    {
                        PhotonNetwork.RaiseEvent(68, new object[] { "vel", GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber } }, SendOptions.SendReliable);
                    } catch { }
                    thestrangled = null;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            false,
                            999999f
                        });
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, false, 999999f);
                    }
                }
            }
        }

        public static void AdminObjectGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "platf", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
        }

        private static float lastnetscale = 1f;
        private static float scalenetdel = 0f;
        private static int lastplayercount = 0;
        public static void AdminNetworkScale()
        {
            if (Time.time > scalenetdel && (lastnetscale != GorillaTagger.Instance.offlineVRRig.scaleFactor || PhotonNetwork.PlayerList.Length != lastplayercount))
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "scale", GorillaTagger.Instance.offlineVRRig.scaleFactor }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                scalenetdel = Time.time + 0.1f;
                lastnetscale = GorillaTagger.Instance.offlineVRRig.scaleFactor;
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }
        public static void UnAdminNetworkScale()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "scale", 1f }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void LightningGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "strike", NewPointer.transform.position }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
        }

        public static void LightningAura()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "strike", GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 1f, MathF.Sin((float)Time.frameCount / 30)) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void LightningRain()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.1f;
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f), 10f, UnityEngine.Random.Range(-10f, 10f)), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
                VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "kick", RigManager.GetPlayerFromVRRig(possibly).UserId }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                } else
                {
                    PhotonNetwork.RaiseEvent(68, new object[] { "strike", Ray.point }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
        }

        private static Vector3 whereOriginalPlayerPos = Vector3.zero;
        private static Vector3 originalMePosition = Vector3.zero;
        public static void AdminFearGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    TeleportPlayer(whoCopy.transform.position + whoCopy.transform.forward);
                    if (Time.time > stupiddelayihate)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "muteall" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", new Vector3(0f, 21f, 0f) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        originalMePosition = GorillaTagger.Instance.bodyCollider.transform.position;
                        whereOriginalPlayerPos = possibly.transform.position;

                        int anum = RigManager.GetPlayerFromVRRig(possibly).ActorNumber;
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 16f, 0f), new Vector3(10f, 1f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 24f, 0f), new Vector3(10f, 1f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(4f, 20f, 0f), new Vector3(1f, 10f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(-4f, 20f, 0f), new Vector3(1f, 10f, 10f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);

                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 20f, 4f), new Vector3(10f, 10f, 1f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
                        PhotonNetwork.RaiseEvent(68, new object[] { "platf", new Vector3(0f, 20f, -4f), new Vector3(10f, 10f, 1f) }, new RaiseEventOptions { TargetActors = new int[] { anum, PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);

                        GameObject lol = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(lol, 60f);
                        lol.GetComponent<Renderer>().material.color = Color.black;
                        lol.transform.position = new Vector3(0f, 20f, 0f);
                        lol.transform.localScale = new Vector3(10f, 1f, 10f);

                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    TeleportPlayer(originalMePosition);
                    PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", whereOriginalPlayerPos }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                    PhotonNetwork.RaiseEvent(68, new object[] { "unmuteall" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber } }, SendOptions.SendReliable);
                }
            }
        }

        public static void AdminSoundMicGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "soundboard", "Alone at the Edge of a Universe", "https://github.com/iiDk-the-actual/ModInfo/raw/main/alone%20at%20the%20edge%20of%20a%20universe.mp3" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void AdminSoundLocalGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "soundcs", "Alone at the Edge of a Universe", "https://github.com/iiDk-the-actual/ModInfo/raw/main/alone%20at%20the%20edge%20of%20a%20universe.mp3" }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void EnableNoAdminIndicator()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "nocone", true }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            lastplayercount = -1;
        }

        public static void NoAdminIndicator()
        {
            if (!PhotonNetwork.InRoom)
            {
                lastplayercount = -1;
            }
            if (PhotonNetwork.PlayerList.Length != lastplayercount && PhotonNetwork.InRoom)
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "nocone", true }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                lastplayercount = PhotonNetwork.PlayerList.Length;
            }
        }

        public static void AdminIndicatorBack()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "nocone", false }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void EnableAdminMenuUserTags()
        {
            PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;
        }

        private static bool lastInRoom = false;
        private static int lastPlayerCount = -1;
        public static void AdminUserTagSys(EventData data)
        {
            try
            {
                if (data.Code == 68 && PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false) != PhotonNetwork.LocalPlayer)
                {
                    object[] args = (object[])data.CustomData;
                    string command = (string)args[0];
                    switch (command)
                    {
                        case "confirmusing":
                            if (admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            {
                                VRRig vrrig = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
                                if (!nametags.ContainsKey(vrrig))
                                {
                                    GameObject go = new GameObject("iiMenu_Nametag");
                                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                    TextMesh textMesh = go.AddComponent<TextMesh>();
                                    textMesh.fontSize = 48;
                                    textMesh.characterSize = 0.1f;
                                    textMesh.anchor = TextAnchor.MiddleCenter;
                                    textMesh.alignment = TextAlignment.Center;

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);

                                    nametags.Add(vrrig, go);
                                } else
                                {
                                    TextMesh textMesh = nametags[vrrig].GetComponent<TextMesh>();

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);
                                }
                            }
                            break;
                    }
                }
            }
            catch { }
        }

        private static Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject> { };
        public static void AdminMenuUserTags()
        {
            if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "isusing" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
            lastInRoom = PhotonNetwork.InRoom;
            lastPlayerCount = PhotonNetwork.PlayerList.Length;
            if (!PhotonNetwork.InRoom)
                lastPlayerCount = -1;
            
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    nametags.Remove(nametag.Key);
                } else
                {
                    nametag.Value.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                    nametag.Value.transform.position = nametag.Key.headMesh.transform.position + nametag.Key.headMesh.transform.up * 0.6f;
                    nametag.Value.transform.LookAt(Camera.main.transform.position);
                    nametag.Value.transform.Rotate(0f, 180f, 0f);
                }
            }
        }

        public static void DisableAdminMenuUserTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                UnityEngine.Object.Destroy(nametag.Value);
            }
            nametags.Clear();
        }

        public static void JoinGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "join", searchText.ToUpper() }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void JoinAll()
        {
            if (rightTrigger > 0.5f && Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(68, new object[] { "join", searchText.ToUpper() }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void NotifyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "notify", searchText }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void NotifyAll()
        {
            if (rightTrigger > 0.5f && Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(68, new object[] { "notify", searchText }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void GetMenuUsers()
        {
            Miscellaneous.indicatorDelay = Time.time + 2f;
            PhotonNetwork.RaiseEvent(68, new object[] { "isusing" }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        private static bool lastLasering = false;
        public static Coroutine laserCoroutine = null;
        public static void AdminLaser()
        {
            if (leftPrimary || rightPrimary)
            {
                Vector3 dir = rightPrimary ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.right : -GorillaTagger.Instance.offlineVRRig.leftHandTransform.right;
                Vector3 startPos = (rightPrimary ? GorillaTagger.Instance.offlineVRRig.rightHandTransform.position : GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) + (dir * 0.1f);
                try
                {
                    Physics.Raycast(startPos + (dir / 3f), dir, out var Ray, 512f, NoInvisLayerMask());
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        PhotonNetwork.RaiseEvent(68, new object[] { "silkick", RigManager.GetPlayerFromVRRig(possibly).UserId }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    }
                } catch { }
                if (Time.time > stupiddelayihate)
                {
                    stupiddelayihate = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(68, new object[] { "laser", true, rightPrimary }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
            bool isLasering = leftPrimary || rightPrimary;
            if (lastLasering && !isLasering)
            {
                PhotonNetwork.RaiseEvent(68, new object[] { "laser", false, false }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
            lastLasering = isLasering;
        }

        private static float beamDelay = 0f;
        public static void AdminBeam()
        {
            if (rightTrigger > 0.5f && Time.time > beamDelay)
            {
                beamDelay = Time.time + 0.05f;
                float h = (Time.frameCount / 180f) % 1f;
                Color color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                PhotonNetwork.RaiseEvent(68, new object[] { "lr", color.r, color.g, color.b, color.a, 0.5f, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.5f, 0f), GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30) * 100f, 0.5f, Mathf.Sin((float)Time.frameCount / 30) * 100f), 0.1f }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void FlyAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "vel", new Vector3(0f, 10f, 0f) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void AdminBringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stupiddelayihate)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        stupiddelayihate = Time.time + 0.1f;
                        PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f) }, new RaiseEventOptions { TargetActors = new int[] { RigManager.GetPlayerFromVRRig(possibly).ActorNumber } }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void BringAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void BringHandAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", TrueRightHand().position + TrueRightHand().forward }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void BringHeadAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void OrbitAllUsing()
        {
            if (Time.time > stupiddelayihate)
            {
                stupiddelayihate = Time.time + 0.05f;
                PhotonNetwork.RaiseEvent(68, new object[] { "tpnv", GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f)) }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void ConfirmNotifyAllUsing()
        {
            PhotonNetwork.RaiseEvent(68, new object[] { "notify", admins[PhotonNetwork.LocalPlayer.UserId] == "goldentrophy" ? "Yes, I am the real goldentrophy. I made the menu." : "Yes, I am the real " + admins[PhotonNetwork.LocalPlayer.UserId] + ". I am an admin in the Discord server." }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        public static void AdminFakeCosmetics()
        {
            foreach (string cosmetic in CosmeticsController.instance.currentWornSet.ToDisplayNameArray())
                PhotonNetwork.RaiseEvent(68, new object[] { "cosmetic", cosmetic }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray());
        }

        public static bool daaind = false;
        public static void DisableAllAdminIndicators()
        {
            daaind = true;
        }
        public static void EnableAllAdminIndicators()
        {
            daaind = false;
        }
    }
}
