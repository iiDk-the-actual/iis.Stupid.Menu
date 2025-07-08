using GorillaExtensions;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Classes
{
    public class CustomParticle : MonoBehaviour
	{
		public float spawnTime;
		public Renderer renderer;
		public Vector3 velocity;
		public void Awake()
		{
			spawnTime = Time.time;

			renderer = gameObject.GetComponent<Renderer>() ?? null;
			velocity = RandomVector3(1f);

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

			transform.position += velocity * Time.unscaledDeltaTime;
			transform.localScale = Vector3.one * Mathf.Lerp(0.025f, 0f, Time.time - spawnTime);
		}
	}
}
