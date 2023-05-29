namespace TBL.Game
{
    public class TeamStatus : ValueTypeStatus<TeamEnum>
    {
        public TeamStatus(TeamEnum value, PlayerStatusType type = PlayerStatusType.Team) : base(value, type) { }
    }
}
