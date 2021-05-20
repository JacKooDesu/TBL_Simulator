using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.Card;
using Mirror;

namespace TBL
{
    public class DeckManager : NetworkBehaviour
    {
        [SerializeField] DeckSetting deck;

        [SerializeField] List<CardSetting> cards = new List<CardSetting>();
        [SerializeField] List<CardSetting> inGameCards = new List<CardSetting>();
        [SerializeField] List<CardSetting> garbageCards = new List<CardSetting>();

        private void OnEnable()
        {
            for (int i = 0; i < deck.CardConfigs.Count; ++i)
            {
                for (int j = 0; j < deck.CardConfigs[i].ColorConfigs.Count; ++j)
                {
                    for (int k = 0; k < deck.CardConfigs[i].ColorConfigs[j].Amount; ++k)
                    {
                        CardSetting temp = deck.CardConfigs[i].Card;
                        Debug.Log((temp.ID & (ushort)CardSendType.Direct) == (ushort)CardSendType.Direct);
                        GameObject g = new GameObject();
                        CardSetting card = g.AddComponent<CardSetting>();
                        card.ID = (ushort)(temp.ID + (ushort)(deck.CardConfigs[i].ColorConfigs[j].Color));
                        g.name = card.GetCardNameFully();

                        cards.Add(card);
                    }

                }
            }

            Shuffle();
        }

        void Shuffle()
        {
            int cardCount = cards.Count;

            for (int i = cardCount - 1; i >= 0; --i)
            {
                int rand = UnityEngine.Random.Range(0, i);
                CardSetting tmpCard = cards[i];
                cards[i] = cards[rand];
                cards[rand] = tmpCard;
            }
        }

        public CardSetting DrawCardFromTop()
        {
            if (cards.Count == 0)
            {
                cards.AddRange(garbageCards);
                garbageCards.Clear();
                Shuffle();
            }

            CardSetting c = cards[0];
            inGameCards.Add(c);
            cards.RemoveAt(0);
            return c;
        }

        public void CardToGarbage(ulong id)
        {
            foreach (CardSetting c in inGameCards)
            {
                if (c.ID == id)
                {
                    garbageCards.Add(c);
                    inGameCards.Remove(c);
                }
            }
        }

    }
}

