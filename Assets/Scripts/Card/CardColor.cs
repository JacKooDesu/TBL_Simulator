namespace TBL.Card
{
    public enum CardColor : ushort
    {
        Black = 1 << 0,
        Red = 1 << 1,
        Blue = 1 << 2,
        BlackRed = Black + Red,
        BlackBlue = Black + Blue
    }
}
