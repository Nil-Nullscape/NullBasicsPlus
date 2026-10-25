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
    public static  class DelusionsIntermission
    {
        public static void OnCollectIntermission(GameObject[] pickups, Pickup currpck, DelusionsItem ei)
        {
            PatchesJohn.IsSavingPossible = false;
            var npc = NullscapeInBB.Instance.AllDelusions[ei.curse].curse;
            // Only add the NPC once (avoid duplicates across floors)
            var pot = NullscapeInBB.Instance.DelusionsPicked;

            // Increment selected count for this character so PickThreeEnemies respects MaxEnemies
            pot.Add(NullscapeInBB.Instance.AllDelusions[npc]);
            NullscapeInBB.Instance.AllDelusions[npc].CursesAction();
            foreach (var item in pickups)
            {
                Object.Destroy(item);
            }
            NullscapeInBB.delay(() => { Extras.NextIntermission(); }, 0.85f);
        }

        public static void CreatePickupsDelusions(ItemObject i1, ItemObject i2, ItemObject i3)
        {
            GameObject PickupPrefab = Resources.FindObjectsOfTypeAll<GameObject>().First(x => x.name == "Pickup" && x.GetInstanceID() > 0);
            var PickUp1 = Object.Instantiate(PickupPrefab, new Vector3(35, -0f, -40), Quaternion.identity);
            PickUp1.GetComponent<Pickup>().showDescription = true;
            PickUp1.GetComponent<Pickup>().AssignItem(i1);
            var PickUp2 = Object.Instantiate(PickupPrefab, new Vector3(30, -0f, -42.5f), Quaternion.identity);
            PickUp2.GetComponent<Pickup>().showDescription = true;
            PickUp2.GetComponent<Pickup>().AssignItem(i2);
            var PickUp3 = Object.Instantiate(PickupPrefab, new Vector3(40, -0f, -42.5f), Quaternion.identity);
            PickUp3.GetComponent<Pickup>().showDescription = true;
            PickUp3.GetComponent<Pickup>().AssignItem(i3);
            DelusionsItem enemItem1 = (DelusionsItem)i1.item;
            DelusionsItem enemItem2 = (DelusionsItem)i2.item;
            DelusionsItem enemItem3 = (DelusionsItem)i3.item;
            PickUp1.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp1.transform.localPosition = new Vector3(335, 5, 60);
            PickUp2.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp2.transform.localPosition = new Vector3(340, 5, 57.5f);
            PickUp3.transform.parent = Singleton<BaseGameManager>.Instance.Ec.transform;
            PickUp3.transform.localPosition = new Vector3(330, 5, 57.5f);

            var renderer = ObjectCreation.MakeRenderer(UsefulHelpers.GetLayerFromEnum(UsefulHelpers.Layers.Overlay), Singleton<BaseGameManager>.Instance.Ec.transform, true, "IntermissionRenderer_Enemies");
            renderer.sprite = NullscapeInBB.Am.Get<Sprite>("spr_intdelusion");
            renderer.transform.localPosition = new Vector3(335, 12.5f, 60);
            renderer.gameObject.AddComponent<IntermissionMovementStuff>();

            // I do not even know what the fucking point of pckup is, I DONT EVEN USE IT :sob:
            PickUp1.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject }, pckup, enemItem1);
            };
            PickUp2.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject }, pckup, enemItem2);
            };
            PickUp3.GetComponent<Pickup>().OnItemCollected += (pckup, plr) =>
            {
                OnCollectIntermission(new GameObject[] { PickUp1, PickUp2, PickUp3, renderer.gameObject }, pckup, enemItem3);
            };
            ;
        }

    }
}
