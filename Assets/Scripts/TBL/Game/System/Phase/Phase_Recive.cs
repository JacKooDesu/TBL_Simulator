namespace TBL.Game.Sys
{
    public class Phase_Recive : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Receive;
        protected override float time => 5;
        public record ReciveData(Player target, CardEnum.Property card);
        ReciveData data;

        public override void Enter(Manager manager, object parameter = null)
        {
            data = parameter as ReciveData;
            base.Enter(manager);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() => true;

        public override void Exit()
        {
            manager.AddTable(data.target, (int)data.card);
            manager.PhaseManager.Insert(PhaseType.End);
        }

    }
}