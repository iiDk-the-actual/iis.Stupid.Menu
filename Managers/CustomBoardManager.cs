/*
 * ii's Stupid Menu  Managers/CustomBoardManager.cs
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

using GorillaNetworking;
using iiMenu.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static iiMenu.Menu.Main;

namespace iiMenu.Managers
{
    public class CustomBoardManager : MonoBehaviour
    {
        public static CustomBoardManager instance;
        public void Awake()
        {
            instance = this;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private static bool _customBoardsEnabled = true;
        public static bool CustomBoardsEnabled
        {
            get => _customBoardsEnabled;
            set
            {
                _customBoardsEnabled = value;

                if (value)
                {
                    instance.ReloadBoards();
                    instance.motdTitle.SetActive(true);
                    instance.motdText.SetActive(true);

                    GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText").SetActive(false);
                    GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText").SetActive(false);
                } else
                {
                    foreach (GorillaNetworkJoinTrigger joinTrigger in PhotonNetworkController.Instance.allJoinTriggers)
                    {
                        try
                        {
                            JoinTriggerUI ui = joinTrigger.ui;
                            JoinTriggerUITemplate temp = ui.template;

                            if (_screenRed == null)
                            {
                                _screenRed = new Material(Shader.Find("GorillaTag/UberShader"))
                                {
                                    color = new Color32(226, 73, 41, 255)
                                };
                            }

                            if (_screenBlack == null)
                            {
                                _screenBlack = new Material(Shader.Find("GorillaTag/UberShader"))
                                {
                                    color = new Color32(39, 34, 28, 255)
                                };
                            }

                            temp.ScreenBG_AbandonPartyAndSoloJoin = _screenRed;
                            temp.ScreenBG_AlreadyInRoom = _screenBlack;
                            temp.ScreenBG_Error = _screenRed;
                        }
                        catch { }
                    }

                    var stumpChildren = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.Children()
                                   .Where(x => x.name.Contains("UnityTempFile"))
                                   .ToList();

                    if (StumpLeaderboardIndex >= 0 && StumpLeaderboardIndex < stumpChildren.Count)
                    {
                        var stumpBoard = stumpChildren[StumpLeaderboardIndex];
                        if (stumpBoard != null && instance.stumpMaterial != null)
                            stumpBoard.GetComponent<Renderer>().material = instance.stumpMaterial;
                    }

                    var forestChildren = GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.Children()
                        .Where(x => x.name.Contains("UnityTempFile"))
                        .ToList();

                    if (ForestLeaderboardIndex >= 0 && ForestLeaderboardIndex < forestChildren.Count)
                    {
                        var forestBoard = forestChildren[ForestLeaderboardIndex];
                        if (forestBoard != null && instance.forestMaterial != null)
                            forestBoard.GetComponent<Renderer>().material = instance.forestMaterial;
                    }

                    foreach (GameObject board in instance.objectBoards.Values)
                        Destroy(board);

                    instance.objectBoards.Clear();

                    instance.motdTitle.SetActive(false);
                    instance.motdText.SetActive(false);

                    GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText").SetActive(true);
                    GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText").SetActive(true);
                }
            }
        }

        private static readonly Dictionary<TextMeshPro, float> characterDistanceArchive = new Dictionary<TextMeshPro, float>();

        private static bool _customBoardFonts;
        public static bool CustomBoardFonts
        {
            get => _customBoardFonts;
            set
            {
                if (!value && _customBoardFonts)
                {
                    foreach (TextMeshPro txt in instance.textMeshPro.Where(text => text.isActiveAndEnabled))
                    {
                        txt.SafeSetFont(instance.archiveGorillaTagFont);
                        txt.SafeSetFontStyle(FontStyles.Normal);

                        if (characterDistanceArchive.TryGetValue(txt, out float charDistance))
                            txt.characterSpacing = charDistance;
                    }
                }

                _customBoardFonts = value;
            }
        }

        private static Material _screenRed;
        private static Material _screenBlack;

        public static bool CustomBoardTextEnabled = true;
        private static Material _boardMaterial = new Material(Shader.Find("GorillaTag/UberShader"));
        public static Material BoardMaterial
        {
            get => _boardMaterial;
            set
            {
                if (value == null)
                    value = new Material(Shader.Find("GorillaTag/UberShader"));

                _boardMaterial = value;
                instance.ReloadBoards();
            }
        }

        #region Game Boards
        public const int StumpLeaderboardIndex = 3;
        public const int ForestLeaderboardIndex = 3;

        public static string motdTemplate = "You are using build {0}. This menu was created by iiDk (@crimsoncauldron) on Discord. " +
        "This menu is completely free and open sourced, if you paid for this menu you have been scammed. " +
        "There are a total of <b>{1}</b> mods on this menu. " +
        "<color=red>I, iiDk, am not responsible for any bans using this menu.</color> " +
        "If you get banned while using this, it's your responsibility.\n\nCurrent menu status: <b>Loading...</b>\nMade with <3 by iiDk, kingofnetflix, and others\n\n<alpha=128>{2} {0} {3}<alpha=255>";

        public Material forestMaterial;
        public Material stumpMaterial;

        public GameObject motdTitle;
        public GameObject motdText;

        private TMP_FontAsset archiveGorillaTagFont;

        private bool hasFoundAllBoards;
        public void ReloadBoards() =>
            hasFoundAllBoards = false;

        public void Update()
        {
            if (!hasFoundAllBoards)
            {
                try
                {
                    foreach (GameObject board in objectBoards.Values)
                        Destroy(board);

                    objectBoards.Clear();

                    var stumpChildren = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.Children()
                       .Where(x => x.name.Contains("UnityTempFile"))
                       .ToList();

                    if (StumpLeaderboardIndex >= 0 && StumpLeaderboardIndex < stumpChildren.Count)
                    {
                        var stumpBoard = stumpChildren[StumpLeaderboardIndex];
                        if (stumpBoard != null)
                        {
                            if (stumpMaterial == null)
                                stumpMaterial = stumpBoard.GetComponent<Renderer>().material;

                            stumpBoard.GetComponent<Renderer>().material = BoardMaterial;
                        }
                    }

                    var forestChildren = GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.Children()
                        .Where(x => x.name.Contains("UnityTempFile"))
                        .ToList();

                    if (ForestLeaderboardIndex >= 0 && ForestLeaderboardIndex < forestChildren.Count)
                    {
                        var forestBoard = forestChildren[ForestLeaderboardIndex];
                        if (forestBoard != null)
                        {
                            if (forestMaterial == null)
                                forestMaterial = forestBoard.GetComponent<Renderer>().material;

                            forestBoard.GetComponent<Renderer>().material = BoardMaterial;
                        }
                    }

                    foreach (GorillaNetworkJoinTrigger joinTrigger in PhotonNetworkController.Instance.allJoinTriggers)
                    {
                        try
                        {
                            JoinTriggerUI ui = joinTrigger.ui;
                            JoinTriggerUITemplate temp = ui.template;

                            temp.ScreenBG_AbandonPartyAndSoloJoin = BoardMaterial;
                            temp.ScreenBG_AlreadyInRoom = BoardMaterial;
                            temp.ScreenBG_ChangingGameModeSoloJoin = BoardMaterial;
                            temp.ScreenBG_Error = BoardMaterial;
                            temp.ScreenBG_InPrivateRoom = BoardMaterial;
                            temp.ScreenBG_LeaveRoomAndGroupJoin = BoardMaterial;
                            temp.ScreenBG_LeaveRoomAndSoloJoin = BoardMaterial;
                            temp.ScreenBG_NotConnectedSoloJoin = BoardMaterial;

                            TextMeshPro text = ui.screenText;
                            if (!textMeshPro.Contains(text))
                                textMeshPro.Add(text);
                        }
                        catch { }
                    }
                    PhotonNetworkController.Instance.UpdateTriggerScreens();

                    string[] objectsWithTMPro = {
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText",
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData",
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/Data",
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/FunctionSelect"
                        };
                    foreach (string objectName in objectsWithTMPro)
                    {
                        GameObject obj = GetObject(objectName);
                        if (obj != null)
                        {
                            TextMeshPro text = obj.GetComponent<TextMeshPro>();
                            if (!textMeshPro.Contains(text))
                                textMeshPro.Add(text);
                        }
                        else
                            LogManager.Log("Could not find " + objectName);
                    }

                    Transform forestTransform = GetObject("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard").transform;
                    for (int i = 0; i < forestTransform.transform.childCount; i++)
                    {
                        GameObject v = forestTransform.GetChild(i).gameObject;
                        if ((!v.name.Contains("Board Text") && !v.name.Contains("Scoreboard_OfflineText")) ||
                            !v.activeSelf) continue;
                        TextMeshPro text = v.GetComponent<TextMeshPro>();
                        if (!textMeshPro.Contains(text))
                            textMeshPro.Add(text);
                    }

                    hasFoundAllBoards = true;
                }
                catch (Exception exc)
                {
                    LogManager.LogError($"Error with board colors at {exc.StackTrace}: {exc.Message}");
                    hasFoundAllBoards = false;
                }
            }

            if (computerMonitor == null)
                computerMonitor = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");

            if (computerMonitor != null)
                computerMonitor.GetComponent<Renderer>().material = BoardMaterial;

            try
            {
                BoardMaterial.color = CustomBoardsEnabled ? backgroundColor.GetCurrentColor() : (Color)new Color32(0, 59, 4, 255);

                if (motdTitle == null)
                {
                    GameObject motdObject = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
                    motdTitle = Instantiate(motdObject, motdObject.transform.parent);
                    motdObject.SetActive(false);
                }

                TextMeshPro motdHeadingText = motdTitle.GetComponent<TextMeshPro>();
                if (!textMeshPro.Contains(motdHeadingText))
                    textMeshPro.Add(motdHeadingText);

                motdHeadingText.richText = true;
                motdHeadingText.SafeSetFontSize(100);
                motdHeadingText.SafeSetText($"Thanks for using {(doCustomName ? customMenuName : "ii's <b>Stupid</b> Menu")}!");
                motdHeadingText.SafeSetFontStyle(activeFontStyle);
                motdHeadingText.SafeSetFont(activeFont);
                FollowMenuSettings(motdHeadingText, -4f);

                if (doCustomName)
                    motdHeadingText.SafeSetText("Thanks for using " + NoRichtextTags(customMenuName) + "!");

                motdHeadingText.SafeSetText(FollowMenuSettings(motdHeadingText.text));

                motdHeadingText.color = textColors[0].GetCurrentColor();
                motdHeadingText.overflowMode = TextOverflowModes.Overflow;

                if (motdText == null)
                {
                    GameObject motdObject = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
                    motdText = Instantiate(motdObject, motdObject.transform.parent);
                    motdObject.SetActive(false);

                    motdText.GetComponent<PlayFabTitleDataTextDisplay>().enabled = false;
                }

                TextMeshPro motdBodyText = motdText.GetComponent<TextMeshPro>();
                if (!textMeshPro.Contains(motdBodyText))
                    textMeshPro.Add(motdBodyText);

                motdBodyText.richText = true;
                motdBodyText.SafeSetFontSize(100);
                motdBodyText.color = textColors[0].GetCurrentColor();
                motdBodyText.SafeSetFontStyle(activeFontStyle);
                motdBodyText.SafeSetFont(activeFont);
                FollowMenuSettings(motdBodyText, -4f);

                motdBodyText.SafeSetText(FollowMenuSettings(string.Format(motdTemplate, PluginInfo.Version, fullModAmount, PluginInfo.BetaBuild ? "Beta" : "Release", PluginInfo.BuildTimestamp )));
            }
            catch { }
            
            try
            {
                Color targetColor = textColors[0].GetCurrentColor();

                if (!CustomBoardsEnabled || !CustomBoardTextEnabled)
                    targetColor = Color.white;

                foreach (TextMeshPro txt in textMeshPro.Where(text => text.isActiveAndEnabled))
                {
                    txt.color = targetColor;

                    if (!CustomBoardFonts) continue;
                    archiveGorillaTagFont ??= txt.font;

                    if (!characterDistanceArchive.ContainsKey(txt))
                        characterDistanceArchive[txt] = txt.characterSpacing;

                    txt.characterSpacing = 0f;

                    txt.SafeSetFont(activeFont);
                    txt.SafeSetFontStyle(activeFontStyle);
                }
            }
            catch { }
        }
        #endregion

        #region Object Boards
        public readonly Dictionary<string, GameObject> objectBoards = new Dictionary<string, GameObject>();
        public List<GorillaNetworkJoinTrigger> triggers = new List<GorillaNetworkJoinTrigger>();
        public readonly List<TextMeshPro> textMeshPro = new List<TextMeshPro>();
        public GameObject computerMonitor;

        public void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!CustomBoardsEnabled) return;
            if (!BoardInformations.TryGetValue(scene.name, out var config)) return;

            CreateObjectBoard(scene.name, config.GameObjectPath, config.Position, config.Rotation, config.Scale);
        }

        public void CreateObjectBoard(string scene, string gameObject, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
        {
            try
            {
                if (objectBoards.TryGetValue(scene, out GameObject existingBoard))
                {
                    if (existingBoard != null)
                        Destroy(existingBoard);

                    objectBoards.Remove(scene);
                }

                GameObject board = GameObject.CreatePrimitive(PrimitiveType.Plane);
                board.transform.parent = GetObject(gameObject).transform;
                board.transform.localPosition = position ?? new Vector3(-22.1964f, -34.9f, 0.57f);
                board.transform.localRotation = Quaternion.Euler(rotation ?? new Vector3(270f, 0f, 0f));
                board.transform.localScale = scale ?? new Vector3(21.6f, 2.4f, 22f);

                Destroy(board.GetComponent<Collider>());
                board.GetComponent<Renderer>().material = BoardMaterial;

                objectBoards.Add(scene, board);
            }
            catch (Exception e)
            {
                LogManager.LogError($"Failed to create object board for scene {scene}: {e}");
            }
        }

        private readonly struct BoardInformation
        {
            public readonly string GameObjectPath;
            public readonly Vector3 Position;
            public readonly Vector3 Rotation;
            public readonly Vector3 Scale;

            public BoardInformation(string path, Vector3 pos, Vector3 rot, Vector3 scale)
            {
                GameObjectPath = path;
                Position = pos;
                Rotation = rot;
                Scale = scale;
            }
        }

        private static readonly Dictionary<string, BoardInformation> BoardInformations = new Dictionary<string, BoardInformation>
        {
            ["Canyon2"] = new BoardInformation(
                "Canyon/CanyonScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-24.5019f, -28.7746f, 0.1f),
                new Vector3(270f, 0f, 0f),
                new Vector3(21.5946f, 1f, 22.1782f)
            ),
            ["Skyjungle"] = new BoardInformation(
                "skyjungle/UI/Scoreboard/GorillaScoreBoard",
                new Vector3(-21.2764f, -32.1928f, 0f),
                new Vector3(270.2987f, 0.2f, 359.9f),
                new Vector3(21.6f, 0.1f, 20.4909f)
            ),
            ["Mountain"] = new BoardInformation(
                "Mountain/MountainScoreboardAnchor/GorillaScoreBoard",
                Vector3.zero,
                Vector3.zero,
                Vector3.one
            ),
            ["Metropolis"] = new BoardInformation(
                "MetroMain/ComputerArea/Scoreboard/GorillaScoreBoard",
                new Vector3(-25.1f, -31f, 0.1502f),
                new Vector3(270.1958f, 0.2086f, 0f),
                new Vector3(21f, 102.9727f, 21.4f)
            ),
            ["Bayou"] = new BoardInformation(
                "BayouMain/ComputerArea/GorillaScoreBoardPhysical",
                new Vector3(-28.3419f, -26.851f, 0.3f),
                new Vector3(270f, 0f, 0f),
                new Vector3(21.3636f, 38f, 21f)
            ),
            ["Beach"] = new BoardInformation(
                "BeachScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -33.7126f, 0.1f),
                new Vector3(270.056f, 0f, 0f),
                new Vector3(21.2f, 2f, 21.6f)
            ),
            ["Cave"] = new BoardInformation(
                "Cave_Main_Prefab/CrystalCaveScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -33.7126f, 0.1f),
                new Vector3(270.056f, 0f, 0f),
                new Vector3(21.2f, 2f, 21.6f)
            ),
            ["Rotating"] = new BoardInformation(
                "RotatingPermanentEntrance/UI (1)/RotatingScoreboard/RotatingScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -33.7126f, 0.1f),
                new Vector3(270.056f, 0f, 0f),
                new Vector3(21.2f, 2f, 21.6f)
            ),
            ["MonkeBlocks"] = new BoardInformation(
                "Environment Objects/MonkeBlocksRoomPersistent/AtticScoreBoard/AtticScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -24.5091f, 0.57f),
                new Vector3(270.1856f, 0.1f, 0f),
                new Vector3(21.6f, 1.2f, 20.8f)
            ),
            ["Basement"] = new BoardInformation(
                "Basement/BasementScoreboardAnchor/GorillaScoreBoard/",
                new Vector3(-22.1964f, -24.5091f, 0.57f),
                new Vector3(270.1856f, 0.1f, 0f),
                new Vector3(21.6f, 1.2f, 20.8f)
            ),
            ["City"] = new BoardInformation(
                "City_Pretty/CosmeticsScoreboardAnchor/GorillaScoreBoard",
                new Vector3(-22.1964f, -34.9f, 0.57f),
                new Vector3(270f, 0f, 0f),
                new Vector3(21.6f, 2.4f, 22f)
            )
        };
        #endregion
    }
}
