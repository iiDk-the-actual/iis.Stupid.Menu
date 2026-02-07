/*
 * ii's Stupid Menu  Mods/Presets.cs
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

using iiMenu.Managers;
using System.IO;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public static class Presets
    {
        public static void LegitimatePreset()
        {
            string[] presetMods = {
                "Joystick Menu",
                "Thin Menu",
                "Disable Enabled GUI",
                "Disable Board Colors",
                "Disable Disconnect Button",
                "Disable Page Buttons",
                "Disable Search Button",
                "Disable Return Button",
                "Hidden on Camera",
                "Hide Notifications on Camera",
                "Hide Text on Camera",
                "Disable FPS Counter",
                "Fix Rig Colors",
            };

            themeType = 29;
            pageButtonType = 1;
            fontCycle = -1;

            Settings.ChangeMenuTheme();
            Settings.ChangePageType();
            Settings.ChangeFontType();

            Settings.Panic();
            foreach (string mod in presetMods)
                Toggle(mod);
            
            NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Legitimate preset enabled successfully.");
        }

        public static void GhostPreset()
        {
            string[] presetMods = {
                "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>",
                "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>",
                "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>",
                "Steam Long Arms",
                "Break Audio Gun",
                "No Finger Movement",
                "Platforms",
                "Change Arm Length"
            };

            Movement.longarmCycle = 2;
            
            Settings.Panic();
            foreach (string mod in presetMods)
                Toggle(mod);

            NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Ghost preset enabled successfully.");
        }

        public static void SaveCustomPreset(int id)
        {
            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/SavedPresets"))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/SavedPresets");
            
            File.WriteAllText($"{PluginInfo.BaseDirectory}/SavedPresets/Preset_" + id + ".txt", Settings.SavePreferencesToText());
        }

        public static void LoadCustomPreset(int id)
        {
            if (Directory.Exists($"{PluginInfo.BaseDirectory}/SavedPresets"))
            {
                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/SavedPresets/Preset_" + id + ".txt");
                LogManager.Log(text);
                Settings.LoadPreferencesFromText(text);
            }
        }

        public static void GoldentrophyPreset()
        {
            string[] presetMods = {
                "Inner Outline Menu",
                "Outline Menu",
                "Freeze Player in Menu",
                "Clear Notifications on Disconnect",
                "Disable Enabled GUI",
                "Legacy Ghostview",
                "Rainbow Projectiles",
                "Force Enable Hands",
                "Anti AFK",
                "Physical Quit Box",
                "Tag Lag Detector",
                "Day Time",
                "Clear Weather",
                "Info Watch",
                "Fix Rig Colors",
                "Cosmetic ESP",
                "Infection Tracers",
                "Infection Distance ESP",
                "Auto Party Kick",
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
                Toggle(mod);
            
            NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Goldentrophy preset enabled successfully.");
        }

        public static void PerformancePreset()
        {
            string[] presetMods = {
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
                Toggle(mod);

            NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Performance preset enabled successfully.");
        }

        public static void SafetyPreset()
        {
            string[] presetMods = {
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
                Toggle(mod);

            NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Safety preset enabled successfully.");
        }
        
        public static void SimplePreset()
        {
            string[] presetMods = {
                "Disable Enabled GUI",
                "Accept TOS",
                "Player Scale Menu",
                "PC Button Click"
            };

            pageButtonType = 2;
            
            foreach (string mod in presetMods)
                Toggle(mod);
            
            NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Simple preset enabled successfully.");
        }
    }
}
