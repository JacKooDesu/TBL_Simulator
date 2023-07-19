using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TBL.Utils;

namespace TBL.Game.Sys
{
    public class Phase_Main : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Main;
        public override string PhaseName => "主要階段";
        protected override float time => 20f;
        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.PassCard;
        public UnityEvent FinishEvent { get; } = new();
        Player passTarget = null;
        int cardId = 0;

        public override void Enter(Manager manager, object parameter = null)
        {
            base.Enter(manager);

            passTarget = null;
            cardId = 0;

            var p = manager.CurrentPlayer;
            p.PlayerStandalone.PacketHandler.PassCardPacketEvent.AutoRemoveListener(OnFinish);
            manager.AddQuest(p, QUEST, FinishEvent);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() =>
            manager.CurrentPlayer
                    .PhaseQuestStatus.Quest
                    .Contains(QUEST);

        public override void Exit()
        {
            var card = (CardEnum.Property)cardId;
            manager.CurrentPlayer
                   .PlayerStandalone
                   .PacketHandler
                   .PassCardPacketEvent
                   .RemoveAllListeners();

            if (Check() || passTarget == null)
            {
                manager.DiscardHandAll(manager.CurrentPlayer);
                manager.FinishQuest(manager.CurrentPlayer, QUEST);
                manager.PhaseManager.Insert(
                    PhaseType.End
                );
            }
            else
            {
                manager.PhaseManager.Insert(
                    new(PhaseType.Passing,
                        new Phase_Passing.PassingData(
                            InitQueue(passTarget, card),
                            cardId)));

                manager.DiscardHand(manager.CurrentPlayer, cardId);
                manager.Deck.hand.MoveTo(c => c.Id == cardId, manager.Deck.table);
            }
        }

        void OnFinish(Networking.PassCardPacket packet)
        {
            FinishEvent.Invoke();
            passTarget = manager.Players.QueryById(packet.target);
            cardId = packet.cardId;
        }

        Queue<Player> InitQueue(Player target, CardEnum.Property card)
        {
            List<Player> list = new();
            if (card.HasFlag(CardEnum.Property.Direct))
            {
                list.Add(target);
                list.Add(manager.CurrentPlayer);
            }
            else
            {
                var players = manager.Players.List;
                var beginId = target.ProfileStatus.Id;
                var ownerId = manager.CurrentPlayer.ProfileStatus.Id;
                int iter = beginId - ownerId;
                if (Mathf.Abs(iter) != 1)
                    iter = iter > 0 ? -1 : 1;

                int current = beginId;
                while (list.Count != players.Count)
                {
                    if (current > players.Count - 1)
                        current = 0;
                    else if (current < 0)
                        current = players.Count - 1;

                    if (!players[current].ReceiverStatus.Current().HasFlag(ReceiveEnum.Skipped))
                        list.Add(players[current]);

                    current += iter;
                }
            }
            return new(list);
        }
    }
}
