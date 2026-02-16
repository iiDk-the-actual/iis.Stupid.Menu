/*
 * ii's Stupid Menu  PluginInfo.cs
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

namespace iiMenu
{
    public class PluginInfo
    {
        public const string GUID = "org.iidk.gorillatag.iimenu";
        public const string Name = "ii's Stupid Menu";
        public const string Description = "Created by @crimsoncauldron with love <3";
        public const string BuildTimestamp = "2026-02-13T18:23:06Z";
        public const string Version = "8.2.4";

        public const string BaseDirectory = "iisStupidMenu";
        public const string ClientResourcePath = "iiMenu.Resources.Client";
        public const string ServerResourcePath = "https://raw.githubusercontent.com/iiDk-the-actual/iis.Stupid.Menu/master/Resources/Server";
        public const string ServerAPI = "https://iidk.online"; // Server now closed source due to bad actors :( For any questions, please make an issue on the GitHub repository.
        
        public const string Logo = @"••╹   ┏┓     • ┓  ┳┳┓      
┓┓ ┏  ┗┓╋┓┏┏┓┓┏┫  ┃┃┃┏┓┏┓┓┏
┗┗ ┛  ┗┛┗┗┻┣┛┗┗┻  ┛ ┗┗ ┛┗┗┻
           ┛";

#if DEBUG
        public static bool BetaBuild = true;
#else
        public static bool BetaBuild = false;
#endif
    }
}
