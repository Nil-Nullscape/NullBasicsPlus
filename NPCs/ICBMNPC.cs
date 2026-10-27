using MTM101BaldAPI;
using NilLib;
using NilLib.Creators;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    internal class IcbmNPC : NPC
    {
        public PropagatedAudioManager audMan;
        public float idleTime = 7f;
        public float sizemultiplier = 1f;
        public float aimingtime = 2.694f;
        public float fallingtime = 1.504f;
        public float explosionTime = 0.05f;
        public Vector3 CurrentlyAimedAt = Vector3.zero;
        public SpriteRenderer AimingRenderer;
        public override void Initialize()
        {
            sizemultiplier = 1 + NullscapeInBB.CurseCount(Curses.BiggerBlast) * 0.5f;
            AimingRenderer.gameObject.SetActive(false);
            AimingRenderer.transform.localScale *= sizemultiplier;
            AimingRenderer.transform.rotation = Quaternion.Euler(-90,0,0);
            spriteRenderer[0].enabled = false;
            audMan = gameObject.GetComponent<PropagatedAudioManager>();
            this.behaviorStateMachine.ChangeState(new Icbm_StateIdle(this));
            base.Initialize();

            
        }
        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
        }
    }

    class Icbm_StateBase(IcbmNPC npc) : NpcState(npc) {
        
    }

    class Icbm_StateIdle(IcbmNPC npc) : Icbm_StateBase(npc)
    {
        public float timeleft = 7f;
        public override void Enter()
        {
            npc.ConvertNpcType<IcbmNPC>().idleTime = UnityEngine.Random.Range(2.45f, 7f);
            timeleft = npc.ConvertNpcType<IcbmNPC>().idleTime;
            npc.spriteRenderer[0].enabled = false;
            npc.ConvertNpcType<IcbmNPC>().AimingRenderer.gameObject.SetActive(false);
            base.Enter();
        }
        public override void Update()
        {
            timeleft -= Time.deltaTime;
            if (timeleft < 0)
            {
                npc.behaviorStateMachine.ChangeState(new Icbm_StateAimPlayer((IcbmNPC)npc));
            }
            base.Update();
        }


    }
    class Icbm_StateAimPlayer(IcbmNPC npc) : Icbm_StateBase(npc)
    {
        public float timeleft = 7f;
        public IcbmNPC icbm;
        public override void Initialize()
        {

            base.Initialize();
        }
        public override void Enter()
        {
            icbm = npc.ConvertNpcType<IcbmNPC>();
            timeleft = npc.ConvertNpcType<IcbmNPC>().aimingtime;
            icbm.audMan.PlaySingleViaAssetMan(NullscapeInBB.Am, "snd_ICBMaim");
            icbm.AimingRenderer.gameObject.SetActive(true);
            base.Enter();
        }
        public override void Update()
        {
            var plrpos = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position;
            timeleft -= Time.deltaTime;
            icbm.transform.position = plrpos;
            icbm.AimingRenderer.transform.position = new(plrpos.x, 0, plrpos.z);
            icbm.CurrentlyAimedAt = plrpos;
            if (timeleft < 0)
            {
                npc.behaviorStateMachine.ChangeState(new Icbm_Falling((IcbmNPC)npc));
            }
            base.Update();
        }


    }
    class Icbm_Falling(IcbmNPC npc) : Icbm_StateBase(npc)
    {
        public float timeleft = 7f;
        public IcbmNPC icbm;
        public override void Initialize()
        {

            base.Initialize();
        }
        public override void Enter()
        {
            icbm = npc.ConvertNpcType<IcbmNPC>();
            timeleft = npc.ConvertNpcType<IcbmNPC>().fallingtime;
            npc.spriteRenderer[0].enabled = true;
            npc.spriteRenderer[0].gameObject.layer = UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay);
            icbm.spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite>("spr_icbm");
            icbm.spriteRenderer[0].transform.localPosition = Vector3.up * (timeleft * 20);
            base.Enter();
        }
        public override void Update()
        {

            timeleft -= Time.deltaTime;
            icbm.spriteRenderer[0].transform.localPosition = Vector3.up * (timeleft * 150);
            if (timeleft < 0)
            {
                npc.behaviorStateMachine.ChangeState(new Icbm_IExplodeByebye((IcbmNPC)npc));
            }
            base.Update();
        }
    }

        class Icbm_IExplodeByebye(IcbmNPC npc) : Icbm_StateBase(npc)
        {
            public float timeleft = 7f;
        public bool died = false;
        public float distance = 9999f;
            public IcbmNPC icbm;
        public SpriteRenderer TempRenderer;
        public float timeanim = 0f;
            public override void Initialize()
            {

                base.Initialize();
            }
            public override void Enter()
            {
                icbm = npc.ConvertNpcType<IcbmNPC>();
                timeleft = npc.ConvertNpcType<IcbmNPC>().explosionTime;
            TempRenderer = ObjectCreation.MakeRenderer(true, "explode");
            TempRenderer.transform.position = icbm.transform.position;
            icbm.spriteRenderer[0].enabled = false;
            icbm.audMan.PlaySingleViaAssetMan(NullscapeInBB.Am, "snd_ICBMExplode");
            base.Enter();
            }
            public override void Update()
            {
            distance = (icbm.transform.position - Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position).magnitude;

                timeleft -= Time.deltaTime;
            TempRenderer.sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_explode")[Mathf.FloorToInt(Mathf.Clamp(timeanim, 0, 8))];
            timeanim += Time.deltaTime * 4;

                if (timeleft < 0)
                {
                UnityEngine.Object.Destroy(TempRenderer.gameObject);
                    npc.behaviorStateMachine.ChangeState(new Icbm_StateIdle((IcbmNPC)npc));
                }
            if (distance < 7.5f * icbm.sizemultiplier && !died) {
                UsefulHelpers.KillPlayer(npc,[
                    new() {
                        selection= NullscapeInBB.Am.Get<SoundObject>("snd_ICBMExplode"),
                        weight=3232
                    }
                ]);
                died = true;
            }
                base.Update();
            }
            


        }

    }
