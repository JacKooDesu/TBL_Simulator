using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Card
{
    public class Skip : CardSetting
    {
        public override void OnUse(NetworkPlayer user)
        {
            base.OnUse(user);
            NetworkRoomManager manager = NetworkRoomManager.singleton as NetworkRoomManager;
            NetCanvas.GameScene netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (i) =>
            {
                user.CmdTestCardAction(new CardAction(user.playerIndex, i, ID, 0));
            });
        }

        // only run on server
        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);
            manager.players[ca.targetIndex].isSkipped = true;
        }
    }
}

