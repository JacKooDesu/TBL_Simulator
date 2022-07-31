using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.GameAction;

namespace TBL.Card
{
    using static CardAttributeHelper;
    [System.Serializable]
    public class CardSetting : MonoBehaviour
    {
        [SerializeField, Header("種類")]
        CardType cardType;
        public CardType CardType
        {
            get => cardType;
        }

        [SerializeField, Header("傳遞方式")]
        CardSendType sendType;
        public CardSendType SendType
        {
            get => sendType;
        }

        [Header("使用方法"), TextArea(3, 10)]
        [SerializeField] protected string tip;
        public virtual string Tip
        {
            get => tip;
        }

        CardColor cardColor;
        public CardColor CardColor
        {
            get => cardColor;
        }

        [SerializeField] int id;
        public int ID
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
                // if(Compare(value, Black))
                //     cardColor=CardColor.Black;
                if (Compare(value, Black))
                    cardColor = CardColor.Black;
                if (Compare(value, Red))
                    cardColor = CardColor.Red;
                if (Compare(value, Blue))
                    cardColor = CardColor.Blue;

                // SendType
                if (Compare(value, Direct))
                    sendType = CardSendType.Direct;
                if (Compare(value, Secret))
                    sendType = CardSendType.Secret;
                if (Compare(value, Public))
                    sendType = CardSendType.Public;

                // CardType
                if (Compare(value, CardAttributeHelper.Burn))
                    cardType = CardType.Burn;
                if (Compare(value, CardAttributeHelper.Gameble))
                    cardType = CardType.Gameble;
                if (Compare(value, CardAttributeHelper.Guess))
                    cardType = CardType.Guess;
                if (Compare(value, CardAttributeHelper.Intercept))
                    cardType = CardType.Intercept;
                if (Compare(value, CardAttributeHelper.Invalidate))
                    cardType = CardType.Invalidate;
                if (Compare(value, CardAttributeHelper.Lock))
                    cardType = CardType.Lock;
                if (Compare(value, CardAttributeHelper.Return))
                    cardType = CardType.Return;
                if (Compare(value, CardAttributeHelper.Skip))
                    cardType = CardType.Skip;
                if (Compare(value, CardAttributeHelper.Test))
                    cardType = CardType.Test;
            }
        }

        public int GetUniqueID()
        {
            int i = DeckSetting.cardUniqueId;
            while ((id & 1 << i) != (1 << i) && i < 32)
            {
                ++i;
            }
            return i + 1 - DeckSetting.cardUniqueId;
        }

        public int GetUniqueID(int id)
        {
            int i = DeckSetting.cardUniqueId;
            while ((id & 1 << i) != (1 << i) && i < 32)
            {
                ++i;
            }
            return i + 1 - DeckSetting.cardUniqueId;
        }

        public string SendTypeText
        {
            get
            {
                switch (sendType)
                {
                    case CardSendType.Direct: return "直達";
                    case CardSendType.Secret: return "密電";
                    case CardSendType.Public: return "公開文本";
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
            get => $"({ColorToString(cardColor)})";
        }

        public string GetCardNameFully()
        {
            return "(" + SendTypeText + ") " + CardName + " " + ColorText;
        }

        static GameObject tempCard;

        public static CardSetting IdToCard(int id)
        {
            if (tempCard == null)
            {
                tempCard = new GameObject("Temp Card", typeof(CardSetting));
            }

            CardSetting c = tempCard.GetComponent<CardSetting>();
            c.ID = id;
            return c;
        }

        public static CardSetting IdToCardAsObject(int id)
        {
            CardSetting c = new CardSetting();
            c.ID = id;
            return c;
        }

        public static explicit operator CardSetting(int id)
        {
            return IdToCardAsObject(id);
        }

        public static string ColorToString(CardColor cardColor)
        {
            switch (cardColor)
            {
                case CardColor.Black: return "黑";
                case CardColor.Red: return "紅";
                case CardColor.Blue: return "藍";
            }

            Debug.LogWarning("CARD COLOR CONVERT : Missing card color");
            return " ";
        }

        public virtual void OnUse(NetworkPlayer user, int originID)
        {

        }

        // only run on server
        public virtual void OnEffect(NetworkRoomManager manager, CardAction ca)
        {

        }
    }
}

