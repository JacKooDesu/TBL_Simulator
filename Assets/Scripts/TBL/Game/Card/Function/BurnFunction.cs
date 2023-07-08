using System.Linq;
using Cysharp.Threading.Tasks;

namespace TBL.Game
{
    using Sys;
    using Property = CardEnum.Property;
    using TBL.Game.Networking;
    using TBL.Utils;
    using UnityEngine;

    public class BurnFunction : ICardFunction
    {
        // public ICardFunction.ExecuteAction Execute => ExecuteAction;
        public void ExecuteAction(Player user, Manager manager)
        {
            GameAction_SelectPlayer selectPlayer = SelectPlayerAction(user, manager);
            selectPlayer.Callback.AutoRemoveListener(p => SelectCard(user, manager, p));
            manager.AddGameAction(selectPlayer);
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

        void SelectCard(Player user, Manager manager, ActionResponsePacket response)
        { Debug.Log($"Select Card {response.Data}"); }

        public void Step()
        {
            throw new System.NotImplementedException();
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
