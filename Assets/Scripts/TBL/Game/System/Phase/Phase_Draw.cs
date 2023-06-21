using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
    public class Phase_Draw : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Draw;
        public override string PhaseName => "抽牌階段";
        protected override float time => 5f;
        const int DRAW_COUNT = 2;
        const PhaseQuestStatus.QuestType QUEST = PhaseQuestStatus.QuestType.DrawCard;

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
            manager.FinishQuest(manager.CurrentPlayer, QUEST);
            manager.Draw(manager.CurrentPlayer, 2);
        }
    }
}
