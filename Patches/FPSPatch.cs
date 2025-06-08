using HarmonyLib;
using UnityEngine.XR.Interaction.Toolkit;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "PackCompetitiveData")]
    public class FPSPatch
    {
        public static bool enabled;
        public static int spoofFPSValue;

        public static void Postfix(ref short __result)
        {
            if (enabled)
            {
                GorillaSnapTurn GorillaSnapTurningComp = GorillaTagger.Instance.offlineVRRig.GorillaSnapTurningComp;

                int turnTypeInt = 0;
                if (GorillaSnapTurningComp != null)
                {
                    GorillaTagger.Instance.offlineVRRig.turnFactor = GorillaSnapTurningComp.turnFactor;
                    GorillaTagger.Instance.offlineVRRig.turnType = GorillaSnapTurningComp.turnType;

                    if (!(GorillaSnapTurningComp.turnType == "SNAP"))
                    {
                        if (GorillaSnapTurningComp.turnType == "SMOOTH")
                            turnTypeInt = 2;
                    }
                    else
                    {
                        turnTypeInt = 1;
                    }
                    turnTypeInt *= 10;
                    turnTypeInt += GorillaSnapTurningComp.turnFactor;
                }
                __result = (short)(spoofFPSValue + (turnTypeInt << 8));
            }
        }
    }
}
