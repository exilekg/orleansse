using BlackRock.OrleansStockExchange.Contracts;
using Orleans;
using Orleans.Concurrency;

namespace BlackRock.OrleansStockExchange.Grains
{
    [StatelessWorker]
    public class SecuritiesListingGrain : Grain, ISecuritiesListingGrain
    {
        private static readonly IEnumerable<Security> securities = GetData();

        private static IReadOnlyCollection<Security> GetData()
            => new[]
            {
                new Security(Guid.Parse("19c1a50d-d4e3-46f1-a561-a5c5e386ae35"), "BLK", "BLACKROCK INC", 150),
                new Security(Guid.Parse("2e389bcd-958b-4fd8-a6ad-965840759dc9"), "TSLA", "TESLA INC", 100),
                new Security(Guid.Parse("efd84928-e3cf-41ec-a7e0-d8d3e1fb585d"), "AAPL", "APPLE INC", 150),
                new Security(Guid.Parse("b9e00318-013d-4630-8f42-c0fa857e7e79"), "GOOGL", "ALPHABET INC", 150),
                new Security(Guid.Parse("32a67ea7-2357-4a24-a7f9-05f0d37932ca"), "MSFT", "MICROSOFT CORP", 150),
                new Security(Guid.Parse("e29eb79a-4e46-4816-b705-23ec1129a027"), "META", "META PLATFORMS INC", 150),
                new Security(Guid.Parse("1293943d-de8c-436c-82cd-f11b3c2a8908"), "AMZN", "AMAZON COM INC", 150),
                new Security(Guid.Parse("ba3a96ac-a5b4-447c-9c65-f5680e34daba"), "ORCL", "ORACLE CORP", 150),
            };

        public Task<Security?> Get(Guid id)
            => Task.FromResult(securities.FirstOrDefault(s => s.Id == id));

        public Task<IEnumerable<Security>> GetAll()
            => Task.FromResult(securities);
    }
}