using MTM101BaldAPI.Reflection;
using NilLib;
using NilLib.Creators;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace NullscapeinBB
{
    class TimedActivityRoomFunctions :  RoomFunction
    {
        float Timer = 19;
        TextMeshPro tmptimer = null;
        bool IsActivityDone = false;
        bool timed = false;
        Activity actt;
        IEnumerator TimeTi()
        {
            foreach (var act in Singleton<BaseGameManager>.Instance.Ec.activities)
            {
                if (act.room == room)
                {
                    actt = act;
                }
            }
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_computercount10"));
            while (!IsActivityDone && Timer > 0 && !actt.IsCompleted)
            {
                yield return new WaitForSeconds(1f - (NullscapeInBB.CurseCount(Curses.FasterTimedActivities) * 0.2f));
                var timerstring = Timer.ToString();
                if (Timer>=10) {
                    if(tmptimer) {
                        tmptimer.text = $"<sprite={timerstring[0]}><sprite={timerstring[1]}>";
                    }
                } else {
                    if (tmptimer)
                    {
                        tmptimer.text = $"<sprite=0><sprite={timerstring[0]}>";
                    }
                }
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
            Destroy(tmptimer.gameObject);
            if (!IsActivityDone)
            {
                foreach (var act in Singleton<BaseGameManager>.Instance.Ec.activities)
                {
                    if (act.room == room && !act.IsCompleted)
                    {
                        act.Completed(0, false);
                        Singleton<BaseGameManager>.Instance.AngerBaldi(1f);
                        if (act is BalloonBuster)
                        {
                            ((BalloonBuster)act).SubmitAnswer();
                            List<BalloonBusterBalloon> baloons = (List<BalloonBusterBalloon>)((BalloonBuster)act).ReflectionGetVariable("unpoppedBalloons");
                            foreach (var item in baloons)
                            {
                                item.Pop(false);
                            }
                        }
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "Activity_Incorrect"));
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
                tmptimer = ObjectCreation.MakeTMPObject(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay),true, "<sprite=0>", UsefulHelpers.GetSpriteassetFromEnum(UsefulHelpers.SpriteAssetSelection.MathmachineNumbers));
                tmptimer.horizontalAlignment = HorizontalAlignmentOptions.Center;
                
                var e = new GameObject("d");
                e.transform.SetParent(room.transform);
                e.transform.position = room.RandomEntitySafeCellNoGarbage().CenterWorldPosition;
                tmptimer.transform.SetParent(e.transform);
                
                tmptimer.gameObject.AddComponent<PickupBob>();
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