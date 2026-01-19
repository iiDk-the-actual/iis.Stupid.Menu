/*
 * ii's Stupid Menu  Classes/Menu/ColorChanger.cs
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

using GorillaExtensions;
using iiMenu.Menu;
using UnityEngine;

namespace iiMenu.Classes.Menu
{
    public class ColorChanger : MonoBehaviour
    {
        public void Start()
        {
            if (colors == null)
            {
                Destroy(this);
                return;
            }

            targetRenderer = GetComponent<Renderer>();

            if (colors.IsFlat())
            {
                Update();
                Destroy(this);
                return;
            }
            
            Update();
        }

        public void Update()
        {
            targetRenderer.enabled = overrideTransparency ?? !colors.transparent;

            if (colors.transparent)
                return;

            if (!Main.dynamicGradients)
                targetRenderer.material.color = colors.GetCurrentColor();
            else
            {
                if (colors.IsFlat())
                    targetRenderer.material.color = colors.GetColor(0);
                else
                {
                    if (targetRenderer.material.shader.name != "Universal Render Pipeline/Unlit" && targetRenderer.material.mainTexture == null)
                    {
                        targetRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                        {
                            mainTexture = Main.GetGradientTexture(colors.GetColor(0), colors.GetColor(1))
                        };

                        if (Main.scrollingGradients)
                            gameObject.GetOrAddComponent<ScrollMaterial>();
                    }
                }
            }

            if (!Main.transparentMenu) return;
            Color color = targetRenderer.material.color;
            color.a = 0.5f;
            targetRenderer.material.color = color;
        }

        public Renderer targetRenderer;
        public ExtGradient colors;
        public bool? overrideTransparency;
    }
}
