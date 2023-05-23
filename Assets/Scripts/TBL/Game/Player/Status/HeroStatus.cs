using System;

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

        public event Action<HeroStatus> OnChanged;

        public HeroStatus Current() => this;

        public PlayerStatusType Type() => PlayerStatusType.Hero;

        public void Update(IPlayerStatus<HeroStatus> status)
        {
            this.heroId = ((HeroStatus)status).heroId;
        }

        public void Update(HeroStatus value)
        {
            throw new NotImplementedException();
        }

        public void Update<S>(S status) where S : IPlayerStatus<HeroStatus>
        {
            throw new NotImplementedException();
        }
    }
}

