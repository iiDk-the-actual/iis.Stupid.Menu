/*
 * ii's Stupid Menu  Classes/Mods/CustomParticle.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

using GorillaLocomotion;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes.Mods
{
    public class CustomParticle : MonoBehaviour
	{
		public float spawnTime;
        public float startScale;

        public Renderer renderer;
		public Vector3 velocity;

		public void Awake()
		{
			spawnTime = Time.time;

			startScale = transform.localScale.x;

            renderer = gameObject.GetComponent<Renderer>() ?? null;
			velocity = RandomVector3(scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            Update();
		}

		public void Update()
		{
			if (renderer != null)
				renderer.material.color = buttonColors[1].GetCurrentColor();

			if (Time.time > spawnTime + 1f)
			{
				Destroy(gameObject);
				return;
			}

			transform.position += velocity * Time.unscaledDeltaTime;
			transform.localScale = Vector3.one * Mathf.Lerp(startScale, 0f, Time.time - spawnTime);
		}
	}
}
