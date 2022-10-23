using System.Collections.Generic;
using UnityEngine;

namespace TBL.UI.LogSystem
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

        public static LogBase UseCard(GameActionData.CardActionData ca)
        {
            return new LogBase(
                ca.UsingLog(),
                true,
                false,
                new int[0]
            );
        }

        public static LogBase PlayerDead(NetworkPlayer p)
        {
            return new LogBase(
                $"玩家 {p.playerIndex} ({p.playerName}) 死亡",
                true,
                false,
                new int[0]
            );
        }

        public static LogBase PlayerWin(NetworkPlayer p)
        {
            string msg = "";

            if (p.Team.team != Settings.TeamSetting.TeamEnum.Green)
                msg = $"{p.Team.name} 獲勝";
            else
                msg = $"玩家 {p.playerIndex} ({p.playerName}) 完成醬油任務";

            return new LogBase(
                msg,
                true,
                false,
                new int[0]
            );
        }
    }

    public class RichTextGeneral
    {
        public static string red
        {
            get
            {
                return RichTextHelper.TextWithStyles(
                    "紅色",
                    new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, new Color(.75f, .15f, .15f)),
                    new RichTextHelper.SettingBase(RichTextHelper.Style.Bold)
                );
            }
        }

        public static string blue
        {
            get
            {
                return RichTextHelper.TextWithStyles(
                    "藍色",
                    new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, new Color(.15f, .15f, .8f)),
                    new RichTextHelper.SettingBase(RichTextHelper.Style.Bold)
                );
            }
        }

        public static string black
        {
            get
            {
                return RichTextHelper.TextWithStyles(
                    "黑色",
                    new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, new Color(.4f, .4f, .4f)),
                    new RichTextHelper.SettingBase(RichTextHelper.Style.Bold)
                );
            }
        }
    }
}
