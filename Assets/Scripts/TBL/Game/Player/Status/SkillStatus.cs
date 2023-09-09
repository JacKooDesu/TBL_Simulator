using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace TBL.Game
{
    [System.Serializable]
    public class SkillStatus : IPlayerStatus<SkillStatus>
    {
        bool[] value;

        public SkillStatus() { }
        public SkillStatus(bool[] value)
        {
            this.value = value;
        }

        public UnityEvent<SkillStatus> OnChanged { get; } = new();
        public SkillStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.Skill;

        public void Update(int index, bool value)
        {
            if (index >= this.value.Length ||
                !this.value[index] ^ value)
                return;

            this.value[index] = value;
            OnChanged.Invoke(this);
        }

        public void Update<S>(S status) where S : IPlayerStatus<SkillStatus>
        {
            value = status.Current().value;
            OnChanged.Invoke(this);
        }

        public void Update(SkillStatus s)
        {
            this.value = s.value;
            OnChanged.Invoke(this);
        }

        public void Update(IPlayerStatus s) =>
            Update(s as SkillStatus);
    }
}

