using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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

        public event Action<CardStatus> OnChanged;

        public CardStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.Card;

        public void Update(IPlayerStatus<CardStatus> status)
        {
            var old = this;
            var target = status.Current();
            this.hand = target.hand;
            this.table = target.table;
            OnChanged.Invoke(this);
        }

        public void Update(CardStatus value)
        {
            throw new NotImplementedException();
        }

        public void Update<S>(S status) where S : IPlayerStatus<CardStatus>
        {
            throw new NotImplementedException();
        }
    }
}