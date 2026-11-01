using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{

    [HarmonyPatch]
    internal class Pathces
    {
        [HarmonyPatch(typeof(HideSeekMenu), "Awake")]
        [HarmonyPostfix]
        static void ReplaceStartWithClasses(HideSeekMenu __instance)
        {
            __instance.transform.Find("MainNew").GetComponent<StandardMenuButton>().OnPress.RemoveAllListeners();
            __instance.transform.Find("MainNew").GetComponent<StandardMenuButton>().OnPress.AddListener(() =>
            {
                var getGameLoader = Resources.FindObjectsOfTypeAll<GameLoader>().First(x => x.IsPrefab());
                var getElev = Resources.FindObjectsOfTypeAll<ElevatorScreen>().First(x => x.IsPrefab() && x.gameObject.activeSelf);
                __instance.gameObject.SetActive(false);
                getElev.gameObject.SetActive(true);
                getGameLoader.CheckSeed();
                getGameLoader.LoadLevel(NullscapeInBB.Am.Get<SceneObject>("scene_classes"));
                getGameLoader.SetSave(true);


            });

            GameObject.FindObjectsOfType<Transform>(true).First(x => x.name == "HideSeekWarning").Find("Yes").GetComponent<StandardMenuButton>().OnPress.RemoveAllListeners();
            GameObject.FindObjectsOfType<Transform>(true).First(x => x.name == "HideSeekWarning").Find("Yes").GetComponent<StandardMenuButton>().OnPress.AddListener(() =>
            {
                var getGameLoader = Resources.FindObjectsOfTypeAll<GameLoader>().First(x => x.IsPrefab());
                var getElev = Resources.FindObjectsOfTypeAll<ElevatorScreen>().First(x => x.IsPrefab() && x.gameObject.activeSelf);
                __instance.gameObject.SetActive(false);
                getElev.gameObject.SetActive(true);
                getGameLoader.CheckSeed();
                getGameLoader.LoadLevel(NullscapeInBB.Am.Get<SceneObject>("scene_classes"));
                getGameLoader.SetSave(true);


            });
        }

        [HarmonyPatch(typeof(Pickup), "ClickableSighted")]
        [HarmonyPostfix]
        static void UpgradeSighted(Pickup __instance, int player)
        {
            // Only show the counter for upgrade pickups.
            if (__instance.item != null &&
                __instance.item.item is UpgradeItem)
            {
                Singleton<CoreGameManager>.Instance
                    .GetHud(player).PointsAnimator.ShowDisplay(true);
            }
        }

        [HarmonyPatch(typeof(Pickup), "ClickableUnsighted")]
        [HarmonyPostfix]
        static void UpgradeUnsighted(Pickup __instance, int player)
        {
            // Only hide it for upgrade pickups.
            if (__instance.item != null &&
                __instance.item.item is UpgradeItem)
            {
                Singleton<CoreGameManager>.Instance
                    .GetHud(player).PointsAnimator.ShowDisplay(false);
            }
        }

    }
}
