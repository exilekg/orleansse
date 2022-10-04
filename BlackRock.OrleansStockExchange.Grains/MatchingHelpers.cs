using BlackRock.OrleansStockExchange.Contracts;
using BlackRock.OrleansStockExchange.Contracts.MainBoardNotifications;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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

        public static bool CanMatch(this Order order, decimal price)
        {
            Comparer<decimal> comparer = order.Side.GetOpositeSide()
                .GetComparerer();

            return comparer.Compare(price, order.Price) <= 0;
        }
    }
}
