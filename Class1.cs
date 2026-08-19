using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using UnityEngine;

namespace NullscapeinBB
{
    public struct EnemyData(Character enemy, int minFloor, int maxEnemies, NPC npcToUse, string Description)
    {
        public Character Enemy = enemy;
        public int minFloor = minFloor;
        public int MaxEnemies = maxEnemies;
        public NPC NPCToUse = npcToUse;
        public Func<bool> Requirements = null;
        public string Description = Description;
    }
    public struct CurseData(Curses Curse, int minFloor, int StackSize = 1, Action action = null)
    {
        public Curses curse = Curse;
        public int MinFloor = minFloor;
        public int MaxCurses = StackSize;
        public Func<bool> Requirements = null;

        public Action CursesAction = action;
    }

    public struct GreaterCurseData(GreaterCurses GreaterCurse, int minFloor, int StackSize = 1, Action action = null)
    {
        public GreaterCurses curse = GreaterCurse;
        public int MinFloor = minFloor;
        public int MaxCurses = StackSize;
        public Func<bool> Requirements = null;
        public Action CursesAction = action;
    }
    public static class Extensions
    {
        public static IntermissionSong GetSongInRange(this IntermissionSong[] list, int floor)
        {
            foreach (IntermissionSong song in list)
            {
                if (floor >= song.IntermissionSongFloorRange.x && floor <= song.IntermissionSongFloorRange.z)
                {
                    return song;
                }
            }
            return null;
        }
    }

    public enum Curses
    {
        MathMachine,
        BalloonBuster,
        MatchMachine,
        AngrierBaldi,
        OneMinutePomp

    }

    public enum GreaterCurses
    {
        OmniscientBaldi,
        Maintenance,
        TimedActivities
    }

    public class IntermissionSong(SoundObject song, int minFloor, int maxFloor)
    {

        public SoundObject IntermissionSongAsset = song;
        public IntVector2 IntermissionSongFloorRange = new IntVector2(minFloor, maxFloor);
    }

    [BepInPlugin("Nil.NullscapeinBB", "Nullscape in BB", "1.0.0.0")]
    public class NullscapeInBB : BaseUnityPlugin
    {
        public static NullscapeInBB Instance;
        public static AssetManager Am;

        public List<IntermissionSong> IntermissionSongs = new List<IntermissionSong>();
        public SceneObject F1;
        public SceneObject F2;
        public SceneObject F1Clone;
        public bool flop;
        public int FloorCounter;
        public List<WeightedRoomAsset> rooms = new List<WeightedRoomAsset>();
        public List<NPC> potentialNPCs = new List<NPC>();
        // Tracks how many times each enemy (Character) has been selected during intermissions
        public Dictionary<Character, int> selectedNPCCounts = new Dictionary<Character, int>();
        public List<WeightedItemObject> potentialItems = new List<WeightedItemObject>();
        public int notebookcount;
        public IntVector2 levelsize;
        public int plotcount;
        public int mapprice;
        public int itembudget;
        public int facultycount;
        public int fakecount;
        public List<IntermissionType> Intermissions = new();
        public List<CurseData> CursesPicked = new();
        public List<GreaterCurseData> GreaterCursesPicked = new();
        public Dictionary<Character, EnemyData> AllEnemies = new Dictionary<Character, EnemyData>();
        public Dictionary<Character, ItemObject> AllEnemiesItems = new Dictionary<Character, ItemObject>();
        public Dictionary<Curses, ItemObject> AllCursesItems = new Dictionary<Curses, ItemObject>();
        public Dictionary<Curses, CurseData> AllCurses = new Dictionary<Curses, CurseData>();

        public List<StructureWithParameters> ForcedStructureDefault;
        public List<StructureWithParameters> ForcedStructures;
        public Dictionary<GreaterCurses, ItemObject> AllGreaterCursesItems = new Dictionary<GreaterCurses, ItemObject>();
        public Dictionary<GreaterCurses, GreaterCurseData> AllGreaterCurses = new Dictionary<GreaterCurses, GreaterCurseData>();

