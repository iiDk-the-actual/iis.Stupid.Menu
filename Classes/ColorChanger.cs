using iiMenu.Menu;
using UnityEngine;

namespace iiMenu.Classes
{
    public class ColorChanger : MonoBehaviour
    {
        public void Start()
        {
            gameObjectRenderer = GetComponent<Renderer>();
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
                            color = Main.RandomColor();
                        gameObjectRenderer.material.color = color;
                    }
                    else
                        gameObjectRenderer.material.color = Main.GetPlayerColor(VRRig.LocalRig);
                }
                else
                {
                    if (colors.colorKeys[0].color == colors.colorKeys[1].color)
                        gameObjectRenderer.material.color = colors.colorKeys[0].color;
                    else
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
