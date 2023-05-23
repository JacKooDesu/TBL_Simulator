using System;
namespace TBL.Game
{
    [Serializable]
    public class ProfileStatus : IPlayerStatus<ProfileStatus>
    {
        public ProfileStatus() => this.name = "";
        public ProfileStatus(string name)
        {
            this.name = name;
        }

        string name;

        public event Action<ProfileStatus> OnChanged;

        public ProfileStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.ProfileStatus;
        public void Update(ProfileStatus value)
        {
            throw new NotImplementedException();
        }

        public void Update<S>(S status) where S : IPlayerStatus<ProfileStatus>
        {
            throw new NotImplementedException();
        }
    }
}