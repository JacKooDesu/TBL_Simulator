using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
    public class Phase_Main : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Main;
        protected override float time => 5f;
        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.SelectCard;

        public override void Enter(Manager manager, object parameter = null)
        {
            base.Enter(manager);
            var p = manager.CurrentPlayer;
            manager.AddQuest(p, QUEST);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() =>
            manager.CurrentPlayer
                    .PhaseQuestStatus.Quest
                    .Contains(QUEST);

        public override void Exit()
        {
            if (Check())
                manager.DiscardHandAll(manager.CurrentPlayer);

            manager.FinishQuest(manager.CurrentPlayer, QUEST);
        }
    }
}
