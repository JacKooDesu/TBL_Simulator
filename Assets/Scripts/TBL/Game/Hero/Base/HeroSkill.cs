using System;
using TBL.Game.Sys;
using UnityEngine.Events;

namespace TBL.Game.Hero
{
    public abstract class HeroSkill
    {
        public const int OFFSET = 1000;
        public int Id => _Id + (int)Hero * OFFSET;
        protected abstract int _Id { get; }
        protected abstract HeroId Hero { get; }
        public UnityEvent<bool> ActiveEvent { get; }
        public abstract void Bind(Manager manager, Player player, int index);
        public abstract bool UsageCheck(Manager manager);

        public abstract void Execute(Manager manager, Player player);
    }
}