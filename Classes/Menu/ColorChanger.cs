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
            targetRenderer.enabled = !colors.transparent;

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
                    if (targetRenderer.material.shader.name != "Universal Render Pipeline/Unlit")
                    {
                        targetRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                        {
                            mainTexture = Main.GetGradientTexture(colors.GetColor(0), colors.GetColor(1))
                        };
                    }
                }
            }
        }

        public Renderer targetRenderer;
        public ExtGradient colors;
    }
}
