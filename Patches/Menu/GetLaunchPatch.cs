using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(Slingshot), "GetLaunchVelocity")]
    public class GetLaunchPatch
    {
        public static bool enabled;

        public static void Postfix(Slingshot __instance, ref Vector3 __result)
        {
            if (enabled)
            {
                List<NetPlayer> infected = InfectedList();
                List<VRRig> rigs = GorillaParent.instance.vrrigs
                    .Where(rig => !rig.isLocal)
                    .Where(rig => !infected.Contains(GetPlayerFromVRRig(rig)))
                    .ToList();


                Transform head = GorillaTagger.Instance.headCollider.transform;
                VRRig targetRig = rigs
                    .Where(rig => rig != null)
                    .Select(rig => new {
                        Rig = rig,
                        ToRig = (rig.transform.position - head.position).normalized,
                        Distance = Vector3.Distance(head.position, rig.transform.position)
                    })
                    .OrderBy(x => Vector3.Angle(head.forward, x.ToRig) + x.Distance * 0.1f)
                    .Select(x => x.Rig)
                    .FirstOrDefault();

                __result = CalcMinSpeed(__instance.centerOrigin.transform.position, targetRig);
            }
        }

        // ChatGPT used for math.. because I'm only 14 and haven't learned this yet and am not taking a class for a Gorilla Tag mod
        private static Vector3 CalcMinSpeed(Vector3 origin, VRRig targetRig)
        {
            Vector3 targetPos = targetRig.headMesh.transform.position;
            Vector3 targetVel = targetRig.LatestVelocity();

            Vector3 displacement = targetPos - origin;
            Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
            Vector3 velocityXZ = new Vector3(targetVel.x, 0, targetVel.z);

            float g = -Physics.gravity.y;

            float x = displacementXZ.magnitude;
            float roughSpeed = 20f;
            float time = x / roughSpeed;

            Vector3 futurePos = targetPos + targetVel * time;
            displacement = futurePos - origin;
            displacementXZ = new Vector3(displacement.x, 0, displacement.z);
            float y = displacement.y;
            x = displacementXZ.magnitude;

            float minSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
            float launchSpeed = minSpeed * 2.5f;

            return CalcVelocity(origin, displacement, launchSpeed);
        }

        private static Vector3 CalcVelocity(Vector3 origin, Vector3 displacement, float speed)
        {
            Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
            float x = displacementXZ.magnitude;
            float y = displacement.y;
            float g = -Physics.gravity.y;
            float v2 = speed * speed;

            float underSqrt = v2 * v2 - g * (g * x * x + 2 * y * v2);
            if (underSqrt <= 0f)
                return displacement.normalized * speed;

            float sqrt = Mathf.Sqrt(underSqrt);
            float angle = Mathf.Atan((v2 - sqrt) / (g * x));

            Vector3 dirXZ = displacementXZ.normalized;
            Vector3 result = dirXZ * Mathf.Cos(angle) * speed + Vector3.up * Mathf.Sin(angle) * speed;
            return result;
        }
    }
}
