using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace NullscapeinBB
{
[HarmonyPatch]
    static public class ShieldPatchesThing
    {
    [HarmonyPrefix]
        [HarmonyPatch(typeof(Baldi),"CaughtPlayer")]
        static bool AntiDeath(Baldi __instance, PlayerManager player) {
            if (Singleton<BaseGameManager>.Instance is MainInfiniteGameManager) {

                return ((MainInfiniteGameManager)Singleton<BaseGameManager>.Instance).canDie(__instance);
            } else {
                return true;
            }
        }
    }
}
