using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.Overpowered;

namespace iiMenu.Mods
{
    internal class Experimental
    {
        public static void LagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (!IsModded())
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            AntiBan();
                        }
                    }
                    else
                    {
                        int num = RigManager.GetPhotonViewFromVRRig(whoCopy).ViewID;
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

        public static void LagAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (!IsModded())
                {
                    if (!GetIndex("Disable Auto Anti Ban").enabled)
                    {
                        AntiBan();
                    }
                }
                else
                {
                    int num = RigManager.GetPhotonViewFromVRRig(RigManager.GetRandomVRRig(false)).ViewID;
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
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void CrashGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (!IsModded())
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            AntiBan();
                        }
                    }
                    else
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable[(byte)0] = RigManager.GetPlayerFromVRRig(whoCopy).ActorNumber;
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(207, hashtable, null, SendOptions.SendReliable);
                        RPCProtection();
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

        public static void CrashAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (!IsModded())
                {
                    if (!GetIndex("Disable Auto Anti Ban").enabled)
                    {
                        AntiBan();
                    }
                }
                else
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable[(byte)0] = -1;
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(207, hashtable, null, SendOptions.SendReliable);
                    RPCProtection();
                }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

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
            if (PhotonNetwork.InRoom && Overpowered.IsModded())
            {
                GetIndex("Auto Set Master").enabled = false;
                ReloadMenu();
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>This mod has been disabled due to security.</color>");
                //PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            }
        }

        /*
        public static void InfiniteRangeTagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

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
        }*/

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
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

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
    }
}
