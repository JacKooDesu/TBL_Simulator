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

                    var sa = new SkillAction();
                    var menu = netCanvas.InitMenu(-1, new Option { str = "次選單", onSelect = () => sa.suffix = 1 });
                    await TaskExtend.WaitUntil(
                        () => sa.suffix == 1, 
                        () => cancel.IsCancellationRequested);

                    if (cancel.IsCancellationRequested) return default;

                    return new SkillAction();
                },
                action = async (_) =>
                {
                    print("Test Hero Skill");
                    judgement.ChangePhase(NetworkJudgement.Phase.HeroSkillReacting);
                    playerStatus.CmdSetWaitingData(true);
                    playerStatus.TargetReturnDataMenu("0", "1", "2", "3");

                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData,
                        () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting);

                    if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting) return;

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

