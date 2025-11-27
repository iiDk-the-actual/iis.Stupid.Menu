/*
 * ii's Stupid Menu  Utilities/FileUtilities.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

using System;
using System.IO;
using UnityEngine;

namespace iiMenu.Utilities
{
    public class FileUtilities
    {
        public static string GetFileExtension(string fileName) =>
            fileName.ToLower().Split(".")[fileName.Split(".").Length - 1];

        public static string RemoveLastDirectory(string directory) =>
            directory == "" || directory.LastIndexOf('/') <= 0 ? "" : directory[..directory.LastIndexOf('/')];

        public static string RemoveFileExtension(string file)
        {
            int index = 0;
            string output = "";
            string[] split = file.Split(".");
            foreach (string data in split)
            {
                index++;
                if (index != split.Length)
                {
                    if (index > 1)
                        output += ".";

                    output += data;
                }
            }
            return output;
        }

        public static AudioType GetAudioType(string extension)
        {
            return extension.ToLower() switch
            {
                "mp3" => AudioType.MPEG,
                "wav" => AudioType.WAV,
                "ogg" => AudioType.OGGVORBIS,
                "aiff" => AudioType.AIFF,
                _ => AudioType.WAV,
            };
        }

        public static string GetFullPath(Transform transform)
        {
            string path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                if (path == "")
                    path = transform.name;
                else
                    path = transform.name + "/" + path;
            }
            return path;
        }

        public static string SanitizeFileName(string input)
        {
            input = input.Trim();
            char[] illegalChars = Path.GetInvalidFileNameChars();
            foreach (char c in illegalChars)
                input = input.Replace(c, '_');

            input = input.Replace("../", "")
                         .Replace("..\\", "")
                         .Replace("./", "")
                         .Replace(".\\", "");

            input = input.Replace(":", "")
                         .Replace("\\", "")
                         .Replace("/", "");

            if (input.Length > 64)
                input = input.Substring(0, 64);

            if (string.IsNullOrWhiteSpace(input))
                input = "file"; // fallback

            return input;
        }
    }
}