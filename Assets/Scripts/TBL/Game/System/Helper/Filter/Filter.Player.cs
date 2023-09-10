using System;
using System.Linq;
using Property = TBL.Game.CardEnum.Property;

namespace TBL.Game.Sys.Helper
{
    public static partial class Filter
    {
        public static bool Is(this Player p, Func<Player, bool> func) =>
            func(p);

        #region TABLE_HAS
        public static bool TableHas(this Player p, Property property) =>
            Is(p, x => x
                .CardStatus
                .Table
                .Any(c => c.AsProperty().Contains(property)));
        public static bool TableHasBlack(this Player p) =>
            TableHas(p, Property.Black);
        public static bool TableHasBlue(this Player p) =>
            TableHas(p, Property.Blue);
        public static bool TableHasRed(this Player p) =>
            TableHas(p, Property.Red);
        #endregion
    }
}
