using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullscapeinBB.Save
{
    internal static class SaveHelper
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

       

        public static string GetSaveKeyFromEnum(TypesOfSaves enumThingie) {
            var chaos = enumThingie.HasFlag(TypesOfSaves.Chaos);
            var quick = NullscapeInBB.Instance.EnableQuickStart.Value;
            var quickplus = enumThingie.HasFlag(TypesOfSaves.QuickPlus);
            var Entities = enumThingie.HasFlag(TypesOfSaves.Entities);
            var Cheating = NullscapeInBB.GetCheatingMods();
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


       
        public static string GetSaveKeyCurrentMode() {
            var chaos = NullscapeInBB.Instance.EnableChaosMode.Value;
            var quick = NullscapeInBB.Instance.EnableQuickStart.Value;
            var quickplus = NullscapeInBB.Instance.EnableQuickStartPlus.Value;
            var Entities = NullscapeInBB.Instance.EnabledNullscapeEntities.Value;
            var Cheating = NullscapeInBB.GetCheatingMods();
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
