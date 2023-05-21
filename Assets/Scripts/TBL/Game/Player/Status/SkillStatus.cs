using System;
using System.Collections;
using System.Collections.Generic;

namespace TBL.Game
{
    [System.Serializable]
    public class SkillStatus : IPlayerStatus<Dictionary<int,bool>>
    {
        Dictionary<int,bool> value;
        public Dictionary<int,bool> Current() => value;
        public PlayerStatusType Type() => PlayerStatusType.Skill;

        public void Update(Dictionary<int,bool> status) => this.value = status;
        public void Update<T>(T status) where T : IList
        {
            value = status as Dictionary<int,bool>;
        }

        public void Update(IPlayerStatus value)
        {
            this.value = ((SkillStatus)value).value;
        }
    }
}

