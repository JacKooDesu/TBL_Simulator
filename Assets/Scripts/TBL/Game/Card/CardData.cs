using UnityEngine;

namespace TBL.Game
{
    /// <summary>
    /// <para>遊戲中所有卡片可以用CardData 來表示。</para>
    /// <para>基本上僅存在伺服端，客戶端僅接收ID並透過 Card 類別。</para>
    /// </summary>
    [System.Serializable]
    public class CardData
    {
        // public CardData(CardData data) => this = data;

        public CardData(CardEnum.Color color, CardEnum.Function function, CardEnum.Type type)
        {
            this.color = color;
            this.function = function;
            this.type = type;

            this.property = ((int)color | (int)function | (int)type).AsProperty();
        }

        public CardData(CardEnum.Property property)
        {
            this.property = property;

            this.color = property.ConvertColor();
            this.function = property.ConvertFunction();
            this.type = property.ConvertType();
        }

        public CardData(int id)
        {
            this.property = id.AsProperty();

            this.color = property.ConvertColor();
            this.function = property.ConvertFunction();
            this.type = property.ConvertType();

            this.id = id;
        }

        [SerializeField] int id;
        public int Id => id;
        public int Unique => Id >> CardEnum.UNIQUE_BIT;

        [SerializeField] CardEnum.Property property;
        public CardEnum.Property Property => property;

        [SerializeField] CardEnum.Color color;
        public CardEnum.Color Color => color;
        [SerializeField] CardEnum.Function function;
        public CardEnum.Function Function => function;
        [SerializeField] CardEnum.Type type;
        public CardEnum.Type Type => type;
    }
}