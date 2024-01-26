using iiMenu.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Presets
    {
        public static void LegitimatePreset()
        {
            string[] presetMods = new string[]
            {
                "Casual Tracers",
                "Remove Leaves"
            };

            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }
        }

        public static void GhostPreset()
        {
            string[] presetMods = new string[]
            {
                "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>",
                "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>",
                "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>",
                "Steam Long Arms",
                "Break Audio Gun"
            };

            longarmCycle = 3;
            armlength = 1.5f;
            
            Settings.Panic();
            foreach (string mod in presetMods)
            {
                Main.Toggle(mod);
            }
        }

        public static void GoldentrophyPreset()
        {
            string[] presetMods = new string[]
            {
                "Thin Menu",
                "Platforms",
                "Disable Network Triggers",
                "Disable Quit Box",
                "Rainbow Projectiles",
                "Finger Gun Projectiles",
                "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>",
                "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>",
                "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>",
            };

            themeType = 5;
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
        }
    }
}
