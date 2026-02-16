/*
 * ii's Stupid Menu  Mods/Visuals.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using GameObjectScheduling;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Classes.Menu;
using iiMenu.Classes.Mods;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore;
using UnityEngine.UI;
using WebSocketSharp;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.GameModeUtilities;
using static iiMenu.Utilities.RigUtilities;
using Object = UnityEngine.Object;

namespace iiMenu.Mods
{
    public class Visuals
    {
        public static readonly Dictionary<(long, float), GameObject> auraPool = new Dictionary<(long, float), GameObject>();
        public static void VisualizeAura(Vector3 position, float range, Color color, long? indexId = null, float alpha = 0.25f)
        {
            long index = indexId ?? BitPackUtils.PackWorldPosForNetwork(position);
            var key = (index, range);

            if (!auraPool.TryGetValue(key, out GameObject visualizeGO))
            {
                visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(visualizeGO.GetComponent<Collider>());

                auraPool.Add(key, visualizeGO);
            }

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = new Vector3(range, range, range);

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;
        }

        public static readonly Dictionary<(Vector3, Quaternion, Vector3), GameObject> cubePool = new Dictionary<(Vector3, Quaternion, Vector3), GameObject>();
        public static void VisualizeCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float alpha = 0.25f)
        {
            var key = (position, rotation, scale);

            if (!cubePool.TryGetValue(key, out GameObject visualizeGO))
            {
                visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.Destroy(visualizeGO.GetComponent<Collider>());

                cubePool.Add(key, visualizeGO);
            }

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = scale;
            visualizeGO.transform.rotation = rotation;

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;
        }

        public static GameObject VisualizeAuraObject(Vector3 position, float range, Color color, float alpha = 0.25f)
        {
            GameObject visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(visualizeGO.GetComponent<Collider>());

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = new Vector3(range, range, range);

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;

            return visualizeGO;
        }

        public static GameObject VisualizeCubeObject(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float alpha = 0.25f)
        {
            GameObject visualizeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(visualizeGO.GetComponent<Collider>());

            visualizeGO.SetActive(true);

            visualizeGO.transform.position = position;
            visualizeGO.transform.localScale = scale;
            visualizeGO.transform.rotation = rotation;

            if (Buttons.GetIndex("Hidden on Camera").enabled)
                visualizeGO.layer = 19;

            Renderer auraRenderer = visualizeGO.GetComponent<Renderer>();

            Color clr = color;
            clr.a = alpha;
            auraRenderer.material.shader = Shader.Find("GUI/Text Shader");
            auraRenderer.material.color = clr;

            return visualizeGO;
        }

        public static void ConductDebug()
        {
            string text = "";
            text += "<color=blue><b>ii's Stupid Menu </b></color>" + PluginInfo.Version + "\\n \\n";
            
            string red = "<color=red>" + MathF.Floor(PlayerPrefs.GetFloat("redValue") * 255f) + "</color>";
            string green = ", <color=green>" + MathF.Floor(PlayerPrefs.GetFloat("greenValue") * 255f) + "</color>";
            string blue = ", <color=blue>" + MathF.Floor(PlayerPrefs.GetFloat("blueValue") * 255f) + "</color>";
            string redS = "<color=red>" + MathF.Round(PlayerPrefs.GetFloat("redValue") * 9f) + "</color>";
            string greenS = ", <color=green>" + MathF.Round(PlayerPrefs.GetFloat("greenValue") * 9f) + "</color>";
            string blueS = ", <color=blue>" + MathF.Round(PlayerPrefs.GetFloat("blueValue") * 9f) + "</color>";
            text += "<color=green>Color</color><color=grey>:</color> " + red + green + blue + " <color=grey>[</color>"+ redS + greenS + blueS +"<color=grey>]</color>\\n";

            string master = PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient ? "<color=grey> [</color><color=red>Master</color><color=grey>]</color>" : "";
            text += "<color=green>Name</color><color=grey>:</color> " + PhotonNetwork.LocalPlayer?.NickName + master + "\\n";

            text += "<color=green>ID</color><color=grey>:</color> " + (Settings.hideId ? "Hidden" : PhotonNetwork.LocalPlayer?.UserId) + "\\n";
            text += "<color=green>Clip</color><color=grey>:</color> " + (GUIUtility.systemCopyBuffer?.Length > 35 ? GUIUtility.systemCopyBuffer[..35] : GUIUtility.systemCopyBuffer) + "\\n";
            text += lastDeltaTime + " <color=green>FPS</color> <color=grey>|</color> " + PhotonNetwork.GetPing() + " <color=green>Ping</color>\\n";

            string room = PhotonNetwork.InRoom ? NetworkSystem.Instance.SessionIsPrivate ? "Private" : "Public" : "Not in room";
            text += "<color=green>" + NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper() + "</color> " + PhotonNetwork.PlayerList.Length + " <color=green>Players</color> <color=grey>|</color> " + room + "\\n \\n";

            string admin = "";
            if (Time.time > 5f)
            {
                if (ServerData.Administrators.TryGetValue(PhotonNetwork.LocalPlayer?.UserId ?? string.Empty, out var administrator))
                    admin = " <color=grey>|</color> <color=red>Console " + (ServerData.SuperAdministrators.Contains(administrator) ? "Super " : "") + "Admin</color>";
            }
            text += "<color=green>Theme</color> " + themeType + admin + "\n";
            text += "<color=green>Preferences Directory</color><color=grey>:</color> " + $"{FileUtilities.GetGamePath()}/{PluginInfo.BaseDirectory}";

            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().SafeSetText(text);
        }

        public static void ToggleSnow(bool enable)
        {
            GameObject snowObject = GetObject("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight").transform.Find("snow").gameObject;
            snowObject.SetActive(enable);
            snowObject.transform.position += Vector3.one * (enable ? 0.001f : -0.001f);
            snowObject.GetComponent<TimeOfDayDependentAudio>().enabled = !enable;
            snowObject.transform.Find("snow partic").gameObject.SetActive(enable);
        }

        public static void WeatherChange(bool rain)
        {
            for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                BetterDayNightManager.instance.weatherCycle[i] = rain ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None;
        }

        public static void DisableFog() =>
            ZoneShaderSettings.activeInstance.SetGroundFogValue(Color.clear, 0f, 0f, 0f);

        public static void EnableFog() =>
            ZoneShaderSettings.activeInstance.SetGroundFogValue(new Color(0.9569f, 0.6941f, 0.502f, 0.1216f), 40f, 10f, 40f);

        private static readonly List<TimeOfDayDependentAudio> disabledAmbientObjects = new List<TimeOfDayDependentAudio>();
        public static void DisableAmbience()
        {
            foreach (TimeOfDayDependentAudio ambientObject in GetAllType<TimeOfDayDependentAudio>())
            {
                if (ambientObject.gameObject.activeSelf)
                {
                    disabledAmbientObjects.Add(ambientObject);
                    ambientObject.gameObject.SetActive(false);
                }
            }
        }

        public static void EnableAmbience()
        {
            foreach (TimeOfDayDependentAudio ambientObject in disabledAmbientObjects)
                ambientObject.gameObject.SetActive(true);

            disabledAmbientObjects.Clear();
        }

        public static void ResetFog() =>
            ZoneShaderSettings.activeInstance.CopySettings(ZoneShaderSettings.defaultsInstance);

        /*
        public static void SpawnLightning() =>
            ManagerRegistry.LightningManager.DoLightningStrike();

        public static float GetTimeUntilNextLightningStrike()
        {
            if (ManagerRegistry.LightningManager.lightningTimestampsRealtime == null ||
                ManagerRegistry.LightningManager.lightningTimestampsRealtime.Count == 0 ||
                ManagerRegistry.LightningManager.nextLightningTimestampIndex < 0 ||
                ManagerRegistry.LightningManager.nextLightningTimestampIndex >= ManagerRegistry.LightningManager.lightningTimestampsRealtime.Count)
                return -1f; 

            float nextStrikeTime = ManagerRegistry.LightningManager.lightningTimestampsRealtime[ManagerRegistry.LightningManager.nextLightningTimestampIndex];
            float timeUntilStrike = nextStrikeTime - Time.realtimeSinceStartup;

            return timeUntilStrike < 0f ? 0f : timeUntilStrike;
        }

        public static void StrikeTimeOverlay()
        {
            float timeUntilStrike = GetTimeUntilNextLightningStrike();
            if (timeUntilStrike < 0f)
            {
                NotificationManager.information.Remove("Lightning");
                return;
            }
            NotificationManager.information["Lightning"] = $"{timeUntilStrike:F1}s";
        }
        */

        public static void CoreESP()
        {
            if (!PhotonNetwork.InRoom)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            Color coreESPColor = fmt ? backgroundColor.GetCurrentColor() : new Color(0.41f, 0.05f, 0.70f);
            if (tt)
                coreESPColor.a = 0.5f;

            List<GameEntity> cores = ManagerRegistry.GhostReactor.GameEntityManager.entities.Where(entity => entity != null && entity.typeId == Overpowered.ObjectByName["GhostReactorCollectibleCore"]).ToList();
            if (cores.Count <= 0)
                return;
            
            for (int i = 0; i < cores.Count; i++)
            {
                Transform corePosition = cores[i].transform;
                VisualizeAura(corePosition.position, 0.15f, coreESPColor, i + 29875, coreESPColor.a);
            }
        }

        public static void CritterESP()
        {
            if (!PhotonNetwork.InRoom)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            Color critterESPColor = fmt ? backgroundColor.GetCurrentColor() : Color.green;
            if (tt)
                critterESPColor.a = 0.5f;

            List<CrittersPawn> actors = CrittersManager.instance.crittersPawns;
            if (actors.Count <= 0)
                return;

            for (int i = 0; i < actors.Count; i++)
            {
                Transform transform = actors[i].transform;
                VisualizeAura(transform.position, 0.15f, critterESPColor, i - 192398, critterESPColor.a);
            }
        }

        public static void CreatureESP()
        {
            if (!PhotonNetwork.InRoom)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            Color critterESPColor = fmt ? backgroundColor.GetCurrentColor() : Color.green;
            if (tt)
                critterESPColor.a = 0.5f;

            ThrowableBug[] bugs = GetAllType<ThrowableBug>();
            if (bugs.Length <= 0)
                return;

            for (int i = 0; i < bugs.Length; i++)
            {
                Transform transform = bugs[i].transform;
                VisualizeAura(transform.position, 0.15f, critterESPColor, i - 201782, critterESPColor.a);
            }
        }

        public static void EnemyESP()
        {
            if (!PhotonNetwork.InRoom)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            Color coreESPColor = fmt ? backgroundColor.GetCurrentColor() : new Color(0.41f, 0.05f, 0.70f);
            if (tt)
                coreESPColor.a = 0.5f;

            List<GameEntity> enemies = ManagerRegistry.GhostReactor.GameEntityManager.entities.Where(entity => entity != null && entity.gameObject.name.ToLower().Contains("enemy")).ToList();
            if (enemies.Count <= 0)
                return;

            for (int i = 0; i < enemies.Count; i++)
            {
                Transform enemy = enemies[i].transform;
                VisualizeAura(enemy.position, 0.15f, coreESPColor, i + 451980, coreESPColor.a);
            }
        }

        public static void ResourceESP()
        {
            if (!PhotonNetwork.InRoom)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            Color coreESPColor = fmt ? backgroundColor.GetCurrentColor() : Color.green;
            if (tt)
                coreESPColor.a = 0.5f;

            List<GameEntity> resources = ManagerRegistry.SuperInfection.GameEntityManager.entities.Where(entity => entity != null && entity.gameObject.name.Contains("Resource")).ToList();
            if (resources.Count <= 0)
                return;

            for (int i = 0; i < resources.Count; i++)
            {
                Transform resource = resources[i].transform;
                VisualizeAura(resource.position, 0.15f, coreESPColor, i + 451961280, coreESPColor.a);
            }
        }

        private static bool previousFullbrightStatus;
        public static void SetFullbrightStatus(bool fullBright)
        {
            if (fullBright)
            {
                previousFullbrightStatus = GameLightingManager.instance.customVertexLightingEnabled;
                GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
            } else
            {
                if (previousFullbrightStatus)
                    GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
            }
        }

        private static float removeBlindfoldDelay;
        public static void RemoveBlindfold()
        {
            if (PhotonNetwork.InRoom && Time.time > removeBlindfoldDelay)
            {
                removeBlindfoldDelay = Time.time + 0.5f;
                GameObject mainCamera = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/Main Camera");
                int childCount = mainCamera.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    GameObject v = mainCamera.transform.GetChild(i).gameObject;
                    if (v.name == "PropHunt_Blindfold_ForCameras_Prefab(Clone)")
                        Object.Destroy(v);
                }
            }
        }

        public static void WatchOn()
        {
            GameObject mainwatch = VRRig.LocalRig.transform.Find("rig/body_pivot/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)").gameObject;
            regwatchobject = Object.Instantiate(mainwatch, rightHand ? VRRig.LocalRig.transform.Find("rig/body_pivot/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : VRRig.LocalRig.transform.Find("rig/body_pivot/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);
            Object.Destroy(regwatchobject.GetComponent<GorillaHuntComputer>());
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

            regwatchShell.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;

            if (rightHand)
            {
                regwatchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
                regwatchShell.transform.parent.localPosition += new Vector3(0.025f, 0f, 0f);
                regwatchShell.transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
            }
        }

        private static TMP_SpriteAsset _infoSpriteAsset;
        public static TMP_SpriteAsset InfoSprites
        {
            get
            {
                if (_infoSpriteAsset == null)
                {
                    _infoSpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
                    _infoSpriteAsset.name = "iiMenu_InfoSprites";

                    var textureList = new List<Texture2D>();
                    var spriteDataList = new List<(string name, int index)>();

                    void AddSprite(string name, Texture2D tex)
                    {
                        spriteDataList.Add((name, textureList.Count));
                        textureList.Add(tex);
                    }

                    AddSprite("Steam", LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/steam.png", "Images/Mods/Visuals/steam.png"));
                    AddSprite("Standalone", LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/oculus.png", "Images/Mods/Visuals/oculus.png"));
                    AddSprite("PC", LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/pc.png", "Images/Mods/Visuals/pc.png"));
                    for (int i = 1; i <= 5; i++)
                        AddSprite($"Ping{i}", LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/ping{i}.png", $"Images/Mods/Visuals/ping{i}.png"));

                    int maxSize = 512;
                    Texture2D spriteSheet = new Texture2D(maxSize, maxSize);
                    Rect[] rects = spriteSheet.PackTextures(textureList.ToArray(), 2, maxSize);

                    _infoSpriteAsset.spriteSheet = spriteSheet;
                    _infoSpriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"))
                    {
                        mainTexture = spriteSheet
                    };

                    _infoSpriteAsset.spriteInfoList = new List<TMP_Sprite>();
                    Traverse.Create(_infoSpriteAsset).Field("m_Version").SetValue("1.1.0"); // TextMeshPro kills itself unless this is set.

                    _infoSpriteAsset.spriteGlyphTable.Clear();
                    for (int i = 0; i < spriteDataList.Count; i++)
                    {
                        var rect = rects[i];

                        var glyph = new TMP_SpriteGlyph
                        {
                            index = (uint)i,
                            metrics = new GlyphMetrics(
                                width: rect.width * spriteSheet.width,
                                height: rect.height * spriteSheet.height,
                                bearingX: -(rect.width * spriteSheet.width) / 2f,
                                bearingY: rect.height * spriteSheet.height * 0.8f,
                                advance: rect.width * spriteSheet.width
                            ),
                            glyphRect = new GlyphRect(
                                x: (int)(rect.x * spriteSheet.width),
                                y: (int)(rect.y * spriteSheet.height),
                                width: (int)(rect.width * spriteSheet.width),
                                height: (int)(rect.height * spriteSheet.height)
                            ),
                            scale = 1f,
                            atlasIndex = 0
                        };
                        _infoSpriteAsset.spriteGlyphTable.Add(glyph);
                    }

                    _infoSpriteAsset.spriteCharacterTable.Clear();
                    for (int i = 0; i < spriteDataList.Count; i++)
                    {
                        var (name, _) = spriteDataList[i];

                        var character = new TMP_SpriteCharacter(0xFFFE, _infoSpriteAsset.spriteGlyphTable[i])
                        {
                            name = name,
                            scale = 1f,
                            glyphIndex = (uint)i
                        };
                        _infoSpriteAsset.spriteCharacterTable.Add(character);
                    }

                    _infoSpriteAsset.UpdateLookupTables();
                }
                return _infoSpriteAsset;
            }
        }

        public static void LeaderboardInfo()
        {
            foreach (GorillaScoreBoard scoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
            {
                scoreboard.boardText.richText = true;
                scoreboard.boardText.spriteAsset ??= InfoSprites;
            }
        }

        public static bool infoWatchMenuName;
        public static bool infoWatchTime;
        public static bool infoWatchClip;
        public static bool infoWatchFPS;
        public static bool infoWatchCode;
        public static void WatchStep()
        {
            bool defaultWatch = !infoWatchMenuName && !infoWatchTime && !infoWatchClip && !infoWatchFPS && !infoWatchCode;
            string watchText = "";

            Text watchTextComponent = regwatchText.GetComponent<Text>();

            if (infoWatchMenuName || defaultWatch) watchTextComponent.text = "ii's Stupid Menu\n<color=grey>";
            if (doCustomName && (infoWatchMenuName || defaultWatch))
                watchTextComponent.text = NoRichtextTags(customMenuName) + "\n<color=grey>";
            if (!infoWatchMenuName && !defaultWatch)
                watchTextComponent.text = "<color=grey>";
            
            if (infoWatchFPS || defaultWatch) watchText += lastDeltaTime + " FPS\n";
            if (infoWatchTime || defaultWatch) watchText += DateTime.Now.ToString("hh:mm tt") + "\n";
            if (infoWatchCode) watchText += (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room") + "\n";
            if (infoWatchClip) watchText += "Clip: " + (GUIUtility.systemCopyBuffer.Length > 20 ? GUIUtility.systemCopyBuffer[..20] : GUIUtility.systemCopyBuffer) + "\n";

            watchText += "</color>";
            watchTextComponent.color = textColors[0].GetCurrentColor();
            watchTextComponent.text += watchText;
            if (lowercaseMode)
                watchTextComponent.text = watchTextComponent.text.ToLower();
            if (uppercaseMode)
                watchTextComponent.text = watchTextComponent.text.ToUpper();
        }

        public static void WatchOff() =>
            Object.Destroy(regwatchobject);

        public static Material oldSkyMat;
        public static void DoCustomSkyboxColor()
        {
            GameObject sky = GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky");
            oldSkyMat = sky.GetComponent<Renderer>().material;
        }

        public static void CustomSkyboxColor() =>
            GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky").GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;

        public static void UnCustomSkyboxColor()
        {
            GameObject sky = GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky");
            sky.GetComponent<Renderer>().material = oldSkyMat;
        }

        public static TrailRenderer trailRenderer;
        public static void DrawGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun(GTPlayer.Instance.locomotionEnabledLayers);
                GameObject NewPointer = GunData.NewPointer;
                
                if (trailRenderer == null)
                {
                    GameObject trailHolder = new GameObject("iiMenu_DrawGunTrail");

                    trailRenderer = trailHolder.AddComponent<TrailRenderer>();
                    trailRenderer.startWidth = 0.1f;
                    trailRenderer.endWidth = 0.1f;

                    trailRenderer.minVertexDistance = 0.05f;

                    trailRenderer.material.shader = Shader.Find("GUI/Text Shader");
                    trailRenderer.time = float.PositiveInfinity;
                    
                    trailRenderer.startColor = Color.black;
                    trailRenderer.endColor = Color.black;

                    if (smoothLines)
                    {
                        trailRenderer.numCapVertices = 10;
                        trailRenderer.numCornerVertices = 5;
                    }
                }

                trailRenderer.emitting = GetGunInput(true);
                trailRenderer.gameObject.transform.position = NewPointer.transform.position;
            }
        }

        public static void DisableDrawGun()
        {
            if (trailRenderer != null)
                Object.Destroy(trailRenderer.gameObject);

            trailRenderer = null;
        }

        private static Material tapMat;
        private static Texture2D tapTxt;
        private static Texture2D warningTxt;

        private static readonly Dictionary<VRRig, object[]> handTaps = new Dictionary<VRRig, object[]>();
        public static void OnHandTapGamesenseRing(VRRig rig, Vector3 position)
        {
            if (rig.isLocal)
                return;

            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position) > 20f)
                return;

            handTaps[rig] = new object[]
            {
                rig,
                position,
                Time.time,
                null
            };
        }

        public static void GamesenseRing()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            List<object[]> handTapValues = handTaps.Values.ToList();

            for (int i = 0; i < handTaps.Count; i++)
            {
                object[] handTapData = handTapValues[i];
                VRRig rig = (VRRig)handTapData[0];
                Vector3 position = (Vector3)handTapData[1];

                float timestamp = (float)handTapData[2];
                GameObject gameObject = (GameObject)handTapData[3];

                if (Time.time > timestamp + 1f)
                {
                    toRemove.Add(rig);
                    continue;
                }

                if (gameObject == null)
                {
                    gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(gameObject.GetComponent<Collider>());

                    if (tapMat == null)
                    {
                        tapMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                        if (tapTxt == null)
                            tapTxt = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/footstep.png", "Images/Mods/Visuals/footstep.png");

                        if (warningTxt == null)
                            warningTxt = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/warning.png", "Images/Mods/Visuals/warning.png");

                        tapMat.SetFloat("_Surface", 1);
                        tapMat.SetFloat("_Blend", 0);
                        tapMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        tapMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        tapMat.SetFloat("_ZWrite", 0);
                        tapMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        tapMat.renderQueue = (int)RenderQueue.Transparent;
                    }

                    Color targetColor = rig.GetColor();

                    if (hoc)
                        gameObject.layer = 19;

                    if (fmt)
                        targetColor = backgroundColor.GetCurrentColor();
                    if (tt)
                        targetColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0.5f);

                    gameObject.GetComponent<Renderer>().material = tapMat;
                    gameObject.GetComponent<Renderer>().material.mainTexture = VRRig.LocalRig.IsTagged() ? rig.IsTagged() ? tapTxt : warningTxt : rig.IsTagged() ? warningTxt : tapTxt;
                    gameObject.GetComponent<Renderer>().material.color = targetColor;

                    handTaps[rig][3] = gameObject;
                }

                Renderer renderer = gameObject.GetComponent<Renderer>();

                Vector3 toTarget = position - Camera.main.transform.position;
                toTarget.Normalize();

                Vector3 camForward = Camera.main.transform.forward.normalized;
                Vector3 camRight = Camera.main.transform.right.normalized;
                Vector3 camUp = Camera.main.transform.up.normalized;

                float x = Vector3.Dot(toTarget, camRight);
                float y = Vector3.Dot(toTarget, camUp);

                Vector2 dirInPlane = new Vector2(x, y).normalized;

                float ringRadius = 0.2f;
                Vector3 ringOffset = (camRight * dirInPlane.x + camUp * dirInPlane.y) * ringRadius;
                Vector3 ringCenter = Camera.main.transform.position + camForward * 0.5f;

                Vector3 finalPos = ringCenter + ringOffset;
                gameObject.transform.position = finalPos;

                gameObject.transform.rotation = Quaternion.LookRotation(finalPos - Camera.main.transform.position, -Camera.main.transform.up);
                Camera.main.transform.forward.X_Z();

                float t = Mathf.Lerp(1f, 0f, Time.time - timestamp);

                Color color = renderer.material.color;
                color.a = Mathf.Clamp01(t);
                renderer.material.color = color;

                gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.01f);
            }

            foreach (VRRig removal in toRemove)
            {
                if ((GameObject)handTaps[removal][3] != null)
                    Object.Destroy((GameObject)handTaps[removal][3]);

                handTaps.Remove(removal);
            }
        }

        public static void DisableGamesenseRing()
        {
            foreach (var removal in handTaps.Where(removal => removal.Value[3] != null))
            {
                Object.Destroy((GameObject)removal.Value[3]);
            }

            handTaps.Clear();

            HandTapPatch.OnHandTap -= OnHandTapGamesenseRing;
        }

        public static bool PerformanceVisuals;

        public static float PerformanceModeStep = 0.2f;
        public static int PerformanceModeStepIndex = 2;
        public static void ChangePerformanceModeVisualStep(bool positive = true)
        {
            if (positive)
                PerformanceModeStepIndex++;
            else
                PerformanceModeStepIndex--;

            PerformanceModeStepIndex %= 11;
            if (PerformanceModeStepIndex < 0)
                PerformanceModeStepIndex = 10;

            PerformanceModeStep = PerformanceModeStepIndex / 10f;
            Buttons.GetIndex("Change Performance Visuals Step").overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>" + PerformanceModeStep + "</color><color=grey>]</color>";
        }

        public static float PerformanceVisualDelay;
        public static int DelayChangeStep;

        public static readonly Dictionary<string, GameObject> labelDictionary = new Dictionary<string, GameObject>();
        public static readonly Dictionary<bool, List<int>> labelDistances = new Dictionary<bool, List<int>>();
        public static float GetLabelDistance(bool leftHand)
        {
            if (!labelDistances.TryGetValue(leftHand, out List<int> frames))
            {
                frames = new List<int> { Time.frameCount };
                labelDistances[leftHand] = frames;
                return 0.2f;
            }

            if (frames[0] == Time.frameCount)
            {
                frames.Add(Time.frameCount);
                return 0.1f + Time.frameCount * 0.1f;
            }

            frames.Clear();
            frames.Add(Time.frameCount);
            return 0.1f + frames.Count * 0.1f;
        }

        /// <summary>
        /// Displays a label with the specified text and color at the position of the player's left or right hand. If a
        /// label with the given code name does not exist, a new label is created.
        /// </summary>
        /// <remarks>If the label already exists, its properties are updated; otherwise, a new label is
        /// created and added. The label is positioned and oriented to face the main camera, and its scale may be
        /// adjusted based on the player's scale. The label is always set active when this method is called.</remarks>
        /// <param name="codeName">The unique identifier for the label. If a label with this code name does not exist, a new label is created.</param>
        /// <param name="leftHand">Indicates whether the label should be positioned at the left hand (<see langword="true"/>) or right hand
        /// (<see langword="false"/>) of the player.</param>
        /// <param name="text">The text content to display on the label.</param>
        /// <param name="color">The color to apply to the label's text.</param>
        public static void GetLabel(string codeName, bool leftHand, string text, Color color)
        {
            if (!labelDictionary.TryGetValue(codeName, out GameObject go))
            {
                go = new GameObject(codeName);
                if (Buttons.GetIndex("Hidden Labels").enabled)
                    go.layer = 19;

                go.transform.localScale = Vector3.one * (0.25f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));

                labelDictionary.Add(codeName, go);
            }

            go.SetActive(true);
            
            TextMeshPro TextMeshPro = go.GetOrAddComponent<TextMeshPro>();
            TextMeshPro.color = color;
            TextMeshPro.fontSize = 2.4f;
            TextMeshPro.SafeSetFontStyle(activeFontStyle);
            TextMeshPro.SafeSetFont(activeFont);
            TextMeshPro.alignment = TextAlignmentOptions.Center;

            TextMeshPro.SafeSetText(text);

            go.transform.position = (leftHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform).position + Vector3.up * (GetLabelDistance(leftHand) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
            go.transform.LookAt(Camera.main.transform.position);
            go.transform.Rotate(0f, 180f, 0f);
        }


        public static string OverallPlaytime;
        private static float playtime;
        public static void UpdatePlaytime()
        {
            CoroutineManager.instance.StartCoroutine(Updateplaytime());
        }
        private static IEnumerator Updateplaytime() 
        {
            playtime += Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(playtime);
            OverallPlaytime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            yield return new WaitForSeconds(0.1f);
        }

        public static void VelocityLabel()
        {
            if (DoPerformanceCheck())
                return;

            GetLabel
            (
                "Velocity",
                false,
                $"{GorillaTagger.Instance.rigidbody.linearVelocity.magnitude:F1}m/s",
                GorillaTagger.Instance.rigidbody.linearVelocity.magnitude >= GTPlayer.Instance.maxJumpSpeed ? Color.green : Color.white
            );
        }

        private static string FormatTimer(int seconds)
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            string timeString = $"{minutes:D2}:{remainingSeconds:D2}";

            return timeString;
        }

        private static float startTime;
        private static float endTime;
        private static bool lastWasTagged;
        public static void TimeLabel()
        {
            if (DoPerformanceCheck())
                return;

            if (PhotonNetwork.InRoom)
            {
                bool isThereTagged = InfectedList().Count > 0;

                if (isThereTagged)
                {
                    bool playerIsTagged = VRRig.LocalRig.IsTagged();
                    switch (playerIsTagged)
                    {
                        case true when !lastWasTagged:
                            endTime = Time.time - startTime;
                            break;
                        case false when lastWasTagged:
                            startTime = Time.time;
                            break;
                    }

                    lastWasTagged = playerIsTagged;

                    GetLabel
                    (
                        "Time",
                        false,
                        FormatTimer(Mathf.FloorToInt(playerIsTagged ? endTime : Time.time - startTime)),
                        playerIsTagged ? Color.green : Color.white
                    );
                }
                else
                    startTime = Time.time;
            }
        }

        public static void PingOverlay()
        {
            VRRig masterRig = PhotonNetwork.MasterClient?.VRRig();
            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient || masterRig == null || !playerPing.ContainsKey(masterRig))
            {
                NotificationManager.information["Ping"] = PhotonNetwork.GetPing() + "ms";
                return;
            }

            NotificationManager.information["Ping"] = (masterRig.GetPing() + PhotonNetwork.GetPing()) + "ms";
        }

        public static void NearbyTaggerOverlay()
        {
            if (DoPerformanceCheck())
                return;
            float closest = float.MaxValue;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.IsTagged() != VRRig.LocalRig.IsTagged())
                {
                    float dist = Vector3.Distance(GorillaTagger.Instance.headCollider.transform.position, vrrig.headMesh.transform.position);
                    if (dist < closest)
                        closest = dist;
                }
            }
            if (!Mathf.Approximately(closest, float.MaxValue))
                NotificationManager.information["Nearby"] = $"{closest:F1}m";
            else
                NotificationManager.information.Remove("Nearby");
        }

        public static void InfoOverlayGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    NotificationManager.information["Name"] = lockTarget.GetName();
                    NotificationManager.information["Color"] = lockTarget.GetColor().ToRGBString();
                    NotificationManager.information["ID"] = lockTarget.GetPlayer().UserId;
                    NotificationManager.information["Platform"] = lockTarget.GetPlatform();
                    NotificationManager.information["Ping"] = lockTarget.GetPing().ToString();
                    NotificationManager.information["FPS"] = lockTarget.fps.ToString();
                    NotificationManager.information["Creation Date"] = lockTarget.GetCreationDate();
                    NotificationManager.information["Turn"] = $"{lockTarget.turnType.ToTitleCase()} {lockTarget.turnFactor}";
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    NotificationManager.information.Remove("Name");
                    NotificationManager.information.Remove("Color");
                    NotificationManager.information.Remove("ID");
                    NotificationManager.information.Remove("Platform");
                    NotificationManager.information.Remove("Ping");
                    NotificationManager.information.Remove("FPS");
                    NotificationManager.information.Remove("Creation Date");
                    NotificationManager.information.Remove("Turn");
                }
            }
        }

        public static void EnableDebugHUD()
        {
            DebugHudStats debugStats = Camera.main.transform.Find("DebugCanvas").GetComponent<DebugHudStats>();

            debugStats.builder = new StringBuilder();
            debugStats.drawCallsRecorder = ProfilerRecorder.StartNew(
                ProfilerCategory.Render,
                "Draw Calls Count"
            );
            debugStats.trisRecorder = ProfilerRecorder.StartNew(
                ProfilerCategory.Render,
                "Tris Count"
            );

            debugStats.gameObject.SetActive(true);
            debugStats.text.gameObject.SetActive(true);

            debugStats.enabled = true;
        }

        public static void DisableDebugHUD()
        {
            DebugHudStats debugStats = Camera.main.transform.Find("DebugCanvas").GetComponent<DebugHudStats>();
            debugStats.gameObject.SetActive(false);
        }

        public static void NearbyTaggerLabel()
        {
            if (DoPerformanceCheck())
                return;

            if (!VRRig.LocalRig.IsTagged())
            {
                float closest = float.MaxValue;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig.IsTagged() != VRRig.LocalRig.IsTagged())
                    {
                        float dist = Vector3.Distance(GorillaTagger.Instance.headCollider.transform.position, vrrig.headMesh.transform.position);
                        if (dist < closest)
                            closest = dist;
                    }
                }

                if (!Mathf.Approximately(closest, float.MaxValue))
                {
                    Color colorn = Color.green;

                    if (closest < 30f)
                        colorn = Color.yellow;
                    
                    if (closest < 20f)
                        colorn = new Color32(255, 90, 0, 255);
                    
                    if (closest < 10f)
                        colorn = Color.red;

                    GetLabel("NearbyTagger", true, $"{closest:F1}m", colorn);
                }
            }
        }

        public static void LastLabel()
        {
            if (DoPerformanceCheck())
                return;

            if (PhotonNetwork.InRoom)
            {
                bool isThereTagged = InfectedList().Count > 0;
                int left = PhotonNetwork.PlayerList.Length - InfectedList().Count;

                if (isThereTagged)
                {
                    GetLabel
                    (
                        "LastLabel",
                        true,
                        left + " left",
                        left <= 1 && !VRRig.LocalRig.IsTagged() ? Color.green : Color.white
                    );
                }
            }
        }

        public static void FakeUnbanSelf()
        {
            PhotonNetworkController.Instance.UpdateTriggerScreens();
            GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
            GorillaComputer.instance.screenText.DisableFailedState();
            GorillaComputer.instance.functionSelectText.DisableFailedState();
        }

        private static GameObject visualizerObject;
        private static GameObject visualizerOutline;
        public static void CreateAudioVisualizer()
        {
            visualizerObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visualizerOutline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.Destroy(visualizerObject.GetComponent<Collider>());
            Object.Destroy(visualizerOutline.GetComponent<Collider>());
        }

        public static void AudioVisualizer()
        {
            visualizerObject.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
            visualizerOutline.GetComponent<Renderer>().material.color = buttonColors[0].GetCurrentColor();

            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GTPlayer.Instance.locomotionEnabledLayers);
            visualizerObject.transform.position = Ray.point;
            visualizerObject.transform.rotation = Quaternion.LookRotation(Ray.normal) * Quaternion.Euler(90f, 0f, 0f);

            float size = 0f;
            GorillaSpeakerLoudness recorder = VRRig.LocalRig.GetComponent<GorillaSpeakerLoudness>();
            if (recorder != null)
                size = recorder.Loudness;

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
            Object.Destroy(visualizerObject);
            Object.Destroy(visualizerOutline);
        }

        private static GameObject headPos;
        private static GameObject leftHandPos;
        private static GameObject rightHandPos;
        public static void ShowServerPosition()
        {
            if (headPos == null)
            {
                headPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(headPos.GetComponent<Collider>());
                headPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }

            if (leftHandPos == null)
            {
                leftHandPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(leftHandPos.GetComponent<Collider>());
                leftHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            if (rightHandPos == null)
            {
                rightHandPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(rightHandPos.GetComponent<Collider>());
                rightHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            headPos.transform.position = ServerPos;
            leftHandPos.transform.position = ServerLeftHandPos;
            rightHandPos.transform.position = ServerRightHandPos;
        }

        public static void ShowScheduledObjects()
        {
            foreach (GameObjectScheduler scheduledObject in GetAllType<GameObjectScheduler>())
            {
                scheduledObject.gameObject.SetActive(true);
                foreach (GameObject gameObject in scheduledObject.scheduledGameObject)
                    gameObject.SetActive(true);
                foreach (Transform child in scheduledObject.gameObject.transform)
                    child.gameObject.SetActive(true);
                scheduledObject.enabled = false;
            }
        }

        public static void DisableShowServerPosition()
        {
            if (headPos != null)
                Object.Destroy(headPos);

            if (leftHandPos != null)
                Object.Destroy(leftHandPos);

            if (rightHandPos != null)
                Object.Destroy(rightHandPos);
        }

        private static readonly Dictionary<VRRig, LineRenderer> predictions = new Dictionary<VRRig, LineRenderer>();
        public static void JumpPredictions()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in predictions.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value.gameObject);
            }

            foreach (VRRig rig in toRemove)
                predictions.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!predictions.TryGetValue(rig, out LineRenderer Line))
                {
                    GameObject LineObject = new GameObject("LineObject");
                    Line = LineObject.AddComponent<LineRenderer>();
                    if (smoothLines)
                    {
                        Line.numCapVertices = 10;
                        Line.numCornerVertices = 5;
                    }
                    Line.material.shader = Shader.Find("GUI/Text Shader");
                    Line.startWidth = 0.025f;
                    Line.endWidth = 0.025f;
                    Line.positionCount = 25;
                    Line.useWorldSpace = true;
                    predictions.Add(rig, Line);
                }

                if (hoc) 
                    Line.gameObject.layer = 19;

                Color color = rig.GetColor();

                if (fmt) 
                    color = backgroundColor.GetCurrentColor();
                if (tt) 
                    color = new Color(color.r, color.g, color.b, 0.5f);

                float width = thinTracers ? 0.0075f : 0.025f;
                Line.startWidth = width;
                Line.endWidth = width;

                Line.startColor = color;
                Line.endColor = color;

                Vector3 position = rig.syncPos;
                Vector3 velocity = rig.LatestVelocity();

                if (velocity.magnitude < 1.5f)
                    continue;

                DrawTrajectory(position, velocity, Line);
            }
        }

        public static void DisableJumpPredictions()
        {
            foreach (KeyValuePair<VRRig, LineRenderer> pred in predictions)
                Object.Destroy(pred.Value.gameObject);

            predictions.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> hitboxESP = new Dictionary<VRRig, GameObject>();
        public static void HitboxPredictions()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var box in hitboxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                hitboxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!hitboxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    Object.Destroy(box.GetComponent<BoxCollider>());

                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    hitboxESP.Add(vrrig, box);
                }

                Color color = vrrig.playerColor;
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                Vector3 velocity = vrrig.LatestVelocity();
                    
                box.GetComponent<Renderer>().enabled = velocity.magnitude > 1f;
                box.GetComponent<Renderer>().material.color = color;

                box.transform.position = vrrig.transform.position + (velocity * 0.5f);
            }
        }

        public static void DisableHitboxPredictions()
        {
            foreach (KeyValuePair<VRRig, GameObject> box in hitboxESP)
                Object.Destroy(box.Value);

            hitboxESP.Clear();
        }

        // TODO: Fix the other players' slingshot trajectory prediction.
        // TODO: Fix gravity of slingshot prediction

        public static readonly Dictionary<SlingshotProjectile, LineRenderer> trajectoryPool = new Dictionary<SlingshotProjectile, LineRenderer>();
        public static LineRenderer localTrajectoryLine;

        public static void PaintbrawlTrajectories()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            List<SlingshotProjectile> toRemoveTrajectories = new List<SlingshotProjectile>();
            foreach (KeyValuePair<SlingshotProjectile, LineRenderer> key in trajectoryPool)
            {
                if (!key.Value.gameObject.activeSelf)
                {
                    toRemoveTrajectories.Add(key.Key);
                    Object.Destroy(key.Value.gameObject);
                }
                else
                    key.Value.gameObject.SetActive(false);
            }

            foreach (SlingshotProjectile item in toRemoveTrajectories)
                trajectoryPool.Remove(item);

            void LoopProjectileArray(LoopingArray<ProjectileTracker.ProjectileInfo> projectileArray)
            {
                if (projectileArray == null || projectileArray.Length <= 0) return;
                for (int index = 0; index < projectileArray.Length; index++)
                {
                    SlingshotProjectile projectileInstance = projectileArray[index].projectileInstance;
                    if (projectileInstance == null || !projectileInstance.gameObject.activeSelf) continue;

                    if ((VRRig.LocalRig.GetSlingshot() as Slingshot).dummyProjectile && (VRRig.LocalRig.GetSlingshot() as Slingshot).dummyProjectile.GetComponent<SlingshotProjectile>() == projectileInstance) continue;

                    if (!trajectoryPool.TryGetValue(projectileInstance, out LineRenderer Line))
                    {
                        GameObject LineObject = new GameObject("LineObject");
                        Line = LineObject.AddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            Line.numCapVertices = 10;
                            Line.numCornerVertices = 5;
                        }
                        Line.material.shader = Shader.Find("GUI/Text Shader");
                        Line.startWidth = 0.025f;
                        Line.endWidth = 0.025f;
                        Line.positionCount = 25;
                        Line.useWorldSpace = true;
                        trajectoryPool.Add(projectileInstance, Line);
                    }

                    Line.gameObject.SetActive(true);

                    if (hoc)
                        Line.gameObject.layer = 19;

                    Color color = projectileInstance.teamColor;

                    if (fmt)
                        color = backgroundColor.GetCurrentColor();
                    if (tt)
                        color = new Color(color.r, color.g, color.b, 0.5f);

                    float width = thinTracers ? 0.0075f : 0.025f;
                    Line.startWidth = width;
                    Line.endWidth = width;

                    Line.startColor = color;
                    Line.endColor = color;

                    Vector3 position = projectileInstance.transform.position;
                    Vector3 velocity = projectileInstance.projectileRigidbody.linearVelocity;

                    Vector3 gravity = Physics.gravity + (projectileInstance.forceComponent?.force ?? Vector3.zero);

                    DrawTrajectory(position, velocity, Line, NoInvisLayerMask(), gravity);
                }
            }

            foreach (LoopingArray<ProjectileTracker.ProjectileInfo> projectileArray in ProjectileTracker.m_playerProjectiles.Values)
                LoopProjectileArray(projectileArray);

            LoopProjectileArray(ProjectileTracker.m_localProjectiles);

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.IsLocal()) continue;

                Slingshot playerSlingshot = rig.GetSlingshot() as Slingshot;
                if (playerSlingshot != null)
                {
                    if (playerSlingshot.InDrawingState() && playerSlingshot.dummyProjectile != null)
                    {
                        SlingshotProjectile projectileInstance = playerSlingshot.dummyProjectile.GetComponent<SlingshotProjectile>();
                        if (projectileInstance == null || !projectileInstance.gameObject.activeSelf) continue;

                        if (!trajectoryPool.TryGetValue(projectileInstance, out LineRenderer Line))
                        {
                            GameObject LineObject = new GameObject("LineObject");
                            Line = LineObject.AddComponent<LineRenderer>();
                            if (smoothLines)
                            {
                                Line.numCapVertices = 10;
                                Line.numCornerVertices = 5;
                            }
                            Line.material.shader = Shader.Find("GUI/Text Shader");
                            Line.startWidth = 0.025f;
                            Line.endWidth = 0.025f;
                            Line.positionCount = 25;
                            Line.useWorldSpace = true;
                            trajectoryPool.Add(projectileInstance, Line);
                        }

                        Line.gameObject.SetActive(true);

                        if (hoc)
                            Line.gameObject.layer = 19;

                        Color color = rig.GetColor();

                        if (fmt)
                            color = backgroundColor.GetCurrentColor();
                        if (tt)
                            color = new Color(color.r, color.g, color.b, 0.5f);

                        float width = thinTracers ? 0.0075f : 0.025f;
                        Line.startWidth = width;
                        Line.endWidth = width;

                        Line.startColor = color;
                        Line.endColor = color;

                        Vector3 position = playerSlingshot.GetTrueLaunchPosition();
                        Vector3 velocity = playerSlingshot.GetNetworkedLaunchVelocity();

                        Vector3 gravity = Physics.gravity + (projectileInstance.forceComponent?.force ?? Vector3.zero);

                        DrawTrajectory(position, velocity, Line, NoInvisLayerMask(), gravity);
                    }
                }
            }

            if (localTrajectoryLine != null)
            {
                if (!localTrajectoryLine.gameObject.activeSelf)
                {
                    localTrajectoryLine = null;
                    Object.Destroy(localTrajectoryLine.gameObject);
                }
                else
                    localTrajectoryLine.gameObject.SetActive(false);
            }

            Slingshot localSlingshot = VRRig.LocalRig.GetSlingshot() as Slingshot;
            if (localSlingshot == null || !localSlingshot.InDrawingState())
                return;

            if (localTrajectoryLine == null)
            {
                GameObject LineObject = new GameObject("LineObject");
                localTrajectoryLine = LineObject.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    localTrajectoryLine.numCapVertices = 10;
                    localTrajectoryLine.numCornerVertices = 5;
                }
                localTrajectoryLine.material.shader = Shader.Find("GUI/Text Shader");
                localTrajectoryLine.startWidth = 0.025f;
                localTrajectoryLine.endWidth = 0.025f;
                localTrajectoryLine.positionCount = 25;
                localTrajectoryLine.useWorldSpace = true;
            }

            localTrajectoryLine.gameObject.SetActive(true);

            if (hoc)
                localTrajectoryLine.gameObject.layer = 19;

            Color localColor = VRRig.LocalRig.GetColor();

            if (fmt)
                localColor = backgroundColor.GetCurrentColor();
            if (tt)
                localColor = new Color(localColor.r, localColor.g, localColor.b, 0.5f);

            float localWidth = thinTracers ? 0.0075f : 0.025f;
            localTrajectoryLine.startWidth = localWidth;
            localTrajectoryLine.endWidth = localWidth;

            localTrajectoryLine.startColor = localColor;
            localTrajectoryLine.endColor = localColor;

            Vector3 localPosition = localSlingshot.GetTrueLaunchPosition();
            Vector3 localVelocity = localSlingshot.GetLaunchVelocity();

            DrawTrajectory(localPosition, localVelocity, localTrajectoryLine, NoInvisLayerMask(), Vector3.down * 10.79f);
        }

        public static void DisablePaintbrawlTrajectories()
        {
            foreach (KeyValuePair<SlingshotProjectile, LineRenderer> pred in trajectoryPool)
                Object.Destroy(pred.Value.gameObject);

            trajectoryPool.Clear();

            if (localTrajectoryLine != null)
            {
                Object.Destroy(localTrajectoryLine.gameObject);
                localTrajectoryLine = null;
            }
        }

        public static void DrawTrajectory(Vector3 position, Vector3 velocity, LineRenderer lineRenderer, int? overrideLayerMask = null, Vector3? overrideGravity = null)
        {
            lineRenderer.enabled = true;

            int stepCount = 25;
            Vector3[] points = new Vector3[stepCount];

            VRRig hitPlayer = null;
            int i;
            for (i = 0; i < stepCount; i++)
            {
                points[i] = position;

                Vector3 nextVelocity = velocity + (overrideGravity ?? Physics.gravity) * 0.1f;
                Vector3 nextPosition = position + velocity * 0.1f;

                if (i >= 1 && Physics.Raycast(position, nextPosition - position, out RaycastHit hit, (nextPosition - position).magnitude, overrideLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers))
                {
                    points[i] = hit.point;
                    i++;

                    VRRig raycastTarget = hit.collider.GetComponentInParent<VRRig>();
                    if (raycastTarget)
                        hitPlayer = raycastTarget;
                    
                    break;
                }

                position = nextPosition;
                velocity = nextVelocity;
            }

            Color lineColor = lineRenderer.startColor;

            if (hitPlayer != null)
            {
                lineColor = hitPlayer.IsLocal() ? Color.red : Color.green;
            }

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;

            Vector3[] finalPoints = new Vector3[i];
            Array.Copy(points, finalPoints, i);

            lineRenderer.positionCount = finalPoints.Length;
            lineRenderer.SetPositions(finalPoints);
        }

        public static void VisualizeNetworkTriggers()
        {
            GameObject triggers = GetObject("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                        VisualizeCube(child.position, child.rotation, child.lossyScale, Color.red);
                } catch { }
            }
        }

        public static void VisualizeWindBarriers()
        {
            foreach (ForceVolume wind in GetAllType<ForceVolume>())
            {
                try
                {
                    VisualizeCube(wind.transform.position, wind.transform.rotation, wind.transform.lossyScale, Color.blue);
                }
                catch { }
            }
        }

        public static void VisualizeMapTriggers()
        {
            GameObject triggers = GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                        VisualizeCube(child.position, child.rotation, child.lossyScale, backgroundColor.GetCurrentColor());
                } catch { }
            }
        }

        private static readonly Dictionary<VRRig, List<int>> ntDistanceList = new Dictionary<VRRig, List<int>>();
        public static float GetTagDistance(VRRig rig)
        {
            if (ntDistanceList.ContainsKey(rig))
            {
                if (ntDistanceList[rig][0] == Time.frameCount)
                {
                    ntDistanceList[rig].Add(Time.frameCount);
                    return (0.25f + ntDistanceList[rig].Count * 0.15f) * rig.scaleFactor;
                }

                ntDistanceList[rig].Clear();
                ntDistanceList[rig].Add(Time.frameCount);
                return (0.25f + ntDistanceList[rig].Count * 0.15f) * rig.scaleFactor;
            }

            ntDistanceList.Add(rig, new List<int> { Time.frameCount });
            return 0.4f * rig.scaleFactor;
        }

        private static int optimizeChangeStep;
        private static float optimizeDelay;
        public static bool NameTagOptimize()
        {
            if (Time.time < optimizeDelay)
            {
                if (Time.frameCount != optimizeChangeStep)
                    return false;
            }
            else
            {
                optimizeDelay = Time.time + (PerformanceVisuals ? PerformanceVisualDelay : 0.1f);
                optimizeChangeStep = Time.frameCount;
            }

            return true;
        }

        private static readonly Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject>();
        public static bool nameTagChams;
        public static bool selfNameTag;
        public static void NameTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = nametags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                nametags.Remove(nametag.Key);
            }

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal || selfNameTag))
            {
                if (!nametags.ContainsKey(vrrig))
                {
                    GameObject go = new GameObject("iiMenu_Nametag");
                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                    TextMeshPro.fontSize = 4.8f;
                    TextMeshPro.alignment = TextAlignmentOptions.Center;

                    nametags.Add(vrrig, go);
                }

                GameObject nameTag = nametags[vrrig];
                TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                if (NameTagOptimize())
                {
                    tmp.SafeSetText(CleanPlayerName(GetPlayerFromVRRig(vrrig).NickName));
                    tmp.color = vrrig.GetColor();
                    tmp.SafeSetFontStyle(activeFontStyle);
                    tmp.SafeSetFont(activeFont);
                }

                if (nameTagChams)
                    tmp.Chams();
                nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                nameTag.transform.LookAt(Camera.main.transform.position);
                nameTag.transform.Rotate(0f, 180f, 0f);
            }
        }

        public static void DisableNameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
                Object.Destroy(nametag.Value);
            
            nametags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> velnametags = new Dictionary<VRRig, GameObject>();
        public static void VelocityTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = velnametags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                velnametags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!velnametags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Veltag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            velnametags.Add(vrrig, go);
                        }

                        GameObject nameTag = velnametags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText($"{vrrig.LatestVelocity().magnitude:F1}m/s");
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableVelocityTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in velnametags)
                Object.Destroy(nametag.Value);
            
            velnametags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> fpsNametags = new Dictionary<VRRig, GameObject>();
        public static void FPSTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = fpsNametags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                fpsNametags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!fpsNametags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_FPStag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            fpsNametags.Add(vrrig, go);
                        }

                        GameObject nameTag = fpsNametags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText($"{vrrig.fps} FPS");
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableFPSTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in fpsNametags)
                Object.Destroy(nametag.Value);

            fpsNametags.Clear();
        }
        
        private static readonly Dictionary<VRRig, GameObject> idNameTags = new Dictionary<VRRig, GameObject>();
        public static void IDTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = idNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                idNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!idNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_IDtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            idNameTags.Add(vrrig, go);
                        }

                        GameObject nameTag = idNameTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText(GetPlayerFromVRRig(vrrig).UserId);
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableIDTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in idNameTags)
                Object.Destroy(nametag.Value);

            idNameTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> platformTags = new Dictionary<VRRig, GameObject>();
        public static void PlatformTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = platformTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                platformTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!platformTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_PlatformTag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            platformTags.Add(vrrig, go);
                        }

                        GameObject nameTag = platformTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText($"{vrrig.GetPlatform()}");
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisablePlatformTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in platformTags)
                Object.Destroy(nametag.Value);

            platformTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> kidNameTags = new Dictionary<VRRig, GameObject>();
        public static void KIDNameTags()
        {
            List<KeyValuePair<VRRig, GameObject>> kidNameTagsCopy = kidNameTags.ToList();
            foreach (KeyValuePair<VRRig, GameObject> nametag in kidNameTagsCopy)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    Object.Destroy(nametag.Value);
                    kidNameTags.Remove(nametag.Key);
                }
                else
                {
                    if (!nametag.Key.IsKIDRestricted())
                    {
                        Object.Destroy(nametag.Value);
                        kidNameTags.Remove(nametag.Key);
                    }
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!kidNameTags.ContainsKey(vrrig))
                        {
                            if (vrrig.IsKIDRestricted())
                            {
                                GameObject go = new GameObject("iiMenu_Kidtag");
                                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                TextMeshPro TextMeshPro = go.GetOrAddComponent<TextMeshPro>();
                                TextMeshPro.fontSize = 4.8f;
                                TextMeshPro.alignment = TextAlignmentOptions.Center;
                                TextMeshPro.color = new Color32(56, 126, 138, 255);
                                TextMeshPro.SafeSetText("k-ID Restricted");

                                TextMeshPro.SafeSetFontStyle(activeFontStyle);
                                TextMeshPro.SafeSetFont(activeFont);

                                kidNameTags.Add(vrrig, go);
                            }
                        }

                        if (kidNameTags.TryGetValue(vrrig, out GameObject nameTag))
                        {
                            TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();
                            if (nameTagChams)
                                tmp.Chams();
                            nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                            nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                            nameTag.transform.LookAt(Camera.main.transform.position);
                            nameTag.transform.Rotate(0f, 180f, 0f);
                        }
                    }
                }
                catch { }
            }
        }

        public static void DisableKIDNameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in kidNameTags)
                Object.Destroy(nametag.Value);

            kidNameTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> subNameTags = new Dictionary<VRRig, GameObject>();
        public static void SubscriberNameTags()
        {
            List<KeyValuePair<VRRig, GameObject>> subNameTagsCopy = subNameTags.ToList();
            foreach (KeyValuePair<VRRig, GameObject> nametag in subNameTagsCopy)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    Object.Destroy(nametag.Value);
                    subNameTags.Remove(nametag.Key);
                }
                else
                {
                    if (SubscriptionManager.GetSubscriptionDetails(nametag.Key).tier <= 0)
                    {
                        Object.Destroy(nametag.Value);
                        subNameTags.Remove(nametag.Key);
                    }
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!subNameTags.ContainsKey(vrrig))
                        {
                            var subDetails = SubscriptionManager.GetSubscriptionDetails(vrrig);
                            if (subDetails.tier > 0)
                            {
                                GameObject go = new GameObject("iiMenu_Kidtag");
                                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                TextMeshPro TextMeshPro = go.GetOrAddComponent<TextMeshPro>();
                                TextMeshPro.fontSize = 4.8f;
                                TextMeshPro.alignment = TextAlignmentOptions.Center;
                                TextMeshPro.color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
                                TextMeshPro.SafeSetText($"Subscriber {subDetails.daysAccrued}d");

                                TextMeshPro.SafeSetFontStyle(activeFontStyle);
                                TextMeshPro.SafeSetFont(activeFont);

                                subNameTags.Add(vrrig, go);
                            }
                        }

                        if (subNameTags.TryGetValue(vrrig, out GameObject nameTag))
                        {
                            TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();
                            if (nameTagChams)
                                tmp.Chams();
                            nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                            nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                            nameTag.transform.LookAt(Camera.main.transform.position);
                            nameTag.transform.Rotate(0f, 180f, 0f);
                        }
                    }
                }
                catch { }
            }
        }

        public static void DisableSubscriberNameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in subNameTags)
                Object.Destroy(nametag.Value);

            subNameTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> creationDateTags = new Dictionary<VRRig, GameObject>();
        public static void CreationDateTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = creationDateTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                creationDateTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!creationDateTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_CreationTag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            creationDateTags.Add(vrrig, go);
                        }

                        GameObject nameTag = creationDateTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText(GetCreationDate(GetPlayerFromVRRig(vrrig).UserId));
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisableCreationDateTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in creationDateTags)
                Object.Destroy(nametag.Value);

            creationDateTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> pingNameTags = new Dictionary<VRRig, GameObject>();
        public static void PingTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = pingNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                pingNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!pingNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Pingtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            pingNameTags.Add(vrrig, go);
                        }

                        GameObject nameTag = pingNameTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText($"{vrrig.GetPing()}ms");
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisablePingTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in pingNameTags)
                Object.Destroy(nametag.Value);

            pingNameTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> turnNameTags = new Dictionary<VRRig, GameObject>();
        public static void TurnTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = turnNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                turnNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!turnNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Turntag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            turnNameTags.Add(vrrig, go);
                        }

                        string turnType = vrrig.turnType;
                        int turnFactor = vrrig.turnFactor;

                        GameObject nameTag = turnNameTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            tmp.SafeSetText(turnType == "NONE" ? "None" : ToTitleCase(turnType) + " " + turnFactor);
                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableTurnTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in turnNameTags)
                Object.Destroy(nametag.Value);

            turnNameTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> taggedNameTags = new Dictionary<VRRig, GameObject>();
        public static void TaggedTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = taggedNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                taggedNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!taggedNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Taggedtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            taggedNameTags.Add(vrrig, go);
                        }

                        
                        GameObject nameTag = taggedNameTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            if (vrrig.IsTagged())
                            {
                                int taggedById = vrrig.taggedById;
                                NetPlayer tagger = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(taggedById);

                                if (tagger != null)
                                    tmp.SafeSetText("Tagged by " + tagger?.NickName);
                            }
                            else
                                tmp.SafeSetText("");

                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableTaggedTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in taggedNameTags)
                Object.Destroy(nametag.Value);

            taggedNameTags.Clear();
        }

        public static readonly Dictionary<string, string> modDictionary = new Dictionary<string, string> {
            { "genesis", "Genesis" },
            { "HP_Left", "Holdable Pad" },
            { "GrateVersion", "Grate" },
            { "void", "Void" },
            { "BANANAOS", "Banana OS" },
            { "GC", "Gorilla Craft" },
            { "CarName", "Gorilla Vehicles" },
            { "6p72ly3j85pau2g9mda6ib8px", "CCM V2" },
            { "FPS-Nametags for Zlothy", "FPS Tags" },
            { "cronos", "Cronos" },
            { "ORBIT", "Orbit" },
            { "Violet On Top", "Violet" },
            { "MP25", "Monke Phone" },
            { "GorillaWatch", "Gorilla Watch" },
            { "InfoWatch", "Gorilla Info Watch" },
            { "BananaPhone", "Banana Phone" },
            { "Vivid", "Vivid" },
            { "RGBA", "Custom Cosmetics" },
            { "cheese is gouda", "Whos Icheating" },
            { "shirtversion", "Gorilla Shirts" },
            { "gpronouns", "Gorilla Pronouns" },
            { "gfaces", "Gorilla Faces" },
            { "monkephone", "Monke Phone" },
            { "pmversion", "Player Models" },
            { "gtrials", "Gorilla Trials" },
            { "msp", "Monke Smartphone" },
            { "gorillastats", "Gorilla Stats" },
            { "using gorilladrift", "Gorilla Drift" },
            { "monkehavocversion", "Monke Havoc" },
            { "tictactoe", "Tic Tac Toe" },
            { "ccolor", "Index" },
            { "imposter", "Gorilla Among Us" },
            { "spectapeversion", "Spec Tape" },
            { "cats", "Cats" },
            { "made by biotest05 :3", "Dogs" },
            { "fys cool magic mod", "Fys Magic Mod" },
            { "colour", "Custom Cosmetics" },
            { "chainedtogether", "Chained Together" },
            { "goofywalkversion", "Goofy Walk" },
            { "void_menu_open", "Void" },
            { "violetpaiduser", "Violet Paid" },
            { "violetfree", "Violet Free" },
            { "obsidianmc", "Obsidian.Lol" },
            { "dark", "Shiba GT Dark" },
            { "hidden menu", "Hidden" },
            { "oblivionuser", "Oblivion" },
            { "hgrehngio889584739_hugb\n", "Resurgence" },
            { "eyerock reborn", "Eye Rock" },
            { "asteroidlite", "Asteroid Lite" },
            { "elux", "Elux" },
            { "cokecosmetics", "Coke Cosmetx" },
            { "GFaces", "G Faces" },
            { "github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
            { "ObsidianMC", "Obsidian" },
            { "hgrehngio889584739_hugb", "Resurgence" },
            { "GTrials", "G Trials" },
            { "github.com/ZlothY29IQ/GorillaMediaDisplay", "Gorilla Media Display" },
            { "github.com/ZlothY29IQ/TooMuchInfo", "Too Much Info" },
            { "github.com/ZlothY29IQ/RoomUtils-IW", "Room Utils IW" },
            { "github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
            { "github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click CI" },
            { "github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
            { "MediaPad", "Media Pad" },
            { "GorillaCinema", "Gorilla Cinema" },
            { "ChainedTogetherActive", "Chained Together" },
            { "GPronouns", "G Pronouns" },
            { "CSVersion", "Custom Skin" },
            { "github.com/ZlothY29IQ/Zloth-RecRoomRig", "Zloth Rec Room Rig" },
            { "ShirtProperties", "Shirts Old" },
            { "GorillaShirts", "Shirts" },
            { "GS", "Old Shirts" },
            { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "CCM V2" },
            { "Body Tracking", "Body Track Old" },
            { "Body Estimation", "Han Body Est" },
            { "Gorilla Track", "Body Track" },
            { "CustomMaterial", "Custom Cosmetics" },
            { "I like cheese", "Rec Room Rig" },
            { "silliness", "Silliness" },
            { "EmoteWheel", "Fortnite Emote Wheel" },
            { "untitled", "Untitled" },
            { "BoyDoILoveInformation Public", "BoyDoILoveInformation" },
            { "DTAOI", "DTAOI" },
            { "GorillaShop", "GorillaShop" },
            { "Fusioned", "Fusioned" },
            { "y u lookin in here weirdo", "Malachi Menu Reborn" },
            { "ØƦƁƖƬ", "Orbit" },
            { "Atlas", "Atlas" }
        };

        private static readonly Dictionary<VRRig, GameObject> modNameTags = new Dictionary<VRRig, GameObject>();
        public static void ModTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = modNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                modNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!modNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Modtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            modNameTags.Add(vrrig, go);
                        }

                        GameObject nameTag = modNameTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            string specialMods = null;
                            Dictionary<string, object> customProps = new Dictionary<string, object>();
                            foreach (DictionaryEntry dictionaryEntry in NetPlayerToPlayer(GetPlayerFromVRRig(vrrig)).CustomProperties)
                                customProps[dictionaryEntry.Key.ToString().ToLower()] = dictionaryEntry.Value;

                            foreach (var mod in modDictionary.Where(mod => customProps.ContainsKey(mod.Key.ToLower())))
                            {
                                if (specialMods == null)
                                    specialMods = mod.Value;
                                else
                                {
                                    if (specialMods.Contains("&"))
                                        specialMods = mod.Value + ", " + specialMods;
                                    else
                                        specialMods += " & " + mod.Value;
                                }
                            }

                            CosmeticsController.CosmeticSet cosmeticSet = vrrig.cosmeticSet;
                            if (cosmeticSet.items.Any(cosmetic => !cosmetic.isNullItem && !vrrig.rawCosmeticString.Contains(cosmetic.itemName)))
                            {
                                if (specialMods == null)
                                    specialMods = "Cosmetx";
                                else
                                {
                                    if (specialMods.Contains("&"))
                                        specialMods = "Cosmetx, " + specialMods;
                                    else
                                        specialMods += " & Cosmetx";
                                }
                            }

                            tmp.SafeSetText(specialMods);

                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                            if (nameTagChams)
                                tmp.Chams();
                        }
                        
                        if (!string.IsNullOrEmpty(tmp.text))
                        {
                            nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                            nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                            nameTag.transform.LookAt(Camera.main.transform.position);
                            nameTag.transform.Rotate(0f, 180f, 0f);
                        }
                    }
                }
                catch { }
            }
        }

        public static void DisableModTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in modNameTags)
                Object.Destroy(nametag.Value);

            modNameTags.Clear();
        }

        public static readonly Dictionary<string, string> specialCosmetics = new Dictionary<string, string>
        {
            { "LBAAD.", "Administrator" },
            { "LBAAK.", "Forest Guide" },
            { "LBADE.", "Finger Painter" },
            { "LBAGS.", "Illustrator" },
            { "LMAPY.", "Forest Guide" },
            { "LBANI.", "AA Creator" }
        };

        private static readonly Dictionary<VRRig, GameObject> cosmeticNameTags = new Dictionary<VRRig, GameObject>();
        public static void CosmeticTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = cosmeticNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                cosmeticNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if ((!vrrig.isLocal || selfNameTag) && cosmetics == null)
                    {
                        if (!cosmeticNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Modtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMeshPro TextMeshPro = go.AddComponent<TextMeshPro>();
                            TextMeshPro.fontSize = 4.8f;
                            TextMeshPro.alignment = TextAlignmentOptions.Center;

                            cosmeticNameTags.Add(vrrig, go);
                        }

                        GameObject nameTag = cosmeticNameTags[vrrig];
                        TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();
                        if (NameTagOptimize())
                        {
                            string cosmetics = null;
                            foreach (var cosmetic in specialCosmetics.Where(cosmetic => vrrig.rawCosmeticString.Contains(cosmetic.Key)))
                            {
                                if (cosmetics == null)
                                    cosmetics = cosmetic.Value;
                                else
                                {
                                    if (cosmetics.Contains("&"))
                                        cosmetics = cosmetic.Value + ", " + cosmetics;
                                    else
                                        cosmetics += " & " + cosmetic.Value;
                                }
                            }

                            tmp.SafeSetText(cosmetics);

                            tmp.color = vrrig.GetColor();
                            tmp.SafeSetFontStyle(activeFontStyle);
                            tmp.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            tmp.Chams();
                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisableCosmeticTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in cosmeticNameTags)
                Object.Destroy(nametag.Value);

            cosmeticNameTags.Clear();
        }

        public static readonly Dictionary<string, string> verifiedDictionary = new Dictionary<string, string>
        {
            { "9DBC90CF7449EF64", "StyledSnail" },
            { "33FFCE29A8DB5BB", "Jmancurly?" },
            { "6FE5FF4D5DF68843", "Pine" },
            { "52529F0635BE0CDF", "PapaSmurf" },
            { "BAC5807405123060", "britishmonke" },
            { "A6FFC7318E1301AF", "jmancurly" },
            { "3B9FD2EEF24ACB3", "VMT" },
            { "33FFA45DBFD33B01", "will" },
            { "D6971CA01F82A975", "Elliot" },
            { "7FB16B1EDEB71A4C", "Elliot" },
            { "636D8846E76C9B5A", "Clown" },
            { "65CB0CCF1AED2BF", "Ethyb" },
            { "48437FE432DE48BE", "Rose" },
            { "61AD990FF3A423B7", "Boda 1" },
            { "AAB44BFD0BA34829", "Boda 2" },
            { "6713DA80D2E9BFB5", "AHauntedArmy" },
            { "B4A3FF01312B55B1", "Pluto" },
            { "339E0D392565DC39", "kishark" },
            { "F08CE3118F9E793E", "TurboAlligator" },
            { "5380BEF3DA4A857D", "Tuxedo" },
            { "D6E20BE9655C798", "TTTPIG 1" },
            { "71AA09D13C0F408D", "TTTPIG 2" },
            { "1D6E20BE9655C798", "TTTPIG 3" },
            { "22A7BCEFFD7A0BBA", "TTTPIG 4" },
            { "C3878B068886F6C3", "ZZEN" },
            { "6F79BE7CB34642AC", "CodyO'Quinn" },
            { "5AA1231973BE8A62", "Apollo" },
            { "7F31BEEC604AE189", "ElectronicWall 1" },
            { "42C809327652ECDD", "ElectronicWall 2" },
            { "ECDE8A2FF8510934", "Antoca" },
            { "80279945E7D3B57D", "Jolyne" },
            { "7E44E8337DF02CC1", "Nunya" },
            { "DE601BC40DB68CE0", "Graic" },
            { "F5B5C64914C13B83", "HatGirl" },
            { "660814E013F31EFA", "HOLLOWZZGT" },
            { "2E408ED946D55D51", "Haunted" },
            { "D345FE394607F946", "Bzzz the 18th" },
            { "498D4C2F23853B37", "POGTROLL" },
            { "BC9764E1EADF8BE0", "Circuit" },
            { "D0CB396539676DD8", "FrogIlla" },
            { "A1A99D33645E4A94", "STEAMVRAVTS / YEAT" },
            { "CBCCBBB6C28A94CF", "PTMstar" },
            { "6DC06EEFFE9DBD39", "Lucio" },
            { "4ACA3C76B334B17F", "Wihz" },
            { "571776944B6162F1", "CubCub" },
            { "FB5FCEBC4A0E0387", "PepsiDee" },
            { "8ED59EACCDC6BA86", "PepsiDee?" },
            { "645222265FB972B", "Chaotic Asriel" },
            { "BC99FA914F506AB8", "Lemming 1" },
            { "3A16560CA65A51DE", "Lemming 2" },
            { "59F3FE769DE93AB9", "Lemming 3" },
            { "EE9FB127CF7DBBD5", "NOTMARK" },
            { "54DCB69545BE0800", "Biffbish" },
            { "A04005517920EB0", "K9" },
            { "ABD60175B46E45C5", "Saltwater" },
            { "964C4A68F65A804C", "YottaBite" },
            { "8FECBBC89D69575E", "KyleTheScientist" },
            { "4D5EB238C8253D04", "Person" },
            { "8B047CEF4F695F3A", "AlecVR" },
            { "E5883BD27F60F99A", "AlecVR?" },
            { "70EEBA9507E8381E", "H4KPY?" },
            { "911691C9FEB63D9F", "H4KPY?" },
            { "D322FC7F6A9875DB", "DecalFree" },
            { "FBE1690495D63A05", "Azora" },
            { "3509A9A428FCD55C", "Polar" },
            { "3F179DCC75FECA1", "Polar" },
            { "450EE31CA7FBDE4C", "ProximusVR" },
            { "180E486699D14963", "Legion" },
            { "AC67B4E838EFB5D3", "PartyMonkeyGT" },
            { "4BB02313F55AA741", "Mosa?" },
            { "E61FD8B23F3264C0", "Authority" },
            { "36B456067A5E1453", "Lofiat" },
            { "FC8CB7FED6EFDC81", "CJVR" },
            { "6C85D07DC2586DC9", "Arctrie" },
            { "22CDF30B107A9BDB", "Durag" },
            { "1940CCB76316556F", "Genet1c" },
            { "6B4FB3EF97A8BB71", "Crisp" },
            { "17BCC7B56F88287A", "Vortex" },
            { "2D35DBED9A3BE6A0", "Tortise" },
            { "2D7D32651E93866", "Graze" },
            { "B4E45E48C5CE0656", "ZBR" },
            { "F7EE771EB6794ABE", "OfficialLemon" },
            { "36FD11C9FB61E50B", "Cryptik" },
            { "28579AFACDE1FB19", "Pepsi Dee" },
            { "8A062E735BBC89ED", "GLTCH" },
            { "A100E9E6C4D91E75", "Mycrafts 1" },
            { "7952F9E08FEF8E83", "Mycrafts 2" },
            { "10E12F25533C13F2", "Kirpi4" },
            { "10621E029A675705", "AA_Mike" },
            { "F8FF7B812B0B2F72", "Foggy" },
            { "1E8298E1E1F40CB2", "Faaduu" },
            { "289C8FAD58A09D6D", "Pixel" },
            { "172E4982BEE4A8AD", "H4KPY" },
            { "A339740A8ED97FC2", "Coffeeperson" },
            { "502575B001FE6FCD", "Mikeyourman" },
            { "7DC729B66A15F9DE", "TrumpGT" }
        };

        private static readonly Dictionary<VRRig, GameObject> verifiedNameTags = new Dictionary<VRRig, GameObject>();
        public static void VerifiedTags()
        {
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = verifiedNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                verifiedNameTags.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag) 
                    {
                        if (!verifiedNameTags.ContainsKey(vrrig))
                        {
                            string userId = GetPlayerFromVRRig(vrrig).UserId;
                            if (verifiedDictionary.TryGetValue(userId, out string name))
                            {
                                GameObject go = new GameObject("iiMenu_Verifiedtag");
                                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                TextMeshPro TextMeshPro = go.GetOrAddComponent<TextMeshPro>();
                                TextMeshPro.fontSize = 4.8f;
                                TextMeshPro.alignment = TextAlignmentOptions.Center;
                                TextMeshPro.SafeSetText(name);

                                verifiedNameTags.Add(vrrig, go);
                            } else if (ServerData.Administrators.TryGetValue(userId, out string adminName))
                            {
                                GameObject go = new GameObject("iiMenu_Verifiedtag");
                                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                TextMeshPro TextMeshPro = go.GetOrAddComponent<TextMeshPro>();
                                TextMeshPro.fontSize = 4.8f;
                                TextMeshPro.alignment = TextAlignmentOptions.Center;
                                TextMeshPro.SafeSetText(adminName);

                                verifiedNameTags.Add(vrrig, go);
                            }
                        }

                        if (verifiedNameTags.TryGetValue(vrrig, out GameObject nameTag))
                        {
                            TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();
                            tmp.color = vrrig.GetColor();
                            if (NameTagOptimize()) 
                            {
                                tmp.SafeSetFontStyle(activeFontStyle);
                                tmp.SafeSetFont(activeFont);
                            }
                            if (nameTagChams)
                                tmp.Chams();
                            nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                            nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                            nameTag.transform.LookAt(Camera.main.transform.position);
                            nameTag.transform.Rotate(0f, 180f, 0f);
                        }
                    }
                }
                catch { }
            }
        }

        public static void DisableVerifiedTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in verifiedNameTags)
                Object.Destroy(nametag.Value);

            verifiedNameTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> crashedNameTags = new Dictionary<VRRig, GameObject>();
        public static void CrashedTags()
        {
            List<KeyValuePair<VRRig, GameObject>> crashedNameTagsCopy = crashedNameTags.ToList();
            foreach (KeyValuePair<VRRig, GameObject> nametag in crashedNameTagsCopy)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    Object.Destroy(nametag.Value);
                    crashedNameTags.Remove(nametag.Key);
                }
                else
                {
                    bool crashed = nametag.Key.GetTruePing() > 500;
                    if (!crashed)
                    {
                        Object.Destroy(nametag.Value);
                        crashedNameTags.Remove(nametag.Key);
                    }
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!crashedNameTags.ContainsKey(vrrig))
                        {
                            int crashPower = vrrig.GetTruePing();
                            bool crashed = crashPower > 500;

                            if (crashed)
                            {
                                Color crashedColor = Color.yellow;
                                if (crashPower > 5000)
                                    crashedColor = Color.black;
                                else if(crashPower > 2500)
                                    crashedColor = Color.red;
                                else if (crashPower > 1500)
                                    crashedColor = new Color32(255, 128, 0, 255);

                                GameObject go = new GameObject("iiMenu_Crashedtag");
                                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                TextMeshPro TextMeshPro = go.GetOrAddComponent<TextMeshPro>();
                                TextMeshPro.fontSize = 4.8f;
                                TextMeshPro.alignment = TextAlignmentOptions.Center;
                                TextMeshPro.color = crashedColor;
                                TextMeshPro.SafeSetText("Lagging");

                                TextMeshPro.SafeSetFontStyle(activeFontStyle);
                                TextMeshPro.SafeSetFont(activeFont);

                                crashedNameTags.Add(vrrig, go);
                            }
                        }

                        if (crashedNameTags.TryGetValue(vrrig, out GameObject nameTag))
                        {
                            double crashPower = Math.Abs(vrrig.velocityHistoryList[0].time * 1000 - PhotonNetwork.ServerTimestamp);

                            Color crashedColor = Color.yellow;
                            if (crashPower > 2500)
                                crashedColor = Color.red;
                            else if (crashPower > 1500)
                                crashedColor = new Color32(255, 128, 0, 255);

                            TextMeshPro tmp = nameTag.GetOrAddComponent<TextMeshPro>();
                            tmp.color = crashedColor;
                            if (nameTagChams)
                                tmp.Chams();
                            nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                            nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                            nameTag.transform.LookAt(Camera.main.transform.position);
                            nameTag.transform.Rotate(0f, 180f, 0f);
                        }
                    }
                }
                catch { }
            }
        }

        public static void DisableCrashedTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in crashedNameTags)
                Object.Destroy(nametag.Value);

            crashedNameTags.Clear();
        }


        private static readonly Dictionary<VRRig, GameObject> compactNameTags = new Dictionary<VRRig, GameObject>();
        private static readonly Dictionary<VRRig, GameObject> compactTagBackgrounds = new Dictionary<VRRig, GameObject>();

        public static string GetPrettyPlatform(VRRig vrrig)
        {
            string platform = vrrig.GetPlatform();

            return platform switch
            {
                "PC" => "<color=#00FFFF>PC</color>",
                "Steam" => "<color=blue>Steam</color>",
                "Standalone" => "<color=green>Standalone</color>",
                _ => platform,
            };
        }

        public static string GetPrettyPing(VRRig vrrig)
        {
            int ping = vrrig.GetPing();

            return ping < 300 ? $"<color=green>{ping}</color>" : ping < 1000 ? $"<color=yellow>{ping}</color>" : $"<color=red>{ping}</color>";
        }

        public static string GetPrettyFPS(VRRig vrrig)
        {
            int fps = vrrig.fps;

            return fps < 30 ? $"<color=red>{fps}</color>" : fps < 60 ? $"<color=yellow>{fps}</color>" : $"<color=green>{fps}</color>";
        }

        public static void CompactTags()
        {
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            List<KeyValuePair<VRRig, GameObject>> nametagsCopy = compactNameTags.ToList();
            foreach (var nametag in nametagsCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                Object.Destroy(compactTagBackgrounds[nametag.Key]);
                compactNameTags.Remove(nametag.Key);
                compactTagBackgrounds.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal || selfNameTag)
                    {
                        if (!compactNameTags.ContainsKey(vrrig))
                        {
                            GameObject textContainer = new GameObject("iimenu_vrctag_text");
                            if (hoc)
                                textContainer.layer = 19;

                            GameObject infoTag = new GameObject("infotag");
                            infoTag.transform.parent = textContainer.transform;
                            infoTag.transform.localPosition = new Vector3(0f, 0.5f, 0f); // change the y to make the info tag farther or closer to the nametag 
                            infoTag.transform.localScale = Vector3.one;
                            TextMeshPro infoMesh = infoTag.AddComponent<TextMeshPro>();
                            infoMesh.fontSize = 2.4f;
                            infoMesh.alignment = TextAlignmentOptions.Center;
                            infoMesh.color = Color.white;
                            infoMesh.richText = true;

                            GameObject nameTag = new GameObject("nametag");
                            nameTag.transform.parent = textContainer.transform;
                            nameTag.transform.localPosition = Vector3.zero;
                            nameTag.transform.localScale = Vector3.one;
                            TextMeshPro nameMesh = nameTag.AddComponent<TextMeshPro>();
                            nameMesh.fontSize = 3.2f;
                            nameMesh.alignment = TextAlignmentOptions.Center;
                            nameMesh.richText = true;

                            GameObject bgContainer = new GameObject("iimenu_vrctag_background");
                            if (hoc)
                                bgContainer.layer = 19;

                            GameObject infoBg = new GameObject("infobg");
                            infoBg.transform.parent = bgContainer.transform;
                            infoBg.transform.localPosition = new Vector3(0f, 0.5f, 0f);  // change the y to make the info tag farther or closer to the nametag 
                            infoBg.transform.localScale = Vector3.one;
                            infoBg.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                            Object.Destroy(infoBg.GetComponent<Collider>());
                            LineRenderer rendererInfo = infoBg.AddComponent<LineRenderer>();
                            rendererInfo.material.shader = nameTagChams ? LoadAsset<Shader>("Chams") : Shader.Find("Sprites/Default");
                            rendererInfo.material.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
                            rendererInfo.numCapVertices = 10;
                            rendererInfo.numCornerVertices = 5;
                            rendererInfo.useWorldSpace = false;
                            rendererInfo.positionCount = 2;
                            rendererInfo.startWidth = 0.05f;
                            rendererInfo.endWidth = 0.05f;

                            GameObject nameBg = new GameObject("namebg");
                            nameBg.transform.parent = bgContainer.transform;
                            nameBg.transform.localPosition = Vector3.zero;
                            nameBg.transform.localScale = Vector3.one;
                            nameBg.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                            Object.Destroy(nameBg.GetComponent<Collider>());
                            LineRenderer rendererName = nameBg.AddComponent<LineRenderer>();
                            rendererName.material.shader = nameTagChams ? LoadAsset<Shader>("Chams") : Shader.Find("Sprites/Default");
                            rendererName.material.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
                            rendererName.numCapVertices = 10;
                            rendererName.numCornerVertices = 5;
                            rendererName.useWorldSpace = false;
                            rendererName.positionCount = 2;
                            rendererName.startWidth = 0.1f;
                            rendererName.endWidth = 0.1f;

                            compactNameTags.Add(vrrig, textContainer);
                            compactTagBackgrounds.Add(vrrig, bgContainer);
                        }

                        GameObject textCont = compactNameTags[vrrig];
                        GameObject bgCont = compactTagBackgrounds[vrrig];

                        Transform infoTextTr = textCont.transform.Find("infotag");
                        Transform nameTextTr = textCont.transform.Find("nametag");
                        Transform infoBgTr = bgCont.transform.Find("infobg");
                        Transform nameBgTr = bgCont.transform.Find("namebg");

                        string tagText = $"{(vrrig.GetTruePing() > 2500 ? "[<color=red>Crashed</color>] | " : "")}[<color=#00FFFF>{GetCreationDate(vrrig.GetPlayer().UserId, null, "MMM dd, yyyy")}</color>] | [{GetPrettyPlatform(vrrig)}] | [Ping: {GetPrettyPing(vrrig)}] | [FPS: {GetPrettyFPS(vrrig)}]{(vrrig.GetPlayer().IsMasterClient ? " | [<color=#00FFFF>Master</color>]" : "")}";

                        if (NameTagOptimize())
                        {
                            infoTextTr.GetComponent<TextMeshPro>().SafeSetText(tagText);
                            infoTextTr.GetComponent<TextMeshPro>().SafeSetFontStyle(activeFontStyle);
                            infoTextTr.GetComponent<TextMeshPro>().SafeSetFont(activeFont);
                        }
                        

                        TextMeshPro tm = infoTextTr.GetComponent<TextMeshPro>();
                        if (nameTagChams)
                            tm.Chams();
                        string plainText = System.Text.RegularExpressions.Regex.Replace(tagText, "<.*?>", string.Empty);
                        float textWidth = tm.GetPreferredValues(plainText).x * 0.65f;
                        float bgHeight = textWidth + 0.15f;

                        // nametag part inherits the player color
                        string playerName = CleanPlayerName(vrrig.GetPlayer().NickName);
                        TextMeshPro nameTm = nameTextTr.GetComponent<TextMeshPro>();

                        if (NameTagOptimize())
                        {
                            nameTm.SafeSetText(playerName);
                            nameTm.SafeSetFontStyle(activeFontStyle);
                            nameTm.SafeSetFont(activeFont);
                        }
                        if (nameTagChams)
                            nameTm.Chams();
                        nameTm.color = vrrig.playerColor;
                        
                        float nameTextWidth = nameTm.GetPreferredValues(playerName).x * 0.5f;
                        float nameBgHeight = nameTextWidth + 0.2f;

                        Color nameBgColor = DarkenColor(vrrig.playerColor);
                        nameBgColor.a = 0.6f;
                        nameBgTr.GetComponent<LineRenderer>().material.color = nameBgColor;

                        float finalScale = 0.15f * vrrig.scaleFactor;
                        textCont.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
                        bgCont.transform.localScale = new Vector3(finalScale, finalScale, finalScale);

                        LineRenderer infoRenderer = infoBgTr.GetComponent<LineRenderer>();
                        infoRenderer.startWidth = 0.05f * vrrig.scaleFactor;
                        infoRenderer.endWidth = 0.05f * vrrig.scaleFactor;
                        infoRenderer.SetPosition(0, new Vector3(0f, -bgHeight, 0f));
                        infoRenderer.SetPosition(1, new Vector3(0f, bgHeight, 0f));

                        LineRenderer nameRenderer = nameBgTr.GetComponent<LineRenderer>();
                        nameRenderer.startWidth = 0.1f * vrrig.scaleFactor;
                        nameRenderer.endWidth = 0.1f * vrrig.scaleFactor;
                        nameRenderer.SetPosition(0, new Vector3(0f, -nameBgHeight, 0f));
                        nameRenderer.SetPosition(1, new Vector3(0f, nameBgHeight, 0f));

                        Vector3 tagPosition = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);

                        textCont.transform.position = tagPosition;
                        textCont.transform.LookAt(Camera.main.transform.position);
                        textCont.transform.Rotate(0f, 180f, 0f);

                        bgCont.transform.position = tagPosition - textCont.transform.forward * 0.01f;
                        bgCont.transform.LookAt(Camera.main.transform.position);
                        bgCont.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisableCompactTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in compactNameTags)
                Object.Destroy(nametag.Value);

            foreach (KeyValuePair<VRRig, GameObject> background in compactTagBackgrounds)
                Object.Destroy(background.Value);

            compactNameTags.Clear();
            compactTagBackgrounds.Clear();
        }

        public static void FixRigColors()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => vrrig.mainSkin.material.name.Contains("gorilla_body") && vrrig.mainSkin.material.shader == Shader.Find("GorillaTag/UberShader")))
                vrrig.mainSkin.material.color = vrrig.playerColor;
        }

        public static string _leavesName;
        public static string LeavesName
        {
            get 
            {
                if (_leavesName == null)
                {
                    var matchingObjects = GetObject("Environment Objects/LocalObjects_Prefab/Forest")
                        .GetComponentsInChildren<Transform>(true)
                        .Where(t => t.name.StartsWith("UnityTempFile"))
                        .GroupBy(t => t.name)
                        .FirstOrDefault(g => g.Count() == 3);

                    _leavesName = matchingObjects?.Key ?? "UnityTempFile";
                }

                return _leavesName;
            } 
        }

        public static readonly List<GameObject> leaves = new List<GameObject>();
        public static void EnableRemoveLeaves()
        {
            GameObject Forest = GetObject("Environment Objects/LocalObjects_Prefab/Forest");
            if (Forest != null)
            {
                for (int i = 0; i < Forest.transform.childCount; i++)
                {
                    GameObject v = Forest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(LeavesName))
                    {
                        v.SetActive(false);
                        leaves.Add(v);
                    }
                }
            }

            GameObject RankedForest = GetObject("RankedMain/Ranked_Layout/Ranked_Forest_prefab");
            if (RankedForest != null)
            {
                for (int i = 0; i < RankedForest.transform.childCount; i++)
                {
                    GameObject v = RankedForest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(LeavesName))
                    {
                        v.SetActive(false);
                        leaves.Add(v);
                    }
                }
            }
        }

        public static void DisableRemoveLeaves()
        {
            foreach (GameObject l in leaves)
                l.SetActive(true);
            
            leaves.Clear();
        }

        public static void EnableStreamerRemoveLeaves()
        {
            GameObject Forest = GetObject("Environment Objects/LocalObjects_Prefab/Forest");
            if (Forest != null)
            {
                for (int i = 0; i < Forest.transform.childCount; i++)
                {
                    GameObject v = Forest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(LeavesName))
                    {
                        v.layer = 21; 
                        leaves.Add(v);
                    }
                }
            }

            GameObject RankedForest = GetObject("RankedMain/Ranked_Layout/Ranked_Forest_prefab");
            if (RankedForest != null)
            {
                for (int i = 0; i < RankedForest.transform.childCount; i++)
                {
                    GameObject v = RankedForest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(LeavesName))
                    {
                        v.layer = 21;
                        leaves.Add(v);
                    }
                }
            }
        }

        public static void DisableStreamerRemoveLeaves()
        {
            foreach (GameObject l in leaves)
                l.layer = 0;
            
            leaves.Clear();
        }

        public static readonly List<GameObject> cosmetics = new List<GameObject>();
        public static void DisableCosmetics()
        {
            try
            {
                Transform HeadCosmetics = VRRig.LocalRig.mainCamera.transform.Find("HeadCosmetics");
                Transform Head = VRRig.LocalRig.transform.Find("rig/head");
                foreach (GameObject Cosmetic in VRRig.LocalRig.cosmetics)
                {
                    if (Cosmetic.activeSelf && (Cosmetic.transform.parent == HeadCosmetics || Cosmetic.transform.parent == Head))
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
                c.SetActive(true);
            
            cosmetics.Clear();
        }

        public static readonly List<Renderer> disabledRenderers = new List<Renderer>();
        public static void Xray()
        {
            if (rightTrigger > 0.5f)
            {
                if (disabledRenderers.Count <= 0)
                {
                    foreach (Renderer renderer in GetAllType<Renderer>().Where(rend => rend != null && rend.gameObject != null && !(rend is SkinnedMeshRenderer) && rend.enabled && rend.gameObject.activeSelf))
                    {
                        renderer.enabled = false;
                        disabledRenderers.Add(renderer);
                    }
                }
            } else
            {
                if (disabledRenderers.Count > 0)
                {
                    foreach (Renderer renderer in disabledRenderers.Where(rend => rend != null && rend.gameObject != null))
                        renderer.enabled = true;

                    disabledRenderers.Clear();
                }
            }
        }

        public static void NoSmoothRigs()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                vrrig.lerpValueBody = 2f;
                vrrig.lerpValueFingers = 1f;
            }
        }

        public static void ReSmoothRigs()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                vrrig.lerpValueBody = VRRig.LocalRig.lerpValueBody;
                vrrig.lerpValueFingers = VRRig.LocalRig.lerpValueFingers;
            }
        }

        public static readonly Dictionary<VRRig, Coroutine> rigLerpCoroutines = new Dictionary<VRRig, Coroutine>();
        public static IEnumerator LerpRig(VRRig rig)
        {
            Quaternion headStartRot = rig.head.rigTarget.localRotation;

            Vector3 syncStartPos = rig.transform.position;
            Quaternion syncStartRot = rig.transform.rotation;

            Vector3 leftHandStartPos = rig.leftHand.rigTarget.localPosition;
            Quaternion leftHandStartRot = rig.leftHand.rigTarget.localRotation;

            Vector3 rightHandStartPos = rig.rightHand.rigTarget.localPosition;
            Quaternion rightHandStartRot = rig.rightHand.rigTarget.localRotation;

            float startTime = Time.time;
            float length = 1f / PhotonNetwork.SerializationRate;

            while (Time.time < startTime + length)
            {
                float t = (Time.time - startTime) / length;

                rig.head.rigTarget.localRotation = Quaternion.Lerp(headStartRot, rig.head.syncRotation, t);

                rig.transform.position = Vector3.Lerp(syncStartPos, rig.syncPos, t);
                rig.transform.rotation = Quaternion.Lerp(syncStartRot, rig.syncRotation, t);

                rig.leftHand.rigTarget.localPosition = Vector3.Lerp(leftHandStartPos, rig.leftHand.syncPos, t);
                rig.leftHand.rigTarget.localRotation = Quaternion.Lerp(leftHandStartRot, rig.leftHand.syncRotation, t);

                rig.rightHand.rigTarget.localPosition = Vector3.Lerp(rightHandStartPos, rig.rightHand.syncPos, t);
                rig.rightHand.rigTarget.localRotation = Quaternion.Lerp(rightHandStartRot, rig.rightHand.syncRotation, t);

                yield return null;
            }

            rigLerpCoroutines.Remove(rig);
        }
        
        public static void BetterRigLerping(VRRig rig)
        {
            if (rigLerpCoroutines.TryGetValue(rig, out Coroutine coroutine))
                CoroutineManager.instance.StopCoroutine(coroutine);

            rigLerpCoroutines[rig] = CoroutineManager.instance.StartCoroutine(LerpRig(rig));
        }

        private static readonly Dictionary<VRRig, GameObject> cosmeticIndicators = new Dictionary<VRRig, GameObject>();
        private static readonly Dictionary<string, Texture2D> cosmeticTextures = new Dictionary<string, Texture2D>();
        private static Material cosmeticMat;
        public static void CosmeticESP()
        {
            List<KeyValuePair<VRRig, GameObject>> indicatorCopy = cosmeticIndicators.ToList();
            foreach (var nametag in indicatorCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                cosmeticIndicators.Remove(nametag.Key);
            }

            List<(string codename, string name)> cosmetics = new List<(string codename, string name)>
            {
                ("LBANI.", "aa"),
                ("LBADE.", "fingerpainter"),
                ("LBAGS.", "illustrator"),
                ("LMAPY.", "forestguide"),
                ("LBAAK.", "stick"),
                ("LBAAD.", "admin")
            };

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                string currentCosmetic = null;
                foreach (var (codename, name) in cosmetics)
                {
                    if (vrrig.rawCosmeticString.Contains(codename))
                    {
                        currentCosmetic = name;
                        break;
                    }
                }

                if (!vrrig.isLocal && currentCosmetic != null)
                {
                    if (!cosmeticIndicators.TryGetValue(vrrig, out GameObject indicator))
                    {
                        indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Object.Destroy(indicator.GetComponent<Collider>());

                        if (cosmeticMat == null)
                            cosmeticMat = new Material(LoadAsset<Shader>("Chams"));
                        indicator.GetComponent<Renderer>().material = cosmeticMat;
                        cosmeticIndicators.Add(vrrig, indicator);
                    }

                    if (!cosmeticTextures.TryGetValue(currentCosmetic, out Texture2D texture))
                    {
                        texture = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/{currentCosmetic}.png", $"Images/Mods/Visuals/{currentCosmetic}.png");
                        cosmeticTextures.Add(currentCosmetic, texture);
                    }

                    indicator.GetComponent<Renderer>().material.mainTexture = texture;

                    indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * vrrig.scaleFactor;
                    indicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (Classes.Menu.Console.GetIndicatorDistance(vrrig) * vrrig.scaleFactor);
                    indicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void DisableCosmeticESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in cosmeticIndicators)
                Object.Destroy(nametag.Value);

            cosmeticIndicators.Clear();
        }

        private static Material platformMat;
        private static Material platformEspMat;

        private static Texture2D GetPlatformTexture(VRRig rig)
        {
            return rig.GetPlatform() switch
            {
                "Steam" => LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/steam.png", "Images/Mods/Visuals/steam.png"),
                "Standalone" => LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/oculus.png", "Images/Mods/Visuals/oculus.png"),
                "PC" => LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/pc.png", "Images/Mods/Visuals/pc.png"),
                _ => LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/unknown.png", "Images/Mods/Visuals/unknown.png"),
            };
        }

        private static readonly Dictionary<VRRig, GameObject> platformIndicators = new Dictionary<VRRig, GameObject>();

        public static void PlatformIndicators()
        {
            List<KeyValuePair<VRRig, GameObject>> indicatorCopy = platformIndicators.ToList();
            foreach (var nametag in indicatorCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                platformIndicators.Remove(nametag.Key);
            }

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal || selfNameTag))
            {
                if (!platformIndicators.TryGetValue(vrrig, out GameObject indicator))
                {
                    indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(indicator.GetComponent<Collider>());

                    if (platformMat == null)
                    {
                        platformMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                        platformMat.SetFloat("_Surface", 1);
                        platformMat.SetFloat("_Blend", 0);
                        platformMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        platformMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        platformMat.SetFloat("_ZWrite", 0);
                        platformMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        platformMat.renderQueue = (int)RenderQueue.Transparent;
                    }
                    indicator.GetComponent<Renderer>().material = platformMat;
                    platformIndicators.Add(vrrig, indicator);
                }

                indicator.GetComponent<Renderer>().material.mainTexture = GetPlatformTexture(vrrig);
                indicator.GetComponent<Renderer>().material.color = vrrig.GetColor();

                indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * vrrig.scaleFactor;
                indicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (Classes.Menu.Console.GetIndicatorDistance(vrrig) * vrrig.scaleFactor);
                indicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void PlatformESP()
        {
            List<KeyValuePair<VRRig, GameObject>> indicatorCopy = platformIndicators.ToList();
            foreach (var nametag in indicatorCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                platformIndicators.Remove(nametag.Key);
            }

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal || selfNameTag))
            {
                if (!platformIndicators.TryGetValue(vrrig, out GameObject indicator))
                {
                    indicator = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    Object.Destroy(indicator.GetComponent<Collider>());

                    indicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    if (platformEspMat == null)
                        platformEspMat = new Material(Shader.Find("GUI/Text Shader"));
                        
                    indicator.GetComponent<Renderer>().material = platformEspMat;
                    platformIndicators.Add(vrrig, indicator);
                }

                indicator.GetComponent<Renderer>().material.mainTexture = GetPlatformTexture(vrrig);
                indicator.GetComponent<Renderer>().material.color = vrrig.GetColor();

                indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * vrrig.scaleFactor;
                indicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (Classes.Menu.Console.GetIndicatorDistance(vrrig) * vrrig.scaleFactor);
                indicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void DisablePlatformIndicators()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in platformIndicators)
                Object.Destroy(nametag.Value);

            platformIndicators.Clear();
        }

        private static Material voiceMat;
        private static Material voiceEspMat;
        private static Texture2D voiceTxt;

        private static readonly Dictionary<VRRig, GameObject> voiceIndicators = new Dictionary<VRRig, GameObject>();
        public static void VoiceIndicators()
        {
            List<KeyValuePair<VRRig, GameObject>> indicatorCopy = voiceIndicators.ToList();
            foreach (var nametag in indicatorCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                voiceIndicators.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal || selfNameTag)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                        size = recorder.Loudness * 3f;
                    
                    if (size > 0f)
                    {
                        if (!voiceIndicators.TryGetValue(vrrig, out GameObject volIndicator))
                        {
                            volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            Object.Destroy(volIndicator.GetComponent<Collider>());
                            
                            if (voiceMat == null)
                            {
                                voiceMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                                if (voiceTxt == null)
                                    voiceTxt = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/speak.png", $"Images/Mods/Visuals/speak.png");

                                voiceMat.mainTexture = voiceTxt;

                                voiceMat.SetFloat("_Surface", 1);
                                voiceMat.SetFloat("_Blend", 0);
                                voiceMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                voiceMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                voiceMat.SetFloat("_ZWrite", 0);
                                voiceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                voiceMat.renderQueue = (int)RenderQueue.Transparent;
                            }
                            volIndicator.GetComponent<Renderer>().material = voiceMat;
                            voiceIndicators.Add(vrrig, volIndicator);
                        }

                        volIndicator.GetComponent<Renderer>().material.color = vrrig.GetColor();
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f) * vrrig.scaleFactor;
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (Classes.Menu.Console.GetIndicatorDistance(vrrig) * vrrig.scaleFactor);
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    } else
                    {
                        if (voiceIndicators.TryGetValue(vrrig, out GameObject existing))
                        {
                            Object.Destroy(existing);
                            voiceIndicators.Remove(vrrig);
                        }
                    }
                }
            }
        }

        public static void VoiceESP()
        {
            List<KeyValuePair<VRRig, GameObject>> indicatorCopy = voiceIndicators.ToList();
            foreach (var nametag in indicatorCopy.Where(nametag => !GorillaParent.instance.vrrigs.Contains(nametag.Key)))
            {
                Object.Destroy(nametag.Value);
                voiceIndicators.Remove(nametag.Key);
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                        size = recorder.Loudness * 3f;

                    if (size > 0f)
                    {
                        if (!voiceIndicators.TryGetValue(vrrig, out GameObject volIndicator))
                        {
                            volIndicator = GameObject.CreatePrimitive(PrimitiveType.Quad);
                            Object.Destroy(volIndicator.GetComponent<Collider>());

                            if (voiceEspMat == null)
                                voiceEspMat = new Material(Shader.Find("GUI/Text Shader"));

                            if (voiceTxt == null)
                                voiceTxt = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Visuals/speak.png", $"Images/Mods/Visuals/speak.png");

                            voiceEspMat.mainTexture = voiceTxt;

                            volIndicator.GetComponent<Renderer>().material = voiceEspMat;
                            voiceIndicators.Add(vrrig, volIndicator);
                        }

                        volIndicator.GetComponent<Renderer>().material.color = vrrig.GetColor();
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f) * vrrig.scaleFactor;
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (Classes.Menu.Console.GetIndicatorDistance(vrrig) * vrrig.scaleFactor);
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    }
                    else
                    {
                        if (voiceIndicators.TryGetValue(vrrig, out GameObject existing))
                        {
                            Object.Destroy(existing);
                            voiceIndicators.Remove(vrrig);
                        }
                    }
                }
            }
        }

        public static void DisableVoiceIndicators()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in voiceIndicators)
                Object.Destroy(nametag.Value);

            voiceIndicators.Clear();
        }

        private static GameObject l;
        private static GameObject r;

        private static void UpdateLimbColor()
        {
            Color color = VRRig.LocalRig.GetColor();

            l.GetComponent<Renderer>().material.color = color;
            r.GetComponent<Renderer>().material.color = color;
        }

        public static void StartNoLimb()
        {
            l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(l.GetComponent<SphereCollider>());

            l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(r.GetComponent<SphereCollider>());

            r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            UpdateLimbColor();
        }

        public static void NoLimbMode()
        {
            l.transform.position = ControllerUtilities.GetTrueLeftHand().position;
            r.transform.position = ControllerUtilities.GetTrueRightHand().position;
            VRRig.LocalRig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
            VRRig.LocalRig.mainSkin.material.color = new Color(VRRig.LocalRig.mainSkin.material.color.r, VRRig.LocalRig.mainSkin.material.color.g, VRRig.LocalRig.mainSkin.material.color.b, 0f);
            UpdateLimbColor();
        }

        public static void EndNoLimb()
        {
            Object.Destroy(l);
            Object.Destroy(r);

            VRRig.LocalRig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
            VRRig.LocalRig.mainSkin.material.color = new Color(VRRig.LocalRig.mainSkin.material.color.r, VRRig.LocalRig.mainSkin.material.color.g, VRRig.LocalRig.mainSkin.material.color.b, 1f);
        }

        private static readonly Dictionary<VRRig, List<LineRenderer>> boneESP = new Dictionary<VRRig, List<LineRenderer>>();
        public static readonly int[] bones = {
            4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7
        };

        public static void CasualBoneESP()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var boness in boneESP.Where(boness => !GorillaParent.instance.vrrigs.Contains(boness.Key)))
            {
                toRemove.Add(boness.Key);

                foreach (LineRenderer renderer in boness.Value)
                    Object.Destroy(renderer);
            }

            foreach (VRRig rig in toRemove)
                boneESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!boneESP.TryGetValue(vrrig, out List<LineRenderer> Lines))
                {
                    Lines = new List<LineRenderer>();

                    LineRenderer LineHead = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (smoothLines)
                    {
                        LineHead.numCapVertices = 10;
                        LineHead.numCornerVertices = 5;
                    }
                    LineHead.material.shader = Shader.Find("GUI/Text Shader");
                    Lines.Add(LineHead);

                    for (int i = 0; i < 19; i++)
                    {
                        LineRenderer Line = vrrig.mainSkin.bones[bones[i * 2]].gameObject.GetOrAddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            Line.numCapVertices = 10;
                            Line.numCornerVertices = 5;
                        }
                        Line.material.shader = Shader.Find("GUI/Text Shader");
                        Lines.Add(Line);
                    }

                    boneESP.Add(vrrig, Lines);
                }

                LineRenderer liner = Lines[0];

                Color color = vrrig.playerColor;
                if (fmt) 
                    color = backgroundColor.GetCurrentColor();
                if (tt) 
                    color.a = 0.5f;
                if (hoc) 
                    liner.gameObject.layer = 19;

                liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                liner.startColor = color;
                liner.endColor = color;

                liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                for (int i = 0; i < 19; i++)
                {
                    liner = Lines[i + 1];

                    if (hoc)
                        liner.gameObject.layer = 19;

                    liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                    liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                    liner.startColor = color;
                    liner.endColor = color;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.mainSkin.bones[bones[i * 2]].position);
                    liner.SetPosition(1, vrrig.mainSkin.bones[bones[i * 2 + 1]].position);
                }
            }
        }

        public static void InfectionBoneESP()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;
            bool selfTagged = VRRig.LocalRig.IsTagged();

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var boness in boneESP.Where(boness => !GorillaParent.instance.vrrigs.Contains(boness.Key)))
            {
                toRemove.Add(boness.Key);

                foreach (LineRenderer renderer in boness.Value)
                    Object.Destroy(renderer);
            }

            foreach (VRRig rig in toRemove)
                boneESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!boneESP.TryGetValue(vrrig, out List<LineRenderer> Lines))
                {
                    Lines = new List<LineRenderer>();

                    LineRenderer LineHead = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (smoothLines)
                    {
                        LineHead.numCapVertices = 10;
                        LineHead.numCornerVertices = 5;
                    }
                    LineHead.material.shader = Shader.Find("GUI/Text Shader");
                    Lines.Add(LineHead);

                    for (int i = 0; i < 19; i++)
                    {
                        LineRenderer Line = vrrig.mainSkin.bones[bones[i * 2]].gameObject.GetOrAddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            Line.numCapVertices = 10;
                            Line.numCornerVertices = 5;
                        }
                        Line.material.shader = Shader.Find("GUI/Text Shader");
                        Lines.Add(Line);
                    }

                    boneESP.Add(vrrig, Lines);
                }

                LineRenderer liner = Lines[0];

                bool playerTagged = vrrig.IsTagged();
                Color color = selfTagged ? vrrig.playerColor : vrrig.GetColor();

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    liner.gameObject.layer = 19;

                liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                liner.startColor = color;
                liner.endColor = color;

                liner.enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;

                liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                for (int i = 0; i < 19; i++)
                {
                    liner = Lines[i + 1];

                    if (hoc)
                        liner.gameObject.layer = 19;

                    liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                    liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                    liner.startColor = color;
                    liner.endColor = color;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;

                    liner.SetPosition(0, vrrig.mainSkin.bones[bones[i * 2]].position);
                    liner.SetPosition(1, vrrig.mainSkin.bones[bones[i * 2 + 1]].position);
                }
            }
        }

        public static void HuntBoneESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (var boness in boneESP.Where(boness => !GorillaParent.instance.vrrigs.Contains(boness.Key)))
            {
                toRemove.Add(boness.Key);

                foreach (LineRenderer renderer in boness.Value)
                    Object.Destroy(renderer);
            }

            foreach (VRRig rig in toRemove)
                boneESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!boneESP.TryGetValue(vrrig, out List<LineRenderer> Lines))
                {
                    Lines = new List<LineRenderer>();

                    LineRenderer LineHead = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (smoothLines)
                    {
                        LineHead.numCapVertices = 10;
                        LineHead.numCornerVertices = 5;
                    }
                    LineHead.material.shader = Shader.Find("GUI/Text Shader");
                    Lines.Add(LineHead);

                    for (int i = 0; i < 19; i++)
                    {
                        LineRenderer Line = vrrig.mainSkin.bones[bones[i * 2]].gameObject.GetOrAddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            Line.numCapVertices = 10;
                            Line.numCornerVertices = 5;
                        }
                        Line.material.shader = Shader.Find("GUI/Text Shader");
                        Lines.Add(Line);
                    }

                    boneESP.Add(vrrig, Lines);
                }

                LineRenderer liner = Lines[0];

                NetPlayer owner = GetPlayerFromVRRig(vrrig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);
                Color color = owner == target ? vrrig.GetColor() : theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear;

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    liner.gameObject.layer = 19;

                liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                liner.startColor = color;
                liner.endColor = color;

                liner.enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;

                liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                for (int i = 0; i < 19; i++)
                {
                    liner = Lines[i + 1];

                    if (hoc)
                        liner.gameObject.layer = 19;

                    liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                    liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                    liner.startColor = color;
                    liner.endColor = color;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;

                    liner.SetPosition(0, vrrig.mainSkin.bones[bones[i * 2]].position);
                    liner.SetPosition(1, vrrig.mainSkin.bones[bones[i * 2 + 1]].position);
                }
            }
        }

        public static void DisableBoneESP()
        {
            foreach (var renderer in boneESP.SelectMany(bones => bones.Value))
                Object.Destroy(renderer);

            boneESP.Clear();
        }

        public static void CasualSkeletonESP()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                vrrig.skeleton.renderer.enabled = true;
                vrrig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                vrrig.skeleton.renderer.material.color = vrrig.playerColor;
                if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.skeleton.renderer.material.color = backgroundColor.GetCurrentColor(); }
                if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.skeleton.renderer.material.color = new Color(vrrig.skeleton.renderer.material.color.r, vrrig.skeleton.renderer.material.color.g, vrrig.skeleton.renderer.material.color.b, 0.5f); }
            }
        }

        public static void InfectionSkeletonESP()
        {
            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.IsTagged())
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!VRRig.LocalRig.IsTagged())
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.IsTagged() && !vrrig.isLocal)
                        {
                            vrrig.skeleton.renderer.enabled = true;
                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.skeleton.renderer.material.color = vrrig.GetColor();
                            if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.skeleton.renderer.material.color = backgroundColor.GetCurrentColor(); }
                            if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.skeleton.renderer.material.color = new Color(vrrig.skeleton.renderer.material.color.r, vrrig.skeleton.renderer.material.color.g, vrrig.skeleton.renderer.material.color.b, 0.5f); }
                        }
                        else
                        {
                            vrrig.skeleton.renderer.enabled = false;
                            vrrig.skeleton.renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                            if (vrrig.skeleton.renderer.material.name.Contains("gorilla_body"))
                                vrrig.skeleton.renderer.material.color = vrrig.playerColor;
                        }
                    }
                }
                else
                {
                    foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.IsTagged() && !vrrig.isLocal))
                    {
                        vrrig.skeleton.renderer.enabled = true;
                        vrrig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.skeleton.renderer.material.color = vrrig.playerColor;
                        if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.skeleton.renderer.material.color = backgroundColor.GetCurrentColor(); }
                        if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.skeleton.renderer.material.color = new Color(vrrig.skeleton.renderer.material.color.r, vrrig.skeleton.renderer.material.color.g, vrrig.skeleton.renderer.material.color.b, 0.5f); }
                    }
                }
            }
            else
            {
                foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
                {
                    vrrig.skeleton.renderer.enabled = true;
                    vrrig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                    if (vrrig.skeleton.renderer.material.name.Contains("gorilla_body"))
                        vrrig.skeleton.renderer.material.color = vrrig.playerColor;
                }
            }
        }

        public static void HuntSkeletonESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = GetVRRigFromPlayer(player);
                if (player == target)
                {
                    vrrig.skeleton.renderer.enabled = true;
                    vrrig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.skeleton.renderer.material.color = vrrig.playerColor;
                    if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.skeleton.renderer.material.color = backgroundColor.GetCurrentColor(); }
                    if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.skeleton.renderer.material.color = new Color(vrrig.skeleton.renderer.material.color.r, vrrig.skeleton.renderer.material.color.g, vrrig.skeleton.renderer.material.color.b, 0.5f); }
                }
                else
                {
                    if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                    {
                        vrrig.skeleton.renderer.enabled = true;
                        vrrig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.skeleton.renderer.material.color = Color.red;
                        if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.skeleton.renderer.material.color = new Color(vrrig.skeleton.renderer.material.color.r, vrrig.skeleton.renderer.material.color.g, vrrig.skeleton.renderer.material.color.b, 0.5f); }
                    }
                    else
                    {
                        vrrig.skeleton.renderer.enabled = false;
                        vrrig.skeleton.renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (vrrig.skeleton.renderer.material.name.Contains("gorilla_body"))
                            vrrig.skeleton.renderer.material.color = vrrig.playerColor;
                    }
                }
            }
        }

        public static void DisableSkeletonESP()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                vrrig.skeleton.renderer.enabled = false;
                vrrig.skeleton.renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                if (vrrig.skeleton.renderer.material.name.Contains("gorilla_body"))
                    vrrig.skeleton.renderer.material.color = vrrig.playerColor;
            }
        }

        private static readonly Dictionary<VRRig, SkinnedWireframeRenderer> wireframes = new Dictionary<VRRig, SkinnedWireframeRenderer>();
        public static void CasualWireframeESP()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in wireframes.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value);
            }

            foreach (VRRig rig in toRemove)
                wireframes.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!wireframes.TryGetValue(rig, out SkinnedWireframeRenderer wireframe))
                {
                    wireframe = rig.mainSkin.AddComponent<SkinnedWireframeRenderer>();
                    wireframes.Add(rig, wireframe);
                }

                if (hoc)
                    wireframe.wireframeObj.layer = 19;

                Color color = rig.GetColor();

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color = new Color(color.r, color.g, color.b, 0.5f);

                wireframe.meshRenderer.material.color = color;

                Vector3 toTarget = rig.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                float angle = Vector3.Angle(GorillaTagger.Instance.headCollider.transform.forward, toTarget);

                bool enabled = angle <= Camera.main.fieldOfView / 1.75f;
                enabled &= Vector3.Distance(rig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.leftHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.rightHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f;

                wireframe.enabled = enabled;
                wireframe.meshRenderer.enabled = enabled;

                if (!enabled)
                {
                    FixRigMaterialESPColors(rig);

                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    rig.mainSkin.material.color = color;
                }
                else
                {
                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (rig.mainSkin.material.name.Contains("gorilla_body"))
                        rig.mainSkin.material.color = rig.playerColor;
                }
            }
        }

        public static void InfectionWireframeESP()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in wireframes.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value);
            }

            foreach (VRRig rig in toRemove)
                wireframes.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool selfTagged = VRRig.LocalRig.IsTagged();

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!wireframes.TryGetValue(rig, out SkinnedWireframeRenderer wireframe))
                {
                    wireframe = rig.mainSkin.AddComponent<SkinnedWireframeRenderer>();
                    wireframes.Add(rig, wireframe);
                }

                bool playerTagged = rig.IsTagged();
                Color color = selfTagged ? rig.playerColor : rig.GetColor();

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    wireframe.wireframeObj.layer = 19;

                wireframe.meshRenderer.material.color = color;

                bool enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;

                Vector3 toTarget = rig.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                float angle = Vector3.Angle(GorillaTagger.Instance.headCollider.transform.forward, toTarget);

                enabled &= angle <= Camera.main.fieldOfView / 1.75f;
                enabled &= Vector3.Distance(rig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.leftHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.rightHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f;

                wireframe.enabled = enabled;
                wireframe.meshRenderer.enabled = enabled;

                if (!enabled)
                {
                    FixRigMaterialESPColors(rig);

                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    rig.mainSkin.material.color = color;
                }
                else
                {
                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (rig.mainSkin.material.name.Contains("gorilla_body"))
                        rig.mainSkin.material.color = rig.playerColor;
                }
            }
        }

        public static void HuntWireframeESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in wireframes.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value);
            }

            foreach (VRRig rig in toRemove)
                wireframes.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!wireframes.TryGetValue(rig, out SkinnedWireframeRenderer wireframe))
                {
                    wireframe = rig.mainSkin.AddComponent<SkinnedWireframeRenderer>();
                    wireframes.Add(rig, wireframe);
                }

                NetPlayer owner = GetPlayerFromVRRig(rig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);

                Color color = owner == target ? rig.GetColor() : theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear;

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    wireframe.wireframeObj.layer = 19;

                wireframe.meshRenderer.material.color = color;

                bool enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;

                Vector3 toTarget = rig.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                float angle = Vector3.Angle(GorillaTagger.Instance.headCollider.transform.forward, toTarget);

                enabled &= angle <= Camera.main.fieldOfView / 1.75f;
                enabled &= Vector3.Distance(rig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.leftHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.rightHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f;

                wireframe.enabled = enabled;
                wireframe.meshRenderer.enabled = enabled;

                if (!enabled)
                {
                    FixRigMaterialESPColors(rig);

                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    rig.mainSkin.material.color = color;
                }
                else
                {
                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (rig.mainSkin.material.name.Contains("gorilla_body"))
                        rig.mainSkin.material.color = rig.playerColor;
                }
            }
        }

        public static void DisableWireframeESP()
        {
            foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> pred in wireframes)
            {
                pred.Key.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                Object.Destroy(pred.Value);
            }

            wireframes.Clear();
        }

        public class SkinnedWireframeRenderer : MonoBehaviour
        {
            public SkinnedMeshRenderer skinnedMeshRenderer;
            public Mesh lineMesh;
            public MeshFilter meshFilter;
            public MeshRenderer meshRenderer;
            public GameObject wireframeObj;
            public Color Color
            {
                get => meshRenderer.material.color;
                set => meshRenderer.material.color = value;
            }

            void Awake()
            {
                skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

                wireframeObj = new GameObject("Wireframe");
                wireframeObj.transform.SetParent(transform, false);

                meshFilter = wireframeObj.AddComponent<MeshFilter>();
                meshRenderer = wireframeObj.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("GUI/Text Shader"))
                {
                    color = Color.green
                };

                lineMesh = new Mesh();
                meshFilter.mesh = lineMesh;
            }

            void Update()
            {
                if (Time.frameCount % 3 > 0)
                    return;

                Mesh bakedMesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(bakedMesh);

                Vector3[] vertices = bakedMesh.vertices;
                int[] triangles = bakedMesh.triangles;

                List<Vector3> lineVertices = new List<Vector3>();
                List<int> lineIndices = new List<int>();

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int i0 = triangles[i];
                    int i1 = triangles[i + 1];
                    int i2 = triangles[i + 2];

                    lineVertices.Add(vertices[i0]);
                    lineVertices.Add(vertices[i1]);

                    lineVertices.Add(vertices[i1]);
                    lineVertices.Add(vertices[i2]);

                    lineVertices.Add(vertices[i2]);
                    lineVertices.Add(vertices[i0]);

                    int baseIndex = lineVertices.Count - 6;
                    for (int j = 0; j < 6; j++)
                        lineIndices.Add(baseIndex + j);
                }

                lineMesh.Clear();
                lineMesh.SetVertices(lineVertices);
                lineMesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
            }

            void OnDestroy()
            {
                if (lineMesh != null)
                {
                    Destroy(lineMesh);
                    lineMesh = null;
                }

                if (wireframeObj != null)
                {
                    Destroy(wireframeObj);
                    wireframeObj = null;
                }

                if (meshRenderer != null && meshRenderer.material != null)
                {
                    Destroy(meshRenderer.material);
                    meshRenderer.material = null;
                }
            }
        }

        private static readonly List<VRRig> convertedRigs = new List<VRRig>();
        public static void FixRigMaterialESPColors(VRRig rig)
        {
            if (!convertedRigs.Contains(rig))
            {
                convertedRigs.Add(rig);

                rig.mainSkin.sharedMesh.colors32 = Enumerable.Repeat((Color32)Color.white, rig.mainSkin.sharedMesh.colors32.Length).ToArray();
                rig.mainSkin.sharedMesh.colors = Enumerable.Repeat(Color.white, rig.mainSkin.sharedMesh.colors.Length).ToArray();
            }
        }

        public static Shader uberChams;

        private static readonly Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

        public static void Chams()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal && vrrig.colorInitialized && vrrig.initializedCosmetics && vrrig.mainSkin.material.shader.name != "Custom/UberChams"))
            {
                if (!uberChams)
                    uberChams = LoadAsset<Shader>("UberChams");

                static void updateShader(Material target, Material src, Color color, bool isFace = false)
                {
                    target.shader = uberChams;
                    Texture2DArray atlas = src.GetTexture("_BaseMap_Atlas") as Texture2DArray;
                    float slice = src.GetFloat("_BaseMap_AtlasSlice");
                    target.SetTexture("_BaseMap_Atlas", atlas);
                    target.SetFloat("_BaseMap_AtlasSlice", slice);
                    if (src.HasProperty("_BaseMap_ST"))
                    {
                        Vector4 st = src.GetVector("_BaseMap_ST");
                        target.SetVector("_BaseMap_ST", st);
                    }
                    if (isFace)
                    {
                        target.SetFloat("_UseMouthMap", 1.0f);
                        if (src.HasProperty("_MouthMap"))
                        {
                            Texture mouthTex = src.GetTexture("_MouthMap");
                            if (mouthTex != null)
                                target.SetTexture("_MouthMap", mouthTex);
                        }
                        if (src.HasProperty("_MouthMap_ST"))
                        {
                            Vector4 mouthST = src.GetVector("_MouthMap_ST");
                            target.SetVector("_MouthMap_ST", mouthST);
                        }
                    }
                    else
                    {
                        target.SetFloat("_UseMouthMap", 0f);
                    }
                    target.SetColor("_Color", color);
                }

                SkinnedMeshRenderer bodyRenderer = vrrig.mainSkin;
                if (!originalMaterials.ContainsKey(bodyRenderer))
                {
                    Material[] originals = new Material[bodyRenderer.materials.Length];
                    for (int i = 0; i < bodyRenderer.materials.Length; i++)
                        originals[i] = new Material(bodyRenderer.materials[i]);
                    originalMaterials[bodyRenderer] = originals;
                }

                Material[] materials = bodyRenderer.materials;
                for (int i = 0; i < materials.Length; i++)
                    updateShader(materials[i], i == 0 ? vrrig.materialsToChangeTo[vrrig.setMatIndex] : originalMaterials[bodyRenderer][i], materials[i].color);

                Renderer faceRenderer = vrrig.myMouthFlap.targetFaceRenderer;
                if (!originalMaterials.ContainsKey(faceRenderer))
                    originalMaterials[faceRenderer] = new[] { new Material(faceRenderer.material) };
                updateShader(faceRenderer.material, originalMaterials[faceRenderer][0], Color.white, true);

                if (Buttons.GetIndex("Show Cosmetics").enabled)
                {
                    foreach (GameObject obj in vrrig.cosmetics)
                    {
                        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (!originalMaterials.ContainsKey(renderer))
                            {
                                Material[] originals = new Material[renderer.materials.Length];
                                for (int i = 0; i < renderer.materials.Length; i++)
                                    originals[i] = new Material(renderer.materials[i]);
                                originalMaterials[renderer] = originals;
                            }
                            updateShader(renderer.material, originalMaterials[renderer][0], renderer.material.color);
                        }
                    }
                }
            }
        }

        public static void DisableShaderChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    SkinnedMeshRenderer bodyRenderer = vrrig.mainSkin;
                    if (originalMaterials.ContainsKey(bodyRenderer))
                    {
                        Material[] restoredMaterials = new Material[originalMaterials[bodyRenderer].Length];
                        for (int i = 0; i < restoredMaterials.Length; i++)
                            restoredMaterials[i] = new Material(originalMaterials[bodyRenderer][i]);
                        bodyRenderer.materials = restoredMaterials;
                        originalMaterials.Remove(bodyRenderer);
                    }

                    Renderer faceRenderer = vrrig.myMouthFlap.targetFaceRenderer;
                    if (originalMaterials.ContainsKey(faceRenderer))
                    {
                        faceRenderer.material = new Material(originalMaterials[faceRenderer][0]);
                        originalMaterials.Remove(faceRenderer);
                    }

                    foreach (GameObject obj in vrrig.cosmetics)
                    {
                        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (originalMaterials.ContainsKey(renderer))
                            {
                                Material[] restoredMaterials = new Material[originalMaterials[renderer].Length];
                                for (int i = 0; i < restoredMaterials.Length; i++)
                                    restoredMaterials[i] = new Material(originalMaterials[renderer][i]);
                                renderer.materials = restoredMaterials;
                                originalMaterials.Remove(renderer);
                            }
                        }
                    }
                }
            }
        }

        public static void CasualChams()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                FixRigMaterialESPColors(vrrig);

                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                vrrig.mainSkin.material.color = vrrig.playerColor;
                if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = backgroundColor.GetCurrentColor(); }
                if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
            }
        }

        public static void InfectionChams()
        {
            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.IsTagged())
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!VRRig.LocalRig.IsTagged())
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.IsTagged() && !vrrig.isLocal)
                        {
                            FixRigMaterialESPColors(vrrig);

                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.mainSkin.material.color = vrrig.GetColor();
                            if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = backgroundColor.GetCurrentColor(); }
                            if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        }
                        else
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                            if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                                vrrig.mainSkin.material.color = vrrig.playerColor;
                        }
                    }
                }
                else
                {
                    foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.IsTagged() && !vrrig.isLocal))
                    {
                        FixRigMaterialESPColors(vrrig);

                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = vrrig.playerColor;
                        if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = backgroundColor.GetCurrentColor(); }
                        if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    }
                }
            }
            else
            {
                foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
                {
                    FixRigMaterialESPColors(vrrig);

                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                        vrrig.mainSkin.material.color = vrrig.playerColor;
                }
            }
        }

        public static void HuntChams()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = GetVRRigFromPlayer(player);
                if (player == target)
                {
                    FixRigMaterialESPColors(vrrig);

                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (Buttons.GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = backgroundColor.GetCurrentColor(); }
                    if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                } 
                else 
                {
                    if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = Color.red;
                        if (Buttons.GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    }
                    else
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                            vrrig.mainSkin.material.color = vrrig.playerColor;
                    }
                }
            }
        }

        public static void DisableChams()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                foreach (Material mat in vrrig.mainSkin.materials)
                    mat.shader = Shader.Find("GorillaTag/UberShader");
                if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                    vrrig.mainSkin.material.color = vrrig.playerColor;
            }
        }

        private static readonly Dictionary<VRRig, GameObject> boxESP = new Dictionary<VRRig, GameObject>();
        public static void CasualBoxESP()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var box in boxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                boxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!boxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(box.GetComponent<BoxCollider>());

                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    boxESP.Add(vrrig, box);
                }

                Color color = vrrig.playerColor;
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                box.GetComponent<Renderer>().material.color = color;

                box.transform.position = vrrig.transform.position;
                box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void InfectionBoxESP()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool selfTagged = VRRig.LocalRig.IsTagged();

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var box in boxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                boxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!boxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(box.GetComponent<BoxCollider>());

                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    boxESP.Add(vrrig, box);
                }

                Color color = selfTagged ? vrrig.playerColor : vrrig.GetColor();
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                box.GetComponent<Renderer>().material.color = color;

                bool playerTagged = vrrig.IsTagged();
                box.SetActive((selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0);

                box.transform.position = vrrig.transform.position;
                box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void HuntBoxESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (var box in boxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                boxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!boxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(box.GetComponent<BoxCollider>());

                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    boxESP.Add(vrrig, box);
                }

                NetPlayer owner = GetPlayerFromVRRig(vrrig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);

                Color color = owner == target ? vrrig.GetColor() : theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear;
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                box.GetComponent<Renderer>().material.color = color;

                box.SetActive(owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer);

                box.transform.position = vrrig.transform.position;
                box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void DisableBoxESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> box in boxESP)
                Object.Destroy(box.Value);

            boxESP.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> hollowBoxESP = new Dictionary<VRRig, GameObject>();
        public static void CasualHollowBoxESP()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var box in hollowBoxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                hollowBoxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!hollowBoxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    Renderer boxRenderer = box.GetComponent<Renderer>();
                    boxRenderer.enabled = false;
                    boxRenderer.material.shader = Shader.Find("GUI/Text Shader");

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(1f, thinTracers ? 0.025f : 0.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 1.025f : 1.1f, thinTracers ? 0.025f : 0.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    hollowBoxESP.Add(vrrig, box);
                }

                Color color = vrrig.playerColor;
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                box.GetComponent<Renderer>().material.color = color;

                box.transform.position = vrrig.transform.position;
                box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void HollowInfectionBoxESP()
        {
            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;
            bool selfTagged = VRRig.LocalRig.IsTagged();

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var box in hollowBoxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                hollowBoxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!hollowBoxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    Renderer boxRenderer = box.GetComponent<Renderer>();
                    boxRenderer.enabled = false;
                    boxRenderer.material.shader = Shader.Find("GUI/Text Shader");

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(1f, thinTracers ? 0.025f : 0.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 1.025f : 1.1f, thinTracers ? 0.025f : 0.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    hollowBoxESP.Add(vrrig, box);
                }

                Color color = selfTagged ? vrrig.playerColor : vrrig.GetColor();
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                box.GetComponent<Renderer>().material.color = color;

                bool playerTagged = vrrig.IsTagged();
                box.SetActive((selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0);

                box.transform.position = vrrig.transform.position;
                box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void HollowHuntBoxESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (var box in hollowBoxESP.Where(box => !GorillaParent.instance.vrrigs.Contains(box.Key)))
            {
                toRemove.Add(box.Key);
                Object.Destroy(box.Value);
            }

            foreach (VRRig rig in toRemove)
                hollowBoxESP.Remove(rig);

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!hollowBoxESP.TryGetValue(vrrig, out GameObject box))
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    Renderer boxRenderer = box.GetComponent<Renderer>();
                    boxRenderer.enabled = false;
                    boxRenderer.material.shader = Shader.Find("GUI/Text Shader");

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(1f, thinTracers ? 0.025f : 0.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 1.025f : 1.1f, thinTracers ? 0.025f : 0.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.SetParent(box.transform);
                    outl.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                    Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                    outl.transform.localRotation = Quaternion.identity;
                    outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                    hollowBoxESP.Add(vrrig, box);
                }

                NetPlayer owner = GetPlayerFromVRRig(vrrig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);

                Color color = owner == target ? vrrig.GetColor() : theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear;
                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    box.layer = 19;

                box.GetComponent<Renderer>().material.color = color;
                box.SetActive(owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer);

                box.transform.position = vrrig.transform.position;
                box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
            }
        }

        public static void DisableHollowBoxESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> box in hollowBoxESP)
                Object.Destroy(box.Value);

            hollowBoxESP.Clear();
        }

        private static readonly Dictionary<VRRig, TrailRenderer> breadcrumbs = new Dictionary<VRRig, TrailRenderer>();
        public static void CasualBreadcrumbs()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in breadcrumbs.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value);
            }

            foreach (VRRig rig in toRemove)
                breadcrumbs.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;
            bool shortBreadcrumbs = Buttons.GetIndex("Short Breadcrumbs").enabled;

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!breadcrumbs.TryGetValue(rig, out TrailRenderer trail))
                {
                    trail = rig.head.rigTarget.gameObject.GetOrAddComponent<TrailRenderer>();

                    trail.minVertexDistance = 0.05f;

                    if (smoothLines)
                    {
                        trail.numCapVertices = 10;
                        trail.numCornerVertices = 5;
                    }

                    trail.material.shader = Shader.Find("GUI/Text Shader");
                    trail.time = shortBreadcrumbs ? 1f : 10f;

                    breadcrumbs.Add(rig, trail);
                }

                trail.startWidth = thinTracers ? 0.0075f : 0.025f;
                trail.endWidth = thinTracers ? 0.0075f : 0.025f;

                if (hoc)
                    trail.gameObject.layer = 19;

                Color color = rig.GetColor();

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color = new Color(color.r, color.g, color.b, 0.5f);

                trail.startColor = color;
                trail.endColor = color;
            }
        }

        public static void InfectionBreadcrumbs()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in breadcrumbs.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value);
            }

            foreach (VRRig rig in toRemove)
                breadcrumbs.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;
            bool shortBreadcrumbs = Buttons.GetIndex("Short Breadcrumbs").enabled;
            bool selfTagged = VRRig.LocalRig.IsTagged();

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!breadcrumbs.TryGetValue(rig, out TrailRenderer trail))
                {
                    trail = rig.head.rigTarget.gameObject.GetOrAddComponent<TrailRenderer>();

                    trail.minVertexDistance = 0.05f;

                    if (smoothLines)
                    {
                        trail.numCapVertices = 10;
                        trail.numCornerVertices = 5;
                    }

                    trail.material.shader = Shader.Find("GUI/Text Shader");
                    trail.time = shortBreadcrumbs ? 1f : 10f;

                    breadcrumbs.Add(rig, trail);
                }

                trail.startWidth = thinTracers ? 0.0075f : 0.025f;
                trail.endWidth = thinTracers ? 0.0075f : 0.025f;

                bool playerTagged = rig.IsTagged();
                Color color = selfTagged ? rig.playerColor : rig.GetColor();

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    trail.gameObject.layer = 19;

                trail.startColor = color;
                trail.endColor = color;

                trail.enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;
            }
        }

        public static void HuntBreadcrumbs()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (var lines in breadcrumbs.Where(lines => !GorillaParent.instance.vrrigs.Contains(lines.Key)))
            {
                toRemove.Add(lines.Key);
                Object.Destroy(lines.Value);
            }

            foreach (VRRig rig in toRemove)
                breadcrumbs.Remove(rig);

            bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
            bool tt = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;
            bool shortBreadcrumbs = Buttons.GetIndex("Short Breadcrumbs").enabled;

            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.isLocal))
            {
                if (!breadcrumbs.TryGetValue(rig, out TrailRenderer trail))
                {
                    trail = rig.head.rigTarget.gameObject.GetOrAddComponent<TrailRenderer>();

                    trail.minVertexDistance = 0.05f;

                    if (smoothLines)
                    {
                        trail.numCapVertices = 10;
                        trail.numCornerVertices = 5;
                    }

                    trail.material.shader = Shader.Find("GUI/Text Shader");
                    trail.time = shortBreadcrumbs ? 1f : 10f;

                    breadcrumbs.Add(rig, trail);
                }

                trail.startWidth = thinTracers ? 0.0075f : 0.025f;
                trail.endWidth = thinTracers ? 0.0075f : 0.025f;

                NetPlayer owner = GetPlayerFromVRRig(rig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);

                Color color = owner == target ? rig.GetColor() : theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear;

                if (fmt)
                    color = backgroundColor.GetCurrentColor();
                if (tt)
                    color.a = 0.5f;
                if (hoc)
                    trail.gameObject.layer = 19;

                trail.startColor = color;
                trail.endColor = color;

                trail.enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;
            }
        }

        public static void DisableBreadcrumbs()
        {
            foreach (KeyValuePair<VRRig, TrailRenderer> pred in breadcrumbs)
                Object.Destroy(pred.Value);

            breadcrumbs.Clear();
        }

        // Thanks DrPerky for rewriting visual mods <@427495360517111809>
        public static bool DoPerformanceCheck()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return true;
                }
                else
                {
                    PerformanceVisualDelay = Time.time + PerformanceModeStep;
                    DelayChangeStep = Time.frameCount;
                }
            }

            return false;
        }

        static GameObject LeftSphere;
        static GameObject RightSphere;
        public static void ShowButtonColliders()
        {
            if (LeftSphere == null || RightSphere == null)
            {
                if (LeftSphere == null)
                {
                    LeftSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(LeftSphere.GetComponent<SphereCollider>());

                    LeftSphere.transform.parent = GorillaTagger.Instance.leftHandTransform;
                    LeftSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (RightSphere == null)
                {
                    RightSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(RightSphere.GetComponent<SphereCollider>());

                    RightSphere.transform.parent = GorillaTagger.Instance.leftHandTransform;
                    RightSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }
            }

            LeftSphere.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
            LeftSphere.transform.localPosition = pointerOffset;

            RightSphere.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
            RightSphere.transform.localPosition = pointerOffset;
        }

        public static void HideButtonColliders()
        {
            Object.Destroy(LeftSphere);
            Object.Destroy(RightSphere);

            LeftSphere = null;
            RightSphere = null;
        }

        public static void AutomaticESP(Action infection, Action hunt, Action other)
        {
            if (!PhotonNetwork.InRoom) return;
            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    infection.Invoke();
                    break;
                case GameModeType.HuntDown:
                    hunt.Invoke();
                    break;
                default:
                    other.Invoke();
                    break;
            }
        }

        // Tracers
        public static void CasualTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            float lineWidth = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            Color menuColor = backgroundColor.GetCurrentColor();

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
                    continue;

                Color lineColor = playerRig.playerColor;

                LineRenderer line = GetLineRender();

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, playerRig.transform.position);
            }
        }
        
        public static void NearestTracer()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            float lineWidth = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            Color menuColor = backgroundColor.GetCurrentColor();

            float distance = float.MaxValue;
            VRRig playerRig = VRRig.LocalRig;
            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => distance > Vector3.Distance(rig.transform.position, VRRig.LocalRig.transform.position) && !rig.isLocal))
            {
                distance = Vector3.Distance(rig.transform.position, VRRig.LocalRig.transform.position);
                playerRig = rig;
            }

            if (!playerRig.isLocal)
            {
                Color lineColor = playerRig.playerColor;

                LineRenderer line = GetLineRender();

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, playerRig.transform.position);
            }
        }

        public static void InfectionTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            float lineWidth = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            bool LocalTagged = VRRig.LocalRig.IsTagged();
            bool NoInfected = InfectedList().Count == 0;

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
                    continue;

                Color lineColor = playerRig.playerColor;

                if (!NoInfected)
                {
                    if (LocalTagged)
                    {
                        if (playerRig.IsTagged())
                            continue;
                    }
                    else
                    {
                        if (!playerRig.IsTagged())
                            continue;

                        lineColor = playerRig.GetColor();
                    }
                }

                LineRenderer line = GetLineRender();

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, playerRig.transform.position);
            }
        }

        public static void HuntTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;

            if (sillyComputer == null)
                return;

            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            float lineWidth = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            foreach (var playerRig in GorillaParent.instance.vrrigs.Where(playerRig => !playerRig.isLocal))
            {
                if (GetPlayerFromVRRig(playerRig) == currentTarget)
                {
                    Color lineColor = playerRig.playerColor;

                    LineRenderer line = GetLineRender();

                    if (transparentTheme)
                        lineColor.a = 0.5f;

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;
                    line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    line.SetPosition(1, playerRig.transform.position);
                }
                else if (sillyComputer.IsTargetOf(GetPlayerFromVRRig(playerRig), PhotonNetwork.LocalPlayer))
                {
                    Color lineColor = Color.red;

                    LineRenderer line = GetLineRender();

                    if (transparentTheme)
                        lineColor.a = 0.5f;

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;
                    line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    line.SetPosition(1, playerRig.transform.position);
                }
            }
        }

        // Beacons
        public static void CasualBeacons()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            _ = Buttons.GetIndex("Hidden on Camera").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            Color menuColor = backgroundColor.GetCurrentColor();

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
                    continue;

                Color lineColor = playerRig.playerColor;

                LineRenderer line = GetLineRender();

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                float width = thinTracers ? 0.0075f : 0.025f;
                line.startWidth = width;
                line.endWidth = width;
                line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
            }
        }

        public static void InfectionBeacons()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            _ = Buttons.GetIndex("Hidden on Camera").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            bool LocalTagged = VRRig.LocalRig.IsTagged();
            bool NoInfected = InfectedList().Count == 0;

            Color menuColor = backgroundColor.GetCurrentColor();

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
                    continue;

                Color lineColor = playerRig.playerColor;

                if (!NoInfected)
                {
                    if (LocalTagged)
                    {
                        if (playerRig.IsTagged())
                            continue;
                    }
                    else
                    {
                        if (!playerRig.IsTagged())
                            continue;

                        lineColor = playerRig.GetColor();
                    }
                }

                LineRenderer line = GetLineRender();

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                float width = thinTracers ? 0.0075f : 0.025f;
                line.startWidth = width;
                line.endWidth = width;
                line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
            }
        }

        public static void HuntBeacons()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;

            if (sillyComputer == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;

            Color menuColor = backgroundColor.GetCurrentColor();

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            foreach (var playerRig in GorillaParent.instance.vrrigs.Where(playerRig => !playerRig.isLocal))
            {
                if (GetPlayerFromVRRig(playerRig) == currentTarget)
                {
                    Color lineColor = playerRig.playerColor;
                    LineRenderer line = GetLineRender();

                    if (followMenuTheme)
                        lineColor = menuColor;

                    if (transparentTheme)
                        lineColor.a = 0.5f;

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    float width = thinTracers ? 0.0075f : 0.025f;
                    line.startWidth = width;
                    line.endWidth = width;
                    line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                    line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
                } 
                else if (sillyComputer.IsTargetOf(GetPlayerFromVRRig(playerRig), PhotonNetwork.LocalPlayer))
                {
                    Color lineColor = Color.red;

                    LineRenderer line = GetLineRender();

                    if (followMenuTheme)
                        lineColor = menuColor;

                    if (transparentTheme)
                        lineColor.a = 0.5f;

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    float width = thinTracers ? 0.0075f : 0.025f;
                    line.startWidth = width;
                    line.endWidth = width;
                    line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                    line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
                }
            }
        }

        // Distance ESP
        public static void CasualDistanceESP()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = Buttons.GetIndex("Hidden on Camera").enabled;

            Color menuColor = backgroundColor.GetCurrentColor();

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal) // Skip local player
                    continue;

                Color tagColor = followMenuTheme ? textColors[0].GetCurrentColor() : Color.white;
                Color backgroundColor = playerRig.playerColor;

                if (followMenuTheme)
                    backgroundColor = menuColor;

                if (transparentTheme)
                {
                    backgroundColor.a = 0.5f;
                    tagColor.a = 0.5f;
                }

                TextMeshPro nameTagText = GetNameTag(hiddenOnCamera);

                nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                nameTagText.color = tagColor;

                _ = $"{Vector3.Distance(Camera.main.transform.position, playerRig.transform.position):F1}m";

                foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) //background color
                {
                    if (transform.gameObject.name == "bg")
                    {
                        transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                        transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    }
                }
            }
        }

        public static void InfectionDistanceESP()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = Buttons.GetIndex("Hidden on Camera").enabled;

            bool LocalTagged = VRRig.LocalRig.IsTagged();
            bool NoInfected = InfectedList().Count == 0;

            Color menuColor = backgroundColor.GetCurrentColor();

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal) //skip local player
                    continue;

                Color tagColor = followMenuTheme ? textColors[0].GetCurrentColor() : Color.white;
                Color backgroundColor = playerRig.playerColor;

                if (!NoInfected)
                {
                    if (LocalTagged)
                    {
                        if (playerRig.IsTagged())
                            continue;
                    }
                    else
                    {
                        if (!playerRig.IsTagged())
                            continue;

                        backgroundColor = playerRig.GetColor();
                    }
                }

                if (followMenuTheme)
                    backgroundColor = menuColor;

                if (transparentTheme)
                {
                    backgroundColor.a = 0.5f;
                    tagColor.a = 0.5f;
                }

                TextMeshPro nameTagText = GetNameTag(hiddenOnCamera);

                nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                nameTagText.color = tagColor;

                _ = $"{Vector3.Distance(Camera.main.transform.position, playerRig.transform.position):F1}m";

                foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) //background color
                {
                    if (transform.gameObject.name == "bg")
                    {
                        transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                        transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    }
                }
            }

        }

        public static void HuntDistanceESP()
        {
            if (DoPerformanceCheck())
                return;

            // Sanity checks, dont remove these

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;
            
            if (sillyComputer == null)
                return;

            // Cache these here so your not finding the values from Buttons.GetIndex every call (Buttons.GetIndex is fucking slow)
            bool followMenuTheme = Buttons.GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = Buttons.GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = Buttons.GetIndex("Hidden on Camera").enabled;

            Color menuColor = backgroundColor.GetCurrentColor();

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            // Color bgColor = backgroundColor.GetCurrentColor(); //dont need to call this function twice, just use a variable

            foreach (var playerRig in GorillaParent.instance.vrrigs.Where(playerRig => !playerRig.isLocal))
            {
                if (GetPlayerFromVRRig(playerRig) == currentTarget) // Use ID for quick comparison
                {
                    Color tagColor = followMenuTheme ? textColors[0].GetCurrentColor() : Color.white;
                    Color backgroundColor = playerRig.playerColor;

                    if (followMenuTheme)
                        backgroundColor = menuColor;

                    if (transparentTheme)
                    {
                        backgroundColor.a = 0.5f;
                        tagColor.a = 0.5f;
                    }

                    TextMeshPro nameTagText = GetNameTag(hiddenOnCamera);

                    nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    nameTagText.color = tagColor;

                    _ = $"{Vector3.Distance(Camera.main.transform.position, playerRig.transform.position):F1}m";

                    foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) // Background color
                    {
                        if (transform.gameObject.name == "bg")
                        {
                            transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                            transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                        }
                    }
                } 
                else if (sillyComputer.IsTargetOf(GetPlayerFromVRRig(playerRig), PhotonNetwork.LocalPlayer))
                {
                    Color tagColor = followMenuTheme ? textColors[0].GetCurrentColor() : Color.white;
                    Color backgroundColor = Color.red;

                    if (followMenuTheme)
                        backgroundColor = menuColor;

                    if (transparentTheme)
                    {
                        backgroundColor.a = 0.5f;
                        tagColor.a = 0.5f;
                    }

                    TextMeshPro nameTagText = GetNameTag(hiddenOnCamera);

                    nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    nameTagText.color = tagColor;

                    _ = $"{Vector3.Distance(Camera.main.transform.position, playerRig.transform.position):F1}m";

                    foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) // Background color
                    {
                        if (transform.gameObject.name == "bg")
                        {
                            transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                            transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                        }
                    }
                }
            }
        }

        // Cache backend

        private static readonly List<TextMeshPro> nameTagPool = new List<TextMeshPro>();

        private static GameObject nameTagHolder;

        public static bool isNameTagQueued;

        private static TextMeshPro GetNameTag(bool hideOnCamera)
        {
            if (nameTagHolder == null)
                nameTagHolder = new GameObject("NameTag_Holder");

            TextMeshPro finalTextMeshPro = null;

            foreach (var TextMeshPro in nameTagPool.Where(TextMeshPro => finalTextMeshPro == null && !TextMeshPro.gameObject.activeInHierarchy))
            {
                TextMeshPro.gameObject.SetActive(true);
                TextMeshPro.gameObject.transform.LookAt(Camera.main.transform.position);
                TextMeshPro.gameObject.transform.Rotate(0f, 180f, 0f);

                TextMeshPro.SafeSetFontStyle(activeFontStyle);
                TextMeshPro.SafeSetFont(activeFont);

                // Update font style of outline here

                finalTextMeshPro = TextMeshPro;
            }

            if (finalTextMeshPro == null)
            {
                GameObject MeshHolder = new GameObject("TextMeshProObject");
                MeshHolder.transform.parent = nameTagHolder.transform;
                TextMeshPro newMesh = MeshHolder.AddComponent<TextMeshPro>();

                Renderer MeshRender = newMesh.GetComponent<Renderer>();

                newMesh.fontSize = 1.8f;
                newMesh.SafeSetFontStyle(activeFontStyle);
                newMesh.alignment = TextAlignmentOptions.Center;
                newMesh.color = Color.white;

                GameObject backgroundObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.Destroy(backgroundObject.GetComponent<Collider>());

                Renderer backgroundRender = backgroundObject.GetComponent<Renderer>();

                backgroundObject.name = "bg";
                backgroundObject.transform.parent = MeshHolder.transform;
                backgroundObject.transform.localPosition = Vector3.zero;
                backgroundObject.transform.localScale = new Vector3(MeshRender.bounds.size.x + 0.2f, 0.2f, 0.01f);
                backgroundRender.material.shader = Shader.Find("GUI/Text Shader");
                backgroundRender.material.color = Color.white;
                MeshRender.material.renderQueue = backgroundRender.material.renderQueue + 2;

                newMesh.outlineWidth = 0.2f;
                newMesh.outlineColor = Color.black;

                nameTagPool.Add(newMesh);

                finalTextMeshPro = newMesh;
            }

            finalTextMeshPro.gameObject.layer = hideOnCamera ? 19 : // What does 19 actually do?
                nameTagHolder.layer;

            return finalTextMeshPro;
        }

        public static void ClearNameTagPool(bool destroy = false) // Set destroy when you disable a feature that needs a lot of nameTags
        {
            if (DoPerformanceCheck())
                return;

            foreach (TextMeshPro TextMeshPro in nameTagPool)
            {
                if (destroy || isNameTagQueued)
                    Object.Destroy(TextMeshPro.gameObject);
                else
                    TextMeshPro.gameObject.SetActive(false);
            }

            if (destroy || isNameTagQueued)
                nameTagPool.Clear();

            isNameTagQueued = false;
        }


        private static readonly List<LineRenderer> linePool = new List<LineRenderer>();

        private static GameObject lineRenderHolder;

        public static bool isLineRenderQueued = false;

        public static LineRenderer GetLineRender()
        {
            bool hideOnCamera = Buttons.GetIndex("Hidden on Camera").enabled;

            if (lineRenderHolder == null)
                lineRenderHolder = new GameObject("LineRender_Holder");

            LineRenderer finalRender = null;

            foreach (var line in linePool.Where(line => finalRender == null).Where(line => !line.gameObject.activeInHierarchy))
            {
                line.gameObject.SetActive(true);
                finalRender = line;
            }

            if (finalRender == null)
            {
                GameObject lineHolder = new GameObject("LineObject");
                lineHolder.transform.parent = lineRenderHolder.transform;
                LineRenderer newLine = lineHolder.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    newLine.numCapVertices = 10;
                    newLine.numCornerVertices = 5;
                }
                newLine.material.shader = Shader.Find("GUI/Text Shader");
                newLine.startWidth = 0.025f;
                newLine.endWidth = 0.025f;
                newLine.positionCount = 2;
                newLine.useWorldSpace = true;

                linePool.Add(newLine);

                finalRender = newLine;
            }

            finalRender.gameObject.layer = hideOnCamera ? 19 : lineRenderHolder.layer;

            return finalRender;
        }

        public static void ClearLinePool(bool destroy = false) // Set destroy when you disable a feature that needs a lot of lines
        {
            if (DoPerformanceCheck())
                return;

            foreach (LineRenderer line in linePool)
            {
                if (destroy || isLineRenderQueued)
                    Object.Destroy(line.gameObject);
                else
                    line.gameObject.SetActive(false);
            }

            if (destroy || isLineRenderQueued)
                linePool.Clear();
        }
        
        public static void ConsoleBeacon(string id, string version, string menuName)
        {
            NetPlayer sender = GetPlayerFromID(id);
            VRRig vrrig = GetVRRigFromPlayer(sender);

            Color userColor = Color.red;

            NotificationManager.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> " + sender.NickName + " is using " + menuName + " version " + version + ".", 3000);
            VRRig.LocalRig.PlayHandTapLocal(29, false, 99999f);
            VRRig.LocalRig.PlayHandTapLocal(29, true, 99999f);
            GameObject line = new GameObject("Line");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = userColor; liner.endColor = userColor; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 2; liner.useWorldSpace = true;

            liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
            liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
            liner.material.shader = Shader.Find("GUI/Text Shader");
            Object.Destroy(line, 3f);
        }
    }
}
