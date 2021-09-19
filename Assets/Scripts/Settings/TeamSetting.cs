using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Settings
{
    [CreateAssetMenu(fileName = "TeamList", menuName = "TBL/Settings/Team List", order = 0)]
    public class TeamSetting : ScriptableObject
    {
        public enum TeamEnum
        {
            Blue,
            Red,
            Green
        }
        [System.Serializable]
        public class Team
        {
            public string name;
            public Sprite icon;
            public TeamEnum team;
        }

        public Team BlueTeam;
        public Team RedTeam;
        public Team GreenTeam;

        [System.Serializable]
        public class TeamPlayerCount
        {
            public int playerCount;

            public int Blue;
            public int Red;
            public int Green;
        }

        public List<TeamPlayerCount> teamPlayerCountSetting = new List<TeamPlayerCount>();
    }
}