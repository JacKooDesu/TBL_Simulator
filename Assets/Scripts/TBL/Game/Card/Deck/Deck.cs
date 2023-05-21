using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace TBL.Game
{
    [System.Serializable]
    public class Deck : Sys.IResource<DeckSetting, Deck>
    {
#if UNITY_EDITOR
        /// <summary>
        /// 僅供 Inspector 檢視，後續可由 Property Drawer 改善。
        /// </summary>
        [SerializeField] CardData[] cardDatas;
#endif
        public ReadOnlyCollection<CardData> CardDatas;

        public CardCollection sleeping = new CardCollection();
        public CardCollection hand = new CardCollection();
        public CardCollection table = new CardCollection();
        public CardCollection graveyard = new CardCollection();

        public Deck Init(DeckSetting setting)
        {
            var list = new List<CardData>();
            foreach (var set in setting.CardSets)
            {
                for (int i = 0; i < set.count; ++i)
                    list.Add(new CardData(set.property));
            }

#if UNITY_EDITOR
            cardDatas = list.ToArray();
#endif
            CardDatas = list.AsReadOnly();

            sleeping.AddRange(CardDatas);

            return this;
        }
    }
}