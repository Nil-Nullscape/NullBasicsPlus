using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    public static partial class StuffCreation
    {
        public static void AddUpgrades() {
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.MoreYtps, new()
            {
                Cost = () => {
                    return 350 * (NullscapeInBB.UpgradesCount(Upgrades.MoreYtps) + 1);
                },
                curse = Upgrades.MoreYtps,
                MaxUpgrades = 4,
                MinFloor = 0


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.MoreSpeed, new()
            {
                Cost = () => {
                    return 300 * (NullscapeInBB.UpgradesCount(Upgrades.MoreSpeed) + 1);
                },
                curse = Upgrades.MoreSpeed,
                MaxUpgrades = 4,
                MinFloor = 0


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.BaldiPraise, new()
            {
                Cost = () => {
                    return 500 * (NullscapeInBB.UpgradesCount(Upgrades.BaldiPraise) + 1);
                },
                curse = Upgrades.BaldiPraise,
                MaxUpgrades = 4,
                MinFloor = 0


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.PlusOneInventorySlot, new()
            {
                Cost = () => {
                    return 850 * (NullscapeInBB.UpgradesCount(Upgrades.PlusOneInventorySlot) + 1);
                },
                curse = Upgrades.PlusOneInventorySlot,
                MaxUpgrades = 4,
                MinFloor = 0,
                OnUpgradeBought = () => {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.UpdateTargetInventorySize();
                }


            });

        }
    }
}
