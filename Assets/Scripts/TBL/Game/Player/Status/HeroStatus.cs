namespace TBL.Game
{
    [System.Serializable]
    public class HeroStatus : IPlayerStatus<HeroStatus>
    {
        public HeroStatus() => heroId = int.MinValue;
        public HeroStatus(int heroId)
        {
            this.heroId = heroId;
        }

        int heroId;

        public HeroStatus Current() => this;

        public PlayerStatusType Type() => PlayerStatusType.Hero;

        public void Update(IPlayerStatus status)
        {
            this.heroId = ((HeroStatus)status).heroId;
        }
    }
}

