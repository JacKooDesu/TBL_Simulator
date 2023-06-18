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
        protected override float time => 5f;
        public record PassingData(Queue<Player> queue, CardEnum.Property card);
        public PassingData data;
        Player target;
        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.AskRecieve;
        public UnityEvent FinishEvent { get; } = new();

        public PhaseManager.PhaseData nextPhase = null;

        public override void Enter(Manager manager, object parameter)
        {
            base.Enter(manager);
            if (parameter != null)
                data = parameter as PassingData;

            target = data.queue.Dequeue();
            if (target == manager.CurrentPlayer)
            {
                nextPhase = CreateRecivePhase();
                forceExit = true;
                return;
            }
            else
                forceExit = false;


            target.PlayerStandalone
                  .PacketHandler
                  .AcceptCardPacketEvent += OnFinish;
            target.PlayerStandalone
                  .PacketHandler
                  .RejectCardPacketEvent += OnFinish;

            manager.AddQuest(target, QUEST, FinishEvent);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() =>
            target.PhaseQuestStatus.Quest
                    .Contains(QUEST);

        public override void Exit()
        {
            if (Check())
            {
                manager.FinishQuest(target, QUEST);
                nextPhase = new(PhaseType.Passing);
            }

            manager.PhaseManager.Insert(nextPhase);

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
            new(PhaseType.Receive, new Phase_Recive.ReciveData(target, data.card));

        PhaseManager.PhaseData CreatePassingPhase() =>
            new(PhaseType.Passing);
    }
}
