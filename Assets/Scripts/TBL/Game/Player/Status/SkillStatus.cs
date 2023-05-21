using System;
using System.Collections;

namespace TBL.Game
{
    public struct SkillStatus : IPlayerStatus<bool[]>
    {
        bool[] value;
        public bool[] Current() => value;
        public PlayerStatusType Type() => PlayerStatusType.Skill;

        public void Update(bool[] status) => this.value = status;
        public void Update<T>(T status) where T : IList
        {
            value = status as bool[];
        }
    }
}

