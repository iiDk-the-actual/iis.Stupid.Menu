using UnityEngine;

namespace iiMenu.Extensions
{
    public static class VectorExtensions
    {
        public static void Distance(this Vector3 point, Vector3 to) =>
            Vector3.Distance(point, to);

        public static long Pack(this Vector3 vec) =>
            BitPackUtils.PackWorldPosForNetwork(vec);
    }
}
