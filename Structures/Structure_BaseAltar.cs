using JetBrains.Annotations;
using MTM101BaldAPI;
using MTM101BaldAPI.PlusExtensions;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.UI;
using NilLib;
using NilLib.Creators;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace NullscapeinBB.Structures
{
    public class BaseAltar : MonoBehaviour
    {
        private class AltarFunction : MonoBehaviour, IClickable<int>
        {
            public BaseAltar ThisAltar;



            public bool ClickableHidden()
            {
                return ThisAltar.HasBeenActivated;
            }

            public bool ClickableRequiresNormalHeight()
            {
                return true;
            }

            public void ClickableSighted(int player)
            {
                Singleton<CoreGameManager>.Instance.GetHud(0).SetTooltip(ThisAltar.keyDesc);
            }

            public void ClickableUnsighted(int player)
            {
                Singleton<CoreGameManager>.Instance.GetHud(0).CloseTooltip();
            }

            public void Clicked(int player)
            {
                ThisAltar.OnActivate();
            }
            
        }

        public Sprite[] AltarItemSprites;
        public string keyDesc;
        public float animationSpeedMultiplier = 1;
        public Sprite PedestalSprite;
        public bool HasBeenActivated = false;
        public virtual int MinFloor => 0;
        public GameObject pedestalPre;

        public SpriteRenderer itemRenderer;
        AltarFunction func;
        
        public void Start() {
            var pedestal = Instantiate(pedestalPre);

            pedestal.GetComponent<SpriteRenderer>().sprite = PedestalSprite;
            pedestal.transform.SetParent(transform);
            pedestal.transform.localPosition = new(0, -2.65f,0);
            var go = new GameObject("Function");
            func = go.AddComponent<AltarFunction>();
            func.ThisAltar = this;
            func.transform.SetParent(transform);
            func.transform.localPosition = Vector3.zero;
            itemRenderer = ObjectCreation.MakeRenderer(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Billboard), func.transform, true, "Renderer");
            itemRenderer.transform.localPosition = Vector3.zero;
            var cd = func.gameObject.AddComponent<SphereCollider>();
            cd.radius = 1.5f;
            cd.isTrigger = true;
            go.layer = UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.ClickableEntities);
            InitializeMisc();


        }
        public virtual void InitializeMisc() {

        }

        public virtual void Animate() {
        
            itemRenderer.sprite = AltarItemSprites[Mathf.FloorToInt(Mathf.Repeat(Time.time * animationSpeedMultiplier, AltarItemSprites.Length))];

        }

        void Update() {
            Animate();
        }

        public static T CreateAltarPrefab<T>(GameObject PedestalPre, Sprite pedestalSprite, Sprite[] AltarItemSprites,string DescKey, float animationSpeed = 1, string altarName = "AltarPre") where T : BaseAltar {
            var gameObject = new GameObject(altarName);
            gameObject.ConvertToPrefab(true);
            var baseAltar = gameObject.AddComponent<T>();
            baseAltar.pedestalPre = PedestalPre;
            baseAltar.PedestalSprite = pedestalSprite;
            baseAltar.animationSpeedMultiplier = animationSpeed;
            baseAltar.keyDesc = DescKey;
            baseAltar.AltarItemSprites = AltarItemSprites;
            NullscapeInBB.Instance.Cooldowns.Add(baseAltar, 0);
            return baseAltar;
        }


        public virtual void OnActivate() {
            HasBeenActivated = true;
            func.enabled = false;
            func.gameObject.GetComponent<SphereCollider>().enabled = false;
            Singleton<CoreGameManager>.Instance.GetHud(0).CloseTooltip(); 
        }




    }

    public class Structure_AltarBuilder : StructureBuilder
    {
        public RoomAsset AltarRoom;

        public override void PostOpenCalcGenerate(LevelGenerator lg, Random rng)
        {
            base.PostOpenCalcGenerate(lg, rng);
            
        }
        public override void PostHallPrep(LevelGenerator lg, Random rng)
        {
            base.PostHallPrep(lg, rng);
            List<List<GameObject>> Altars = new();
            int totalAltars = 0;

            for (int i = 0; i < parameters.prefab.Length; i++)
            {
                Altars.Add(new());
            }

            int MaxAltars = 1 + Mathf.FloorToInt(NullscapeInBB.Instance.FloorCounter * 0.15f);


            for (int i = 0; i < MaxAltars; i++)
            {

                var retries = 0;
                var index = WeightedGameObject.ControlledRandomIndex(parameters.prefab, rng);
                GameObject chosenAltar = parameters.prefab[index].selection;

                while ((Altars[index].Count >= parameters.minMax[index].z || NullscapeInBB.Instance.FloorCounter < parameters.prefab[index].selection.GetComponent<BaseAltar>().MinFloor || NullscapeInBB.GetCooldown(chosenAltar.GetComponent<BaseAltar>()) > 0 ) && retries < 10)
                {
                    index = WeightedGameObject.ControlledRandomIndex(parameters.prefab, rng);
                    chosenAltar = parameters.prefab[index].selection;
                    retries++;
                }
                if (retries >= 10) continue;
                lg.UpdatePotentialSpawnsForRooms(true);
                if (lg.RandomlyPlaceRoom(AltarRoom, null, true, out var Room))
                {

                    var altar = Instantiate(chosenAltar);

                    altar.transform.SetParent(Room.transform, true);
                    altar.transform.position = ec.CellFromPosition(ec.RealRoomMid(Room)).CenterWorldPosition;
                    Altars[index].Add(chosenAltar);



                    totalAltars++;
                }


            }
            Debug.Log($"{totalAltars} altars generated.");
        }
        public override void PostSpecialRoomPrep(LevelGenerator lg, Random rng)
        {
            base.PostSpecialRoomPrep(lg, rng);
            
        }
        public override void Generate(LevelGenerator lg, Random rng)
        {
            base.Generate(lg, rng);
            
          




        }
    }
}
