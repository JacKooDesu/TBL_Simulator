using System;
namespace TBL.Game
{
    [Serializable]

    public abstract class ValueTypeStatus<T> : IPlayerStatus<T>
    where T : struct
    {
        public ValueTypeStatus(T value)
        {
            this.value = value;
            OnChanged = (v) => Current();
        }

        T value;

        public event Action<T> OnChanged = _ => { };

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
    }
}