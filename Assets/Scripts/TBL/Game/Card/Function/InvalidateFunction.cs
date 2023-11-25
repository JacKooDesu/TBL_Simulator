using System;
using UnityEngine;

namespace TBL.Game
{
    using Sys;
    using Property = CardEnum.Property;
    using static CardFunctionUtil;
    public class InvalidateFunction : ICardFunction
    {
        public bool ClientCheck() => true;

        public bool ServerCheck() => true;

        public void ExecuteAction(Player user, Manager manager, int id) =>
            manager.AddResolve(Resolve(user));

        Phase_Resolving.ResolveDetail Resolve(Player user) =>
            Phase_Resolving.ResolveDetail.Just(
                _ => (Manager.Instance.PhaseManager.Current() as Phase_Resolving).Pop(),
                user);
    }
}
