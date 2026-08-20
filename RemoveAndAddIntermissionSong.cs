using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.UI;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;
using MTM101BaldAPI.PlusExtensions;

namespace NullscapeinBB
{
    public enum IntermissionType
    {
        Enemy,
        Curse,
        GreaterCurse
    }
    static class Extras
    {

        
       public static void NextIntermission()
        {
            var main = NullscapeInBB.Instance;
            if (main == null) return;
            if (main.Intermissions == null || main.Intermissions.Count == 0)
            {
                Singleton<BaseGameManager>.Instance.Ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
                return;
            }

            switch (main.Intermissions[0])
            {
                case IntermissionType.Curse:
                    main.Intermissions.RemoveAt(0);
                    var random3curses = main.PickThreeCurses();
                    if (random3curses == null || random3curses.Count < 3)
                    {
                        NextIntermission();
                        return;
                    }

                    if (!main.AllCursesItems.ContainsKey(random3curses[0].curse) || !main.AllCursesItems.ContainsKey(random3curses[1].curse) || !main.AllCursesItems.ContainsKey(random3curses[2].curse))
                    {
                        return;
                    }

                    var i11 = main.AllCursesItems[random3curses[0].curse];
                    var i21 = main.AllCursesItems[random3curses[1].curse];
                    var i31 = main.AllCursesItems[random3curses[2].curse];
                    CreatePickupsCurses(i11, i21, i31);
                    break;
                case IntermissionType.Enemy:
                    main.Intermissions.RemoveAt(0);
                    var random3enemies = main.PickThreeEnemies();
                    if (random3enemies == null || random3enemies.Count < 3)
                    {
                        NextIntermission();
                        // Not enough enemies available; abort safely.
                        return;
                    }

                    if (!main.AllEnemiesItems.ContainsKey(random3enemies[0].Enemy) || !main.AllEnemiesItems.ContainsKey(random3enemies[1].Enemy) || !main.AllEnemiesItems.ContainsKey(random3enemies[2].Enemy))
                    {
                        return;
                    }

                    var i1 = main.AllEnemiesItems[random3enemies[0].Enemy];
                    var i2 = main.AllEnemiesItems[random3enemies[1].Enemy];
                    var i3 = main.AllEnemiesItems[random3enemies[2].Enemy];
                    CreatePickupsEnemy(i1, i2, i3);
                    break;
                case IntermissionType.GreaterCurse:
                    main.Intermissions.RemoveAt(0);
                    var random3greatercurses = main.PickThreeGreaterCurses();
                    if (random3greatercurses == null || random3greatercurses.Count < 3)
                    {
                        NextIntermission();
                        // Not enough enemies available; abort safely.
                        return;
                    }

                    if (!main.AllGreaterCursesItems.ContainsKey(random3greatercurses[0].curse) || !main.AllGreaterCursesItems.ContainsKey(random3greatercurses[1].curse) || !main.AllGreaterCursesItems.ContainsKey(random3greatercurses[2].curse))
                    {
                        return;
                    }

                    var i100 = main.AllGreaterCursesItems[random3greatercurses[0].curse];
                    var i200 = main.AllGreaterCursesItems[random3greatercurses[1].curse];
                    var i300 = main.AllGreaterCursesItems[random3greatercurses[2].curse];
                    CreatePickupsGreaterCurses(i100, i200, i300);
                    break;
            }
        }

        static void OnCollectIntermissionEnemy(GameObject[] pickups, Pickup currpck,EnemItem ei)
        {
            var npc = NullscapeInBB.Instance.AllEnemies[ei.Enemy].NPCToUse;
            // Only add the NPC once (avoid duplicates across floors)
            var pot = NullscapeInBB.Instance.potentialNPCs;
          
            // Increment selected count for this character so PickThreeEnemies respects MaxEnemies
            var counts = NullscapeInBB.Instance.selectedNPCCounts;
            if (counts.ContainsKey(ei.Enemy)) counts[ei.Enemy]++;
            else counts[ei.Enemy] = 1;
            try
            {
            }
            catch { }
            if (counts[ei.Enemy] <= NullscapeInBB.Instance.AllEnemies[ei.Enemy].MaxEnemies)
            {
                pot.Add(npc);
            }
            foreach (var item in pickups)
            {
                Object.Destroy(item);
            }
            NullscapeInBB.delay(() => {NextIntermission();},0.85f);
        }
         static void OnCollectIntermissionGreaterCurses(GameObject[] pickups, Pickup currpck,GreaterCurseItem ei)
        {
            var npc = NullscapeInBB.Instance.AllGreaterCurses[ei.curse].curse;
            // Only add the NPC once (avoid duplicates across floors)
            var pot = NullscapeInBB.Instance.GreaterCursesPicked;
          
            // Increment selected count for this character so PickThreeEnemies respects MaxEnemies
            pot.Add(NullscapeInBB.Instance.AllGreaterCurses[npc]);
            NullscapeInBB.Instance.AllGreaterCurses[npc].CursesAction();
            foreach (var item in pickups)
            {
                Object.Destroy(item);
            }
            NullscapeInBB.delay(() => {NextIntermission();},0.85f);
        }
  
