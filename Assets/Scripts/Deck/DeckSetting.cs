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
            public class PhaseSetting
            {
                public NetworkJudgement.Phase phase;
                public bool roundHost;
                public bool sendingHost;
            }
            public List<PhaseSetting> canUsePhases = new List<PhaseSetting>();

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

        public int cardUniqueId = 15;

        public List<CardConfig.PhaseSetting> GetCardPhaseSetting(int id)
        {
            List<CardConfig.PhaseSetting> setting = new List<CardConfig.PhaseSetting>();
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

        public CardSetting GetCardPrototype(int id)
        {
            CardSetting card = CardSetting.IDConvertCard(id);
            foreach (CardConfig config in cardConfigs)
            {
                if (config.Card.CardType == card.CardType)
                {
                    return config.Card;
                }
            }

            return null;
        }

        public int GetCardUniqueID(int id)
        {
            int i = cardUniqueId;
            while ((id & 1 << i) != (1 << i) && i < 32)
            {
                ++i;
            }
            return i;
        }
    }
}
