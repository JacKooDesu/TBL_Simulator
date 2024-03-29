using TBL.Game.Sys;

namespace TBL.Game
{
    internal class InterceptFunction : ICardFunction
    {
        public void ExecuteAction(Player user, Manager manager, int id)
        {
            var phase = manager.PhaseManager.Current();
            if (phase.Type() is not PhaseType.Passing)
                return;

            manager.AddResolve(
                Phase_Resolving.ResolveDetail.Card(
                    detail => (phase as Phase_Passing)?.Intercept(detail.user),
                    CardEnum.Function.Intercept,
                    user,
                    null));
        }

        public bool ClientCheck() => true;

        public bool ServerCheck()
        {
            var phase = Manager.Instance.PhaseManager.Current();
            if (phase.Type() is not PhaseType.Passing)
                return false;

            var target = (phase as Phase_Passing).Target;
            if (target == null)
                return false;

            return true;
        }
    }
}