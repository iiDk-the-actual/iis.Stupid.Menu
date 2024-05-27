using UnityEngine;
using static iiMenu.Menu.Main;
using static UnityEngine.Object;

namespace iiMenu.Mods
{
    internal class Beach
    {
        public static void DisableWater()
        {
            GameObject water = GameObject.Find("Beach/B_WaterVolumes");
            Transform waterTransform = water.transform;
            for (int i = 0; i < waterTransform.childCount; i++)
            {
                GameObject v = waterTransform.GetChild(i).gameObject;
                v.layer = LayerMask.NameToLayer("TransparentFX");
            }
        }

        public static void SolidWater()
        {
            GameObject water = GameObject.Find("Beach/B_WaterVolumes");
            Transform waterTransform = water.transform;
            for (int i = 0; i < waterTransform.childCount; i++)
            {
                GameObject v = waterTransform.GetChild(i).gameObject;
                v.layer = LayerMask.NameToLayer("Default");
            }
        }

        public static void FixWater()
        {
            GameObject water = GameObject.Find("Beach/B_WaterVolumes");
            Transform waterTransform = water.transform;
            for (int i = 0; i < waterTransform.childCount; i++)
            {
                GameObject v = waterTransform.GetChild(i).gameObject;
                v.layer = LayerMask.NameToLayer("Water");
            }
        }

        public static void AirSwim()
        {
            if (airSwimPart == null)
            {
                airSwimPart = Instantiate<GameObject>(GameObject.Find("Environment Objects/LocalObjects_Prefab/ForestToBeach/ForestToBeach_Prefab_V4/CaveWaterVolume"));
                airSwimPart.transform.localScale = new Vector3(5f, 5f, 5f);
                airSwimPart.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                GorillaLocomotion.Player.Instance.audioManager.UnsetMixerSnapshot(0.1f);
                airSwimPart.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 2.5f, 0f);
            }
        }

        public static void DisableAirSwim()
        {
            if (airSwimPart != null)
            {
                UnityEngine.Object.Destroy(airSwimPart);
            }
        }

        public static void FastSwim()
        {
            if (GorillaLocomotion.Player.Instance.InWater)
            {
                GorillaLocomotion.Player.Instance.gameObject.GetComponent<Rigidbody>().velocity *= 1.069f;
            }
        }
    }
}
