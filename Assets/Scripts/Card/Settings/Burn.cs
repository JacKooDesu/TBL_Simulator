using System.Collections;
using System.Collections.Generic;
using TBL.GameAction;
using UnityEngine;

namespace TBL.Card
{
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
                bool hasBlack = false;
                foreach (int i in p.netCards)
                {
                    if (CardSetting.IdToCard(i).CardColor == CardColor.Black)
                    {
                        hasBlack = true;
                        break;
                    }
                }

                if (hasBlack)
                    playerList.Add(p.playerIndex);
            }

            netCanvas.BindSelectPlayer(playerList, (i) =>
            {
                netCanvas.ShowPlayerCard(
                    i,
                    (j) => user.CmdTestCardAction(new CardAction(user.playerIndex, i, ID, originID, j)),
                    new List<CardColor> { CardColor.Black }
                );
            });
        }

        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);

            manager.players[ca.target].netCards.Remove(ca.suffix);
        }
    }
}

