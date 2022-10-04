using BlackRock.OrleansStockExchange.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    internal class OrderMatchingState
    {
        public SortedList<decimal, int> BidMarketDepth { get; set; }

        public SortedList<decimal, int> AskMarketDepth { get; set; }

        public SortedList<decimal, int>  GetMarketDepth(OrderSide side)
            => side == OrderSide.Buy ? this.BidMarketDepth : this.AskMarketDepth;

    }
}
