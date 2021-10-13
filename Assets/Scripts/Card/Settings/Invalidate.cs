using System.Collections;
using System.Collections.Generic;
using TBL.Action;
using UnityEngine;

namespace TBL.Card
{
    public class Invalidate : CardSetting
    {
        public override void OnUse(NetworkPlayer user)
        {
            base.OnUse(user);

            user.CmdTestCardAction(new CardAction(user.playerIndex, 0, ID, 0));
        }
    }
}

