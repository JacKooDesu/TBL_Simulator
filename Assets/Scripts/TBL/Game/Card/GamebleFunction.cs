using TBL.Game.Sys;

namespace TBL.Game
{
    internal class GamebleFunction : ICardFunction
    {
        public void ExecuteAction(Player user, Manager manager, int id) =>
            manager.AddResolve(Phase_Resolving.ResolveDetail.Card(
                _ => FlipCards(manager),
                CardEnum.Function.Gameble,
                user,
                null));

        void FlipCards(Manager manager)
        {
            var sleeping = manager.Deck.sleeping;
            var table = manager.Deck.table;
            foreach (var p in manager.Players.List)
                manager.AddTable(p, sleeping.MoveTo(0, table).Id);
        }

        public bool ClientCheck() => true;

        public bool ServerCheck() => true;
    }
}