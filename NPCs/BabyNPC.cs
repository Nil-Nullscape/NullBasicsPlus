using NilLib.Creators;
using NilLib.extension;
using NullscapeinBB.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.NPCs
{
    public class BabyNPC : NPC
    {
        public AudioManager AudMan;

        public AnimatorBBP Anim;
        public override void Initialize()
        {
            base.Initialize();
            AudMan = ObjectCreation.CreateAudioManager(40, 200, true, gameObject);
            Anim = AnimatorBBP.Setup(spriteRenderer[0], NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetBabyIdle),12.5f);
            Anim.AddAnimationToStorage("Dash", NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetBabyDash), 12.5f);
            Anim.AddAnimationToStorage("Locked", NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetBabyLocked), 12.5f);
            Anim.AddAnimationToStorage("Trans", NullscapeInBB.Am.Get<Sprite[]>(ModAssetNames.SprSheetBabyTrans), 12.5f);
        }
    }

    public class Baby_BaseState(BabyNPC npc) : NpcState(npc) {
        public readonly BabyNPC baby = npc;
    }

    public class Baby_Cooldown(BabyNPC npc) : Baby_BaseState(npc) {
        public float cooldown = 2f;
        public override void Update()
        {
            if (cooldown < 0) baby.behaviorStateMachine.ChangeState(new Baby_Dash(baby));
        }
    }

    public class Baby_Dash(BabyNPC npc) : Baby_BaseState(npc) {
        public Vector3 Direction = Vector3.left;

        public bool Locked = false;
        public bool Dashed = false;
        public float totalTimeLeft = 1.75f;

        public override void Enter()
        {
            base.Enter();
            Direction = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - baby.transform.position).normalized;
            baby.Anim.QueueAnimation("Locked", true, true);
            baby

        }

        public override void Exit()
        {
            base.Exit();
            baby.Anim.QueueAnimation(AnimatorBBP.DefaultAnimation, true, true);
        }
    }
}
