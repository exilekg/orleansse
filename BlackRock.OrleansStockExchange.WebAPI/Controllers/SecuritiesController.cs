using BlackRock.OrleansStockExchange.Contracts;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace BlackRock.OrleansStockExchange.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecuritiesController : Controller
    {
        private readonly IGrainFactory grainFactory;

        public SecuritiesController(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        [HttpGet]
        public Task<IEnumerable<Security>> Get()
            => this.grainFactory
                    .GetGrain<ISecuritiesListingGrain>(Guid.Empty)
                        .GetAll();

        [HttpGet("{id}/depth")]
        public Task<IEnumerable<MarketDepthLevel>> GetDepth(string id)
        {
            var orderMathcer = this.grainFactory.GetGrain<IOrderMatchingGrain>(Guid.Parse(id));
            return orderMathcer.GetMarketDepth();
        }

        [HttpPost("{id}/orders")]
        public Task<bool> NewOrder(Guid id, [FromBody]Order order)
        {
            var orderMathcer = this.grainFactory.GetGrain<IOrderMatchingGrain>(id);
            return orderMathcer.AddNewOrder(order);
        }
    }
}
