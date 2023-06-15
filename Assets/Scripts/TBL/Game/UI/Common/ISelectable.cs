using System;
using UnityEngine.Events;

namespace TBL.Game.UI
{
    public interface ISelectable<T> : ISelectable
    {
        UnityEvent<T> OnSelectEvent { get; }
        T data { get; }
    }

    public interface ISelectable
    {
        SelectableType Type { get; }
    }

    public enum SelectableType
    {
        Card,
    }
}
