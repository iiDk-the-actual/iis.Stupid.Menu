using UnityEngine;

namespace iiMenu.Classes
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
            if (rigidbody.velocity.magnitude < 0.01f)
                Destroy(gameObject);
        }

        public Rigidbody rigidbody;
    }
}
