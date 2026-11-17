using NullscapeinBB.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    public static partial class StuffCreation
    {
        public static void AddDelusions() {
            NullscapeInBB.Instance.AllDelusions.Add(Delusions.baldihappybutapplebye, new()
            {
                curse = Delusions.baldihappybutapplebye,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () => { }
            });
            NullscapeInBB.Instance.AllDelusions.Add(Delusions.MoreShieldButLessShield, new()
            {
                curse = Delusions.MoreShieldButLessShield,
                MaxCurses = 1,
                MinFloor = 0,
                ExecuteActionOnLoad = false,
                CursesAction = () => {
                    NullscapeInBB.Instance.shields += 2;
                }
            });

            NullscapeInBB.Instance.AllDelusions.Add(Delusions.MoreplaytimeYTPSButForcedPlaytimeMinigame, new()
            {
                curse = Delusions.MoreplaytimeYTPSButForcedPlaytimeMinigame,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () => {
                }
            });
            NullscapeInBB.Instance.AllDelusions.Add(Delusions.ForcedLap2ButMoreYTPs, new()
            {
                curse = Delusions.ForcedLap2ButMoreYTPs,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () => {
                },
                Requirements = () => {
                    return NullscapeInBB.CurseCount(Curses.Lap2) >= 1 && SaveHelper.IsAchivementCompleted(SaveHelper.AchivementsNullBasics.Lap3);
                }
            });
            NullscapeInBB.Instance.AllDelusions.Add(Delusions.StackableUpgradesOneMoreStackButRandomizedChoices, new()
            {
                curse = Delusions.StackableUpgradesOneMoreStackButRandomizedChoices,
                MaxCurses = 1,
                MinFloor = 0,
                CursesAction = () => {
                }
            });
        }
    }
}
