using System.Collections;
using System.Collections.Generic;
using TBL.GameActionData;
using UnityEngine;

namespace TBL.Card
{
    public class Invalidate : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);

            user.CmdTestCardAction(new CardActionData(user.playerIndex, 0, ID, originID, 0));
        }
    }
}

