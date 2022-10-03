using BlackRock.OrleansStockExchange.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    public static class MatchingHelpers
    {
        private static Comparer<decimal> BidComparerer = Comparer<decimal>.Create((x, y) => y.CompareTo(x));
        private static Comparer<decimal> AskComparerer = Comparer<decimal>.Default;

        public static OrderSide GetOpositeSide(this OrderSide side)
            => side == OrderSide.Sell ? OrderSide.Buy : OrderSide.Sell;

        public static Comparer<decimal> GetComparerer(this OrderSide side)
            => side == OrderSide.Buy ? BidComparerer : AskComparerer;
    }
}
