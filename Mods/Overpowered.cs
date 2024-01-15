using ExitGames.Client.Photon;
using GorillaNetworking;
using Mono.Cecil.Cil;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.Spammers.Projectiles;

namespace iiMenu.Mods
{
    internal class Overpowered
    {
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
                        GorillaGameManager.instance.FindVRRigForPlayer(player).RPC("SetTaggedTime", player, null);
                        RPCProtection();
                        kgDebounce = Time.time + 0.2f;
                    }
                }
            }
        }

        public static void SlowAll()
        {
            if (Time.time > kgDebounce)
            {
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    GorillaGameManager.instance.FindVRRigForPlayer(player).RPC("SetTaggedTime", player, null);
                    RPCProtection();
                    kgDebounce = Time.time + 0.5f;
                }
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
                        GorillaTagger.Instance.myVRRig.RPC("SetJoinTaggedTime", owner, null);
                        RPCProtection();
                        kgDebounce = Time.time + 0.2f;
                    }
                }
            }
        }

        public static void VibrateAll()
        {
            if (Time.time > kgDebounce)
            {
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    GorillaTagger.Instance.myVRRig.RPC("SetJoinTaggedTime", player, null);
                    RPCProtection();
                    kgDebounce = Time.time + 0.5f;
                }
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
                            /*PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                            PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                            PhotonView.Get(GorillaGameManager.instance).RPC("JoinPubWithFriends", owner, new object[]
                            {
                                PhotonNetworkController.Instance.shuffler,
                                PhotonNetworkController.Instance.keyStr
                            });
                            PhotonNetwork.SendAllOutgoingCommands();
                            RPCProtection();*/
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
                        PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                        PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                        PhotonView.Get(GorillaGameManager.instance).RPC("JoinPubWithFriends", player, new object[]
                        {
                            PhotonNetworkController.Instance.shuffler,
                            PhotonNetworkController.Instance.keyStr
                        });
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
                        213,
                        false,
                        0.01f
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
            GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
                213,
                false,
                //999999f
                0.01f
            });
        }
    }
}
