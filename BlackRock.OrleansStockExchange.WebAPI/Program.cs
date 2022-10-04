using BlackRock.OrleansStockExchange.Contracts;
using BlackRock.OrleansStockExchange.Grains;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Orleans;
using Orleans.Hosting;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add Orleans
builder.Host.UseOrleans(orleansBuilder =>
{
    orleansBuilder
        //.UseLocalhostClustering()
        //.AddSimpleMessageStreamProvider(StorageConstants.TransactionsStreamName, o => o.FireAndForgetDelivery = true)
        //.AddMemoryGrainStorage("PubSubStore")
        .UseKubernetesHosting()
        .AddAzureTableGrainStorage(
            "PubSubStore",
            options => options.ConfigureTableServiceClient(builder.Configuration["StorageAccount:ConnectionString"]))
        .AddEventHubStreams(StorageConstants.MainBoardStreamName, (ISiloEventHubStreamConfigurator configurator) =>
        {
            configurator.ConfigureEventHub(eventHub => eventHub.Configure(options =>
            {
                options.ConfigureEventHubConnection(
                    builder.Configuration["EventHub:ConnectionString"],
                    "streamhub",
                    "$Default");
            }));
            configurator.UseAzureTableCheckpointer(
                tableBulder => tableBulder.Configure(options =>
                {
                    options.ConfigureTableServiceClient(builder.Configuration["StorageAccount:ConnectionString"]);
                    options.PersistInterval = TimeSpan.FromSeconds(10);
                }));
        })
        .AddAzureTableGrainStorage(
            StorageConstants.BlobStorageName,

            options => options.ConfigureTableServiceClient(builder.Configuration["StorageAccount:ConnectionString"]));

    orleansBuilder.UseRedisClustering(options => options.ConnectionString = "redis:6379");
    orleansBuilder.AddRedisGrainStorage("definitions", options => options.ConnectionString = "redis:6379");

    orleansBuilder.UseDashboard();
});

//.AddAzureBlobGrainStorage(StorageConstants.BlobStorageName, blob =>
//{
//    blob.ConfigureBlobServiceClient(builder.Configuration["StorageAccount:ConnectionString"]);
//    blob.UseJson = true;
//}));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddSignalR()
    .AddAzureSignalR(builder.Configuration["SignalR:ConnectionString"]);

builder.Services.AddCors(
    options => options.AddDefaultPolicy(
        policy => policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed((host) => true)
              .AllowCredentials()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.MapControllers();
app.MapHub<NotificationsHub>("/notificationsHub");

app.Run();
