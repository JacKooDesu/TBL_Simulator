namespace TBL.Card
{
    public enum CardSendType : int
    {
        Direct = 1 << 12, // 直達
        Secret = 1 << 13,     // 密電
        Public = 1 << 14      // 文本
    }
}