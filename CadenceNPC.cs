using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    internal class CadenceNPC : NPC
    {
        public PropagatedAudioManager audMan;
        public float timeleft = 50f;
        public float timeanim = 0;
        public override void Initialize()
        {
           
            audMan = gameObject.GetComponent<PropagatedAudioManager>();
            audMan.DisableVolumetricAudio();
            audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_martloop"), true);
            audMan.SetLoop(true);
            this.behaviorStateMachine.ChangeState(new CadenceNPC_stateidle(this));
            base.Initialize();

            
        }
        protected override void VirtualUpdate()
        {
            timeanim += Time.deltaTime * 1f;
            spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_cadence")[Mathf.RoundToInt(Mathf.Repeat(timeanim, NullscapeInBB.Am.Get<Sprite[]>("sprsheet_cadence").Length-1))];
            base.VirtualUpdate();
        }
    }

    class CadenceNPC_statebase(CadenceNPC npc) : NpcState(npc) {
        
    }

    class CadenceNPC_stateidle(CadenceNPC npc) : CadenceNPC_statebase(npc) {
        public override void Update()
        {
           

            base.Update();
        }

        
    }
}
