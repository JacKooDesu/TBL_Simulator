namespace TBL.Game.Sys
{
    public class Phase_End : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.End;
        protected override float time => 10;

        public override void Enter(Manager manager, object parameter = null)
        {
            base.Enter(manager);
        }

        public override bool Update(float dt) =>
        Check() & base.Update(dt);

        bool Check() => true;

        public override void Exit()
        {

        }
    }
}