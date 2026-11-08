using MTM101BaldAPI.AssetTools;
using NilLib;
using NilLib.Creators;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.NPCs
{
    // Oh look im self inserting, i should DIE for doing that
    public class NilNPC : NPC
    {


        public const float DashDistance = 15f;
        public const float FullyVisibleDistance = 20f;
        public AudioManager AudMan;
        public float totalTime = 0f;
        public MaterialPropertyBlock m;
        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].material.SetTexture("_LightMap", AssetLoader.TextureFromMod(NullscapeInBB.Instance, "textures", "whiteTex.png"));
            spriteRenderer[0].gameObject.layer = UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay);
            AudMan = ObjectCreation.CreateAudioManager(10, 90, true, gameObject);
            AudMan.QueueaudioViaAssetManExt(NullscapeInBB.Am, "snd_NilLoop", true, true, true);
            behaviorStateMachine.ChangeState(new Nil_IdleOrSmth(this));
            m = new();
        }


        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            totalTime += Time.deltaTime;
            var dist = (transform.position - Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position).magnitude;
            spriteRenderer[0].color = Color.Lerp(new(1, 1, 1, 1), new(1, 1, 1, 0.05f), (dist - 15) / FullyVisibleDistance);

            spriteRenderer[0].sprite = NullscapeInBB.Am.Get<Sprite[]>("sprsheet_Nil")[Mathf.FloorToInt(Mathf.Repeat(totalTime * 8, 4))];
            spriteRenderer[0].GetPropertyBlock(m);
            m.SetFloat("_PercentInvisible", 0);
            spriteRenderer[0].SetPropertyBlock(m);
        }
    }

    public class Nil_BaseState(NilNPC npc) : NpcState(npc) {
        public NilNPC Nil = npc.ConvertType<NilNPC>();
        public override void DoorHit(StandardDoor door)
        {
            
        }
    }

    public class Nil_IdleOrSmth(NilNPC nil) : Nil_BaseState(nil) {
        public float Distance = 0f;


        public override void Update()
        {
            Distance = (Nil.transform.position - Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position).magnitude;
            var Dir = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - Nil.transform.position).normalized;
            Nil.transform.position += Dir * 7 * Time.deltaTime;
            if (Distance < NilNPC.DashDistance) {
                Nil.behaviorStateMachine.ChangeState(new Nil_Dash(Nil));
            }

            base.Update();
        }

    }

    public class Nil_Dash(NilNPC nil) : Nil_BaseState(nil) {
        public float TimeRemainingState = 2.25f;

        public Vector3 Dir;
        public override void Enter()
        {
            base.Enter();
            Dir = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - Nil.transform.position).normalized;

            Nil.AudMan.PlaySingleViaAssetMan(NullscapeInBB.Am, "snd_NilDash");

        }
        public override void Update()
        {
            TimeRemainingState -= Time.deltaTime;
            if (TimeRemainingState < 2f) {
                Nil.transform.position += Dir * 60 * Time.deltaTime;
            }
            if (TimeRemainingState < 0f) {
                Nil.transform.position = Nil.ec.RandomCell(true, true, false).CenterWorldPosition;
                Nil.behaviorStateMachine.ChangeState(new Nil_IdleOrSmth(nil));
            }
            base.Update();
        }
        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            if (otherEntity.GetComponent<PlayerManager>()) {
                UsefulHelpers.KillPlayer(Nil, [new() {
                selection = NullscapeInBB.Am.Get<SoundObject>("snd_NilKill"),
                weight = 99
            }]);
            }
        }
    }
}
