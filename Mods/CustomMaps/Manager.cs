using GorillaTagScripts.ModIO;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods.CustomMaps.Maps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.CustomMaps
{
    public class Manager
    {
        private static Dictionary<long, string> mapScriptArchives = new Dictionary<long, string>();
        public static void UpdateCustomMapsTab(long? overwriteId = null)
        {
            int category = GetCategory("Custom Maps");
            List<ButtonInfo> buttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Custom Maps", method = () => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." } };

            if (overwriteId != -1 && CustomMapLoader.IsModLoaded())
            {
                long mapID = overwriteId ?? CustomMapLoader.LoadedMapModId;
                if (!mapScriptArchives.ContainsKey(mapID))
                    mapScriptArchives.Add(mapID, CustomGameMode.LuaScript);

                CustomMap map = GetMapByID(mapID);
                if (map == null)
                    buttons.Add(new ButtonInfo { buttonText = "This map is not supported yet.", label = true });
                else
                    buttons.AddRange(map.Buttons);

                buttons.AddRange(new ButtonInfo[]
                {
                    new ButtonInfo { buttonText = "Crash Gun", method =() => CrashGun(), toolTip = "Crashes whoever your hand desires in the custom map." },
                    new ButtonInfo { buttonText = "Crash All", method =() => CrashAll(), isTogglable = false, toolTip = "Crashes everyone in the custom map." },
                    new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>", method =() => AntiReportCrash(), toolTip = "Crashes everyone who tries to report you." },

                    new ButtonInfo { buttonText = " ", label = true },
                    new ButtonInfo { buttonText = "Edit Custom Script", method =() => EditUserScript(), isTogglable = false, toolTip = "Opens your custom script for this map." },
                    new ButtonInfo { buttonText = "Delete Custom Script", method =() => DeleteUserScript(), isTogglable = false, toolTip = "Deletes your custom script for this map." },
                    new ButtonInfo { buttonText = "Run Custom Script", enableMethod =() => StartUserScript(), disableMethod =() => StopUserScript(), toolTip = "Runs your custom script for this map." }
                });
            }
            else
                buttons.Add(new ButtonInfo { buttonText = "You have not loaded a map.", label = true });

            Buttons.buttons[category] = buttons.ToArray();
        }

        public static void ModifyCustomScript(Dictionary<int, string> replacements)
        {
            string input = CustomGameMode.LuaScript;
            string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (KeyValuePair<int, string> kvp in replacements)
            {
                int lineIndex = kvp.Key;
                if (lineIndex >= 0 && lineIndex < lines.Length)
                {
                    LogManager.Log("Replacing " + lines[lineIndex] + " with " + kvp.Value);
                    lines[lineIndex] = kvp.Value;
                }

            }

            CustomGameMode.LuaScript = string.Join(Environment.NewLine, lines);

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();

            CustomMapManager.ReturnToVirtualStump();
        }

        public static void EditUserScript()
        {
            string DirectoryTarget = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau";
            if (!File.Exists(DirectoryTarget))
                File.WriteAllText(DirectoryTarget, mapScriptArchives[CustomMapManager.currentRoomMapModId]);
            Process.Start(DirectoryTarget);
        }

        public static void DeleteUserScript()
        {
            string DirectoryTarget = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau";
            if (File.Exists(DirectoryTarget))
                File.Delete(DirectoryTarget);
        }

        public static void StartUserScript()
        {
            string DirectoryTarget = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau").Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/CustomScripts/{CustomMapLoader.LoadedMapModId}.luau";
            if (File.Exists(DirectoryTarget))
                CustomGameMode.LuaScript = File.ReadAllText(DirectoryTarget);

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();
            CustomMapManager.ReturnToVirtualStump();
        }

        public static void StopUserScript()
        {
            CustomGameMode.LuaScript = mapScriptArchives[CustomMapManager.currentRoomMapModId];

            if (NetworkSystem.Instance.InRoom)
                LuauHud.Instance.RestartLuauScript();
            CustomMapManager.ReturnToVirtualStump();
        }

        public static void RevertCustomScript(int[] lines)
        {
            Dictionary<int, string> replacements = new Dictionary<int, string>();
            foreach (int line in lines)
                replacements.Add(line, mapScriptArchives[CustomMapManager.currentRoomMapModId].Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[line]);

            ModifyCustomScript(replacements);
        }

        public static void RevertCustomScript(int line) =>
            RevertCustomScript(new int[] { line });

        public static Dictionary<long, CustomMap> mapCache = new Dictionary<long, CustomMap>();
        public static CustomMap GetMapByID(long id)
        {
            if (!mapCache.TryGetValue(id, out var instance))
            {
                var mapTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CustomMap)));

                foreach (var type in mapTypes)
                {
                    CustomMap mapInstance = (CustomMap)Activator.CreateInstance(type);
                    if (mapInstance.MapID == id)
                    {
                        instance = mapInstance;
                        break;
                    }
                }

                mapCache[id] = instance;
            }

            return instance;
        }

        // I don't know who made this
        public static float crashDelay = 0f;
        public static void CrashPlayer(int ActorNumber)
        {
            Photon.Pun.PhotonNetwork.RaiseEvent(180, new object[] { "leaveGame", (double)ActorNumber, false, (double)ActorNumber }, new Photon.Realtime.RaiseEventOptions()
            {
                TargetActors = new int[]
                {
                    ActorNumber
                }
            }, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                UnityEngine.RaycastHit Ray = GunData.Ray;
                UnityEngine.GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null && UnityEngine.Time.time > crashDelay)
                {
                    NetPlayer Player = RigManager.GetPlayerFromVRRig(lockTarget);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = UnityEngine.Time.time + 0.1f;
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }
        public static void CrashAll()
        {
            if (UnityEngine.Time.time > crashDelay)
            {
                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    CrashPlayer(Player.ActorNumber);
                }
                crashDelay = UnityEngine.Time.time + 0.1f;
            }
        }
        public static void AntiReportCrash()
        {
            Safety.AntiReport((vrrig, position) =>
            {
                
                if (UnityEngine.Time.time > crashDelay)
                {
                    NetPlayer Player = RigManager.GetPlayerFromVRRig(vrrig);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = UnityEngine.Time.time + 0.5f;
                    Notifications.NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigManager.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been crashed.");
                }
            });
        }
    }
}
