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
        public Sprite[] sheet;
        public SpriteRenderer renderer;
        public int Ytp = 1;
        public bool canAnimate = true;
        public float timeanim = 0f; 

        public void Init() {
            renderer.sprite = sheet[0];
            canAnimate = true;
            timeanim = UnityEngine.Random.Range(-500f, 500f);
            transform.position += Vector3.up * 1.5f;
        }

        void Update() {
            if (canAnimate) {
                timeanim += Time.deltaTime * 15.5f;
                renderer.sprite = sheet[Mathf.FloorToInt(Mathf.Repeat(timeanim,sheet.Length))];
            }
        }

        void OnTriggerEnter(Collider other) {
            if (other.GetComponent<PlayerManager>() && activity.room.Powered) {
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
        public Sprite[] sprGiftSheet;
        public Sprite[] sprGoldenGiftSheet;
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
                    dupe.GetComponent<Gift>().sheet = sprGiftSheet;
                    dupe.GetComponent<Gift>().sndcollect = sndCollectNormal;
                } else {
                    dupe.GetComponent<Gift>().sheet = sprGoldenGiftSheet;
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
                Completed(0,true);
            }
        }
        public override void Completed(int player)
        {
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(Resources.FindObjectsOfTypeAll<SoundObject>().First(x => x.name == "Activity_Correct"));
            
            Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.AddStamina(3000f, true);
            Singleton<BaseGameManager>.Instance.ActivityCompleted(correct, this);
            
            base.Completed(player);
        }
        public override void Completed(int player, bool correct)
        {
            base.Completed(player, correct);
            if (!correct) {
                foreach (var item in GiftsToCollect)
                {
                    Destroy(item.gameObject);

                }
                Singleton<BaseGameManager>.Instance.AngerBaldi(1f);
            } else {
                Singleton<BaseGameManager>.Instance.PleaseBaldi(5.5f, true);
            }
            Completed(player);
        }
        public override void SetBonusMode(bool val)
        {
            if (true) {
                SpawnGifts(true);
            }
            base.SetBonusMode(val);
        }
        
    }
}
