/*
 * ii's Stupid Menu  Classes/Menu/KeyboardCollider.cs
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
    public class KeyboardKey : MonoBehaviour
	{
		public string key;
		public static float delay;
		
		public void OnTriggerEnter(Collider collider)
		{
			if ((collider == lKeyCollider || collider == rKeyCollider) && menu != null && Time.time > delay)
			{
				delay = Time.time + 0.1f;

                if (doButtonsVibrate)
					GorillaTagger.Instance.StartVibration(collider == lKeyCollider, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
				
                VRRig.LocalRig.PlayHandTapLocal(66, collider == lKeyCollider, buttonClickVolume / 10f);
				PressKeyboardKey(key);
            }
		}
	}
}
