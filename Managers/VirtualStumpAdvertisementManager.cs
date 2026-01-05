using GorillaExtensions;
using HarmonyLib;
using iiMenu.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace iiMenu.Managers
{
    public class VirtualStumpAdvertisementManager : MonoBehaviour
    {
        private bool        hasSetupFeaturedMapVideo;
        private VideoPlayer videoPlayer;

        private void Update()
        {
            if (hasSetupFeaturedMapVideo && !videoPlayer.isPlaying)
                videoPlayer.Play();

            if (hasSetupFeaturedMapVideo)
                return;

            GameObject loadingText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");

            GameObject mapInfoText =
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");

            GameObject featuredMaps =
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/");

            GameObject displayTextObj =
                    GameObject.Find(
                            "Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/DisplayText");

            if (displayTextObj != null)
                foreach (Transform child in displayTextObj.transform)
                    if (child.name.ToLower().EndsWith("tmp"))
                            // Safely gets destroyed by new maps display and for some reason lets this work, idk why but yeah
                        child.gameObject.SetActive(!child.gameObject.activeSelf);

            if (mapInfoText == null || featuredMaps == null)
                return;

            try
            {
                TextMeshPro featuredMapText = mapInfoText.GetComponent<TextMeshPro>();
                if (featuredMapText != null)
                    featuredMapText.text = "<color=black>HAMBURBUR ON TOP!</color>"; // modify this to whatever you want!

                //Lazy fix
                if (loadingText != null)
                    Destroy(loadingText);

                GameObject featuredMapImage = featuredMaps.transform.Find("FeaturedMapImage")?.gameObject;

                if (featuredMapImage == null)
                    return;

                if (featuredMapImage.TryGetComponent(out SpriteRenderer spriteRenderer))
                    Destroy(spriteRenderer);

                MeshFilter mf = featuredMapImage.GetOrAddComponent<MeshFilter>();
                mf.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");

                MeshRenderer mr = featuredMapImage.GetOrAddComponent<MeshRenderer>();

                Material videoMat = new Material(Shader.Find("Unlit/Texture"));
                mr.material = videoMat;

                videoPlayer = featuredMapImage.AddComponent<VideoPlayer>();
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                videoPlayer.url = "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/hamburger.mp4"; // modify this too!

                RenderTexture rt = new RenderTexture(512, 512, 0);
                videoPlayer.targetTexture = rt;
                mr.material.mainTexture   = rt;

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
    }
    
    [HarmonyPatch(typeof(NewMapsDisplay), nameof(NewMapsDisplay.UpdateSlideshow))]
    public static class NewMapsDisplay_UpdateSlideshow_Patch
    {
        private static bool Prefix(NewMapsDisplay __instance)
        {
            if (__instance == null)
                return true;

            return __instance.mapImage != null && __instance.mapImage.gameObject != null;
        }
    }
}