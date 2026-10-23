using System;

namespace NullscapeinBB
{
    public class EnemItem : Item
    {
        public Character Enemy;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }

        
    }
    public class CurseItem : Item
    {
        public Curses curse;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }

        
    }
    public class GreaterCurseItem : Item
    {
        public GreaterCurses curse;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }


    }
    public class ChaosItem : Item
    {
        public enum ChaosType
        {
            Curse,
            GreaterCurse,
            Enemy
        }
        public ChaosCurseData data;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }


    }
}