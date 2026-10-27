using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullscapeinBB
{
    internal class PitstopHUDManager : MonoBehaviour
    {
        float timeAnimation = 1f;
        float timeRemainingBeforeSwitched = 4f;
        IntermissionType[] IntermissionsSeeable = [IntermissionType.Enemy, IntermissionType.Curse, IntermissionType.GreaterCurse, IntermissionType.Delusions, IntermissionType.Upgrades];
        public int currentIntermission = -1;
        public int CurrentPage = 0;

        public List<IntermissionType> Done = new();
        TextMeshProUGUI text;
        Image[] Slots = [];


        public struct IconIntermission
        {
            public Sprite icon;
            public int amount;
        }

        Dictionary<IntermissionType, List<IconIntermission>> ListOf = new();



        void Start() {
            var sss = new List<Image>();
            var p = AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 32, "items", "spr_random.png");
            for (int i = 1  ; i < 10; i++)
            {
                var go = new GameObject($"Slot_{i}");
                go.AddComponent<Image>();
                go.transform.SetParent(transform);
                var pos = new Vector2(0.1f + i * 0.08f, 0.63f);
                go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                go.GetComponent<RectTransform>().anchorMax = pos;
                go.GetComponent<RectTransform>().anchorMin = pos;
                go.GetComponent<Image>().sprite = p;
                go.transform.localScale = Vector3.one * 0.015f;
                var txt = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "x1", go.transform, Vector3.zero);
                txt.name = "txt";
                txt.rectTransform.anchorMax = new(1.5f, 0.1f);
                txt.rectTransform.anchorMin = new(1.5f, 0.1f);
                txt.fontSize = 30;
                sss.Add(go.GetComponent<Image>());
                go.SetActive(false);
            }
            text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "placeholder", transform, Vector3.zero);
            text.alignment = TextAlignmentOptions.Center;
            text.rectTransform.anchorMax = new(0.5f, 0.9f);
            text.rectTransform.anchorMin = new(0.5f, 0.9f);
            text.transform.localScale = Vector3.one * 0.075f;
            text.color = Color.white;
            text.text = "You have nothing.";
            
            Slots = sss.ToArray();
            ListOf.Add(IntermissionType.Enemy, new());
            ListOf.Add(IntermissionType.Curse, new());

            ListOf.Add(IntermissionType.GreaterCurse, new());
            ListOf.Add(IntermissionType.Upgrades, new());
            ListOf.Add(IntermissionType.Delusions, new());
            UpdateContent();
            UpdateHud();
        }
        void UpdateContent() {
            List<IconIntermission> TempList = new();
            TempList.Clear();
            foreach (var item in NullscapeInBB.Instance.selectedNPCCounts)
            {
                var icon = NullscapeInBB.Instance.AllEnemiesItems[item.Key].itemSpriteLarge;
                var amount = item.Value;
                TempList.Add(new()
                {
                    icon = icon,
                    amount = amount
                });
            }
            ListOf[IntermissionType.Enemy] = TempList.ToList();
            TempList.Clear();
            foreach (var item in NullscapeInBB.Instance.AllCurses)
            {
                var icon = NullscapeInBB.Instance.AllCursesItems[item.Value.curse].itemSpriteLarge;
                var amount = NullscapeInBB.CurseCount(item.Value.curse);
                if (amount == 0) continue;
                TempList.Add(new()
                {
                    icon = icon,
                    amount = amount
                });
            }
            ListOf[IntermissionType.Curse] = TempList.ToList();
            TempList.Clear();
            foreach (var item in NullscapeInBB.Instance.AllGreaterCurses)
            {
                var icon = NullscapeInBB.Instance.AllGreaterCursesItems[item.Value.curse].itemSpriteLarge;
                var amount = NullscapeInBB.GreaterCurseCount(item.Value.curse);
                if (amount == 0) continue;
                TempList.Add(new()
                {
                    icon = icon,
                    amount = amount
                });
            }
            ListOf[IntermissionType.GreaterCurse] = TempList.ToList();
            TempList.Clear();
            foreach (var item in NullscapeInBB.Instance.AllDelusions)
            {
                var icon = NullscapeInBB.Instance.AllDelusionsItems[item.Value.curse].itemSpriteLarge;
                var amount = NullscapeInBB.DelusionsCount(item.Value.curse);
                if (amount == 0) continue;
                TempList.Add(new()
                {
                    icon = icon,
                    amount = amount
                });
            }
            ListOf[IntermissionType.Delusions] = TempList.ToList();
            TempList.Clear();
            foreach (var item in NullscapeInBB.Instance.AllUpgrades)
            {
                var icon = NullscapeInBB.Instance.AllUpgradesItems[item.Value.curse].itemSpriteLarge;
                var amount = NullscapeInBB.UpgradesCount(item.Value.curse);
                if (amount == 0) continue;
                TempList.Add(new()
                {
                    icon = icon,
                    amount = amount
                });
            }
            ListOf[IntermissionType.Upgrades] = TempList.ToList();

        }
        int ListsWithContent() {
            var a = 0;
            foreach (var item in ListOf)
            {
                if (item.Value.Count > 0) a++;

            }
            return a;
        }
        void UpdateHud() {
            UpdateContent();
            int pages = -1;
            bool skipLoop = false;
            if (currentIntermission >= 0) {
                pages = Mathf.FloorToInt(ListOf[IntermissionsSeeable[currentIntermission]].Count / 9f);
                if (CurrentPage < pages) skipLoop = true;
            }
            if (Done.Count >= ListsWithContent()) Done.Clear();

            for (int i = 0; i < IntermissionsSeeable.Length; i++)
            {
                if (skipLoop) continue;
                if (ListOf[IntermissionsSeeable[i]].Count > 0 && currentIntermission != i && !Done.Contains(IntermissionsSeeable[i])) {
                    CurrentPage = 0;
                    currentIntermission = i;
                    Done.Add(IntermissionsSeeable[i]);
                    text.text = Singleton<LocalizationManager>.Instance.GetLocalizedText($"title_{IntermissionsSeeable[i].ToString()}");
                    break;
                }
            }

            if (currentIntermission >= 0) {
                pages = Mathf.FloorToInt(ListOf[IntermissionsSeeable[currentIntermission]].Count / 9f);
                int currentSlot = 0;
                for (int i = 9 * CurrentPage; i < 9 + 9 * CurrentPage; i++)
                {
                    if (i < ListOf[IntermissionsSeeable[currentIntermission]].Count) {
                        Slots[currentSlot].gameObject.SetActive(true);
                        Slots[currentSlot].sprite = ListOf[IntermissionsSeeable[currentIntermission]][i].icon;
                        Slots[currentSlot].transform.Find("txt").GetComponent<TextMeshProUGUI>().text = $"x{ListOf[IntermissionsSeeable[currentIntermission]][i].amount}";
                    } else {
                        Slots[currentSlot].gameObject.SetActive(false);
                    }
                    currentSlot++;
                }
                CurrentPage = (CurrentPage + 1) % (pages + 1);

            }
        }

        void Update() {

            timeRemainingBeforeSwitched -= Time.deltaTime;
            timeAnimation = Mathf.Clamp(timeAnimation - Time.deltaTime,0,2);
            var e = Vector2.Lerp(new(0.5f, 1.1f), new(0.5f, 0.9f), 1 - timeAnimation);
            GetComponent<RectTransform>().anchorMax = e;
            GetComponent<RectTransform>().anchorMin = e;
            if (timeRemainingBeforeSwitched < 0) {
                UpdateHud();
                timeRemainingBeforeSwitched = 2;
            }
            
        }
    }
}
