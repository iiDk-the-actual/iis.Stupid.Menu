using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GameEntityManager), "TryGrabLocal")]
    public class EntityGrabPatch
    {
        public static bool enabled;

        public static bool Prefix(GameEntityManager __instance, Vector3 handPosition, ref GameEntityId __result)
        {
            if (enabled)
            {
                List<GameEntity> entities = __instance.entities;

                GameEntityId gameEntityId = GameEntityId.Invalid;
                float closestDist = float.MaxValue;
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i] != null)
                    {
                        double reach = 5;

                        float distance = (handPosition - entities[i].transform.position).sqrMagnitude;
                        if ((double)distance < reach && distance < closestDist)
                        {
                            gameEntityId = entities[i].id;
                            closestDist = distance;
                        }
                    }
                }

                __result = gameEntityId;

                return false;
            }
            
            return true;
        }
    }
}
