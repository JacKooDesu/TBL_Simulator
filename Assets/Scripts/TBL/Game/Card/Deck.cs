using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace TBL.Game
{
    using Setting;
    using Utils;
    [System.Serializable]
    public class Deck
    //  : Sys.IResource<DeckSetting, Deck>
    {
#if UNITY_EDITOR
        /// <summary>
        /// 僅供 Inspector 檢視，後續可由 Property Drawer 改善。
        /// </summary>
        [SerializeField] CardData[] cardDatas;
#endif
        public ReadOnlyCollection<CardData> CardDatas;
        public CardData this[int index] { get => CardDatas[index]; }

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
                {
                    var unique = i << (CardEnum.UNIQUE_BIT);
                    list.Add(new CardData((int)set.property | unique));
                }
            }

#if UNITY_EDITOR
            cardDatas = list.ToArray();
#endif
            CardDatas = list.AsReadOnly();

            return this;
        }

        public CardCollection Draw(int count)
        {
            CardCollection result = new();
            for (int i = 0; i < count; ++i)
                result.Add(sleeping.MoveTo(0, hand));

            return result;
        }
    }
}