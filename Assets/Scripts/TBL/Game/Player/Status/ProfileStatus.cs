using System;
namespace TBL.Game
{
    [Serializable]
    public class ProfileStatus : IPlayerStatus<ProfileStatus>
    {
        public ProfileStatus() => this.name = "";
        public ProfileStatus(string name, int id)
        {
            this.name = name;
            this.id = id;
        }

        string name;
        /// <summary>
        /// 玩家遊戲中順位。
        /// </summary>
        int id;
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
    }
}