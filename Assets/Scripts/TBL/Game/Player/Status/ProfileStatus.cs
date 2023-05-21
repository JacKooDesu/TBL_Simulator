namespace TBL.Game
{
    [System.Serializable]
    public class ProfileStatus : IPlayerStatus<ProfileStatus>
    {
        public ProfileStatus() => this.name = "";
        public ProfileStatus(string name)
        {
            this.name = name;
        }

        string name;
        public ProfileStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.ProfileStatus;
        public void Update(IPlayerStatus status)
        {
            this.name = ((ProfileStatus)status).name;
        }
    }
}