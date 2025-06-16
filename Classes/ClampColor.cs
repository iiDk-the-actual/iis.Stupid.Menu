using UnityEngine;

namespace iiMenu.Classes
{
    public class ClampColor : MonoBehaviour
    {
        public void Start()
        {
            targetRenderer.gameObject.GetComponent<ColorChanger>()?.Start();

            gameObjectRenderer = GetComponent<Renderer>();
            Update();
        }

        public void Update() =>
            gameObjectRenderer.material.color = targetRenderer.material.color;

        public Renderer gameObjectRenderer;
        public Renderer targetRenderer;
    }
}
