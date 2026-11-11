using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTM101BaldAPI.AssetTools;
using PineDebug;
using static PineDebug.PineDebugManager;
namespace NullscapeinBB.PineDebugShit
{
    internal static class PinedebugSupport
    {
        //public static List<PineButtonList>
        public static void addsupport() {
            var MainList = new PineButtonList("NullbasicsStuff");

            var Curses = new PineButtonList("Curses");
            var GreaterCurses = new PineButtonList("Greater Curse");
            var Upgrades = new PineButtonList("Upgrades");
            var Delusions = new PineButtonList("Delusions");

            foreach (var item in NullscapeInBB.Instance.AllCurses)
            {
                Curses.Add(PineDebugManager.CreateButton(item.Key.ToString(), () => {
                    NullscapeInBB.Instance.CursesPicked.Add(item.Value);
                    item.Value.CursesAction?.Invoke();

                }, NullscapeInBB.Instance.AllCursesItems[item.Key].itemSpriteLarge.texture));
            }

            foreach (var item in NullscapeInBB.Instance.AllGreaterCurses)
            {
                GreaterCurses.Add(PineDebugManager.CreateButton(item.Key.ToString(), () => {
                    NullscapeInBB.Instance.GreaterCursesPicked.Add(item.Value);
                    item.Value.CursesAction?.Invoke();

                }, NullscapeInBB.Instance.AllGreaterCursesItems[item.Key].itemSpriteLarge.texture));
            }
            foreach (var item in NullscapeInBB.Instance.AllUpgrades)
            {
                Upgrades.Add(PineDebugManager.CreateButton(item.Key.ToString(), () => {
                    NullscapeInBB.Instance.UpgradesBought.Add(item.Value);
                    item.Value.OnUpgradeBought?.Invoke();

                }, NullscapeInBB.Instance.AllUpgradesItems[item.Key].itemSpriteLarge.texture));
            }
            foreach (var item in NullscapeInBB.Instance.AllDelusions)
            {
                Delusions.Add(PineDebugManager.CreateButton(item.Key.ToString(), () => {
                    NullscapeInBB.Instance.DelusionsPicked.Add(item.Value);
                    item.Value.CursesAction?.Invoke();

                }, NullscapeInBB.Instance.AllDelusionsItems[item.Key].itemSpriteLarge.texture));
            }

            MainList.Add(CreateButton("Curses", () =>
            {
                PineDebugManager.Instance.ChangePage(MainList, Curses);
            }, AssetLoader.TextureFromMod(NullscapeInBB.Instance, "items", "spr_AngrierBaldi.png")));
            MainList.Add(CreateButton("Greater curses", () =>
            {
                PineDebugManager.Instance.ChangePage(MainList, GreaterCurses);
            }, AssetLoader.TextureFromMod(NullscapeInBB.Instance, "items", "spr_TimedActivities.png")));
            MainList.Add(CreateButton("Upgrades", () =>
            {
                PineDebugManager.Instance.ChangePage(MainList, Upgrades);
            }, AssetLoader.TextureFromMod(NullscapeInBB.Instance, "items", "spr_MoreYtps.png")));
            MainList.Add(CreateButton("Delusions", () =>
            {
                PineDebugManager.Instance.ChangePage(MainList, Delusions);
            }, AssetLoader.TextureFromMod(NullscapeInBB.Instance, "items", "placeholder.png")));

            PineDebugManager.Instance.buttonLists[0].Add(CreateButton("Nullbasics", () =>
            {
                PineDebugManager.Instance.ChangePage(PineDebugManager.Instance.buttonLists[0], MainList);
            },AssetLoader.TextureFromMod(NullscapeInBB.Instance,"items","placeholder.png")));


        }
    }
}
