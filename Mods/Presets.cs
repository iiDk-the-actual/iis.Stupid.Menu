using ExitGames.Client.Photon;
using GorillaTag.CosmeticSystem;
using GorillaTag.GuidedRefs;
using iiMenu.Menu;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine;
using static iiMenu.Menu.Main;
using static Photon.Pun.UtilityScripts.TabViewManager;
using static Technie.PhysicsCreator.SphereUtils;
using static UnityEngine.Rendering.DebugUI;

namespace iiMenu.Mods
{
    internal class Presets
    {
        public static void LegitimatePreset()
        {
            string[] presetMods = new string[]
            {
                "Joystick Menu",
                "Thin Menu",
                "Force Enable Hands",
                "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>",
                "Disable Enabled GUI",
                "Disable Board Colors",
                "Disable Disconnect Button",
                "Disable Page Buttons",
                "Disable FPS Counter",
                "Fake Oculus Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>"
            };

            themeType = 29;
            pageButtonType = 1;
            fontCycle = -1;

            Settings.ChangeMenuTheme();
            Settings.ChangePageType();
            Settings.ChangeFontType();

            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }
            NotifiLib.ClearAllNotifications();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Legitimate preset enabled successfully.");
        }

        public static void GhostPreset()
        {
            string[] presetMods = new string[]
            {
                "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>",
                "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>",
                "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>",
                "Steam Long Arms",
                "Break Audio Gun",
                "No Finger Movement",
                "Platforms",
                "Thin Menu"
            };

            longarmCycle = 3;
            armlength = 1.5f;
            
            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }
            NotifiLib.ClearAllNotifications();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Ghost preset enabled successfully.");
        }

        public static void GoldentrophyPreset()
        {
            string[] presetMods = new string[]
            {
                "Outline Menu",
                "Thin Menu",
                "Freeze Player in Menu",
                "Clear Notifications on Disconnect",
                "Disable Enabled GUI",
                "Legacy Ghostview",
                "Finger Gun Projectiles",
                "Rainbow Projectiles",
                "Force Enable Hands",
                "Anti AFK",
                "Physical Quit Box",
                "Tag Lag Detector",
                "Spoof Support Page",
                "Hand Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>",
                "Uncap Max Velocity",
                "Weak Slide Control",
                "Clear Weather",
                "Info Watch",
                "Fix Rig Colors",
                "Cosmetic ESP",
                "Infection Tracers",
                "Infection Distance ESP",
                "Obnoxious Tag"
            };

            themeType = 33;
            pageButtonType = 1;
            fontCycle = -1;

            Settings.ChangeMenuTheme();
            Settings.ChangePageType();
            Settings.ChangeFontType();

            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }
            NotifiLib.ClearAllNotifications();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Goldentrophy preset enabled successfully.");
        }

        public static void PerformancePreset()
        {
            string[] presetMods = new string[]
            {
                "Thin Menu",
                "Disable Enabled GUI",
                "Disable Board Colors",
                "Disable FPS Counter",
                "FPS Boost",
                "Disable Ghostview"
            };

            themeType = 31;
            pageButtonType = 1;
            fontCycle = 0;

            Settings.ChangeMenuTheme();
            Settings.ChangePageType();
            Settings.ChangeFontType();

            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }

            NotifiLib.ClearAllNotifications();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Performance preset enabled successfully.");
        }

        public static void SafetyPreset()
        {
            string[] presetMods = new string[]
            {
                "Thin Menu",
                "No Finger Movement",
                "Fake Oculus Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>",
                "Disable Gamemode Buttons",
                "Anti Crash",
                "Anti Moderator",
                "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>",
                "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>"
            };

            themeType = 34;
            pageButtonType = 1;
            fontCycle = 0;

            Settings.ChangeMenuTheme();
            Settings.ChangePageType();
            Settings.ChangeFontType();

            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }

            NotifiLib.ClearAllNotifications();
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Safety preset enabled successfully.");
        }
    }
}
