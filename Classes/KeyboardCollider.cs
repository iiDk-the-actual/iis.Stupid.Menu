using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
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
				
                RigManager.LocalRig.PlayHandTapLocal(66, collider == lKeyCollider, buttonClickVolume / 10f);
				PressKeyboardKey(key);
            }
		}
	}
}
