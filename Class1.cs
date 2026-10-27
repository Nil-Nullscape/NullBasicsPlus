using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlusStudioLevelFormat;
using BepInEx.Bootstrap;
using PlusStudioLevelLoader;
using NilLib;
using MTM101BaldAPI.Reflection;
using NilLib.Creators;
using MTM101BaldAPI.OptionsAPI;
using NullscapeinBB.Structures;
using NilLib.extension;
using NilLib.Builders;
using MTM101BaldAPI.SaveSystem;
using NullscapeinBB.SaveHandlerStuff;

namespace NullscapeinBB
{
    public struct EnemyData(Character enemy, int minFloor, int maxEnemies, NPC npcToUse, string Description = "unused")
    {
        public Character Enemy = enemy;
        public int minFloor = minFloor;
        public int MaxEnemies = maxEnemies;
        public NPC NPCToUse = npcToUse;
        public Sprite Icon = null;
        public Func<bool> Requirements = null;
        public string Description = Description;
    }
    public struct CurseData(Curses Curse, int minFloor, int StackSize = 1, Action action = null, int value = 0)
    {
        public Curses curse = Curse;
        public int MinFloor = minFloor;
        public int MaxCurses = StackSize;
        public Func<bool> Requirements = null;
        public Sprite Icon = null;
        // used for cursebreaker
        public int CurseValue = value;
        public Action CursesAction = action;
    }

    public struct GreaterCurseData(GreaterCurses GreaterCurse, int minFloor, int StackSize = 1, Action action = null)
    {
        public GreaterCurses curse = GreaterCurse;
        public int MinFloor = minFloor;
        public int MaxCurses = StackSize;
        public Func<bool> Requirements = null;
        public Sprite Icon = null;
        public Action CursesAction = action;
    }

    public struct DelusionData(Delusions delusion, int minFloor, int StackSize = 1, Action action = null)
    {
        public Delusions curse = delusion;
        public int MinFloor = minFloor;
        public int MaxCurses = StackSize;
        public Func<bool> Requirements = null;
        public Sprite Icon = null;
        public Action CursesAction = action;
    }

    public struct UpgradeData(Upgrades Upgrade, int minFloor, Func<int> cost, int StackSize = 1, Action action = null)
    {
        public Upgrades curse = Upgrade;
        public int MinFloor = minFloor;
        public int MaxUpgrades = StackSize;
        public Func<int> Cost = cost;
        public Func<bool> Requirements = null;
        public Sprite Icon = null;
        public Action OnUpgradeBought = action;
    }
    public struct ChaosCurseData
    {
        public ChaosItem.ChaosType ChaosType;
        public EnemyData EnemyData;
        public CurseData CurseData;
        public GreaterCurseData GreaterCurseData;

        public ChaosCurseData(EnemyData enemy)
        {
            ChaosType = ChaosItem.ChaosType.Enemy;
            EnemyData = enemy;
            CurseData = default;
            GreaterCurseData = default;
        }

        public ChaosCurseData(CurseData curse)
        {
            ChaosType = ChaosItem.ChaosType.Curse;
            EnemyData = default;
            CurseData = curse;
            GreaterCurseData = default;
        }

        public ChaosCurseData(GreaterCurseData greaterCurse)
        {
            ChaosType = ChaosItem.ChaosType.GreaterCurse;
            EnemyData = default;
            CurseData = default;
            GreaterCurseData = greaterCurse;
        }
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

        public static IngameSong GetRandomSongInRange(this IngameSong[] list, int floor)
        {
            List<IngameSong> validSongs = new List<IngameSong>();

            foreach (IngameSong song in list)
            {
                if (floor >= song.SongFloorRange.x && floor <= song.SongFloorRange.z)
                {
                    validSongs.Add(song);
                }
            }

            if (validSongs.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, validSongs.Count);
            return validSongs[randomIndex];
        }

        public static BaldiRoomAsset[] LoadFolderRooms(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            var d = new List<BaldiRoomAsset>();
            foreach (var item in files)
            {
                BinaryReader binaryReader = new(File.OpenRead(item));
                var a = BaldiRoomAsset.Read(binaryReader);
                d.Add(a);
                binaryReader.Close();

            }
            return d.ToArray();
        }

        public static BaldiRoomAsset LoadRoom(string path)
        {

            BinaryReader binaryReader = new(File.OpenRead(path));
            var a = BaldiRoomAsset.Read(binaryReader);

            binaryReader.Close();


            return a;
        }
    }
    public enum Upgrades {
        MoreYtps,
        MoreSpeed,
        PlusOneInventorySlot,
        BaldiPraise,
        TempShield,
        LongerReach,
        TeleportRandomInCompletedActivityRoom
    }
    public enum Curses
    {
        MathMachine,
        BalloonBuster,
        MatchMachine,
        AngrierBaldi,
        OneMinutePomp,
        OneMoreProblem,
        FasterTimedActivities,
        Lap2,
        GiftActivity,
        BiggerBlast,
        FasterTurningFirstPrize,
        TooJealousArts,
        FasterEnviromnent,
        FasterNPCs,
        ThreeStrikesPrincipal,
        RandomTilesAreSilenced,
        OldChalkface,
        CadenceOneInstrument
    }

    public enum GreaterCurses
    {
        OmniscientBaldi,
        Maintenance,
        TimedActivities,
        HarderPlaytime,
        NoMorePraise,
        TwiceMoreNpcs,
        Nothing,
        Tantrum,
        BaldiAppleEatFaster,
        LessShield,
        SilentNpcs,
        byebyestickystickers
    }

    public enum Delusions {
        baldihappybutapplebye
    }
    public class IntermissionSong(SoundObject song, int minFloor, int maxFloor)
    {

        public SoundObject IntermissionSongAsset = song;
        public IntVector2 IntermissionSongFloorRange = new IntVector2(minFloor, maxFloor);
    }

    public class IngameSong(SoundObject song,SoundObject allnbSong, int minFloor, int maxFloor)
    {

        public SoundObject SongAsset = song;
        public SoundObject allnbSongAsset = allnbSong;
        public IntVector2 SongFloorRange = new IntVector2(minFloor, maxFloor);
    }

    /// <summary>
    /// HEY over here to add your stuff in my mod
    /// </summary>
    public static class ModderHelpers
    {
        /// <summary>
        /// Adds your enemy/npc to the list of choices and ability to spawn. needs to be done before the final loading state
        /// </summary>
        /// <param name="NPCEnum">The enum of your npc</param>
        /// <param name="data">The EnemyData for like the amount of times you can pick this enemy</param>
        public static void AddNPCToChoicesList(Character NPCEnum, EnemyData data)
        {
            if (!NullscapeInBB.Instance.AllEnemies.ContainsKey(NPCEnum))
            {
                NullscapeInBB.Instance.AllEnemies.Add(NPCEnum, data);
                NullscapeInBB.Instance.AllModdedEnemies.Add(NPCEnum,data);
            }
            else
            {
                Debug.LogWarning($"Cannot add {data.NPCToUse.name} to the choices, check if it was not already added.");
            }
        }
        /// <summary>
        /// Creates a curse, requires extending the <see cref="Curses"/> enum. needs to be done before the final loading state
        /// </summary>
        /// <param name="ExtendedCurse">The extended enum curse</param>
        /// <param name="data"></param>
        public static void CreateNewCurseToChoicesList(Curses ExtendedCurse, CurseData data)
        {

            if (!NullscapeInBB.Instance.AllCurses.ContainsKey(ExtendedCurse))
            {
                NullscapeInBB.Instance.AllCurses.Add(ExtendedCurse, data);
            }
            else
            {
                Debug.LogWarning($"Couldnt add a curse to the choices.");
            }
        }




