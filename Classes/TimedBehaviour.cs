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
            this.startTime = Time.time;
        }

        public virtual void Update()
        {
            if (!this.complete)
            {
                this.progress = Mathf.Clamp((Time.time - this.startTime) / this.duration, 0f, 1f);
                if (Time.time - this.startTime > this.duration)
                {
                    if (this.loop)
                    {
                        this.OnLoop();
                    }
                    else
                    {
                        this.complete = true;
                    }
                }
            }
        }

        public virtual void OnLoop()
        {
            this.startTime = Time.time;
        }

        public bool complete = false;

        public bool loop = true;

        public float progress = 0f;

        protected bool paused = false;

        protected float startTime;

        protected float duration = 2f;
    }
}
