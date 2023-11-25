using System;
using System.Linq;

namespace TBL.Game
{
    using Sys;
    using static CardFunctionUtil;
    public class SkipFunction : ICardFunction
    {
        public bool ClientCheck() =>
            IPlayerStandalone.Standalones
                             .Select(x => x.player.ReceiverStatus.Current())
                             .Any(x => !x.HasFlag(ReceiveEnum.Skipped));

        public bool ServerCheck() =>
            Manager.Instance
                   .Players.List
                   .Select(x => x.ReceiverStatus.Current())
                   .Any(x => !x.HasFlag(ReceiveEnum.Skipped));

        public void ExecuteAction(Player user, Manager manager, int id)
        {
            SelectPlayer(user, manager);
        }

        void SelectPlayer(Player user, Manager manager)
        {
            SelectPlayerAction(
                user,
                manager,
                p => !p.Equals(user) && !p.ReceiverStatus.Current().HasFlag(ReceiveEnum.Skipped))
                .AndThen<int>(
                    id => manager.AddResolve(
                        Phase_Resolving.ResolveDetail.Card(
                            detail => manager.AddReciverStatus(
                                detail.target, ReceiveEnum.Skipped),
                            CardEnum.Function.Skip,
                            user,
                            manager.Players.QueryById(id))))
                .AddToFlow();
        }
    }
}
