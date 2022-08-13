﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.GameActionData;

namespace TBL.Card
{
    public class Lock : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);
            NetworkRoomManager manager = NetworkRoomManager.singleton as NetworkRoomManager;
            NetCanvas.GameScene netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (i) =>
            {
                user.CmdTestCardAction(new CardActionData(user.playerIndex, i, ID, originID, 0));
            });
        }

        // only run on server
        public override void OnEffect(NetworkRoomManager manager, CardActionData ca)
        {
            base.OnEffect(manager, ca);
            manager.players[ca.target].isLocked = true;

            print($"玩家{ca.user} 鎖定 玩家{ca.target}");
        }
    }
}

