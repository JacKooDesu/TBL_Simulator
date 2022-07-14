using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TBL.Hero
{
    using Card;
    using GameAction;

    public class TestHero : HeroBase
    {
        protected override void BindSkill()
        {
            var skill0 = new HeroSkill
            {
                name = "",
                description = "",
                autoActivate = false,
                localAction = async () =>
                {
                    print("Test Hero Skill");
                    return new SkillAction();
                },
                action = (_) =>
                {
                    playerStatus.CmdDrawCard(3);
                },
                checker = () =>
                {
                    return true;
                }
            };

            skills = new HeroSkill[] { skill0 };
        }

        protected override void BindSpecialMission()
        {

        }
    }
}

