using HarmonyLib;
using iiMenu.Classes;
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
                GorillaSnapTurn GorillaSnapTurningComp = RigManager.LocalRig.GorillaSnapTurningComp;

                int turnTypeInt = 0;
                if (GorillaSnapTurningComp != null)
                {
                    RigManager.LocalRig.turnFactor = GorillaSnapTurningComp.turnFactor;
                    RigManager.LocalRig.turnType = GorillaSnapTurningComp.turnType;

                    if (!(GorillaSnapTurningComp.turnType == "SNAP"))
                    {
                        if (GorillaSnapTurningComp.turnType == "SMOOTH")
                            turnTypeInt = 2;
                    }
                    else
                        turnTypeInt = 1;
                    
                    turnTypeInt *= 10;
                    turnTypeInt += GorillaSnapTurningComp.turnFactor;
                }
                __result = (short)(spoofFPSValue + (turnTypeInt << 8));
            }
        }
    }
}
