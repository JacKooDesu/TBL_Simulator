using System;

namespace TBL.Card
{
    public struct CardAction
    {
        public int userIndex;
        public int targetIndex;
        public int cardID;
        public int suffix;

        public CardAction(int user, int target, int id, int suffix)
        {
            this.userIndex = user;
            this.targetIndex = target;
            this.cardID = id;
            this.suffix = suffix;
        }
    }
}
