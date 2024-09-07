using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace iiMenu.Classes
{
    public class ColorChanger : TimedBehaviour
    {
        public override void Start()
        {
            base.Start();
            gameObjectRenderer = base.GetComponent<Renderer>();
            Update();
        }

        public override void Update()
        {
            base.Update();
            if (colors != null)
            {
                if (!isMonkeColors)
                {
                    if (timeBased)
                    {
                        //color = colors.Evaluate(progress);
                        color = colors.Evaluate((Time.time / 2f) % 1);
                    }
                    if (isRainbow)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    }
                    if (isPastelRainbow)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        color = UnityEngine.Color.HSVToRGB(h, 0.3f, 1f);
                    }
                    if (isEpileptic)
                    {
                        color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255);
                    }
                    gameObjectRenderer.material.color = color;
                }
                else
                {
                    if (!Menu.Main.PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                    {
                        gameObjectRenderer.material.color = GorillaTagger.Instance.offlineVRRig.mainSkin.material.color;
                    } else
                    {
                        gameObjectRenderer.material.color = new Color32(255, 111, 0, 255);
                    }
                    
                }
            }
        }

        public Renderer gameObjectRenderer;
        public Gradient colors = null;
        public Color32 color;
        public bool timeBased = true;
        public bool isRainbow = false;
        public bool isPastelRainbow = false;
        public bool isEpileptic = false;
        public bool isMonkeColors = false;
    }
}
