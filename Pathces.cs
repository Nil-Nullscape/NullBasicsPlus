using HarmonyLib;
using MTM101BaldAPI;
using NullscapeinBB.Misc;
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
        [HarmonyPatch(typeof(Skybox), "InitializeMaterials")]
        [HarmonyPrefix]
        static bool ByebyeSkyboxesIg(Skybox __instance) {
            foreach (var item in __instance.gameObject.GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
            return false;
        }

        [HarmonyPatch(typeof(BaseGameManager),"Start")]
        [HarmonyPostfix]
        static void SkySphereNullscape(BaseGameManager __instance) {
        if (__instance is MainInfiniteGameManager || __instance is PitstopGameManager || __instance is ClassesGameManager) {
                var skybox = GameObject.Instantiate(NullscapeInBB.Am.Get<GameObject>(ModAssetNames.SkyboxPrefab));
                skybox.transform.SetParent(__instance.Ec.transform);
                skybox.transform.position = Vector3.zero;
                skybox.AddComponent<SkySphereAnimation>();

        }
        }
    }

    public class SkySphereAnimation : MonoBehaviour {
        Color color = new Color(1, 1, 1, 1);
        void LateUpdate() {
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
            transform.localScale = new(-0.1f, 0.1f, 0.1f);
            if (Singleton<CoreGameManager>.Instance.GetPlayer(0)) {
                transform.position = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position;
                var rot = Singleton<CoreGameManager>.Instance.GetPlayer(0).cameraBase.rotation;
                transform.rotation = Quaternion.Euler(rot.eulerAngles.x + Time.time * (NullscapeInBB.Instance.FloorCounter * 0.1156f) * 8.0145f, rot.eulerAngles.y + Time.time * (NullscapeInBB.Instance.FloorCounter * 0.3116f) * 6.67966f, rot.eulerAngles.z + Time.time * (NullscapeInBB.Instance.FloorCounter * 0.225f) * 7.14568f);

            }
            if (NullscapeInBB.Instance.FloorCounter < 10)
            {
                color = Color.Lerp(new Color(0f, 0.7f, 1.4f), new Color(0f, 1.4f, 1.6f), Mathf.Sin(Time.time * 1.5f) / 2f);
            }
            else if (NullscapeInBB.Instance.FloorCounter >= 10 && NullscapeInBB.Instance.FloorCounter < 30)
            {
                color = Color.Lerp(new Color(0.5f, 0, 0.5f), new Color(1.5f, 0, 1.5f), Mathf.Sin(Time.time * 1.5f) / 2f);
            }
            else if (NullscapeInBB.Instance.FloorCounter >= 30 && NullscapeInBB.Instance.FloorCounter < 40)
            {
                color = Color.Lerp(new Color(0f, 0, 1.4f), new Color(0f, 0, 2.5f), Mathf.Sin(Time.time * 1.5f) / 2f);
            }
            else if (NullscapeInBB.Instance.FloorCounter >= 40 && NullscapeInBB.Instance.FloorCounter < 100)
            {
                color = Color.Lerp(new Color(0.25f, 0.25f, 0.25f), new Color(1.5f, 1.5f, 1.5f), Mathf.Sin(Time.time * 0.75f) / 2f);
            }
        }
    }
}
