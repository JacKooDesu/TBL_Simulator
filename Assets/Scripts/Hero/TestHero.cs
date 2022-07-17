using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace TBL.Hero
{
    using Util;
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

                    var sa = new SkillAction
                    {
                        skill = 0,
                        user = playerStatus.playerIndex
                    };
                    var menu = netCanvas.InitMenu(-1, new Option { str = "次選單", onSelect = () => sa.suffix = 1 });
                    await TaskExtend.WaitUntil(
                        () => sa.suffix == 1,
                        () => cancel.IsCancellationRequested);

                    if (cancel.IsCancellationRequested) return default;

                    return sa;
                },
                action = async (_) =>
                {
                    print("Test Hero Skill");
                    playerStatus.isWaitingData = true;
                    playerStatus.TargetReturnDataMenu("0", "1", "2", "3");
                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData,
                        () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting);

                    if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                    {
                        playerStatus.isWaitingData = false;
                        return;
                    }
                    print(playerStatus.tempData);
                    playerStatus.DrawCard(playerStatus.tempData);
                    playerStatus.CmdClearTempData();
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

