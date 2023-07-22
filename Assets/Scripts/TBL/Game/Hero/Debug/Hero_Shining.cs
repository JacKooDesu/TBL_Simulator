using System;

namespace TBL.Game.Hero
{
    public class Hero_Shining : HeroBase
    {
        public override string Name => "閃靈";
        public override Gender Gender { get; protected set; } = Gender.Female;
        public override SpecialPassive SpecialPassive { get; protected set; }
    }
}
