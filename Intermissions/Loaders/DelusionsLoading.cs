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
        }
    }
}
