using MTM101BaldAPI.OptionsAPI;
using NullscapeinBB.Save;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace NullscapeinBB
{
    public class NullbasicsStats : CustomOptionsCategory
    {
        public override void Build()
        {
            CreateNewText("idkBro", $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("stat_FloorReachedNormal")}{NullscapeInBB.CurrentSave.GetInt(SaveHelper.GetSaveKeyFromEnum(SaveHelper.TypesOfSaves.Entities))}", 1);
            CreateNewText("idkBro2", $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("stat_FloorReachedQuickNormal")}{NullscapeInBB.CurrentSave.GetInt(SaveHelper.GetSaveKeyFromEnum(SaveHelper.TypesOfSaves.Entities | SaveHelper.TypesOfSaves.QuickPlus))}", 2);

        }

        public TextMeshProUGUI CreateNewText(string id, string name, int order)
        {
            Vector3 originVec = new Vector3(0f, 85f, 0f);
            var toggle = CreateText(id, name, originVec + new Vector3(0, order * -31.5f, 0),MTM101BaldAPI.UI.BaldiFonts.ComicSans12, TextAlignmentOptions.Center, new(400,32),Color.black,false);
            

            return toggle;
        }

        
    }
}
