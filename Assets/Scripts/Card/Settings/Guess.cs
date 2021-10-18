using System.Collections;
using System.Collections.Generic;
using TBL.Action;
using UnityEngine;

namespace TBL.Card
{
    public class Guess : CardSetting
    {
        public override void OnUse(NetworkPlayer user, int originID)
        {
            base.OnUse(user, originID);
            NetworkRoomManager manager = NetworkRoomManager.singleton as NetworkRoomManager;
            NetCanvas.GameScene netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            netCanvas.AskColorCard(
                (j) => user.CmdTestCardAction(new CardAction(user.playerIndex, user.playerIndex, ID, originID, j)),
                new List<CardColor> { CardColor.Red, CardColor.Blue, CardColor.Black }
            );
        }

        public override void OnEffect(NetworkRoomManager manager, CardAction ca)
        {
            base.OnEffect(manager, ca);

            if ((CardColor)ca.suffix == CardSetting.IDConvertCard(manager.Judgement.currentRoundSendingCardId).CardColor)
            {
                manager.players[ca.user].DrawCard(1);
            }
        }
    }
}

