using MTM101BaldAPI;
using NilLib;
using NilLib.Creators;
using NilLib.extension;
using NullscapeinBB.Intermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NullscapeinBB
{
    public static class UpgradesIntermission
    {
        public static void OnCollectIntermission(GameObject[] pickups, Pickup currpck, UpgradeItem ei)
        {
            var upg = NullscapeInBB.Instance.AllUpgrades[ei.curse];
            NullscapeInBB.Instance.UpgradesBought.Add(upg);
                if (upg.OnUpgradeBought != null) upg.OnUpgradeBought();
            foreach (var item in pickups)
            {
                Object.Destroy(item);
            }
            var e = NullscapeInBB.Instance.PickThreeUpgrades();
            if (e.Count > 0) CreatePickupsUpgrade(NullscapeInBB.Instance.AllUpgradesItems[e[0].curse], NullscapeInBB.Instance.AllUpgradesItems[e[1].curse], NullscapeInBB.Instance.AllUpgradesItems[e[2].curse]);
            else NullscapeInBB.delay(() => { Extras.NextIntermission(); }, 0.5f);
        }

        public static void CreatePickupsUpgrade(ItemObject i1, ItemObject i2, ItemObject i3)
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup" && x.GetInstanceID() > 0);
            var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
            PickUp1.GetComponent<Pickup>().showDescription = true;
            PickUp1.GetComponent<Pickup>().AssignItem(i1);
            var PickUp2 = Object.Instantiate(PickupPrefab, new Vector3(30, -0f, -42.5f), Quaternion.identity);
            PickUp2.GetComponent<Pickup>().showDescription = true;
           PickUp2.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.ContinueItem);
            var PickUp3 = Object.Instantiate(PickupPrefab, new Vector3(40, -0f, -42.5f), Quaternion.identity);
            PickUp3.GetComponent<Pickup>().showDescription = true;
            PickUp3.GetComponent<Pickup>().AssignItem(i3);
            

            UpgradeItem enemItem1 = (UpgradeItem)i1.item;
            UpgradeItem enemItem2 = (UpgradeItem)i2.item;
            UpgradeItem enemItem3 = (UpgradeItem)i3.item;
            i1.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_{Enum.GetName(typeof(Upgrades), i1.item.ConvertType<UpgradeItem>().curse)}")}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_upgcost")} {NullscapeInBB.Instance.AllUpgrades[i1.item.ConvertType<UpgradeItem>().curse].Cost()} YTPs\n({NullscapeInBB.UpgradesCount(i1.item.ConvertType<UpgradeItem>().curse)}/{NullscapeInBB.Instance.AllUpgrades[i1.item.ConvertType<UpgradeItem>().curse].MaxUpgrades})";
            i2.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_{Enum.GetName(typeof(Upgrades), i2.item.ConvertType<UpgradeItem>().curse)}")}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_upgcost")} {NullscapeInBB.Instance.AllUpgrades[i2.item.ConvertType<UpgradeItem>().curse].Cost()} YTPs\n({NullscapeInBB.UpgradesCount(i2.item.ConvertType<UpgradeItem>().curse)}/{NullscapeInBB.Instance.AllUpgrades[i2.item.ConvertType<UpgradeItem>().curse].MaxUpgrades})";
            i3.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_{Enum.GetName(typeof(Upgrades), i3.item.ConvertType<UpgradeItem>().curse)}")}\n{Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_upgcost")} {NullscapeInBB.Instance.AllUpgrades[i3.item.ConvertType<UpgradeItem>().curse].Cost()} YTPs\n({NullscapeInBB.UpgradesCount(i3.item.ConvertType<UpgradeItem>().curse)}/{NullscapeInBB.Instance.AllUpgrades[i3.item.ConvertType<UpgradeItem>().curse].MaxUpgrades})";
            PickUp1.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp1.transform.localPosition = new Vector3(340, 5, 57.5f);
            PickUp2.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp2.transform.localPosition = new Vector3(335, 5, 60);
            PickUp3.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp3.transform.localPosition = new Vector3(330, 5, 57.5f);

            PickUp1.GetComponent<Pickup>().free = false;
            PickUp1.GetComponent<Pickup>().price = NullscapeInBB.Instance.AllUpgrades[enemItem1.curse].Cost();
            PickUp2.GetComponent<Pickup>().free = true;
            PickUp2.GetComponent<Pickup>().price = 0;
            PickUp3.GetComponent<Pickup>().free = false;
            PickUp3.GetComponent<Pickup>().price = NullscapeInBB.Instance.AllUpgrades[enemItem3.curse].Cost();

            var Reroll = Object.Instantiate(PickupPrefab, new Vector3(60, -0f, -45f), Quaternion.identity);
            Reroll.GetComponent<Pickup>().showDescription = true;
            NullscapeInBB.Instance.RerollItem.descKey = $"desc_reroll_unlimited";

            Reroll.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.RerollItem);

            Reroll.GetComponent<Pickup>().free = true;


            Reroll.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            Reroll.transform.localPosition = new Vector3(350, 5, 57.5f);
            var renderer = ObjectCreation.MakeRenderer(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay), Singleton<BaseGameManager>.Instance.Ec.transform, true, "IntermissionRenderer_Enemies");
            renderer.sprite = NullscapeInBB.Am.Get<Sprite>("spr_intupgrade");
            renderer.transform.localPosition = new Vector3(335, 12.5f, 60);
            renderer.gameObject.AddComponent<IntermissionMovementStuff>();
            Reroll.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {

                Object.Destroy(PickUp1);
                Object.Destroy(PickUp2);
                Object.Destroy(PickUp3);
                Object.Destroy(renderer.gameObject);
                Object.Destroy(Reroll);
                NullscapeInBB.delay(() =>
                {
                    var random3 = NullscapeInBB.Instance.PickThreeUpgrades();
                    if (random3.Count > 0)
                    {
                        CreatePickupsUpgrade(NullscapeInBB.Instance.AllUpgradesItems[random3[0].curse], NullscapeInBB.Instance.AllUpgradesItems[random3[1].curse], NullscapeInBB.Instance.AllUpgradesItems[random3[2].curse]);
                    } else {
                        Extras.NextIntermission();
                    }
                }, 0.5f);

            };

            
            

            // Add a small 3D text label above the middle pedestal indicating the intermission type



            // I do not even know what the fucking point of pckup is, I DONT EVEN USE IT :sob:
            PickUp1.GetComponent<Pickup>().OnItemPurchased += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject, Reroll }, pckup, enemItem1);
                
            };

            PickUp2.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                Object.Destroy(PickUp1);
                Object.Destroy(PickUp2);
                Object.Destroy(PickUp3);
                Object.Destroy(renderer.gameObject);
                Object.Destroy(Reroll);
                NullscapeInBB.delay(() =>
                {
                    Extras.NextIntermission();
                }, 0.5f);
            };
                
            PickUp3.GetComponent<Pickup>().OnItemPurchased += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject, Reroll }, pckup, enemItem3);
            };
            ;
        }

    }
}
