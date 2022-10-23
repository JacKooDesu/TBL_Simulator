using TBL;

namespace TBL.Judgement.Phase
{
    public abstract class PhaseBase
    {
        public abstract void Init(NetworkJudgement judgement);
        public abstract void Updater(NetworkJudgement judgement);
    }
}
