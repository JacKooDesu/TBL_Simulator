using System;
using UnityEngine.Events;

namespace TBL.Game
{
    [Serializable]

    public abstract class ValueTypeStatus<T> : IPlayerStatus<T>
    where T : struct
    {
        public ValueTypeStatus(T value)
        {
            this.value = value;
        }

        T value;

        public UnityEvent<T> OnChanged { get; } = new();

        public T Current() => this.value;
        public abstract PlayerStatusType Type();

        public void Update(T value)
        {
            var old = this.value;
            this.value = value;
            if (!old.Equals(this.value))
                OnChanged.Invoke(value);
        }

        public void Update<S>(S status) where S : IPlayerStatus<T>
        {
            var old = this.value;
            this.value = status.Current();
            if (!old.Equals(this.value))
                OnChanged.Invoke(value);
        }

        public void Update(IPlayerStatus value)
            => Update(value as ValueTypeStatus<T>);
    }
}