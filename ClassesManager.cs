using NilLib;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NullscapeinBB
{
    public class ClassesGameManager : BaseGameManager
    {
        public override void Initialize()
        {
            Singleton<CoreGameManager>.Instance.SpawnPlayers(ec);
            ec.AssignPlayers();
            _ = completeMapOnReady;
            generatorFinished = true;
            for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
            {
            }

            CollectNotebooks(0);
            GC.Collect();
            Singleton<CoreGameManager>.Instance.ResetCameras();
            Singleton<CoreGameManager>.Instance.ResetShaders();
            Singleton<CoreGameManager>.Instance.readyToStart = true;
            Singleton<CoreGameManager>.Instance.GetHud(0).SetNotebookDisplay(val: false);

            if (beginPlayImmediately)
            {
                BeginPlay();
            }

            if (spawnNpcsOnInit)
            {
                ec.SpawnNPCs();
                ec.StartEventTimers();
            }

            NullscapeInBB.Instance.SelectedClass = Classes.Student;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.ClearItems();
        }

        protected override void AwakeFunction()
        {
            base.AwakeFunction();
        }
        protected override void VirtualUpdate()
        {
            foreach (var item in ec.lights)
            {
                item.lightColor = Color.Lerp(UsefulHelpers.GetColorFromRgb(129, 0, 255), UsefulHelpers.GetColorFromRgb(240, 0, 206), 0.5f + Mathf.Sin(Time.time/2f) * 0.5f);
                ec.QueueLightSourceForRegenerate(item);
            }
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.AddStamina(100, true);
            ec.UpdateQueuedLightChanges();
        }
        public override void BeginPlay()
        {
            base.BeginPlay();
            ec.ElevatorManager.SetAllElevators(ElevatorState.Open);
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<CoreGameManager>.Instance.musicMan.QueueaudioViaAssetManExt(NullscapeInBB.Am, "mus_class", true, true, true);
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup" && x.GetInstanceID() > 0);
            List<Pickup> pickups = new();
            for (int i = 0; i < 5; i++)
            {
                if (i > NullscapeInBB.Instance.Classes.Count - 1) break;
                var classs = NullscapeInBB.Instance.Classes[i];
                var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
                PickUp1.GetComponent<Pickup>().showDescription = true;
                PickUp1.GetComponent<Pickup>().AssignItem(classs.item);
                classs.item.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText(classs.NameKey)}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText(classs.DescKey)}";
                if (classs.usesAbilityKey) {
                    classs.item.descKey += $"\n{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_AbilityKeyWarning")}{NullscapeInBB.Instance.KeyAbility.Value.ToString()}";
                    
                }
                PickUp1.transform.position = ec.rooms[0].objectObject.transform.GetChild(i).transform.position + (Vector3.up * 5);
                pickups.Add(PickUp1.GetComponent<Pickup>());
                PickUp1.GetComponent<Pickup>().OnItemCollected += (pk, plr) =>
                {
                    allNotebooksFound = true;
                    NullscapeInBB.Instance.SelectedClass = classs.classEnum;
                    ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
                    foreach (var item in pickups)
                    {
                        Destroy(item.gameObject);
                    }

                };


            }
            
        }
        protected override void AllNotebooks()
        {
        }
        protected override void LoadSceneObject(SceneObject sceneObject, bool restarting)
        {
            base.LoadSceneObject(Singleton<CoreGameManager>.Instance.nextLevel, false);
        }
        void Load() {
            Singleton<CoreGameManager>.Instance.PrepareForReload();
            Singleton<SubtitleManager>.Instance.DestroyAll();
            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
            Singleton<CoreGameManager>.Instance.nextLevel = Singleton<CoreGameManager>.Instance.sceneObject.nextLevel;
            LoadSceneObject(Singleton<CoreGameManager>.Instance.sceneObject.nextLevel);
        }
        public override void LoadNextLevel()
        {
            PrepareToLoad();
            
                elevatorScreen = UnityEngine.Object.Instantiate(elevatorScreenPre);
                elevatorScreen.OnLoadReady += Load;
                elevatorScreen.Initialize();
            
        }
    }
}
