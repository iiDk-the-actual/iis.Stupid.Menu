using UnityEngine;

namespace iiMenu.Classes
{
    public class ClampColor : MonoBehaviour
    {
        public void Start()
        {
            if (targetRenderer.GetComponent<ColorChanger>() != null)
                targetRenderer.GetComponent<ColorChanger>().Update();

            gameObjectRenderer = GetComponent<Renderer>();
            Update();
        }

        public void Update() =>
            gameObjectRenderer.material.color = targetRenderer.material.color;

        public Renderer gameObjectRenderer;
        public Renderer targetRenderer;
    }
}
