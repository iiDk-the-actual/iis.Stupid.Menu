using iiMenu.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace iiMenu.Classes
{
    public class ColorChanger : MonoBehaviour
    {
        public void Start()
        {
            gameObjectRenderer = base.GetComponent<Renderer>();
            Update();
        }

        public void Update()
        {
            if (colors != null)
            {
                if (!Main.dynamicGradients)
                {
                    if (!isMonkeColors)
                    {
                        color = colors.Evaluate((Time.time / 2f) % 1);

                        if (isRainbow)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            color = Color.HSVToRGB(h, 1f, 1f);
                        }
                        if (isPastelRainbow)
                        {
                            float h = (Time.frameCount / 180f) % 1f;
                            color = Color.HSVToRGB(h, 0.3f, 1f);
                        }
                        if (isEpileptic)
                            color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                        gameObjectRenderer.material.color = color;
                    }
                    else
                    {
                        if (!Menu.Main.PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                            gameObjectRenderer.material.color = GorillaTagger.Instance.offlineVRRig.mainSkin.material.color;
                        else
                            gameObjectRenderer.material.color = new Color32(255, 111, 0, 255);
                    }
                }
                else
                {
                    if (colors.colorKeys[0].color == colors.colorKeys[1].color)
                    {
                        gameObjectRenderer.material.color = colors.colorKeys[0].color;
                    } else
                    {
                        if (gameObjectRenderer.material.shader.name != "Universal Render Pipeline/Lit")
                        {
                            gameObjectRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                            gameObjectRenderer.material.SetFloat("_Glossiness", 0f);
                            gameObjectRenderer.material.SetFloat("_Metallic", 0f);

                            gameObjectRenderer.material.mainTexture = Main.GetGradientTexture(colors.colorKeys[0].color, colors.colorKeys[1].color);
                        }
                    }
                }
            }
        }

        public Renderer gameObjectRenderer;
        public Gradient colors = null;
        public Color32 color;
        public bool isRainbow = false;
        public bool isPastelRainbow = false;
        public bool isEpileptic = false;
        public bool isMonkeColors = false;
    }
}
