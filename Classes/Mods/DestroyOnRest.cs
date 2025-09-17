using UnityEngine;

namespace iiMenu.Classes.Mods
{
    public class DestroyOnRest : MonoBehaviour
    {
        public void Start()
        {
            rigidbody = gameObject.GetComponent<Rigidbody>();
            Update();
        }

        public void Update()
        {
            if (rigidbody.linearVelocity.magnitude < 0.01f)
                Destroy(gameObject);
        }

        public Rigidbody rigidbody;
    }
}
