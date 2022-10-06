using BlackRock.OrleansStockExchange.Contracts;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace BlackRock.OrleansStockExchange.Grains
{
    internal class OrderMatchingGrain : Grain, IOrderMatchingGrain
    {
        private readonly IPersistentState<OrderMatchingState> state;

        public OrderMatchingGrain(
            [PersistentState("order", StorageConstants.StorageName)] IPersistentState<OrderMatchingState> state)
        {
            this.state = state;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            this.InitState();
        }

        private void InitState()
        {
            if (this.state.State.AskMarketDepth == default)
            {
                this.state.State.AskMarketDepth = new SortedList<decimal, int>(OrderSide.Sell.GetComparerer());
            }

            if (this.state.State.BidMarketDepth == default)
            {
                this.state.State.BidMarketDepth = new SortedList<decimal, int>(OrderSide.Buy.GetComparerer());
            }
            else
            {
                this.state.State.BidMarketDepth = new SortedList<decimal, int>(
                    this.state.State.BidMarketDepth,
                    OrderSide.Buy.GetComparerer());
            }
        }

        public async Task<bool> AddNewOrder(Order order)
        {
            NewOrderChange change = ChangeState(order);
            await this.state.WriteStateAsync();

            change.BidAmount = this.state.State.BidMarketDepth.Values.Sum();
            change.BidPrice = this.state.State.BidMarketDepth.Keys.FirstOrDefault();
            change.AskAmount = this.state.State.AskMarketDepth.Values.Sum();
            change.AskPrice = this.state.State.AskMarketDepth.Keys.FirstOrDefault();

            await this.PublishChange(change);
            return true;
        }

        private Task PublishChange(NewOrderChange change)
        {
            IStreamProvider streamProvider = this.GetStreamProvider(StorageConstants.EventStreamName);
            var stream = streamProvider.GetStream<IMainBoardChange>(this.GetPrimaryKey(), StorageConstants.MainBoardStreamNamespace);
            return stream.OnNextAsync(change);
        }

        private NewOrderChange ChangeState(Order order)
        {
            var change = this.ExecuteTrade(order);
            this.AddToDepth(order, change.LeavesQuantity);
            return change;
        }

        private NewOrderChange ExecuteTrade(Order order)
        {
            NewOrderChange change = new()
            {
                SecurityId = this.GetPrimaryKey(),
                LeavesQuantity = order.Quantity,
            };

            OrderSide opositeSide = order.Side.GetOpositeSide();
            SortedList<decimal, int> marketDepth = this.state.State.GetMarketDepth(opositeSide);

            while (marketDepth.Any()
                && change.LeavesQuantity > 0
                && order.CanMatch(marketDepth.First().Key))
            {
                decimal depthPrice = marketDepth.First().Key;
                change.TransactionPrice ??= depthPrice;
                change.MatchedQuantity ??= 0;

                var quantity = Math.Min(change.LeavesQuantity, marketDepth[depthPrice]);
                change.LeavesQuantity -= quantity;
                change.MatchedQuantity += quantity;
                marketDepth[depthPrice] -= quantity;

                if (marketDepth[depthPrice] <= 0)
                {
                    marketDepth.Remove(depthPrice);
                }
            }

            return change;
        }

        private void AddToDepth(Order order, int leavesQuantity)
        {
            if (leavesQuantity <= 0)
                return;

            SortedList<decimal, int> marketDepth = this.state.State.GetMarketDepth(order.Side);
            if (marketDepth.ContainsKey(order.Price))
            {
                marketDepth[order.Price] += leavesQuantity;
            }
            else
            {
                marketDepth[order.Price] = leavesQuantity;
            }
        }

        public Task<IEnumerable<MarketDepthLevel>> GetMarketDepth()
            => Task.FromResult(this.GetMarketDeptRows()
                .ToList()
                .AsEnumerable());

        private IEnumerable<MarketDepthLevel> GetMarketDeptRows()
        {
            SortedList<decimal, int> askMarketDepth = this.state.State.AskMarketDepth;
            SortedList<decimal, int> bidMarketDepth = this.state.State.BidMarketDepth;

            int maxDepth = Math.Max(askMarketDepth.Count, bidMarketDepth.Count);
            for (int i = 0; i < maxDepth; i++)
            {
                var askPrice = askMarketDepth.Count > i ? askMarketDepth.Keys[i] : default(decimal?);
                var bidPrice = bidMarketDepth.Count > i ? bidMarketDepth.Keys[i] : default(decimal?);
                yield return new MarketDepthLevel
                {
                    Index = i,
                    AskPrice = askPrice,
                    BidPrice = bidPrice,
                    AskAmount = askPrice.HasValue ? askMarketDepth[askPrice.Value] : default(int?),
                    BidAmount = bidPrice.HasValue ? bidMarketDepth[bidPrice.Value] : default(int?),
                };
            }
        }
    }
}
