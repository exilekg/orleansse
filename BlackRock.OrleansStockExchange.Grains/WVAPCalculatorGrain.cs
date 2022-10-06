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
    internal class WVAPCalculatorGrain : Grain, IWVAPCalculatorGrain, IRemindable, IAsyncObserver<IMainBoardChange>
    {
        private const string ReminderName = "VWAPCalculationReminder";
        private const int CalculationPeriod = 60;
        private readonly IPersistentState<WVAPCalculatorGrainState> state;

        public WVAPCalculatorGrain(
            [PersistentState("VWAPStorage", StorageConstants.StorageName)] IPersistentState<WVAPCalculatorGrainState> state)
        {
            this.state = state;
        }


        public async override Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            if (this.state.State.TickPrice <= 0)
            {
                await this.RehidrateState();
            }

            await this.SubscribeToStream();

            await this.RegistarReminder();
        }

        private async Task RegistarReminder()
        {
            await RegisterOrUpdateReminder(
                ReminderName,
                TimeSpan.FromSeconds(CalculationPeriod),
                TimeSpan.FromSeconds(CalculationPeriod));
        }

        private async Task SubscribeToStream()
        {
            var streamProvider = GetStreamProvider(StorageConstants.EventStreamName);
            var stream = streamProvider.GetStream<IMainBoardChange>(this.GetPrimaryKey(), StorageConstants.MainBoardStreamNamespace);
            await stream.SubscribeAsync(this);
        }

        private async Task RehidrateState()
        {
            var listingGrain = this.GrainFactory.GetGrain<ISecuritiesListingGrain>(Guid.Empty);
            var security = await listingGrain.Get(this.GetPrimaryKey());
            if (security is not null)
            {
                this.state.State.TotalVolume = 0;
                this.state.State.TotalWeightedVolume = 0;
                this.state.State.VWAP = 0;
                this.state.State.TickPrice = security.ClosingPrice;
                this.ResetTickValues();
                await this.state.WriteStateAsync();
            }
        }

        private void ResetTickValues()
        {
            this.state.State.TickClose = this.state.State.TickPrice;
            this.state.State.TickHigh = this.state.State.TickPrice;
            this.state.State.TickLow = this.state.State.TickPrice;
            this.state.State.TickVolume = 0;
        }

        public Task<decimal> GetVWAP()
            => Task.FromResult(this.state.State.VWAP);

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            WVAPCalculatorGrainState state = this.state.State;
            if (state.TotalVolume == 0 || state.TickVolume == 0)
                return;

            decimal averagePrice = (state.TickHigh + state.TickLow + state.TickClose) / 3M;
            decimal tickeVeightedVolume = averagePrice * state.TickVolume;
            this.state.State.TotalWeightedVolume += tickeVeightedVolume;
            this.state.State.VWAP = Math.Round(state.TotalWeightedVolume / state.TotalVolume, 2, MidpointRounding.AwayFromZero);
            this.ResetTickValues();
            await this.state.WriteStateAsync();

            await this.PublishChange();
        }

        private Task PublishChange()
        {
            IStreamProvider streamProvider = this.GetStreamProvider(StorageConstants.EventStreamName);
            var stream = streamProvider.GetStream<IMainBoardChange>(this.GetPrimaryKey(), StorageConstants.MainBoardStreamNamespace);
            return stream.OnNextAsync(new VWAPChange
            {
                VWAP = this.state.State.VWAP,
            });
        }

        public async Task OnNextAsync(IMainBoardChange change, StreamSequenceToken token = null)
        {
            if (change is NewOrderChange newOrderChange)
            {
                await this.Update(newOrderChange);
            }
        }

        public async Task Update(NewOrderChange change)
        {
            decimal price = change.TransactionPrice ?? 0;
            int quantity = change.MatchedQuantity ?? 0;

            if (price <= 0 || quantity <= 0)
                return;

            this.state.State.TickPrice = price;
            this.state.State.TickHigh = Math.Max(price, this.state.State.TickHigh);
            this.state.State.TickLow = Math.Min(price, this.state.State.TickLow);
            this.state.State.TickVolume += quantity;
            this.state.State.TotalVolume += quantity;

            await this.state.WriteStateAsync();
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;
    }
}
