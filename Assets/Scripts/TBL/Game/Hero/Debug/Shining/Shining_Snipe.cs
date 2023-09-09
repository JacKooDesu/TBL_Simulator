using System.Linq;
using TBL.Game.Sys;

namespace TBL.Game.Hero
{
    using Property = CardEnum.Property;
    public class Shining_Snipe : HeroSkill
    {
        public override int Id => Hero_Shining.SNIPE_SKILL_ID;
        public override void Bind(Manager manager, Player player, int index)
        {
            manager.PhaseManager.AfterEnter.AddListener(
                _ => player.SkillStatus.Update(index, UsageCheck()));
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override bool UsageCheck()
        {
            return Manager.Instance
                   .Players
                   .List
                   .Any(
                    x => x.CardStatus.Table.Any(
                        c => ((Property)c).Contains(Property.Black)));
        }
    }
}