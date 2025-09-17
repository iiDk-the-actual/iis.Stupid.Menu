using iiMenu.Classes.Menu;
using UnityEngine;

namespace iiMenu.Classes.Mods
{
    public class ClampColor : MonoBehaviour
    {
        public void Start()
        {
            targetRenderer.gameObject.GetComponent<ColorChanger>()?.Start();

            gameObjectRenderer = GetComponent<Renderer>();
            Update();
        }

        public void Update()
        {
            if (gameObjectRenderer.material.shader != targetRenderer.material.shader)
                gameObjectRenderer.material = new Material(targetRenderer.material.shader);

            if (targetRenderer.material.mainTexture != null && gameObjectRenderer.material.mainTexture != targetRenderer.material.mainTexture)
                gameObjectRenderer.material.mainTexture = targetRenderer.material.mainTexture;

            gameObjectRenderer.material.color = targetRenderer.material.color;
        }

        public Renderer gameObjectRenderer;
        public Renderer targetRenderer;
    }
}
