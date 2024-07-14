using GorillaTag;
using Oculus.Platform.Models;
using Photon.Pun;
using System;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Basement
    {
        /*public static void SodaSelf()
        {
            ScienceExperimentManager.instance.photonView.RPC("PlayerEnteredGameAreaRPC", RpcTarget.MasterClient, Array.Empty<object>());
            ScienceExperimentManager.instance.photonView.RPC("PlayerTouchedLavaRPC", RpcTarget.MasterClient, Array.Empty<object>());
            RPCProtection();
        }
        public static void UnsodaSelf()
        {
            ScienceExperimentManager.instance.photonView.RPC("PlayerTouchedRefreshWaterRPC", RpcTarget.All, Array.Empty<object>());
            ScienceExperimentManager.instance.photonView.RPC("PlayerExitedGameAreaRPC", RpcTarget.All, Array.Empty<object>());
            RPCProtection();
        }*/

        public static void SlowMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                    monkeyeAI.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                }
            }
        }

        public static void MonsterGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    foreach (MonkeyeAI monkeyeAI in GetMonsters())
                    {
                        //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            }
        }

        public static void OrbitMonsters()
        {
            MonkeyeAI[] them = GetMonsters();
            int index = 0;
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                float offset = (360f / (float)them.Length) * index;
                monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);
                index++;
            }
        }

        public static void DestroyMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
            }
        }

        public static void FloatSelf()
        {
            float monsDist = 0.065f;
            Vector3 anchor = GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(0f, -0.375f, 0f);
            Vector3[] lol = new Vector3[]
            {
                anchor + new Vector3(0f, 0f, monsDist),
                anchor - new Vector3(0f, 0f, monsDist),
                anchor + new Vector3(monsDist, 0f, 0f),
                anchor - new Vector3(monsDist, 0f, 0f),
                /*anchor + GorillaTagger.Instance.offlineVRRig.transform.forward * monsDist,
                anchor - GorillaTagger.Instance.offlineVRRig.transform.forward * monsDist,
                anchor + GorillaTagger.Instance.offlineVRRig.transform.right * monsDist,
                anchor - GorillaTagger.Instance.offlineVRRig.transform.right * monsDist,*/
            };
            int i = 0;
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                i++;
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = lol[i];
            }
        }

        public static void FloatGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    float monsDist = 0.065f;
                    Vector3 anchor = whoCopy.transform.position + new Vector3(0f, -0.375f, 0f);
                    Vector3[] lol = new Vector3[] // Editors note, if this doesn't work try doing the other values
                    {
                        anchor + new Vector3(0f, 0f, monsDist),
                        anchor - new Vector3(0f, 0f, monsDist),
                        anchor + new Vector3(monsDist, 0f, 0f),
                        anchor - new Vector3(monsDist, 0f, 0f),
                        /*anchor + whoCopy.transform.forward * monsDist,
                        anchor - whoCopy.transform.forward * monsDist,
                        anchor + whoCopy.transform.right * monsDist,
                        anchor - whoCopy.transform.right * monsDist,*/
                    };
                    int i = 0;
                    foreach (MonkeyeAI monkeyeAI in GetMonsters())
                    {
                        i++;
                        //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = lol[i];
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

        public static void FreezeSelf()
        {
            float monsDist = 0.09f;
            Vector3 anchor = GorillaTagger.Instance.offlineVRRig.transform.position;
            Vector3[] lol = new Vector3[]
            {
                anchor + GorillaTagger.Instance.offlineVRRig.transform.forward * monsDist,
                anchor - GorillaTagger.Instance.offlineVRRig.transform.forward * monsDist,
                anchor + GorillaTagger.Instance.offlineVRRig.transform.right * monsDist,
                anchor - GorillaTagger.Instance.offlineVRRig.transform.right * monsDist,
            };
            int i = 0;
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                i++;
                //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = lol[i];
            }
        }

        public static void FreezeGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    float monsDist = 0.09f;
                    Vector3 anchor = whoCopy.transform.position;
                    Vector3[] lol = new Vector3[]
                    {
                        anchor + GorillaTagger.Instance.offlineVRRig.transform.forward * monsDist,
                        anchor - GorillaTagger.Instance.offlineVRRig.transform.forward * monsDist,
                        anchor + GorillaTagger.Instance.offlineVRRig.transform.right * monsDist,
                        anchor - GorillaTagger.Instance.offlineVRRig.transform.right * monsDist,
                    };
                    int i = 0;
                    foreach (MonkeyeAI monkeyeAI in GetMonsters())
                    {
                        i++;
                        //GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = lol[i];
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

        private static bool lasttriggerthing = false;
        public static void FreezeAll()
        {
            if (rightTrigger > 0.5f)
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    //GetOwnership(monkeyeAI.gameObject.GetComponent<PhotonView>());
                    monkeyeAI.gameObject.transform.position = new Vector3(1e11f, 1e11f, 1e11f); // e is scientific notation to the skids
                }
            } else
            {
                if (lasttriggerthing)
                {
                    foreach (MonkeyeAI monkeyeAI in GetMonsters())
                    {
                        //GetOwnership(monkeyeAI.gameObject.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = Vector3.zero;
                    }
                }
            }
            lasttriggerthing = rightTrigger > 0.5;
        }
    }
}
