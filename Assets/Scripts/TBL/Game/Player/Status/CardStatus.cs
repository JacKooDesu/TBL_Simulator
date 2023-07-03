using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using UnityEngine.Events;

namespace TBL.Game
{
    [System.Serializable]

    public class CardStatus : IPlayerStatus<CardStatus>
    {
        public CardStatus(List<int> hand = null, List<int> table = null)
        {
            this.hand = hand ?? new List<int>();
            this.table = table ?? new List<int>();
        }

        List<int> hand;
        List<int> table;
        public ReadOnlyCollection<int> Hand => hand.AsReadOnly();
        public ReadOnlyCollection<int> Table => table.AsReadOnly();

        public UnityEvent<CardStatus> OnChanged { get; } = new();

        public CardStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.Card;

        public void Update(CardStatus value)
        {
            var old = this;
            this.hand = value.hand;
            this.table = value.table;
            OnChanged.Invoke(this);
        }

        public void Update<S>(S status)
        where S : IPlayerStatus<CardStatus> =>
            Update(status);

        public void Update(IPlayerStatus value) =>
            Update(value as CardStatus);

        #region SERVER_ONLY
        public void AddHandCards(params int[] ids)
        {
            hand.AddRange(ids);
            OnChanged.Invoke(this);
        }

        public void AddTableCards(params int[] ids)
        {
            table.AddRange(ids);
            OnChanged.Invoke(this);
        }

        public void RemoveHandCards(params int[] ids)
        {
            foreach (var id in ids)
                hand.Remove(id);

            OnChanged.Invoke(this);
        }

        public void RemoveTableCards(params int[] ids)
        {
            foreach (var id in ids)
                table.Remove(id);

            OnChanged.Invoke(this);
        }
        #endregion
    }
}