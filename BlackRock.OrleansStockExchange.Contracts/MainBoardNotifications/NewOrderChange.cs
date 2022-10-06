using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public class NewOrderChange : IMainBoardChange
    {
        public Guid SecurityId { get; set; }

        public decimal? TransactionPrice { get; set; }

        public int? MatchedQuantity { get; set; }

        public int BidAmount { get; set; }

        public decimal BidPrice { get; set; }

        public int AskAmount { get; set; }

        public decimal AskPrice { get; set; }

        public int LeavesQuantity { get; set; }
    }
}
