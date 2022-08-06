namespace TBL.Hero
{
    using GameAction;
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
                localAction = async (cancel) =>
                {
                    var sa = new SkillAction();
                    return new SkillAction();
                },
                action = async (_) =>
                {
                    playerStatus.DrawCard(1);
                    return true;
                },
                checker = () =>
                {
                    return false;
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

