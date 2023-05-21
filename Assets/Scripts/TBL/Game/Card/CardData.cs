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

            this.property = (CardEnum.Property)((int)color | (int)function | (int)type);
        }

        public CardData(int id)
        {
            this.property = (CardEnum.Property)id;
            this.color = (CardEnum.Color)id;
            this.function = (CardEnum.Function)id;
            this.type = (CardEnum.Type)id;
        }

        int id;
        int unique;

        [SerializeField] CardEnum.Property property;

        [SerializeField] CardEnum.Color color;
        [SerializeField] CardEnum.Function function;
        [SerializeField] CardEnum.Type type;
    }
}