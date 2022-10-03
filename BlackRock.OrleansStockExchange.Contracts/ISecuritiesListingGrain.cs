using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public interface ISecuritiesListingGrain : IGrainWithGuidKey
    {
        Task<IEnumerable<Security>> GetAll();

        Task<Security?> Get(Guid id);
    }
}
