using System;

namespace TBL.Game.Hero
{
    public class Hero_Shining : HeroBase
    {
        internal const int SNIPE_SKILL_ID = 0;

        public override HeroId Id => HeroId.Shining;
        public override Gender Gender { get; protected set; } = Gender.Female;
        public override SpecialPassive SpecialPassive { get; protected set; } = 0;

        internal Hero_Shining()
        {
            skillDict.Add(SNIPE_SKILL_ID, new Shining_Snipe());
        }
    }
}
