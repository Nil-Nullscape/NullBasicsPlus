using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.PlusExtensions;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.UI;
using NilLib;
using NilLib.Builders;
using NilLib.Creators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NullscapeinBB
{
    public enum IntermissionType
    {
        Enemy,
        Curse,
        GreaterCurse,
        CurseBreaker,
        Upgrades,
        Delusions,
        Chaos
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
                    CurseIntermission.CreatePickupsCurses(i11, i21, i31);
                    break;
                case IntermissionType.Enemy:
                    main.Intermissions.RemoveAt(0);
                    var question = false;
                    var random3enemies = main.PickThreeEnemies();
                    if (random3enemies == null || random3enemies.Count < 3)
                    {
                        random3enemies = new()
                        {
                            main.AllEnemies.GetValueSafe(EnumExtensions.GetFromExtendedName<Character>("???")),
                            main.AllEnemies.GetValueSafe(EnumExtensions.GetFromExtendedName<Character>("???")),
                            main.AllEnemies.GetValueSafe(EnumExtensions.GetFromExtendedName<Character>("???"))
                        };
                        question = true;
                    }

                    if (!main.AllEnemiesItems.ContainsKey(random3enemies[0].Enemy) || !main.AllEnemiesItems.ContainsKey(random3enemies[1].Enemy) || !main.AllEnemiesItems.ContainsKey(random3enemies[2].Enemy))
                    {
                        return;
                    }

                    var i1 = main.AllEnemiesItems[random3enemies[0].Enemy];
                    var i2 = main.AllEnemiesItems[random3enemies[1].Enemy];
                    var i3 = main.AllEnemiesItems[random3enemies[2].Enemy];
                    EnemyIntermission.CreatePickupsEnemy(i1, i2, i3,question);
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
                    GreaterCurseIntermission.CreatePickupsGreaterCurses(i100, i200, i300);
                    break;
                case IntermissionType.CurseBreaker:
                    main.Intermissions.RemoveAt(0);
                    var random3curs = main.PickThreeExistingCursesByValue();
                    if (random3curs == null || random3curs.Count < 3)
                    {
                        NextIntermission();
                        // Not enough enemies available; abort safely.
                        return;
                    }


                    var i1000 = main.AllCursesItems[random3curs[0].curse];
                    var i2000 = main.AllCursesItems[random3curs[1].curse];
                    var i3000 = main.AllCursesItems[random3curs[2].curse];
                    CurseBreakerIntermission.CreatePickupsCursesBreaker(i1000, i2000, i3000);
                    break;
                case IntermissionType.Upgrades:
                    main.Intermissions.RemoveAt(0);
                    var rnd3Upg = main.PickThreeUpgrades();
                    if (rnd3Upg == null || rnd3Upg.Count < 3)
                    {
                        NextIntermission();
                        // Not enough enemies available; abort safely.
                        return;
                    }


                    var u1 = main.AllUpgradesItems[rnd3Upg[0].curse];
                    var u2 = main.AllUpgradesItems[rnd3Upg[1].curse];
                    var u3 = main.AllUpgradesItems[rnd3Upg[2].curse];
                    UpgradesIntermission.CreatePickupsUpgrade(u1, u2, u3);
                    break;
                case IntermissionType.Delusions:
                    main.Intermissions.RemoveAt(0);
                    var rnd3del = main.PickThreeDelusions();
                    if (rnd3del == null || rnd3del.Count < 3)
                    {
                        NextIntermission();
                        // Not enough enemies available; abort safely.
                        return;
                    }


                    var u10 = main.AllDelusionsItems[rnd3del[0].curse];
                    var u20 = main.AllDelusionsItems[rnd3del[1].curse];
                    var u30 = main.AllDelusionsItems[rnd3del[2].curse];
                    DelusionsIntermission.CreatePickupsDelusions(u10, u20, u30);
                    break;
                case IntermissionType.Chaos:
                    main.Intermissions.RemoveAt(0);
                    var random3chaos = main.PickThreeChaos();
                    if (random3chaos == null || random3chaos.Count < 3)
                    {
                        NextIntermission();
                        // Not enough enemies available; abort safely.
                        return;
                    }
                    List<ItemObject> yeah = new();
                    for (int i = 0; i < 3; i++)
                    {
                        switch (random3chaos[i].ChaosType)
                        {
                            case ChaosItem.ChaosType.Curse:
                                yeah.Add(main.AllCursesItems[random3chaos[i].CurseData.curse]);
                                break;
                            case ChaosItem.ChaosType.GreaterCurse:
                                yeah.Add(main.AllGreaterCursesItems[random3chaos[i].GreaterCurseData.curse]);
                                break;
                            case ChaosItem.ChaosType.Enemy:
                                yeah.Add(main.AllEnemiesItems[random3chaos[i].EnemyData.Enemy]);
                                break;
                            default:
                                break;
                        }
                    }

                    ChaosIntermission.CreatePickupsChaos(yeah[0], yeah[1], yeah[2]);
                    break;
            }
        }
        public static void CreateCurseBreakerPickup()
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup" && x.GetInstanceID() > 0);

            var Cursebreaker = Object.Instantiate(PickupPrefab, new Vector3(60, -0f, -45f), Quaternion.identity);
            Cursebreaker.GetComponent<Pickup>().showDescription = true;

            NullscapeInBB.Instance.CursebreakerItem.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_cursebreakeritm")} {NullscapeInBB.Instance.cursebreakerprice} {Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_cursebreakeritm2")}";

            Cursebreaker.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.CursebreakerItem);

            Cursebreaker.GetComponent<Pickup>().free = false;
            Cursebreaker.GetComponent<Pickup>().price = NullscapeInBB.Instance.cursebreakerprice;




            Cursebreaker.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            Cursebreaker.transform.localPosition = new Vector3(360, 5, 57.5f);


            Cursebreaker.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {

                NullscapeInBB.Instance.QueuedIntermission.Add(IntermissionType.CurseBreaker);
                NullscapeInBB.Instance.cursebreakerprice += 1000;
                NullscapeInBB.Instance.LastTimeCursebreakerUsed = NullscapeInBB.Instance.FloorCounter + 1;
                Object.Destroy(Cursebreaker);



            };

        }
        public static void CreateShieldPickup()
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup" && x.GetInstanceID() > 0);

            var Cursebreaker = Object.Instantiate(PickupPrefab, new Vector3(60, -0f, -45f), Quaternion.identity);
            Cursebreaker.GetComponent<Pickup>().showDescription = true;

            NullscapeInBB.Instance.ShieldItem.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_shielditm")} {NullscapeInBB.Instance.shieldcost} {Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_shielditm2")}";

            Cursebreaker.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.ShieldItem);

            Cursebreaker.GetComponent<Pickup>().free = false;
            Cursebreaker.GetComponent<Pickup>().price = NullscapeInBB.Instance.shieldcost;

            var yes = new GameObject("yeah");
            yes.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            var shtext = ObjectCreation.MakeTMPObject(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Billboard),true, "", UsefulHelpers.GetSpriteassetFromEnum(UsefulHelpers.SpriteAssetSelection.MathmachineNumbers));
            shtext.transform.parent = yes.transform;
            yes.transform.localPosition = new Vector3(355, 5, 61.5f);
            shtext.horizontalAlignment = HorizontalAlignmentOptions.Center;
            shtext.gameObject.AddComponent<ShieldNumberUpdater>().text = shtext;
            shtext.gameObject.AddComponent<PickupBob>();


            Cursebreaker.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            Cursebreaker.transform.localPosition = new Vector3(355, 5, 57.5f);


            Cursebreaker.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {

                NullscapeInBB.Instance.shieldcost += NullscapeInBB.Instance.shieldcostadd;
                NullscapeInBB.Instance.shields += 1;
                Object.Destroy(Cursebreaker);



            };

        }



        


           

    }
    public class ShieldNumberUpdater : MonoBehaviour {
        public TextMeshPro text;
        
    void Update() {
            var shields = NullscapeInBB.Instance.shields;
            var shstring = shields.ToString();
            if (text) {
                if (shields < 10)
                {
                    text.text = $"<sprite={shields}>";
                } else if (shields >= 10 && shields < 100) {
                    text.text = $"<sprite={shstring[0]}><sprite={shstring[1]}>";
                } else {
                    text.text = $"<sprite={shstring[0]}><sprite={shstring[1]}><sprite={shstring[2]}>";
                }
                }    
        }
    }

    [HarmonyPatch]
    static class PatchesJohn
    {
        public static bool IsSavingPossible = false;
        public static int GetNearestLevel(int level)
        {
            int[] targets = { 0, 3, 5, 8, 10 };
            int remainder = level % 10;
            int baseValue = (level / 10) * 10;

            // Find the first target that is greater than or equal to the remainder
            foreach (int t in targets)
            {
                if (t >= remainder)
                {
                    return baseValue + t;
                }
            }

            // Fallback to the next decade's 0 if nothing matched
            return baseValue + 10;
        }
        [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
        [HarmonyPrefix]
        static void stopandstart2(BaseGameManager __instance)
        {
            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
            Singleton<CoreGameManager>.Instance.SetLives(0, true);
            NullscapeInBB.Instance.YtpMultipliers.Clear();
            if (__instance is PitstopGameManager)
            {
                foreach (var item in NullscapeInBB.Instance.Cooldowns.Keys.ToList())
                {
                    NullscapeInBB.Instance.Cooldowns[item] -= 1;
                    Debug.Log($"Lowered {item.ToStringExtended()} to {NullscapeInBB.Instance.Cooldowns[item]} ");
                }
                var level = NullscapeInBB.Instance.FloorCounter + 1;
                var poster = ObjectCreators.CreatePosterObject(Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "chk_blank"), [
                new() {
                    alignment = TMPro.TextAlignmentOptions.Center,
                    color = Color.white,
                    font = BaldiFonts.ComicSans24.FontAsset(),
                    fontSize = 24,
                    position = new(0,174),
                    size = new(256,32),
                    style = TMPro.FontStyles.Normal,
                    textKey = !(level % 10 == 3 || level % 10 == 5 || level % 10 == 8 || level % 10 == 0) ? "store_locked" : "store_opened"
                },
                new() {
                    alignment = TMPro.TextAlignmentOptions.Center,
                    color = Color.white,
                    font = BaldiFonts.ComicSans12.FontAsset(),
                    fontSize = 12,
                    position = new(24,88),
                    size = new(208,96),
                    style = TMPro.FontStyles.Normal,
                    textKey = !(level % 10 == 3 || level % 10 == 5 || level % 10 == 8 || level % 10 == 0) ? $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("store_locked1")} F{GetNearestLevel(level)}" : $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("store_notice")} F{GetNearestLevel(level+1)}"
                }
            ]);
                __instance.Ec.BuildPoster(poster, __instance.Ec.CellFromPosition(new IntVector2(32, 9)), Direction.North);
                
                List<string> ProgEvents = new();
                NullscapeInBB.Instance.Intermissions.Clear();
                var extraNPCIntermission = 0;
                var extraCurseIntermission = 0;
                extraNPCIntermission += NullscapeInBB.GreaterCurseCount(GreaterCurses.TwiceMoreNpcs);
                if (NullscapeInBB.Instance.FloorCounter + 1 >= 50)
                {
                    extraNPCIntermission += 1;
                    extraCurseIntermission += 1;
                }
                if (NullscapeInBB.Instance.FloorCounter + 1 >= 150)
                {
                    extraNPCIntermission += 1;
                    extraCurseIntermission += 1;
                }
                extraNPCIntermission += NullscapeInBB.Instance.extraenemyintermission;
                
                if ((NullscapeInBB.Instance.FloorCounter + 1) % 2 == 0)
                {
                    for (int i = 0; i < 1 + extraNPCIntermission; i++)
                    {
                        NullscapeInBB.Instance.Intermissions.Add(IntermissionType.Enemy);
                    }
                }

                if ((NullscapeInBB.Instance.FloorCounter + 1) % 3 == 0)
                {
                    for (int i = 0; i < 1 + extraCurseIntermission; i++)
                    {
                        NullscapeInBB.Instance.Intermissions.Add(IntermissionType.Curse);
                    }
                }
                if ((level % 10 == 3 || level % 10 == 5 || level % 10 == 8 || level % 10 == 0))
                {
                    
                   NullscapeInBB.Instance.Intermissions.Add(IntermissionType.Upgrades);
                    
                }
                if ((NullscapeInBB.Instance.FloorCounter + 1) % 5 == 0)
                {
                    if ((NullscapeInBB.Instance.FloorCounter + 1) < 20)
                    {
                        if (((NullscapeInBB.Instance.FloorCounter + 1) == 10) || (NullscapeInBB.Instance.FloorCounter + 1) == 20)
                        {
                            NullscapeInBB.Instance.Intermissions.Add(IntermissionType.GreaterCurse);
                        }

                    }
                    else
                    {
                        NullscapeInBB.Instance.Intermissions.Add(IntermissionType.GreaterCurse);
                    }
                }
                if (NullscapeInBB.Instance.EnableChaosMode.Value) {
                    NullscapeInBB.Instance.Intermissions.Add(IntermissionType.Chaos);
                }
                int[] BaldifasterEvents = [10, 15,20,30,40,50,60,80,100,125,150,175,200,250,300,400,500,600];
                if (BaldifasterEvents.Contains(NullscapeInBB.Instance.FloorCounter + 1))
                {
                    ProgEvents.Add("pevent_fasterbaldi");
                    NullscapeInBB.Instance.CursesPicked.Add(new CurseData()
                    {
                        curse = Curses.AngrierBaldi,
                        MaxCurses = 999,
                        MinFloor = 0,
                        CursesAction = () =>
                        {

                        }
                    });

                }
                if ((NullscapeInBB.Instance.FloorCounter + 1) >= 10 && (NullscapeInBB.Instance.FloorCounter + 1) % 5 == 0)
                {
                    

                }
                if ((NullscapeInBB.Instance.FloorCounter + 1) % 12 == 0)
                {
                    if (NullscapeInBB.Instance.EnabledNullscapeEntities.Value) {
                        ProgEvents.Add("pevent_fastermart");
                    }

                }
                if ((NullscapeInBB.Instance.FloorCounter + 1) % 10 == 0)
                {
                    ProgEvents.Add("pevent_freeshield");
                    ProgEvents.Add("pevent_inflation");
                    NullscapeInBB.Instance.shields += 1;

                }
                if ((NullscapeInBB.Instance.FloorCounter + 1) == 20 && NullscapeInBB.Instance.EnableQuickStart.Value) {
                    ProgEvents.Add("pevent_fastermart");
                    ProgEvents.Add("pevent_fasterbaldi");
                    ProgEvents.Add("pevent_fasterbaldi");
                    ProgEvents.Add("pevent_freeshield");
                    NullscapeInBB.Instance.shields += 1;
                    NullscapeInBB.Instance.CursesPicked.Add(new CurseData()
                    {
                        curse = Curses.AngrierBaldi,
                        MaxCurses = 999,
                        MinFloor = 0,
                        CursesAction = () =>
                        {

                        }
                    });
                    NullscapeInBB.Instance.CursesPicked.Add(new CurseData()
                    {
                        curse = Curses.AngrierBaldi,
                        MaxCurses = 999,
                        MinFloor = 0,
                        CursesAction = () =>
                        {

                        }
                    });
                    NullscapeInBB.Instance.EventsList.Add(Resources.FindObjectsOfTypeAll<RandomEvent>().Where(
                        x => !NullscapeInBB.Instance.EventsList.Contains(x)
                    ).Take(1).Single());
                    NullscapeInBB.Instance.EventsList.Add(Resources.FindObjectsOfTypeAll<RandomEvent>().Where(
                        x => !NullscapeInBB.Instance.EventsList.Contains(x)
                    ).Take(1).Single());
                    NullscapeInBB.Instance.EventsList.Add(Resources.FindObjectsOfTypeAll<RandomEvent>().Where(
                        x => !NullscapeInBB.Instance.EventsList.Contains(x)
                    ).Take(1).Single());
                }

                if ((NullscapeInBB.Instance.FloorCounter + 1) == 50 || (NullscapeInBB.Instance.FloorCounter + 1) == 150)
                {
                    ProgEvents.Add("pevent_morecursesandnpcs");

                }

                if ((NullscapeInBB.Instance.FloorCounter + 1) % 6 == 0)
                {
                    NullscapeInBB.Instance.EventsList.Add(Resources.FindObjectsOfTypeAll<RandomEvent>().Where(
                        x => !NullscapeInBB.Instance.EventsList.Contains(x)
                    ).Take(1).Single());
                }

               

                if (NullscapeInBB.Instance.FloorCounter + 1 - NullscapeInBB.Instance.LastTimeCursebreakerUsed >= 4 && NullscapeInBB.Instance.PickThreeExistingCursesByValue().Count >= 3)
                {
                    Extras.CreateCurseBreakerPickup();
                }
                Extras.CreateShieldPickup();
                foreach (var item in NullscapeInBB.Instance.QueuedIntermission)
                {
                    NullscapeInBB.Instance.Intermissions.Add(item);
                }
                NullscapeInBB.Instance.QueuedIntermission.Clear();
                List<string> tempListForEvents = new();
                var iee = 1;
                if (ProgEvents.Count > 0) IsSavingPossible = false;
                foreach (var item in ProgEvents)
                {
                    tempListForEvents.Add($"{Singleton<LocalizationManager>.Instance.GetLocalizedText(item)}\n({iee}/{ProgEvents.Count})");
                    iee++;
                }

                if (ProgEvents.Count > 0)
                {
                    NullscapeInBB.ProgressionEventNotifier(tempListForEvents.ToArray());
                }
                if (NullscapeInBB.Instance.Intermissions.Count > 0)
                {
                    __instance.Ec.ElevatorManager.SetAllElevators(ElevatorState.Open);
                }
                var calculatedTimeLimit = 45 + (NullscapeInBB.Instance.notebookcount - 1) * 35;
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


            Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier().AddModifier("walkSpeed", new(1 + NullscapeInBB.UpgradesCount(Upgrades.MoreSpeed)*0.1f, 4.5f));
            Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier().AddModifier("runSpeed", new(1 + NullscapeInBB.UpgradesCount(Upgrades.MoreSpeed) * 0.1f, 6.5f));

        }
        [HarmonyPatch(typeof(NoLateTeacher), "Initialize")]
        [HarmonyPrefix]
        static void MsPompPatch(NoLateTeacher __instance)
        {
            if (NullscapeInBB.CurseCount(Curses.OneMinutePomp) >= 1)
            {
                
                __instance.ReflectionSetVariable("classTime", 60);
            }
        }
        [HarmonyPatch(typeof(CoreGameManager), "YtpMultiplier", MethodType.Getter)]
        [HarmonyPostfix]
        static void Deez(ref float __result)
        {
            __result *= 1 + NullscapeInBB.CurseCount(Curses.Lap2);
            __result *= 1 + NullscapeInBB.UpgradesCount(Upgrades.MoreYtps)*0.05f;
            foreach (var item in NullscapeInBB.Instance.YtpMultipliers)
            {
                __result *= item;
            }
        }
        [HarmonyPatch(typeof(PlayerClick), "Reach", MethodType.Getter)]
        [HarmonyPostfix]
        public static void ReachUpgrade(ref float __result) {
            __result *= 1 + NullscapeInBB.UpgradesCount(Upgrades.LongerReach) * 0.35f;
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
                if (NullscapeInBB.Instance.FloorCounter + 1 >= 10 && NullscapeInBB.Instance.FloorCounter + 1 < 30)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(128f / 255f, 0, 128f / 255f), new Color(35f / 255f, 4f / 255f, 67f / 255f), Mathf.Abs(Mathf.Sin(__instance.realTime / 2 + i / 10)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                    __instance.Ec.UpdateQueuedLightChanges();

                }
                if (NullscapeInBB.Instance.FloorCounter + 1 >= 30 && NullscapeInBB.Instance.FloorCounter + 1 < 40)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(1, 0f, 0), new Color(1f, 0.6f, 0f), Mathf.Abs(Mathf.Sin(__instance.realTime / 3 + i / 10)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                }
                if (NullscapeInBB.Instance.FloorCounter + 1 >= 40 && NullscapeInBB.Instance.FloorCounter + 1 < 50)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(0.5f, 0.5f, 1), new Color(0, 0, 0.25f), Mathf.Abs(Mathf.Tan(__instance.realTime / 6 + i / 100)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                    __instance.Ec.UpdateQueuedLightChanges();

                }

                if (NullscapeInBB.Instance.FloorCounter + 1 >= 50 && NullscapeInBB.Instance.FloorCounter + 1 < 10000)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(1, 1, 1), new Color(0.05f, 0.05f, 0.05f), Mathf.Abs(Mathf.Sin(__instance.realTime / 10 + i / 100)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                    __instance.Ec.UpdateQueuedLightChanges();

                }
            }
            if (__instance is MainInfiniteGameManager)
            {
                if (NullscapeInBB.Instance.FloorCounter >= 10 && NullscapeInBB.Instance.FloorCounter < 30)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(128f / 255f, 0, 128f / 255f), new Color(35f / 255f, 4f / 255f, 67f / 255f), Mathf.Abs(Mathf.Sin(__instance.realTime / 2 + i / 10)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                    __instance.Ec.UpdateQueuedLightChanges();

                }
                if (NullscapeInBB.Instance.FloorCounter >= 30 && NullscapeInBB.Instance.FloorCounter < 40)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(1, 0f, 0), new Color(1f, 0.6f, 0f), Mathf.Abs(Mathf.Sin(__instance.realTime / 3 + i / 10)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                }
                if (NullscapeInBB.Instance.FloorCounter >= 40 && NullscapeInBB.Instance.FloorCounter < 50)
                {
                    var i = 0;
                    foreach (var light in __instance.Ec.lights)
                    {
                        i++;
                        light.lightColor = Color.Lerp(new Color(0.5f, 0.5f, 1), new Color(0, 0, 0.25f), Mathf.Abs(Mathf.Tan(__instance.realTime / 6 + i / 100)));
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
                        light.lightColor = Color.Lerp(new Color(1, 1, 1), new Color(0.05f, 0.05f, 0.05f), Mathf.Abs(Mathf.Sin(__instance.realTime / 10 + i / 100)));
                        __instance.Ec.QueueLightSourceForRegenerate(light);
                    }
                    __instance.Ec.UpdateQueuedLightChanges();

                }
            }

        }
        [HarmonyPatch(typeof(SwingDoor), "Initialize")]
        [HarmonyPostfix]
        static void YellowDoorStore(SwingDoor __instance)
        {
            var listofLevelstoNotLockTheDoor = new int[] { 3, 5, 8, 10, 13, 15, 18, 20, 23, 25, 28, 30, 33, 35, 38, 40, 43, 45, 48, 50, 53, 55, 58, 60, 63, 65 };
            var level = NullscapeInBB.Instance.FloorCounter + 1;
            if (__instance.transform.parent && __instance.transform.parent.parent && __instance.transform.parent.parent.gameObject.name == "JohnnysStore" && !(level % 10 == 3 || level % 10 == 5 || level % 10 == 8 || level % 10 == 0))
            {
                __instance.Lock(true);
            }
        }

        [HarmonyPatch(typeof(PitstopGameManager), "BeginPlay")]
        [HarmonyPostfix]
        static void stopandstart(PitstopGameManager __instance)
        {

            if (Singleton<BaseGameManager>.Instance is PitstopGameManager)
            {
                Singleton<MusicManager>.Instance.StopMidi();

                Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(NullscapeInBB.Instance.IntermissionSongs.ToArray().GetSongInRange(NullscapeInBB.Instance.FloorCounter + 1).IntermissionSongAsset, true);
                Singleton<CoreGameManager>.Instance.musicMan.SetLoop(true);

            }
        }

        [HarmonyPatch(typeof(StickerManager), "StickerValue")]
        [HarmonyPostfix]
        static void ModificationsAbtThat(ref int __result, Sticker sticker, StickerManager __instance)
        {
            var Addition = 0;
            for (int i = 0; i < __instance.activeStickerData.Length; i++)
            {
                if (__instance.activeStickerData[i].sticker == sticker && __instance.SlotUpgraded(i))
                {
                    Addition += 2;
                }
            }
            __result += Addition;

        }
        [HarmonyPatch(typeof(StoreRoomFunction), "Initialize")]
        [HarmonyPostfix]
        static void Yeaheayj(StoreRoomFunction __instance)
        {
            if (Singleton<BaseGameManager>.Instance is PitstopGameManager)
            {
                

                __instance.Room.objectObject.transform.Find("RoomBase").Find("Stickers").gameObject.SetActive(false);
                __instance.Room.objectObject.transform.Find("RoomBase").Find("MiscPurchases").gameObject.SetActive(false);
            }

        }
        [HarmonyPatch(typeof(PitstopGameManager),"InPitstop")]
        [HarmonyPostfix]
        public static void OverrideThisStupidPitstop(PitstopGameManager __instance, ref bool __result) {
            if (Singleton<BaseGameManager>.Instance is PitstopGameManager) {
                if (__instance.FieldTripMode || !IsSavingPossible) __result = false;
                else __result = true;
            }
        }
        public static IEnumerator SadlyHaveToDoMyOwnIenumeratorUPDATEforDoor(StandardDoor door) {
            while (true)
            {
                if (!IsSavingPossible) {
                    
                    door.Lock(true);
                    break;
                }
                yield return null;
            }
        }

        [HarmonyPatch(typeof(StandardDoor), "Initialize")]
        [HarmonyPostfix]
        static void UpdateDoor(StandardDoor __instance) {
            if (Singleton<BaseGameManager>.Instance is PitstopGameManager) {

                if ( __instance.aTile.room.name == "Room0_Lobby") {
                    IsSavingPossible = true;
                    __instance.StartCoroutine(SadlyHaveToDoMyOwnIenumeratorUPDATEforDoor(__instance));
                }
            }
        }
    }


    [HarmonyPatch(typeof(BaseGameManager), "FinishLevel")]
    class FinishLevelPatch
    {
        [HarmonyPrefix]
        static void Prefix(BaseGameManager __instance)
        {
            NullscapeInBB main = NullscapeInBB.Instance;

            

            if (__instance is PitstopGameManager)
            {
                main.FloorCounter++;
                main.F1.previousLevels = [];
                main.F2.previousLevels = [];

                GenerationStuff.GlobalGenStuff();
                GenerationStuff.GenStuff(main.F2);
                GenerationStuff.GenStuff(main.F1);
                
            }
            if (__instance is MainInfiniteGameManager) {
                if (NullscapeInBB.Instance.EnableQuickStart.Value) {
                    NullscapeInBB.Instance.FloorCounter = 19;
                    for (int i = 1; i < 20; i++)
                    {
                        if (i % 2 == 0) NullscapeInBB.Instance.QueuedIntermission.Add(IntermissionType.Enemy);
                    }
                    for (int i = 1; i < 20; i++)
                    {
                        if (i % 3 == 0) NullscapeInBB.Instance.QueuedIntermission.Add(IntermissionType.Curse);
                    }
                    NullscapeInBB.Instance.QueuedIntermission.Add(IntermissionType.GreaterCurse);
                    Singleton<CoreGameManager>.Instance.AddPoints(10000, 0, true,false,false);
                }
            }
        }
    }


    [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
    class EndGamePatch
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            //NullscapeInBB.Instance.ResetValues();
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