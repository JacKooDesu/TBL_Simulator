using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.Action;

namespace TBL.Card
{
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
                if ((value & (int)CardColor.Black) == (int)CardColor.Black)
                    cardColor = CardColor.Black;
                if ((value & (int)CardColor.Red) == (int)CardColor.Red)
                    cardColor = CardColor.Red;
                if ((value & (int)CardColor.Blue) == (int)CardColor.Blue)
                    cardColor = CardColor.Blue;

                // SendType
                if ((value & (int)CardSendType.Direct) == (int)CardSendType.Direct)
                    sendType = CardSendType.Direct;
                if ((value & (int)CardSendType.Secret) == (int)CardSendType.Secret)
                    sendType = CardSendType.Secret;
                if ((value & (int)CardSendType.Public) == (int)CardSendType.Public)
                    sendType = CardSendType.Public;

                // CardType
                if ((value & (int)CardType.Burn) == (int)CardType.Burn)
                    cardType = CardType.Burn;
                if ((value & (int)CardType.Gameble) == (int)CardType.Gameble)
                    cardType = CardType.Gameble;
                if ((value & (int)CardType.Guess) == (int)CardType.Guess)
                    cardType = CardType.Guess;
                if ((value & (int)CardType.Intercept) == (int)CardType.Intercept)
                    cardType = CardType.Intercept;
                if ((value & (int)CardType.Invalidate) == (int)CardType.Invalidate)
                    cardType = CardType.Invalidate;
                if ((value & (int)CardType.Lock) == (int)CardType.Lock)
                    cardType = CardType.Lock;
                if ((value & (int)CardType.Return) == (int)CardType.Return)
                    cardType = CardType.Return;
                if ((value & (int)CardType.Skip) == (int)CardType.Skip)
                    cardType = CardType.Skip;
                if ((value & (int)CardType.Test) == (int)CardType.Test)
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

        static GameObject tempCard;

        public static CardSetting IDConvertCard(int id)
        {
            if (tempCard == null)
            {
                tempCard = new GameObject("Temp Card", typeof(CardSetting));
            }

            CardSetting c = tempCard.GetComponent<CardSetting>();
            c.ID = id;
            return c;
        }

        public static CardSetting IDConvertCardAsObject(int id)
        {
            CardSetting c = new CardSetting();
            c.ID = id;
            return c;
        }

        public static explicit operator CardSetting(int id)
        {
            return IDConvertCardAsObject(id);
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

