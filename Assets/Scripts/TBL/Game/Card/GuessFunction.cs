using System;
using TBL.Game.Sys;
using UnityEngine;

namespace TBL.Game
{
    using ActionOption = GameAction_SelectAction.Option;
    using Property = CardEnum.Property;

    internal class GuessFunction : ICardFunction
    {
        public void ExecuteAction(Player user, Manager manager, int id)
        {
            var phase = manager.PhaseManager.Current();
            if (phase.Type() is not PhaseType.Passing)
                return;

            Property card = (Property)(phase as Phase_Passing).Data.cardId;

            manager.AddResolve(
                Phase_Resolving.ResolveDetail.Card(
                    detail => AskColor(detail.user, manager, card),
                    CardEnum.Function.Guess,
                    user));
        }

        void AskColor(Player user, Manager manager, Property card)
        {
            Action<CardEnum.Color> CheckAction =
                color =>
                {
                    if (card.Contains(color))
                        manager.Draw(user, 1);
                };

            ActionOption red = new("紅", () => CheckAction(CardEnum.Color.Red));
            ActionOption blue = new("藍", () => CheckAction(CardEnum.Color.Blue));
            ActionOption black = new("黑", () => CheckAction(CardEnum.Color.Black));

            GameAction_SelectAction result =
                new(user, new(new[] { red, blue, black }));

            result.AddToFlow();
        }

        // FIXME 確認是否為公開文本
        public bool ClientCheck() => true;

        public bool ServerCheck()
        {
            var phase = Manager.Instance.PhaseManager.Current();
            if (phase.Type() is not PhaseType.Passing)
                return false;

            var target = (phase as Phase_Passing).Target;
            if (target == null)
                return false;

            return true;
        }
    }
}