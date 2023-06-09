namespace TBL.Game
{
    public class TeamStatus : ValueTypeStatus<TeamEnum>
    {
        public override PlayerStatusType Type() => PlayerStatusType.Team;
        public TeamStatus(TeamEnum value) : base(value) { }
    }
}
