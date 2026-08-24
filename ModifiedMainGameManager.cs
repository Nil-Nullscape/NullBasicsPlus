namespace NullscapeinBB
{
    class MainInfiniteGameManager : MainGameManager
    {
        
        public override void AngerBaldi(float val)
        {

            var baseAngerValue = 6.5f / ec.notebookTotal;
            base.AngerBaldi(val * baseAngerValue * (1 + NullscapeInBB.CurseCount(Curses.AngrierBaldi) * 1.5f));
        }

        protected override void AwakeFunction()
        {
            defaultLives = 0;
            base.AwakeFunction();
        }

        protected override void VirtualUpdate()
        {
            
            var plr = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            if (NullscapeInBB.GreaterCurseCount(GreaterCurses.OmniscientBaldi) > 0)
            {

                foreach (NPC npc in ec.Npcs)
                {
                    if (npc.Character == Character.Baldi)
                    {
                        npc.GetComponent<Baldi>().Hear(plr.gameObject, plr.transform.position, 127, false);
                    }
                }
            }
            base.VirtualUpdate();
        }

    }
}