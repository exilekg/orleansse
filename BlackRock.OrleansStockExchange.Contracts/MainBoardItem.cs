namespace BlackRock.OrleansStockExchange.Contracts
{
    public class MainBoardItem
    {
        public Guid SecurityId { get; set; }

        public string Symbol { get; set; }

        public int BidAmount { get; set; }

        public decimal BidPrice { get; set; }

        public int AskAmount { get; set; }

        public decimal AskPrice { get; set; }

        public decimal Price { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Change { get; set; }

        public decimal ChangePercent { get; set; }

        public decimal VWAP { get; set; }

        public int Volume { get; set; }
    }
}
