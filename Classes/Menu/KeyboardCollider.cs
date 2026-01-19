/*
 * ii's Stupid Menu  Classes/Menu/KeyboardCollider.cs
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

using System.Collections.Generic;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes.Menu
{
    public class KeyboardKey : MonoBehaviour
	{
		public static readonly Dictionary<string, KeyboardKey> keyLookupDictionary = new Dictionary<string, KeyboardKey>();
		public string key;
		public static float delay;

		public void Start() =>
            keyLookupDictionary[gameObject.name] = this;
		
		public void OnTriggerEnter(Collider collider)
		{
			if ((collider != lKeyCollider && collider != rKeyCollider) || menu == null || !(Time.time > delay)) return;
			if (!iiMenu.Menu.Buttons.GetIndex("Disable Keyboard Delay").enabled)
				delay = Time.time + 0.1f;

			if (doButtonsVibrate)
				GorillaTagger.Instance.StartVibration(collider == lKeyCollider, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
				
			VRRig.LocalRig.PlayHandTapLocal(66, collider == lKeyCollider, buttonClickVolume / 10f);
			PressKeyboardKey(key);
		}
	}
}
