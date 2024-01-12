using Photon.Pun;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    internal class Projectiles
    {
        public static void ChangeProjectile()
        {
            projmode++;
            if (projmode > 15)
            {
                projmode = 0;
            }

            string[] shortProjectileNames = new string[] {
                "Slingshot",
                "Snowball",
                "Water Balloon",
                "Lava Rock",
                "Deadshot",
                "Pride",
                "Cupid",
                "Ice",
                "Leaves",
                "Lava Slingshot",
                "Cotton Swab",
                "Candy Cane",
                "Coal",
                "Roll Present",
                "Round Present",
                "Square Present"
            };

            GetIndex("Change Projectile").overlapText = "Change Projectile <color=grey>[</color><color=green>" + shortProjectileNames[projmode] + "</color><color=grey>]</color>";
        }

        public static void ChangeTrail()
        {
            trailmode++;
            if (trailmode > 8)
            {
                trailmode = 0;
            }

            string[] shortTrailNames = new string[]
            {
                "Regular",
                "Laser",
                "Pride",
                "Pink",
                "Ice",
                "Bow",
                "Lava",
                "Spider",
                "None"
            };

            GetIndex("Change Trail").overlapText = "Change Trail <color=grey>[</color><color=green>" + shortTrailNames[trailmode] + "</color><color=grey>]</color>"; // Regular
        }

        public static void ChangeShootSpeed()
        {
            shootCycle++;
            if (shootCycle > 3)
            {
                shootCycle = 0;
            }

            float[] ShootStrengthTypes = new float[]
            {
                9.72f,
                19.44f,
                38.88f,
                10000000f
            };

            string[] ShootStrengthNames = new string[]
            {
                "Slow",
                "Medium",
                "Fast",
                "Instant"
            };

            ShootStrength = ShootStrengthTypes[shootCycle];
            GetIndex("Change Shoot Speed").overlapText = "Change Shoot Speed <color=grey>[</color><color=green>" + ShootStrengthNames[shootCycle] + "</color><color=grey>]</color>";
        }

        public static void IncreaseRed()
        {
            red = red + 0.1f;
            if (red > 1.05f)
            {
                red = 0f;
            }

            GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + (Mathf.Floor(red * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseGreen()
        {
            green = green + 0.1f;
            if (green > 1.05f)
            {
                green = 0f;
            }

            GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + (Mathf.Floor(green * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseBlue()
        {
            blue = blue + 0.1f;
            if (blue > 1.05f)
            {
                blue = 0f;
            }

            GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + (Mathf.Floor(blue * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void ProjectileDelay()
        {
            projDebounceType = projDebounceType + 0.1f;
            if (projDebounceType > 1.05f)
            {
                projDebounceType = 0f;
            }

            GetIndex("Projectile Delay").overlapText = "Projectile Delay <color=grey>[</color><color=green>" + (Mathf.Floor(projDebounceType * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }
        /*
        public static void ProjectileSpam()
        {
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
                "SpiderBowProjectileTrail Variant",
                "SlingshotProjectileTrail"
            };

            int projIndex = projmode;
            int trailIndex = trailmode;

            if ((rightGrab || Mouse.current.leftButton.isPressed) && Time.time > projDebounce)
            {
                if (GetIndex("Random Projectile").enabled)
                {
                    projIndex = UnityEngine.Random.Range(0, 15);
                }
                string projectilename = fullProjectileNames[projIndex];

                if (GetIndex("Random Trail").enabled)
                {
                    trailIndex = UnityEngine.Random.Range(0, 8);
                }
                string trailname = fullTrailNames[trailIndex];

                GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                int hasha = PoolUtils.GameObjHashCode(projectile);
                int hashb = PoolUtils.GameObjHashCode(trail);
                int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                if (trailmode == 8)
                {
                    hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                }

                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                Vector3 charvel = GorillaLocomotion.Player.Instance.currentVelocity;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 100);
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel = charvel * (ShootStrength * 2);
                    }
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up * ShootStrength);
                    //charvel = GorillaLocomotion.Player.Instance.currentVelocity + GorillaTagger.Instance.rightHandTransform.forward - GorillaTagger.Instance.offlineVRRig.rightHand.trackingRotationOffset;
                }

                if (GetIndex("Random Direction").enabled)
                {
                    charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                }

                if (GetIndex("Above Players").enabled)
                {
                    charvel = Vector3.zero;
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 2f, UnityEngine.Random.Range(-2f, 2f));
                    charvel = Vector3.zero;
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                    charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
                }

                if (GetIndex("Include Hand Velocity").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                }

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                if (!GetIndex("Client Sided Projectiles").enabled)
                {
                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        GetIndex("Orange Team Projectiles").enabled,
                        hashc,
                        true && !(GetIndex("Blue Team Projectiles").enabled || GetIndex("Orange Team Projectiles").enabled),
                        randa / 255f,
                        randb / 255f,
                        randc / 255f,
                        1f
                    });
                    RPCProtection();
                }

                originalprojectile.SetActive(true);

                if (trailmode != 8)
                {
                    trail.SetActive(true);
                    ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                }

                comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, hashc, 1f, true, new UnityEngine.Color(randa / 255f, randb / 255f, randc / 255f, 1f));

                if (projDebounceType > 0f)
                {
                    projDebounce = Time.time + projDebounceType;
                }
            }

            if (leftGrab && Time.time > projDebounce)
            {
                if (GetIndex("Random Projectile").enabled)
                {
                    projIndex = UnityEngine.Random.Range(0, 15);
                }
                string projectilename = fullProjectileNames[projIndex];

                if (GetIndex("Random Trail").enabled)
                {
                    trailIndex = UnityEngine.Random.Range(0, 8);
                }
                string trailname = fullTrailNames[trailIndex];

                GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                int hasha = PoolUtils.GameObjHashCode(projectile);
                int hashb = PoolUtils.GameObjHashCode(trail);
                int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                if (trailmode == 8)
                {
                    hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                }

                Vector3 startpos = GorillaTagger.Instance.leftHandTransform.position;
                Vector3 charvel = GorillaLocomotion.Player.Instance.currentVelocity;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.leftHandTransform.transform.forward * ShootStrength);
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up * ShootStrength);
                }

                if (GetIndex("Random Direction").enabled)
                {
                    charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                }

                if (GetIndex("Above Players").enabled)
                {
                    charvel = Vector3.zero;
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                    charvel = Vector3.zero;
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                    charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
                }

                if (GetIndex("Include Hand Velocity").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                }

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                if (!GetIndex("Client Sided Projectiles").enabled)
                {
                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        GetIndex("Orange Team Projectiles").enabled,
                        hashc,
                        true && !(GetIndex("Blue Team Projectiles").enabled || GetIndex("Orange Team Projectiles").enabled),
                        randa / 255f,
                        randb / 255f,
                        randc / 255f,
                        1f
                    });
                }
                RPCProtection();

                originalprojectile.SetActive(true);

                if (trailmode != 8)
                {
                    trail.SetActive(true);
                    ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                }

                comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, hashc, 1f, true, new UnityEngine.Color(randa / 255f, randb / 255f, randc / 255f, 1f));

                if (projDebounceType > 0f)
                {
                    projDebounce = Time.time + projDebounceType;
                }
            }
        }

        public static void GiveProjectileSpamGun()
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
                        "SpiderBowProjectileTrail Variant",
                        "SlingshotProjectileTrail"
                    };

                    int projIndex = projmode;
                    int trailIndex = trailmode;
                    
                    if (whoCopy.rightMiddle.calcT > 0.5f && Time.time > projDebounce)
                    {
                        if (GetIndex("Random Projectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 15);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        if (GetIndex("Random Trail").enabled)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                        GameObject originalprojectile = projectile;
                        projectile = ObjectPools.instance.Instantiate(projectile);

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        if (trailmode == 8)
                        {
                            hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                        }

                        Vector3 startpos = whoCopy.rightHandTransform.position;
                        Vector3 charvel = Vector3.zero;

                        if (GetIndex("Shoot Projectiles").enabled)
                        {
                            charvel = (whoCopy.rightHandTransform.transform.forward * ShootStrength);
                        }

                        if (GetIndex("Finger Gun Projectiles").enabled)
                        {
                            charvel = (whoCopy.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up * ShootStrength);
                        }

                        if (GetIndex("Random Direction").enabled)
                        {
                            charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                        }

                        if (GetIndex("Above Players").enabled)
                        {
                            charvel = Vector3.zero;
                            //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                            startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                        }

                        if (GetIndex("Rain Projectiles").enabled)
                        {
                            startpos = whoCopy.headMesh.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                            charvel = Vector3.zero;
                        }

                        if (GetIndex("Projectile Aura").enabled)
                        {
                            float time = Time.frameCount;
                            startpos = whoCopy.headMesh.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                        }

                        if (GetIndex("Projectile Fountain").enabled)
                        {
                            startpos = whoCopy.headMesh.transform.position + new Vector3(0, 1, 0);
                            charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
                        }

                        float randa = 255f;
                        float randb = 255f;
                        float randc = 255f;

                        if (GetIndex("Random Color").enabled)
                        {
                            randa = UnityEngine.Random.Range(0, 255);
                            randb = UnityEngine.Random.Range(0, 255);
                            randc = UnityEngine.Random.Range(0, 255);
                        }

                        if (GetIndex("Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = rgbcolor.r * 255;
                            randb = rgbcolor.g * 255;
                            randc = rgbcolor.b * 255;
                        }

                        if (GetIndex("Hard Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                            randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                            randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                        }

                        if (GetIndex("Black Projectiles").enabled)
                        {
                            randa = 0f;
                            randb = 0f;
                            randc = 0f;
                        }

                        if (GetIndex("No Texture Projectiles").enabled)
                        {
                            randa = 25500f;
                            randb = 0f;
                            randc = 25500f;
                        }

                        if (GetIndex("Custom Colored Projectiles").enabled)
                        {
                            randa = red * 255;
                            randb = green * 255;
                            randc = blue * 255;
                        }

                        if (!GetIndex("Client Sided Projectiles").enabled)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                                startpos,
                                charvel,
                                hasha,
                                hashb,
                                GetIndex("Orange Team Projectiles").enabled,
                                hashc,
                                true && !(GetIndex("Blue Team Projectiles").enabled || GetIndex("Orange Team Projectiles").enabled),
                                randa / 255f,
                                randb / 255f,
                                randc / 255f,
                                1f
                            });
                            RPCProtection();
                        }

                        originalprojectile.SetActive(true);

                        if (trailmode != 8)
                        {
                            trail.SetActive(true);
                            ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                        }

                        comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, hashc, 1f, true, new UnityEngine.Color(randa / 255f, randb / 255f, randc / 255f, 1f));

                        if (projDebounceType > 0f)
                        {
                            projDebounce = Time.time + projDebounceType;
                        }
                    }

                    if (whoCopy.leftMiddle.calcT > 0.5f && Time.time > projDebounce)
                    {
                        if (GetIndex("Random Projectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 15);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        if (GetIndex("Random Trail").enabled)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                        GameObject originalprojectile = projectile;
                        projectile = ObjectPools.instance.Instantiate(projectile);

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        if (trailmode == 8)
                        {
                            hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;;
                        }

                        Vector3 startpos = whoCopy.leftHandTransform.position;
                        Vector3 charvel = Vector3.zero;

                        if (GetIndex("Shoot Projectiles").enabled)
                        {
                            charvel = (whoCopy.leftHandTransform.transform.forward * ShootStrength);
                        }

                        if (GetIndex("Finger Gun Projectiles").enabled)
                        {
                            charvel = (whoCopy.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up * ShootStrength);
                        }

                        if (GetIndex("Random Direction").enabled)
                        {
                            charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                        }

                        if (GetIndex("Above Players").enabled)
                        {
                            charvel = Vector3.zero;
                            //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                            startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                        }

                        if (GetIndex("Rain Projectiles").enabled)
                        {
                            startpos = whoCopy.headMesh.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                            charvel = Vector3.zero;
                        }

                        if (GetIndex("Projectile Aura").enabled)
                        {
                            float time = Time.frameCount;
                            startpos = whoCopy.headMesh.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                        }

                        if (GetIndex("Projectile Fountain").enabled)
                        {
                            startpos = whoCopy.headMesh.transform.position + new Vector3(0, 1, 0);
                            charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
                        }

                        float randa = 255f;
                        float randb = 255f;
                        float randc = 255f;

                        if (GetIndex("Random Color").enabled)
                        {
                            randa = UnityEngine.Random.Range(0, 255);
                            randb = UnityEngine.Random.Range(0, 255);
                            randc = UnityEngine.Random.Range(0, 255);
                        }

                        if (GetIndex("Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = rgbcolor.r * 255;
                            randb = rgbcolor.g * 255;
                            randc = rgbcolor.b * 255;
                        }

                        if (GetIndex("Hard Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                            randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                            randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                        }

                        if (GetIndex("Black Projectiles").enabled)
                        {
                            randa = 0f;
                            randb = 0f;
                            randc = 0f;
                        }

                        if (GetIndex("No Texture Projectiles").enabled)
                        {
                            randa = 25500f;
                            randb = 0f;
                            randc = 25500f;
                        }

                        if (GetIndex("Custom Colored Projectiles").enabled)
                        {
                            randa = red * 255;
                            randb = green * 255;
                            randc = blue * 255;
                        }

                        if (!GetIndex("Client Sided Projectiles").enabled)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                                startpos,
                                charvel,
                                hasha,
                                hashb,
                                GetIndex("Orange Team Projectiles").enabled,
                                hashc,
                                true && !(GetIndex("Blue Team Projectiles").enabled || GetIndex("Orange Team Projectiles").enabled),
                                randa / 255f,
                                randb / 255f,
                                randc / 255f,
                                1f
                            });
                            RPCProtection();
                        }

                        originalprojectile.SetActive(true);

                        if (trailmode != 8)
                        {
                            trail.SetActive(true);
                            ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                        }

                        comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, hashc, 1f, true, new UnityEngine.Color(randa / 255f, randb / 255f, randc / 255f, 1f));

                        if (projDebounceType > 0f)
                        {
                            projDebounce = Time.time + projDebounceType;
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

        public static void ProjectileGun()
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
                            "SpiderBowProjectileTrail Variant",
                            "SlingshotProjectileTrail"
                        };

                        int projIndex = projmode;
                        int trailIndex = trailmode;

                        if (GetIndex("RandomProjectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 15);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        if (GetIndex("Random Trail").enabled)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        if (GetIndex("RandomProjectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 15);
                        }
                        projectilename = fullProjectileNames[projIndex];

                        if (GetIndex("Random Trail").enabled)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        trailname = fullTrailNames[trailIndex];

                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                        GameObject originalprojectile = projectile;
                        projectile = ObjectPools.instance.Instantiate(projectile);

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        if (trailmode == 8)
                        {
                            hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;;
                        }

                        Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                        Vector3 charvel = GorillaLocomotion.Player.Instance.currentVelocity;

                        if (GetIndex("Shoot Projectiles").enabled)
                        {
                            charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength);
                            if (Mouse.current.leftButton.isPressed)
                            {
                                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                                Physics.Raycast(ray, out var hit, 100);
                                charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                                charvel.Normalize();
                                charvel = charvel * (ShootStrength * 2);
                            }
                        }

                        if (GetIndex("Finger Gun Projectiles").enabled)
                        {
                            charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up * ShootStrength);
                        }

                        if (GetIndex("Random Direction").enabled)
                        {
                            charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                        }

                        if (GetIndex("Above Players").enabled)
                        {
                            charvel = Vector3.zero;
                            //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                            startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                        }

                        if (GetIndex("Rain Projectiles").enabled)
                        {
                            startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                            charvel = Vector3.zero;
                        }

                        if (GetIndex("Projectile Aura").enabled)
                        {
                            float time = Time.frameCount;
                            startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                        }

                        if (GetIndex("Projectile Fountain").enabled)
                        {
                            startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                            charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
                        }

                        if (GetIndex("Include Hand Velocity").enabled)
                        {
                            charvel = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                        }

                        float randa = 255f;
                        float randb = 255f;
                        float randc = 255f;

                        if (GetIndex("Random Color").enabled)
                        {
                            randa = UnityEngine.Random.Range(0, 255);
                            randb = UnityEngine.Random.Range(0, 255);
                            randc = UnityEngine.Random.Range(0, 255);
                        }

                        if (GetIndex("Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = rgbcolor.r * 255;
                            randb = rgbcolor.g * 255;
                            randc = rgbcolor.b * 255;
                        }

                        if (GetIndex("Hard Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                            randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                            randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                        }

                        if (GetIndex("Black Projectiles").enabled)
                        {
                            randa = 0f;
                            randb = 0f;
                            randc = 0f;
                        }

                        if (GetIndex("No Texture Projectiles").enabled)
                        {
                            randa = 25500f;
                            randb = 0f;
                            randc = 25500f;
                        }

                        if (GetIndex("Custom Colored Projectiles").enabled)
                        {
                            randa = red * 255;
                            randb = green * 255;
                            randc = blue * 255;
                        }

                        if (!GetIndex("Client Sided Projectiles").enabled)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", GetPlayerFromVRRig(possibly), new object[]
                            {
                                startpos,
                                charvel,
                                hasha,
                                hashb,
                                GetIndex("Orange Team Projectiles").enabled,
                                hashc,
                                true && !(GetIndex("Blue Team Projectiles").enabled || GetIndex("Orange Team Projectiles").enabled),
                                randa / 255f,
                                randb / 255f,
                                randc / 255f,
                                1f
                            });
                            RPCProtection();
                        }

                        originalprojectile.SetActive(true);

                        if (trailmode != 8)
                        {
                            trail.SetActive(true);
                            ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                        }

                        comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, hashc, 1f, true, new UnityEngine.Color(randa / 255f, randb / 255f, randc / 255f, 1f));

                        if (projDebounceType > 0f)
                        {
                            projDebounce = Time.time + projDebounceType;
                        }
                    }
                }
            }
        }

        public static void ProjectileBomb()
        {
            if (rightGrab)
            {
                if (ProjBombObject == null)
                {
                    ProjBombObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(ProjBombObject.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(ProjBombObject.GetComponent<SphereCollider>());
                    ProjBombObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                ProjBombObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            if (ProjBombObject != null)
            {
                if (rightPrimary)
                {
                    Vector3 spawnPos = ProjBombObject.transform.position;
                    UnityEngine.Object.Destroy(ProjBombObject);
                    ProjBombObject = null;

                    for (var i = 0; i < 20; i++)
                    {
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
                            "SpiderBowProjectileTrail Variant",
                            "SlingshotProjectileTrail"
                        };

                        int projIndex = projmode;
                        if (GetIndex("Random Projectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 15);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        int trailIndex = trailmode;
                        if (GetIndex("Random Trail").enabled)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                        GameObject originalprojectile = projectile;
                        projectile = ObjectPools.instance.Instantiate(projectile);

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        if (trailmode == 8)
                        {
                            hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;;
                        }

                        Vector3 startpos = spawnPos;
                        Vector3 charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));

                        float randa = 255f;
                        float randb = 255f;
                        float randc = 255f;

                        if (GetIndex("Random Color").enabled)
                        {
                            randa = UnityEngine.Random.Range(0, 255);
                            randb = UnityEngine.Random.Range(0, 255);
                            randc = UnityEngine.Random.Range(0, 255);
                        }

                        if (GetIndex("Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = rgbcolor.r * 255;
                            randb = rgbcolor.g * 255;
                            randc = rgbcolor.b * 255;
                        }

                        if (GetIndex("Hard Rainbow Projectiles").enabled)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                            randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                            randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                            randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                        }

                        if (GetIndex("Black Projectiles").enabled)
                        {
                            randa = 0f;
                            randb = 0f;
                            randc = 0f;
                        }

                        if (GetIndex("No Texture Projectiles").enabled)
                        {
                            randa = 25500f;
                            randb = 0f;
                            randc = 25500f;
                        }

                        if (GetIndex("Custom Colored Projectiles").enabled)
                        {
                            randa = red * 255;
                            randb = green * 255;
                            randc = blue * 255;
                        }

                        if (!GetIndex("Client Sided Projectiles").enabled)
                        {
                            GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                            {
                                startpos,
                                charvel,
                                hasha,
                                hashb,
                                GetIndex("Orange Team Projectiles").enabled,
                                hashc,
                                true && !(GetIndex("Blue Team Projectiles").enabled || GetIndex("Orange Team Projectiles").enabled),
                                randa / 255f,
                                randb / 255f,
                                randc / 255f,
                                1f
                            });
                            RPCProtection();
                        }

                        originalprojectile.SetActive(true);

                        if (trailmode != 8)
                        {
                            trail.SetActive(true);
                            ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                        }

                        comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, hashc, 1f, true, new UnityEngine.Color(randa / 255f, randb / 255f, randc / 255f, 1f));
                    }
                }
                else
                {
                    ProjBombObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                }
            }
        }

        public static void DisableProjectileBomb()
        {
            if (ProjBombObject != null)
            {
                UnityEngine.Object.Destroy(ProjBombObject);
                ProjBombObject = null;
            }
        }

        public static void ImpactSpam()
        {
            if ((rightGrab || Mouse.current.leftButton.isPressed) && Time.time > projDebounce)
            {
                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out Ray, 100);
                    }
                    startpos = Ray.point;
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up, out var Ray);
                    startpos = Ray.point;
                }

                if (GetIndex("Above Players").enabled)
                {
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                }

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                GorillaGameManager.instance.photonView.RPC("SpawnSlingshotPlayerImpactEffect", RpcTarget.All, new object[]
                {
                    startpos,
                    randa / 255f,
                    randb / 255f,
                    randc / 255f,
                    1f,
                    1
                });
                RPCProtection();

                if (projDebounceType > 0f)
                {
                    projDebounce = Time.time + projDebounceType;
                }
            }

            if (leftGrab && Time.time > projDebounce)
            {
                Vector3 startpos = GorillaTagger.Instance.leftHandTransform.position;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out var Ray);
                    startpos = Ray.point;
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up, out var Ray);
                    startpos = Ray.point;
                }

                if (GetIndex("Above Players").enabled)
                {
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                }

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                GorillaGameManager.instance.photonView.RPC("SpawnSlingshotPlayerImpactEffect", RpcTarget.All, new object[]
                {
                    startpos,
                    randa / 255f,
                    randb / 255f,
                    randc / 255f,
                    1f,
                    1
                });
                RPCProtection();

                if (projDebounceType > 0f)
                {
                    projDebounce = Time.time + projDebounceType;
                }
            }
        }

        public static void Urine()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SlingshotProjectile(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                int hasha = PoolUtils.GameObjHashCode(projectile);
                int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                {
                    startpos,
                    charvel,
                    hasha,
                    hashb,
                    false,
                    hashc,
                    true,
                    100f,
                    100f,
                    0f,
                    1f
                });
                RPCProtection();
                originalprojectile.SetActive(true);
                comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(100f, 100f, 0f, 1f));
            }
        }

        public static void Feces()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SnowballProjectile(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                int hasha = PoolUtils.GameObjHashCode(projectile);
                int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.3f, 0f);
                Vector3 charvel = Vector3.zero;

                GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                {
                    startpos,
                    charvel,
                    hasha,
                    hashb,
                    false,
                    hashc,
                    true,
                    (99f/255f),
                    (43f/255f),
                    0f,
                    1f
                });
                RPCProtection();
                originalprojectile.SetActive(true);
                comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new Color32(99, 43, 0, 255));
            }
        }

        public static void Semen()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SlingshotProjectile(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                int hasha = PoolUtils.GameObjHashCode(projectile);
                int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                {
                    startpos,
                    charvel,
                    hasha,
                    hashb,
                    false,
                    hashc,
                    true,
                    100f,
                    100f,
                    100f,
                    1f
                });
                RPCProtection();
                originalprojectile.SetActive(true);
                comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(100f, 100f, 100f, 1f));
            }
        }

        public static void Vomit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SnowballProjectile(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                int hasha = PoolUtils.GameObjHashCode(projectile);
                int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);// 0;
                int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                {
                    startpos,
                    charvel,
                    hasha,
                    hashb,
                    false,
                    hashc,
                    true,
                    0f,
                    100f,
                    0f,
                    1f
                });
                RPCProtection();
                originalprojectile.SetActive(true);
                comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(0f, 100f, 0f, 1f));
            }
        }

        public static void ServersidedTracers()
        {
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
                "SpiderBowProjectileTrail Variant",
                "SlingshotProjectileTrail"
            };

            int projIndex = projmode;
            if (GetIndex("Random Projectile").enabled)
            {
                projIndex = UnityEngine.Random.Range(0, 15);
            }
            string projectilename = fullProjectileNames[projIndex];

            int trailIndex = trailmode;
            if (GetIndex("Random Trail").enabled)
            {
                trailIndex = UnityEngine.Random.Range(0, 8);
            }
            string trailname = fullTrailNames[trailIndex];

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.mainSkin.material.name.Contains("fected"))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                {
                    VRRig vrrig = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                        GameObject originalprojectile = projectile;
                        projectile = ObjectPools.instance.Instantiate(projectile);

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        if (trailmode == 8)
                        {
                            hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                        }

                        Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                        Vector3 charvel = vrrig.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                        charvel.Normalize();
                        charvel = charvel * 100f;

                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                        {
                            startpos,
                            charvel,
                            hasha,
                            hashb,
                            false,
                            hashc,
                            true,
                            0f,
                            0f,
                            0f,
                            1f
                        });
                        RPCProtection();

                        originalprojectile.SetActive(true);

                        if (trailmode != 8)
                        {
                            trail.SetActive(true);
                            ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                        }

                        comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(0f, 0f, 0f, 1f));
                    }
                }
                else
                {
                    VRRig vrrig = GetRandomVRRig(false);// GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                        GameObject originalprojectile = projectile;
                        projectile = ObjectPools.instance.Instantiate(projectile);

                        GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                        SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                        int hasha = PoolUtils.GameObjHashCode(projectile);
                        int hashb = PoolUtils.GameObjHashCode(trail);
                        int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                        if (trailmode == 8)
                        {
                            hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                        }

                        Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                        Vector3 charvel = vrrig.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                        charvel.Normalize();
                        charvel = charvel * 100f;

                        GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                        {
                            startpos,
                            charvel,
                            hasha,
                            hashb,
                            false,
                            hashc,
                            true,
                            vrrig.playerColor.r,
                            vrrig.playerColor.g,
                            vrrig.playerColor.b,
                            1f
                        });
                        RPCProtection();

                        originalprojectile.SetActive(true);

                        if (trailmode != 8)
                        {
                            trail.SetActive(true);
                            ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                        }

                        comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, vrrig.playerColor);
                    }
                }
            }
            else
            {
                VRRig vrrig = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                    GameObject originalprojectile = projectile;
                    projectile = ObjectPools.instance.Instantiate(projectile);

                    GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                    SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                    int hasha = PoolUtils.GameObjHashCode(projectile);
                    int hashb = PoolUtils.GameObjHashCode(trail);
                    int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                    if (trailmode == 8)
                    {
                        hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                    }

                    Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                    Vector3 charvel = vrrig.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                    charvel.Normalize();
                    charvel = charvel * 100f;

                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        false,
                        hashc,
                        true,
                        vrrig.playerColor.r,
                        vrrig.playerColor.g,
                        vrrig.playerColor.b,
                        1f
                    });
                    RPCProtection();

                    originalprojectile.SetActive(true);

                    if (trailmode != 8)
                    {
                        trail.SetActive(true);
                        ObjectPools.instance.Instantiate(trail).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile, false, false);
                    }

                    comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, vrrig.playerColor);
                }
            }
        }

        public static void UrineGun()
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
                    GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SlingshotProjectile(Clone)");
                    GameObject originalprojectile = projectile;
                    projectile = ObjectPools.instance.Instantiate(projectile);

                    SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                    int hasha = PoolUtils.GameObjHashCode(projectile);
                    int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                    int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.4f, 0f) + (whoCopy.transform.forward * 0.2f);
                    Vector3 charvel = whoCopy.transform.forward * 8.33f;

                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        false,
                        hashc,
                        true,
                        100f,
                        100f,
                        0f,
                        1f
                    });
                    RPCProtection();
                    originalprojectile.SetActive(true);
                    comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(100f, 100f, 0f, 1f));
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

        public static void FecesGun()
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
                    GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SnowballProjectile(Clone)");
                    GameObject originalprojectile = projectile;
                    projectile = ObjectPools.instance.Instantiate(projectile);

                    SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                    int hasha = PoolUtils.GameObjHashCode(projectile);
                    int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                    int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.65f, 0f);
                    Vector3 charvel = Vector3.zero;

                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        false,
                        hashc,
                        true,
                        (99f/255f),
                        (43f/255f),
                        0f,
                        1f
                    });
                    RPCProtection();
                    originalprojectile.SetActive(true);
                    comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new Color32(99, 43, 0, 255));
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

        public static void SemenGun()
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
                    GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SlingshotProjectile(Clone)");
                    GameObject originalprojectile = projectile;
                    projectile = ObjectPools.instance.Instantiate(projectile);

                    SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                    int hasha = PoolUtils.GameObjHashCode(projectile);
                    int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                    int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.4f, 0f) + (whoCopy.transform.forward * 0.2f);
                    Vector3 charvel = whoCopy.transform.forward * 8.33f;

                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        false,
                        hashc,
                        true,
                        100f,
                        100f,
                        100f,
                        1f
                    });
                    RPCProtection();
                    originalprojectile.SetActive(true);
                    comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(100f, 100f, 100f, 1f));
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

        public static void VomitGun()
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
                    GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/SnowballProjectile(Clone)");
                    GameObject originalprojectile = projectile;
                    projectile = ObjectPools.instance.Instantiate(projectile);

                    SlingshotProjectile comp = projectile.GetComponent<SlingshotProjectile>();

                    int hasha = PoolUtils.GameObjHashCode(projectile);
                    int hashb = PoolUtils.GameObjHashCode(GorillaTagger.Instance.offlineVRRig.slingshot.projectileTrail);//0;
                    int hashc = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();

                    Vector3 startpos = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * 0.4f) + (whoCopy.headMesh.transform.up * -0.05f);
                    Vector3 charvel = whoCopy.headMesh.transform.forward * 8.33f;

                    GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
                    {
                        startpos,
                        charvel,
                        hasha,
                        hashb,
                        false,
                        hashc,
                        true,
                        0f,
                        100f,
                        0f,
                        1f
                    });
                    RPCProtection();
                    originalprojectile.SetActive(true);
                    comp.Launch(startpos, charvel, PhotonNetwork.LocalPlayer, false, false, hashc, 1f, true, new UnityEngine.Color(0f, 100f, 0f, 1f));
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
        }*/
    }
}
