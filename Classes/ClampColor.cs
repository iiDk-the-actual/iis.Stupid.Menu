using iiMenu.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace iiMenu.Classes
{
    public class ClampColor : MonoBehaviour
    {
        public void Start()
        {
            gameObjectRenderer = GetComponent<Renderer>();
            Update();
        }

        public void Update()
        {
            gameObjectRenderer.material.color = targetRenderer.material.color;
        }

        public Renderer gameObjectRenderer;
        public Renderer targetRenderer;
    }
}
