using System.Collections.Generic;

namespace TBL.Game.Hero
{
    public static class HeroList
    {
        static readonly Dictionary<HeroId, HeroBase> Dict = new(){
            {HeroId.Shining,new Hero_Shining()}
        };

        public static HeroBase GetHero(int id) => GetHero((HeroId)id);
        public static HeroBase GetHero(HeroId id) =>
            Dict.TryGetValue(id, out var hero) ? hero : null;
        public static bool TryGetHero(int id, out HeroBase hero) =>
            TryGetHero((HeroId)id, out hero);
        public static bool TryGetHero(HeroId id, out HeroBase hero) =>
            Dict.TryGetValue(id, out hero);
    }

    public enum HeroId : int
    {
        _Offset = 10,

        Debugger = 0,
        Shining = 1,

        MAX,
    }
}