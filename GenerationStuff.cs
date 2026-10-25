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
                cuslevob.roomGroup.First(x => x.name == "Class").maxRooms = main.notebookcount;
                cuslevob.roomGroup.First(x => x.name == "Class").minRooms = main.notebookcount;
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


        public static void GlobalGenStuff()
        {
            NullscapeInBB main = NullscapeInBB.Instance;

            if (main.FloorCounter % 2 == 0 && main.notebookcount >= 7)
            {
                main.notebookcount++;
                main.mapprice += 50;
            }
            else if (main.notebookcount < 7)
            {
                main.notebookcount += 2;
                main.mapprice += 25;
                main.levelsize += new IntVector2(1, 2);
            }
            if (main.FloorCounter % 2 == 0)
            {
                if (!(((IntVector2)(main.levelsize + new IntVector2(1, 1))).x > 225))
                {
                    main.levelsize += new IntVector2(1, 1);
                    main.facultycount++;
                    main.itembudget += 20;
                    main.mapprice += 50;
                }
            }
            if (main.FloorCounter % 3 == 0)
            {
                main.plotcount++;
            }
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

