﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Contracts
{
    public interface IOrderMatchingGrain : IGrainWithGuidKey
    {
        Task<bool> AddNewOrder(Order order);

        Task<IEnumerable<MarketDepthRow>> GetMarketDepth();
    }
}
