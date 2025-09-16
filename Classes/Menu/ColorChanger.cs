using iiMenu.Menu;
using UnityEngine;

namespace iiMenu.Classes
{
    public class ColorChanger : MonoBehaviour
    {
        public void Start()
        {
            if (colors == null)
            {
                Destroy(this);
                return;
            }

            targetRenderer = GetComponent<Renderer>();

            if (colors.IsFlat())
            {
                Update();
                Destroy(this);
                return;
            }
            
            Update();
        }

        public void Update()
        {
            targetRenderer.enabled = overrideTransparency ?? !colors.transparent;

            if (colors.transparent)
                return;

            if (!Main.dynamicGradients)
                targetRenderer.material.color = colors.GetCurrentColor();
            else
            {
                if (colors.IsFlat())
                    targetRenderer.material.color = colors.GetColor(0);
                else
                {
                    if (targetRenderer.material.shader.name != "Universal Render Pipeline/Unlit" && targetRenderer.material.mainTexture == null)
                    {
                        targetRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                        {
                            mainTexture = Main.GetGradientTexture(colors.GetColor(0), colors.GetColor(1))
                        };
                    }
                }
            }

            if (Main.transparentMenu)
            {
                Color color = targetRenderer.material.color;
                color.a = 0.5f;
                targetRenderer.material.color = color;
            }
        }

        public Renderer targetRenderer;
        public ExtGradient colors;
        public bool? overrideTransparency;
    }
}
