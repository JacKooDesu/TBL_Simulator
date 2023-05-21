using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace TBL.Game
{
    public static class CardEnum
    {
        /// <summary>
        /// 卡片單元，不可使用。
        /// </summary>
        public const int None = 1 << 0;

        /// <summary>
        /// 用於存放卡片所有屬性，多使用於ID轉換。
        /// <para>注意：由 Property 強轉換至 Color、Function、Type 是不被允許的。</para>
        /// </summary>
        [Flags]
        public enum Property
        {
            #region Color
            [Description("藍")]
            Blue = 1 << 1,
            [Description("紅")]
            Red = 1 << 2,
            [Description("黑")]
            Black = 1 << 3,
            #endregion

            #region Functions
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
            #endregion

            #region Type
            [Description("直達")]
            Direct = 1 << 13,
            [Description("密電")]
            Secret = 1 << 14,
            [Description("文本")]
            Public = 1 << 15
            #endregion
        }

        /// <summary>
        /// 卡片顏色。
        /// </summary>
        [Flags]
        public enum Color
        {
            [Description("藍")]
            Blue = Property.Blue,
            [Description("紅")]
            Red = Property.Red,
            [Description("黑")]
            Black = Property.Black,
        }

        /// <summary>
        /// 卡片名稱。
        /// </summary>
        public enum Function
        {
            [Description("鎖定")]
            Lock = Property.Lock,
            [Description("調虎離山")]
            Skip = Property.Skip,
            [Description("退回")]
            Return = Property.Return,
            [Description("截獲")]
            Intercept = Property.Intercept,
            [Description("破譯")]
            Guess = Property.Guess,
            [Description("燒毀")]
            Burn = Property.Burn,
            [Description("識破")]
            Invalidate = Property.Invalidate,
            [Description("真偽莫辨")]
            Gameble = Property.Gameble,
            [Description("試探")]
            Test = Property.Test,
        }

        /// <summary>
        /// 卡片傳送方式。
        /// </summary>
        public enum Type
        {
            [Description("直達")]
            Direct = Property.Direct,
            [Description("密電")]
            Secret = Property.Secret,
            [Description("文本")]
            Public = Property.Public
        }

        /// <summary>
        /// 提供基本中文轉換於 Editor 內使用( DeckSetting、Deck 等 )，可能有效能問題所以不使用於遊戲內。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>    
        public static string ToDescription(this Property value)
        {
            var result = "";

            var values = value.ToString().Split(',');
            int iter = values.Length - 1;
            foreach (var v in values)
            {
                var field = typeof(Property).GetField(v.Trim());
                if (field == null) continue;

                DescriptionAttribute attr =
                    Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) as DescriptionAttribute;
                result +=
                    attr.Description + (iter <= 0 ? "" : " , ");
                iter--;
            }

            return result;
        }

        public static string ToDescription(this Enum value) =>
            value.GetType()
            .GetRuntimeField(value.ToString())
            .GetCustomAttributes<System.ComponentModel.DescriptionAttribute>()
            .FirstOrDefault()?.Description ?? string.Empty;
    }
}
