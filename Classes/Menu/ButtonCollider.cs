/*
 * ii's Stupid Menu  Classes/Menu/ButtonCollider.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes.Menu
{
    public class Button : MonoBehaviour
	{
		public string relatedText;

		public bool incremental;
		public bool positive;

		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && (collider == buttonCollider || collider == lKeyCollider || collider == rKeyCollider) && menu != null)
			{
                buttonCooldown = Time.time + 0.2f;
                PlayButtonSound(relatedText);

				if (incremental)
					ToggleIncremental(relatedText, positive);
				else
					Toggle(relatedText, true);
            }
		}
	}
}
