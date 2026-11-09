using MTM101BaldAPI.UI;
using NilLib;
using NilLib.Creators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace NullscapeinBB.NPCs
{
    public class OperatorNPC : NPC
    {
        public float LengthTick = 0.25f;
        public int NumberTicks = 13;

        public AudioManager AudMan;
        public SoundObject DefaultTickSound;
        public SoundObject Spawn;
        public SoundObject Success;

        public Dictionary<int, SoundObject> SpecialSoundAtTick = new();
        public Image OperatorImg;

        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].enabled = false;
            AudMan = ObjectCreation.CreateAudioManager(1, 1, false, gameObject);
            DefaultTickSound = NullscapeInBB.Am.Get<SoundObject>("snd_OperatorTick1");
            Spawn = NullscapeInBB.Am.Get<SoundObject>("snd_OperatorSpawn");
            Success = NullscapeInBB.Am.Get<SoundObject>("snd_OperatorSuccess");
            SpecialSoundAtTick.Add(12, NullscapeInBB.Am.Get<SoundObject>("snd_OperatorTick2"));
            SpecialSoundAtTick.Add(13, NullscapeInBB.Am.Get<SoundObject>("snd_OperatorTick3"));

            behaviorStateMachine.ChangeState(new Operator_Cooldown(Random.Range(10f, 25f), this));

            var canv = UIHelpers.CreateBlankUIScreen("Operator's canvas", false, true);
            canv.worldCamera = Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().worldCamera;
            canv.sortingOrder = -99;
            var op = new GameObject("Operator");
            op.transform.SetParent(canv.transform);
            OperatorImg = op.AddComponent<Image>();
            OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_operatorGreen");
            OperatorImg.SetNativeSize();
            OperatorImg.rectTransform.anchoredPosition = Vector2.zero;
            OperatorImg.rectTransform.localScale = Vector3.one * 2.5f;
            OperatorImg.rectTransform.anchorMax = new(0.5f, 0.5f);
            OperatorImg.rectTransform.anchorMin = new(0.5f, 0.5f);

            


        }

        protected override void VirtualUpdate()
        {
            OperatorImg.gameObject.SetActive(!Singleton<CoreGameManager>.Instance.Paused && behaviorStateMachine.currentState is Operator_Counting);
            OperatorImg.rectTransform.anchoredPosition = new(0, Mathf.Sin(Time.time) * 25f);
        }
    }

    public class Operator_BaseState(OperatorNPC npc) : NpcState(npc) {
        public readonly OperatorNPC Operator = npc;
        public override void DoorHit(StandardDoor door)
        {
            
        }
    }

    public class Operator_Cooldown(float time, OperatorNPC npc) : Operator_BaseState(npc) {
        public float timeRemaining = time;
        public override void Update()
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) {
                SwitchToCount();
            }
        }
        public virtual void SwitchToCount() {
            Operator.behaviorStateMachine.ChangeState(new Operator_Counting(Operator));
        }
    }

    public class Operator_Counting(OperatorNPC npc) : Operator_BaseState(npc) {
        public int CurrentTick = -3;
        public float totalTickTime = 0f;

        public override void Enter()
        {
            Operator.OperatorImg.gameObject.SetActive(true);
            Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_operatorYellow");
            Operator.AudMan.PlaySingle(Operator.Spawn);
        }
        public override void Exit()
        {

            Operator.OperatorImg.gameObject.SetActive(false);
        }
        public bool CheckCurrentTick() {
            if (Operator.SpecialSoundAtTick.TryGetValue(CurrentTick, out var sound)) {
                Operator.AudMan.PlaySingle(sound);
            } else {
                Operator.AudMan.PlaySingle(Operator.DefaultTickSound);
            }
            

            if (CurrentTick == 12) Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_operatorRed1");
            if (CurrentTick == 13) Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_operatorRed2");

            if (CurrentTick == Operator.NumberTicks) {
                UsefulHelpers.KillPlayer(npc, [
                    new() {
                        selection = NullscapeInBB.Am.Get<SoundObject>("snd_OperatorKill")
                    }
                ]);
                return true;
            }

            if (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.RealVelocity < Mathf.Epsilon) {
                Operator.AudMan.PlaySingle(Operator.Success);
                return true;
            }

            return false;
        }

        public override void Update()
        {
            totalTickTime += Time.deltaTime;
            if (totalTickTime > Operator.LengthTick) {
                CurrentTick++;
                totalTickTime = 0;
                if (CurrentTick >= 0 && CheckCurrentTick()) {
                    Operator.behaviorStateMachine.ChangeState(new Operator_Cooldown(Random.Range(10f, 25f), Operator));
                }
            }

            if (CurrentTick % 2 == 0) {
                Operator.OperatorImg.rectTransform.rotation = Quaternion.Euler(0, 0, -5);
            } else {
                Operator.OperatorImg.rectTransform.rotation = Quaternion.Euler(0, 0, 5);
            }
        }

    }
}
