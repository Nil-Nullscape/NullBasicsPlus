using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NullscapeinBB
{
    public static partial class StuffCreation
    {
        public static float GetInflationPercentage() { return 1 + ((NullscapeInBB.Instance.FloorCounter +2) / 10) * 0.2f; }
        public static void AddUpgrades() {
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.MoreYtps, new()
            {
                Cost = () => {
                    return Mathf.RoundToInt(750 * (NullscapeInBB.UpgradesCount(Upgrades.MoreYtps) + 1) * GetInflationPercentage());
                },
                curse = Upgrades.MoreYtps,
                MaxUpgrades = 10,
                MinFloor = 0


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.MoreSpeed, new()
            {
                Cost = () => {
                    return Mathf.RoundToInt(250 * (NullscapeInBB.UpgradesCount(Upgrades.MoreSpeed) + 1) * GetInflationPercentage());
                },
                curse = Upgrades.MoreSpeed,
                MaxUpgrades = 6,
                MinFloor = 0,
                Requirements = () => {
                    return NullscapeInBB.Instance.SelectedClass != Classes.Prisoner;
                }


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.BaldiPraise, new()
            {
                Cost = () => {
                    return Mathf.RoundToInt(500 * (NullscapeInBB.UpgradesCount(Upgrades.BaldiPraise) + 1) * GetInflationPercentage());
                },
                curse = Upgrades.BaldiPraise,
                MaxUpgrades = 8,
                MinFloor = 0


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.PlusOneInventorySlot, new()
            {
                Cost = () => {
                    return Mathf.CeilToInt(850 * (NullscapeInBB.UpgradesCount(Upgrades.PlusOneInventorySlot) * 1.5f + 1) * GetInflationPercentage()) ;
                },
                curse = Upgrades.PlusOneInventorySlot,
                MaxUpgrades = 4,
                MinFloor = 0,
                OnUpgradeBought = () => {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.UpdateTargetInventorySize();
                },
                Requirements = () => {
                    return NullscapeInBB.Instance.SelectedClass != Classes.Prisoner;
                }


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.TempShield, new()
            {
                Cost = () => {
                    return Mathf.CeilToInt(15000 * (NullscapeInBB.UpgradesCount(Upgrades.TempShield) + 1) * GetInflationPercentage()) ;
                },
                curse = Upgrades.TempShield,
                MaxUpgrades = 2,
                MinFloor = 9,
                OnUpgradeBought = () => {
                },
                Requirements = () => {
                    return NullscapeInBB.Instance.SelectedClass != Classes.Prisoner;
                }


            });
            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.LongerReach, new()
            {
                Cost = () => {
                    return Mathf.CeilToInt(1250 * (NullscapeInBB.UpgradesCount(Upgrades.LongerReach) + 1) * (NullscapeInBB.UpgradesCount(Upgrades.LongerReach) / 10f + 1) * GetInflationPercentage());
                },
                curse = Upgrades.LongerReach,
                MaxUpgrades = 10,
                MinFloor = 9,
                OnUpgradeBought = () => {
                },
                Requirements = () => {
                    return NullscapeInBB.Instance.SelectedClass != Classes.Prisoner;
                }


            });


            NullscapeInBB.Instance.AllUpgrades.Add(Upgrades.TeleportRandomInCompletedActivityRoom, new()
            {
                Cost = () => {
                    return Mathf.CeilToInt(5000 * (NullscapeInBB.UpgradesCount(Upgrades.TeleportRandomInCompletedActivityRoom) + 1) * GetInflationPercentage());
                },
                curse = Upgrades.TeleportRandomInCompletedActivityRoom,
                MaxUpgrades = 4,
                MinFloor = 14,
                OnUpgradeBought = () => {
                }


            });

        }
    }
}
