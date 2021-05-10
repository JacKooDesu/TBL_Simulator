using UnityEngine;
using TBL.Card;
using System.Collections.Generic;

namespace TBL
{
    [CreateAssetMenu(fileName = "DeckSetting", menuName = "TBL/Create DeckSetting", order = 0)]
    public class DeckSetting : ScriptableObject
    {
        [System.Serializable]
        public class CardConfig
        {
            [SerializeField] CardSetting card;
            public CardSetting Card
            {
                get => card;
            }

            [System.Serializable]
            public class ColorConfig
            {
                [SerializeField] CardColor color;
                public CardColor Color
                {
                    get => color;
                }
                [SerializeField] int amount = 0;
                public int Amount
                {
                    get => amount;
                }
            }
            [SerializeField] List<ColorConfig> colorConfigs = new List<ColorConfig>();

            public List<ColorConfig> ColorConfigs
            {
                get => colorConfigs;
            }
        }

        [SerializeField] List<CardConfig> cardConfigs = new List<CardConfig>();
        public List<CardConfig> CardConfigs
        {
            get => cardConfigs;
        }
    }
}
