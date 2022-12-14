using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<IEnumerable<Guid>> GetSecurities();

        Task AddSecurity(Guid id);

        Task RemoveSecurity(Guid id);

        Task<IEnumerable<MainBoardItem>> GetMainBoard();
    }
}
