using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering;
using GorillaNetworking;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace iiMenu.Classes
{
    public class Console : MonoBehaviour
    {
        #region Configuration
        public static string MenuName = "stupid";
        public static string MenuVersion = PluginInfo.Version;

        public static string ConeResourceLocation = "iiMenu.Resources.icon.png";

        public static bool DisableMenu // Variable used to disable menu from opening
        {
            get => Menu.Main.Lockdown;
            set =>
                Menu.Main.Lockdown = value;
        }

        public static void SendNotification(string text, int sendTime = 1000) => // Method used to spawn notifications
            Notifications.NotifiLib.SendNotification(text, sendTime);

        public static void EnableMod(string mod, bool enable) // Method used to enable mods
        {
            ButtonInfo Button = Menu.Main.GetIndex(mod);
            if (!Button.isTogglable)
                Button.method.Invoke();
            else
            {
                Button.enabled = !enable;
                ToggleMod(Button.buttonText);
            }
        }

        public static void ToggleMod(string mod) => // Method used to toggle mod by name
            Menu.Main.Toggle(mod);

        public static void Log(string text) => // Method used to log info
            LogManager.Log(text);

        #endregion

        #region Events
        public const string ConsoleVersion = "2.0.0";
        public static Console instance;

        public void Awake()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;

            Log($@"

     ▄▄·        ▐ ▄ .▄▄ ·       ▄▄▌  ▄▄▄ .
    ▐█ ▌▪▪     •█▌▐█▐█ ▀. ▪     ██•  ▀▄.▀·
    ██ ▄▄ ▄█▀▄ ▐█▐▐▌▄▀▀▀█▄ ▄█▀▄ ██▪  ▐▀▀▪▄
    ▐███▌▐█▌.▐▌██▐█▌▐█▄▪▐█▐█▌.▐▌▐█▌▐▌▐█▄▄▌
    ·▀▀▀  ▀█▄▀▪▀▀ █▪ ▀▀▀▀  ▀█▄▀▪.▀▀▀  ▀▀▀       
           Console Portable {ConsoleVersion}
     Developed by goldentrophy & Twigcore
");
        }

        public void OnDisable() =>
            PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;

        public static Texture2D LoadTextureFromResource(string resourcePath)
        {
            Texture2D texture = new Texture2D(2, 2);

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                byte[] fileData = new byte[stream.Length];
                stream.Read(fileData, 0, (int)stream.Length);
                texture.LoadImage(fileData);
            }

            return texture;
        }

        public const int ConsoleByte = 68; // Do not change this unless you want a local version of Console only your mod can be used by

        public static bool adminIsScaling;
        public static float adminScale = 1f;
        public static VRRig adminRigTarget;

        public static Player adminConeExclusion;
        public static Material adminConeMaterial;
        public static Texture2D adminConeTexture;

        public void Update()
        {
            if (PhotonNetwork.InRoom)
            {
                try
                {
                    // Admin indicators
                    foreach (Player player in PhotonNetwork.PlayerListOthers)
                    {
                        if (ServerData.Administrators.ContainsKey(player.UserId))
                        {
                            if (player != adminConeExclusion)
                            {
                                VRRig playerRig = GetVRRigFromPlayer(player);
                                if (playerRig != null)
                                {
                                    GameObject adminConeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    Destroy(adminConeObject.GetComponent<Collider>());
                                    Destroy(adminConeObject, Time.deltaTime);

                                    if (adminConeMaterial == null)
                                    {
                                        adminConeMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                                        if (adminConeTexture == null)
                                            adminConeTexture = LoadTextureFromResource(ConeResourceLocation);

                                        adminConeMaterial.mainTexture = adminConeTexture;

                                        adminConeMaterial.SetFloat("_Surface", 1);
                                        adminConeMaterial.SetFloat("_Blend", 0);
                                        adminConeMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                        adminConeMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                        adminConeMaterial.SetFloat("_ZWrite", 0);
                                        adminConeMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                        adminConeMaterial.renderQueue = (int)RenderQueue.Transparent;

                                        adminConeMaterial.SetFloat("_Glossiness", 0f);
                                        adminConeMaterial.SetFloat("_Metallic", 0f);
                                    }

                                    adminConeObject.GetComponent<Renderer>().material = adminConeMaterial;
                                    adminConeObject.GetComponent<Renderer>().material.color = playerRig.playerColor;

                                    adminConeObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f);
                                    adminConeObject.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * 0.8f;
                                    adminConeObject.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                                        
                                    Vector3 rot = adminConeObject.transform.rotation.eulerAngles;
                                    rot += new Vector3(0f, 0f, Mathf.Sin(Time.time * 2f) * 10f);
                                    adminConeObject.transform.rotation = Quaternion.Euler(rot);
                                }
                            }
                        }
                    }

                    // Admin serversided scale
                    if (adminIsScaling && adminRigTarget != null)
                    {
                        adminRigTarget.NativeScale = adminScale;
                        adminRigTarget.lastScaleFactor = adminRigTarget.scaleFactor;
                    }
                }
                catch { }
            }
        }

        private static Dictionary<string, Color> menuColors = new Dictionary<string, Color> {
            { "stupid", new Color32(255, 128, 0, 255) },
            { "symex", new Color32(138, 43, 226, 255) },
            { "colossal", new Color32(204, 0, 255, 255) },
            { "ccm", new Color32(204, 0, 255, 255) },
            { "untitled", new Color32(45, 115, 175, 255) },
            { "genesis", Color.blue },
            { "steal", Color.gray }
        };

        public static int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static int Zone = LayerMask.NameToLayer("Zone");
        public static int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
        public static int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

        public static int NoInvisLayerMask() =>
            ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone | 1 << GorillaTrigger | 1 << GorillaBoundary | 1 << GorillaCosmetics | 1 << GorillaParticle);

        public static Vector3 World2Player(Vector3 world) =>
            world - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position;

        public static Color GetMenuTypeName(string type)
        {
            if (menuColors.ContainsKey(type))
                return menuColors[type];

            return Color.red;
        }

        public static VRRig GetVRRigFromPlayer(NetPlayer p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

        public static NetPlayer GetPlayerFromID(string id) =>
            PhotonNetwork.PlayerList.FirstOrDefault(player => player.UserId == id);

        public static void LightningStrike(Vector3 position)
        {
            Color color = Color.cyan;

            GameObject line = new GameObject("LightningOuter");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = color; liner.endColor = color; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 5; liner.useWorldSpace = true;
            Vector3 victim = position;
            for (int i = 0; i < 5; i++)
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, false, 0.25f);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, true, 0.25f);

                liner.SetPosition(i, victim);
                victim += new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
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
                Destroy(line, Time.deltaTime);

                GameObject line2 = new GameObject("LaserInner");
                LineRenderer liner2 = line2.AddComponent<LineRenderer>();
                liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.1f; liner2.endWidth = 0.1f; liner2.positionCount = 2; liner2.useWorldSpace = true;
                liner2.SetPosition(0, startPos + (dir * 0.1f));
                liner2.SetPosition(1, endPos);
                liner2.material.shader = Shader.Find("GUI/Text Shader");
                liner2.material.renderQueue = liner.material.renderQueue + 1;
                Destroy(line2, Time.deltaTime);

                GameObject whiteParticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(whiteParticle, 2f);
                Destroy(whiteParticle.GetComponent<Collider>());
                whiteParticle.GetComponent<Renderer>().material.color = Color.yellow;
                whiteParticle.AddComponent<Rigidbody>().velocity = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 7.5f), Random.Range(-7.5f, 7.5f));
                whiteParticle.transform.position = endPos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                whiteParticle.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                yield return null;
            }
        }

        private static Dictionary<VRRig, float> confirmUsingDelay = new Dictionary<VRRig, float> { };
        public static float indicatorDelay = 0f;

        public static void EventReceived(EventData data)
        {
            try
            {
                if (data.Code == ConsoleByte) // Admin mods, before you try anything yes it's player ID locked
                {
                    Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);

                    object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                    string command = args.Length > 0 ? (string)args[0] : "";

                    if (ServerData.Administrators.ContainsKey(sender.UserId))
                    {
                        NetPlayer Target = null;

                        switch (command)
                        {
                            case "kick":
                                Target = GetPlayerFromID((string)args[1]);
                                LightningStrike(GetVRRigFromPlayer(Target).headMesh.transform.position);
                                if (!ServerData.Administrators.ContainsKey(Target.UserId) || ServerData.Administrators[sender.UserId] == "goldentrophy")
                                {
                                    if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                        PhotonNetwork.Disconnect();
                                }
                                break;
                            case "silkick":
                                Target = GetPlayerFromID((string)args[1]);
                                if (!ServerData.Administrators.ContainsKey(Target.UserId) || ServerData.Administrators[sender.UserId] == "goldentrophy")
                                {
                                    if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                        PhotonNetwork.Disconnect();
                                }
                                break;
                            case "join":
                                if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || ServerData.Administrators[sender.UserId] == "goldentrophy")
                                {
                                    PhotonNetwork.Disconnect();
                                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom((string)args[1], GorillaNetworking.JoinType.Solo);
                                }
                                break;
                            case "kickall":
                                foreach (Player plr in ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? PhotonNetwork.PlayerListOthers : PhotonNetwork.PlayerList)
                                    LightningStrike(GetVRRigFromPlayer(plr).headMesh.transform.position);
                                
                                if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                                    PhotonNetwork.Disconnect();
                                break;
                            case "isusing":
                                ExecuteCommand("confirmusing", sender.actorNumber, MenuVersion, MenuName);
                                break;
                            case "forceenable":
                                string ForceMod = (string)args[1];
                                bool EnableValue = (bool)args[2];

                                EnableMod(ForceMod, EnableValue);
                                break;
                            case "toggle":
                                string Mod = (string)args[1];
                                ToggleMod(Mod);
                                break;
                            case "togglemenu":
                                DisableMenu = (bool)args[1];
                                break;
                            case "tp":
                                GorillaLocomotion.GTPlayer.Instance.TeleportTo(
                                    World2Player((Vector3)args[1]),
                                    GorillaLocomotion.GTPlayer.Instance.transform.rotation);
                                break;
                            case "nocone":
                                adminConeExclusion = (bool)args[1] ? sender : null;
                                break;
                            case "vel":
                                GorillaTagger.Instance.rigidbody.velocity = (Vector3)args[1];
                                break;
                            case "tpnv":
                                GorillaLocomotion.GTPlayer.Instance.TeleportTo(
                                    World2Player((Vector3)args[1]),
                                    GorillaLocomotion.GTPlayer.Instance.transform.rotation);
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
                                Color thecolor = new Color((float)args[1], (float)args[2], (float)args[3], (float)args[4]);
                                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = (float)args[5]; liner.endWidth = (float)args[5]; liner.positionCount = 2; liner.useWorldSpace = true;
                                liner.SetPosition(0, (Vector3)args[6]);
                                liner.SetPosition(1, (Vector3)args[7]);
                                liner.material.shader = Shader.Find("GUI/Text Shader");
                                Destroy(lines, (float)args[8]);
                                break;
                            case "notify":
                                SendNotification("<color=grey>[</color><color=red>ANNOUNCE</color><color=grey>]</color> " + (string)args[1], 5000);
                                break;
                            case "platf":
                                GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                Destroy(platform, args.Length > 8 ? (float)args[8] : 60f);

                                if ((float)args[7] == 0)
                                    Destroy(platform.GetComponent<Renderer>());
                                else
                                    platform.GetComponent<Renderer>().material.color = args.Length > 4 ? new Color((float)args[4], (float)args[5], (float)args[6], (float)args[7]) : Color.black;
                                
                                platform.transform.position = (Vector3)args[1];
                                platform.transform.rotation = args.Length > 3 ? Quaternion.Euler((Vector3)args[3]) : Quaternion.identity;
                                platform.transform.localScale = args.Length > 2 ? (Vector3)args[2] : new Vector3(1f, 0.1f, 1f);

                                break;
                            case "muteall":
                                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                                {
                                    if (!line.playerVRRig.muted && !ServerData.Administrators.ContainsKey(line.linePlayer.UserId))
                                        line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);
                                }
                                break;
                            case "unmuteall":
                                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                                {
                                    if (line.playerVRRig.muted)
                                        line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);
                                }
                                break;
                        }
                    }
                    switch (command)
                    {
                        case "confirmusing":
                            if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
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

                                    SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> " + sender.NickName + " is using version " + (string)args[1] + ".", 3000);
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(29, false, 99999f);
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(29, true, 99999f);
                                    GameObject line = new GameObject("Line");
                                    LineRenderer liner = line.AddComponent<LineRenderer>();
                                    liner.startColor = userColor; liner.endColor = userColor; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 2; liner.useWorldSpace = true;

                                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                                    liner.material.shader = Shader.Find("GUI/Text Shader");
                                    Destroy(line, 3f);
                                }
                            }
                            break;
                    }
                }
            }
            catch { }
        }

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.RaiseEvent(ConsoleByte, 
                (new object[] { command })
                    .Concat(parameters)
                    .ToArray(),
            options, SendOptions.SendReliable);
        }

        public static void ExecuteCommand(string command, int[] targets, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = targets }, parameters);

        public static void ExecuteCommand(string command, int target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = new int[] { target } }, parameters);

        public static void ExecuteCommand(string command, ReceiverGroup target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { Receivers = target }, parameters);

        #endregion
    }
}