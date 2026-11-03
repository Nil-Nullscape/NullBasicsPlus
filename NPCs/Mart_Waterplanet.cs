using NilLib;
using NilLib.extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.NPCs
{
    internal class Mart_Waterplanet : MartNPC
    {
        public override Vector3 BaseMartSize => Vector3.one * 50;


        public void SetMartColor(WaterplanetColor color) {
            switch (color)
            {
                case WaterplanetColor.Blue:
                    CurrentState = MartState.Waterplanet_Blue;
                    break;
                case WaterplanetColor.Pink:
                    CurrentState = MartState.Waterplanet_Pink;
                    break;
                default:
                    break;
            }
        }
        public enum WaterplanetColor
        {
            Blue,
            Pink
        }
        public readonly TimeScaleModifier pinkMartTime = new(0.4f, 0.4f, 1);
        public readonly TimeScaleModifier blueMartTime = new(1.7f, 1.7f, 1);

        public override void Initialize()
        {
            SetMartColor(WaterplanetColor.Blue);
            base.Initialize();
            speed = 1;
            spriteRenderer[0].gameObject.layer = UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay);
            
            audMan.audioSourceManager.minDistance = 10f;
            audMan.audioSourceManager.maxDistance = 100;
            audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_martloop2"), true);
            audMan.SetLoop(true);
            this.behaviorStateMachine.ChangeState(new Waterplanet_Chase(this));
        }
        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            var distance = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - transform.position).magnitude;

            spriteRenderer[0].color = new Color(1, 1, 1, Mathf.Clamp(distance / 45f, 0.25f, 1f));
        }
    }
    class Waterplanet_Chase(Mart_Waterplanet mart) : Mart_statebase(mart) {
        public override void Update()
        {
            var direction = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - npc.transform.position).normalized;
            npc.transform.position += direction * ((MartNPC)npc).speed * Time.deltaTime;
        }

        public IEnumerator TimeSlow()
        {

            npc.ec.AddTimeScale(npc.ConvertType<Mart_Waterplanet>().pinkMartTime);
            yield return new WaitForSeconds(15);
            npc.ec.RemoveTimeScale(npc.ConvertType<Mart_Waterplanet>().pinkMartTime);
        }
        public IEnumerator TimeFast()
        {

            npc.ec.AddTimeScale(npc.ConvertType<Mart_Waterplanet>().blueMartTime);
            yield return new WaitForSeconds(15);
            npc.ec.RemoveTimeScale(npc.ConvertType<Mart_Waterplanet>().blueMartTime);
        }

        public override void OnStateTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            if (Singleton<BaseGameManager>.Instance is MainInfiniteGameManager Migm)
            {
                if (otherEntity == Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity && Migm.CheckMultipleStatusEffectsIfDone(MainInfiniteGameManager.StatusEffectsTypes.TimeSlow,MainInfiniteGameManager.StatusEffectsTypes.TimeFast))
                {
                    switch (npc.ConvertType<Mart_Waterplanet>().CurrentState)
                    {
                        case MartNPC.MartState.Waterplanet_Blue:
                            Migm.NewStatusEffect(MainInfiniteGameManager.StatusEffectsTypes.TimeFast, 15);
                            npc.StartCoroutine(TimeFast());
                            break;
                        case MartNPC.MartState.Waterplanet_Pink:
                            Migm.NewStatusEffect(MainInfiniteGameManager.StatusEffectsTypes.TimeSlow, 15);
                            npc.StartCoroutine(TimeSlow());

                            break;

                        default:
                            break;
                    }

                }
            }
            if (otherEntity.gameObject.GetComponent<MartNPC>() && otherEntity.gameObject.GetComponent<MartNPC>().priority < ((MartNPC)npc).priority)
            {
                UnityEngine.Object.Destroy(otherEntity.gameObject);
                ((MartNPC)npc).speed += otherEntity.gameObject.GetComponent<MartNPC>().speed * 1.2f;
                ((MartNPC)npc).audMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_martmerge"));

            }
        }
        
    }
}
