namespace TBL.Game
{
    public struct TeamStatus : IPlayerStatus<TeamStatus>
    {
        public TeamStatus(TeamEnum team)
        {
            this.team = team;
        }

        public TeamEnum team;

        public TeamStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.TeamStatus;
        public void Update(IPlayerStatus status) => this = (TeamStatus)status;
    }

    public enum TeamEnum
    {
        Blue,
        Red,
        Green
    }
}