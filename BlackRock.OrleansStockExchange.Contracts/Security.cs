using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public class Security
    {
        public Security(string symbol, string name, decimal closingPrice, decimal upperAbsoluteLimit = 0.2m, decimal lowerAbsoluteLimit = 0.2m)
            : this(Guid.NewGuid(), symbol, name, closingPrice, upperAbsoluteLimit, lowerAbsoluteLimit) { }
        public Security(Guid id, string symbol, string name, decimal closingPrice, decimal upperAbsoluteLimit = 0.2m, decimal lowerAbsoluteLimit = 0.2m)
        {
            Id = id;
            Symbol = symbol;
            Name = name;
            ClosingPrice = closingPrice;
            UpperAbsoluteLimit = upperAbsoluteLimit;
            LowerAbsoluteLimit = lowerAbsoluteLimit;
        }

        public Guid Id { get; }

        public string Symbol { get; }

        public string Name { get; }

        public decimal ClosingPrice { get; }

        public decimal UpperAbsoluteLimit { get; }

        public decimal LowerAbsoluteLimit { get; }
    }
}
