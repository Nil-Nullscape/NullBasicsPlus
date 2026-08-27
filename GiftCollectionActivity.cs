using JetBrains.Annotations;
using MidiPlayerTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    class ActivityGiftPlacementHint : MonoBehaviour {

    }
    class Gift : MonoBehaviour 
    {
        public GiftCollectionActivity activity;
        public SoundObject sndcollect;
        public Sprite giftspr;
        public SpriteRenderer renderer;
        public int Ytp = 1;

        public void Init() {
            renderer.sprite = giftspr;
        }
        void OnTriggerEnter(Collider other) {
            if (other.GetComponent<PlayerManager>()) {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(sndcollect);
                Singleton<CoreGameManager>.Instance.AddPoints(Ytp,0,true);
                other.GetComponent<PlayerManager>().plm.AddStamina(3.75f, false);
                activity.GiftsToCollect.Remove(this);
                activity.UpdateState();
                Destroy(gameObject);
            }
        }


    }
    class GiftCollectionActivity : Activity
    {
        public Sprite sprgoldenGift;
        public Sprite sprGift;
        public SoundObject sndCollectNormal;
        public SoundObject sndCollectGolden;
        

        public GameObject giftPre;

        public List<Gift> GiftsToCollect;
        public void SpawnGifts(bool golden) {
            GiftsToCollect = new();
            foreach (var item in room.objectObject.GetComponentsInChildren<ActivityGiftPlacementHint>(true))
            {
                var dupe = Instantiate(giftPre);
                dupe.transform.SetParent(room.objectObject.transform);
                dupe.transform.localPosition = item.transform.localPosition;
                dupe.transform.localPosition += Vector3.up * 1.5f;
                if (!golden) {
                    dupe.GetComponent<Gift>().giftspr = sprGift;
                    dupe.GetComponent<Gift>().sndcollect = sndCollectNormal;
                } else {
                    dupe.GetComponent<Gift>().giftspr = sprgoldenGift;
                    dupe.GetComponent<Gift>().sndcollect = sndCollectGolden;
                    dupe.GetComponent<Gift>().Ytp = 2;
                }
                
                dupe.GetComponent<Gift>().activity = this;
                GiftsToCollect.Add(dupe.GetComponent<Gift>());

            }

            foreach (var item in GiftsToCollect)
            {
                item.Init();
            }
             
        }
        public void Start() {
            room.ec.AddActivity(this);
            
            ReInit();
        }
        public override void ReInit()
        {
            SpawnGifts(false);
            completed = false;
            notebook.Hide(true);
            base.ReInit();
        }
        public void UpdateState() {
            if (GiftsToCollect.Count <= 0) {
                Completed(0);
            }
        }
        public override void Completed(int player)
        {
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "Activity_Correct"));
            Singleton<BaseGameManager>.Instance.PleaseBaldi(5.5f,true);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.AddStamina(3000f, true);
            Singleton<BaseGameManager>.Instance.ActivityCompleted(correct, this);

            base.Completed(player);
        }
        public override void SetBonusMode(bool val)
        {
            if (true) {
                SpawnGifts(true);
            }
            base.SetBonusMode(val);
        }
        
        public override void Initialize()
        {
            
        }
    }
}
