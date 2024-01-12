using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Canyons
    {
        public static void CanyonsRopeControl()
        {
            Vector2 joy = ControllerInputPoller.instance.rightControllerPrimary2DAxis;

            if (Mathf.Abs(joy.x) > 0.3 || Mathf.Abs(joy.y) > 0.3)
            {
                foreach (GorillaRopeSwing rope in GameObject.FindObjectsOfType(typeof(GorillaRopeSwing)))
                {
                    rope.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                    {
                        1,
                        new Vector3(joy.x*50f, joy.y*50f, 0f),
                        true
                    });
                    RPCProtection();
                }
            }
        }
    }
}
