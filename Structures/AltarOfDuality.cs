using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullscapeinBB.Structures
{
    public class AltarOfDuality : BaseAltar
    {
        public override int MinFloor => 15;
        public override AltarTypes Type => AltarTypes.Duality;
        public override void OnActivate()
        {
            NullscapeInBB.Instance.QueuedIntermission.Add(IntermissionType.Delusions);
            NullscapeInBB.AltarEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("Delusion_Activated"));
            NullscapeInBB.SetCooldown(AltarTypes.Duality,5);
            base.OnActivate();
        }
    }
}
