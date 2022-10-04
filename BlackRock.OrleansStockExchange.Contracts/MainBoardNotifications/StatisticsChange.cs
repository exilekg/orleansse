using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public class StatisticsChange : IMainBoardChange
    {
        public Guid SecurityId { get; set; }

        public decimal Price { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Change { get; set; }

        public decimal ChangePercent { get; set; }

        public int Volume { get; set; }
    }
}
