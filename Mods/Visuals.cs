﻿using GorillaExtensions;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Notifications;
using Pathfinding.RVO;
using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Visuals
    {
        public static void LightningStrike(Vector3 position)
        {
            GameObject line = new GameObject("LightningOuter");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = Color.cyan; liner.endColor = Color.cyan; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 5; liner.useWorldSpace = true;
            Vector3 victim = position;
            for (int i = 0; i < 5; i++)
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, false, 5f);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, true, 5f);
                liner.SetPosition(i, victim);
                victim += new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
            }
            liner.material.shader = Shader.Find("GUI/Text Shader");
            UnityEngine.Object.Destroy(line, 2f);

            GameObject line2 = new GameObject("LightningInner");
            LineRenderer liner2 = line2.AddComponent<LineRenderer>();
            liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.15f; liner2.endWidth = 0.15f; liner2.positionCount = 5; liner2.useWorldSpace = true;
            for (int i = 0; i < 5; i++)
            {
                liner2.SetPosition(i, liner.GetPosition(i));
            }
            liner2.material.shader = Shader.Find("GUI/Text Shader");
            liner2.material.renderQueue = liner.material.renderQueue + 1;
            UnityEngine.Object.Destroy(line2, 2f);
        }

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
                } catch { }
                liner.SetPosition(0, startPos + (dir * 0.1f));
                liner.SetPosition(1, endPos);
                liner.material.shader = Shader.Find("GUI/Text Shader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                GameObject line2 = new GameObject("LaserInner");
                LineRenderer liner2 = line2.AddComponent<LineRenderer>();
                liner2.startColor = Color.white; liner2.endColor = Color.white; liner2.startWidth = 0.1f; liner2.endWidth = 0.1f; liner2.positionCount = 2; liner2.useWorldSpace = true;
                liner2.SetPosition(0, startPos + (dir * 0.1f));
                liner2.SetPosition(1, endPos);
                liner2.material.shader = Shader.Find("GUI/Text Shader");
                liner2.material.renderQueue = liner.material.renderQueue + 1;
                UnityEngine.Object.Destroy(line2, Time.deltaTime);

                GameObject whiteParticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(whiteParticle, 2f);
                UnityEngine.Object.Destroy(whiteParticle.GetComponent<Collider>());
                whiteParticle.GetComponent<Renderer>().material.color = Color.yellow;
                whiteParticle.AddComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-7.5f, 7.5f), UnityEngine.Random.Range(0f, 7.5f), UnityEngine.Random.Range(-7.5f, 7.5f));
                whiteParticle.transform.position = endPos + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
                whiteParticle.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                yield return null;
            }
        }

        public static void WatchOn()
        {
            GameObject mainwatch = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            regwatchobject = UnityEngine.Object.Instantiate(mainwatch, rightHand ? GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);
            UnityEngine.Object.Destroy(regwatchobject.GetComponent<GorillaHuntComputer>());
            regwatchobject.SetActive(true);
            
            Transform thething = regwatchobject.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
            thething.Find("Hat").gameObject.SetActive(false);
            thething.Find("Face").gameObject.SetActive(false);
            thething.Find("Badge").gameObject.SetActive(false);
            thething.Find("Material").gameObject.SetActive(false);
            thething.Find("Left Hand").gameObject.SetActive(false);
            thething.Find("Right Hand").gameObject.SetActive(false);

            regwatchText = thething.Find("Text").gameObject;
            regwatchShell = regwatchobject.transform.Find("HuntWatch_ScreenLocal").gameObject;

            regwatchShell.GetComponent<Renderer>().material = OrangeUI;

            if (rightHand)
            {
                regwatchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
                regwatchShell.transform.parent.localPosition += new Vector3(0.025f, 0f, 0f);
                regwatchShell.transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
            }
        }

        public static void WatchStep()
        {
            regwatchText.GetComponent<UnityEngine.UI.Text>().text = "ii's Stupid Menu";
            if (doCustomName)
            {
                regwatchText.GetComponent<UnityEngine.UI.Text>().text = customMenuName;
            }
            regwatchText.GetComponent<UnityEngine.UI.Text>().text += "\n<color=grey>" + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString() + " FPS\n" + DateTime.Now.ToString("hh:mm tt") + "</color>";
            regwatchText.GetComponent<UnityEngine.UI.Text>().color = titleColor;

            if (lowercaseMode)
            {
                regwatchText.GetComponent<UnityEngine.UI.Text>().text = regwatchText.GetComponent<UnityEngine.UI.Text>().text.ToLower();
            }
        }

        public static void WatchOff()
        {
            UnityEngine.Object.Destroy(regwatchobject);
        }

        public static Material oldSkyMat = null;
        public static void DoCustomSkyboxColor()
        {
            GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
            oldSkyMat = sky.GetComponent<Renderer>().material;
        }

        public static void CustomSkyboxColor()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky").GetComponent<Renderer>().material = OrangeUI;
        }

        public static void UnCustomSkyboxColor()
        {
            GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
            sky.GetComponent<Renderer>().material = oldSkyMat;
        }

        public static bool PerformanceVisuals;
        public static void PerformanceVisualsEnabled()
        {
            PerformanceVisuals = true;
        }
        public static void PerformanceVisualsDisabled()
        {
            PerformanceVisuals = false;
        }

        public static float PerformanceModeStep = 0.2f;
        public static int PerformanceModeStepIndex = 2;
        public static void ChangePerformanceModeVisualStep()
        {
            PerformanceModeStepIndex++;
            if (PerformanceModeStepIndex > 10)
                PerformanceModeStepIndex = 0;

            PerformanceModeStep = PerformanceModeStepIndex / 10f;
            GetIndex("Change Performance Visuals Step").overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>" + PerformanceModeStep.ToString() + "</color><color=grey>]</color>";
        }
        public static float PerformanceVisualDelay;
        public static int DelayChangeStep;

        public static void GreenScreen()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            Color bgcolor = Color.green;

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);
        }

        public static void BlueScreen()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            Color bgcolor = Color.blue;

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);
        }

        public static void RedScreen()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            Color bgcolor = Color.red;

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);
        }

        public static void VelocityLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GameObject go = new GameObject("Lbl");
            if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            TextMesh textMesh = go.AddComponent<TextMesh>();
            textMesh.color = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity.magnitude >= GorillaLocomotion.Player.Instance.maxJumpSpeed ? Color.green : Color.white;
            textMesh.fontSize = 24;
            textMesh.fontStyle = activeFontStyle;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.text = string.Format("{0:F1}m/s", GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity.magnitude);

            go.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.1f, 0f);
            go.transform.LookAt(Camera.main.transform.position);
            go.transform.Rotate(0f, 180f, 0f);
            UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
        }

        private static float startTime = 0f;
        private static float endTime = 0f;
        private static bool lastWasTagged = false;
        public static void TimeLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            if (PhotonNetwork.InRoom)
            {
                bool isThereTagged = false;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (PlayerIsTagged(vrrig))
                    {
                        isThereTagged = true;
                        break;
                    }
                }
                if (isThereTagged)
                {
                    bool playerIsTagged = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
                    if (playerIsTagged && !lastWasTagged)
                    {
                        endTime = Time.time - startTime;
                    }
                    if (!playerIsTagged && lastWasTagged)
                    {
                        startTime = Time.time;
                    }
                    lastWasTagged = playerIsTagged;

                    GameObject go = new GameObject("Lbl");
                    if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.color = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) ? Color.green : Color.white;
                    textMesh.fontSize = 24;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.text = !playerIsTagged ?
                        FormatUnix(Mathf.FloorToInt(Time.time - startTime)) :
                        FormatUnix(Mathf.FloorToInt(endTime));

                    go.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, GetIndex("Velocity Label").enabled ? 0.2f : 0.1f, 0f);
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                else
                {
                    startTime = Time.time;
                }
            }
        }

        public static void NearbyTaggerLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                float closest = float.MaxValue;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig && PlayerIsTagged(vrrig))
                    {
                        float dist = Vector3.Distance(GorillaTagger.Instance.headCollider.transform.position, vrrig.headMesh.transform.position);
                        if (dist < closest)
                        {
                            closest = dist;
                        }
                    }
                }
                if (closest != float.MaxValue)
                {
                    Color colorn = Color.green;
                    if (closest < 30f)
                    {
                        colorn = Color.yellow;
                    }
                    if (closest < 20f)
                    {
                        colorn = new Color32(255, 90, 0, 255);
                    }
                    if (closest < 10f)
                    {
                        colorn = Color.red;
                    }
                    GameObject go = new GameObject("Lbl");
                    if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.color = colorn;
                    textMesh.fontSize = 24;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.text = string.Format("{0:F1}m", closest);

                    go.transform.position = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, GetIndex("Last Label").enabled ? 0.2f : 0.1f, 0f);
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void LastLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            bool isThereTagged = false;
            int left = PhotonNetwork.PlayerList.Length;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isThereTagged = true;
                    left--;
                }
            }
            if (PhotonNetwork.InRoom && isThereTagged)
            {
                GameObject go = new GameObject("Lbl");
                if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                TextMesh textMesh = go.AddComponent<TextMesh>();
                textMesh.color = left <= 1 && !PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) ? Color.green : Color.white;
                textMesh.fontSize = 24;
                textMesh.fontStyle = activeFontStyle;
                textMesh.characterSize = 0.1f;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;
                textMesh.text = left.ToString() + " left";

                go.transform.position = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, 0.1f, 0f);
                go.transform.LookAt(Camera.main.transform.position);
                go.transform.Rotate(0f, 180f, 0f);
                UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
            }
        }

        public static void FakeUnbanSelf()
        {
            PhotonNetworkController.Instance.UpdateTriggerScreens();
            GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
            GorillaComputer.instance.screenText.DisableFailedState();
            GorillaComputer.instance.functionSelectText.DisableFailedState();
        }

        private static GameObject visualizerObject = null;
        private static GameObject visualizerOutline = null;
        public static void CreateAudioVisualizer()
        {
            visualizerObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visualizerOutline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            UnityEngine.Object.Destroy(visualizerObject.GetComponent<Collider>());
            UnityEngine.Object.Destroy(visualizerOutline.GetComponent<Collider>());
            UnityEngine.Object.Destroy(visualizerObject.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(visualizerOutline.GetComponent<Rigidbody>());
        }

        public static void AudioVisualizer()
        {
            visualizerObject.GetComponent<Renderer>().material.color = GetBGColor(0f);
            visualizerOutline.GetComponent<Renderer>().material.color = GetBRColor(0f);

            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GorillaLocomotion.Player.Instance.locomotionEnabledLayers);
            visualizerObject.transform.position = Ray.point;
            visualizerObject.transform.rotation = Quaternion.LookRotation(Ray.normal) * Quaternion.Euler(90f, 0f, 0f);

            float size = 0f;
            GorillaSpeakerLoudness recorder = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
            if (recorder != null)
            {
                size = recorder.Loudness;
            }

            size *= 16f;
            visualizerObject.transform.localScale = new Vector3(size, 0.05f, size);

            visualizerObject.GetComponent<Renderer>().enabled = size > 0.05f;
            visualizerOutline.GetComponent<Renderer>().enabled = size > 0.05f;

            visualizerOutline.transform.position = visualizerObject.transform.position;
            visualizerOutline.transform.rotation = visualizerObject.transform.rotation;
            visualizerOutline.transform.localScale = new Vector3(size + 0.05f, 0.025f, size + 0.05f);
        }

        public static void DestroyAudioVisualizer()
        {
            UnityEngine.Object.Destroy(visualizerObject);
            UnityEngine.Object.Destroy(visualizerOutline);
        }

        public static void VisualizeNetworkTriggers()
        {
            GameObject triggers = GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab");
            for (int i=0; i<triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        VisualizeCube(child.position, child.rotation, child.localScale, Color.red);
                    }
                } catch { }
            }
        }

        public static void VisualizeMapTriggers()
        {
            GameObject triggers = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        VisualizeCube(child.position, child.rotation, child.localScale, GetBGColor(0f));
                    }
                } catch { }
            }
        }

        private static Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject> { };
        public static void NameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    nametags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    if (!nametags.ContainsKey(vrrig))
                    {
                        GameObject go = new GameObject("iiMenu_Nametag");
                        go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                        TextMesh textMesh = go.AddComponent<TextMesh>();
                        textMesh.fontSize = 48;
                        textMesh.characterSize = 0.1f;
                        textMesh.anchor = TextAnchor.MiddleCenter;
                        textMesh.alignment = TextAlignment.Center;
                        
                        nametags.Add(vrrig, go);
                    }

                    GameObject nameTag = nametags[vrrig];
                    nameTag.GetComponent<TextMesh>().text = RigManager.GetPlayerFromVRRig(vrrig).NickName;
                    nameTag.GetComponent<TextMesh>().color = vrrig.playerColor;
                    nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                    nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.4f;
                    nameTag.transform.LookAt(Camera.main.transform.position);
                    nameTag.transform.Rotate(0f, 180f, 0f);
                }
            }
        }

        public static void DisableNameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                UnityEngine.Object.Destroy(nametag.Value);
            }
            nametags.Clear();
        }

        public static void ShowPlayspaceCenter()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GameObject playspaceCenter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(playspaceCenter.GetComponent<Collider>());
            UnityEngine.Object.Destroy(playspaceCenter.GetComponent<Renderer>());

            playspaceCenter.GetComponent<Renderer>().material.color = GetBGColor(0f);
            playspaceCenter.transform.localScale = new Vector3(0.1f, 0.1f, 0.15f);

            Physics.Raycast(new Vector3(GorillaTagger.Instance.transform.position.x, GorillaTagger.Instance.headCollider.transform.position.y, GorillaTagger.Instance.transform.position.x), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
            playspaceCenter.transform.position = Ray.point;
            playspaceCenter.transform.rotation = GorillaTagger.Instance.transform.rotation;

            UnityEngine.Object.Destroy(playspaceCenter, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
        }

        public static void FixRigColors()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                {
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                }
            }
        }

        /*public static void EnableRemoveLeaves()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("smallleaves"))
                {
                    g.SetActive(false);
                    leaves.Add(g);
                }
            }
        }

        public static void DisableRemoveLeaves()
        {
            foreach (GameObject l in leaves)
            {
                l.SetActive(true);
            }
            leaves.Clear();
        }*/
        public static List<GameObject> leaves = new List<GameObject> { };
        public static void EnableRemoveLeaves()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && (g.name.Contains("leaves_green") || g.name.Contains("fallleaves")))
                {
                    g.SetActive(false);
                    leaves.Add(g);
                }
            }
        }

        public static void DisableRemoveLeaves()
        {
            foreach (GameObject l in leaves)
            {
                l.SetActive(true);
            }
            leaves.Clear();
        }

        public static void EnableStreamerRemoveLeaves()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && (g.name.Contains("leaves_green") || g.name.Contains("fallleaves")))
                {
                    g.layer = 16;
                    leaves.Add(g);
                }
            }
        }

        public static void DisableStreamerRemoveLeaves()
        {
            foreach (GameObject l in leaves)
            {
                l.layer = 0;
            }
            leaves.Clear();
        }

        /*
        public static void EnableRemoveCherryBlossoms()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("Cherry Blossoms"))
                {
                    g.SetActive(false);
                    cblos.Add(g);
                }
            }
        }

        public static void DisableRemoveCherryBlossoms()
        {
            foreach (GameObject l in cblos)
            {
                l.SetActive(true);
            }
            cblos.Clear();
        }*/

        public static List<GameObject> cosmetics = new List<GameObject> { };
        public static void DisableCosmetics()
        {
            try
            {
                foreach (GameObject Cosmetic in GorillaTagger.Instance.offlineVRRig.cosmetics)
                {
                    if (Cosmetic.activeSelf && Cosmetic.transform.parent == GorillaTagger.Instance.offlineVRRig.mainCamera.transform)
                    {
                        cosmetics.Add(Cosmetic);
                        Cosmetic.SetActive(false);
                    }
                }
            }
            catch { }
        }

        public static void EnableCosmetics()
        {
            foreach (GameObject c in cosmetics)
            {
                c.SetActive(true);
            }
            cosmetics.Clear();
        }

        public static void NoSmoothRigs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.lerpValueBody = 1f;
                    vrrig.lerpValueFingers = 1f;
                }
            }
        }

        public static void ReSmoothRigs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.lerpValueBody = GorillaTagger.Instance.offlineVRRig.lerpValueBody;
                    vrrig.lerpValueFingers = GorillaTagger.Instance.offlineVRRig.lerpValueFingers;
                }
            }
        }

        public static void CosmeticESP() // Messy code lol
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    bool showtracersplz = false;
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBADE") || vrrig.concatStringOfCosmeticsAllowed.Contains("LBAGS"))
                    {
                        thecolor = Color.green;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBACP"))
                    {
                        thecolor = Color.blue;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK"))
                    {
                        thecolor = Color.red;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAD"))
                    {
                        thecolor = Color.white;
                        showtracersplz = true;
                    }
                    if (showtracersplz)
                    {
                        GameObject line = new GameObject("Line");
                        if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                        LineRenderer liner = line.AddComponent<LineRenderer>();
                        if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                        liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        private static Texture2D voicetxt = null;
        public static void VoiceESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                    {
                        size = recorder.Loudness * 3f;
                    }
                    if (size > 0f)
                    {
                        GameObject volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                        UnityEngine.Object.Destroy(volIndicator, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        volIndicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        if (voicetxt == null)
                        {
                            voicetxt = LoadTextureFromResource("iiMenu.Resources.speak.png");
                        }
                        volIndicator.GetComponent<Renderer>().material.mainTexture = voicetxt;
                        volIndicator.GetComponent<Renderer>().material.color = PlayerIsTagged(vrrig) ? (Color)new Color32(255, 111, 0, 255) : vrrig.playerColor;
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f);
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.8f;
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    }
                }
            }
        }

        private static Material voiceMat = null;
        public static void VoiceIndicators()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                    {
                        size = recorder.Loudness * 3f;
                    }
                    if (size > 0f)
                    {
                        GameObject volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                        UnityEngine.Object.Destroy(volIndicator, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        if (voiceMat == null)
                        {
                            voiceMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                            if (voicetxt == null)
                            {
                                voicetxt = LoadTextureFromResource("iiMenu.Resources.speak.png");
                            }
                            voiceMat.mainTexture = voicetxt;

                            voiceMat.SetFloat("_Surface", 1);
                            voiceMat.SetFloat("_Blend", 0);
                            voiceMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                            voiceMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                            voiceMat.SetFloat("_ZWrite", 0);
                            voiceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                            voiceMat.renderQueue = (int)RenderQueue.Transparent;

                            voiceMat.SetFloat("_Glossiness", 0f);
                            voiceMat.SetFloat("_Metallic", 0f);
                        }
                        volIndicator.GetComponent<Renderer>().material = voiceMat;
                        volIndicator.GetComponent<Renderer>().material.color = PlayerIsTagged(vrrig) ? (Color)new Color32(255, 111, 0, 255) : vrrig.playerColor;
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f);
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.8f; ;
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    }
                }
            }
        }

        private static GameObject l = null;
        private static GameObject r = null;

        private static void UpdateLimbColor()
        {
            Color limbcolor = GorillaTagger.Instance.offlineVRRig.playerColor;
            if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                limbcolor = new Color32(255, 111, 0, 255);
            }

            l.GetComponent<Renderer>().material.color = limbcolor;
            r.GetComponent<Renderer>().material.color = limbcolor;
        }

        public static void StartNoLimb()
        {
            l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(l.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

            l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(r.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(r.GetComponent<SphereCollider>());

            r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            UpdateLimbColor();
        }

        public static void NoLimbMode()
        {
            l.transform.position = TrueLeftHand().position;
            r.transform.position = TrueRightHand().position;
            GorillaTagger.Instance.offlineVRRig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
            GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = new Color(GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.r, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.g, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.b, 0f);
            UpdateLimbColor();
        }

        public static void EndNoLimb()
        {
            UnityEngine.Object.Destroy(l);
            UnityEngine.Object.Destroy(r);

            GorillaTagger.Instance.offlineVRRig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
            GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = new Color(GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.r, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.g, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.b, 1f);
        }

        public static void CasualTracers()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject line = new GameObject("Line");
                    if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void InfectionTracers()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;
            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                            liner.SetPosition(1, vrrig.transform.position);
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, GorillaLocomotion.Player.Instance.rightControllerTransform.position);
                            liner.SetPosition(1, vrrig.transform.position);
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject line = new GameObject("Line");
                        if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                        LineRenderer liner = line.AddComponent<LineRenderer>();
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                        liner.SetPosition(1, vrrig.transform.position);
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntTracers()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    GameObject line = new GameObject("Line");
                    if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    GameObject line = new GameObject("Line");
                    if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void CasualBoneESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                UnityEngine.Color thecolor = vrrig.playerColor;
                if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void InfectionBoneESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                            if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                            liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                            UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                            for (int i = 0; i < bones.Count<int>(); i += 2)
                            {
                                liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                                liner.startWidth = 0.025f;
                                liner.endWidth = 0.025f;

                                liner.startColor = thecolor;
                                liner.endColor = thecolor;

                                liner.material.shader = Shader.Find("GUI/Text Shader");

                                liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                                UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                            }
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                            if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                            liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                            UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                            for (int i = 0; i < bones.Count<int>(); i += 2)
                            {
                                liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                                liner.startWidth = 0.025f;
                                liner.endWidth = 0.025f;

                                liner.startColor = thecolor;
                                liner.endColor = thecolor;

                                liner.material.shader = Shader.Find("GUI/Text Shader");

                                liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                                UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                        if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                        liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        for (int i = 0; i < bones.Count<int>(); i += 2)
                        {
                            liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                            liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                            UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
            }
        }

        public static void HuntBoneESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetOrAddComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void CasualChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                }
            }
        }

        public static void InfectionChams()
        {
            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.mainSkin.material.color = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        }
                        else
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.mainSkin.material.color = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = vrrig.playerColor;
                    }
                }
            }
        }

        public static void HuntChams()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                } else {
                    if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = Color.red;
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    }
                    else
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    }
                }
            }
        }

        public static void DisableChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
        }

        public static void CasualBeacons()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject line = new GameObject("Line");
                    if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void InfectionBeacons()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                            liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                            liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject line = new GameObject("Line");
                        if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                        LineRenderer liner = line.AddComponent<LineRenderer>();
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                        liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntBeacons()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    GameObject line = new GameObject("Line");
                    if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    GameObject line = new GameObject("Line");
                    if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void CasualBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f,0.5f,0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void InfectionBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            box.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            box.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        box.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void CasualHollowBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().enabled = false;

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
            }
        }

        public static void HollowInfectionBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().enabled = false;

                            GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            UnityEngine.Object.Destroy(box);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().enabled = false;

                            GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            UnityEngine.Object.Destroy(box);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        box.GetComponent<Renderer>().enabled = false;

                        GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        UnityEngine.Object.Destroy(box);
                    }
                }
            }
        }

        public static void HollowHuntBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().enabled = false;

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().enabled = false;

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
            }
        }

        public static void CasualBreadcrumbs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
            }
        }

        public static void InfectionBreadcrumbs()
        {
            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                            UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                            sphere.GetComponent<Renderer>().material.color = thecolor;
                            sphere.transform.position = vrrig.transform.position;
                            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            UnityEngine.Object.Destroy(sphere, 10f);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                            UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                            sphere.GetComponent<Renderer>().material.color = thecolor;
                            sphere.transform.position = vrrig.transform.position;
                            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            UnityEngine.Object.Destroy(sphere, 10f);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                        UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                        sphere.GetComponent<Renderer>().material.color = thecolor;
                        sphere.transform.position = vrrig.transform.position;
                        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        UnityEngine.Object.Destroy(sphere, 10f);
                    }
                }
            }
        }

        public static void HuntBreadcrumbs()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
            }
        }

        public static void CasualDistanceESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    UnityEngine.Color thecolor2 = Color.white;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                    if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                    GameObject go = new GameObject("Dist");
                    if (GetIndex("Hidden on Camera").enabled) { go.layer = 19; }
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.fontSize = 18;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.color = thecolor2;
                    go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                    textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                    bg.transform.parent = go.transform;
                    bg.transform.localPosition = Vector3.zero;
                    bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    bg.GetComponent<Renderer>().material.color = thecolor;
                    textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void InfectionDistanceESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            UnityEngine.Color thecolor2 = Color.white;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                            if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                            GameObject go = new GameObject("Dist");
                            if (GetIndex("Hidden on Camera").enabled) { go.layer = 19; }
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 18;
                            textMesh.fontStyle = activeFontStyle;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;
                            textMesh.color = thecolor2;
                            go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                            textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                            bg.transform.parent = go.transform;
                            bg.transform.localPosition = Vector3.zero;
                            bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                            bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            bg.GetComponent<Renderer>().material.color = thecolor;
                            textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                            go.transform.LookAt(Camera.main.transform.position);
                            go.transform.Rotate(0f, 180f, 0f);
                            UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            UnityEngine.Color thecolor2 = Color.white;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                            if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                            GameObject go = new GameObject("Dist");
                            if (GetIndex("Hidden on Camera").enabled) { go.layer = 19; }
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 18;
                            textMesh.fontStyle = activeFontStyle;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;
                            textMesh.color = thecolor2;
                            go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                            textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                            bg.transform.parent = go.transform;
                            bg.transform.localPosition = Vector3.zero;
                            bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                            bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            bg.GetComponent<Renderer>().material.color = thecolor;
                            textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                            go.transform.LookAt(Camera.main.transform.position);
                            go.transform.Rotate(0f, 180f, 0f);
                            UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        UnityEngine.Color thecolor2 = Color.white;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                        if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                        GameObject go = new GameObject("Dist");
                        if (GetIndex("Hidden on Camera").enabled) { go.layer = 19; }
                        TextMesh textMesh = go.AddComponent<TextMesh>();
                        textMesh.fontSize = 18;
                        textMesh.fontStyle = activeFontStyle;
                        textMesh.characterSize = 0.1f;
                        textMesh.anchor = TextAnchor.MiddleCenter;
                        textMesh.alignment = TextAlignment.Center;
                        textMesh.color = thecolor2;
                        go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                        textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                        bg.transform.parent = go.transform;
                        bg.transform.localPosition = Vector3.zero;
                        bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                        bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        bg.GetComponent<Renderer>().material.color = thecolor;
                        textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                        go.transform.LookAt(Camera.main.transform.position);
                        go.transform.Rotate(0f, 180f, 0f);
                        UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntDistanceESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    UnityEngine.Color thecolor2 = Color.white;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                    if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                    GameObject go = new GameObject("Dist");
                    if (GetIndex("Hidden on Camera").enabled) { go.layer = 19; }
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.fontSize = 18;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.color = thecolor2;
                    go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                    textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                    bg.transform.parent = go.transform;
                    bg.transform.localPosition = Vector3.zero;
                    bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    bg.GetComponent<Renderer>().material.color = thecolor;
                    textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    UnityEngine.Color thecolor2 = Color.white;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                    if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                    GameObject go = new GameObject("Dist");
                    if (GetIndex("Hidden on Camera").enabled) { go.layer = 19; }
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.fontSize = 18;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.color = thecolor2;
                    go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                    textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                    bg.transform.parent = go.transform;
                    bg.transform.localPosition = Vector3.zero;
                    bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    bg.GetComponent<Renderer>().material.color = thecolor;
                    textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void ShowButtonColliders()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                {
                    { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
                    DelayChangeStep = Time.frameCount;
                }
            }

            GameObject left = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            left.transform.parent = GorillaTagger.Instance.leftHandTransform;
            left.GetComponent<Renderer>().material.color = bgColorA;
            left.transform.localPosition = pointerOffset;
            left.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            UnityEngine.Object.Destroy(left.GetComponent<SphereCollider>());
            UnityEngine.Object.Destroy(left, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

            GameObject right = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            right.transform.parent = GorillaTagger.Instance.rightHandTransform;
            right.GetComponent<Renderer>().material.color = bgColorA;
            right.transform.localPosition = pointerOffset;
            right.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            UnityEngine.Object.Destroy(right.GetComponent<SphereCollider>());
            UnityEngine.Object.Destroy(right, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
        }
    }
}
