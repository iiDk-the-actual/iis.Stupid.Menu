using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Experimental
    {
        public static void AntiRPCBan()
        {
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);

            
            /*NetworkSystem.OnLeftRoom();
            NetworkSystem.OnPreLeavingRoom();
            NetworkSystem.OnLeftLobby();*/
            

            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            ScienceExperimentManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);

            try
            {
                GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
                GorillaNot.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
                GorillaNot.instance.OnLeftRoom();
                GorillaNot.instance.OnPreLeavingRoom();
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
            GorillaNot.instance.OnLeftRoom();
        }

        public static void AutoSetMaster()
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().ToLower().Contains("modded"))
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            }
        }

        public static void InfiniteRangeTagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = (isCopying || (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)) ? buttonClickedA : buttonDefaultA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = isCopying ? whoCopy.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f);
                liner.endColor = GetBGColor(0.5f);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, isCopying ? whoCopy.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > teleDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();

                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                        raiseEventOptions.Flags = new WebFlags(1);

                        object[] eventContent = new object[]
                        {
                            PhotonNetwork.LocalPlayer.UserId,
                            RigManager.GetPlayerFromVRRig(possibly).UserId,
                            GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>().currentInfected.Count
                        };
                        PhotonNetwork.RaiseEvent(2, eventContent, raiseEventOptions, SendOptions.SendReliable);
                        teleDebounce = Time.time + 0.2f;
                    }
                }
            }
        }

        public static void BetterFPSBoost()
        {
            foreach (Renderer v in Resources.FindObjectsOfTypeAll<Renderer>())
            {
                try
                {
                    if (v.material.shader.name == "GorillaTag/UberShader")
                    {
                        Material replacement = new Material(Shader.Find("GorillaTag/UberShader"));
                        replacement.color = v.material.color;
                        v.material = replacement;
                    }
                } catch (System.Exception exception) { UnityEngine.Debug.LogError(string.Format("mat error {1} - {0}", exception.Message, exception.StackTrace)); }
            }
        }

        public static float pookiebear = -1f;
        public static void ChangeNameGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = (isCopying || (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)) ? buttonClickedA : buttonDefaultA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = isCopying ? whoCopy.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f);
                liner.endColor = GetBGColor(0.5f);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, isCopying ? whoCopy.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (isCopying && whoCopy != null)
                    if (!Overpowered.IsModded())
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            Overpowered.AntiBan();
                        }
                    }
                    else
                    {
                        { 
                            if (Time.time > pookiebear) 
                            { 
                                pookiebear = Time.time + 0.2f; 
                                Photon.Realtime.Player plr = RigManager.GetPlayerFromVRRig(whoCopy); 
                                plr.NickName = PhotonNetwork.LocalPlayer.NickName; 
                                System.Type targ = typeof(Photon.Realtime.Player); 
                                MethodInfo StartEruptionMethod = targ.GetMethod("SetPlayerNameProperty", BindingFlags.NonPublic | BindingFlags.Instance); 
                                StartEruptionMethod?.Invoke(plr, new object[] { }); 
                                RPCProtection(); 
                            } 
                        } 
                    }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
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
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void ChangeNameAll()
        {
            if (Time.time > pookiebear && rightTrigger > 0.5f)
            { 
                if (!Overpowered.IsModded())
                {
                    if (!GetIndex("Disable Auto Anti Ban").enabled)
                    {
                        Overpowered.AntiBan();
                    }
                }
                else
                {
                    { 
                        pookiebear = Time.time + 0.2f; 
                        foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerListOthers) 
                        { 
                            plr.NickName = PhotonNetwork.LocalPlayer.NickName; 
                            System.Type targ = typeof(Photon.Realtime.Player); 
                            MethodInfo StartEruptionMethod = targ.GetMethod("SetPlayerNameProperty", BindingFlags.NonPublic | BindingFlags.Instance); 
                            StartEruptionMethod?.Invoke(plr, new object[] { }); 
                            RPCProtection(); 
                        }
                    }
                }
            }
        }

        public static void BanGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = (isCopying || (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)) ? buttonClickedA : buttonDefaultA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = isCopying ? whoCopy.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f);
                liner.endColor = GetBGColor(0.5f);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, isCopying ? whoCopy.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (isCopying && whoCopy != null)
                {
                    if (!Overpowered.IsModded())
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            Overpowered.AntiBan();
                        }
                    }
                    else
                    {
                        if (Time.time > pookiebear) 
                        { 
                            pookiebear = Time.time + 0.2f; 
                            Photon.Realtime.Player plr = RigManager.GetPlayerFromVRRig(whoCopy); 
                            plr.NickName = bannableNames[UnityEngine.Random.Range(0, bannableNames.Length - 1)]; 
                            System.Type targ = typeof(Photon.Realtime.Player); 
                            MethodInfo StartEruptionMethod = targ.GetMethod("SetPlayerNameProperty", BindingFlags.NonPublic | BindingFlags.Instance); 
                            StartEruptionMethod?.Invoke(plr, new object[] { }); 
                            RPCProtection(); 
                        } 
                    }
                } 
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
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
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void BanAll()
        {
            if (Time.time > pookiebear && rightTrigger > 0.5f) 
            { 
                if (!Overpowered.IsModded())
                {
                    if (!GetIndex("Disable Auto Anti Ban").enabled)
                    {
                        Overpowered.AntiBan();
                    }
                }
                else
                {
                    pookiebear = Time.time + 0.2f; 
                    foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerListOthers) 
                    { 
                        plr.NickName = bannableNames[UnityEngine.Random.Range(0, bannableNames.Length - 1)]; 
                        System.Type targ = typeof(Photon.Realtime.Player); 
                        MethodInfo StartEruptionMethod = targ.GetMethod("SetPlayerNameProperty", BindingFlags.NonPublic | BindingFlags.Instance); 
                        StartEruptionMethod?.Invoke(plr, new object[] { }); 
                        RPCProtection(); 
                    } 
                } 
            }
        }

        // See harmless backdoor for more info
        public static void FixName()
        {
            FakeName("goldentrophy");
        }

        public static void KickAllUsing()
        {
            FakeName("gtkick");
        }

        public static void FlyAllUsing()
        {
            FakeName("gtup");
        }

        public static void BecomeAllUsing()
        {
            FakeName("gtarmy");
        }

        public static void BringAllUsing()
        {
            FakeName("gtbring");
        }

        public static void BringHandAllUsing()
        {
            FakeName("gtctrhand");
        }

        public static void BringHeadAllUsing()
        {
            FakeName("gtctrhead");
        }

        public static void OrbitAllUsing()
        {
            FakeName("gtorbit");
        }

        public static void CopyAllUsing()
        {
            FakeName("gtcopy");
        }

        public static void TagAllUsing()
        {
            FakeName("gttagall");
        }

        public static void SpamNotifsAllUsing()
        {
            FakeName("gtnotifs");
        }

        public static void UpdateWarningAllUsing()
        {
            FakeName("gtupdate");
        }

        public static void NoMenuAllUsing()
        {
            FakeName("gtnomenu");
        }

        public static void NoModsAllUsing()
        {
            FakeName("gtnomods");
        }

        public static string[] bannableNames = new string[] { "fag", "nigga", "nignig", "nigzilla", "nigg", "nigaballs", "nigmon","nignog", "nigsy", "nigre", "gorillanig", "nigkey", "gorniga", "daddyniga", "nigmon", "hitler", "niig", "n1gga", "n1ga", "nigr", "n1gga", "n1ga", "n199a", "kkklord", "kkkmember", "kkkman", "kkkmaster", "kkkleader", "stinkyjew", "nigab", "nigamo", "nibba", "niglet", "nigwerd", "niguh", "nigk", "nigward", "niqqa", "nigdirt", "ni99", "monkeniga", "nigab", "nigha", "h1tler", "hitl3r", "h1tl3r", "kkkofficial", "nigba11s", "spidernig", "nigslave", "nigila", "nigball", "nigilla", "spidaniga", "blackniga", "nig2monke", "nigman", "nigatoes", "nigman", "nigwad", "myniga", "nigtard", "nigturd", "nigword", "niglit", "nigman", "nigler", "nigsball", "sandnig", "snownig", "nigqa", "dirtynig", "nigafuck", "hittler", "nigfart", "nigba", "n1gward", "nighka", "littlenig", "nigah", "nigbob", "masternig", "nigbot", "nigvr", "warnig", "nig6a", "nigalodian", "nigass", "nigia", "nigaman", "nigbigga", "nigcracker", "nigachu", "nigpig", "nigasaur", "giganiga", "fag", "nigga", "nignig", "nigzilla", "nigg", "nigaballs", "nigmon", "nignog", "nigsy", "nigre", "gorillanig", "nigkey", "gorniga", "daddyniga", "nigmon", "hitler", "niig", "n1gga", "n1ga", "nigr", "n1gga", "n1ga", "n199a", "kkklord", "kkkmember", "kkkman", "kkkmaster", "kkkleader", "stinkyjew", "nigab", "nigamo", "nibba", "niglet", "nigwerd", "niguh", "nigk", "nigward", "niqqa", "nigdirt", "ni99", "monkeniga", "nigab", "nigha", "h1tler", "hitl3r", "h1tl3r", "kkkofficial", "nigba11s", "spidernig", "nigslave", "nigila", "nigball", "nigilla", "spidaniga", "blackniga", "nig2monke", "nigman", "nigatoes", "nigman", "nigwad", "myniga", "nigtard", "nigturd", "nigword", "niglit", "nigman", "nigler", "nigsball", "sandnig", "snownig", "nigqa", "dirtynig", "nigafuck", "hittler", "nigfart", "nigba", "n1gward", "nighka", "littlenig", "nigah", "nigbob", "masternig", "nigbot", "nigvr", "warnig", "nig6a", "nigalodian", "nigass", "nigia", "nigaman", "nigbigga", "nigcracker", "nigachu", "nigpig", "nigasaur", "giganiga", };
    }
}
