/*
 * ii's Stupid Menu  Classes/Mods/DestroyOnRest.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using UnityEngine;

namespace iiMenu.Classes.Mods
{
    public class DestroyOnRest : MonoBehaviour
    {
        public void Start()
        {
            rigidbody = gameObject.GetComponent<Rigidbody>();
            Update();
        }

        public void Update()
        {
            if (rigidbody.linearVelocity.magnitude < 0.01f)
                Destroy(gameObject);
        }

        public Rigidbody rigidbody;
    }
}