        /// <summary>
        /// Creates a greater curse, requires extending the <see cref="GreaterCurses"/> enum. needs to be done before the final loading state
        /// </summary>
        /// <param name="ExtendedGCurse">The extended enum greater curse</param>
        /// <param name="data"></param>
        public static void CreateNewGreaterCurseToChoicesList(GreaterCurses ExtendedGCurse, GreaterCurseData data)
        {

            if (!NullscapeInBB.Instance.AllGreaterCurses.ContainsKey(ExtendedGCurse))
            {
                NullscapeInBB.Instance.AllGreaterCurses.Add(ExtendedGCurse, data);
            }
            else
            {
                Debug.LogWarning($"Couldnt add a curse to the choices.");
            }
        }
    }

    [BepInDependency("Nil.Library", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudioloader", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("Nil.NullscapeinBB", "Nullscape in BB", "1.0.1.0")]
    public class NullscapeInBB : BaseUnityPlugin
    {
        public static NullscapeInBB Instance;
        public static AssetManager Am;
        public List<IntermissionSong> IntermissionSongs = new List<IntermissionSong>();
        public List<IngameSong> IngameSongs = new List<IngameSong>();
        public SceneObject F1;
        public int shields = 1;
        public int shieldcost = 5000;
        public int shieldcostadd = 2500;
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
        public int freeRerolls = 3;
        public int rerollPrice = 100;
        public int cursebreakerprice = 5000;
        public int LastTimeCursebreakerUsed = -99;
        public int itembudget;
        public int facultycount;
        public int fakecount;
        public List<IntermissionType> QueuedIntermission = new();
        public int extraenemyintermission = 0;
        
        public List<IntermissionType> Intermissions = new();

        public List<RandomEvent> EventsList = new();
        public List<CurseData> CursesPicked = new();

        public List<GreaterCurseData> GreaterCursesPicked = new();
        public List<DelusionData> DelusionsPicked = new();
        public List<UpgradeData> UpgradesBought = new();
        public Dictionary<Character, EnemyData> AllModdedEnemies = new();
        public Dictionary<Character, EnemyData> AllEnemies = new Dictionary<Character, EnemyData>();
        public Dictionary<Character, ItemObject> AllEnemiesItems = new Dictionary<Character, ItemObject>();
        public Dictionary<Curses, ItemObject> AllCursesItems = new Dictionary<Curses, ItemObject>();
        public Dictionary<Curses, CurseData> AllCurses = new Dictionary<Curses, CurseData>();
        public ItemObject RerollItem;
        public ItemObject CursebreakerItem;
        public ItemObject ShieldItem;
        public ItemObject ContinueItem;
        public List<StructureWithParameters> ForcedStructureDefault;
        public List<StructureWithParameters> ForcedStructures;
        public Dictionary<GreaterCurses, ItemObject> AllGreaterCursesItems = new Dictionary<GreaterCurses, ItemObject>();
        public Dictionary<GreaterCurses, GreaterCurseData> AllGreaterCurses = new Dictionary<GreaterCurses, GreaterCurseData>();

        public Dictionary<Delusions, ItemObject> AllDelusionsItems = new Dictionary<Delusions, ItemObject>();
        public Dictionary<Delusions, DelusionData> AllDelusions = new Dictionary<Delusions, DelusionData>();

        public Dictionary<Upgrades, ItemObject> AllUpgradesItems = new();
        public Dictionary<Upgrades, UpgradeData> AllUpgrades = new();

        public ConfigEntry<bool> EnabledNullscapeEntities;
        public ConfigEntry<bool> DisableCopyright;
        public ConfigEntry<bool> EnableChaosMode;
        public ConfigEntry<bool> EnableQuickStart;
        public List<RoomAsset> GiftsActivityRooms = new();
        public List<AltarData> Altars = new();
        public List<float> YtpMultipliers = new();
        public Dictionary<AltarTypes, int> Cooldowns = new();
        public struct AltarData()
        {
            public BaseAltar Altar;
            public int Weight;
            public int Max;
        }
        public void UpdateListOfEnemies() {
            AllEnemies.Clear();
            AllEnemies.Add(Character.Baldi, new(Character.Baldi, 0, 3, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Baldi_Main3"), "Baldi."));
            AllEnemies.Add(Character.Principal, new(Character.Principal, 0, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Principal"), "Baldi."));
            AllEnemies.Add(Character.Playtime, new(Character.Playtime, 0, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Playtime"), "Baldi."));
            AllEnemies.Add(Character.Prize, new(Character.Prize, 5, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "FirstPrize"), "Baldi."));
            AllEnemies.Add(Character.Bully, new(Character.Bully, 5, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Bully"), "Baldi."));
            AllEnemies.Add(Character.Pomp, new(Character.Pomp, 5, 3, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Mrs Pomp"), "Baldi."));
            AllEnemies.Add(Character.Sweep, new(Character.Sweep, 0, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Gotta Sweep"), "Baldi."));
            AllEnemies.Add(Character.Cumulo, new(Character.Cumulo, 10, 3, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "CloudyCopter"), "Baldi."));
            AllEnemies.Add(Character.DrReflex, new(Character.DrReflex, 10, 1, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "DrReflex"), "Baldi."));
            AllEnemies.Add(Character.LookAt, new(Character.LookAt, 10, 1, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "LookAt"), "Baldi."));
            AllEnemies.Add(Character.Chalkles, new(Character.Chalkles, 10, 1, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "ChalkFace"), "Baldi."));
            AllEnemies.Add(Character.Beans, new(Character.Beans, 10, 3, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Beans"), "Baldi."));
            AllEnemies.Add(Character.Crafters, new(Character.Crafters, 10, 2, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Arts and Crafters"), "Baldi."));
            

            if (EnabledNullscapeEntities.Value)
            {
                AllEnemies.Add(Am.Get<NPC>("npc_Mart").Character, new(Am.Get<NPC>("npc_Mart").Character, 0, 2, Am.Get<NPC>("npc_Mart"), "Baldi."));
                AllEnemies.Add(Am.Get<NPC>("npc_Cadence").Character, new(Am.Get<NPC>("npc_Cadence").Character, 9, 1, Am.Get<NPC>("npc_Cadence"), "Baldi."));
                AllEnemies.Add(Am.Get<NPC>("npc_ICBM").Character, new(Am.Get<NPC>("npc_ICBM").Character, 0, 2, Am.Get<NPC>("npc_ICBM"), "Baldi."));
                AllEnemies.Add(Am.Get<NPC>("npc_Tele").Character, new(Am.Get<NPC>("npc_Tele").Character, 9, 2, Am.Get<NPC>("npc_Tele"), "Baldi."));
            }
            // gets used when no more enemies
            AllEnemies.Add(EnumExtensions.ExtendEnum<Character>("???"), new(EnumExtensions.ExtendEnum<Character>("???"), int.MaxValue, int.MaxValue, Resources.FindObjectsOfTypeAll<NPC>().First(x => x.name == "Arts and Crafters"), "Baldi."));
            foreach (var item in AllModdedEnemies)
            {
                AllEnemies.Add(item.Key, item.Value);
            }
            AllEnemiesItems.Clear();
            foreach (EnemyData enemy in AllEnemies.Values)
            {
                if (enemy.NPCToUse != null)
                {
                    if (enemy.Enemy != EnumExtensions.GetFromExtendedName<Character>("???"))
                    {
                        var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{enemy.NPCToUse.name}", $"desc_{enemy.NPCToUse.name}")
                    .SetSprites(enemy.Icon != null ? enemy.Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{enemy.NPCToUse.name}.png"), enemy.Icon != null ? enemy.Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{enemy.NPCToUse.name}.png"))
                    .SetEnum($"itmenemy_{enemy.NPCToUse.name}")
                    .SetItemComponent<EnemItem>()
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .Build();
                        ((EnemItem)ie.item).Enemy = enemy.Enemy;
                        AllEnemiesItems.Add(enemy.Enemy, ie);
                    }
                    else
                    {
                        var ie = new ItemBuilder(this.Info)
                        .SetAsInstantUse()
                        .SetNameAndDescription($"itm_???", $"desc_???")
                        .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_random.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_random.png"))
                        .SetEnum($"itmenemy_random")
                        .SetItemComponent<EnemItem>()
                        .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                        .Build();
                        ((EnemItem)ie.item).Enemy = enemy.Enemy;
                        AllEnemiesItems.Add(enemy.Enemy, ie);
                    }
                }
            }
        }
        public void CheckIfItemSpriteExistsOtherwiseAutomaticallyAddPlaceholderImageToFile(string nameofitem) {
            if (!File.Exists(Path.Combine(this.GetModPath(), "items", $"spr_{nameofitem}.png")))
            {
                File.Copy(Path.Combine(this.GetModPath(), "items", "placeholder.png"), Path.Combine(this.GetModPath(), "items", $"spr_{nameofitem}.png"));
                Debug.LogWarning($"{nameofitem}.png didnt exist, so a placeholder image has been made.");
            }
        }

        public IEnumerator FinalLoad()
        {
            yield return MTM101BaldiDevAPI.itemMetadata.GetAllWithoutFlags(ItemFlags.Unobtainable).Length + 1;
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
            yield return "Making all items for curses and stuff";
            
            foreach (Curses curse in EnumExtensions.GetValues<Curses>())
            {
                CheckIfItemSpriteExistsOtherwiseAutomaticallyAddPlaceholderImageToFile(curse.ToStringExtended());
                
                var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{curse.ToStringExtended()}", $"desc_{curse.ToStringExtended()}")
                    .SetSprites(AllCurses[curse].Icon != null ? AllCurses[curse].Icon :  AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"), AllCurses[curse].Icon != null ? AllCurses[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"))
                    .SetEnum($"itmcurse_{curse.ToStringExtended()}")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .SetItemComponent<CurseItem>()
                    .Build();
                ((CurseItem)ie.item).curse = curse;
                AllCursesItems.Add(curse, ie);
            }

            foreach (GreaterCurses curse in EnumExtensions.GetValues<GreaterCurses>())
            {
                CheckIfItemSpriteExistsOtherwiseAutomaticallyAddPlaceholderImageToFile(curse.ToStringExtended());
                var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{curse.ToStringExtended()}", $"desc_{curse.ToStringExtended()}")
                    .SetSprites(AllGreaterCurses[curse].Icon != null ? AllGreaterCurses[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"), AllGreaterCurses[curse].Icon != null ? AllGreaterCurses[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"))
                    .SetEnum($"itmgreatercurse_{curse.ToString()}")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .SetItemComponent<GreaterCurseItem>()
                    .Build();
                ((GreaterCurseItem)ie.item).curse = curse;
                AllGreaterCursesItems.Add(curse, ie);
            }

            foreach (Delusions curse in EnumExtensions.GetValues<Delusions>())
            {
                CheckIfItemSpriteExistsOtherwiseAutomaticallyAddPlaceholderImageToFile(curse.ToStringExtended());
                var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{curse.ToStringExtended()}", $"desc_{curse.ToStringExtended()}")
                    .SetSprites(AllDelusions[curse].Icon != null ? AllDelusions[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"), AllDelusions[curse].Icon != null ? AllDelusions[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"))
                    .SetEnum($"itmdelusions_{curse.ToString()}")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
                    .SetItemComponent<DelusionsItem>()
                    .Build();
                ((DelusionsItem)ie.item).curse = curse;
                AllDelusionsItems.Add(curse, ie);
            }
            foreach (Upgrades curse in EnumExtensions.GetValues<Upgrades>())
            {
                CheckIfItemSpriteExistsOtherwiseAutomaticallyAddPlaceholderImageToFile(curse.ToStringExtended());
                var ie = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_{curse.ToStringExtended()}", $"desc_{curse.ToStringExtended()}")
                    .SetSprites(AllUpgrades[curse].Icon != null ? AllUpgrades[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"), AllUpgrades[curse].Icon != null ? AllUpgrades[curse].Icon : AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_{curse.ToStringExtended()}.png"))
                    .SetEnum($"itmupgrade_{curse.ToString()}")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase2"))
                    .SetItemComponent<UpgradeItem>()
                    .Build();
                ((UpgradeItem)ie.item).curse = curse;
                AllUpgradesItems.Add(curse, ie);
            }

            RerollItem = new ItemBuilder(this.Info)
                    .SetAsInstantUse()
                    .SetNameAndDescription($"itm_reroll", $"desc_reroll")
                    .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_Reroll.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_Reroll.png"))
                    .SetEnum($"itmreroll")
                    .SetPickupSound(Am.Get<SoundObject>("snd_purchase2"))
                    .SetItemComponent<Item>()
                    .Build();
            CursebreakerItem = new ItemBuilder(this.Info)
            .SetAsInstantUse()
            .SetNameAndDescription($"itm_unused", $"desc_cursebreakeritm")
            .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_CurseBreaker.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_CurseBreaker.png"))
            .SetEnum($"itmcursebreaker")
            .SetPickupSound(Am.Get<SoundObject>("snd_purchase2"))
            .SetItemComponent<Item>()
            .Build();
            ShieldItem = new ItemBuilder(this.Info)
            .SetAsInstantUse()
            .SetNameAndDescription($"itm_unused", $"desc_shielditm")
            .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_shield.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_shield.png"))
            .SetEnum($"itmshield")
            .SetPickupSound(Am.Get<SoundObject>("snd_purchase2"))
            .SetItemComponent<Item>()
            .Build();

            ContinueItem = new ItemBuilder(this.Info)
            .SetAsInstantUse()
            .SetNameAndDescription($"itm_unused", $"desc_continueitm")
            .SetSprites(AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_continue.png"), AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", $"spr_continue.png"))
            .SetEnum($"itmcontinue")
            .SetPickupSound(Am.Get<SoundObject>("snd_purchase"))
            .SetItemComponent<Item>()
            .Build();
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

        static public int UpgradesCount(Upgrades upg)
        {
            var e = 0;
            foreach (var cur in Instance.UpgradesBought)
            {
                if (cur.curse == upg) e++;
            }
            return e;
        }
        static public int DelusionsCount(Delusions upg)
        {
            var e = 0;
            foreach (var cur in Instance.DelusionsPicked)
            {
                if (cur.curse == upg) e++;
            }
            return e;
        }

        static public void ProgressionEventNotifier(params string[] Texts)
        {
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Am.Get<SoundObject>("snd_event"));

            delay(() =>
            {
                var blackImageThingie = new GameObject("eventthingie").AddComponent<RawImage>();
                blackImageThingie.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform, false);
                blackImageThingie.rectTransform.anchorMin = new(0, 0f);
                blackImageThingie.rectTransform.anchorMax = new(1, 0.25f);
                blackImageThingie.rectTransform.offsetMin = new(-0, 0);
                blackImageThingie.rectTransform.offsetMax = new(0, -0);
                blackImageThingie.color = new(0, 0, 0, 0.45f);

                var text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", blackImageThingie.transform, Vector3.zero, true);
                text.color = Color.white;
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.verticalAlignment = VerticalAlignmentOptions.Middle;
                text.rectTransform.anchorMin = new(0f, 1f);
                text.rectTransform.anchorMax = new(1f, 0f);
                text.rectTransform.offsetMin = new(-0, 0);
                text.rectTransform.offsetMax = new(0, -0);
                var i = 0;
                foreach (var item in Texts)
                {
                    delay(() =>
                    {
                        text.text = item;
                    }, 2.5f * i);
                    i++;
                }
                delay(() =>
                {
                    Destroy(blackImageThingie);
                    Destroy(text);
                }, i * 2.5f);


            }, 3f);

        }
        static public int GetCooldown(AltarTypes altar) {
            return Instance.Cooldowns[altar];
        }
        static public void SetCooldown(AltarTypes altar,int cooldown) {
            NullscapeInBB.Instance.Cooldowns[altar] = cooldown;
        } 
        static public void ShieldEventNotifier(string enemykey, int tempShieldRemaining = 0, bool lostTempShield = false)
        {
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Am.Get<SoundObject>("snd_shieldbreak"));

            delay(() =>
            {
                var blackImageThingie = new GameObject("eventthingie").AddComponent<RawImage>();
                blackImageThingie.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform, false);
                blackImageThingie.rectTransform.anchorMin = new(0, 0f);
                blackImageThingie.rectTransform.anchorMax = new(1, 0.25f);
                blackImageThingie.rectTransform.offsetMin = new(-0, 0);
                blackImageThingie.rectTransform.offsetMax = new(0, -0);
                blackImageThingie.color = new(0, 0, 0, 0.45f);

                var text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", blackImageThingie.transform, Vector3.zero, true);
                text.color = Color.white;
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.verticalAlignment = VerticalAlignmentOptions.Middle;
                text.rectTransform.anchorMin = new(0f, 1f);
                text.rectTransform.anchorMax = new(1f, 0f);
                text.rectTransform.offsetMin = new(-0, 0);
                text.rectTransform.offsetMax = new(0, -0);
                text.text = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld1")} {Singleton<LocalizationManager>.Instance.GetLocalizedText(enemykey)}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld2")} {Instance.shields} {Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld3")}";
                if (lostTempShield || tempShieldRemaining > 0) {
                    text.text = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld1")} {Singleton<LocalizationManager>.Instance.GetLocalizedText(enemykey)}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld2")} {Instance.shields} {Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld3B")} {tempShieldRemaining} {Instance.shields} {Singleton<LocalizationManager>.Instance.GetLocalizedText("Shld4B")}";

                }
                delay(() =>
                {
                    Destroy(blackImageThingie.gameObject);
                }, 4f);


            }, 0f);

        }

        static public void AltarEventNotifier(string texyt)
        {
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Am.Get<SoundObject>("snd_altarSound"));

            delay(() =>
            {
                var blackImageThingie = new GameObject("eventthingie").AddComponent<RawImage>();
                blackImageThingie.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform, false);
                blackImageThingie.rectTransform.anchorMin = new(0, 0f);
                blackImageThingie.rectTransform.anchorMax = new(1, 0.25f);
                blackImageThingie.rectTransform.offsetMin = new(-0, 0);
                blackImageThingie.rectTransform.offsetMax = new(0, -0);
                blackImageThingie.color = new(0, 0, 0, 0.45f);

                var text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", blackImageThingie.transform, Vector3.zero, true);
                text.color = Color.white;
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.verticalAlignment = VerticalAlignmentOptions.Middle;
                text.rectTransform.anchorMin = new(0f, 1f);
                text.rectTransform.anchorMax = new(1f, 0f);
                text.rectTransform.offsetMin = new(-0, 0);
                text.rectTransform.offsetMax = new(0, -0);
                text.text = texyt;
                delay(() =>
                {
                    Destroy(blackImageThingie);
                    Destroy(text);
                }, 4f);


            }, 0f);

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
        static public void delay(Action codeToDelay, float time = 1f)
        {
            Instance.StartCoroutine(Instance.DelayCode(codeToDelay, time));

        }

        public IEnumerator DelayCode(Action code, float time)
        {
            yield return new WaitForSeconds(time);
            code();
        }
        public static void AddCurseToList(Curses cursetoadd) {
            Instance.CursesPicked.Add(Instance.AllCurses[cursetoadd]);
        }
        public void Awake()
        {
            Instance = this;
            EnabledNullscapeEntities = Config.Bind("Gameplay stuff", "EnableNullscapeEnemies", true, "This enables the enemies from nullscape if you dont want them set this to false");
            EnableChaosMode = Config.Bind("Gameplay stuff", "EnableChaosMode", false, "This enables chaos mode, an extra intermission will be added every level, where greater curses, curses or enemies can all spawn");
            EnableQuickStart = Config.Bind("Gameplay stuff", "EnableQuickStart", false, "This enables quick start, after you beat level 1 you will be at level 20, you gotta pick every intermission you missed for example and you start with 10k ytps");
            DisableCopyright = Config.Bind("Misc stuff", "DisableCopyrightedStuff", false, "This switches most sounds or music to smth else or mute them (like teledancer's song)");
            Am = new AssetManager();
            AssetLoader.LocalizationFromMod(this);
            LoadingEvents.RegisterOnAssetsLoaded(this.Info, Preload(), LoadingEventOrder.Pre);
            GeneratorManagement.Register((BaseUnityPlugin)this, GenerationModType.Base, new Action<string, int, SceneObject>(AddContent));
            LoadingEvents.RegisterOnAssetsLoaded(this.Info, FinalLoad(), LoadingEventOrder.Final);
            LoadingEvents.RegisterOnAssetsLoaded(this.Info, PostLoad(), LoadingEventOrder.Post);
            MTM101BaldAPI.AssetTools.AssetLoader.LoadLocalizationFolder(Path.Combine(MTM101BaldAPI.AssetTools.AssetLoader.GetModPath(this), "Lang"), Language.English);
            MTM101BaldiDevAPI.AddWarningScreen("If you dont like the nullscape entities and prefer only the vanilla content, feel free to disable them in the config (BepInEx/config/Nil.NullscapeinBB.cfg)", false);
            MTM101BaldiDevAPI.AddWarningScreen("also incase you scared of copyrighted songs/sound you can enable the config to switch or mute the copyrighted ones (BepInEx/config/Nil.NullscapeinBB.cfg)", false);
            Harmony harmony = new Harmony("Nil.NullscapeinBB");
            // MTM101BaldiDevAPI.SaveGamesEnabled = false;
            CustomOptionsCore.OnMenuInitialize += AddCategory;
            ModdedSaveGame.AddSaveHandler(new SaveHandler());
            harmony.PatchAll();
            
        }
        void AddCategory(OptionsMenu __instance, CustomOptionsHandler handler) {
            if (Singleton<CoreGameManager>.Instance != null) return;
            handler.AddCategory<NullbasicsSettings>("Nullbasics Settings");
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
            UpdateListOfEnemies();
            yield return "Creating All Curses";


            // greater curses

            StuffCreation.AddCurses();
            StuffCreation.AddGreaterCurses();
            StuffCreation.AddUpgrades();
            StuffCreation.AddDelusions();

        }
        IEnumerator Preload()
        {
            yield return 8;
            #region assets stuff
            yield return "Preloading audios (this might take a bit)";
            Am.Add("mus_i1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss1.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_findyourflame", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Find-your-Flame.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss2.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i3", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss3.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i4", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss4.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i5", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss5.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_i6", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "intermiss6.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_fire1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "FearBeforeFailure-1.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_fire2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "FearBeforeFailure-2.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_fire3", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "FearBeforeFailure-3.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_unforseenunforgiving1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "unforseenunforgiving1.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_unforseenunforgiving2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "unforseenunforgiving2.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_cb1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Choirofthebells-1.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_cb2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Choirofthebells-2.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_eh", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Event_Horizon.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_nil", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Nil.mp3"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("snd_purchase", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Purchase.mp3"), "mus_intermission", SoundType.Effect, UnityEngine.Color.black, 0.01f));
            Am.Add("snd_purchase2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Purchase2.mp3"), "mus_intermission", SoundType.Effect, UnityEngine.Color.black, 0.01f));
            Am.Add("snd_allnb", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "BAL_AllNotebooks.ogg"), "nbal_all1", SoundType.Voice, UnityEngine.Color.green, 16f));
            Am.Add("snd_event", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "EventStartBell.wav"), "snd_progressionEvent", SoundType.Effect, UnityEngine.Color.white, 4f));
            Am.Add("snd_martloop", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "MartLoop.mp3"), "snd_mart", SoundType.Effect, UnityEngine.Color.cyan, int.MaxValue));
            Am.Add("snd_martkill", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "MartKill.mp3"), "snd_martkill", SoundType.Effect, UnityEngine.Color.cyan, 1f));
            Am.Add("snd_martmerge", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Mart_Merge.ogg"), "snd_martmerge", SoundType.Effect, UnityEngine.Color.cyan, 1f));
            Am.Add("snd_computercount10", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "ComputerCount_10.ogg"), "comp_10", SoundType.Voice, UnityEngine.Color.white, 4f));
            Am.Add("mus_normal_domasp", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Domasp-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_domasp", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Domasp-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_lineoffire", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Line_of_Fire-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_lineoffire", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Line_of_Fire-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_icbm", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "ICBM-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_icbm", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "ICBM-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_sygt", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Sygt-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_sygt", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Sygt-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_formergarden", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Former-Gardens-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_formergarden", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Former-Gardens-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_cadence", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "cadence-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_cadence", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "cadence-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_deathdefiance", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Death-Defiance-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_deathdefiance", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Death-Defiance-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_congaline", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Congaline-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_congaline", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "Congaline-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_normal_babyface", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "BabyFace-normal.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("mus_collapse_babyface", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "music", "Ingame", "BabyFace-collapse.ogg"), "mus_intermission", SoundType.Music, UnityEngine.Color.black, 0.01f));
            Am.Add("snd_cadenceP1", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cad_lv1.wav"), "snd_cadenceticking", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_cadenceP2", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cad_lv2.wav"), "snd_cadenceticking", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_cadenceP3", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cad_lv3.wav"), "snd_cadenceintenseticking", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            
            Am.Add("snd_cadenceP4", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cad_lv4.wav"), "snd_cadenceveryintenseticking", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_harpInst", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "HarpAmbience.wav"), "snd_harpambience", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_violinInst", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "ViolinAmbience.wav"), "snd_violinambience", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_instwarning", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cadence_Spawn_Alarm.ogg"), "snd_cadencespawninstrument", SoundType.Effect, UnityEngine.Color.gray, 2));
            Am.Add("snd_collectharp", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "CollectHarp.wav"), "snd_harp", SoundType.Effect, UnityEngine.Color.gray, 3));
            Am.Add("snd_collectviolin", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "CollectViolin.wav"), "snd_violin", SoundType.Effect, UnityEngine.Color.gray, 3));
            Am.Add("snd_cadencechasebegin", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cadence_ChaseImpact.ogg"), "snd_angrycadencebegin", SoundType.Effect, UnityEngine.Color.gray, 3));
            Am.Add("snd_cadencechaseend", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "Cadence_ChaseStop.ogg"), "snd_angrycadenceend", SoundType.Effect, UnityEngine.Color.gray, 3));
            Am.Add("snd_cadencechase", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "cadence", "CadenceChase.ogg"), "snd_angrycadence", SoundType.Effect, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_ICBMaim", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "ICBMStrikeBegin.ogg"), "snd_icbmAiming", SoundType.Effect, UnityEngine.Color.red, 8));
            Am.Add("snd_ICBMExplode", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "ICBMStrikeKABOOM.ogg"), "snd_icbmBoom", SoundType.Effect, UnityEngine.Color.red, 3));
            
                Am.Add("mus_teledance", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "TeledancerThemeNewer.mp3"), "mus_teledance", SoundType.Voice, UnityEngine.Color.gray, int.MaxValue));
            
            Am.Add("snd_telefraggertp", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Teleport.mp3"), "snd_telefraggertp", SoundType.Voice, UnityEngine.Color.gray, 1f));
            Am.Add("snd_telefraggerwalk", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Telefragger_Walk_Patch5.ogg"), "snd_telefraggerwalk", SoundType.Voice, UnityEngine.Color.gray, int.MaxValue));
            Am.Add("snd_shieldbreak", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "Shield_Break.ogg"), "snd_shieldbreak", SoundType.Effect, UnityEngine.Color.cyan, 0.01f));
            Am.Add("snd_altarSound", MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "altars", "Main_Altar_sfx.ogg"), "snd_usedaltar", SoundType.Effect, UnityEngine.Color.cyan, 0.01f));
            yield return "Loading textures/Images";

            Am.Add<Texture2D>("tex_poster1", AssetLoader.TextureFromMod(this, "posters", "cadencebutdontcare.png"));
            Am.Add<Texture2D>("tex_poster2", AssetLoader.TextureFromMod(this, "posters", "mart.png"));
            Am.Add<Sprite>("spr_mart", AssetLoader.SpriteFromMod(this,Vector2.one/2,20, "newenemies", "mart.png"));
            Am.Add<Sprite>("spr_icbmmarker", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 10, "newenemies", "ICBM_Marker.png"));
            Am.Add<Sprite>("spr_icbm", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 10, "newenemies", "ICBMAnimated.png"));
            Am.Add("sprsheet_mart", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "newenemies", "martSpritesheet.png"), 84, 84, 20, 24));
            Am.Add("sprsheet_tele", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "newenemies", "Telefragger_Spritesheet.png"), 100, 100, 30, 2));
            Am.Add("sprsheet_cadence", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "newenemies", "CadenceSpritesheet.png"), 200, 200, 35, 12));
            Am.Add("sprsheet_teledance", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "newenemies", "Teledance_spritesheet.png"), 480, 480, 100, 441));
            Am.Add("sprsheet_gift", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "activitiesstuff", "newgift.png"), 150, 150, 55, 23));
            Am.Add("sprsheet_ggift", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "activitiesstuff", "newgoldgift.png"), 150, 150, 55, 23));
            Am.Add("sprsheet_cadence2", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "newenemies", "AngryCadenceSpritesheet.png"), 120, 120, 20, 24));
            Am.Add("sprsheet_explode", AssetLoader.SpritesFromSpriteSheetCount(AssetLoader.TextureFromMod(this, "newenemies", "Explostion_Sheet.png"), 128, 128, 5, 9));
            Am.Add("spr_harpinst", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 100, "newenemies", "CadenceHarpInstrument.png"));
            Am.Add("spr_violininst", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 100, "newenemies", "CadenceViolinInstrument.png"));
            Am.Add("spr_intenemy", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent1.png"));
            Am.Add("spr_intcurse", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent2.png"));
            Am.Add("spr_intgcurse", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent3.png"));
            Am.Add("spr_intcursebreaker", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent4.png"));
            Am.Add("spr_intchaos", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent5.png"));
            Am.Add("spr_intupgrade", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent6.png"));
            Am.Add("spr_intdelusion", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 25, "intermissionText", "NULLBASIC_text_transparent7.png"));
            Am.Add("spr_cadencestring", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "newenemies", "Cadence_new_chain.png"));
            Am.Add("spr_brokenshield", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", "spr_brokenshield.png"));
            Am.Add("spr_hudlist", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "textures", "HUD_ListOf.png"));
            yield return "Adding ingame Songs to the list of lists";
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_domasp"), Am.Get<SoundObject>("mus_collapse_domasp"), 0, 15));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_lineoffire"), Am.Get<SoundObject>("mus_collapse_lineoffire"), 0, 9));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_icbm"), Am.Get<SoundObject>("mus_collapse_icbm"), 0, 15));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_sygt"), Am.Get<SoundObject>("mus_collapse_sygt"), 4, 20));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_formergarden"), Am.Get<SoundObject>("mus_collapse_formergarden"), 20, 100000));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_cadence"), Am.Get<SoundObject>("mus_collapse_cadence"), 25, 100000000));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_congaline"), Am.Get<SoundObject>("mus_collapse_congaline"), 10, 1000000));
            IngameSongs.Add(new(Am.Get<SoundObject>("mus_normal_babyface"), Am.Get<SoundObject>("mus_collapse_babyface"), 5, 20));
            yield return "Adding Intermission songs";
            
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i1"), 1, 2));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i2"), 3, 4));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i3"), 5, 6));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i4"), 7, 8));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i5"), 9, 9));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_i6"), 10, 14));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_fire1"), 15, 16));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_fire2"), 17, 18));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_fire3"), 19, 19));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_unforseenunforgiving1"), 20, 24));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_unforseenunforgiving2"), 25, 29));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_cb1"), 30, 34));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_cb2"), 35, 39));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_eh"), 40, 49));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_nil"), 50, 99));
            IntermissionSongs.Add(new IntermissionSong(Am.Get<SoundObject>("mus_cb2"), 100, int.MaxValue - 1));
            Am.Get<SoundObject>("snd_allnb").additionalKeys = [
                new() { key = "nbal_all2", time = 2.231f},
                new() { key = "nbal_all3", time = 4.802f},
                new() { key = "nbal_all4", time = 8.008f},
                new() { key = "nbal_all5", time = 14.712f},
            ];
            #endregion
            yield return "Adding misc. stuff";
            Am.AddFromResources<RandomEvent>();
            yield return "Creating posters";
            Am.Add("pst_poster1", ObjectCreators.CreatePosterObject(Am.Get<Texture2D>("tex_poster1"), [
                new() {
                alignment = TextAlignmentOptions.Center,
                color = Color.black,
                font = BaldiFonts.BoldComicSans24.FontAsset(),
                fontSize = 22,
                position = new(35,15),
                size = new(181,106),
                textKey = "pst_cadencedontcare"
                }
            ]));
            Am.Add("pst_poster2", ObjectCreators.CreatePosterObject(Am.Get<Texture2D>("tex_poster2"), []));
            yield return "Making new NPCs";
            Am.Add("npc_Mart", new NPCBuilder<MartNPC>(this.Info)
                .AddTrigger()
                .SetAirborne()
                .SetEnum("Mart")
                .SetStationary()
                .SetName("Mart")
                .Build());
            Am.Get<NPC>("npc_Mart").spriteRenderer[0].sprite = Am.Get<Sprite>("spr_mart");
            Destroy(Am.Get<MartNPC>("npc_Mart").GetComponent<PropagatedAudioManager>());
            Am.Add("npc_Tele", new NPCBuilder<TelefraggerNPC>(this.Info)
                .AddTrigger()
                .SetAirborne()
                .SetEnum("Telefragger")
                .SetStationary()
                .SetMinMaxAudioDistance(25,125)
                .SetName("Telefragger")
                .Build());
            Am.Get<NPC>("npc_Tele").spriteRenderer[0].sprite = Am.Get<Sprite>("npc_Tele");
            Destroy(Am.Get<TelefraggerNPC>("npc_Tele").GetComponent<PropagatedAudioManager>());
            Am.Add("npc_Cadence", new NPCBuilder<CadenceNPC>(this.Info)
                .AddTrigger()
                .SetAirborne()
                .SetEnum("Cadence")
                .SetStationary()
                .SetName("Cadence")
                .IgnorePlayerOnSpawn()
                .SetMinMaxAudioDistance(50, 750)
                .Build());
            Destroy(Am.Get<CadenceNPC>("npc_Cadence").GetComponent<PropagatedAudioManager>());
            var Inst = new GameObject("InstrumentPre").AddComponent<Instrument>();
            Inst.gameObject.ConvertToPrefab(true);
            Inst.gameObject.AddComponent<SphereCollider>().radius = 7.5f;
            Inst.gameObject.GetComponent<SphereCollider>().isTrigger = true;
            Inst.gameObject.AddComponent<PropagatedAudioManager>();
            Inst.AudMan = Inst.gameObject.GetComponent<PropagatedAudioManager>();
            var InstRenderer = new GameObject("Renderer").AddComponent<SpriteRenderer>();
            InstRenderer.gameObject.layer = LayerMask.NameToLayer("Overlay");
            InstRenderer.material = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "SpriteStandard_Billboard");
            InstRenderer.transform.parent = Inst.transform;
            Inst.Renderer = InstRenderer;
            Am.Get<CadenceNPC>("npc_Cadence").Pre = Inst;
            Am.Get<NPC>("npc_Cadence").spriteRenderer[0].sprite = Am.Get<Sprite[]>("sprsheet_cadence")[0];
            Am.Add("npc_ICBM", new NPCBuilder<IcbmNPC>(this.Info)
                .AddTrigger()
                .SetAirborne()
                .SetEnum("ICBM")
                .SetStationary()
                .SetName("ICBM")
                .IgnorePlayerOnSpawn()
                .SetMinMaxAudioDistance(15, 100)
                .Build());
            Am.Get<IcbmNPC>("npc_ICBM").AimingRenderer = ObjectCreation.MakeRenderer(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay),Am.Get<IcbmNPC>("npc_ICBM").transform, false, "AimICBMPre");
            Am.Get<IcbmNPC>("npc_ICBM").AimingRenderer.sprite = Am.Get<Sprite>("spr_icbmmarker");
            Am.Get<IcbmNPC>("npc_ICBM").AimingRenderer.transform.localScale = Vector2.one * 1.5f;
            yield return "Creating all altars";
            var pedestalPre = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Decor_Pedestal");






            Altars.Add(new()
            {
                Altar = BaseAltar.CreateAltarPrefab<AltarChance>(pedestalPre, AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "altars", "altar_pedestalPlaceholder.png"), [AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "altars", "chanceAltarItem.png")], "altar_Chance", 1, "AltarChancePre"),
                Max = 2,
                Weight = 75
            });

            Altars.Add(new()
            {
                Altar = BaseAltar.CreateAltarPrefab<AltarPurgatory>(pedestalPre, AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "altars", "Altar_PurgPedestal.png"), [AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "altars", "purgatoryAltarItem.png")], "altar_Purgatory", 1, "AltarPurgPre"),
                Max = 1,
                Weight = 999
            });

            Altars.Add(new()
            {
                Altar = BaseAltar.CreateAltarPrefab<AltarOfDuality>(pedestalPre, AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "altars", "altar_pedestalPlaceholder.png"), [AssetLoader.SpriteFromMod(this, Vector2.one / 2, 30, "altars", "delusionAltarItem.png")], "altar_Delusion", 1, "AltarDeluluPre"),
                Max = 1,
                Weight = 45
            });

            var s = new GameObject("structure");
            s.ConvertToPrefab(true);
            s.AddComponent<Structure_AltarBuilder>()
            
            
            
            
            
            ;

            var AltarRoom = LevelImporter.CreateVanillaRoomAsset(NilLib.FormatLoaderExtension.LoadRoom(Path.Combine(this.GetModPath(), "rooms", "AltarRoom.rbpl")));
            AltarRoom.roomFunctionContainer = RoomFunctionContainerBuilder.CreateBaseObject().Build();
            AltarRoom.category = RoomCategory.Null;
            AltarRoom.color = UsefulHelpers.GetColorFromRgb(250, 255, 0);
            AltarRoom.doorMats = ObjectCreators.CreateDoorDataObject("dor", AssetLoader.TextureFromMod(this, "textures", "DoorAltar_Opened.png"), AssetLoader.TextureFromMod(this, "textures", "DoorAltar_Closed.png"));
            s.GetComponent<Structure_AltarBuilder>().AltarRoom = AltarRoom;
            Am.Add("altar", s.GetComponent<Structure_AltarBuilder>());
            
            
               

            yield return "adding misc. stuff 2 (editor stuff and activities)";
            var gcactgameobj = new GameObject("Act_giftCollection");
            gcactgameobj.ConvertToPrefab(true);
            gcactgameobj.AddComponent<BoxCollider>().size = Vector3.one * 3;
            gcactgameobj.GetComponent<BoxCollider>().isTrigger = true;
            var gcact = gcactgameobj.AddComponent<GiftCollectionActivity>();
            ObjectCreation.CreateHangingBonusSign(gcact.transform, gcact, Vector3.up);
            var anim = ObjectCreation.CreateActivityWallSign("Activi", AssetLoader.SpriteFromMod(this, new(1, 0.5f), 35, "activitiesstuff", "giftactivity_right.png"), AssetLoader.SpriteFromMod(this, new(0, 0.5f), 35, "activitiesstuff", "giftactivity_left.png"));
            gcact.SetCachedReflection("exteriorSignPrefab", anim);

            var giftPre = new GameObject("GiftPre").AddComponent<Gift>();
            giftPre.gameObject.ConvertToPrefab(true);
            var giftcollider = giftPre.gameObject.AddComponent<BoxCollider>();
            giftcollider.size = Vector3.one * 4 + Vector3.up * 100;
            giftcollider.isTrigger = true;
            var renderer = new GameObject("Renderer").AddComponent<SpriteRenderer>();
            renderer.transform.SetParent(giftPre.transform);
            renderer.gameObject.layer = LayerMask.NameToLayer("Billboard");
            giftPre.renderer = renderer;
            renderer.material = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "SpriteStandard_Billboard");
            gcact.giftPre = giftPre.gameObject;
            gcact.sprGiftSheet = Am.Get<Sprite[]>("sprsheet_gift");
            gcact.sprGoldenGiftSheet = Am.Get<Sprite[]>("sprsheet_ggift");
            gcact.sndCollectNormal = ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "GiftCollect.ogg"), "snd_giftcollect", SoundType.Effect, UnityEngine.Color.magenta, 0.4f);
            gcact.sndCollectGolden = ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "sounds", "GoldGiftCollect.ogg"), "snd_giftcollect", SoundType.Effect, UnityEngine.Color.yellow, 0.4f);

            var HintObject = new GameObject("GiftPlacementHint").AddComponent<ActivityGiftPlacementHint>();
            HintObject.gameObject.ConvertToPrefab(true);
            HintObject.gameObject.AddComponent<BoxCollider>();
            HintObject.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            HintObject.gameObject.GetComponent<BoxCollider>().size = Vector3.one * 3;


            var VisualEditoronly = new GameObject("VEO_veo");
            VisualEditoronly.AddComponent<SpriteRenderer>().sprite = AssetLoader.SpriteFromMod(this, Vector2.one / 2, 35, "items", "placeholder.png");
            Am.Add("pre_Visual", VisualEditoronly);

            PlusStudioLevelLoader.LevelLoaderPlugin.Instance.activityAliases.Add("null_giftcollection", gcact);
            PlusStudioLevelLoader.LevelLoaderPlugin.Instance.basicObjects.Add("null_giftcollectiongifthint", HintObject.gameObject);

            if (Chainloader.PluginInfos.ContainsKey("mtm101.rulerp.baldiplus.levelstudio")) {
                Debug.Log("adding editor studffff))))))))))))))");
                leveleditorshit.addEditorStuff();
            }
            var e =Extensions.LoadFolderRooms(Path.Combine(MTM101BaldAPI.AssetTools.AssetLoader.GetModPath(this), "rooms", "GiftCollectionActivity"));
            foreach (var item in e)
            {
                GiftsActivityRooms.Add(LevelImporter.CreateVanillaRoomAsset(item));
            }
            foreach (var room in GiftsActivityRooms) {
                room.color = UsefulHelpers.GetColorFromRgb(194, 0, 255);
            }
        }

        public void GetAssets()
        {
        }


        public List<ChaosCurseData> PickThreeChaos()
        {
            List<ChaosCurseData> pool = new List<ChaosCurseData>();

            // 1. Add all Enemies without filtering
            foreach (var enemy in AllEnemies.Values)
            {
                pool.Add(new ChaosCurseData(enemy));
            }

            // 2. Add all Curses without filtering
            foreach (var curse in AllCurses.Values)
            {
                pool.Add(new ChaosCurseData(curse));
            }

            // 3. Add all Greater Curses without filtering
            foreach (var greaterCurse in AllGreaterCurses.Values)
            {
                pool.Add(new ChaosCurseData(greaterCurse));
            }

            // If total pool is empty, return empty list
            if (pool.Count == 0) return new List<ChaosCurseData>();

            List<ChaosCurseData> result = new List<ChaosCurseData>(3);
            int count = pool.Count;
            System.Random rng = new System.Random();

            if (count >= 3)
            {
                var indices = Enumerable.Range(0, count).OrderBy(_ => rng.Next()).Take(3);
                foreach (int i in indices)
                {
                    result.Add(pool[i]);
                }
            }
            else if (count == 2)
            {
                result.Add(pool[0]);
                result.Add(pool[1]);
                result.Add(pool[rng.Next(count)]);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    result.Add(pool[0]);
                }
            }

            return result;
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

        public List<CurseData> PickThreeExistingCursesByValue()
        {
            var values = CursesPicked
                .Where(x => x.CurseValue > 0)
                .Distinct()
                .OrderByDescending(x => x.CurseValue)
                .ToList();

            int count = values.Count;
            if (count == 0) return new List<CurseData>();

            List<CurseData> result = new List<CurseData>(3);

            if (count >= 3)
            {
                result.Add(values[0]);
                result.Add(values[1]);
                result.Add(values[2]);
            }
            else if (count == 2)
            {
                result.Add(values[0]);
                result.Add(values[1]);
                result.Add(values[0]);
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

        public List<DelusionData> PickThreeDelusions()
        {
            var values = AllDelusions.Values.ToList();

            values = values.Where(x =>
            {
                bool meetsFloor = FloorCounter >= x.MinFloor;
                int currentCount = DelusionsPicked.Count(c => c.curse == x.curse);
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
            values = values.Where(x => AllDelusionsItems.ContainsKey(x.curse)).ToList();

            // If none are valid, return empty so caller can skip the intermission
            if (values.Count == 0) return new List<DelusionData>();

            List<DelusionData> result = new List<DelusionData>(3);
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



        public List<UpgradeData> PickThreeUpgrades()
        {
            var values = AllUpgrades.Values.ToList();

            values = values.Where(x =>
            {
                bool meetsFloor = FloorCounter >= x.MinFloor;
                int currentCount = UpgradesCount(x.curse);
                bool underMax = currentCount < x.MaxUpgrades;
                bool extrarequirement = false;
                if (x.Requirements != null)
                {
                    extrarequirement = x.Requirements();
                }
                else extrarequirement = true;
                return meetsFloor && underMax && extrarequirement;
            }).ToList();

            // Only consider curses that have a spawnable ItemObject prepared
            values = values.Where(x => AllUpgradesItems.ContainsKey(x.curse)).ToList();

            // If none are valid, return empty so caller can skip the intermission
            if (values.Count == 0) return new List<UpgradeData>();

            List<UpgradeData> result = new List<UpgradeData>(3);
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
            if (scene.levelTitle == "F1" || scene.levelTitle == "F2" || scene.levelTitle == "F3" || scene.levelTitle == "F4" || scene.levelTitle == "F5")
            {
                FloorCounter = 1;
                fakecount = 0;
                flop = false;
                scene.manager = new MainGameManagerBuilder<MainInfiniteGameManager>()
                .SetAllNotebooksSound(Am.Get<SoundObject>("snd_allnb"))
                .Build();
                EventsList.Clear();
                EventsList.Add(Resources.FindObjectsOfTypeAll<RandomEvent>().First(x => x is FogEvent));
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
                    itembudget = 20;
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
                        cuslevob.posters = cuslevob.posters.AddToArray(new()
                        {
                            selection = Am.Get<PosterObject>("pst_poster1"),
                            weight = 65
                        });
                        cuslevob.posters = cuslevob.posters.AddToArray(new()
                        {
                            selection = Am.Get<PosterObject>("pst_poster2"),
                            weight = 75
                        });
                        cuslevob.maxPlots = plotcount + 1;
                        cuslevob.minPlots = plotcount - 1;
                        cuslevob.randomEvents.Clear();
                        foreach (var item in NullscapeInBB.Instance.EventsList)
                        {
                            cuslevob.randomEvents.Add(new()
                            {
                                selection = item,
                                weight = 100
                            });
                        }
                        ForcedStructureDefault = cuslevob.forcedStructures.ToList();
                        List<WeightedGameObject> weighted = new();
                        List<IntVector2> Vec2s = new();
                        foreach (var item in Altars)
                        {
                            weighted.Add(new()
                            {
                                selection = item.Altar.gameObject,
                                weight = item.Weight
                            });
                            Vec2s.Add(new(0, item.Max));
                        }

                        ForcedStructureDefault.Add(new()
                        {
                            prefab = Am.Get<Structure_AltarBuilder>("altar"),
                            parameters = new()
                            {
                                prefab = weighted.ToArray(),
                                minMax = Vec2s.ToArray()
                            }
                        });

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
                        var r = cuslevob.roomGroup.ToList();
                        r.RemoveAll(x => x.name == "LockedRoom");
                        cuslevob.roomGroup = r.ToArray();
                        cuslevob.potentialStructures = new WeightedStructureWithParameters[0];
                        cuslevob.posters = cuslevob.posters.AddToArray(new()
                        {
                            selection = Am.Get<PosterObject>("pst_poster1"),
                            weight = 75
                        });
                        cuslevob.posters = cuslevob.posters.AddToArray(new()
                        {
                            selection = Am.Get<PosterObject>("pst_poster2"),
                            weight = 75
                        });
                    }
                }

                ForcedStructures = new();
                foreach (var item in ForcedStructureDefault)
                {
                    ForcedStructures.Add(item);
                }
                var e = scene.potentialStickers.ToList();
                e.RemoveAll(x => x.selection == Sticker.CashRegister);
                scene.potentialStickers = e.ToArray();
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


                    foreach (var rg in cuslevob.roomGroup)
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
        }

        public void ResetValues()
        {
            notebookcount = 1;
            facultycount = 2;
            mapprice = 100;
            shieldcost = 5000;
            shields = 1;
            plotcount = 3;
            itembudget = 200;
            levelsize = new IntVector2(14, 14);
            FloorCounter = 1;
            fakecount = 0;
            potentialNPCs.Clear();
            selectedNPCCounts.Clear();
            rooms.Clear();
            CursesPicked.Clear();
            GreaterCursesPicked.Clear();
            ForcedStructures = new();
            freeRerolls = 3;
            rerollPrice = 100;
            cursebreakerprice = 5000;
            LastTimeCursebreakerUsed = -99;
            EventsList.Clear();
            EventsList.Add(Resources.FindObjectsOfTypeAll<RandomEvent>().First(x => x is FogEvent));
            QueuedIntermission.Clear();
            YtpMultipliers.Clear();
            UpgradesBought.Clear();
            DelusionsPicked.Clear();
            foreach (var item in Cooldowns.Keys.ToList())
            {
                Cooldowns[item] = 0;
            }
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
                    cuslevob.roomGroup.First(x => x.name == "Class").potentialRooms = rooms.ToArray();
                    var r = cuslevob.roomGroup.ToList();
                    r.RemoveAll(x => x.name == "LockedRoom");
                    cuslevob.roomGroup = r.ToArray();
                    cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = facultycount + 2;
                    cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = facultycount - 2;
                    cuslevob.maxItemValue = itembudget;
                    cuslevob.randomEvents.Clear();
                    cuslevob.randomEvents.Add(new()
                    {
                        selection = Resources.FindObjectsOfTypeAll<RandomEvent>().First(x => x is FogEvent && x.GetInstanceID() > 0),
                        weight = 100
                    });
                    cuslevob.maxPlots = plotcount + 1;
                    cuslevob.minPlots = plotcount - 1;
                    cuslevob.forcedNpcs = potentialNPCs.ToArray();
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
            if (F2 != null)
            {
                F2.additionalNPCs = 0;
                CustomLevelObject[] customLevelObjectsa = F2.GetCustomLevelObjects();

                foreach (CustomLevelObject cuslevob in customLevelObjectsa)
                {
                    cuslevob.maxSize = levelsize;
                    cuslevob.minSize = levelsize;
                    cuslevob.forcedStructures = ForcedStructures.ToArray();
                    cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = notebookcount;
                    cuslevob.roomGroup.First(x => x.name == "Class").minRooms = notebookcount;
                    cuslevob.roomGroup.First(x => x.name == "Class").potentialRooms = rooms.ToArray();
                    var r = cuslevob.roomGroup.ToList();
                    r.RemoveAll(x => x.name == "LockedRoom");
                    cuslevob.roomGroup = r.ToArray();
                    cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = facultycount + 2;
                    cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = facultycount - 2;
                    cuslevob.maxItemValue = itembudget;
                    cuslevob.randomEvents.Clear();
                    cuslevob.randomEvents.Add(new()
                    {
                        selection = Resources.FindObjectsOfTypeAll<RandomEvent>().First(x => x is FogEvent && x.GetInstanceID() > 0),
                        weight = 100
                    });
                    cuslevob.maxPlots = plotcount + 1;
                    cuslevob.minPlots = plotcount - 1;
                    cuslevob.forcedNpcs = potentialNPCs.ToArray();
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

                F2.levelTitle = "F2";
            }
            GenerationStuff.GlobalGenStuff();
            GenerationStuff.GenStuff(F1);
            GenerationStuff.GenStuff(F2);
        }

        private SceneObject DuplicateSceneObject(SceneObject sceneObject)
        {
            return sceneObject == null ? null : UnityEngine.Object.Instantiate(sceneObject);
        }
    }
}
