using HarmonyLib;
using System;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "SerializeReadShared")]
    public class PlayerSerializePatch
    {
        public static event Action<VRRig> OnPlayerSerialize;

        public static void Postfix(VRRig __instance, InputStruct data) =>
            OnPlayerSerialize?.Invoke(__instance);
    }
}
