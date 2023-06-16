using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
    public class Phase_Passing : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Passing;
        protected override float time => 5f;
        Player target;
        public Queue<Player> playerQueue = new();
        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.AskRecieve;
        public UnityEvent FinishEvent { get; } = new();

        public override void Enter(Manager manager, object parameter)
        {
            base.Enter(manager);
            this.target = parameter as Player;

            manager.AddQuest(target, QUEST, FinishEvent);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() =>
            target.PhaseQuestStatus.Quest
                    .Contains(QUEST);

        public override void Exit()
        {
            manager.FinishQuest(target, QUEST);
            this.target = null;
        }

        void OnFinish(Unit _)
        {
            FinishEvent.Invoke();
        }
    }
}
