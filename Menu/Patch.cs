using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    public class Menu : MonoBehaviour
    {
        public static bool IsPatched { get; private set; }
        //public static GameObject pointer { get; internal set; }

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = new Harmony(PluginInfo.GUID);
                }
                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                IsPatched = false;
            }
        }

        private static Harmony instance;
        public const string InstanceId = PluginInfo.GUID;
    }
}
