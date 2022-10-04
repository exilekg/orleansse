using BlackRock.OrleansStockExchange.Contracts;
using BlackRock.OrleansStockExchange.Contracts.MainBoardNotifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    [ImplicitStreamSubscription(StorageConstants.MainBoardStreamNamespace)]
    public class MainBoardPublisherGrain : Grain, IMainBoardPublisher, IAsyncObserver<IMainBoardChange>
    {
        private readonly IHubContext<NotificationsHub> hub;
        private readonly ILogger<MainBoardPublisherGrain> logger;

        public MainBoardPublisherGrain(IHubContext<NotificationsHub> hub, ILogger<MainBoardPublisherGrain> logger)
        {
            this.hub = hub;
            this.logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = GetStreamProvider(StorageConstants.MainBoardStreamName);
            var stream = streamProvider.GetStream<IMainBoardChange>(this.GetPrimaryKey(), StorageConstants.MainBoardStreamNamespace);
            await stream.SubscribeAsync(this);
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

        public async Task OnNextAsync(IMainBoardChange item, StreamSequenceToken? token = null)
        {
            this.logger.LogInformation("publishing to all clients");
            this.hub.Clients.All.SendAsync("mainBoardChange", item);
        }
    }
}
