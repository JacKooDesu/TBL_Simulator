using UnityEngine;
using TBL.Game.Sys;

namespace TBL.Game
{
    using Sys;
    using static CardFunctionUtil;
    internal sealed class TestFunction : ICardFunction
    {
        public bool ClientCheck() => true;

        public bool ServerCheck() => true;

        public void ExecuteAction(Player user, Manager manager, int id)
        {
            SelectPlayer(user, manager);
        }

        void SelectPlayer(Player user, Manager manager)
        {
            SelectPlayerAction(user, manager, p => !user.Equals(p))
                .AndThen<int>(id => manager.AddResolve(
                    // () => manager.AddGameAction()
                    () => Debug.Log("Test!")
                ));
        }
    }
}