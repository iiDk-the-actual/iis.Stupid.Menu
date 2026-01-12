/*
 * ii's Stupid Menu  Mods/CustomMaps/SceneMapRegistry.cs
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


using System.Collections.Generic;

namespace iiMenu.Mods.CustomMaps
{
    public static class SceneMapRegistry
    {
        public static readonly Dictionary<string, SceneMap> sceneMapLookup = new Dictionary<string, SceneMap>();

        public static void RegisterMap(long mapID, string sceneName)
        {
            if (!sceneMapLookup.ContainsKey(sceneName))
                sceneMapLookup.Add(sceneName, new SceneMap(mapID, sceneName));
        }

        public static SceneMap GetMapForScene(string sceneName)
        {
            sceneMapLookup.TryGetValue(sceneName, out var map);
            return map;
        }

        public static void FillRegistry()
        {
            RegisterMap(5107228, "monke-magic-halloween-alt"); 
            RegisterMap(5135423, "Guns");          
            RegisterMap(5024157, "Flight-Simulator");          
            RegisterMap(4977315, "MiningSimulator");          
        }
    }
}
