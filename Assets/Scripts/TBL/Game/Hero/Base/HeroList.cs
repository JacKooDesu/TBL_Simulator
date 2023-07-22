using System.Collections.Generic;

namespace TBL.Game.Hero
{
    public static class HeroList
    {
        static readonly Dictionary<HeroId, HeroBase> Dict = new(){
            {HeroId.Shining,new Hero_Shining()}
        };

        public static HeroBase GetHero(int id) => GetHero((HeroId)id);
        public static HeroBase GetHero(HeroId id) => Dict[id];
    }

    public enum HeroId : int
    {
        Debugger = -1,
        Shining = 0,


        MAX,
    }
}