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
                    }

                }
            }
        }
    }
}

