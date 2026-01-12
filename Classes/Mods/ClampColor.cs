/*
 * ii's Stupid Menu  Classes/Mods/ClampColor.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

﻿using iiMenu.Classes.Menu;
using UnityEngine;

namespace iiMenu.Classes.Mods
{
    public class ClampColor : MonoBehaviour
    {
        public void Start()
        {
            targetRenderer.gameObject.GetComponent<ColorChanger>()?.Start();

            gameObjectRenderer = GetComponent<Renderer>();
            Update();
        }

        public void Update()
        {
            if (gameObjectRenderer.material.shader != targetRenderer.material.shader)
                gameObjectRenderer.material = new Material(targetRenderer.material.shader);

            if (targetRenderer.material.mainTexture != null && gameObjectRenderer.material.mainTexture != targetRenderer.material.mainTexture)
                gameObjectRenderer.material.mainTexture = targetRenderer.material.mainTexture;

            gameObjectRenderer.material.color = targetRenderer.material.color;
        }

        public Renderer gameObjectRenderer;
        public Renderer targetRenderer;
    }
}
