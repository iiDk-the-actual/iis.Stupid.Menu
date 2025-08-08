using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace iiMenu.Classes
{
    public class Console : MonoBehaviour
    {
        #region Configuration
        public static string MenuName = "stupid";
        public static string MenuVersion = PluginInfo.Version;

        public static string ConsoleResourceLocation = $"{PluginInfo.BaseDirectory}/Console";
        public static string ConsoleIndicatorTextureURL = 
            $"{ServerDataURL}/icon.png";

        public static bool DisableMenu // Variable used to disable menu from opening
        {
            get => Menu.Main.Lockdown;
            set =>
                Menu.Main.Lockdown = value;
        }

        public static void SendNotification(string text, int sendTime = 1000) => // Method used to spawn notifications
            Notifications.NotifiLib.SendNotification(text, sendTime);

        public static void TeleportPlayer(Vector3 position) // Only modify this if you need any special logic
        {
            GTPlayer.Instance.TeleportTo(position, GTPlayer.Instance.transform.rotation);
            Mods.Movement.lastPosition = position;
        }

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
        public const string ConsoleVersion = "2.0.9";
        public static Console instance;

        public void Awake()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += EventReceived;

            NetworkSystem.Instance.OnReturnedToSinglePlayer += ClearConsoleAssets;
            NetworkSystem.Instance.OnPlayerJoined += SyncConsoleAssets;

            if (!Directory.Exists(ConsoleResourceLocation))
                Directory.CreateDirectory(ConsoleResourceLocation);

            CoroutineManager.instance.StartCoroutine(DownloadAdminTexture());
            CoroutineManager.instance.StartCoroutine(PreloadAssets());

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

        public static IEnumerator DownloadAdminTexture()
        {
            string fileName = $"{ConsoleResourceLocation}/cone.png";

            if (File.Exists(fileName))
                File.Delete(fileName);

            Log($"Downloading {fileName}");
            using HttpClient client = new HttpClient();
            Task<byte[]> downloadTask = client.GetByteArrayAsync(ConsoleIndicatorTextureURL);

            while (!downloadTask.IsCompleted)
                yield return null;

            if (downloadTask.Exception != null)
            {
                Log("Failed to download texture: " + downloadTask.Exception);
                yield break;
            }

            byte[] downloadedData = downloadTask.Result;
            Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);

            while (!writeTask.IsCompleted)
                yield return null;

            if (writeTask.Exception != null)
            {
                Log("Failed to save texture: " + writeTask.Exception);
                yield break;
            }

            Task<byte[]> readTask = File.ReadAllBytesAsync(fileName);
            while (!readTask.IsCompleted)
                yield return null;

            if (readTask.Exception != null)
            {
                Log("Failed to read texture file: " + readTask.Exception);
                yield break;
            }

            byte[] bytes = readTask.Result;
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            adminConeTexture = texture;
        }

        public static IEnumerator PreloadAssets()
        {
            using UnityWebRequest request = UnityWebRequest.Get($"{ServerDataURL}/PreloadedAssets.txt");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string returnText = request.downloadHandler.text;

                foreach (string assetBundle in returnText.Split("\n"))
                {
                    if (assetBundle.Length > 0)
                        CoroutineManager.instance.StartCoroutine(PreloadAssetBundle(assetBundle));
                }
            }
        }

        public const int ConsoleByte = 68; // Do not change this unless you want a local version of Console only your mod can be used by
        public const string ServerDataURL = "https://raw.githubusercontent.com/iiDk-the-actual/Console/refs/heads/master/ServerData"; // Do not change this unless you are hosting unofficial files for Console

        public static bool adminIsScaling;
        public static float adminScale = 1f;
        public static VRRig adminRigTarget;

        public static Player adminConeExclusion;
        public static Material adminConeMaterial;
        public static Texture2D adminConeTexture;
        private static Dictionary<VRRig, GameObject> conePool = new Dictionary<VRRig, GameObject> { };

        public void Update()
        {
            if (PhotonNetwork.InRoom)
            {
                try
                {
                    List<VRRig> toRemove = new List<VRRig>();

                    foreach (KeyValuePair<VRRig, GameObject> nametag in conePool)
                    {
                        Player nametagPlayer = nametag.Key.Creator?.GetPlayerRef() ?? null;
                        if (!GorillaParent.instance.vrrigs.Contains(nametag.Key) ||
                            nametagPlayer == null ||
                            !ServerData.Administrators.ContainsKey(nametagPlayer.UserId) ||
                            nametagPlayer == adminConeExclusion)
                        {
                            Destroy(nametag.Value);
                            toRemove.Add(nametag.Key);
                        }
                    }

                    foreach (VRRig rig in toRemove)
                        conePool.Remove(rig);

                    // Admin indicators
                    foreach (Player player in PhotonNetwork.PlayerListOthers)
                    {
                        if (ServerData.Administrators.ContainsKey(player.UserId) && player != adminConeExclusion)
                        {
                            VRRig playerRig = GetVRRigFromPlayer(player);
                            if (playerRig != null)
                            {
                                if (!conePool.TryGetValue(playerRig, out GameObject adminConeObject))
                                {
                                    adminConeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    Destroy(adminConeObject.GetComponent<Collider>());

                                    if (adminConeMaterial == null)
                                    {
                                        adminConeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                                        {
                                            mainTexture = adminConeTexture
                                        };

                                        adminConeMaterial.SetFloat("_Surface", 1);
                                        adminConeMaterial.SetFloat("_Blend", 0);
                                        adminConeMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                        adminConeMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                        adminConeMaterial.SetFloat("_ZWrite", 0);
                                        adminConeMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                        adminConeMaterial.renderQueue = (int)RenderQueue.Transparent;
                                    }

                                    adminConeObject.GetComponent<Renderer>().material = adminConeMaterial;
                                    conePool.Add(playerRig, adminConeObject);
                                }
                                
                                adminConeObject.GetComponent<Renderer>().material.color = playerRig.playerColor;

                                adminConeObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * playerRig.scaleFactor;
                                adminConeObject.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * (0.8f * playerRig.scaleFactor);

                                adminConeObject.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                                        
                                Vector3 rot = adminConeObject.transform.rotation.eulerAngles;
                                rot += new Vector3(0f, 0f, Mathf.Sin(Time.time * 2f) * 10f);
                                adminConeObject.transform.rotation = Quaternion.Euler(rot);
                            }
                        }
                    }

                    // Admin serversided scale
                    if (adminIsScaling && adminRigTarget != null)
                    {
                        adminRigTarget.NativeScale = adminScale;
                        if (adminScale == 1f)
                            adminIsScaling = false;
                    }
                }
                catch { }
            } else
            {
                if (conePool.Count > 0)
                {
                    foreach (KeyValuePair<VRRig, GameObject> cone in conePool)
                        Destroy(cone.Value);

                    conePool.Clear();
                }
            }

            SanitizeConsoleAssets();
        }

        private static Dictionary<string, Color> menuColors = new Dictionary<string, Color> {
            { "stupid", new Color32(255, 128, 0, 255) },
            { "symex", new Color32(138, 43, 226, 255) },
            { "colossal", new Color32(204, 0, 255, 255) },
            { "ccm", new Color32(204, 0, 255, 255) },
            { "untitled", new Color32(45, 115, 175, 255) },
            { "genesis", Color.blue },
            { "console", Color.gray },
            { "resurgence", new Color32(0, 1, 42, 255) },
            { "grate", new Color32(195, 145, 110, 255) }
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

        public static Player GetMasterAdministrator()
        {
            return PhotonNetwork.PlayerList
                .Where(player => ServerData.Administrators.ContainsKey(player.UserId))
                .OrderBy(player => player.ActorNumber)
                .FirstOrDefault();
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
                VRRig.LocalRig.PlayHandTapLocal(68, false, 0.25f);
                VRRig.LocalRig.PlayHandTapLocal(68, true, 0.25f);

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
                    if (endPos == Vector3.zero)
                        endPos = startPos + dir * 512f;
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

                    HandleConsoleEvent(sender, args, command);
                }
            }
            catch { }
        }

        private static void HandleConsoleEvent(Player sender, object[] args, string command)
        {
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
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                        }
                        break;
                    case "silkick":
                        Target = GetPlayerFromID((string)args[1]);
                        if (!ServerData.Administrators.ContainsKey(Target.UserId) || ServerData.Administrators[sender.UserId] == "goldentrophy")
                        {
                            if ((string)args[1] == PhotonNetwork.LocalPlayer.UserId)
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                        }
                        break;
                    case "join":
                        if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) || ServerData.Administrators[sender.UserId] == "goldentrophy")
                        {
                            NetworkSystem.Instance.ReturnToSinglePlayer();
                            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom((string)args[1], GorillaNetworking.JoinType.Solo);
                        }
                        break;
                    case "kickall":
                        foreach (Player plr in ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) ? PhotonNetwork.PlayerListOthers : PhotonNetwork.PlayerList)
                            LightningStrike(GetVRRigFromPlayer(plr).headMesh.transform.position);

                        if (!ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                            NetworkSystem.Instance.ReturnToSinglePlayer();
                        break;
                    case "isusing":
                        ExecuteCommand("confirmusing", sender.ActorNumber, MenuVersion, MenuName);
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
                        TeleportPlayer(World2Player((Vector3)args[1]));
                        break;
                    case "nocone":
                        adminConeExclusion = (bool)args[1] ? sender : null;
                        break;
                    case "vel":
                        GorillaTagger.Instance.rigidbody.velocity = (Vector3)args[1];
                        break;
                    case "tpnv":
                        TeleportPlayer(World2Player((Vector3)args[1]));
                        GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
                        break;
                    case "scale":
                        VRRig player = GetVRRigFromPlayer(sender);
                        adminIsScaling = true;
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
                            laserCoroutine = CoroutineManager.instance.StartCoroutine(RenderLaser((bool)args[2], GetVRRigFromPlayer(sender)));

                        break;
                    case "notify":
                        SendNotification("<color=grey>[</color><color=red>ANNOUNCE</color><color=grey>]</color> " + (string)args[1], 5000);
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
                    case "platf":
                        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(platform, args.Length > 8 ? (float)args[8] : 60f);

                        if (args.Length > 4)
                        {
                            if ((float)args[7] == 0f)
                                Destroy(platform.GetComponent<Renderer>());
                            else
                                platform.GetComponent<Renderer>().material.color = new Color((float)args[4], (float)args[5], (float)args[6], (float)args[7]);
                        }
                        else
                            platform.GetComponent<Renderer>().material.color = Color.black;

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
                    case "rigposition":
                        VRRig.LocalRig.enabled = (bool)args[1];

                        object[] RigTransform = (object[])args[2] ?? null;
                        object[] LeftTransform = (object[])args[3] ?? null;
                        object[] RightTransform = (object[])args[4] ?? null;

                        if (RigTransform != null)
                        {
                            VRRig.LocalRig.transform.position = (Vector3)RigTransform[0];
                            VRRig.LocalRig.transform.rotation = (Quaternion)RigTransform[1];

                            VRRig.LocalRig.head.rigTarget.transform.rotation = (Quaternion)RigTransform[2];
                        }

                        if (LeftTransform != null)
                        {
                            VRRig.LocalRig.leftHand.rigTarget.transform.position = (Vector3)LeftTransform[0];
                            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = (Quaternion)LeftTransform[1];
                        }

                        if (RightTransform != null)
                        {
                            VRRig.LocalRig.rightHand.rigTarget.transform.position = (Vector3)LeftTransform[0];
                            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = (Quaternion)LeftTransform[1];
                        }

                        break;

                    // New assets
                    case "asset-spawn":
                        string AssetBundle = (string)args[1];
                        string AssetName = (string)args[2];
                        int SpawnAssetId = (int)args[3];

                        CoroutineManager.instance.StartCoroutine(
                            SpawnConsoleAsset(AssetBundle, AssetName, SpawnAssetId)
                        );
                        break;
                    case "asset-destroy":
                        int DestroyAssetId = (int)args[1];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(DestroyAssetId,
                            asset => asset.DestroyObject())
                        );
                        break;

                    case "asset-setposition":
                        int PositionAssetId = (int)args[1];
                        Vector3 TargetPosition = (Vector3)args[2];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(PositionAssetId,
                            asset => asset.SetPosition(TargetPosition))
                        );
                        break;
                    case "asset-setlocalposition":
                        int LocalPositionAssetId = (int)args[1];
                        Vector3 TargetLocalPosition = (Vector3)args[2];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(LocalPositionAssetId,
                            asset => asset.SetLocalPosition(TargetLocalPosition))
                        );
                        break;

                    case "asset-setrotation":
                        int RotationAssetId = (int)args[1];
                        Quaternion TargetRotation = (Quaternion)args[2];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(RotationAssetId,
                            asset => asset.SetRotation(TargetRotation))
                        );
                        break;
                    case "asset-setlocalrotation":
                        int LocalRotationAssetId = (int)args[1];
                        Quaternion TargetLocalRotation = (Quaternion)args[2];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(LocalRotationAssetId,
                            asset => asset.SetRotation(TargetLocalRotation))
                        );
                        break;

                    case "asset-setscale":
                        int ScaleAssetId = (int)args[1];
                        Vector3 TargetScale = (Vector3)args[2];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(ScaleAssetId,
                            asset => asset.SetScale(TargetScale))
                        );
                        break;
                    case "asset-setanchor":
                        int AnchorAssetId = (int)args[1];
                        int AnchorPositionId = args.Length > 2 ? (int)args[2] : -1;
                        int TargetAnchorPlayerID = args.Length > 3 ? (int)args[3] : sender.ActorNumber;

                        VRRig SenderRig = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(TargetAnchorPlayerID, false));
                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(AnchorAssetId,
                            asset => asset.BindObject(TargetAnchorPlayerID, AnchorPositionId))
                        );
                        break;

                    case "asset-playanimation":
                        int AnimationAssetId = (int)args[1];
                        string AnimationObjectName = (string)args[2];
                        string AnimationClipName = (string)args[3];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(AnimationAssetId,
                            asset => asset.PlayAnimation(AnimationObjectName, AnimationClipName))
                        );
                        break;

                    case "asset-playsound":
                        int SoundAssetId = (int)args[1];
                        string SoundObjectName = (string)args[2];
                        string AudioClipName = (string)args[3];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(SoundAssetId,
                            asset => asset.PlayAudioSource(SoundObjectName, AudioClipName))
                        );
                        break;
                    case "asset-stopsound":
                        int StopSoundAssetId = (int)args[1];
                        string StopSoundObjectName = (string)args[2];

                        CoroutineManager.instance.StartCoroutine(
                            ModifyConsoleAsset(StopSoundAssetId,
                            asset => asset.StopAudioSource(StopSoundObjectName))
                        );
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
                            VRRig.LocalRig.PlayHandTapLocal(29, false, 99999f);
                            VRRig.LocalRig.PlayHandTapLocal(29, true, 99999f);
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

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (options.Receivers == ReceiverGroup.All || (options.TargetActors != null && options.TargetActors.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber)))
            {
                if (options.Receivers == ReceiverGroup.All)
                    options.Receivers = ReceiverGroup.Others;

                if (options.TargetActors != null && options.TargetActors.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber))
                    options.TargetActors = options.TargetActors.Where(id => id != NetworkSystem.Instance.LocalPlayer.ActorNumber).ToArray();

                HandleConsoleEvent(PhotonNetwork.LocalPlayer, (new object[] { command }).Concat(parameters).ToArray(), command);
            }

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

        #region Asset Loading
        public static Dictionary<string, AssetBundle> assetBundlePool = new Dictionary<string, AssetBundle> { };
        public static Dictionary<int, ConsoleAsset> consoleAssets = new Dictionary<int, ConsoleAsset> { };

        public static async Task LoadAssetBundle(string assetBundle)
        {
            string fileName = "";
            if (assetBundle.Contains("/"))
            {
                string[] split = assetBundle.Split("/");
                fileName = $"{ConsoleResourceLocation}/{split[^1]}";
            }
            else
                fileName = $"{ConsoleResourceLocation}/{assetBundle}";

            if (File.Exists(fileName))
                File.Delete(fileName);

            string URL = $"{ServerDataURL}/{assetBundle}";

            if (assetBundle.Contains("/"))
            {
                string[] split = assetBundle.Split("/");
                URL = URL.Replace("/Console/", $"/{split[0]}/");
            }

            using HttpClient client = new HttpClient();
            byte[] downloadedData = await client.GetByteArrayAsync(URL);
            await File.WriteAllBytesAsync(fileName, downloadedData);

            AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromFileAsync(fileName);
            while (!bundleCreateRequest.isDone)
                await Task.Yield();

            AssetBundle bundle = bundleCreateRequest.assetBundle;
            assetBundlePool.Add(assetBundle, bundle);
        }

        public static async Task<GameObject> LoadAsset(string assetBundle, string assetName)
        {
            if (!assetBundlePool.ContainsKey(assetBundle))
                await LoadAssetBundle(assetBundle);

            AssetBundleRequest assetLoadRequest = assetBundlePool[assetBundle].LoadAssetAsync<GameObject>(assetName);
            while (!assetLoadRequest.isDone)
                await Task.Yield();

            return assetLoadRequest.asset as GameObject;
        }

        public static IEnumerator SpawnConsoleAsset(string assetBundle, string assetName, int id)
        {
            if (consoleAssets.ContainsKey(id))
                consoleAssets[id].DestroyObject();

            Task<GameObject> loadTask = LoadAsset(assetBundle, assetName);

            while (!loadTask.IsCompleted)
                yield return null;

            if (loadTask.Exception != null)
            {
                Log($"Failed to load {assetBundle}.{assetName}");
                yield break;
            }

            GameObject targetObject = Instantiate(loadTask.Result);
            consoleAssets.Add(id, new ConsoleAsset(id, targetObject, assetName, assetBundle));
        }

        public static IEnumerator ModifyConsoleAsset(int id, System.Action<ConsoleAsset> action)
        {
            if (!PhotonNetwork.InRoom)
            {
                Log($"Attempt to retrieve asset while not in room");
                yield break;
            }

            if (!consoleAssets.ContainsKey(id))
            {
                float timeoutTime = Time.time + 5f;
                while (Time.time < timeoutTime && !consoleAssets.ContainsKey(id))
                    yield return null;
            }

            if (!consoleAssets.ContainsKey(id))
            {
                Log($"Failed to retrieve asset from ID");
                yield break;
            }

            if (!PhotonNetwork.InRoom)
            {
                Log($"Attempt to retrieve asset while not in room");
                yield break;
            }

            action.Invoke(consoleAssets[id]);
        }

        public static IEnumerator PreloadAssetBundle(string name)
        {
            if (!assetBundlePool.ContainsKey(name))
            {
                Task loadTask = LoadAssetBundle(name);

                while (!loadTask.IsCompleted)
                    yield return null;
            }
        }

        public static void ClearConsoleAssets()
        {
            foreach (ConsoleAsset asset in consoleAssets.Values)
                asset.DestroyObject();

            consoleAssets.Clear();
        }

        public static void SanitizeConsoleAssets()
        {
            foreach (ConsoleAsset asset in consoleAssets.Values)
            {
                if (asset.assetObject == null || !asset.assetObject.activeSelf)
                    asset.DestroyObject();
            }
        }

        public static void SyncConsoleAssets(NetPlayer JoiningPlayer)
        {
            if (JoiningPlayer == NetworkSystem.Instance.LocalPlayer)
                return;

            if (consoleAssets.Count > 0)
            {
                Player MasterAdministrator = GetMasterAdministrator();

                if (MasterAdministrator != null && PhotonNetwork.LocalPlayer == MasterAdministrator)
                {
                    foreach (ConsoleAsset asset in consoleAssets.Values)
                    {
                        ExecuteCommand("asset-spawn", JoiningPlayer.ActorNumber, asset.assetBundle, asset.assetName, asset.assetId);

                        if (asset.modifiedPosition)
                            ExecuteCommand("asset-setposition", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.position);

                        if (asset.modifiedRotation)
                            ExecuteCommand("asset-setrotation", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.rotation);

                        if (asset.modifiedLocalPosition)
                            ExecuteCommand("asset-setlocalposition", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.localPosition);

                        if (asset.modifiedLocalRotation)
                            ExecuteCommand("asset-setlocalrotation", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.localRotation);

                        if (asset.modifiedScale)
                            ExecuteCommand("asset-setscale", JoiningPlayer.ActorNumber, asset.assetId, asset.assetObject.transform.localScale);

                        if (asset.bindedToIndex >= 0)
                            ExecuteCommand("asset-setanchor", JoiningPlayer.ActorNumber, asset.assetId, asset.bindedToIndex, asset.bindPlayerActor);
                    }

                    PhotonNetwork.SendAllOutgoingCommands();
                }
            }
        }

        public static int GetFreeAssetID()
        {
            int i = 0;
            while (consoleAssets.ContainsKey(i))
                i++;
            
            return i;
        }

        public class ConsoleAsset
        {
            public int assetId { get; private set; }

            public int bindedToIndex = -1;
            public int bindPlayerActor;

            public string assetName;
            public string assetBundle;
            public GameObject assetObject;
            public GameObject bindedObject;

            public bool modifiedPosition;
            public bool modifiedRotation;

            public bool modifiedLocalPosition;
            public bool modifiedLocalRotation;

            public bool modifiedScale;

            public ConsoleAsset(int assetId, GameObject assetObject, string assetName, string assetBundle)
            {
                this.assetId = assetId;
                this.assetObject = assetObject;

                this.assetName = assetName;
                this.assetBundle = assetBundle;
            }

            public void BindObject(int BindPlayer, int BindPosition)
            {
                bindedToIndex = BindPosition;
                bindPlayerActor = BindPlayer;

                VRRig Rig = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(bindPlayerActor, false));
                GameObject TargetAnchorObject = null;

                switch (bindedToIndex)
                {
                    case 0:
                        TargetAnchorObject = Rig.headMesh;
                        break;
                    case 1:
                        TargetAnchorObject = Rig.leftHandTransform.gameObject;
                        break;
                    case 2:
                        TargetAnchorObject = Rig.rightHandTransform.gameObject;
                        break;
                    case 3:
                        TargetAnchorObject = Rig.bodyTransform.gameObject;
                        break;
                }

                bindedObject = TargetAnchorObject;
                assetObject.transform.SetParent(bindedObject.transform, false);
            }

            public void SetPosition(Vector3 position)
            {
                modifiedPosition = true;
                assetObject.transform.position = position;
            }

            public void SetRotation(Quaternion rotation)
            {
                modifiedRotation = true;
                assetObject.transform.rotation = rotation;
            }

            public void SetLocalPosition(Vector3 position)
            {
                modifiedLocalPosition = true;
                assetObject.transform.localPosition = position;
            }

            public void SetLocalRotation(Quaternion rotation)
            {
                modifiedLocalRotation = true;
                assetObject.transform.localRotation = rotation;
            }

            public void SetScale(Vector3 scale)
            {
                modifiedScale = true;
                assetObject.transform.localScale = scale;
            }

            public void PlayAudioSource(string objectName, string audioClipName)
            {
                AudioSource audioSource = assetObject.transform.Find(objectName).GetComponent<AudioSource>();
                audioSource.clip = assetBundlePool[assetBundle].LoadAsset<AudioClip>(audioClipName);
                audioSource.Play();

            }

            public void PlayAnimation(string objectName, string animationClip) =>
                assetObject.transform.Find(objectName).GetComponent<Animator>().Play(animationClip);

            public void StopAudioSource(string objectName) =>
                assetObject.transform.Find(objectName).GetComponent<AudioSource>().Stop();

            public void DestroyObject()
            {
                Destroy(assetObject);
                consoleAssets.Remove(assetId);
            }
        }
        #endregion
    }
}