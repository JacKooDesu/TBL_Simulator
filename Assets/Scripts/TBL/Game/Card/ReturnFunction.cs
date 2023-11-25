using TBL.Game.Sys;

namespace TBL.Game
{
    internal class ReturnFunction : ICardFunction
    {
        public void ExecuteAction(Player user, Manager manager, int id)
        {
            var phase = manager.PhaseManager.Current();
            if (phase.Type() is not PhaseType.Passing)
                return;

            manager.AddResolve(
                Phase_Resolving.ResolveDetail.Card(
                    _ => (phase as Phase_Passing)?.Return(),
                    CardEnum.Function.Return,
                    user));
        }

        public bool ClientCheck() =>
            !IPlayerStandalone.Me
                             .player
                             .ReceiverStatus
                             .Current()
                             .HasFlag(ReceiveEnum.Locked);

        public bool ServerCheck()
        {
            var phase = Manager.Instance.PhaseManager.Current();
            if (phase.Type() is not PhaseType.Passing)
                return false;

            var target = (phase as Phase_Passing).Target;
            if (target == null)
                return false;
            if (target!.ReceiverStatus.Current().HasFlag(ReceiveEnum.Locked))
                return false;

            return true;
        }
    }
}