        static void OnCollectIntermissionCurses(GameObject[] pickups, Pickup currpck,CurseItem ei)
        {
            var CURSETOEXECUTE = NullscapeInBB.Instance.AllCurses[ei.curse];
            // Record that this curse was picked so PickThreeCurses respects MaxCurses
            try
            {
                NullscapeInBB.Instance.CursesPicked.Add(CURSETOEXECUTE);
            }
            catch { }
            CURSETOEXECUTE.CursesAction();
            foreach (var item in pickups)
            {
                Object.Destroy(item);
            }
            NullscapeInBB.delay(() => {NextIntermission();},0.85f);
        }
        
        public static void CreatePickupsEnemy(ItemObject i1, ItemObject i2, ItemObject i3)
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup");
                var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
                PickUp1.GetComponent<Pickup>().showDescription = true;
                PickUp1.GetComponent<Pickup>().AssignItem(i1);
                var PickUp2 = Object.Instantiate(PickupPrefab, new Vector3(30, -0f, -42.5f), Quaternion.identity);
                PickUp2.GetComponent<Pickup>().showDescription = true;
                PickUp2.GetComponent<Pickup>().AssignItem(i2);
                var PickUp3 = Object.Instantiate(PickupPrefab, new Vector3(40, -0f, -42.5f), Quaternion.identity);
                PickUp3.GetComponent<Pickup>().showDescription = true;
                PickUp3.GetComponent<Pickup>().AssignItem(i3);
                EnemItem enemItem1 = ((EnemItem)i1.item);
                EnemItem enemItem2 = ((EnemItem)i2.item);
                EnemItem enemItem3 = ((EnemItem)i3.item);
                PickUp1.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp1.transform.localPosition = new Vector3(335, 5, 60);
                PickUp2.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp2.transform.localPosition = new Vector3(340, 5, 57.5f);
                PickUp3.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp3.transform.localPosition = new Vector3(330, 5, 57.5f);
            var Reroll = Object.Instantiate(PickupPrefab, new Vector3(60, -0f, -45f), Quaternion.identity);
            Reroll.GetComponent<Pickup>().showDescription = true;
            if (NullscapeInBB.Instance.freeRerolls > 0)
            {
                NullscapeInBB.Instance.RerollItem.descKey = $"desc_reroll_free{NullscapeInBB.Instance.freeRerolls}";

            }
            else
            {
                NullscapeInBB.Instance.RerollItem.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_reroll")} {NullscapeInBB.Instance.rerollPrice} {Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_reroll2")}";
            }
            Reroll.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.RerollItem);

            if (NullscapeInBB.Instance.freeRerolls > 0)
            {
                Reroll.GetComponent<Pickup>().free = true;
                
            }
            else
            {
                Reroll.GetComponent<Pickup>().free = false;
                Reroll.GetComponent<Pickup>().price = NullscapeInBB.Instance.rerollPrice;
                
            }
            Reroll.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {

                if (NullscapeInBB.Instance.freeRerolls >0)
                {
                    NullscapeInBB.Instance.freeRerolls -= 1;
                }
                else
                {
                    NullscapeInBB.Instance.rerollPrice += 500;
                }
                Object.Destroy(PickUp1);
                Object.Destroy(PickUp2);
                Object.Destroy(PickUp3);
                Object.Destroy(Reroll);
                NullscapeInBB.delay(() =>
                {
                    var random3 = NullscapeInBB.Instance.PickThreeEnemies();
                    CreatePickupsEnemy(NullscapeInBB.Instance.AllEnemiesItems[random3[0].Enemy], NullscapeInBB.Instance.AllEnemiesItems[random3[1].Enemy], NullscapeInBB.Instance.AllEnemiesItems[random3[2].Enemy]);
                }, 0.5f);

            };
            Reroll.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            Reroll.transform.localPosition = new Vector3(360, 5, 57.5f);
            // Add a small 3D text label above the middle pedestal indicating the intermission type
            GameObject label = new GameObject("IntermissionLabel_Enemy");
                var tm = label.AddComponent<TextMeshPro>();
                tm.text = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("int_Enemy")}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_Enemy")}";
                    tm.fontSize = 8f;
                    tm.richText = true;
                    tm.alignment = TextAlignmentOptions.Center;
                    // Try to assign a Comic Sans TMP font from BaldiFonts via reflection if available
                    tm.color = Color.red;
                    tm.font = BaldiFonts.ComicSans24.FontAsset();

                label.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                label.transform.localPosition = new Vector3(335, 8f, 60);



                // I do not even know what the fucking point of pckup is, I DONT EVEN USE IT :sob:
                PickUp1.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionEnemy(new GameObject[] { PickUp1, PickUp2, PickUp3, label, Reroll }, pckup,enemItem1);
                };
                PickUp2.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionEnemy(new GameObject[] { PickUp1, PickUp2, PickUp3, label, Reroll }, pckup,enemItem2);
                };
                PickUp3.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionEnemy(new GameObject[] { PickUp1, PickUp2, PickUp3, label, Reroll }, pckup,enemItem3);
                };
            ;
        }
     public static void CreatePickupsCurses(ItemObject i1, ItemObject i2, ItemObject i3)
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup");
                var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
                PickUp1.GetComponent<Pickup>().showDescription = true;
                PickUp1.GetComponent<Pickup>().AssignItem(i1);
                var PickUp2 = Object.Instantiate(PickupPrefab, new Vector3(30, -0f, -42.5f), Quaternion.identity);
                PickUp2.GetComponent<Pickup>().showDescription = true;
                PickUp2.GetComponent<Pickup>().AssignItem(i2);
            var PickUp3 = Object.Instantiate(PickupPrefab, new Vector3(40, -0f, -42.5f), Quaternion.identity);
            PickUp3.GetComponent<Pickup>().showDescription = true;
            PickUp3.GetComponent<Pickup>().AssignItem(i3);
            var Reroll = Object.Instantiate(PickupPrefab, new Vector3(60, -0f, -45f), Quaternion.identity);
            Reroll.GetComponent<Pickup>().showDescription = true;
            if (NullscapeInBB.Instance.freeRerolls > 0)
            {
                NullscapeInBB.Instance.RerollItem.descKey = $"desc_reroll_free{NullscapeInBB.Instance.freeRerolls}";

            }
            else
            {
                NullscapeInBB.Instance.RerollItem.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_reroll")} {NullscapeInBB.Instance.rerollPrice} {Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_reroll2")}";
            }
            Reroll.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.RerollItem);

            if (NullscapeInBB.Instance.freeRerolls > 0) {
                Reroll.GetComponent<Pickup>().free = true;
                
            } else {
                Reroll.GetComponent<Pickup>().free = false;
                Reroll.GetComponent<Pickup>().price = NullscapeInBB.Instance.rerollPrice;
            }
            Reroll.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                
                if (NullscapeInBB.Instance.freeRerolls > 0) {
                    NullscapeInBB.Instance.freeRerolls -= 1;
                } else {
                    NullscapeInBB.Instance.rerollPrice += 500;
                }
                Object.Destroy(PickUp1);
                Object.Destroy(PickUp2);
                Object.Destroy(PickUp3);
                Object.Destroy(Reroll);
                NullscapeInBB.delay(() =>
                {
                    var random3 = NullscapeInBB.Instance.PickThreeCurses();
                    CreatePickupsCurses(NullscapeInBB.Instance.AllCursesItems[random3[0].curse], NullscapeInBB.Instance.AllCursesItems[random3[1].curse], NullscapeInBB.Instance.AllCursesItems[random3[2].curse]);
                },0.5f);
                
            };
            
            CurseItem enemItem1 = ((CurseItem)i1.item);
                CurseItem enemItem2 = ((CurseItem)i2.item);
                CurseItem enemItem3 = ((CurseItem)i3.item);
                PickUp1.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp1.transform.localPosition = new Vector3(335, 5, 60);
                PickUp2.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp2.transform.localPosition = new Vector3(340, 5, 57.5f);
            PickUp3.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp3.transform.localPosition = new Vector3(330, 5, 57.5f);
            Reroll.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            Reroll.transform.localPosition = new Vector3(360, 5, 57.5f);

            // Add a small 3D text label above the middle pedestal indicating the intermission type
            GameObject label = new GameObject("IntermissionLabel_Curse");
                var tm = label.AddComponent<TextMeshPro>();
                tm.text = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("int_Curse")}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_Curse")}";
                    tm.fontSize = 8f;
                    tm.richText = true;
                    tm.alignment = TextAlignmentOptions.Center;
                    // Try to assign a Comic Sans TMP font from BaldiFonts via reflection if available
                    tm.color = new Color(168f/255f,0f,202f/255f);
                    tm.font = BaldiFonts.ComicSans24.FontAsset();

                label.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                label.transform.localPosition = new Vector3(335, 8f, 60);

        
                // I do not even know what the fucking point of pckup is, I DONT EVEN USE IT :sob:
                PickUp1.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionCurses(new GameObject[] { PickUp1, PickUp2, PickUp3, label, Reroll }, pckup,enemItem1);
                };
                PickUp2.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionCurses(new GameObject[] { PickUp1, PickUp2, PickUp3, label, Reroll }, pckup,enemItem2);
                };
                PickUp3.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionCurses(new GameObject[] { PickUp1, PickUp2, PickUp3, label,Reroll }, pckup,enemItem3);
                };
            ;
        }
    
    
    public static void CreatePickupsGreaterCurses(ItemObject i1, ItemObject i2, ItemObject i3)
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup");
                var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
                PickUp1.GetComponent<Pickup>().showDescription = true;
                PickUp1.GetComponent<Pickup>().AssignItem(i1);
                var PickUp2 = Object.Instantiate(PickupPrefab, new Vector3(30, -0f, -42.5f), Quaternion.identity);
                PickUp2.GetComponent<Pickup>().showDescription = true;
                PickUp2.GetComponent<Pickup>().AssignItem(i2);
                var PickUp3 = Object.Instantiate(PickupPrefab, new Vector3(40, -0f, -42.5f), Quaternion.identity);
                PickUp3.GetComponent<Pickup>().showDescription = true;
                PickUp3.GetComponent<Pickup>().AssignItem(i3);
                GreaterCurseItem enemItem1 = ((GreaterCurseItem)i1.item);
                GreaterCurseItem enemItem2 = ((GreaterCurseItem)i2.item);
                GreaterCurseItem enemItem3 = ((GreaterCurseItem)i3.item);
                PickUp1.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp1.transform.localPosition = new Vector3(335, 5, 60);
                PickUp2.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp2.transform.localPosition = new Vector3(340, 5, 57.5f);
                PickUp3.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                PickUp3.transform.localPosition = new Vector3(330, 5, 57.5f);
           
            // Add a small 3D text label above the middle pedestal indicating the intermission type
            GameObject label = new GameObject("IntermissionLabel_GreaterCurse");
                var tm = label.AddComponent<TextMeshPro>();
                tm.text = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("int_GreaterCurse")}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_GreaterCurse")}";
                    tm.fontSize = 8f;
                    tm.richText = true;
                    tm.alignment = TextAlignmentOptions.Center;
                    // Try to assign a Comic Sans TMP font from BaldiFonts via reflection if available
                    tm.color = new Color(1,165f/255f,0);
                    tm.font = BaldiFonts.ComicSans24.FontAsset();

                label.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
                label.transform.localPosition = new Vector3(335, 8f, 60);

        
                // I do not even know what the fucking point of pckup is, I DONT EVEN USE IT :sob:
                PickUp1.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionGreaterCurses(new GameObject[] { PickUp1, PickUp2, PickUp3, label }, pckup,enemItem1);
                };
                PickUp2.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionGreaterCurses(new GameObject[] { PickUp1, PickUp2, PickUp3, label }, pckup,enemItem2);
                };
                PickUp3.GetComponent<Pickup>().OnItemCollected += (pckup,plr) => {
                    OnCollectIntermissionGreaterCurses(new GameObject[] { PickUp1, PickUp2, PickUp3, label }, pckup,enemItem3);
                };
            ;
        }
    
    }

    [HarmonyPatch]
    static class PatchesJohn
    {
        [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
        [HarmonyPrefix]
        static void stopandstart2(BaseGameManager __instance)
        {
            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
            Singleton<CoreGameManager>.Instance.SetLives(0,true);
            if (__instance is PitstopGameManager)
            {
                NullscapeInBB.Instance.Intermissions.Clear();
                if ((NullscapeInBB.Instance.FloorCounter + 1) % 2 == 0)
                {
                    for (int i = 0; i < 1 + NullscapeInBB.GreaterCurseCount(GreaterCurses.TwiceMoreNpcs); i++)
                    {
                        NullscapeInBB.Instance.Intermissions.Add(IntermissionType.Enemy);
                    }
                }
                
                if ((NullscapeInBB.Instance.FloorCounter+1 ) % 3 == 0) NullscapeInBB.Instance.Intermissions.Add(IntermissionType.Curse);
                if ((NullscapeInBB.Instance.FloorCounter+1 ) % 5 == 0)
                {
                    if ((NullscapeInBB.Instance.FloorCounter+1 ) < 30)
                    {
                        if (((NullscapeInBB.Instance.FloorCounter+1 ) == 15) || (NullscapeInBB.Instance.FloorCounter+1 ) == 25)
                        {
                            NullscapeInBB.Instance.Intermissions.Add(IntermissionType.GreaterCurse);    
                        }
                        
                    } else
                    {
                        NullscapeInBB.Instance.Intermissions.Add(IntermissionType.GreaterCurse);
                    }
                }

                if (NullscapeInBB.Instance.Intermissions.Count > 0) {
                    __instance.Ec.ElevatorManager.SetAllElevators(ElevatorState.Open);
                }
                var calculatedTimeLimit = 45 + (NullscapeInBB.Instance.notebookcount-1)*35;
                foreach (var cuslevob in NullscapeInBB.Instance.F1.GetCustomLevelObjects())
                {
                    cuslevob.timeLimit = calculatedTimeLimit;
                }
                foreach (var cuslevob in NullscapeInBB.Instance.F2.GetCustomLevelObjects())
                {
                    cuslevob.timeLimit = calculatedTimeLimit;
                }
                
                Extras.NextIntermission();
            }
            Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier().AddModifier("walkSpeed",new(1,4.5f));
            Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier().AddModifier("runSpeed",new(1,6.5f));

        }
        [HarmonyPatch(typeof(NoLateTeacher), "Initialize")]
        [HarmonyPrefix]
        static void MsPompPatch(NoLateTeacher __instance)
        {
            if (NullscapeInBB.CurseCount(Curses.OneMinutePomp) >= 1)
            {
                __instance.ReflectionSetVariable("classTime",60);
            }
        }

        [HarmonyPatch(typeof(Activity), "Initialize")]
        [HarmonyPrefix]
        static void MathMachineFunc(Activity __instance)
        {
            if (!(__instance is NoActivity) && NullscapeInBB.GreaterCurseCount(GreaterCurses.TimedActivities) >= 1)
            {
                var azertyuiop = __instance.room.functionObject.AddComponent<TimedActivityRoomFunctions>();
            __instance.room.functions.AddFunction(azertyuiop);
            azertyuiop.ReflectionSetVariable("room", __instance.room);
            }
        }

        [HarmonyPatch(typeof(BaseGameManager), "Update")]
        [HarmonyPrefix]
        static void CoolLight(BaseGameManager __instance)
        {
            if (__instance is PitstopGameManager)
            {
                if (NullscapeInBB.Instance.FloorCounter+1 >= 10 && NullscapeInBB.Instance.FloorCounter+1 < 25)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(128f/255f,0,128f/255f),new Color(35f/255f,4f/255f,67f/255f),Mathf.Abs(Mathf.Sin(__instance.realTime/2 + i/10)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                __instance.Ec.UpdateQueuedLightChanges();
                
                }
                if (NullscapeInBB.Instance.FloorCounter+1 >= 25 && NullscapeInBB.Instance.FloorCounter+1 < 35)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(1,0f,0),new Color(1f,0.6f,0f),Mathf.Abs(Mathf.Sin(__instance.realTime/3 + i/10)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                }
                if (NullscapeInBB.Instance.FloorCounter+1 >= 35 && NullscapeInBB.Instance.FloorCounter+1 < 50)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(0.5f,0.5f,1),new Color(0,0,0.25f),Mathf.Abs(Mathf.Tan(__instance.realTime/6 + i/100)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                __instance.Ec.UpdateQueuedLightChanges();
                
                }

                if (NullscapeInBB.Instance.FloorCounter+1 >= 50 && NullscapeInBB.Instance.FloorCounter+1 < 10000)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(1,1,1),new Color(0.05f,0.05f,0.05f),Mathf.Abs(Mathf.Sin(__instance.realTime/10 + i/100)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                __instance.Ec.UpdateQueuedLightChanges();
                
                }
            }
        if (__instance is MainInfiniteGameManager)
            {
                if (NullscapeInBB.Instance.FloorCounter >= 10 && NullscapeInBB.Instance.FloorCounter < 25)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(128f/255f,0,128f/255f),new Color(35f/255f,4f/255f,67f/255f),Mathf.Abs(Mathf.Sin(__instance.realTime/2 + i/10)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                __instance.Ec.UpdateQueuedLightChanges();
                
                }
                if (NullscapeInBB.Instance.FloorCounter >= 25 && NullscapeInBB.Instance.FloorCounter < 35)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(1,0f,0),new Color(1f,0.6f,0f),Mathf.Abs(Mathf.Sin(__instance.realTime/3 + i/10)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                }
                if (NullscapeInBB.Instance.FloorCounter >= 35 && NullscapeInBB.Instance.FloorCounter < 50)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(0.5f,0.5f,1),new Color(0,0,0.25f),Mathf.Abs(Mathf.Tan(__instance.realTime/6 + i/100)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                __instance.Ec.UpdateQueuedLightChanges();
                
                }

                if (NullscapeInBB.Instance.FloorCounter >= 50 && NullscapeInBB.Instance.FloorCounter < 10000)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                {
                    i++;
                    light.lightColor = Color.Lerp(new Color(1,1,1),new Color(0.05f,0.05f,0.05f),Mathf.Abs(Mathf.Sin(__instance.realTime/10 + i/100)));
                    __instance.Ec.QueueLightSourceForRegenerate(light);
                }
                __instance.Ec.UpdateQueuedLightChanges();
                
                }
            }
        
        }
        [HarmonyPatch(typeof(SwingDoor), "Initialize")]
        [HarmonyPostfix]
        static void YellowDoorStore(SwingDoor __instance) {
            var listofLevelstoNotLockTheDoor = new int[] {3,5,8,10,13,15,18,20,23,25,28,30,33,35,38,40,43,45,48,50,53,55,58,60,63,65};
            var level = NullscapeInBB.Instance.FloorCounter+1;
            if (__instance.transform.parent && __instance.transform.parent.parent && __instance.transform.parent.parent.gameObject.name == "JohnnysStore" && !(level % 10 == 3 || level % 10 == 5 || level % 10 == 8 || level % 10 == 0 ))
            {
                __instance.Lock(true);
            }
        }

        [HarmonyPatch(typeof(PitstopGameManager),"BeginPlay")]
        [HarmonyPostfix]
        static void stopandstart(PitstopGameManager __instance)
        {
            
            if (__instance is PitstopGameManager && NullscapeInBB.Instance.fakecount % 2 == 1)
            {
                            Singleton<MusicManager>.Instance.StopMidi();
            
            Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(NullscapeInBB.Instance.IntermissionSongs.ToArray().GetSongInRange(NullscapeInBB.Instance.FloorCounter + 1).IntermissionSongAsset,true);
            Singleton<CoreGameManager>.Instance.musicMan.SetLoop(true);
        
            }
        }

        [HarmonyPatch(typeof(StickerManager),"StickerValue")]
        [HarmonyPostfix]
        static void ModificationsAbtThat(ref int __result, Sticker sticker,StickerManager __instance) {
            var Addition = 0;
            for (int i = 0; i < __instance.activeStickerData.Length; i++)
            {
                if (__instance.activeStickerData[i].sticker == sticker && __instance.SlotUpgraded(i)) {
                    Addition += 2;
                }
            }
            __result += Addition;
            
        }
        [HarmonyPatch(typeof(StoreRoomFunction),"Initialize")]
        [HarmonyPostfix]
        static void Yeaheayj(StoreRoomFunction __instance)
        {
            Pickup glueStickPickup = (Pickup)__instance.ReflectionGetVariable("glueStickPickup");
            PriceTag glueStickTag = (PriceTag)__instance.ReflectionGetVariable("glueStickTag");
            if (Singleton<StickerManager>.Instance.TotalGlueSticks < 4)
            {
                glueStickPickup.price = 10000 + Singleton<StickerManager>.Instance.TotalGlueSticks * 5000;
                glueStickTag.SetText(glueStickPickup.price.ToString());
            }
            else
            {
                glueStickPickup.gameObject.SetActive(value: false);
                glueStickTag.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText("TAG_Sold"));
            }
        }


    }

        [HarmonyPatch(typeof(BaseGameManager), "FinishLevel")]
        class FinishLevelPatch
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            NullscapeInBB main = NullscapeInBB.Instance;
            bool flopthing = main.flop;

            main.fakecount++;
            
            if (main.fakecount % 2 == 0)
            {
                main.FloorCounter++;
                main.F1.previousLevels = [];
                main.F2.previousLevels = [];
                
                
                    if (main.FloorCounter % 2 == 0 && main.notebookcount >= 7)
                {
                    main.notebookcount++;
                    main.mapprice += 50;
                } else if (main.notebookcount < 7) {
                    main.notebookcount+= 2;
                    main.mapprice += 25;
                    main.levelsize += new IntVector2(1, 2);
                } 
                if (main.FloorCounter % 2 == 0)
                {
                    if (!(((IntVector2)(main.levelsize + new IntVector2(1, 1))).x > 225))
                    {
                        main.levelsize += new IntVector2(1, 1);
                        main.facultycount++;
                        main.itembudget += 75;
                        main.mapprice += 50;
                    }
                }
                if (main.FloorCounter % 3 == 0)
                {
                    main.plotcount++;
                }
    
                if (flopthing == false)
                {
                   
                    main.F2.previousLevels = [];
                    


                    
                    if (main.FloorCounter % 3 == 0)
                    {
                        main.ForcedStructures.Add(new()
                        {
                            parameters = new()
                            {
                                minMax = [new(1,1)]
                            },
                            prefab = Resources.FindObjectsOfTypeAll<Structure_StudentSpawner>().First()
                        });
                    }
                    
                    main.F2.forcedNpcs = main.potentialNPCs.ToArray();
                    if (main.FloorCounter % 5 == 0)
                    {
                        main.F2.additionalNPCs = 0;
                    }
                    else
                    {
                        main.F2.additionalNPCs = 0;
                    }

                    main.F2.mapPrice = main.mapprice;
                    main.F2.levelTitle = "F" + main.FloorCounter;
                    CustomLevelObject[] customLevelObjects = main.F2.GetCustomLevelObjects();
                    foreach (CustomLevelObject cuslevob in customLevelObjects)
                    {
                        cuslevob.maxSize = main.levelsize;
                        cuslevob.minSize = main.levelsize;
                        cuslevob.forcedStructures = main.ForcedStructures.ToArray();
                        cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = main.notebookcount;
                        cuslevob.roomGroup.First(x => x.name == "Class").minRooms = main.notebookcount;
                        cuslevob.roomGroup.First(x => x.name == "Class").potentialRooms = main.rooms.ToArray();
                        cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = main.facultycount + 2;
                        cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = main.facultycount - 2;
                        cuslevob.maxItemValue = main.itembudget;
                        cuslevob.maxPlots = main.plotcount + 1;
                        cuslevob.minPlots = main.plotcount - 1;

                        if (main.FloorCounter >= 35)
                        {
                            cuslevob.exitCount = 4;
                        }
                        else if (main.FloorCounter >= 25)
                        {
                            cuslevob.exitCount = 3;
                        }
                        else if (main.FloorCounter >= 15)
                        {
                            cuslevob.exitCount = 2;
                        }
                        else
                        {
                            cuslevob.exitCount = 1;
                        }
                        
                    }
                }
                else
                {
                    
                    main.F1.previousLevels = [];

                    
                    main.F1.previousLevels = [];
                   
                    main.F1.forcedNpcs = main.potentialNPCs.ToArray();
                    if (main.FloorCounter % 3 == 0)
                    {
                        main.ForcedStructures.Add(new()
                        {
                            parameters = new()
                            {
                                minMax = [new(1,1)]
                            },
                            prefab = Resources.FindObjectsOfTypeAll<Structure_StudentSpawner>().First()
                        });
                    }

                    main.F1.mapPrice = main.mapprice;
                    main.F1.levelTitle = "F" + main.FloorCounter;
                    CustomLevelObject[] customLevelObjects = main.F1.GetCustomLevelObjects();
                    foreach (CustomLevelObject cuslevob in customLevelObjects)
                    {
                        cuslevob.maxSize = main.levelsize;
                        cuslevob.minSize = main.levelsize;
                        cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = main.notebookcount;
                        cuslevob.roomGroup.First(x => x.name == "Class").minRooms = main.notebookcount;
                        cuslevob.roomGroup.First(x => x.name == "Class").potentialRooms = main.rooms.ToArray();
                        cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = main.facultycount + 2;
                        cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = main.facultycount - 2;
                        cuslevob.maxItemValue = main.itembudget;
                        cuslevob.maxPlots = main.plotcount + 1;
                        cuslevob.forcedStructures = main.ForcedStructures.ToArray();
                        cuslevob.minPlots = main.plotcount - 1;
                        if (main.FloorCounter >= 35)
                        {
                            cuslevob.exitCount = 4;
                        }
                        else if (main.FloorCounter >= 25)
                        {
                            cuslevob.exitCount = 3;
                        }
                        else if (main.FloorCounter >= 15)
                        {
                            cuslevob.exitCount = 2;
                        }
                        else
                        {
                            cuslevob.exitCount = 1;
                        }    
                    }
                }

                bool newflop = false;

                if (main.flop == false)
                {
                    newflop = true;
                }
                else
                {
                    newflop = false;
                }

                main.flop = newflop;
            }
        }
    }


    [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
    class EndGamePatch
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            NullscapeInBB.Instance.ResetValues();
            NullscapeInBB.Instance.F1.previousLevels = new SceneObject[0];
            NullscapeInBB.Instance.F2.previousLevels = new SceneObject[0];
        }
    }

        public static class FloorDuplicator
    {
        private static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("InfiniteFloorsDietEdition_FloorDuplicator");
        private static readonly Dictionary<object, object> _objectMap = new Dictionary<object, object>();

        public static LevelObject CloneLevelObjectDeepReflection(LevelObject originalLevelObject)
        {
            if (originalLevelObject == null)
            {
                return null;
            }

            _objectMap.Clear();
            return (LevelObject)InternalDeepCopy(originalLevelObject);
        }

        private static object InternalDeepCopy(object originalObject)
        {
            if (originalObject == null)
            {
                return null;
            }

            Type type = originalObject.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return originalObject;
            }

            if (_objectMap.ContainsKey(originalObject))
            {
                return _objectMap[originalObject];
            }

            if (originalObject is Object unityObject)
            {
                Logger.LogWarning($"Attempting to deep copy a Unity Object directly via reflection: {unityObject.name}. " +
                                  "Consider using Object.Instantiate for proper Unity object cloning, " +
                                  "then deep-copying its *non-Unity* members if necessary.");
                return Object.Instantiate(unityObject);
            }

            object newObject;
            try
            {
                newObject = Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                Logger.LogError($"[ModName] Type '{type.FullName}' does not have a public parameterless constructor. " +
                                "Cannot create a new instance for deep copy. Returning original object (shallow copy).");
                return originalObject;
            }

            _objectMap.Add(originalObject, newObject);

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    object originalValue = field.GetValue(originalObject);

                    if (originalValue == null)
                    {
                        field.SetValue(newObject, null);
                        continue;
                    }

                    Type fieldType = field.FieldType;

                    if (typeof(IList).IsAssignableFrom(fieldType) && fieldType.IsGenericType)
                    {
                        IList originalList = (IList)originalValue;
                        IList newList = (IList)Activator.CreateInstance(fieldType);

                        foreach (object item in originalList)
                        {
                            newList.Add(InternalDeepCopy(item));
                        }
                        field.SetValue(newObject, newList);
                    }
                    else if (fieldType.IsArray)
                    {
                        Array originalArray = (Array)originalValue;
                        Array newArray = (Array)Activator.CreateInstance(fieldType, originalArray.Length);

                        for (int i = 0; i < originalArray.Length; i++)
                        {
                            newArray.SetValue(InternalDeepCopy(originalArray.GetValue(i)), i);
                        }
                        field.SetValue(newObject, newArray);
                    }
                    else if (!fieldType.IsValueType && fieldType != typeof(string) && !(originalValue is Object))
                    {
                        field.SetValue(newObject, InternalDeepCopy(originalValue));
                    }
                    else
                    {
                        field.SetValue(newObject, originalValue);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"[ModName] Error copying field '{field.Name}' of type '{type.Name}': {ex.Message}");
                }
            }

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    try
                    {
                        object originalValue = prop.GetValue(originalObject, null);

                        if (originalValue == null)
                        {
                            prop.SetValue(newObject, null, null);
                            continue;
                        }

                        Type propType = prop.PropertyType;

                        if (typeof(IList).IsAssignableFrom(propType) && propType.IsGenericType)
                        {
                            IList originalList = (IList)originalValue;
                            IList newList = (IList)Activator.CreateInstance(propType);

                            foreach (object item in originalList)
                            {
                                newList.Add(InternalDeepCopy(item));
                            }
                            prop.SetValue(newObject, newList, null);
                        }
                        else if (propType.IsArray)
                        {
                            Array originalArray = (Array)originalValue;
                            Array newArray = (Array)Activator.CreateInstance(propType, originalArray.Length);

                            for (int i = 0; i < originalArray.Length; i++)
                            {
                                newArray.SetValue(InternalDeepCopy(originalArray.GetValue(i)), i);
                            }
                            prop.SetValue(newObject, newArray, null);
                        }
                        else if (!propType.IsValueType && propType != typeof(string) && !(originalValue is Object))
                        {
                            prop.SetValue(newObject, InternalDeepCopy(originalValue), null);
                        }
                        else
                        {
                            prop.SetValue(newObject, originalValue, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"[ModName] Error copying property '{prop.Name}' of type '{type.Name}': {ex.Message}");
                    }
                }
            }

            return newObject;
        }

        public static SceneObject DuplicateSceneObject(SceneObject SO)
        {
            SceneObject sceneObject = UnityEngine.Object.Instantiate<SceneObject>(SO);

            WeightedLevelObject[] weightedLevelObjectArray = new WeightedLevelObject[0];

            foreach (WeightedLevelObject originalWeightedLevelObject in SO.randomizedLevelObject)
            {
                WeightedLevelObject copyOfWeightedLevelObject = new WeightedLevelObject();

                LevelObject copyOfLevelObject = null;
                try
                {
                    copyOfLevelObject = CloneLevelObjectDeepReflection(originalWeightedLevelObject.selection);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ModName] Critical error during LevelObject deep reflection clone! " +
                                   $"Falling back to shallow copy of LevelObject. Error: {ex.Message}");
                    copyOfLevelObject = originalWeightedLevelObject.selection;
                }

                copyOfWeightedLevelObject.selection = copyOfLevelObject;
                copyOfWeightedLevelObject.weight = originalWeightedLevelObject.weight;

                weightedLevelObjectArray = HarmonyLib.CollectionExtensions.AddToArray<WeightedLevelObject>(weightedLevelObjectArray, copyOfWeightedLevelObject);
            }

            return sceneObject;
        }
    }


}