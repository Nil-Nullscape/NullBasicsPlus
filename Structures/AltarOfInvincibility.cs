using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullscapeinBB.Structures
{
    public class AltarOfInvincibility : BaseAltar
    {
        public override int MinFloor => 15;
        public override AltarTypes Type => AltarTypes.Invincibility;
        readonly int cost = NullscapeInBB.Instance.FloorCounter * 75;
        public override void InitializeMisc()
        {
            
            keyDesc = $"{Singleton<LocalizationManager>.Instance.GetLocalizedText("altar_Invincibility")} {cost}";
        }
        public override void OnActivate()
        {
            if (Singleton<CoreGameManager>.Instance.GetPoints(0) < cost) return;
            
                Singleton<CoreGameManager>.Instance.AddPoints(-cost, 0, true, false, false);
            
            NullscapeInBB.AltarEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("Invincibility_Activated"));
            NullscapeInBB.SetCooldown(AltarTypes.Invincibility,15);
            if (Singleton<BaseGameManager>.Instance is MainInfiniteGameManager Igm) {
                Igm.shieldtimeleft = 60f;
                Igm.MaxTime = 60f;
            }
            base.OnActivate();
        }
    }
}
