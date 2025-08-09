using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
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
