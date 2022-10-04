using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public class MainBoardItem
    {
        public Guid SecurityId { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public int Volume { get; set; }

        public int BidAmount { get; set; }

        public decimal BidPrice { get; set; }

        public int AskAmount { get; set; }

        public decimal AskPrice { get; set; }
    }
}
