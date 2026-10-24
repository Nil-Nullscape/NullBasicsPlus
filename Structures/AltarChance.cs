using MTM101BaldAPI.Registers;
using NilLib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullscapeinBB.Structures
{
    public class WeightedActions : WeightedSelection<Action>;
    public class AltarChance : BaseAltar
    {
        public override int MinFloor => 3;


        public WeightedActions[] actions = {
            new() {
                selection = () => {
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_happyBaldi"));
                    Singleton<BaseGameManager>.Instance.PleaseBaldi(10,false);


                },
                weight = 40
             },
             new() {
                selection = () => {
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_angrierBaldi"));
                    Singleton<BaseGameManager>.Instance.AngerBaldi(3.5f);


                },
                weight = 80
             },
             new() {
                selection = () => {
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_freeYtps"));
                    Singleton<CoreGameManager>.Instance.AddPoints(UnityEngine.Random.Range(1,50)*10,0,true,true,true);


                },
                weight = 95
             },
             new() {
                selection = () => {
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_paidYtps"));
                    Singleton<CoreGameManager>.Instance.AddPoints(-(UnityEngine.Random.Range(1,50)*10),0,true,true,true);


                },
                weight = 90
             }
             ,
             new() {
                selection = () => {

                List<WeightedItemObject> PotentialItems =
                [
                    new()
                    {
                    selection = ItemMetaStorage.Instance.FindByEnum(Items.Bsoda).value,
                    weight = 200
                    },
                    new()
                    {
                        selection = ItemMetaStorage.Instance.FindByEnum(Items.Teleporter).value,
                        weight = 50
                    },
                    new()
                    {
                        selection = ItemMetaStorage.Instance.FindByEnum(Items.Apple).value,
                        weight = 5
                    },
                    new()
                    {
                        selection = ItemMetaStorage.Instance.FindByEnum(Items.GrapplingHook).value,
                        weight = 100
                    },
                ];
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_FreeItem"));
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.AddItem(WeightedItemObject.RandomSelection(PotentialItems.ToArray()));


                },
                weight = 90
             },new() {
                selection = () => {
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_RemoveItem"));
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.RemoveRandomItem();


                },
                weight = 100
             }
             ,new() {
                selection = () => {
                    NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("chance_ExtraShield"));
                    NullscapeInBB.Instance.shields+=1;


                },
                weight = 25
             }

        };
        public override void OnActivate()
        {
            base.OnActivate();
            WeightedActions.RandomSelection(actions)();

            
        }
    }
}
