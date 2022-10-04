using BlackRock.OrleansStockExchange.Grains;
using Orleans;
using Orleans.Hosting;

namespace BlackRock.OrleansStockExchange.WebAPI
{
    public static class Extensions
    {
        public static IHostBuilder UseOrleansCluster(this IHostBuilder builder, IWebHostEnvironment environment, IConfiguration config)
            => builder.UseOrleans(orleansBuilder =>
            {
                if (environment.IsDevelopment())
                {
                    orleansBuilder.UseDevelopmentCluster();
                }
                else
                {
                    orleansBuilder.UseAzureCluster(config);
                }
                orleansBuilder.UseDashboard();
            });

        private static void UseAzureCluster(this ISiloBuilder orleansBuilder, IConfiguration config)
        {
            orleansBuilder
                .UseKubernetesHosting()
                .UseRedisClustering(options => options.ConnectionString = "redis:6379")
                .AddRedisGrainStorage("definitions", options => options.ConnectionString = "redis:6379")
                .AddAzureTableGrainStorage(
                    "PubSubStore",
                    options => options.ConfigureTableServiceClient(config["StorageAccount:ConnectionString"]))
                .AddEventHubStreams(StorageConstants.EventStreamName, configurator =>
                {
                    configurator.ConfigureEventHub(eventHub => eventHub.Configure(options =>
                    {
                        options.ConfigureEventHubConnection(
                            config["EventHub:ConnectionString"],
                            "streamhub",
                            "$Default");
                    }));

                    configurator.UseAzureTableCheckpointer(
                        tableBulder => tableBulder.Configure(options =>
                        {
                            options.ConfigureTableServiceClient(config["StorageAccount:ConnectionString"]);
                            options.PersistInterval = TimeSpan.FromSeconds(10);
                        }));
                })
                .AddAzureTableGrainStorage(
                    StorageConstants.StorageName,

                    options => options.ConfigureTableServiceClient(config["StorageAccount:ConnectionString"]));
        }

        private static void UseDevelopmentCluster(this ISiloBuilder orleansBuilder)
            => orleansBuilder
                .UseLocalhostClustering()
                .AddSimpleMessageStreamProvider(StorageConstants.EventStreamName, o => o.FireAndForgetDelivery = true)
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryGrainStorage(StorageConstants.StorageName);
    }
}
