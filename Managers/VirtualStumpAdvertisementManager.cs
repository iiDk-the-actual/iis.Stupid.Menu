using GorillaExtensions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using static iiMenu.Menu.Main;

namespace iiMenu.Managers
{
    public class VirtualStumpAdvertisementManager : MonoBehaviour
    {
        public static VirtualStumpAdvertisementManager Instance { get; private set; }
        
        public static SpriteRenderer SpriteRenderer { get; private set; }
        
        private bool        hasSetupFeaturedMapVideo;
        private VideoPlayer videoPlayer;

        public static GameObject LoadingText;
        public static GameObject MapInfoText;
        public static GameObject FeaturedMaps;
        public static GameObject DisplayTextObj;
        
        private Vector3  oldLocalScale = Vector3.zero;
        private string   oldText = "";
        
        private SpriteRendererData cachedSpriteRendererData;


        private void Awake() => Instance = this;

        private void OnDisable()
        {
            hasSetupFeaturedMapVideo = false;
            TextMeshPro featuredMapText = MapInfoText.GetComponent<TextMeshPro>();
            featuredMapText.text = oldText;
            
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
            
            LoadingText.SetActive(true);
            
            ApplySpriteRenderer(featuredMapImage);
        }

        private void Update()
        {
            if (hasSetupFeaturedMapVideo && !videoPlayer.isPlaying && videoPlayer.enabled)
                videoPlayer.Play();

            if (hasSetupFeaturedMapVideo)
                return;

            LoadingText = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");
            MapInfoText = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");
            FeaturedMaps = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/");
            DisplayTextObj = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/DisplayText");

            if (DisplayTextObj != null)
                foreach (Transform child in DisplayTextObj.transform)
                    if (child.name.ToLower().EndsWith("tmp"))
                            // Safely gets destroyed by new maps display and for some reason lets this work, idk why but yeah
                        child.gameObject.SetActive(!child.gameObject.activeSelf);

            if (MapInfoText == null || FeaturedMaps == null)
                return;

            try
            {
                TextMeshPro featuredMapText = MapInfoText.GetComponent<TextMeshPro>();
                if (featuredMapText != null)
                {
                    if (string.IsNullOrEmpty(oldText))
                        oldText = featuredMapText.text;
                    
                    featuredMapText.text = "<b><color=#FF8000>ii's Stupid Menu</color></b>";
                }

                //Lazy fix
                if (LoadingText != null)
                    LoadingText.SetActive(false);

                GameObject featuredMapImage = FeaturedMaps.transform.Find("FeaturedMapImage")?.gameObject;

                if (featuredMapImage == null)
                    return;

                CacheAndRemoveSpriteRenderer(featuredMapImage);

                MeshFilter mf                = featuredMapImage.GetOrAddComponent<MeshFilter>();
                mf.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");

                MeshRenderer mr = featuredMapImage.GetOrAddComponent<MeshRenderer>();

                Material videoMat                    = new Material(Shader.Find("Unlit/Texture"));
                mr.material = videoMat;

                videoPlayer = featuredMapImage.GetOrAddComponent<VideoPlayer>();
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                videoPlayer.url = "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/hamburger.mp4"; // modify this

                RenderTexture rt = new RenderTexture(512, 512, 0);
                videoPlayer.targetTexture = rt;
                mr.material.mainTexture   = rt;

                if (oldLocalScale == Vector3.zero) oldLocalScale = featuredMapImage.transform.localScale;
                featuredMapImage.transform.localScale = new Vector3(0.845f, 0.445f, 1f);

                videoPlayer.isLooping = true;
                videoPlayer.Play();

                featuredMapImage.SetActive(true);

                hasSetupFeaturedMapVideo = true;
            }
            catch
            {
                //fine it threw ONE null reference exception without the try block
            }
        }
        
        private void CacheAndRemoveSpriteRenderer(GameObject target)
        {
            SpriteRenderer = target.GetComponent<SpriteRenderer>();
            if (SpriteRenderer == null)
                return;

            cachedSpriteRendererData = new SpriteRendererData
            {
                    Sprite         = SpriteRenderer.sprite,
                    Material       = SpriteRenderer.material,
                    Color          = SpriteRenderer.color,
                    SortingLayerID = SpriteRenderer.sortingLayerID,
                    SortingOrder   = SpriteRenderer.sortingOrder,
                    FlipX          = SpriteRenderer.flipX,
                    FlipY          = SpriteRenderer.flipY,
                    DrawMode       = SpriteRenderer.drawMode,
                    Size           = SpriteRenderer.size
            };

            Destroy(SpriteRenderer);
        }
        
        private void ApplySpriteRenderer(GameObject target)
        {
            if (cachedSpriteRendererData == null)
                return;

            SpriteRenderer = target.AddComponent<SpriteRenderer>();

            SpriteRenderer.sprite         = cachedSpriteRendererData.Sprite;
            SpriteRenderer.material       = cachedSpriteRendererData.Material;
            SpriteRenderer.color          = cachedSpriteRendererData.Color;
            SpriteRenderer.sortingLayerID = cachedSpriteRendererData.SortingLayerID;
            SpriteRenderer.sortingOrder   = cachedSpriteRendererData.SortingOrder;
            SpriteRenderer.flipX          = cachedSpriteRendererData.FlipX;
            SpriteRenderer.flipY          = cachedSpriteRendererData.FlipY;
            SpriteRenderer.drawMode       = cachedSpriteRendererData.DrawMode;
            SpriteRenderer.size           = cachedSpriteRendererData.Size;
        }
    }
    
    public sealed class SpriteRendererData
    {
        public Sprite         Sprite;
        public Material       Material;
        public Color          Color;
        public int            SortingLayerID;
        public int            SortingOrder;
        public bool           FlipX;
        public bool           FlipY;
        public SpriteDrawMode DrawMode;
        public Vector2        Size;
    }

    
    [HarmonyPatch(typeof(NewMapsDisplay), nameof(NewMapsDisplay.UpdateSlideshow))]
    public static class NewMapsDisplay_UpdateSlideshow_Patch
    {
        private static bool Prefix(NewMapsDisplay __instance)
        {
            if (__instance == null || !VirtualStumpAdvertisementManager.Instance.enabled)
            {
                __instance.mapImage   = VirtualStumpAdvertisementManager.SpriteRenderer;
                __instance.mapInfoTMP = VirtualStumpAdvertisementManager.MapInfoText.GetComponent<TextMeshPro>();
                return true;
            }
            
            return __instance.mapImage != null && __instance.mapImage.gameObject != null;
        }
    }
}