/*
 * ii's Stupid Menu  Classes/Mods/VirtualStumpAd.cs
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

using GorillaExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes.Mods
{
    public class VirtualStumpAd : MonoBehaviour
    {
        public static VirtualStumpAd Instance { get; private set; }
        
        public static SpriteRenderer SpriteRenderer { get; private set; }
        
        private bool hasSetupFeaturedMapVideo;
        private VideoPlayer videoPlayer;

        public static GameObject LoadingText;
        public static GameObject MapInfoText;
        public static GameObject FeaturedMaps;
        public static GameObject DisplayTextObj;
        
        private Vector3 oldLocalScale = Vector3.zero;
        private string oldText = "";
        
        private SpriteRendererData cachedSpriteRendererData;


        private void Awake() => Instance = this;

        private void OnDisable()
        {
            hasSetupFeaturedMapVideo = false;
            TextMeshPro featuredMapText = MapInfoText.GetComponent<TextMeshPro>();
            featuredMapText.text = oldText;
            MapInfoText.SetActive(false);
            LoadingText.SetActive(true);
            
            foreach (Transform child in DisplayTextObj.transform)
                if (child.name.ToLower().EndsWith("tmp"))
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
            
            GameObject featuredMapImage = FeaturedMaps.transform.Find("FeaturedMapImage")?.gameObject;
            
            if (featuredMapImage == null)
                return;

            Destroy(featuredMapImage.GetOrAddComponent<MeshFilter>());
            Destroy(featuredMapImage.GetOrAddComponent<MeshRenderer>());

            featuredMapImage.transform.localScale                = oldLocalScale;
            Destroy(featuredMapImage.GetOrAddComponent<VideoPlayer>());
            
            ApplySpriteRenderer(featuredMapImage);
        }

        private void Update()
        {
            if (hasSetupFeaturedMapVideo && !videoPlayer.isPlaying && videoPlayer.enabled)
            {
                if (!videoPlayer.isLooping)
                    videoPlayer.isLooping = true;
                videoPlayer.Play();
            }
                

            if (hasSetupFeaturedMapVideo)
                return;

            LoadingText = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");
            MapInfoText = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");
            FeaturedMaps = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay");
            DisplayTextObj = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/DisplayText");

            if (DisplayTextObj != null)
                foreach (Transform child in DisplayTextObj.transform)
                    if (child.name.ToLower().EndsWith("tmp"))
                        child.gameObject.SetActive(!child.gameObject.activeSelf);

            if (MapInfoText == null || FeaturedMaps == null)
                return;

            try
            {
                TextMeshPro featuredMapText = MapInfoText.GetComponent<TextMeshPro>();
                if (featuredMapText != null)
                {
                    oldText              = featuredMapText.text;
                    featuredMapText.text = "<b><color=#FF8000>ii's Stupid Menu</color></b>";
                    MapInfoText.SetActive(true);
                }

                LoadingText?.SetActive(false);

                GameObject featuredMapImage = FeaturedMaps.transform.Find("FeaturedMapImage")?.gameObject;

                if (featuredMapImage == null)
                    return;

                CacheAndRemoveSpriteRenderer(featuredMapImage);

                MeshFilter mf = featuredMapImage.GetOrAddComponent<MeshFilter>();
                mf.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");

                MeshRenderer mr = featuredMapImage.GetOrAddComponent<MeshRenderer>();

                Material videoMat = new Material(Shader.Find("Unlit/Texture"));
                mr.material = videoMat;

                videoPlayer = featuredMapImage.GetOrAddComponent<VideoPlayer>();
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                videoPlayer.url = $"{PluginInfo.ServerResourcePath}/Videos/vstump-video.mp4";

                RenderTexture rt = new RenderTexture(512, 512, 0);
                videoPlayer.targetTexture = rt;
                mr.material.mainTexture = rt;

                if (oldLocalScale == Vector3.zero) oldLocalScale = featuredMapImage.transform.localScale;
                featuredMapImage.transform.localScale = new Vector3(0.845f, 0.445f, 1f);

                videoPlayer.isLooping = true;
                videoPlayer.Play();

                featuredMapImage.SetActive(true);

                hasSetupFeaturedMapVideo = true;
            }
            catch { }
        }
        
        private void CacheAndRemoveSpriteRenderer(GameObject target)
        {
            SpriteRenderer = target.GetComponent<SpriteRenderer>();
            if (SpriteRenderer == null)
                return;

            cachedSpriteRendererData = new SpriteRendererData
            {
                Sprite = SpriteRenderer.sprite,
                Material = SpriteRenderer.material,
                Color = SpriteRenderer.color,
                SortingLayerID = SpriteRenderer.sortingLayerID,
                SortingOrder = SpriteRenderer.sortingOrder,
                FlipX = SpriteRenderer.flipX,
                FlipY = SpriteRenderer.flipY,
                DrawMode = SpriteRenderer.drawMode,
                Size = SpriteRenderer.size
            };

            Destroy(SpriteRenderer);
        }
        
        private void ApplySpriteRenderer(GameObject target)
        {
            if (cachedSpriteRendererData == null)
                return;

            SpriteRenderer = target.AddComponent<SpriteRenderer>();

            SpriteRenderer.sprite = cachedSpriteRendererData.Sprite;
            SpriteRenderer.material = cachedSpriteRendererData.Material;
            SpriteRenderer.color = cachedSpriteRendererData.Color;
            SpriteRenderer.sortingLayerID = cachedSpriteRendererData.SortingLayerID;
            SpriteRenderer.sortingOrder = cachedSpriteRendererData.SortingOrder;
            SpriteRenderer.flipX = cachedSpriteRendererData.FlipX;
            SpriteRenderer.flipY = cachedSpriteRendererData.FlipY;
            SpriteRenderer.drawMode = cachedSpriteRendererData.DrawMode;
            SpriteRenderer.size = cachedSpriteRendererData.Size;
        }
    }
    
    public sealed class SpriteRendererData
    {
        public Sprite Sprite;
        public Material Material;
        public Color Color;
        public int SortingLayerID;
        public int SortingOrder;
        public bool FlipX;
        public bool FlipY;
        public SpriteDrawMode DrawMode;
        public Vector2 Size;
    }
}