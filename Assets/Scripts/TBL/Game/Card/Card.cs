using System;
using System.ComponentModel;

namespace TBL.Game
{
    public static partial class Card
    {
        /// <summary>
        /// 卡片單元，不可使用。
        /// </summary>
        public const int None = 1 << 0;

        /// <summary>
        /// 卡片顏色。
        /// </summary>
        [Flags]
        public enum Color
        {
            [Description("藍")]
            Blue = 1 << 1,
            [Description("紅")]
            Red = 1 << 2,
            [Description("黑")]
            Black = 1 << 3,
        }

        /// <summary>
        /// 卡片功能。
        /// </summary>
        [Flags]
        public enum Function
        {
            [Description("鎖定")]
            Lock = 1 << 4,
            [Description("調虎離山")]
            Skip = 1 << 5,
            [Description("退回")]
            Return = 1 << 6,
            [Description("截獲")]
            Intercept = 1 << 7,
            [Description("破譯")]
            Guess = 1 << 8,
            [Description("燒毀")]
            Burn = 1 << 9,
            [Description("識破")]
            Invalidate = 1 << 10,
            [Description("真偽莫辨")]
            Gameble = 1 << 11,
            [Description("試探")]
            Test = 1 << 12,
        }

        /// <summary>
        /// 卡片傳送方式。
        /// </summary>
        [Flags]
        public enum Type
        {
            [Description("直達")]
            Direct = 1 << 13,
            [Description("密電")]
            Secret = 1 << 14,
            [Description("文本")]
            Public = 1 << 15
        }
    }
}
