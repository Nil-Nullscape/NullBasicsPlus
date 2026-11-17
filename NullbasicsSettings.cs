using MTM101BaldAPI.OptionsAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace NullscapeinBB
{
    public class NullbasicsSettings : CustomOptionsCategory
    {
        public override void Build()
        {
            var Chaos = CreateNewModifier("ChaosMode", "set_chaos", "desc_chaosmode", 1);
            var EnabledNullscapeEntities = CreateNewModifier("EnabledNullscapeEntities", "set_entities", "desc_entities", 2);
            

            Chaos.Set(NullscapeInBB.Instance.EnableChaosMode.Value);
            EnabledNullscapeEntities.Set(NullscapeInBB.Instance.EnabledNullscapeEntities.Value);

            var QuickStart = CreateNewModifier("QuickStartSetting", "set_QuickStart", "desc_QuickStart", 4);
            QuickStart.Set(NullscapeInBB.Instance.EnableQuickStart.Value);

            var QuickStartPlus = CreateNewModifier("QuickStartSetting", "set_QuickStartPlus", "desc_QuickStartPlus", 5);
            QuickStartPlus.Set(NullscapeInBB.Instance.EnableQuickStart.Value);
            if (NullscapeInBB.CurrentSave.GetInt("MaxLevelNormalReached", 0) < 35) QuickStartPlus.Set(false);
            CreateApplyButton(() =>
            {
                NullscapeInBB.Instance.EnableChaosMode.Value = Chaos.Value;
                NullscapeInBB.Instance.EnabledNullscapeEntities.Value = EnabledNullscapeEntities.Value;
                if (NullscapeInBB.CurrentSave.GetInt("MaxLevelNormalReached", 0) < 35) QuickStartPlus.Set(false);
                NullscapeInBB.Instance.EnableQuickStartPlus.Value = QuickStartPlus.Value;
                NullscapeInBB.Instance.EnableQuickStart.Value = QuickStart.Value;

               
                GenerationStuff.GlobalGenStuff();
                GenerationStuff.GenStuff(NullscapeInBB.Instance.F1);
                GenerationStuff.GenStuff(NullscapeInBB.Instance.F2);
                NullscapeInBB.Instance.UpdateListOfEnemies();
            });

            CreateUneditableConfigText($"{Singleton<LocalizationManager>.Instance.GetLocalizedText("set_AbilityKey")}{NullscapeInBB.Instance.KeyAbility.Value}", "desc_noModify", 6);
            CreateUneditableConfigText($"{Singleton<LocalizationManager>.Instance.GetLocalizedText("set_UnlockPitstopKey")}{NullscapeInBB.Instance.KeyMouseUnlock.Value}", "desc_noModify", 7);

        }

        public void CreateUneditableConfigText(string text, string tooltip, int order) {
            var pos = new Vector3(0, 85 + order * -29.5f, 0);
            CreateText("idk", text, pos, MTM101BaldAPI.UI.BaldiFonts.ComicSans18, TextAlignmentOptions.Center, new(300, 32), Color.black, false);
            AddTooltipRegion("idk",pos,new(300,32),tooltip);
        }

        public MenuToggle CreateNewModifier(string id, string name, string tooltipKey, int order)
        {
            Vector3 originVec = new Vector3(40f, 85f, 0f);
            var toggle = CreateToggle(id, name, false, originVec + new Vector3(0, order * -29.5f, 0), 250f);
            AddTooltip(toggle, tooltipKey);

            return toggle;
        }

        public AdjustmentBars CreateNewModifierBar(string id, string name, string tooltipKey, int order, int length, Color textColor)
        {
            Vector3 originVec = new Vector3(40f, 85f, 0f);
            var bar = CreateBars(() => {

            }, id, originVec + new Vector3(-30, order * -29.5f, 0), length);
            CreateText(id + "_text", name, originVec + new Vector3(-125, order * -28, 0), MTM101BaldAPI.UI.BaldiFonts.ComicSans24, TextAlignmentOptions.Left, new Vector2(250, 10), textColor, false);
            AddTooltip(bar, tooltipKey);

            return bar;
        }
    }
}
