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
    using static CardFunctionUtil;
    using UnityEngine.Events;

    public class BurnFunction : ICardFunction
    {
        public bool ClientCheck() =>
            IPlayerStandalone.Standalones
                             .Select(x => x.player.CardStatus.Table)
                             .Any(x =>
                                x.Any(c => ((Property)c).Contains(Property.Black)));

        public bool ServerCheck() =>
            Manager.Instance
                   .Players.Players
                   .Select(x => x.CardStatus.Table)
                   .Any(x =>
                        x.Any(c => ((Property)c).Contains(Property.Black)));

        public void ExecuteAction(Player user, Manager manager, int id) =>
            SelectPlayer(user, manager);


        void SelectPlayer(Player user, Manager manager)
        {
            Func<Player, bool> fileter =
                p => p.CardStatus
                      .Table
                      .Any(c => ((Property)c).Contains(Property.Black));
            SelectPlayerAction(user, manager, fileter)
                .AndThen<int>(id => SelectCard(user, manager, id))
                .AddToFlow();
        }

        void SelectCard(Player user, Manager manager, int targetId)
        {
            var target = manager.Players.QueryById(targetId);
            Func<Property[], int[]> GetResultIds = cards =>
                cards.Select(x => ((int)x)).ToArray();

            SelectTableCardAction(
                user,
                manager,
                target,
                p => p.Contains(Property.Black))
                .AndThen<Property[]>(
                    cards => manager.AddResolve(
                        () => manager.DiscardTable(target, GetResultIds(cards))))
                .AddToFlow();
        }

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
