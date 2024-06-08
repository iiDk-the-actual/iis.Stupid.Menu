using GorillaLocomotion.Gameplay;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Canyons
    {
        private static float RopeDelay = 0f;
        public static void CanyonsRopeControl()
        {
            Vector2 joy = ControllerInputPoller.instance.rightControllerPrimary2DAxis;

            if ((Mathf.Abs(joy.x) > 0.3 || Mathf.Abs(joy.y) > 0.3) && Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.25f;
                foreach (GorillaRopeSwing rope in GameObject.FindObjectsOfType(typeof(GorillaRopeSwing)))
                {
                    RopeSwingManager.instance.SendSetVelocity_RPC(rope.ropeId, 1, new Vector3(joy.x * 50f, joy.y * 50f, 0f), true);
                    RPCProtection();
                }
            }
        }

        public static void FastRopes()
        {
            foreach (GorillaRopeSwingSettings settings in GameObject.FindObjectsOfType(typeof(GorillaRopeSwingSettings)))
            {
                if (settings.name.Contains("Default"))
                {
                    settings.inheritVelocityMultiplier = 4f;
                }
            }
        }

        public static void RegularRopes()
        {
            foreach (GorillaRopeSwingSettings settings in GameObject.FindObjectsOfType(typeof(GorillaRopeSwingSettings)))
            {
                if (settings.name.Contains("Default"))
                {
                    settings.inheritVelocityMultiplier = 0.9f;
                }
            }
        }
    }
}
