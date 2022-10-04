using BlackRock.OrleansStockExchange.Contracts;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    internal class UserGrain : Grain, IUserGrain
    {

        private readonly IPersistentState<UserGrainState> state;

        public UserGrain(
            [PersistentState("user", StorageConstants.StorageName)] IPersistentState<UserGrainState> state)
        {
            this.state = state;
        }

        public Task<IEnumerable<Guid>> GetSecurities()
            => Task.FromResult(this.state.State.Securities.AsEnumerable());

        public async Task AddSecurity(Guid id)
        {
            this.state.State.Securities.Add(id);
            await this.state.WriteStateAsync();
        }
        public async Task RemoveSecurity(Guid id)
        {
            this.state.State.Securities.Remove(id);
            await this.state.WriteStateAsync();
        }

        public async Task<IEnumerable<MainBoardItem>> GetMainBoard()
        {
            var result = new List<MainBoardItem>();
            foreach (var security in this.state.State.Securities)
            {
                var mainBoardItemGrain = this.GrainFactory.GetGrain<IMainBoardItemGrain>(security);
                var item = await mainBoardItemGrain.GetMainBoardItem();
                result.Add(item);
            }

            return result;
        }
    }
}
