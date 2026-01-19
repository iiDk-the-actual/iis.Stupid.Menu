/*
 * ii's Stupid Menu  Utilities/AssetUtilities.cs
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static iiMenu.Utilities.FileUtilities;

namespace iiMenu.Utilities
{
    public class AssetUtilities
    {
        private static AssetBundle assetBundle;
        private static void LoadAssetBundle()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{PluginInfo.ClientResourcePath}.iimenu");
            if (stream != null)
                assetBundle = AssetBundle.LoadFromStream(stream);
            else
                LogManager.LogError("Failed to load assetbundle");
        }

        public static T LoadObject<T>(string assetName) where T : Object
        {
            if (assetBundle == null)
                LoadAssetBundle();

            T gameObject = Object.Instantiate(assetBundle.LoadAsset<T>(assetName));
            return gameObject;
        }

        public static T LoadAsset<T>(string assetName) where T : Object
        {
            if (assetBundle == null)
                LoadAssetBundle();

            T gameObject = assetBundle.LoadAsset(assetName) as T;
            return gameObject;
        }

        public static readonly Dictionary<string, AudioClip> audioFilePool = new Dictionary<string, AudioClip>();
        public static AudioClip LoadSoundFromFile(string fileName) // Thanks to ShibaGT for help with loading the audio from file
        {
            AudioClip sound;
            if (!audioFilePool.TryGetValue(fileName, out var value))
            {
                string filePath = $"{GetGamePath()}/{PluginInfo.BaseDirectory}/{fileName}";

                UnityWebRequest actualrequest = UnityWebRequestMultimedia.GetAudioClip($"file://{filePath}", GetAudioType(GetFileExtension(fileName)));
                UnityWebRequestAsyncOperation newvar = actualrequest.SendWebRequest();
                while (!newvar.isDone) { }

                AudioClip actualclip = DownloadHandlerAudioClip.GetContent(actualrequest);
                sound = Task.FromResult(actualclip).Result;

                audioFilePool.Add(fileName, sound);
            }
            else
                sound = value;

            return sound;
        }

        public static AudioClip LoadSoundFromURL(string resourcePath, string fileName)
        {
            string filePath = $"{PluginInfo.BaseDirectory}/{fileName}";
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(directory);

            if (File.Exists(filePath)) return LoadSoundFromFile(fileName);
            LogManager.Log("Downloading " + fileName);
            using WebClient stream = new WebClient();
            stream.DownloadFile(resourcePath, filePath);

            return LoadSoundFromFile(fileName);
        }

        public static readonly Dictionary<string, Texture2D> textureResourceDictionary = new Dictionary<string, Texture2D>();
        public static Texture2D LoadTextureFromResource(string resourcePath)
        {
            if (textureResourceDictionary.TryGetValue(resourcePath, out Texture2D existingTexture))
                return existingTexture;

            Texture2D texture = new Texture2D(2, 2);

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                byte[] fileData = new byte[stream.Length];
                // ReSharper disable once MustUseReturnValue
                stream.Read(fileData, 0, (int)stream.Length);
                texture.LoadImage(fileData);
            }
            else
                LogManager.LogError("Failed to load texture from resource: " + resourcePath);

            textureResourceDictionary[resourcePath] = texture;

            return texture;
        }

        public static readonly Dictionary<string, Texture2D> textureUrlDictionary = new Dictionary<string, Texture2D>();
        public static Texture2D LoadTextureFromURL(string resourcePath, string fileName)
        {
            if (textureUrlDictionary.TryGetValue(resourcePath, out Texture2D existingTexture))
                return existingTexture;

            string filePath = $"{PluginInfo.BaseDirectory}/{fileName}";
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(directory);

            if (!File.Exists(filePath))
            {
                LogManager.Log("Downloading " + fileName);
                WebClient stream = new WebClient();
                stream.DownloadFile(resourcePath, filePath);
            }

            Texture2D texture = LoadTextureFromFile(fileName);

            textureUrlDictionary[resourcePath] = texture;

            return texture;
        }

        public static readonly Dictionary<string, Texture2D> textureFileDirectory = new Dictionary<string, Texture2D>();
        public static Texture2D LoadTextureFromFile(string fileName)
        {
            if (textureFileDirectory.TryGetValue(fileName, out Texture2D existingTexture))
                return existingTexture;

            string filePath = $"{PluginInfo.BaseDirectory}/{fileName}";
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Texture2D texture = new Texture2D(2, 2);

            byte[] bytes = File.ReadAllBytes(filePath);
            texture.LoadImage(bytes);

            textureFileDirectory[fileName] = texture;

            return texture;
        }
    }
}