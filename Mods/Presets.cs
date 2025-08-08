using iiMenu.Classes;
using iiMenu.Notifications;
using System.IO;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Presets
    {
        public static void LegitimatePreset()
        {
            string[] presetMods = new string[]
            {
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
                "Change Arm Length"
            };

            Movement.longarmCycle = 2;
            
            Settings.Panic();
            foreach (string mod in presetMods)
                Toggle(mod);

            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Ghost preset enabled successfully.");
        }

        public static void SaveCustomPreset(int id)
        {
            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/SavedPresets"))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/SavedPresets");
            
            File.WriteAllText($"{PluginInfo.BaseDirectory}/SavedPresets/Preset_" + id.ToString() + ".txt", Settings.SavePreferencesToText());
        }

        public static void LoadCustomPreset(int id)
        {
            if (Directory.Exists($"{PluginInfo.BaseDirectory}/SavedPresets"))
            {
                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/SavedPresets/Preset_" + id.ToString() + ".txt");
                LogManager.Log(text);
                Settings.LoadPreferencesFromText(text);
            }
        }

        public static void GoldentrophyPreset()
        {
            string[] presetMods = new string[]
            {
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
            
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Goldentrophy preset enabled successfully.");
        }

        public static void PerformancePreset()
        {
            string[] presetMods = new string[]
            {
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

            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Performance preset enabled successfully.");
        }

        public static void SafetyPreset()
        {
            string[] presetMods = new string[]
            {
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

            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Safety preset enabled successfully.");
        }
        
        public static void SimplePreset()
        {
            string[] presetMods = new string[]
            {
                "Disable Enabled GUI",
                "Accept TOS",
                "Player Scale Menu",
                "High Quality Text",
                "PC Button Click"
            };

            pageButtonType = 2;
            
            foreach (string mod in presetMods)
                Toggle(mod);
            
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Simple preset enabled successfully.");
        }
    }
}
