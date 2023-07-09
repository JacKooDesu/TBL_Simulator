using System.Linq;
using Cysharp.Threading.Tasks;

namespace TBL.Game
{
    using Sys;
    using Property = CardEnum.Property;
    using TBL.Game.Networking;
    using TBL.Utils;
    using UnityEngine;
    using System;

    public class BurnFunction : ICardFunction
    {
        // public ICardFunction.ExecuteAction Execute => ExecuteAction;
        public void ExecuteAction(Player user, Manager manager) =>
            SelectPlayer(user, manager);


        void SelectPlayer(Player user, Manager manager)
        {
            var action = SelectPlayerAction(user, manager);
            action.CompleteCallback.AutoRemoveListener(() => SelectCard(user, manager, action.Result));
            manager.AddGameAction(action);
        }
        GameAction_SelectPlayer SelectPlayerAction(Player user, Manager manager) =>
            new(user,
                manager
                    .Players
                    .Players
                    .Where(
                        p => p.CardStatus
                            .Table
                            .Any(c => ((Property)c)
                            .Contains(Property.Black)))
                .Select(x => x.ProfileStatus.Id)
                .ToArray());

        void SelectCard(Player user, Manager manager, int targetId)
        {
            var target = manager.Players.QueryById(targetId);
            var cards = target.CardStatus
                              .Table
                              .Select(x => (Property)x)
                              .Where(x => x.Contains(Property.Black))
                              .ToArray();
            var action = SelectCardAction(user, cards);
            Func<GameAction_SelectCard, int[]> GetResultIds = action =>
                action.Result.Select(x => ((int)x)).ToArray();

            action.CompleteCallback.AutoRemoveListener(
                () => manager.DiscardTable(target, GetResultIds(action)));

            manager.AddGameAction(action);
        }

        GameAction_SelectCard SelectCardAction(Player user, Property[] cards) =>
            new(user, new(cards));

        public static bool AdditionCheck(Manager manager)
        {
            return manager
                    .Players
                    .Players
                    .Any(x => x.CardStatus
                                .Table
                                .Any(c => ((Property)c).Contains(Property.Black)));
        }
    }
}
