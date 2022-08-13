namespace TBL.Hero
{
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
                localAction = async (cancel) =>
                {
                    var sa = new SkillActionData();
                    return new SkillActionData();
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

