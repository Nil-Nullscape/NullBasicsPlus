using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    public static partial class StuffCreation
    {
        public static void AddCurses() {
            NullscapeInBB.Instance.AllCurses.Add(Curses.MathMachine, new()
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
                            NullscapeInBB.Instance.rooms.Add(new()
                            {
                                selection = room,
                                weight = 50
                            });
                        }
                    }
                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.GiftActivity, new()
            {
                curse = Curses.GiftActivity,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {
                    foreach (var room in NullscapeInBB.Instance.GiftsActivityRooms)
                    {

                        NullscapeInBB.Instance.rooms.Add(new()
                        {
                            selection = room,
                            weight = 55
                        });

                    }
                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.BalloonBuster, new()
            {
                curse = Curses.BalloonBuster,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {
                    foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
                    {
                        if (room.activity.prefab is BalloonBuster)
                        {
                            NullscapeInBB.Instance.rooms.Add(new()
                            {
                                selection = room,
                                weight = 50
                            });
                        }
                    }
                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.MatchMachine, new()
            {
                curse = Curses.MatchMachine,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {
                    foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
                    {
                        if (room.activity.prefab is MatchActivity)
                        {
                            NullscapeInBB.Instance.rooms.Add(new()
                            {
                                selection = room,
                                weight = 50
                            });
                        }
                    }
                }
            });

            NullscapeInBB.Instance.AllCurses.Add(Curses.AngrierBaldi, new()
            {
                curse = Curses.AngrierBaldi,
                MaxCurses = 67000000,
                MinFloor = int.MaxValue - 1,
                CursesAction = () =>
                {

                }
            });

            NullscapeInBB.Instance.AllCurses.Add(Curses.OneMinutePomp, new()
            {
                curse = Curses.OneMinutePomp,
                MaxCurses = 1,
                MinFloor = 0,
                CurseValue = 25,
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(Character.Pomp) >= 1;
                },
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.Lap2, new()
            {
                curse = Curses.Lap2,
                MaxCurses = 1,
                MinFloor = 8,
                CurseValue = 150,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.BiggerBlast, new()
            {
                curse = Curses.BiggerBlast,
                MaxCurses = 2,
                MinFloor = 0,
                CurseValue = 40,
                Requirements = () => {
                    return NullscapeInBB.EnemyCount(NullscapeInBB.Am.Get<NPC>("npc_ICBM").Character) >= 1;
                },
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.OneMoreProblem, new()
            {
                curse = Curses.OneMoreProblem,
                MaxCurses = 4,
                MinFloor = 0,
                CurseValue = 30,
                Requirements = () =>
                {
                    return NullscapeInBB.CurseCount(Curses.MathMachine) >= 1;
                },
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.FasterTimedActivities, new()
            {
                curse = Curses.FasterTimedActivities,
                MaxCurses = 3,
                MinFloor = 0,
                CurseValue = 30,
                Requirements = () =>
                {
                    return NullscapeInBB.GreaterCurseCount(GreaterCurses.TimedActivities) >= 1;
                },
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.FasterTurningFirstPrize, new()
            {
                curse = Curses.FasterTurningFirstPrize,
                MaxCurses = 3,
                MinFloor = 0,
                CurseValue = 30,
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(Character.Prize) >= 1;
                },
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.TooJealousArts, new()
            {
                curse = Curses.TooJealousArts,
                MaxCurses = 1,
                MinFloor = 9,
                CurseValue = 0,
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(Character.Crafters) >= 1;

                },
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.FasterEnviromnent, new()
            {
                curse = Curses.FasterEnviromnent,
                MaxCurses = 3,
                MinFloor = 5,
                CurseValue = 20,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.FasterNPCs, new()
            {
                curse = Curses.FasterNPCs,
                MaxCurses = 3,
                MinFloor = 5,
                CurseValue = 20,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.ThreeStrikesPrincipal, new()
            {
                curse = Curses.ThreeStrikesPrincipal,
                MaxCurses = 1,
                MinFloor = 5,
                CurseValue = 0,
                CursesAction = () =>
                {

                },
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(Character.Principal) >= 1;

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.RandomTilesAreSilenced, new()
            {
                curse = Curses.RandomTilesAreSilenced,
                MaxCurses = 3,
                MinFloor = 5,
                CurseValue = 0,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.OldChalkface, new()
            {
                curse = Curses.OldChalkface,
                MaxCurses = 1,
                MinFloor = 5000000,
                CurseValue = 0,
                CursesAction = () =>
                {

                },
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(Character.Chalkles) >= 1;

                }
            });
            NullscapeInBB.Instance.AllCurses.Add(Curses.CadenceOneInstrument, new()
            {
                curse = Curses.CadenceOneInstrument,
                MaxCurses = 1,
                MinFloor = 5,
                CurseValue = 0,
                Icon = Resources.FindObjectsOfTypeAll<Sprite>().First(),
                CursesAction = () =>
                {

                },
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(NullscapeInBB.Am.Get<CadenceNPC>("npc_Cadence").Character) >= 1;

                }
            });
        }
    }
}
