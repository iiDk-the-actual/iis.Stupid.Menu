
using UnityEngine;

namespace iiMenu.Utilities
{
    public class RandomUtilities
    {
        public static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(Random.Range(-range, range),
                        Random.Range(-range, range),
                        Random.Range(-range, range));

        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(Random.Range(0f, range),
                        Random.Range(0f, range),
                        Random.Range(0f, range));

        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
            new Color32((byte)Random.Range(0, range),
                        (byte)Random.Range(0, range),
                        (byte)Random.Range(0, range),
                        alpha);

        public static string RandomString(int length = 4)
        {
            string random = "";
            for (int i = 0; i < length; i++)
            {
                int rand = Random.Range(0, 36);
                char c = rand < 26
                    ? (char)('A' + rand)
                    : (char)('0' + (rand - 26));
                random += c;
            }

            return random;
        }
    }
}
