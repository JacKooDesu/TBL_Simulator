using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.Card;

namespace TBL
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] DeckSetting deck;

        List<CardSetting> cards = new List<CardSetting>();

        private void OnEnable()
        {
            int counter = 0;
            for (int i = 0; i < deck.CardConfigs.Count; ++i)
            {
                for (int j = 0; j < deck.CardConfigs[i].ColorConfigs.Count; ++j)
                {
                    for (int k = 0; k < deck.CardConfigs[i].ColorConfigs[j].Amount; ++k)
                    {
                        CardSetting temp = deck.CardConfigs[i].Card;
                        Debug.Log((temp.ID & (ushort)CardSendType.Direct) == (ushort)CardSendType.Direct);
                        CardSetting card = new CardSetting();
                        card.ID = temp.ID;
                        card.CardColor = deck.CardConfigs[i].ColorConfigs[j].Color;
                        GameObject g = new GameObject(card.GetCardNameFully() + counter.ToString());
                        counter++;
                    }

                }
            }
        }
    }
}

