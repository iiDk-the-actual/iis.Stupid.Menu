using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using HarmonyLib;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.Spammers.Projectiles;

namespace iiMenu.Mods
{
    internal class Overpowered
    {
        /*public static void AntiBan() // patched ?
        {
            object obj;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
            if (!obj.ToString().Contains("MODDED") && PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsConnected)
            {
                ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
                executeCloudScriptRequest.FunctionName = "RoomClosed";
                executeCloudScriptRequest.FunctionParameter = new
                {
                    GameId = PhotonNetwork.CurrentRoom.Name,
                    Region = Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper(),
                    UserId = PhotonNetwork.MasterClient.UserId,
                    ActorNr = 1,
                    ActorCount = 1,
                    AppVersion = PhotonNetwork.AppVersion
                };
                PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate (ExecuteCloudScriptResult result)
                {
                }, null, null, null);
                string gamemode = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace(GorillaComputer.instance.currentQueue, GorillaComputer.instance.currentQueue + "MODDED_MODDED_");
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
                {
                    { "gameMode", gamemode }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }
        }*/

        public static void AntiBan()
        {
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED"))
            {
                PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest
                {
                    FunctionName = "RoomClosed",
                    FunctionParameter = new
                    {
                        GameId = PhotonNetwork.CurrentRoom.Name,
                        Region = Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper(),
                        UserId = PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length + 1)].UserId,
                        ActorNr = PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length + 1)],
                        ActorCount = PhotonNetwork.ViewCount,
                        AppVersion = PhotonNetwork.AppVersion
                    },
                }, result =>
                {
                    hasAntiBanned = true;
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTIBAN</color><color=grey>]</color> <color=white>The anti ban has been enabled successfully. All detected mods are undetected.</color>");
                }, null);
                string gamemode = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Replace(GorillaComputer.instance.currentQueue, GorillaComputer.instance.currentQueue + "MODDEDMODDED");
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
                {
                    { "gameMode", gamemode }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED") && !PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            }
        }

        public static void ForceEruptLava()
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

        public static void ForceUneruptLava()
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

        public static void ForceRiseLava()
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

        public static void ForceDrainLava()
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

        public static void SpazLava()
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
            stateFieldInfo2.SetValue(reliableState, PhotonNetwork.Time + UnityEngine.Random.Range(0f,20f));

            fieldInfo.SetValue(controller, reliableState);
        }

        public static void SpazTargets()
        {
            System.Type targ = typeof(HitTargetScoreDisplay);
            MethodInfo StartEruptionMethod = targ.GetMethod("OnScoreChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            HitTargetScoreDisplay[] vs = GameObject.FindObjectsOfType<HitTargetScoreDisplay>();
            HitTargetScoreDisplay v = vs[UnityEngine.Random.Range(0, vs.Length - 1)];
            v.rotateSpeed = 9999;
            StartEruptionMethod?.Invoke(v, new object[] { Random.Range(0,999) });
        }

        public static void BubbleGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
                if (isCopying && whoCopy != null)
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        ScienceExperimentManager.instance.photonView.RPC("SpawnSodaBubbleRPC", GetPlayerFromVRRig(whoCopy), new object[]
                        {
                            new Vector2(0.3f, 0.3f),
                            2f,
                            10f,
                            PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)
                        });
                        RPCProtection();
                    } else
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
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
                }
            }
        }

        public static void BubbleAll()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                ScienceExperimentManager.instance.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[]
                {
                    new Vector2(0.3f, 0.3f),
                    2f,
                    10f,
                    PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)
                });
                RPCProtection();
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void LagGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
                if ((isCopying && whoCopy != null) && Time.time > kgDebounce)
                {
                    int num = GetPhotonViewFromVRRig(whoCopy).ViewID;
                    Hashtable ServerCleanDestroyEvent = new Hashtable();
                    RaiseEventOptions ServerCleanOptions = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.RemoveFromRoomCache
                    };
                    ServerCleanDestroyEvent[0] = num;
                    ServerCleanOptions.CachingOption = EventCaching.DoNotCache;
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, ServerCleanDestroyEvent, ServerCleanOptions, SendOptions.SendUnreliable);
                    kgDebounce = Time.time + 0.2f;
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
                }
            }
        }

        public static void LagAll()
        {
            if ((rightTrigger > 0.5f) && Time.time > kgDebounce)
            {
                int num = GetPhotonViewFromVRRig(GetVRRigFromPlayer(PhotonNetwork.PlayerListOthers[UnityEngine.Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)])).ViewID;
                Hashtable ServerCleanDestroyEvent = new Hashtable();
                RaiseEventOptions ServerCleanOptions = new RaiseEventOptions
                {
                    CachingOption = EventCaching.RemoveFromRoomCache
                };
                ServerCleanDestroyEvent[0] = num;
                ServerCleanOptions.CachingOption = EventCaching.DoNotCache;
                PhotonNetwork.NetworkingClient.OpRaiseEvent(204, ServerCleanDestroyEvent, ServerCleanOptions, SendOptions.SendUnreliable);
                kgDebounce = Time.time + 0.2f;
            }
        }

        public static void CrashGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
                if (isCopying && whoCopy != null)
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        ScienceExperimentManager.instance.photonView.RPC("SpawnSodaBubbleRPC", GetPlayerFromVRRig(whoCopy), new object[]
                        {
                            Random.insideUnitCircle.normalized + new Vector2(0.5f, 2f),
                            float.MaxValue,
                            float.MaxValue,
                            PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)
                        });
                        RPCProtection();
                    }
                    else
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
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
                }
            }
        }

        public static void CrashAll()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                ScienceExperimentManager.instance.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, new object[]
                {
                    Random.insideUnitCircle.normalized + new Vector2(0.5f, 2f),
                    float.MaxValue,
                    float.MaxValue,
                    PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)
                });
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void AcidSelf()
        {
            Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(1);
            ScienceExperimentManager.PlayerGameState[] states = new ScienceExperimentManager.PlayerGameState[1];
            int ownerIndex = states.Length > PhotonNetwork.LocalPlayer.ActorNumber ? PhotonNetwork.LocalPlayer.ActorNumber : 0;
            states[ownerIndex].touchedLiquid = true;
            states[ownerIndex].playerId = PhotonNetwork.LocalPlayer.ActorNumber;
            Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(states);
        }

        public static void AcidGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player player = GetPlayerFromVRRig(possibly);
                        // Not created by me, leaked by REV
                        Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(1);
                        ScienceExperimentManager.PlayerGameState[] states = new ScienceExperimentManager.PlayerGameState[1];
                        int ownerIndex = states.Length > player.ActorNumber ? player.ActorNumber : 0;
                        states[ownerIndex].touchedLiquid = true;
                        states[ownerIndex].playerId = player.ActorNumber;
                        Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(states);
                        RPCProtection();
                        kgDebounce = Time.time + 1f;
                    }
                }
            }
        }

        public static void AcidAll()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                // Not created by me, leaked by REV
                Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerCount").SetValue(10);
                ScienceExperimentManager.PlayerGameState[] states = new ScienceExperimentManager.PlayerGameState[10];
                for (int i = 0; i < 10; i++)
                {
                    states[i].touchedLiquid = true;
                    states[i].playerId = PhotonNetwork.PlayerList[i] == null ? 0 : PhotonNetwork.PlayerList[i].ActorNumber;
                }
                Traverse.Create(ScienceExperimentManager.instance).Field("inGamePlayerStates").SetValue(states);
                RPCProtection();
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BetaSetStatus(int state, RaiseEventOptions balls)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                object[] statusSendData = new object[1];
                statusSendData[0] = state;
                object[] sendEventData = new object[3];
                sendEventData[0] = PhotonNetwork.ServerTimestamp;
                sendEventData[1] = (byte)2;
                sendEventData[2] = statusSendData;
                PhotonNetwork.RaiseEvent(3, sendEventData, balls, SendOptions.SendUnreliable);
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void InfectionGamemode()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_INFECTION");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
            } else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void CasualGamemode()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_CASUALCASUAL");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void HuntGamemode()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_HUNTHUNT");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleGamemode()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_BATTLEPAINTBRAWL");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void SSDisableNetworkTriggers()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("gameMode", "forestcitybasementcanyonsmountainsbeachskycaves" + PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString());
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void TrapStump()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
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
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void SlowGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
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
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
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
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
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

                    BetaFireProjectile("WaterBalloonProjectile", startpos, charvel, new Color32(0, 0, 0, 255));
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

            BetaFireProjectile("WaterBalloonProjectile", startpos, charvel, new Color32(0, 0, 0, 255));
        }
        
        public static void KickGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
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
        public static void CrashGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
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
                    Vector3 lagPosition = new Vector3(0f, 0f, 0f);

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = lagPosition;
                    GorillaTagger.Instance.myVRRig.transform.position = lagPosition;

                    GameObject l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(l.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

                    l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    l.transform.position = GorillaTagger.Instance.leftHandTransform.position;

                    GameObject r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(r.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(r.GetComponent<SphereCollider>());

                    r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    r.transform.position = GorillaTagger.Instance.rightHandTransform.position;

                    l.GetComponent<Renderer>().material.color = bgColorA;
                    r.GetComponent<Renderer>().material.color = bgColorA;

                    UnityEngine.Object.Destroy(l, Time.deltaTime);
                    UnityEngine.Object.Destroy(r, Time.deltaTime);

                    string[] fullProjectileNames = new string[]
                    {
                        "SlingshotProjectile",
                        "SnowballProjectile",
                        "WaterBalloonProjectile",
                        "LavaRockProjectile",
                        "HornsSlingshotProjectile_PrefabV",
                        "CloudSlingshot_Projectile",
                        "CupidArrow_Projectile",
                        "IceSlingshotProjectile_PrefabV Variant",
                        "ElfBow_Projectile",
                        "MoltenRockSlingshot_Projectile",
                        "SpiderBowProjectile Variant",
                        "BucketGift_Cane_Projectile Variant",
                        "BucketGift_Coal_Projectile Variant",
                        "BucketGift_Roll_Projectile Variant",
                        "BucketGift_Round_Projectile Variant",
                        "BucketGift_Square_Projectile Variant"
                    };

                    string[] fullTrailNames = new string[]
                    {
                        "SlingshotProjectileTrail",
                        "HornsSlingshotProjectileTrail_PrefabV",
                        "CloudSlingshot_ProjectileTrailFX",
                        "CupidArrow_ProjectileTrailFX",
                        "IceSlingshotProjectileTrail Variant",
                        "ElfBow_ProjectileTrail",
                        "MoltenRockSlingshotProjectileTrail",
                        "SpiderBowProjectileTrail Variant"
                    };

                    //string projname = fullProjectileNames[UnityEngine.Random.Range(0, 15)];
                    string projname = fullProjectileNames[2];
                    string trailname = fullTrailNames[UnityEngine.Random.Range(0, 7)];

                    for (var i = 0; i < crashAmount; i++)
                    {
                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projname + "(Clone)");

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        Vector3 startpos = lagPosition - new Vector3(0f, 0.5f, 0f);
                        Vector3 charvel = Vector3.zero;

                        float randa = UnityEngine.Random.Range(0, 255);
                        float randb = UnityEngine.Random.Range(0, 255);
                        float randc = UnityEngine.Random.Range(0, 255);

                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", GetPlayerFromVRRig(whoCopy), new object[]
                        {
                            startpos,
                            charvel,
                            hasha,
                            hashb,
                            false,
                            hashc,
                            true,
                            randa / 255f,
                            randb / 255f,
                            randc / 255f,
                            1f
                        });
                        RPCProtection();
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
                Vector3 lagPosition = new Vector3(0f, 0f, 0f);

                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = lagPosition;
                GorillaTagger.Instance.myVRRig.transform.position = lagPosition;

                GameObject l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(l.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

                l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                l.transform.position = GorillaTagger.Instance.leftHandTransform.position;

                GameObject r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(r.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(r.GetComponent<SphereCollider>());

                r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                r.transform.position = GorillaTagger.Instance.rightHandTransform.position;

                l.GetComponent<Renderer>().material.color = bgColorA;
                r.GetComponent<Renderer>().material.color = bgColorA;

                UnityEngine.Object.Destroy(l, Time.deltaTime);
                UnityEngine.Object.Destroy(r, Time.deltaTime);

                string[] fullProjectileNames = new string[]
                {
                    "SlingshotProjectile",
                    "SnowballProjectile",
                    "WaterBalloonProjectile",
                    "LavaRockProjectile",
                    "HornsSlingshotProjectile_PrefabV",
                    "CloudSlingshot_Projectile",
                    "CupidArrow_Projectile",
                    "IceSlingshotProjectile_PrefabV Variant",
                    "ElfBow_Projectile",
                    "MoltenRockSlingshot_Projectile",
                    "SpiderBowProjectile Variant",
                    "BucketGift_Cane_Projectile Variant",
                    "BucketGift_Coal_Projectile Variant",
                    "BucketGift_Roll_Projectile Variant",
                    "BucketGift_Round_Projectile Variant",
                    "BucketGift_Square_Projectile Variant"
                };

                string[] fullTrailNames = new string[]
                {
                    "SlingshotProjectileTrail",
                    "HornsSlingshotProjectileTrail_PrefabV",
                    "CloudSlingshot_ProjectileTrailFX",
                    "CupidArrow_ProjectileTrailFX",
                    "IceSlingshotProjectileTrail Variant",
                    "ElfBow_ProjectileTrail",
                    "MoltenRockSlingshotProjectileTrail",
                    "SpiderBowProjectileTrail Variant"
                };

                //string projname = fullProjectileNames[UnityEngine.Random.Range(0, 15)];
                string projname = fullProjectileNames[2];
                string trailname = fullTrailNames[UnityEngine.Random.Range(0, 7)];

                for (var i = 0; i < crashAmount; i++)
                {
                    GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projname + "(Clone)");

                    GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                    int hasha = PoolUtils.GameObjHashCode(projectile);
                    int hashb = PoolUtils.GameObjHashCode(trail);
                    int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                    Vector3 startpos = lagPosition - new Vector3(0f, 0.5f, 0f);
                    Vector3 charvel = Vector3.zero;

                    float randa = UnityEngine.Random.Range(0, 255);
                    float randb = UnityEngine.Random.Range(0, 255);
                    float randc = UnityEngine.Random.Range(0, 255);

                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        false,
                        hashc,
                        true,
                        randa / 255f,
                        randb / 255f,
                        randc / 255f,
                        1f
                    });
                    RPCProtection();
                }
            } else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }
        */
        public static void DestroyGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
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

        public static void DestroyAll()
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.CurrentRoom.StorePlayer(player);
                PhotonNetwork.CurrentRoom.Players.Remove(player.ActorNumber);
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
            }
        }

        public static void BreakAudioGun()
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
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
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
