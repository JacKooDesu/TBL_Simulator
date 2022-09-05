using System.Threading.Tasks;

namespace TBL.Hero
{
    using Judgement;
    using GameActionData;
    using Util;
    public class TheBeauty : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill
            {
                name = "name",
                description = "description",
                autoActivate = true,
                checker = () =>
                {
                    if (judgement.currentPhase == NetworkJudgement.Phase.ChooseToSend &&
                        judgement.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return true;

                    return false;
                }
            };
            skill1.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 多抽1
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        playerStatus.DrawCard(1);
                        return Task.CompletedTask;
                    }
                }
            };

            skills = new HeroSkill[]{
                skill1
            };
        }

        protected override void BindSpecialMission()
        {

        }
    }
}

