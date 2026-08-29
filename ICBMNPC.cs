using MTM101BaldAPI;
using NilLib;
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
        public float aimingtime = 2.694f;
        public float fallingtime = 1.504f;
        public float explosionTime = 1;
        public Vector3 CurrentlyAimedAt = Vector3.zero;
        public SpriteRenderer AimingRenderer;
        public override void Initialize()
        {
            AimingRenderer.gameObject.SetActive(false);
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
                npc.behaviorStateMachine.ChangeState(new Icbm_StateIdle((IcbmNPC)npc));
            }
            base.Update();
        }
    }

        class Icbm_IExplodeByebye(IcbmNPC npc) : Icbm_StateBase(npc)
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
                timeleft = npc.ConvertNpcType<IcbmNPC>().explosionTime;
                base.Enter();
            }
            public override void Update()
            {

                timeleft -= Time.deltaTime;

                if (timeleft < 0)
                {
                    npc.behaviorStateMachine.ChangeState(new Icbm_StateIdle((IcbmNPC)npc));
                }
                base.Update();
            }


        }

    }
