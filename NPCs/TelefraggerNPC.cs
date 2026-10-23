using MTM101BaldAPI;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    public class TelefraggerNPC : NPC
    {
        public AudioManager audMan;
        public float speed = 5f;
        public int TheFunValue = 0;
        public bool IsHeDancing = false;
        public int forceValue = -1;
        public float timeanim = 0;
        public float timepertps = 7f;
        public float timeleft = 7f; 
        public override void Initialize()
        {
            TheFunValue = UnityEngine.Random.Range(0, 100);
            timeleft = UnityEngine.Random.Range(1f, 12f);
            gameObject.SetActive(false);
            audMan = gameObject.AddComponent<AudioManager>();

            audMan.audioDevice = gameObject.AddComponent<AudioSource>();

            audMan.audioDevice.spatialBlend = 1f;
            gameObject.SetActive(true);
            audMan.audioSourceManager.minDistance = 5f;
            audMan.audioSourceManager.maxDistance = 65f;
            if (forceValue > 0) {
                TheFunValue = forceValue;
            }
            if (TheFunValue == 99) {
                IsHeDancing = true;
            }
            if (IsHeDancing) {
                audMan.QueueaudioViaAssetManExt(NullscapeInBB.Am, "mus_teledance", true, true, true);
            } else {
                audMan.QueueaudioViaAssetManExt(NullscapeInBB.Am, "snd_telefraggerwalk", true, true, true);
            }
            this.behaviorStateMachine.ChangeState(new Telefragger_StateChase(this));
            base.Initialize();

            
        }
        protected override void VirtualUpdate()
        {
            
            if (IsHeDancing)
            {
                timeanim += Time.deltaTime * 30f;
                spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_teledance")[Mathf.FloorToInt(Mathf.Repeat(timeanim, NullscapeInBB.Am.Get<Sprite[]>("sprsheet_teledance").Length))];
            }
            else
            {
                timeanim += Time.deltaTime * 2f;
                spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_tele")[Mathf.FloorToInt(Mathf.Repeat(timeanim, NullscapeInBB.Am.Get<Sprite[]>("sprsheet_tele").Length))];
            }
            base.VirtualUpdate();
        }
    }

    class Telefragger_statebase(TelefraggerNPC npcd) : NpcState(npcd) {
        public TelefraggerNPC telefragger => npcd.ConvertNpcType<TelefraggerNPC>();
    }

    class Telefragger_StateChase(TelefraggerNPC npc) : Telefragger_statebase(npc) {
        bool didtpsound = false;
        public override void Update()
        {
        
            var direction = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - npc.transform.position).normalized;
            npc.transform.position += direction * telefragger.speed * Time.deltaTime;
            telefragger.timeleft -= Time.deltaTime;
            if (telefragger.timeleft < 1 && !didtpsound) {
                didtpsound = true;
                Singleton<CoreGameManager>.Instance.audMan.PlaySingleViaAssetMan(NullscapeInBB.Am, "snd_telefraggertp");
            }
            if (telefragger.timeleft < 0) {
                telefragger.timeleft = telefragger.timepertps;
                telefragger.transform.position = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position + Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.forward * 25;
                didtpsound = false;
            }

            base.Update();
        }

        public override void OnStateTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            if (otherEntity == Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity) {
                var e = npc.gameObject.AddComponent<Baldi>();
                e.enabled = false;
                e.loseSounds = [
                    new() {
                    selection = NullscapeInBB.Am.Get<SoundObject>("mus_teledance"),
                    weight = 6969
                    }
                ];
                e.CaughtPlayer(Singleton<CoreGameManager>.Instance.GetPlayer(0));
                UnityEngine.Object.Destroy(e);
            }


            base.OnStateTriggerEnter(otherEntity, other, validCollision);
        }
    }
}
