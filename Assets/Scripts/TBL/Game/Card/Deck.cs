using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game
{
    [System.Serializable]
    public class Deck : Sys.IResource<DeckSetting, Deck>
    {
        [SerializeField] 
        CardData[] cardDatas;

        public Deck Init(DeckSetting setting)
        {
            var list = new List<CardData>();
            foreach (var set in setting.CardSets)
            {
                for (int i = 0; i < set.count; ++i)
                    list.Add(new CardData(set.property));
            }

            cardDatas = list.ToArray();
            return this;
        }
    }
}