using System;

namespace NullscapeinBB
{
    class EnemItem : Item
    {
        public Character Enemy;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }

        
    }
    class CurseItem : Item
    {
        public Curses curse;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }

        
    }
    class GreaterCurseItem : Item
    {
        public GreaterCurses curse;
        public override bool Use(PlayerManager pm)
        {
            return base.Use(pm);
        }

        
    }
}