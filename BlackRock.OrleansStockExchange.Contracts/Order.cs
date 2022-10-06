using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public class Order
    {
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public OrderSide Side { get; set; }
    }
}
