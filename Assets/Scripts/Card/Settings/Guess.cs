using System.Collections;
using System.Collections.Generic;
using TBL.Action;
using UnityEngine;

namespace TBL.Card
{
    public class Guess : CardSetting
    {
        public override void OnUse(NetworkPlayer user)
        {
            base.OnUse(user);

            user.CmdTestCardAction(new CardAction(user.playerIndex, 0, ID, 0));
        }

        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);

            
        }
    }
}

