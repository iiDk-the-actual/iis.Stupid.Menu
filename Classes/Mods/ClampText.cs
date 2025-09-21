/*
 * ii's Stupid Menu  Classes/Mods/ClampText.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using UnityEngine;
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
