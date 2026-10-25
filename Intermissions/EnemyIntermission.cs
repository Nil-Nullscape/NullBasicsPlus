using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using NilLib;
using NilLib.Creators;
using NullscapeinBB.Intermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NullscapeinBB
{
    public static class EnemyIntermission
    {

        public static void OnCollectIntermission(GameObject[] pickups, Pickup currpck, EnemItem ei, bool isitquestionmark = false)
        {
            PatchesJohn.IsSavingPossible = false;
            var npc = NullscapeInBB.Instance.AllEnemies[ei.Enemy].NPCToUse;
            // Only add the NPC once (avoid duplicates across floors)
            var pot = NullscapeInBB.Instance.potentialNPCs;
            
            // Increment selected count for this character so PickThreeEnemies respects MaxEnemies
            var counts = NullscapeInBB.Instance.selectedNPCCounts;
            if (!isitquestionmark)
            {
                if (counts.ContainsKey(ei.Enemy)) counts[ei.Enemy]++;
                else counts[ei.Enemy] = 1;
                try
                {
                }
                catch { }
                if (counts[ei.Enemy] <= NullscapeInBB.Instance.AllEnemies[ei.Enemy].MaxEnemies)
                {
                    pot.Add(npc);
                }
            }
            else
            {
                var allNpcsL = NullscapeInBB.Instance.AllEnemies.Values.ToList();
                allNpcsL.RemoveAll(x => x.Enemy == EnumExtensions.GetFromExtendedName<Character>("???"));
                var allNpcs = allNpcsL.ToArray();
                var randomNPC = allNpcs[UnityEngine.Random.Range(0, allNpcs.Length)].NPCToUse;
                pot.Add(randomNPC);
            }
            foreach (var item in pickups)
            {
                Object.Destroy(item);
            }
            NullscapeInBB.delay(() => { Extras.NextIntermission(); }, 0.85f);
        }
        public static void CreatePickupsEnemy(ItemObject i1, ItemObject i2, ItemObject i3, bool isitquestionmark = false)
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup" && x.GetInstanceID() > 0);
            var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
            PickUp1.GetComponent<Pickup>().showDescription = true;
            PickUp1.GetComponent<Pickup>().AssignItem(i1);
            if (isitquestionmark) PickUp1.GetComponent<Pickup>().itemSprite.sprite = AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 35, "items", "placeholder.png");
            var PickUp2 = Object.Instantiate(PickupPrefab, new Vector3(30, -0f, -42.5f), Quaternion.identity);
            PickUp2.GetComponent<Pickup>().showDescription = true;
            PickUp2.GetComponent<Pickup>().AssignItem(i2);
            if (isitquestionmark) PickUp2.GetComponent<Pickup>().itemSprite.sprite = AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 35, "items", "placeholder.png");
            var PickUp3 = Object.Instantiate(PickupPrefab, new Vector3(40, -0f, -42.5f), Quaternion.identity);
            PickUp3.GetComponent<Pickup>().showDescription = true;
            PickUp3.GetComponent<Pickup>().AssignItem(i3);
            if (isitquestionmark) PickUp3.GetComponent<Pickup>().itemSprite.sprite = AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 35, "items", "placeholder.png");
            EnemItem enemItem1 = (EnemItem)i1.item;
            EnemItem enemItem2 = (EnemItem)i2.item;
            EnemItem enemItem3 = (EnemItem)i3.item;
            PickUp1.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp1.transform.localPosition = new Vector3(335, 5, 60);
            PickUp2.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp2.transform.localPosition = new Vector3(340, 5, 57.5f);
            PickUp3.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp3.transform.localPosition = new Vector3(330, 5, 57.5f);
            var Reroll = Object.Instantiate(PickupPrefab, new Vector3(60, -0f, -45f), Quaternion.identity);
            Reroll.GetComponent<Pickup>().showDescription = true;
            if (NullscapeInBB.Instance.freeRerolls > 0)
            {
                NullscapeInBB.Instance.RerollItem.descKey = $"desc_reroll_free{NullscapeInBB.Instance.freeRerolls}";

            }
            else
            {
                NullscapeInBB.Instance.RerollItem.descKey = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_reroll")} {NullscapeInBB.Instance.rerollPrice} {Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_reroll2")}";
            }
            Reroll.GetComponent<Pickup>().AssignItem(NullscapeInBB.Instance.RerollItem);

            if (NullscapeInBB.Instance.freeRerolls > 0)
            {
                Reroll.GetComponent<Pickup>().free = true;

            }
            else
            {
                Reroll.GetComponent<Pickup>().free = false;
                Reroll.GetComponent<Pickup>().price = NullscapeInBB.Instance.rerollPrice;

            }

            Reroll.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            Reroll.transform.localPosition = new Vector3(350, 5, 57.5f);
            // Add a small 3D text label above the middle pedestal indicating the intermission type
            var renderer = ObjectCreation.MakeRenderer(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay), Singleton<BaseGameManager>.Instance.Ec.transform, true, "IntermissionRenderer_Enemies");
            renderer.sprite = NullscapeInBB.Am.Get<Sprite>("spr_intenemy");
            renderer.transform.localPosition = new Vector3(335, 12.5f, 60);
            renderer.gameObject.AddComponent<IntermissionMovementStuff>();
            Reroll.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {

                if (NullscapeInBB.Instance.freeRerolls > 0)
                {
                    NullscapeInBB.Instance.freeRerolls -= 1;
                }
                else
                {
                    NullscapeInBB.Instance.rerollPrice += 100;
                }
                Object.Destroy(PickUp1);
                Object.Destroy(PickUp2);
                Object.Destroy(PickUp3);
                Object.Destroy(renderer.gameObject);
                Object.Destroy(Reroll);
                NullscapeInBB.delay(() =>
                {
                    var random3 = NullscapeInBB.Instance.PickThreeEnemies();
                    if (!isitquestionmark)
                    {
                        CreatePickupsEnemy(NullscapeInBB.Instance.AllEnemiesItems[random3[0].Enemy], NullscapeInBB.Instance.AllEnemiesItems[random3[1].Enemy], NullscapeInBB.Instance.AllEnemiesItems[random3[2].Enemy], isitquestionmark);
                    }
                    else
                    {
                        CreatePickupsEnemy(i1, i2, i3, isitquestionmark);
                    }
                }, 0.5f);

            };

            // I do not even know what the fucking point of pckup is, I DONT EVEN USE IT :sob:
            PickUp1.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject, Reroll }, pckup, enemItem1, isitquestionmark);
            };
            PickUp2.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject, Reroll }, pckup, enemItem2, isitquestionmark);
            };
            PickUp3.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject, Reroll }, pckup, enemItem3, isitquestionmark);
            };
            ;
        }



    }
}
