using GorillaTag;
using Oculus.Platform.Models;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Basement
    {
        public static void SodaSelf()
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
        }

        public static void SlowMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GameObject.FindObjectsOfType<MonkeyeAI>())
            {
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GameObject.FindObjectsOfType<MonkeyeAI>())
            {
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GameObject.FindObjectsOfType<MonkeyeAI>())
            {
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GameObject.FindObjectsOfType<MonkeyeAI>())
                {
                    monkeyeAI.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                }
            }
        }

        public static void MonsterGun()
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
                    foreach (MonkeyeAI monkeyeAI in GameObject.FindObjectsOfType<MonkeyeAI>())
                    {
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    }
                }
            }
        }

        public static void DestroyMonsters()
        {
            GameObject.Find("Cave Bat Holdable").transform.position = new Vector3(99999f, 99999f, 99999f);
        }
    }
}
