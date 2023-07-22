using System;
using System.Collections.Generic;
using System.Linq;

namespace TBL.Game.Hero
{
    public abstract class HeroBase
    {
        public abstract string Name { get; }
        public abstract Gender Gender { get; protected set; }
        public abstract SpecialPassive SpecialPassive { get; protected set; }

        public readonly Dictionary<int, HeroSkill> skillDict;
        public int[] GetAllSkillId() => skillDict.Keys.ToArray();
        public HeroSkill GetHeroSkill(int id) => skillDict[id];
    }

    [Flags]
    public enum Gender
    {
        None = 0,
        Male = 1 << 0,
        Female = 1 << 1
    }

    [Flags]
    public enum SpecialPassive
    {
        Faker
    }
}
