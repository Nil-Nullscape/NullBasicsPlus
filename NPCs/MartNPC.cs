using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    internal class MartNPC : NPC
    {
        public AudioManager audMan;
        public float speed = 3.5f;
        public int priority = 0;
        public float timeanim = 0;
        public Vector3 BaseMartSize => Vector3.one * 4f;
        public override void Initialize()
        {
            priority = UnityEngine.Random.Range(0, 25555555);
            speed = 1.5f + Mathf.Floor(NullscapeInBB.Instance.FloorCounter / 12f);
            gameObject.SetActive(false);
            audMan = gameObject.AddComponent<AudioManager>();
            
            audMan.audioDevice = gameObject.AddComponent<AudioSource>();
            
            audMan.audioDevice.spatialBlend = 1f;
            gameObject.SetActive(true);
            audMan.audioSourceManager.minDistance = 15f;
            audMan.audioSourceManager.maxDistance = 140f;
            audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_martloop"), true);
            audMan.SetLoop(true);
            this.behaviorStateMachine.ChangeState(new Mart_StateChase(this));
            base.Initialize();

            
        }
        protected override void VirtualUpdate()
        {
            spriteRenderer[0].transform.localScale = Vector3.Lerp(spriteRenderer[0].transform.localScale, BaseMartSize * (speed / 10f), 2f*Time.deltaTime);
            ((CapsuleCollider)baseTrigger[0]).radius = BaseMartSize.y * (speed / 10f);
            timeanim += Time.deltaTime * 16.7f;
            spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_mart")[Mathf.RoundToInt(Mathf.Repeat(timeanim, NullscapeInBB.Am.Get<Sprite[]>("sprsheet_mart").Length-1))];
            base.VirtualUpdate();
        }
    }

    class Mart_statebase(MartNPC npc) : NpcState(npc) {
        
    }

    class Mart_StateChase(MartNPC npc) : Mart_statebase(npc) {
        public override void Update()
        {
            var direction = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - npc.transform.position).normalized;
            npc.transform.position += direction * ((MartNPC)npc).speed * Time.deltaTime;

            base.Update();
        }

        public override void OnStateTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            if (otherEntity == Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity) {
                var e = npc.gameObject.AddComponent<Baldi>();
                e.enabled = false;
                e.loseSounds = [
                    new() {
                    selection = NullscapeInBB.Am.Get<SoundObject>("snd_martkill"),
                    weight = 6969
                    }
                ];
                e.CaughtPlayer(Singleton<CoreGameManager>.Instance.GetPlayer(0));
                UnityEngine.Object.Destroy(e);
            }
            if (otherEntity.gameObject.GetComponent<MartNPC>() && otherEntity.gameObject.GetComponent<MartNPC>().priority < ((MartNPC)npc).priority) {
                UnityEngine.Object.Destroy(otherEntity.gameObject);
                ((MartNPC)npc).speed += otherEntity.gameObject.GetComponent<MartNPC>().speed * 1.2f;
                ((MartNPC)npc).audMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_martmerge"));

            }


            base.OnStateTriggerEnter(otherEntity, other, validCollision);
        }
    }
}
