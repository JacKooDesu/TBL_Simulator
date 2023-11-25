using System;
using System.Linq;
using System.Collections.Generic;
namespace TBL.Game.Hero
{
    using Sys;
    using Sys.Helper;
    using Property = CardEnum.Property;
    public class Hero_AgentK : HeroBase
    {
        public override HeroId Id => HeroId.AgentK;
        public override Gender Gender { get; protected set; } = Gender.Male;
        public override SpecialPassive SpecialPassive { get; protected set; } = 0;
        public override IEnumerable<HeroSkill> Skills() => new HeroSkill[]{

        };
    }
}