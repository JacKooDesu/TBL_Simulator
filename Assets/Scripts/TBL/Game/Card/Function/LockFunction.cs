using System;
using System.Linq;
using UnityEngine;

namespace TBL.Game
{
    using Sys;
    using Property = CardEnum.Property;
    using static CardFunctionUtil;
    public class LockFunction : ICardFunction
    {
        public bool ClientCheck() =>
            IPlayerStandalone.Standalones
                             .Select(x => x.player.ReceiverStatus.Current())
                             .Any(x => !x.HasFlag(ReceiveEnum.Locked));

        public bool ServerCheck() =>
            Manager.Instance
                   .Players.List
                   .Select(x => x.ReceiverStatus.Current())
                   .Any(x => !x.HasFlag(ReceiveEnum.Locked));

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
                        () => manager.AddReciverStatus(
                            manager.Players.QueryById(id), ReceiveEnum.Skipped)))
                .AddToFlow();
        }
    }
}