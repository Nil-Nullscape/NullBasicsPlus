using HarmonyLib;
using MTM101BaldAPI;
using NilLib;
using NilLib.Creators;
using NullscapeinBB.Misc;
using NullscapeinBB.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

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
        static bool ByebyeSkyboxesIg(Skybox __instance)
        {
            foreach (var item in __instance.gameObject.GetComponentsInChildren<Renderer>())
            {
                item.gameObject.SetActive(false);
            }
            return false;
        }

        [HarmonyPatch(typeof(NPC), "Initialize")]
        [HarmonyPostfix]
        static void Radar(NPC __instance)
        {
            if (NullscapeInBB.UpgradesCount(Upgrades.RadarNPCs) >= 1 && __instance.Character != EnumExtensions.GetFromExtendedName<Character>("NIL")) __instance.ec.map.AddArrow(__instance.Entity, new Color(1, 0, 0));
        }

        [HarmonyPatch(typeof(Baldi), "Slap")]
        [HarmonyPostfix]
        static void ExtraSlap(Baldi __instance)
        {
            if (NullscapeInBB.CurseCount(Curses.OneMoreSlap) >= 1 && Random.Range(0, 100) < (NullscapeInBB.CurseCount(Curses.OneMoreSlap) * 10)) {
                UsefulHelpers.delay(() =>
                {
                    __instance.SetCachedReflection("nextSlapDistance", __instance.GetCachedReflection<float>("slapDistance") * 1.5f);
                    __instance.SlapNormal();
                    __instance.Slap();
                }, 0.15f);
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "AddPoints",[typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(bool)])]
        [HarmonyPostfix]
        static void AddPointsStat(CoreGameManager __instance, int points, int player, bool playAnimation, bool includeInLevelTotal, bool multiply)
        {
            if (points > 0) {
                SaveHelper.SetMiscData(0, SaveHelper.GetMiscData(0) + points);
                
            } else {
                SaveHelper.SetMiscData(1, SaveHelper.GetMiscData(1) - points);
            }
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
        public readonly int FloorCounter = Singleton<BaseGameManager>.Instance is PitstopGameManager ? NullscapeInBB.Instance.FloorCounter + 1 : NullscapeInBB.Instance.FloorCounter;

        public AudioManager audMan;
        public float timeBeforNextThunder = 10f;
        public float thunderEffect = 0f;
        public GameObject ThunderGameObjectIg;
        void Start() {
            var aud = new GameObject("aud");
            aud.transform.SetParent(transform);
            audMan = ObjectCreation.CreateAudioManager(1, 1, false, aud);
            timeBeforNextThunder = Random.Range(10f, 25f);
            var mat = ObjectCreation.CreateMaterial(NilLib.MaterialTypes.Unlit, NullscapeInBB.Am.Get<Sprite>(ModAssetNames.tex_WhiteTex).texture);
            mat.shader = Shader.Find("Sprites/Default");
            mat.mainTexture = NullscapeInBB.Am.Get<Sprite>(ModAssetNames.tex_WhiteTex).texture;
            ThunderGameObjectIg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ThunderGameObjectIg.transform.localScale = new(25, 2000, 25);
            ThunderGameObjectIg.GetComponent<Collider>().enabled = false;
        }

        void Update() {
            if (FloorCounter >= 20) {
                timeBeforNextThunder -= Time.deltaTime;
                ThunderGameObjectIg.GetComponent<Renderer>().material.color = new Color(1, 1, 1, thunderEffect);
                if (thunderEffect > 0) {
                    thunderEffect -= Time.deltaTime / 2f;

                }
                if (timeBeforNextThunder < 0) {
                    timeBeforNextThunder = Random.Range(10f, 25f);
                    thunderEffect = 1f;
                    ThunderGameObjectIg.transform.position = new Vector3(Random.Range(-1000f, 1000f), 0, Random.Range(-1000f, 1000f));
                    audMan.PlaySingle(NullscapeInBB.Instance.ThunderSounds[Random.Range(0, NullscapeInBB.Instance.ThunderSounds.Count)]);

                }
            }
        }


        void LateUpdate() {
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
            transform.localScale = new(-0.1f, 0.1f, 0.1f);
            if (Singleton<CoreGameManager>.Instance.GetPlayer(0)) {
                transform.position = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position;
                var rot = Singleton<CoreGameManager>.Instance.GetPlayer(0).cameraBase.rotation;
                transform.rotation = Quaternion.Euler(rot.eulerAngles.x + Time.time * (FloorCounter * 0.1156f) * 8.0145f, rot.eulerAngles.y + Time.time * (FloorCounter * 0.3116f) * 6.67966f, rot.eulerAngles.z + Time.time * (FloorCounter * 0.225f) * 7.14568f);

            }
            if (FloorCounter < 10)
            {
                color = Color.Lerp(new Color(0f, 0.7f, 1.4f), new Color(0f, 1.4f, 1.6f), Mathf.Sin(Time.time * 1.5f) / 2f) + Color.Lerp(new(0,0,0,0), new(1, 1, 1,0), thunderEffect);
            }
            else if (FloorCounter >= 10 && FloorCounter < 30)
            {
                color = Color.Lerp(new Color(0.5f, 0, 0.5f), new Color(1.5f, 0, 1.5f), Mathf.Sin(Time.time * 1.5f) / 2f) + Color.Lerp(new(0, 0, 0, 0), new(1, 1, 1, 0), thunderEffect); 
            }
            else if (FloorCounter >= 30 && FloorCounter < 40)
            {
                color = Color.Lerp(new Color(0f, 0, 1.4f), new Color(0f, 0, 2.5f), Mathf.Sin(Time.time * 1.5f) / 2f) + Color.Lerp(new(0, 0, 0, 0), new(1, 1, 1, 0), thunderEffect); 
            }
            else if (FloorCounter >= 40)
            {
                color = Color.Lerp(new Color(0.25f, 0.25f, 0.25f), new Color(1.5f, 1.5f, 1.5f), Mathf.Sin(Time.time * 0.75f) / 2f) + Color.Lerp(new(0, 0, 0, 0), new(1, 1, 1, 0), thunderEffect) ;
            }
        }
    }
}
