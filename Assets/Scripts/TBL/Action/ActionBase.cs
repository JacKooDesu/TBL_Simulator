﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.GameActionData
{
    [System.Serializable]
    public struct CardActionData
    {
        public int user;
        public int target;
        public int cardId;
        public int originCardId;
        public int suffix;

        public CardActionData(int user, int target, int cardId, int originCardId, int suffix)
        {
            this.user = user;
            this.target = target;
            this.cardId = cardId;
            this.originCardId = originCardId;
            this.suffix = suffix;
        }

        public string UsingLog()
        {
            var card = (Card.CardSetting)cardId;
            var user = (NetworkRoomManager.singleton as NetworkRoomManager).players[this.user];
            string log = $"玩家 {user.playerIndex} ({user.playerName}) ";
            NetworkPlayer target;
            switch (card.CardType)
            {
                case Card.CardType.Invalidate:
                case Card.CardType.Guess:
                case Card.CardType.Gameble:
                case Card.CardType.Return:
                case Card.CardType.Intercept:
                    break;

                case Card.CardType.Lock:
                case Card.CardType.Skip:
                case Card.CardType.Test:
                    target = (NetworkRoomManager.singleton as NetworkRoomManager).players[this.target];
                    log += $"對 玩家 {target.playerIndex} ({target.playerName}) ";
                    break;

                case Card.CardType.Burn:
                    target = (NetworkRoomManager.singleton as NetworkRoomManager).players[this.target];
                    var targetCard = (Card.CardSetting)target.netCards[suffix];
                    log += $"對 玩家 {target.playerIndex} ({target.playerName}) 的 {RichTextHelper.TextWithBold(targetCard.CardName)} ";
                    break;
            }
            if (suffix == -1)
                log += $"用技能發動 {RichTextHelper.TextWithBold(card.CardName)}";
            else
                log += $"使用 {RichTextHelper.TextWithBold(card.CardName)}";

            return log;
        }

        public string EffectLog()
        {
            var card = (Card.CardSetting)cardId;
            var user = (NetworkRoomManager.singleton as NetworkRoomManager).players[this.user];
            string log = $"玩家 {user.playerIndex} ({user.playerName}) ";
            NetworkPlayer target;
            switch (card.CardType)
            {
                case Card.CardType.Invalidate:
                case Card.CardType.Guess:
                case Card.CardType.Gameble:
                case Card.CardType.Return:
                case Card.CardType.Intercept:
                    break;

                case Card.CardType.Lock:
                case Card.CardType.Skip:
                case Card.CardType.Test:
                    target = (NetworkRoomManager.singleton as NetworkRoomManager).players[this.target];
                    log += $"對 玩家 {target.playerIndex} ({target.playerName}) ";
                    break;

                case Card.CardType.Burn:
                    target = (NetworkRoomManager.singleton as NetworkRoomManager).players[this.target];
                    var targetCard = (Card.CardSetting)target.netCards[suffix];
                    log += $"對 玩家 {target.playerIndex} ({target.playerName}) 的 {RichTextHelper.TextWithBold(targetCard.CardName)} ";
                    break;
            }
            log += $"發動 {RichTextHelper.TextWithBold(card.CardName)} 成功";

            return log;
        }
    }

    public struct SkillActionData
    {
        public int user;
        public int target;
        public int skill;
        public int suffix;

        public SkillActionData(int user = int.MinValue, int target = int.MinValue, int skill = int.MinValue, int suffix = int.MinValue)
        {
            this.user = user;
            this.target = target;
            this.skill = skill;
            this.suffix = suffix;
        }

        public SkillActionData Default()
        {
            this.user = int.MinValue;
            this.target = int.MinValue;
            this.skill = int.MinValue;
            this.suffix = int.MinValue;

            return this;
        }
    }

    public class ClassifyStruct<T> where T : struct
    {
        public ClassifyStruct()
        {
            data = default(T);
        }
        public ClassifyStruct(T data)
        {
            this.data = data;
        }
        public T data;

        public List<object> tempObjects = new List<object>();
    }
}