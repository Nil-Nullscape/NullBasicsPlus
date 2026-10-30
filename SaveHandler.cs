using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json;
using NullscapeinBB.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.SaveHandlerStuff
{
    [System.Serializable]
    public sealed class DictionaryToClass<K, V> {
        public K[] Key;
        public V[] Value;

        public Dictionary<K, V> ConvertToDictionary()
        {
            var dict = new Dictionary<K, V>();
            if (Key == null || Value == null) return dict;

            for (int i = 0; i < Key.Length; i++)
            {
                if (i < Value.Length)
                {
                    dict.Add(Key[i], Value[i]);
                }
            }
            return dict;
        }

        public DictionaryToClass(Dictionary<K, V> dict)
        {
            // SAFETY CHECK: If the dictionary is null, initialize empty arrays and exit
            if (dict == null || dict.Count == 0)
            {
                Key = new K[0];
                Value = new V[0];
                return;
            }

            // Pre-allocate array sizes safely
            Key = new K[dict.Count];
            Value = new V[dict.Count];

            int i = 0;
            foreach (var item in dict)
            {
                Key[i] = item.Key;
                Value[i] = item.Value;
                i++;
            }
        }


        public static implicit operator DictionaryToClass<K, V>(Dictionary<K, V> dict) => dict.ConvertToDictionaryClass();
        public static implicit operator Dictionary<K, V>(DictionaryToClass<K, V> dict) => dict.ConvertToDictionary();

    }

    public static class DictionaryToClassExtension {
        public static DictionaryToClass<K,V> ConvertToDictionaryClass<K,V>(this Dictionary<K,V> dict) {
            return new DictionaryToClass<K, V>(dict);
        }
    }
    public static class SaveClassExtensions {
        public static List<RandomEventType> RandomEventToRandomEventType(this List<RandomEvent> RandomEvents) {
            var lst = new List<RandomEventType>();

            foreach (var item in RandomEvents)
            {
                lst.Add(item.Type);
            }
            return lst;
        }

        public static List<RandomEvent> RandomEventTypeToRandomEvent(this List<RandomEventType> RandomEvents) {
            var lst = new List<RandomEvent>();
            var randomEvents = Resources.FindObjectsOfTypeAll<RandomEvent>();
            foreach (var item in RandomEvents)
            {
                lst.Add(randomEvents.First(x => x.Type == item));
            }
            return lst;
        }

        public static List<int> CurseDataToCurses<FROM>(this List<FROM> curses) 
        {
            var lst = new List<int>();

            foreach (var item in curses)
            {
                if (item is CurseData data) lst.Add((int)data.curse);
                if (item is GreaterCurseData data2) lst.Add((int)data2.curse);
                if (item is UpgradeData data3) lst.Add((int)data3.curse);
                if (item is DelusionData data4) lst.Add((int)data4.curse);


            }
            return lst;
        }

        public static List<CurseData> IntListToCurseData(this List<int> list)
        {
            var NewList = new List<CurseData>();
            foreach (var item in list)
            {
                NewList.Add(NullscapeInBB.Instance.AllCurses[(Curses)item]);
            }
            return NewList;

        }
        public static List<GreaterCurseData> IntListToGreaterCurseData(this List<int> list)
        {
            var NewList = new List<GreaterCurseData>();
            foreach (var item in list)
            {
                NewList.Add(NullscapeInBB.Instance.AllGreaterCurses[(GreaterCurses)item]);
            }
            return NewList;

        }
        public static List<UpgradeData> IntListToUpgradeData(this List<int> list)
        {
            var NewList = new List<UpgradeData>();
            foreach (var item in list)
            {
                NewList.Add(NullscapeInBB.Instance.AllUpgrades[(Upgrades)item]);
            }
            return NewList;

        }
        public static List<DelusionData> IntListToDelusionData(this List<int> list)
        {
            var NewList = new List<DelusionData>();
            foreach (var item in list)
            {
                NewList.Add(NullscapeInBB.Instance.AllDelusions[(Delusions)item]);
            }
            return NewList;

        }





    }

    [System.Serializable]
    public class SaveClass {
        public int Shields = 1;
        public int ShieldCost = 5000;
        public int floorCounter = 1;
        public int freeRerolls = 3;
        public int cursebreakerprice = 5000;
        public int LastTimeCursebreakerUsed = -99;
        public int itembudget;
        public int facultycount;
        public int rerollPrice = 100;
        public int notebookcount;
        public IntVector2 levelsize;
        public int plotcount;
        public int mapprice;
        public DictionaryToClass<Character, int> selectedNPCCounts;
        public DictionaryToClass<AltarTypes, int> Cooldowns;

        public List<RandomEventType> EventsList = new();
        public List<int> CursesPicked = new();

        public List<int> GreaterCursesPicked = new();
        public List<int> DelusionsPicked = new();
        public List<int> UpgradesBought = new();

        public Classes SelectedClass = Classes.Student;

    }
    internal class SaveHandler : ModdedSaveGameIOText
    {
        public override PluginInfo pluginInfo => NullscapeInBB.Instance.Info;
        #region OLD
        /*
        public SaveClass GetObjectToSave()
        {
            return new()
            {
                notebookcount = NullscapeInBB.Instance.notebookcount,
                DelusionsPicked = NullscapeInBB.Instance.DelusionsPicked,
                Cooldowns = NullscapeInBB.Instance.Cooldowns,
                cursebreakerprice = NullscapeInBB.Instance.cursebreakerprice,
                CursesPicked = NullscapeInBB.Instance.CursesPicked,
                EventsList = NullscapeInBB.Instance.EventsList.RandomEventToRandomEventType(),
                facultycount = NullscapeInBB.Instance.facultycount,
                floorCounter = NullscapeInBB.Instance.FloorCounter,
                freeRerolls = NullscapeInBB.Instance.freeRerolls,
                GreaterCursesPicked = NullscapeInBB.Instance.GreaterCursesPicked,
                itembudget = NullscapeInBB.Instance.itembudget,
                LastTimeCursebreakerUsed = NullscapeInBB.Instance.LastTimeCursebreakerUsed,
                levelsize = NullscapeInBB.Instance.levelsize,
                mapprice = NullscapeInBB.Instance.mapprice,
                plotcount = NullscapeInBB.Instance.plotcount,
                rerollPrice = NullscapeInBB.Instance.rerollPrice,
                ShieldCost = NullscapeInBB.Instance.shieldcost,
                Shields = NullscapeInBB.Instance.shields,
                UpgradesBought = NullscapeInBB.Instance.UpgradesBought,
                selectedNPCCounts = NullscapeInBB.Instance.selectedNPCCounts
            };
        }
        
        public void OnObjectLoaded(SaveClass loadedObject)
        {
            NullscapeInBB.Instance.notebookcount = loadedObject.notebookcount;
            NullscapeInBB.Instance.DelusionsPicked = loadedObject.DelusionsPicked;
            NullscapeInBB.Instance.Cooldowns = loadedObject.Cooldowns;
            NullscapeInBB.Instance.cursebreakerprice = loadedObject.cursebreakerprice;
            NullscapeInBB.Instance.CursesPicked = loadedObject.CursesPicked;
            NullscapeInBB.Instance.EventsList = loadedObject.EventsList.RandomEventTypeToRandomEvent();
            NullscapeInBB.Instance.facultycount = loadedObject.facultycount;
            NullscapeInBB.Instance.FloorCounter = loadedObject.floorCounter;
            NullscapeInBB.Instance.freeRerolls = loadedObject.freeRerolls;
            NullscapeInBB.Instance.GreaterCursesPicked = loadedObject.GreaterCursesPicked;
            NullscapeInBB.Instance.itembudget = loadedObject.itembudget;
            NullscapeInBB.Instance.LastTimeCursebreakerUsed = loadedObject.LastTimeCursebreakerUsed;
            NullscapeInBB.Instance.levelsize = loadedObject.levelsize;
            NullscapeInBB.Instance.mapprice = loadedObject.mapprice;
            NullscapeInBB.Instance.plotcount = loadedObject.plotcount;
            NullscapeInBB.Instance.rerollPrice = loadedObject.rerollPrice;
            NullscapeInBB.Instance.shieldcost = loadedObject.ShieldCost;
            NullscapeInBB.Instance.shields = loadedObject.Shields;
            NullscapeInBB.Instance.UpgradesBought = loadedObject.UpgradesBought;
            NullscapeInBB.Instance.selectedNPCCounts = loadedObject.selectedNPCCounts;
            NullscapeInBB.Instance.flop = !Convert.ToBoolean(loadedObject.floorCounter % 2);
        } */
        #endregion
        public override string DisplayTags(string[] tags)
        {
            bool chaosMode = false;
            bool NullscapeEntities = true;
            if (tags.Contains("Nullbasics_ChaosMode")) chaosMode = true;
            if (tags.Contains("Nullbasics_DisabledEntities")) NullscapeEntities = false;
            var text = "";
            if (chaosMode) text += "Chaos mode enabled\n";
            if (NullscapeEntities) text += "Nullscape entities Enabled";
            else text += "Nullscape entities Disabled";
            return text;


        }
        public override string[] GenerateTags()
        {
            List<string> tags = new();
            if (NullscapeInBB.Instance.EnableChaosMode.Value) tags.Add("Nullbasics_ChaosMode");
            if (!NullscapeInBB.Instance.EnabledNullscapeEntities.Value) tags.Add("Nullbasics_DisabledEntities");
            return tags.ToArray();
        }
        public override void Reset()
        {
            NullscapeInBB.Instance.ResetValues();
            GenerationStuff.GenStuff(NullscapeInBB.Instance.F1);
            GenerationStuff.GenStuff(NullscapeInBB.Instance.F2);
            GenerationStuff.GlobalGenStuff();
        }
        
        public override string SaveText()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                // Ignore circular references instead of throwing an error
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

                // Allow non-string dictionary keys (serializes object keys properly)
                MetadataPropertyHandling = MetadataPropertyHandling.Default,

                // Optional: pretty print for easy debugging
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(new SaveClass()
            {
                notebookcount = NullscapeInBB.Instance.notebookcount,
                DelusionsPicked = NullscapeInBB.Instance.DelusionsPicked.CurseDataToCurses(),
                Cooldowns = NullscapeInBB.Instance.Cooldowns,
                cursebreakerprice = NullscapeInBB.Instance.cursebreakerprice,
                CursesPicked = NullscapeInBB.Instance.CursesPicked.CurseDataToCurses(),
                EventsList = NullscapeInBB.Instance.EventsList.RandomEventToRandomEventType(),
                facultycount = NullscapeInBB.Instance.facultycount,
                floorCounter = NullscapeInBB.Instance.FloorCounter,
                freeRerolls = NullscapeInBB.Instance.freeRerolls,
                GreaterCursesPicked = NullscapeInBB.Instance.GreaterCursesPicked.CurseDataToCurses(),
                itembudget = NullscapeInBB.Instance.itembudget,
                LastTimeCursebreakerUsed = NullscapeInBB.Instance.LastTimeCursebreakerUsed,
                levelsize = NullscapeInBB.Instance.levelsize,
                mapprice = NullscapeInBB.Instance.mapprice,
                plotcount = NullscapeInBB.Instance.plotcount,
                rerollPrice = NullscapeInBB.Instance.rerollPrice,
                ShieldCost = NullscapeInBB.Instance.shieldcost,
                Shields = NullscapeInBB.Instance.shields,
                UpgradesBought = NullscapeInBB.Instance.UpgradesBought.CurseDataToCurses(),
                selectedNPCCounts = NullscapeInBB.Instance.selectedNPCCounts,
                SelectedClass = NullscapeInBB.Instance.SelectedClass
            },settings);
        }
        public override void OnCGMCreated(CoreGameManager instance, bool isFromSavedGame)
        {
            if (isFromSavedGame) {
                Debug.Log("Reexecuting all curses,greater curses, upgrades and delusions");
                foreach (var item in NullscapeInBB.Instance.CursesPicked)
                {
                    if (item.CursesAction != null) item.CursesAction();
                }
                foreach (var item in NullscapeInBB.Instance.GreaterCursesPicked)
                {
                    if (item.CursesAction != null) item.CursesAction();
                }
                foreach (var item in NullscapeInBB.Instance.DelusionsPicked)
                {
                    if (item.CursesAction != null) item.CursesAction();
                }
                foreach (var item in NullscapeInBB.Instance.UpgradesBought)
                {
                    if (item.OnUpgradeBought != null) item.OnUpgradeBought();
                }
            } else {
                NullscapeInBB.Instance.ResetValues();
                GenerationStuff.GenStuff(NullscapeInBB.Instance.F1);
                GenerationStuff.GenStuff(NullscapeInBB.Instance.F2);
                GenerationStuff.GlobalGenStuff();
            }
        }

        

        public override void LoadText(string toLoad)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                // Ignore circular references instead of throwing an error
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

                // Allow non-string dictionary keys (serializes object keys properly)
                MetadataPropertyHandling = MetadataPropertyHandling.Default,

                // Optional: pretty print for easy debugging
                Formatting = Formatting.Indented
            };
            var loadedObject = JsonConvert.DeserializeObject<SaveClass>(toLoad,settings);
            NullscapeInBB.Instance.notebookcount = loadedObject.notebookcount;
            NullscapeInBB.Instance.DelusionsPicked = loadedObject.DelusionsPicked.IntListToDelusionData();
            NullscapeInBB.Instance.Cooldowns = loadedObject.Cooldowns;
            NullscapeInBB.Instance.cursebreakerprice = loadedObject.cursebreakerprice;
            NullscapeInBB.Instance.CursesPicked = loadedObject.CursesPicked.IntListToCurseData();
            NullscapeInBB.Instance.EventsList = loadedObject.EventsList.RandomEventTypeToRandomEvent();
            NullscapeInBB.Instance.facultycount = loadedObject.facultycount;
            NullscapeInBB.Instance.FloorCounter = loadedObject.floorCounter;
            NullscapeInBB.Instance.freeRerolls = loadedObject.freeRerolls;
            NullscapeInBB.Instance.GreaterCursesPicked = loadedObject.GreaterCursesPicked.IntListToGreaterCurseData();
            NullscapeInBB.Instance.itembudget = loadedObject.itembudget;
            NullscapeInBB.Instance.LastTimeCursebreakerUsed = loadedObject.LastTimeCursebreakerUsed;
            NullscapeInBB.Instance.levelsize = loadedObject.levelsize;
            NullscapeInBB.Instance.mapprice = loadedObject.mapprice;
            NullscapeInBB.Instance.plotcount = loadedObject.plotcount;
            NullscapeInBB.Instance.rerollPrice = loadedObject.rerollPrice;
            NullscapeInBB.Instance.shieldcost = loadedObject.ShieldCost;
            NullscapeInBB.Instance.shields = loadedObject.Shields;
            NullscapeInBB.Instance.UpgradesBought = loadedObject.UpgradesBought.IntListToUpgradeData();
            NullscapeInBB.Instance.selectedNPCCounts = loadedObject.selectedNPCCounts;
            NullscapeInBB.Instance.flop = !Convert.ToBoolean(loadedObject.floorCounter % 2);
            NullscapeInBB.Instance.SelectedClass = loadedObject.SelectedClass;

            GenerationStuff.GenStuff(NullscapeInBB.Instance.F1);
            GenerationStuff.GenStuff(NullscapeInBB.Instance.F2);
            GenerationStuff.GlobalGenStuff();

        }
    }
}
