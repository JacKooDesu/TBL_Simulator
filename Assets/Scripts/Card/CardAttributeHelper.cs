namespace TBL.Card
{
    public static class CardAttributeHelper
    {
        // Types
        public static int Lock => ((int)CardType.Lock);
        public static int Skip => ((int)CardType.Skip);
        public static int Return => ((int)CardType.Return);
        public static int Intercept => ((int)CardType.Intercept);
        public static int Guess => ((int)CardType.Guess);
        public static int Burn => ((int)CardType.Burn);
        public static int Invalidate => ((int)CardType.Invalidate);
        public static int Gameble => ((int)CardType.Gameble);
        public static int Test => ((int)CardType.Test);
        // Send Types
        public static int Direct => ((int)CardSendType.Direct);
        public static int Secret => ((int)CardSendType.Secret);
        public static int Public => ((int)CardSendType.Public);

        // Colors
        public static int Black => ((int)CardColor.Black);
        public static int Red => ((int)CardColor.Red);
        public static int Blue => ((int)CardColor.Blue);

        public static bool Compare(this CardSetting target, params int[] requests)
        {
            return (Compare(target.ID));
        }

        public static bool Compare(int id, params int[] requests)
        {
            if (requests.Length == 0)
                return true;
            
            foreach (var r in requests)
            {
                if ((id & r) != r)
                    return false;
            }
            return true;
        }
    }
}
