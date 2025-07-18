using HarmonyLib;
using iiMenu.Classes;
using System;
using System.Linq;
using System.Reflection;

namespace iiMenu.Patches
{
    public class PatchHandler
    {
        public static bool IsPatched { get; private set; }
        public static int PatchErrors { get; private set; }

        public static void PatchAll()
        {
            if (!IsPatched)
            {
                instance ??= new Harmony(PluginInfo.GUID);

                foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.IsClass && t.GetCustomAttribute<HarmonyPatch>() != null))
                {
                    try
                    {
                        instance.CreateClassProcessor(type).Patch();
                    }
                    catch (Exception ex)
                    {
                        PatchErrors++;
                        LogManager.LogError($"Failed to patch {type.FullName}: {ex}");
                    }
                }

                LogManager.Log($"Patched with {PatchErrors} errors");

                IsPatched = true;
            }
        }

        public static void UnpatchAll()
        {
            if (instance != null && IsPatched)
            {
                instance.UnpatchSelf();
                IsPatched = false;
                instance = null;
            }
        }

        public static void ApplyPatch(Type targetClass, string methodName, MethodInfo prefix = null, MethodInfo postfix = null, Type[] parameterTypes = null)
        {
            var original =
                parameterTypes == null ?
                targetClass.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) :
                targetClass.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, parameterTypes, null);

            if (original == null)
                throw new Exception($"Method '{methodName}' not found on {targetClass.FullName}");

            instance.Patch(original,
                prefix: prefix != null ? new HarmonyMethod(prefix) : null,
                postfix: postfix != null ? new HarmonyMethod(postfix) : null);
        }

        public static void RemovePatch(Type targetClass, string methodName, Type[] parameterTypes = null)
        {
            var original =
                parameterTypes == null ?
                targetClass.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) :
                targetClass.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, parameterTypes, null);
            if (original == null)
                throw new Exception($"Method '{methodName}' not found on {targetClass.FullName}");

            instance.Unpatch(original, HarmonyPatchType.All, instance.Id);
        }

        private static Harmony instance;
        public const string InstanceId = PluginInfo.GUID;
    }
}
