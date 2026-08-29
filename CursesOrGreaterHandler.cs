using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnvironmentController), "SpawnNPCs")]
        static void Nothingtnogitno(EnvironmentController __instance) {
            if (Singleton<BaseGameManager>.Instance is MainInfiniteGameManager)
            {
                if (NullscapeInBB.GreaterCurseCount(GreaterCurses.Nothing) >= 1) {
                    var numberOfNpcs = Mathf.FloorToInt(NullscapeInBB.Instance.FloorCounter / 6f);
                    for (int i = 0; i < numberOfNpcs; i++)
                    {
                        var randomCell = __instance.RandomCell(false, false, true);
                        var distance = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - randomCell.CenterWorldPosition).magnitude;
                        while (distance < 120)
                        {
                            randomCell = __instance.RandomCell(false, false, true);
                            distance = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - randomCell.CenterWorldPosition).magnitude;
                        }
                        var allNpcsL = NullscapeInBB.Instance.AllEnemies.Values.ToList();
                        allNpcsL.RemoveAll(x => x.Enemy == EnumExtensions.GetFromExtendedName<Character>("???"));
                        var allNpcs = allNpcsL.ToArray();
                        var randomNPC = allNpcs[UnityEngine.Random.Range(0, allNpcs.Length)].NPCToUse;
                        __instance.SpawnNPC(randomNPC, randomCell.position);
                    }
                }
            }
        }
    }
}
