using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.GameAction;

namespace TBL.Card
{
    public class Skip : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);
            NetworkRoomManager manager = NetworkRoomManager.singleton as NetworkRoomManager;
            NetCanvas.GameScene netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            netCanvas.BindSelectPlayer(manager.GetAllPlayers(), (i) =>
            {
                user.CmdTestCardAction(new CardAction(user.playerIndex, i, ID, originID, 0));
            });
        }

        // only run on server
        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);
            manager.players[ca.target].isSkipped = true;
        }
    }
}

