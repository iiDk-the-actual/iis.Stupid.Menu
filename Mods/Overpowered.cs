using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using static GorillaNetworking.CosmeticsController;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.Spammers.Projectiles;
using GorillaGameModes;

namespace iiMenu.Mods
{
    internal class Overpowered
    {
        public static float lastTime = -1f;
        public static float gamemodeSetTimeAt = -1f;
        public static bool antibanworked = false;

        public static void AntiBan()
        {
            /*if (PhotonNetwork.InRoom)
            {
                if (!IsModded())
                {
                    if (!PhotonNetwork.CurrentRoom.IsOpen)
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ANTIBAN</color><color=grey>]</color> <color=white>Anti ban has already been used in this code.</color>");
                        return;
                    }
                    string gamemode = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace(GorillaComputer.instance.currentGameMode.Value, "MODDED_MODDED" + GorillaComputer.instance.currentGameMode.Value);
                    ExitGames.Client.Photon.Hashtable gamehash = new ExitGames.Client.Photon.Hashtable
                {
                    { "gameMode", gamemode }
                };
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTIBAN</color><color=grey>]</color> <color=white>Setting master client...</color>");
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;

                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTIBAN</color><color=grey>]</color> <color=white>Setting gamemode...</color>");
                    PhotonNetwork.CurrentRoom.SetCustomProperties(gamehash, null, null);

                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
                    {
                        FunctionName = "RoomClosed",
                        FunctionParameter = new
                        {
                            GameId = PhotonNetwork.CurrentRoom.Name,
                            Region = Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper(),
                            ActorNr = PhotonNetwork.LocalPlayer.ActorNumber,
                            ActorCount = 0,
                            UserId = PhotonNetwork.LocalPlayer.UserId,
                            AppVersion = PhotonNetwork.AppVersion,
                            AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime,
                            Type = "Close"
                        }
                    },
                    delegate (ExecuteCloudScriptResult result)
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTIBAN</color><color=grey>]</color> <color=white>Anti ban has been executed successfully. I take no responsibility for any bans using this mod.</color>");
                    }, error => { NotifiLib.SendNotification("<color=grey>[</color><color=red>ANTIBAN</color><color=grey>]</color> <color=white>Anti ban has failed to execute, you have been disconnected to prevent any bans."); PhotonNetwork.Disconnect(); }, null, null);

                }
                else
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ANTIBAN</color><color=grey>]</color> <color=white>Anti ban has already been used in this lobby.</color>");
                }
                GetIndex("Anti Ban").enabled = false;
                ReloadMenu();
            } else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ANTIBAN</color><color=grey>]</color> <color=white>You are not in a lobby.</color>");
            }*/
            GetIndex("Anti Ban").enabled = false;
            ReloadMenu();
            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>This mod has been disabled due to security.</color>"); 
        }

        public static bool IsModded()
        {
            return false;// (PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")/* && Time.time > gamemodeSetTimeAt*/);
        }

        public static void FastMaster()
        {
            /*if (!IsModded() || !PhotonNetwork.InRoom)
            {
                GetIndex("Anti Ban").enabled = true;
                //AntiBan();
            }
            else
            {
                GetIndex("Set Master").enabled = false;
                ReloadMenu();
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>This mod has been disabled due to security.</color>");
                //PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            }*/
            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
        }

