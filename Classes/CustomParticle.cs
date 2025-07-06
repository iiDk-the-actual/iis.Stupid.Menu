using GorillaExtensions;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
{
    public class CustomParticle : MonoBehaviour
	{
		public float spawnTime;
		public Renderer renderer;
		public Rigidbody rigidbody;
		public void Awake()
		{
			spawnTime = Time.time;

			if (gameObject.GetComponent<Collider>())
				Destroy(gameObject.GetComponent<Collider>());

			renderer = gameObject.GetComponent<Renderer>() ?? null;
			rigidbody = gameObject.GetOrAddComponent<Rigidbody>() ?? null;

			if (rigidbody != null)
			{
                rigidbody.velocity = RandomVector3(1f);
				rigidbody.useGravity = false;
            }
            
			Update();
		}

		public void Update()
		{
			if (renderer != null)
				renderer.material.color = GetBDColor(0f);

			if (Time.time > spawnTime + 1f)
			{
				Destroy(gameObject);
				return;
			}

			transform.localScale = Vector3.one * Mathf.Lerp(0.025f, 0f, Time.time - spawnTime);
		}
	}
}
