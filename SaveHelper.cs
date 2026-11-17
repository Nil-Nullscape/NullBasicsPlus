using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullscapeinBB.Save
{
    public static class SaveHelper
    {
        [Flags]
        public enum TypesOfSaves
        {
            None = 1 << 0,
            Chaos = 1 << 1,
            Entities = 1 << 2,
            QuickPlus = 1 << 3,
            Cheating = 1 << 4,
        }

        public enum AchivementsNullBasics
        {
            Level25,
            Level50,
            Lap3

        }

        public static void SetAchivement(AchivementsNullBasics achivement, bool val) {
            NullscapeInBB.CurrentSave.SetData($"Achivement_{achivement}", true);
        }
        public static bool IsAchivementCompleted(AchivementsNullBasics achivement) {
            return NullscapeInBB.CurrentSave.GetBool($"Achivement_{achivement}", false);
        }


        public static string GetSaveKeyFromEnum(TypesOfSaves enumThingie) {
            var chaos = enumThingie.HasFlag(TypesOfSaves.Chaos);
            var quick = NullscapeInBB.Instance.EnableQuickStart.Value;
            var quickplus = enumThingie.HasFlag(TypesOfSaves.QuickPlus);
            var Entities = enumThingie.HasFlag(TypesOfSaves.Entities);
            var Cheating = enumThingie.HasFlag(TypesOfSaves.Cheating);
            if (Cheating) return "MaxLevelReachedWithCheating";
            if (!chaos && !quickplus && Entities) return "MaxLevelNormalReached";
            if (!chaos && quickplus && Entities) return "MaxLevelNormalQuickStartPlusReached";
            if (!chaos && !quickplus && !Entities) return "MaxLevelNormalWithoutEntitiesReached";
            if (!chaos && quickplus && !Entities) return "MaxLevelNormalWithoutEntitiesQuickStartPlusReached";
            if (chaos && !quickplus && Entities) return "MaxLevelChaosReached";
            if (chaos && quickplus && Entities) return "MaxLevelChaosQuickStartPlusReached";
            if (chaos && !quickplus && !Entities) return "MaxLevelChaosWithoutEntitiesReached";
            if (chaos && quickplus && !Entities) return "MaxLevelChaosWithoutEntitiesQuickStartPlusReached";
            return "MaxLevelReachedWithCheating";
        }

        public static List<TypesOfSaves> GetListOfAllPossibleSaves() {
            return new() {
                TypesOfSaves.Entities,
                TypesOfSaves.Entities | TypesOfSaves.QuickPlus,
                TypesOfSaves.None,
                TypesOfSaves.QuickPlus,
                TypesOfSaves.Chaos | TypesOfSaves.Entities,
                TypesOfSaves.Chaos | TypesOfSaves.Entities | TypesOfSaves.QuickPlus,
                TypesOfSaves.Chaos,
                TypesOfSaves.Chaos | TypesOfSaves.QuickPlus,
                TypesOfSaves.Cheating
            };
        }

        public static int GetMiscData(int Slot, int defaultVal = 0)
        {
            return NullscapeInBB.CurrentSave.GetInt($"MiscStat_{Slot}", defaultVal);
        }
        public static void SetMiscData(int Slot, int Val = 0)
        {
           NullscapeInBB.CurrentSave.SetData($"MiscStat_{Slot}", Val);
            NullscapeInBB.CurrentSave.Save();
        }




        /// <summary>
        /// What "level" is required to be able to get the achivement or unlockable content
        /// </summary>
        public enum LevelAchivement {
            /// The options must be strictly Entities, no chaos and no Quick start plus
            High,
            /// The options must be entities, and can be chaos and must not be quick start plus
            Med,
            /// The options can be entities, can be chaos and must not be quick start plus
            Low
        }

        public static bool CanGetAchivement(LevelAchivement levelRequired) {
            switch (levelRequired)
            {
                case LevelAchivement.High:
                    return NullscapeInBB.Instance.EnabledNullscapeEntities.Value && !NullscapeInBB.Instance.EnableChaosMode.Value && !NullscapeInBB.Instance.EnableQuickStartPlus.Value && !NullscapeInBB.GetCheatingMods();
                case LevelAchivement.Med:
                    return NullscapeInBB.Instance.EnabledNullscapeEntities.Value && !NullscapeInBB.Instance.EnableQuickStartPlus.Value && !NullscapeInBB.GetCheatingMods();
                case LevelAchivement.Low:
                    return !NullscapeInBB.Instance.EnableQuickStartPlus.Value && !NullscapeInBB.GetCheatingMods();
                default:
                    return false;
            }
        }

        public static string GetSaveKeyCurrentMode() {
            var chaos = NullscapeInBB.Instance.EnableChaosMode.Value;
            var quick = NullscapeInBB.Instance.EnableQuickStart.Value;
            var quickplus = NullscapeInBB.Instance.EnableQuickStartPlus.Value;
            var Entities = NullscapeInBB.Instance.EnabledNullscapeEntities.Value;
            var Cheating = NullscapeInBB.GetCheatingMods();
            if (Cheating) return GetSaveKeyFromEnum(TypesOfSaves.Cheating);
            if (!chaos && !quickplus && Entities) return GetSaveKeyFromEnum(TypesOfSaves.Entities);
            if (!chaos && quickplus && Entities) return GetSaveKeyFromEnum(TypesOfSaves.Entities | TypesOfSaves.QuickPlus);
            if (!chaos && !quickplus && !Entities) return GetSaveKeyFromEnum(TypesOfSaves.None);
            if (!chaos && quickplus && !Entities) return GetSaveKeyFromEnum(TypesOfSaves.QuickPlus);
            if (chaos && !quickplus && Entities) return GetSaveKeyFromEnum(TypesOfSaves.Entities | TypesOfSaves.Chaos);
            if (chaos && quickplus && Entities) return GetSaveKeyFromEnum(TypesOfSaves.Entities | TypesOfSaves.Chaos |TypesOfSaves.QuickPlus );
            if (chaos && !quickplus && !Entities) return GetSaveKeyFromEnum(TypesOfSaves.Chaos);
            if (chaos && quickplus && !Entities) return GetSaveKeyFromEnum(TypesOfSaves.Chaos | TypesOfSaves.QuickPlus);

            return "MaxLevelReachedWithCheating";
        }
    }
}
