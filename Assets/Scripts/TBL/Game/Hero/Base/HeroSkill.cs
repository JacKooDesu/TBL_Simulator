using System;
using TBL.Game.Sys;
using UnityEngine.Events;

namespace TBL.Game.Hero
{
    public abstract class HeroSkill
    {
        public abstract int Id { get; }
        public UnityEvent<bool> ActiveEvent { get; }
        public abstract void Bind(Manager manager, Player player, int index);
        public abstract bool UsageCheck();

        public abstract void Execute();
    }
}