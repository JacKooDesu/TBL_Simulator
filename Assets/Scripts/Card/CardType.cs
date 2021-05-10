namespace TBL.Card
{
    public enum CardType : ushort
    {
        Lock = 1 << 3,   //鎖定
        Skip = 1 << 4,       //調虎離山
        Return = 1 << 5,     //退回
        Intercept = 1 << 6,  //截獲
        Guess = 1 << 7,      //破譯
        Burn = 1 << 8,       //燒毀
        Invalidate = 1 << 9, //識破
        Gameble = 1 << 10,    //真偽莫辨
        Test = 1 << 11       //試探
    }
}
