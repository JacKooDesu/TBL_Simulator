using UnityEngine;
using UnityEngine.Events;

namespace TBL.Game.UI.Main
{
    using Core.UI;
    public abstract class OptionItemBase<TData> : MonoBehaviour, ISelectable
    {
        public abstract SelectableType Type { get; }
        public TData Data { get; private set; }
        public virtual OptionItemBase<TData> SetData(TData data)
        {
            Data = data;
            return this;
        }
        public abstract UnityEvent<TData> OnSelect { get; protected set; }
        public virtual OptionItemBase<TData> Disabled()
        {
            gameObject.SetActive(false);
            return this;
        }
    }
}