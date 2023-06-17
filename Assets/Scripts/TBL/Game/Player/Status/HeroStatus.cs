using System;

namespace TBL.Game
{
    [System.Serializable]
    public class HeroStatus : IPlayerStatus<HeroStatus>
    {
        public HeroStatus() => HeroId = int.MinValue;
        public HeroStatus(int heroId)
        {
            this.HeroId = heroId;
        }

        public int HeroId { get; private set; }
        public bool isHiding { get; private set; }

        public event Action<HeroStatus> OnChanged;

        public HeroStatus Current() => this;

        public PlayerStatusType Type() => PlayerStatusType.Hero;

        public void Update(HeroStatus value)
        {
            // throw new NotImplementedException();
        }

        public void Update<S>(S status) where S : IPlayerStatus<HeroStatus>
        {
            // throw new NotImplementedException();
        }

        public void Update(IPlayerStatus value) =>
            Update(value as HeroStatus);
    }
}

