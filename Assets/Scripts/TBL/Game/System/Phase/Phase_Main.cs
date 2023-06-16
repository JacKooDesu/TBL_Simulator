using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
namespace TBL.Game.Sys
{
    public class Phase_Main : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Main;
        protected override float time => 20f;
        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.PassCard;
        public UnityEvent FinishEvent { get; } = new();
        Player passTarget = null;
        CardEnum.Property card = 0;

        public override void Enter(Manager manager, object parameter = null)
        {
            base.Enter(manager);

            passTarget = null;
            card = 0;

            var p = manager.CurrentPlayer;
            p.PlayerStandalone.PacketHandler.PassCardPacketEvent += OnFinish;
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
            manager.CurrentPlayer
                   .PlayerStandalone
                   .PacketHandler
                   .PassCardPacketEvent -= OnFinish;

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
                manager.PhaseManager.InsertRange(
                    InitQueue(passTarget, card)
                        .Select(p => new PhaseManager.PhaseData(PhaseType.Passing, p))
                        .ToArray());
            }
        }

        void OnFinish(Networking.PassCardPacket packet)
        {
            FinishEvent.Invoke();
            passTarget = manager.Players.QueryById(packet.target);
            card = packet.card;
        }

        List<Player> InitQueue(Player target, CardEnum.Property card)
        {
            List<Player> queue = new();
            if (card.HasFlag(CardEnum.Property.Direct))
            {
                queue.Add(target);
                queue.Add(manager.CurrentPlayer);
            }
            else
            {
                var players = manager.Players.Players;
                var beginId = target.ProfileStatus.Id;
                var ownerId = manager.CurrentPlayer.ProfileStatus.Id;
                int iter = beginId - ownerId;
                if (Mathf.Abs(iter) != 1)
                    iter = iter > 0 ? -1 : 1;

                int current = beginId;
                while (queue.Count != players.Count)
                {
                    if (current > players.Count - 1)
                        current = 0;
                    else if (current < 0)
                        current = players.Count - 1;

                    if (!players[current].ReceiverStatus.Current().HasFlag(ReceiveEnum.Skipped))
                        queue.Add(players[current]);

                    current += iter;
                }
            }
            return queue;
        }
    }
}
