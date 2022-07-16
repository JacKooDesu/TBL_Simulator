using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

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
                localAction = async (cancel) =>
                {

                    var sa = new SkillAction();
                    var menu = netCanvas.InitMenu(-1, new Option { str = "次選單", onSelect = () => sa.suffix = 1 });
                    while (sa.suffix != 1)
                    {
                        print("waiting");
                        if (cancel.IsCancellationRequested)
                            return default;
                        await Task.Yield();
                    }

                    return new SkillAction();
                },
                action = async (_) =>
                {
                    print("Test Hero Skill");
                    judgement.ChangePhase(NetworkJudgement.Phase.HeroSkillReacting);
                    playerStatus.CmdSetWaitingData(true);
                    playerStatus.TargetReturnDataMenu("0", "1", "2", "3");
                    while (playerStatus.isWaitingData)
                    {
                        if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                            return;
                        await Task.Yield();
                    }
                    playerStatus.CmdDrawCard(playerStatus.tempData);
                    playerStatus.CmdClearTempData();
                    judgement.ChangePhase(judgement.lastPhase);
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

