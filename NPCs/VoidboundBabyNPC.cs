using NilLib;
using NilLib.Creators;
using NilLib.extension;
using NullscapeinBB.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NullscapeinBB.NPCs
{
    public class VoidboundBabyNPC : NPC
    {
        public AudioManager AudMan;

        public AnimatorBBP Anim;
        public override void Initialize()
        {
            base.Initialize();
            AudMan = ObjectCreation.CreateAudioManager(40, 200, true, gameObject);
            Anim = AnimatorBBP.Setup(spriteRenderer[0], NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetvbBabyIdle), 10);
            Anim.AddAnimationToStorage("Dash", NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetvbBabyDash), 10);
            Anim.AddAnimationToStorage("Locked", NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetvbBabyLocked), 10);
            Anim.AddAnimationToStorage("Trans", NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetvbBabyTrans), 10);

            behaviorStateMachine.ChangeState(new VBBaby_Cooldown(this));
        }
    }

    public class VBBaby_BaseState(VoidboundBabyNPC npc) : NpcState(npc)
    {
        public readonly VoidboundBabyNPC baby = npc;
    }

    public class VBBaby_Cooldown(VoidboundBabyNPC npc) : VBBaby_BaseState(npc)
    {
        public float cooldown = 0.35f;

        public override void Enter()
        {
        }
        public override void Update()
        {
            cooldown -= Time.deltaTime;
            if (cooldown < 0) baby.behaviorStateMachine.ChangeState(new VBBaby_Dash(baby));
        }
    }

    public class VBBaby_Dash(VoidboundBabyNPC npc) : VBBaby_BaseState(npc)
    {
        public Vector3 Direction = Vector3.left;
        public Vector3 NewPos;
        public Vector3 Oldpos;
        public bool Locked = false;
        public bool Dashed = false;
        public float totalTimeLeft = 1f;
        public List<GameObject> Line = new();
        public const int AmountOfLine = 80;
        public override void Enter()
        {
            base.Enter();
            baby.AudMan.PlaySingleViaAssetMan(NullscapeInBB.Am, ModAssetNames.SndvbBabyWarn);
            Direction = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - baby.transform.position).normalized;
            NewPos = baby.transform.position + Direction * 70;
            Oldpos = baby.transform.position;
            baby.Anim.QueueAnimation("Locked", true, true);

            for (int i = 0; i < AmountOfLine; i++)
            {
                var renderer = ObjectCreation.MakeRenderer(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Billboard), true, "line");
                renderer.sprite = NullscapeInBB.Am.Get<Sprite>(ModAssetNames.tex_WhiteTex);
                renderer.transform.position = Vector3.Lerp(Oldpos + Vector3.down * 2.5f, NewPos + Vector3.down * 2.5f, (float)i / AmountOfLine);
                renderer.transform.localScale = Vector3.one * 0.015f;
                renderer.color = new(0.5f, 0, 0.5f, 0.5f);
                Line.Add(renderer.gameObject);
            }

            Debug.Log("Dashing");

        }
        public override void Update()
        {
            totalTimeLeft -= Time.deltaTime;
            if (totalTimeLeft < 0.5f)
            {
                if (!Dashed)
                {
                    Dashed = true;
                    baby.Anim.QueueAnimation("Trans", true, true);
                    baby.Anim.QueueAnimation("Dash");
                    baby.AudMan.PlaySingleViaAssetMan(NullscapeInBB.Am, ModAssetNames.SndvbBabyDash);
                }
                baby.transform.position = Vector3.Lerp(Oldpos, NewPos, (0.5f - totalTimeLeft) / 0.5f);
            }
            if (totalTimeLeft < -0.1f)
            {
                baby.behaviorStateMachine.ChangeState(new VBBaby_Cooldown(baby));

            }
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
            foreach (var item in Line)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
            Line.Clear();
            baby.Anim.QueueAnimation("Trans", true, true);
            baby.Anim.QueueAnimation(AnimatorBBP.DefaultAnimation);
        }
        public override void OnStateTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            if (otherEntity.GetComponent<PlayerManager>())
            {
                UsefulHelpers.KillPlayer(baby, [new() {
                selection = NullscapeInBB.Am.Get<SoundObject>("snd_NilKill"),
                weight = 99
            }]);
            }
        }
    }
}
