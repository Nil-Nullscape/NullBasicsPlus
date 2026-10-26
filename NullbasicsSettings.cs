using MTM101BaldAPI.OptionsAPI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace NullscapeinBB
{
    public class NullbasicsSettings : CustomOptionsCategory
    {
        public override void Build()
        {
            var Chaos = CreateNewModifier("ChaosMode", "set_chaos", "desc_chaosmode", 2);
            var EnabledNullscapeEntities = CreateNewModifier("EnabledNullscapeEntities", "set_entities", "desc_entities", 3);
            var QuickStart = CreateNewModifier("QuickStartSetting", "set_QuickStart", "desc_QuickStart", 5);

            Chaos.Set(NullscapeInBB.Instance.EnableChaosMode.Value);
            EnabledNullscapeEntities.Set(NullscapeInBB.Instance.EnabledNullscapeEntities.Value);
            QuickStart.Set(NullscapeInBB.Instance.EnableQuickStart.Value);

            CreateApplyButton(() =>
            {
                NullscapeInBB.Instance.EnableChaosMode.Value = Chaos.Value;
                NullscapeInBB.Instance.EnabledNullscapeEntities.Value = EnabledNullscapeEntities.Value;
                NullscapeInBB.Instance.EnableQuickStart.Value = QuickStart.Value;
                NullscapeInBB.Instance.UpdateListOfEnemies();
            });

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
