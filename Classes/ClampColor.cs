using hykmMenu.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace hykmMenu.Classes
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
