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
            [PersistentState("order", StorageConstants.BlobStorageName)] IPersistentState<OrderMatchingState> state)
        {
            this.state = state;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
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
            order.LeavesQuantity = order.Quantity;
            var transaction = ExecuteTrades(order);

            if (order.LeavesQuantity > 0)
            {
                AddOrder(order);
            }
            await this.state.WriteStateAsync();

            if (transaction is not null)
            {
                await this.PublishTransaction(transaction);
            }
            return true;
        }

        private Task PublishTransaction(Transaction transaction)
        {
            IStreamProvider streamProvider = GetStreamProvider(StorageConstants.TransactionsStreamName);
            var stream = streamProvider.GetStream<Transaction>(this.GetPrimaryKey(), nameof(Transaction));
            return stream.OnNextAsync(transaction);
        }

        private Transaction? ExecuteTrades(Order order)
        {
            var depthChanges = this.GetTradeDepthChanges(order);
            if (depthChanges?.Any() != true)
                return null;

            Transaction transacion = new()
            {
                Price = depthChanges.First().price,
                Quantity = depthChanges.Sum(c => c.qty)
            };

            RemoveFromDepth(order, depthChanges);

            return transacion;
        }

        private void RemoveFromDepth(Order order, IEnumerable<(decimal price, int qty)> depthChanges)
        {
            OrderSide opositeSide = order.Side.GetOpositeSide();
            SortedList<decimal, int> marketDepth = this.state.State.GetMarketDepth(opositeSide);
            foreach (var change in depthChanges)
            {
                if (marketDepth[change.price] <= change.qty)
                {
                    marketDepth.Remove(change.price);
                }
                else
                {
                    marketDepth[change.price] -= change.qty;
                }
            }
        }

        private IEnumerable<(decimal price, int qty)> GetTradeDepthChanges(Order order)
        {
            OrderSide opositeSide = order.Side.GetOpositeSide();
            SortedList<decimal, int> marketDepth = this.state.State.GetMarketDepth(opositeSide);
            Comparer<decimal> comparer = order.Side.GetComparerer();

            List<(decimal, int)> depthChanges = new();

            foreach (var depthPrice in marketDepth.Keys)
            {
                if (order.LeavesQuantity <= 0 || comparer.Compare(depthPrice, order.Price) < 0)
                    break;

                var quantity = Math.Min(order.LeavesQuantity, marketDepth[depthPrice]);
                order.LeavesQuantity -= quantity;

                depthChanges.Add((depthPrice, quantity));
            }

            return depthChanges;
        }

        private void AddOrder(Order order)
        {
            SortedList<decimal, int> marketDepth = this.state.State.GetMarketDepth(order.Side);
            if (marketDepth.ContainsKey(order.Price))
            {
                marketDepth[order.Price] += order.LeavesQuantity;
            }
            else
            {
                marketDepth[order.Price] = order.LeavesQuantity;
            }
        }

        public Task<IEnumerable<MarketDepthRow>> GetMarketDepth()
            => Task.FromResult(this.GetMarketDeptRows()
                .ToList()
                .AsEnumerable());

        private IEnumerable<MarketDepthRow> GetMarketDeptRows()
        {
            SortedList<decimal, int> askMarketDepth = this.state.State.AskMarketDepth;
            SortedList<decimal, int> bidMarketDepth = this.state.State.BidMarketDepth;

            int maxDepth = Math.Max(askMarketDepth.Count, bidMarketDepth.Count);
            for (int i = 0; i < maxDepth; i++)
            {
                var askPrice = askMarketDepth.Count > i ? askMarketDepth.Keys[i] : default(decimal?);
                var bidPrice = bidMarketDepth.Count > i ? bidMarketDepth.Keys[i] : default(decimal?);
                yield return new MarketDepthRow
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
