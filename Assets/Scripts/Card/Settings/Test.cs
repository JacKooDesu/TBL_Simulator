using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.Action;

namespace TBL.Card
{
    public class Test : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);
        }

        // only run on server
        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);
        }
    }
}

