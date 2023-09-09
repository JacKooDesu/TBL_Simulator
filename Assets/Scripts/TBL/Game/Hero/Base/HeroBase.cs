using System;
using System.Collections.Generic;
using System.Linq;
using TBL.Game.Sys;

namespace TBL.Game.Hero
{
    public abstract class HeroBase
    {
        public abstract HeroId Id { get; }
        public abstract Gender Gender { get; protected set; }
        public abstract SpecialPassive SpecialPassive { get; protected set; }

        public readonly Dictionary<int, HeroSkill> skillDict = new();
        public int[] GetAllSkillId() => skillDict.Keys.ToArray();
        public HeroSkill GetHeroSkill(int id) => skillDict[id];

        public void SetupForPlayer(Player p, Manager manager)
        {
            int iter = 0;
            foreach (var (id, skill) in skillDict)
            {
                var index = iter;
                skill.Bind(manager, p, index);
                iter++;
            }
            p.SkillStatus.Update(new(new bool[iter]));
            p.HeroStatus.Update(new((int)Id));
        }
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
        None = 0,
        Faker = 1 << 0,
    }
}
