using MTM101BaldAPI.OptionsAPI;
using NullscapeinBB.Save;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static NullscapeinBB.Save.SaveHelper;

namespace NullscapeinBB
{
    public interface IPage { }
    public class BasePage<T>(NullbasicsStats stat) : IPage {
        
        internal NullbasicsStats stats = stat;
        public List<T> Values = new();
        public virtual void CreateValue(int i, T item) {

        }
    }

    internal class FloorReachedPage(NullbasicsStats stat) : BasePage<TypesOfSaves>(stat) {
        public override void CreateValue(int i, TypesOfSaves item)
        {
            var e = SaveHelper.GetSaveKeyFromEnum(item);
            stats.Texts.Add(stats.CreateNewText("idkBro", $"{Singleton<LocalizationManager>.Instance.GetLocalizedText($"stat_{e}")}{NullscapeInBB.CurrentSave.GetInt(e)}", $"desc_{e}", i));
        }
    }
    internal class OtherStatsPage(NullbasicsStats stat) : BasePage<int>(stat)
    {
        public override void CreateValue(int i, int item)
        {

            stats.Texts.Add(stats.CreateNewText("idkBro", $"{Singleton<LocalizationManager>.Instance.GetLocalizedText($"stat_MiscStat{item}")}{NullscapeInBB.CurrentSave.GetInt($"MiscStat_{item}")}", $"desc_MiscStat{item}", i));
        }
    }
    public class NullbasicsStats : CustomOptionsCategory
    {
        public int page = 0;

        public List<TextMeshProUGUI> Texts = new();

        public Dictionary<int,IPage> Pages = new();

        public TextMeshProUGUI PageText;

        public void UpdateSettings(int NextPage) {
            

            page = Mathf.Clamp(NextPage,0,99);
            foreach (var item in Texts)
            {
                Destroy(item.gameObject);
            }
            Texts.Clear();
            PageText.text = $"{page + 1}/{Pages.Count}";
            var Uhh = Pages[page];
                var i = 0;
                if (Uhh is FloorReachedPage page1)
                {
                    foreach (var itm in page1.Values)
                    {
                        page1.CreateValue(i, itm);
                        i++;
                    }
                }
                if (Uhh is OtherStatsPage page2)
                {
                    foreach (var itm in page2.Values)
                    {
                        page2.CreateValue(i, itm);
                        i++;
                    }
                }
            
        }

        public override void Build()
        {
            var allSaves = SaveHelper.GetListOfAllPossibleSaves();

            var PagesAmountSaves = Mathf.CeilToInt(allSaves.Count / 8f);
            var CurrentTotalPage = 0;
            for (int i = 0; i < PagesAmountSaves; i++)
            {
                
                Pages.Add(CurrentTotalPage, new FloorReachedPage(this));

                for (int j = 8 * i; j < 8 + 8 * i && j < allSaves.Count; j++)
                {
                    (Pages[CurrentTotalPage] as FloorReachedPage).Values.Add(allSaves[j]);
                }
                CurrentTotalPage++;
            }

            Pages.Add(CurrentTotalPage, new OtherStatsPage(this));
            (Pages[CurrentTotalPage] as OtherStatsPage).Values.Add(0);
            (Pages[CurrentTotalPage] as OtherStatsPage).Values.Add(1);
            CurrentTotalPage++;


            PageText = CreateNewText2("idkBro2", $"{page+1}/{Pages.Count}",2);
            CreateButton(() => UpdateSettings((page + 1) % Pages.Count), menuArrowRight, menuArrowRightHighlight, "Next", new(75, -150, 0));
            CreateButton(() => UpdateSettings((page - 1) % Pages.Count), menuArrowLeft, menuArrowLeftHighlight, "Next", new(-75, -150, 0));
            UpdateSettings(0);
        }
        public TextMeshProUGUI CreateNewText2(string id, string name, int order)
        {
            var toggle = CreateText(id, name,new Vector3(0, -150, 0), MTM101BaldAPI.UI.BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new(400, 32), Color.black, false);
            

            return toggle;
        }
        public TextMeshProUGUI CreateNewText(string id, string name,string descKey, int order)
        {
            Vector3 originVec = new Vector3(0f, 85f, 0f);
            var toggle = CreateText(id, name, originVec + new Vector3(0, order * -29.5f, 0),MTM101BaldAPI.UI.BaldiFonts.ComicSans18, TextAlignmentOptions.Center, new(400,32),Color.black,false);
            this.AddTooltipRegion("eee", originVec + new Vector3(0, order * -29.5f, 0), new(400, 32), descKey);

            return toggle;
        }

        
    }
}
