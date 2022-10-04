using BlackRock.OrleansStockExchange.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;

namespace BlackRock.OrleansStockExchange.Grains
{
    public class MainBoardPublisherGrain : Grain, IMainBoardPublisher
    {
        private readonly IHubContext<NotificationsHub> hub;

        public MainBoardPublisherGrain(IHubContext<NotificationsHub> hub)
        {
            this.hub = hub;
        }

        public Task NewMainBoardState(MainBoardItem item)
        {
            return this.hub.Clients.All.SendAsync("mainBoardChange", item);
        }
    }
}
