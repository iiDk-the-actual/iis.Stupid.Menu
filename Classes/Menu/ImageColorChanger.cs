using iiMenu.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace iiMenu.Classes
{
    public class ImageColorChanger : MonoBehaviour
    {
        public void Start()
        {
            if (colors == null)
            {
                Destroy(this);
                return;
            }

            targetImage = gameObject.GetComponent<Image>();

            if (colors.IsFlat())
            {
                Update();
                Destroy(this);
                return;
            }

            Update();
        }

        public void Update() =>
            targetImage.color = colors.GetCurrentColor();

        public Image targetImage;
        public ExtGradient colors;
    }
}
