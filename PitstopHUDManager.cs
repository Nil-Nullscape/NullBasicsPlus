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
using UnityEngine.EventSystems;
using MTM101BaldAPI;

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

        KeyCode cursorToggleKey = NullscapeInBB.Instance.KeyMouseUnlock.Value;

        bool cursorUnlocked;
        GraphicRaycaster raycaster;
        EventSystem eventSystem;
        TooltipController tooltip;

        GameObject hoveredObject;

        public struct IconIntermission
        {
            public Sprite icon;
            public int amount;
            public string descriptionKey;
        }

        Dictionary<IntermissionType, List<IconIntermission>> ListOf = new();

        public class PitstopIconHover : MonoBehaviour
        {
            public string descriptionKey;
        }

        void Start()
        {
            var sss = new List<Image>();

            var p = AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 32, "items", "spr_random.png");

            for (int i = 1; i < 10; i++)
            {
                var go = new GameObject($"Slot_{i}");
                go.AddComponent<Image>();
                go.transform.SetParent(transform);

                var pos = new Vector2(0.1f + i * 0.08f, 0.63f);

                go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                go.GetComponent<RectTransform>().anchorMax = pos;
                go.GetComponent<RectTransform>().anchorMin = pos;

                go.GetComponent<Image>().sprite = p;
                go.GetComponent<Image>().raycastTarget = true;

                go.transform.localScale = Vector3.one * 0.015f;

                var txt = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "x1", go.transform, Vector3.zero);
                txt.name = "txt";
                txt.rectTransform.anchorMax = new(1.5f, 0.1f);
                txt.rectTransform.anchorMin = new(1.5f, 0.1f);
                txt.fontSize = 30;
                txt.raycastTarget = false;

                go.AddComponent<PitstopIconHover>();

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

            raycaster = GetComponentInParent<Canvas>()?.GetComponent<GraphicRaycaster>();
            eventSystem = EventSystem.current;
            tooltip = FindObjectOfType<TooltipController>();

            ListOf.Add(IntermissionType.Enemy, new());
            ListOf.Add(IntermissionType.Curse, new());
            ListOf.Add(IntermissionType.GreaterCurse, new());
            ListOf.Add(IntermissionType.Upgrades, new());
            ListOf.Add(IntermissionType.Delusions, new());

            UpdateContent();
            UpdateHud();
        }

        void ToggleCursor()
        {
            cursorUnlocked = !cursorUnlocked;

            if (cursorUnlocked)
            {
                Singleton<CursorManager>.Instance.UnlockCursor();

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Singleton<CursorManager>.Instance.LockCursor();

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                if (tooltip != null)
                    tooltip.CloseTooltip();

                hoveredObject = null;
            }
        }

        void UpdateHover()
        {
            if (!cursorUnlocked || raycaster == null || eventSystem == null)
                return;

            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new();
            raycaster.Raycast(pointerData, results);

            GameObject currentObject = null;

            foreach (var result in results)
            {
                var hover = result.gameObject.GetComponent<PitstopIconHover>();

                if (hover != null)
                {
                    currentObject = result.gameObject;
                    break;
                }
            }

            if (hoveredObject != null && tooltip != null)
                tooltip.CloseTooltip();

            hoveredObject = currentObject;

            if (hoveredObject != null && tooltip != null)
            {
                var hover = hoveredObject.GetComponent<PitstopIconHover>();



                if (!string.IsNullOrEmpty(hover.descriptionKey))
                    tooltip.UpdateTooltip(hover.descriptionKey);
            }
        }

        void UpdateContent()
        {
            List<IconIntermission> TempList = new();

            TempList.Clear();

            foreach (var item in NullscapeInBB.Instance.selectedNPCCounts)
            {
                var icon = NullscapeInBB.Instance.AllEnemiesItems[item.Key].itemSpriteLarge;
                var amount = item.Value;

                TempList.Add(new()
                {
                    icon = icon,
                    amount = amount,
                    descriptionKey = Singleton<LocalizationManager>.Instance.GetLocalizedText($"desc_{NullscapeInBB.Instance.AllEnemies[item.Key].NPCToUse.name}")
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
                    amount = amount,
                    descriptionKey = Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_" + item.Value.curse.ToStringExtended())
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
                    amount = amount,
                    descriptionKey = Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_" + item.Value.curse.ToStringExtended())
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
                    amount = amount,
                    descriptionKey = Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_" + item.Value.curse.ToStringExtended())
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
                    amount = amount,
                    descriptionKey = Singleton<LocalizationManager>.Instance.GetLocalizedText("desc_" + item.Value.curse.ToStringExtended())
                });
            }

            ListOf[IntermissionType.Upgrades] = TempList.ToList();
        }

        int ListsWithContent()
        {
            var a = 0;

            foreach (var item in ListOf)
            {
                if (item.Value.Count > 0) a++;
            }

            return a;
        }

        void UpdateHud()
        {
            UpdateContent();

            int totalContentLists = ListsWithContent();
            if (totalContentLists == 0)
            {
                currentIntermission = -1;
                CurrentPage = 0;
                if (text != null)
                    text.text = "You have nothing.";
                foreach (var slot in Slots)
                    slot.gameObject.SetActive(false);
                return;
            }

            if (Done.Count >= totalContentLists)
                Done.Clear();

            bool needsSwitch = currentIntermission < 0;

            if (!needsSwitch)
            {
                IntermissionType currentType = IntermissionsSeeable[currentIntermission];
                int totalItems = ListOf[currentType].Count;
                int maxPage = Mathf.Max(0, Mathf.CeilToInt(totalItems / 9f) - 1);

                if (totalItems == 0 || CurrentPage > maxPage)
                {
                    needsSwitch = true;
                    CurrentPage = 0;
                }
            }

            if (needsSwitch)
            {
                bool found = false;
                for (int i = 0; i < IntermissionsSeeable.Length; i++)
                {
                    IntermissionType type = IntermissionsSeeable[i];
                    if (ListOf[type].Count > 0 && !Done.Contains(type))
                    {
                        currentIntermission = i;
                        CurrentPage = 0;
                        Done.Add(type);
                        found = true;
                        break;
                    }
                }

                if (!found && Done.Count > 0)
                {
                    Done.Clear();
                    for (int i = 0; i < IntermissionsSeeable.Length; i++)
                    {
                        IntermissionType type = IntermissionsSeeable[i];
                        if (ListOf[type].Count > 0)
                        {
                            currentIntermission = i;
                            CurrentPage = 0;
                            Done.Add(type);
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (currentIntermission >= 0 && currentIntermission < IntermissionsSeeable.Length)
            {
                IntermissionType currentType = IntermissionsSeeable[currentIntermission];
                text.text = Singleton<LocalizationManager>.Instance.GetLocalizedText($"title_{currentType.ToString()}");

                int startIndex = 9 * CurrentPage;
                int currentSlot = 0;

                for (int i = startIndex; i < startIndex + 9; i++)
                {
                    if (i < ListOf[currentType].Count)
                    {
                        var item = ListOf[currentType][i];

                        Slots[currentSlot].gameObject.SetActive(true);
                        Slots[currentSlot].sprite = item.icon;

                        Slots[currentSlot].transform.Find("txt").GetComponent<TextMeshProUGUI>().text = $"x{item.amount}";

                        var hover = Slots[currentSlot].GetComponent<PitstopIconHover>();
                        hover.descriptionKey = item.descriptionKey;
                    }
                    else
                    {
                        Slots[currentSlot].gameObject.SetActive(false);
                    }

                    currentSlot++;
                }

                CurrentPage++;
            }
        }
        void Update()
        {
            if (Input.GetKeyDown(cursorToggleKey))
                ToggleCursor();

            UpdateHover();

            timeRemainingBeforeSwitched -= Time.deltaTime;

            timeAnimation = Mathf.Clamp(timeAnimation - Time.deltaTime, 0, 2);

            var e = Vector2.Lerp(new(0.5f, 1.1f), new(0.5f, 0.9f), 1 - timeAnimation);

            GetComponent<RectTransform>().anchorMax = e;
            GetComponent<RectTransform>().anchorMin = e;

            if (timeRemainingBeforeSwitched < 0)
            {
                UpdateHud();
                timeRemainingBeforeSwitched = 2;
            }
        }

        void OnDisable()
        {
            if (cursorUnlocked)
            {
                Singleton<CursorManager>.Instance.LockCursor();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                cursorUnlocked = false;
            }

            if (tooltip != null)
                tooltip.CloseTooltip();
        }
    }
}
