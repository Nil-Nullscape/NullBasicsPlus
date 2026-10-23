using HarmonyLib;
using NilLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
[HarmonyPatch]
    internal static class menuButtonPatchesToRemoveYEAHHH
    {

        [HarmonyPatch(typeof(StandardMenuButton), "Update")]
        [HarmonyPrefix]
        public static void RemoveAllYes(StandardMenuButton __instance)
        {
            List<string> namesToCheck = ["Main"];
            if (namesToCheck.Contains(__instance.gameObject.name))
            {
                __instance.GetComponent<TextLocalizer>().key = "Mode_Nullbasics";
            }
        }

        [HarmonyPatch(typeof(HideSeekMenu), "Awake")]
        [HarmonyPrefix]
        public static bool yeah(HideSeekMenu __instance)
        {
            __instance.GetCachedReflection<MenuToggle>("timeLimitChallengeToggle").Set(false);
            __instance.GetCachedReflection<MenuToggle>("mapChallengeToggle").Set(false);
            __instance.GetCachedReflection<MenuToggle>("inventoryChallengeToggle").Set(false);
            __instance.InventoryChallengeChanged();
            __instance.MapChallengeChanged();
            __instance.TimeLimitChallengeChanged();

            __instance.GetCachedReflection<MenuToggle>("timeLimitChallengeToggle").gameObject.SetActive(false);
            __instance.GetCachedReflection<MenuToggle>("mapChallengeToggle").gameObject.SetActive(false);
            __instance.GetCachedReflection<MenuToggle>("inventoryChallengeToggle").gameObject.SetActive(false);
            return false;
        }
    }
}
