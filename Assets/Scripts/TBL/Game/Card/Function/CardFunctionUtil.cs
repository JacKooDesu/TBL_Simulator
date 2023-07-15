using UnityEngine;
using System;
using System.Linq;

namespace TBL.Game
{
    using Sys;
    using Property = CardEnum.Property;

    public static class CardFunctionUtil
    {
        public static GameAction_SelectPlayer SelectPlayerAction(
            Player user, Manager manager, Func<Player, bool> fileter) =>
            new(
                user,
                manager.Players
                    .Players
                    .Where(fileter)
                    .Select(x => x.ProfileStatus.Id)
                    .ToArray()
            );

        public static GameAction_SelectCard SelectCardAction(
            Player user, Manager manager, Property[] cards) =>
            new(user, new(cards));

        public static GameAction_SelectCard SelectTableCardAction(
            Player user, Manager manager, Player target, Func<Property, bool> fileter) =>
            SelectCardAction(
                user,
                manager,
                target.CardStatus
                      .Table
                      .Select(x => (Property)x)
                      .Where(fileter)
                      .ToArray());
    }
}
