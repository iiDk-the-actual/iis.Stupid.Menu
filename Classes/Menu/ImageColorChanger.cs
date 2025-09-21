/*
 * ii's Stupid Menu  Classes/Menu/ImageColorChanger.cs
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
