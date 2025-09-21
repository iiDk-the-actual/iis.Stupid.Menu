/*
 * ii's Stupid Menu  Classes/Menu/TextColorChanger.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

using UnityEngine;
using UnityEngine.UI;

namespace iiMenu.Classes.Menu
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
