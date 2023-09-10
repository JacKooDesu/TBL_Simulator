using System;
using Newtonsoft.Json;
using TBL.Game.Hero;
using UnityEngine.Events;

namespace TBL.Game
{
    [System.Serializable, JsonObject]
    public class HeroStatus : IPlayerStatus<HeroStatus>
    {
        public HeroStatus() => HeroId = int.MinValue;
        public HeroStatus(int heroId)
        {
            this.HeroId = heroId;
        }

        [JsonProperty]
        public int HeroId { get; private set; }
        [JsonProperty]
        public bool isHiding { get; private set; }

        public UnityEvent<HeroStatus> OnChanged { get; } = new();

        public HeroStatus Current() => this;

        public PlayerStatusType Type() => PlayerStatusType.Hero;

        public void Update(HeroStatus value)
        {
            HeroId = value.HeroId;
            OnChanged.Invoke(this);
        }

        public void Update<S>(S status) where S : IPlayerStatus<HeroStatus>
        {
            HeroId = status.Current().HeroId;
            OnChanged.Invoke(this);
        }

        public void Update(IPlayerStatus value) =>
            Update(value as HeroStatus);
    }
}

