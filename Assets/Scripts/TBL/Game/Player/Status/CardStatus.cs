using System.Collections.Generic;

namespace TBL.Game
{
    public struct CardStatus : IPlayerStatus<CardStatus>
    {
        public CardStatus(List<int> hand = null, List<int> table = null)
        {
            this.hand = hand ?? new List<int>();
            this.table = table ?? new List<int>();
        }

        List<int> hand;
        List<int> table;

        public CardStatus Current() => this;
        public PlayerStatusType Type() => PlayerStatusType.CardStatus;
        public void Update(IPlayerStatus status) => this = (CardStatus)status;
    }
}