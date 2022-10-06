using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    internal class WVAPCalculatorGrainState
    {
        public decimal VWAP { get; set; }

        public decimal TickHigh { get; set; }

        public decimal TickLow { get; set; }

        public decimal TickPrice { get; set; }

        public decimal TickClose { get; set; }

        public decimal TickVolume { get; set; }

        public decimal TotalVolume { get; set; }

        public decimal TotalWeightedVolume { get; set; }
    }
}
