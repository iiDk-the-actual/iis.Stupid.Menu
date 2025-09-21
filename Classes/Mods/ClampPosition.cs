/*
 * ii's Stupid Menu  Classes/Mods/ClampPosition.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using UnityEngine;

namespace iiMenu.Classes.Mods
{
    public class ClampPosition : MonoBehaviour
    {
        public void Start() =>
            Update();

        public void Update()
        {
            if (targetTransform == null || targetTransform.gameObject == null)
                Destroy(this);

            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        public Transform targetTransform;
    }
}
