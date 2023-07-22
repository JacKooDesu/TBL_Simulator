using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.Sys
{
    using Random = UnityEngine.Random;
    using Game.Hero;
    using TBL.Utils;

    public class HeroManager
    {
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
    }
}