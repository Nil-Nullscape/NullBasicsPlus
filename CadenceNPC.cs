using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using PlusLevelStudio.Editor.ModeSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    internal class CadenceNPC : NPC
    {
        public PropagatedAudioManager audMan;
        public Instrument Pre;
        public float timeleft = 50f;
        public float timeanim = 0;
        public int phase = 1;
        public int currentCheckedphase = 0;
        public float timebeforenextInstrument = 45f;
        public List<Instrument> instruments;
        public override void Initialize()
        {
           
            audMan = gameObject.GetComponent<PropagatedAudioManager>();
            audMan.DisableVolumetricAudio();
            audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_martloop"), true);
            
            audMan.SetLoop(true);
            this.behaviorStateMachine.ChangeState(new CadenceNPC_stateidle(this));
            base.Initialize();

            
        }
        public void RemoveInstrument(Instrument instrument){
            instruments.RemoveAll(x => x == instrument);
        }

        public void SpawnNewInstrument() {
            audMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_instwarning"));
            Instrument inst = Instantiate(Pre);
            inst.Cadence = this;
            if (instruments.Count % 2 == 0) {
                inst.typeOfInstrument = Instrument.InstrumentType.Harp;
            } else {
                inst.typeOfInstrument = Instrument.InstrumentType.Violin;
            }
            var randomCell = ec.RandomCell(true, false, true);
            inst.transform.SetParent(ec.transform);
            inst.transform.position = randomCell.CenterWorldPosition;
            instruments.Add(inst);


            inst.Init();
        }
        public void OnCollectInstrument() {
            if (instruments.Count > 2) timeleft = Mathf.Clamp(timeleft + 15,0,50);
        }
        public void CheckNewPhase() {
            if (phase == currentCheckedphase) {

            } else {
                currentCheckedphase = phase;
                switch (phase) {
                    case 1:
                        audMan.FlushQueue(true);
                        audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_cadenceP1"), true);
                        audMan.SetLoop(true);
                        break;
                    case 2:
                        audMan.FlushQueue(true);
                        audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_cadenceP2"), true);
                        audMan.SetLoop(true);
                        break;
                    case 3:
                        audMan.FlushQueue(true);
                        audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_cadenceP3"), true);
                        audMan.SetLoop(true);
                        break;
                    case 4:
                        audMan.FlushQueue(true);
                        audMan.QueueAudio(NullscapeInBB.Am.Get<SoundObject>("snd_cadenceP4"), true);
                        audMan.SetLoop(true);
                        break;
                }
            }

        }
        protected override void VirtualUpdate()
        {
            timebeforenextInstrument -= Time.deltaTime;
            if (timebeforenextInstrument < 0) {
                timebeforenextInstrument = 45f;
                SpawnNewInstrument();
            }
            
            base.VirtualUpdate();
        }
    }

    class CadenceNPC_statebase(CadenceNPC npc) : NpcState(npc) {
        
    }

    class CadenceNPC_stateidle(CadenceNPC npc) : CadenceNPC_statebase(npc) {
        public override void Update()
        {
            ((CadenceNPC) npc).timeanim += Time.deltaTime * Mathf.Pow(2,((CadenceNPC)npc).phase-1);
            npc.spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_cadence")[Mathf.FloorToInt(Mathf.Repeat(((CadenceNPC)npc).timeanim, NullscapeInBB.Am.Get<Sprite[]>("sprsheet_cadence").Length - 1))];
            if (((CadenceNPC)npc).instruments.Count >= 2) {
                ((CadenceNPC)npc).timeleft -= Time.deltaTime;
            } else {
                ((CadenceNPC)npc).timeleft = 50;
            }
            if (((CadenceNPC)npc).timeleft > 37.5)
            {
                ((CadenceNPC)npc).phase = 1;
            }
            if (((CadenceNPC)npc).timeleft > 25 && ((CadenceNPC)npc).timeleft <= 37.5)
            {
                ((CadenceNPC)npc).phase = 2;
            }
            if (((CadenceNPC)npc).timeleft > 12.5 && ((CadenceNPC)npc).timeleft <= 25)
            {
                ((CadenceNPC)npc).phase = 3;
            }
            if (((CadenceNPC)npc).timeleft > 0 && ((CadenceNPC)npc).timeleft <= 12.5)
            {
                ((CadenceNPC)npc).phase = 4;
            }
            ((CadenceNPC)npc).CheckNewPhase();
            base.Update();
        }

        
    }

    class CadenceNPC_statechase(CadenceNPC npc) : CadenceNPC_statebase(npc) {

    }
    class Instrument : MonoBehaviour {
        public CadenceNPC Cadence;
        public SpriteRenderer Renderer;
        public PropagatedAudioManager AudMan;
        public SoundObject SoundInstrumentLoop;
        public bool collected = false;


        public enum InstrumentType
        {
            Harp,
            Violin,
            None // bro i swear if its possible to get that normally im grilling myself
        }

        public InstrumentType typeOfInstrument = InstrumentType.Harp;

        void Start()
        {
            
        }

        public void Init(InstrumentType typeofinstrument = InstrumentType.None)
        {
        
            if (typeofinstrument != InstrumentType.None) { typeOfInstrument = typeofinstrument; }
            switch (typeOfInstrument)
            {
                default:
                case InstrumentType.Harp:
                    SoundInstrumentLoop = NullscapeInBB.Am.Get<SoundObject>("snd_harpInst");
                    Renderer.sprite = NullscapeInBB.Am.Get<Sprite>("spr_harpinst");

                    break;
                case InstrumentType.Violin:
                    SoundInstrumentLoop = NullscapeInBB.Am.Get<SoundObject>("snd_violinInst");
                    Renderer.sprite = NullscapeInBB.Am.Get<Sprite>("spr_violininst");

                    break;
            }
            AudMan.QueueAudio(SoundInstrumentLoop,true);
            AudMan.SetLoop(true);
            AudMan.ReflectionSetVariable("minDistance", 20);
            AudMan.ReflectionSetVariable("maxDistance", 400);

        }
        IEnumerator DespawnOrSmthLikethat() {
            AudMan.FlushQueue(true);
        switch (typeOfInstrument) {
                default:
                case InstrumentType.Harp:
                    AudMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_collectharp"));
                    break;
                case InstrumentType.Violin:
                    AudMan.PlaySingle(NullscapeInBB.Am.Get<SoundObject>("snd_collectviolin"));
                    break;
            }
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
            
        }

        void OnTriggerEnter(Collider other) {
            if (other.GetComponent<PlayerManager>() && !collected) {
                Cadence.OnCollectInstrument();
                Cadence.instruments.Remove(this);
                
                collected = true;
                Renderer.enabled = false;
                StartCoroutine(DespawnOrSmthLikethat());
            }
        }
    }
}
