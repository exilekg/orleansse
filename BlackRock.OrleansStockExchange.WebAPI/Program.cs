using Azure.Identity;
using BlackRock.OrleansStockExchange.Grains;
using BlackRock.OrleansStockExchange.WebAPI;

var builder = WebApplication.CreateBuilder(args);

//if (!builder.Environment.IsDevelopment())
//{
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}

// Add Orleans
builder.Host.UseOrleansCluster(builder.Environment, builder.Configuration);

builder.Services.AddServices(builder.Environment, builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.MapControllers();
app.MapHub<NotificationsHub>("/notificationsHub");

app.Run();
