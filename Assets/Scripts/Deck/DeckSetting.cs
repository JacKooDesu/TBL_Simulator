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

            public List<NetworkJudgement.Phase> canUsePhases = new List<NetworkJudgement.Phase>();

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

        public List<TBL.NetworkJudgement.Phase> GetCardPhaseSetting(int id)
        {
            List<TBL.NetworkJudgement.Phase> setting = new List<NetworkJudgement.Phase>();
            CardSetting cs = CardSetting.IDConvertCard(id);
            foreach (CardConfig config in cardConfigs)
            {
                if (config.Card.CardType == cs.CardType)
                {
                    setting.AddRange(config.canUsePhases);
                }
            }

            return setting;
        }
    }
}
