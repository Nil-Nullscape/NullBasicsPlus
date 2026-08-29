using MTM101BaldAPI.Reflection;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace NullscapeinBB
{
    class MainInfiniteGameManager : MainGameManager
    {
        private IngameSong Song;
        private bool spoopmode = false;
        private int BonusactivitiesLeft = 0;
        private int BonusactivitiesMax = 0;
        public override void AngerBaldi(float val)
        {

            var baseAngerValue = 6.5f / ec.notebookTotal;
            base.AngerBaldi(val * baseAngerValue * (1 + NullscapeInBB.CurseCount(Curses.AngrierBaldi) * 0.75f));
        }

        protected override void AwakeFunction()
        {
            defaultLives = 0;
            Song = NullscapeInBB.Instance.IngameSongs.ToArray().GetRandomSongInRange(NullscapeInBB.Instance.FloorCounter);
            base.AwakeFunction();
        }
        public override void BeginSpoopMode()
        {
            spoopmode = true;
            if (allNotebooksFound) {
                Singleton<CoreGameManager>.Instance.audMan.FlushQueue(true);
                Singleton<CoreGameManager>.Instance.audMan.QueueAudio(Song.allnbSongAsset, true);
                Singleton<CoreGameManager>.Instance.audMan.SetLoop(true);
            } else {
                Singleton<CoreGameManager>.Instance.audMan.FlushQueue(true);
                Singleton<CoreGameManager>.Instance.audMan.QueueAudio(Song.SongAsset, true);
                Singleton<CoreGameManager>.Instance.audMan.SetLoop(true);
            }
            base.BeginSpoopMode();
        }

        protected override void AllNotebooks()
        {
            if (!allNotebooksFound && spoopmode) {
                Singleton<CoreGameManager>.Instance.audMan.FlushQueue(true);
                Singleton<CoreGameManager>.Instance.audMan.QueueAudio(Song.allnbSongAsset, true);
                Singleton<CoreGameManager>.Instance.audMan.SetLoop(true);
            }

            base.AllNotebooks();
            
            
            foreach (var activity in ec.activities)
            {
                if (activity is not NoActivity && activity.InBonusMode && NullscapeInBB.GreaterCurseCount(GreaterCurses.TimedActivities) >= 1) {
                    List<RoomFunction> roomfuncs = (List<RoomFunction>)activity.room.functions.ReflectionGetVariable("functions");
                    TimedActivityRoomFunctions TARF = (TimedActivityRoomFunctions)roomfuncs.First(x => x is TimedActivityRoomFunctions);
                    TARF.ResetFunction();
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
            base.ActivityCompleted(correct, activity);
        }
        

        protected override void VirtualUpdate()
        {
            
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
            base.VirtualUpdate();
        }

    }
}