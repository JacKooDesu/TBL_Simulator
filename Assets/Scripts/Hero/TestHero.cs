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
                name = "測試",
                description = "",
                autoActivate = false,
                localAction = async () =>
                {
                    var sa = new SkillAction();
                    var menu = netCanvas.InitMenu(-1, new Option { str = "次選單", onSelect = () => sa.suffix = 1 });
                    while (sa.suffix != 1)
                        await Task.Yield();

                    return new SkillAction();
                },
                action = (_) =>
                {
                    print("Test Hero Skill");
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

