using NilLib;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NullscapeinBB.NPCs
{
    public class VoidboundOperatorNPC : OperatorNPC
    {
        public int[] TicksToAttackAt = [21, 12, 12];
        public SoundObject Success2;
        public override void Initialize()
        {
            base.Initialize();
            Spawn = NullscapeInBB.Am.Get<SoundObject>("snd_VboSpawn");
            DefaultTickSound = NullscapeInBB.Am.Get<SoundObject>("snd_VboTick");
            Success = NullscapeInBB.Am.Get<SoundObject>("snd_VboGood1");
            Success2 = NullscapeInBB.Am.Get<SoundObject>("snd_VboGood2");

            SpecialSoundAtTick.Clear();
            behaviorStateMachine.ChangeState(new VBOperator_Cooldown(15, this));
        }
        protected override void VirtualUpdate()
        {
            OperatorImg.gameObject.SetActive(!Singleton<CoreGameManager>.Instance.Paused && behaviorStateMachine.currentState is VBOperator_Counting);
            OperatorImg.rectTransform.anchoredPosition = new(0, Mathf.Sin(Time.time) * 10f);
        }
    }
    public class VBOperator_BaseState(VoidboundOperatorNPC npc) : NpcState(npc)
    {
        public readonly VoidboundOperatorNPC Operator = npc;
        public override void DoorHit(StandardDoor door)
        {

        }
    }

    public class VBOperator_Cooldown(float time, VoidboundOperatorNPC npc) : Operator_Cooldown(time,npc) {
        public override void SwitchToCount()
        {
            Operator.behaviorStateMachine.ChangeState(new VBOperator_Counting(Operator.ConvertType<VoidboundOperatorNPC>()));
        }
    }
    public class VBOperator_Counting(VoidboundOperatorNPC npc) : VBOperator_BaseState(npc)
    {
        public int CurrentTick = -3;
        public float totalTickTime = 0f;

        
        public int currentRound = 0;

        public override void Enter()
        {
            Operator.OperatorImg.gameObject.SetActive(true);
            Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_vboIdle");
            Operator.OperatorImg.SetNativeSize();
            Operator.AudMan.PlaySingle(Operator.Spawn);
        }
        public override void Exit()
        {

            Operator.OperatorImg.gameObject.SetActive(false);
        }
        public bool CheckCurrentTick()
        {
           
                Operator.AudMan.PlaySingle(Operator.DefaultTickSound);
            if (CurrentTick >= Operator.TicksToAttackAt[currentRound] - 2) Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_vboCheck");
            else Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_vboIdle");

            if (CurrentTick == Operator.TicksToAttackAt[currentRound] - 2) {
                Operator.AudMan.PlaySingleViaAssetMan(NullscapeInBB.Am, "snd_VboAttack");

            }

            if (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.RealVelocity < Mathf.Epsilon && CurrentTick >= Operator.TicksToAttackAt[currentRound] - 2 && currentRound != 2)
            {
                Operator.AudMan.PlaySingle(Operator.Success);

                Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_vboIdle");
                return true;
            }
            if (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.RealVelocity < Mathf.Epsilon && CurrentTick >= Operator.TicksToAttackAt[currentRound] - 2 && currentRound == 2)
            {
                Operator.AudMan.PlaySingle(Operator.Success2);
                return true;
            }

            if (CurrentTick == Operator.TicksToAttackAt[currentRound]) {
                Operator.OperatorImg.sprite = NullscapeInBB.Am.Get<Sprite>("spr_vboKill");
                UsefulHelpers.KillPlayer(npc, [
                    new() {
                        selection = NullscapeInBB.Am.Get<SoundObject>("snd_VboFail")
                    }
                ]);
                return true;
            }


            return false;
        }

        public override void Update()
        {
            totalTickTime += Time.deltaTime;
            if (totalTickTime > Operator.LengthTick)
            {
                CurrentTick++;
                totalTickTime = 0;
                if (CurrentTick >= 0 && CheckCurrentTick())
                {
                    currentRound++;

                    if (currentRound >= 3) {
                        Operator.behaviorStateMachine.ChangeState(new VBOperator_Cooldown(15, Operator));

                    }
                    CurrentTick = 0;
                }
            }

            if (CurrentTick % 2 == 0)
            {
                Operator.OperatorImg.rectTransform.rotation = Quaternion.Euler(0, 0, -5);
            }
            else
            {
                Operator.OperatorImg.rectTransform.rotation = Quaternion.Euler(0, 0, 5);
            }
        }

    }

}
