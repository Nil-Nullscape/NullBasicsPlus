using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullscapeinBB
{
    class TimedActivityRoomFunctions :  RoomFunction
    {
        float Timer = 20;
        bool IsActivityDone = false;
        bool timed = false;
        IEnumerator TimeTi()
        {

            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_computercount10"));
            while (!IsActivityDone && Timer > 0)
            {
                yield return new WaitForSeconds(1f - (NullscapeInBB.CurseCount(Curses.FasterTimedActivities) * 0.2f));
                if (Timer == 18) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_9"));
                if (Timer == 16) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_8"));
                if (Timer == 14) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_7"));
                if (Timer == 12) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_6"));
                if (Timer == 10) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_5"));
                if (Timer == 8) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_4"));
                if (Timer == 6) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_3"));
                if (Timer == 4) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "ComputerCount_2"));
                if (Timer == 2) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "ComputerCount_1"));
                if (Timer == 0) Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "ComputerCount_0"));
                Timer -= 1;
            }
            if (!IsActivityDone)
            {
                foreach (var act in Singleton<BaseGameManager>.Instance.Ec.activities)
                {
                    if (act.room == room) {
                        act.Completed(0,false);
                        Singleton<BaseGameManager>.Instance.AngerBaldi(1f);
                        if (act is BalloonBuster)
                        {
                            ((BalloonBuster)act).SubmitAnswer();
                        }
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x=>x.name == "Activity_Incorrect"));
                    }
                }
            }
        }

        public void ResetFunction() {
            Timer = 20;
            IsActivityDone = false;
            timed = false;
        }
        public override void OnPlayerEnter(PlayerManager player)
        {
            if (!timed && room.Powered) {
                timed = true;
                StartCoroutine(TimeTi());
            }
            base.OnPlayerEnter(player);
        }
        
        public override void OnActivityCompletion()
        {
            IsActivityDone = true;
            base.OnActivityCompletion();
        }
        

        
    }
}