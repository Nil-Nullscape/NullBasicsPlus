using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.Structures
{
    public class AltarPurgatory : BaseAltar
    {
        public override int MinFloor => 8;
        public override void Animate()
        {
            if (Singleton<BaseGameManager>.Instance.FoundNotebooks > Singleton<BaseGameManager>.Instance.NotebookTotal / 2) {
                base.OnActivate();
            }
            base.Animate();
        }
        public override void OnActivate()
        {
            NullscapeInBB.Instance.YtpMultipliers.Add(2.5f);
            NullscapeInBB.ProgressionEventNotifier(Singleton<LocalizationManager>.Instance.GetLocalizedText("Purgatory_Spawned"));
            NullscapeInBB.SetCooldown<AltarPurgatory>(2);
            var ec = Singleton<BaseGameManager>.Instance.Ec;
                var numberOfNpcs = 8 + Mathf.FloorToInt(NullscapeInBB.Instance.FloorCounter / 6f);
                for (int i = 0; i < numberOfNpcs; i++)
                {
                    var randomCell = ec.RandomCell(false, false, true);
                    var distance = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - randomCell.CenterWorldPosition).magnitude;
                    while (distance < 120)
                    {
                        randomCell = ec.RandomCell(false, false, true);
                        distance = (Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position - randomCell.CenterWorldPosition).magnitude;
                    }
                    var allNpcsL = NullscapeInBB.Instance.AllEnemies.Values.ToList();
                    allNpcsL.RemoveAll(x => x.Enemy == EnumExtensions.GetFromExtendedName<Character>("???"));
                    var allNpcs = allNpcsL.ToArray();
                    var randomNPC = allNpcs[UnityEngine.Random.Range(0, allNpcs.Length)].NPCToUse;
                    ec.SpawnNPC(randomNPC, randomCell.position);
                }
            itemRenderer.enabled = false;

            base.OnActivate();
        }
    }
}
