using BlackRock.OrleansStockExchange.Contracts;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    [ImplicitStreamSubscription(nameof(Transaction))]
    public class TransactionPublisherGrain : Grain, ITransactionPublisher, IAsyncObserver<Transaction>
    {
        private readonly IHubContext<NotificationsHub> hub;

        public TransactionPublisherGrain(IHubContext<NotificationsHub> hub)
        {
            this.hub = hub;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var streamProvider = GetStreamProvider(StorageConstants.TransactionsStreamName);
            var stream = streamProvider.GetStream<Transaction>(this.GetPrimaryKey(), nameof(Transaction));
            await stream.SubscribeAsync(this);
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

        public Task OnNextAsync(Transaction item, StreamSequenceToken? token = null)
            => this.hub.Clients.All.SendAsync("test", item);
    }
}
