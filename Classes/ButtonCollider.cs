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
			if (Time.time > buttonCooldown && collider == buttonCollider && menu != null)
			{
                buttonCooldown = Time.time + 0.2f;
                GorillaTagger.Instance.StartVibration(GetIndex("Right Hand").enabled, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, GetIndex("Right Hand").enabled, 0.4f);
				if (GetIndex("Serversided Button Sounds").enabled && PhotonNetwork.InRoom)
				{
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]{
						66,
                        GetIndex("Right Hand").enabled,
						0.4f
					});
                    RPCProtection();
                }
				Toggle(this.relatedText, true);
            }
		}
	}
}
