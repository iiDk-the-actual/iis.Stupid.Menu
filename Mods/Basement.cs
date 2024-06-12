using GorillaTag;
using Oculus.Platform.Models;
using Photon.Pun;
using System;
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
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        monkeyeAI.speed = 0.02f;
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
        }

        public static void FastMonsters()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        monkeyeAI.speed = 0.5f;
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
        }

        public static void FixMonsters()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        monkeyeAI.speed = 0.1f;
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    if (!GetIndex("Disable Auto Anti Ban").enabled)
                    {
                        Overpowered.FastMaster();
                    }
                }
                else
                {
                    foreach (MonkeyeAI monkeyeAI in GetMonsters())
                    {
                        if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                        {
                            monkeyeAI.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                        }
                        else
                        {
                            monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                        }
                    }
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
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            Overpowered.FastMaster();
                        }
                    }
                    else
                    {
                        foreach (MonkeyeAI monkeyeAI in GetMonsters())
                        {
                            if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                            {
                                monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                            }
                            else
                            {
                                monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                            }
                        }
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
        }

        public static void OrbitMonsters()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                MonkeyeAI[] them = GetMonsters();
                int index = 0;
                foreach (MonkeyeAI monkeyeAI in them)
                {
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        float offset = (360f / (float)them.Length) * index;
                        monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);
                        index++;
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
        }

        public static void DestroyMonsters()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        monkeyeAI.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
        }

        public static void FloatSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
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
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        i++;
                        monkeyeAI.gameObject.transform.position = lol[i];
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
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
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            Overpowered.FastMaster();
                        }
                    }
                    else
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
                            if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                            {
                                i++;
                                monkeyeAI.gameObject.transform.position = lol[i];
                            }
                            else
                            {
                                monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                            }
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
                }
            }
        }

        public static void FreezeSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
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
                    if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        i++;
                        monkeyeAI.gameObject.transform.position = lol[i];
                    }
                    else
                    {
                        monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
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
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            Overpowered.FastMaster();
                        }
                    }
                    else
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
                            if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                            {
                                i++;
                                monkeyeAI.gameObject.transform.position = lol[i];
                            }
                            else
                            {
                                monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                            }
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
                }
            }
        }
    }
}
