using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Card
{
    public class Return : CardSetting
    {
        public override void OnUse(NetworkPlayer user)
        {
            base.OnUse(user);

            user.CmdTestCardAction(new CardAction(user.playerIndex, 0, ID, 0));
        }

        // only run on server
        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);
            manager.Judgement.currentSendReverse = true;
        }
    }
}

