using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public interface IMainBoardPublisher : IGrainWithGuidKey
    {
        [OneWay]
        Task NewMainBoardState(MainBoardItem item);
    }
}
