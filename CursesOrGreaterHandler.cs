using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using NilLib;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        static bool ByebyePraising(BaseGameManager __instance, float time)
        {
            if (NullscapeInBB.GreaterCurseCount(GreaterCurses.NoMorePraise) >= 1)
            {
                if (__instance is MainInfiniteGameManager && NullscapeInBB.DelusionsCount(Delusions.baldihappybutapplebye) >= 1) __instance.ConvertType<MainInfiniteGameManager>().BetterPleaseBaldi(2);
            } else {
                if (__instance is MainInfiniteGameManager) __instance.ConvertType<MainInfiniteGameManager>().BetterPleaseBaldi(time + NullscapeInBB.DelusionsCount(Delusions.baldihappybutapplebye) * 5f);
            }
            return false;
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
                if (NullscapeInBB.Instance.EnableQuickStart.Value && NullscapeInBB.Instance.FloorCounter > 0 && NullscapeInBB.Instance.FloorCounter < 20) {
                    var numberOfNpcs = 2;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FirstPrize), "Initialize")]
        static void TurnTURN(FirstPrize __instance)
        {
            __instance.turnSpeed = 22.5f * (NullscapeInBB.CurseCount(Curses.FasterTurningFirstPrize) + 1);
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Baldi), "Initialize")]
        static void TurnTURN(Baldi __instance)
        {
            __instance.appleTime = 10f / (NullscapeInBB.GreaterCurseCount(GreaterCurses.BaldiAppleEatFaster) * 2 + 1);
            if (NullscapeInBB.DelusionsCount(Delusions.baldihappybutapplebye) >= 1) __instance.appleTime = 0; 
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ArtsAndCrafters), "Jealous", MethodType.Getter)]
        static bool angy(ArtsAndCrafters __instance, ref bool __result)
        {
            if (NullscapeInBB.CurseCount(Curses.TooJealousArts) >= 1)
            {
                __result = Singleton<BaseGameManager>.Instance.FoundNotebooks >= Singleton<BaseGameManager>.Instance.NotebookTotal - 3;
            } else {
                return true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NPC), "VirtualUpdate")]
        static void byebyedaudio(NPC __instance)
        {
            if (NullscapeInBB.GreaterCurseCount(GreaterCurses.SilentNpcs) >= 1)
            {
                foreach (var item in __instance.Entity.propagatedAudioManager)
                {
                    item.volumeModifier = -100f;
                    item.FlushQueue(true);

                }

            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CoreGameManager), "AddPoints",[typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(bool)])]
        static void AddPointsGreed(CoreGameManager __instance, int points, bool multiply)
        {
            
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemManager), "UpdateTargetInventorySize")]
        public static bool rewriteOfInventoryUpdat(ItemManager __instance) {
            __instance.SetCachedReflection("targetInventorySize", __instance.defaultInventorySize + NullscapeInBB.UpgradesCount(Upgrades.PlusOneInventorySlot));
            if (NullscapeInBB.Instance.SelectedClass == Classes.Prisoner) __instance.SetCachedReflection("targetInventorySize", 3);
            __instance.SetCachedReflection("maxItem",Mathf.Max(__instance.GetCachedReflection<int>("targetInventorySize"), Mathf.Max(__instance.TotalItemsInInventory, __instance.MaxSlotWithItem + 1)) - 1);
            Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).UpdateInventorySize(Mathf.Max(__instance.GetCachedReflection<int>("targetInventorySize"), Mathf.Max(__instance.TotalItemsInInventory, __instance.MaxSlotWithItem + 1)));
            if (__instance.selectedItem > __instance.maxItem)
            {
                __instance.selectedItem = __instance.maxItem;
            }

            __instance.UpdateSelect();
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Principal), "SendToDetention")]
        static void EvilParenImeanPrincipals(Principal __instance, bool validCollision) {
            if (NullscapeInBB.CurseCount(Curses.ThreeStrikesPrincipal) >= 1) {
                switch ((int)__instance.ReflectionGetVariable("detentionLevel"))
                {
                    default:
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "ComputerCount_2"));
                        break;
                    case 3:
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "ComputerCount_0"));
                        break;
                    case 2:
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "ComputerCount_0"));
                        break;
                    case 1:
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "ComputerCount_1"));
                        break;
                }
                if ((int)__instance.ReflectionGetVariable("detentionLevel") >= 2)
                {
                    var big = __instance.gameObject.AddComponent<Baldi>();
                    big.enabled = false;
                    big.loseSounds = [
                        new() {
                        selection = Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "Lose_Creepy"),
                        weight = 212
                    }
                    ];
                    big.CaughtPlayer(Singleton<CoreGameManager>.Instance.GetPlayer(0));
                    UnityEngine.Object.Destroy(big);
                }
            }
        }
    }
}
