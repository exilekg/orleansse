using BlackRock.OrleansStockExchange.Contracts;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace BlackRock.OrleansStockExchange.WebAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly IGrainFactory grainFactory;

        public UsersController(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        [HttpGet("{id}/securities")]
        public Task<IEnumerable<Guid>> GetSecurities(string id)
        {
            var orderMathcer = this.grainFactory.GetGrain<IUserGrain>(id);
            return orderMathcer.GetSecurities();
        }

        [HttpPut("{id}/securities/{securityId}")]
        public Task AddSecurity(string id, Guid securityId)
        {
            var orderMathcer = this.grainFactory.GetGrain<IUserGrain>(id);
            return orderMathcer.AddSecurity(securityId);
        }

        [HttpDelete("{id}/securities/{securityId}")]
        public Task RemoveSecurity(string id, Guid securityId)
        {
            var orderMathcer = this.grainFactory.GetGrain<IUserGrain>(id);
            return orderMathcer.RemoveSecurity(securityId);
        }

        [HttpGet("{id}/mainboard")]
        public Task<IEnumerable<MainBoardItem>> GetMainBoard(string id)
        {
            var orderMathcer = this.grainFactory.GetGrain<IUserGrain>(id);
            return orderMathcer.GetMainBoard();
        }
    }
}
