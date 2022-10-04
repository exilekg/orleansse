using BlackRock.OrleansStockExchange.Contracts;
using BlackRock.OrleansStockExchange.Grains;
using BlackRock.OrleansStockExchange.WebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Orleans;
using Orleans.Hosting;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add Orleans
builder.Host.UseOrleansCluster(builder.Environment, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddSignalR();
}
else
{
    builder.Services
        .AddSignalR()
        .AddAzureSignalR(builder.Configuration["SignalR:ConnectionString"]);
}


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
