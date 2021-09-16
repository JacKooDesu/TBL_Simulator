﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Card
{
    [System.Serializable]
    public class CardSetting : MonoBehaviour
    {
        [SerializeField, Header("種類")]
        CardType cardType;

        [SerializeField, Header("傳遞方式")]
        CardSendType sendType;

        CardColor cardColor;

        [SerializeField] ushort id;
        public ushort ID
        {
            get
            {
                return id == 0 ?
                    System.Convert.ToUInt16((int)cardType + (int)cardColor + (int)sendType) :
                    id;
            }
            set
            {
                id = value;
                // Color
                if ((value & (ushort)CardColor.Black) == (ushort)CardColor.Black)
                    cardColor = CardColor.Black;
                if ((value & (ushort)CardColor.Red) == (ushort)CardColor.Red)
                    cardColor = CardColor.Red;
                if ((value & (ushort)CardColor.Blue) == (ushort)CardColor.Blue)
                    cardColor = CardColor.Blue;

                // SendType
                if ((value & (ushort)CardSendType.Direct) == (ushort)CardSendType.Direct)
                    sendType = CardSendType.Direct;
                if ((value & (ushort)CardSendType.Secret) == (ushort)CardSendType.Secret)
                    sendType = CardSendType.Secret;
                if ((value & (ushort)CardSendType.Public) == (ushort)CardSendType.Public)
                    sendType = CardSendType.Public;

                // CardType
                if ((value & (ushort)CardType.Burn) == (ushort)CardType.Burn)
                    cardType = CardType.Burn;
                if ((value & (ushort)CardType.Gameble) == (ushort)CardType.Gameble)
                    cardType = CardType.Gameble;
                if ((value & (ushort)CardType.Guess) == (ushort)CardType.Guess)
                    cardType = CardType.Guess;
                if ((value & (ushort)CardType.Intercept) == (ushort)CardType.Intercept)
                    cardType = CardType.Intercept;
                if ((value & (ushort)CardType.Invalidate) == (ushort)CardType.Invalidate)
                    cardType = CardType.Invalidate;
                if ((value & (ushort)CardType.Lock) == (ushort)CardType.Lock)
                    cardType = CardType.Lock;
                if ((value & (ushort)CardType.Return) == (ushort)CardType.Return)
                    cardType = CardType.Return;
                if ((value & (ushort)CardType.Skip) == (ushort)CardType.Skip)
                    cardType = CardType.Skip;
                if ((value & (ushort)CardType.Test) == (ushort)CardType.Test)
                    cardType = CardType.Test;

            }
        }

        public string SendTypeText
        {
            get
            {
                switch (sendType)
                {
                    case CardSendType.Direct: return "直達";
                    case CardSendType.Secret: return "密電";
                    case CardSendType.Public: return "文本";
                }

                return "MISSING SEND TYPE";
            }
        }
        public string CardName
        {
            get
            {
                switch (cardType)
                {
                    case CardType.Lock:
                        return "鎖定";

                    case CardType.Test:
                        return "試探";

                    case CardType.Gameble:
                        return "真偽莫辨";

                    case CardType.Guess:
                        return "破譯";

                    case CardType.Burn:
                        return "燒毀";

                    case CardType.Skip:
                        return "調虎離山";

                    case CardType.Return:
                        return "退回";

                    case CardType.Intercept:
                        return "截獲";

                    case CardType.Invalidate:
                        return "識破";
                }
                return "MISSING CARD TYPE";
            }
        }

        public Color Color
        {
            get
            {
                switch (cardColor)
                {
                    case CardColor.Black:
                        return new Color(.4f, .4f, .4f);

                    case CardColor.Red:
                        return new Color(.75f, .15f, .15f);


                    case CardColor.Blue:
                        return new Color(.15f, .15f, .8f);
                }

                Debug.Log("MISSING CARD COLOR");
                return new Color(0f, 0f, 0f);

            }

        }

        public string ColorText
        {
            get
            {
                switch (cardColor)
                {
                    case CardColor.Black: return "(黑)";
                    case CardColor.Red: return "(紅)";
                    case CardColor.Blue: return "(藍)";
                }

                Debug.Log("MISSING CARD COLOR");
                return "MISSING CARD COLOR";
            }
        }

        public string GetCardNameFully()
        {
            return "(" + SendTypeText + ") " + CardName + " " + ColorText;
        }

        public static CardSetting IDConvertCard(ushort id)
        {
            CardSetting c = new CardSetting();
            c.ID = id;
            return c;
        }
    }
}
