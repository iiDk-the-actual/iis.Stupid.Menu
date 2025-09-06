using UnityEngine;
using UnityEngine.UI;

namespace iiMenu.Classes
{
    public class TextColorChanger : MonoBehaviour
    {
        public void Start()
        {
            if (colors == null)
            {
                Destroy(this);
                return;
            }

            targetText = gameObject.GetComponent<Text>();

            if (colors.IsFlat())
            {
                Update();
                Destroy(this);
                return;
            }

            Update();
        }

        public void Update() =>
            targetText.color = colors.GetCurrentColor();
            
        public Text targetText;
        public ExtGradient colors;
    }
}
