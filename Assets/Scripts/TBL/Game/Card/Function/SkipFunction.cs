using System;
using UnityEngine;

namespace TBL.Game
{
    using Sys;
    using Property = CardEnum.Property;
    using static CardFunctionUtil;
    public class SkipFunction : ICardFunction
    {
        public void ExecuteAction(Player user, Manager manager)
        {
            SelectPlayer(user, manager);
        }

        void SelectPlayer(Player user, Manager manager)
        {
            SelectPlayerAction(
                user,
                manager,
                p => !p.ReceiverStatus.Current().HasFlag(ReceiveEnum.Skipped))
                .AndThen<int>(
                    id => manager.AddResolve(
                        () => manager.AddReciverStatus(
                            manager.Players.QueryById(id), ReceiveEnum.Skipped)))
                .AddToFlow();
        }
    }
}
