using System;
using Newtonsoft.Json;
namespace TBL.Game
{
    [Serializable,JsonObject]
    public class ProfileStatus : IPlayerStatus<ProfileStatus>
    {
        public ProfileStatus() { }
        public ProfileStatus(string name, int id)
        {
            this.name = name;
            this.id = id;
        }

        [JsonProperty]
        string name;
        public string Name => name;
        /// <summary>
        /// 玩家遊戲中順位。
        /// </summary>
        [JsonProperty]
        int id;
        /// <summary>
        /// 玩家遊戲中順位。
        /// </summary>
        public int Id => id;

        public event Action<ProfileStatus> OnChanged = delegate { };

        public ProfileStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.Profile;
        public void Update(ProfileStatus value)
        {
            this.name = value.name;
            this.id = value.id;
            OnChanged.Invoke(this);
        }

        public void Update<S>(S status)
        where S : IPlayerStatus<ProfileStatus>
            => Update(status);

        public void Update(IPlayerStatus value) =>
            Update(value as ProfileStatus);
    }
}