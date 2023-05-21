namespace TBL.Game
{
    public struct ProfileStatus : IPlayerStatus<ProfileStatus>
    {
        public ProfileStatus(string name)
        {
            this.name = name;
        }
        
        string name;
        public ProfileStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.ProfileStatus;
        public void Update(IPlayerStatus status) => this = (ProfileStatus)status;
    }
}