using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB
{
    public static class GenerationStuff
    {
        public static void GenStuff(SceneObject toModify)
        {
            Debug.Log($"Gen stuff for {toModify.levelTitle}");
            NullscapeInBB main = NullscapeInBB.Instance;



            toModify.previousLevels = [];






            toModify.forcedNpcs = main.potentialNPCs.ToArray();
            
            toModify.mapPrice = main.mapprice;
            toModify.levelTitle = "F" + main.FloorCounter;
            CustomLevelObject[] customLevelObjects = toModify.GetCustomLevelObjects();
            foreach (CustomLevelObject cuslevob in customLevelObjects)
            {
                cuslevob.maxSize = main.levelsize;
                cuslevob.minSize = main.levelsize;
                cuslevob.forcedStructures = main.ForcedStructures.ToArray();
                cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = main.notebookcount + 1 ;
                cuslevob.roomGroup.First(x => x.name == "Class").minRooms = Mathf.Clamp(main.notebookcount - 1,1,999);
                cuslevob.roomGroup.First(x => x.name == "Class").potentialRooms = main.rooms.ToArray();
                cuslevob.roomGroup.First(x => x.name == "Faculty").maxRooms = main.facultycount + 2;
                cuslevob.roomGroup.First(x => x.name == "Faculty").minRooms = main.facultycount - 2;
                cuslevob.maxItemValue = main.itembudget;
                cuslevob.maxPlots = main.plotcount + 1;
                cuslevob.minPlots = main.plotcount - 1;
                cuslevob.randomEvents.Clear();
                foreach (var item in NullscapeInBB.Instance.EventsList)
                {
                    cuslevob.randomEvents.Add(new()
                    {
                        selection = item,
                        weight = 100
                    });
                }

                if (main.FloorCounter >= 30)
                {
                    cuslevob.exitCount = 4;
                }
                else if (main.FloorCounter >= 20)
                {
                    cuslevob.exitCount = 3;
                }
                else if (main.FloorCounter >= 10)
                {
                    cuslevob.exitCount = 2;
                }
                else
                {
                    cuslevob.exitCount = 1;
                }

            }
        }

        public static int GetNumberOfNotebooks() {
            NullscapeInBB main = NullscapeInBB.Instance;
            if (main.FloorCounter <= 4) {
                return 1 + (main.FloorCounter - 1) * 2;
            } else {
                return 7 + Mathf.FloorToInt((main.FloorCounter - 4) / 2.5f);
            }
        }
        public static int GetNumberOfFaculty() {
            NullscapeInBB main = NullscapeInBB.Instance;
            return Mathf.Clamp(Mathf.FloorToInt(1 * (main.FloorCounter - (main.FloorCounter*0.60f))),1,30);
        }
        public static int GetItemBudget()
        {
            NullscapeInBB main = NullscapeInBB.Instance;
            return 40 + Mathf.RoundToInt((main.FloorCounter * (Mathf.PI * GetNumberOfNotebooks())) * 0.436661f);
        }
        public static int GetPlots() {
            NullscapeInBB main = NullscapeInBB.Instance;
            return 4+ Mathf.FloorToInt(main.FloorCounter / 2f);
        }
        public static IntVector2 GetLevelSize() {
            NullscapeInBB main = NullscapeInBB.Instance;
            var num = Mathf.Clamp((7 + Mathf.FloorToInt(main.FloorCounter / 4.5f)) * Mathf.FloorToInt(main.FloorCounter / (3.1415f + main.FloorCounter / 5.2f)  ),0,239);
            return  new(14 + num, 14 + num);
        }

        public static void GlobalGenStuff()
        {
            Debug.Log("Global gen Executed");
            NullscapeInBB main = NullscapeInBB.Instance;
            main.notebookcount = GetNumberOfNotebooks();
            main.levelsize = GetLevelSize();
            main.facultycount = GetNumberOfFaculty();
            main.itembudget = GetItemBudget();
            main.plotcount = GetPlots();
         
           
            if (main.FloorCounter % 5 == 0)
            {
                main.ForcedStructures.Add(new()
                {
                    parameters = new()
                    {
                        minMax = [new(1, 1)]
                    },
                    prefab = Resources.FindObjectsOfTypeAll<Structure_StudentSpawner>().First()
                });

                foreach (var item in main.F1.GetCustomLevelObjects())
                {
                    if (item.roomGroup.FirstOrDefault(x => x.name == "LockedRoom") == null)
                    {
                        item.roomGroup = item.roomGroup.AddToArray(new()
                        {
                            name = "LockedRoom",
                            ceilingTexture = [
                                new() {
                                        selection = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "CeilingNoLight"),
                                            weight = 500
                                        }
                            ],
                            wallTexture = [
                                new() {
                                        selection = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "SaloonWall"),
                                            weight = 500
                                        }
                            ],
                            floorTexture = [
                                new() {
                                        selection = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "Carpet"),
                                            weight = 500
                                        }
                            ],
                            light = [
                                new() {
                                            selection = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "FluorescentLight").transform,
                                            weight = 500
                                        }
                            ],
                            maxRooms = 1,
                            minRooms = 1,
                            potentialRooms = [
                                new() {
                                            selection = Resources.FindObjectsOfTypeAll<RoomAsset>().First(x => ((UnityEngine.Object)x).name == "Room_Faculty_School_Locked_0_HighEndItems"),
                                            weight = 100
                                        }
                            ]
                        });
                    }
                    else
                    {
                        item.roomGroup.First(x => x.name == "LockedRoom").maxRooms += 1;
                    }
                }
                foreach (var itemm in main.F2.GetCustomLevelObjects())
                {
                    if (itemm.roomGroup.FirstOrDefault(x => x.name == "LockedRoom") == null)
                    {
                        itemm.roomGroup = itemm.roomGroup.AddToArray(new()
                        {
                            name = "LockedRoom",
                            ceilingTexture = [
                                new() {
                                        selection = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "CeilingNoLight"),
                                            weight = 500
                                        }
                            ],
                            wallTexture = [
                                new() {
                                        selection = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "SaloonWall"),
                                            weight = 500
                                        }
                            ],
                            floorTexture = [
                                new() {
                                        selection = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "Carpet"),
                                            weight = 500
                                        }
                            ],
                            light = [
                                new() {
                                            selection = Resources.FindObjectsOfTypeAll<Transform>().First(x => x.name == "FluorescentLight"),
                                            weight = 500
                                        }
                            ],
                            maxRooms = 1,
                            minRooms = 1,
                            potentialRooms = [
                                new() {
                                            selection = Resources.FindObjectsOfTypeAll<RoomAsset>().First(x => ((UnityEngine.Object)x).name == "Room_Faculty_School_Locked_0_HighEndItems"),
                                            weight = 100
                                        }
                            ]
                        });
                    }
                    else
                    {
                        itemm.roomGroup.First(x => x.name == "LockedRoom").maxRooms += 1;
                    }
                }
            }
        }
    }
}

