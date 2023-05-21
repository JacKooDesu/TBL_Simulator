namespace TBL.Game
{
    public struct HeroStatus : IPlayerStatus<HeroStatus>
    {
        public HeroStatus(int heroId)
        {
            this.heroId = heroId;
        }

        int heroId;

        public HeroStatus Current() => this;

        public PlayerStatusType Type() => PlayerStatusType.Hero;

        public void Update(IPlayerStatus status) => this = (HeroStatus)status;
    }
}

