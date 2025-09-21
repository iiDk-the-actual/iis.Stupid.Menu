using UnityEngine;
using UnityEngine.UI;

namespace iiMenu.Classes.Mods
{
    public class ClampText : MonoBehaviour
    {
        public void Start()
        {
            currentText = GetComponent<Text>();
            LateUpdate();
        }

        public void LateUpdate() =>
            currentText.text = targetText.text;

        public Text currentText;
        public Text targetText;
    }
}
