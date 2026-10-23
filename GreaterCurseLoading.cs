using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    public static partial class StuffCreation
    {
        public static void AddGreaterCurses() {
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.OmniscientBaldi, new()
            {
                curse = GreaterCurses.OmniscientBaldi,
                MaxCurses = 1,
                MinFloor = 19,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.LessShield, new()
            {
                curse = GreaterCurses.LessShield,
                MaxCurses = 1,
                MinFloor = 19,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.SilentNpcs, new()
            {
                curse = GreaterCurses.SilentNpcs,
                MaxCurses = 1,
                MinFloor = 19,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.BaldiAppleEatFaster, new()
            {
                curse = GreaterCurses.BaldiAppleEatFaster,
                MaxCurses = 1,
                MinFloor = 9,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.byebyestickystickers, new()
            {
                curse = GreaterCurses.byebyestickystickers,
                MaxCurses = 1,
                MinFloor = 9,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.HarderPlaytime, new()
            {
                curse = GreaterCurses.HarderPlaytime,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {

                },
                Requirements = () =>
                {
                    return NullscapeInBB.EnemyCount(Character.Playtime) >= 1;
                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.TimedActivities, new()
            {
                curse = GreaterCurses.TimedActivities,
                Requirements = () =>
                {
                    return NullscapeInBB.CurseCount(Curses.MathMachine) >= 1 || NullscapeInBB.CurseCount(Curses.MatchMachine) >= 1 || NullscapeInBB.CurseCount(Curses.BalloonBuster) >= 1;
                },
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {

                }
            });

            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.NoMorePraise, new()
            {
                curse = GreaterCurses.NoMorePraise,
                Requirements = () =>
                {
                    return NullscapeInBB.CurseCount(Curses.MathMachine) >= 1 || NullscapeInBB.CurseCount(Curses.MatchMachine) >= 1 || NullscapeInBB.CurseCount(Curses.BalloonBuster) >= 1;
                },
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.TwiceMoreNpcs, new()
            {
                curse = GreaterCurses.TwiceMoreNpcs,
                MaxCurses = 1,
                MinFloor = 19,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.Nothing, new()
            {
                curse = GreaterCurses.Nothing,
                MaxCurses = 1,
                MinFloor = 24,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.Tantrum, new()
            {
                curse = GreaterCurses.Tantrum,
                MaxCurses = 1,
                MinFloor = 2500,
                CursesAction = () =>
                {

                }
            });
            NullscapeInBB.Instance.AllGreaterCurses.Add(GreaterCurses.Maintenance, new()
            {
                curse = GreaterCurses.Maintenance,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () =>
                {

                    Debug.Log("Maintenance test");
                    NullscapeInBB.Instance.ForcedStructures.Add(new()
                    {
                        prefab = Resources.FindObjectsOfTypeAll<Structure_PowerLever>().First(),
                        parameters = new()
                        {
                            chance = [1, 1],
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
        }
    }
}
