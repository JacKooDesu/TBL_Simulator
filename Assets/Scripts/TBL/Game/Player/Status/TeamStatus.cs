namespace TBL.Game
{
    [System.Serializable]
    public class TeamStatus : IPlayerStatus<TeamStatus>
    {
        public TeamStatus()
        {
            team = default;
        }

        public TeamStatus(TeamEnum team)
        {
            this.team = team;
        }

        public TeamEnum team;

        public TeamStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.TeamStatus;
        public void Update(IPlayerStatus status) => Update((TeamStatus)status);
        void Update(TeamStatus status)
        {
            this.team = status.team;
        }
    }

    public enum TeamEnum
    {
        Blue,
        Red,
        Green
    }
}