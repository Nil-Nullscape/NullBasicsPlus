using MTM101BaldAPI.OptionsAPI;
using NullscapeinBB.Save;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TMPro;
using UnityEngine;
using static NullscapeinBB.Save.SaveHelper;
using Color = UnityEngine.Color;

namespace NullscapeinBB
{
    public class NullbasicsAchivement : CustomOptionsCategory
    {
        public override void Build()
        {
            AddAchivement(AchivementsNullBasics.Level25, 1);
            AddAchivement(AchivementsNullBasics.Level50, 2);

            AddAchivement(AchivementsNullBasics.Lap3, 3);
        }

        
        public void AddAchivement(AchivementsNullBasics achivement, int order) {
            var pos = new Vector3(0, 85 + order * -29.5f, 0);
            CreateText("ddd", $"achivement_{achivement}", pos, MTM101BaldAPI.UI.BaldiFonts.ComicSans18, TextAlignmentOptions.Center, new(300, 25), SaveHelper.IsAchivementCompleted(achivement) ? Color.green : Color.red);
            AddTooltipRegion("ddd", pos,new(300,25),$"desc_achivement_{achivement}");

        }
        
    }
}
