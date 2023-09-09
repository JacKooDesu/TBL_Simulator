using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.Sys
{
    using Random = UnityEngine.Random;
    using Game.Hero;
    using TBL.Utils;

    [Serializable]
    public class HeroManager
    {
        int DEBUG_ITER = 0;
        [SerializeField] HeroId[] DEBUG_POOL;
        Queue<int> Pool { get; set; } = new();

        // TODO Debug階段不洗牌部生成Pool
        public void Init()
        {
            List<int> list = new();
            for (int i = 0; i < ((int)HeroId.MAX); ++i)
                list.Add(i);
            Pool = new(list.Shuffle());
        }

        public int Draw() =>
            Random.Range(0, (int)HeroId.MAX);
        // Pool.Dequeue();

        public void SetupForPlayer(Player p, Manager manager)
        {
            var heroId = -1;
            if (DEBUG_POOL.Length is not 0)
            {
                heroId = ((int)DEBUG_POOL[DEBUG_ITER % DEBUG_POOL.Length]);
                DEBUG_ITER++;
            }
            else
            {
                heroId = Draw();
            }

            HeroList.GetHero(heroId).SetupForPlayer(p, manager);
        }
    }
}