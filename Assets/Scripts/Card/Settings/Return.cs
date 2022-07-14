using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.GameAction;

namespace TBL.Card
{
    public class Return : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);

            user.CmdTestCardAction(new CardAction(user.playerIndex, 0, ID, originID, 0));
        }

        // only run on server
        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);
            manager.Judgement.currentSendReverse = true;
        }
    }
}

