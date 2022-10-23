namespace TBL.Hero
{
    public class HeroMission
    {
        public string description;
        public System.Func<bool> checker;

        public HeroMission(string description, System.Func<bool> checker)
        {
            this.description = description;
            this.checker = checker;
        }
    }
}