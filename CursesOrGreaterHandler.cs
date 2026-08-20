using HarmonyLib;
using MTM101BaldAPI.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NullscapeinBB
{
[HarmonyPatch]
    internal class CursesOrGreaterHandler
    {
        [HarmonyPatch(typeof(Playtime),"Initialize")]
        [HarmonyPostfix]
        static void MoreJumpsYeyyyy(Playtime __instance) {
            Jumprope pre = (Jumprope)__instance.ReflectionGetVariable("jumpropePre");

            if (NullscapeInBB.GreaterCurseCount(GreaterCurses.HarderPlaytime) >= 1) {
                pre.ReflectionSetVariable("maxJumps", 10);
            }
        }

        [HarmonyPatch(typeof(MathMachine), "Start")]
        [HarmonyPrefix]
        static void MoreProblemsYeyyyy(MathMachine __instance)
        {
            if (NullscapeInBB.CurseCount(Curses.OneMoreProblem) >= 1)
            {
                __instance.ReflectionSetVariable("totalProblems", 1 + NullscapeInBB.CurseCount(Curses.OneMoreProblem));
            }
        }

        [HarmonyPatch(typeof(BaseGameManager), "PleaseBaldi")]
        [HarmonyPrefix]
        static bool ByebyePraising(BaseGameManager __instance)
        {
            if (NullscapeInBB.GreaterCurseCount(GreaterCurses.NoMorePraise) >= 1)
            {
                return false;
            } else {
                return true;
            }
        }
    }
}
