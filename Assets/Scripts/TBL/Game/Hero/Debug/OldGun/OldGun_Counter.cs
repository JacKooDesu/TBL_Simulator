using TBL.Game.Sys;

namespace TBL.Game.Hero
{
    public class OldGun_Counter : HeroSkill
    {
        protected override int _Id => Hero_OldGun.COUNTER_SKILL_ID;
        protected override HeroId Hero => HeroId.OldGun;

        public override void Bind(Manager manager, Player player, int index)
        {
            manager.PhaseManager.AfterEnter.AddListener(Checker);
            void Checker(PhaseBase phase)
            {
                if (phase.Type() is not PhaseType.Resolving)
                    return;

                var current = (phase as Phase_Resolving).GetCurrent();

                if (UsageCheck(manager))
                    Execute(manager, player);
            }
        }

        public override void Execute(Manager manager, Player player)
        {
            
        }

        public override bool UsageCheck(Manager manager)
        {
            return true;
        }
    }
}