using System.Collections;
using System.Collections.Generic;
using TBL.GameAction;
using UnityEngine;

namespace TBL.Card
{
    public class Intercept : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);

            user.CmdTestCardAction(new CardAction(user.playerIndex, 0, ID, originID, 0));
        }

        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);

            manager.Judgement.playerIntercept = ca.user;
        }
    }
}

