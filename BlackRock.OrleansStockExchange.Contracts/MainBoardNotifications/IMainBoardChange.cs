using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public interface IMainBoardChange
    {
        public Guid SecurityId { get; set; }
    }
}
