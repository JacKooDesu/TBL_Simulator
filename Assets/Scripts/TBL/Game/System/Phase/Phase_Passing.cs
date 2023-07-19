using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
    using Game.Networking;

    public class Phase_Passing : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Passing;
        public override string PhaseName => "卡片傳遞階段";
        protected override float time => 5f;

        public record PassingData(Queue<Player> queue, int cardId);
        PassingData data;
        public PassingData Data => data;

        Stack<Player> PassedPlayer { get; } = new();

        Player target;
        public Player Target => target;

        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.AskRecieve;
        public UnityEvent FinishEvent { get; } = new();

        public PhaseManager.PhaseData nextPhase = null;

        public override void Enter(Manager manager, object parameter)
        {
            base.Enter(manager);
            if (parameter != null)
            {
                data = parameter as PassingData;
                PassedPlayer.Clear();
            }

            target = data.queue.Dequeue();
            if (target == manager.CurrentPlayer || data.queue.Count == 0)
            {
                nextPhase = CreateRecivePhase();
                forceExit = true;
                return;
            }
            else
                forceExit = false;


            // FIXME: 應該合併卡片接收的封包?
            target.PlayerStandalone
                  .PacketHandler
                  .AcceptCardPacketEvent
                  .AddListener(OnFinish);
            target.PlayerStandalone
                  .PacketHandler
                  .RejectCardPacketEvent
                  .AddListener(OnFinish);

            manager.AddQuest(target, QUEST, FinishEvent);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() =>
            target.PhaseQuestStatus.Quest
                    .Contains(QUEST);

        public override void Exit()
        {
            PassedPlayer.Push(target);
            if (Check() && !forceExit)
            {
                manager.FinishQuest(target, QUEST);
                nextPhase = new(PhaseType.Passing);
            }

            manager.PhaseManager.Insert(nextPhase);

            target.PlayerStandalone
                  .PacketHandler
                  .AcceptCardPacketEvent
                  .RemoveAllListeners();
            target.PlayerStandalone
                  .PacketHandler
                  .RejectCardPacketEvent
                  .RemoveAllListeners();

            this.target = null;
            this.nextPhase = null;
        }

        void OnFinish(IPacket p)
        {
            if (p is AcceptCardPacket)
                nextPhase = CreateRecivePhase();
            else
                nextPhase = CreatePassingPhase();

            FinishEvent.Invoke();
        }

        PhaseManager.PhaseData CreateRecivePhase() =>
            new(PhaseType.Receive, new Phase_Recive.ReciveData(target, (CardEnum.Property)data.cardId));

        PhaseManager.PhaseData CreatePassingPhase() =>
            new(PhaseType.Passing);

        public void Return()
        {
            data.queue.Clear();
            while (PassedPlayer.TryPop(out var p))
                data.queue.Enqueue(p);

            forceExit = true;
        }

        public void Intercept(Player user)
        {
            data.queue.Clear();
            data.queue.Enqueue(user);

            forceExit = true;
        }
    }
}
