using System;
namespace TBL.Game
{
    [Serializable]

    public abstract class ValueTypeStatus<T> : IPlayerStatus<T>
    where T : struct
    {
        public ValueTypeStatus(T value, PlayerStatusType type)
        {
            this.value = value;
            this.type = type;
            OnChanged = (v) => Current();
        }

        readonly PlayerStatusType type;
        T value;

        public event Action<T> OnChanged;

        public T Current() => this.value;
        public PlayerStatusType Type() => type;

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
    }
}