        public IEnumerator FinalLoad()
        {
            yield return MTM101BaldiDevAPI.itemMetadata.GetAllWithoutFlags(ItemFlags.Unobtainable).Length;
            yield return "Making all items more... expensive";
            foreach (ItemMetaData item in MTM101BaldiDevAPI.itemMetadata.GetAllWithoutFlags(ItemFlags.Unobtainable))
            {
                foreach (ItemObject iob in item.itemObjects)
                {
                    iob.price = Mathf.RoundToInt(iob.price * 5f);

                }
                yield return $"Updated {item.nameKey} price to {item.itemObjects[0].price}";
                yield return new WaitForEndOfFrame();

            }
        }
        static public int CurseCount(Curses curse)
        {
            var e = 0;
            foreach (var cur in Instance.CursesPicked)
            {
                if (cur.curse == curse) e++;
            }
            return e;
        }

        static public int GreaterCurseCount(GreaterCurses curse)
        {
            var e = 0;
            foreach (var cur in Instance.GreaterCursesPicked)
            {
                if (cur.curse == curse) e++;
            }
            return e;
        }
        static public int EnemyCount(Character enemy)
        {
            var e = 0;
            if (Instance == null) return 0;
            Instance.selectedNPCCounts.TryGetValue(enemy, out e);
            return e;
        }
        static public void delay(Action codeToDelay,float time=1f)
        {
            Instance.StartCoroutine(Instance.DelayCode(codeToDelay,time));
            
        }
        public IEnumerator DelayCode(Action code,float time)
        {
            yield return new WaitForSeconds(time);
            code();
        }
        public void Awake()
        {
            Instance = this;
            Am = new AssetManager();
            AssetLoader.LocalizationFromMod(this);
            LoadingEvents.RegisterOnAssetsLoaded(this.Info, Preload(), LoadingEventOrder.Pre);
            GeneratorManagement.Register((BaseUnityPlugin)this, GenerationModType.Addend, new Action<string, int, SceneObject>(AddContent));
            LoadingEvents.RegisterOnAssetsLoaded(this.Info, FinalLoad(), LoadingEventOrder.Final);
            LoadingEvents.RegisterOnAssetsLoaded(this.Info, PostLoad(), LoadingEventOrder.Post);
            MTM101BaldAPI.AssetTools.AssetLoader.LoadLocalizationFolder(Path.Combine(MTM101BaldAPI.AssetTools.AssetLoader.GetModPath(this), "Lang"), Language.English);
            Harmony harmony = new Harmony("Nil.NullscapeinBB");
            harmony.PatchAll();
        }
        IEnumerator PostLoad()
        {
            yield return 2;
            yield return "Modifying the store";
            RoomAsset StoreRoom = Resources.FindObjectsOfTypeAll<RoomAsset>().First(x => ((UnityEngine.Object)x).name == "Room_JohnnysStore");
            GameObject Counter = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Decor_Pedestal");
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup");

            StoreRoom.basicObjects.Add(new()
            {
                position = new Vector3(35, -0f, -40),
                rotation = Quaternion.identity,
                prefab = Counter.transform
            });
            StoreRoom.basicObjects.Add(new()
            {
                position = new Vector3(30, -0f, -42.5f),
                rotation = Quaternion.identity,
                prefab = Counter.transform
            });
            StoreRoom.basicObjects.Add(new()
            {
                position = new Vector3(40, -0f, -42.5f),
                rotation = Quaternion.identity,
                prefab = Counter.transform
            });
            
            yield return "Adding All enemies";
            AllEnemies.Add(Character.Baldi, new(Character.Baldi, 0, 3, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Baldi_Main3"), "Baldi."));
            AllEnemies.Add(Character.Principal, new(Character.Principal, 0, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Principal"), "Baldi."));
            AllEnemies.Add(Character.Playtime, new(Character.Playtime, 0, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Playtime"), "Baldi."));
            AllEnemies.Add(Character.Prize, new(Character.Prize, 5, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "FirstPrize"), "Baldi."));
            AllEnemies.Add(Character.Bully, new(Character.Bully, 5, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Bully"), "Baldi."));
            AllEnemies.Add(Character.Pomp, new(Character.Pomp, 5, 3, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Mrs Pomp"), "Baldi."));
            yield return "Creating All Curses";
            AllCurses.Add(Curses.MathMachine, new()
            {
                curse = Curses.MathMachine,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {
                    foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
                    {
                        if (room.activity.prefab is MathMachine)
                        {
                            rooms.Add(new()
                            {
                                selection = room,
                                weight = 50
                            });
                        }
                    }
                }
            });
            AllCurses.Add(Curses.BalloonBuster, new()
            {
                curse = Curses.BalloonBuster,
                MaxCurses = 1,
                MinFloor = 5,
                CursesAction = () =>
                {
                    foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
                    {
                        if (room.activity.prefab is BalloonBuster)
                        {
                            rooms.Add(new()
                            {
                                selection = room,
                                weight = 50
                            });
                        }
                    }
                }
            });
            AllCurses.Add(Curses.MatchMachine, new()
            {
                curse = Curses.MatchMachine,
                MaxCurses = 1,
                MinFloor = 5,
                CursesAction = () =>
                {
                    foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
                    {
                        if (room.activity.prefab is MatchActivity)
                        {
                            rooms.Add(new()
                            {
                                selection = room,
                                weight = 50
                            });
                        }
                    }
                }
            });

            AllCurses.Add(Curses.AngrierBaldi, new()
            {
                curse = Curses.AngrierBaldi,
                MaxCurses = 3,
                MinFloor = 0,
                CursesAction = () =>
                {

                }
            });

            AllCurses.Add(Curses.OneMinutePomp, new()
            {
                curse = Curses.OneMinutePomp,
                MaxCurses = 1,
                MinFloor = 0,
                Requirements = () => {
                    return EnemyCount(Character.Pomp) >= 1;
                },
                CursesAction = () =>
                {

                }
            });

            AllGreaterCurses.Add(GreaterCurses.OmniscientBaldi, new()
            {
                curse = GreaterCurses.OmniscientBaldi,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {

                }
            });
            AllGreaterCurses.Add(GreaterCurses.TimedActivities, new()
            {
                curse = GreaterCurses.TimedActivities,
                Requirements = () =>
                {
                    return CurseCount(Curses.MathMachine) >= 1 || CurseCount(Curses.MatchMachine) >= 1 || CurseCount(Curses.BalloonBuster) >= 1; 
                },
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {
                    
                }
            });
            AllGreaterCurses.Add(GreaterCurses.Maintenance, new()
            {
                curse = GreaterCurses.Maintenance,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {
                    
                    Debug.Log("Maintenance test");
                    Instance.ForcedStructures.Add(new()
                    {
                        prefab = Resources.FindObjectsOfTypeAll<Structure_PowerLever>().First(),
                        parameters = new()
                        {
                            chance = [1,1],
                            minMax = [
                                new(5,5),
                                new(1,1),
                                new(2,4),
                                new(0,50),
                                new(2,4)
                            ]
                        }
                    });
                }
            });

            yield return "Making all Items";
            foreach (EnemyData enemy in AllEnemies.Values)
            {
                if (enemy.NPCToUse != null)
                {
                    var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{enemy.NPCToUse.name}", $"desc_{enemy.NPCToUse.name}")
                    .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{enemy.NPCToUse.name}.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{enemy.NPCToUse.name}.png"))
                    .SetEnum($"itmenemy_{enemy.NPCToUse.name}")
                    .SetItemComponent<EnemItem>()
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .Build();
                    ((EnemItem)ie.item).Enemy = enemy.Enemy;
                    AllEnemiesItems.Add(enemy.Enemy, ie);
                }
            }
            foreach (Curses curse in Enum.GetValues(typeof(Curses)))
            {
                var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{curse.ToString()}", $"desc_{curse.ToString()}")
                    .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToString()}.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToString()}.png"))
                    .SetEnum($"itmcurse_{curse.ToString()}")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .SetItemComponent<CurseItem>()
                    .Build();
                ((CurseItem)ie.item).curse = curse;
                AllCursesItems.Add(curse, ie);
            }

            foreach (GreaterCurses curse in Enum.GetValues(typeof(GreaterCurses)))
            {
                var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{curse.ToString()}", $"desc_{curse.ToString()}")
                    .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToString()}.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToString()}.png"))
                    .SetEnum($"itmgreatercurse_{curse.ToString()}")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .SetItemComponent<GreaterCurseItem>()
                    .Build();
                ((GreaterCurseItem)ie.item).curse = curse;
                AllGreaterCursesItems.Add(curse, ie);
            }
        }
        IEnumerator Preload()
        {
            yield return 2;
            yield return "Preloading audios";
            Am.Add("mus_i1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss1.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss2.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i3", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss3.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i4", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss4.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i5", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss5.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i6", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss6.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_unforseenunforgiving1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "unforseenunforgiving1.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_unforseenunforgiving2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "unforseenunforgiving2.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_cb1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Choirofthebells-1.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_cb2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Choirofthebells-2.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_eh", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Event_Horizon.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_nil", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Nil.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("snd_purchase", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Purchase.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));

            yield return "Adding Intermission songs";
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i1"), 1, 2));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i2"), 3, 4));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i3"), 5, 6));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i4"), 7, 8));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i5"), 9, 9));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i6"), 10, 14));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_unforseenunforgiving1"), 15, 19));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_unforseenunforgiving2"), 20, 24));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_cb1"), 25, 29));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_cb2"), 30, 34));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_eh"), 35, 49));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_nil"), 50, 99));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_cb2"), 100, int.MaxValue-1));
        }

        public void GetAssets()
        {
        }

        public List<EnemyData> PickThreeEnemies()
        {
            // start from all enemies and filter to those valid for current floor and not exceeding max instances
            var values = AllEnemies.Values.ToList();

            values = values.Where(x =>
            {
                bool meetsFloor = FloorCounter >= x.minFloor;
                int currentCount = 0;
                selectedNPCCounts.TryGetValue(x.Enemy, out currentCount);
                bool underMax = currentCount < x.MaxEnemies;
                if (!underMax)
                {
                    try { Logger.LogInfo($"Filtering out {x.Enemy} in PickThreeEnemies: selected {currentCount} / max {x.MaxEnemies}"); } catch { }
                }
                bool extrarequirement = false;
                if (x.Requirements != null)
                {
                    extrarequirement = x.Requirements();
                }
                else extrarequirement = true;
                return meetsFloor && underMax && extrarequirement;
            }).ToList();

            // Only consider enemies that have a spawnable ItemObject prepared
            values = values.Where(x => AllEnemiesItems.ContainsKey(x.Enemy)).ToList();

            // If none are valid, return empty so caller can skip the intermission
            if (values.Count == 0) return new List<EnemyData>();

            List<EnemyData> result = new List<EnemyData>(3);
            int count = values.Count;

            System.Random rng = new System.Random();

            if (count >= 3)
            {
                var indices = Enumerable.Range(0, count).OrderBy(_ => rng.Next()).Take(3);
                foreach (int i in indices)
                {
                    result.Add(values[i]);
                }
            }
            else if (count == 2)
            {
                result.Add(values[0]);
                result.Add(values[1]);
                result.Add(values[rng.Next(count)]);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    result.Add(values[0]);
                }
            }

            return result;
        }

        public List<CurseData> PickThreeCurses()
        {
            var values = AllCurses.Values.ToList();

            values = values.Where(x =>
            {
                bool meetsFloor = FloorCounter >= x.MinFloor;
                int currentCount = CursesPicked.Count(c => c.curse == x.curse);
                bool underMax = currentCount < x.MaxCurses;
                bool extrarequirement = false;
                if (x.Requirements != null)
                {
                    extrarequirement = x.Requirements();
                }
                else extrarequirement = true;
                return meetsFloor && underMax && extrarequirement;
            }).ToList();

            // Only consider curses that have a spawnable ItemObject prepared
            values = values.Where(x => AllCursesItems.ContainsKey(x.curse)).ToList();

            // If none are valid, return empty so caller can skip the intermission
            if (values.Count == 0) return new List<CurseData>();

            List<CurseData> result = new List<CurseData>(3);
            int count = values.Count;

            System.Random rng = new System.Random();

            if (count >= 3)
            {
                var indices = Enumerable.Range(0, count).OrderBy(_ => rng.Next()).Take(3);
                foreach (int i in indices)
                {
                    result.Add(values[i]);
                }
            }
            else if (count == 2)
            {
                result.Add(values[0]);
                result.Add(values[1]);
                result.Add(values[rng.Next(count)]);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    result.Add(values[0]);
                }
            }

            return result;
        }

        public List<GreaterCurseData> PickThreeGreaterCurses()
        {
            var values = AllGreaterCurses.Values.ToList();

            values = values.Where(x =>
            {
                bool meetsFloor = FloorCounter >= x.MinFloor;
                int currentCount = GreaterCursesPicked.Count(c => c.curse == x.curse);
                bool underMax = currentCount < x.MaxCurses;
                bool extrarequirement = false;
                if (x.Requirements != null)
                {
                    extrarequirement = x.Requirements();
                }
                else extrarequirement = true;
                return meetsFloor && underMax && extrarequirement;
            }).ToList();

            // Only consider curses that have a spawnable ItemObject prepared
            values = values.Where(x => AllGreaterCursesItems.ContainsKey(x.curse)).ToList();

            // If none are valid, return empty so caller can skip the intermission
            if (values.Count == 0) return new List<GreaterCurseData>();

            List<GreaterCurseData> result = new List<GreaterCurseData>(3);
            int count = values.Count;

            System.Random rng = new System.Random();

            if (count >= 3)
            {
                var indices = Enumerable.Range(0, count).OrderBy(_ => rng.Next()).Take(3);
                foreach (int i in indices)
                {
                    result.Add(values[i]);
                }
            }
            else if (count == 2)
            {
                result.Add(values[0]);
                result.Add(values[1]);
                result.Add(values[rng.Next(count)]);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    result.Add(values[0]);
                }
            }

            return result;
        }
        private void AddContent(string floor, int floorNumber, SceneObject scene)
        {
            FloorCounter = 1;
            fakecount = 0;
            flop = false;
            scene.manager = new MainGameManagerBuilder<MainInfiniteGameManager>().Build();
            if (scene.levelTitle == "F1")
            {
                F1 = scene;
                F1Clone = DuplicateSceneObject(F1);
                F1.additionalNPCs = 0;
                F1.totalShopItems = 12;
                F1.previousLevels = new SceneObject[0];
                F1.forcedNpcs = new NPC[0];

                F1.potentialNPCs = new WeightedNPC[0].ToList();
                notebookcount = 1;
                facultycount = 2;
                mapprice = 100;
                plotcount = 3;
                itembudget = 200;
                levelsize = new IntVector2(14, 14);

                CustomLevelObject[] customLevelObjectsa = F1.GetCustomLevelObjects();
                foreach (CustomLevelObject cuslevob in customLevelObjectsa)
                {
                    cuslevob.maxSize = levelsize;
                    cuslevob.minSize = levelsize;
                    cuslevob.timeLimit = 45;
                    cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = notebookcount;
                    cuslevob.roomGroup.First(x => x.name == "Class").minRooms = notebookcount;
                    cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = facultycount + 2;
                    cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = facultycount - 2;
                    cuslevob.maxItemValue = itembudget;
                    cuslevob.maxPlots = plotcount + 1;
                    cuslevob.minPlots = plotcount - 1;
                    ForcedStructureDefault = cuslevob.forcedStructures.ToList();
                    if (FloorCounter >= 18)
                    {
                        cuslevob.exitCount = 4;
                    }
                    else if (FloorCounter >= 12)
                    {
                        cuslevob.exitCount = 3;
                    }
                    else if (FloorCounter >= 6)
                    {
                        cuslevob.exitCount = 2;
                    }
                    else
                    {
                        cuslevob.exitCount = 1;
                    }
                }
            }
            else if (scene.levelTitle == "F2")
            {
                F2 = scene;
                F2.nextLevel = F1;
                F1.nextLevel = F2;
                F1.baldiPrefab = F2.baldiPrefab;

                F2.additionalNPCs = 0;
                F2.totalShopItems = 12;
                F2.mapPrice = 100;
                try
                {
                    F2.previousLevels = new SceneObject[0];
                }
                catch
                {
                }

                CustomLevelObject[] customLevelObjectsb = scene.GetCustomLevelObjects();
                foreach (CustomLevelObject cuslevob in customLevelObjectsb)
                {
                    cuslevob.forcedStructures = ForcedStructureDefault.ToArray();
                    cuslevob.timeLimit = 45;
                    cuslevob.potentialStructures = new WeightedStructureWithParameters[0];
                }
            }

            ForcedStructures = new();
            foreach (var item in ForcedStructureDefault)
            {
                ForcedStructures.Add(item);
            }

            
            
            AssetManager miscman = new AssetManager();
            miscman.AddFromResourcesNoClones<NPC>();
            CustomLevelObject[] customLevelObjects = scene.GetCustomLevelObjects();
            foreach (CustomLevelObject cuslevob in customLevelObjects)
            {
                foreach (WeightedItemObject witem in cuslevob.potentialItems)
                {
                    if (potentialItems.Count != 0)
                    {
                        bool canadd = true;
                        foreach (WeightedItemObject wwitem in potentialItems)
                        {
                            if (wwitem.selection == witem.selection)
                            {
                                canadd = false;
                            }
                        }

                        if (canadd)
                        {
                            potentialItems.Add(witem);
                        }
                    }
                }
                foreach (var cus in F1.GetCustomLevelObjects())
                {
                    foreach (var rg in cus.roomGroup)
                    {
                        if (rg.name == "Class")
                        {
                            foreach (var roo in rg.potentialRooms)
                            {
                                if (!rooms.Contains(roo) && roo.selection.activity.prefab is NoActivity)
                                {
                                    rooms.Add(roo);
                                }
                            }
                        }
                    }
                }
                /* foreach (RoomGroup rg in cuslevob.roomGroup)
                {
                    if (rg.name == "Class")
                    {
                        foreach (WeightedRoomAsset ra in rg.potentialRooms)
                        {
                            if (!rooms.Contains(ra) && ra.selection.activity == Activity.Class)
                            {
                                rooms.Add(ra);
                            }
                        }
                    }
                }  */
            }


        }

        public void ResetValues()
        {
            notebookcount = 1;
            facultycount = 2;
            mapprice = 100;
            plotcount = 3;
            itembudget = 200;
            levelsize = new IntVector2(14, 14);
            FloorCounter = 1;
            fakecount = 0;
            potentialNPCs.Clear();
            selectedNPCCounts.Clear();
            rooms.Clear();
            CursesPicked.Clear();
            ForcedStructures = new();
            foreach (var item in ForcedStructureDefault)
            {
                ForcedStructures.Add(item);
            }

            flop = false;
            foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
            {
                if (room.activity.prefab is NoActivity)
                {
                    rooms.Add(new()
                    {
                        selection = room,
                        weight = 50
                    });
                }
            }
            if (F1 != null)
            {
                F1.additionalNPCs = 0;
                CustomLevelObject[] customLevelObjectsa = F1.GetCustomLevelObjects();

                foreach (CustomLevelObject cuslevob in customLevelObjectsa)
                {
                    cuslevob.maxSize = levelsize;
                    cuslevob.minSize = levelsize;
                    cuslevob.forcedStructures = ForcedStructures.ToArray();
                    cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = notebookcount;
                    cuslevob.roomGroup.First(x => x.name == "Class").minRooms = notebookcount;
                    cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = facultycount + 2;
                    cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = facultycount - 2;
                    cuslevob.maxItemValue = itembudget;
                    cuslevob.maxPlots = plotcount + 1;
                    cuslevob.minPlots = plotcount - 1;
                    
                    cuslevob.timeLimit = 45;
                    if (FloorCounter >= 18)
                    {
                        cuslevob.exitCount = 4;
                    }
                    else if (FloorCounter >= 12)
                    {
                        cuslevob.exitCount = 3;
                    }
                    else if (FloorCounter >= 6)
                    {
                        cuslevob.exitCount = 2;
                    }
                    else
                    {
                        cuslevob.exitCount = 1;
                    }
                }

                F1.levelTitle = "F1";
            }
        }

        private SceneObject DuplicateSceneObject(SceneObject sceneObject)
        {
            return sceneObject == null ? null : UnityEngine.Object.Instantiate(sceneObject);
        }
    }
}
