/*
 * ii's Stupid Menu  Patches/Menu/ShaderFix.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
    public class ShaderFix
    {
        private static void Postfix(GameObject __result)
        {
            if (crystallizemenu && CrystalMaterial != null)
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