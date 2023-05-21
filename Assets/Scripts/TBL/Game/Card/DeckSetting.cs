using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game
{
    [CreateAssetMenu(fileName = "Deck Setting", menuName = "TBL/Game/DeckSetting", order = 0)]
    public class DeckSetting : ScriptableObject
    {
        [System.Serializable]
        public class CardSet
        {
            [HideInInspector] public string name;

            [HideInInspector] public CardEnum.Property property;
            public CardEnum.Color color;
            public CardEnum.Function function;
            public CardEnum.Type type;

            public int count;

            public void Update()
            {
                property = (CardEnum.Property)((int)color | (int)function | (int)type);
                name = property.ToDescription();
            }
        }
        public List<CardSet> CardSets = new List<CardSet>();

        [ContextMenu("Init Defaults")]
        void InitDefaults()
        {
            CardSets.Clear();

            foreach (var c in System.Enum.GetValues(typeof(CardEnum.Color)))
            {
                foreach (var f in System.Enum.GetValues(typeof(CardEnum.Function)))
                {
                    var set = new CardSet
                    {
                        color = (CardEnum.Color)c,
                        function = (CardEnum.Function)f,
                        type = CardEnum.Type.Direct
                    };
                    set.Update();
                    CardSets.Add(set);
                }
            }
        }

        void OnValidate()
        {
            foreach (var c in CardSets)
                c.Update();
        }
    }
}