        public static void AntiBanCheck()
        {
            if (IsModded())
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>The anti ban is enabled!</color>");
            } else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>The anti ban is disabled!</color>");
            }
        }

        public static void MasterCheck()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You are master client!</color>");
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client!</color>");
            }
        }

        public static void SetMasterGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player owner = GetPlayerFromVRRig(possibly);
                        if (!IsModded())
                        {
                            if (!GetIndex("Disable Auto Anti Ban").enabled)
                            {
                                AntiBan();
                            }
                        }
                        else
                        {
                            GetIndex("Set Master Gum").enabled = false;
                            ReloadMenu();
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>This mod has been disabled due to security.</color>");
                            //PhotonNetwork.CurrentRoom.SetMasterClient(owner);
                        }
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        /*
        public static void ForceEruptLava()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                InfectionLavaController controller = InfectionLavaController.Instance;
                System.Type type = controller.GetType();

                FieldInfo fieldInfo = type.GetField("reliableState", BindingFlags.NonPublic | BindingFlags.Instance);

                object reliableState = fieldInfo.GetValue(controller);

                FieldInfo stateFieldInfo = reliableState.GetType().GetField("state");
                stateFieldInfo.SetValue(reliableState, InfectionLavaController.RisingLavaState.Erupting);

                FieldInfo stateFieldInfo2 = reliableState.GetType().GetField("stateStartTime");
                stateFieldInfo2.SetValue(reliableState, PhotonNetwork.Time);

                fieldInfo.SetValue(controller, reliableState);
            }
        }

        public static void ForceUneruptLava()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                InfectionLavaController controller = InfectionLavaController.Instance;
                System.Type type = controller.GetType();

                FieldInfo fieldInfo = type.GetField("reliableState", BindingFlags.NonPublic | BindingFlags.Instance);

                object reliableState = fieldInfo.GetValue(controller);

                FieldInfo stateFieldInfo = reliableState.GetType().GetField("state");
                stateFieldInfo.SetValue(reliableState, InfectionLavaController.RisingLavaState.Draining);

                FieldInfo stateFieldInfo2 = reliableState.GetType().GetField("stateStartTime");
                stateFieldInfo2.SetValue(reliableState, PhotonNetwork.Time);

                fieldInfo.SetValue(controller, reliableState);
            }
        }

        public static void ForceRiseLava()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                InfectionLavaController controller = InfectionLavaController.Instance;
                System.Type type = controller.GetType();

                FieldInfo fieldInfo = type.GetField("reliableState", BindingFlags.NonPublic | BindingFlags.Instance);

                object reliableState = fieldInfo.GetValue(controller);

                FieldInfo stateFieldInfo = reliableState.GetType().GetField("state");
                stateFieldInfo.SetValue(reliableState, InfectionLavaController.RisingLavaState.Full);

                FieldInfo stateFieldInfo2 = reliableState.GetType().GetField("stateStartTime");
                stateFieldInfo2.SetValue(reliableState, PhotonNetwork.Time);

                fieldInfo.SetValue(controller, reliableState);
            }
        }

        public static void ForceDrainLava()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                InfectionLavaController controller = InfectionLavaController.Instance;
                System.Type type = controller.GetType();

                FieldInfo fieldInfo = type.GetField("reliableState", BindingFlags.NonPublic | BindingFlags.Instance);

                object reliableState = fieldInfo.GetValue(controller);

                FieldInfo stateFieldInfo = reliableState.GetType().GetField("state");
                stateFieldInfo.SetValue(reliableState, InfectionLavaController.RisingLavaState.Drained);

                FieldInfo stateFieldInfo2 = reliableState.GetType().GetField("stateStartTime");
                stateFieldInfo2.SetValue(reliableState, PhotonNetwork.Time);

                fieldInfo.SetValue(controller, reliableState);
            }
        }

        public static void SpazLava()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                InfectionLavaController controller = InfectionLavaController.Instance;
                System.Type type = controller.GetType();

                FieldInfo fieldInfo = type.GetField("reliableState", BindingFlags.NonPublic | BindingFlags.Instance);

                object reliableState = fieldInfo.GetValue(controller);

                FieldInfo stateFieldInfo = reliableState.GetType().GetField("state");
                if (spazLavaType)
                {
                    stateFieldInfo.SetValue(reliableState, InfectionLavaController.RisingLavaState.Full);
                }
                else
                {
                    stateFieldInfo.SetValue(reliableState, InfectionLavaController.RisingLavaState.Drained);
                }
                spazLavaType = !spazLavaType;

                FieldInfo stateFieldInfo2 = reliableState.GetType().GetField("stateStartTime");
                stateFieldInfo2.SetValue(reliableState, PhotonNetwork.Time + UnityEngine.Random.Range(0f, 20f));

                fieldInfo.SetValue(controller, reliableState);
            }
        }*/

        public static void SpazTargets()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                System.Type targ = typeof(HitTargetScoreDisplay);
                MethodInfo StartEruptionMethod = targ.GetMethod("OnScoreChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                HitTargetScoreDisplay[] vs = GameObject.FindObjectsOfType<HitTargetScoreDisplay>();
                HitTargetScoreDisplay v = vs[UnityEngine.Random.Range(0, vs.Length - 1)];
                v.rotateSpeed = 9999;
                StartEruptionMethod?.Invoke(v, new object[] { Random.Range(0, 999) });
            }
        }

        public static void SpawnSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
        }

        public static void AngerSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated)
            {
                secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            }
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemotePlayerSeen", RpcTarget.All, new object[] { });
        }

        public static void ThrowSecondLook()
        {
            GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
            secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
            if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated)
            {
                secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
            }
            secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemotePlayerCaught", RpcTarget.All, new object[] { });
        }

        public static float lasttimeaa = 0f;
        public static void SpazSecondLook()
        {
            if (Time.time > lasttimeaa)
            {
                lasttimeaa = Time.time + 0.75f;
                GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
                secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Unactivated || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.PlayerThrown || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Reset)
                {
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
                }
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Activated || secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Patrolling)
                {
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemotePlayerSeen", RpcTarget.All, new object[] { });
                }
                if (secondlook.GetComponent<SecondLookSkeleton>().currentState == SecondLookSkeleton.GhostState.Chasing)
                {
                    secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.RPC("RemoteActivateGhost", RpcTarget.All, new object[] { });
                }
            }
        }

        /*
        public static void GrabSecondLookGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject secondlook = GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton");
                        secondlook.GetComponent<SecondLookSkeleton>().tapped = true;
                        secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                        secondlook.GetComponent<SecondLookSkeletonSynchValues>().photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                        secondlook.GetComponent<SecondLookSkeleton>().currentState = SecondLookSkeleton.GhostState.CaughtPlayer;
                        secondlook.GetComponent<SecondLookSkeleton>().spookyGhost.transform.position = possibly.transform.position;
                    }
                }
            }
        }*/

        public static void StepCrashMethod(RpcTarget who)
        {
            /*
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            Vector3 crashPos = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom").transform.position;
            GorillaTagger.Instance.offlineVRRig.transform.position = crashPos;
            try
            {
                GorillaTagger.Instance.myVRRig.transform.position = crashPos;
            }
            catch { }

            string[] items = new string[]
            {
                "HEADPHONES1",
                "CHEFS HAT",
                "NECK SCARF",
                "FEZ",
                "COWBOY HAT",
                "SUNHAT",
                "BASIC BEANIE"
            };
            CosmeticItem flowers = GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/ShoppingCenterAnchor/Stuff/Stand1Anchor/" + items[UnityEngine.Random.Range(0, items.Length - 1)]).GetComponent<CosmeticStand>().thisCosmeticItem;
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, flowers, false, false);

            items = new string[]
            {
                "PARTY HAT",
                "BASIC SCARF",
                "SWEATBAND",
                "BASEBALL CAP",
                "USHANKA"
            };
            flowers = GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/ShoppingCenterAnchor/Stuff/Stand2Anchor/" + items[UnityEngine.Random.Range(0, items.Length - 1)]).GetComponent<CosmeticStand>().thisCosmeticItem;
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, flowers, false, false);

            items = new string[]
            {
                "BOWTIE",
                "TOP HAT",
                "FLOWER CROWN",
                "WHITE FEDORA",
                "CLOCHE"
            };
            flowers = GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/ShoppingCenterAnchor/Stuff/Stand3Anchor/" + items[UnityEngine.Random.Range(0, items.Length - 1)]).GetComponent<CosmeticStand>().thisCosmeticItem;
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, flowers, false, false);

            CosmeticsController.instance.UpdateShoppingCart();
            GorillaTagger.Instance.offlineVRRig.inTryOnRoom = true;
            GorillaTagger.Instance.myVRRig.RPC("UpdateCosmeticsWithTryon", who, CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray());
            RPCProtection();*/
            ReloadMenu();
            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>This mod has been disabled due to security.</color>");
        }
        public static void StepCrashMethod(Player who)
        {
            ReloadMenu();
            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>This mod has been disabled due to security.</color>");
            /*
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            Vector3 crashPos = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom").transform.position;
            GorillaTagger.Instance.offlineVRRig.transform.position = crashPos;
            try
            {
                GorillaTagger.Instance.myVRRig.transform.position = crashPos;
            }
            catch { }

            string[] items = new string[]
            {
                "HEADPHONES1",
                "CHEFS HAT",
                "NECK SCARF",
                "FEZ",
                "COWBOY HAT",
                "SUNHAT",
                "BASIC BEANIE"
            };
            CosmeticItem flowers = GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/ShoppingCenterAnchor/Stuff/Stand1Anchor/" + items[UnityEngine.Random.Range(0, items.Length - 1)]).GetComponent<CosmeticStand>().thisCosmeticItem;
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, flowers, false, false);

            items = new string[]
            {
                "PARTY HAT",
                "BASIC SCARF",
                "SWEATBAND",
                "BASEBALL CAP",
                "USHANKA"
            };
            flowers = GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/ShoppingCenterAnchor/Stuff/Stand2Anchor/" + items[UnityEngine.Random.Range(0, items.Length - 1)]).GetComponent<CosmeticStand>().thisCosmeticItem;
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, flowers, false, false);

            items = new string[]
            {
                "BOWTIE",
                "TOP HAT",
                "FLOWER CROWN",
                "WHITE FEDORA",
                "CLOCHE"
            };
            flowers = GameObject.Find("Environment Objects/LocalObjects_Prefab/City/CosmeticsRoomAnchor/ShoppingCenterAnchor/Stuff/Stand3Anchor/" + items[UnityEngine.Random.Range(0, items.Length - 1)]).GetComponent<CosmeticStand>().thisCosmeticItem;
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, flowers, false, false);

            CosmeticsController.instance.UpdateShoppingCart();
            GorillaTagger.Instance.offlineVRRig.inTryOnRoom = true;
            GorillaTagger.Instance.myVRRig.RPC("UpdateCosmeticsWithTryon", who, CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray());
            RPCProtection();
            */
        }

        public static void LagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((isCopying && whoCopy != null) && Time.time > kgDebounce)
                {
                    kgDebounce = Time.time + 0.25f;
                    StepCrashMethod(GetPlayerFromVRRig(whoCopy));
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
            if ((rightTrigger > 0.5f) && Time.time > kgDebounce)
            {
                kgDebounce = Time.time + 0.25f;
                StepCrashMethod(RpcTarget.Others);
            } else
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
                    StepCrashMethod(GetPlayerFromVRRig(whoCopy));
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
                StepCrashMethod(RpcTarget.Others);
            } else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        /*
        public static void RigSpamTest() // This doesnt work
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
                    Hashtable SendInstantiateEvHashtable = new Hashtable();
                    SendInstantiateEvHashtable[(byte)0] = "Player Objects/Local VRRig/Local Gorilla Player";
                    SendInstantiateEvHashtable[(byte)1] = GorillaTagger.Instance.bodyCollider.transform.position;
                    SendInstantiateEvHashtable[(byte)2] = GorillaTagger.Instance.bodyCollider.transform.rotation;
                    SendInstantiateEvHashtable[(byte)3] = (byte)0;
                    SendInstantiateEvHashtable[(byte)4] = null;
                    SendInstantiateEvHashtable[(byte)5] = null;
                    SendInstantiateEvHashtable[(byte)6] = PhotonNetwork.ServerTimestamp;
                    SendInstantiateEvHashtable[(byte)7] = PhotonNetwork.AllocateViewID(PhotonNetwork.LocalPlayer.ActorNumber);
                    SendInstantiateEvHashtable[(byte)8] = (byte)0;
                    RaiseEventOptions SendInstantiateRaiseEventOptions = new RaiseEventOptions();
                    SendInstantiateRaiseEventOptions.CachingOption = (true ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache);
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(202, SendInstantiateEvHashtable, SendInstantiateRaiseEventOptions, SendOptions.SendReliable);
                }
            }
        }*/

        public static void AcidSelf()
        {
            if (!IsModded())
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(PhotonNetwork.CurrentRoom.PlayerCount);
                ScienceExperimentManager.PlayerGameState[] states = new ScienceExperimentManager.PlayerGameState[10];
                int ownerIndex = states.Length > PhotonNetwork.LocalPlayer.ActorNumber ? PhotonNetwork.LocalPlayer.ActorNumber : 0;
                states[ownerIndex].touchedLiquid = true;
                states[ownerIndex].playerId = PhotonNetwork.LocalPlayer.ActorNumber;
                Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(states);
            }
        }

        public static void AcidGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player player = GetPlayerFromVRRig(possibly);
                        if (!IsModded())
                        {
                            if (!GetIndex("Disable Auto Anti Ban").enabled)
                            {
                                AntiBan();
                            }
                        }
                        else
                        {
                            Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(PhotonNetwork.CurrentRoom.PlayerCount);
                            ScienceExperimentManager.PlayerGameState[] states = new ScienceExperimentManager.PlayerGameState[10];
                            int ownerIndex = states.Length > player.ActorNumber ? player.ActorNumber : 0;
                            states[ownerIndex].touchedLiquid = true;
                            states[ownerIndex].playerId = player.ActorNumber;
                            Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(states);
                            RPCProtection();
                            kgDebounce = Time.time + 0.2f;
                        }
                    }
                }
            }
        }

        public static void AcidAll()
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
                Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(PhotonNetwork.CurrentRoom.PlayerCount);
                ScienceExperimentManager.PlayerGameState[] states = new ScienceExperimentManager.PlayerGameState[10];
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    states[i].touchedLiquid = true;
                    states[i].playerId = PhotonNetwork.PlayerList[i] == null ? 0 : PhotonNetwork.PlayerList[i].ActorNumber;
                }
                Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(states);
                RPCProtection();
            }
        }

        public static void InfectionToTag()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                gorillaTagManager.SetisCurrentlyTag(true);
                gorillaTagManager.ClearInfectionState();
                gorillaTagManager.ChangeCurrentIt(GameMode.ParticipatingPlayers[UnityEngine.Random.Range(0, GameMode.ParticipatingPlayers.Count)]);
            }
        }

        public static void TagToInfection()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                gorillaTagManager.SetisCurrentlyTag(false);
                gorillaTagManager.ClearInfectionState();
                Player victim = GameMode.ParticipatingPlayers[UnityEngine.Random.Range(0, GameMode.ParticipatingPlayers.Count)];
                gorillaTagManager.AddInfectedPlayer(victim);
                gorillaTagManager.lastInfectedPlayer = victim;
            }
        }

        public static void BetaSetStatus(int state, RaiseEventOptions balls)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    FastMaster();
                }
            }
            else
            {
                object[] statusSendData = new object[1];
                statusSendData[0] = state;
                object[] sendEventData = new object[3];
                sendEventData[0] = PhotonNetwork.ServerTimestamp;
                sendEventData[1] = (byte)2;
                sendEventData[2] = statusSendData;
                PhotonNetwork.RaiseEvent(3, sendEventData, balls, SendOptions.SendUnreliable);
            }
        }

        public static void InfectionGamemode()
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
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_INFECTION");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable);
            }
        }

        public static void CasualGamemode()
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
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_CASUALCASUAL");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable);
            }
        }

        public static void HuntGamemode()
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
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_HUNTHUNT");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable);
            }
        }

        public static void BattleGamemode()
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
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_BATTLEPAINTBRAWL");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable);
            }
        }

        public static void SSDisableNetworkTriggers()
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
                hashtable.Add("gameMode", "forestcitybasementcanyonsmountainsbeachskycavesrotational" + PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString());
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable);
            }
        }

        public static void TrapStump()
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
                string name = "";
                foreach (char character in PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString())
                {
                    if (!char.IsLower(character))
                    {
                        name += character;
                    }
                }
                hashtable.Add("gameMode", name);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetCustomPropertiesOfRoom(hashtable);
            }
        }

        public static void MakeRoomPrivate()
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
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
        }

        public static void MakeRoomPublic()
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
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }

        public static void SlowGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player player = GetPlayerFromVRRig(possibly);
                        //GorillaGameManager.instance.FindVRRigForPlayer(player).RPC("SetTaggedTime", player, null);
                        BetaSetStatus(0, new RaiseEventOptions { TargetActors = new int[1] { player.ActorNumber } });
                        RPCProtection();
                        kgDebounce = Time.time + 1f;
                    }
                }
            }
        }

        public static void SlowAll()
        {
            if (Time.time > kgDebounce)
            {
                /*foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                GorillaGameManager.instance.FindVRRigForPlayer(player).RPC("SetTaggedTime", player, null);*/
                BetaSetStatus(0, new RaiseEventOptions { Receivers = ReceiverGroup.Others });
                RPCProtection();
                kgDebounce = Time.time + 1f;
                //}
            }
        }

        public static void VibrateGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player owner = GetPlayerFromVRRig(possibly);
                        //GorillaTagger.Instance.myVRRig.RPC("SetJoinTaggedTime", owner, null);
                        BetaSetStatus(1, new RaiseEventOptions { TargetActors = new int[1] { owner.ActorNumber } });
                        RPCProtection();
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void VibrateAll()
        {
            if (Time.time > kgDebounce)
            {
                /*foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                GorillaTagger.Instance.myVRRig.RPC("SetJoinTaggedTime", player, null);*/
                BetaSetStatus(1, new RaiseEventOptions { Receivers = ReceiverGroup.Others });
                RPCProtection();
                kgDebounce = Time.time + 0.5f;
                //}
            }
        }
        
        public static void BlindGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }

                if (isCopying)
                {
                    Vector3 startpos = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * 0.5f);
                    Vector3 charvel = Vector3.zero;

                    BetaFireProjectile("WaterBalloon", startpos, charvel, new Color32(0, 0, 0, 255));
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

        public static void BlindAll()
        {
            VRRig randomRig = GetRandomVRRig(false);

            Vector3 startpos = randomRig.headMesh.transform.position + (randomRig.headMesh.transform.forward * 0.5f);
            Vector3 charvel = Vector3.zero;

            BetaFireProjectile("WaterBalloon", startpos, charvel, new Color32(0, 0, 0, 255));
        }

        public static void GliderBlindGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }

                if (isCopying)
                {
                    foreach (GliderHoldable glider in GetGliders())
                    {
                        if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            glider.gameObject.transform.position = whoCopy.headMesh.transform.position;
                            glider.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                        }
                        else
                        {
                            glider.OnHover(null, null);
                        }
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

        public static void GliderBlindAll()
        {
            GliderHoldable[] those = GetGliders();
            int index = 0;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    try
                    {
                        GliderHoldable glider = those[index];
                        if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            glider.gameObject.transform.position = vrrig.headMesh.transform.position;
                            glider.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                        }
                        else
                        {
                            glider.OnHover(null, null);
                        }
                    } catch { }
                    index++;
                }
            }
        }

        // Revert kick mods to 3.4.2
        public static void KickGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player owner = GetPlayerFromVRRig(possibly);
                        if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(owner.UserId) && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                        {
                            PhotonNetworkController.Instance.friendIDList = new List<string>(GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching);
                            PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                            PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');

                            object[] groupJoinSendData = new object[2];
                            groupJoinSendData[0] = PhotonNetworkController.Instance.shuffler;
                            groupJoinSendData[1] = PhotonNetworkController.Instance.keyStr;
                            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                            {
                                TargetActors = new int[1] { owner.ActorNumber }
                            };

                            object obj = groupJoinSendData;
                            object[] sendEventData = new object[3];
                            sendEventData[0] = PhotonNetwork.ServerTimestamp;
                            sendEventData[1] = (byte)4;
                            sendEventData[2] = groupJoinSendData;
                            PhotonNetwork.RaiseEvent(3, sendEventData, raiseEventOptions, SendOptions.SendUnreliable);
                            RPCProtection();
                        }
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void KickAll()
        {
            if (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible)
            {
                PhotonNetworkController.Instance.friendIDList = new List<string>(GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching);
                PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
                    {
                        object[] groupJoinSendData = new object[2];
                        groupJoinSendData[0] = PhotonNetworkController.Instance.shuffler;
                        groupJoinSendData[1] = PhotonNetworkController.Instance.keyStr;
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                        {
                            TargetActors = new int[1] { player.ActorNumber }
                        };

                        object obj = groupJoinSendData;
                        object[] sendEventData = new object[3];
                        sendEventData[0] = PhotonNetwork.ServerTimestamp;
                        sendEventData[1] = (byte)4;
                        sendEventData[2] = groupJoinSendData;
                        PhotonNetwork.RaiseEvent(3, sendEventData, raiseEventOptions, SendOptions.SendUnreliable);
                        RPCProtection();
                    }
                }
                PhotonNetwork.SendAllOutgoingCommands();
                RPCProtection();
            }
        }

        /*
        public static void FlingGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        foreach (GliderHoldable glider in GetGliders())
                        {
                            FieldInfo SyncedStateField = typeof(GliderHoldable).GetField("syncedState", BindingFlags.NonPublic | BindingFlags.Instance);
                            object SyncedStateValue = SyncedStateField.GetValue(glider);

                            FieldInfo RiderIdField = SyncedStateValue.GetType().GetField("riderId", BindingFlags.Public | BindingFlags.Instance);
                            RiderIdField.SetValue(SyncedStateValue, GetPlayerFromVRRig(possibly).ActorNumber);

                            SyncedStateField.SetValue(glider, SyncedStateValue);

                            FieldInfo RigidField = typeof(GliderHoldable).GetField("rb", BindingFlags.NonPublic | BindingFlags.Instance);
                            Rigidbody rb = (Rigidbody)RigidField.GetValue(glider);

                            rb.isKinematic = false;
                            rb.velocity = new Vector3(0f, 100f, 0f);

                            RPCProtection();
                        }
                    }
                }
            }
        }
        */

        public static void DestroyGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
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
                            Photon.Realtime.Player player = GetPlayerFromVRRig(possibly);
                            PhotonNetwork.CurrentRoom.StorePlayer(player);
                            PhotonNetwork.CurrentRoom.Players.Remove(player.ActorNumber);
                            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
                            kgDebounce = Time.time + 0.5f;
                        }
                    }
                }
            }
        }

        public static void DestroyAll()
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
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                {
                    PhotonNetwork.CurrentRoom.StorePlayer(player);
                    PhotonNetwork.CurrentRoom.Players.Remove(player.ActorNumber);
                    PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
                }
            }
        }

        public static void BreakAudioGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", GetPlayerFromVRRig(whoCopy), new object[]{
                        111,
                        false,
                        999999f
                        //0.01f
                    });
                    RPCProtection();
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

        public static void BreakAudioAll()
        {
            if (rightTrigger > 0.5f)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
                    111,
                    false,
                    999999f
                });
            }
        }
    }
}
