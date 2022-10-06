using BlackRock.OrleansStockExchange.Contracts;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    [ImplicitStreamSubscription(StorageConstants.MainBoardStreamNamespace)]
    internal class MainBoardItemGrain : Grain, IMainBoardItemGrain, IAsyncObserver<IMainBoardChange>
    {
        private readonly IPersistentState<MainBoardItem> state;

        public MainBoardItemGrain(
            [PersistentState("mainBoadr", StorageConstants.StorageName)] IPersistentState<MainBoardItem> state)
        {
            this.state = state;
        }

        public Task<MainBoardItem> GetMainBoardItem()
            => Task.FromResult(this.state.State);

        public async override Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            if (string.IsNullOrEmpty(this.state.State.Symbol))
            {
                await RehidrateState();
            }

            await SubscribeToStream();
        }

        private async Task SubscribeToStream()
        {
            var streamProvider = GetStreamProvider(StorageConstants.EventStreamName);
            var stream = streamProvider.GetStream<IMainBoardChange>(this.GetPrimaryKey(), StorageConstants.MainBoardStreamNamespace);
            await stream.SubscribeAsync(this);
        }

        private async Task PublishChange()
        {
            var publisher = this.GrainFactory.GetGrain<IMainBoardPublisher>(this.GetPrimaryKey());
            await publisher.NewMainBoardState(this.state.State);
        }

        private async Task RehidrateState()
        {
            var listingGrain = this.GrainFactory.GetGrain<ISecuritiesListingGrain>(Guid.Empty);
            var security = await listingGrain.Get(this.GetPrimaryKey());
            if (security is not null)
            {
                this.state.State.SecurityId = security.Id;
                this.state.State.Symbol = security.Symbol;
                this.state.State.Open = security.ClosingPrice;
                this.state.State.High = security.ClosingPrice;
                this.state.State.Low = security.ClosingPrice;
                this.state.State.Price = security.ClosingPrice;
                await this.state.WriteStateAsync();
            }
        }

        public async Task OnNextAsync(IMainBoardChange change, StreamSequenceToken? token = null)
        {
            if (change is NewOrderChange newOrderChange)
            {
                this.Update(newOrderChange);
            }
            else if (change is VWAPChange vwapChange)
            {
                this.Update(vwapChange);
            }

            await this.state.WriteStateAsync();
            await this.PublishChange();
        }

        private void Update(VWAPChange vwapChange)
        {
            this.state.State.VWAP = vwapChange.VWAP;
        }
        private void Update(NewOrderChange newOrderChange)
        {
            MainBoardItem currentState = this.state.State;
            currentState.BidAmount = newOrderChange.BidAmount;
            currentState.AskAmount = newOrderChange.AskAmount;
            currentState.BidPrice = newOrderChange.BidPrice;
            currentState.AskPrice = newOrderChange.AskPrice;

            decimal price = newOrderChange.TransactionPrice ?? 0;
            decimal change = price - currentState.Open;
            int volume = newOrderChange.MatchedQuantity ?? 0;

            if (price <= 0 || volume <= 0)
            {
                return;
            }

            currentState.Price = price;
            currentState.High = Math.Max(price, currentState.High);
            currentState.Low = Math.Min(price, currentState.Low);
            currentState.Change = change;
            currentState.ChangePercent = Math.Round(change / currentState.Open * 100, 2, MidpointRounding.AwayFromZero);
            currentState.Volume += volume;
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;
    }
}
