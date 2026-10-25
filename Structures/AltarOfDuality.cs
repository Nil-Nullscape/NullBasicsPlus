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
        public override void OnActivate()
        {
            NullscapeInBB.Instance.QueuedIntermission.Add(IntermissionType.Delusions);
            NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("Delusion_Activated"));
            NullscapeInBB.SetCooldown<AltarOfDuality>(8);
            base.OnActivate();
        }
    }
}
