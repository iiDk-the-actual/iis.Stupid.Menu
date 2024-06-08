using Photon.Pun;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
{
	internal class KeyboardKey : MonoBehaviour
	{
		public string key;
		
		public void OnTriggerEnter(Collider collider)
		{
			if ((collider == lKeyCollider || collider == rKeyCollider) && menu != null)
			{
				if (doButtonsVibrate)
				{
					GorillaTagger.Instance.StartVibration(collider == lKeyCollider, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
				}
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, collider == lKeyCollider, buttonClickVolume / 10f);
				PressKeyboardKey(key);
            }
		}
	}
}
