/*
 * ii's Stupid Menu  Patches/Menu/ShaderFix.cs
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

﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GameObject), nameof(GameObject.CreatePrimitive))]
    public class ShaderFix
    {
        private static void Postfix(GameObject __result)
        {
            if (crystallizeMenu && CrystalMaterial != null)
                __result.GetComponent<Renderer>().material = CrystalMaterial;
            else if (transparentMenu)
            {
                Material material = __result.GetComponent<Renderer>().material;
                material.shader = Shader.Find(shinyMenu ? "Universal Render Pipeline/Lit" : "Universal Render Pipeline/Unlit");

                material.SetFloat("_Surface", 1);
                material.SetFloat("_Blend", 0);
                material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = (int)RenderQueue.Transparent;
            } else
                __result.GetComponent<Renderer>().material.shader = Shader.Find(shinyMenu ? "Universal Render Pipeline/Lit" : "GorillaTag/UberShader");
            
            __result.GetComponent<Renderer>().material.color = backgroundColor.GetColor(0);
        }
    }
}