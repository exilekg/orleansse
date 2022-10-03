using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public class MarketDepthRow
    {
        public int Index { get; set; }

        public decimal? BidPrice { get; set; }

        public int? BidAmount { get; set; }

        public decimal? AskPrice { get; set; }

        public int? AskAmount { get; set; }
    }
}
