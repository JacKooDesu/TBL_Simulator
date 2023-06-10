using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
    public class Phase_Draw : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Draw;
        protected override float Time => 10f;
        const int DRAW_COUNT = 2;

        public override void Enter(Manager manager, object parameter = null)
        {
            base.Enter(manager);
        }

        public override bool Update(float dt)
        {
            return base.Update(dt);
        }

        void Check(){
            // manager.CurrentPlayer
        }

        public override void Exit()
        {
            manager.Draw(manager.CurrentPlayer, 2);
        }
    }
}
