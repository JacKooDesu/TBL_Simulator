using System.Collections;
using System.Collections.Generic;
using TBL.GameAction;
using UnityEngine;

namespace TBL.Card
{
    public class Gameble : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);

            user.CmdTestCardAction(new CardAction(user.playerIndex, 0, ID, originID, 0));
        }

        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);

            foreach (NetworkPlayer p in manager.players)
            {
                p.AddCard(manager.DeckManager.DrawCardFromTop().ID);
            }
        }
    }
}

