using System.Collections.Generic;
using UnityEngine;

namespace TBL.UI
{
    public class LogGeneral
    {
        public static LogBase RoundStart(NetworkPlayer player)
        {
            return new LogBase(
                $"玩家 {player.playerIndex} ({player.playerName}) 回合開始",
                false,
                false,
                new int[0]
            );
        }

        public static LogBase SendCard(NetworkPlayer from, Card.CardSetting card)
        {
            string cardName = $"{card.SendTypeText}";
            cardName = RichTextHelper.TextWithBold(cardName);
            if (card.SendType == Card.CardSendType.Public)
                cardName = RichTextHelper.TextWithColor(cardName, card.Color);
            else
                cardName = RichTextHelper.TextWithColor(cardName, Color.magenta);

            return new LogBase(
                $"玩家 {from.playerIndex} ({from.playerName}) 傳送了 {cardName}",
                true,
                false,
                new int[0]
            );
        }

        public static LogBase AcceptCard(NetworkPlayer accept, Card.CardSetting card)
        {
            string cardName = $"{card.SendTypeText}";
            cardName = RichTextHelper.TextWithBold(cardName);
            cardName = RichTextHelper.TextWithColor(cardName, card.Color);

            return new LogBase(
                $"玩家 {accept.playerIndex} ({accept.playerName}) 接收 {cardName}",
                true,
                false,
                new int[0]
            );
        }

        public static LogBase RoundTimeOver(NetworkPlayer player)
        {
            return new LogBase(
                $"玩家 {player.playerIndex} {player.playerName} 操作逾時",
                true,
                false,
                new int[0]
            );
        }

        public static LogBase UseCard(Action.CardAction ca)
        {
            return new LogBase(
                ca.UsingLog(),
                true,
                false,
                new int[0]
            );
        }
    }
}
