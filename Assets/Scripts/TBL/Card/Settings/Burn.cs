using System.Collections;
using System.Collections.Generic;
using TBL.GameActionData;
using UnityEngine;

namespace TBL.Card
{
    using static CardAttributeHelper;
    public class Burn : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);
            NetworkRoomManager manager = NetworkRoomManager.singleton as NetworkRoomManager;
            NetCanvas.GameScene netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            List<int> playerList = new List<int>();
            foreach (NetworkPlayer p in manager.players)
            {
                if (p.GetCardCount(Black) != 0)
                    playerList.Add(p.playerIndex);
            }

            netCanvas.BindSelectPlayer(playerList, (i) =>
            {
                netCanvas.ShowPlayerCard(
                    i,
                    (j) => user.CmdTestCardAction(new CardActionData(user.playerIndex, i, ID, originID, j)),
                    CardAttributeHelper.Black
                );
            });
        }

        public override void OnEffect(NetworkRoomManager manager, CardActionData ca)
        {
            base.OnEffect(manager, ca);

            manager.players[ca.target].netCards.Remove(ca.suffix);
        }
    }
}

