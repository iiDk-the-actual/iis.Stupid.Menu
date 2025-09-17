using UnityEngine;

namespace iiMenu.Classes.Mods
{
    public class ClampPosition : MonoBehaviour
    {
        public void Start() =>
            Update();

        public void Update()
        {
            if (targetTransform == null || targetTransform.gameObject == null)
                Destroy(this);

            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        public Transform targetTransform;
    }
}
