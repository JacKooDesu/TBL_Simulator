using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace TBL.Game
{
    [System.Serializable]
    public class SkillStatus : IPlayerStatus<Dictionary<int, bool>>
    {
        Dictionary<int, bool> value;

        public UnityEvent<Dictionary<int, bool>> OnChanged { get; } = new();

        public Dictionary<int, bool> Current() => value;
        public PlayerStatusType Type() => PlayerStatusType.Skill;

        public void Update<S>(S status) where S : IPlayerStatus<Dictionary<int, bool>>
        {
            this.value = status.Current();
        }

        public void Update(Dictionary<int, bool> value)
        {
            throw new NotImplementedException();
        }

        public void Update(IPlayerStatus value) =>
            Update(value as SkillStatus);
    }
}

