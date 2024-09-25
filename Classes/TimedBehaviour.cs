using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace iiMenu.Classes
{
    public class TimedBehaviour : MonoBehaviour
    {
        public virtual void Start()
        {
            startTime = Time.time;
        }

        public virtual void Update()
        {
            if (!complete)
            {
                progress = Mathf.Clamp((Time.time - startTime) / duration, 0f, 1f);
                if (Time.time - startTime > duration)
                {
                    if (loop)
                    {
                        OnLoop();
                    }
                    else
                    {
                        complete = true;
                    }
                }
            }
        }

        public virtual void OnLoop()
        {
            startTime = Time.time;
        }

        public bool complete = false;

        public bool loop = true;

        public float progress = 0f;

        protected bool paused = false;

        protected float startTime;

        protected float duration = 2f;
    }
}
