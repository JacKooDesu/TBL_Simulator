namespace TBL.Game.Sys
{
    public class Phase_Draw : PhaseBase
    {
        protected override float Time => 10f;
        const int DRAW_COUNT = 2;

        public override void Enter(object parameter = null)
        {
            base.Enter();
        }

        public override bool Update(float dt)
        {
            return base.Update(dt);
        }

        public override void Exit()
        {
        }
    }
}
