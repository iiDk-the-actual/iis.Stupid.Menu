using Photon.Pun;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
{
	internal class Button : MonoBehaviour
	{
		public string relatedText;
		
		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && (collider == buttonCollider || collider == lKeyCollider || collider == rKeyCollider) && menu != null)
			{
                buttonCooldown = Time.time + 0.2f;
				MakeButtonSound(relatedText);
				Toggle(relatedText, true);
            }
		}
	}
}
