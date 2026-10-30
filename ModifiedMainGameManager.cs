using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using NilLib;
using NilLib.extension;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace NullscapeinBB
{
    class MainInfiniteGameManager : MainGameManager
    {
     
        private IngameSong Song;
        private bool spoopmode = false;
        private int BonusactivitiesLeft = 0;
        private int BonusactivitiesMax = 0;
        public int TempShieldRemaining = 0;
        public float shieldtimeleft = 0f;
        private HudGauge ShieldGauge;
        private HudGauge AbilityGauge;
        public float AbilityGaugeMaxTime = 0f;
        public float AbilityTimeLeft = 0f;
        public bool AbilityEnabled = false;
        public TimeScaleModifier SlowMo;
        public bool isSlowed = false;
        public bool FindyourflameIsPlaying = false;
        public float MaxTime = 5f;

        public override void AngerBaldi(float val)
        {
            
            var baseAngerValue = 6.5f / ec.notebookTotal;
            base.AngerBaldi(val * baseAngerValue * (1 + NullscapeInBB.CurseCount(Curses.AngrierBaldi) * 0.25f));
        }

        public bool canDie(Baldi inst) {
            if (shieldtimeleft > 0) {
                return false;
            }
            
            if (TempShieldRemaining > 0 || NullscapeInBB.Instance.shields > 0) {
                if (NullscapeInBB.GreaterCurseCount(GreaterCurses.LessShield) > 0)
                {
                    MaxTime = 2f;
                    shieldtimeleft = 2f;
                    
                }
                else {
                    MaxTime = 5f;
                    shieldtimeleft = 5f;
                }

                bool lostFrom = false;
                if (TempShieldRemaining > 0) { 
                TempShieldRemaining--;
                    lostFrom = true;
                }
                else NullscapeInBB.Instance.shields--;
                NullscapeInBB.ShieldEventNotifier($"shld_{inst.gameObject.GetComponent<NPC>().Character.ToStringExtended()}",TempShieldRemaining,lostFrom);
                if (NullscapeInBB.Instance.shields == 0 && TempShieldRemaining == 0 && NotebookTotal >= 10 && FoundNotebooks < Mathf.FloorToInt(NotebookTotal / 1.5f)) {
                    FindyourflameIsPlaying = true;
                    Singleton<CoreGameManager>.Instance.musicMan.QueueaudioViaAssetManExt(NullscapeInBB.Am, "mus_findyourflame", true,true,true);
                }
                return false;
            }
            return true;
        }

        protected override void AwakeFunction()
        {
            defaultLives = 0;
            Song = NullscapeInBB.Instance.IngameSongs.ToArray().GetRandomSongInRange(NullscapeInBB.Instance.FloorCounter);
            TempShieldRemaining = NullscapeInBB.UpgradesCount(Upgrades.TempShield);
            base.AwakeFunction();
        }
        public override void BeginSpoopMode()
        {
            spoopmode = true;
            if (!FindyourflameIsPlaying)
            {
                if (allNotebooksFound)
                {
                    Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
                    Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(Song.allnbSongAsset, true);
                    Singleton<CoreGameManager>.Instance.musicMan.SetLoop(true);
                }
                else
                {
                    Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
                    Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(Song.SongAsset, true);
                    Singleton<CoreGameManager>.Instance.musicMan.SetLoop(true);
                }
            }
            base.BeginSpoopMode();
        }

        protected override void AllNotebooks()
        {
            if (!allNotebooksFound && spoopmode && !FindyourflameIsPlaying) {
                Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
                Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(Song.allnbSongAsset, true);
                Singleton<CoreGameManager>.Instance.musicMan.SetLoop(true);
            }

            base.AllNotebooks();
            
            
            foreach (var activity in ec.activities)
            {
                if (activity is not NoActivity && activity.InBonusMode && NullscapeInBB.GreaterCurseCount(GreaterCurses.TimedActivities) >= 1) {
                    List<RoomFunction> roomfuncs = (List<RoomFunction>)activity.room.functions.ReflectionGetVariable("functions");
                    TimedActivityRoomFunctions TARF = (TimedActivityRoomFunctions)roomfuncs.First(x => x is TimedActivityRoomFunctions);
                    if (TARF) {
                        TARF?.ResetFunction();
                    }
                }
                if (activity is not NoActivity && activity.InBonusMode) {
                    
                    BonusactivitiesMax += 1;
                }
            }
            BonusactivitiesLeft = BonusactivitiesMax;
            if (NullscapeInBB.CurseCount(Curses.Lap2) >= 1 && BonusactivitiesLeft > 0)
            {
                ec.ElevatorManager.SetAllElevators(ElevatorState.NoPower);
            }

        }
        public override void ActivityCompleted(bool correct, Activity activity)
        {
            if (correct) {
                if (NullscapeInBB.Instance.SelectedClass == Classes.TimeController) {
                    AbilityTimeLeft = Mathf.Clamp(AbilityTimeLeft + 1.5f, 0, AbilityGaugeMaxTime);
                }
            }
            if (allNotebooksFound && NullscapeInBB.CurseCount(Curses.Lap2) >= 1) {
                BonusactivitiesLeft -= 1;
                if (BonusactivitiesLeft <= 0) {
                    ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
                }
                if (activity is MathMachine) {
                    TMP_Text val1 = (TMP_Text)((MathMachine)activity).ReflectionGetVariable("val1Text");
                    TMP_Text val2 = (TMP_Text)((MathMachine)activity).ReflectionGetVariable("val2Text");
                    TMP_Text sign = (TMP_Text)((MathMachine)activity).ReflectionGetVariable("signText");
                    TMP_Text answer = (TMP_Text)((MathMachine)activity).ReflectionGetVariable("answerText");
                    val1.text = $"{BonusactivitiesMax - BonusactivitiesLeft}";
                    val2.text = $"{BonusactivitiesMax}";
                    sign.text = $"/";
                    answer.text = "!";
                } else if (activity is BalloonBuster) {
                    TMP_Text val1 = (TMP_Text)((BalloonBuster)activity).ReflectionGetVariable("countTmp");
                    TMP_Text val2 = (TMP_Text)((BalloonBuster)activity).ReflectionGetVariable("solutionTmp");
                    val1.text = $"{BonusactivitiesMax - BonusactivitiesLeft}";
                    val2.text = $"{BonusactivitiesMax}";

                }
            }

            if (Random.Range(0, 100) > (100 - NullscapeInBB.UpgradesCount(Upgrades.TeleportRandomInCompletedActivityRoom) * 10)) {
                RoomController nextRoom = null;
                foreach (var item in ec.rooms)
                {
                    if (item.HasIncompleteActivity && ec.activities.FirstOrDefault(x => x.room == item))
                    {
                        nextRoom = item;
                        break;
                    }
                }
                if (nextRoom) {
                    CollectNotebook(activity.GetCachedReflection<Notebook>("notebook"));
                    activity.GetCachedReflection<Notebook>("notebook").Hide(true);
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).Teleport(nextRoom.RandomEntitySafeCellNoGarbage().CenterWorldPosition);
                    shieldtimeleft = 5f;
                    MaxTime = 5f;
                }
            }
            base.ActivityCompleted(correct, activity);
        }
        public void BetterPleaseBaldi(float time) {
            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Character == Character.Baldi)
                {
                    npc.GetComponent<Baldi>().Praise(time + NullscapeInBB.UpgradesCount(Upgrades.BaldiPraise) * 0.5f,false);
                }
            }
        }
        public override void BeginPlay()
        {
            for (int i = 0; i < NullscapeInBB.CurseCount(Curses.RandomTilesAreSilenced)*10; i++)
            {
                var cell = ec.RandomCell(true, true, true);
                while (cell.Silent)
                {
                    cell = ec.RandomCell(true, true, true);
                }
                cell.SetSilence(true);
            }
            ec.AddTimeScale(new(1 + (NullscapeInBB.CurseCount(Curses.FasterNPCs) * 0.25f), 1 + (NullscapeInBB.CurseCount(Curses.FasterEnviromnent) * 0.25f), 1));
            if (NullscapeInBB.Instance.SelectedClass == Classes.TimeController) {
                AbilityGaugeMaxTime = 20f;
                AbilityTimeLeft = 10f;
                AbilityGauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager.ActivateNewGauge(NullscapeInBB.Am.Get<Sprite>("spr_slowtime"), 10f);
                SlowMo = new(0.1f, 0.1f, 1f);
            }
            base.BeginPlay();
        }
        bool ShouldForceOpenExit = false;
        protected override void VirtualUpdate()
        {
            if (spoopmode) Singleton<MusicManager>.Instance.StopMidi();
            var plr = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            if (NullscapeInBB.GreaterCurseCount(GreaterCurses.OmniscientBaldi) > 0)
            {

                foreach (NPC npc in ec.Npcs)
                {
                    if (npc.Character == Character.Baldi)
                    {
                        npc.GetComponent<Baldi>().Hear(plr.gameObject, plr.transform.position, 127, false);
                    }
                }
            }
            if (!ShieldGauge && shieldtimeleft >0) {
                ShieldGauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager.ActivateNewGauge(NullscapeInBB.Am.Get<Sprite>("spr_brokenshield"),5f);

            }
            if (NullscapeInBB.CurseCount(Curses.Lap2) >= 1 && BonusactivitiesMax > 0 && !ShouldForceOpenExit) {
                var unpoweredActivities = 0;
                foreach (var activities in ec.activities)
                {
                    if (activities.InBonusMode && !activities.room.Powered && activities is not NoActivity) {
                        unpoweredActivities += 1;
                    }
                }
                if (BonusactivitiesMax - unpoweredActivities <= 0) {
                    ShouldForceOpenExit = true;
                    ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
                }

            }
            if (ShieldGauge) {
                if (shieldtimeleft > 0) {
                    ShieldGauge.SetValue(MaxTime, shieldtimeleft);
                   
                    shieldtimeleft -= Time.deltaTime * (1 + 1.75f * NullscapeInBB.DelusionsCount(Delusions.MoreShieldButLessShield));
                } else {
                    shieldtimeleft = -1;
                    ShieldGauge.Deactivate();
                    ShieldGauge = null;
                }
            }

            

            if (AbilityTimeLeft <= 0) {
                AbilityEnabled = false;
            }
            if (AbilityEnabled) {
                if ( NullscapeInBB.Instance.SelectedClass == Classes.TimeController) {
                    AbilityTimeLeft -= Time.deltaTime;
                    if (!isSlowed) {
                        isSlowed = true;
                        ec.AddTimeScale(SlowMo);
                    }
                }
            } else {
                if (NullscapeInBB.Instance.SelectedClass == Classes.TimeController)
                {
                    if (isSlowed)
                    {
                        isSlowed = false;
                        ec.RemoveTimeScale(SlowMo);
                    }
                }
            }

            if (AbilityGauge)
            {
                AbilityGauge.SetValue(AbilityGaugeMaxTime, AbilityTimeLeft);
            }
            if (Input.GetKeyDown(NullscapeInBB.Instance.KeyAbility.Value)) {
                if (AbilityTimeLeft > 0 && NullscapeInBB.Instance.SelectedClass == Classes.TimeController) AbilityEnabled = !AbilityEnabled;
            }


            base.VirtualUpdate();
        }

    }
}